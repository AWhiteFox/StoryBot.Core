using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace StoryBot.Core.Logging
{
    /// <summary>
    /// Class to handle Discord Webhooks
    /// </summary>
    public class DiscordWebhook
    {
        /// <summary>
        /// HTTP Client
        /// </summary>
        private static readonly HttpClient httpClient = new HttpClient();
        
        /// <summary>
        /// URL for making request
        /// </summary>
        private readonly string webhookUrl;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        public DiscordWebhook(string id, string token)
        {
            webhookUrl = $"https://discordapp.com/api/webhooks/{id}/{token}";
        }

        /// <summary>
        /// Sends webhook message
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            httpClient.PostAsync(webhookUrl, new StringContent(JsonConvert.SerializeObject(new WebhookBody(message)), Encoding.UTF8, "application/json"));
        }

        /// <summary>
        /// Helper class for serialization
        /// </summary>
        [Serializable]
        private class WebhookBody
        {
            public WebhookBody(string content)
            {
                this.Content = content;
            }

            [JsonProperty("content")]
            public string Content { get; set; }
        }
    }
}
