namespace Lychee.Publisher.Application.Abstractions;

public interface IDataEncryptionService
{
	string Encrypt(string plainText);

	string Decrypt(string cipherText);
}
