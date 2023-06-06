using System;
using System.Collections.Generic;
using System.Linq;
namespace Infor.HammPdfReading.Interpreter
{
    // использование этого класса должно быть заменено на использование HorizontalExpression<T> и стратегию для кастинга результатов
    public abstract class SwitchExpression<T> : ClientExpression<T>
    {
        IExpression<T> _selectedExpression;

        protected abstract IExpression<T>[] Expressions { get; }

        protected override bool IsMatchingBody
        { 
            get
            {
                if (_selectedExpression == null)
                    return false;
                else
                    return _selectedExpression.IsMatching;
            } 
        }

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
        protected override void Move(Context<T> context) { }
    }
}
