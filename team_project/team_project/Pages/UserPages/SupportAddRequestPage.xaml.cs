using Notification.Wpf;
using System;
using System.Collections.Generic;
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
using team_project.Api;
using team_project.Model;

namespace team_project.Pages.UserPages
{
    /// <summary>
    /// Логика взаимодействия для SupportAddRequestPage.xaml
    /// </summary>
    public partial class SupportAddRequestPage : Page
    {
        private SupportRequest request;
        private List<RequestType> supportTypes;
        private ApiSupport apiRequest = new ApiSupport();
        NotificationManager notificationManager = new NotificationManager();


        public SupportAddRequestPage()
        {
            InitializeComponent();
            LoadSupportTypes();
        }

        private async void Button_SendRequest_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_SupportType.SelectedItem == null || ComboBox_SupportType.SelectedIndex == -1)
            {
                notificationManager.Show("Выберите тип запроса", NotificationType.Error);
                return;
            }
            if (TextBox_RequestTitle.Text == null || TextBox_RequestTitle.Text == "")
            {
                notificationManager.Show("Заполните тему запроса", NotificationType.Error);
                return;
            }

            RequestType supportType = ComboBox_SupportType.SelectedItem as RequestType;
            string request_title = TextBox_RequestTitle.Text;

            await apiRequest.AddSupportRequest(supportType.RequestTypeId, request_title);
            if (await apiRequest.GetLastCodeStatusAsync() == System.Net.HttpStatusCode.Created)
            {
                notificationManager.Show("Запрос добавлен", NotificationType.Success);
                this.NavigationService.Navigate(new UserSupportServicePage());
            }
            else
            {
                MessageBox.Show(apiRequest.GetLastCodeStatusAsync().Result.ToString());
            }
            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async Task LoadSupportTypes()
        {
            supportTypes = await apiRequest.GetSupportType();
            if (ComboBox_SupportType.Items.Count != 0)
            {
                ComboBox_SupportType.Items.Clear();
            }
            foreach (var support_type in supportTypes)
            {
                ComboBox_SupportType.Items.Add(support_type);
            }
        }
    }
}
