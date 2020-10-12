using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NBitcoin;

namespace ElectrumMnemonic
{
    public static class ElectrumMnemonic
    {
        public static string NormalizeSeedPhrase(string input)
        {
            return input
                .Trim()
                .Split(' ', '\r', '\n', '\t')
                .Where(t => t != "")
                .Select(t => t.ToLower())
                .Aggregate((a, b) => a + " " + b);
        }

        public static int GetSeedVersion(string input)
        {
            var mnemo = new NBitcoin.Mnemonic(NormalizeSeedPhrase(input));

            var encoding = Encoding.UTF8;
            var keyByte = encoding.GetBytes("Seed version");
            var hmacsha512 = new HMACSHA512(keyByte);
            var messageBytes = encoding.GetBytes(mnemo.ToString());
            var hashMessage = hmacsha512.ComputeHash(messageBytes);

            return hashMessage.First();
        }

        public static ExtKey GetExtRoot(string input, string passPhrase = "", RootKeyType keyType = RootKeyType.Legacy)
        {
            var mnemo = new NBitcoin.Mnemonic(NormalizeSeedPhrase(input));

            byte[] pbkf_hmacked;
            {
                // Generate a salt
                var saltBytes = Encoding.UTF8.GetBytes("electrum" + passPhrase);

                // Generate the hash
                // TODO: .Net Standard 2.0
                var pbkdf2 = new Rfc2898DeriveBytes(
                    mnemo.ToString(),
                    saltBytes,
                    2048,
                    HashAlgorithmName.SHA512
                );

                var keyByte = Encoding.UTF8.GetBytes("Bitcoin seed");
                var hmacsha512 = new HMACSHA512(keyByte);
                pbkf_hmacked = hmacsha512.ComputeHash(pbkdf2.GetBytes(64));
            }

            var fullKey = new byte[4 + 1 + 4 + 4 + 32 + 1 + 32];
            Array.Copy(new byte[] {0x04, 0x88, 0xAD, 0xE4,}, fullKey, 4);
            Array.Copy(pbkf_hmacked, 32, fullKey, 4 + 1 + 4 + 4, 32);
            Array.Copy(pbkf_hmacked, 0, fullKey, 4 + 1 + 4 + 4 + 32 + 1, 32);
            // var xpriv = Base58CheckEncoding.Encode(fullKey);

            var chainCode = new byte[32];
            Array.Copy(fullKey, 4 + 1 + 4 + 4, chainCode, 0, 32);
            var keyHex = new byte[32];
            Array.Copy(fullKey, 4 + 1 + 4 + 4 + 32 + 1, keyHex, 0, 32);
            var HDRoot = new ExtKey(new Key(keyHex), chainCode);

            return HDRoot;
        }

        public static string GetDerivationPath(int number, bool isChange = false)
        {
            return string.Format("m/{0}/{1}", isChange ? 1 : 0, number);
        }

        public static ExtKey GetWallet(string seedPhrase, int number, bool isChange = false)
        {
            var hdRoot = GetExtRoot(seedPhrase);
            return GetWallet(hdRoot, number, isChange);
        }

        public static ExtKey GetWallet(ExtKey hdRoot, int number, bool isChange = false)
        {
            var path = GetDerivationPath(number, isChange);
            return hdRoot.Derive(new KeyPath(path));
        }

        public static string GetWalletAddress(string seedPhrase, int number, bool isChange = false)
        {
            var wallet = GetWallet(seedPhrase, number, isChange);
            return wallet
                .GetPublicKey()
                .GetAddress(ScriptPubKeyType.Legacy, Network.Main)
                .ToString();
        }

        public static string GetWalletAddress(ExtKey hdRoot, int number, bool isChange = false)
        {
            var wallet = GetWallet(hdRoot, number, isChange);
            return wallet
                .GetPublicKey()
                .GetAddress(ScriptPubKeyType.Legacy, Network.Main)
                .ToString();
        }
    }
}