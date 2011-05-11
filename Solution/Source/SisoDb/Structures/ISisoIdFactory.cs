using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures
{
    /// <summary>
    /// Creates <see cref="ISisoId"/> instances by
    /// extracting the SisoId value of sent items.
    /// </summary>
    public interface ISisoIdFactory
    {
        /// <summary>
        /// Extracts Id-value from sent <typeparamref name="T"/> item,
        /// and returns an <see cref="ISisoId"/>.
        /// If the SisoId member of <typeparamref name="T"/> item,
        /// does not contain a value, an exception is thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idAccessor"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        ISisoId GetId<T>(IIdAccessor idAccessor, T item) where T : class;
    }
}