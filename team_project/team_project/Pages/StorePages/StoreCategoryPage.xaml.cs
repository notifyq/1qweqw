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
using team_project.Model;

namespace team_project.Pages.StorePages
{
    /// <summary>
    /// Логика взаимодействия для StoreCategoryPage.xaml
    /// </summary>
    public partial class StoreCategoryPage : Page
    {
        private string CategoryName;

        public StoreCategoryPage(string CategoryName)
        {
            InitializeComponent();
            this.CategoryName = CategoryName;
        }

        private void GridSingleProduct_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Product selected_product = new Product();
            NavigationService.Navigate(new ProductPage(selected_product));
        }
    }
}
