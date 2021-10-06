using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerotechDocs.Shared
{
    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int ContractId { get; set; }
        public int? DatabaseFileId { get; set; }
        public virtual DatabaseFile File { get; set; }
        public virtual Contract Contract { get; set; }
    }
}
