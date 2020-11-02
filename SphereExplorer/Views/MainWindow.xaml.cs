using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SphereExplorer.ViewModels;
using SphereSave_Analyser;

namespace SphereExplorer.Views
{
    public class MainWindow : Window
    {
        public ListBox ListAccs;
        public ListBox ListChars;
        public ListBox ListItems;
        List<WorldItem> result = new List<WorldItem>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            ListAccs = this.FindControl<ListBox>("lstAccs");
            ListChars = this.FindControl<ListBox>("lstChars");
            ListItems = this.FindControl<ListBox>("lstItems");
            ListAccs.SelectionChanged += ListAccs_SelectionChanged;
            ListChars.SelectionChanged += ListChars_SelectionChanged;            
        }

        private void ListChars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ((ListBox)sender).SelectedItem;
            result = new List<WorldItem>();
            if (item == null)
            {
                ListItems.Items = null;
                return;
            }
            int serial = (((ListBox)sender).SelectedItem as WorldChar).serial;
            SphereFileReader reader = (DataContext as MainWindowViewModel).Reader;
            List<WorldItem> a = GetAllItemsForContainer(reader,serial);
            ListItems.Items = new ObservableCollection<WorldItem>(a);
        }

        public List<WorldItem> GetAllItemsForContainer(SphereFileReader reader, int cont)
        {
            var itemforuid = from obj in reader.WorldItems
                             where obj.cont == cont
                             select obj;

            foreach (var o in itemforuid)
            {
                GetAllItemsForContainer(reader,o.serial);
                result.Add(o);
            }
            return result;
        }

        private void ListAccs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<WorldChar> charlist = (((ListBox)sender).SelectedItem as Account).Characters;
            ListChars.Items = new ObservableCollection<WorldChar>(charlist);
        }
    }
}