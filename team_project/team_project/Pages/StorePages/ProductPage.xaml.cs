using Notification.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using team_project.Api;
using team_project.Model;

namespace team_project.Pages.StorePages
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        Product selected_product;
        ApiProduct api = new ApiProduct();
        List<BitmapImage> images = new List<BitmapImage>(0);
        NotificationManager notificationManager = new NotificationManager();
        public ProductPage(Product selected_product)
        {
            this.selected_product = selected_product;
            InitializeComponent();
            /*            LoadBtnImage();*/

            Grid_Product.DataContext = selected_product;
            if (selected_product.ProductStatusId == 14)
            {
                Grid_Product.Opacity = 0.5;
                Grid_Product.IsEnabled = false;
                TextBlock_ProductName.Text = "Товар не в продаже";
                Button_AddToCart.IsEnabled = false;
            }
            /*LoadInfo();*/
        }

        /*  public async void LoadInfo()
          {
              images = await api.GetProductImages(selected_product.ProductId);
          }*/
        private void BtnCategoryClick(object sender, EventArgs e)
        {
            //MessageBox.Show(((Button)sender).Content.ToString());

            NavigationService.Navigate(new StoreCategoryPage(((Button)sender).Content.ToString()));
        }
        private async void Button_AddToCart_Click(object sender, RoutedEventArgs e)
        {
            Product product = Grid_Product.DataContext as Product;
            if (product != null)
            {
                Cart cart = new Cart();
                cart.AddToCart(product.ProductId);
                notificationManager.Show("Товар добавлен в корзину", NotificationType.Success);
            }
        }
        private async void Image_productImage_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = (Image)sender;
            images = await LoadImages();
            try
            {
                if (images.Count > 0)
                {
                    img.Source = images[0];
                    Grid_Product_BackGroundImage.ImageSource = images[0];
                    if (images.Count > 1)
                    {
                        foreach (var item in images)
                        {
                            IMGListView.Items.Add(item);
                        }
                        IMGListView.SelectedIndex = 0;
                    }


                }
                else
                {
                    img.Source = (BitmapImage)Application.Current.TryFindResource("DefaultImage");
                }
            }
            catch (Exception)
            {

            }
        }

        int image_index = 0;
        private void Image_Back_Click(object sender, RoutedEventArgs e)
        {
            if (MainImage == null)
                return;
            if (image_index <= 0 || images.Count > 1)
            {
                return;
            }
            else
            {
                image_index--;
                MainImage.Source = images[image_index];
            }
        }

        private void Image_Next_Click(object sender, RoutedEventArgs e)
        {
            if (MainImage == null)
                return;
            if (image_index >= images.Count - 1 || images.Count > 1)
            {
                return;
            }
            else
            {
                image_index++;
                MainImage.Source = images[image_index];
            }
        }

        private void MainImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (images.Count > 0)
            {
                MainImage.Source = images[image_index];
            }
        }

        private void IMGListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (images.Count > 0)
            {

            }
        }

        private async Task<List<BitmapImage>> LoadImages()
        {
            await api.LoadProductImages(selected_product.ProductId);
            return await api.GetImages(selected_product.ProductId);
        }
    }
}
