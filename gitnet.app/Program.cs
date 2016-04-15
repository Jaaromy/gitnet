using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Octokit;

namespace gitnet.app
{
	class Program
	{
		static void Main(string[] args)
		{
			DoIt();
		}

		public static void DoIt()
		{
			const int timestamp = 1408492800; // Aug 20, 2014
			var dict = new Dictionary<string, UserStats>();

			var tokenAuth = new Credentials("CRED_HERE");
			var github = new GitHubClient(new ProductHeaderValue("Jaaromy")) {Credentials = tokenAuth};

			var repos = github.Repository.GetAllForOrg("evidon").Result.Select(x => x.Name).ToList();

			foreach (var repo in repos)
			{
				Console.WriteLine(repo);
				try
				{
					var contributers = github.Repository.Statistics.GetContributors("evidon", repo).Result;

					foreach (var contributer in contributers)
					{
						if (!dict.ContainsKey(contributer.Author.Login))
						{
							dict.Add(contributer.Author.Login, new UserStats { Login = contributer.Author.Login });
						}

						var weeks = contributer.Weeks.Where(x => x.W >= timestamp).ToList();

						if (weeks.Count == 0)
						{
							continue;
						}

						var additions = weeks.Sum(x => x.Additions);
						var deletions = weeks.Sum(x => x.Deletions);
						var commits = weeks.Sum(x => x.Commits);
						var changes = additions + deletions;

						dict[contributer.Author.Login].TotalAdditions += additions;
						dict[contributer.Author.Login].TotalDeletions += deletions;
						dict[contributer.Author.Login].TotalCommits += commits;
						dict[contributer.Author.Login].TotalChanges += changes;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}

			var stats = dict.Values.OrderByDescending(x => x.TotalCommits).ToList();

			var results = JsonConvert.SerializeObject(stats.ToArray());

			using (var fw = new StreamWriter("results_SinceAug2014.json"))
			{
				fw.Write(results);
			}
		}
	}
}
