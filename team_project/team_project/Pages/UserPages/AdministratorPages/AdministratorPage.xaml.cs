using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace team_project.Pages.UserPages.AdministratorPages
{
    /// <summary>
    /// Логика взаимодействия для AdministratorPage.xaml
    /// </summary>
    public partial class AdministratorPage : Page
    {
        ApiUser api = new ApiUser();
        ApiPublisher apiPublisher = new ApiPublisher();
        List<User> users = new List<User>(0);
        ICollectionView collectionView;
        User selected_user;
        List<Role> roles;
        int selected_role_id = 0;


        public AdministratorPage()
        {
            InitializeComponent();
            LoadInfo();
        }

        private async void Grid_User_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            selected_user = grid.DataContext as User;
            if (selected_user != null)
            {
                await LoadUserInfo(selected_user.UserId);
            }
        }

        public async void LoadInfo()
        {
            await LoadUsers();
            await LoadRoles();
            await SetFilter();
        }

        public async Task LoadUsers()
        {
            users = await api.GetUsers();
            collectionView = CollectionViewSource.GetDefaultView(users);
            ListBox_Users.ItemsSource = collectionView;
        }

        public async Task LoadUserInfo(int user_id)
        {
            User user = await api.GetUser(user_id);
            StackPanel_UserInfo.DataContext = user;
            if (user.UserRole == 3)
            {
                Publisher publisher = await api.GetPublisher(user_id);
                if (publisher != null)
                {
                    TextBox_PublisherName.Text = $"Имя издателя: {publisher.PublisherName}";
                }
            }
            else
            {
                TextBox_PublisherName.Text = "";
            }
           
        }

        private void TextBox_SearchUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            collectionView.Refresh();
        }

        private async Task SetFilter()
        {
            collectionView.Filter = obj =>
            {
                User user = obj as User;
                bool textMatch = string.IsNullOrEmpty(TextBox_SearchUser.Text) || user.UserLogin.Contains(TextBox_SearchUser.Text);
                bool roleMatch = selected_role_id == 0 || user.UserRole == selected_role_id;
                return textMatch && roleMatch;
            };
        }

        private void ComboBox_ChangeRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selected_user != null && ComboBox_ChangeRole.SelectedValue != null)
            {
                int new_role_id = (int)ComboBox_ChangeRole.SelectedValue;
                if (selected_user.UserRole != new_role_id)
                {
                    Button_SaveRoleChanges.IsEnabled = true;
                    Button_SaveRoleChanges.Opacity = 1;
                }
                else
                {
                    Button_SaveRoleChanges.IsEnabled = false;
                    Button_SaveRoleChanges.Opacity = 0.5;

                }
            }
        }

        private async void Button_SaveRoleChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selected_user == null)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            int new_role_id = (int)ComboBox_ChangeRole.SelectedValue;
            statusCode = await api.ChangeUserRole(selected_user.UserId, new_role_id);
            if (statusCode == HttpStatusCode.OK)
            {
                MessageBox.Show("Роль пользователя успешно изменена");
                await LoadUsers();
                await LoadUserInfo(selected_user.UserId);
            }
        }

        public async Task LoadRoles()
        {
            roles = await api.GetRoles();
            ComboBox_ChangeRole.ItemsSource = roles;


            ComboBox_UserRoles.Items.Add(new Role { RoleId = 0, RoleName = "Все роли" });

            foreach (var item in roles)
            {
                ComboBox_UserRoles.Items.Add(item);
            }
         
        }

        private void ListBox_Users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_UserRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_UserRoles.SelectedValue != null)
                selected_role_id = (int)ComboBox_UserRoles.SelectedValue;
            collectionView.Refresh();
        }
    }
}
