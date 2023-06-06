using System.Text.RegularExpressions;

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

    internal class DesignationListConvertingExpression : ConvertingExpression<List<Designations>, Designations>
    {
        protected override IExpression<Designations> Expression { get; } = new DesignationBodyExpression();

        protected override void WriteToChildContext(Context<List<Designations>> context)
        {
            _childContext = new Context<Designations>()
            {
                Index = context.Index,
                Text = context.Text,
                Result = new Designations()
            };
        }

        protected override void WriteToMainContext(Context<List<Designations>> context)
        {
            context.Result.Add(_childContext.Result);
        }
    }

    internal class DesignationListExpression : HorizontalExpression<List<Designations>>
    {
        protected override IExpression<List<Designations>>[] GetNewExpressions() => 
            new IExpression<List<Designations>>[]
            { 
                new DesignationListConvertingExpression() 
            };
    }

    public class HeaderDesignationLookforwardExpression : VerticalExpression<Module>
    {
        IExpression<Module>[] _expressions;

        protected override IExpression<Module>[] Expressions => _expressions;

        public HeaderDesignationLookforwardExpression()
        {
            _expressions = new IExpression<Module>[] {
                new DesignationModuleExpression(),
                new SeriesExpression()
            };
        }
    }
}
