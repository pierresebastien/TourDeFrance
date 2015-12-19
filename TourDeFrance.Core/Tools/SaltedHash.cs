using System;
using System.Security.Cryptography;
using System.Text;

namespace TourDeFrance.Core.Tools
{
	/// <summary>
	///     Thank you Martijn
	///     http://www.dijksterhuis.org/creating-salted-hash-values-in-c/
	/// </summary>
	public class SaltedHash
	{
		private static readonly MD5 Md5 = MD5.Create();
		private readonly HashAlgorithm _hashProvider;
		private readonly int _salthLength;

		public SaltedHash(HashAlgorithm hashAlgorithm, int theSaltLength)
		{
			_hashProvider = hashAlgorithm;
			_salthLength = theSaltLength;
		}

		public SaltedHash() : this(new SHA256Managed(), 4)
		{
		}

		private byte[] ComputeHash(byte[] data, byte[] salt)
		{
			var dataAndSalt = new byte[data.Length + _salthLength];
			Array.Copy(data, dataAndSalt, data.Length);
			Array.Copy(salt, 0, dataAndSalt, data.Length, _salthLength);

			return _hashProvider.ComputeHash(dataAndSalt);
		}

		public void GetHashAndSalt(byte[] data, out byte[] hash, out byte[] salt)
		{
			salt = new byte[_salthLength];

			var random = new RNGCryptoServiceProvider();
			random.GetNonZeroBytes(salt);

			hash = ComputeHash(data, salt);
		}

		public void GetHashAndSaltString(string data, out string hash, out string salt)
		{
			byte[] hashOut;
			byte[] saltOut;

			GetHashAndSalt(Encoding.UTF8.GetBytes(data), out hashOut, out saltOut);

			hash = Convert.ToBase64String(hashOut);
			salt = Convert.ToBase64String(saltOut);
		}

		public bool VerifyHash(byte[] data, byte[] hash, byte[] salt)
		{
			var newHash = ComputeHash(data, salt);

			if (newHash.Length != hash.Length)
			{
				return false;
			}

			for (var lp = 0; lp < hash.Length; lp++)
			{
				if (!hash[lp].Equals(newHash[lp]))
				{
					return false;
				}
			}

			return true;
		}

		public bool VerifyHashString(string data, string hash, string salt)
		{
			byte[] hashToVerify = Convert.FromBase64String(hash);
			byte[] saltToVerify = Convert.FromBase64String(salt);
			byte[] dataToVerify = Encoding.UTF8.GetBytes(data);
			return VerifyHash(dataToVerify, hashToVerify, saltToVerify);
		}

		public static string ComputeHash(string text)
		{
			byte[] data = Md5.ComputeHash(Encoding.UTF8.GetBytes(text));
			var sb = new StringBuilder();
			foreach (byte b in data)
			{
				sb.Append(b.ToString("X2"));
			}
			return sb.ToString();
		}
	}
}