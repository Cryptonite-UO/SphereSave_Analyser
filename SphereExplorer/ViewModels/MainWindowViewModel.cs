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
        public ObservableCollection<WorldChar> WorldPerso { get; set; }

        public ObservableCollection<WorldItem> WorldItems { get; set; }

        public ObservableCollection<Account> Accounts { get; set; }

        public ObservableCollection<WorldItem> StaticItems { get; set; }

        public MainWindowViewModel(SphereFileReader reader)
        {
        }

    }
}
