using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreApi.Filter.Authorization
{
    /// <summary>
    /// 在Action方法上添加此特性，以声明该Action需要用户拥有指定权限才能访问
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiAuthorizeAttribute : Attribute/*, IApiAuthorizeAttribute*/, IFilterMetadata
    {
        /// <summary>
        /// 需要鉴定的权限名
        /// </summary>
        public string[] Permissions { get; }

        /// <summary>
        /// 如果此属性设置为true, 用户必须拥有<see cref="Permissions"/>中的全部权限
        /// 如果为false, 用户至少拥有<see cref="Permissions"/>中的任一权限
        /// 默认值：false.
        /// </summary>
        public bool RequireAllPermissions { get; set; }

        /// <summary>
        /// 创建一个 <see cref="ApiAuthorizeAttribute"/> 实例
        /// </summary>
        /// <param name="permissions">需要鉴定的权限名</param>
        public ApiAuthorizeAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }
    }

}
