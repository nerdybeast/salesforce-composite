using salesforce_composite;
using salesforce_composite.attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salesforce.Composite.Tests.MockSobjects
{
    public class Account : Sobject
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [SalesforceName("NumberOfEmployees")]
        public int EmployeeCount { get; set; }

        [SalesforceIgnore]
        public bool ShouldCancelAccount { get; set; }
    }
}
