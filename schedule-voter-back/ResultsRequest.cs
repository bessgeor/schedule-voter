using Newtonsoft.Json;

using System;

#nullable enable

namespace schedule_voter_back
{
	public class ResultsRequest
	{
		[JsonConstructor]
		public ResultsRequest(string staticName, DateTime[] dates, bool? showPast)
		{
			StaticName = staticName;
			Dates = dates;
			ShowPast = showPast;
		}

		public string StaticName { get; }
		public DateTime[] Dates { get; }
		public bool? ShowPast { get; }
	}
}
