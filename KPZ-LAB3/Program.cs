using ClassLibrary;
using LightHTML_Patterns;
namespace KPZ_2LAB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("--- Завдання 1: Адаптер ---");
            LoggerTest();

            Console.WriteLine("--- Завдання 2: Декоратор ---");
            DecoratorTest();

            Console.WriteLine("--- Завдання 3: Міст ---");
            BridgeTest();

            Console.WriteLine("--- Завдання 4: Проксі ---");
            ProxyTest();

            Console.WriteLine("--- Завдання 5: Компонувальник ---");
            CompilerTest();

            Console.WriteLine("--- Завдання 6: Легковаговик ---");
            BookFlyweightTest();

            Console.ReadLine();
        }
        public static void LoggerTest()
        {
            ILogger consoleLogger = new ConsoleLogger();
            consoleLogger.Log("Система запущена.");
            consoleLogger.Warn("Пам'ять заповнена на 80%.");
            consoleLogger.Error("Критична помилка бази даних!");

            string logFile = "C:\\Users\\lenovo\\source\\repos\\KPZ-LAB3\\ClassLibrary\\system_log.txt";
            if (File.Exists(logFile)) File.Delete(logFile);

            FileWriter writer = new FileWriter(logFile);
            ILogger fileLogger = new FileLoggerAdapter(writer);

            fileLogger.Log("Запис у файл.");
            fileLogger.Warn("Файлове попередження.");
            fileLogger.Error("Файлова помилка.");
            Console.WriteLine($"Логи також записані у файл: {logFile}\n");
        }
        public static void DecoratorTest()
        {
            Hero myHero = new Warrior();
            Console.WriteLine($"Base: {myHero.GetDescription()} | Power: {myHero.GetPower()}");

            myHero = new Armor(myHero);
            myHero = new Weapon(myHero);
            myHero = new Artifact(myHero);
            myHero = new Weapon(myHero);

            Console.WriteLine($"Equipped: {myHero.GetDescription()} | Power: {myHero.GetPower()}\n");
        }
        public static void BridgeTest()
        {
            IRenderer vector = new VectorRenderer();
            IRenderer raster = new RasterRenderer();

            Shape circle = new Circle(vector);
            Shape triangle = new Triangle(raster);

            circle.Draw();
            triangle.Draw();
            Console.WriteLine();
        }
        public static void ProxyTest()
        {
            string testFile = "C:\\Users\\lenovo\\source\\repos\\KPZ-LAB3\\ClassLibrary\\test_proxy.txt";
            string secretFile = "C:\\Users\\lenovo\\source\\repos\\KPZ-LAB3\\ClassLibrary\\secret_file.txt";
            File.WriteAllText(testFile, "Hello World\nDesign Patterns");
            File.WriteAllText(secretFile, "Top Secret Data");

            ITextReader baseReader = new SmartTextReader();
            ITextReader checkerProxy = new SmartTextChecker(baseReader);

            ITextReader lockerProxy = new SmartTextReaderLocker(checkerProxy, ".*secret.*");

            Console.WriteLine("Читання звичайного файлу:");
            lockerProxy.ReadText(testFile);

            Console.WriteLine("\nЧитання захищеного файлу:");
            lockerProxy.ReadText(secretFile);
            Console.WriteLine();
        }
        public static void CompilerTest()
        {
            var ul = new LightElementNode("ul", DisplayType.Block, ClosingType.Paired);
            ul.CssClasses.Add("my-list");

            for (int i = 1; i <= 3; i++)
            {
                var li = new LightElementNode("li", DisplayType.Block, ClosingType.Paired);
                li.Add(new LightTextNode($"Пункт списку {i}"));
                ul.Add(li);
            }

            Console.WriteLine("OuterHTML згенерованого елемента:");
            Console.WriteLine(ul.OuterHTML);
            Console.WriteLine("InnerHTML згенерованого елемента:");
            Console.WriteLine(ul.InnerHTML);
        }

        public static void BookFlyweightTest()
        {
            string filePath = "C:\\Users\\lenovo\\source\\repos\\KPZ-LAB3\\ClassLibrary\\Book.txt";

            string[] bookLines = File.ReadAllLines(filePath);

            Console.WriteLine($"Прочитано рядків з файлу: {bookLines.Length}\n");

            GC.Collect();
            long memoryBeforeHeavy = GC.GetTotalMemory(true);

            var heavyTree = ParseWithoutFlyweight(bookLines);

            long memoryAfterHeavy = GC.GetTotalMemory(true);
            long heavyUsed = memoryAfterHeavy - memoryBeforeHeavy;

            heavyTree = null;
            GC.Collect();

            long memoryBeforeLight = GC.GetTotalMemory(true);

            var lightTree = ParseWithFlyweight(bookLines);

            long memoryAfterLight = GC.GetTotalMemory(true);
            long lightUsed = memoryAfterLight - memoryBeforeLight;

            Console.WriteLine($"Пам'ять БЕЗ Легковаговика (Звичайні класи): {heavyUsed / 1024 / 1024.0:F2} MB");
            Console.WriteLine($"Пам'ять З Легковаговиком (Flyweight):      {lightUsed / 1024 / 1024.0:F2} MB");

 
            Console.WriteLine("\nФрагмент згенерованої верстки (перші 150 символів):");
            var root = (LightElementNodeFlyweight)lightTree;
            string fullHtml = root.InnerHTML;

            string readableHtml = fullHtml.Replace("><", ">\n<");

            Console.WriteLine(readableHtml.Substring(0, Math.Min(1000, readableHtml.Length)));
        }
        static LightNode ParseWithoutFlyweight(string[] lines)
        {
            var root = new LightElementNode("div", DisplayType.Block, ClosingType.Paired);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                LightElementNode node;

                if (i == 0)
                    node = new LightElementNode("h1", DisplayType.Block, ClosingType.Paired);
                else if (line.Length < 20)
                    node = new LightElementNode("h2", DisplayType.Block, ClosingType.Paired);
                else if (line.StartsWith(" "))
                    node = new LightElementNode("blockquote", DisplayType.Block, ClosingType.Paired);
                else
                    node = new LightElementNode("p", DisplayType.Block, ClosingType.Paired);

                node.Add(new LightTextNode(line));
                root.Add(node);
            }
            return root;
        }

        static LightNode ParseWithFlyweight(string[] lines)
        {
            var factory = new TagInfoFactory();

            var divInfo = factory.GetTagInfo("div", DisplayType.Block, ClosingType.Paired);
            var h1Info = factory.GetTagInfo("h1", DisplayType.Block, ClosingType.Paired);
            var h2Info = factory.GetTagInfo("h2", DisplayType.Block, ClosingType.Paired);
            var bqInfo = factory.GetTagInfo("blockquote", DisplayType.Block, ClosingType.Paired);
            var pInfo = factory.GetTagInfo("p", DisplayType.Block, ClosingType.Paired);

            var root = new LightElementNodeFlyweight(divInfo);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                LightElementNodeFlyweight node;

                if (i == 0)
                    node = new LightElementNodeFlyweight(h1Info);
                else if (line.Length < 20)
                    node = new LightElementNodeFlyweight(h2Info);
                else if (line.StartsWith(" "))
                    node = new LightElementNodeFlyweight(bqInfo);
                else
                    node = new LightElementNodeFlyweight(pInfo);

                node.Add(new LightTextNode(line));
                root.Add(node);
            }
            return root;
        }
    }
}
