namespace Infor.HammPdfReading.Interpreter
{
    public abstract class ConvertingExpression<T1, T2> : ClientExpression<T1>
    {
        bool _isMatching = false;

        protected Context<T2> _childContext;

        protected abstract IExpression<T2> Expression { get; }

        protected abstract void WriteToChildContext(Context<T1> context);
        protected abstract void WriteToMainContext(Context<T1> context);

        protected override bool IsMatchingBody => _isMatching;

        protected override void WatchBody(Context<T1> context)
        {
            WriteToChildContext(context);
            Expression.Watch(_childContext);
            _isMatching = Expression.IsMatching;
        }

        protected override void WriteBody(Context<T1> context)
        {
            WriteToChildContext(context);
            Expression.Write(_childContext);
            WriteToMainContext(context);
        }

        protected override void Move(Context<T1> context)
        {
            context.Index = _childContext.Index;
        }
    }
}
