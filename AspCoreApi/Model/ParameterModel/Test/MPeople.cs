using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.ParameterModel.Test
{
    /// <summary>
    /// 测试类
    /// </summary>
    public class MPeople
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "名字是必填的")]
        public string name { get; set; }
        /// <summary>
        /// 年纪
        /// </summary>
        [RegularExpression(@"[1-9]\d{1,2}", ErrorMessage = "年纪只能在1-99范围")]
        public int age { get; set; }
    }
}
