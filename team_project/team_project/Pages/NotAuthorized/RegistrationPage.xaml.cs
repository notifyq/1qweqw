using Newtonsoft.Json;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
using team_project.Model;

namespace team_project.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        Api.Api api = new Api.Api();
        NotificationManager notificationManager = new NotificationManager();
        public RegistrationPage()
        {
            InitializeComponent();
        }

        public bool IsValidEmailAddress(string email)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(email);
        }

        private async void Button_Registration_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBox_Login.Text;
            string password = PasswordBox_Password.Password;
            string email = TextBox_Email.Text;
            string verifypassword = PasswordBox_PasswordVerify.Password;

            if (login == null || login == "" ||
                password == null || password == "" ||
                email == null || email == "" ||
                verifypassword == null || verifypassword == "")
            {
                notificationManager.Show("Заполните все поля", NotificationType.Warning);
                return;
            }

            if (password != verifypassword)
            {
                notificationManager.Show("Пароли не совпадают", NotificationType.Warning);
                return;
            }
/*            if (!IsValidEmailAddress(email))
            {
                notificationManager.Show("Почта", NotificationType.Warning);
                return;
            }*/

            await api.UserRegistrationAsync(login, password, email);

            if (await api.GetLastCodeStatusAsync() == HttpStatusCode.Created)
            {
                this.NavigationService.GoBack();
            }
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
