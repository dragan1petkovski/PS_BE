using DTO.Client;
using DomainModel;

namespace DataMapper
{
	public class ClientDataMapper
	{
		public List<ClientForUsers> ConvertClientListToClientDTOListForUsers(List<Client> clients)
		{
			List<ClientForUsers> output = new List<ClientForUsers>();
			foreach (Client client in clients)
			{
				output.Add(new ClientForUsers()
				{
					name = client.name,
					id = client.id
				});
			}
			return output;
		}

		public List<ClientForAdmins> ConvertClientListToClientDTOListForAdmins(List<Client> clients)
		{
			List<ClientForAdmins> output = new List<ClientForAdmins>();
			foreach (Client client in clients)
			{
				output.Add(new ClientForAdmins()
				{
					name = client.name,
					id = client.id,
					createdate = client.createdate,
					updatedate = client.updatedate,
				});
			}
			return output;
		}
	}
}
