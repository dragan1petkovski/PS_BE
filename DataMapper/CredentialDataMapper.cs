using DomainModel;
using DTOModel.CredentialDTO;
using TransitionObjectMapper;

namespace DataMapper
{
	public class CredentialDataMapper
	{
		public List<CredentialDTO> ConvertCredentialtoDTO(List<TeamCredentialsMap> credentials)
		{
			List<CredentialDTO> output = new List<CredentialDTO>();

			foreach (TeamCredentialsMap teamCredentialMap in credentials)
			{

				foreach (Credential teamcred in teamCredentialMap.credentials)
				{
					output.Add(new CredentialDTO
					{
						id = teamcred.id,
						username = teamcred.username,
						domain = teamcred.domain,
						password = "******",
						email = teamcred.email,
						note = teamcred.note,
						remote = teamcred.remote,
						teamname = teamCredentialMap.teamname,
						teamid = teamCredentialMap.teamid,
						clientid = teamCredentialMap.clientid,
						
					});
				}
			}
			return output;
		}
	
		public CredentialDTO ConvertToCredentialDTO(Credential credential)
		{
			CredentialDTO output = new CredentialDTO();
			output.id = credential.id;
			output.username = credential.username;
			output.domain = credential.domain;
			output.email = credential.email;
			output.remote = credential.remote;
			output.note = credential.note;
			return output;
		}
	
		public PersonalCredentialDTO ConvertToPersonalCredentialDTO(Credential credential)
		{
			PersonalCredentialDTO output = new PersonalCredentialDTO();
			output.id = credential.id;
			output.username = credential.username;
			output.domain = credential.domain;
			output.email = credential.email;
			output.remote = credential.remote;
			output.note = credential.note;
			return output;
		}
	}
}
