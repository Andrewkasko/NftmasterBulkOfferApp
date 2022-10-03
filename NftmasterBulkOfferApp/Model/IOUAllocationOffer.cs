using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFTMaster.BulkOfferApp.Model
{
    public class IOUAllocationOffer
    {
        public string NFTokenID { get; set; }
        public string Amount { get; set; }
        public string Destination { get; set; }
        public int Flags { get; set; }
        public string MemoType { get; set; }
        public string MemoData { get; set; }
    }
}
