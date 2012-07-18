namespace PineCone.Structures.Schemas.Builders
{
    public interface ISchemaBuilder
    {
        IDataTypeConverter DataTypeConverter { get; set; }

        IStructureSchema CreateSchema(IStructureType structureType);
    }
}