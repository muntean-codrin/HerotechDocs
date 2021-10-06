using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerotechDocs.Shared
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Supplier { get; set; }
        public double Price { get; set; }
        public string ReceiptId { get; set; }
        public int? DatabaseFileId { get; set; }
        public string Description { get; set; }
        public int ContractId { get; set; }
        public string CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual Contract Contract { get; set; }
        public virtual DatabaseFile File { get; set; }
    }
}
