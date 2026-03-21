using LightHTML_Patterns;

namespace LightHTML_Command
{
    public interface ILightCommand
    {
        void Execute();
        void Undo();
        string Description { get; }
    }

    public class AddChildCommand : ILightCommand
    {
        private readonly LightElementNode _parent;
        private readonly LightNode _child;

        public AddChildCommand(LightElementNode parent, LightNode child)
        {
            _parent = parent;
            _child = child;
        }

        public string Description => $"Додати {ChildLabel()} до <{_parent.TagName}>";

        public void Execute()
        {
            _parent.Children.Add(_child);
            Console.WriteLine($"[Command] Execute: {Description}");
        }

        public void Undo()
        {
            _parent.Children.Remove(_child);
            Console.WriteLine($"[Command] Undo: видалено {ChildLabel()} з <{_parent.TagName}>");
        }

        private string ChildLabel() => _child is LightElementNode el ? $"<{el.TagName}>" : "текстовий вузол";
    }

    public class RemoveChildCommand : ILightCommand
    {
        private readonly LightElementNode _parent;
        private readonly LightNode _child;
        private int _removedIndex = -1;

        public RemoveChildCommand(LightElementNode parent, LightNode child)
        {
            _parent = parent;
            _child = child;
        }

        public string Description => $"Видалити {ChildLabel()} з <{_parent.TagName}>";

        public void Execute()
        {
            _removedIndex = _parent.Children.IndexOf(_child);
            if (_removedIndex >= 0)
            {
                _parent.Children.RemoveAt(_removedIndex);
                Console.WriteLine($"[Command] Execute: {Description} (індекс {_removedIndex})");
            }
        }

        public void Undo()
        {
            if (_removedIndex >= 0)
            {
                _parent.Children.Insert(_removedIndex, _child);
                Console.WriteLine($"[Command] Undo: відновлено {ChildLabel()} у <{_parent.TagName}> на позиції {_removedIndex}");
            }
        }

        private string ChildLabel() => _child is LightElementNode el ? $"<{el.TagName}>" : "текстовий вузол";
    }

    public class AddClassCommand : ILightCommand
    {
        private readonly LightElementNode _element;
        private readonly string _className;

        public AddClassCommand(LightElementNode element, string className)
        {
            _element = element;
            _className = className;
        }

        public string Description => $"Додати клас '{_className}' до <{_element.TagName}>";

        public void Execute()
        {
            _element.CssClasses.Add(_className);
            Console.WriteLine($"[Command] Execute: {Description}");
        }

        public void Undo()
        {
            _element.CssClasses.Remove(_className);
            Console.WriteLine($"[Command] Undo: видалено клас '{_className}' з <{_element.TagName}>");
        }
    }

    public class SetTextCommand : ILightCommand
    {
        private readonly LightElementNode _parent;
        private readonly LightTextNode _node;
        private readonly string _newText;
        private string _oldText = string.Empty;

        public SetTextCommand(LightElementNode parent, LightTextNode node, string newText)
        {
            _parent = parent;
            _node = node;
            _newText = newText;
        }

        public string Description => $"Змінити текст у <{_parent.TagName}>";

        public void Execute()
        {
            _oldText = _node.InnerHTML;
            int idx = _parent.Children.IndexOf(_node);
            if (idx >= 0)
            {
                _parent.Children[idx] = new LightTextNode(_newText);
                Console.WriteLine($"[Command] Execute: {Description} → «{_newText}»");
            }
        }

        public void Undo()
        {
            int idx = _parent.Children.FindIndex(
                n => n is LightTextNode t && t.InnerHTML == _newText);
            if (idx >= 0)
            {
                _parent.Children[idx] = _node;
                Console.WriteLine($"[Command] Undo: {Description} → «{_oldText}»");
            }
        }
    }

    public class DomCommandManager
    {
        private readonly Stack<ILightCommand> _undoStack = new Stack<ILightCommand>();
        private readonly Stack<ILightCommand> _redoStack = new Stack<ILightCommand>();

        public void Execute(ILightCommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count == 0)
            {
                Console.WriteLine("[CommandManager] Немає дій для скасування.");
                return;
            }
            var cmd = _undoStack.Pop();
            cmd.Undo();
            _redoStack.Push(cmd);
        }

        public void Redo()
        {
            if (_redoStack.Count == 0)
            {
                Console.WriteLine("[CommandManager] Немає дій для повтору.");
                return;
            }
            var cmd = _redoStack.Pop();
            cmd.Execute();
            _undoStack.Push(cmd);
        }

        public void PrintHistory()
        {
            Console.WriteLine("\n[CommandManager] Стек Undo (остання дія вгорі):");
            int i = 0;
            foreach (var cmd in _undoStack)
                Console.WriteLine($"  [{i++}] {cmd.Description}");
        }
    }
}