namespace FamilyTree.MVVM;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		InitializeComponent();
	}

	private void ToAddPeoplePageBtnClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//PeopleEditorPage");
	}
}

