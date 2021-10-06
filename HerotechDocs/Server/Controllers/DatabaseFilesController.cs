using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HerotechDocs.Server;
using HerotechDocs.Shared;
using Microsoft.AspNetCore.Authorization;

namespace HerotechDocs.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DatabaseFilesController : ControllerBase
    {
        private readonly DocsContext _context;

        public DatabaseFilesController(DocsContext context)
        {
            _context = context;
        }

        // GET: api/DatabaseFiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DatabaseFile>>> GetDatabaseFiles()
        {
            return await _context.DatabaseFiles.ToListAsync();
        }

        // GET: api/DatabaseFiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DatabaseFile>> GetDatabaseFile(int id)
        {
            var databaseFile = await _context.DatabaseFiles.FindAsync(id);

            if (databaseFile == null)
            {
                return NotFound();
            }

            return databaseFile;
        }

        // PUT: api/DatabaseFiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDatabaseFile(int id, DatabaseFile databaseFile)
        {
            if (id != databaseFile.Id)
            {
                return BadRequest();
            }

            _context.Entry(databaseFile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DatabaseFileExists(id))
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

        // POST: api/DatabaseFiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DatabaseFile>> PostDatabaseFile(DatabaseFile databaseFile)
        {
            _context.DatabaseFiles.Add(databaseFile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDatabaseFile", new { id = databaseFile.Id }, databaseFile);
        }

        // DELETE: api/DatabaseFiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatabaseFile(int id)
        {
            var databaseFile = await _context.DatabaseFiles.FindAsync(id);
            if (databaseFile == null)
            {
                return NotFound();
            }

            _context.DatabaseFiles.Remove(databaseFile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DatabaseFileExists(int id)
        {
            return _context.DatabaseFiles.Any(e => e.Id == id);
        }
    }
}
