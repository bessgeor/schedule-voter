using System;

#nullable enable

namespace schedule_voter_back._Db
{
	public class VoteResult
	{
		public int UserId { get; set; }
		public DateTime Tourney { get; set; }
		public Vote Vote { get; set; }

		public virtual User User { get; set; }
	}
}
