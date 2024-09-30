using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using TransitionObjectMapper;

namespace EncryptionLayer
{
	public class SymmetricEncryption
	{
		public SymmetricKey EncryptString(string plainText, IConfiguration configuration)
		{
			byte[] key = Encoding.UTF8.GetBytes(configuration.GetSection("AES:key").Value);
			byte[] nonce = Encoding.UTF8.GetBytes(configuration.GetSection("AES:nonce").Value);
			byte[] tag = new byte[16];
			using (AesGcm aes = new AesGcm(key, 16))
			{
				byte[] encodedPlainText = Encoding.UTF8.GetBytes(plainText);
				byte[] cipherText = new byte[encodedPlainText.Length];
				aes.Encrypt(nonce, encodedPlainText, cipherText, tag);
				return new SymmetricKey { password = Convert.ToBase64String(cipherText), aad = Convert.ToBase64String(tag) };
			}

		}
		public string DecryptString(string cipherText, string aad , IConfiguration configuration)
		{
			byte[] key = Encoding.UTF8.GetBytes(configuration.GetSection("AES:key").Value);
			byte[] nonce = Encoding.UTF8.GetBytes(configuration.GetSection("AES:nonce").Value);
			
			using (AesGcm aes = new AesGcm(key, 16))
			{
				byte[] tag = Convert.FromBase64String(aad);
				byte[] EncodedCipherText = Convert.FromBase64String(cipherText);
				byte[] EncodedplainText = new byte[EncodedCipherText.Length];
				aes.Decrypt(nonce, EncodedCipherText, tag, EncodedplainText);
				string plainText = Encoding.UTF8.GetString(EncodedplainText);
				return plainText;
			}
		}
	}
}
