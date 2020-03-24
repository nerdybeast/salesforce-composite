using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace salesforce_composite
{
    public class CompositeResponseBody
    {
        public List<CompositeSubrequestResult> CompositeResponse { get; set; }
    }
}
