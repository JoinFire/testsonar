using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using static TrtInterface.Iot.IotHelp;

namespace TrtInterface.Sql
{
    /// <summary>
    /// 数据访问基础类
    /// </summary>
    public static class DBOraHelper
    {
        /// <summary>
        /// 数据库连接字符串属性，可重新设置数据库连接字符串
        /// </summary>
        public static string ConnectionString { get; private set; } = ConfigurationManager.AppSettings["conn"].ToString();
        /// <summary>  
        /// 执行一条计算查询结果语句，返回查询结果（object）。  
        /// </summary>  
        /// <param name="SQLString">计算查询结果语句</param>  
        /// <returns>查询结果（object）</returns>  
        public static object GetSingle(string SQLString, params OracleParameter[] cmdParms)
         {  
             using (OracleConnection conn = new OracleConnection(ConnectionString))  
             {  
                 using (OracleCommand cmd = new OracleCommand())  
                 {  
                     try  
                     {  
                         PrepareCommand(cmd, conn, null, SQLString, cmdParms);  
                         object obj = cmd.ExecuteScalar();  
                         cmd.Parameters.Clear();  
                         if((Object.Equals(obj,null))||(Object.Equals(obj, System.DBNull.Value)))  
                         {                     
                             return null;  
                         }  
                         else  
                         {  
                             return obj;  
                         }                 
                     }  
                     catch(Oracle.ManagedDataAccess.Client.OracleException e)  
                     {                 
                         throw new Exception(e.Message);  
                     }
                    finally
                    {
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
                        }
                    }
                }  
             }  
         }  
         /// <summary>  
         /// 执行SQL语句，返回影响的记录数  
         /// </summary>  
         /// <param name="SQLString">SQL语句</param>  
         /// <returns>影响的记录数</returns>  
         public static int ExecuteSql(string SQLString, params OracleParameter[] cmdParms)
         {  
             using (OracleConnection conn = new OracleConnection(ConnectionString))  
             {                 
                 using (OracleCommand cmd = new OracleCommand())  
                 {  
                     try  
                     {         
                         PrepareCommand(cmd, conn, null, SQLString, cmdParms);  
                         int rows = cmd.ExecuteNonQuery();  
                         cmd.Parameters.Clear();  
                         return rows;  
                     }  
                     catch(Oracle.ManagedDataAccess.Client.OracleException E)  
                     {                 
                         throw new Exception(E.Message);  
                     }
                    finally
                    {
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
                        }
                    }
                }                 
             }
        }

        public static int ExecuteSql(List<SqlsInfo> Sqls)
        {
            int rows = 0;
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        foreach (SqlsInfo item in Sqls)
                        {
                            if (item.Type == 0)
                            {
                                cmd.Parameters.Clear();
                                PrepareCommand(cmd, conn, null, item.Sql, new OracleParameter[] { item.Parameter });
                                Log.Logs.WriteDXLog(string.Format("执行sql语句:{0}",item.Sql));
                                rows += cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                PrepareCommand(cmd, conn, null, item.Sql, null);
                                rows += cmd.ExecuteNonQuery();
                            }
                        }

                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (Oracle.ManagedDataAccess.Client.OracleException E)
                    {
                        cmd.Transaction.Rollback();
                        Log.Logs.WriteJsonLogForDebug("执行sql时出现错误",E);
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
                        }
                    }
                }
            }
        }


         private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
         {  
             if (conn.State != ConnectionState.Open)  
                 conn.Open();  
             cmd.Connection = conn;  
             cmd.CommandText = cmdText;  
             if (trans != null)  
                 cmd.Transaction = trans;  
             cmd.CommandType = CommandType.Text;//cmdType;  
             if (cmdParms != null)   
             {  
                 foreach (OracleParameter parm in cmdParms)  
                     cmd.Parameters.Add(parm);  
             }
        }

        /// <summary>  
        /// 执行多条SQL语句，实现数据库事务。  
        /// </summary>  
        /// <param name="SQLStringList">多条SQL语句</param>       
        public static void ExecuteSqlTran(List<String> SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                OracleTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    foreach (string sql in SQLStringList)
                    {
                        if (!String.IsNullOrEmpty(sql))
                        {
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (Oracle.ManagedDataAccess.Client.OracleException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>        
        public static string ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                OracleTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return string.Empty;
                }
                catch (Oracle.ManagedDataAccess.Client.OracleException E)
                {
                    tx.Rollback();
                    return E.Message.ToString();
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
                return ds;
            }
        }

        public static DataSet Query(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (OracleException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                    }
                    return ds;
                }
            }
        }
    }
}