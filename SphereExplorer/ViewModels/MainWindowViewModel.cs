using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SphereSave_Analyser;
using ReactiveUI;
using Avalonia.Controls;
using Avalonia;

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
        public SphereFileReader Reader;
        public ObservableCollection<WorldChar> WorldPerso { get; set; }

        public ObservableCollection<WorldItem> WorldItems { get; set; }

        public ObservableCollection<Account> Accounts { get; set; }

        public MainWindowViewModel(SphereFileReader reader)
        {
            Reader = reader;
            var query = Reader.WorldCharacters.GroupBy(x => x.account);

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
            WorldPerso = new ObservableCollection<WorldChar>(Reader.WorldCharacters.Where(x => x.IsPlayer).OrderBy(x => x.name).Take(5));
        }
    }
}
