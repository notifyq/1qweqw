using Newtonsoft.Json;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using team_project.Model;

namespace team_project.Api
{
    internal class ApiSupport: Api
    {
        public async Task AddSupportRequest(int support_type_id, string support_title)
        {
            if (Properties.Settings.Default.Token == null)
            {
                notificationManager.Show(title: "Аутентификация", message: ERROR_not_authorized, NotificationType.Error);
                return;
            }

            SupportAdd new_request = new SupportAdd()
            {
                support_type_id = support_type_id,
                support_title = support_title,
            };

            var json = JsonConvert.SerializeObject(new_request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Support/AddSupportRequest", data);
            current_status = response.StatusCode;

            return;
        }
        public async Task<List<SupportRequest>> GetUserSupportRequests()
        {
            List<SupportRequest> supportRequests = new List<SupportRequest>();
            if (Properties.Settings.Default.Token == null)
            {
                notificationManager.Show(title: "Аутентификация", message: ERROR_not_authorized, NotificationType.Error);
                return supportRequests;
            }
            var response = await client.GetAsync($"Support/GetUserSupportRequest/{Properties.Settings.Default.UserID}");
            current_status = response.StatusCode;

            if (current_status == HttpStatusCode.OK)
            {
                supportRequests = JsonConvert.DeserializeObject<List<SupportRequest>>(response.Content.ReadAsStringAsync().Result);
            }


            return supportRequests;
        }

        public async Task<List<RequestType>> GetSupportType()
        {
            List<RequestType> supportTypes = new List<RequestType>();

            var response = await client.GetAsync("SupportType");
            current_status = response.StatusCode;

            supportTypes = JsonConvert.DeserializeObject<List<RequestType>>(response.Content.ReadAsStringAsync().Result);

            return supportTypes;
        }

        public async Task<List<SupportRequestMessage>> GetSupportMessages(int support_request_id)
        {
            List<SupportRequestMessage> supportMessages = new List<SupportRequestMessage>(0);

            if (Properties.Settings.Default.Token == null)
            {
                notificationManager.Show(title: "Аутентификация", message: ERROR_not_authorized, NotificationType.Error);
                return supportMessages;
            }

            var response = await client.GetAsync($"SupportMessages/GetRequestMessages/{support_request_id}");
            current_status = response.StatusCode;
            if (current_status == HttpStatusCode.OK)
            {
                supportMessages = JsonConvert.DeserializeObject<List<SupportRequestMessage>>(response.Content.ReadAsStringAsync().Result);
            }
            return supportMessages;
        }

        public async Task<SupportRequestMessage> SendSupportMessage(int support_request_id, string message)
        {
            SupportRequestMessage sended_message = new SupportRequestMessage();
            if (Properties.Settings.Default.Token == null)
            {
                notificationManager.Show(title: "Аутентификация", message: ERROR_not_authorized, NotificationType.Error);
                return sended_message;
            }

            SendMessageModel sendMessageModel = new SendMessageModel()
            {
                support_request_id = support_request_id,
                message = message,
            };

            var json = JsonConvert.SerializeObject(sendMessageModel);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("SupportMessages/SendMessage", data);
            current_status = response.StatusCode;
            if (current_status == HttpStatusCode.OK)
            {
                sended_message = JsonConvert.DeserializeObject<SupportRequestMessage>(response.Content.ReadAsStringAsync().Result);
            }
            return sended_message;
        }
    }
}
