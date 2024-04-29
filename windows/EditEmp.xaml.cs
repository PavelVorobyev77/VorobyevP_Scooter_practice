using hash_pswd;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VorobyevP_Scooter_practice.db;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace VorobyevP_Scooter_practice.windows
{
    /// <summary>
    /// Логика взаимодействия для EditEmp.xaml
    /// </summary>
    public partial class EditEmp : Window
    {
        private int? id_role;
        private int? id_work_schedule;
        private string SelectedRoleName;

        public ObservableCollection<employees> employees { get; set; }
        private VorobyevP_Scooter_practiceEntities context;
        private employees employee;

        public class WorkScheduleViewModel
        {
            public int Id { get; set; }
            public TimeSpan ShiftStarted { get; set; }
            public TimeSpan ShiftEnd { get; set; }

            public string NameSchedule => $"{ShiftStarted} - {ShiftEnd}";
        }

        public EditEmp(employees Employee)
        {
            InitializeComponent();
            this.employee = Employee;
            context = new VorobyevP_Scooter_practiceEntities();
            DataContext = this;
            LoadData(employee.id_employee); // Вызов метода LoadData() для загрузки данных выбранного сотрудника
            LoadRoles();
            LoadSchedule();
            LoadGenders();

        }

        private void LoadRoles()
        {
            try
            {
                using (var context = new VorobyevP_Scooter_practiceEntities())
                {
                    var roles = context.role.ToList();
                    cbRoles.ItemsSource = roles;
                    cbRoles.DisplayMemberPath = "namerole";
                    cbRoles.SelectedValuePath = "id_role";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSchedule()
        {
            try
            {
                using (var context = new VorobyevP_Scooter_practiceEntities())
                {
                    var wschedule = context.work_schedule.ToList()
                        .Select(ws => new WorkScheduleViewModel
                        {
                            Id = ws.id_work_schedule,
                            ShiftStarted = ws.shift_started,
                            ShiftEnd = ws.shift_end
                        })
                        .ToList();
                    cbwsch.ItemsSource = wschedule;
                    cbwsch.DisplayMemberPath = "NameSchedule";
                    cbwsch.SelectedValuePath = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке графика работы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGenders()
        {
            try
            {
                using (var context = new VorobyevP_Scooter_practiceEntities())
                {
                    var genders = context.gender.ToList();
                    cbgen.ItemsSource = genders;
                    cbgen.DisplayMemberPath = "namegender";
                    cbgen.SelectedValuePath = "id_gender";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке гендеров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка данных для указанного сотрудника
        private void LoadData(int employeeId)
        {
            using (var context = new VorobyevP_Scooter_practiceEntities())
            {
                employees employee = context.employees.Find(employeeId);
                try
                {
                    if (employee == null)
                    {
                        // Сотрудник не найден, можно обработать этот сценарий
                        // Например, можно вывести сообщение или очистить поля ввода
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("fail");
                }
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

            cbRoles.SelectedIndex = -1; // Очищаем выбранное значение
            cbwsch.SelectedIndex = -1; // Очищаем выбранное значение
            cbgen.SelectedIndex = -1; // Очищаем выбранное значение
        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // Создание диалогового окна для выбора изображения.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Все файлы (*.*)|*.*";

            // Отображение диалогового окна и установка выбранного изображения как источника для элемента Image.
            if (openFileDialog.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            // Создание объекта editedemp с новыми данными из полей ввода.
            employees editedemp = new employees()
            {
                emp_name = txtemp_name.Text,
                emp_surname = txtemp_surname.Text,
                emp_patronymic = txtemp_patronymic.Text,
                phone_number = txtphone_number.Text,
                email = txtLogin.Text,
                pswd = txtPswd.Text,
                id_gender = (int)cbgen.SelectedValue,
                id_role = (int)cbRoles.SelectedValue,
                id_work_schedule = (int)cbwsch.SelectedValue
            };

            // Создание списка для хранения результатов валидации
            var validationResults = new List<ValidationResult>();

            int salary;
            if (!int.TryParse(txtSalary.Text, out salary))
            {
                validationResults.Add(new ValidationResult("Зарплата должна быть числом.", new[] { nameof(editedemp.salary) }));
            }
            else
            {
                editedemp.salary = salary;
            }

            var validationContext = new ValidationContext(editedemp, serviceProvider: null, items: null);
            Validator.TryValidateObject(editedemp, validationContext, validationResults, validateAllProperties: true);

            // Проверка наличия ошибок валидации и вывод сообщения об ошибках при их обнаружении
            if (validationResults.Any())
            {
                string errorMessages = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                MessageBox.Show(errorMessages, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Поиск сотрудника в базе данных для редактирования
            employees selectedemp = context.employees.FirstOrDefault(emp => emp.id_employee == employee.id_employee);
            /*
            * Если работник найден:
            *  - Обновляются свойства работника значениями из нового объекта.
            *  - Если пароль был отредактирован:
            *      - Хешируется новый пароль с помощью объекта hash (предположительно, внедренная зависимость).
            *      - Обновляется пароль работника хешированным значением.
            */
            if (selectedemp != null)
            {
                selectedemp.emp_name = editedemp.emp_name;
                selectedemp.emp_surname = editedemp.emp_surname;
                selectedemp.emp_patronymic = editedemp.emp_patronymic;
                selectedemp.phone_number = editedemp.phone_number;
                selectedemp.email = editedemp.email;
                selectedemp.id_gender = (int)cbgen.SelectedValue;
                selectedemp.id_role = (int)cbRoles.SelectedValue;
                selectedemp.id_work_schedule = (int)cbwsch.SelectedValue;
                selectedemp.salary = editedemp.salary;

                if (!string.IsNullOrWhiteSpace(txtPswd.Text))
                {
                    string hashedPassword = hash.HashPassword(txtPswd.Text);
                    selectedemp.pswd = hashedPassword;
                }

                try
                {
                    context.SaveChanges();
                    MessageBox.Show("Данные сохранены успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PrintList_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                FlowDocument flowdoc = Doc.Document as FlowDocument;
                IDocumentPaginatorSource idp = flowdoc;
                pd.PrintDocument(idp.DocumentPaginator, "Doc");
            }
        }

        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbwsch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbgen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту карточку?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int employeeId = employee.id_employee;

                if (employeeId > 0)
                {
                    employees selectedemp = context.employees.FirstOrDefault(emp => emp.id_employee == employeeId);

                    if (selectedemp != null)
                    {
                        context.employees.Remove(selectedemp);
                        context.SaveChanges();

                        MessageBox.Show("Карточка удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
    }
}

