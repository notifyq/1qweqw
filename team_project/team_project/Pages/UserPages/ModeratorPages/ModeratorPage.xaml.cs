using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static team_project.Api.ApiProduct;
using team_project.Api;
using team_project.Model;
using System.Net;
using team_project.Pages.UserPages.ModeratorPages.ModalWindows;

namespace team_project.Pages.UserPages.ModeratorPages
{
    /// <summary>
    /// Логика взаимодействия для ModeratorPage.xaml
    /// </summary>
    public partial class ModeratorPage : Page
    {
        ApiProduct api = new ApiProduct();
        ApiStatus apiStatus = new ApiStatus();
        ApiPublisher apiPublisher = new ApiPublisher();
        List<ProductImageBitMap> images;
        List<Product> products = new List<Product>(0);
        ICollectionView collectionView;
        List<ProductUpdate> productUpdates; 
        Product selected_product;
        List<Status> statuses;
        List<Status> statuses_for_update;
        List<Publisher> publishers;

        public ModeratorPage()
        {
            InitializeComponent();
            LoadInfo();
        }
        private async void Grid_Product_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            selected_product = grid.DataContext as Product;
            if (selected_product != null)
            {
                await LoadProductInfo(selected_product.ProductId);
                await LoadProductImagesInfo(selected_product.ProductId);
                await LoadProductVersionsInfo(selected_product.ProductId);
            }
        }

        public async void LoadInfo()
        {
            await LoadProducts();
            await LoadProductStatuses();
            await SetFilter();
        }

        public async Task LoadProductVersionsInfo(int product_id)
        {
            productUpdates = await api.GetProductUpdatesInfo(product_id);
            ListBox_ProductUpdates.ItemsSource = productUpdates;
        }
        public async Task LoadProducts()
        {
            products = await api.GetProducts();
            collectionView = CollectionViewSource.GetDefaultView(products);
            LoadedProducts.ItemsSource = collectionView;
        }
        public async Task LoadProductInfo(int product_id)
        {
            Product product = await api.GetProduct(product_id);
            StackPanel_ProductInfo.DataContext = product;
        }

        public async Task LoadProductImagesInfo(int product_id)
        {
            await api.LoadProductImagesWithId(product_id);
            images = await api.GetImagesWithId(product_id);
            ProductImages.ItemsSource = images;
        }
        private void TextBox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            collectionView.Refresh();
        }
        int status_id = 0;
        private async Task SetFilter()
        {
            collectionView.Filter = obj =>
            {
                Product product = obj as Product;
                bool textMatch = string.IsNullOrEmpty(TextBox_Search.Text) || product.ProductName.Contains(TextBox_Search.Text);
                bool statusMatch = status_id == 0 || product.ProductStatusId == status_id;
                return textMatch && statusMatch;
            };
        }

        private void ComboBox_ProductStatuses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_ProductStatuses.SelectedValue != null)
                status_id = (int)ComboBox_ProductStatuses.SelectedValue;
            collectionView.Refresh();
        }

        private void ComboBox_Statuses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void Button_SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selected_product == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            int selected_status = (int)ComboBox_Statuses.SelectedValue;
            if (selected_status == 15)
            {
                statusCode = await api.SetForSale(selected_product.ProductId);

            }
            else if (selected_status == 14)
            {
                statusCode = await api.SetNotForSale(selected_product.ProductId);
            }
            if (statusCode == HttpStatusCode.OK)
            {
                MessageBox.Show("Статус товара обновлён");
            }
        }

        public async Task LoadProductStatuses()
        {
            statuses = await apiStatus.GetProductStatuses();
            statuses_for_update = await apiStatus.GetProductStatuses();
            if (statuses_for_update.Count == 3)
            {
                statuses_for_update.RemoveAt(2);
            }
            ComboBox_ProductStatuses.ItemsSource = statuses;
            ComboBox_Statuses.ItemsSource = statuses_for_update;

        }

        /* public async void LoadUpdateStatuses()
         {
             statuses = await apiStatus.GetUpdateStatuses();
         }*/

        private static ProductUpdateModalWindow productUpdateModalWindow = null;

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            ProductUpdate productUpdate = grid.DataContext as ProductUpdate;
            if (productUpdate != null)
            {
                if (productUpdateModalWindow == null)
                {
                    // Если нет, создайте новый экземпляр
                    productUpdateModalWindow = new ProductUpdateModalWindow(productUpdate);
                    productUpdateModalWindow.Closed += (s, eventArgs) =>
                    {
                        productUpdateModalWindow = null;
                        LoadProductVersionsInfo(selected_product.ProductId); // Ваш метод обновления данных
                    };
                    productUpdateModalWindow.ShowDialog();
                }
                else
                {
                    // Если окно уже открыто, просто активируйте его
                    productUpdateModalWindow.Activate();
                }
            }
        }
    }
}
