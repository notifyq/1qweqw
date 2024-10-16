using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using team_project.Model;

namespace team_project.Api
{
    public class UploadService : INotifyPropertyChanged
    {
        public static UploadService Instance { get; } = new UploadService();
        private UploadService() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private double _uploadSpeed;
        public double UploadSpeed
        {
            get { return _uploadSpeed; }
            set
            {
                if (_uploadSpeed != value)
                {
                    _uploadSpeed = value;
                    OnPropertyChanged(nameof(UploadSpeed));
                }
            }
        }
        private double _uploadedSize;
        public double UploadedSize
        {
            get { return _uploadedSize; }
            set
            {
                if (_uploadedSize != value)
                {
                    _uploadedSize = value;
                    OnPropertyChanged(nameof(UploadedSize));
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
                }
            }
        }

        private string _uploadStatus;
        public string UploadStatus
        {
            get { return _uploadStatus; }
            set
            {
                if (_uploadStatus != value)
                {
                    _uploadStatus = value;
                    OnPropertyChanged(nameof(UploadStatus));
                }
            }
        }

        private CancellationTokenSource _cancellationTokenSource;
        private readonly ApiProduct _api = new ApiProduct();

        private DateTime _UploadStartTime;
        private Stopwatch _stopwatch = new Stopwatch();
        private long _lastBytesTransferred = 0;

        public event Action<double> ProgressChanged;

        public async Task StartUpload(ProductUpdate productUpdate, string folderPath)
        {
            Console.WriteLine("Запуск загрузки...");
            if (_cancellationTokenSource != null)
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    MessageBox.Show("Загрузка уже выполняется");
                    return;
                }
            }
            _cancellationTokenSource = new CancellationTokenSource();

            var progress = new Progress<double>(value =>
            {
                ProgressChanged?.Invoke(value);
            });

            await UploadProductFiles(productUpdate.ProductId, productUpdate.ProductVersion, folderPath, progress, _cancellationTokenSource.Token);
        }

        public async Task UploadProductFiles(int product_id, string updateVersion, string folderPath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            Console.WriteLine("Начало загрузки");

            _UploadStartTime = DateTime.Now;
            _cancellationTokenSource = new CancellationTokenSource();
            TotalSize = await CalculateFolderSize(folderPath);

            try
            {
                // Загрузите файл
                Console.WriteLine("Отправка запроса");

                await _api.UploadProductFiles(product_id, updateVersion, folderPath, progress, cancellationToken);


                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    UploadStatus = "Загрузка завершена";
                    MessageBox.Show("Добавление версии продукта завершено");
                    Console.WriteLine("Успешное завершение загрузки");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Отмена загрузки");

                UploadStatus = "Загрузка отменена";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);

                UploadStatus = $"Ошибка при загрузке: {ex.Message}";
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }
            }
        }

        public void CancelUpload()
        {

            _cancellationTokenSource?.Cancel();
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
