using IServices.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Authorization
{
    /// <summary>
    /// IPermissionChecker接口的缺省实现，所有权限检查都将返回true
    /// </summary>
    //public sealed class NullPermissionCheckerService 
    //{
    //    /// <summary>
    //    /// 单实例
    //    /// </summary>
    //    public static NullPermissionCheckerService Instance { get; } = new NullPermissionCheckerService();

    //    /// <summary>
    //    /// 此方法始终返回true
    //    /// </summary>
    //    public Task<bool> IsGrantedAsync(string permissionName,string code)
    //    {
    //        return Task.FromResult(true);
    //    }

    //}
}
