namespace SisoDb.Structures
{
    public static class DataTypeCodeExtensions
    {
         public static bool IsValueType(this DataTypeCode dataTypeCode)
         {
             return
                 dataTypeCode != DataTypeCode.String && 
                 dataTypeCode != DataTypeCode.Enum &&
                 dataTypeCode != DataTypeCode.Text;
         }
    }
}