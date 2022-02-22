using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace Auth
{
    class Program
    {
        private const String usersFile = "users.json";

        public static List<User> Users;

        public static void InitUsers()
        {
            // Есть ли файл с пользователями?
            if (File.Exists(usersFile))
            {
                // считываем - десериализуем
                using (var reader = new StreamReader(usersFile))
                {
                    Users = JsonSerializer.Deserialize<List<User>>(
                        reader.ReadToEnd());
                }
            }
            else  // файла нет - создаем новый
            {
                // создаем коллекцию
                Users = new List<User>();

                // создаем пользователя, добавляем в коллекцию
                Users.Add(new User
                {
                    Login = "Admin",
                    Password = "123"
                });
                // сериализуем коллекцию и сохраняем в файл
                using (var writer = new StreamWriter(usersFile))
                {
                    writer.Write(
                        JsonSerializer.Serialize<List<User>>(Users)
                    );
                }
            }
        }

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            try
            {
                InitUsers();
            }
            catch (IOException ex)
            {
                Console.WriteLine("IO Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Init Error: " + ex.Message);
            }
            Console.WriteLine("Welcome");
            Console.WriteLine("----------------------------------------");
            while (true)
            {
                Console.WriteLine("1. Log in");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Admin");
                Console.WriteLine("0. Exit");
                var key = Console.ReadKey();
                if (key.KeyChar == '0') break;

                Console.WriteLine();
                switch (key.KeyChar)
                {
                    case '1': AuthUser(); break;
                    case '2': RegisterUser(); break;
                    case '3': ShowUsers(); break;
                    default: Console.WriteLine("Invalid argument"); break;
                }
            }

        }

        private static void AuthUser()
        {
            String userLogin;
            String userPassword;
            Console.WriteLine("Enter login: ");
            userLogin = Console.ReadLine();

            userPassword = String.Empty;
            ConsoleKeyInfo key;
            Console.WriteLine("Password: ");
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                Console.Write("*");
                userPassword += key.KeyChar;
            }
            Console.WriteLine();
            User user = Users.FirstOrDefault(u => u.Login == userLogin && u.Password == userPassword);
            if (user == null) Console.WriteLine("Auth denied");
            else
            {
                Console.WriteLine($"Welcome to system, {user.RealName}");
                user.LastLogin = DateTime.Now.ToString();
            }
        }
        private static void RegisterUser()
        {
            String userLogin;
            while (true)
            {
                Console.Write("Enter Login: ");
                userLogin = Console.ReadLine();
                if (userLogin.Equals(String.Empty))
                {
                    Console.WriteLine("Login could not be empty");
                }
                else if (userLogin.Contains(" "))
                {
                    Console.WriteLine("Spaces are not allowed in login");
                }
                else if (userLogin.Length < 4)
                {
                    Console.WriteLine("Login too short (4 sym min)");
                }
                else if (Users.Where(u => u.Login == userLogin).Count() > 0)
                {
                    Console.WriteLine("Login in use");
                }
                else break;
            }
            String userRealname;
            while (true)
            {
                Console.WriteLine("Your Real Name");
                userRealname = Console.ReadLine();
                string[] bannedSymb = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "/", @"\", "|" };
                //Regex regex = new Regex(@"^[^0-9]$");
                //bool isMatch = regex.IsMatch(userRealname);
                if (userRealname.Equals(String.Empty))
                {
                    Console.WriteLine("Real Name couldn't be empty");
                }
                else if (bannedSymb.Where(us => userRealname.Contains(us)).Any())
                {
                    Console.WriteLine("Имя должно содержать только буквы.");
                }
                else break;
            }

            String password1;
            String errorMessage;
            ConsoleKeyInfo key;
            do
            {
                errorMessage = String.Empty;
                password1 = String.Empty;
                Console.Write("Password: ");
                while (true)
                {
                    key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter) break;
                    Console.Write('*');
                    password1 += key.KeyChar;
                }
                if (password1.Length < 3)
                {
                    errorMessage = "Password too short (3 sym at least)";
                }
                Console.WriteLine(errorMessage);
            } while (errorMessage != String.Empty);

            String password2;
            password2 = String.Empty;
            Console.Write("Repeat password: ");
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                Console.Write('*');
                password2 += key.KeyChar;
            }
            if (!password1.Equals(password2))
            {
                Console.WriteLine();
                Console.WriteLine("Passwords mismatch. Registration cancelled");
                return;
            }

            // Add new user to list
            Users.Add(new User
            {
                Login = userLogin,
                Password = password1,
                RealName = userRealname,
                LastLogin = DateTime.Now.ToString()
            });
            // Serialize new list
            using (var writer = new StreamWriter(usersFile))
            {
                writer.Write(
                    JsonSerializer.Serialize<List<User>>(Users)
                );
            }
            Console.WriteLine($"\nSuccessful registration, {userRealname}");
        }

        private static void ShowUsers()
        {
            if (Users == null)
            {
                Console.WriteLine("Users init fail");
                return;
            }
            Console.WriteLine();
            Console.WriteLine("Как отсортировать пользователей? ");
            Console.WriteLine("1. По логину");
            Console.WriteLine("2. По имени");
            Console.WriteLine("3. По последней авторизации");
            Int32 choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    {
                        foreach (User user in Users.OrderBy((u) => { return u.Login; }))
                        {
                            Console.WriteLine(user);
                        }
                        break;
                    }
                case 2:
                    {
                        foreach (User user in Users.OrderBy((u) => { return u.RealName; }))
                        {
                            Console.WriteLine(user);
                        }
                        break;
                    }
                case 3:
                    {
                        foreach (User user in Users.OrderBy((u) => { return u.LastLogin; }))
                        {
                            Console.WriteLine(user);
                        }
                        break;
                    }
                default: Console.WriteLine("Invalid argument"); break;
            }
            Console.WriteLine();
        }
    }
}