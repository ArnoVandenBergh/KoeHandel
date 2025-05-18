using KoeHandel.BL;

try
{
    var arnoPlayer = new Player("Arno");
    var dagmarPlayer = new Player("Dagmar");
    var gillPlayer = new Player("Gill");

    var game = new Game(arnoPlayer);
    game.AddPlayer(dagmarPlayer);
    game.AddPlayer(gillPlayer);

    game.StartGame();

    var auction = game.StartNewAuction(arnoPlayer);








}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}




