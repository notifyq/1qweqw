using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
using System.Windows.Threading;
using team_project.Api;
using team_project.Model;

namespace team_project.Pages.UserPages
{
    /// <summary>
    /// Логика взаимодействия для DownloadPage.xaml
    /// </summary>
    public partial class DownloadPage : Page
    {
        private readonly static int product_id;
        private readonly ApiProduct _api = new ApiProduct();
        private readonly DispatcherTimer _uiUpdateTimer = new DispatcherTimer();
        private double _downloadProgress;
        public DownloadPage()
        {
            InitializeComponent();

            if (DownloadService.Instance.CurrentProductId != 0)
            {
                Button_Cancel.Visibility = Visibility.Visible;
                Button_Pause.Visibility = Visibility.Visible;
                if (DownloadService.Instance._isPaused)
                {
                    Button_Pause.Content = "Возобновить загрузку";

                }
                else
                {
                    Button_Pause.Content = "Остановить загрузку";
                }
            }
            else
            {
                Button_Cancel.Visibility = Visibility.Collapsed;
                Button_Pause.Visibility = Visibility.Collapsed;
                DownloadService.Instance.DownloadStatus = "Ожидание начала загрузки";
                DownloadService.Instance.TotalSize = 0;
                DownloadService.Instance.DownloadedSize = 0;
                DownloadService.Instance.DownloadSpeed = 0;
            }
           

            // Подписка на событие изменения прогресса
            /*            DownloadService.Instance.ProgressChanged += value =>
                        {
                            _downloadProgress = value;
                        };*/
            /*            _uiUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
                        _uiUpdateTimer.Tick += UiUpdateTimer_Tick;
                        _uiUpdateTimer.Start();*/
        }
        private async void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            ProgressBar_DownloadPercent.Value = _downloadProgress * 100;
            TextBlock_ProgressBarInfo.Text = $"Загрузка {_downloadProgress:P0}";
            TextBlock_DownloadSpeed.Text = "Скорость загрузки: " + DownloadService.Instance.DownloadSpeed.ToString() + " МБ/сек";
            TextBlock_Downloaded.Text = DownloadService.Instance.DownloadedSize.ToString() + " ГБ";
            TextBlock_NeedDownload.Text = " из " + DownloadService.Instance.TotalSize.ToString() + " ГБ";
        }

        private async void Button_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (DownloadService.Instance._isPaused)
            {
                Button_Pause.Content = "Остановить загрузку";
                await DownloadService.Instance.ResumeDownload(DownloadService.Instance.CurrentProductId);
            }
            else
            {
                Button_Pause.Content = "Возобновить загрузку";
                await DownloadService.Instance.PauseDownload();
            }
        }

        private async void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            await DownloadService.Instance.StopDownload();
/*            NavigationService.GoBack();
*/        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Product product = await _api.GetProduct(DownloadService.Instance.CurrentProductId);
            Grid_ProductInfo.DataContext = product;
        }

        private async void Image_productImage_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = (Image)sender;

            Product product = await _api.GetProduct(DownloadService.Instance.CurrentProductId);

            try
            {
                if (product == null)
                {
                    img.Source = (BitmapImage)System.Windows.Application.Current.TryFindResource("DefaultImage");
                    return;
                }
                List<BitmapImage> images = new List<BitmapImage>(0);
                string cacheFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CodeFlow", product.ProductId.ToString(), "ImageCache");
                if (Directory.Exists(cacheFolder))
                {
                    images = await _api.GetImages(product.ProductId);
                    if (images.Count > 0)
                    {
                        img.Source = images[0];
                    }
                    else
                    {
                        await _api.LoadProductImages(product.ProductId);
                        images = await _api.GetImages(product.ProductId);
                        if (images.Count > 0)
                        {
                            img.Source = images[0];
                        }
                        else
                        {
                            img.Source = (BitmapImage)System.Windows.Application.Current.TryFindResource("DefaultImage");
                        }
                    }
                }
                else
                {
                    await _api.LoadProductImages(product.ProductId);
                    images = await _api.GetImages(product.ProductId);
                    if (images.Count > 0)
                    {
                        img.Source = images[0];
                    }
                    else
                    {
                        img.Source = (BitmapImage)System.Windows.Application.Current.TryFindResource("DefaultImage");
                    }

                }


            }
            catch (Exception)
            {
                img.Source = (BitmapImage)System.Windows.Application.Current.TryFindResource("DefaultImage");
            }

        }
    }
}
