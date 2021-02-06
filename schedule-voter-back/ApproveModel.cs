using Newtonsoft.Json;

using schedule_voter_back._Db;

using System;

#nullable enable

namespace schedule_voter_back
{
	public class ApproveModel
	{
		[JsonConstructor]
		public ApproveModel(string staticName, string disAccount, string gw2Account, Voting[] votes)
		{
			StaticName = staticName;
			DisAccount = disAccount;
			Gw2Account = gw2Account;
			Votes = votes;
		}

		public string StaticName { get; }
		public string DisAccount { get; }
		public string Gw2Account { get; }
		public Voting[] Votes { get; }
	}

	public class Voting
	{
		[JsonConstructor]
		public Voting(DateTime tourney, Vote vote)
		{
			Tourney = tourney;
			Vote = vote;
		}

		public DateTime Tourney { get; }
		public Vote Vote { get; }
	}
}
