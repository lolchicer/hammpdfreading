namespace Infor.HammPdfReading.Interpreter
{
    public abstract class HorizontalExpression<T> : ClientExpression<T>
    {
        protected abstract IExpression<T>[] GetNewExpressions();
        protected List<IExpression<T>> MatchingExpressions { get; } = new List<IExpression<T>>();

        public override bool IsMatching => true;

        protected override void WatchBody(Context<T> context)
        {
            bool isMoving = true;

            while (isMoving)
            {
                isMoving = false;
                foreach (var expression in GetNewExpressions())
                {
                    var startIndex = context.Index;
                    expression.Watch(context);
                    if (expression.IsMatching && startIndex != context.Index)
                    {
                        MatchingExpressions.Add(expression);
                        isMoving = true;
                        break;
                    }
                }
            }
        }

        protected override void WriteBody(Context<T> context)
        {
            foreach (var expression in MatchingExpressions)
                expression.Write(context);
        }

        protected override void Move(Context<T> context) { }
    }
}
