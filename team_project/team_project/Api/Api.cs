using Newtonsoft.Json;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using team_project.Model;

namespace team_project.Api
{
    public class Api
    {
        public static string ERROR_authorization = "Неверные данные";
        public static string ERROR_not_authorized = "Токен не действителен";
        public static string ERROR_forbidden = "Нет доступа к этому действию";
        public static string ERROR_null = "Список пуст";
        public static string STATUS_success = "Успешно";
        internal static NotificationManager notificationManager = new NotificationManager();
        public HttpStatusCode current_status;

        public static string ApiConntectionString
        {
            get
            {
                if (Properties.Settings.Default["ApiConntectionString"] != null)
                {
                    return Properties.Settings.Default["ApiConntectionString"].ToString();
                }
                return "";
            }
            set
            {
                Properties.Settings.Default["ApiConntectionString"] = value;
                Properties.Settings.Default.Save();
            }
        }

        internal static HttpClient client = new HttpClient(new TokenRefreshHandler(new HttpClientHandler()))
        {
            BaseAddress = new Uri(ApiConntectionString),
            Timeout = TimeSpan.FromMilliseconds(-1),

        };

        public Api()
        {
            current_status = new HttpStatusCode();
        }
        public async Task UserRegistrationAsync(string login, string password, string email)
        {
            RegistrationModel registrationModel = new RegistrationModel()
            {
                email = email,
                password = password,
                login = login,
            };

            var json = JsonConvert.SerializeObject(registrationModel);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Registration", data);
            current_status = response.StatusCode;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                notificationManager.Show(title: "Регистрация", message: STATUS_success, NotificationType.Success);
            }
            else
            {
                notificationManager.Show(title: "Регистрация", message: $"{response.Content.ReadAsStringAsync().Result}", NotificationType.Error);
            }
        }

        public async Task<string> UserLoginAsync(string login, string password)
        {
            string token = String.Empty;
            LoginModel loginModel = new LoginModel()
            {
                login = login,
                password = password,
            };

            var json = JsonConvert.SerializeObject(loginModel);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Login", data);
            current_status = response.StatusCode;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return String.Empty;
            }
            else
            {
                notificationManager.Show(title: "Вход", message: "Успешно", NotificationType.Success);
                token = response.Content.ReadAsStringAsync().Result;
                return token;
            }
        }
        public async Task SetTokenForClientAsync(string token)
        {
            if (token == null || token == "")
            {
                /*notificationManager.Show(title: "Вход", message: ERROR_not_authorized, NotificationType.Error);*/
                return;
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("User/GetCurrentUserInfo");
            current_status = response.StatusCode;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                User user = JsonConvert.DeserializeObject<User>(response.Content.ReadAsStringAsync().Result);
                Properties.Settings.Default.Token = token;
                Properties.Settings.Default.UserID = user.UserId;
                Properties.Settings.Default.UserRole = user.UserRoleNavigation.RoleName;

                Properties.Settings.Default.Save();
            }
            else
            {
                notificationManager.Show(title: "Вход", message: ERROR_not_authorized, NotificationType.Error);
            }
        }
        public async Task<HttpStatusCode> GetLastCodeStatusAsync()
        {
            return current_status;
        }
    }
}
