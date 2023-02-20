using FamilyTree.Logic;
using FamilyTree.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FamilyTree.MVVM;

public partial class FamilyTreePage : ContentPage
{
    // Collections
    public ObservableCollection<Person> People { get; set; } = new();
    public ObservableCollection<Person> PersonsSiblings { get; set; } = new();
    public ObservableCollection<Person> PersonsChildren { get; set; } = new();
    public ObservableCollection<Person> PersonsGrandChildren { get; set; } = new();


    public FamilyTreePage()
    {
        InitializeComponent();

        BindingContext = this;
    }

    private void PersonSelectedFromList(object sender, SelectedItemChangedEventArgs e)
    {
        ClearAllContainers();

        if (e.SelectedItem is Person person)
        {
            // Update person
            PersonName.Text = person.name;
            PersonId.Text = person.personalId.ToString();

            // Update spouse
            if (person.spouseId != 0)
            {
                SpouseName.Text = FamilyTreeManager.people[person.spouseId].name;
                SpouseId.Text = person.spouseId.ToString();
            }
            else
            {
                SpouseName.Text = "No spouse";
                SpouseId.Text = String.Empty;
            }

            // Update parents
            List<Person> parents = FamilyTreeManager.GetParents(person);

            if (parents.Count == 2)
            {
                Parent1Name.Text = parents.First().name;
                Parent1Id.Text = parents.First().personalId.ToString();
                Parent2Name.Text = parents.Last().name;
                Parent2Id.Text = parents.Last().personalId.ToString();
            }
            else if (parents.Count == 1)
            {
                Parent1Name.Text = parents.First().name;
                Parent1Id.Text = parents.First().personalId.ToString();
            }

            // Update siblings
            List<Person> siblings = FamilyTreeManager.GetSiblings(person, parents);
            if (siblings != null)
            {
                foreach (var s in siblings)
                {
                    PersonsSiblings.Add(s);
                }
            }

            // Update children
            List<Person> children = FamilyTreeManager.GetChildren(person).Result;
            if (children != null)
            {
                foreach (var c in children)
                {
                    PersonsChildren.Add(c);
                }
            }

            // Update grandchildren
            foreach (var c in PersonsChildren)
            {
                List<Person> grandchildren = FamilyTreeManager.GetChildren(c).Result;
                if (grandchildren != null)
                {
                    foreach (var gc in grandchildren)
                    {
                        PersonsGrandChildren.Add(gc);
                    }
                }
            }

        }
    }

    public void ClearAllContainers()
    {
        PersonName.Text = String.Empty;
        PersonId.Text = String.Empty;
        SpouseName.Text = String.Empty;
        SpouseId.Text = String.Empty;
        Parent1Name.Text = String.Empty;
        Parent1Id.Text = String.Empty;
        Parent2Name.Text = String.Empty;
        Parent2Id.Text = String.Empty;
        PersonsSiblings.Clear();
        PersonsChildren.Clear();
        PersonsGrandChildren.Clear();
    }

    public async void GetPeopleToListAsync(object sender, EventArgs e)
    {
        Debug.WriteLine("GetPeopleToListAsync");

        await FamilyTreeManager.UpdatePeopleList();
        People.Clear();

        if (FamilyTreeManager.people.Count == 0) return;
        foreach (var person in FamilyTreeManager.people.Values)
        {
            People.Add(person);
        }
    }
}