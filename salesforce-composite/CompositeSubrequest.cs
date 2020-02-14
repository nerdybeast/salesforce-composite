using Newtonsoft.Json;
using salesforce_composite.enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace salesforce_composite
{
    /// <summary>
    /// https://developer.salesforce.com/docs/atlas.en-us.api_rest.meta/api_rest/requests_composite.htm
    /// NOTE: Using json property names because the composite Salesforce api is case-sensitive 
    /// </summary>
    public class CompositeSubrequestBase
    {
        [JsonProperty("method")]
        public string HttpMethod { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        //[JsonProperty("body")]
        //public object Body { get; set; }
    }

    public class CompositeSubrequest : CompositeSubrequestBase
    {
        [JsonProperty("body")]
        public object Body { get; set; }
    }

    public class CompositeSobjectSubrequest<T> : CompositeSubrequestBase where T : Sobject
    {
        [JsonProperty("body")]
        public T Body { get; set; }
    }
}
