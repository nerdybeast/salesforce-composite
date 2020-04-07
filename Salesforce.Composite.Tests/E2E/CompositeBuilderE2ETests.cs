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
using FluentAssertions;
using System.Net;

namespace Salesforce.Composite.Tests.E2E
{
	[TestFixture]
	[Explicit]
	public class CompositeBuilderE2ETests
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
		public async Task ExecuteAsync_SingleBuilder_HappyPath()
		{
			var expectedEmployeeCount = 10;

			var expectedAccount = new Account
			{
				Name = "ACME",
				EmployeeCount = 100
			};

			_builder.CreateSobject("NewAccount", expectedAccount, out string accountIdReference);

			var expectedContact = new Contact
			{
				AccountId = accountIdReference,
				FirstName = "Bugs",
				LastName = "Bunny"
			};

			_builder
			
				.CreateSobject("NewContact", expectedContact, out string contactIdReference)

				.PatchSobject("UpdateAccount", new Account
				{
					Id = accountIdReference,
					EmployeeCount = expectedEmployeeCount
				})

				.RetrieveSobject<Contact>("NewContactInfo", contactIdReference)
				.RetrieveSobject<Account>("NewAccountInfo", accountIdReference)
				.DeleteSobject<Account>("DeleteAccount", accountIdReference);

			var results = await _builder.ExecuteAsync();
			
			CompositeSubrequestResult newAccountResult = results.FirstOrDefault(x => x.ReferenceId == "NewAccount");
			newAccountResult.Should().NotBeNull();
			newAccountResult.RawBody.Should().NotBeNull();
			newAccountResult.HttpStatusCode.Should().Be(HttpStatusCode.Created);
			
			var accountCreationResult = newAccountResult.Body<CreateResponseModel>();
			accountCreationResult.Should().NotBeNull();
			accountCreationResult.Id.Should().NotBeNullOrEmpty();
			accountCreationResult.Success.Should().BeTrue();

			CompositeSubrequestResult newContactResult = results.FirstOrDefault(x => x.ReferenceId == "NewContact");
			newContactResult.Should().NotBeNull();
			newContactResult.RawBody.Should().NotBeNull();
			newContactResult.HttpStatusCode.Should().Be(HttpStatusCode.Created);
			
			var contactCreationResult = newContactResult.Body<CreateResponseModel>();
			contactCreationResult.Should().NotBeNull();
			contactCreationResult.Id.Should().NotBeNullOrEmpty();
			contactCreationResult.Success.Should().BeTrue();

			CompositeSubrequestResult accountUpdateResult = results.FirstOrDefault(x => x.ReferenceId == "UpdateAccount");
			accountUpdateResult.Should().NotBeNull();
			accountUpdateResult.RawBody.Should().BeNull(); //Updates do not return a response body
			accountUpdateResult.HttpStatusCode.Should().Be(HttpStatusCode.NoContent);

			CompositeSubrequestResult accountInfo = results.FirstOrDefault(x => x.ReferenceId == "NewAccountInfo");
			accountInfo.Should().NotBeNull();
			accountInfo.RawBody.Should().NotBeNull();
			accountInfo.HttpStatusCode.Should().Be(HttpStatusCode.OK);

			var account = accountInfo.Body<Account>();
			account.Should().NotBeNull();
			account.Id.Should().NotBeNullOrEmpty();

			//This property is called "NumberOfEmployees" when it comes back from Salesforce
			//Need to deserialize that property into this one...
			//Possibly in the Body<T>() method we need a custom deserializer that will factor in our
			//[SalesforceName] attribute...
			//account.EmployeeCount.Should().Be(expectedEmployeeCount);

			CompositeSubrequestResult contactInfo = results.FirstOrDefault(x => x.ReferenceId == "NewContactInfo");
			contactInfo.Should().NotBeNull();
			contactInfo.RawBody.Should().NotBeNull();
			contactInfo.HttpStatusCode.Should().Be(HttpStatusCode.OK);

			var contact = contactInfo.Body<Contact>();
			contact.Should().NotBeNull();
			contact.FirstName.Should().Be(expectedContact.FirstName);
			contact.LastName.Should().Be(expectedContact.LastName);
			contact.AccountId.Should().Be(account.Id);

			CompositeSubrequestResult accountDeleteResult = results.FirstOrDefault(x => x.ReferenceId == "DeleteAccount");
			accountDeleteResult.Should().NotBeNull();
			accountDeleteResult.RawBody.Should().BeNull(); //Deletes do not return a response body
			accountDeleteResult.HttpStatusCode.Should().Be(HttpStatusCode.NoContent);
		}

		// [Test]
		// public async Task MultipleBuilders()
		// {
		// 	var referenceId = "NewAccount";

		// 	var builder = new CompositeBuilder(_client, _appSettings.SalesforceApiVersion)
		// 		.CreateSobject(referenceId, new Account
		// 		{
		// 			Name = "Avengers Inc."
		// 		});

		// 	List<CompositeSubrequestResult> results = await builder.ExecuteAsync();

		// 	CompositeSubrequestResult newAccountResult = results.First(x => x.ReferenceId == referenceId);

		// 	CreateResponseModel res = ((JObject)newAccountResult.RawBody).ToObject<CreateResponseModel>();

		// 	await builder
		// 		.DeleteSobject<Account>("DeleteAccount", res.Id)
		// 		.ExecuteAsync();
		// }
	}
}