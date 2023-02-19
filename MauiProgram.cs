using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using FamilyTree.Logic;

namespace FamilyTree;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>().UseMauiCommunityToolkit();
        builder.UseMauiApp<App>().UseMauiCommunityToolkitCore();

        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        FamilyTreeManager.isInitialized = true;

        return builder.Build();
    }
}
