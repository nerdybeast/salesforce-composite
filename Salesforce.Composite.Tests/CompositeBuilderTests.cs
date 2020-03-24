using NUnit.Framework;
using Salesforce.Composite.Tests.MockSobjects;
using salesforce_composite;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using salesforce_composite.enums;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using salesforce_composite.ResponseModels;

namespace Salesforce.Composite.Tests
{
    public class CompositeBuilderTests
    {
        private readonly int _salesforceApiVersion = 38;

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
        public void RetrieveSobject()
        {
            var referenceId = "NewAccount";
            var accountId = "12345";

            _builder.RetrieveSobject(referenceId, accountId, out Account accountRef);

            Subrequest subrequest = _builder.Subrequests.FirstOrDefault();

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

            _builder.CreateSobject(referenceId, account, out string accountRef);

            Subrequest subrequest = _builder.Subrequests.FirstOrDefault();

            Assert.IsNotNull(subrequest);
            Assert.AreEqual(SalesforceSerialization.CREATE, subrequest.salesforceSerialization);
            Assert.AreEqual($"@{{{referenceId}.id}}", accountRef);
            Assert.AreEqual(referenceId, subrequest.compositeSubrequestBase.ReferenceId);
            Assert.AreEqual(CompositeHttpMethod.POST.ToString(), subrequest.compositeSubrequestBase.HttpMethod);
            Assert.AreEqual($"/services/data/v{_salesforceApiVersion}.0/sobjects/{account.GetType().Name}", subrequest.compositeSubrequestBase.Url);
        }

        [Test]
        public async Task fdfdf()
        {
            _builder

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

            var result = await _builder.ExecuteAsync();
            
        }

        [Test]
        public async Task MultipleBuilders()
        {
            var referenceId = "NewAccount";

            var builder = new CompositeBuilder(_client, _salesforceApiVersion)
                .CreateSobject(referenceId, new Account
                {
                    Name = "Avengers Inc."
                });

            List<CompositeSubrequestResult> results = await builder.ExecuteAsync();

            CompositeSubrequestResult newAccountResult = results.First(x => x.ReferenceId == referenceId);

            CreateResponseModel res = (CreateResponseModel)newAccountResult.Body;

            await builder
                .DeleteSobject<Account>("DeleteAccount", res.Id)
                .ExecuteAsync();
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