using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterStreamClient
{
    class DataStore
    {
        public static bool Add(status status)
        {
            Console.WriteLine("=========================");
            Console.WriteLine("KIEDY: " + status.created_at);
            Console.WriteLine("TXT: " + status.text);
            Console.WriteLine("USER: " + status.user.name);
            return true;
        }
    }
}
