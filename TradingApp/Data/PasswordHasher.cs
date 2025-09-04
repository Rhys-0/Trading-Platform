using System.Security.Cryptography;
using System.Text;

namespace TradingApp.Data {
	internal static class PasswordHasher {
		public static string ComputeSha256Hash(string input) {
			using var sha256 = SHA256.Create();
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			byte[] hashBytes = sha256.ComputeHash(bytes);
			var builder = new StringBuilder(hashBytes.Length * 2);
			for (int i = 0; i < hashBytes.Length; i++) {
				builder.Append(hashBytes[i].ToString("x2"));
			}
			return builder.ToString();
		}
	}
}


