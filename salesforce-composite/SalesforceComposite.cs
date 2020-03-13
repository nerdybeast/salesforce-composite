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
        //private string BaseUrl => $"{SalesforceDomain}/services/data/v{SalesforceApiVersion}.0/composite/";
        private static HttpClient _client;

        public SalesforceComposite(int salesforceApiVersion, string salesforceDomain, string salesforceAccessToken)
        {
            this.SalesforceApiVersion = salesforceApiVersion;
            this.SalesforceDomain = salesforceDomain;

            _client = new HttpClient
            {
                BaseAddress = new Uri(salesforceDomain)
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", salesforceAccessToken);
        }

        // public async Task<List<CompositeSubrequestResult>> SendAsync(List<CompositeSubrequestBase> compositeRequests, bool allOrNone = true)
        // {
        //     var payload = new CompositeRequestBody
        //     {
        //         AllOrNone = allOrNone,
        //         CompositeRequest = compositeRequests
        //     };

        //     HttpResponseMessage response = await _client.PostAsJsonAsync("/", payload);
        //     CompositeResponseBody compositeResponseBody = await response.Content.ReadAsAsync<CompositeResponseBody>();
        //     return compositeResponseBody.CompositeResponse;
        // }

    }
}
