using NUnit.Framework;
using Salesforce.Composite.Tests.MockSobjects;
using salesforce_composite;
using salesforce_composite.enums;
using System.Linq;

namespace Salesforce.Composite.Tests
{
    public class CompositeBuilderTests
    {
        private readonly int _salesforceApiVersion = 38;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RetrieveSobject()
        {
            var referenceId = "NewAccount";
            var accountId = "12345";

            var builder = new CompositeBuilder(_salesforceApiVersion)
                .RetrieveSobject(referenceId, accountId, out Account accountRef);

            Subrequest subrequest = builder.Subrequests.FirstOrDefault();

            Assert.IsNotNull(subrequest);
            Assert.AreEqual(SalesforceSerialization.RETRIEVE, subrequest.salesforceSerialization);
            Assert.AreEqual($"@{{{referenceId}.Name}}", accountRef.Name);
            Assert.AreEqual(referenceId, subrequest.compositeSubrequestBase.ReferenceId);
            Assert.AreEqual(CompositeHttpMethod.GET.ToString(), subrequest.compositeSubrequestBase.HttpMethod);

            var url = $"/services/data/v{_salesforceApiVersion}.0/sobjects/{typeof(Account).Name}/{accountId}";
            Assert.AreEqual(url, subrequest.compositeSubrequestBase.Url);
        }

        [Test]
        public void CreateSobject()
        {
            var referenceId = "NewAccount";

            var account = new Account
            {
                Name = "Temp"
            };

            var builder = new CompositeBuilder(_salesforceApiVersion)
                .CreateSobject(referenceId, account, out string accountRef);

            Subrequest subrequest = builder.Subrequests.FirstOrDefault();

            Assert.IsNotNull(subrequest);
            Assert.AreEqual(SalesforceSerialization.CREATE, subrequest.salesforceSerialization);
            Assert.AreEqual($"@{{{referenceId}.id}}", accountRef);
            Assert.AreEqual(referenceId, subrequest.compositeSubrequestBase.ReferenceId);
            Assert.AreEqual(CompositeHttpMethod.POST.ToString(), subrequest.compositeSubrequestBase.HttpMethod);
            Assert.AreEqual($"/services/data/v{_salesforceApiVersion}.0/sobjects/{account.GetType().Name}", subrequest.compositeSubrequestBase.Url);
        }

        [Test]
        public void fdfdf()
        {
            new CompositeBuilder(_salesforceApiVersion)

                .CreateSobject("NewAccount", new Account
                {
                    Name = "ACME",
                    EmployeeCount = 100
                }, out string accountId)

                .CreateSobject("NewContact", new Contact
                {
                    AccountId = accountId,
                    FirstName = "Bugs",
                    LastName = "Bunny"
                }, out string contactId)

                .PatchSobject("UpdateAccount", new Account
                {
                    Id = accountId,
                    EmployeeCount = 10
                })

                .RetrieveSobject<Contact>("NewContactInfo", contactId)
                .RetrieveSobject<Account>("NewAccountInfo", accountId)
                
                .DeleteSobject<Account>("DeleteAccount", accountId)

                .Execute();
        }

        [Test]
        public void UpdateSobjectWithNullProperties()
        {
            new CompositeBuilder(_salesforceApiVersion)
                .UpdateSobject<Account>("UpdateAccount", new Account
                {
                    Id = "Test",
                    EmployeeCount = 100
                })
                .Execute();
        }
    }

    public class TestDummy
    {
        private string _privateTestField;

        public string PublicTestField;

        public TestDummy()
        {
            this._privateTestField = "this is test (private field)";
            this.PublicTestField = "this is test (public field)";

            this.Test1 = "this is test #1";
            this.Test2 = "this is test #2";
            this.Test3 = "this is test #3";
        }

        public string Test1 { get; set; }
        public string Test2 { get; private set; }
        private string Test3 { get; set; }
    }
}