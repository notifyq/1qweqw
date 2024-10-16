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
using team_project.Model;
using Xceed.Words.NET;
using Xceed.Document.NET;

namespace team_project.Pages.UserPages.PublisherPages.ModalWindows
{
    /// <summary>
    /// Логика взаимодействия для GetReportModalWindow.xaml
    /// </summary>
    public partial class GetReportModalWindow : Window
    {
        Api.ApiProduct api = new Api.ApiProduct();
        List<Product> publisherProducts = new List<Product>(0);
        Publisher currentPublisher = new Publisher();

        public GetReportModalWindow()
        {
            InitializeComponent();
            LoadProducts();
            LoadPublisherInfo();
        }

        public async void LoadProducts()
        {
            publisherProducts = await api.GetPublishedProducts();
            ListBox_Products.ItemsSource = publisherProducts;
        }

        public async void LoadPublisherInfo()
        {
            currentPublisher = await api.GetCurrentPublisherInfo();
        }

        public void CreateReport(Publisher publisher, DateTime startDate, DateTime endDate, List<Product> selectedProducts)
        {
            try
            {

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                string fileName = $"Отчет по продажам от {startDate:yyyyMMdd} до {endDate:yyyyMMdd}.docx";
                string fullPath = System.IO.Path.Combine(documentsPath, fileName);

                using (DocX document = DocX.Create(fullPath))
                {
                    // Заголовок отчета
                    document.InsertParagraph($"Издатель: {publisher.PublisherName}")
                        .FontSize(20)
                        .Alignment = Alignment.left;

                    // Даты отчета
                    document.InsertParagraph($"Выборка даты")
                        .FontSize(14)
                        .Alignment = Alignment.left;
                    document.InsertParagraph($"От: {startDate.ToShortDateString()}")
                        .FontSize(14)
                        .Alignment = Alignment.left;

                    document.InsertParagraph($"До: {endDate.ToShortDateString()}")
                       .FontSize(14)
                       .Alignment = Alignment.left;

                    // Выбранные товары
                  /*  document.InsertParagraph("Выбранные товары:")
                        .FontSize(14);
                    foreach (var product in selectedProducts)
                    {
                        document.InsertParagraph($"- {product.ProductName} (Идентификатор: {product.ProductId})");
                    }*/

                    // Таблица продаж
                    var table = document.AddTable(selectedProducts.Count + 1, 3);
                    table.Rows[0].Cells[0].Paragraphs.First().Append("Наименование товара");
                    table.Rows[0].Cells[1].Paragraphs.First().Append("Доходы от продаж (руб.)");
                    table.Rows[0].Cells[2].Paragraphs.First().Append("Кол-во продаж");

                    decimal totalIncome = 0;
                    int totalSales = 0;

                    for (int i = 0; i < selectedProducts.Count; i++)
                    {
                        var product = selectedProducts[i];
                        var sales = product.PurchaseLists.Where(pl => pl.Purchase.PurchaseDate >= startDate && pl.Purchase.PurchaseDate <= endDate);
                        decimal income = sales.Sum(s => s.ProductSpentMoney);
                        int salesCount = sales.Count();

                        string product_with_id = $"{product.ProductName} (Идентификатор: {product.ProductId})";

                        table.Rows[i + 1].Cells[0].Paragraphs.First().Append(product_with_id);
                        table.Rows[i + 1].Cells[1].Paragraphs.First().Append(income.ToString());
                        table.Rows[i + 1].Cells[2].Paragraphs.First().Append(salesCount.ToString());

                        totalIncome += income;
                        totalSales += salesCount;
                    }

                    document.InsertTable(table);

                    // Общий доход
                    document.InsertParagraph($"ОБЩИЙ ДОХОД: {totalIncome} руб.")
                        .FontSize(14)
                        .Alignment = Alignment.right;

                    document.Save();
                    MessageBox.Show($"Файл {fileName} отправлен в директорию 'Документы'");
                }
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("Документ используется другим процессом");
            }

            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

        }

        private void Button_Create_Click(object sender, RoutedEventArgs e)
        {
            if (!DatePicker_From.SelectedDate.HasValue || !DatePicker_To.SelectedDate.HasValue)
            {
                MessageBox.Show("Неверная дата");
                return;
            }
            var selectedProducts = publisherProducts.Where(p => p.IsSelected).ToList();

            if (selectedProducts.Count == 0)
            {
                MessageBox.Show("Товары не выбраны");
                return;
            }
            var startDate = DatePicker_From.SelectedDate.Value;
            var endDate = DatePicker_To.SelectedDate.Value;
            CreateReport(currentPublisher, startDate, endDate, selectedProducts);

        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
