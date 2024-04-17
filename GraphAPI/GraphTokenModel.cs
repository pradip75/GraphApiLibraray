using Newtonsoft.Json;

namespace GraphAPI
{
    public class GraphTokenModel
    {
        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the application secret.
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the calls uri of the application.
        /// </summary>
        [JsonProperty("scope")]
        public string ExpiresInExt { get; set; }

        /// <summary>
        /// Gets or sets the comms platform endpoint uri.
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
