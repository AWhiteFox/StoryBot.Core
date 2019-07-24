using MongoDB.Driver;
using StoryBot.Core.Abstractions;
using StoryBot.Core.Model;

namespace StoryBot.Core.Logic
{
    public class SavesContext : ISavesContext
    {
        private readonly IMongoCollection<SaveDocument> collection;

        public SavesContext(IMongoCollection<SaveDocument> collection)
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
            return collection.Find(Builders<SaveDocument>.Filter.Eq("id", id)).Single();
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
