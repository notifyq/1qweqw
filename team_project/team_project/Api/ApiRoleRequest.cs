using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApplication4.Model;

namespace team_project.Api
{
    public class ApiRoleRequest: Api
    {
        public ApiRoleRequest() { }
        public async Task<List<RoleRequest>> GetRoleRequests()
        {
            List<RoleRequest> roleRequestList = new List<RoleRequest>();
            var response = await client.GetAsync("RoleRequest");
            roleRequestList = JsonConvert.DeserializeObject<List<RoleRequest>>(response.Content.ReadAsStringAsync().Result);
            return roleRequestList;
        }

        public async Task<RoleRequest> GetRoleRequest(int requestId)
        {
            RoleRequest roleRequest = new RoleRequest();
            var response = await client.GetAsync($"RoleRequest/{requestId}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                roleRequest = JsonConvert.DeserializeObject<RoleRequest>(response.Content.ReadAsStringAsync().Result);
            }
            return roleRequest;
        }

        public async Task<HttpStatusCode> UpdateRoleRequest(RoleRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"RoleRequest/{request.RoleRequestId}", data);

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> AddRoleRequest(RoleRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("RoleRequest", data);

            return response.StatusCode;
        }

        public async Task<List<RoleRequest>> GetCurrentUserRoleRequests()
        {
            List<RoleRequest> roleRequestList = new List<RoleRequest>();
            var response = await client.GetAsync("RoleRequest/MyRequests");
            roleRequestList = JsonConvert.DeserializeObject<List<RoleRequest>>(response.Content.ReadAsStringAsync().Result);
            return roleRequestList;
        }

        public async Task<RoleRequest> GetCurrentUserRoleRequest(int requestId)
        {
            RoleRequest roleRequest = new RoleRequest();
            var response = await client.GetAsync($"RoleRequest/MyRequests/{requestId}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                roleRequest = JsonConvert.DeserializeObject<RoleRequest>(response.Content.ReadAsStringAsync().Result);
            }
            return roleRequest;
        }

    }
}
