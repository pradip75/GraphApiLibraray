using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace GraphAPI.Controllers
{
    [ApiController]
    public class GraphController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GraphController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("api/graph/getauthtoken")]
        public async Task<GraphTokenModel> GetAuthTokenAsync()
        {
            HttpResponseMessage tokenRes = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    AzureSettings azureSettings = _configuration.GetSection("AzureAd").Get<AzureSettings>();
                    string replaceTenantString = "{tenantId}";
                    string oauthV2TokenLink = "https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
                    var tokenLink = oauthV2TokenLink.Replace(replaceTenantString, azureSettings.TenantId);

                    var dict = new Dictionary<string, string>();

                    dict.Add("grant_type", "client_credentials");
                    dict.Add("client_id", azureSettings.ClientId);
                    dict.Add("client_secret", azureSettings.AppSecret);
                    dict.Add("scope", "https://graph.microsoft.com/.default");

                    var tokentReq = new HttpRequestMessage(HttpMethod.Post, tokenLink)
                    {
                        Content = new FormUrlEncodedContent(dict),
                    };

                    foreach (var header in dict)
                    {
                        tokentReq.Headers.Add(header.Key, header.Value);
                    }

                    var content = (Dictionary<string, string>)(object)dict;

                    tokenRes = client.SendAsync(tokentReq).Result;
                    var responseStream = await tokenRes.Content.ReadAsStringAsync();
                    GraphTokenModel tokenObj = JsonConvert.DeserializeObject<GraphTokenModel>(responseStream);
                    return tokenObj;
                }
            }
            catch (Exception ex)
            {
                return new GraphTokenModel();
            }
        }

        [HttpPost]
        [Route("api/graph/createuser")]
        public async Task<CreateUserResponse> CreateUserAsync([FromHeader(Name = "Token")] string token, [FromBody] User user)
        {
            HttpResponseMessage tokenRes = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string createUserUrl = "https://graph.microsoft.com/v1.0/users";
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var dict = new Dictionary<string, string>();

                    string routingJson = JsonConvert.SerializeObject(user);
                    var routingReq = new HttpRequestMessage(HttpMethod.Post, createUserUrl)
                    {
                        Content = new StringContent(routingJson, Encoding.UTF8, "application/json"),
                    };
                    HttpResponseMessage routingRes = client.SendAsync(routingReq).Result;
                    var responseStream = await tokenRes.Content.ReadAsStringAsync();
                    CreateUserResponse tokenObj = JsonConvert.DeserializeObject<CreateUserResponse>(responseStream);
                    return tokenObj;
                }
            }
            catch (Exception ex)
            {
                return new CreateUserResponse();
            }
        }
    }
}
