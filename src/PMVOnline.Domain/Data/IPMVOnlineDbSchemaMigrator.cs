using System.Threading.Tasks;

namespace PMVOnline.Data
{
    public interface IPMVOnlineDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
