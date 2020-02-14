using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace salesforce_composite
{
    public class SalesforceComposite
    {
        private readonly int SalesforceApiVersion;
        private readonly string SalesforceDomain;
        private string BaseUrl => $"{SalesforceDomain}/services/data/v{SalesforceApiVersion}.0/composite/";
        private HttpClient _client;

        public SalesforceComposite(int SalesforceApiVersion, string SalesforceDomain, string SalesforceAccessToken)
        {
            this.SalesforceApiVersion = SalesforceApiVersion;
            this.SalesforceDomain = SalesforceDomain;

            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SalesforceAccessToken);
        }

        public async Task<List<CompositeSubrequestResult>> SendAsync(List<CompositeSubrequestBase> compositeRequests, bool allOrNone = true)
        {
            var payload = new CompositeRequestBody
            {
                AllOrNone = allOrNone,
                CompositeRequest = compositeRequests
            };

            HttpResponseMessage response = await _client.PostAsJsonAsync("/", payload);
            CompositeResponseBody compositeResponseBody = await response.Content.ReadAsAsync<CompositeResponseBody>();
            return compositeResponseBody.CompositeResponse;
        }
    }
}
