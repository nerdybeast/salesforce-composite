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
using salesforce_composite.ResponseModels;

namespace salesforce_composite
{
    /// <summary>
    /// https://developer.salesforce.com/docs/atlas.en-us.api_rest.meta/api_rest/using_resources_working_with_records.htm
    /// </summary>
    public class CompositeBuilder
    {
        public List<Subrequest> Subrequests { get; } = new List<Subrequest>();
        private readonly int _salesforceApiVersion;
        private readonly bool _allOrNone;
        private static HttpClient _client;

        public CompositeBuilder(HttpClient client, int salesforceApiVersion, bool allOrNone = true)
        {
            _client = client;
            _salesforceApiVersion = salesforceApiVersion;
            _allOrNone = allOrNone;
        }

        public CompositeBuilder RetrieveSobject<T>(string referenceId, string sobjectId) where T : Sobject
        {
            var action = new RetrieveSobject(_salesforceApiVersion, referenceId, typeof(T).Name, sobjectId);
            Subrequests.Add(new Subrequest(SalesforceSerialization.RETRIEVE, action));

            return this;
        }

        public CompositeBuilder RetrieveSobject<T>(string referenceId, string sobjectId, out T sobjectReference) where T : Sobject, new()
        {
            var action = new RetrieveSobject(_salesforceApiVersion, referenceId, typeof(T).Name, sobjectId);
            Subrequests.Add(new Subrequest(SalesforceSerialization.RETRIEVE, action));
            
            sobjectReference = new T().PrependValueToStringProperties(referenceId);
            
            return this;
        }

        public CompositeBuilder CreateSobject<T>(string referenceId, T sobject) where T : Sobject
        {
            var action = new CreateSobject<T>(_salesforceApiVersion, referenceId, typeof(T).Name, sobject);
            Subrequests.Add(new Subrequest(SalesforceSerialization.CREATE, action));

            return this;
        }

        public CompositeBuilder CreateSobject<T>(string referenceId, T sobject, out string sobjectIdReference) where T : Sobject
        {
            var action = new CreateSobject<T>(_salesforceApiVersion, referenceId, typeof(T).Name, sobject);
            Subrequests.Add(new Subrequest(SalesforceSerialization.CREATE, action));

            sobjectIdReference = $"@{{{referenceId}.id}}";

            return this;
        }

        /// <summary>
        /// Runs a full sobject update request to Salesforce, any null properties will be sent in the request
        /// which will null those values out inside of Salesforce.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="sobject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeBuilder UpdateSobject<T>(string referenceId, T sobject) where T : Sobject
        {
            var action = new UpdateSobject<T>(_salesforceApiVersion, referenceId, typeof(T).Name, sobject);
            Subrequests.Add(new Subrequest(SalesforceSerialization.UPDATE, action));
            return this;
        }

        /// <summary>
        /// Runs a soft update request to Salesforce, any null properties will be witheld from the request.
        /// Example:
        /// 
        /// PatchSobject("AccountPatch", new Account {
        ///     Name: "ACME",
        ///     Description: null //This property will be ignored because it's null
        /// });
        /// 
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="sobject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CompositeBuilder PatchSobject<T>(string referenceId, T sobject) where T : Sobject
        {
            var action = new UpdateSobject<T>(_salesforceApiVersion, referenceId, typeof(T).Name, sobject);
            Subrequests.Add(new Subrequest(SalesforceSerialization.PATCH, action));
            return this;
        }

        public CompositeBuilder DeleteSobject<T>(string referenceId, string sobjectId) where T : Sobject
        {
            var action = new DeleteSobject(_salesforceApiVersion, referenceId, typeof(T).Name, sobjectId);
            Subrequests.Add(new Subrequest(SalesforceSerialization.DELETE, action));

            return this;
        }

        public void Clear()
        {
            Subrequests.Clear();
        }

        public async Task<List<CompositeSubrequestResult>> ExecuteAsync()
        {
            var json = Subrequests.Select(req => SubrequestSerialization.Serialize(req));

            var body = $"{{ \"allOrNone\": {_allOrNone.ToString().ToLower()}, \"compositeRequest\": [{string.Join(",", json)}] }}";

            var content = new StringContent(body);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await _client.PostAsync($"/services/data/v{_salesforceApiVersion}.0/composite/", content);
            var result = await response.Content.ReadAsStringAsync();

            if(!response.IsSuccessStatusCode) {

                var ex = new Exception("Salesforce communication error");

                var errors = JsonConvert.DeserializeObject<List<SalesforceHtpError>>(result);

                foreach(var error in errors) {
                    ex.Data[error.ErrorCode] = error.Message;
                }

                throw ex;
            }

            Clear();

            var responseBody = JsonConvert.DeserializeObject<CompositeResponseBody>(result);
            //var results = new List<CompositeSubrequestResult<T>>();

            //foreach (var compositeResponse in responseBody.CompositeResponse)
            //{
            //    var subrequest = Subrequests.First(x => x.compositeSubrequestBase.ReferenceId == compositeResponse.ReferenceId);

            //    if(subrequest.salesforceSerialization == SalesforceSerialization.CREATE)
            //    {
            //        var convertedBody = JsonConvert.DeserializeObject<CreateResponseModel>(compositeResponse.Body);
            //        var convertedSubrequestResult = new CompositeSubrequestResult<CreateResponseModel>(convertedBody, compositeResponse);
            //    }

            //    var dasdasd = JsonConvert.DeserializeObject(compositeResponse.Body, subrequest.responseType);
            //}

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
