using LightHTML_Patterns;

namespace ClassLibrary
{
    public interface IImageLoadStrategy
    {
        void Load(string href);
    }

    public class NetworkImageLoadStrategy : IImageLoadStrategy
    {
        public void Load(string href)
        {
            Console.WriteLine($"[Стратегія: Мережа] Завантаження зображення через HTTP-запит з URL: {href}");
        }
    }

    public class LocalFileImageLoadStrategy : IImageLoadStrategy
    {
        public void Load(string href)
        {
            Console.WriteLine($"[Стратегія: Файли] Завантаження зображення з жорсткого диску за шляхом: {href}");
        }
    }

    public class LightImageNode : LightNode
    {
        private string _href;
        private IImageLoadStrategy _loadStrategy;

        public LightImageNode(string href)
        {
            _href = href;

            if (_href.StartsWith("http://") || _href.StartsWith("https://"))
            {
                _loadStrategy = new NetworkImageLoadStrategy();
            }
            else
            {
                _loadStrategy = new LocalFileImageLoadStrategy();
            }
        }

        public void LoadImage()
        {
            _loadStrategy.Load(_href);
        }

        public override string InnerHTML => string.Empty;
        public override string OuterHTML => $"<img src=\"{_href}\" />";
    }
}
