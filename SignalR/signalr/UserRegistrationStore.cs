using System.Collections.Generic;

namespace SignalR.signalr
{
	public static class UserRegistrationStore
	{
		private static Dictionary<Guid,List<(string, string)>> _userRgistrationList = new Dictionary<Guid, List<(string, string)>>();


		public static void AddNewTeam(List<Guid> teams, (string connectionId, string type) pair)
		{
			foreach (Guid team in teams)
			{
				if(_userRgistrationList.Keys.ToList().Any(tid => tid == team))
				{
					_userRgistrationList[team].Add(pair);
				}
				else
				{
					List<(string, string)> connections = new List<(string connectionid, string type)>();
					connections.Add(pair);
					_userRgistrationList.TryAdd(team, connections);
				}			
			}
		}
	
		public static List<(string type, string connectionid)> GetTeamConnections(Guid teamid)
		{
			if(_userRgistrationList.TryGetValue(teamid, out List<(string, string)> connections))
			{
				return connections;
			}
			return new List<(string, string)>();
		}
	}
}
