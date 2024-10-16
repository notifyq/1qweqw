using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace team_project.Model
{
    public class UserStatusNavigation
    {
        [JsonProperty("statusId")]
        public int StatusId { get; set; }
        [JsonProperty("statusName")]
        public string StatusName { get; set; }
    }
}
