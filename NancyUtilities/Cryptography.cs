using Nancy.Cryptography;

namespace NancyUtilities
{
    public class Cryptography : IEncryptionProvider
    {
        private static CryptographyConfiguration cryptographyConfiguration
        => new CryptographyConfiguration(
                    new RijndaelEncryptionProvider(new PassphraseKeyGenerator("SuperSecretPass", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })),
                    new DefaultHmacProvider(new PassphraseKeyGenerator("UberSuperSecure", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })));

        public string Encrypt(string data)
        {
            return cryptographyConfiguration.EncryptionProvider.Encrypt(data);
        }

        public string Decrypt(string data)
        {
            return cryptographyConfiguration.EncryptionProvider.Decrypt(data);           
        }
    }
}
