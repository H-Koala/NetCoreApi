using IServices.Authorization;
using Model.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Authorization
{
    public class PermissionCheckerService : IPermissionCheckerService
    {

        private readonly Dictionary<string, string[]> userPermissions = new Dictionary<string, string[]>
        {
            //Id=1的用户具有全部模块的全部功能
            { "1", new[] { PermissionNames.Admin, PermissionNames.Staff, PermissionNames.Supervisor } },
            //Id=2的用户具有Staff模块的编辑和删除功能
            { "2", new[] { PermissionNames.Staff} }
        };

        /// <summary>
        /// 此處進行權限檢查
        /// </summary>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        public Task<bool> IsGrantedAsync(string permissionName,string code)
        {
            if (!userPermissions.Any(p => p.Key == code))
                return Task.FromResult(false);
            var up = userPermissions.Where(p => p.Key == code).First();
            var granted = up.Value.Any(permission => permission.Equals(permissionName, StringComparison.InvariantCultureIgnoreCase));
            return Task.FromResult(granted);
        }
    }
}
