using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HerotechDocs.Server;
using HerotechDocs.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

namespace HerotechDocs.Server.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ContractDetailsController : ControllerBase
    {
        private readonly DocsContext _context;

        public ContractDetailsController(DocsContext context)
        {
            _context = context;
        }

        // GET: api/ContractDetails/1
        [HttpGet("{contractId}")]
        public async Task<ActionResult<DatabaseFile>> Get(int contractId)
        {
            var contract = _context.Contracts.FirstOrDefault(c => c.Id == contractId);
            var expenses = await _context.Expenses.Where(e => e.ContractId == contractId).ToListAsync();
            using (var package = new ExcelPackage())
            {
                //Add a new worksheet to the empty workbook
                var worksheet = package.Workbook.Worksheets.Add("Sponsorizare");
                //Add the headers
                worksheet.Cells[1, 1].Value = "ID Factura";
                worksheet.Cells[1, 2].Value = "Furnizor";
                worksheet.Cells[1, 3].Value = "Data";
                worksheet.Cells[1, 4].Value = "Pret (RON)";
                worksheet.Cells[1, 5].Value = "Descriere";
                worksheet.Cells[1, 6].Value = "Categorie";

                int i = 2;
                foreach(var expense in expenses)
                {
                    worksheet.Cells[i, 1].Value = expense.ReceiptId;
                    worksheet.Cells[i, 2].Value = expense.Supplier;
                    worksheet.Cells[i, 3].Value = expense.Date.ToString("d");
                    worksheet.Cells[i, 4].Value = expense.Price;
                    worksheet.Cells[i, 5].Value = expense.Description;
                    worksheet.Cells[i, 6].Value = expense.CategoryId;

                    i++;
                }

                worksheet.Cells[$"D{i}"].Formula = $"SUM(D2:D{i - 1})";

                //Ok now format the values;
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                worksheet.Cells[$"A2:A{i-1}"].Style.Numberformat.Format = "@";   //Format as text

                //There is actually no need to calculate, Excel will do it for you, but in some cases it might be useful. 
                //For example if you link to this workbook from another workbook or you will open the workbook in a program that hasn't a calculation engine or 
                //you want to use the result of a formula in your program.
                worksheet.Calculate();

                worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                // Lets set the header text 
                worksheet.HeaderFooter.OddHeader.CenteredText = $"&24&U&\"Arial,Regular Bold\" Sponsorizare {contract.Sponsor} {contract.Date.ToString("d")}";
                // Add the page number to the footer plus the total number of pages
                worksheet.HeaderFooter.OddFooter.RightAlignedText =
                    string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                // Add the sheet name to the footer
                worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                // Add the file path to the footer
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

                // Set some document properties
                package.Workbook.Properties.Title = "Sponsorizare";
                package.Workbook.Properties.Author = "Muntean Codrin";
                
                // Set some extended property values
                package.Workbook.Properties.Company = "Herotech";
                var file = new DatabaseFile();
                file.Data = package.GetAsByteArray();
                return file;
            }

        }
    }
}
