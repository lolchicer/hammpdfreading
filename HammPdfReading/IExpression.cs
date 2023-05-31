using Infor.HammPdfReading;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Infor.HammPdfReading.HammPdfReader;
using static iTextSharp.text.pdf.AcroFields;
using static System.Net.Mime.MediaTypeNames;

namespace Infor.HammPdfReading
{
    public class Context<T>
    {
        public T Result { get; set; }
        public string Text { get; set; } //hfgdhdfgh
        public int Index { get; set; }
    }

    public interface IExpression<T>
    {
        public bool IsMatching { get; }

        public void Watch(Context<T> context);
        public void Write(Context<T> context);
        public void Move(Context<T> context);
    }

    // использование этого класса должно быть заменено на использование HorizontalExpression<T> и стратегию для кастинга результатов
    public abstract class SwitchExpression<T> : ClientExpression<T>
    {
        IExpression<T> _selectedExpression;

        protected abstract IExpression<T>[] Expressions { get; }

        public override bool IsMatching => _selectedExpression.IsMatching;

        protected override void WatchBody(Context<T> context)
        {
            foreach (var expression in Expressions)
            {
                expression.Watch(context);
                if (expression.IsMatching)
                {
                    _selectedExpression = expression;
                    break;
                }
            }
        }

        protected override void WriteBody(Context<T> context) => _selectedExpression.Write(context);
        public override void Move(Context<T> context) { }
    }

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

        public override void Move(Context<T> context) { }
    }

    public abstract class VerticalExpression<T> : ClientExpression<T>
    {
        bool _isMatching = false;

        protected abstract IExpression<T>[] Expressions { get; }

        public override bool IsMatching => _isMatching;

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

        public override void Move(Context<T> context) { }
    }

    public abstract class ConvertingExpression<T1, T2> : ClientExpression<T1>
    {
        bool _isMatching = false;

        protected Context<T2> _childContext;

        protected abstract IExpression<T2> Expression { get; }

        protected abstract void WriteToChildContext(Context<T1> context);
        protected abstract void WriteToMainContext(Context<T1> context);

        public override bool IsMatching => _isMatching;

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

        public override void Move(Context<T1> context)
        {
            context.Index = _childContext.Index;
        }
    }

    internal abstract class MetaExpression<T> : ClientExpression<T>
    {
        bool _isMatching = false;

        Func<Context<T>, bool> _watch;

        protected abstract IExpression<T> Expression { get; }

        public override bool IsMatching => _isMatching;

        protected override void WatchBody(Context<T> context)
        {
            if (_watch(context))
            {
                Expression.Watch(context);
                _isMatching = Expression.IsMatching;
            }
        }
        protected override void WriteBody(Context<T> context) => Expression.Write(context);
        public override void Move(Context<T> context) { }

        public MetaExpression(Func<Context<T>, bool> watch)
        {
            _watch = watch;
        }
    }

    internal abstract class RegexExpression<T> : ClientExpression<T>
    {
        protected Match _match;

        protected abstract string Pattern();

        public override bool IsMatching => _match.Success;

        protected override void WatchBody(Context<T> context)
        {
            _match = Regex.Match(context.Text.Substring(context.Index), Pattern());
        }

        public override void Move(Context<T> context)
        {
            context.Index += _match.Index + _match.Length;
        }
    }

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

    public class Designations
    {
        public string DesignationSpace = string.Empty;
        public string DesignationRussian = string.Empty;
    }

    public class DetailContext
    {
        public Detail Detail { get; set; }
        public List<string> Designations { get; } = new List<string>();

        public int DesignationIndex { get; set; }
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

        public override void Move(Context<Designations> context)
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
