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

namespace team_project.Pages.UserPages.UserProfilePages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private EventAggregator _eventAggregator;

        public ProfilePage(EventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;

        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            ProfileFrame.NavigationService.Navigate(new ProfileSettingsPage(_eventAggregator));
        }

        private void Button_History_Click(object sender, RoutedEventArgs e)
        {
            ProfileFrame.NavigationService.Navigate(new PurchaseHistoryPage());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ProfileFrame.NavigationService.Navigate(new ProfileSettingsPage(_eventAggregator));
        }
    }
}
