namespace Salesforce.Composite.Tests
{
	public class AppSettings {

		/// <summary>
		/// The domain ONLY of the Salesforce environment, example: "https://my-company.salesforce.com"
		/// </summary>
		public string SalesforceDomain { get; set; }

		public string SalesforceAccessToken { get; set; }
		public int SalesforceApiVersion { get; set; }
	}
}