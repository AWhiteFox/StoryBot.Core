namespace StoryBot.Core.Abstractions
{
    public interface IBotDefaultCommands
    {
        void List(long peerId);
        void List(long peerId, int storyId);
        void List(long peerId, int storyId, int episodeId);

        void Repeat(long peerId);
        void Select(long peerId);
    }
}
