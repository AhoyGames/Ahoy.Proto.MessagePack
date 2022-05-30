namespace Ahoy.Proto.MessagePack;

/// <summary>
/// Utility functions for consistent hashing.
/// </summary>
internal static class ConsistentHash
{
    /// <summary>
    /// Gets stable hashcode of the given string.
    /// In comparison to GetHashCode, this is always consistent between .net versions.
    /// Implementation is copied from https://stackoverflow.com/a/36845864
    /// </summary>
    public static int GetStableHashCode(this string str)
    {
        unchecked
        {
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i + 1] == '\0')
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

    public static long GetStableHashCode64(this string input)
    {
        string str1 = input.Substring(0, input.Length / 2);
        string str2 = input.Substring(input.Length / 2);
        return (long)str1.GetStableHashCode() << 32 | (long)str2.GetStableHashCode();
    }
}