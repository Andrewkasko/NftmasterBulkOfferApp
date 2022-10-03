using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFTMaster.BulkOfferApp.Model
{
    public class BulkIOUAllocationOfferBatchReturn
    {
        public string Id { get; set; }
        public List<IOUAllocationOffer> Offers { get; set; }
        public string Message { get; set; }
    }
}
