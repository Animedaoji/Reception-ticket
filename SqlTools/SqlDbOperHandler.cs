using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SqlTools
{
    /// <summary>
    /// 本对象用与提供对SqlServer数据库的常用访问操作。
    /// </summary>
    public class SqlDbOperHandler : DbOperHandler
    {

        /// <summary>
        /// 构造函数，接收一个SqlServer数据库连接对象SqlConnection
        /// </summary>
        public SqlDbOperHandler(System.Data.SqlClient.SqlConnection _conn)
        {
            conn = _conn;
            dbType = DatabaseType.SqlServer;
            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandTimeout = 0;
            da = new System.Data.SqlClient.SqlDataAdapter();

        }
        public SqlDbOperHandler()
        {

            string dbConnStr = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            //string dbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            System.Data.SqlClient.SqlConnection _conn = new System.Data.SqlClient.SqlConnection(dbConnStr);
            conn = _conn;
            dbType = DatabaseType.SqlServer;
            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandTimeout = 0;
            da = new System.Data.SqlClient.SqlDataAdapter();

        }
        /// <summary>
        /// 产生SqlCommand对象所需的查询参数。
        /// </summary>
        protected override void GenParameters()
        {
            System.Data.SqlClient.SqlCommand sqlCmd = (System.Data.SqlClient.SqlCommand)cmd;
            if (this.alFieldItems.Count > 0)
            {
                for (int i = 0; i < alFieldItems.Count; i++)
                {
                    sqlCmd.Parameters.AddWithValue("@para" + i.ToString(), ((DbKeyItem)alFieldItems[i]).fieldValue.ToString());
                }
            }

            if (this.alSqlCmdParameters.Count > 0)
            {
                for (int i = 0; i < this.alSqlCmdParameters.Count; i++)
                {
                    sqlCmd.Parameters.AddWithValue(((DbKeyItem)alSqlCmdParameters[i]).fieldName.ToString(), ((DbKeyItem)alSqlCmdParameters[i]).fieldValue.ToString());
                }
            }
            if (this.alConditionParameters.Count > 0)
            {
                for (int i = 0; i < this.alConditionParameters.Count; i++)
                {
                    sqlCmd.Parameters.AddWithValue(((DbKeyItem)alConditionParameters[i]).fieldName.ToString(), ((DbKeyItem)alConditionParameters[i]).fieldValue.ToString());
                }
            }
        }
        public static DataTable QueryPagedDt(string TableName, string FieldKey, int PageCurrent, int PageSize, string FieldShow, string FieldOrder, string Where, ref int RecordCount)
        {
            string dbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SqlConnection conn = new SqlConnection(dbConnStr);
            conn.Open();
            SqlDataAdapter cmd = new SqlDataAdapter("sp_PageView", conn);
            cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter[] pars = new SqlParameter[] {
                new SqlParameter("@tbname",TableName),
                new SqlParameter("@FieldKey",FieldKey),
                new SqlParameter("@PageCurrent",PageCurrent),
                new SqlParameter("@PageSize",PageSize),
                new SqlParameter("@FieldShow",FieldShow),
                new SqlParameter("@FieldOrder",FieldOrder),
                new SqlParameter("@Where",Where),
                new SqlParameter("@RecordCount",RecordCount)
            };
            pars[7].Direction = ParameterDirection.Output;
            foreach (SqlParameter p in pars)
                cmd.SelectCommand.Parameters.Add(p);
            DataSet ds = new DataSet();
            cmd.Fill(ds);
            cmd.SelectCommand.Parameters.Clear();
            RecordCount = (int)pars[7].Value;
            conn.Close();

            return ds.Tables[0];
        }

        public static DataTable sp_Query(string sp_name, ref List<SqlParameter> pars)
        {
            string dbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SqlConnection conn = new SqlConnection(dbConnStr);
            conn.Open();
            SqlDataAdapter cmd = new SqlDataAdapter(sp_name, conn);
            cmd.SelectCommand.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter p in pars)
                cmd.SelectCommand.Parameters.Add(p);
            DataSet ds = new DataSet();
            cmd.Fill(ds);
            cmd.SelectCommand.Parameters.Clear();
            conn.Close();
            return ds.Tables[0];
        }



        public static void ExecuteSql(string Sql)
        {
            string dbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SqlConnection conn = new SqlConnection(dbConnStr);
            conn.Open();
            SqlCommand cmd = new SqlCommand(Sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public static void ExecuteSql(string Sql, SqlParameter[] pars)
        {
            try
            {
                string dbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                SqlConnection conn = new SqlConnection(dbConnStr);
                conn.Open();
                SqlCommand cmd = new SqlCommand(Sql, conn);
                if (pars != null)
                {
                    foreach (SqlParameter p in pars)
                    {
                        cmd.Parameters.Add(p);
                    }
                }
                int effectcount = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();
            }
            catch
            {

            }
        }
        public static void CopyUserInfo(string Owner, string newowner)
        {
            try
            {
                string dbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                SqlConnection conn = new SqlConnection(dbConnStr);
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[DishCopy]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] pars = new SqlParameter[] {
                new SqlParameter("@Owner",Owner),
                 new SqlParameter("@newowner",newowner)
            };
                foreach (SqlParameter p in pars)
                    cmd.Parameters.Add(p);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();

            }
            catch
            {
            }

        }
        public static void UpdateUserInfo(string Owner, string newowner)
        {
            try
            {
                string dbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                SqlConnection conn = new SqlConnection(dbConnStr);
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[DishUpdate]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] pars = new SqlParameter[] {
                new SqlParameter("@Owner",Owner),
                 new SqlParameter("@newowner",newowner)
            };
                foreach (SqlParameter p in pars)
                    cmd.Parameters.Add(p);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();

            }
            catch
            {
            }
        }

        public static bool ExecSqlTransaction(List<string> strSql, List<SqlParameter[]> sqlParameterList)
        {
            string dbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            bool returnState = false;

            SqlConnection conn = new SqlConnection(dbConnStr);
            conn.Open();

            SqlTransaction sqlTransaction = conn.BeginTransaction();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandTimeout = 90;
            sqlCommand.Connection = conn;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Transaction = sqlTransaction;

            try
            {
                int i = 0;
                foreach (string sql in strSql)
                {
                    sqlCommand.CommandText = sql;
                    sqlCommand.Parameters.Clear();
                    try
                    {
                        if (sqlParameterList != null && sqlParameterList.Count > i)
                        {
                            SqlParameter[] prams = (SqlParameter[])sqlParameterList[i];
                            if (prams != null)
                            {
                                foreach (SqlParameter pram in prams)
                                {
                                    sqlCommand.Parameters.Add(pram);
                                }
                            }
                        }
                    }
                    catch { }
                    sqlCommand.ExecuteNonQuery();
                    i++;
                }
                sqlTransaction.Commit();
                returnState = true;
            }
            catch
            {
                sqlTransaction.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
            return returnState;
        }
        public static void ExecProcedure(string ProcName, SqlParameter[] pars)
        {
            string dbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SqlConnection conn = new SqlConnection(dbConnStr);
            conn.Open();
            SqlCommand cmd = new SqlCommand(ProcName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (pars != null)
            {
                foreach (SqlParameter p in pars)
                {
                    cmd.Parameters.Add(p);
                }
            }
            cmd.ExecuteNonQuery();
            conn.Close();

        }
        public static DataTable QueryDtByProc(string TableName, SqlParameter[] pars)
        {
            string dbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SqlConnection conn = new SqlConnection(dbConnStr);
            conn.Open();
            SqlDataAdapter cmd = new SqlDataAdapter(TableName, conn);
            cmd.SelectCommand.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter p in pars)
                cmd.SelectCommand.Parameters.Add(p);
            DataSet ds = new DataSet();
            cmd.Fill(ds);
            cmd.SelectCommand.Parameters.Clear();
            conn.Close();

            return ds.Tables[0];
        }

        public void SqlBulkCopyByDatatable(string TableName, DataTable dt)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

            using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction))
            {
                sqlbulkcopy.BulkCopyTimeout = 0;
                try
                {
                    sqlbulkcopy.DestinationTableName = TableName;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                    }
                    sqlbulkcopy.WriteToServer(dt);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                sqlbulkcopy.Close();
            }
        }
    }
}
