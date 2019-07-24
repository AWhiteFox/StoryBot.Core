using StoryBot.Core.Model;
using System.Collections.Generic;

namespace StoryBot.Core.Abstractions
{
    public interface IMessageBuilder<T>
    {
        /// <summary>
        /// Default content message
        /// </summary>
        /// <param name="storylineElement"></param>
        /// <param name="unlockables"></param>
        /// <returns></returns>
        T BuildContent(StorylineElement storylineElement, List<string> unlockables);
        /// <summary>
        /// Achievement message
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns></returns>
        T BuildAchievement(StoryAchievement achievement);
        /// <summary>
        /// Ending message
        /// </summary>
        /// <param name="story"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        T BuildEnding(StoryDocument story, int position);

        /// <summary>
        /// Story select dialog
        /// </summary>
        /// <param name="prologues"></param>
        /// <returns></returns>
        T BuildStorySelectDialog(List<StoryDocument> prologues);
        /// <summary>
        /// Episode select dialog
        /// </summary>
        /// <param name="episodes"></param>
        /// <param name="storyProgress"></param>
        /// <returns></returns>
        T BuildEpisodeSelectDialog(List<StoryDocument> episodes, SaveStoryStats storyProgress);

        /// <summary>
        /// Short stats for all stories
        /// </summary>
        /// <param name="prologues"></param>
        /// <param name="storiesStats"></param>
        /// <returns></returns>
        T BuildStats(List<StoryDocument> prologues, List<SaveStoryStats> storiesStats);
        /// <summary>
        /// Detailed stats for one story
        /// </summary>
        /// <param name="episodes"></param>
        /// <param name="episodesStats"></param>
        /// <returns></returns>
        T BuildStoryStats(List<StoryDocument> episodes, List<SaveEpisodeStats> episodesStats);
        /// <summary>
        /// Detailed stats for one episode
        /// </summary>
        /// <param name="episodeData"></param>
        /// <param name="episodeStats"></param>
        /// <returns></returns>
        T BuildEpisodeStats(StoryDocument episodeData, SaveEpisodeStats episodeStats);

        /// <summary>
        /// Beginning message
        /// </summary>
        /// <returns></returns>
        T BuildBeginningMessage();
        /// <summary>
        /// Use this message when user select wrong number
        /// </summary>
        /// <returns></returns>
        T BuildIndexOutOfRangeMessage();
        /// <summary>
        /// Error message
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        T BuildSomethingWentWrongMessage(string exception = null);
        /// <summary>
        /// Command list
        /// </summary>
        /// <returns></returns>
        T BuildCommandList();
    }
}
