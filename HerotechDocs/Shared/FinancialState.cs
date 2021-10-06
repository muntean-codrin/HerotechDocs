using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerotechDocs.Shared
{
    public class FinancialState
    {
        public double Earnt { get; set; }
        public double Spent { get; set; }

        public string Percentage()
        {
            return (Spent * 100 / Earnt).ToString("F2");
        }
    }
}
