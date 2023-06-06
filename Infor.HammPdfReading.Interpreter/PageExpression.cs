using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Infor.HammPdfReading.Interpreter
{
    public class Page
    {
        public List<Detail> Details { get; set; }
        public Module Module { get; set; }
        public ExtendedDetail[] GetExtendedDetails()
        {
            throw new NotImplementedException();
        }
    }

    public class HeaderExpression : RegexExpression<Page>
    {
        protected override string Pattern() =>
            "Position Teile-Nr\\. Gültig für Menge Einheit Benennung Designation Denominación Наименование\r\n" +
            "Item Part No\\. Valid for Quantity Unit\r\nPosición Referencia Válido para Cantidad Unidad\r\n" +
            "Позиц\r\nия\r\n№ детали Действует\r\nдля\r\nКоличе\r\nство\nЕд\\.\n";

        protected override void WriteBody(Context<Page> context) { }
    }

    public class AssemblyExpression : RegexExpression<Module>
    {
        protected override string Pattern() => "[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}";
        protected override void WriteBody(Context<Module> context)
        {
            var resultCopy = context.Result;
            resultCopy.Assembly = _match.Value;
            context.Result = resultCopy;
        }
    }

    public class NoExpression : RegexExpression<Module>
    {
        protected override string Pattern() => "(?<=\\n).+";
        protected override void WriteBody(Context<Module> context)
        {
            var resultCopy = context.Result;
            resultCopy.No = Convert.ToInt32(_match.Value);
            context.Result = resultCopy;
        }
    }

    public class DateExpression : RegexExpression<Module>
    {
        protected override string Pattern() => "[0-9]{2}\\.[0-9]{2}\\.[0-9]{4}";
        protected override void WriteBody(Context<Module> context) { }
    }

    public class ModuleHeaderExpression : RegexExpression<Module>
    {
        protected override string Pattern() =>
            "Benennung:\\r\\nDesignation:\\r\\nDenominación:\\r\\nНаименование:\\r\\n" +
            "Gruppe:\\r\\nAssembly:\\r\\nGrupo:\\r\\nГруппа:\\r\\n" +
            "Baureihe:\\r\\nSeries:\\r\\nCódigo der serie:\\r\\nКонструктивный\\r\\nряд:";
        protected override void WriteBody(Context<Module> context) { }
    }

    public class DesignationModuleExpression : ClientExpression<Module>
    {
        Context<Designations> _childContext;
        bool _isMatching = false;
        Match _match;

        protected IExpression<Designations> Expression { get; } = new DesignationBodyExpression();

        protected void WriteToChildContext(Context<Module> context)
        {
            _childContext = new Context<Designations>
            {
                Index = context.Index,
                Text = context.Text.Substring(context.Index, context.Index + _match.Length),
                Result = new Designations()
            };
        }

        protected void WriteToMainContext(Context<Module> context)
        {
            var resultCopy = context.Result;
            resultCopy.Designation = _childContext.Result.DesignationRussian;
            context.Result = resultCopy;
        }

        protected override bool IsMatchingBody => _isMatching;

        protected override void WatchBody(Context<Module> context)
        {
            var textCut = context.Text.Substring(context.Index);

            _match = Regex.Match(
                textCut,
                "(.|\\n)*(?=" +
                "Benennung:\\r\\nDesignation:\\r\\nDenominación:\\r\\nНаименование:\\r\\n" +
                "Gruppe:\\r\\nAssembly:\\r\\nGrupo:\\r\\nГруппа:\\r\\n" +
                "Baureihe:\\r\\nSeries:\\r\\nCódigo der serie:\\r\\nКонструктивный\\r\\nряд:" +
                ")");

            if (_match.Success)
            {
                WriteToChildContext(context);
                Expression.Watch(_childContext);
                _isMatching = Expression.IsMatching;
            }
        }

        protected override void WriteBody(Context<Module> context)
        {
            WriteToChildContext(context);
            Expression.Write(_childContext);
            WriteToMainContext(context);
        }

        protected override void Move(Context<Module> context)
        {
            context.Index = _childContext.Index;
        }
    }

    public class SeriesExpression : RegexExpression<Module>
    {
        protected override string Pattern() => "(?<=Конструктивный\\r\\nряд:\\r\\n)(.|\\n)*";
        protected override void WriteBody(Context<Module> context)
        {
            var resultCopy = context.Result;
            resultCopy.Series = _match.Value;
            context.Result = resultCopy;
        }
    }

    public class ModuleExpression : VerticalExpression<Module>
    {
        protected override IExpression<Module>[] Expressions { get; } =
        {
            new AssemblyExpression(),
            new NoExpression(),
            new DateExpression(),
            new DesignationModuleExpression(),
            new SeriesExpression()
        };
    }

    public class DetailTablePageExpression : ConvertingExpression<Page, List<Detail>>
    {
        protected override IExpression<List<Detail>> Expression { get; } = new DetailTableExprssion();

        protected override void WriteToChildContext(Context<Page> context)
        {
            _childContext = new Context<List<Detail>>()
            {
                Index = context.Index,
                Text = context.Text,
                Result = new List<Detail>()
            };
        }

        protected override void WriteToMainContext(Context<Page> context)
        {
            context.Result.Details = _childContext.Result;
        }
    }

    public class ModulePageExpresion : ConvertingExpression<Page, Module>
    {
        protected override IExpression<Module> Expression { get; } = new ModuleExpression();

        protected override void WriteToChildContext(Context<Page> context)
        {
            _childContext = new Context<Module>
            {
                Index = context.Index,
                Text = context.Text
            };
        }

        protected override void WriteToMainContext(Context<Page> context)
        {
            context.Result.Module = _childContext.Result;
        }
    }

    public class PageExpression : VerticalExpression<Page>
    {
        protected override IExpression<Page>[] Expressions { get; } =
        {
            new HeaderExpression(),
            new DetailTablePageExpression(),
            new ModulePageExpresion()
        };
    }
}
