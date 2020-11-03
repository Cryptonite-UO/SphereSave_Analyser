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
        public SphereFileReader Reader { get; set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Reader = new SphereFileReader();
                //reader.ReadFileToObj("/Users/jmmiljours/Documents/Cryptonite/dossier sans titre/spherechars.scp", SphereFileType.SphereWorld);
                //reader.ReadFileToObj("/Users/jmmiljours/Documents/Cryptonite/dossier sans titre/sphereworld.scp", SphereFileType.SphereWorld);

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(Reader),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}