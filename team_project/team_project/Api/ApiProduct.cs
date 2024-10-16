using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using team_project.Model;
using Xceed.Wpf.Toolkit;

namespace team_project.Api
{
    internal class ApiProduct : Api
    {
        public async Task<List<Product>> GetProducts()
        {
            List<Product> products = new List<Product>();

            if (Properties.Settings.Default.Token == null)
            {
                notificationManager.Show(title: "Аутентификация", message: ERROR_not_authorized, NotificationType.Error);
                return products;
            }

            var response = await client.GetAsync($"Product/ProductList");
            current_status = response.StatusCode;
            if (current_status == HttpStatusCode.OK)
            {
                products = JsonConvert.DeserializeObject<List<Product>>(response.Content.ReadAsStringAsync().Result);
            }

            return products;
        }
        public async Task<Publisher> GetCurrentPublisherInfo()
        {
            Publisher publisher = new Publisher();
            var response = await client.GetAsync($"Publisher/CurrentPublisher");
            publisher = JsonConvert.DeserializeObject<Publisher>(response.Content.ReadAsStringAsync().Result);
            return publisher;
        }

        public async Task<HttpStatusCode> ChangeUpdateStatus(int update_id, int status_id)
        {
            string URL = $"Product/Updates/{update_id}/{status_id}";
            var response = await client.PutAsync(URL, null);
            return response.StatusCode;
        }
        public async Task<HttpStatusCode> ArchiveProduct(int product_id)
        {
            string URL = $"Product/SetArchive/{product_id}";
            var response = await client.PutAsync(URL, null);
            return response.StatusCode;
        }
        public async Task<HttpStatusCode> SetForSale(int product_id)
        {
            string URL = $"Product/SetForSale/{product_id}";
            var response = await client.PutAsync(URL, null);
            return response.StatusCode;
        }
        public async Task<HttpStatusCode> SetNotForSale(int product_id)
        {
            string URL = $"Product/SetNotForSale/{product_id}";
            var response = await client.PutAsync(URL, null);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteProductImage(int image_id)
        {
            string URL = $"ProductImages/{image_id}";
            var response = await client.DeleteAsync(URL);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteUpdate(int update_id)
        {
            string URL = $"Product/Updates/{update_id}/";
            var response = await client.DeleteAsync(URL);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> SaveProductInfo(ProductData productData)
        {
            string URL = $"Product/UpdateProductInfo";
            var json = JsonConvert.SerializeObject(productData);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(URL, data);
            return response.StatusCode;
        }
        public class ProductData
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductDescription { get; set; }
            public decimal ProductPrice { get; set; }
            public List<int> GenreIds { get; set; }

        }
        public async Task<List<ProductUpdate>> GetProductUpdatesInfo(int product_id)
        {
            try
            {
                List<ProductUpdate> productUpdates = new List<ProductUpdate>();
                Console.WriteLine("Отправка GET запроса на получение списка обновлений товара");
                var response = await client.GetAsync($"Product/Updates/{product_id}");
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                productUpdates = JsonConvert.DeserializeObject<List<ProductUpdate>>(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine(response.StatusCode);
                return productUpdates;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<Product>> GetPublisherProducts(int publisher_id)
        {
            List<Product> products = new List<Product>();
            var response = await client.GetAsync($"Publisher/{publisher_id}");
            products = JsonConvert.DeserializeObject<List<Product>>(response.Content.ReadAsStringAsync().Result);
            return products;
        }

        public async Task<List<Product>> GetPublishedProducts()
        {
            List<Product> products = new List<Product>();
            Console.WriteLine("Отправка GET запроса на получение товаров загруженных пользователем");
            var response = await client.GetAsync($"Publisher/PublishedProducts");
            products = JsonConvert.DeserializeObject<List<Product>>(response.Content.ReadAsStringAsync().Result);
            Console.WriteLine(response.StatusCode);
            return products;
        }
        public async Task<HttpStatusCode> AddNewProduct(ProductAdd product)
        {
            var json = JsonConvert.SerializeObject(product);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Product", data);

            return response.StatusCode;
        }
        public async Task<Product> GetProduct(int product_id)
        {
            Product product = new Product();
            var response = await client.GetAsync($"Product/{product_id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                product = JsonConvert.DeserializeObject<Product>(response.Content.ReadAsStringAsync().Result);
            }

            return product;
        }

        public async Task<ProductUpdate> GetLastVersion(int product_id)
        {
            ProductUpdate productUpdate = new ProductUpdate();
            var response = await client.GetAsync($"Product/LatestVersion/{product_id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                productUpdate = JsonConvert.DeserializeObject<ProductUpdate>(response.Content.ReadAsStringAsync().Result);
            }
            return productUpdate;
        }

        public async Task<List<Product>> GetCartList(List<int> product_id_list)
        {
            List<Product> products = new List<Product>();

            if (Properties.Settings.Default.Token == null)
            {
                notificationManager.Show(title: "Аутентификация", message: ERROR_not_authorized, NotificationType.Error);
                return products;
            }


            var json = JsonConvert.SerializeObject(product_id_list);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"Product/ProductRangeList", data);
            current_status = response.StatusCode;
            if (current_status == HttpStatusCode.OK)
            {
/*                System.Windows.MessageBox.Show(response.Content.ReadAsStringAsync().Result);
*/                products = JsonConvert.DeserializeObject<List<Product>>(response.Content.ReadAsStringAsync().Result);
            }

            return products;
        }


        public async Task<List<Genre>> GetGenres()
        {
            List<Genre> products = new List<Genre>();

            if (Properties.Settings.Default.Token == null)
            {
                notificationManager.Show(title: "Аутентификация", message: ERROR_not_authorized, NotificationType.Error);
                return products;
            }

            var response = await client.GetAsync($"Genre");
            current_status = response.StatusCode;
            if (current_status == HttpStatusCode.OK)
            {
                products = JsonConvert.DeserializeObject<List<Genre>>(response.Content.ReadAsStringAsync().Result);
            }

            return products;
        }


        CancellationTokenSource cts = new CancellationTokenSource();

        public async Task LoadProductImages(int product_id)
        {
            try
            {
                /*                List<BitmapImage> productImages = new List<BitmapImage>(0);
                */
                List<string> imagesString = new List<string>(0);

                var response = await client.GetAsync($"Product/Images/{product_id}", cts.Token);

                current_status = response.StatusCode;
                if (current_status == HttpStatusCode.OK)
                {
                    imagesString = JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);


                    string cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CodeFlow", product_id.ToString(), "ImageCache");
                    if (Directory.Exists(cacheFolder))
                    {
                        Directory.Delete(cacheFolder, true);
                    }
                    if (!Directory.Exists(cacheFolder))
                    {
                        Directory.CreateDirectory(cacheFolder);
                    }


                    foreach (var imageBase64 in imagesString)
                    {
                        byte[] imagesBytes = Convert.FromBase64String(imageBase64);

                        string imagePath = Path.Combine(cacheFolder, $"{Guid.NewGuid()}.png");
                        File.WriteAllBytes(imagePath, imagesBytes);

                        /*                        using (MemoryStream stream = new MemoryStream(imagesBytes))
                                                {
                                                    bitmapImage.BeginInit();
                        *//*                          bitmapImage.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.Revalidate);
                        *//*                          bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                                    bitmapImage.StreamSource = stream;
                                                    bitmapImage.EndInit();
                                                }
                                                productImages.Add(bitmapImage);*/
                    }
                }
                return;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Загрузка изображений отменена");
                return;
            }
        }

        public async Task<List<BitmapImage>> GetImages(int product_id)
        {
            List<BitmapImage> productImages = new List<BitmapImage>(0);
            string cacheFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CodeFlow", product_id.ToString(), "ImageCache");
            if (Directory.Exists(cacheFolder))
            {
                string[] imageFiles = Directory.GetFiles(cacheFolder);

                if (imageFiles.Length > 0)
                {
                    foreach (var item in imageFiles)
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(item);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        productImages.Add(bitmapImage);
                    }
                }
            }
            return productImages;
        }

        public async Task LoadProductImagesWithId(int product_id)
        {
            try
            {
                var response = await client.GetAsync($"Product/ImagesWithId/{product_id}", cts.Token);

                current_status = response.StatusCode;
                if (current_status == HttpStatusCode.OK)
                {
                    List<ProductImage64string> productImages = JsonConvert.DeserializeObject<List<ProductImage64string>>(response.Content.ReadAsStringAsync().Result);

                    string cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CodeFlow", "publisher", product_id.ToString(), "ImageCache");
                    if (Directory.Exists(cacheFolder))
                    {
                        Directory.Delete(cacheFolder, true);
                    }
                    if (!Directory.Exists(cacheFolder))
                    {
                        Directory.CreateDirectory(cacheFolder);
                    }

                    foreach (var productImage in productImages)
                    {
                        byte[] imagesBytes = Convert.FromBase64String(productImage.ImageData);

                        string imagePath = Path.Combine(cacheFolder, $"{productImage.ImageId}.png");
                        File.WriteAllBytes(imagePath, imagesBytes);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Загрузка изображений отменена");
            }
        }

        public class ProductImageBitMap
        {
            public int ImageId { get; set; }
            public BitmapImage BitmapImage { get; set; }
        }
        public class ProductImage64string
        {
            public int ImageId { get; set; }
            public string ImageData { get; set; }
        }
        public async Task<List<ProductImageBitMap>> GetImagesWithId(int product_id)
        {
            List<ProductImageBitMap> productImages = new List<ProductImageBitMap>(0);
            string cacheFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CodeFlow", "publisher", product_id.ToString(), "ImageCache");
            if (Directory.Exists(cacheFolder))
            {
                string[] imageFiles = Directory.GetFiles(cacheFolder);

                if (imageFiles.Length > 0)
                {
                    foreach (var item in imageFiles)
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(item);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        int imageId = int.Parse(Path.GetFileNameWithoutExtension(item));

                        productImages.Add(new ProductImageBitMap() { ImageId = imageId, BitmapImage = bitmapImage });
                    }
                }
            }
            return productImages;
        }

        public async Task<HttpStatusCode> UploadImage(int product_id, string imagePath)
        {

            using (var content = new MultipartFormDataContent())
            {
/*                content.Add(new StringContent(productId.ToString()), "product_id");
*/                var fileContent = new ByteArrayContent(File.ReadAllBytes(imagePath));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                content.Add(fileContent, "image", Path.GetFileName(imagePath));

                var response = await client.PostAsync($"ProductImages/AddProductImage/{product_id}", content);
                return response.StatusCode;
            }
            
        }


        private Stopwatch _stopwatch = new Stopwatch();
        private long _lastBytesTransferred = 0;
        public async Task<ProductUpdate> DownloadProductFiles(int product_id, IProgress<double> progress, CancellationToken cancellationToken, DateTime _downloadStartTime)
        {
            ProductUpdate productUpdate = null;
            try
            {
                // Проверьте, есть ли уже какой-то прогресс загрузки
                var downloadedBytes = GetDownloadedBytes(product_id);
                Console.WriteLine("Байт было загружено: " + downloadedBytes);

                var getRequest = new HttpRequestMessage(HttpMethod.Get, $"Product/Download/{product_id}/null");
                if (downloadedBytes > 0)
                {
                    getRequest.Headers.Range = new RangeHeaderValue(downloadedBytes, null);
                }

                var getResponse = await client.SendAsync(getRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (getResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    System.Windows.MessageBox.Show("Не найдены версии продукта");
                    return productUpdate;
                }
                var totalBytes = getResponse.Content.Headers.ContentLength.GetValueOrDefault();
                var updateInfoJson = getResponse.Headers.GetValues("Update-Info").FirstOrDefault();
                productUpdate = JsonConvert.DeserializeObject<ProductUpdate>(updateInfoJson);


                Console.WriteLine("Всего нужно загрузить байт: " + totalBytes);

                double previousProgress = 0;
                var bytesTransferred = downloadedBytes;

                using (var fileStream = new FileStream($"{product_id}.zip", downloadedBytes > 0 ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None, 81920, true))
                {
                    DownloadService.Instance.TotalSize = Math.Round(totalBytes / (1024.0 * 1024.0 * 1024.0), 2);
                    DownloadService.Instance.DownloadStatus = "Загрузка";

                    using (var downloadStream = await getResponse.Content.ReadAsStreamAsync())
                    {
                        // Копируйте данные из потока загрузки в файловый поток
                        await downloadStream.CopyToAsync(fileStream, progress, cancellationToken, _downloadStartTime, downloadedBytes, totalBytes, product_id);

                    }
                }
                return productUpdate;
            }
            catch (Exception ex)
            {
                DownloadService.Instance.DownloadStatus = "Ошибка загрузки";
                // Логирование ошибок
                Console.WriteLine($"Произошла ошибка при загрузке файла: {ex.Message}");
                return productUpdate;
            }
            finally
            {
            }
        }

       
        private long GetDownloadedBytes(int productId)
        {
            var settingName = "DownloadInfo";
            var settingValue = Properties.Settings.Default[settingName];
            if (settingValue != null)
            {
                // Если настройка существует, десериализуйте ее из JSON
                var downloadInfos = JsonConvert.DeserializeObject<List<DownloadInfo>>((string)settingValue);
                var downloadInfo = downloadInfos.FirstOrDefault(di => di.ProductId == productId);
                return downloadInfo?.DownloadedBytes ?? 0;
            }
            else
            {
                // Если настройки не существует, верните 0
                return 0;
            }
        }
        public class DownloadInfo
        {
            public int ProductId { get; set; }
            public long DownloadedBytes { get; set; }
            // Добавьте здесь любую другую информацию, которую вы хотите сохранить
        }
        


        public async Task UploadProductFiles(int product_id, string updateVersion, string folderPath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            string zipPath = $"{folderPath}.zip";
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(folderPath, zipPath);

            UploadService.Instance.TotalSize = GetArchiveSize(zipPath);

            using (var fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
            {
                var content = new AsyncStreamContent(() => Task.FromResult((Stream)fileStream), new MediaTypeHeaderValue("multipart/form-data"));
                content.Headers.ContentLength = new FileInfo(zipPath).Length;

                var request = new HttpRequestMessage(HttpMethod.Post, $"Product/Upload/{product_id}?updateVersion={updateVersion}")
                {
                    Content = content
                };

                await SendRequestAsync(request, progress, cancellationToken);
            }

            File.Delete(zipPath);
        }


        public class AsyncStreamContent : StreamContent
        {
            private readonly Func<Task<Stream>> _streamFactory;
            private readonly MediaTypeHeaderValue _contentType;
/*            private readonly ILogger _logger;
*/
            public AsyncStreamContent(Func<Task<Stream>> streamFactory, MediaTypeHeaderValue contentType/*, ILogger<AsyncStreamContent> logger*/) : base(new MemoryStream())
            {
                _streamFactory = streamFactory;
                _contentType = contentType;
/*                _logger = logger;
*/            }

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                try
                {
                    var contentStream = await _streamFactory();
                    Headers.ContentLength = contentStream.Length;
                    Headers.ContentType = _contentType;
                    await contentStream.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, "Failed to serialize content to stream.");
                    throw;
                }
            }
        }

        private async Task SendRequestAsync(HttpRequestMessage request, IProgress<double> progress, CancellationToken cancellationToken)
        {
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var buffer = new byte[4096];
                    long totalBytesRead = 0;
                    long lastBytesTransferred = 0;
                    long totalBytesToSend = new FileInfo(request.Content.Headers.ContentLength.ToString()).Length;

                    while (true)
                    {
                        var bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (bytesRead == 0)
                            break;

                        totalBytesRead += bytesRead;
                        lastBytesTransferred = totalBytesRead;

                        double percentage = (double)((double)lastBytesTransferred / UploadService.Instance.TotalSize * 100);
                        progress.Report(percentage);
                        UploadService.Instance.UploadSpeed = Math.Round((lastBytesTransferred / (double)Stopwatch.GetTimestamp()), 2);
                        UploadService.Instance.UploadedSize = lastBytesTransferred;
                        UploadService.Instance.UploadStatus = $"Загрузка...";
                    }
                }
            }
            else
            {

/*                var content = await response.Content.ReadAsStringAsync();
                var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

                // handle error response
                Console.WriteLine($"Error: {problemDetails.Title} - {problemDetails.Detail}");*/

                throw new HttpRequestException($"Error uploading file: {response.StatusCode}");
            }
        }
        public class ProblemDetails
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("status")]
            public int Status { get; set; }

            [JsonProperty("detail")]
            public string Detail { get; set; }

            [JsonProperty("instance")]
            public string Instance { get; set; }
        }
        /*using (var content = new MultipartFormDataContent())
        using (var fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
        {
            var fileContent = new PushStreamContent(async (stream, contentContext, transportContext) =>
            {
                await fileStream.CopyToAsync(stream);
                await stream.FlushAsync(); // Закрываем поток, чтобы завершить PushStreamContent
            });*/

        public long GetArchiveSize(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Length;
        }

      
        private Stopwatch _stopwatch2 = new Stopwatch();
        /*public async Task UploadProductFiles(int product_id, string updateVersion, string folderPath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            // Создать zip-архив папки
            Console.WriteLine("Создание архива");

            string zipPath = $"{folderPath}.zip";
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(folderPath, zipPath);

            using (client)
            using (var content = new MultipartFormDataContent())
            using (var fileStream = File.OpenRead(zipPath))
            using (var streamContent = new StreamContent(fileStream))
            {
                Console.WriteLine("Создание запроса");

                content.Add(new StringContent(product_id.ToString()), "product_id");
                content.Add(new StringContent(updateVersion), "update_version");
                content.Add(streamContent, "file", Path.GetFileName(zipPath));
                Console.WriteLine("Отправка POST запроса");

                var response = await client.PostAsync($"Product/Upload/{product_id}", content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    // Обработка ошибок
                    return;
                }

                var totalBytes = fileStream.Length;
                var bytesTransferred = 0L;
                var buffer = new byte[81920];
                var bytesRead = 0;
                DateTime _UploadStartTime = DateTime.Now;

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                fileStream.Close();

                using (var newFileStream = File.OpenRead(zipPath))
                {
                    while ((bytesRead = await newFileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                    {
                        Console.WriteLine("Запись файла");

*//*                        await newFileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
*//*                        bytesTransferred += bytesRead;
                        UploadService.Instance.UploadedSize = bytesTransferred;

                        if (stopwatch.ElapsedMilliseconds >= 100)
                        {
                            Console.WriteLine("Расчет значений");

                            // Расчет скорости загрузки
                            var elapsedTime = DateTime.Now - _UploadStartTime;
                            var uploadSpeed = elapsedTime.TotalSeconds > 0 ? (bytesTransferred - _lastBytesTransferred) / elapsedTime.TotalSeconds : 0;
                            uploadSpeed = uploadSpeed / (1024 * 1024);
                            UploadService.Instance.UploadSpeed = Math.Round(uploadSpeed, 2);

                            if (totalBytes > 0)
                            {
                                var progressValue = (double)(bytesTransferred - _lastBytesTransferred) / totalBytes;
                                progress.Report(progressValue);
                            }

                            _lastBytesTransferred = bytesTransferred;
                            stopwatch.Restart();
                        }
                    }

                    stopwatch.Stop();

                    // Удалить временный zip-файл
                    Console.WriteLine("Удаление временного файла");

                    File.Delete(zipPath);
                }
            }
              

        }*/

    }
}