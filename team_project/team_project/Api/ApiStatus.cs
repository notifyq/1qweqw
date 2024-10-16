using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using team_project.Model;

namespace team_project.Api
{
    public class ApiStatus: Api
    {
        public async Task<List<Status>> GetStatuses()
        {
            List<Status> statusList = new List<Status>();
            var response = await client.GetAsync("Status");
            statusList = JsonConvert.DeserializeObject<List<Status>>(response.Content.ReadAsStringAsync().Result);
            return statusList;
        }

        public async Task<List<Status>> GetProductStatuses()
        {
            List<Status> statusList = new List<Status>();
            var response = await client.GetAsync("Status/ProductStatuses");
            statusList = JsonConvert.DeserializeObject<List<Status>>(response.Content.ReadAsStringAsync().Result);
            return statusList;
        }
        public async Task<List<Status>> GetUpdateStatuses()
        {
            List<Status> statusList = new List<Status>();
            var response = await client.GetAsync("Status/UpdateStatuses");
            statusList = JsonConvert.DeserializeObject<List<Status>>(response.Content.ReadAsStringAsync().Result);
            return statusList;
        }
    }
}
