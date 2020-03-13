using salesforce_composite.attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace salesforce_composite
{
    public class Sobject
    {
        [SalesforceSerialization]
        public string Id { get; set; }

        [SalesforceSerialization]
        public bool IsDeleted { get; set; }
    }
}
