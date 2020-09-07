using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreApi.Middleware.HttpHeaders
{
    /// <summary>
    /// 添加和删除的标头的列表：
    /// </summary>
    public class SecurityHeadersPolicy
    {
        public IDictionary<string, string> SetHeaders { get; }
        = new Dictionary<string, string>();

        public ISet<string> RemoveHeaders { get; }
            = new HashSet<string>();
    }
}
