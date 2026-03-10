using System.Text;
using System.IO;

namespace TextFileMerger
{
    class Program
    {
        static readonly HashSet<string> IgnoredFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
        };

        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".sln", ".user", ".md", ".json", ".txt", ".cs", ".xml", ".gitignore", ".gitattributes", ".html", ".htm",
            ".css", ".js", ".ts", ".yml", ".yaml", ".ini", ".config", ".conf", ".bat", ".svelte", ".sh", ".ps1", ".php",
            ".scss", ".jsx", ".py", ".java", ".cpp", ".h", ".hpp", ".c", ".go", ".rs", ".swift", ".kt", ".sql", ".log",
            ".csv", ".tsv", ".csproj"
        };
        
        private static readonly StringBuilder Output = new StringBuilder();
            
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }
            
            string outputFilePath = args[0];
            string currentDir = Directory.GetCurrentDirectory();
            
            IgnoredFiles.Add(Path.GetFileName(outputFilePath));

            try
            {
                ProcessDirectory(currentDir, currentDir);
                File.WriteAllText(outputFilePath, Output.ToString());

                Console.WriteLine($"Successfully created: {outputFilePath}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void ProcessTextFile(string filePath, string rootDir)
        {
            string relativePath = filePath.Substring(rootDir.Length).TrimStart(Path.DirectorySeparatorChar);

            Output.AppendLine($"// ======== FILE: {relativePath} ========");
            Output.AppendLine(File.ReadAllText(filePath));
            Output.AppendLine();
        }

        static void ProcessDirectory(string currentDir, string rootDir)
        {
            foreach (var filePath in Directory.GetFiles(currentDir))
            {
                string fileName = Path.GetFileName(filePath);
                string extension = Path.GetExtension(filePath);

                if (IgnoredFiles.Contains(fileName))
                {
                    continue;
                }

                if (AllowedExtensions.Contains(extension))
                {
                    ProcessTextFile(filePath, rootDir);
                }
            }

            foreach (var dir in Directory.GetDirectories(currentDir))
            {
                ProcessDirectory(dir, rootDir);
            }
        }
    }
}