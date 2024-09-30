using DTOModel.ClientDTO;
using DomainModel;

namespace DataMapper
{
	public class ClientDataMapper
	{
		public List<ClientDTOForUsers> ConvertClientListToClientDTOListForUsers(List<Client> clients)
		{
			List<ClientDTOForUsers> output = new List<ClientDTOForUsers>();
			foreach (Client client in clients)
			{
				output.Add(new ClientDTOForUsers()
				{
					name = client.name,
					id = client.id
				});
			}
			return output;
		}

		public List<ClientDTOForAdmins> ConvertClientListToClientDTOListForAdmins(List<Client> clients)
		{
			List<ClientDTOForAdmins> output = new List<ClientDTOForAdmins>();
			foreach (Client client in clients)
			{
				output.Add(new ClientDTOForAdmins()
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
