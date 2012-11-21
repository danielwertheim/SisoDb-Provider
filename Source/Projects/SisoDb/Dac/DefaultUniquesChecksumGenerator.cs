using SisoDb.NCore.Cryptography;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.NCore;

namespace SisoDb.Dac
{
    public class DefaultUniquesChecksumGenerator : IDefaultUniquesChecksumGenerator
    {
        protected readonly IHashService HashService;

        public DefaultUniquesChecksumGenerator(IHashService hashService = null)
        {
            HashService = hashService ?? new Crc32HashService();
        }

        public virtual string Generate(IStructureIndex structureIndex)
        {
            if(structureIndex.DataTypeCode == DataTypeCode.Text)
                throw new SisoDbNotSupportedException(ExceptionMessages.DefaultUniquesChecksumGenerator_UnsupportedDataType.Inject(structureIndex.Path));

            return HashService.GenerateHash(SisoEnvironment.StringConverter.AsString(structureIndex.Value));
        }
    }
}