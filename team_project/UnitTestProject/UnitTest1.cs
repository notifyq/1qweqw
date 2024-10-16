using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using team_project;
using System.Net;
using System.Threading.Tasks;
using team_project.Api;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestIncorrectLogin()
        {
            var api = new Api();

            string Login = "IncorrectLogin";
            string Password = "IncorrectPassword";
            string result = String.Empty;

            string fact_Result = await api.UserLoginAsync(Login, Password);

            Assert.AreEqual(result, fact_Result);
        }
        [TestMethod]
        public async Task TestCorrectLogin()
        {
            var api = new Api();

            string Login = "notifyq";
            string Password = "123456789";
            HttpStatusCode ResultStatus = HttpStatusCode.OK;

            string token = await api.UserLoginAsync(Login, Password);

            HttpStatusCode FactResultStatus = await api.GetLastCodeStatusAsync();

            Assert.AreEqual(ResultStatus, FactResultStatus);
        }

        [TestMethod]
        public async Task TestIncorrectRegistration()
        {
            var api = new Api();

            string Login = "notifyq";
            string Password = "1324253897";
            string Email = "adsasd@mail.ru";
            HttpStatusCode ResultStatus = HttpStatusCode.Conflict;

            await api.UserRegistrationAsync(Login, Password, Email);

            HttpStatusCode FactResultStatus = await api.GetLastCodeStatusAsync();

            Assert.AreEqual(ResultStatus, FactResultStatus);
        }
        [TestMethod]
        public async Task TestIncorrectRegistration_EmptyProperty()
        {
            var api = new Api();

            string Login = "incor";
            string Password = "i";
            string Email = null;
            HttpStatusCode ResultStatus = HttpStatusCode.BadRequest;

            await api.UserRegistrationAsync(Login, Password, Email);

            HttpStatusCode FactResultStatus = await api.GetLastCodeStatusAsync();

            Assert.AreEqual(ResultStatus, FactResultStatus);
        }

        [TestMethod]
        public async Task TestCorrectRegistration()
        {
            var api = new Api();

            string Login = "correctLogin_14";
            string Password = "correctPassword_1";
            string Email = "correctEmail@mail.ru";
            HttpStatusCode ResultStatus = HttpStatusCode.Created;

            await api.UserRegistrationAsync(Login, Password, Email);
            HttpStatusCode FactResultStatus = await api.GetLastCodeStatusAsync();

            Assert.AreEqual(ResultStatus, FactResultStatus);
        }

        [TestMethod]
        public async Task GetUserLibrary()
        {
            var api = new Api();
            var apiUser = new ApiUser();
            string token = await api.UserLoginAsync("notifyq","123456789");
            api.SetTokenForClientAsync(token);

            await apiUser.GetUserProducts();
            HttpStatusCode FactResultStatus = await api.GetLastCodeStatusAsync();

            Assert.AreEqual(HttpStatusCode.OK, FactResultStatus);
        }

    }
}
