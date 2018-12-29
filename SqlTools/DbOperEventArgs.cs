using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlTools
{
    class DbOperEventArgs : System.EventArgs
    {
        public int id;
        public DbOperEventArgs(int _id)
        {
            id = _id;
        }
    }
}
