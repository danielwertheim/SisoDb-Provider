using System;

namespace SisoDb.Dynamic
{
    public static class DynamicSessionExtensions
    {
         public static ISisoDynamicQueryable Query(this ISession session, Type structureType)
         {
             return new SisoDynamicQueryable(
                 structureType,
                 session.Db.ProviderFactory.GetQueryBuilder(structureType, session.Db.StructureSchemas),
                 session.QueryEngine,
                 SisoDynamicRuntime.LambdaBuilderResolver.Invoke());
         }
    }
}