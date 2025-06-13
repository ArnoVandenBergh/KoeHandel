namespace KoeHandel.API.Models.Game
{
    public class CreateGameRequest
    {
        public required string PlayerName { get; set; }
    }

    public class NewGameResponse
    {
        public required int GameId { get; set; }
        public List<NewPlayerResponse> Players { get; set; } = [];
    }

    public class NewPlayerResponse
    {
        public required string Name { get; set; }
        public required Guid Id { get; set; }
    }


}
