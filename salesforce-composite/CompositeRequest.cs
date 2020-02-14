using salesforce_composite.enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using salesforce_composite.serialization;

namespace salesforce_composite
{
    public class CompositeBuilder
    {
        private List<CompositeSubrequestBase> _requests { get; set; } = new List<CompositeSubrequestBase>();

        public CompositeBuilder RetrieveSobject<T>(string referenceId, string sobjectId) where T : Sobject
        {
            _requests.Add(new RetrieveSobject(referenceId, typeof(T).Name, sobjectId));
            return this;
        }

        public CompositeBuilder RetrieveSobject<T>(string referenceId, string sobjectId, out T sobjectReference) where T : Sobject, new()
        {
            _requests.Add(new RetrieveSobject(referenceId, typeof(T).Name, sobjectId));
            sobjectReference = new T().PrependValueToStringProperties(referenceId);
            return this;
        }

        public CompositeBuilder CreateSobject<T>(string referenceId, T sobject, out T sobjectReference) where T : Sobject, new()
        {
            _requests.Add(new CreateSobject<T>(referenceId, typeof(T).Name, sobject));
            sobjectReference = new T().PrependValueToStringProperties(referenceId);
            return this;
        }

        public CompositeBuilder UpdateSobject<T>(string referenceId, T sobject) where T : Sobject, new()
        {
            _requests.Add(new UpdateSobject<T>(referenceId, typeof(T).Name, sobject));
            return this;
        }

        public List<CompositeSubrequestResult> Execute()
        {
            var obj = new CompositeRequestBody
            {
                CompositeRequest = _requests
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SalesforceResolver()
            };

            var x = JsonConvert.SerializeObject(obj, Formatting.Indented, settings);

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
