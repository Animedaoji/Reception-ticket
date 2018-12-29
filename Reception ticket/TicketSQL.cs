using SqlTools;
using System;
using System.Data;

namespace Reception_ticket
{
    public class TicketSQL
    {
        // 扣费SQL操作
        public string WithdrawMoney(string QRCode)
        {
            //返回的字符串
            string callBack = null;
            DataTable dt = null;
            //判读二维码，并执行相应的操作
            int status = IfUsedOrOutTime(QRCode);
            switch (status)
            {
                case 100:
                    callBack = "判读餐票状态错误";
                    break;
                case 101:
                    callBack = "已消费,请勿重刷";
                    break;
                case 102:
                    callBack = "已退款，请勿重刷";
                    break;
                case 1:
                    callBack = "就餐时间已过，请按时就餐";
                    break;
                case -1:
                    callBack = "未到就餐时间，请耐心等待";
                    break;
                case 0:
                    //判读有效
                    try
                    {
                        SqlDbOperHandler doh = new SqlDbOperHandler();//开启连接数据库
                        doh.Reset();
                        doh.SqlCmd = "update [m_t_application] set ticketStatus = '已消费',UsedTime = GETDATE() where identification = '" + QRCode + "'";
                        doh.AddConditionParameter("@identification", QRCode);
                        dt = doh.GetDataTable();//获取返回的表格
                        doh.Dispose();//释放资源
                    }
                    catch (Exception e)
                    {
                        LogClass.CreateLog(e.Message.ToString());
                    }
                    callBack = "二维码验证成功";
                    break;
                case 404:
                    callBack = "找不到该二维码";
                    break;
                default:
                    callBack = "扣费查询错误,请联系行政管理员";
                    break;
            }
            return callBack;
        }
        // 退款SQL操作
        public string Refund(string QRCode)
        {
            //返回的字符串
            string callBack = null;
            DataTable dt = null;
            //判读二维码，并执行相应的操作
            int status = IfUsedOrOutTime(QRCode);
            switch (status)
            {
                case 100:
                    callBack = "判读餐票状态错误";
                    break;
                case 101:
                    callBack = "已消费,请勿重刷";
                    break;
                case 102:
                    callBack = "已退款，请勿重刷";
                    break;
                case 1:
                    callBack = "就餐时间已过，无法退款";
                    break;
                case -1:
                    try
                    {
                        SqlDbOperHandler doh = new SqlDbOperHandler();
                        doh.Reset();
                        doh.SqlCmd = "update [m_t_application] set ticketStatus = '已退款',UsedTime = GETDATE() where identification = '" + QRCode + "'";
                        dt = doh.GetDataTable();
                        doh.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogClass.CreateLog(e.Message.ToString());
                    }
                    finally
                    {
                        callBack = "退款成功";
                    }
                    break;
                case 0:
                    //判读有效
                    try
                    {
                        SqlDbOperHandler doh = new SqlDbOperHandler();
                        doh.Reset();
                        doh.SqlCmd = "update [m_t_application] set ticketStatus = '已退款',UsedTime = GETDATE() where identification = '" + QRCode + "'";
                        dt = doh.GetDataTable();
                        doh.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogClass.CreateLog(e.Message.ToString());
                    }
                    finally
                    {
                        callBack = "退款成功";
                    }
                    break;
                case 404:
                    callBack = "找不到该二维码";
                    break;
                default:
                    callBack = "退款查询错误,请联系行政管理员";
                    break;
            }
            return callBack;
        }
        // 判断二维码是否已经使用或者过期
        public int IfUsedOrOutTime(string QRCode)
        {
            int ifHave = 404;
            //100代表判读状态失误，101代表已消费，102代表已退款
            //0代表有效，-1代表未到吃饭时间，1代表超过吃饭时间
            DataTable dt = null;
            DateTime meal = DateTime.Now;
            try
            {
                SqlDbOperHandler doh = new SqlDbOperHandler();//开启连接数据库
                doh.Reset();
                doh.SqlCmd = "select top(1) [identification],[meal],[ticketStatus] from [m_t_application] where identification = " + QRCode + " order by ticketCreate desc";
                dt = doh.GetDataTable();//获取返回的表格
                doh.Dispose();//释放资源
            }
            catch (Exception e)
            {
                LogClass.CreateLog(e.Message.ToString());
            }

            #region 判读餐票状态的逻辑
            string ticketStatus = dt.Rows[0]["ticketStatus"].ToString();
            if (ticketStatus == "未使用")
            {
                #region 判断就餐时间逻辑
                string eatType = dt.Rows[0]["meal"].ToString();
                string strEat = System.Configuration.ConfigurationManager.AppSettings["EatStrategy"].ToString();
                string[] EatStrategy = strEat.Split('|');
                foreach (string type in EatStrategy)
                {
                    if (type == eatType)
                    {
                        ifHave = EatDateTime(type, meal, ifHave);
                    }
                }
                #endregion
            }
            else if (ticketStatus == "已消费")
            {
                ifHave = 101;
            }
            else if (ticketStatus == "已退款")
            {
                ifHave = 102;
            }
            else
            {
                ifHave = 100;
            }
            #endregion

            return ifHave;
        }
        #region 私有方法

        /// <summary>
        /// 查询配置文件判读就餐策略
        /// </summary>
        /// <param name="eatType">餐次类型</param>
        /// <param name="timeNow">刷二维码的时间</param>
        /// <param name="ifHave">返回字段</param>
        /// <returns></returns>
        private int EatDateTime(string eatType, DateTime timeNow, int ifHave)
        {
            string strGet = System.Configuration.ConfigurationManager.AppSettings[eatType].ToString();
            string[] eatTime = strGet.Split('|');

            DateTime start = Convert.ToDateTime(eatTime[0]);
            DateTime end = Convert.ToDateTime(eatTime[1]);
            if (timeNow >= start && timeNow <= end)
            {
                ifHave = 0;
            }
            else if (timeNow < start)
            {
                ifHave = -1;
            }
            else if (timeNow > end)
            {
                ifHave = 1;
            }
            return ifHave;
        }
        #endregion
    }
}