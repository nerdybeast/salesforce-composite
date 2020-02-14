using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace salesforce_composite
{
    public class CompositeSubrequestResult
    {
        public object Body { get; }
        public Dictionary<string, string> HttpHeaders { get; }
        public HttpStatusCode HttpStatusCode { get; }
        public string ReferenceId { get; }
    }
}
