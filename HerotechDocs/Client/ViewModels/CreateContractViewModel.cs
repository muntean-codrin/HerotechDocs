using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HerotechDocs.Client.ViewModels
{
    public class CreateContractViewModel
    {
        public CreateContractViewModel()
        {
            Date = DateTime.Now;
        }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Sponsor { get; set; }
        [Required]
        public double? Amount { get; set; }
        [Required]
        public string Purpose { get; set; }
        public IBrowserFile File { get; set; }
    }
}
