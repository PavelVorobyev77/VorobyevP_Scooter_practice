using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VorobyevP_Scooter_practice.db;

namespace VorobyevP_Scooter_practice.windows
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public string WelcomeMessage { get; set; }

        public ObservableCollection<scooters> scooters { get; set; }
        public AdminWindow()
        {
            InitializeComponent();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            using (var context = new VorobyevP_Scooter_practiceEntities())
            {
                scooters = new ObservableCollection<scooters>(context.scooters.ToList());
            }
        }

        private void txtSearch_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();

            ICollectionView view = CollectionViewSource.GetDefaultView(LViewProduct.ItemsSource);

            if (view != null)
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    view.Filter = null; //Очистка фильтра, если поле поиска пустое
                }
                else
                {
                    //Устанавливается фильтр для поиска по нескольким полям объекта scooter.

                    view.Filter = item =>
                    {
                        scooters dataItem = item as scooters;

                        if (dataItem != null)
                        {
                            string itemModel = dataItem.model.ToLower();
                            string itemManufact = dataItem.manufacturer.ToLower();
                            string itemMax = dataItem.max_speed.ToString();

                            /*
                             * Поиск по нескольким полям
                             */
                            return itemModel.Contains(searchText) ||
                                   itemManufact.Contains(searchText) ||
                                   itemMax.Contains(searchText);
                        }

                        return false;
                    };
                }
            }
        }

        private void ApplySorting()
        {
            /*
            * Применение сортировки к списку Scooters по выбранному параметру и направлению.
            */
            ICollectionView view = CollectionViewSource.GetDefaultView(scooters);

            if (view != null)
            {
                ComboBoxItem selectedSortItem = cmbSorting.SelectedItem as ComboBoxItem;

                if (selectedSortItem != null && selectedSortItem.Tag != null)
                {
                    string[] sortParams = selectedSortItem.Tag.ToString().Split(',');
                    string sortProperty = sortParams[0];
                    ListSortDirection sortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), sortParams[1]);

                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(sortProperty, sortDirection));
                }
            }
        }
        private void cmbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySorting();
        }

        private void btnPrintList_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                IDocumentPaginatorSource idp = doc;
                pd.PrintDocument(idp.DocumentPaginator, Title);
            }
        }

        /*
         * Обработчик двойного щелчка по элементу списка scooters.
         * Открывает окно редактирования выбранного работника.
         */
        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (LViewProduct.SelectedItem != null)
            {
                using (VorobyevP_Scooter_practiceEntities db = new VorobyevP_Scooter_practiceEntities())
                {
                    int id = (LViewProduct.SelectedItem as scooters).id_scooter;
                    scooters scooter = db.scooters.FirstOrDefault(s => s.id_scooter == id);
                    EditScooter editWindow = new EditScooter(scooter);
                    editWindow.DataContext = LViewProduct.SelectedItem;
                    bool? result = editWindow.ShowDialog();

                    if (result == true)
                    {
                        LoadData();
                    }
                }
            }
        }
        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow newAdminWindow = new AdminWindow();
            newAdminWindow.Show();
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddScooter addWindow = new AddScooter();
            addWindow.Show();
        }
    }
}
