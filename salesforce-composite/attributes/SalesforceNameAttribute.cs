﻿using System;
using System.Collections.Generic;
using System.Text;

namespace salesforce_composite.attributes
{
    public sealed class SalesforceNameAttribute : Attribute
    {
        public readonly string SalesforcePropertyName;

        public SalesforceNameAttribute(string name)
        {
            SalesforcePropertyName = name;
        }
    }
}
