using Dapper;
using System.ComponentModel.DataAnnotations.Schema;

namespace SectorControlApi.Helpers.Dapper
{
    public class ColumnAttributeTypeMapper<T> : FallbackTypeMapper
    {
        public ColumnAttributeTypeMapper()
            : base(new SqlMapper.ITypeMap[]
                {
                new CustomPropertyTypeMap(
                   typeof(T),
                   (type, columnName) =>
                       type.GetProperties().FirstOrDefault(prop =>
                           prop.GetCustomAttributes(false)
                               .OfType<ColumnAttribute>()
                               .Any(attr => attr.Name == columnName)
                           )
                   ),
                new DefaultTypeMap(typeof(T))
                })
        {
        }
    }
}
