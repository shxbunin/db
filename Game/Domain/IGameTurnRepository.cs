using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        GameTurnEntity Insert(GameTurnEntity gameTurn);
        IList<GameTurnEntity> GetLastTurnsForGame(Guid gameId, int count = 5);
        IList<GameTurnEntity> GetAllTurnsForGame(Guid gameId);
    }
}