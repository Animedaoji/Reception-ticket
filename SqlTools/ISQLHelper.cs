using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlTools
{
    public class ISQLHelper
    {
        public DataTable select(int PageCurrent, int PageSize, string FieldKey, string FieldOrder, string Where, ref int RecordCount)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = SqlDbOperHandler.QueryPagedDt(this.GetType().Name, FieldKey, PageCurrent, PageSize, "", FieldOrder, Where, ref RecordCount);
            }
            catch (Exception ex)
            { }
            return dt;
        }

        public DataTable select(int PageCurrent, int PageSize, string FieldOrder, string Where, ref int RecordCount)
        {

            DataTable dt = new DataTable();
            try
            {
                dt = SqlDbOperHandler.QueryPagedDt(this.GetType().Name, "id", PageCurrent, PageSize, "", FieldOrder, Where, ref RecordCount);
            }
            catch (Exception ex)
            { }
            return dt;
        }

        public DataTable select(string PageCurrent, string PageSize, string FieldOrder, string Where, ref int RecordCount)
        {

            int pc = 0;
            int ps = 0;

            int.TryParse(PageCurrent, out pc);
            int.TryParse(PageSize, out ps);
            DataTable dt = new DataTable();
            try
            {
                dt = SqlDbOperHandler.QueryPagedDt(this.GetType().Name, "id", pc, ps, "", FieldOrder, Where, ref RecordCount);
            }
            catch (Exception ex)
            { }
            return dt;
        }

        public string sqlsecure(string p)
        {

            p = new Regex("select", RegexOptions.IgnoreCase).Replace(p, "");
            p = new Regex("delete", RegexOptions.IgnoreCase).Replace(p, "");
            p = new Regex("update", RegexOptions.IgnoreCase).Replace(p, "");
            p = new Regex("insert", RegexOptions.IgnoreCase).Replace(p, "");
            return p;
        }
    }
}
