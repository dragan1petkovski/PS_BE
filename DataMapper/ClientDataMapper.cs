using DTO.Client;
using DomainModel;

namespace DataMapper
{
	public class ClientDataMapper
	{
		public List<ClientForUsers> ConvertClientListToClientDTOListForUsers(List<Client> clients)
		{
			return clients.Select(c => new ClientForUsers() { 
				id = c.id, 
				name = c.name 
			}).ToList();
		}

		public List<ClientForAdmins> ConvertClientListToClientDTOListForAdmins(List<Client> clients)
		{
			return clients.Select(c => new ClientForAdmins() { 
				name = c.name,
				id = c.id,
				createdate = c.createdate,
				updatedate = c.updatedate,
			}).ToList();
		}
	}
}
