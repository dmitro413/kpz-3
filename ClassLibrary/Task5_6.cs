using System.Collections.Generic;
using System.Text;

namespace LightHTML_Patterns
{
    public enum DisplayType { Block, Inline }
    public enum ClosingType { Single, Paired }

    public abstract class LightNode
    {
        public abstract string OuterHTML { get; }
        public abstract string InnerHTML { get; }
    }

    public class LightTextNode : LightNode
    {
        private string _text;
        public LightTextNode(string text) { _text = text; }
        public override string OuterHTML => _text;
        public override string InnerHTML => _text;
    }

    public class LightElementNode : LightNode
    {
        public string TagName { get; set; }
        public DisplayType Display { get; set; }
        public ClosingType Closing { get; set; }

        public List<string> CssClasses { get; set; } = new List<string>();
        public List<LightNode> Children { get; set; } = new List<LightNode>();

        private Dictionary<string, List<Action>> _eventListeners = new Dictionary<string, List<Action>>();

        public LightElementNode(string tagName, DisplayType display, ClosingType closing)
        {
            TagName = tagName;
            Display = display;
            Closing = closing;
        }

        public void Add(LightNode node) => Children.Add(node);
        public void AddEventListener(string eventType, Action listener)
        {
            if (!_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType] = new List<Action>();
            }
            _eventListeners[eventType].Add(listener);
        }
        public void RemoveEventListener(string eventType, Action listener)
        {
            if (_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType].Remove(listener);
            }
        }
        public void DispatchEvent(string eventType)
        {
            if (_eventListeners.ContainsKey(eventType))
            {
                Console.WriteLine($"[Подія] Викликано подію '{eventType}' на елементі <{TagName}>");
                foreach (var listener in _eventListeners[eventType])
                {
                    listener.Invoke();
                }
            }
        }
        public override string InnerHTML
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var child in Children) sb.Append(child.OuterHTML);
                return sb.ToString();
            }
        }

        public override string OuterHTML
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append($"<{TagName}");
                if (CssClasses.Count > 0)
                    sb.Append($" class=\"{string.Join(" ", CssClasses)}\"");

                if (Closing == ClosingType.Single) 
                {
                    sb.Append(" />");
                }
                else
                {
                    sb.Append(">");
                    sb.Append(InnerHTML);
                    sb.Append($"</{TagName}>");
                }
                return sb.ToString();
            }
        }
    }



    public class TagInfo
    {
        public string TagName { get; }
        public DisplayType Display { get; }
        public ClosingType Closing { get; }

        public TagInfo(string tagName, DisplayType display, ClosingType closing)
        {
            TagName = tagName;
            Display = display;
            Closing = closing;
        }
    }

    public class TagInfoFactory
    {
        private Dictionary<string, TagInfo> _pool = new Dictionary<string, TagInfo>();

        public TagInfo GetTagInfo   (string tagName, DisplayType display, ClosingType closing)
        {
            string key = $"{tagName}_{display}_{closing}";
            if (!_pool.ContainsKey(key))
            {
                _pool[key] = new TagInfo(tagName, display, closing);
            }
            return _pool[key];
        }
    }

    public class LightElementNodeFlyweight : LightNode
    {
        private TagInfo _tagInfo;

        private List<string> _cssClasses;
        private List<LightNode> _children;

        public LightElementNodeFlyweight(TagInfo tagInfo)
        {
            _tagInfo = tagInfo;
        }

        public void AddClass(string className)
        {
            if (_cssClasses == null) _cssClasses = new List<string>();
            _cssClasses.Add(className);
        }

        public void Add(LightNode node)
        {
            if (_children == null) _children = new List<LightNode>();
            _children.Add(node);
        }

        public override string InnerHTML
        {
            get
            {
                if (_children == null) return string.Empty;
                var sb = new StringBuilder();
                foreach (var child in _children) sb.Append(child.OuterHTML);
                return sb.ToString();
            }
        }

        public override string OuterHTML
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append($"<{_tagInfo.TagName}");
                if (_cssClasses != null && _cssClasses.Count > 0)
                    sb.Append($" class=\"{string.Join(" ", _cssClasses)}\"");

                if (_tagInfo.Closing == ClosingType.Single)
                {
                    sb.Append(" />");
                }
                else
                {
                    sb.Append(">");
                    sb.Append(InnerHTML);
                    sb.Append($"</{_tagInfo.TagName}>");
                }
                return sb.ToString();
            }
        }
    }
}