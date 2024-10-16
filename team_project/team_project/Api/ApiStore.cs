using Newtonsoft.Json;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using team_project.Model;

namespace team_project.Api
{
    internal class ApiStore: Api
    {
        Cart cart = new Cart();
        public async Task Purchase()
        {
            var json = JsonConvert.SerializeObject(cart.GetCartList());
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Purchase", data);
            current_status = response.StatusCode;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                notificationManager.Show(title: "Покупка", message: STATUS_success, NotificationType.Success);
            }
            else
            {
                notificationManager.Show(title: "Покупка", message: $"{response.Content.ReadAsStringAsync().Result}", NotificationType.Error);
            }
        }
    }
}
