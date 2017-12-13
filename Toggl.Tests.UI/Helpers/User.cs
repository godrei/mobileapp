using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Toggl.Tests.UI.Helpers
{
    public static class User
    {
        public static async Task<string> Create(string email)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            var password = "123456";

            using (var client = new HttpClient())
            {
                var json = $"{{\"user\":{{\"email\":\"{email}\",\"password\":\"{password}\"}}}}";
                var url = new Uri("https://toggl.space/api/v8/signups");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await client.PostAsync(url, content);
            }

            return password;
        }
    }
}
