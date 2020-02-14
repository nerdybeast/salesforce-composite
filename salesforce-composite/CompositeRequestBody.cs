using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace salesforce_composite
{
    /// <summary>
    /// https://developer.salesforce.com/docs/atlas.en-us.api_rest.meta/api_rest/requests_composite.htm
    /// </summary>
    public class CompositeRequestBody
    {
        /// <summary>
        /// Specifies what to do when an error occurs while processing a subrequest. If the value is true, the entire composite request is rolled back. The top-level request returns HTTP 200 and includes responses for each subrequest. 
        /// If the value is false, the remaining subrequests that don’t depend on the failed subrequest are executed.Dependent subrequests aren’t executed.
        /// In either case, the top-level request returns HTTP 200 and includes responses for each subrequest.
        /// Default: true
        /// </summary>
        [JsonProperty("allOrNone")]
        public bool AllOrNone { get; set; } = true;

        [JsonProperty("compositeRequest")]
        public List<CompositeSubrequestBase> CompositeRequest { get; set; } = new List<CompositeSubrequestBase>();
    }
}
