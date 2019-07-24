using StoryBot.Core.Abstractions;
using StoryBot.Core.Model;
using System;
using System.Linq;

namespace StoryBot.Core.Logic
{
    /// <summary>
    /// Connects MessageBuilder and MessageSender
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReplyHandler<T> : IBotDefaultCommands
    {       
        /// <summary>
        /// Gets stories
        /// </summary>
        private readonly IStoriesContext stories;

        /// <summary>
        /// Gets and updates saves
        /// </summary>
        private readonly ISavesContext saves;

        /// <summary>
        /// Message generator
        /// </summary>
        private readonly IMessageBuilder<T> messageBuilder;

        /// <summary>
        /// Sends messages
        /// </summary>
        private readonly IMessageSender<T> messageSender;

        /// <summary>
        /// Command prefix
        /// </summary>
        public char Prefix { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="vk"></param>
        /// <param name="stories"></param>
        /// <param name="saves"></param>
        public ReplyHandler(IStoriesContext stories,
                            ISavesContext saves,
                            IMessageBuilder<T> messageBuilder,
                            IMessageSender<T> messageSender,
                            char prefix)
        {
            this.stories = stories;
            this.saves = saves;
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.Prefix = prefix;
        }

        #region Replies

        /// <summary>
        /// Sends response to provided number
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="number"></param>
        public void ReplyToNumber(long userId, int number)
        {
            // Try-catch block: if something goes wrong replies with "Index not found"
            try
            {
                // Getting save
                var save = saves.Get(userId);

                if (save.Current.Story != null) // IF story already selected
                {
                    if (save.Current.Episode != null) // IF episode already selected
                    {
                        // Getting story from DB
                        StoryDocument story = stories.GetEpisode(save.Current.Story.Value, save.Current.Episode.Value);

                        // Finding selected option
                        StoryOption selectedOption = story.GetStoryline(save.Current.Storyline)
                                .Elements[save.Current.Position]
                                .Options[number - 1];

                        if (selectedOption.Storyline == "Ending") // If selected option is ending...
                        {
                            // Add ending to progress
                            save.AddEnding(story.StoryId, story.Episode, selectedOption.Position.Value);

                            // Send messages
                            messageSender.Send(userId, messageBuilder.BuildEnding(story, selectedOption.Position.Value));
                            var episodes = stories.GetStoryEpisodes(story.StoryId);
                            var progress = save.GetStoryStats(story.StoryId);
                            messageSender.Send(userId, messageBuilder.BuildEpisodeSelectDialog(episodes, progress));
                        }
                        else // Default case
                        {
                            // Updating save progress
                            if (selectedOption.Storyline != null)
                                save.Current.Storyline = selectedOption.Storyline;
                            if (selectedOption.Position != null)
                                save.Current.Position = selectedOption.Position.Value;

                            // "Needed" check
                            if (selectedOption.Needed != null && !selectedOption.Needed.All(save.Current.Unlockables.Contains))
                                goto IndexNotFound;

                            // "Unlocks" handling
                            if (!string.IsNullOrEmpty(selectedOption.Unlocks))
                            {
                                if (!save.Current.Unlockables.Contains(selectedOption.Unlocks))
                                    save.AddUnlockable(selectedOption.Unlocks);
                                else
                                    goto IndexNotFound;
                            }

                            // If selected option contains an achievement
                            if (selectedOption.Achievement != null)
                            {
                                // Add achievement to save stats
                                save.AddAchievement(story.StoryId, story.Episode, selectedOption.Achievement.Value);
                                // Edit response message
                                messageSender.Send(userId, messageBuilder.BuildAchievement(story.Achievements[selectedOption.Achievement.Value]));
                            }

                            // Send created message
                            var storylineElement = story.GetStoryline(save.Current.Storyline).Elements[save.Current.Position];
                            messageSender.Send(userId, messageBuilder.BuildContent(storylineElement, save.Current.Unlockables));
                        }
                    }
                    else // From episode selection
                    {
                        // Check that previous episode's canonical ending completed
                        if (number == 0 || save.GetStoryStats(save.Current.Story.Value).Episodes[number - 1].ObtainedEndings.Contains(0))
                        {
                            // Get episode by provided number
                            StoryDocument story = stories.GetEpisode(save.Current.Story.Value, number);

                            // Update current progress
                            save.Current.Episode = number;
                            save.Current.Storyline = story.Storylines[0].Tag;
                            save.Current.Position = 0;

                            // Send message
                            messageSender.Send(userId, messageBuilder.BuildContent(story.Storylines[0].Elements[0], save.Current.Unlockables));
                        }
                        else
                            goto IndexNotFound;
                    }
                }
                else // From story selection
                {
                    // Find story progress
                    var stats = save.GetStoryStats(number);

                    // Get all story episodes
                    var episodes = stories.GetStoryEpisodes(number);
                    save.Current.Story = number;

                    // Send episode select dialog
                    messageSender.Send(userId, messageBuilder.BuildEpisodeSelectDialog(episodes, stats));
                }

                // Finally update save
                saves.Update(save);
                return;
            }
            catch (Exception e)
            {
                if (e is IndexOutOfRangeException || e is ArgumentOutOfRangeException)
                {
                    goto IndexNotFound;
                }
                else throw;
            }

        IndexNotFound: // If something went wrong...
            messageSender.Send(userId, messageBuilder.BuildIndexOutOfRangeMessage());
        }

        /// <summary>
        /// Sends error message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        public void ReplyWithError(long userId, string message = null) => messageSender.Send(userId, messageBuilder.BuildSomethingWentWrongMessage(message));

        /// <summary>
        /// Sends Index Out Of Range message
        /// </summary>
        /// <param name="userId"></param>
        public void ReplyWithIndexOutOfRange(long userId) => messageSender.Send(userId, messageBuilder.BuildIndexOutOfRangeMessage());

        /// <summary>
        /// Used when user starts conversation with bot
        /// </summary>
        /// <param name="userId"></param>
        public void ReplyToTheFirstMessage(long userId)
        {
            // Create new save
            saves.CreateNew(userId);

            // Send messages
            messageSender.Send(userId, messageBuilder.BuildBeginningMessage());
            messageSender.Send(userId, messageBuilder.BuildCommandList());
            messageSender.Send(userId, messageBuilder.BuildStorySelectDialog(stories.GetAllPrologues()));
        }

        #endregion

        #region Default Bot Commands

        public IBotDefaultCommands Commands { get { return this; } }

        void IBotDefaultCommands.List(long peerId)
        {
            // Send stories stats
            messageSender.Send(peerId,
                messageBuilder.BuildStats(stories.GetAllPrologues(),
                saves.Get(peerId).StoriesStats));
        }

        void IBotDefaultCommands.List(long peerId, int storyId)
        {
            // Send story episodes stats
            messageSender.Send(peerId,
                messageBuilder.BuildStoryStats(stories.GetStoryEpisodes(storyId),
                saves.Get(peerId).GetStoryStats(storyId).Episodes));
        }

        void IBotDefaultCommands.List(long peerId, int storyId, int episodeId)
        {
            // Send episode stats
            messageSender.Send(peerId,
                messageBuilder.BuildEpisodeStats(stories.GetEpisode(storyId, episodeId),
                saves.Get(peerId).GetStoryStats(storyId).Episodes[episodeId]));
        }

        void IBotDefaultCommands.Repeat(long peerId)
        {
            // Get save
            var save = saves.Get(peerId);
            if (save.Current.Story != null)
            {
                if (save.Current.Episode != null) // Default case
                {
                    var storylineElement = stories.GetEpisode(save.Current.Story.Value, save.Current.Episode.Value)
                        .GetStoryline(save.Current.Storyline).Elements[save.Current.Position];
                    messageSender.Send(peerId,
                        messageBuilder.BuildContent(storylineElement, save.Current.Unlockables));
                }
                else // Episode select
                {
                    var episodes = stories.GetStoryEpisodes(save.Current.Story.Value);
                    var stats = save.GetStoryStats(save.Current.Story.Value);

                    messageSender.Send(peerId,
                        messageBuilder.BuildEpisodeSelectDialog(episodes, stats));
                }
            }
            else // Story select
            {
                messageSender.Send(peerId, messageBuilder.BuildStorySelectDialog(stories.GetAllPrologues()));
            }
        }

        void IBotDefaultCommands.Select(long peerId)
        {
            // Reset progress
            var save = saves.Get(peerId);
            save.Current = new SaveProgress();
            saves.Update(save);

            // Send story select dialog
            messageSender.Send(peerId, messageBuilder.BuildStorySelectDialog(stories.GetAllPrologues()));
        }

        #endregion
    }
}
