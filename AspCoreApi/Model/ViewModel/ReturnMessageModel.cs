using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.ViewModel
{
    /// <summary>
    /// 返回數據模型
    /// </summary>
    public class ReturnMessageModel<T>
    {
       
        /// <summary>
        /// 0表示成功,1表示失敗,3表示Token 失效或者無效
        /// </summary>
        public int Status { get; set; } 
        
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        ///  返回數據
        /// </summary>
        public T Data { get; set; } = default(T);

        /// <summary>
        /// 无参数构造
        /// </summary>
        public ReturnMessageModel()
        {
        }
        /// <summary>
        /// 查询存在数据时，直接返回
        /// </summary>
        /// <param name="data"></param>
        public ReturnMessageModel(T data)
        {
            Status = 0;
            Message = "Success";
            Data = data;
        }
        /// <summary>
        /// 自定义返回数据的相关信息
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public ReturnMessageModel( int status, string message, T data)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}
