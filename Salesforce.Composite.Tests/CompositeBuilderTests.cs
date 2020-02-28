using NUnit.Framework;
using Salesforce.Composite.Tests.MockSobjects;
using salesforce_composite;

namespace Salesforce.Composite.Tests
{
    public class CompositeBuilderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var test = new TestDummy().PrependValueToStringProperties("TestPrepend");
            Assert.AreEqual("@{TestPrepend.Test1}", test.Test1);

            //new CompositeBuilder()
        }

        [Test]
        public void fdfdf()
        {
            new CompositeBuilder()

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
            new CompositeBuilder()
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