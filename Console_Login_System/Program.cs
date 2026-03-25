using System;
using System.Collections.Generic;
using System.IO;

namespace AccountRegistrationLoginApp
{
    class AccountSystem
    {
        private readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;

        private string accountFolder;
        private string commonPasswordFile;

        private HashSet<string> commonPasswords;

        public AccountSystem()
        {
            accountFolder = Path.Combine(basePath, "accounts");
            commonPasswordFile = Path.Combine(basePath, "common_passwords.txt");

            CreateAccountFolder();
            EnsurePasswordFileExists();
            LoadCommonPasswords();
        }

        private void CreateAccountFolder()
        {
            if (!Directory.Exists(accountFolder))
            {
                Directory.CreateDirectory(accountFolder);
            }
        }

        // Only creates empty file (NO passwords inside code)
        private void EnsurePasswordFileExists()
        {
            if (!File.Exists(commonPasswordFile))
            {
                File.Create(commonPasswordFile).Close();

                Console.WriteLine("\nIMPORTANT:");
                Console.WriteLine("common_passwords.txt created at:");
                Console.WriteLine(commonPasswordFile);
                Console.WriteLine("Please open it and add common passwords.\n");
            }
        }

        private void LoadCommonPasswords()
        {
            commonPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                foreach (string line in File.ReadAllLines(commonPasswordFile))
                {
                    string password = line.Trim();
                    if (!string.IsNullOrEmpty(password))
                    {
                        commonPasswords.Add(password);
                    }
                }

                Console.WriteLine("Loaded passwords: " + commonPasswords.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading password file: " + ex.Message);
            }
        }

        public bool RegisterUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty.");
                return false;
            }

            if (!IsValidUsername(username))
            {
                Console.WriteLine("Invalid username.");
                return false;
            }

            if (IsUsernameTaken(username))
            {
                Console.WriteLine("Username already exists.");
                return false;
            }

            if (password.Length < 10)
            {
                Console.WriteLine("Password must be at least 10 characters.");
                return false;
            }

            if (commonPasswords.Contains(password))
            {
                Console.WriteLine("Password is too common.");
                return false;
            }

            return SaveUser(username, password);
        }

        public bool LoginUser(string username, string password)
        {
            string path = GetUserFilePath(username);

            if (!File.Exists(path))
            {
                Console.WriteLine("Username not found.");
                return false;
            }

            try
            {
                string[] data = File.ReadAllLines(path);

                if (data.Length >= 2 &&
                    data[0] == username &&
                    data[1] == password)
                {
                    Console.WriteLine("Login successful!");
                    return true;
                }

                Console.WriteLine("Incorrect password.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
                return false;
            }
        }

        private bool IsUsernameTaken(string username)
        {
            return File.Exists(GetUserFilePath(username));
        }

        private string GetUserFilePath(string username)
        {
            return Path.Combine(accountFolder, username + ".txt");
        }

        private bool IsValidUsername(string username)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                if (username.Contains(c))
                    return false;
            }
            return true;
        }

        private bool SaveUser(string username, string password)
        {
            try
            {
                File.WriteAllLines(GetUserFilePath(username), new string[] { username, password });
                Console.WriteLine("Registration successful!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving user: " + ex.Message);
                return false;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            AccountSystem system = new AccountSystem();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n==== MENU ====");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Quit");
                Console.Write("Choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Username: ");
                        string u1 = Console.ReadLine();

                        Console.Write("Password: ");
                        string p1 = Console.ReadLine();

                        system.RegisterUser(u1, p1);
                        break;

                    case "2":
                        Console.Write("Username: ");
                        string u2 = Console.ReadLine();

                        Console.Write("Password: ");
                        string p2 = Console.ReadLine();

                        system.LoginUser(u2, p2);
                        break;

                    case "3":
                        running = false;
                        Console.WriteLine("Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}