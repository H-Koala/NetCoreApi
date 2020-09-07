using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace IServices.ICommon.IRepository.IDapper
{
    /// <summary>
    /// Dapper 数据库操作接口
    /// </summary>
    public interface ISqlDapperService
    {

        #region 同步方法

        /// <summary>
        /// 执行强类型查询 返回List T
        /// </summary>
        /// <typeparam name="T">映射的結果類型</typeparam>
        /// <param name="cmd">要执行的查询</param>
        /// <param name="param">查询参数（默认= null）</param>
        /// <param name="commandType">命令类型(默认= null)</param>
        /// <returns></returns>
        List<T> QueryList<T>(string cmd, object param, CommandType? commandType = null) where T : class;
        /// <summary>
        /// 执行查询并映射第一个结果（強類型）
        /// </summary>
        /// <typeparam name="T">映射的結果類型</typeparam>
        /// <param name="cmd">要执行的查询</param>
        /// <param name="param">查询参数（默认= null）</param>
        /// <param name="commandType">命令类型(默认= null)</param>
        /// <returns></returns>
        T QueryFirst<T>(string cmd, object param , CommandType? commandType = null) where T : class;
        /// <summary>
        /// 执行查询，并返回查询返回的结果集中第一行的第一列。其他列或行将被忽略
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        object ExecuteScalar(string cmd, object param, CommandType? commandType = null);
        /// <summary>
        /// 執行RUD 語句，并返回受影響的行數
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        int ExcuteNonQuery(string cmd, object param, CommandType? commandType = null);
        /// <summary>
        /// Dapper不处理查询结果时，通常使用此方法。例如，填写DataTable或DataSet
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        IDataReader ExecuteReader(string cmd, object param, CommandType? commandType = null);
        /// <summary>
        /// 同一命令内执行多个查询并映射结果
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        SqlMapper.GridReader QueryMultiple(string cmd, object param, CommandType? commandType = null);
        /// <summary>
        /// 同一命令内执行兩个查询并映射结果
        /// </summary>
        /// <typeparam name="T1">返回的第一個Model</typeparam>
        /// <typeparam name="T2">返回的第二個Model</typeparam>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        (List<T1>, List<T2>) QueryMultiple<T1, T2>(string cmd, object param, CommandType? commandType = null);
        /// <summary>
        /// 同一命令内执行三個个查询并映射结果
        /// </summary>
        /// <typeparam name="T1">返回的第一個Model</typeparam>
        /// <typeparam name="T2">返回的第二個Model</typeparam>
        /// <typeparam name="T3">返回的第三個Model</typeparam>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        (List<T1>, List<T2>, List<T3>) QueryMultiple<T1, T2, T3>(string cmd, object param, CommandType? commandType = null);
        /// <summary>
        /// 执行查询 返回動態類型
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        List<dynamic> Query(string cmd, object param , CommandType? commandType = null);
        /// <summary>
        /// 执行查询語句 返回DataTable
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        DataTable QueryDataTable(string cmd, object param, CommandType? commandType = null);
        #endregion

        #region 異步方法
        /// <summary>
        /// 执行强类型查询 返回List (異步)<T>
        /// </summary>
        /// <typeparam name="T">映射的結果類型</typeparam>
        /// <param name="cmd">要执行的查询</param>
        /// <param name="param">查询参数（默认= null）</param>
        /// <param name="commandType">命令类型(默认= null)</param>
        /// <returns></returns>
        Task<List<T>> QueryListAsync<T>(string cmd, object param, CommandType? commandType = null) where T : class;
        /// <summary>
        /// 执行查询并映射第一个结果（強類型）異步
        /// </summary>
        /// <typeparam name="T">映射的結果類型</typeparam>
        /// <param name="cmd">要执行的查询</param>
        /// <param name="param">查询参数（默认= null）</param>
        /// <param name="commandType">命令类型(默认= null)</param>
        /// <returns></returns>
        Task<T> QueryFirstAsync<T>(string cmd, object param, CommandType? commandType = null) where T : class;

        /// <summary>
        /// 執行RUD 語句，并返回受影響的行數 異步
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        Task<int> ExcuteNonQueryAsync(string cmd, object param, CommandType? commandType = null);
        /// <summary>
        /// 同一命令内执行多个查询并映射结果 異步
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        Task<GridReader> QueryMultipleAsync(string cmd, object param, CommandType? commandType = null);

        /// <summary>
        /// 同一命令内执行兩个查询并映射结果 異步
        /// </summary>
        /// <typeparam name="T1">返回的第一個Model</typeparam>
        /// <typeparam name="T2">返回的第二個Model</typeparam>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        Task<(List<T1>, List<T2>)> QueryMultipleAsync<T1, T2>(string cmd, object param, CommandType? commandType = null);


        /// <summary>
        /// 同一命令内执行三個个查询并映射结果 異步
        /// </summary>
        /// <typeparam name="T1">返回的第一個Model</typeparam>
        /// <typeparam name="T2">返回的第二個Model</typeparam>
        /// <typeparam name="T3">返回的第三個Model</typeparam>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        Task<(List<T1>, List<T2>, List<T3>)> QueryMultipleAsync<T1, T2, T3>(string cmd, object param, CommandType? commandType = null);


        /// <summary>
        /// 执行查询 返回動態類型 異步
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        Task<List<dynamic>> QueryAsync(string cmd, object param, CommandType? commandType = null);


        /// <summary>
        /// 执行查询語句 返回DataTable 異步
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        Task<DataTable> QueryDataTableAsync(string cmd, object param, CommandType? commandType = null);


        #endregion

        #region 事务相关
        /// <summary>
        /// 显示释放资源
        /// </summary>
        void Dispose();
        /// <summary>
        /// 开启事务
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// 事务提交
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// 事务回滚
        /// </summary>
        void RollBackTransaction();
        #endregion

        #region other
        /// <summary>
        /// 事務嵌套
        /// </summary>
        bool Committed { get; set; }
        #endregion
    }
}
