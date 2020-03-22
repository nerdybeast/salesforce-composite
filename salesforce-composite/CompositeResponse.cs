using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace salesforce_composite
{
    public class CompositeSubrequestResult
    {
        //public object Body { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> HttpHeaders { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ReferenceId { get; set; }
    }
}
