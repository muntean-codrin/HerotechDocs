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

    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly DocsContext _context;

        public ExpensesController(DocsContext context)
        {
            _context = context;
        }

        // GET: api/Expenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
            var list =  await _context.Expenses.Include(a => a.Category).Include(a => a.Contract).ToListAsync();
            list.Sort((a, b) => b.Date.CompareTo(a.Date));
            return list;
        }


        // GET: api/Expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
            {
                return NotFound();
            }

            return expense;
        }

        // PUT: api/Expenses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpense(int id, Expense expense)
        {
            if (id != expense.Id)
            {
                return BadRequest();
            }

            if (expense.File != null)
            {
                _context.DatabaseFiles.Add(expense.File);
                await _context.SaveChangesAsync();
                expense.DatabaseFileId = expense.File.Id;
            }

            expense.Category = await _context.Category.FirstOrDefaultAsync(s => s.Id == expense.CategoryId);


            _context.Expenses.Update(expense);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            expense.File = null;
            return Ok(expense);
        }

        // POST: api/Expenses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            expense.Category = await _context.Category.FirstOrDefaultAsync(s => s.Id == expense.CategoryId);

            return CreatedAtAction("GetExpense", new { id = expense.Id }, expense);
        }

        // DELETE: api/Expenses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }
            if (expense.DatabaseFileId != null)
            {
                _context.DatabaseFiles.Remove(new DatabaseFile() { Id = (int)expense.DatabaseFileId });
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
    }
}
