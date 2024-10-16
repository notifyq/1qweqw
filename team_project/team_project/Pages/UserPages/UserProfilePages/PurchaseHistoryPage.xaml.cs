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

namespace team_project.Pages.UserPages.UserProfilePages
{
    /// <summary>
    /// Логика взаимодействия для PurchaseHistoryPage.xaml
    /// </summary>
    public partial class PurchaseHistoryPage : Page
    {
        ApiUser api = new ApiUser();
        List<Purchase> purchases = new List<Purchase>();
        public PurchaseHistoryPage()
        {
            InitializeComponent();
            LoadPurchases();
        }

        public async void LoadPurchases()
        {
            purchases = await api.GetPurchases();
            if (purchases.Count == 0)
            {
                MessageBox.Show("История покупок пуста");
                return;
            }
            SetPurchases(purchases);
        }

        public async void SetPurchases(List<Purchase> purchases)
        {
            var purchaseHistory = purchases.Select(p => new
            {
                PurchaseInfo =
                $"Покупка №{p.PurchasesId} " +
                $"Дата: {p.PurchaseDate} " +
                $"Статус: {p.PurchaseStatusNavigation.StatusName} ",
                Products = p.PurchaseLists.Select(pl => new
                {
                    ProductInfo = $"Товар: {pl.Product.ProductName}, Цена: {pl.ProductSpentMoney} руб.",
                    Key = pl.Key
                })
            });

            ListView_PurchaseHistory.ItemsSource = purchaseHistory;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            var selectedProduct = (dynamic)menuItem.DataContext;
            if (selectedProduct != null) 
            {
                string key = selectedProduct.Key;
                Clipboard.SetText(selectedProduct.Key);
                MessageBox.Show("Ключ скопирован в буфер обмена");
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            var selectedProduct = (dynamic)grid.DataContext;
            if (selectedProduct != null)
            {
                string key = selectedProduct.Key;
                Clipboard.SetText(selectedProduct.Key);
                MessageBox.Show("Ключ скопирован в буфер обмена");
            }
        }
    }
}
