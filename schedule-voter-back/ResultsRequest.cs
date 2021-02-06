using Newtonsoft.Json;

using System;

#nullable enable

namespace schedule_voter_back
{
	public class ResultsRequest
	{
		[JsonConstructor]
		public ResultsRequest(string staticName, DateTime[] dates)
		{
			StaticName = staticName;
			Dates = dates;
		}

		public string StaticName { get; }
		public DateTime[] Dates { get; }
	}
}
