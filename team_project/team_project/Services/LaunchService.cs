using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using team_project.Model;
using static team_project.Api.DownloadService;

namespace team_project.Services
{
    public class LaunchService
    {
        public static LaunchService Instance { get; } = new LaunchService(); // экземпляр класса
        private Process _process; // Процесс для запуска и остановки приложения

        private LaunchService() { }

        public bool IsRunning => _process?.HasExited == false; // Проверка на то что игра запущена

        public void Start(int productId)
        {
            if (IsRunning)
            {
                MessageBox.Show("Процесс уже запущен");
            }
            else
            {
                try
                {
                    // Получите информацию о продукте
                    var productsInfoJson = Properties.Settings.Default["ProductsInfo"] as string;
                    var productsInfo = string.IsNullOrEmpty(productsInfoJson)
                        ? new List<ProductInfo>()
                        : JsonConvert.DeserializeObject<List<ProductInfo>>(productsInfoJson);
                    var productInfo = productsInfo.FirstOrDefault(pi => pi.ProductId == productId);

                    if (productInfo == null || productInfo.ExeFiles.Count == 0)
                    {
                        MessageBox.Show("Исполняемый файл продукта не найден");
                        return;
                    }
                                        // Запустите первый исполняемый файл продукта
                    _process = Process.Start(productInfo.ExeFiles[0]);
                    Console.WriteLine("Запуск процесса");
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                    Console.WriteLine($"Ошибка при запуске процесса: {ex.Message}");
                    throw;
                }
            }
        }

        public void Close()
        {
            if (!IsRunning)
            {
                MessageBox.Show("Процесс не найден");
            }
            else
            {
                try
                {
                    _process.Kill();
                    _process = null;
                    Console.WriteLine("Завершение работы процесса");
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                    Console.WriteLine($"Ошибка при завершении процесса: {ex.Message}");
                    throw;
                }
            }
        }

        public void DeleteProductFiles(int product_id)
        {
            string projectDirectory = System.IO.Directory.GetCurrentDirectory();
            string productDirectory = System.IO.Path.Combine(projectDirectory, $"Product_{product_id}");

            if (Directory.Exists(productDirectory))
            {
                try
                {
                    Directory.Delete(productDirectory, true);
                    Console.WriteLine("Удаление файлов продукта");
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                    Console.WriteLine($"Ошибка при удалении файлов продукта: {ex.Message}");
                    throw;
                }
            }
            else
            {
                MessageBox.Show("Директория продукта не найдена");
            }

            // Удаление информации о товаре из Properties.Settings.ProductsInfo
            var productsInfoJson = Properties.Settings.Default["ProductsInfo"] as string;
            if (!string.IsNullOrEmpty(productsInfoJson))
            {
                var productsInfo = JsonConvert.DeserializeObject<List<ProductInfo>>(productsInfoJson);
                var productInfo = productsInfo.FirstOrDefault(pi => pi.ProductId == product_id);
                if (productInfo != null)
                {
                    productsInfo.Remove(productInfo);
                    Properties.Settings.Default["ProductsInfo"] = JsonConvert.SerializeObject(productsInfo);
                    Properties.Settings.Default.Save();
                    Console.WriteLine("Удаление информации о товаре");
                }
            }
        }

    }
}
