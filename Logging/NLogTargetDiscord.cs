using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBot.Core.Logging
{
    /// <summary>
    /// NLog target to logging into Discord Webhook
    /// </summary>
    [Target("Discord")]
    public sealed class NLogTargetDiscord : TargetWithLayout
    {
        /// <summary>
        /// Webhook
        /// </summary>
        private readonly DiscordWebhook discord;

        /// <summary>
        /// Default constructor
        /// </summary>
        public NLogTargetDiscord()
        {
            discord = new DiscordWebhook(Environment.GetEnvironmentVariable("DISCORD_WEBHOOKID"),
                                         Environment.GetEnvironmentVariable("DISCORD_WEBHOOKTOKEN"));
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent.Exception != null)
            {
                string[] lines = logEvent.Exception.ToString().Split(Environment.NewLine);

                StringBuilder stringBuilder = new StringBuilder();
                List<string> sendingList = new List<string>();
                foreach (string x in lines)
                {
                    if (stringBuilder.Length + x.Length <= 2000)
                    {
                        stringBuilder.Append(x + "\n");
                    }
                    else
                    {
                        sendingList.Add(stringBuilder.ToString());
                        stringBuilder.Clear();
                    }
                }
                sendingList.Add(stringBuilder.ToString());

                logEvent.Exception = null;
                discord.Send(Layout.Render(logEvent));

                foreach (string x in sendingList)
                {
                    discord.Send($"```csharp\n{x}\n```");
                }
            }
            else
            {
                discord.Send(Layout.Render(logEvent));
            }
        }
    }
}
