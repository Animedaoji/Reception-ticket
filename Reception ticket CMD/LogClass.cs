using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Reception_ticket_CMD
{
    public class LogClass
    {
        public static void CreateLog(string strlog)
        {
            string str1 = "QYWeixin_log" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            //BS CS应用日志自适应
            string path = HttpContext.Current == null ? Path.GetFullPath("..") + "\\temp\\" : System.Web.HttpContext.Current.Server.MapPath("temp");
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, str1);
                StreamWriter sw = File.AppendText(path);
                sw.WriteLine("\n" + DateTime.Now + "--->>\t" + strlog);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
    }
}