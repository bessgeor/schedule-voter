using System;

#nullable enable

namespace schedule_voter_back
{
	public class ResultsResponse
	{
		public ResultsResponse(DateTime date, VoteResultView[] votes)
		{
			Date = date;
			Votes = votes;
		}

		public DateTime Date { get; }
		public VoteResultView[] Votes { get; }
	}

	public class VoteResultView
	{
		public string Gw2Acc { get; set; }
		public string DisAcc { get; set; }
		public _Db.Vote Vote { get; set; }
	}
}
