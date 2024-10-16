using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
using team_project.Services;

namespace team_project.Pages.UserPages
{
    /// <summary>
    /// Логика взаимодействия для UserDefaultPage.xaml
    /// </summary>
    public partial class UserLibraryPage : Page
    {
        ApiProduct api = new ApiProduct();
        NotificationManager notificationManager = new NotificationManager();
        public ICollectionView ProductsView { get; set; }

        public UserLibraryPage()
        {
            InitializeComponent();
            LoadProductsAsync();
        }

        public async void LoadProductsAsync()
        {
            ApiUser apiRequest = new ApiUser();
            List<Product> products = await apiRequest.GetUserProducts();

            ProductsView = CollectionViewSource.GetDefaultView(products);
            LibraryListView.ItemsSource = ProductsView;
            Label_ProductsCount.SetBinding(ContentProperty, new Binding("Count") { Source = ProductsView, StringFormat = "Товаров найдено: {0}" });
/*            Label_DownloadedProductsCount.SetBinding(ContentProperty, new Binding("Count") { Source = ProductsView, StringFormat = "Товаров найдено: {0}" });
*/
        }

        private void ProductLaunch_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            Product product = menuItem.DataContext as Product;
            LaunchService.Instance.Start(product.ProductId);
            notificationManager.Show("Запуск", NotificationType.Success);
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

        public void SetImage()
        {
            return;
        }

        private async void ProductDownload_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            Product product = menuItem.DataContext as Product;
            if (product != null)
            {
                await DownloadService.Instance.StartDownload(product.ProductId);
            }
            else
            {

            }
        }

        private void ProductDelete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            Product product = menuItem.DataContext as Product;
            LaunchService.Instance.DeleteProductFiles(product.ProductId);
            notificationManager.Show("Удаление...", NotificationType.Success);
        }
    }

   
}
/* // Путь к папке кеша
 string cacheFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CodeFlow", product.ProductId.ToString(), "ImageCache");

 if (Directory.Exists(cacheFolder))
 {
     string[] imageFiles = Directory.GetFiles(cacheFolder);

     if (imageFiles.Length > 0)
     {
         BitmapImage bitmapImage = new BitmapImage();
         bitmapImage.BeginInit();
         bitmapImage.UriSource = new Uri(imageFiles[0]);
         bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
         bitmapImage.EndInit();
         img.Source = bitmapImage;
     }
     else
     {
         img.Source = (BitmapImage)Application.Current.TryFindResource("DefaultImage"); ;
     }
 }*/



/*                List<BitmapImage> images = await api.GetProductImages(product.ProductId);
*/