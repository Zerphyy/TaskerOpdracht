using Newtonsoft.Json;

namespace Setup.Data
{
    public class RecaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string>? ErrorCodes { get; set; }
    }
}
