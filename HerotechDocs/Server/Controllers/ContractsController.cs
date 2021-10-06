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
    public class ContractsController : ControllerBase
    {
        private readonly DocsContext _context;

        public ContractsController(DocsContext context)
        {
            _context = context;
        }

        // GET: api/Contracts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContracts()
        {
            var list = await _context.Contracts.ToListAsync();
            list.Sort((a, b) => b.Date.CompareTo(a.Date));
            return list;
        }

        //// GET: api/Contracts/contractId
        //public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses(int contractId)
        //{
        //    return await _context.Expenses.Include(a => a.Category).Include(a => a.Contract).Where(a => a.ContractId == contractId).ToListAsync();
        //}

        // GET: api/Contracts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return contract;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutContract(int id, Contract contract)
        {
            if (id != contract.Id)
            {
                return BadRequest();
            }


            if(contract.File != null)
            {
                _context.DatabaseFiles.Add(contract.File);
                await _context.SaveChangesAsync();
                contract.DatabaseFileId = contract.File.Id;
            }

            _context.Contracts.Update(contract);

            //_context.Entry(contract).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            contract.File = null;
            return Ok(contract);
        }

        [HttpPost]
        public async Task<ActionResult<Contract>> PostContract(Contract contract)
        {
            if(contract.File != null)
            {
                _context.DatabaseFiles.Add(contract.File);
                await _context.SaveChangesAsync();
                contract.DatabaseFileId = contract.File.Id;
            }

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContract", new { id = contract.Id }, contract);
        }

        // DELETE: api/Contracts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null || _context.Expenses.Any(a => a.ContractId == contract.Id))
            {
                return NotFound();
            }
            if(contract.DatabaseFileId != null)
            {
                _context.DatabaseFiles.Remove(new DatabaseFile() { Id = (int)contract.DatabaseFileId });
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}
