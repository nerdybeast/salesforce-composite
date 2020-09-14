using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Salesforce.Composite.Tests.MockSobjects;
using salesforce_composite;
using salesforce_composite.ResponseModels;
using salesforce_composite.serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Salesforce.Composite.Tests
{
    public class SubrequestDeserializationTests
    {
        private string _data;
        private string _sobjectJson;

        [OneTimeSetUp]
        public void Setup()
        {
            _data = File.ReadAllText("./SampleData/CompositeResponse.json");
            _sobjectJson = File.ReadAllText("./SampleData/Sobject.json");
        }

        [Test]
        public void DefaultDeserialization()
        {
            var responseBody = JsonConvert.DeserializeObject<CompositeResponseBody>(_data);
            List<CompositeSubrequestResult> results = responseBody.CompositeResponse;

            var newAccountResult = results.First(x => x.ReferenceId == "NewAccount");
            var newAccountCreationResult = newAccountResult.Body<CreateResponseModel>();

            var newContactInfoResult = results.First(x => x.ReferenceId == "NewContactInfo");
            var newContactInfoModel = newContactInfoResult.Body<Contact>();
        }

        [Test]
        public void DeserializeSobject()
        {
            var sobjectCreationConverter = new SobjectCreationConverter<Contact>();
            Contact contact = JsonConvert.DeserializeObject<Contact>(_sobjectJson, sobjectCreationConverter);
        }
    }
}
