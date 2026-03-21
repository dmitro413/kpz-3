using System.Text;
using LightHTML_Patterns;

namespace LightHTML_TemplateMethod
{
    public abstract class LightElementWithHooks : LightNode
    {
        public string TagName { get; }
        public DisplayType Display { get; }
        public ClosingType Closing { get; }
        protected List<string> CssClasses { get; } = new List<string>();
        protected List<LightNode> Children { get; } = new List<LightNode>();

        protected LightElementWithHooks(string tagName, DisplayType display, ClosingType closing)
        {
            TagName = tagName;
            Display = display;
            Closing = closing;
            OnCreated();
        }
        public void AddClass(string cls)
        {
            CssClasses.Add(cls);
            OnClassListApplied(cls);
        }

        public void Add(LightNode child)
        {
            Children.Add(child);
            if (child is LightTextNode t) OnTextRendered(t.InnerHTML);
            OnChildAdded(child);
        }
        public string Render()
        {
            var sb = new StringBuilder();
            sb.Append($"<{TagName}");

            string? inlineStyle = OnStylesApplied();
            if (!string.IsNullOrEmpty(inlineStyle)) sb.Append($" style=\"{inlineStyle}\"");

            if (CssClasses.Count > 0) sb.Append($" class=\"{string.Join(" ", CssClasses)}\"");

            if (Closing == ClosingType.Single)
            {
                sb.Append(" />");
            }
            else
            {
                sb.Append(">");
                foreach (var child in Children) sb.Append(child.OuterHTML);
                sb.Append($"</{TagName}>");
            }

            string html = sb.ToString();
            OnRendered(html);
            return html;
        }

        public override string OuterHTML => Render();
        public override string InnerHTML
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var c in Children) sb.Append(c.OuterHTML);
                return sb.ToString();
            }
        }
        protected virtual void OnCreated() { }

        protected virtual void OnChildAdded(LightNode child) { }
        protected virtual string? OnStylesApplied() => null;

        protected virtual void OnClassListApplied(string className) { }
        protected virtual void OnTextRendered(string text) { }
        protected virtual void OnRendered(string html) { }
    }
    public class LoggingDivElement : LightElementWithHooks
    {
        public LoggingDivElement() : base("div", DisplayType.Block, ClosingType.Paired) { }

        protected override void OnCreated() => Console.WriteLine("[Hook/OnCreated]        <div> щойно створено");

        protected override void OnChildAdded(LightNode child)
        {
            string label = child is LightElementNode el ? $"<{el.TagName}>" : "текст";
            Console.WriteLine($"[Hook/OnChildAdded]     додано: {label}");
        }
        protected override string? OnStylesApplied()
        {
            Console.WriteLine("[Hook/OnStylesApplied]  застосовано стилі");
            return "padding: 8px; border: 1px solid #ccc;";
        }
        protected override void OnClassListApplied(string className) => Console.WriteLine($"[Hook/OnClassListApplied] додано клас: '{className}'");

        protected override void OnTextRendered(string text)  => Console.WriteLine($"[Hook/OnTextRendered]   текст: «{text}»");

        protected override void OnRendered(string html) => Console.WriteLine($"[Hook/OnRendered]       HTML довжиною {html.Length} символів готовий");
    }
    public class TrackedButtonElement : LightElementWithHooks
    {
        private int _renderCount = 0;

        public TrackedButtonElement() : base("button", DisplayType.Inline, ClosingType.Paired) { }

        protected override void OnCreated() => Console.WriteLine("[Hook/OnCreated] <button> створено та готовий до вставки");

        protected override string? OnStylesApplied()
        {
            _renderCount++;
            return $"cursor:pointer; render-count:{_renderCount}";
        }
        protected override void OnRendered(string html) => Console.WriteLine($"[Hook/OnRendered] Рендер #{_renderCount}: <button> згенеровано");
    }
}