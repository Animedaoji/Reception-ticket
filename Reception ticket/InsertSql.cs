using SqlTools;
using System;
using System.Data;
using System.Text;

namespace Reception_ticket
{
    public class InsertSql
    {

        /// <summary>
        /// 返回此次生成的二维码
        /// </summary>
        /// <param name="meal_date">就餐日期</param>
        /// <param name="TicketCount">生成数量</param>
        /// <returns></returns>
        public DataTable SelectQRCode(DateTime meal_date, int TicketCount)
        {
            DataTable dt = null;
            try
            {
                SqlDbOperHandler doh = new SqlDbOperHandler();//开启连接数据库
                doh.Reset();
                doh.SqlCmd = "select top(@TicketCount) * from [m_t_application] where meal_date = @meal_date order by ticketCreate desc";
                doh.AddConditionParameter("@TicketCount", TicketCount);
                doh.AddConditionParameter("@meal_date", meal_date.ToString("yyyy-MM-dd"));
                dt = doh.GetDataTable();//获取返回的表格
                doh.Dispose();//释放资源
            }
            catch (Exception e)
            {
                LogClass.CreateLog(e.Message.ToString());
            }
            return dt;
        }
        #region 插入餐票到数据库
        public string InsertTickets(DateTime meal_date, string meal_location, string meal_type, string meal, decimal amount, string operatorMan, int TicketCount)
        {
            //返回的字符串
            StringBuilder CallbackStr = new StringBuilder();

            //餐票生成的时间
            DateTime ticketCreate = DateTime.Now;
            //餐票状态
            //string ticketStatus
            //二维码标识 yyyy-MM-dd Random(6) id
            string identification = null;
            //生成失败的数量
            int errorCount = 0;
            //生成二维码的唯一ID
            int OrderID = GetLastID() + 1;

            if (OrderID > 0)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < TicketCount; i++)
                {
                    identification = ticketCreate.ToString("yyyyMMdd") + GetRandomString(6) + (OrderID + i).ToString();
                    //插入数据库
                    int error = InsertTicketsSql(meal_date, meal_location, meal_type, meal, amount, identification, operatorMan, ticketCreate);
                    errorCount += error;
                    if (error == 0)
                    {
                        sb.Append("|" + identification);
                    }
                    LogClass.CreateLog("------------- 餐票生成 -------------");
                    LogClass.CreateLog(operatorMan + "插入餐票：" + identification);
                    LogClass.CreateLog("------------- 餐票生成 -------------");
                }
                CallbackStr.Append(errorCount);
                CallbackStr.Append(sb.ToString());
            }
            else
            {
                CallbackStr.Append("404|生成二维码时未检索到表的存在");
            }
            return CallbackStr.ToString();
        }

        private int InsertTicketsSql(DateTime meal_date, string meal_location, string meal_type, string meal, decimal amount, string identification, string operator_man, DateTime ticketCreate)
        {
            try
            {
                SqlDbOperHandler doh = new SqlDbOperHandler();//开启数据库连接
                doh.Reset();
                doh.SqlCmd = String.Format("insert into [m_t_application] (meal_date,meal_location,meal_type,meal,amount,identification,operator_man,ticketStatus,ticketCreate)" +
                   " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');",
                   meal_date.Date, meal_location, meal_type, meal, amount.ToString("#0.00"), identification, operator_man, "未使用", ticketCreate);//"insert into [m_t_application] (meal_date,meal_location,,meal_type,meal,amount,identification,operator_man,ticketStatus,ticketCreate) values();";//sql语句
                doh.ExecuteSqlNonQuery();//执行不返回的方法
                doh.Dispose();//释放资源
            }
            catch (Exception e)
            {
                //自己写的打印日志的工具类
                LogClass.CreateLog("error:" + e.Message.ToString());
                return 1;
            }

            return 0;
        }
        #endregion


        #region 内部方法
        /// <summary>
        /// 生成随机数，以辩别图片
        /// </summary>
        /// <param name="iLength"></param>
        /// <returns></returns>
        private static string GetRandomString(int iLength)
        {
            string buffer = "0123456789";// 随机字符中也可以为汉字（任何）
            StringBuilder sb = new StringBuilder();

            int range = buffer.Length;
            for (int i = 0; i < iLength; i++)
            {
                long tick = DateTime.Now.Ticks;
                Random r = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32 + i));
                sb.Append(buffer.Substring(r.Next(range), 1));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取最新的ID
        /// </summary>
        /// <returns>ID</returns>
        private int GetLastID()
        {
            DataTable dt = null;
            try
            {
                SqlDbOperHandler doh = new SqlDbOperHandler();//开启连接数据库
                doh.Reset();
                doh.SqlCmd = "select top(1) [id] from [m_t_application] where 1 = 1 order by ticketCreate desc";
                dt = doh.GetDataTable();//获取返回的表格
                doh.Dispose();//释放资源
            }
            catch (Exception e)
            {
                LogClass.CreateLog(e.Message.ToString());
            }
            if (dt == null)
            {
                return -1;//-1 代表无表
            }
            else
            {
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt16(dt.Rows[0]["id"].ToString());
                }
                else
                {
                    return 0;
                }
            }

        }
        #endregion
    }
}