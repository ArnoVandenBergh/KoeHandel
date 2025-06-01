using KoeHandel.BLTests.Models;

namespace KoeHandel.BL.Tests
{
    public class BaseTests
    {
        protected readonly Player _player1;
        protected readonly Player _player2;
        protected readonly Player _player3;
        protected readonly Game _game;

        public BaseTests()
        {
            _player1 = new Player("Player 1");
            _player2 = new Player("Player 2");
            _player3 = new Player("Player 3");
            _game = new Game(_player1, new TestAnimalDeck());
            _game.AddPlayer(_player2);
            _game.AddPlayer(_player3);
        }
    }
}