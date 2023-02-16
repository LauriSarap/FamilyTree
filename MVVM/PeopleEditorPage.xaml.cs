using System.Collections.ObjectModel;
using FamilyTree.Logic;
using FamilyTree.Models;
using CommunityToolkit.Mvvm.Input;

namespace FamilyTree.MVVM;

public partial class PeopleEditorPage : ContentPage
{
    public ObservableCollection<Person> PersonCollection { get; set; } = new ();
    public IAsyncRelayCommand GetPeopleCommand { get; }
    public IAsyncRelayCommand SavePersonCommand { get; }
    public IAsyncRelayCommand ResetFieldsCommand { get; }

    public PeopleEditorPage()
	{
		InitializeComponent();

        GetPeopleCommand = new AsyncRelayCommand(GetPeopleAsync);
        SavePersonCommand = new AsyncRelayCommand(SavePersonAsync);
        ResetFieldsCommand = new AsyncRelayCommand(ResetFieldsAsync);


        BindingContext = this;
	}

    public async Task GetPeopleAsync()
    {
        await FamilyTreeManager.UpdatePeopleList();
        PersonCollection.Clear();
        foreach (var person in FamilyTreeManager.people.Values)
        {
            PersonCollection.Add(person);
        }
    }

    public async Task SavePersonAsync()
    {
        if (FieldsAreValid() == false)
        {
            return;
        }

        if (FamilyTreeManager.DoesPersonExist(PersonIdEntry.Text))
        {
            await DisplayAlert("Error", "Person with this id already exists!", "OK");
            return;
        }

        Person newPerson = new Person
        {
            personalId = long.Parse(PersonIdEntry.Text),
            name = PersonNameEntry.Text,
            spouseId = long.Parse(SpouseIdEntry.Text),
            childrenIds = new long[]{}
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
            SpouseIdEntry.Text = String.Empty;
            ChildIdEntry.Text = String.Empty;
            PersonSelectionList.SelectedItem = null;

        }
    }

    public bool FieldsAreValid()
    {
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

        // Spouse's ID
        if (string.IsNullOrEmpty(SpouseIdEntry.Text))
        {
            DisplayAlert("Error", "Please enter a valid last name!", "OK");
            return false;
        }

        if (long.TryParse(SpouseIdEntry.Text, out long idS) == false || SpouseIdEntry.Text.Length != 11)
        {
            DisplayAlert("Error", "Please enter a valid 11 digit spouse id or leave it blank!", "OK");
            return false;
        }

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

        return true;
    }

    private void PersonSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Person person)
        {
            PersonIdEntry.Text = person.personalId.ToString();
            PersonNameEntry.Text = person.name;

            if (person.spouseId != 0)
            {
                SpouseIdEntry.Text = person.spouseId.ToString();
            }
            else
            {
                SpouseIdEntry.Text = String.Empty;
            }

            if (person.childrenIds.Length > 0)
            {
                string text = String.Empty;
                foreach (var child in person.childrenIds)
                {
                    text += child.ToString() + ", ";
                }

                text = text.Remove(text.Length - 2);

                ChildIdEntry.Text = text;
            }
            else
            {
                ChildIdEntry.Text = String.Empty;
            }
        }
    }
}