using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain
{
    public class InMemoryGameTurnRepository : IGameTurnRepository
    {
        private readonly Dictionary<Guid, GameTurnEntity> entities = new();

        public GameTurnEntity Insert(GameTurnEntity gameTurn)
        {
            entities[gameTurn.Id] = gameTurn;
            return gameTurn;
        }

        public IList<GameTurnEntity> GetLastTurnsForGame(Guid gameId, int count = 5)
        {
            return entities.Values
                .Where(gt => gt.GameId == gameId)
                .OrderByDescending(gt => gt.FinishedAt)
                .Take(count)
                .ToList();
        }

        public IList<GameTurnEntity> GetAllTurnsForGame(Guid gameId)
        {
            return entities.Values
                .Where(gt => gt.GameId == gameId)
                .OrderByDescending(gt => gt.FinishedAt)
                .ToList();
        }
    }
}
