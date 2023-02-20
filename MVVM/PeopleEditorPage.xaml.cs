using CommunityToolkit.Mvvm.Input;
using FamilyTree.Logic;
using FamilyTree.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FamilyTree.Data;

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
    public IAsyncRelayCommand SearchForChildrenCommand { get; }
    public Person SelectedPerson { get; set; }

    public PeopleEditorPage()
    {
        InitializeComponent();

        GetPeopleCommand = new AsyncRelayCommand(GetPeopleAsync);
        SavePersonCommand = new AsyncRelayCommand(SavePersonAsync);
        ResetFieldsCommand = new AsyncRelayCommand(ResetFieldsAsync);
        SearchForSpousesCommand = new AsyncRelayCommand(SearchForSpousesBtnClicked);
        SearchForChildrenCommand = new AsyncRelayCommand(SearchForChildrenBtnClicked);

        BindingContext = this;
    }

    // BUTTONS

    public async Task GetPeopleAsync()
    {
        await FamilyTreeManager.UpdatePeopleList();
        PersonCollection.Clear();

        if (FamilyTreeManager.people == null) return;
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

        FamilyTreeManager.PeopleUpdatedEventCalled();

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

        Dictionary<long, Person> possibleSpouses = new();

        foreach (var person in FamilyTreeManager.people.Values)
        {
            possibleSpouses.Add(person.personalId, person);
        }

        possibleSpouses.Remove(SelectedPerson.personalId);


        /*if (SelectedPerson.childrenIds.Length > 0)
        {
            foreach (var c in SelectedPerson.childrenIds)
            {
                possibleSpouses.Remove(c);
            }
        }*/

        List<Person> descendants = await FamilyTreeManager.GetDescendants(SelectedPerson);
        if (descendants.Count > 0)
        {
            foreach (var d in descendants)
            {
                if (possibleSpouses.ContainsKey(d.personalId))
                {
                    possibleSpouses.Remove(d.personalId);
                }
            }
        }


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
                if (possibleSpouses.ContainsKey(s.personalId))
                {
                    possibleSpouses.Remove(s.personalId);
                }
            }
        }



        if (SelectedPerson.spouseId != 0)
        {
            if (possibleSpouses.ContainsKey(SelectedPerson.spouseId))
            {
                possibleSpouses.Remove(SelectedPerson.spouseId);
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

    private async Task SearchForChildrenBtnClicked()
    {
        if (SelectedPerson == null) return;

        await FamilyTreeManager.UpdatePeopleList();
        ChildrenSelectionCollection.Clear();

        Dictionary<long, Person> possibleChildren = new();

        foreach (var person in FamilyTreeManager.people.Values)
        {
            possibleChildren.Add(person.personalId, person);
        }

        // Remove person themselves
        possibleChildren.Remove(SelectedPerson.personalId);

        // Remove all people who are already children or grandchildren
        List<Person> descendants = await FamilyTreeManager.GetDescendants(SelectedPerson);
        if (descendants.Count > 0)
        {
            foreach (var d in descendants)
            {
                if (possibleChildren.ContainsKey(d.personalId))
                {
                    possibleChildren.Remove(d.personalId);
                }
            }
        }

        // Remove all people who are already parents or grandparents
        List<Person> ancestors = await FamilyTreeManager.GetAncestors(SelectedPerson);
        if (ancestors.Count > 0)
        {
            foreach (var a in ancestors)
            {
                possibleChildren.Remove(a.personalId);
            }
        }

        // Remove all siblings
        List<Person> parents = FamilyTreeManager.GetParents(SelectedPerson);
        List<Person> siblings = FamilyTreeManager.GetSiblings(SelectedPerson, parents);
        if (siblings.Count > 0)
        {
            foreach (var s in siblings)
            {
                if (possibleChildren.ContainsKey(s.personalId))
                {
                    possibleChildren.Remove(s.personalId);
                }
            }
        }

        // Remove all spouses
        if (SelectedPerson.spouseId != 0)
        {
            if (possibleChildren.ContainsKey(SelectedPerson.spouseId))
            {
                possibleChildren.Remove(SelectedPerson.spouseId);
            }
        }

        if (SelectedPerson.spouseId != 0)
        {
            // Remove all descendants of spouses

            List<Person> spouseDescendants = await FamilyTreeManager.GetDescendants(FamilyTreeManager.people[SelectedPerson.spouseId]);
            if (spouseDescendants.Count > 0)
            {
                foreach (var sd in spouseDescendants)
                {
                    if (possibleChildren.ContainsKey(sd.personalId))
                    {
                        possibleChildren.Remove(sd.personalId);
                    }
                }
            }

            // Remove all ancestors of spouse
            List<Person> spouseAncestors = await FamilyTreeManager.GetAncestors(FamilyTreeManager.people[SelectedPerson.spouseId]);
            if (spouseAncestors.Count > 0)
            {
                foreach (var sa in spouseAncestors)
                {
                    if (possibleChildren.ContainsKey(sa.personalId))
                    {
                        possibleChildren.Remove(sa.personalId);
                    }
                }
            }

            // Remove all siblings of spouse
            List<Person> spouseParents = FamilyTreeManager.GetParents(FamilyTreeManager.people[SelectedPerson.spouseId]);
            List<Person> spouseSiblings = FamilyTreeManager.GetSiblings(FamilyTreeManager.people[SelectedPerson.spouseId], spouseParents);
            if (spouseSiblings.Count > 0)
            {
                foreach (var ss in spouseSiblings)
                {
                    if (possibleChildren.ContainsKey(ss.personalId))
                    {
                        possibleChildren.Remove(ss.personalId);
                    }
                }
            }
        }

        // Remove all people who have parents already or who have children already
        foreach (var p in possibleChildren.Values)
        {
            if (p.parentIds.Length != 0)
            {
                possibleChildren.Remove(p.personalId);
            }
            else if (p.childrenIds.Length != 0)
            {
                possibleChildren.Remove(p.personalId);
            }
        }

        try
        {
            foreach (var c in possibleChildren)
            {
                ChildrenSelectionCollection.Add(c.Value);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("Couldn't add children to list: " + e);
            throw;
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
        }
    }

    private async void SpouseSelectionBtnCliced(object sender, EventArgs e)
    {
        var selectedSpouse = (sender as Button).BindingContext as Person;

        // Remove person from old spouse
        if (SelectedPerson.spouseId != 0)
        {
            Debug.WriteLine($"Removing spouse from: {FamilyTreeManager.people[SelectedPerson.spouseId].name}");
            await PersonDatabase.RemoveSpouse(SelectedPerson.spouseId);
        }

        // Update person's spouse
        SelectedPerson.spouseId = selectedSpouse.personalId;

        // Update spouse's spouse
        await FamilyTreeManager.AddSpouse(selectedSpouse.personalId, SelectedPerson.personalId);

        SpouseSelectionCollection.Clear();

        FamilyTreeManager.PeopleUpdatedEventCalled();

        await DisplayAlert("Success!", $"Selected {selectedSpouse.name} as Spouse", "Okay");

    }

    private async void ChildrenSelectionBtnClicked(object sender, EventArgs e)
    {
        var selectedChild = (sender as Button).BindingContext as Person;

        if (selectedChild == null)
        {
            Debug.WriteLine("Selected child is null!");
            return;
        }

        // Add child to person
        List<long> childToBeAdded = new List<long>();
        childToBeAdded.Add(selectedChild.personalId);

        await FamilyTreeManager.AddChildren(SelectedPerson.personalId, childToBeAdded);

        // Add person to child as a parent
        await FamilyTreeManager.AddParents(selectedChild.personalId);

        ChildrenSelectionCollection.Clear();

        FamilyTreeManager.PeopleUpdatedEventCalled();

        await DisplayAlert("Success!", $"Selected {selectedChild.name} as Child", "Okay");
    }
}