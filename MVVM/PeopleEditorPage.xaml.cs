using System.Diagnostics;
using FamilyTree.Logic;
using FamilyTree.Models;

namespace FamilyTree.MVVM;

public partial class PeopleEditorPage : ContentPage
{
	public PeopleEditorPage()
	{
		InitializeComponent();
	}

    public void OnSearchBtnClicked(object sender, EventArgs e)
    {
		FamilyTreeManager.UpdatePeopleList();

        foreach (var person in FamilyTreeManager.people.Values)
        {
            Debug.WriteLine(person.personalId);
        }   
    }

    public void OnSaveBtnClicked(object sender, EventArgs e)
    {
        if (FieldsAreValid() == false)
        {
            return;
        }

        if (FamilyTreeManager.DoesPersonExist(PersonIdEntry.Text))
        {
            DisplayAlert("Error", "Person with this id already exists!", "OK");
            return;
        }

        Person newPerson = new Person
        {
            personalId = long.Parse(PersonIdEntry.Text),
            name = PersonNameEntry.Text,
            spouseId = long.Parse(SpouseIdEntry.Text),
            childrenIds = new long[]{}
        };

        FamilyTreeManager.AddPerson(newPerson);
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
}