using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class RecaptchaResponse
    {
        public bool Success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTimeOffset? ChallengeTs { get; set; }

        public string Hostname { get; set; }

        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; } = new();
    }
}
