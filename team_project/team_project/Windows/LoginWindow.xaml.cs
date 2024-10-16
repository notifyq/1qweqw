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
using System.Windows.Shapes;
using team_project.Pages;

namespace team_project.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GuestFrame.NavigationService.Navigate(new LoginPage());
        }

        private void GuestFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (GuestFrame.Content != null)
            {
                Page page = GuestFrame.Content as Page;
                this.Title = page.Title;
            }
        }
    }
}
