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
    /// Логика взаимодействия для UserSupportServicePage.xaml
    /// </summary>
    public partial class UserSupportServicePage : Page
    {
        List<SupportRequest> request_list = new List<SupportRequest>(0); // весь список
        List<SupportRequest> request_list_temp = new List<SupportRequest>(0); //для привязки
        SupportRequest selected_request = new SupportRequest();
        ApiSupport apiRequest = new ApiSupport();
        int selected_status_id = 0;
        public UserSupportServicePage()
        {
            InitializeComponent();
            LoadUserRequests();
            SearthTextBox.Text = "";
            selected_status_id = 0;
            SetRequests();
        }

        public async void LoadUserRequests()
        {
            request_list = await apiRequest.GetUserSupportRequests();
        }

        private void AllRequests_Click(object sender, RoutedEventArgs e)
        {
            SearthTextBox.Text = "";
            selected_status_id = 0;
            LoadUserRequests();
            SetRequests();
        }

        private void NewRequests_Click(object sender, RoutedEventArgs e)
        {
            selected_status_id = 7;
            LoadUserRequests();
            SetRequests();
        }

        private void ActiveRequests_Click(object sender, RoutedEventArgs e)
        {
            selected_status_id = 9; //9-10
            LoadUserRequests();
            SetRequests();
        }

        private void CompletedRequests_Click(object sender, RoutedEventArgs e)
        {
            selected_status_id = 8;
            LoadUserRequests();
            SetRequests();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selected_request = ListView_SupportRequests.SelectedItem as SupportRequest;
            if (selected_request != null)
            {
                this.NavigationService.Navigate(new SupportRequestPage(selected_request));
            }
           
            /* NavigationService.Navigate(new DescrtiptionSupportPage());*/
        }

        private void Button_AddRequest_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SupportAddRequestPage());
        }

        private void ListView_SupportRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*this.NavigationService.Navigate(new SupportRequestPage(selected_request));*/
        }

        private void SearthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadUserRequests();
            SetRequests();
        }

        public void SetRequests()
        {
            ListView_SupportRequests.Items.Clear();
            if (selected_status_id == null || selected_status_id == 0)
            {
                foreach (var item in request_list)
                {
                    if (item.RequestTitle.Contains(SearthTextBox.Text))
                    {
                        ListView_SupportRequests.Items.Add(item);
                    }
                }

            }
            else if (selected_status_id == 9)
            {
                foreach (var item in request_list)
                {
                    if (item.RequestTitle.Contains(SearthTextBox.Text) & (item.RequestStatusId == 9 || item.RequestStatusId == 10))
                    {
                        ListView_SupportRequests.Items.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in request_list)
                {
                    if (item.RequestTitle.Contains(SearthTextBox.Text) && item.RequestStatusId == selected_status_id)
                    {
                        ListView_SupportRequests.Items.Add(item);
                    }
                }
            }
            ListView_SupportRequests.Items.Refresh();

        }
    }
}
