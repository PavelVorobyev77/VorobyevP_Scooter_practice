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
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using VorobyevP_Scooter_practice.db;

namespace VorobyevP_Scooter_practice.windows
{
    /// <summary>
    /// Логика взаимодействия для EditScooter.xaml
    /// </summary>
    public partial class EditScooter : Window
    {
        public ObservableCollection<scooters> scooters { get; set; }
        private VorobyevP_Scooter_practiceEntities context;
        private scooters scooter;

        public EditScooter(scooters Scooter)
        {
            InitializeComponent();
            this.scooter = Scooter;
            context = new VorobyevP_Scooter_practiceEntities();
            DataContext = this;
            LoadData(Scooter.id_scooter); // Вызов метода LoadData() для загрузки данных

        }

        // Загрузка данных
        private void LoadData(int scooterId)
        {
            using (var context = new VorobyevP_Scooter_practiceEntities())
            {
                scooter = context.scooters.Find(scooterId);
                try
                {
                    if (scooter == null)
                    {
                        // самокат не найден, можно обработать этот сценарий
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
            txtmodel.Text = string.Empty;
            txtmanufacturer.Text = string.Empty;
            txtpower.Text = string.Empty;
            txtmaxspeed.Text = string.Empty;
            txtweight.Text = string.Empty;
            txtloadcap.Text = string.Empty;
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
            // Создание объекта editedScooter с новыми данными из полей ввода.
            scooters editedScooter = new scooters()
            {
                model = txtmodel.Text,
                manufacturer = txtmanufacturer.Text,
                power = txtpower.Text,
                load_capacity = txtloadcap.Text
            };

            // Создание списка для хранения результатов валидации
            var validationResults = new List<ValidationResult>();

            // Преобразование строки в число для свойств max_speed и weight
            int maxSpeed;
            if (!int.TryParse(txtmaxspeed.Text, out maxSpeed))
            {
                validationResults.Add(new ValidationResult("Максимальная скорость должна быть числом.", new[] { nameof(editedScooter.max_speed) }));
            }
            else
            {
                editedScooter.max_speed = maxSpeed;
            }

            int weight;
            if (!int.TryParse(txtweight.Text, out weight))
            {
                validationResults.Add(new ValidationResult("Вес должен быть числом.", new[] { nameof(editedScooter.weight) }));
            }
            else
            {
                editedScooter.weight = weight;
            }

            // Обработка ошибок валидации
            var validationContext = new ValidationContext(editedScooter, serviceProvider: null, items: null);
            Validator.TryValidateObject(editedScooter, validationContext, validationResults, validateAllProperties: true);

            // Проверка наличия ошибок валидации и вывод сообщения об ошибках при их обнаружении
            if (validationResults.Any())
            {
                string errorMessages = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                MessageBox.Show(errorMessages, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Поиск самоката в базе данных для редактирования
            scooters selectedScooter = context.scooters.FirstOrDefault(s => s.id_scooter == scooter.id_scooter);

            // Если самокат найден:
            //  - Обновляются свойства самоката значениями из нового объекта.
            if (selectedScooter != null)
            {
                selectedScooter.model = editedScooter.model;
                selectedScooter.manufacturer = editedScooter.manufacturer;
                selectedScooter.power = editedScooter.power;
                selectedScooter.max_speed = editedScooter.max_speed;
                selectedScooter.weight = editedScooter.weight;
                selectedScooter.load_capacity = editedScooter.load_capacity;

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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту карточку?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int scooterId = scooter.id_scooter;

                if (scooterId > 0)
                {
                    scooters selectedScooter = context.scooters.FirstOrDefault(s => s.id_scooter == scooterId);

                    if (selectedScooter != null)
                    {
                        context.scooters.Remove(selectedScooter);
                        context.SaveChanges();

                        MessageBox.Show("Карточка удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
    }
}
