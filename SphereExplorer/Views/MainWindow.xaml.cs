using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        public ListBox ListStatic;
        private SphereFileReader reader = ((App)Application.Current).Reader;

        List<WorldItem> result = new List<WorldItem>();

        public MainWindow()
        {
            InitializeComponent();
            GetPath();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            ListAccs = this.FindControl<ListBox>("lstAccs");
            ListChars = this.FindControl<ListBox>("lstChars");
            ListItems = this.FindControl<ListBox>("lstItems");
            ListStatic = this.FindControl<ListBox>("lstStatic");
            ListAccs.SelectionChanged += ListAccs_SelectionChanged;
            ListChars.SelectionChanged += ListChars_SelectionChanged;            
        }

        public async Task GetPath()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Sphere file", Extensions = { "scp" } });
            dlg.AllowMultiple = true;

            var dialog = await dlg.ShowAsync(this);
            if (dialog != null)
            {
                await GetPath(dialog);
            }
        }

        public async Task GetPath(string [] path)
        {
            foreach (string s in path)
            { 
                reader.ReadFileToObj(s, SphereFileType.SphereWorld);
            }
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
            ListAccs.Items = new ObservableCollection<Account>(accounts.OrderBy(x => x.Name));
            ListStatic.Items = GetAllStaticItems();
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