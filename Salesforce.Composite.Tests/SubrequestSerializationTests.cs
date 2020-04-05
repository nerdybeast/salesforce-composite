using Newtonsoft.Json;
using NUnit.Framework;
using Salesforce.Composite.Tests.MockSobjects;
using salesforce_composite;
using salesforce_composite.attributes;
using salesforce_composite.serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Salesforce.Composite.Tests
{
    public class SubrequestSerializationTests
    {
        private readonly int _salesforceApiVersion = 38;
        private CompositeBuilder _compositeBuilder;

        private class MockSobject : Sobject
        {
            public string Username { get; set; }

            [SalesforceIgnore]
            public string Password { get; set; }

            [SalesforceName("Security_Token__c")]
            public string SecurityToken { get; set; }

            [SalesforceSerialization(create: true, read: false, update: false, delete: false)]
            public int CreateOnlyField { get; set; }

            [SalesforceSerialization(create: false, read: true, update: false, delete: false)]
            public int ReadOnlyField { get; set; }

            [SalesforceSerialization(create: false, read: false, update: true, delete: false)]
            public int UpdateOnlyField { get; set; }

            [SalesforceSerialization(create: false, read: false, update: false, delete: true)]
            public int DeleteOnlyField { get; set; }
        }

        [SetUp]
        public void Setup()
        {
            _compositeBuilder = new CompositeBuilder(null, _salesforceApiVersion);
        }

        [Test]
        public void RetrieveSobject()
        {
            var referenceId = "TheRecord";
            var id = "12345";

            var builder = _compositeBuilder.RetrieveSobject(referenceId, id, out MockSobject objRef);

            Subrequest subrequest = builder.Subrequests.FirstOrDefault();
            Assert.IsNotNull(subrequest);

            string json = SubrequestSerialization.Serialize(subrequest, Formatting.None);

            Assert.That(json.Contains($"\"method\":\"GET\""));
            Assert.That(json.Contains($"\"url\":\"/services/data/v{_salesforceApiVersion}.0/sobjects/{typeof(MockSobject).Name}/{id}\""));
            Assert.That(json.Contains($"\"referenceId\":\"{referenceId}\""));

            Assert.That(!json.Contains("body"));
        }

        [Test]
        public void CreateSobject()
        {
            var referenceId = "TheRecord";

            MockSobject sobject = CreateDefaultMockSobject();

            var builder = _compositeBuilder.CreateSobject(referenceId, sobject);

            Subrequest subrequest = builder.Subrequests.FirstOrDefault();
            Assert.IsNotNull(subrequest);

            string json = SubrequestSerialization.Serialize(subrequest, Formatting.None);

            Assert.That(json.Contains($"\"method\":\"POST\""));
            Assert.That(json.Contains($"\"url\":\"/services/data/v{_salesforceApiVersion}.0/sobjects/{typeof(MockSobject).Name}\""));
            Assert.That(json.Contains($"\"Username\":\"{sobject.Username}\""));
            Assert.That(json.Contains($"\"Security_Token__c\":\"{sobject.SecurityToken}\""));
            Assert.That(json.Contains($"\"referenceId\":\"{referenceId}\""));
            Assert.That(json.Contains($"CreateOnlyField"));

            Assert.That(!json.Contains("Password"));
            Assert.That(!json.Contains("ReadOnlyField"));
            Assert.That(!json.Contains("UpdateOnlyField"));
            Assert.That(!json.Contains("DeleteOnlyField"));
        }

        [Test]
        public void UpdateSobject()
        {
            var referenceId = "TheRecord";
            var id = "1qa2ws3ed4rf5tg6yh";

            MockSobject sobject = CreateDefaultMockSobject(id);

            var builder = _compositeBuilder.UpdateSobject(referenceId, sobject);

            Subrequest subrequest = builder.Subrequests.FirstOrDefault();
            Assert.IsNotNull(subrequest);

            string json = SubrequestSerialization.Serialize(subrequest, Formatting.None);

            Assert.That(json.Contains($"\"method\":\"PATCH\""));
            Assert.That(json.Contains($"\"url\":\"/services/data/v{_salesforceApiVersion}.0/sobjects/{typeof(MockSobject).Name}/{id}\""));
            Assert.That(json.Contains($"\"Username\":\"{sobject.Username}\""));
            Assert.That(json.Contains($"\"Security_Token__c\":\"{sobject.SecurityToken}\""));
            Assert.That(json.Contains($"\"referenceId\":\"{referenceId}\""));
            Assert.That(json.Contains($"UpdateOnlyField"));

            Assert.That(!json.Contains("Password"));
            Assert.That(!json.Contains("ReadOnlyField"));
            Assert.That(!json.Contains("CreateOnlyField"));
            Assert.That(!json.Contains("DeleteOnlyField"));
        }

        [Test]
        public void DeleteSobject() {

            var referenceId = "TheRecord";
            var id = "1qa2ws3ed4rf5tg6yh";

            MockSobject sobject = CreateDefaultMockSobject(id);

            var builder = _compositeBuilder.DeleteSobject<MockSobject>(referenceId, id);

            Subrequest subrequest = builder.Subrequests.FirstOrDefault();
            Assert.IsNotNull(subrequest);

            string json = SubrequestSerialization.Serialize(subrequest, Formatting.None);

            Assert.That(json.Contains($"\"method\":\"DELETE\""));
            Assert.That(json.Contains($"\"url\":\"/services/data/v{_salesforceApiVersion}.0/sobjects/{typeof(MockSobject).Name}/{id}\""));
            Assert.That(json.Contains($"\"referenceId\":\"{referenceId}\""));

            Assert.That(!json.Contains("body"));
        }

        private MockSobject CreateDefaultMockSobject(string id = null) {
            return new MockSobject {
                CreateOnlyField = 100,
                DeleteOnlyField = 100,
                ReadOnlyField = 100,
                UpdateOnlyField = 100,
                Password = "project-pegasus",
                Username = "Tony Stark",
                SecurityToken = "123456",
                Id = id
            };
        }
    }
}
