using Newtonsoft.Json;

namespace NewNFTGel.Models
{
    public abstract class AuthModel
    {
        [JsonProperty("secret_key")]
        public string secret_key { get; init; }

        [JsonProperty("public_key")]
        public string public_key { get; init; }

        [JsonProperty("address")]
        public string address { get; init; }

        [JsonProperty("IsFinaleAuth")]
        public bool IsFinaleAuth { get; set; }

        [JsonProperty("User")]
        public static string User => "PersonLog";
    }
}
