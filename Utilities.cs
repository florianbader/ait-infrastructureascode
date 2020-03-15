using System.Text;

namespace AIT.InfrastructureAsCode
{
    public static class Utilities
    {
        private static CryptoRandom _cryptoRandom = new CryptoRandom();

        public static string GeneratePassword(int length = 10)
        {
            const string NonAlphanumericCharacters = "#!%$";
            const string Numbers = "1234567890";
            const string AlphanumericCharactersLowercase = "abcdefghijklmnopqrstuvwxyz";
            const string AlphanumericCharactersUppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string Valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            bool hasUppercase = false;
            bool hasLowercase = false;
            bool hasNumbers = false;
            bool hasNonAlphanumeric = false;

            var stringBuilder = new StringBuilder();

            while (0 < length--)
            {
                var character = Valid[_cryptoRandom.Next(Valid.Length)];
                stringBuilder.Append(character);

                if (NonAlphanumericCharacters.Contains(character))
                {
                    hasNonAlphanumeric = true;
                }

                if (AlphanumericCharactersLowercase.Contains(character))
                {
                    hasLowercase = true;
                }

                if (AlphanumericCharactersUppercase.Contains(character))
                {
                    hasUppercase = true;
                }

                if (Numbers.Contains(character))
                {
                    hasNumbers = true;
                }
            }

            if (!hasLowercase)
            {
                stringBuilder.Append(AlphanumericCharactersLowercase[_cryptoRandom.Next(AlphanumericCharactersLowercase.Length)]);
            }

            if (!hasUppercase)
            {
                stringBuilder.Append(AlphanumericCharactersUppercase[_cryptoRandom.Next(AlphanumericCharactersUppercase.Length)]);
            }

            if (!hasNonAlphanumeric)
            {
                stringBuilder.Append(NonAlphanumericCharacters[_cryptoRandom.Next(NonAlphanumericCharacters.Length)]);
            }

            if (!hasNumbers)
            {
                stringBuilder.Append(Numbers[_cryptoRandom.Next(Numbers.Length)]);
            }

            return stringBuilder.ToString();
        }
    }
}