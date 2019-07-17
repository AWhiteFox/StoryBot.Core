using MongoDB.Bson.Serialization.Attributes;

namespace StoryBot.Core.Model
{
    public class Storyline
    {
        [BsonElement("tag")]
        public string Tag { get; set; }

        [BsonElement("elements")]
        public StorylineElement[] Elements { get; set; }
    }
}
