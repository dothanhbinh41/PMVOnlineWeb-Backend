using PMVOnline.Departments;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Identity;

namespace PMVOnline.Users
{
    public class UserDepartmentCreateDto : IdentityUserCreateDto
    {
        public CreateDepartmentNameUserDto[] Departments { get; set; }
    }
}
