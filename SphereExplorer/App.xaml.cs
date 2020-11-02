using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SphereExplorer.ViewModels;
using SphereExplorer.Views;
using SphereSave_Analyser;

namespace SphereExplorer
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                SphereFileReader reader = new SphereFileReader();
                reader.ReadFileToObj(reader.dirpathsave + "/spherechars.scp", SphereFileType.SphereWorld);
                reader.ReadFileToObj(reader.dirpathsave +  "/sphereworld.scp", SphereFileType.SphereWorld);

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(reader),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}