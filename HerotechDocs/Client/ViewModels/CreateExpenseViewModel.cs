using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HerotechDocs.Client.ViewModels
{
    public class CreateExpenseViewModel
    {
        public CreateExpenseViewModel()
        {
            Date = DateTime.Now;
        }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Supplier { get; set; }
        [Required]
        public double? Price { get; set; }
        public IBrowserFile File { get; set; }
        [Required]
        public string Description { get; set; }
        public string ReceiptId { get; set; }

        [Required]
        public string CategoryId { get; set; }
        [Required]
        public int? ContractId { get; set; }

    }
}
