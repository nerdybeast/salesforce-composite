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

        public string AccountNumber { get; set; }

        public string Description { get; set; }

        [SalesforceName("Total_Employees__c")]
        public int EmployeeCount { get; set; }

        [SalesforceIgnore]
        public bool ShouldCancelAccount { get; set; }

        //[SalesforceName("CreatedBy")]
        [SalesforceSerialization]
        public string CreatedById { get; set; }
    }
}
