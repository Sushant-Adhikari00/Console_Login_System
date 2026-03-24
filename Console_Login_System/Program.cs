using System;
using System.Collections.Generic;
using System.IO;

namespace AccountRegistrationLoginApp
{
    class AccountSystem
    {
        private readonly string accountFolder = "accounts";
        private readonly string commonPasswordFile = "common_passwords.txt";
        private HashSet<string> commonPasswords;

        public AccountSystem()
        {
            CreateAccountFolder();
            LoadCommonPasswords();
        }

        private void CreateAccountFolder()
        {
            if (!Directory.Exists(accountFolder))
            {
                Directory.CreateDirectory(accountFolder);
            }
        }

        private void LoadCommonPasswords()
        {
            commonPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                if (File.Exists(commonPasswordFile))
                {
                    foreach (string line in File.ReadAllLines(commonPasswordFile))
                    {
                        string password = line.Trim();
                        if (!string.IsNullOrEmpty(password))
                        {
                            commonPasswords.Add(password);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Warning: common_passwords.txt file not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading common passwords: " + ex.Message);
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
                Console.WriteLine("Username contains invalid characters.");
                return false;
            }

            if (IsUsernameTaken(username))
            {
                Console.WriteLine("Registration failed: Username already exists.");
                return false;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 10)
            {
                Console.WriteLine("Registration failed: Password must be at least 10 characters long.");
                return false;
            }

            if (commonPasswords.Contains(password))
            {
                Console.WriteLine("Registration failed: Password is too common. Choose a stronger password.");
                return false;
            }

            return SaveUserToFile(username, password);
        }

        public bool LoginUser(string username, string password)
        {
            string filePath = GetUserFilePath(username);

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Login unsuccessful: Username not found.");
                return false;
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length >= 2 &&
                    lines[0] == username &&
                    lines[1] == password)
                {
                    Console.WriteLine("Login successful!");
                    return true;
                }

                Console.WriteLine("Login unsuccessful: Incorrect password.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading user file: " + ex.Message);
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
                {
                    return false;
                }
            }
            return true;
        }

        private bool SaveUserToFile(string username, string password)
        {
            try
            {
                File.WriteAllLines(GetUserFilePath(username), new string[] { username, password });
                Console.WriteLine("Registration successful!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving user file: " + ex.Message);
                return false;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            AccountSystem accountSystem = new AccountSystem();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n=== Account Registration and Login System ===");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Quit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter a username: ");
                        string regUsername = Console.ReadLine();

                        Console.Write("Enter a password: ");
                        string regPassword = Console.ReadLine();

                        accountSystem.RegisterUser(regUsername, regPassword);
                        break;

                    case "2":
                        Console.Write("Enter your username: ");
                        string loginUsername = Console.ReadLine();

                        Console.Write("Enter your password: ");
                        string loginPassword = Console.ReadLine();

                        accountSystem.LoginUser(loginUsername, loginPassword);
                        break;

                    case "3":
                        Console.WriteLine("Exiting program...");
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}