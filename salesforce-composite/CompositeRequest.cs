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
    public class CompositeBuilder
    {
        //private List<CompositeSubrequestBase> _requests { get; set; } = new List<CompositeSubrequestBase>();
        private List<Subrequest> _requests = new List<Subrequest>();

        public CompositeBuilder RetrieveSobject<T>(string referenceId, string sobjectId) where T : Sobject
        {
            //_requests.Add(new RetrieveSobject(referenceId, typeof(T).Name, sobjectId));
            var action = new RetrieveSobject(referenceId, typeof(T).Name, sobjectId);
            _requests.Add(new Subrequest(SalesforceSerialization.RETRIEVE, action));

            return this;
        }

        public CompositeBuilder RetrieveSobject<T>(string referenceId, string sobjectId, out T sobjectReference) where T : Sobject, new()
        {
            //_requests.Add(new RetrieveSobject(referenceId, typeof(T).Name, sobjectId));
            var action = new RetrieveSobject(referenceId, typeof(T).Name, sobjectId);
            _requests.Add(new Subrequest(SalesforceSerialization.RETRIEVE, action));
            
            sobjectReference = new T().PrependValueToStringProperties(referenceId);
            
            return this;
        }

        public CompositeBuilder CreateSobject<T>(string referenceId, T sobject, out string sobjectIdReference) where T : Sobject, new()
        {
            //_requests.Add(new CreateSobject<T>(referenceId, typeof(T).Name, sobject));
            var action = new CreateSobject<T>(referenceId, typeof(T).Name, sobject);
            _requests.Add(new Subrequest(SalesforceSerialization.CREATE, action));

            //sobjectReference = new T().PrependValueToStringProperties(referenceId);
            sobjectIdReference = $"@{{{referenceId}.id}}";

            return this;
        }

        public CompositeBuilder UpdateSobject<T>(string referenceId, T sobject) where T : Sobject, new()
        {
            var action = new UpdateSobject<T>(referenceId, typeof(T).Name, sobject);
            _requests.Add(new Subrequest(SalesforceSerialization.UPDATE, action));
            return this;
        }

        public List<CompositeSubrequestResult> Execute()
        {
            var json = _requests.Select(req => 
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new SalesforceResolver(req.salesforceSerialization)
                };

                var x = JsonConvert.SerializeObject(req.compositeSubrequestBase, Formatting.Indented, settings);

                return x;
            });

            // var settings = new JsonSerializerSettings
            // {
            //     ContractResolver = new SalesforceResolver()
            // };

            // var x = JsonConvert.SerializeObject(obj, Formatting.Indented, settings);

            var obj = new CompositeRequestBody
            {
                CompositeRequest = json.ToList()
            };

            //Temp
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("")
            };

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "00D230000000...");
            HttpResponseMessage response = httpClient.PostAsJsonAsync("/services/data/v38.0/composite/", obj).GetAwaiter().GetResult();
            var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return new List<CompositeSubrequestResult>();
        }
    }

    public class RetrieveSobject : CompositeSobjectSubrequest<Sobject>
    {
        public RetrieveSobject(string referenceId, string sobjectType, string sobjectId)
        {
            base.ReferenceId = referenceId;
            HttpMethod = CompositeHttpMethod.GET.ToString();
            Url = $"/services/data/v38.0/sobjects/{sobjectType}/{sobjectId}";
        }
    }

    public class CreateSobject<T> : CompositeSobjectSubrequest<T> where T : Sobject
    {
        public CreateSobject(string referenceId, string sobjectType, T sobject)
        {
            base.ReferenceId = referenceId;
            HttpMethod = CompositeHttpMethod.POST.ToString();
            Url = $"/services/data/v38.0/sobjects/{sobjectType}";
            Body = sobject;
        }
    }

    public class UpdateSobject<T> : CompositeSobjectSubrequest<T> where T : Sobject
    {
        public UpdateSobject(string referenceId, string sobjectType, T sobject)
        {
            base.ReferenceId = referenceId;
            HttpMethod = CompositeHttpMethod.PATCH.ToString();
            Url = $"/services/data/v38.0/sobjects/{sobjectType}/{sobject.Id}";
            Body = sobject;
        }
    }

    public class PatchSobject
    {
        public PatchSobject(string referenceId, string sobjectType, string sobjectId, object sobject)
        {

        }
    }

    public class DeleteSobject
    {
        public DeleteSobject(string referenceId, string sobjectType, string sobjectId)
        {

        }
    }
}
