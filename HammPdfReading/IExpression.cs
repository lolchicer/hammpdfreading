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

    // с RegexExpression создание новых объектов в Expressions ломает _match
    public abstract class HorizontalExpression<T> : IExpression<T>
    {
        protected abstract IExpression<T>[] Expressions { get; }
        protected List<IExpression<T>> MatchingExpressions { get; } = new List<IExpression<T>>();

        public bool IsMatching => true;

        public void Watch(Context<T> context)
        {
            var startIndex = context.Index;

            bool isMoving = true;

            while (isMoving)
            {
                foreach (var expression in Expressions)
                {
                    expression.Watch(context);
                    if (expression.IsMatching)
                    {
                        MatchingExpressions.Add(expression);
                        expression.Move(context);
                        break;
                    }
                    isMoving = false;
                }
            }

            context.Index = startIndex;
        }

        public void Write(Context<T> context)
        {
            foreach (var expression in MatchingExpressions)
                expression.Write(context);
        }

        public void Move(Context<T> context)
        {
            foreach (var expression in MatchingExpressions)
                expression.Move(context);
        }
    }

    public abstract class VerticalExpression<T> : IExpression<T>
    {
        bool _isMatching = false;

        protected abstract IExpression<T>[] Expressions { get; }

        public bool IsMatching => _isMatching;

        public void Watch(Context<T> context)
        {
            var startIndex = context.Index;

            foreach (var expression in Expressions)
            {
                _isMatching = true;
                expression.Watch(context);
                expression.Move(context);
                if (!expression.IsMatching)
                {
                    _isMatching = false;
                    break;
                }
            }

            context.Index = startIndex;
        }

        public void Write(Context<T> context)
        {
            foreach (var expression in Expressions)
                expression.Write(context);
        }

        public void Move(Context<T> context)
        {
            foreach (var expression in Expressions)
                expression.Move(context);
        }
    }

    public abstract class ConvertingExpression<T1, T2> : IExpression<T1>
    {
        bool _isMatching = false;

        protected Context<T2> _childContext;

        protected abstract IExpression<T2> Expression { get; }

        protected abstract void WriteToChildContext(Context<T1> context);
        protected abstract void WriteToMainContext(Context<T1> context);

        public bool IsMatching => _isMatching;

        public void Watch(Context<T1> context)
        {
            WriteToChildContext(context);
            Expression.Watch(_childContext);
            _isMatching = Expression.IsMatching;
        }

        public void Write(Context<T1> context)
        {
            WriteToChildContext(context);
            Expression.Write(_childContext);
            WriteToMainContext(context);
        }

        public void Move(Context<T1> context)
        {
            Expression.Move(_childContext);
            context.Index = _childContext.Index;
        }
    }

    internal abstract class MetaExpression<T> : IExpression<T>
    {
        bool _isMatching;

        Func<Context<T>, bool> _isPlain;

        protected abstract IExpression<T> Expression { get; }

        public bool IsMatching => _isMatching;

        public void Watch(Context<T> context) { _isMatching = _isPlain(context); }
        public void Write(Context<T> context) => Expression.Write(context);
        public void Move(Context<T> context) => Expression.Move(context);

        public MetaExpression(Func<Context<T>, bool> isPlain)
        {
            _isPlain = isPlain;
        }
    }

    internal abstract class RegexExpression<T> : IExpression<T>
    {
        protected Match _match;

        protected abstract string Pattern();

        public bool IsMatching => _match.Success;

        public void Watch(Context<T> context)
        {
            _match = Regex.Match(context.Text.Substring(context.Index), Pattern());
        }

        public abstract void Write(Context<T> context);

        public void Move(Context<T> context)
        {
            context.Index += _match.Index + _match.Length;
        }
    }

    internal abstract class DetailRegexExpression : RegexExpression<Detail>
    {
        protected abstract void WriteToDetail(ref Detail detail);

        public override void Write(Context<Detail> context)
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

    class Designations
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
        public override void Write(Context<Designations> context) =>
            context.Result.DesignationSpace = _match.Value;
    }

    internal class DesignationRussianExpression : RegexExpression<Designations>
    {
        protected override string Pattern() => ".+";
        public override void Write(Context<Designations> context) =>
            context.Result.DesignationRussian = _match.Value;
    }

    internal class DesignationRepeatingExpression : IExpression<Designations>
    {
        Match _match;

        public bool IsMatching => _match.Groups[0].Success;

        public void Watch(Context<Designations> context)
        {
            var reversed = context.Text.Substring(context.Index).ToCharArray();
            Array.Reverse(reversed);
            _match = Regex.Match(new string(reversed), "((.|\\n)*)(.|\\n)*\\1");
        }

        public void Write(Context<Designations> context)
        {
            var matched = _match.Groups[0].Value.ToCharArray();
            Array.Reverse(matched);
            context.Result.DesignationRussian = new string(matched);
        }

        public void Move(Context<Designations> context)
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

    internal class DesignationBodyExpression : HorizontalExpression<Designations>
    {
        protected override IExpression<Designations>[] Expressions { get; } = new IExpression<Designations>[] {
            new DesignationDefaultExpression(),
            new DesignationRepeatingExpression()
        };
    }

    internal class DesignationExpression : HorizontalExpression<Designations>
    {
        IExpression<Designations>[] _expressions;

        protected override IExpression<Designations>[] Expressions => _expressions;

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
            _expression = new DesignationExpression(isPlain);
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
                _mainExpression.Watch(new Context<Detail>() { Index = context.Index, Text = context.Text });
                return !_mainExpression.IsMatching;
            });

            _expressions = new IExpression<Detail>[] {
                _mainExpression // ,
                // _designationDetailExpression
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
        protected override IExpression<List<Detail>>[] Expressions { get; } = new IExpression<List<Detail>>[] {
            new DetailTableRowExpression(),
        };
    }
}
