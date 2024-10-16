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
using team_project.Api;

namespace team_project.Pages.UserPages.PublisherPages.Windows
{
    /// <summary>
    /// Логика взаимодействия для UploadWindow.xaml
    /// </summary>
    public partial class UploadWindow : Window
    {
        private readonly UploadService _uploadService = UploadService.Instance;

        public UploadWindow()
        {
            InitializeComponent();
            _uploadService.ProgressChanged += UpdateProgress;
        }

        private void UpdateProgress(double progress)
        {
            ProgressBar_DownloadPercent.Value = progress;
            UploadSpeedTextBlock.Text = $"Скорость: {_uploadService.UploadSpeed} МБ/c";
            UploadStatusTextBlock.Text = $"Статус: {_uploadService.UploadStatus}";
            UploadedSizeTextBlock.Text = $"Отправлено {_uploadService.TotalSize / (1024 * 1024 * 1024)} ГБ из {_uploadService.TotalSize / (1024 * 1024 * 1024)} ГБ (отправлено {_uploadService.TotalSize} байт)";
        }

        private void CancelUploadButton_Click(object sender, RoutedEventArgs e)
        {
            _uploadService.CancelUpload();
        }
    }
}
