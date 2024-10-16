using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using static team_project.Api.ApiProduct;
using System.ComponentModel;
using team_project.Model;
using System.Windows.Forms;
using team_project.Pages.StorePages;

namespace team_project.Api
{
    public class DownloadService : INotifyPropertyChanged
    { // Статический экземпляр класса
        public static DownloadService Instance { get; } = new DownloadService();
        public int CurrentProductId
        {
            get
            {
                if (Properties.Settings.Default["CurrentProductId"] != null)
                {
                    return (int)Properties.Settings.Default["CurrentProductId"];
                }
                return 0;
            }
            set
            {
                Properties.Settings.Default["CurrentProductId"] = value;
                Properties.Settings.Default.Save();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private double _downloadSpeed;
        public double DownloadSpeed
        {
            get { return _downloadSpeed; }
            set
            {
                if (_downloadSpeed != value)
                {
                    _downloadSpeed = value;
                    OnPropertyChanged(nameof(DownloadSpeed));
                }
            }
        }
        private double _downloadedSize;
        public double DownloadedSize
        {
            get { return _downloadedSize; }
            set
            {
                if (_downloadedSize != value)
                {
                    _downloadedSize = value;
                    OnPropertyChanged(nameof(DownloadedSize));
                    OnPropertyChanged(nameof(DownloadProgress));
                }
            }
        }

        public double DownloadProgress
        {
            get
            {
                if (TotalSize == 0)
                {
                    return 0;
                }
                else
                {
                    double progress = (DownloadedSize / TotalSize) * 100;
                    return Math.Round(progress, 2);
                }
            }
        }

        private double _totalSize;
        public double TotalSize
        {
            get { return _totalSize; }
            set
            {
                if (_totalSize != value)
                {
                    _totalSize = value;
                    OnPropertyChanged(nameof(TotalSize));
                    OnPropertyChanged(nameof(DownloadProgress));
                }
            }
        }

        private string _downloadStatus;
        public string DownloadStatus
        {
            get { return _downloadStatus; }
            set
            {
                if (_downloadStatus != value)
                {
                    _downloadStatus = value;
                    OnPropertyChanged(nameof(DownloadStatus));
                }
            }
        }
        public string ProductPath
        {
            get
            {
                if (Properties.Settings.Default["ProductPath"] != null)
                {
                    return Properties.Settings.Default["ProductPath"].ToString();
                }
                return string.Empty;
            }
            set
            {
                Properties.Settings.Default["ProductPath"] = value;
                Properties.Settings.Default.Save();
            }
        }

        private DateTime _downloadStartTime;


        // Закрытый конструктор, чтобы предотвратить создание других экземпляров
        private DownloadService() { }

        private CancellationTokenSource _cancellationTokenSource;
        private readonly ApiProduct _api = new ApiProduct();
        

        public event Action<double> ProgressChanged;

        public bool _isPaused = false;

        public async Task ChooseDownloadPath()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                ProductPath = folderBrowserDialog.SelectedPath;
            }
        }

        public async Task PauseDownload()
        {
            if (_cancellationTokenSource != null && !_isPaused)
            {
                Console.WriteLine("Приостановка загрузки...");

                _cancellationTokenSource.Cancel();
                _isPaused = true;
            }
        }

        public async Task ResumeDownload(int product_id)
        {
            if (_isPaused)
            {
                Console.WriteLine("Возобновление загрузки");

                _cancellationTokenSource = new CancellationTokenSource();
                _isPaused = false;
                await StartDownload(product_id);
            }
        }

        public async Task StartDownload(int product_id)
        {
            try
            {

                if (_cancellationTokenSource != null)
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        System.Windows.MessageBox.Show("Загрузка уже выполняется");
                        return;
                    }
                }
                if (ProductPath == "")
                {
                    await ChooseDownloadPath();
                }

                var progress = new Progress<double>(value =>
                {
                    ProgressChanged?.Invoke(value);
                });
                CurrentProductId = product_id;

                Console.WriteLine("Начало загрузки...");
                Console.WriteLine("Идентификатор товара: " + product_id);

                await DownloadProductFiles(product_id, progress);
            }
            catch (Exception ex)
            {
                CurrentProductId = 0;
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource?.Cancel();
                }
                    Console.WriteLine($"Произошла ошибка при начале загрузки: {ex.Message}");
            }
        }

        public async Task DownloadProductFiles(int product_id, IProgress<double> progress)
        {

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                Console.WriteLine("Привязка значений для отображения информации и статусе загрузки");

                _downloadStartTime = DateTime.Now;

                CurrentProductId = product_id;

                var productUpdate = await _api.DownloadProductFiles(product_id, progress, _cancellationTokenSource.Token, _downloadStartTime).ConfigureAwait(false);

                ProgressChanged?.Invoke(1);
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Console.WriteLine("Разархивирование файла");

                    UnzipAndSaveProduct(product_id, productUpdate);
                }
            }
            catch (OperationCanceledException)
            {
                await RemoveDownloadedBytesProgressInfo(CurrentProductId);
                CurrentProductId = 0;
                ProductPath = "";
                ProgressChanged?.Invoke(0);
            }
            catch (Exception ex)
            {
                if (_cancellationTokenSource != null)
                {
                    RemoveDownloadedBytesProgressInfo(CurrentProductId);
                    CurrentProductId = 0;
                    ProductPath = "";
                    _cancellationTokenSource?.Cancel();
                }
            }
            
        }
        private async Task RemoveDownloadedBytesProgressInfo(int productId)
        {
            var settingName = "DownloadInfo";
            var settingValue = Properties.Settings.Default[settingName];
            List<DownloadInfo> downloadInfos;
            if (settingValue != null)
            {
                downloadInfos = JsonConvert.DeserializeObject<List<DownloadInfo>>((string)settingValue);
            }
            else
            {
                downloadInfos = new List<DownloadInfo>();
            }

            var downloadInfo = downloadInfos.FirstOrDefault(di => di.ProductId == productId);
            if (downloadInfo == null)
            {
                return;
            }

            downloadInfo.DownloadedBytes = 0;

            settingValue = JsonConvert.SerializeObject(downloadInfos);
            Properties.Settings.Default[settingName] = settingValue;
            Properties.Settings.Default.Save();
        }
        public async Task StopDownload()
        {
            try
            {
                if (_cancellationTokenSource != null)
                {
                    Console.WriteLine("Отмена загрузки...");

                    _cancellationTokenSource.Cancel();
                }
                Console.WriteLine("Удаление информации о количестве загруженных байт");

                await RemoveDownloadedBytesProgressInfo(CurrentProductId);
                await Task.Delay(5000);

                Console.WriteLine("Удаление архива...");

                await DeleteDownloadedFiles(CurrentProductId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при остановке загрузки: {ex.Message}");
            }
            finally 
            { 
                CurrentProductId = 0;
            }
        }
        private async Task DeleteDownloadedFiles(int productId)
        {
            string zipFilePath = $"{productId}.zip";
            string projectDirectory = System.IO.Directory.GetCurrentDirectory();
            string zipFilePathDirectory = System.IO.Path.Combine(projectDirectory, $"{zipFilePath}");
            if (System.IO.File.Exists(zipFilePathDirectory))
            {
                System.IO.File.Delete(zipFilePathDirectory);
            }

        }

        

        private async Task UnzipAndSaveProduct(int productId, ProductUpdate productUpdate)
        {
            if (_cancellationTokenSource != null)
            {
                string zipFilePath = $"{productId}.zip";
                string projectDirectory = System.IO.Directory.GetCurrentDirectory();
                string productDirectory = System.IO.Path.Combine(ProductPath, $"Product_{productId}");


                if (System.IO.File.Exists(productDirectory))
                {
                    System.IO.File.Delete(productDirectory);
                }
                Directory.CreateDirectory(productDirectory);

                try
                {
                    ZipFile.ExtractToDirectory(zipFilePath, productDirectory);

                    var productsInfoJson = Properties.Settings.Default["ProductsInfo"] as string;
                    var productsInfo = string.IsNullOrEmpty(productsInfoJson)
                        ? new List<ProductInfo>()
                        : JsonConvert.DeserializeObject<List<ProductInfo>>(productsInfoJson);

                    var productInfo = productsInfo.FirstOrDefault(pi => pi.ProductId == productId);
                    if (productInfo == null)
                    {
                        productInfo = new ProductInfo { ProductId = productId };
                        productsInfo.Add(productInfo);
                    }
                    productInfo.UpdateInfo = productUpdate;

                    string[] exeFiles = Directory.GetFiles(productDirectory, "*.exe", SearchOption.AllDirectories);
                    productInfo.ExeFiles = exeFiles.ToList();

                    productsInfoJson = JsonConvert.SerializeObject(productsInfo);
                    Properties.Settings.Default["ProductsInfo"] = productsInfoJson;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
                finally
                {
                    await RemoveDownloadedBytesProgressInfo(CurrentProductId);
                    CurrentProductId = 0;
                    _isPaused = true;
                    if (_cancellationTokenSource != null)
                    {
                        ProductPath = "";
                        System.Windows.MessageBox.Show("Загрузка завершена");
                        DownloadStatus = "Загрузка завершена";
                        _cancellationTokenSource?.Cancel();
                    }
                }
            }
        }

        public class ProductInfo
        {
            public int ProductId { get; set; }
            public List<string> ExeFiles { get; set; }
            public ProductUpdate UpdateInfo { get; set; }
        }
    }
}
