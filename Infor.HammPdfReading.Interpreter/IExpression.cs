namespace Infor.HammPdfReading.Interpreter
{
    public interface IExpression<T>
    {
        public void Interpret(Context<T> context);
        public bool IsMatching { get; }
        public void Watch(Context<T> context);
        public void Write(Context<T> context);
    }
}