using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace PMVOnline.Data
{
    /* This is used if database provider does't define
     * IPMVOnlineDbSchemaMigrator implementation.
     */
    public class NullPMVOnlineDbSchemaMigrator : IPMVOnlineDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}