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
            var builder = new CompositeBuilder()

                //.CreateSobject("NewAccount", new Account
                //{
                //    Name = "ACME",
                //    EmployeeCount = 100
                //}, out Account accountRef)

                //.CreateSobject("NewContact", new Contact
                //{
                //    AccountId = accountRef.Id,
                //    FirstName = "Bugs",
                //    LastName = "Bunny"
                //}, out Contact contactRef)

                //.RetrieveSobject<Contact>("NewContactInfo", contactRef.Id)

                //.UpdateSobject("UpdateAccount", new Account
                //{
                //    Id = accountRef.Id
                //})

                //.RetrieveSobject<Account>("NewAccountInfo", accountRef.Id, out Account newAccount);

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

                .RetrieveSobject<Contact>("NewContactInfo", contactId)

                .UpdateSobject("UpdateAccount", new Account
                {
                    Id = accountId,
                    EmployeeCount = 10
                })

                .RetrieveSobject<Account>("NewAccountInfo", accountId, out Account newAccount);

            var result = builder.Execute();
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