using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterStreamClient
{
    class DataStore
    {
        class ZapisanyStatus
        {
            public string text;
            public string user;

        }
        public static bool Add(status status)
        {
            Console.WriteLine("=========================");
            Console.WriteLine("KIEDY: " + status.created_at);
            Console.WriteLine("TXT: " + status.text);
            Console.WriteLine("USER: " + status.user.name);
            Console.WriteLine("SRC: " + status.source);
            if (status.geo == null)
            {
                Console.WriteLine("GEO: NULL");
            }
            else
            {
                Console.WriteLine("GEO: " + status.geo.ToString());
            }
            int wykrzyknikow = status.text.Count(f => f == '!');
            int pytajnikow = status.text.Count(f => f == '?');
        
            Console.WriteLine("Pytajnikow: " +pytajnikow + ", wykrzyknikow " + wykrzyknikow);

            return true;
        }
    }
}
