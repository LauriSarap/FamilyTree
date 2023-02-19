using CommunityToolkit.Mvvm.Input;
using FamilyTree.Logic;
using FamilyTree.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FamilyTree.MVVM;

public partial class PeopleEditorPage : ContentPage
{
    public ObservableCollection<Person> PersonCollection { get; set; } = new();
    public ObservableCollection<Person> SpouseSelectionCollection { get; set; } = new();
    public ObservableCollection<Person> ChildrenSelectionCollection { get; set; } = new();
    public IAsyncRelayCommand GetPeopleCommand { get; }
    public IAsyncRelayCommand SavePersonCommand { get; }
    public IAsyncRelayCommand ResetFieldsCommand { get; }
    public IAsyncRelayCommand SearchForSpousesCommand { get; }
    public Person SelectedPerson { get; set; }

    public PeopleEditorPage()
    {
        InitializeComponent();

        GetPeopleCommand = new AsyncRelayCommand(GetPeopleAsync);
        SavePersonCommand = new AsyncRelayCommand(SavePersonAsync);
        ResetFieldsCommand = new AsyncRelayCommand(ResetFieldsAsync);
        SearchForSpousesCommand = new AsyncRelayCommand(SearchForSpousesBtnClicked);


        BindingContext = this;
    }

    // BUTTONS

    public async Task GetPeopleAsync()
    {
        await FamilyTreeManager.UpdatePeopleList();
        PersonCollection.Clear();

        if (FamilyTreeManager.people.Count == 0) return;
        foreach (var person in FamilyTreeManager.people.Values)
        {
            PersonCollection.Add(person);
        }
    }

    public async Task SavePersonAsync()
    {
        if (FieldsAreValid() == false) return;

        if (FamilyTreeManager.DoesPersonExist(PersonIdEntry.Text))
        {
            await DisplayAlert("Error", "Person with this id already exists!", "OK");
            return;
        }

        Person newPerson = new Person
        {
            personalId = long.Parse(PersonIdEntry.Text),
            name = PersonNameEntry.Text
        };

        await FamilyTreeManager.AddPerson(newPerson);

        await DisplayAlert("Success", "Person added successfully!", "OK");
    }

    public async Task ResetFieldsAsync()
    {
        if (await DisplayAlert("Warning", "Are you sure you want to reset all fields?", "Yes", "No"))
        {
            PersonIdEntry.Text = String.Empty;
            PersonNameEntry.Text = String.Empty;
            PersonSelectionList.SelectedItem = null;
            SpouseSelectionCollection.Clear();
            ChildrenSelectionCollection.Clear();
        }
    }

    private async Task SearchForSpousesBtnClicked()
    {
        if (SelectedPerson == null) return;

        await FamilyTreeManager.UpdatePeopleList();
        SpouseSelectionCollection.Clear();

        Dictionary<long, Person> possibleSpouses = FamilyTreeManager.people;

        possibleSpouses.Remove(SelectedPerson.personalId);

        if (SelectedPerson.childrenIds.Length > 0)
        {
            foreach (var c in SelectedPerson.childrenIds)
            {
                possibleSpouses.Remove(c);
            }
        }

        /*List<Person> parents = FamilyTreeManager.GetParents(SelectedPerson);
        if (parents.Count > 0)
        {
            foreach (var p in parents)
            {
                possibleSpouses.Remove(p.personalId);
            }
        }*/

        List<Person> ancestors = await FamilyTreeManager.GetAncestors(SelectedPerson);
        if (ancestors.Count > 0)
        {
            foreach (var a in ancestors)
            {
                possibleSpouses.Remove(a.personalId);
            }
        }

        List<Person> parents = FamilyTreeManager.GetParents(SelectedPerson);
        List<Person> siblings = FamilyTreeManager.GetSiblings(SelectedPerson, parents);
        if (siblings.Count > 0)
        {
            foreach (var s in siblings)
            {
                possibleSpouses.Remove(s.personalId);
            }
        }

        try
        {
            foreach (var s in possibleSpouses)
            {
                SpouseSelectionCollection.Add(s.Value);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("Couldn't add spouses to list: " + e);
            throw;
        }

        if (SelectedPerson.spouseId != 0)
        {
            foreach (var s in SpouseSelectionCollection)
            {
                if (s.personalId == SelectedPerson.spouseId)
                {
                    break;
                }
            }
        }
    }

    public bool FieldsAreValid()
    {
        // Person's name
        if (string.IsNullOrEmpty(PersonNameEntry.Text))
        {
            DisplayAlert("Error", "Please enter a valid 11 characters long name!", "OK");
            return false;
        }

        foreach (char c in PersonNameEntry.Text)
        {
            if (char.IsLetter(c) == false)
            {
                DisplayAlert("Error", "Please enter a valid name!", "OK");
                return false;
            }
        }

        // Person's ID
        if (string.IsNullOrEmpty(PersonIdEntry.Text))
        {
            DisplayAlert("Error", "Personal ID can't be empty!)", "OK");
            return false;
        }

        if (long.TryParse(PersonIdEntry.Text, out long idP) == false || PersonIdEntry.Text.Length != 11)
        {
            DisplayAlert("Error", "Please enter a valid 11 digit personal id!", "OK");
            return false;
        }

        return true;
    }

    private void PersonSelectBtnClicked(object sender, EventArgs e)
    {
        var selectedPerson = (sender as Button).BindingContext as Person;
        if (selectedPerson != null)
        {
            SelectedPerson = selectedPerson;

            PersonIdEntry.Text = selectedPerson.personalId.ToString();
            PersonNameEntry.Text = selectedPerson.name;

            if (selectedPerson.spouseId != 0)
            {

            }
            else
            {
            }

            if (selectedPerson.childrenIds.Length > 0)
            {

            }
            else
            {

            }
        }
    }

    private void SpouseSelectionBtnCliced(object sender, EventArgs e)
    {
        var selectedSpouse = (sender as Button).BindingContext as Person;
        DisplayAlert("Success!", $"Selected {selectedSpouse.name} as Spouse", "Okay");
    }
}