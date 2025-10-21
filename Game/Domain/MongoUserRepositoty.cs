using System;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            
            var indexKeys = Builders<UserEntity>.IndexKeys.Ascending(u => u.Login);
            var indexOptions = new CreateIndexOptions { Unique = true, Name = "unique_login" };
            var indexModel = new CreateIndexModel<UserEntity>(indexKeys, indexOptions);
            userCollection.Indexes.CreateOne(indexModel);
        }

        public UserEntity Insert(UserEntity user)
        {
            if (user.Id == Guid.Empty)
                user = new UserEntity(Guid.NewGuid(), user.Login, user.LastName, user.FirstName, user.GamesPlayed, user.CurrentGameId);

            userCollection.InsertOne(user);
            return user;
        }
        
        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(u => u.Id == id).FirstOrDefault();
        }
        
        public UserEntity GetOrCreateByLogin(string login)
        {
            var existing = userCollection.Find(u => u.Login == login).FirstOrDefault();
            if (existing != null)
                return existing;
            
            var newUser = new UserEntity(Guid.NewGuid(), login, "", "", 0, null);

            try
            {
                userCollection.InsertOne(newUser);
                return newUser;
            }
            catch (MongoWriteException e) when (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return userCollection.Find(u => u.Login == login).FirstOrDefault();
            }
        }
        
        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(u => u.Id == user.Id, user);
        }
        
        public void Delete(Guid id)
        {
            userCollection.DeleteOne(u => u.Id == id);
        }
        
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var totalCount = userCollection.CountDocuments(FilterDefinition<UserEntity>.Empty);

            var users = userCollection
                .Find(FilterDefinition<UserEntity>.Empty)
                .SortBy(u => u.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            return new PageList<UserEntity>(users, totalCount, pageNumber, pageSize);
        }

        // Не требуется реализовывать
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}
