using HitBallWebServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HitBallWebServer
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/scores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Score>>> GetScores()
        {
            return await _context.Scores.ToListAsync();
        }

        // GET: api/scores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Score>> GetScore(int id)
        {
            var score = await _context.Scores.FindAsync(id);

            if (score == null)
            {
                return NotFound();
            }

            return score;
        }

        // GET: api/scores/player/1
        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<Score>>> GetScoresByPlayer(string playerId)
        {
            return await _context.Scores.Where(s => s.Id == playerId).ToListAsync();
        }

        // POST: api/scores
        [HttpPost]
        public async Task<ActionResult<Score>> PostScore(Score score)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Scores.Add(score);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetScore), new { id = score.GameId }, score);
        }

        // PUT: api/scores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScore(int id, Score score)
        {
            if (id != score.GameId)
            {
                return BadRequest();
            }

            _context.Entry(score).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreExists(id))
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

        // DELETE: api/scores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScore(int id)
        {
            var score = await _context.Scores.FindAsync(id);
            if (score == null)
            {
                return NotFound();
            }

            _context.Scores.Remove(score);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/scores/playerinfo?id=1&major=ComputerScience&name=JohnDoe
        [HttpGet("playerinfo")]
        public async Task<ActionResult<int>> GetCountByPlayerInfo([FromQuery] string id, [FromQuery] string major, [FromQuery] string name)
        {
            // id, major, name 값에 해당하는 데이터의 개수를 구합니다.
            var count = await _context.Scores
                .Where(s => s.Id == id && s.Major == major && s.Name == name)
                .CountAsync();

            // 개수를 반환합니다.
            return Ok(count);
        }

        private bool ScoreExists(int id)
        {
            return _context.Scores.Any(e => e.GameId == id);
        }
    }
}
