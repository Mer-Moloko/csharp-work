using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    internal class Program
    {
        static List<string> wordList = new List<string>
        {
            "apple", "banana", "cherry", "dog", "cat", "elephant", 
            "fox", "giraffe", "house", "window", "door", "table", "chair", "computer", "phone", "book", "pen",
            "paper", "water", "coffee", "tea", "bread", "cheese", "butter", "milk", "sugar", "salt", "pepper",
            "happy", "sad", "angry", "excited", "brave", "clever", "gentle", "kind", "smart", "funny", "serious",
            "quiet", "loud", "bright", "dark", "warm", "cold", "hot", "cool", "sunny", "rainy", "windy", "snowy",
            "cloudy", "stormy", "summer", "winter", "spring", "autumn", "morning", "afternoon", "evening", "night",
            "today", "tomorrow", "yesterday", "always", "never", "sometimes", "often", "seldom", "quickly",
            "slowly", "carefully", "easily", "happily", "sadly", "beautiful", "ugly", "large", "small", "huge",
            "tiny", "heavy", "light", "strong", "weak", "fast", "slow", "early", "late", "young", "old", "new",
            "ancient", "modern", "future", "past", "present", "world", "peace", "love"
        };
            
        static List<string> passList = new List<string> 
        { 
            "123456", "123456789", "12345", "12345678", "1234", 
            "1234567", "111111", "1234567890", "qwerty", "qwertyuiop", 
            "qwer", "asdfghjk", "zxcvbnm", "wawawa", "pirates", 
            "password", "passwort", "admin", "secret", "master", 
            "access", "michael", "jordan", "daniel", "marina", 
            "hello", "welcome", "football", "soccer", "monkey" 
        };
        
        public static void Main(string[] args)
        {

            Console.WriteLine("Эта программа проверит вас на прочность и пароль тоже");
            while (true)
            {
                Console.WriteLine("Меню\n");
                Console.WriteLine("\t1 - Посчитать размер алфавита");
                Console.WriteLine("\t2 - Стойкость пароля в битах");
                Console.WriteLine("\t3 - Время перебора");
                Console.WriteLine("\t4 - Генератор пароля");
                Console.WriteLine("\t5 - Генератор парольной фразы");

                switch (Console.ReadLine() ?? "")
                {
                    case "1":
                    {
                        Console.WriteLine("Введите пароль: ");
                        string pass = Console.ReadLine() ?? "";
                        
                        if (GetPasswordCheck(pass))
                        {
                            Console.Clear();
                            Console.WriteLine($"Размер алфавита: {GetAlphabetSize(pass)}\n");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Пароль есть в базе данных или имеет недопустимый вид - ВЗРЫВ\n");
                            break;
                            
                        }
                    }

                    case "2":
                    {
                        Console.Clear();
                        Console.WriteLine("Введите пароль: ");
                        string pass = Console.ReadLine() ?? "";
                        if (GetPasswordCheck(pass))
                        {
                            Console.WriteLine($"Стойкость в битах: {GetBitSize(pass)}");
                            Console.WriteLine($"Статус пароля: {GetDurabilityScale(pass)}\n");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Пароль есть в базе данных или имеет недопустимый вид - ВЗРЫВ\n");
                            break;
                            
                        }
                    }

                    case "3":
                    {
                        Console.Clear();
                        Console.WriteLine("Введите пароль: ");
                        string pass = Console.ReadLine() ?? "";
                        if (GetPasswordCheck(pass))
                        {
                            Console.WriteLine("\nСкорость 10.000.000.000 паролей в секунду: ");
                            GetTimeSearch(Math.Pow(GetAlphabetSize(pass),pass.Length), 10e9 );
                            Console.WriteLine("\nСкорость 10.000 паролей в секунду: ");
                            GetTimeSearch(Math.Pow(GetAlphabetSize(pass),pass.Length), 10000 );
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Пароль есть в базе данных или имеет недопустимый вид - ВЗРЫВ\n");
                            break;
                            
                        }
                    }

                    case "4":
                    {
                        Console.WriteLine("Введите длинну пароля (от 8 до 64): ");
                        string passLength = Console.ReadLine() ?? "";
                        int.TryParse(passLength, out int length);
                        Console.WriteLine("Введите допустимые группы символов (1A% - пароль с цифрами, буквами и символами): ");
                        string groupChar = Console.ReadLine() ?? "";

                        if (length <= 64 && length >= 8)
                        {
                            string newPass = GenPassword(passLength, groupChar);
                            Console.WriteLine($"Сгенерированный пароль: {newPass}");
                            Console.WriteLine($"Стойкость в битах: {GetBitSize(newPass)}");
                            Console.WriteLine($"Статус пароля: {GetDurabilityScale(newPass)}");
                            Console.WriteLine("\nСкорость 10.000.000.000 паролей в секунду: ");
                            GetTimeSearch(Math.Pow(GetAlphabetSize(newPass),newPass.Length), 10e9 );
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Длинна не допустима!!! УНИЧТОЖЕНИЕ");
                            break;
                        }
                    }

                    case "5":
                    {
                        Console.WriteLine("Введите длинну парольной фразы: ");
                        int.TryParse(Console.ReadLine(), out int length);
                        string pass = GenWordPassword(length, out double bits);
                        Console.WriteLine($"Пароль: {pass}");
                        Console.WriteLine($"Стойкость: {bits:F1} бит");
                        
                        break;
                    }
                        
                    default:
                        Console.WriteLine("ААААА ВЗРЫВ");
                        return;
                }
            }
        }
        static string GenWordPassword(int length, out double bits)
        {
            string[] words = new string[length];
            Parallel.For(0, length, i =>
                words[i] = wordList[RandomNumberGenerator.GetInt32(wordList.Count)]);
            bits = length * Math.Log2(wordList.Count); 
            return string.Join("-", words);
        }
        static string GenPassword(string passLength, string groupChar)
        {
            string allowedChars = "";
            if (groupChar.Any(char.IsAsciiLetter)) allowedChars += "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            if (groupChar.Any(char.IsDigit))       allowedChars += "1234567890";
            if (groupChar.Any(c => char.IsAscii(c) && !char.IsLetterOrDigit(c))) allowedChars += "!@#$%^&*()";
                
            if (int.TryParse(passLength, out int length) && length > 0 && allowedChars.Length > 0)
            {
                return string.Create(length, allowedChars, (span, alphabet) => {
                    for (int i = 0; i < span.Length; i++)
                        span[i] = alphabet[RandomNumberGenerator.GetInt32(0, alphabet.Length)];
                });
            }
            return "";
        }
        static bool GetPasswordCheck(string pass)
        {
            if (string.IsNullOrEmpty(pass)) return false;
                
            if (pass.Distinct().Count() == 1) return false;
                
            if (passList.Any(w => Regex.IsMatch(pass, $"^{Regex.Escape(w)}\\d*$", RegexOptions.IgnoreCase)))
                return false;
                
            for (int i = 0; i <= pass.Length - 3; i++)
            {
                int d1 = pass[i + 1] - pass[i];
                int d2 = pass[i + 2] - pass[i + 1];
                if ((d1 == 1 && d2 == 1) || (d1 == -1 && d2 == -1))
                    return false;
            }

            return true;
        }
        static void GetTimeSearch(double seconds, double speed)
        {
            const double SecPerYear = 31557600, UniverseAge = 13.8e9;
            double s = seconds / speed, years = s / SecPerYear;

            if (years > UniverseAge)
                Console.WriteLine("Превышает возраст Вселенной");
            else
            {
                Console.WriteLine($"Минуты: {s / 60:G3}");
                Console.WriteLine($"Часы:   {s / 3600:G3}");
                Console.WriteLine($"Дни:    {s / 86400:G3}");
                Console.WriteLine($"Годы:   {years:G3}");
            }
        }
        static string GetDurabilityScale(string pass)
        {
            Double bits = GetBitSize(pass);
            if (bits < 28) return "очень фиговый";
            if (bits < 36) return "слабый";
            if (bits < 60) return "средний";
            if (bits < 128) return "сильный";
            return "МОЩА СОВЕТСКОГО СОЮЗА!!! ЗА РОДИНУ!!!";
        }
        static double GetBitSize(string pass)
        {
            return pass.Length * Math.Log2(GetAlphabetSize(pass));
        }
        static int GetAlphabetSize(string pass)
        {
            int isLetter = 0;
            int isUpper = 0;
            int isDigit = 0;
            int isAscii = 0;
                
            foreach (char a in pass)
            {
                if (char.IsLetter(a)) isLetter = 26;
                else if (char.IsUpper(a)) isUpper = 26;
                else if (char.IsDigit(a)) isDigit = 10;
                else if (char.IsAscii(a)) isAscii = 33;
            }
            return isDigit + isLetter + isUpper + isAscii;
        }
    }
}