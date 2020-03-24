using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Salesforce.Composite.Tests.MockSobjects;
using salesforce_composite;
using salesforce_composite.ResponseModels;
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

        [OneTimeSetUp]
        public void Setup()
        {
            _data = File.ReadAllText("./SampleData/CompositeResponse.json");
        }

        [Test]
        public void asdasd()
        {
            var responseBody = JsonConvert.DeserializeObject<CompositeResponseBody>(_data);

            var newAccountResult = responseBody.CompositeResponse.First(x => x.ReferenceId == "NewAccount");
            var newAccountModel = ((JObject)newAccountResult.Body).ToObject<CreateResponseModel>();

            var newContactInfoResult = responseBody.CompositeResponse.First(x => x.ReferenceId == "NewContactInfo");
            var newContactInfoModel = ((JObject)newContactInfoResult.Body).ToObject<Contact>();
        }
    }
}
