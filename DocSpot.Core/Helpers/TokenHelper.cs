namespace DocSpot.Core.Helpers
{
    public static class TokenHelper
    {
        /// <summary>
        /// Generates a cryptographically secure, URL-safe token encoded as a Base64 string.
        /// </summary>
        /// <remarks>This method replaces '+' and '/' characters with '-' and '_', respectively, and
        /// removes any trailing '=' padding to ensure the token is safe for use in URLs and filenames. The token is
        /// suitable for use as a unique identifier or secret in web applications.</remarks>
        /// <param name="length">The desired length, in bytes, of the random token. Must be a positive integer. The resulting string will be
        /// longer than this value due to Base64 encoding.</param>
        /// <returns>A URL-safe Base64-encoded string representing the random token. The string contains only alphanumeric
        /// characters, hyphens, and underscores, and does not include padding.</returns>
        public static string GenerateUrlSafeToken(int length = 32)
        {
            var randomBytes = new byte[length];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }

        // SHA256 hash (hex string) to store in DB
        public static string ComputeSha256Hash(string rawData)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                var builder = new System.Text.StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
