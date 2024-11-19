using DomainModel;
using DTO.SignalR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Runtime.ConstrainedExecution;

namespace SignalR
{
	public class ClientNotification
	{

		public async Task Notification(string _status, string _type, dynamic _data, IHubContext hubContext)
		{

			if(_data is DTO.Credential.Credential)
			{
				DTO.Credential.Credential cred =  (DTO.Credential.Credential)_data;
				await hubContext.Clients.Group(cred.id.ToString()).SendAsync("Notify", new { status = _status, type = _type, data = cred });
			}
			if(_data is DTO.Certificate.Certificate) 
			{
				DTO.Certificate.Certificate cert = (DTO.Certificate.Certificate)_data;
				await hubContext.Clients.Group(cert.id.ToString()).SendAsync("Notify", new { status = _status, type = _type, data = cert });
			}
			if(_data is DeleteItem)
			{
				DeleteItem deleteItem = (DeleteItem)_data;
				await hubContext.Clients.Group(deleteItem.teamid.ToString()).SendAsync("Notify", new { status = _status, type = _type, data = deleteItem });
			}
		}
	}
}
