namespace Infor.HammPdfReading.Interpreter
{
    public abstract class VerticalExpression<T> : ClientExpression<T>
    {
        bool _isMatching = false;

        protected abstract IExpression<T>[] Expressions { get; }

        protected override bool IsMatchingBody => _isMatching;

        protected override void WatchBody(Context<T> context)
        {
            foreach (var expression in Expressions)
            {
                expression.Watch(context);
                if (!expression.IsMatching)
                    return;
            }

            _isMatching = true;
        }

        protected override void WriteBody(Context<T> context)
        {
            foreach (var expression in Expressions)
                expression.Write(context);
        }

        protected override void Move(Context<T> context) { }
    }
}
