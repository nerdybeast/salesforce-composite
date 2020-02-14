using salesforce_composite.attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace salesforce_composite
{
    public class Sobject
    {
        [SalesforceSerialization(create: false, read: true, update: false, delete: false)]
        public string Id { get; set; }

        [SalesforceSerialization(create: false, read: true, update: false, delete: false)]
        public bool IsDeleted { get; set; }
    }
}
