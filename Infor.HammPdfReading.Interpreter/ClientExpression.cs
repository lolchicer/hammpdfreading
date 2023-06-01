namespace Infor.HammPdfReading.Interpreter
{
    public abstract class ClientExpression<T> : IExpression<T>
    {
        protected abstract void WatchBody(Context<T> context);
        protected abstract void WriteBody(Context<T> context);

        public abstract bool IsMatching { get; }

        protected abstract void Move(Context<T> context);

        public void Watch(Context<T> context)
        {
            if (context.Index < context.Text.Length)
            {
                WatchBody(context);
                if (IsMatching)
                    Move(context);
            }
        }

        public void Write(Context<T> context)
        {
            WriteBody(context);
            Move(context);
        }

        public void Interpret(Context<T> context)
        {
            Watch(context);
            context.Index = 0;
            if (IsMatching)
                Write(context);
        }
    }
}
