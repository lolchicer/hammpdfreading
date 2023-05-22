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
        public void Interpret(Context<T> context);
    }

    internal abstract class MovingExpression<T> : IExpression<T>
    {
        protected abstract void SetResult(string text, int index, ref T Result);
        protected abstract void SetIndex(T result, ref int index);

        public void Interpret(Context<T> context)
        {
            var contextResult = context.Result;
            SetResult(context.Text, context.Index, ref contextResult);

            var contextIndex = context.Index;
            SetIndex(context.Result, ref contextIndex);

            context.Index = contextIndex;
            context.Result = contextResult;
        }
    }

    internal abstract class HorizontalExpression<T> : IExpression<T>
    {
        protected abstract IExpression<T>[] Expressions { get; }

        public void Interpret(Context<T> context)
        {
            while (context.Index < context.Text.Length) // это не работает без нормальных членов Expressions
                foreach (var expression in Expressions)
                    expression.Interpret(context);
        }
    }

    internal abstract class VerticalExpression<T> : IExpression<T>
    {
        protected abstract IExpression<T>[] Expressions { get; }

        public void Interpret(Context<T> context)
        {
            foreach (var expression in Expressions)
                expression.Interpret(context);
        }
    }

    internal abstract class ConvertingExpression<T1, T2> : MovingExpression<T1>
    {
        int _childIndex;

        protected abstract IExpression<T2> Expression { get; }

        protected abstract Context<T2> FromMainContext(string text, int index, T1 result);
        protected abstract void ToMainContext(Context<T2> childContext, ref T1 result);

        protected override void SetResult(string text, int index, ref T1 result)
        {
            var childContext = FromMainContext(text, index, result);

            Expression.Interpret(childContext);

            _childIndex = childContext.Index;
            ToMainContext(childContext, ref result);
        }

        protected override void SetIndex(T1 result, ref int index) =>
            index = _childIndex;
    }

    internal abstract class RegexExpression<T> : MovingExpression<T>
    {
        protected Match _match;

        protected abstract string Pattern();
        protected abstract void Write(ref T result);

        protected override void SetResult(string text, int index, ref T result)
        {
            _match = Regex.Match(text.Substring(index), Pattern());
            if (_match.Success)
                Write(ref result);
        }

        protected override void SetIndex(T result, ref int index)
        {
            index += _match.Index + _match.Length;
        }
    }

    internal abstract class DetailContextExpression : RegexExpression<DetailContext>
    {
        protected abstract void WriteToDetail(ref Detail detail);

        protected override void Write(ref DetailContext result)
        {
            var detail = result.Detail;
            WriteToDetail(ref detail);
            result.Detail = detail;
            if (!_match.Success)
                result.IsSkipped = true;
        }
    }

    internal class ItemExpression : DetailContextExpression
    {
        protected override string Pattern() => "[0-9]+(\\.[0-9]{2})?";
        protected override void WriteToDetail(ref Detail detail) =>
            detail.Item = Convert.ToDouble(_match.Value.Replace('.', ','));
    }

    internal class PartNoExpression : DetailContextExpression
    {
        protected override string Pattern() => "[0-9]+";
        protected override void WriteToDetail(ref Detail result) =>
            result.PartNo = Convert.ToInt32(_match.Value);
    }

    internal class ValidForExpression : DetailContextExpression
    {
        protected override string Pattern() => "[0-9]{1,4}-[0-9]{1,4}";
        protected override void WriteToDetail(ref Detail result) =>
            result.ValidFor = IDetail.ToValidFor(_match.Value);
    }

    internal class QuantityExpression : DetailContextExpression
    {
        protected override string Pattern() => "[0-9]+";
        protected override void WriteToDetail(ref Detail result) =>
            result.Quantity = Convert.ToDouble(_match.Value);
    }

    internal class UnitExpression : DetailContextExpression
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

    internal class DesignationSpaceExpression : RegexExpression<Designations>
    {
        protected override string Pattern() => "[^А-я]*";
        protected override void Write(ref Designations result) =>
            result.DesignationSpace = _match.Value;
    }

    internal class DesignationRussianExpression : RegexExpression<Designations>
    {
        protected override string Pattern() => ".+";
        protected override void Write(ref Designations result) =>
            result.DesignationRussian = _match.Value;
    }

    internal class DesignationRepeatingExrpession : IExpression<string>
    {
        public void Interpret(Context<string> context)
        {
            throw new NotImplementedException();
        }
    }

    public class DetailContext
    {
        public bool IsSkipped { get; set; } = false;
        public Detail Detail { get; set; }
        public List<string> Designations { get; } = new List<string>();

        public int DesignationIndex { get; set; }
    }

    internal abstract class DesignationExpression<T> : IExpression<T>
    {
        protected abstract void Write(Context<T> context, string value);
        protected abstract Context<Designations> FromMainContext(Context<T> mainContext);

        public void Interpret(Context<T> context)
        {
            var designationContext = FromMainContext(context);

            IExpression<Designations>[] expressions = {
                new DesignationSpaceExpression(),
                new DesignationRussianExpression()
            };

            for (int i = 0; i < expressions.Length; i++)
                expressions[i].Interpret(designationContext);

            var text = designationContext.Text;

            string result;

            if (designationContext.Result.DesignationRussian == string.Empty)
            {
                // гигантское количество функций, разворачивающих строку
                var reversed = text.ToCharArray();
                Array.Reverse(reversed);
                var reversedMatched = Regex.Match(new string(reversed), "((.|\\n)*)(.|\\n)*\\1");
                var matched = reversedMatched.Groups[0].Value.ToCharArray();
                Array.Reverse(matched);

                result = new string(matched) ?? string.Empty;
            }
            else
            {
                result = designationContext.Result.DesignationRussian;
            }

            Write(context, result);
        }
    }

    internal class DesignationExpressionDetailContext : DesignationExpression<DetailContext>
    {
        protected override void Write(Context<DetailContext> context, string value)
        {
            context.Result.Designations.Clear();

            // гигансткое количество функций, записывающих результат в структуру
            var detailClone = context.Result.Detail;
            detailClone.Designation = value;
            context.Result.Detail = detailClone;
        }

        protected override Context<Designations> FromMainContext(Context<DetailContext> mainContext) =>
            new Context<Designations> { Text = string.Concat(mainContext.Result.Designations), Result = new Designations() };
    }

    internal class DesignationExpressionDetail : DesignationExpression<Detail>
    {
        protected override void Write(Context<Detail> context, string value)
        {
            var detailClone = context.Result;
            detailClone.Designation = value;
            context.Result = detailClone;
        }

        protected override Context<Designations> FromMainContext(Context<Detail> mainContext) =>
            new Context<Designations> { Text = mainContext.Result.Designation, Result = new Designations() };
    }

    internal class MainExpression : VerticalExpression<DetailContext>
    {
        protected override IExpression<DetailContext>[] Expressions => new IExpression<DetailContext>[] {
                    new ItemExpression(),
                    new PartNoExpression(),
                    new ValidForExpression(),
                    new QuantityExpression(),
                    new UnitExpression()
                };

        // должен быть механизм по предотвращению записи пустых строчек (соответственно контекст тоже нужно поменять)
    }

    internal class DesignationLineExpression : RegexExpression<DetailContext>
    {
        protected override string Pattern() => ".*";
        protected override void Write(ref DetailContext result) =>
            result.Designations.Add(_match.Value);
    }

    internal class NewLineExpression : RegexExpression<List<Detail>>
    {
        protected override string Pattern() => "\n";
        protected override void Write(ref List<Detail> result) =>
            result.Add(new Detail { Designation = string.Empty });
    }

    internal class DetailExpression : VerticalExpression<DetailContext>
    {
        protected override IExpression<DetailContext>[] Expressions => new IExpression<DetailContext>[] {
                new MainExpression(),
                new DesignationLineExpression()
            };
    }

    public class DetailTableExprssion : HorizontalExpression<List<Detail>>
    {
        DesignationExpressionDetailContext _designationExpression = new DesignationExpressionDetailContext();
        DetailExpression _detailExpression = new DetailExpression();
        protected override IExpression<List<Detail>>[] Expressions { get => new IExpression<List<Detail>>[] {
                new MainExpression(),
                new DesignationLineExpression()
            };

        public void Interpret(Context<List<Detail>> context)
        {
            var detailContext = new Context<DetailContext>() { Index = context.Index, Text = context.Text, Result = new DetailContext() };
            while (context.Index < context.Text.Length)
            {
                _detailExpression.Interpret(detailContext);
                if (!detailContext.Result.IsSkipped)
                {
                    _designationExpression.Interpret(detailContext);

                    context.Result.Add(detailContext.Result.Detail);
                }
                context.Index = detailContext.Index;
            }
        }
    }
}
