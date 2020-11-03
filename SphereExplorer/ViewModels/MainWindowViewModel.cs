using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SphereSave_Analyser;
using ReactiveUI;
using Avalonia.Controls;
using Avalonia;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;

namespace SphereExplorer.ViewModels
{
    public class Account
    {
        public string Name;

        public List<WorldChar> Characters;

        public Account()
        {
            Characters = new List<WorldChar>();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        
        private SphereFileReader reader = ((App)Application.Current).Reader;
        private ObservableCollection<Account> accounts;
        private ObservableCollection<WorldItem> staticitems;

        public ObservableCollection<Account> Accounts
        {
            get => accounts;
            set
            {
                this.RaiseAndSetIfChanged(ref accounts, value);
            }
        }

        public ObservableCollection<WorldItem> StaticItems
        {
            get => staticitems;
            set
            {
                this.RaiseAndSetIfChanged(ref staticitems, value);
            }
        }

        public ObservableCollection<WorldChar> WorldPerso { get; set; }

        public ObservableCollection<WorldItem> WorldItems { get; set; }

        public ReactiveCommand<Unit, Unit> OpenCommand { get; }

        public MainWindowViewModel(SphereFileReader reader)
        {

            OpenCommand = ReactiveCommand.CreateFromTask(Open);
        }

        public async Task Open()
        {
            var dlg = new OpenFolderDialog();
            dlg.Title = "Select Sphere saves to load";

            var dialog = await dlg.ShowAsync(GetWindow());
            if (dialog != null)
            {
                await Open(dialog);
            }
        }

        public async Task Open(string path)
        {
            reader.ReadFileToObj(path + "/save/sphereworld.scp", SphereFileType.SphereWorld);
            reader.ReadFileToObj(path + "/save/spherechars.scp", SphereFileType.SphereWorld);
            ReloadData();
        }

        private void ReloadData()
        {
            var query = reader.WorldCharacters.GroupBy(x => x.account);

            List<Account> accounts = new List<Account>();

            foreach (var x in query)
            {
                Account acc = new Account
                {
                    Name = x.Key
                };
                foreach (var c in x)
                {
                    acc.Characters.Add(c);
                }
                accounts.Add(acc);
            }
            Accounts = new ObservableCollection<Account>(accounts.OrderBy(x => x.Name));
            StaticItems = GetAllStaticItems();
        }

        public ObservableCollection<WorldItem> GetAllStaticItems()
        {
            var colection = new ObservableCollection<WorldItem>();
            foreach (var c in reader.WorldItems)
            {
                if ((c.attr & (int)Flags.ATTR_STATIC) > 0)
                {
                    colection.Add(c);
                }
            }
            return colection;
        }

        private Window GetWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }
            return null;
        }
    }
}
