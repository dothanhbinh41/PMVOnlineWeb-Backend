using PMVOnline.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace PMVOnline.Permissions
{
    public class PMVOnlinePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(PMVOnlinePermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(PMVOnlinePermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<PMVOnlineResource>(name);
        }
    }
}
