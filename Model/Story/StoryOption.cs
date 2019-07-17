﻿using MongoDB.Bson.Serialization.Attributes;

namespace StoryBot.Core.Model
{
    public class StoryOption
    {
        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("storyline")]
        public string Storyline { get; set; }

        [BsonElement("position")]
        public int? Position { get; set; }

        [BsonElement("achievement")]
        public int? Achievement { get; set; }

        [BsonElement("unlocks")]
        public string Unlocks { get; set; }

        [BsonElement("needed")]
        public string[] Needed { get; set; }
    }
}
