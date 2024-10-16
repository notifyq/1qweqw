using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

namespace team_project.Pages.StorePages
{
    /// <summary>
    /// Логика взаимодействия для StorePage.xaml
    /// </summary>
    public partial class StorePage : Page
    {
        ApiProduct api = new ApiProduct();
        NotificationManager notificationManager = new NotificationManager();
        List<Product> products = new List<Product>();
        ICollectionView collectionView;
        private ObservableCollection<Genre> genres;


        public StorePage()
        {
            InitializeComponent();
            LoadInfo();
        }

        public async Task LoadInfo()
        {
            await LoadProducts();
            await LoadGenres();
            collectionView = CollectionViewSource.GetDefaultView(products);
            SetFilter();
            ListViewProductOut.ItemsSource = collectionView;
            collectionView.Refresh();
            Label_ProductsCount.SetBinding(ContentProperty, new Binding("Count") { Source = collectionView, StringFormat = "Товаров найдено: {0}" });
        }

        private void SetFilter()
        {
            collectionView.Filter = obj =>
            {
                Product product = obj as Product;
                bool textMatch = string.IsNullOrEmpty(SearthTextBox.Text) || product.ProductName.Contains(SearthTextBox.Text);
                bool genreMatch = !genres.Any(g => g.IsSelected) || product.ProductGenres.Any(pg => genres.Any(g => g.IsSelected && g.GenreId == pg.GenreId));
                return textMatch && genreMatch;
            };
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            collectionView.Refresh();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            collectionView.Refresh();
        }
        public async Task LoadProducts()
        {
            products = await api.GetProducts();

        }
        private async void SearthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            collectionView.Refresh();
        }
        private void GenreListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            collectionView.Refresh();
        }
        public async Task LoadGenres()
        {
            genres = new ObservableCollection<Genre>(await api.GetGenres());
            GenreListBox.ItemsSource = genres;
        }


        /*  private void ScrollImage_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
          {
              if (e.Delta > 0)
              {
                  ScrollImage.LineLeft();
              }
              else if (e.Delta < 0)
              {
                  ScrollImage.LineRight();
              }
              e.Handled = true;
          }

          private void ScrollCategory_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
          {
              if (e.Delta > 0)
              {
                  ScrollCategory.LineLeft();
              }
              else if (e.Delta < 0)
              {
                  ScrollCategory.LineRight();
              }
              e.Handled = true;
          }*/

        private void BtnCategoryClick(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(((Button)sender).Name.ToString());

            NavigationService.Navigate(new StoreCategoryPage(((Button)sender).Content.ToString()));
        }

        private void GridSingleProduct_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Product selected_product = ListViewProductOut.SelectedItem as Product;
            if (selected_product != null)
            {
                NavigationService.Navigate(new ProductPage(selected_product));
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Product selected_product = new Product();
            NavigationService.Navigate(new ProductPage(selected_product));
        }

        private void CategoryListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private async void CategoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            SearthTextBox.Text = "";
            await LoadProducts();
        }

        private void Button_Basket_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new BasketPage());
        }

        private async void Image_productImage_Loaded(object sender, RoutedEventArgs e)
        {
           Image img = (Image)sender;
           var product = (Product)img.DataContext;
           try
            {
                List<BitmapImage> images = new List<BitmapImage>(0);
                string cacheFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CodeFlow", product.ProductId.ToString(), "ImageCache");
                if (Directory.Exists(cacheFolder))
                {
                    images = await api.GetImages(product.ProductId);
                    if (images.Count > 0)
                    {
                        img.Source = images[0];
                    }
                    else
                    {
                        await api.LoadProductImages(product.ProductId);
                        images = await api.GetImages(product.ProductId);
                        if (images.Count > 0)
                        {
                            img.Source = images[0];
                        }
                        else
                        {
                            img.Source = (BitmapImage)System.Windows.Application.Current.TryFindResource("DefaultImage");
                        }
                    }
                }
                else
                {
                    await api.LoadProductImages(product.ProductId);
                    images = await api.GetImages(product.ProductId);
                    if (images.Count > 0)
                    {
                        img.Source = images[0];
                    }
                    else
                    {
                        img.Source = (BitmapImage)System.Windows.Application.Current.TryFindResource("DefaultImage");
                    }

                }


            }
            catch (Exception)
            {
                img.Source = (BitmapImage)System.Windows.Application.Current.TryFindResource("DefaultImage");
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            var product = (Product)menuItem.DataContext;
            if (product != null)
            {
                Cart cart = new Cart();
                cart.AddToCart(product.ProductId);
                notificationManager.Show("Товар добавлен в корзину", NotificationType.Success);
            }
        }

        private void Grid_RecomendedProduct_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Category_Click(object sender, RoutedEventArgs e)
        {
            GenrePopup.IsOpen = true;
        }

        
    }
}
