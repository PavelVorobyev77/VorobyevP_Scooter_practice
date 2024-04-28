using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using System.Windows.Threading;
using hash_pswd;
using VorobyevP_Scooter_practice.db;
using VorobyevP_Scooter_practice.windows;

namespace VorobyevP_Scooter_practice
{
    public partial class MainWindow : Window
    {
        private int countUnsuccessful = 0; // Счетчик неудачных попыток входа
        private string captcha = string.Empty; // Переменная для хранения капчи
        private bool isButtonBlocked = false; // блокировка кнопки
        private int countdownDuration = 10; // Длительность блокировки в секундах
        private DispatcherTimer countdownTimer; // Таймер для отсчета времени
        private int z = 0; // Переменная для отслеживания времени блокировки

        public MainWindow()
        {
            InitializeComponent();

            txtCaptcha.Visibility = Visibility.Hidden;
            textBlockCaptcha.Visibility = Visibility.Hidden;

            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = new TimeSpan(0, 0, 1);
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            int remainingSeconds = countdownDuration - z;
            if (remainingSeconds > 0)
            {
                seconds.Text = $"Вход заблокирован, попробуйте\nснова, через: {remainingSeconds} секунд";
                z++;
            }
            else
            {
                countdownTimer.Stop();
                isButtonBlocked = false;
                btnEnter.IsEnabled = true;
                txtLogin.IsEnabled = true;
                txtPassword.IsEnabled = true;
                txtCaptcha.IsEnabled = true;
                z = 0;
                seconds.Text = null;
            }
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            //string hashedPassword = hash.HashPassword(password);

            using (var db = new VorobyevP_Scooter_practiceEntities())
            {
                var employee = db.employees.FirstOrDefault(emp => emp.email == login && emp.pswd == password);

                if (employee != null)
                {
                    string welcomeMessage = GetWelcomeMessage(employee);
                    MessageBox.Show(welcomeMessage);
                    countUnsuccessful = 0;
                    LoadForm(employee.role.namerole, welcomeMessage);
                }
                else
                {
                    countUnsuccessful++;
                    GenerateCaptcha();
                    MessageBox.Show("Вы ввели неверный логин или пароль! Введите капчу для продолжения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (countUnsuccessful >= 3)
                {
                    BlockLoginButton();
                }
            }
        }

        private string GetWelcomeMessage(employees employee)
        {
            string greeting = "";

            int currentHour = DateTime.Now.Hour;
            if (currentHour >= 10 && currentHour <= 12)
            {
                greeting = "Доброе утро";
            }
            else if (currentHour >= 12 && currentHour <= 17)
            {
                greeting = "Добрый день";
            }
            else if (currentHour >= 17 && currentHour <= 24)
            {
                greeting = "Добрый вечер";
            }

            string fullName = $"{employee.emp_surname} {employee.emp_name} {employee.emp_patronymic}";

            string welcomeMessage = $"{greeting}, {GetSalutation(employee)} {fullName} ({employee.role.namerole})!";
            return welcomeMessage;
        }

        private string GetSalutation(employees employee)
        {
            if (employee.id_gender == 2)
            {
                return "Mrs";
            }
            else
            {
                return "Mr";
            }
        }

        private void GenerateCaptcha()
        {
            textBlockCaptcha.Visibility = Visibility.Visible;
            txtCaptcha.Visibility = Visibility.Visible;

            Random random = new Random();
            int length = random.Next(5, 10);
            string captchaCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string generatedCaptcha = string.Empty;

            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(0, captchaCharacters.Length);
                generatedCaptcha += captchaCharacters[randomIndex];
            }

            captcha = generatedCaptcha;

            textBlockCaptcha.Text = captcha;
            textBlockCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }

        private void BlockLoginButton()
        {
            txtLogin.IsEnabled = false;
            txtPassword.IsEnabled = false;
            txtCaptcha.IsEnabled = false;
            isButtonBlocked = true;
            btnEnter.IsEnabled = false;
            countdownTimer.Start();
        }

        private void LoadForm(string role, string welcomeMessage)
        {
            switch (role)
            {
                case "Администратор": //1
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.WelcomeMessage = welcomeMessage;
                    adminWindow.Show();
                    Close();
                    break;
                case "Менеджер по операциям": //2
                    ManagerWindow managerWindow = new ManagerWindow();
                    managerWindow.WelcomeMessage = welcomeMessage;
                    managerWindow.Show();
                    Close();
                    break;
            }
        }
    }
}
