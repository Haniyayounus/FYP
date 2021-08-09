using System;
using System.Linq;

namespace Medius.Utility
{
    public static class StringUtility
    {
        public static string GetNumbers(this string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }
        public static string TrimSpacesFromString(string input)
        {
            var trimmedString = string.Empty;
            input = input.Replace("$", " $ ");
            var stringArray = input.Split(new[] { " " }, StringSplitOptions.None);
            foreach (var item in stringArray)
            {
                if (!string.IsNullOrEmpty(item.Trim()))
                {
                    trimmedString += item + " ";
                }
            }
            return trimmedString.Trim();
        }

        //Fetches the  index of the character occuring on specific time in string
        public static int GetNthIndex(string s, char searchChar, int time)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == searchChar)
                {
                    count++;
                    if (count == time)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static string GenerateVerificationCode(int characterCount, bool alphanumeric = false, int id = 0)
        {
            var random = new Random();
            string result;
            if (alphanumeric)
            {
                var idString = id.ToString();
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var stringChars = new char[6 - idString.Length];

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                result = new string(stringChars);
                return result + idString;
            }

            result = random.Next((int)Math.Pow(10, (characterCount - 1)), (int)Math.Pow(10, characterCount) - 1)
                .ToString();

            return result;
        }
    }
}
