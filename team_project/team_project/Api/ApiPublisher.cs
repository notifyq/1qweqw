using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using team_project.Model;

namespace team_project.Api
{
    public class ApiPublisher: Api
    {
        public async Task<List<Publisher>> GetPublishers()
        {
            List<Publisher> publisherList = new List<Publisher>();
            var response = await client.GetAsync("Publisher");
            publisherList = JsonConvert.DeserializeObject<List<Publisher>>(response.Content.ReadAsStringAsync().Result);
            return publisherList;
        }
    }
}
