using System;
using System.Security.Cryptography;

namespace Interview_Calendar.Helpers
{
	public class PasswordHasher
	{
        private const int _saltSize = 128 / 8;
        private const int _keySize = 256 / 8;
        private const int _iterations = 1000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private const char _delimeter = ',';

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(_saltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _hashAlgorithmName, _keySize);

            return string.Join(_delimeter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));

        }

        public bool verify(string passwordHash, string inputPassword)
        {
            var elements = passwordHash.Split(_delimeter);
            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);

            var hashInput = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, _iterations, _hashAlgorithmName, _keySize);

            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }
    }
}

