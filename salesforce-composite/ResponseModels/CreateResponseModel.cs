using System;
using System.Collections.Generic;
using System.Text;

namespace salesforce_composite.ResponseModels
{
    public class CreateResponseModel
    {
        public string Id { get; set; }
        public bool Success { get; set; }

        //TODO: Figure out what this error model looks like
        //https://developer.salesforce.com/docs/atlas.en-us.api_rest.meta/api_rest/dome_sobject_create.htm
        //public List<???> errors { get; set; }
    }
}
