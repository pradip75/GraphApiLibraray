using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
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

        //scope = "https://graph.microsoft.com/.default" for getting token for creating user
        [HttpPost]
        [Route("api/graph/getauthtoken")]
        public async Task<GraphTokenModel> GetAuthTokenAsync()
        {
            HttpResponseMessage tokenRes = new HttpResponseMessage();
            GraphTokenModel responseObj = new GraphTokenModel();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    AzureSettings azureSettings = new AzureSettings();
                    azureSettings = _configuration.GetSection("AzureAd").Get<AzureSettings>();
                    if (azureSettings != null)
                    {
                        string oauthV2TokenLink = string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", azureSettings.TenantId);

                        var dict = new Dictionary<string, string>();

                        dict.Add("grant_type", "client_credentials");
                        dict.Add("client_id", azureSettings.ClientId);
                        dict.Add("client_secret", azureSettings.AppSecret);
                        //dict.Add("scope", "api://b62dbfe5-7d37-4503-8a81-cfb6ee03373d/.default");
                        dict.Add("scope", "https://graph.microsoft.com/.default");

                        var tokentReq = new HttpRequestMessage(HttpMethod.Post, oauthV2TokenLink)
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
                        responseObj = JsonConvert.DeserializeObject<GraphTokenModel>(responseStream);
                        return responseObj;
                    }
                    return responseObj;
                }
            }
            catch (Exception ex)
            {
                return responseObj;
            }
        }


        [HttpPost]
        [Route("api/graph/authorize")]
        public async Task<GraphTokenModel> Auhthorize()
        {
            HttpResponseMessage tokenRes = new HttpResponseMessage();
            GraphTokenModel responseObj = new GraphTokenModel();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    AzureSettings azureSettings = new AzureSettings();
                    azureSettings = _configuration.GetSection("AzureAd").Get<AzureSettings>();
                    if (azureSettings != null)
                    {
                        string oauthV2TokenLink = string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/authorize", azureSettings.TenantId);

                        var dict = new Dictionary<string, string>();

                        dict.Add("client_id", azureSettings.ClientId);
                        dict.Add("response_type", "code");
                        dict.Add("redirect_uri", "https://localhost:7001/api/auth/callback");
                        dict.Add("response_mode", "query");
                        dict.Add("grant_type", "authorization_code");
                        dict.Add("scope", "api://b62dbfe5-7d37-4503-8a81-cfb6ee03373d/Forecast.Read");
                        dict.Add("state", "12345");

                        var tokentReq = new HttpRequestMessage(HttpMethod.Get, oauthV2TokenLink)
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
                        //responseObj = JsonConvert.DeserializeObject<GraphTokenModel>(responseStream);
                        return responseObj;
                    }
                    return responseObj;
                }
            }
            catch (Exception ex)
            {
                return responseObj;
            }
        }

        [HttpPost]
        [Route("api/graph/createuser")]
        public async Task<CreateUserResponse> CreateUserAsync([FromHeader(Name = "Token")] string token, [FromBody] CreateUserRequest user)
        {
            HttpResponseMessage routingRes = new HttpResponseMessage();
            CreateUserResponse responseObj = new CreateUserResponse();
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
                    routingRes = client.SendAsync(routingReq).Result;
                    var responseStream = await routingRes.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<CreateUserResponse>(responseStream);
                    return responseObj;
                }
            }
            catch (Exception ex)
            {
                return responseObj;
            }
        }

        [HttpPost]
        [Route("api/graph/approleassignments")]
        public async Task<AppRoleAssignmentsResponse> AppRoleAssignmentsAsync([FromHeader(Name = "Token")] string token, [FromBody] AppRoleAssignmentsRequest role)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            AppRoleAssignmentsResponse responseObj = new AppRoleAssignmentsResponse();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string createRoleUrl = string.Format("https://graph.microsoft.com/v1.0/servicePrincipals/{0}/appRoleAssignedTo", role.ResourceId);

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var dict = new Dictionary<string, string>();

                    string routingJson = JsonConvert.SerializeObject(role);
                    var routingReq = new HttpRequestMessage(HttpMethod.Post, createRoleUrl)
                    {
                        Content = new StringContent(routingJson, Encoding.UTF8, "application/json"),
                    };
                    HttpResponseMessage routingRes = client.SendAsync(routingReq).Result;
                    var responseStream = await response.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<AppRoleAssignmentsResponse>(responseStream);
                }
                return responseObj;

            }
            catch (Exception ex)
            {
                return responseObj;
            }
        }

        [HttpPost]
        [Route("api/graph/createapprole")]
        public async Task<AppRoleResponse> CreateAppRole([FromHeader(Name = "Token")] string token, [FromHeader(Name = "ObjectId-AppReg")] string objectIdAppReg, [FromHeader(Name = "ObjectId-EnpApp")] string objectIdEnpApp, [FromBody] AppRoleRequest appRole)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            AppRoleResponse appRoleResponse = new AppRoleResponse();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string getRolesUrl = string.Format("https://graph.microsoft.com/v1.0/servicePrincipals/{0}/appRoles", objectIdEnpApp);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    response = await client.GetAsync(getRolesUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        AppRoleResponse responseObj = new AppRoleResponse();

                        string responseBody = await response.Content.ReadAsStringAsync();
                        responseObj = JsonConvert.DeserializeObject<AppRoleResponse>(responseBody);

                        foreach (var item in responseObj.Value)
                        {
                            appRole.AppRoles.Add( new AppRole()
                            {
                                AllowedMemberTypes = item.AllowedMemberTypes,
                                Description = item.Description,
                                DisplayName = item.DisplayName,
                                Id = item.Id,
                                IsEnabled = item.IsEnabled,
                                Origin = item.Origin,
                                Value = item.Value
                            });

                        }

                        string createRoleUrl = string.Format("https://graph.microsoft.com/v1.0/applications/{0}", objectIdAppReg);

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        var dict = new Dictionary<string, string>();

                        string routingJson = JsonConvert.SerializeObject(appRole);
                        var routingReq = new HttpRequestMessage(HttpMethod.Patch, createRoleUrl)
                        {
                            Content = new StringContent(routingJson, Encoding.UTF8, "application/json"),
                        };

                        HttpResponseMessage routingRes = client.SendAsync(routingReq).Result;
                        var responseStream = await response.Content.ReadAsStringAsync();
                        appRoleResponse = JsonConvert.DeserializeObject<AppRoleResponse>(responseStream);
                        return appRoleResponse;
                    }
                    return appRoleResponse;
                }
            }
            catch (Exception ex)
            {
                return appRoleResponse;
            }
        }

        [HttpGet]
        [Route("api/auth/authcallback")]
        public async Task<GraphTokenModel> AuthCallback(string code, string state)
        {
            GraphTokenModel responseObj = new GraphTokenModel();

            if (string.IsNullOrEmpty(code))
            {
                // Handle error
                return responseObj;
            }

            // Exchange authorization code for tokens
            var tokenResponse = await ExchangeCodeForTokens(code);

            if (tokenResponse.IsSuccessStatusCode)
            {

                // Tokens retrieved successfully, handle further processing
                // For example, save tokens to secure storage and proceed to the application's main page
                var tokenRes = await tokenResponse.Content.ReadAsStringAsync();
                responseObj = JsonConvert.DeserializeObject<GraphTokenModel>(tokenRes);
                return responseObj;
            }
            else
            {
                // Handle token retrieval error
                var error = await tokenResponse.Content.ReadAsStringAsync();
                return responseObj;
            }
        }



        [HttpPost]
        [Route("api/graph/revokesigninsessions")]
        public async Task<HttpResponseMessage> RevokeSignInSessions([FromHeader(Name = "Token")] string token, [FromHeader(Name = "UserPrincipalName")] string userPrincipalName)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string revokeUrl = string.Format("https://graph.microsoft.com/v1.0/users/{0}/revokesigninsessions", userPrincipalName);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    response = await client.PostAsync(revokeUrl, null);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return response;
            }
        }

        [HttpPost]
        [Route("api/graph/userclaiminfo")]
        public async Task<string> UserClaimInfo([FromHeader(Name = "Token")] string token)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            AppRoleResponse appRoleResponse = new AppRoleResponse();
            try
            {
                if (token != null && token.StartsWith("Bearer"))
                {
                    var _token = token.Substring("Bearer ".Length).Trim();
                    var decodeToken = new JwtSecurityToken(jwtEncodedString: token);
                    string tenantId = decodeToken.Claims.First(c => c.Type == "tid").Value;
                    string userId = decodeToken.Claims.First(c => c.Type == "oid").Value;
                    string userName = decodeToken.Claims.First(c => c.Type == "name").Value;
                    string email = decodeToken.Claims.First(c => c.Type == "upn").Value;
                    return tenantId;
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private async Task<HttpResponseMessage> ExchangeCodeForTokens(string code)
        { 

            AzureSettings azureSettings = new AzureSettings();
            azureSettings = _configuration.GetSection("AzureAd").Get<AzureSettings>();
            string tokenEndpoint = string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", azureSettings.TenantId);

            var clientId = azureSettings.ClientId;
            var clientSecret = azureSettings.AppSecret;
            var redirectUri = "https://localhost:7001/api/auth/authcallback";

            var httpClient = new HttpClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            tokenRequest.Content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("scope", "User.Read"),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            });

            return await httpClient.SendAsync(tokenRequest);
        }

        //scope = "https://graph.microsoft.com/Directory.AccessAsUser.All" for change password
        //scope = "api://b62dbfe5-7d37-4503-8a81-cfb6ee03373d/.default" for authorization
        [HttpPost]
        [Route("api/graph/getauthtokenpassword")]
        public async Task<GraphTokenModel> GetAuthTokenPasswordAsync([FromHeader(Name = "UserName")] string username, [FromHeader(Name = "Password")] string password)
        {
            HttpResponseMessage tokenRes = new HttpResponseMessage();
            GraphTokenModel responseObj = new GraphTokenModel();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    AzureSettings azureSettings = new AzureSettings();
                    azureSettings = _configuration.GetSection("AzureAd").Get<AzureSettings>();
                    string tokenEndpoint = string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", azureSettings.TenantId);

                    var clientId = azureSettings.ClientId;
                    var clientSecret = azureSettings.AppSecret;

                    var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                    tokenRequest.Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("client_secret", clientSecret),
                        new KeyValuePair<string, string>("scope", "api://b62dbfe5-7d37-4503-8a81-cfb6ee03373d/.default"),
                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", password),
                        new KeyValuePair<string, string>("grant_type", "password"),
                    });

                    tokenRes = await httpClient.SendAsync(tokenRequest);
                    var responseStream = await tokenRes.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<GraphTokenModel>(responseStream);
                    return responseObj;
                }
            }
            catch (Exception ex)
            {
                return responseObj;
            }
        }


        [HttpPost]
        [Route("api/graph/getrefreshtoken")]
        public async Task<GraphTokenModel> GetRefreshToken([FromHeader(Name = "Refresh-Token")] string refreshToken)
        {
            HttpResponseMessage tokenRes = new HttpResponseMessage();
            GraphTokenModel responseObj = new GraphTokenModel();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    AzureSettings azureSettings = new AzureSettings();
                    azureSettings = _configuration.GetSection("AzureAd").Get<AzureSettings>();
                    string tokenEndpoint = string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", azureSettings.TenantId);

                    var clientId = azureSettings.ClientId;
                    var clientSecret = azureSettings.AppSecret;

                    var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                    tokenRequest.Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("scope", "user.read"),
                        new KeyValuePair<string, string>("refresh_token", refreshToken),
                        new KeyValuePair<string, string>("grant_type", "refresh_token"),
                        new KeyValuePair<string, string>("client_secret", clientSecret),
                    });

                    tokenRes = await httpClient.SendAsync(tokenRequest);
                    var responseStream = await tokenRes.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<GraphTokenModel>(responseStream);
                    return responseObj;
                }
            }
            catch (Exception ex)
            {
                return responseObj;
            }
        }


        [HttpPost]
        [Route("api/graph/changepassword")]
        public async Task<HttpResponseMessage> ChangePasswordAsync([FromHeader(Name = "Token")] string token, [FromBody] ChangePasswordRequest changePasswordRequest)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string changePasswordUrl = $"https://graph.microsoft.com/v1.0/me/changePassword";
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    string routingJson = JsonConvert.SerializeObject(changePasswordRequest);
                    var routingReq = new HttpRequestMessage(HttpMethod.Post, changePasswordUrl)
                    {
                        Content = new StringContent(routingJson, Encoding.UTF8, "application/json"),
                    };
                    response = client.SendAsync(routingReq).Result;
                    return response;
                }
            }
            catch (Exception ex)
            {
                return response;
            }
        }

        [HttpPost]
        [Route("api/graph/getauthtokenazure_adb2c_ropc")]
        public async Task<GraphTokenModel> GetAuthTokenAzure_ADB2C_ROPC([FromHeader(Name = "UserName")] string username, [FromHeader(Name = "Password")] string password)
        {
            HttpResponseMessage tokenRes = new HttpResponseMessage();
            GraphTokenModel responseObj = new GraphTokenModel();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    AzureSettings azureSettings = new AzureSettings();
                    azureSettings = _configuration.GetSection("AzureAd").Get<AzureSettings>();
                    string tokenEndpoint = $"https://mydurapp.b2clogin.com/mydurapp.onmicrosoft.com/b2c_1_ropc_auth/oauth2/v2.0/token";

                    var clientId = azureSettings.ClientId;

                    var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                    tokenRequest.Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", password),
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("scope", "openid 2a8858ec-8299-4dfa-b162-f5620a2e94f5 offline_access"),
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("response_type", "token id_token"),
                    });

                    tokenRes = await httpClient.SendAsync(tokenRequest);
                    var responseStream = await tokenRes.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<GraphTokenModel>(responseStream);
                    return responseObj;
                }
            }
            catch (Exception ex)
            {
                responseObj.AccessToken = JsonConvert.SerializeObject(ex);
                return responseObj;
            }
        }

    }
}
