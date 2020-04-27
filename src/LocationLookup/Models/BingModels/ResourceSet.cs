using System;
using System.Collections.Generic;
using System.Text;

namespace LocationLookup.Models.BingModels
{
   public  class ResourceSet
    {
        public int EstimatedTotal { get; set; }
        public IEnumerable<Resource> Resources { get; set; }
    }
}
