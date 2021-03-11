using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace PMVOnline.Users
{
    /* This entity shares the same table/collection ("AbpUsers" by default) with the
     * IdentityUser entity of the Identity module.
     *
     * - You can define your custom properties into this class.
     * - You never create or delete this entity, because it is Identity module's job.
     * - You can query users from database with this entity.
     * - You can update values of your custom properties.
     */
    public class AppUser : FullAuditedAggregateRoot<Guid>,IUser
    {
        #region Base properties

        /* These properties are shared with the IdentityUser entity of the Identity module.
         * Do not change these properties through this class. Instead, use Identity module
         * services (like IdentityUserManager) to change them.
         * So, this properties are designed as read only!
         */
        public AppUser(Guid id): base(id)
        { 

        }
         
        public virtual Guid? TenantId { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Name { get; set; }

        public virtual string Surname { get; set; }

        public virtual string Email { get; set; }

        public virtual bool EmailConfirmed { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; } 
        #endregion
    }
}
