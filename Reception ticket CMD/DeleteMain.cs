using SqlTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Reception_ticket_CMD
{
    class DeleteMain
    {
        static void Main(string[] args)
        {
            string identification = "201812294921691029";
            TicketSQL ins = new TicketSQL();
            do
            {
                string str = ins.Refund(identification);
                Console.WriteLine(str);
                Console.ReadKey();
            } while (true);
        }
       
    }
}
