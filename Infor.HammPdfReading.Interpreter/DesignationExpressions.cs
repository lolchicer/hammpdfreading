using System.Text.RegularExpressions;

namespace Infor.HammPdfReading.Interpreter
{
    public class Designations
    {
        public string DesignationSpace = string.Empty;
        public string DesignationRussian = string.Empty;
    }

    internal class DesignationSpaceExpression : RegexExpression<Designations>
    {
        protected override string Pattern() => "[^А-я]*";
        protected override void WriteBody(Context<Designations> context) =>
            context.Result.DesignationSpace = _match.Value;
    }

    internal class DesignationRussianExpression : RegexExpression<Designations>
    {
        protected override string Pattern() => ".+";
        protected override void WriteBody(Context<Designations> context) =>
            context.Result.DesignationRussian = _match.Value;
    }

    internal class DesignationRepeatingExpression : ClientExpression<Designations>
    {
        Match _match;

        public override bool IsMatching => _match.Groups[0].Success;

        protected override void WatchBody(Context<Designations> context)
        {
            var reversed = context.Text.Substring(context.Index).ToCharArray();
            Array.Reverse(reversed);
            _match = Regex.Match(new string(reversed), "((.|\\n)*)(.|\\n)*\\1");
        }

        protected override void WriteBody(Context<Designations> context)
        {
            var matched = _match.Groups[0].Value.ToCharArray();
            Array.Reverse(matched);
            context.Result.DesignationRussian = new string(matched);
        }

        protected override void Move(Context<Designations> context)
        {
            context.Index += _match.Index + _match.Length;
        }
    }

    internal class DesignationDefaultExpression : VerticalExpression<Designations>
    {
        protected override IExpression<Designations>[] Expressions { get; } = new IExpression<Designations>[] {
            new DesignationSpaceExpression(),
            new DesignationRussianExpression()
        };
    }

    public class DesignationBodyExpression : SwitchExpression<Designations>
    {
        protected override IExpression<Designations>[] Expressions { get; } = new IExpression<Designations>[] {
            new DesignationDefaultExpression(),
            new DesignationRepeatingExpression()
        };
    }

    public class DesignationExpression : HorizontalExpression<Designations>
    {
        IExpression<Designations>[] _expressions;

        protected override IExpression<Designations>[] GetNewExpressions() => _expressions;

        public DesignationExpression(Func<Context<Designations>, bool> isPlain)
        {
            _expressions = new IExpression<Designations>[]
            {
                new DesignationMetaExpression(isPlain)
            };
        }
    }

    internal class DesignationMetaExpression : MetaExpression<Designations>
    {
        protected override IExpression<Designations> Expression { get; } = new DesignationBodyExpression();

        public DesignationMetaExpression(Func<Context<Designations>, bool> isPlain)
            : base(isPlain) { }
    }
}
