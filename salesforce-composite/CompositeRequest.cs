using salesforce_composite.enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using salesforce_composite.serialization;
using System.Linq;

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

        public CompositeBuilder(int salesforceApiVersion, bool allOrNone = true)
        {
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

        public CompositeBuilder CreateSobject<T>(string referenceId, T sobject) where T : Sobject, new()
        {
            var action = new CreateSobject<T>(_salesforceApiVersion, referenceId, typeof(T).Name, sobject);
            Subrequests.Add(new Subrequest(SalesforceSerialization.CREATE, action));

            return this;
        }

        public CompositeBuilder CreateSobject<T>(string referenceId, T sobject, out string sobjectIdReference) where T : Sobject, new()
        {
            var action = new CreateSobject<T>(_salesforceApiVersion, referenceId, typeof(T).Name, sobject);
            Subrequests.Add(new Subrequest(SalesforceSerialization.CREATE, action));

            sobjectIdReference = $"@{{{referenceId}.id}}";

            return this;
        }

        public CompositeBuilder UpdateSobject<T>(string referenceId, T sobject) where T : Sobject, new()
        {
            var action = new UpdateSobject<T>(_salesforceApiVersion, referenceId, typeof(T).Name, sobject);
            Subrequests.Add(new Subrequest(SalesforceSerialization.UPDATE, action));
            return this;
        }

        public CompositeBuilder PatchSobject<T>(string referenceId, T sobject) where T : Sobject, new()
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

        public List<CompositeSubrequestResult> Execute()
        {
            var json = Subrequests.Select(req => SubrequestSerialization.Serialize(req));

            var body = $"{{ \"allOrNone\": {_allOrNone.ToString().ToLower()}, \"compositeRequest\": [{string.Join(",", json)}] }}";

            ////Temp
            //var httpClient = new HttpClient
            //{
            //    BaseAddress = new Uri("")
            //};

            //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "00D230000000...");
            //HttpResponseMessage response = httpClient.PostAsJsonAsync("/services/data/v38.0/composite/", body).GetAwaiter().GetResult();
            //var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return new List<CompositeSubrequestResult>();
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
