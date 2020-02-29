using NUnit.Framework;
using Salesforce.Composite.Tests.MockSobjects;
using salesforce_composite;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Salesforce.Composite.Tests
{
    public class CompositeBuilderTests
    {
        private AppSettings _appSettings;
        private static HttpClient _client;
        private CompositeBuilder _builder;

        [OneTimeSetUp]
        public void SingleSetup()
        {
            var appSettings = new AppSettings();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
                
            config.GetSection("AppSettings").Bind(appSettings);

            _appSettings = appSettings;

            _client = new HttpClient
            {
                BaseAddress = new System.Uri(_appSettings.SalesforceDomain)
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _appSettings.SalesforceAccessToken);
        }

        [SetUp]
        public void Setup()
        {
            _builder = new CompositeBuilder(_client, _appSettings.SalesforceApiVersion);
        }

        [Test]
        public void Test1()
        {
            var test = new TestDummy().PrependValueToStringProperties("TestPrepend");
            Assert.AreEqual("@{TestPrepend.Test1}", test.Test1);

            //new CompositeBuilder()
        }

        [Test]
        public async Task fdfdf()
        {
            var builder = _builder

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
                
                .DeleteSobject<Account>("DeleteAccount", accountId);

            var result = await builder.ExecuteAsync();
            
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