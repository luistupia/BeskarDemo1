using Common.Helper;
using Dapper;

namespace Common.Extensions;

public  class DapperExtension
{
   public static void ToMapped<T>()
    {
        var map = new CustomPropertyTypeMap(typeof(T), (type, columnName)
            => type.GetProperties().FirstOrDefault(prop => Utils.GetDescriptionFromAttribute(prop) == columnName.ToUpper()));
        SqlMapper.SetTypeMap(typeof(T), map);

    }
}