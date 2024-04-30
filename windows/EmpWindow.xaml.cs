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
    /// Логика взаимодействия для EmpWindow.xaml
    /// </summary>
    public partial class EmpWindow : Window
    {
        public ObservableCollection<employees> employees { get; set; }
        public ObservableCollection<role> roles { get; set; }
        public EmpWindow()
        {
            InitializeComponent();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            using (var context = new VorobyevP_Scooter_practiceEntities())
            {
                employees = new ObservableCollection<employees>(context.employees.ToList());
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
                    //Устанавливается фильтр для поиска по нескольким полям объекта employees.

                    view.Filter = item =>
                    {
                        employees dataItem = item as employees;

                        if (dataItem != null)
                        {
                            string itemPatronymic = "";
                            string itemName = dataItem.emp_surname.ToLower();
                            string itemSurname = dataItem.emp_name.ToLower();
                            if (dataItem.emp_patronymic != null)
                            {
                                itemPatronymic = dataItem.emp_patronymic.ToLower();
                            }
                            string itemWLogin = dataItem.email.ToLower();

                            /*
                             * Поиск по нескольким полям: фамилии, имени, отчеству и логину.
                             */
                            return itemName.Contains(searchText) ||
                                   itemSurname.Contains(searchText) ||
                                   itemPatronymic.Contains(searchText) ||
                                   itemWLogin.Contains(searchText); ;
                        }

                        return false;
                    };
                }
            }
        }

        private void ApplySorting()
        {
            /*
            * Применение сортировки к списку employees по выбранному параметру и направлению.
            */
            ICollectionView view = CollectionViewSource.GetDefaultView(employees);

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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddEmp addEmp = new AddEmp();
            addEmp.Show();

        }

        /*
         * Обработчик двойного щелчка по элементу списка employees.
         * Открывает окно редактирования выбранного работника.
         */
        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (LViewProduct.SelectedItem != null)
            {
                using (VorobyevP_Scooter_practiceEntities db = new VorobyevP_Scooter_practiceEntities())
                {
                    int id = (LViewProduct.SelectedItem as employees).id_employee;
                    employees employee = db.employees.FirstOrDefault(emp => emp.id_employee == id);
                    EditEmp editemp = new EditEmp(employee);
                    editemp.DataContext = LViewProduct.SelectedItem;
                    bool? result = editemp.ShowDialog();

                    if (result == true)
                    {
                        LoadData();
                    }
                }
            }
        }

        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            EmpWindow newEmpWindow = new EmpWindow();
            newEmpWindow.Show();
            Close();
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
    }
}