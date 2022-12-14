using JobOfferService.Database.Context;
using JobOfferService.Database.Models;
using JobOfferService.Models.Authentication;
using JobOfferService.Utilities.Authentication;
using JobOfferService.Utilities.PasswordClient;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace JobOfferService.Utilities.UserHandler
{
    public class UserHandler : IUserHandler
    {
        public string HashUserPassword(string password)
        {
            return HashPassword(password);
        }

        public bool VerifyUserPassword(string dbPassword, string userInputPassword)
        {
            var hashedInputPassword = HashPassword(userInputPassword);

            return hashedInputPassword.Equals(dbPassword);
        }

        public string HashPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                // Generate a 128-bit salt using a sequence of
                // cryptographically strong random bytes.
                byte[] salt = new byte[1024];
                Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

                // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password!,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                return hashed;
            }
            else
                throw new Exception("Password is empty!");
        }
    }
}
