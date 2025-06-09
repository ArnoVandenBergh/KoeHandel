namespace KoeHandel.API.Mappers
{
    public static class GameMappers
    {
        public static KoeHandel.Persistence.Game ToPersistenceGame(this BL.Game game)
        {
            return new KoeHandel.Persistence.Game
            {
                Id = game.Id,
                State = game.State,
                Players = game.Players.Select(p => p.ToPersistencePlayer()).ToList(),
                CurrentPlayer = game.CurrentPlayer?.ToPersistencePlayer()!
            };
        }
    }

    public static class PlayerMappers
    {
        public static KoeHandel.Persistence.Player ToPersistencePlayer(this BL.Player player)
        {
            return new KoeHandel.Persistence.Player
            {
                Id = player.Id,
                Name = player.Name
            };
        }

    }
}
