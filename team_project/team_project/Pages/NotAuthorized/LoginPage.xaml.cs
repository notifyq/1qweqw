using Newtonsoft.Json;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using team_project.Api;
using team_project.Model;
using team_project.Windows;

namespace team_project.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        NotificationManager notificationManager = new NotificationManager();
        Api.Api apiRequest = new Api.Api();

        public LoginPage()
        {
            InitializeComponent();
            TryLoginWithSavedInfo();
        }


        public async void TryLoginWithSavedInfo()
        {
            try
            {

                Console.WriteLine("Попытка автоматического входа...");
                string login = UserInfoCrypt.DecryptString(Properties.Settings.Default.UserLogin);
                string password = UserInfoCrypt.DecryptString(Properties.Settings.Default.UserPassword);

                if (login == "" || password == "")
                {
                    Console.WriteLine("Поля для автоматического входа были пусты");

                    return;
                }

                string token = await apiRequest.UserLoginAsync(login, password);
                if (await apiRequest.GetLastCodeStatusAsync() == HttpStatusCode.OK)
                {
                    await apiRequest.SetTokenForClientAsync(token);
                    Console.WriteLine("Успешный автоматический вход");

                    Application.Current.MainWindow.Close();

                    UserWindow userWindow = new UserWindow();
                    userWindow.Show();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка автоматического входа:\n {ex.Message}");
            }
        }
        private void Button_Registration_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new RegistrationPage());
        }

        private async void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBox_Login.Text;
            string password = PasswordBox_Password.Password;

            if (login == null || login == "" || password == null || password == "")
            {
                notificationManager.Show("Заполните все поля", NotificationType.Warning);
                return;
            }
            else
            {
                string token = await apiRequest.UserLoginAsync(login, password);

                if (await apiRequest.GetLastCodeStatusAsync() == HttpStatusCode.OK)
                {
                    await apiRequest.SetTokenForClientAsync(token);

                    Properties.Settings.Default.UserLogin = UserInfoCrypt.EncryptString(login);
                    Properties.Settings.Default.UserPassword = UserInfoCrypt.EncryptString(password);
                    Properties.Settings.Default.Save();
                }
                else 
                {
                    return;
                }

                Window mainWindow = Window.GetWindow(this);
                mainWindow.Close();

                UserWindow userWindow = new UserWindow();
                userWindow.Show();

                
            }
        }
    }
}
