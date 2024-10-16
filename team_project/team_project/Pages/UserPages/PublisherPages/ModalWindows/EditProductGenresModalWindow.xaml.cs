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
    /// Логика взаимодействия для EditProductGenresModalWindow.xaml
    /// </summary>
    public partial class EditProductGenresModalWindow : Window
    {
        public static List<int> SelectedGenres;
        Product selected_product;
        ApiProduct api = new ApiProduct();
        List<Genre> genres = new List<Genre>();
        public EditProductGenresModalWindow(Product product)
        {
            InitializeComponent();
            selected_product = product;
            LoadGenres();
        }
        public async void LoadGenres()
        {
            genres = await api.GetGenres();
            foreach (var genre in genres)
            {
                genre.IsSelected = selected_product.ProductGenres.Any(pg => pg.GenreId == genre.GenreId);
            }
            ListBox_Products.ItemsSource = genres;
        }

        private void Button_Create_Click(object sender, RoutedEventArgs e)
        {
            SelectedGenres = genres.Where(g => g.IsSelected).Select(g => g.GenreId).ToList();
            this.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            SelectedGenres = null;
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
