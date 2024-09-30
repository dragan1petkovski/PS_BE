
namespace LogginMessages
{
	public static class EntityLog
	{
		public static Func<string, string, string> NotFound = (string type, string entityId) => $"{DateTime.Now} - {type} entity not found - {entityId}";

		public static Func<string,string> InvalidJsonString = (string jsonString) => $"{DateTime.Now} - Invalid Json String - {jsonString}";

		public static Func<string> InvalidJWTToken = () => $"{DateTime.Now} - Invalid JwtToekn";
		
		public static Func<string, string> SecurityTokenForgered = (string potentialUserId = null) => $"{DateTime.Now} - Forgered Security Token - Possible application attack - {potentialUserId}";

	}
}
