using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static team_project.Api.ApiProduct;

namespace team_project.Api
{
    public static class StreamExtensions
    {
        private static readonly object _lock = new object();

        public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<double> progress, CancellationToken cancellationToken, DateTime _downloadStartTime, long downloadedBytes, long totalBytes, int product_id)
        {
            var buffer = new byte[81920];
            int bytesRead;
            var totalRead = 0L;
            double previousProgress = 0;
            int updateCounter = 0; // Счетчик для обновления прогресса и скорости загрузки
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                totalRead += bytesRead;
                updateCounter += bytesRead;

                if (updateCounter >= 1024 * 1024) // Обновление каждый мегабайт
                {
                    // Запуск новой задачи для выполнения расчетов
                    await Task.Run(async () =>
                    {
                        // Сохранение загруженных байт
                        await SaveDownloadedBytes(product_id, totalRead);

                        lock (_lock)
                        {
                            // Расчет скорости загрузки
                            var elapsedTime = DateTime.Now - _downloadStartTime;
                            var downloadSpeed = elapsedTime.TotalSeconds > 0 ? (totalRead - downloadedBytes) / elapsedTime.TotalSeconds : 0;
                            downloadSpeed = downloadSpeed / (1024 * 1024);
                            DownloadService.Instance.DownloadSpeed = Math.Round(downloadSpeed, 2);

                            if (totalBytes > 0)
                            {
                                var progressValue = (double)(totalRead - downloadedBytes) / totalBytes;
                                if (Math.Abs(progressValue - previousProgress) >= 0.01)
                                {
                                    progress.Report(progressValue);
                                    previousProgress = progressValue;
                                    DownloadService.Instance.DownloadedSize = (Math.Round(totalRead / (1024.0 * 1024.0 * 1024.0), 2)); // Сколько загружено в Гигабайтах
                                    DownloadService.Instance.DownloadStatus = "Загрузка...";
                                }
                            }
                        }
                    });

                    updateCounter = 0; // Сброс счетчика
                }
            }
        
    }

        private static async Task SaveDownloadedBytes(int productId, long downloadedBytes)
        {
            var settingName = "DownloadInfo";
            var settingValue = Properties.Settings.Default[settingName];
            List<DownloadInfo> downloadInfos;
            if (settingValue != null)
            {
                // Если настройка существует, десериализуйте ее из JSON
                downloadInfos = JsonConvert.DeserializeObject<List<DownloadInfo>>((string)settingValue);
            }
            else
            {
                // Если настройки не существует, создайте новый список
                downloadInfos = new List<DownloadInfo>();
            }

            // Найдите информацию о загрузке для данного продукта или создайте новую
            var downloadInfo = downloadInfos.FirstOrDefault(di => di.ProductId == productId);
            if (downloadInfo == null)
            {
                downloadInfo = new DownloadInfo { ProductId = productId };
                downloadInfos.Add(downloadInfo);
            }

            // Обновите количество загруженных байтов
            downloadInfo.DownloadedBytes = downloadedBytes;

            // Сериализуйте информацию о загрузке в JSON и сохраните ее
            settingValue = JsonConvert.SerializeObject(downloadInfos);
            Properties.Settings.Default[settingName] = settingValue;
            Properties.Settings.Default.Save();
        }
    }
}
