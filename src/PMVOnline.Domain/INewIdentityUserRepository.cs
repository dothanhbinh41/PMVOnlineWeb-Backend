using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace PMVOnline
{

    public interface INewIdentityUserRepository: IIdentityUserRepository
    {
        Task<List<IdentityUser>> GetUserInRolesAsync(
            string roleName,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);
    }
}
