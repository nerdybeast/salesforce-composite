using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace salesforce_composite.tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void kjh()
        {
            var test = new TestDummy().PrependValueToStringProperties("TestPrepend.");
        }

        [Test]
        public async Task Good()
        {
            var account = new Account
            {
                Name = "Temp"
            };

            var contact = new Contact
            {
                FirstName = "Tony",
                LastName = "Stark",
                AccountId = account.Id
            };



            //new CompositeBuilder()
            //    .CreateSobject("NewAccount", "Account", account)
            //    .RetrieveSobject("TheAccount", "Account", "{@NewAccount.Id}");

            //new CompositeBuilder()
            //    .CreateSobject("NewAccount", "Account", account, out string newAccountId)
            //    .RetrieveSobject("TheAccount", "Account", newAccountId);

            new CompositeBuilder()
                .CreateSobject("NewAccount", account, out string newAccountId)
                .RetrieveSobject<Account>("TheAccount", newAccountId);

            

        }

        //public void x()
        //{
        //    var accountWrapper = Create("MyNewAccount", new Account
        //    {
        //        Name = "Temp"
        //    });

        //    var contact = Create("MyNewContact", new Contact
        //    {
        //        FirstName = "Tony",
        //        LastName = "Stark",
        //        AccountId = account.Id,
        //        Title = $"CEO of {account.Name}"
        //    });

        //    new CompositeBuilder()
        //        .CreateSobject(account)
        //        .CreateSobject(contact)
        //        .UpdateSobject(Create(new Account
        //        {
        //            Id = account.Id,
        //            Primary_Contact__c = contact.Id
        //        }))
        //        .RetrieveSobject<Account>(account.Id)
        //        .RetrieveSobject<Contact>(contact.Id)
        //        .Send();
        //}

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

/*
{
    "allOrNone": true,
    "compositeRequest" : [{
        "method" : "POST",
        "url" : "/services/data/v38.0/sobjects/Account",
        "referenceId" : "NewAccount",
        "body" : {  
            "Name" : "Stark Industries",
            "BillingStreet" : "890 5th Ave",
            "BillingCity" : "New York",
            "BillingState" : "NY",
            "Industry" : "Technology"
        }
    },{
        "method" : "GET",
        "referenceId" : "NewAccountInfo",
        "url" : "/services/data/v38.0/sobjects/Account/@{NewAccount.id}"
    },{
    	"method": "POST",
    	"referenceId" : "NewPrimaryContact",
    	"url" : "/services/data/v38.0/sobjects/Contact",
    	"body" : {
    		"Firstname": "Tony",
            "lastname" : "Stark",
            "Title" : "CEO of @{NewAccountInfo.Name}",
            "MailingStreet" : "@{NewAccountInfo.BillingStreet}",
            "MailingCity" : "@{NewAccountInfo.BillingAddress.city}",
            "MailingState" : "@{NewAccountInfo.BillingState}",
            "AccountId" : "@{NewAccountInfo.Id}",
            "Email" : "tony.stark@StarkIndustries.com",
            "Phone" : "1234567890"
        }
    },{
    	"method": "PATCH",
    	"referenceId": "AccountUpdate",
    	"url": "/services/data/v38.0/sobjects/Account/@{NewAccount.id}",
    	"body": {
    		"Primary_Contact__c": "@{NewPrimaryContact.id}"
    	}
    },{
        "method" : "GET",
        "referenceId" : "UpdatedAccountInfo",
        "url" : "/services/data/v38.0/sobjects/Account/@{NewAccount.id}"
    },{
        "method" : "DELETE",
        "referenceId" : "DeletePrimaryContact",
        "url" : "/services/data/v38.0/sobjects/Contact/@{NewPrimaryContact.id}"
    },{
        "method" : "DELETE",
        "referenceId" : "DeleteAccount",
        "url" : "/services/data/v38.0/sobjects/Account/@{NewAccount.id}"
    }]
} 
*/
