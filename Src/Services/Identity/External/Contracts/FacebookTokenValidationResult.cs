using System.Text.Json.Serialization;

namespace Identity.External.Contracts;

   public class FacebookTokenValidationResult
    {
        [JsonPropertyName("data")]
        public FacebookTokenValidationData Data { get; set; }
    }

    public class FacebookTokenValidationData
{
        [JsonPropertyName("app_id")]
        public string AppId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("application")]
        public string Application { get; set; }

        [JsonPropertyName("data_access_expires_at")]
        public long DataAccessExpiresAt { get; set; }

        [JsonPropertyName("expires_at")]
        public long ExpiresAt { get; set; }

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("scopes")]
        public string[] Scopes { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }

