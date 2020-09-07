using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IServices.Authorization
{
    /// <summary>
    /// 权限检查器接口
    /// </summary>
    public interface IPermissionCheckerService
    {
        /// <summary>
        /// 检查当前用户是否拥有某项权限
        /// </summary>
        /// <param name="permissionName">权限名</param>
        /// <param name="code">員工code碼</param>
        Task<bool> IsGrantedAsync(string permissionName,string code);

    }
}
