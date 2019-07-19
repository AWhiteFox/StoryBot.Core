using MongoDB.Driver;
using StoryBot.Core.Abstractions;
using StoryBot.Core.Model;
using System;

namespace StoryBot.Core.Logic
{
    public class SavesHandler : ISavesHandler
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IMongoCollection<SaveDocument> collection;

        public SavesHandler(IMongoCollection<SaveDocument> collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// Returns save by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SaveDocument Get(long id)
        {
            var results = collection.Find(Builders<SaveDocument>.Filter.Eq("id", id));
            try
            {
                return results.Single();
            }
            catch (InvalidOperationException)
            {
                if (results.CountDocuments() == 0)
                {
                    logger.Warn($"Save for {id} not found. Creating one...");
                    collection.InsertOne(new SaveDocument(id));
                    return Get(id);
                }
                else throw;
            }
        }

        /// <summary>
        /// Creates new SaveDocument in saves collection
        /// </summary>
        /// <param name="save"></param>
        public void CreateNew(long id)
        {
            if (collection.CountDocuments(x => x.Id == id) == 0)
                collection.InsertOne(new SaveDocument(id));
        }

        /// <summary>
        /// Updates existing save document
        /// </summary>
        /// <param name="save"></param>
        public void Update(SaveDocument save)
        {
            collection.ReplaceOne(Builders<SaveDocument>.Filter.Eq("id", save.Id), save);
        }
    }
}
