using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> gameTurnCollection;
        public const string CollectionName = "gameTurns";

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            gameTurnCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            var indexKeys = Builders<GameTurnEntity>.IndexKeys
                .Ascending(gt => gt.GameId)
                .Descending(gt => gt.FinishedAt);
            var indexOptions = new CreateIndexOptions { Name = "gameId_finishedAt_desc" };
            var indexModel = new CreateIndexModel<GameTurnEntity>(indexKeys, indexOptions);
            gameTurnCollection.Indexes.CreateOne(indexModel);
        }

        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            gameTurnCollection.InsertOne(gameTurn);
            return gameTurn;
        }

        public IList<GameTurnEntity> GetLastTurnsForGame(Guid gameId, int count = 5)
        {
            return gameTurnCollection
                .Find(gt => gt.GameId == gameId)
                .SortByDescending(gt => gt.FinishedAt)
                .Limit(count)
                .ToList();
        }

        public IList<GameTurnEntity> GetAllTurnsForGame(Guid gameId)
        {
            return gameTurnCollection
                .Find(gt => gt.GameId == gameId)
                .SortByDescending(gt => gt.FinishedAt)
                .ToList();
        }
    }
}