using Newtonsoft.Json;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }

    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;

        public TokenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetTokenAsync()
        {
            var formData = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "MVCUserApiClient" },
                { "client_secret", "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0" }, 
                { "username", "yagmurnov2@gmail.com" },
                { "password", "Nov2005!!" }
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync("http://localhost:5001/connect/token", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Token request failed.");
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);


            return tokenResponse.access_token;
        }
    }
}
