using System.Windows;
using System.Windows.Controls;
using team_project.Pages.StorePages;
using team_project.Pages.UserPages.AdministratorPages;
using team_project.Pages.UserPages.ModeratorPages;
using team_project.Pages.UserPages.PublisherPages;

namespace team_project.Pages.UserPages
{
    /// <summary>
    /// Логика взаимодействия для OverlayPage.xaml
    /// </summary>
    public partial class OverlayPage : Page
    {
        Window mainWindow;
        Frame UserFrame;
        public OverlayPage()
        {
            InitializeComponent();
            string userRole = Properties.Settings.Default["UserRole"] as string;
            if (userRole.Length > 0)
            {
                if (userRole == "Издатель")
                {
                    Button_Publisher.Visibility = Visibility.Visible;
                }
                if (userRole == "Менеджер")
                {
                    Button_Moderator.Visibility = Visibility.Visible;
                }
                if (userRole == "Администратор")
                {
                    Button_Administrator.Visibility = Visibility.Visible;
                }
            }
        }

        private void Button_Library_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.NavigationService.Navigate(new UserLibraryPage());
        }

        private void Button_SupportService_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.NavigationService.Navigate(new UserSupportServicePage());
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            ClearUserInfo();
            mainWindow.Close();
        }

        public void ClearUserInfo()
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
        }

        public void LoadCurrentWindow()
        {
            mainWindow = Window.GetWindow(this);
            UserFrame = mainWindow.FindName("UserFrame") as Frame;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCurrentWindow();
        }

        private void Button_Store_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.NavigationService.Navigate(new StorePage());
        }

        private void Button_Friends_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.NavigationService.Navigate(new UserFriendsPage());
        }

        private void Button_Downloads_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.NavigationService.Navigate(new DownloadPage());
        }

        private void Button_Publisher_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.NavigationService.Navigate(new PublisherPage());
        }

        private void Button_Moderator_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.NavigationService.Navigate(new ModeratorPage());
        }

        private void Button_Administrator_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.NavigationService.Navigate(new AdministratorPage());
        }
    }
}
