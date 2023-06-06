using System.Text.RegularExpressions;

namespace Infor.HammPdfReading.Interpreter
{
    public abstract class RegexExpression<T> : ClientExpression<T>
    {
        protected Match _match;

        protected abstract string Pattern();

        protected override bool IsMatchingBody => _match.Success;

        protected override void WatchBody(Context<T> context)
        {
            _match = Regex.Match(context.Text.Substring(context.Index), Pattern());
        }

        protected override void Move(Context<T> context)
        {
            context.Index += _match.Index + _match.Length;
        }
    }
}
