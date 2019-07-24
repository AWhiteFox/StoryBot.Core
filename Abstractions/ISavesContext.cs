using StoryBot.Core.Model;

namespace StoryBot.Core.Abstractions
{
    public interface ISavesContext
    {
        void CreateNew(long id);
        SaveDocument Get(long id);
        void Update(SaveDocument save);
    }
}