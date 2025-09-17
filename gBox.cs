//Сам попытался и таки сгенерировал код для последней версии (41.3.30.1038).
//E3JP-1G1H-IR7K-F63L

using System;
using System.Linq;
using System.Reflection;
 
namespace GemBox.Spreadsheet
{
    public static class SpreadsheetLicense
    {
        private static readonly Type GxRandomType;
        private static readonly Random Random;
 
        public static void Apply()
        {
            var version = GetReleaseVersion();
            var serialKey = GetSerialKey(version);
            SpreadsheetInfo.SetLicense(serialKey);
        }
 
        private static string GetSerialKey(int version)
        {
            var licParams = KeyUtilities
                .NumberToString(
                    (int)KeyUtilities.LicenseOptions.FullLicense) +  
                    KeyUtilities.NumberToString(version);
            licParams = licParams + GetRandomString(13 - licParams.Length);
 
            var chars = new char[14];
            chars[0] = GetRandomChar();
 
            var args = KeyUtilities.CharToDigit36(chars[0]) * 27961;
            var gxRandom = Activator.CreateInstance(
                GxRandomType,  
                BindingFlags.Public | BindingFlags.Instance,  
                null,  
                new object[]{ args },  
                null);
 
            for (var index = 0; index < licParams.Length; ++index)
            {
                var random = (int)GxRandomType
                    .GetMethod("Next", new [] { typeof(int), typeof(int) })
                    .Invoke(gxRandom, new object[] { 0, 36 });
                var digit = KeyUtilities.CharToDigit36(licParams[index]);
                var convert = KeyUtilities.RandomConverter(digit, random);
                chars[index + 1] = KeyUtilities.Digit36ToChar(convert);
            }
 
            var pass = "E" + new string(chars);
            pass = pass + KeyUtilities.GetHash(pass);
 
            return string.Format("{0}-{1}-{2}-{3}",
                pass.Substring(0, 4),
                pass.Substring(4, 4),
                pass.Substring(8, 4),
                pass.Substring(12, 4));
        }
 
        private static int GetReleaseVersion()
        {
            var field = typeof(SpreadsheetInfo).GetField(
                "LicenseReleaseID",  
                BindingFlags.NonPublic | BindingFlags.Static);
 
            return (int)(field?.GetValue(null) ?? 0);
        }
 
        private static string GetRandomString(int length)
        {
            var chars = new char[length];
 
            for (var index = 0; index < length; ++index)
                chars[index] = KeyUtilities.Digit36ToChar(Random.Next(0, 36));
 
            return new string(chars);
        }
 
        private static char GetRandomChar()
        {
            return KeyUtilities.Digit36ToChar(Random.Next(0, 36));
        }
 
        static SpreadsheetLicense()
        {
            Random = new Random();
 
            var types = Assembly.GetAssembly(typeof(SpreadsheetInfo))
                .DefinedTypes.Select(x => x.AsType()).ToList();
 
            GxRandomType = types.First(x => x.Name.Equals("GXRandom"));
        }
 
        private static class KeyUtilities
        {
            public static char GetHash(string str)
            {
                var num1 = 0;
                for (var index = 0; index < str.Length; ++index)
                {
                    var num2 = CharToDigit36(str[index]);
                    if (index % 2 == 1)
                    {
                        var num3 = num2 * 2;
                        num1 += num3 / 36 + num3 % 36;
                    }
                    else
                        num1 += num2;
                }
                return Digit36ToChar(num1 % 36);
            }
 
            public static string NumberToString(int num)
            {
                var result = string.Empty;
                while (num > 0)
                {
                    var charV = num % 36;
                    result = Digit36ToChar(charV) + result;
 
                    num = num - charV;
                    if (num % 36 == 0)
                        num = num / 36;
                }
                return result;
            }
 
            public static char Digit36ToChar(int digit)
            {
                if (digit > 9)
                    return (char)(65 + digit - 10);
                return (char)(48 + digit);
            }
 
            public static int CharToDigit36(char ch)
            {
                if (!char.IsDigit(ch) && ch < 65 && ch > 90)
                    throw new ArgumentException("Wrong char value.", "ch");
                if (char.IsDigit(ch))
                    return ch - 48;
                return ch - 65 + 10;
            }
 
            public static int RandomConverter(int num, int random)
            {
                if (num == 0 & random == 0)
                    return 36;
                if (num == 0 & random == 36)
                    return 0;
                return (num + random) % 36;
            }
 
            public enum LicenseOptions
            {
                None = 0,
                UseMaintenanceNumber = 1,
                UseLockingCode = 2,
                RuntimeLicense = 4,
                FullLicense = 8,
                TimeLimited = 32
            }
        }
    }
}
