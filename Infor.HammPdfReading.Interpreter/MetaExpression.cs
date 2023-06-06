namespace Infor.HammPdfReading.Interpreter
{
    internal abstract class MetaExpression<T> : ClientExpression<T>
    {
        bool _isMatching = false;

        Func<Context<T>, bool> _watch;

        protected abstract IExpression<T> Expression { get; }

        protected override bool IsMatchingBody => _isMatching;

        protected override void WatchBody(Context<T> context)
        {
            if (_watch(context))
            {
                Expression.Watch(context);
                _isMatching = Expression.IsMatching;
            }
        }
        protected override void WriteBody(Context<T> context) => Expression.Write(context);
        protected override void Move(Context<T> context) { }

        public MetaExpression(Func<Context<T>, bool> watch)
        {
            _watch = watch;
        }
    }
}
