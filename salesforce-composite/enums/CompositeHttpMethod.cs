using System;
using System.Collections.Generic;
using System.Text;

namespace salesforce_composite.enums
{
    /// <summary>
    /// Http methods supported by Salesforce's composite api, from the docs:
    /// The method to use with the requested resource. Possible values are POST, PUT, PATCH, GET, and DELETE (case-sensitive).
    /// For a list of valid methods, refer to the documentation for the requested resource.
    /// </summary>
    public enum CompositeHttpMethod
    {
        GET,
        POST,
        PUT,
        PATCH,
        DELETE
    }
}
