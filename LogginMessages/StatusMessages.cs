using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogginMessages
{
	public class StatusMessages
	{
		public string message { get; }
		private int HTTPStatusCode { get; }
		private StatusMessages(string _message,int _statusCode) 
		{
			message = _message;
			HTTPStatusCode = _statusCode;
		}
		public StatusMessages CreateStatusMessage(string _message,int _statusCode)
		{
			return new StatusMessages(_message,_statusCode);
		}


		public static implicit operator string(StatusMessages statusMessage)
		{
			return statusMessage.message;
		}

		public static implicit operator int(StatusMessages statusMessages)
		{
			return statusMessages.HTTPStatusCode;
		}
		public override string ToString()
		{
			return message;
		}

		#region Server Error Messages
		public static StatusMessages UnableToService = new StatusMessages("Services is experiencing difficulties, please try again later",503);
		public static StatusMessages AccessDenied = new StatusMessages("Access Denied", 403);
		public static StatusMessages UnauthorizedAccess = new StatusMessages("Unauthorized Access Denied", 401);
		public static StatusMessages ResourceNotFound = new StatusMessages("Resource Not Found", 404);
		public static StatusMessages Ok = new StatusMessages("OK", 200);
		public static StatusMessages InvalidRequest = new StatusMessages("Invalid Request", 400);
		#endregion

		#region User Status Messages
		public static StatusMessages UserExist = new StatusMessages("User is already created with that e-mail address", 409);
		public static StatusMessages UserNotexist = new StatusMessages("User does not exist", 404);
		public static StatusMessages AddNewUser = new StatusMessages("New User successfull added", 201);
		public static StatusMessages UpdateUser = new StatusMessages("User is successfully updated", 200);
		public static StatusMessages FailedToAddUser = new StatusMessages("Failed to add New User", 503);
		public static StatusMessages FailedtoUpdateUser = new StatusMessages("Failed to update the User", 503);
		public static StatusMessages FailedtoDeleteUser = new StatusMessages("Failed to delete the User", 503);
		public static StatusMessages DeleteUser = new StatusMessages("User is successfully deleted", 204);
		public static StatusMessages ResetPassword = new StatusMessages("Reset Password notification is sent to the user", 200);
		#endregion

		#region Team Status Messages
		public static StatusMessages TeamNotexist = new StatusMessages("Team does not exist", 404);
		public static StatusMessages AddNewTeam = new StatusMessages("New Team successfull added", 201);
		public static StatusMessages UpdateTeam = new StatusMessages("Team is successfully updated", 200);
		public static StatusMessages DeleteTeam = new StatusMessages("Team is successfully deleted", 204);
		public static StatusMessages FailedToAddTeam = new StatusMessages("Failed to add New Team", 503);
		public static StatusMessages FailedtoUpdateTeam = new StatusMessages("Failed to update the Team", 503);
		public static StatusMessages FailedtoDeleteTeam = new StatusMessages("Failed to delete the Team", 503);
		#endregion

		#region Password Status Messages
		public static StatusMessages PasswordChange = new StatusMessages("Successfully changed password", 200);
		public static StatusMessages FailedPasswordChange = new StatusMessages("Failed to change password", 503);
		public static StatusMessages IncorrectConfirmPassword = new StatusMessages("New password and confirmed Password are not same", 422);
		public static StatusMessages IncorrectPasswordComplexity = new StatusMessages("New password has to be more then 8 characters, and should contain at least 1 number, 1 lower case, 1 upper case, 1 special character", 422);
		#endregion

		#region Verification Code Status Messages
		public static StatusMessages InvalidVerificationCode = new StatusMessages("Invalid Verification code", 400);
		public static StatusMessages SendVerificationCode = new StatusMessages("Successfully send Verification Code", 200);
		public static StatusMessages FailtoSendVerificationCode = new StatusMessages("Failed to send Verification Code", 503);
		#endregion

		#region Client Status Messages
		public static StatusMessages ClientNotexist = new StatusMessages("Client does not exist", 404);
		public static StatusMessages AddNewClient = new StatusMessages("New Client successfull added", 201);
		public static StatusMessages UpdateClient = new StatusMessages("Client is successfully updated", 200);
		public static StatusMessages DeleteClient = new StatusMessages("Client is successfully deleted", 204);
		public static StatusMessages FailedToAddClient = new StatusMessages("Failed to add New Client", 503);
		public static StatusMessages FailedtoUpdateClient = new StatusMessages("Failed to update the Client", 503);
		public static StatusMessages FailedtoDeleteClient = new StatusMessages("Failed to delete the Client", 503);
		#endregion

		#region Credential Status Messages
		public static StatusMessages CredentialNotexist = new StatusMessages("Credential does not exist", 404);
		public static StatusMessages AddNewCredential = new StatusMessages("New Credential successfull added", 201);
		public static StatusMessages UpdateCredential = new StatusMessages("Credential is successfully updated", 200);
		public static StatusMessages DeleteCredential = new StatusMessages("Credential is successfully deleted", 204);
		public static StatusMessages FailedToAddCredentail = new StatusMessages("Failed to add New Credential", 503);
		public static StatusMessages FailedtoUpdateCredential = new StatusMessages("Failed to update the Credential", 503);
		public static StatusMessages FailedtoDeleteCredential = new StatusMessages("Failed to delete the Credential", 503);
		public static StatusMessages GiveCredential = new StatusMessages("Successfully give credential", 200);
		public static StatusMessages FailedToGiveCredential = new StatusMessages("Failed to give Credential", 503);
		#endregion

		#region Certificate Status Messages
		public static StatusMessages CertificateNotexist = new StatusMessages("Certificate does not exist", 404);
		public static StatusMessages AddNewCertificate = new StatusMessages("New Certificate successfull added", 201);
		public static StatusMessages DeleteCertificate = new StatusMessages("Certificate is successfully deleted", 204);
		public static StatusMessages FailedToAddCertificate = new StatusMessages("Failed to add New Certificate", 503);
		public static StatusMessages FailedtoUpdateCertificate = new StatusMessages("Failed to update the Certificate", 503);
		public static StatusMessages FailedtoDeleteCertificate = new StatusMessages("Failed to delete the Certificate", 503);
		public static StatusMessages InvalidCertificate = new StatusMessages("Invalid certificate - wrong password, wrong pem key or currpted certificate file", 422);
		#endregion

		#region Personal Folder Status Messages
		public static StatusMessages PersonalFolderNotexist = new StatusMessages("Personal folder does not exist", 404);
		public static StatusMessages AddNewPersonalFolder = new StatusMessages("New Personal Folder successfull added", 201);
		public static StatusMessages DeletePersonalFolder = new StatusMessages("The Personal Folder is successfully deleted", 204);
		public static StatusMessages UpdatePersonalFolder = new StatusMessages("The Personal Folder is successfully updated",200);
		#endregion

		#region Validation Status Messages
		public static StatusMessages InvalidName = new StatusMessages("Invalid name - alphanumeric characters only", 422);
		public static StatusMessages InvalidValue = new StatusMessages("Invalid value - value presented is invalid", 422);
		#endregion










	}
}
