using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using team_project.Pages.UserPages.PublisherPages.ModalWindows;
using team_project.Pages.UserPages.PublisherPages.Windows;
using static team_project.Api.ApiProduct;

namespace team_project.Pages.UserPages.PublisherPages
{
    /// <summary>
    /// Логика взаимодействия для PublisherPage.xaml
    /// </summary>
    public partial class PublisherPage : Page
    {
        ApiProduct api = new ApiProduct();
        List<ProductImageBitMap> images;
        List<Product> products = new List<Product>(0);
        ICollectionView collectionView;
        List<ProductUpdate> productUpdates;

        public PublisherPage()
        {
            InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProducts();
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
        public async Task LoadProducts()
        {
            products = await api.GetPublishedProducts();
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


        private void ProductImages_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public async Task LoadProductVersionsInfo(int product_id)
        {
            productUpdates = await api.GetProductUpdatesInfo(product_id);
            ListBox_ProductUpdates.ItemsSource = productUpdates;
        }

        private void GetReport_Click(object sender, RoutedEventArgs e)
        {
            GetReportModalWindow modalWindow = new GetReportModalWindow();
            modalWindow.ShowDialog();
        }

        private void UploadPage_Click(object sender, RoutedEventArgs e)
        {
            UploadWindow uploadWindow = new UploadWindow();
            uploadWindow.Show();
        }

        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProductModalWindow modalWindow = new AddProductModalWindow();
            modalWindow.ShowDialog();
            ProductAdd new_product = AddProductModalWindow.new_product;
            if (new_product != null && AddProductModalWindow.isSuccess)
            {
                await api.AddNewProduct(new_product);
                await LoadProducts();
                CollectionViewSource.GetDefaultView(products).Refresh();
            }
        }
        private void TextBox_Price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверка на ввод только чисел с плавающей точкой
            decimal result;
            bool isValid = Decimal.TryParse(e.Text, out result);
            e.Handled = !isValid;
        }
        private async void Button_SaveChanges_Click(object sender, RoutedEventArgs e)
        {

            if (selected_product == null)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Вы уверены, что действительно хотите обновить информацию о товаре?", "Подтверждение изменения данных", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (!Decimal.TryParse(TextBox_Price.Text, out decimal price))
                {
                    MessageBox.Show("Введенная стоимость не является числом");
                    return;
                }
                List<int> Genres = new List<int>();
                if (GenreIds == null)
                {
                    Genres = selected_product.ProductGenres.Select(pg => pg.GenreId).ToList();
                }
                else
                {
                    Genres = GenreIds;
                }
                ProductData productData = new ProductData()
                {
                    ProductId = selected_product.ProductId,
                    ProductName = TextBox_ProductName.Text,
                    ProductDescription = TextBox_ProductDescription.Text,
                    ProductPrice = price,
                    GenreIds = Genres,
                };
                await api.SaveProductInfo(productData);
                await LoadProducts();
                await LoadProductInfo(selected_product.ProductId);

            }
        }

        private async void UploadUpdate_Click(object sender, RoutedEventArgs e)
            {
            if (selected_product == null)
            {
            }
            AddUpdateModalWindow modalWindow = new AddUpdateModalWindow(selected_product.ProductId);
            modalWindow.ShowDialog();

            if (!Directory.Exists(AddUpdateModalWindow.FolderPath))
            {
                return;
            }
            ProductUpdate productUpdate = AddUpdateModalWindow.new_product_update;
            if (productUpdate != null && AddUpdateModalWindow.isSuccess)
            {
                await UploadService.Instance.StartUpload(productUpdate, AddUpdateModalWindow.FolderPath);
            }
            
        }

        private async void DeleteUpdate_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ProductUpdate productUpdate = menuItem.DataContext as ProductUpdate;
            if (productUpdate == null)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show($"Вы уверены, что действительно хотите удалить эту версию {productUpdate.ProductVersion} этого товара?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await api.DeleteUpdate(productUpdate.ProductId);
            }
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            Product product = menuItem.DataContext as Product;
            if (product == null)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show("Вы уверены, что действительно хотите отправить в архив этот товар?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await api.ArchiveProduct(product.ProductId);
            }
        }

        private async void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ProductImageBitMap image = menuItem.DataContext as ProductImageBitMap;
            if (image != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить это изображение?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    HttpStatusCode statusCode = await api.DeleteProductImage(image.ImageId);
                    if (statusCode == HttpStatusCode.OK)
                    {
                        images.Remove(image);
                        await LoadProductImagesInfo(selected_product.ProductId);
                    }
                    

                }
            }
        }
        Product selected_product;
        private async void AddImage_Click(object sender, RoutedEventArgs e)
        {
            if (selected_product == null)
            {
                return;
            }

            // Открыть диалог выбора файла
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // Загрузить изображение
                HttpStatusCode statusCode = await api.UploadImage(selected_product.ProductId, openFileDialog.FileName);
                if (statusCode == HttpStatusCode.Created)
                {
                    MessageBox.Show("Изображение добавлено");
                    await LoadProductImagesInfo(selected_product.ProductId);
                    return;
                }
            }
        }

        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string searchText = textBox.Text;
                collectionView.Filter = product => ((Product)product).ProductName.Contains(searchText);
            }
        }
        List<int> GenreIds;
        private void Button_EditProductGenres_Click(object sender, RoutedEventArgs e)
        {
            if (selected_product == null)
            {
                MessageBox.Show("Товар не выбран");
                return;
            }
            EditProductGenresModalWindow modalWindow = new EditProductGenresModalWindow(selected_product);
            modalWindow.ShowDialog();
            GenreIds = EditProductGenresModalWindow.SelectedGenres;
            EditProductGenresModalWindow.SelectedGenres = null;

        }
    }
}
