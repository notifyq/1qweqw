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
using System.Windows.Shapes;
using team_project.Api;
using team_project.Model;

namespace team_project.Pages.UserPages.ModeratorPages.ModalWindows
{
    /// <summary>
    /// Логика взаимодействия для ProductUpdateModalWindow.xaml
    /// </summary>
    public partial class ProductUpdateModalWindow : Window
    {
        ApiStatus apiStatus = new ApiStatus();
        ApiProduct api = new ApiProduct();
        List<Status> statusList = new List<Status>();
        ProductUpdate selected_update;
        public ProductUpdateModalWindow(ProductUpdate productUpdate)
        {
            InitializeComponent();
            selected_update = productUpdate;
            Grid_ProductUpdate.DataContext = selected_update;

            LoadStatuses();
        }
        public async void LoadStatuses()
        {
            statusList = await apiStatus.GetUpdateStatuses();
            ComboBox_Statuses.ItemsSource = statusList;
        }

        private async void Button_SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selected_update == null)
            {
                return;
            }
            if (ComboBox_Statuses.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите статус");
                return;
            }
            HttpStatusCode statusCode = await api.ChangeUpdateStatus(selected_update.ProductUpdateId, (int)ComboBox_Statuses.SelectedValue);
            if (statusCode == HttpStatusCode.OK)
            {
                MessageBox.Show("Статус обновлен");
                this.Close();
            }

            
        }
    }
}
