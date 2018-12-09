using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyfisIntegrator.Entities
{
    class ISPOSMstItemPrice
    {
        public Int32 ArticleId { get; set; }
        public String PriceDescription { get; set; }
        public Decimal Price { get; set; }
        public String Remarks { get; set; }
    }
}
