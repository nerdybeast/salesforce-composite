using System;

namespace salesforce_composite {
	
	public class Subrequest {

		public CompositeSubrequestBase compositeSubrequestBase { get; set; }

		public SalesforceSerialization salesforceSerialization { get; set; }

		public Subrequest(SalesforceSerialization salesforceSerialization, CompositeSubrequestBase compositeSubrequestBase) {
			this.compositeSubrequestBase = compositeSubrequestBase;
			this.salesforceSerialization = salesforceSerialization;
		}
		
	}

}