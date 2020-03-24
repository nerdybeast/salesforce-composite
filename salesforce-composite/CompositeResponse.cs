﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace salesforce_composite
{
    public class CompositeSubrequestResult
    {
        ///// <summary>
        ///// Empty constructor for use by Newtonsoft
        ///// </summary>
        //[JsonConstructor]
        //public CompositeSubrequestResult()
        //{

        //}

        //public CompositeSubrequestResult(T body, CompositeSubrequestResult<string> compositeSubrequestResult)
        //{
        //    Body = body;
        //    HttpHeaders = compositeSubrequestResult.HttpHeaders;
        //    HttpStatusCode = compositeSubrequestResult.HttpStatusCode;
        //    ReferenceId = compositeSubrequestResult.ReferenceId;
        //}

        public object Body { get; set; }
        public Dictionary<string, string> HttpHeaders { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ReferenceId { get; set; }
    }

    //public class CompositeSubrequestResultBase
    //{
    //    //public object Body { get; set; }
    //    //public string Body { get; set; }
    //    public Dictionary<string, string> HttpHeaders { get; set; }
    //    public HttpStatusCode HttpStatusCode { get; set; }
    //    public string ReferenceId { get; set; }
    //}

    //public class CompositeSubrequestResultOfString : CompositeSubrequestResultBase
    //{
    //    public string Body { get; set; }
    //}

    //public class CompositeSubrequestResultOfT<T> : CompositeSubrequestResultBase where T : class
    //{
    //    public T Body { get; set; }
    //}
}
