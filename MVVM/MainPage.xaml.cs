using CommunityToolkit.Mvvm.Input;
using FamilyTree.Data;
using FamilyTree.Logic;
using FamilyTree.Models;
using System.Diagnostics;

namespace FamilyTree.MVVM;

public partial class MainPage : ContentPage
{
    public IAsyncRelayCommand ImportFileCommand { get; }
    public IAsyncRelayCommand ClearDatabaseCommand { get; }

    public MainPage()
    {
        InitializeComponent();

        ImportFileCommand = new AsyncRelayCommand(ImportPeopleFileBtnClicked);
        ClearDatabaseCommand = new AsyncRelayCommand(ClearDatabaseBtnClicked);

        BindingContext = this;
    }

    private void ToAddPeoplePageBtnClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//PeopleEditorPage");
    }

    private async Task ImportPeopleFileBtnClicked()
    {
        string filePath = String.Empty;
        var chosenFile = await FilePicker.PickAsync();

        if (chosenFile == null)
        {
            await DisplayAlert("No File Chosen!", "As no file was chosen then no people were imported!", "Okay");
            return;
        }

        if (Path.GetExtension(chosenFile.FileName) != ".txt")
        {
            await DisplayAlert("Wrong File Type!", "As the file chosen was not a .txt file then no people were imported!", "Okay");
            return;
        }

        filePath = chosenFile.FullPath;
        Debug.WriteLine("Txt file's path is at: " + filePath);

        Dictionary<long, Person> people = await TxtDataReader.ReadFile(filePath);

        foreach (var p in people.Values)
        {
            await PersonDatabase.AddPerson(p);
        }

        await FamilyTreeManager.UpdatePeopleList();

        foreach (var p in people.Values)
        {
            await FamilyTreeManager.AddParents(p.personalId);
        }

        ImportPeopleBtn.Text = $"Imported from {chosenFile.FileName}!";

        await DisplayAlert("Success!", "People were successfully imported!", "Okay");
    }

    private async Task ClearDatabaseBtnClicked()
    {
        await PersonDatabase.ClearDatabase();
        await FamilyTreeManager.UpdatePeopleList();
        FamilyTreeManager.PeopleUpdatedEventCalled();
        await DisplayAlert("Success!", "Database was successfully cleared!", "Okay");
    }
}

