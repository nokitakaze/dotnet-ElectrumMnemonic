using System.Collections.Generic;
using NBitcoin;
using Xunit;

namespace ElectrumMnemonic.Test
{
    public class MainTest
    {
        public static IEnumerable<object[]> NormalizeSeedPhraseData()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ret = new List<object[]>();
            ret.Add(new object[]
            {
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
            });
            ret.Add(new object[]
            {
                " cycle rocket west magnet parrot shuffle foot correct salt library feed song",
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
            });
            ret.Add(new object[]
            {
                " cycle rocket west magnet parrot shuffle foot   correct salt library feed song",
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
            });
            ret.Add(new object[]
            {
                " cycle rocket west magnet parrot SHUFFLE foot   correct salt library feed song",
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
            });
            ret.Add(new object[]
            {
                "CYCLE rocket west magnet parrot shuffle foot correct salt library feed song",
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
            });
            ret.Add(new object[]
            {
                "CYCLE rocket west magnet parrot shuffle foot correct salt library feed SONG     ",
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
            });
            ret.Add(new object[]
            {
                "ROCKET ROCKET ROCKET ROCKET",
                "rocket rocket rocket rocket",
            });
            ret.Add(new object[]
            {
                "Pachryamba xYNta XYN",
                "pachryamba xynta xyn",
            });

            return ret;
        }

        [Theory]
        [MemberData(nameof(NormalizeSeedPhraseData))]
        public void NormalizeSeedPhraseTest(string input, string expected)
        {
            var actual = ElectrumMnemonic.NormalizeSeedPhrase(input);
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> GetSeedVersionData()
        {
            // 
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ret = new List<object[]>();
            ret.Add(new object[]
            {
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
                1,
            });

            return ret;
        }

        [Theory]
        [MemberData(nameof(GetSeedVersionData))]
        public void GetSeedVersionTest(string input, int expected)
        {
            var actual = ElectrumMnemonic.GetSeedVersion(input);
            Assert.Equal(expected, actual);
        }


        public static IEnumerable<object[]> GetExtRootData()
        {
            // 
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ret = new List<object[]>();
            ret.Add(new object[]
            {
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
                "xprv9s21ZrQH143K32jECVM729vWgGq4mUDJCk1ozqAStTphzQtCTuoFmFafNoG1g55iCnBTXUzz3zWnDb5CVLGiFvmaZjuazHDL8a81cPQ8KL6",
            });

            return ret;
        }

        [Theory]
        [MemberData(nameof(GetExtRootData))]
        public void GetExtRootTest(string input, string expected)
        {
            var actual = ElectrumMnemonic.GetExtRoot(input);
            var actualXpriv = actual.GetWif(Network.Main).ToString();

            Assert.Equal(expected, actualXpriv);
        }

        public static IEnumerable<object[]> GetWalletData()
        {
            // 
            // ReSharper disable once UseObjectOrCollectionInitializer
            var ret = new List<object[]>();
            ret.Add(new object[]
            {
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
                false,
                0,
                "1NNkttn1YvVGdqBW4PR6zvc3Zx3H5owKRf",
            });
            ret.Add(new object[]
            {
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
                false,
                1,
                "18KCZB4y7SKPmTGN2rDQuUsNxyVBX7CAbG",
            });
            ret.Add(new object[]
            {
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
                false,
                19,
                "19ZNT9zvtbmuPu5Yu7P9KUBFwVrhkfJKA3",
            });
            ret.Add(new object[]
            {
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
                true,
                0,
                "1KSezYMhAJMWqFbVFB2JshYg69UpmEXR4D",
            });
            ret.Add(new object[]
            {
                "cycle rocket west magnet parrot shuffle foot correct salt library feed song",
                true,
                9,
                "1L7mRQn5qNBmsRXvos3Etji1Twghdf8fVh",
            });

            //
            ret.Add(new object[]
            {
                "canvas lens access that bracket panther turtle fence mouse learn episode guilt",
                false,
                0,
                "16gkzS9LnerUgTwxWzCSYyh7PVss9AxFfu",
            });
            ret.Add(new object[]
            {
                "canvas lens access that bracket panther turtle fence mouse learn episode guilt",
                false,
                13,
                "1DAYp2KcfWSgxEiJLqNk9vs8jnNcJXV6kE",
            });
            ret.Add(new object[]
            {
                "canvas lens access that bracket panther turtle fence mouse learn episode guilt",
                true,
                1,
                "1A736Ak9adg3HcMaVbGmo9EHbF282BE29j",
            });
            ret.Add(new object[]
            {
                "canvas lens access that bracket panther turtle fence mouse learn episode guilt",
                true,
                8,
                "1MoPpA47exiFt2BMJn5L6TrAT2EixpSEi7",
            });

            return ret;
        }

        [Theory]
        [MemberData(nameof(GetWalletData))]
        public void GetWalletTest(string seedPhrase, bool isChange, int walletId, string actualAddress)
        {
            // #1
            var wallet = ElectrumMnemonic.GetWallet(seedPhrase, walletId, isChange);
            var actual1 = wallet
                .GetPublicKey()
                .GetAddress(ScriptPubKeyType.Legacy, Network.Main)
                .ToString();
            Assert.Equal(actualAddress, actual1);

            // #2
            var actual2 = ElectrumMnemonic.GetWalletAddress(seedPhrase, walletId, isChange);
            Assert.Equal(actualAddress, actual2);

            // #3
            var root = ElectrumMnemonic.GetExtRoot(seedPhrase);

            var wallet3 = ElectrumMnemonic.GetWallet(root, walletId, isChange);
            var actual3 = wallet3
                .GetPublicKey()
                .GetAddress(ScriptPubKeyType.Legacy, Network.Main)
                .ToString();
            Assert.Equal(actualAddress, actual3);

            // #4
            var actual4 = ElectrumMnemonic.GetWalletAddress(root, walletId, isChange);
            Assert.Equal(actualAddress, actual4);
        }
    }
}