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
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using VorobyevP_Scooter_practice.db;

namespace VorobyevP_Scooter_practice.windows
{
    /// <summary>
    /// Логика взаимодействия для AddScooter.xaml
    /// </summary>
    public partial class AddScooter : Window
    {
        private VorobyevP_Scooter_practiceEntities context;


        public AddScooter()
        {
            InitializeComponent();
            context = new VorobyevP_Scooter_practiceEntities();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            scooters newScooter = new scooters
            {
                model = txtmodel.Text,
                manufacturer = txtmanufacturer.Text,
                power = txtpower.Text,
                max_speed = int.Parse(txtmaxspeed.Text),
                weight = int.Parse(txtweight.Text),
                load_capacity = txtloadcap.Text
            };


            /*
            * Обработка ошибок валидации:
            *  - Если присутствуют ошибки валидации:
            *      - Объединяются все сообщения об ошибках в одну строку.
            *      - Отображается сообщение об ошибке с объединенными сообщениями.
            */
            var validationContext = new ValidationContext(newScooter, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(newScooter, validationContext, validationResults, validateAllProperties: true);

            if (validationResults.Any())
            {
                string errorMessages = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                MessageBox.Show(errorMessages, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                context.scooters.Add(newScooter);

                context.SaveChanges();

                MessageBox.Show("Данные сохранены успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
