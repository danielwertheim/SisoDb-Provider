using System;
using SisoDb.EnsureThat;
using SisoDb.Maintenance;

namespace SisoDb
{
    public static class Migrate<TFrom> where TFrom : class 
    {
        public static Migrate<TFrom, TTo> To<TTo>() where TTo : class
        {
            return new Migrate<TFrom, TTo>();
        }
    }

    public class Migrate<TFrom, TTo>
        where TFrom : class
        where TTo : class
    {
        public Migration<TFrom, TTo> Using(Func<TFrom, TTo, MigrationStatuses> modifier)
        {
            Ensure.That(modifier, "modifier").IsNotNull();
            return new Migration<TFrom, TTo>(modifier);
        }

        public Migration<TFrom, TFromTemplate, TTo> Using<TFromTemplate>(Func<TFromTemplate, TTo, MigrationStatuses> modifier) where TFromTemplate : class
        {
            Ensure.That(modifier, "modifier").IsNotNull();

            return new Migration<TFrom, TFromTemplate, TTo>(modifier);
        }

        public Migration<TFrom, TFromTemplate, TTo> Using<TFromTemplate>(TFromTemplate template, Func<TFromTemplate, TTo, MigrationStatuses> modifier) where TFromTemplate : class
        {
            Ensure.That(modifier, "modifier").IsNotNull();

            return new Migration<TFrom, TFromTemplate, TTo>(modifier);
        }
    }
}