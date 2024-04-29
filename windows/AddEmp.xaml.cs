using hash_pswd;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using System.Windows.Shapes;
using System.Xml.Linq;
using VorobyevP_Scooter_practice.db;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace VorobyevP_Scooter_practice.windows
{
    /// <summary>
    /// Логика взаимодействия для AddEmp.xaml
    /// </summary>
    public partial class AddEmp : Window
    {
        private VorobyevP_Scooter_practiceEntities context;
        private int? id_role;
        private int? id_work_schedule;
        private int? id_gender;
        private string SelectedRoleName;
        private string SelectedWorkSchedule;
        private string SelectedGender;
        public bool DialogResult { get; set; }


        public AddEmp()
        {
            InitializeComponent();
            context = new VorobyevP_Scooter_practiceEntities();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (id_role == null)
            {
                MessageBox.Show("Пожалуйста, выберите роль перед сохранением данных.");
                return;
            }

            if (id_work_schedule == null)
            {
                MessageBox.Show("Пожалуйста, выберите график работы перед сохранением данных.");
                return;
            }

            employees newemp = new employees
            {
                emp_name = txtemp_name.Text,
                emp_surname = txtemp_surname.Text,
                emp_patronymic = txtemp_patronymic.Text,
                phone_number = txtphone_number.Text,
                email = txtLogin.Text,
                pswd = hash.HashPassword(txtPswd.Text),
                id_gender = (int)id_gender,
                id_role = (int)id_role,
                id_work_schedule = (int)id_work_schedule


            };

            // Создание списка для хранения результатов валидации
            var validationResults = new List<ValidationResult>();

            int salary;
            if (!int.TryParse(txtSalary.Text, out salary))
            {
                validationResults.Add(new ValidationResult("Зарплата должна быть числом.", new[] { nameof(newemp.salary) }));
            }
            else
            {
                newemp.salary = salary;
            }


            var validationContext = new ValidationContext(newemp, serviceProvider: null, items: null);
            Validator.TryValidateObject(newemp, validationContext, validationResults, validateAllProperties: true);

            if (validationResults.Any())
            {
                string errorMessages = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                MessageBox.Show(errorMessages, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                context.employees.Add(newemp);

                context.SaveChanges();

                DialogResult = true;

                MessageBox.Show("Данные сохранены успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            txtemp_name.Text = string.Empty;
            txtemp_surname.Text = string.Empty;
            txtemp_patronymic.Text = string.Empty;
            txtphone_number.Text = string.Empty;
            txtLogin.Text = string.Empty;
            txtPswd.Text = string.Empty;
            txtSalary.Text = string.Empty;

            cb.SelectedIndex = -1; // Очищаем выбранное значение
            cb2.SelectedIndex = -1; // Очищаем выбранное значение
            cb3.SelectedIndex = -1; // Очищаем выбранное значение
        }

        private void PrintList_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                FlowDocument flowDoc = Doc.Document as FlowDocument;
                IDocumentPaginatorSource idp = flowdoc;
                pd.PrintDocument(idp.DocumentPaginator, "Doc");
            }
        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // Создание диалогового окна для выбора изображения.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // Установка изображения
                imgPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb.SelectedItem != null)
            {
                var selectedComboBoxItem = (ComboBoxItem)cb.SelectedItem;
                SelectedRoleName = selectedComboBoxItem.Content.ToString();
                int roleId;
                if (int.TryParse(selectedComboBoxItem.Tag.ToString(), out roleId))
                {
                    id_role = roleId;
                }
                else
                {
                    MessageBox.Show("Ошибка при получении ID роли.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ComboBox_SelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            if (cb2.SelectedItem != null)
            {
                var selectedComboBoxItem = (ComboBoxItem)cb2.SelectedItem;
                SelectedWorkSchedule = selectedComboBoxItem.Content.ToString();
                int wschId;
                if (int.TryParse(selectedComboBoxItem.Tag.ToString(), out wschId))
                {
                    id_work_schedule = wschId;
                }
                else
                {
                    MessageBox.Show("Ошибка при получении ID графика работы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ComboBox_SelectionChanged3(object sender, SelectionChangedEventArgs e)
        {
            if (cb3.SelectedItem != null)
            {
                var selectedComboBoxItem = (ComboBoxItem)cb3.SelectedItem;
                SelectedGender = selectedComboBoxItem.Content.ToString();
                int genId;
                if (int.TryParse(selectedComboBoxItem.Tag.ToString(), out genId))
                {
                    id_gender = genId;
                }
                else
                {
                    MessageBox.Show("Ошибка при получении ID пола.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
