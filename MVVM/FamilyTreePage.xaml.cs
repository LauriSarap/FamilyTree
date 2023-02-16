using System.Collections.ObjectModel;
using System.Diagnostics;
using FamilyTree.Logic;
using FamilyTree.Models;
using CommunityToolkit.Mvvm.Input;

namespace FamilyTree.MVVM;

public partial class FamilyTreePage : ContentPage
{
    // Collections
    public ObservableCollection<Person> People { get; set; } = new();
	public ObservableCollection<Person> PersonsSiblings { get; set; } = new();
	public ObservableCollection<Person> PersonsChildren { get; set; } = new();

    // Commands

    public FamilyTreePage()
	{
		InitializeComponent();


        BindingContext = this;
    }

    private void PersonSelectedFromList(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Person person)
        {
            Debug.WriteLine($"Chose {person.name}!");
        }
    }

    public async void GetPeopleToListAsync(object sender, EventArgs e)
    {
        Debug.WriteLine("GetPeopleToListAsync");

        await FamilyTreeManager.UpdatePeopleList();
        People.Clear();
        foreach (var person in FamilyTreeManager.people.Values)
        {
            People.Add(person);
        }
    }
}