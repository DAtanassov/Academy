using System.Security.Cryptography;
using HotelRoomReservationSystem.Helpers.Interfaces;

namespace HotelRoomReservationSystem.Helpers
{
    public class Hasher : IHasher
    {
        private const int HashSize = 32;
        private const int SaltSize = 16;
        private const int Iteractions = 10000;

        private readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA3_256;

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteractions, Algorithm, HashSize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public bool Verify(string password, string passwordHash)
        {
            string[] parts = passwordHash.Split('-');
            if (parts.Length != 2)
                return false;

            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteractions, Algorithm, HashSize);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}
