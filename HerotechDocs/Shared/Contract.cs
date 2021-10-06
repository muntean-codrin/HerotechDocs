using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HerotechDocs.Shared
{
    public class Contract
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Sponsor { get; set; }
        public double Amount { get; set; }
        public string Purpose { get; set; }
        public int? DatabaseFileId { get; set; }
        public bool Concluded { get; set; }
        public DatabaseFile File { get; set; }
    }
}
