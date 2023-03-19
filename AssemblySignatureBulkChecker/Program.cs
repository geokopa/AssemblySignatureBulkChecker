using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AssemblySignatureBulkChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string directoryPath = @"C:\Users\gkopadze\Desktop\Test";

            if (args.Length > 0 && args[0] == "-d")
            {
                directoryPath = args[0];
            }

            // Specify the directory where the assemblies are located
            string signInToolSdk = "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.19041.0\\x64\\signtool.exe";

            Console.WriteLine($"Load assemblies from directory: {directoryPath}");

            // Get all files with the .dll or .exe extension from the directory
            string[] assemblyFiles = Directory.GetFiles(directoryPath, "*.dll");
            assemblyFiles = assemblyFiles.Concat(Directory.GetFiles(directoryPath, "*.exe")).ToArray();

            foreach (string assemblyFile in assemblyFiles)
            {
                try
                {
                    // Call the signtool verify command on the assembly file
                    ProcessStartInfo startInfo = new(signInToolSdk, $"verify /pa /v \"{assemblyFile}\"")
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };

                    Process process = Process.Start(startInfo) ?? new Process();

                    process.WaitForExit();

                    // Check if the output of the command indicates that the assembly is properly signed
                    string output = process.StandardOutput.ReadToEnd();
                    bool isProperlySigned = Regex.IsMatch(output, @"Number of errors: 0");

                    // Display a message indicating whether the assembly is properly signed or not
                    if (isProperlySigned)
                    {
                        Success($"{assemblyFile} is properly signed");
                    }
                    else
                    {
                        Error($"{assemblyFile} is not properly signed");
                    }
                }
                catch (Exception ex)
                {
                    Error($"Error checking signature of {assemblyFile}: {ex.Message}");
                }
            }

            Console.WriteLine("\nVerification has been completed successfully");
            Console.ReadLine();
        }

        static void Success(string message)
        {
            DisplayMessage(message, ConsoleColor.Green);
        }

        static void Error(string message)
        {
            DisplayMessage(message, ConsoleColor.Red);
        }

        static void DisplayMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}