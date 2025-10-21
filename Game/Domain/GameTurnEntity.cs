using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public GameTurnEntity(Guid gameId, int turnNumber, List<PlayerTurnDecision> playerDecisions, Guid? winnerId, bool isDraw)
        {
            Id = Guid.NewGuid();
            GameId = gameId;
            TurnNumber = turnNumber;
            PlayerDecisions = playerDecisions;
            WinnerId = winnerId;
            IsDraw = isDraw;
            FinishedAt = DateTime.UtcNow;
        }

        [BsonElement("id")]
        public Guid Id { get; set; }

        [BsonElement("gameId")]
        public Guid GameId { get; set; }

        [BsonElement("turnNumber")]
        public int TurnNumber { get; set; }

        [BsonElement("playerDecisions")]
        public List<PlayerTurnDecision> PlayerDecisions { get; set; }

        [BsonElement("winnerId")]
        public Guid? WinnerId { get; set; }

        [BsonElement("isDraw")]
        public bool IsDraw { get; set; }

        [BsonElement("finishedAt")]
        public DateTime FinishedAt { get; set; }
    }

    public class PlayerTurnDecision
    {
        [BsonConstructor]
        public PlayerTurnDecision()
        {
        }

        public PlayerTurnDecision(Guid userId, string playerName, PlayerDecision decision)
        {
            UserId = userId;
            PlayerName = playerName;
            Decision = decision;
        }

        [BsonElement("userId")]
        public Guid UserId { get; set; }

        [BsonElement("playerName")]
        public string PlayerName { get; set; }

        [BsonElement("decision")]
        public PlayerDecision Decision { get; set; }
    }
}