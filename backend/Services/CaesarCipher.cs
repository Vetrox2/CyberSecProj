namespace backend.Services
{
    public static class CaesarCipher
    {
        private const int DefaultShift = 3;

        public static string Encrypt(string text, int shift = DefaultShift)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var result = new char[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    int shifted = (c - offset + shift) % 26;
                    if (shifted < 0) shifted += 26;
                    result[i] = (char)(shifted + offset);
                }
                else if (char.IsDigit(c))
                {
                    int shifted = (c - '0' + shift) % 10;
                    if (shifted < 0) shifted += 10;
                    result[i] = (char)(shifted + '0');
                }
                else
                {
                    result[i] = c;
                }
            }

            return new string(result);
        }

        public static string Decrypt(string text, int shift = DefaultShift)
        {
            return Encrypt(text, -shift);
        }

        public static string GenerateUnlockKey(Guid userId)
        {
            var userIdString = userId.ToString("N");
            var timestamp = DateTime.UtcNow.Ticks.ToString();
            var rawKey = $"{userIdString.Substring(0, 8)}-{timestamp.Substring(timestamp.Length - 8)}";

            return Encrypt(rawKey);
        }

        public static bool ValidateUnlockKey(string encryptedKey, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(encryptedKey))
                return false;

            try
            {
                var decrypted = Decrypt(encryptedKey);

                var parts = decrypted.Split('-');
                if (parts.Length != 2)
                    return false;

                var userIdPart = parts[0];
                var userIdString = userId.ToString("N").Substring(0, 8);

                return userIdPart.Equals(userIdString, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
