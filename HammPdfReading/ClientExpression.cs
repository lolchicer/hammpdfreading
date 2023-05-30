﻿namespace Infor.HammPdfReading
{
    public abstract class ClientExpression<T> : IExpression<T>, IInterpretingExpression<T>
    {
        protected abstract void WatchBody(Context<T> context);
        protected abstract void WriteBody(Context<T> context);

        public abstract bool IsMatching { get; }

        public abstract void Move(Context<T> context);

        public void Watch(Context<T> context)
        {
            WatchBody(context);
            if (IsMatching)
                Move(context);
        }

        public void Write(Context<T> context)
        {
            WriteBody(context);
            Move(context);
        }

        public void Interpet(Context<T> context)
        {
            Watch(context);
            context.Index = 0;
            if (IsMatching)
                Write(context);
        }
    }
}