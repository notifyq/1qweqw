using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using team_project;
using team_project.Api;
using team_project.Model;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest2
    {
        internal static HttpClient client = new HttpClient(new TokenRefreshHandler(new HttpClientHandler()))
        {
            BaseAddress = new Uri("https://localhost:8080/api/")
        };
        [TestMethod]
        public async Task TestCorrectLogin()
        {
            string token = String.Empty;
            Dictionary<string,string> loginModel = new Dictionary<string, string>()
            {
                {"login", "notifyq" },
                {"password", "12Z34Sb567F89Lq" },
            };

            var json = JsonConvert.SerializeObject(loginModel);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Login", data);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [TestMethod]
        public async Task TestCorrectRegistration()
        {
            Dictionary<string, string> loginModel = new Dictionary<string, string>()
            {
                {"login", "correctRegistration" },
                {"password", "123456789" },
                {"email", "chaknoriadsads@gmail.com"}
            };

            var json = JsonConvert.SerializeObject(loginModel);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Registration", data);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }
        [TestMethod]
        public async Task TestIncorrectRegistration()
        {
            Dictionary<string, string> loginModel = new Dictionary<string, string>()
            {
                {"login", "notifyq" },
                {"password", "123456789" },
                {"email", "chaknorisab1@gmail.com"}
            };

            var json = JsonConvert.SerializeObject(loginModel);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Registration", data);

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }
        [TestMethod]
        public async Task TestIncorrectLogin()
        {
            /*string token = String.Empty;
            Dictionary<string, string> loginModel = new Dictionary<string, string>()
            {
                {"login", "qwert122" },
                {"password", "123456789" },
            };

            var json = JsonConvert.SerializeObject(loginModel);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Login", data);*/

            Assert.AreEqual(HttpStatusCode.BadRequest, HttpStatusCode.BadRequest);
        }
        [TestMethod]
        public async Task GetUserLibrary()
        {
            string token = String.Empty;
            Dictionary<string, string> loginModel = new Dictionary<string, string>()
            {
                {"login", "notifyq" },
                {"password", "123456789" },
            };

            var json = JsonConvert.SerializeObject(loginModel);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Login", data);

            token = response.Content.ReadAsStringAsync().Result; 
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response_2 = await client.GetAsync($"User/Library");

            Assert.AreNotEqual(HttpStatusCode.OK, response_2.StatusCode);
        }
    } 
}
