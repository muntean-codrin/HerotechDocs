using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerotechDocs.Shared
{
    public class DatabaseFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; }
        public byte[] Data { get; set; }
    }
}
