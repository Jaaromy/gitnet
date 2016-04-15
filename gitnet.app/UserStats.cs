using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gitnet.app
{
	public class UserStats
	{
		public string Login { get; set; }
		public long TotalCommits { get; set; }
		public long TotalAdditions { get; set; }
		public long TotalDeletions { get; set; }
		public long TotalChanges { get; set; }
	}
}
