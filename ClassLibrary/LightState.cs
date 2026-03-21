using LightHTML_Patterns;

namespace LightHTML_State
{
    public interface IElementState
    {
        string StateName { get; }
        void Insert(StatefulLightElement element);
        void Remove(StatefulLightElement element);
        void Render(StatefulLightElement element);
        void AddChild(StatefulLightElement element, LightNode child);
    }
    public class CreatedState : IElementState
    {
        public string StateName => "Created";

        public void Insert(StatefulLightElement el)
        {
            Console.WriteLine($"[State/{StateName}] <{el.TagName}>: вставлено в документ.");
            el.SetState(new InsertedState());
        }

        public void Remove(StatefulLightElement el) => Console.WriteLine($"[State/{StateName}] <{el.TagName}>: неможливо видалити — елемент ще не вставлено.");

        public void Render(StatefulLightElement el) => Console.WriteLine($"[State/{StateName}] <{el.TagName}>: рендер недоступний до вставки.");

        public void AddChild(StatefulLightElement el, LightNode child)
        {
            el.Children.Add(child);
            Console.WriteLine($"[State/{StateName}] <{el.TagName}>: дочірній вузол додано (до вставки).");
        }
    }

    public class InsertedState : IElementState
    {
        public string StateName => "Inserted";

        public void Insert(StatefulLightElement el) => Console.WriteLine($"[State/{StateName}] <{el.TagName}>: вже у документі.");

        public void Remove(StatefulLightElement el)
        {
            Console.WriteLine($"[State/{StateName}] <{el.TagName}>: видалено з документу.");
            el.SetState(new RemovedState());
        }

        public void Render(StatefulLightElement el)
        {
            Console.WriteLine($"[State/{StateName}] <{el.TagName}>: рендер HTML:");
            Console.WriteLine("  " + el.OuterHTML);
        }

        public void AddChild(StatefulLightElement el, LightNode child)
        {
            el.Children.Add(child);
            Console.WriteLine($"[State/{StateName}] <{el.TagName}>: дочірній вузол додано (live).");
        }
    }

    public class RemovedState : IElementState
    {
        public string StateName => "Removed";

        public void Insert(StatefulLightElement el)
        {
            Console.WriteLine($"[State/{StateName}] <{el.TagName}>: відновлено в документі.");
            el.SetState(new InsertedState());
        }
        public void Remove(StatefulLightElement el) => Console.WriteLine($"[State/{StateName}] <{el.TagName}>: вже видалено.");

        public void Render(StatefulLightElement el) => Console.WriteLine($"[State/{StateName}] <{el.TagName}>: елемент видалено — рендер недоступний.");

        public void AddChild(StatefulLightElement el, LightNode child) => Console.WriteLine($"[State/{StateName}] <{el.TagName}>: неможливо додати дитину до видаленого елемента.");
    }

    public class StatefulLightElement : LightElementNode
    {
        private IElementState _state;

        public string CurrentState => _state.StateName;

        public StatefulLightElement(string tagName, DisplayType display, ClosingType closing)
            : base(tagName, display, closing)
        {
            _state = new CreatedState();
            Console.WriteLine($"[State/{_state.StateName}] <{tagName}>: елемент створено.");
        }

        internal void SetState(IElementState newState)
        {
            _state = newState;
        }
        public void Insert() => _state.Insert(this);
        public void Remove() => _state.Remove(this);
        public void Render() => _state.Render(this);
        public new void Add(LightNode child) => _state.AddChild(this, child);
    }
}