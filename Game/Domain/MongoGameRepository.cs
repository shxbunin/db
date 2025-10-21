using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gameCollection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db)
        {
            gameCollection = db.GetCollection<GameEntity>(CollectionName);

            var indexKeys = Builders<GameEntity>.IndexKeys.Ascending(g => g.Status);
            var indexModel = new CreateIndexModel<GameEntity>(indexKeys);
            gameCollection.Indexes.CreateOne(indexModel);
        }

        public GameEntity Insert(GameEntity game)
        {
            if (game.Id == Guid.Empty)
            {
                var idField = typeof(GameEntity).GetProperty("Id");
                idField?.SetValue(game, Guid.NewGuid());
            }

            gameCollection.InsertOne(game);
            return game;
        }


        public GameEntity FindById(Guid gameId)
        {
            return gameCollection.Find(g => g.Id == gameId).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            gameCollection.ReplaceOne(g => g.Id == game.Id, game);
        }

        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameCollection
                .Find(g => g.Status == GameStatus.WaitingToStart)
                .Sort(Builders<GameEntity>.Sort.Ascending("_id"))
                .Limit(limit)
                .ToList();
        }



        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var filter = Builders<GameEntity>.Filter.And(
                Builders<GameEntity>.Filter.Eq(g => g.Id, game.Id),
                Builders<GameEntity>.Filter.Eq(g => g.Status, GameStatus.WaitingToStart)
            );

            var result = gameCollection.ReplaceOne(filter, game);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
