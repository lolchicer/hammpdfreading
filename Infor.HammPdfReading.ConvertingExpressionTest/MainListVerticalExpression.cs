using System.Collections.Generic;

namespace Infor.HammPdfReading.UnitTest
{
    internal class TestConvertingExpression : ConvertingExpression<Detail, Detail>
    {
        protected override IExpression<Detail> Expression { get; } = new MainExpression();

        protected override void WriteToChildContext(Context<Detail> context)
        {
            _childContext = new Context<Detail>() { Index = context.Index, Text = context.Text };
        }

        protected override void WriteToMainContext(Context<Detail> context)
        {
            context.Result = _childContext.Result;
        }
    }

    internal class MainListConvertingExpression : ConvertingExpression<List<Detail>, Detail>
    {
        protected override IExpression<Detail> Expression { get; } = new MainExpression();

        protected override void WriteToChildContext(Context<List<Detail>> context)
        {
            _childContext = new Context<Detail>() { Index = context.Index, Text = context.Text };
        }

        protected override void WriteToMainContext(Context<List<Detail>> context)
        {
            context.Result.Add(_childContext.Result);
        }
    }

    internal class MainListVerticalExpression : VerticalExpression<List<Detail>>
    {
        protected override IExpression<List<Detail>>[] Expressions { get; } =
        {
            new MainListConvertingExpression(),
            new MainListConvertingExpression()
        };
    }

    internal class MainListHorizontalExpression : HorizontalExpression<List<Detail>>
    {
        protected override IExpression<List<Detail>>[] GetNewExpressions() => new IExpression<List<Detail>>[]
        {
            new MainListConvertingExpression()
        };
    }
}
