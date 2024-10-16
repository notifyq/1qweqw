using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using team_project.Model;

namespace team_project.Pages.UserPages.PublisherPages.ModalWindows
{
    /// <summary>
    /// Логика взаимодействия для AddUpdateModalWindow.xaml
    /// </summary>
    public partial class AddUpdateModalWindow : Window
    {
        public static ProductUpdate new_product_update;
        public static bool isSuccess = false;
        public static string FolderPath = "";
        public static int _selected_product_id;

        public AddUpdateModalWindow(int selected_product_id)
        {
            InitializeComponent();
            if (new_product_update != null)
            {
                new_product_update = null;
            }
            if (selected_product_id == 0)
            {
                MessageBox.Show("Неизвестная ошибка");
                this.Close();
            }
            _selected_product_id = selected_product_id;
            
        }

        private void Button_Submit_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_Name.Text.Length == 0)
            {
                return;
            }
            else
            {
                new_product_update = new ProductUpdate
                {
                    ProductVersion = TextBox_Name.Text,
                    ProductId = _selected_product_id,
                };
                isSuccess = true;
                this.Close();
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            FolderPath = "";
            isSuccess = false;
            new_product_update = null; 
            this.Close();
        }

        private async void Button_SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ValidateNames = false;
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = true;
            // Это позволяет пользователю выбирать только папки, а не файлы
            dlg.FileName = "Выберите папку";
            if (dlg.ShowDialog() == true)
            {
                var folder = System.IO.Path.GetDirectoryName(dlg.FileName);
                Label_FolderPath.Content = "Директория: " + folder;
                FolderPath = folder;
                float folderSize = await CalculateFolderSize(folder);
                Label_FolderSize.Content = $"Размер: {folderSize/(1024*1024*1024)} ГБ ({folderSize} байт)";
            }
        }

        protected async static Task<float> CalculateFolderSize(string folder)
        {
            float folderSize = 0.0f;
            try
            {
                if (!Directory.Exists(folder))
                    return folderSize;
                else
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(folder))
                        {
                            if (File.Exists(file))
                            {
                                FileInfo finfo = new FileInfo(file);
                                folderSize += finfo.Length;
                            }
                        }

                        foreach (string dir in Directory.GetDirectories(folder))
                            folderSize += await CalculateFolderSize(dir);
                    }
                    catch (NotSupportedException e)
                    {
                        Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
            }
            return folderSize;
        }
    }
}
