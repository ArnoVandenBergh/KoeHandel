using KoeHandel.API.Mappers;
using KoeHandel.API.Models.Game;
using KoeHandel.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KoeHandel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly KoeHandelContext _context;

        public GamesController(KoeHandelContext context)
        {
            _context = context;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            return await _context.Games.Include(g => g.Players).ToListAsync();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NewGameResponse>> GetGame(int id)
        {
            var game = await _context.Games.Include(g => g.Players).Where(g => g.Id == id).FirstOrDefaultAsync();

            if (game == null)
            {
                return NotFound();
            }

            return new NewGameResponse()
            {
                GameId = game.Id,
                Players = game.Players.Select(p => new NewPlayerResponse() { Id = p.Id, Name = p.Name }).ToList(),
                GameState = game.State
            };
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, Game game)
        {
            if (id != game.Id)
            {
                return BadRequest();
            }

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NewGameResponse>> CreateNewGame(CreateGameRequest createGame)
        {
            var player = new BL.Player(createGame.PlayerName);
            var game = new BL.Game(player);

            var dbGame = game.ToPersistenceGame();

            _context.Games.Add(dbGame);
            await _context.SaveChangesAsync();

            game.Id = dbGame.Id;

            var response = new NewGameResponse
            {
                GameId = game.Id,
                Players = [new() { Id = player.Id, Name = player.Name }],
                GameState = game.State
            };

            return CreatedAtAction("GetGame", new { id = game.Id }, response);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
