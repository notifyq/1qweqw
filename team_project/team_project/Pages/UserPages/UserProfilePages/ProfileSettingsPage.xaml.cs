using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace team_project.Pages.UserPages.UserProfilePages
{
    /// <summary>
    /// Логика взаимодействия для ProfileSettingsPage.xaml
    /// </summary>
    public partial class ProfileSettingsPage : Page
    {
        private EventAggregator _eventAggregator;

        ApiUser api = new ApiUser();
        public ProfileSettingsPage(EventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            LoadUserInfo();
        }

        public async Task LoadUserInfo()
        {
            User userInfo = await api.GetCurrentUserInfo();
            StackPanel_UserInfo.DataContext = userInfo;
        }
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private async void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_UserName.Text.Length == 0)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            User user = new User()
            {
                UserName = TextBox_UserName.Text,
            };
            HttpStatusCode statusCode = await api.EditUserInfo(user);
            if (statusCode == HttpStatusCode.OK)
            {
                _eventAggregator.OnUserUpdated();
                MessageBox.Show("Успешно");
            }
        }

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
