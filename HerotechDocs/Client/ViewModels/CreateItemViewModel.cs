using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace HerotechDocs.Client.ViewModels
{
    public class CreateItemViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
