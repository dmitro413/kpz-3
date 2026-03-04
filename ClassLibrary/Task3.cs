namespace ClassLibrary
{
    public interface IRenderer
    {
        void RenderShape(string shapeName);
    }

    public class VectorRenderer : IRenderer
    {
        public void RenderShape(string shapeName) => Console.WriteLine($"Drawing {shapeName} as vector lines.");
    }

    public class RasterRenderer : IRenderer
    {
        public void RenderShape(string shapeName) => Console.WriteLine($"Drawing {shapeName} as pixels.");
    }

    public abstract class Shape
    {
        protected IRenderer _renderer;

        protected Shape(IRenderer renderer)
        {
            _renderer = renderer;
        }
        public abstract void Draw();
    }

    public class Circle : Shape
    {
        public Circle(IRenderer renderer) : base(renderer) { }
        public override void Draw() => _renderer.RenderShape("Circle");
    }

    public class Square : Shape
    {
        public Square(IRenderer renderer) : base(renderer) { }
        public override void Draw() => _renderer.RenderShape("Square");
    }

    public class Triangle : Shape
    {
        public Triangle(IRenderer renderer) : base(renderer) { }
        public override void Draw() => _renderer.RenderShape("Triangle");
    }
}
