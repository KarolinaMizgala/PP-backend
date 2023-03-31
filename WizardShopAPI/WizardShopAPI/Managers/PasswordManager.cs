using System.Security.Cryptography;
using System.Text;

namespace WizardShopAPI.Managers
{
    public class PasswordManager
    {
        readonly string plainPassword;

        readonly string salt;
        public string Salt { get { return salt; } }

        string computedHashedPassword;
        public string ComputedHashedPassword { get { return computedHashedPassword; } }

        readonly HashAlgorithmName algorithm = HashAlgorithmName.SHA384;

        //keySize value should align with hash size of used algorithm (bytes)
        //SHA384 produces a 48-byte hash value
        const int keySize = 48;

        const int iterations = 100_000;

        //this constructor is used for registration
        public PasswordManager(string plainPassword)
        {
            this.plainPassword = plainPassword;
            salt = Convert.ToHexString(RandomNumberGenerator.GetBytes(keySize));
            computedHashedPassword = HashPassword();
        }

        //this constructor is used for login
        public PasswordManager(string plainPassword, string salt)
        {
            this.plainPassword = plainPassword;
            this.salt = salt;
            computedHashedPassword = HashPassword();
        }

        private string HashPassword()
        {
            byte[] hashedPassword = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(plainPassword),
                                                    Convert.FromHexString(salt),
                                                    iterations,
                                                    algorithm,
                                                    keySize);
            return Convert.ToHexString(hashedPassword);
        }

        /// <summary>
        /// Method for password verification
        /// </summary>
        /// <param name="dbHashedPassword"> hashed password from database</param>
        /// <returns> True if passwords match otherwise false</returns>
        public bool Compare(string dbHashedPassword)
        {
            return ComputedHashedPassword.Equals(dbHashedPassword);
        }

    }
}
