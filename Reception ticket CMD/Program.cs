using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reception_ticket_CMD
{
    class Program
    {


        static void Main(string[] args)
        {
            //就餐日期
            DateTime meal_date = DateTime.Now.Date;
            //就餐地点
            string meal_location = "测试地点";
            //餐票类型
            string meal_type = "接待票";
            //餐次
            string meal = "晚餐";
            //金额
            decimal amount = 5.5555m;
            //操作人
            string operatorMan = "测试员";
            //生成餐票的数量
            int TicketCount = 5;
            InsertSql ins = new InsertSql();

            do
            {
                StringBuilder sb = new StringBuilder();
                string backstr = ins.InsertTickets(meal_date, meal_location, meal_type, meal, amount, operatorMan, TicketCount);

                string[] QRCode = backstr.Split('|');
                string error = QRCode[0];
                if (error == "404")
                {
                    string s1 = QRCode[1];
                    sb.Append("{\"error\":404}");
                }
                else
                {
                    sb.Append("{\"error\":" + (TicketCount - Convert.ToInt16(QRCode[0])));
                    for (int i = 1; i < QRCode.Length; i++)
                    {
                        string s2 = QRCode[i];
                        sb.Append(",\"" + i + "\":\"" + QRCode[i] + "\"");

                    }
                    sb.Append("}");
                }
                Console.WriteLine(sb.ToString());
                Console.ReadKey();
            } while (true);
        }
        

    }
}
