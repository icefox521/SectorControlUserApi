using Dapper;
using SectorControlApi.Data.Users;

namespace SectorControlApi.Helpers.Dapper
{
    public static class TypeMapperInitializer
    {
        public static void InitializeTypeMaps()
        {
            SqlMapper.SetTypeMap(typeof(User), new ColumnAttributeTypeMapper<User>());
        }
    }
}
