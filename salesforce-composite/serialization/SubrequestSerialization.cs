using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace salesforce_composite.serialization
{
    public static class SubrequestSerialization
    {
        public static string Serialize(Subrequest subrequest, Formatting formatting = Formatting.Indented)
        {
            var nullValueHandling = NullValueHandling.Ignore;

            if (subrequest.salesforceSerialization == SalesforceSerialization.UPDATE)
            {
                nullValueHandling = NullValueHandling.Include;
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SalesforceResolver(subrequest.salesforceSerialization),
                NullValueHandling = nullValueHandling
            };

            var json = JsonConvert.SerializeObject(subrequest.compositeSubrequestBase, formatting, settings);

            return json;
        }
    }
}
