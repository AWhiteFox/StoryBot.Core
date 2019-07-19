using StoryBot.Core.Model;

namespace StoryBot.Core.Abstractions
{
    public interface ISavesHandler
    {
        void CreateNew(long id);
        SaveDocument Get(long id);
        void Update(SaveDocument save);
    }
}