using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using System.Windows.Forms.DataVisualization.Charting;

namespace ZMITAD_WinForms
{
    public partial class Form1 : Form, MojDataStore
    {
        decimal start;
        public Form1()
        {
            InitializeComponent();

            start = DateTime.Now.Ticks / (decimal)TimeSpan.TicksPerMillisecond; ;

            comboBox2.Items.Add("Ilosc statusow na przedzial czasu");
            comboBox2.Items.Add("Ilosc znakow na przedzial czasu");
            comboBox2.Items.Add("Ilosc slow na przedzial czasu");
            comboBox2.Items.Add("Histogram (dlugosc statusu w znakach)");
            comboBox2.SelectedIndex = 0;
        }

        private int ilosc = 0;
        private int popMin = -1;
        private decimal teraz;
        private List<decimal> czasyNadejscia = new List<decimal>();
        private List<int> ilosciSlow = new List<int>();
        private List<int> ilosciZnakow = new List<int>();
        private List<string> tresciStatusow = new List<string>();
        private int sumaDlugosciStatusow;
        private int sumaIlosciSlow;
        // dlugosc najkrotszego i nalduzszego statussu
        private int ileZnakowMin, ileZnakowMax;

        private double getCzasPrzedzialu()
        {
            /*
                     comboBox1.Items.Add("Co 1 sekunda");
            comboBox1.Items.Add("Co 30 sekund");
            comboBox1.Items.Add("Co minuta");
            comboBox1.Items.Add("Co 5 minut");
            */

            int ileSekund = 1;
            if (comboBox1.SelectedIndex == 0)
                ileSekund = 1;
            else if (comboBox1.SelectedIndex == 1)
                ileSekund = 30;
            else if (comboBox1.SelectedIndex == 2)
                ileSekund = 60;
            else if (comboBox1.SelectedIndex == 3)
                ileSekund = 60 * 5;
            return ileSekund * 1000.0;
        }
        private void odswiezWykres()
        {
            chart1.Series[0].Points.Clear();
            if (comboBox2.SelectedIndex == 3)
            {
                comboBox1.Enabled = false;
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;

                // histogram
                double przedzial = ileZnakowMax - ileZnakowMin;
                int ileSlupkow = 10; // ilosc slupkow
                double skok = przedzial / ileSlupkow;
                double min = ileZnakowMin;
                for(int i = 0; i < ileSlupkow; i++)
                {
                    // slupek i-ty
                    double max = min + skok;
                    int ileStatusow = 0;
                    // zlicz ile mamy takich statusow
                    for(int j = 0; j < ilosciZnakow.Count; j++)
                    {
                        int znakow = ilosciZnakow[j];
                        if(znakow >= min && znakow < max)
                        {
                            ileStatusow++;
                        }
                    }
                    chart1.Series[0].Points.AddXY(i, ileStatusow);
                    min += skok;
                }
            }
            else
            {
                comboBox1.Enabled = true;
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[0].Points.AddXY(0, 0);
                int px = -1;
                int ile = 0;
                for (int i = 0; i < czasyNadejscia.Count; i++)
                {
                    // czas w milisekundach
                    double x = ((double)czasyNadejscia[i]) / getCzasPrzedzialu();
                    int ix = (int)x;
                    if (px != ix)
                    {
                        if (px != -1)
                        {
                            chart1.Series[0].Points.AddXY(ix, ile);
                        }
                        px = ix;
                        ile = 0;
                    }


                    // dodaj do ile, to co zliczamy
                    if (comboBox2.SelectedIndex == 0)
                    {
                        // zliczamy statusy - dodaj 1
                        ile++;
                    }
                    else if (comboBox2.SelectedIndex == 1)
                    {
                        // zliczamy znaki - dodaj ilosc znakow i-tego statusu
                        ile += ilosciZnakow[i];
                    }
                    else if (comboBox2.SelectedIndex == 2)
                    {
                        // zliczamy slowa - dodaj ilosc slow i-tego statusu
                        ile += ilosciSlow[i];
                    }
                }
                if (ile != 0 && px != -1)
                {
                    chart1.Series[0].Points.AddXY(px, ile);
                }
            }
        }
        public double getSredniaDlStatusu()
        {
            double sr = sumaDlugosciStatusow / (double)czasyNadejscia.Count;
            return sr;
        }
        public double getSredniaIloscSlowStatusu()
        {
            double sr = sumaIlosciSlow / (double)czasyNadejscia.Count;
            return sr;
        }
        public string getCzasDzialaniaStr()
        {
            int sek = (int)(((double)teraz) / 1000);
            if(sek > 60)
            {
                int minut = sek / 60;
                sek %= 60;
                return minut.ToString() + " minut i " + sek.ToString() + " sekund";
            }
            return sek.ToString() + " sekund";
        }
        public void add(status s)
        {
            // czy wywolano z innego watku niz gui?
            if (InvokeRequired)
            {// jesli tak

                // to z pomoca invoke wywolaj jeszcze raz ta metode 
                // na watku gui
                //
          // jest to potrzebne, poniewaz gui mozna edytowac tylko z watku gui
                Invoke(new MethodInvoker(delegate
                {
                    add(s);
                }));
                // i wyjdz, bo w watku innym niz gui sie wiecej nie zrobi
                return;
            }
            // to sie juz wykona w watku gui
            if(ilosciSlow.Count == 0)
            {
                ileZnakowMin = ileZnakowMax = s.text.Length;
            }
            else
            {
                if (ileZnakowMax < s.text.Length)
                    ileZnakowMax = s.text.Length;
                if (ileZnakowMin > s.text.Length)
                    ileZnakowMin = s.text.Length;
            }
            sumaDlugosciStatusow += s.text.Length;
            int ileSlow = policzSlowa(s.text);
            sumaIlosciSlow += ileSlow;
            ilosciSlow.Add(ileSlow);
            ilosciZnakow.Add(s.text.Length);
            teraz = DateTime.Now.Ticks / (decimal)TimeSpan.TicksPerMillisecond; ;
            teraz = teraz - start;
            czasyNadejscia.Add(teraz);
            odswiezWykres();
            int sekunda = (int)((double)teraz / 1000.0);
            tresciStatusow.Add(s.text);
            textBox1.Text += "[sekunda "+sekunda+"]: "+ "Tresc statusu " + s.text + Environment.NewLine;
            //double minuta = ((double)teraz / getCzasPrzedzialu());
            //int imin = (int)minuta;
            //if(popMin == -1)
            //{
            //    //test
            //    popMin = imin;
            //    ilosc++;
            //}
            //else
            //{
            //    if(popMin != imin)
            //    {
            //        chart1.Series[0].Points.AddXY(popMin, ilosc);
            //        popMin = imin;
            //        ilosc = 0;
            //    }
            //    else
            //    {

            //    }
            //    ilosc++;
            //}
            label3.Text = "Przeanalizowano " + czasyNadejscia.Count + " statusow"
                +
                ". Program dziala " + getCzasDzialaniaStr()
                +
                " Sr. dl. statusu " + ((int)getSredniaDlStatusu()).ToString()
                +
                " Sr. ilosc slow statusu " + ((int)getSredniaIloscSlowStatusu()).ToString();

                
        }
        public void CallToChildThread()
        {
            TwitterStream stream = new TwitterStream();
            stream.Stream2Queue(this);
        }
        private int policzSlowa(String text)
        {
            // mozna by ulepszyc
            return text.Split(' ').Length;
        }
        public decimal getCzasStatusu(int i)
        {
            return czasyNadejscia[i];
        }
        public int getIloscZnakow(int i)
        {
            return ilosciZnakow[i];
        }
        public string getTresc(int i)
        {
            return tresciStatusow[i];
        }
        public int getIloscSlow(int i)
        {
            return ilosciSlow[i];
        }
        public int getIleStatusow()
        {
            return ilosciSlow.Count;
        }
        private void testNormalnosci(List<float> Dane)
        {
            // http://stats.stackexchange.com/questions/97796/appropriate-test-for-detecting-a-signal-in-normally-distributed-noise
   
            float[] sortedYs;
            sortedYs = Dane.OrderBy(a => a).ToArray<float>();
            float[] cdf = new float[sortedYs.Length];

            MathNet.Numerics.Distributions.Laplace ld = new MathNet.Numerics.Distributions.Laplace();
            for (int i = 0; i < sortedYs.Length; i++)
            {
                cdf[i] = (float)ld.CumulativeDistribution(sortedYs[i]);
            }

            float AD = 0;
            for (int i = 0; i < cdf.Length; i++)
            {
                AD -= (float)((2 * (i + 1) - 1) * (Math.Log(cdf[i]) + Math.Log(1 - cdf[cdf.Length - 1 - i])));
            }
            AD /= cdf.Length;
            AD -= cdf.Length;
            AD *= (float)(1 + (0.75 + 2.25 / cdf.Length) / cdf.Length);
        }
        private void test2()
        {
            TTestResult result = chart1.DataManipulator.Statistics.TTestUnequalVariances(0.2, 0.05, "Series1", "Series2");
            chart1.DataManipulator.Statistics.NormalDistribution
        }
        private void test3()
        {
            Accord.Statistics.Testing.ShapiroWilkTest t;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ThreadStart childref = new ThreadStart(CallToChildThread);
            Thread childThread = new Thread(childref);
            childThread.Start();

            comboBox1.Items.Add("Co 1 sekunda");
            comboBox1.Items.Add("Co 30 sekund");
            comboBox1.Items.Add("Co minuta");
            comboBox1.Items.Add("Co 5 minut");
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            odswiezWykres();
        }

        Historia h;
        private void button1_Click(object sender, EventArgs e)
        {
            if (h == null)
                h = new Historia(this);
            h.Show();
        }
        public void zamknietoHistorie()
        {
            h = null;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            odswiezWykres();
        }
    }
}
