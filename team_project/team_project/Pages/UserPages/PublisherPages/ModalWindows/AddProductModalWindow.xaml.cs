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
using team_project.Api;
using team_project.Model;

namespace team_project.Pages.UserPages.PublisherPages.ModalWindows
{
    /// <summary>
    /// Логика взаимодействия для AddProductModalWindow.xaml
    /// </summary>
    public partial class AddProductModalWindow : Window
    {
        public static Model.ProductAdd new_product;
        public static bool isSuccess = false;
        ApiProduct api = new ApiProduct();
        List<Genre> genres = new List<Genre>();

        public AddProductModalWindow()
        {
            InitializeComponent();
            if (new_product != null)
            {
                new_product = null;
            }
            LoadGenres();
        }

        private void Button_Submit_Click(object sender, RoutedEventArgs e)
        {
            isSuccess = true;
            try
            {
                new_product = new ProductAdd()
                {
                    ProductName = TextBox_Name.Text,
                    ProductDescription = TextBox_Description.Text,
                    ProductPrice = Convert.ToDecimal(TextBox_Price.Text),
                    GenreIds = genres.Where(g => g.IsSelected).Select(g => g.GenreId).ToList(),
                };
                this.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Ошибка: " + ex.Message);
            }
            
        }

        public async void LoadGenres()
        {
            genres = await api.GetGenres();
            ListBox_Products.ItemsSource = genres;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            isSuccess = false;
            this.Close();
        }
    }
}
