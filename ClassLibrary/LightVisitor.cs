using System.Text;
using LightHTML_Patterns;

namespace LightHTML_Visitor
{
    public interface ILightNodeVisitor
    {
        void VisitElement(VisitableLightElement element);
        void VisitText(VisitableLightText text);
    }
    public interface IVisitableLightNode
    {
        void Accept(ILightNodeVisitor visitor);
    }
    public class VisitableLightElement : LightNode, IVisitableLightNode
    {
        public string TagName { get; }
        public DisplayType Display { get; }
        public ClosingType Closing { get; }
        public List<string> CssClasses { get; } = new();
        public List<IVisitableLightNode> VisitableChildren { get; } = new();

        public VisitableLightElement(string tagName, DisplayType display, ClosingType closing)
        {
            TagName = tagName;
            Display = display;
            Closing = closing;
        }

        public void AddChild(VisitableLightElement child) => VisitableChildren.Add(child);
        public void AddText(VisitableLightText text) => VisitableChildren.Add(text);
        public void AddClass(string cls) => CssClasses.Add(cls);

        public void Accept(ILightNodeVisitor visitor)
        {
            visitor.VisitElement(this);
            foreach (var child in VisitableChildren) child.Accept(visitor);
        }
        public override string InnerHTML
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var c in VisitableChildren)
                    if (c is LightNode n) sb.Append(n.OuterHTML);
                return sb.ToString();
            }
        }
        public override string OuterHTML => InnerHTML;
    }

    public class VisitableLightText : LightNode, IVisitableLightNode
    {
        public string Text { get; }
        public VisitableLightText(string text) { Text = text; }

        public void Accept(ILightNodeVisitor visitor) => visitor.VisitText(this);

        public override string InnerHTML => Text;
        public override string OuterHTML => Text;
    }
    public class HtmlRenderVisitor : ILightNodeVisitor
    {
        private readonly StringBuilder _sb = new();
        private int _indent = 0;

        public string Result => _sb.ToString();

        public void VisitElement(VisitableLightElement el)
        {
            string pad = new string(' ', _indent * 2);
            _sb.Append(pad).Append($"<{el.TagName}");

            if (el.CssClasses.Count > 0)
                _sb.Append($" class=\"{string.Join(" ", el.CssClasses)}\"");

            if (el.Closing == ClosingType.Single)
            {
                _sb.AppendLine(" />");
                return;
            }

            _sb.AppendLine(">");
            _indent++;
            foreach (var child in el.VisitableChildren)
                child.Accept(this);
            _indent--;
            _sb.AppendLine(pad + $"</{el.TagName}>");
        }

        public void VisitText(VisitableLightText text)
        {
            string pad = new string(' ', _indent * 2);
            if (!string.IsNullOrWhiteSpace(text.Text))
                _sb.AppendLine(pad + text.Text);
        }
    }
    public class StatisticsVisitor : ILightNodeVisitor
    {
        public int ElementCount { get; private set; }
        public int TextNodeCount { get; private set; }
        public int MaxDepth { get; private set; }
        public Dictionary<string, int> TagFrequency { get; } = new();

        private int _currentDepth = 0;

        public void VisitElement(VisitableLightElement el)
        {
            ElementCount++;
            _currentDepth++;
            if (_currentDepth > MaxDepth) MaxDepth = _currentDepth;

            TagFrequency[el.TagName] = TagFrequency.TryGetValue(el.TagName, out int c) ? c + 1 : 1;

            foreach (var child in el.VisitableChildren) child.Accept(this);

            _currentDepth--;
        }

        public void VisitText(VisitableLightText text) => TextNodeCount++;

        public void PrintReport()
        {
            Console.WriteLine("[StatisticsVisitor] Звіт:");
            Console.WriteLine($"  Елементів:       {ElementCount}");
            Console.WriteLine($"  Текстових вузлів:{TextNodeCount}");
            Console.WriteLine($"  Макс. глибина:   {MaxDepth}");
            Console.WriteLine("  Частота тегів:");
            foreach (var kv in TagFrequency) Console.WriteLine($"    <{kv.Key}>: {kv.Value}");
        }
    }

    public class TextExtractVisitor : ILightNodeVisitor
    {
        private readonly StringBuilder _sb = new();

        public string Text => _sb.ToString().Trim();

        public void VisitElement(VisitableLightElement el)
        {
            foreach (var child in el.VisitableChildren)
                child.Accept(this);
            if (el.Display == DisplayType.Block && _sb.Length > 0 && _sb[^1] != '\n')
                _sb.Append('\n');
        }

        public void VisitText(VisitableLightText text)
        {
            if (!string.IsNullOrWhiteSpace(text.Text))
                _sb.Append(text.Text + " ");
        }
    }

    public class ClassAuditVisitor : ILightNodeVisitor
    {
        private readonly Dictionary<string, List<string>> _classToTags = new();

        public void VisitElement(VisitableLightElement el)
        {
            foreach (var cls in el.CssClasses)
            {
                if (!_classToTags.ContainsKey(cls))
                    _classToTags[cls] = new List<string>();
                _classToTags[cls].Add(el.TagName);
            }
            foreach (var child in el.VisitableChildren)
                child.Accept(this);
        }

        public void VisitText(VisitableLightText text) { }

        public void PrintReport()
        {
            Console.WriteLine("[ClassAuditVisitor] CSS-класи в документі:");
            if (_classToTags.Count == 0)
            {
                Console.WriteLine("  (немає класів)");
                return;
            }
            foreach (var kv in _classToTags) Console.WriteLine($"  .{kv.Key} → використовується в: {string.Join(", ", kv.Value.Select(t => $"<{t}>"))}");
        }
    }
}