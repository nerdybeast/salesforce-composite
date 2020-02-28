using salesforce_composite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salesforce.Composite.Tests.MockSobjects
{
    public class Contact : Sobject
    {
        public string AccountId { get; set; }
        public DateTime? Birthdate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
