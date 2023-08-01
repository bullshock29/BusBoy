using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class Core
    {
        public string DoWork(string message)
        {
            return message + " Replied";
        }
    }
}
