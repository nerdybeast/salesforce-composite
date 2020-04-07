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
using Newtonsoft.Json.Linq;

namespace Salesforce.Composite.Tests
{
	public class CompositeBuilderTests
	{
		private AppSettings _appSettings;
		private static HttpClient _client = new HttpClient();
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

			var url = $"/services/data/v{_appSettings.SalesforceApiVersion}.0/sobjects/{typeof(Account).Name}/{accountId}";
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
			Assert.AreEqual($"/services/data/v{_appSettings.SalesforceApiVersion}.0/sobjects/{account.GetType().Name}", subrequest.compositeSubrequestBase.Url);
		}
	}
}