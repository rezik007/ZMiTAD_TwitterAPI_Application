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

            public int wykrzyknikow;
            public int pytajnikow;
            public int kropek;
            public int przecinkow;
        }

        // czas pierwszego pakietu
        static decimal start;
        static int sumaDlugosciZnakow;
        static List<ZapisanyStatus> historia = new List<ZapisanyStatus>();

        public static bool Add(status status)
        {
            decimal teraz = DateTime.Now.Ticks / (decimal)TimeSpan.TicksPerMillisecond; ;

            Console.WriteLine("=========================");
            Console.WriteLine("KIEDY: " + status.created_at);
            Console.WriteLine("TXT: " + status.text);
            Console.WriteLine("USER: " + status.user.name);
            Console.WriteLine("SRC: " + status.source);
            //geolokalizacja jeszcze nie dziala
            if (status.geo == null)
            {
                Console.WriteLine("GEO: NULL");
            }
            else
            {
                Console.WriteLine("GEO: " + status.geo.ToString());
            }


            ZapisanyStatus zp = new ZapisanyStatus();
            zp.wykrzyknikow = status.text.Count(f => f == '!');
            zp.pytajnikow = status.text.Count(f => f == '?');
            zp.kropek = status.text.Count(f => f == '.');
            zp.przecinkow = status.text.Count(f => f == ',');

            Console.WriteLine("Pytajnikow: " +zp.pytajnikow +
                ", wykrzyknikow " + zp.wykrzyknikow + ", kropek " + zp.kropek + ", przecinkow " + zp.przecinkow);

            zp.text = status.text;
            zp.user = status.user.name;

            sumaDlugosciZnakow += status.text.Length;

            if (historia.Count == 0)
            {
                start = teraz;
            }
            else
            {
                double minelo = (double) (teraz - start);
                minelo *= 0.001;
                Console.WriteLine("Minelo " + minelo + " sekund od pierwszego statusu, jest " + ( historia.Count + 1) + " statusow");
                double ileStatusow = historia.Count + 1;
                double ileNaSekunde = ileStatusow / minelo;
                double mineloMinut = minelo / 60.0;
                double ileNaMinute = ileStatusow / mineloMinut;
                Console.WriteLine(" " + ileNaSekunde + " statusow na sekunde");
                Console.WriteLine(" " + ileNaMinute + " statusow na minute");
                double sredniaDlugoscWpisu = (double)sumaDlugosciZnakow / ((double)historia.Count + 1);
                Console.WriteLine("Srednia dlugosc wpisu " + sredniaDlugoscWpisu + " znakow" + "\n\r");
            }
            historia.Add(zp);
            return true;
        }
    }
}
