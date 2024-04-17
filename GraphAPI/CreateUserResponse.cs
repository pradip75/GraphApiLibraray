using Newtonsoft.Json;

namespace GraphAPI
{
    public class CreateUserResponse
    {
        /// <summary>
        /// Gets or sets ODataType.
        /// </summary>
        [JsonProperty("@odata.context")]
        public string ODataContext { get; set; }

        /// <summary>
        /// Gets or sets id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets businessPhones.
        /// </summary>
        [JsonProperty("businessPhones")]
        public List<object> BusinessPhones { get; set; }

        /// <summary>
        /// Gets or sets displayName.
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets givenName.
        /// </summary>
        [JsonProperty("givenName")]
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets jobTitle.
        /// </summary>
        [JsonProperty("jobTitle")]
        public string JobTitle { get; set; }

        /// <summary>
        /// Gets or sets mail.
        /// </summary>
        [JsonProperty("mail")]
        public string Mail { get; set; }

        /// <summary>
        /// Gets or sets mobilePhone.
        /// </summary>
        [JsonProperty("mobilePhone")]
        public string MobilePhone { get; set; }

        /// <summary>
        /// Gets or sets officeLocation.
        /// </summary>
        [JsonProperty("officeLocation")]
        public string OfficeLocation { get; set; }

        /// <summary>
        /// Gets or sets preferredLanguage.
        /// </summary>
        [JsonProperty("preferredLanguage")]
        public string PreferredLanguage { get; set; }

        /// <summary>
        /// Gets or sets surname.
        /// </summary>
        [JsonProperty("surname")]
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets userPrincipalName.
        /// </summary>
        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }
    }
}
