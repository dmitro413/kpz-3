using LightHTML_Patterns;

namespace ClassLibrary
{
    public interface ILightNodeIterator
    {
        bool HasNext();
        LightNode Next();
        void Reset();
    }

    public class DepthFirstIterator : ILightNodeIterator
    {
        private readonly LightNode _root;
        private readonly Stack<LightNode> _stack = new Stack<LightNode>();

        public DepthFirstIterator(LightNode root)
        {
            _root = root;
            _stack.Push(root);
        }

        public bool HasNext() => _stack.Count > 0;

        public LightNode Next()
        {
            var node = _stack.Pop();
            if (node is LightElementNode element)
            {
                for (int i = element.Children.Count - 1; i >= 0; i--)
                    _stack.Push(element.Children[i]);
            }

            return node;
        }

        public void Reset()
        {
            _stack.Clear();
            _stack.Push(_root);
        }
    }

    public class BreadthFirstIterator : ILightNodeIterator
    {
        private readonly LightNode _root;
        private readonly Queue<LightNode> _queue = new Queue<LightNode>();

        public BreadthFirstIterator(LightNode root)
        {
            _root = root;
            _queue.Enqueue(root);
        }

        public bool HasNext() => _queue.Count > 0;

        public LightNode Next()
        {
            var node = _queue.Dequeue();

            if (node is LightElementNode element)
            {
                foreach (var child in element.Children)
                    _queue.Enqueue(child);
            }

            return node;
        }

        public void Reset()
        {
            _queue.Clear();
            _queue.Enqueue(_root);
        }
    }
    public static class LightElementNodeIteratorExtensions
    {
        public static ILightNodeIterator GetDepthFirstIterator(this LightElementNode node)
            => new DepthFirstIterator(node);

        public static ILightNodeIterator GetBreadthFirstIterator(this LightElementNode node)
            => new BreadthFirstIterator(node);
    }
}