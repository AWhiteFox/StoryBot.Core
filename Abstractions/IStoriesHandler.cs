using System.Collections.Generic;
using StoryBot.Core.Model;

namespace StoryBot.Core.Abstractions
{
    public interface IStoriesHandler
    {
        List<StoryDocument> GetAllPrologues();
        StoryDocument GetEpisode(int storyId, int episodeId);
        List<StoryDocument> GetStoryEpisodes(int storyId);
    }
}