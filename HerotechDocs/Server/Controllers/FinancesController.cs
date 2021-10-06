using HerotechDocs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HerotechDocs.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FinancesController : ControllerBase
    {
        private readonly DocsContext _context;

        public FinancesController(DocsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<FinancialState> Get()
        {
            var state = new FinancialState();
            state.Earnt = _context.Contracts.Sum(c => c.Amount);
            state.Spent = _context.Expenses.Sum(e => e.Price);

            return Ok(state);
        }
    }
}
