namespace LogginMessages
{
	public static class DatabaseLog
	{
		public static Func<string, string> DBConnectionLog = (string details) => $"{DateTime.Now.ToShortTimeString()} - Database connection issue\nDetails: {details}\n\n\n";

		public static string DBConnectionMsg = "We can't serve you at the moment please try again later";
	}
}
