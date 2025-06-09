namespace KoeHandel.API.Models.Game
{
    public class CreateGameRequest
    {
        public required string PlayerName { get; set; }
    }

    public class CreateGameResponse
    {
        public required int GameId { get; set; }
        public required string PlayerName { get; set; }
        public required Guid PlayerId { get; set; }
    }


}
