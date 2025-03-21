
namespace Stegosaurus.CLI
{
    public static class CliInputHandler
    {
        public static bool IsValid(string filePath)
        {
            if (!Path.IsPathRooted(filePath))
                filePath = Path.GetFullPath(filePath);
            return Path.Exists(filePath);
        }

        public static string ReadPassword()
        {
            Console.Write("Enter password: ");
            string password = "";
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            return password;
        }   
    }
}