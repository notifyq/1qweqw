using Notification.Wpf;
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
using team_project.Api;
using team_project.Model;

namespace team_project.Pages.StorePages
{
    /// <summary>
    /// Логика взаимодействия для BasketPage.xaml
    /// </summary>
    public partial class BasketPage : Page
    {
        NotificationManager notificationManager = new NotificationManager();
        ApiStore apiStore = new ApiStore();
        List<BitmapImage> images = new List<BitmapImage>(0);
        ApiProduct apiProduct = new ApiProduct();
        List<int> user_cart;
        List<Product> products;
        Cart cart = new Cart();

        public BasketPage()
        {
            InitializeComponent();
            LoadCart();
        }

        public async Task LoadCart()
        {
            user_cart = cart.GetCartList();

            if (user_cart.Count > 0)
            {
                products = await apiProduct.GetCartList(user_cart);
            }
            else
            {
                return;
            }

            decimal? total_price = 0;
            

            if (ListViewBusketOut.Items.Count != 0)
            {
                ListViewBusketOut.Items.Clear();
            }
            foreach (var item in products)
            {
                total_price += item.ProductPrice;
                ListViewBusketOut.Items.Add(item);
            }
            TextBlock_TotalPrice.Text = "Общая цена: " + total_price.ToString() + " руб.";

        }

        private void DeleteFromBusketTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            var product = (Product)textBlock.DataContext;

            if (product != null)
            {
                cart = new Cart();
                cart.RemoveFromCart(product.ProductId);
                LoadCart();
            }
            /*notificationManager.Show("Товар удален из корзины", NotificationType.Information);
*/
        }

        private async void BuyBtn_Click(object sender, RoutedEventArgs e)
        {
            /*await api.BuyProducts(user_cart);*/
            apiStore.Purchase();
            notificationManager.Show("Чек отправлен на вашу почту");
            cart.ClearCart();
            this.NavigationService.GoBack();
        }

        private async void DeleteAllProductInBusket_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cart.ClearCart();
            await LoadCart();
        }

        private void ProductNameTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private async void DeleteAllProductInBusket_Click(object sender, RoutedEventArgs e)
        {
             cart.ClearCart();
             await LoadCart();
        }

        private void ListViewBusketOut_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private async void MainImage_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = (Image)sender;
            Product product = (Product)img.DataContext;
            images = await LoadImages(product.ProductId);
            try
            {
                if (images.Count > 0)
                {
                    img.Source = images[0];
                }
                else
                {
                    img.Source = (BitmapImage)Application.Current.TryFindResource("DefaultImage");
                }
            }
            catch (Exception)
            {
                img.Source = (BitmapImage)Application.Current.TryFindResource("DefaultImage");
            }
        }
        private async Task<List<BitmapImage>> LoadImages(int product_id)
        {
            await apiProduct.LoadProductImages(product_id);
            return await apiProduct.GetImages(product_id);
        }
    }
}
