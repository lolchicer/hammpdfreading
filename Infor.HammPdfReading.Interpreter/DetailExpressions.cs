namespace Infor.HammPdfReading.Interpreter
{
    internal abstract class DetailRegexExpression : RegexExpression<Detail>
    {
        protected abstract void WriteToDetail(ref Detail detail);

        protected override void WriteBody(Context<Detail> context)
        {
            var detail = context.Result;
            WriteToDetail(ref detail);
            context.Result = detail;
        }
    }

    internal class ItemExpression : DetailRegexExpression
    {
        protected override string Pattern() => "[0-9]+(\\.[0-9]{2})?";
        protected override void WriteToDetail(ref Detail detail) =>
            detail.Item = Convert.ToDouble(_match.Value.Replace('.', ','));
    }

    internal class PartNoExpression : DetailRegexExpression
    {
        protected override string Pattern() => "[0-9]+";
        protected override void WriteToDetail(ref Detail result) =>
            result.PartNo = Convert.ToInt64(_match.Value);
    }

    internal class ValidForExpression : DetailRegexExpression
    {
        protected override string Pattern() => "[0-9]{1,4}-[0-9]{1,4}";
        protected override void WriteToDetail(ref Detail result) =>
            result.ValidFor = IDetail.ToValidFor(_match.Value);
    }

    internal class QuantityExpression : DetailRegexExpression
    {
        protected override string Pattern() => "[0-9]+";
        protected override void WriteToDetail(ref Detail result) =>
            result.Quantity = Convert.ToDouble(_match.Value);
    }

    internal class UnitExpression : DetailRegexExpression
    {
        protected override string Pattern() => "[A-Z]{1,2}";
        protected override void WriteToDetail(ref Detail result) =>
            result.Unit = IDetail.ToUnitType(_match.Value);
    }

    public class DetailContext
    {
        public Detail Detail { get; set; }
        public List<string> Designations { get; } = new List<string>();

        public int DesignationIndex { get; set; }
    }

    internal class DesignationDetailExpression : ConvertingExpression<Detail, Designations>
    {
        IExpression<Designations> _expression;

        protected override IExpression<Designations> Expression => _expression;

        protected override void WriteToChildContext(Context<Detail> context)
        {
            _childContext = new Context<Designations> { Text = context.Text, Index = context.Index, Result = new Designations() };
        }

        protected override void WriteToMainContext(Context<Detail> context)
        {
            var detailCopy = context.Result;
            detailCopy.Designation = _childContext.Result.DesignationRussian;
            context.Result = detailCopy;
        }

        public DesignationDetailExpression(Func<Context<Designations>, bool> isPlain)
        {
            _expression = new DesignationMetaExpression(isPlain);
        }
    }

    public class MainExpression : VerticalExpression<Detail>
    {
        protected override IExpression<Detail>[] Expressions { get; } = new IExpression<Detail>[] {
            new ItemExpression(),
            new PartNoExpression(),
            new ValidForExpression(),
            new QuantityExpression(),
            new UnitExpression()
        };
    }

    internal class DetailExpression : VerticalExpression<Detail>
    {
        MainExpression _mainExpression;
        DesignationDetailExpression _designationDetailExpression;

        IExpression<Detail>[] _expressions;

        protected override IExpression<Detail>[] Expressions => _expressions;

        public DetailExpression()
        {
            _mainExpression = new MainExpression();
            _designationDetailExpression = new DesignationDetailExpression((Context<Designations> context) =>
            {
                var textCut = context.Text.Substring(
                    0,
                    context.Index +
                    context.Text.Substring(
                        context.Index).IndexOf('\n'));
                var conditionExpression = new MainExpression();
                conditionExpression.Watch(new Context<Detail>() { Index = context.Index, Text = textCut });
                return !conditionExpression.IsMatching;
            });

            _expressions = new IExpression<Detail>[] {
                _mainExpression,
                _designationDetailExpression
            };
        }
    }

    public class DetailTableRowExpression : ConvertingExpression<List<Detail>, Detail>
    {
        protected override IExpression<Detail> Expression { get; } = new DetailExpression();

        protected override void WriteToChildContext(Context<List<Detail>> context)
        {
            _childContext = new Context<Detail>() { Text = context.Text, Index = context.Index };
        }

        protected override void WriteToMainContext(Context<List<Detail>> context)
        {
            context.Result.Add(_childContext.Result);
        }
    }

    public class DetailTableExprssion : HorizontalExpression<List<Detail>>
    {
        protected override IExpression<List<Detail>>[] GetNewExpressions() => new IExpression<List<Detail>>[] {
            new DetailTableRowExpression(),
        };
    }
}
