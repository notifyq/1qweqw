using Newtonsoft.Json;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using team_project.Model;

namespace team_project.Api
{
    public class ApiUser: Api
    {

        public async Task<User> GetUser(int user_id)
        {
            Console.WriteLine($"Отправка GET запроса на получение информации о пользователе с ID {user_id}");
            var response = await client.GetAsync($"User/GetUser/{user_id}");
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(userJson);
                return user;
            }
            else
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при получении информации о пользователе: {response.StatusCode}");
                return null;
            }
        }
        public async Task<Publisher> GetPublisher(int user_id)
        {
            Console.WriteLine($"Отправка GET запроса на получение информации о пользователе как издателе с ID {user_id}");
            var response = await client.GetAsync($"User/GetPublisher/{user_id}");
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<Publisher>(userJson);
                return user;
            }
            else
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при получении информации о пользователе: {response.StatusCode}");
                return null;
            }
        }

        public async Task<List<User>> GetUsers()
        {
            Console.WriteLine("Отправка GET запроса на получение списка пользователей");
            var response = await client.GetAsync("User/GetUsers");
            if (response.IsSuccessStatusCode)
            {
                var usersJson = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<User>>(usersJson);
                return users;
            }
            else
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при получении списка пользователей: {response.StatusCode}");
                return null;
            }
        }
        public async Task<List<Role>> GetRoles()
        {
            Console.WriteLine("Отправка GET запроса на получение списка ролей");
            var response = await client.GetAsync("Role");
            if (response.IsSuccessStatusCode)
            {
                var usersJson = await response.Content.ReadAsStringAsync();
                var roleList = JsonConvert.DeserializeObject<List<Role>>(usersJson);
                return roleList;
            }
            else
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при получении списка ролей: {response.StatusCode}");
                return null;
            }
        }

        public async Task<HttpStatusCode> ChangeUserRole(int user_id, int new_role_id)
        {
            Console.WriteLine($"Отправка PUT запроса на изменение роли пользователя с ID {user_id}");
            var response = await client.PutAsync($"User/ChangeUserRole/{user_id}/{new_role_id}", null);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Роль пользователя успешно изменена");
            }
            else
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при изменении роли пользователя: {response.StatusCode}");
            }
            return response.StatusCode;
        }

        public async Task<User> GetCurrentUserInfo()
        {
            Console.WriteLine("Отправка GET запроса на получение информации о текущем пользователе");
            var response = await client.GetAsync("User/GetCurrentUserInfo");
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(userJson);
                return user;
            }
            else
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при получении информации о пользователе: {response.StatusCode}");
                return null;
            }
        }

        public async Task<HttpStatusCode> EditUserInfo(User user)
        {
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine("Отправка PUT запроса на изменение информации о пользователе");
            var response = await client.PutAsync("User/EditUserInfo", data); // Предполагается, что у вас есть соответствующий метод в API

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Информация успешно обновлена");
            }
            else
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при обновлении информации о пользователе: {response.StatusCode}");
            }

            return response.StatusCode;
        }
        public async Task<List<Product>> GetUserProducts()
        {
            List<Product> products = new List<Product>();

            try
            {
                var response = await client.GetAsync($"User/Library");
                current_status = response.StatusCode;

                if (current_status == HttpStatusCode.OK)
                {
                    products = JsonConvert.DeserializeObject<List<Product>>(response.Content.ReadAsStringAsync().Result);
                }

                return products;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return products;
            }
        }

        public async Task<List<ProductKey>> GetKeys()
        {
            List<ProductKey> keys = new List<ProductKey>();

            try
            {
                var response = await client.GetAsync($"User/Library/Keys");
                current_status = response.StatusCode;
                if (current_status == HttpStatusCode.OK)
                {
                    keys = JsonConvert.DeserializeObject<List<ProductKey>>(response.Content.ReadAsStringAsync().Result);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return keys;
            }

            return keys;
        }

        public async Task<List<Purchase>> GetPurchases()
        {
            List<Purchase> purchases = new List<Purchase>();
            try
            {
                var response = await client.GetAsync($"User/Purchases");
                current_status = response.StatusCode;
                if (current_status == HttpStatusCode.OK)
                {
                    purchases = JsonConvert.DeserializeObject<List<Purchase>>(response.Content.ReadAsStringAsync().Result);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return purchases;
            }
            return purchases;
        }
    }
}
