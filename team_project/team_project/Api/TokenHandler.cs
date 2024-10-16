using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace team_project.Api
{

    public class TokenRefreshHandler : DelegatingHandler
    {
        static Api api = new Api();
        public TokenRefreshHandler(HttpMessageHandler innerHandler)
        : base(innerHandler)
        {
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            try
            {
                if (!response.IsSuccessStatusCode) // Если ответ неуспешен
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}"); // Статус кода
                    Console.WriteLine($"Сообщение от сервера: {response.ReasonPhrase}"); // Сообщение от сервера

                    if (request.Content != null)
                    {
                        Console.WriteLine($"Тело запроса: {await request.Content.ReadAsStringAsync()}"); // Тело запроса
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Обновить токен
                var token = await api.UserLoginAsync(Properties.Settings.Default.UserLogin, Properties.Settings.Default.UserPassword);

                if (!string.IsNullOrEmpty(token))
                {
                    await api.SetTokenForClientAsync(token);

                    // Обновить заголовок авторизации с новым токеном
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Если это запрос на загрузку файла, нужно пересоздать содержимое запроса
                    if (request.RequestUri.AbsolutePath.StartsWith("/Product/Upload"))
                    {
                        var fileContent = (PushStreamContent)request.Content;
                        var newFileContent = new PushStreamContent(async (stream, contentContext, transportContext) =>
                        {
                            // Пересоздать поток файла
                            using (var fileStream = new FileStream(fileContent.Headers.ContentDisposition.FileName.Trim('\"'), FileMode.Open, FileAccess.Read))
                            {
                                await fileStream.CopyToAsync(stream);
                            }
                            await stream.FlushAsync(); // Закрываем поток, чтобы завершить PushStreamContent
                        });

                        newFileContent.Headers.ContentDisposition = fileContent.Headers.ContentDisposition;
                        newFileContent.Headers.ContentLength = fileContent.Headers.ContentLength;

                        // Заменить старое содержимое запроса новым
                        request.Content = newFileContent;
                    }

                    // Повторно отправить запрос с новым токеном
                    response = await base.SendAsync(request, cancellationToken);
                    Console.WriteLine("Токен обновлен");
                }
            }

            return response;
        }
    }
}

