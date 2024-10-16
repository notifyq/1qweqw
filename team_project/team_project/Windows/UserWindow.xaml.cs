using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using team_project.Api;
using team_project.Pages.UserPages;
using team_project.Pages.UserPages.UserProfilePages;

namespace team_project.Windows
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        Page empty_page = new Page();
        OverlayPage overlay_page = new OverlayPage();
        ApiUser apiUser = new ApiUser();
        private EventAggregator _eventAggregator;
        public UserWindow()
        {
            InitializeComponent();
            UserFrame.NavigationService.Navigate(new UserLibraryPage());
            OverlayFrame.NavigationService.Navigate(empty_page);
            if (DownloadService.Instance.CurrentProductId != 0)
            {
                DownloadService.Instance.StartDownload(DownloadService.Instance.CurrentProductId);
            }

            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                string value = Properties.Settings.Default[currentProperty.Name].ToString();
                Console.WriteLine($"{currentProperty.Name}: {value}");
            }
            LoadUserName();
            _eventAggregator = new EventAggregator();
            _eventAggregator.UserUpdated += LoadUserName;
        }
        public async void LoadUserName()
        {
            Model.User user = await apiUser.GetCurrentUserInfo();
            if (user != null) 
            { 
                Label_UserName.Content = user.UserName;
            }
        }
        private void Button_MenuOverlay_Click(object sender, RoutedEventArgs e)
        {
            if (OverlayFrame.Content as Page == overlay_page)
            {
                OverlayFrame.NavigationService.GoBack();
                UserFrame.Opacity = 1;
                OverlayRectangle.IsEnabled = true;
            }
            else if (OverlayFrame.Content as Page == empty_page)
            {
                OverlayFrame.Navigate(overlay_page);
                UserFrame.Opacity = 0.5;
                OverlayRectangle.IsEnabled = false;
            }
            
        }


        private void OverlayFrame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private async void OverlayFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (OverlayFrame.Content != null)
            {
                var ta = new ThicknessAnimation();
                ta.Duration = TimeSpan.FromSeconds(0.5);
                ta.DecelerationRatio = 0.7;
                ta.To = new Thickness(0, 0, 0, 0);
                if (e.NavigationMode == NavigationMode.New)
                {
                    ta.From = new Thickness(-700, 0, 0, 0);
                    (OverlayFrame as Frame).BeginAnimation(MarginProperty, ta);
                }
                else if (e.NavigationMode == NavigationMode.Back)
                {
                    ta.From = new Thickness(0, 0, OverlayFrame.ActualWidth, 0);
                    (OverlayFrame as Frame).BeginAnimation(MarginProperty, ta);
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(element), null);
                Keyboard.ClearFocus();
            }
        }

        private void UserFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (OverlayFrame.Content as Page == overlay_page)
            {
                OverlayFrame.NavigationService.GoBack();
                UserFrame.Opacity = 1;
                OverlayRectangle.IsEnabled = true;
            }
            
        }

        private void UserFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (UserFrame.Content != null)
            {
                Page page = UserFrame.Content as Page;
                this.Title = page.Title;
            }
        }

        private void Button_Profile_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.NavigationService.Navigate(new ProfilePage(_eventAggregator));
        }
    }
}
