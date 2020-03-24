using salesforce_composite.attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace salesforce_composite
{
    public class Sobject
    {
        [SalesforceSerialization]
        public SobjectAttributes Attributes { get; set; }

        [SalesforceSerialization]
        public string Id { get; set; }

        [SalesforceSerialization]
        public bool IsDeleted { get; set; }
    }

    public class SobjectAttributes
    {
        [SalesforceSerialization]
        public string Type { get; set; }

        [SalesforceSerialization]
        public string Url { get; set; }
    }
}
