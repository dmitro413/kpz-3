using System.Text.RegularExpressions;
namespace ClassLibrary
{
    public interface ITextReader
    {
        char[][] ReadText(string filePath);
    }

    public class SmartTextReader : ITextReader
    {
        public char[][] ReadText(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            char[][] result = new char[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                result[i] = lines[i].ToCharArray();
            }
            return result;
        }
    }

    public class SmartTextChecker : ITextReader
    {
        private readonly ITextReader _reader;

        public SmartTextChecker(ITextReader reader) { _reader = reader; }

        public char[][] ReadText(string filePath)
        {
            Console.WriteLine($"[Checker] Відкриття файлу: {filePath}");
            char[][] result = _reader.ReadText(filePath);
            Console.WriteLine($"[Checker] Файл успішно прочитано та закрито.");

            int totalChars = 0;
            foreach (var row in result) totalChars += row.Length;

            Console.WriteLine($"[Checker] Рядків: {result.Length}, Символів: {totalChars}");
            return result;
        }
    }

    public class SmartTextReaderLocker : ITextReader
    {
        private readonly ITextReader _reader;
        private readonly Regex _restrictedRegex;

        public SmartTextReaderLocker(ITextReader reader, string regexPattern)
        {
            _reader = reader;
            _restrictedRegex = new Regex(regexPattern, RegexOptions.IgnoreCase);
        }

        public char[][] ReadText(string filePath)
        {
            if (_restrictedRegex.IsMatch(filePath))
            {
                Console.WriteLine("Access denied!");
                return Array.Empty<char[]>();
            }
            return _reader.ReadText(filePath);
        }
    }
}