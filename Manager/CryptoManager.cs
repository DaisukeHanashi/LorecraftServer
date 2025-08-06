using System.Security.Cryptography;
using Lorecraft_API.StaticFactory;
using NSec.Cryptography;

namespace Lorecraft_API.Manager
{
    public class CryptoManager(Argon2ParameterFactory argon2ParamFactory)
    {
        // private static string SecretKey { get; set; } = null!;
        // private static int KeySize { get; set; }
        private readonly Argon2ParameterFactory _argon2ParameterFactory = argon2ParamFactory;

        private const int BlockSize = 16;

        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 4;
        private const int InsaneIterations = 1000;
        private const int DegreeOfParallelism = 8;
        private const int MemorySize = 65536;

        // public static void SetUp(IConfigurationSection tokenAuthSection)
        // {
        //     SecretKey = tokenAuthSection.GetValue<string>("SecretKey") ?? "QmlyaWJpcmlNaXNha2FNaWtv";

        //     KeySize = DetermineKeySizeFromBase64String(SecretKey);

        // }


        private static int DetermineKeySizeFromBase64String(string key)
        {
            byte[] bytes = Convert.FromBase64String(key);
            return DetermineKeySize(bytes);
        }

        private static int DetermineKeySize(byte[] keyBytes)
        {
            var keyLength = keyBytes.Length * 8; // Convert byte length to bit length

            return keyLength switch
            {
                128 or 192 or 256 => keyLength,
                _ => throw new ArgumentException("Invalid key size"),
            };
        }

        private byte[] HashPassword(byte[] passwordBytes)
        {

            Argon2Parameters parameters = _argon2ParameterFactory.CreateParameters();

            Argon2id argonHash = PasswordBasedKeyDerivationAlgorithm.Argon2id(parameters);
            
            byte[] salt = GenerateRandomBytes(SaltSize);

            byte[] hash = argonHash.DeriveBytes(passwordBytes, salt, SaltSize);

            return [.. salt, .. hash]; 
        }


        public string HashPassword(string password)
        {

            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            byte[] hash = HashPassword(passwordBytes);

            return Convert.ToBase64String(hash);
        }

        private byte[] HashPasswordFromAttempt(byte[] passwordBytes, byte[] salt){

            Argon2Parameters parameters = _argon2ParameterFactory.CreateParameters();

            Argon2id argonHash = PasswordBasedKeyDerivationAlgorithm.Argon2id(parameters);

            return argonHash.DeriveBytes(passwordBytes, salt, SaltSize);
        }
        public bool VerifyPassword(string password, string hashedPassword)
        {

            byte[] decodedHash = Convert.FromBase64String(hashedPassword);
            
            byte[] salt = [.. decodedHash.Take(SaltSize)];
            
            byte[] originalHash = [.. decodedHash.Skip(SaltSize)];

            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            byte[] inputtedHash = HashPasswordFromAttempt(passwordBytes, salt); 

            return inputtedHash.SequenceEqual(originalHash);  

        }

        private static byte[] GenerateRandomBytes(int anySize)
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[anySize];
            rng.GetBytes(bytes);
            return bytes;
        }



    }
}