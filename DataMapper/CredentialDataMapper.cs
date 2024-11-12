using DomainModel;
using DTO.Credential;
using TransitionObjectMapper;

namespace DataMapper
{
	public class CredentialDataMapper
	{
		public List<DTO.Credential.Credential> ConvertToCredentialDTO(List<TeamCredentialsMap> credentials)
		{
			List<DTO.Credential.Credential> output = new List<DTO.Credential.Credential>();

			foreach (TeamCredentialsMap teamCredentialMap in credentials)
			{

				foreach (DomainModel.Credential teamcred in teamCredentialMap.credentials)
				{
					output.Add(new DTO.Credential.Credential
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
	
		public DTO.Credential.Credential ConvertToCredentialDTO(DomainModel.Credential credential)
		{
			DTO.Credential.Credential output = new DTO.Credential.Credential();
			output.id = credential.id;
			output.username = credential.username;
			output.domain = credential.domain;
			output.email = credential.email;
			output.remote = credential.remote;
			output.note = credential.note;
			return output;
		}
	
		public PersonalCredential ConvertToPersonalCredentialDTO(DomainModel.Credential credential)
		{
			PersonalCredential output = new PersonalCredential();
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
