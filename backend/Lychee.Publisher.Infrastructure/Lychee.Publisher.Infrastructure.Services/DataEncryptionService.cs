using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Lychee.Publisher.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Lychee.Publisher.Infrastructure.Services;

public sealed class DataEncryptionService : IDataEncryptionService
{
	private readonly byte[] _key;

	public DataEncryptionService(IConfiguration configuration)
	{
		string s = configuration["Jwt:SigningKey"] ?? "REPLACE_WITH_64_CHARACTER_KEY_FROM_KEY_VAULT_OR_SECRETS_MANAGER";
		_key = SHA256.HashData(Encoding.UTF8.GetBytes(s));
	}

	public string Encrypt(string plainText)
	{
		if (string.IsNullOrEmpty(plainText))
		{
			return plainText;
		}
		using Aes aes = Aes.Create();
		aes.Key = _key;
		aes.GenerateIV();
		byte[] iV = aes.IV;
		using ICryptoTransform transform = aes.CreateEncryptor(aes.Key, iV);
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(iV, 0, iV.Length);
		using (CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
		{
			using StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
			streamWriter.Write(plainText);
		}
		return Convert.ToBase64String(memoryStream.ToArray());
	}

	public string Decrypt(string cipherText)
	{
		if (string.IsNullOrEmpty(cipherText))
		{
			return cipherText;
		}
		byte[] array = Convert.FromBase64String(cipherText);
		using Aes aes = Aes.Create();
		aes.Key = _key;
		byte[] array2 = new byte[aes.BlockSize / 8];
		byte[] array3 = new byte[array.Length - array2.Length];
		Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
		Buffer.BlockCopy(array, array2.Length, array3, 0, array3.Length);
		using ICryptoTransform transform = aes.CreateDecryptor(aes.Key, array2);
		using MemoryStream stream = new MemoryStream(array3);
		using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
		using StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8);
		return streamReader.ReadToEnd();
	}
}
