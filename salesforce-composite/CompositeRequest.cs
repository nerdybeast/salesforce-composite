using salesforce_composite.enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using salesforce_composite.serialization;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace salesforce_composite
{
    /// <summary>
    /// https://developer.salesforce.com/docs/atlas.en-us.api_rest.meta/api_rest/using_resources_working_with_records.htm
    /// </summary>
    public class CompositeBuilder
    {
        private readonly List<Subrequest> _requests = new List<Subrequest>();
        private readonly bool _allOrNone;
        private static HttpClient _client;
        private int _salesforceApiVersion;

        public CompositeBuilder(HttpClient client, int salesforceApiVersion, bool allOrNone = true)
        {
            _client = client;
            _salesforceApiVersion = salesforceApiVersion;
            _allOrNone = allOrNone;
        }

        public CompositeBuilder RetrieveSobject<T>(string referenceId, string sobjectId) where T : Sobject
        {
            var action = new RetrieveSobject(_salesforceApiVersion, referenceId, typeof(T).Name, sobjectId);
            _requests.Add(new Subrequest(SalesforceSerialization.RETRIEVE, action));

            return this;
        }

        public CompositeBuilder RetrieveSobject<T>(string referenceId, string sobjectId, out T sobjectReference) where T : Sobject, new()
        {
            var action = new RetrieveSobject(_salesforceApiVersion, referenceId, typeof(T).Name, sobjectId);
            _requests.Add(new Subrequest(SalesforceSerialization.RETRIEVE, action));
            
            sobjectReference = new T().PrependValueToStringProperties(referenceId);
            
            return this;
        }

        public CompositeBuilder CreateSobject<T>(string referenceId, T sobject, out string sobjectIdReference) where T : Sobject, new()
        {
            var action = new CreateSobject<T>(_salesforceApiVersion, referenceId, typeof(T).Name, sobject);
            _requests.Add(new Subrequest(SalesforceSerialization.CREATE, action));

            sobjectIdReference = $"@{{{referenceId}.id}}";

            return this;
        }

        public CompositeBuilder UpdateSobject<T>(string referenceId, T sobject) where T : Sobject, new()
        {
            var action = new UpdateSobject<T>(_salesforceApiVersion, referenceId, typeof(T).Name, sobject);
            _requests.Add(new Subrequest(SalesforceSerialization.UPDATE, action));
            return this;
        }

        public CompositeBuilder PatchSobject<T>(string referenceId, T sobject) where T : Sobject, new()
        {
            var action = new UpdateSobject<T>(_salesforceApiVersion, referenceId, typeof(T).Name, sobject);
            _requests.Add(new Subrequest(SalesforceSerialization.PATCH, action));
            return this;
        }

        public CompositeBuilder DeleteSobject<T>(string referenceId, string sobjectId) where T : Sobject
        {
            var action = new DeleteSobject(_salesforceApiVersion, referenceId, typeof(T).Name, sobjectId);
            _requests.Add(new Subrequest(SalesforceSerialization.DELETE, action));

            return this;
        }

        public async Task<List<CompositeSubrequestResult>> ExecuteAsync()
        {
            var json = _requests.Select(req => 
            {
                var nullValueHandling = NullValueHandling.Ignore;

                if(req.salesforceSerialization == SalesforceSerialization.UPDATE)
                {
                    nullValueHandling = NullValueHandling.Include;
                }

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new SalesforceResolver(req.salesforceSerialization),
                    NullValueHandling = nullValueHandling
                };

                var x = JsonConvert.SerializeObject(req.compositeSubrequestBase, Formatting.Indented, settings);

                return x;
            });

            var body = $"{{ \"allOrNone\": {_allOrNone.ToString().ToLower()}, \"compositeRequest\": [{string.Join(",", json)}] }}";

            var content = new StringContent(body);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await _client.PostAsync($"/services/data/v{_salesforceApiVersion}.0/composite/", content);
            var result = await response.Content.ReadAsStringAsync();

            var responseBody = JsonConvert.DeserializeObject<CompositeResponseBody>(result);

            return responseBody.CompositeResponse;
        }
    }

    public class RetrieveSobject : CompositeSobjectSubrequest<Sobject>
    {
        public RetrieveSobject(int salesforceApiVersion, string referenceId, string sobjectType, string sobjectId)
        {
            base.ReferenceId = referenceId;
            HttpMethod = CompositeHttpMethod.GET.ToString();
            Url = $"/services/data/v{salesforceApiVersion}.0/sobjects/{sobjectType}/{sobjectId}";
        }
    }

    public class CreateSobject<T> : CompositeSobjectSubrequest<T> where T : Sobject
    {
        public CreateSobject(int salesforceApiVersion, string referenceId, string sobjectType, T sobject)
        {
            base.ReferenceId = referenceId;
            HttpMethod = CompositeHttpMethod.POST.ToString();
            Url = $"/services/data/v{salesforceApiVersion}.0/sobjects/{sobjectType}";
            Body = sobject;
        }
    }

    public class UpdateSobject<T> : CompositeSobjectSubrequest<T> where T : Sobject
    {
        public UpdateSobject(int salesforceApiVersion, string referenceId, string sobjectType, T sobject)
        {
            base.ReferenceId = referenceId;
            HttpMethod = CompositeHttpMethod.PATCH.ToString();
            Url = $"/services/data/v{salesforceApiVersion}.0/sobjects/{sobjectType}/{sobject.Id}";
            Body = sobject;
        }
    }

    public class DeleteSobject : CompositeSobjectSubrequest<Sobject>
    {
        public DeleteSobject(int salesforceApiVersion, string referenceId, string sobjectType, string sobjectId)
        {
            base.ReferenceId = referenceId;
            HttpMethod = CompositeHttpMethod.DELETE.ToString();
            Url = $"/services/data/v{salesforceApiVersion}.0/sobjects/{sobjectType}/{sobjectId}";
        }
    }
}
