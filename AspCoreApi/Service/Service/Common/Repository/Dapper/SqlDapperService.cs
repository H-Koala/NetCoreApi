using Dapper;
using IServices.ICommon.IRepository.IDapper;
using Oracle.ManagedDataAccess.Client;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Services.Common.Repository.Dapper
{
    public class SqlDapperService : ISqlDapperService
    {
        #region 初始化
        /// <summary>
        /// 數據庫連接字符串
        /// </summary>
        private string _connectionString;
        /// <summary>
        /// 事務標識
        /// </summary>
        private bool _Committed = true;
        /// <summary>
        /// IDbConnection
        /// </summary>
        private IDbConnection _connection { get; set; }
        /// <summary>
        /// Connection
        /// </summary>
        private IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new ProfiledDbConnection(new SqlConnection(_connectionString), MiniProfiler.Current); //new SqlConnection(_connectionString);
                }
                return _connection;
            }
        }
        /// <summary>
        /// 事务是否打開
        /// </summary>
        public bool Committed
        {
            get { return _Committed; }
            set { Committed = true; }
        }


        

        /// <summary>
        /// 事务
        /// </summary>
        public IDbTransaction DbTransaction { get; set; } = null;
        #endregion

        /// <summary>
        ///  構造函數
        /// </summary>
        /// <param name="connKeyName">數據庫連接字符串</param>
        /// <param name="dbName">DB 類型</param>
        public SqlDapperService(string connKeyName, Db_Type.DbName dbName)
        {
            _connectionString = connKeyName;
            //var _connection = new ProfiledDbConnection(new SqlConnection(_connectionString), MiniProfiler.Current);
            switch ((int)dbName)
            {
                case 0:
                    _connection = new ProfiledDbConnection(new OracleConnection(_connectionString), MiniProfiler.Current); // new OracleConnection(_connectionString);
                    break;
                case 1:
                    _connection = new ProfiledDbConnection(new MySql.Data.MySqlClient.MySqlConnection(_connectionString), MiniProfiler.Current); //new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
                    break;
                case 2:
                    _connection = new ProfiledDbConnection(new SqlConnection(_connectionString), MiniProfiler.Current); //new SqlConnection(_connectionString);
                    break;
                default:
                    break;
            }
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHandler());
            SqlMapper.AddTypeMap(typeof(DateTime), System.Data.DbType.Date);
        }

        #region 同步方法
        /// <summary>
        /// 执行强类型查询 返回List<T>
        /// </summary>
        /// <typeparam name="T">映射的結果類型</typeparam>
        /// <param name="cmd">要执行的查询</param>
        /// <param name="param">查询参数（默认= null）</param>
        /// <param name="commandType">命令类型(默认= null)</param>
        /// <returns></returns>
        public List<T> QueryList<T>(string cmd, object param, CommandType? commandType = null) where T : class
        {
            return Execute((conn, dbTransaction) =>
            {
                return conn.Query<T>(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text).ToList();
            });
        }
        /// <summary>
        /// 执行查询并映射第一个结果（強類型）
        /// </summary>
        /// <typeparam name="T">映射的結果類型</typeparam>
        /// <param name="cmd">要执行的查询</param>
        /// <param name="param">查询参数（默认= null）</param>
        /// <param name="commandType">命令类型(默认= null)</param>
        /// <returns></returns>
        public T QueryFirst<T>(string cmd, object param, CommandType? commandType = null) where T : class
        {
            List<T> list = QueryList<T>(cmd, param, commandType: commandType ?? CommandType.Text).ToList();
            return list.Count == 0 ? null : list[0];
        }
        /// <summary>
        /// 执行查询，并返回查询返回的结果集中第一行的第一列。其他列或行将被忽略
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public object ExecuteScalar(string cmd, object param, CommandType? commandType = null)
        {
            return Execute<object>((conn, dbTransaction) =>
            {
                return conn.ExecuteScalar(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text);
            });
        }
        /// <summary>
        /// 執行RUD 語句，并返回受影響的行數
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public int ExcuteNonQuery(string cmd, object param, CommandType? commandType = null)
        {
            return Execute<int>((conn, dbTransaction) =>
            {
                return conn.Execute(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text);
            });
        }
        /// <summary>
        /// Dapper不处理查询结果时，通常使用此方法。例如，填写DataTable或DataSet
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string cmd, object param, CommandType? commandType = null)
        {
            return Execute<IDataReader>((conn, dbTransaction) =>
            {
                return conn.ExecuteReader(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text);
            });
        }
        /// <summary>
        /// 同一命令内执行多个查询并映射结果
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public SqlMapper.GridReader QueryMultiple(string cmd, object param, CommandType? commandType = null)
        {
            return Execute((conn, dbTransaction) =>
            {
                return conn.QueryMultiple(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text);
            });
        }

        /// <summary>
        /// 同一命令内执行兩个查询并映射结果
        /// </summary>
        /// <typeparam name="T1">返回的第一個Model</typeparam>
        /// <typeparam name="T2">返回的第二個Model</typeparam>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public (List<T1>, List<T2>) QueryMultiple<T1, T2>(string cmd, object param, CommandType? commandType = null)
        {
            using (SqlMapper.GridReader reader = QueryMultiple(cmd, param, commandType))
            {
                return (reader.Read<T1>().ToList(), reader.Read<T2>().ToList());
            }
        }
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
        public (List<T1>, List<T2>, List<T3>) QueryMultiple<T1, T2, T3>(string cmd, object param, CommandType? commandType = null)
        {
            using (SqlMapper.GridReader reader = QueryMultiple(cmd, param, commandType))
            {
                return (reader.Read<T1>().ToList(), reader.Read<T2>().ToList(), reader.Read<T3>().ToList());
            }
        }

        /// <summary>
        /// 执行查询 返回動態類型
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public List<dynamic> Query(string cmd, object param, CommandType? commandType = null)
        {
            return Execute((conn, dbTransaction) =>
            {
                return conn.Query(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text).ToList();
            });
        }
        /// <summary>
        /// 执行查询語句 返回DataTable
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public DataTable QueryDataTable(string cmd, object param, CommandType? commandType = null)
        {

            return Execute((conn, dbTransaction) =>
            {
                DataTable dt = new DataTable("table");
                dt.Load(conn.ExecuteReader(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text));
                return dt;
            });

        }

        /// <summary>
        /// 執行方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">泛型函數</param>
        /// <returns></returns>
        private T Execute<T>(Func<IDbConnection, IDbTransaction, T> func)
        {
            try
            {
                T reslutT = func(Connection, DbTransaction);
                return reslutT;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
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
        public Task<List<T>> QueryListAsync<T>(string cmd, object param, CommandType? commandType = null) where T : class {
            return ExecuteAsync(async (conn, dbTransaction) =>
            {
                var result = await conn.QueryAsync<T>(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text);
                return result.ToList();
            });
        }

        /// <summary>
        /// 执行查询并映射第一个结果（強類型）異步
        /// </summary>
        /// <typeparam name="T">映射的結果類型</typeparam>
        /// <param name="cmd">要执行的查询</param>
        /// <param name="param">查询参数（默认= null）</param>
        /// <param name="commandType">命令类型(默认= null)</param>
        /// <returns></returns>
        public Task<T> QueryFirstAsync<T>(string cmd, object param, CommandType? commandType = null) where T : class {
            return ExecuteAsync(async (conn, dbTransaction) =>
            {
                var result = await conn.QueryAsync<T>(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text);
                return result.ToList().Count == 0 ? null : result.ToList()[0];
            });
        }

        /// <summary>
        /// 執行RUD 語句，并返回受影響的行數 異步
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public Task<int> ExcuteNonQueryAsync(string cmd, object param, CommandType? commandType = null) {
            return ExecuteAsync(async (conn, dbTransaction) =>
            {
                return await conn.ExecuteAsync(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text);
            });
        }


        /// <summary>
        /// 同一命令内执行多个查询并映射结果
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public Task<GridReader> QueryMultipleAsync(string cmd, object param, CommandType? commandType = null) {
            return ExecuteAsync(async (conn, dbTransaction) =>
            {
                return await conn.QueryMultipleAsync(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text);
            });
        }

        /// <summary>
        /// 同一命令内执行兩个查询并映射结果 異步
        /// </summary>
        /// <typeparam name="T1">返回的第一個Model</typeparam>
        /// <typeparam name="T2">返回的第二個Model</typeparam>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public async Task<(List<T1>, List<T2>)> QueryMultipleAsync<T1, T2>(string cmd, object param, CommandType? commandType = null) {
            using (GridReader reader = await QueryMultipleAsync(cmd, param, commandType))
            {
                return (reader.Read<T1>().ToList(), reader.Read<T2>().ToList());
            }
        }


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
        public async Task<(List<T1>, List<T2>, List<T3>)> QueryMultipleAsync<T1, T2, T3>(string cmd, object param, CommandType? commandType = null) {
            using (GridReader reader = await QueryMultipleAsync(cmd, param, commandType))
            {
                return (reader.Read<T1>().ToList(), reader.Read<T2>().ToList(), reader.Read<T3>().ToList());
            }
        }

        /// <summary>
        /// 执行查询 返回動態類型
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public Task<List<dynamic>> QueryAsync(string cmd, object param, CommandType? commandType = null) {
            return ExecuteAsync(async (conn, dbTransaction) =>
            {
                var result = await conn.QueryAsync(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text);
                return result.ToList();
            });
        }



        /// <summary>
        /// 执行查询語句 返回DataTable 異步
        /// </summary>
        /// <param name="cmd">要执行的命令文本</param>
        /// <param name="param">命令参数（默认= null）</param>
        /// <param name="commandType">命令类型（默认= null）</param>
        /// <returns></returns>
        public Task<DataTable> QueryDataTableAsync(string cmd, object param, CommandType? commandType = null) {
            return ExecuteAsync(async (conn, dbTransaction) =>
            {
                DataTable dt = new DataTable("table");
                dt.Load( await conn.ExecuteReaderAsync(cmd, param, dbTransaction, commandType: commandType ?? CommandType.Text));
                return dt;
            });
        }


        /// <summary>
        /// 異步執行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private Task<T> ExecuteAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> func)
        {
            try
            {
                Task<T> reslutT = func(Connection, DbTransaction);
                return reslutT;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region 事務相關
        /// <summary>
        /// 显式释放资源
        /// </summary>
        public void Dispose()
        {
            DbTransaction?.Dispose();
            if (Connection.State == ConnectionState.Open)
                Connection?.Close();
            Connection.Dispose();
        }
        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTransaction()
        {
            _Committed = false;
            bool isClosed = Connection.State == ConnectionState.Closed;
            if (isClosed) Connection.Open();
            DbTransaction = Connection?.BeginTransaction();
        }


        /// <summary>
        /// 事务提交
        /// </summary>
        public void CommitTransaction()
        {
            DbTransaction?.Commit();
            _Committed = true;
            Dispose();
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void RollBackTransaction()
        {
            DbTransaction?.Rollback();
            _Committed = true;
            Dispose();
        }
        #endregion

        #region Other
        /// <summary>
        ///将DataTable转换为标准的CSV
        /// </summary>
        /// <param name="table">数据表</param>
        /// <returns>返回标准的CSV</returns>
        private string DataTableToCsv(DataTable table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            Type typeString = typeof(string);
            Type typeDate = typeof(DateTime);

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeString && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else if (colum.DataType == typeDate)
                    {
                        //centos系统里把datatable里的日期转换成了10/18/18 3:26:15 PM格式
                        bool b = DateTime.TryParse(row[colum].ToString(), out DateTime dt);
                        sb.Append(b ? dt.ToString("yyyy-MM-dd HH:mm:ss") : "");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
        #endregion
    }
    public class GuidTypeHandler : SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            OracleParameter oracleParameter = (OracleParameter)parameter;
            oracleParameter.OracleDbType = OracleDbType.Raw;
            parameter.Value = value;
        }
        
        public object Parse(Type destinationType, object value)
        {
            return new Guid((byte[])value);
        }
    }
}
