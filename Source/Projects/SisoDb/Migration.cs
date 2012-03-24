using System;
using EnsureThat;
using SisoDb.Maintenance;

namespace SisoDb
{
    public abstract class Migration : IMigrationInfo
    {
        public Type From { get; private set; }
        public Type To { get; private set; }

        protected Migration(Type from, Type to)
        {
            Ensure.That(from, "from").IsNotNull();
            Ensure.That(to, "to").IsNotNull();

            From = from;
            To = to;
        }
    }

    public class Migration<TFrom, TTo> : Migration
        where TFrom : class
        where TTo : class
    {
        public Func<TFrom, TTo, MigrationStatuses> Modifier { get; private set; }

        public Migration(Func<TFrom, TTo, MigrationStatuses> modifier) 
            : base(typeof(TFrom), typeof(TTo))
        {
            Ensure.That(modifier, "modifier").IsNotNull();
            Modifier = modifier;
        }
    }

    public class Migration<TFrom, TFromTemplate, TTo> : Migration
        where TFrom : class
        where TFromTemplate : class
        where TTo : class
    {
        public Type FromTemplate { get; private set; }

        public Func<TFromTemplate, TTo, MigrationStatuses> Modifier { get; private set; }

        public Migration(Func<TFromTemplate, TTo, MigrationStatuses> modifier)
            : base(typeof(TFrom), typeof(TTo))
        {
            Ensure.That(modifier, "modifier").IsNotNull();
            Modifier = modifier;
            FromTemplate = typeof (TFromTemplate);
        }
    }
}