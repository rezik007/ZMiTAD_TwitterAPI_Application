using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;

namespace ZMITAD_WinForms
{
    public partial class Form1 : Form, MojDataStore
    {
        decimal start;
        public Form1()
        {
            InitializeComponent();

            start = DateTime.Now.Ticks / (decimal)TimeSpan.TicksPerMillisecond; ;

            comboBox2.Items.Add("Ilość statusów na przedział czasu");
            comboBox2.Items.Add("Ilość znaków na przedział czasu");
            comboBox2.Items.Add("Ilość słów na przedział czasu");
            comboBox2.Items.Add("Histogram (długość statusu w znakach)");
            comboBox2.SelectedIndex = 0;
        }

        private decimal teraz;
        private List<decimal> czasyNadejscia = new List<decimal>();
        private List<int> ilosciSlow = new List<int>();
        private List<int> ilosciZnakow = new List<int>();
        private List<string> tresciStatusow = new List<string>();
        private int sumaDlugosciStatusow;
        private int sumaIlosciSlow;
        // dlugosc najkrotszego i najdluzszego statusu
        private int ileZnakowMin, ileZnakowMax;
        // tablica hashujaca slowa
        private Hashtable slowa = new Hashtable();
        // forma do slow
        private Slowa s;

        public Hashtable getHashTableSlowa()
        {
            return slowa;
        }
        public void zamknietoSlowa()
        {
            s = null;
        }
        // tablica hashujaca dla kazdego slowa zawiera jego ilosc powtorzen
        private void dodajSlowo(string slowo)
        {
            if (slowo.Length == 0)
                return;
            if (slowo[0] == '-')
                return;
            if (slowo[0] == '?')
                return;
            if (slowo[0] == '!')
                return;
            if (slowa.Contains(slowo))
            {
                int old = (int)slowa[slowo];
                slowa[slowo] = old + 1;
            }
            else
            {
                slowa.Add(slowo,1);
            }
        }

        private double getCzasPrzedzialu()
        {

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
            chart1.Series[0].IsVisibleInLegend = false;
            if (comboBox2.SelectedIndex == 3)
            {
                comboBox1.Enabled = false;
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
                chart1.ChartAreas[0].AxisY.Title = "Ilość statusów";
                chart1.ChartAreas[0].AxisX.Title = "Przedziały ilości znaków w poj. statusie";

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
                        chart1.ChartAreas[0].AxisY.Title = "Ilość statusów";
                        chart1.ChartAreas[0].AxisX.Title = "Przedział czasu";
                        // zliczamy statusy - dodaj 1
                        ile++;
                    }
                    else if (comboBox2.SelectedIndex == 1)
                    {
                        chart1.ChartAreas[0].AxisY.Title = "Ilość znaków";
                        chart1.ChartAreas[0].AxisX.Title = "Przedział czasu";
                        // zliczamy znaki - dodaj ilosc znakow i-tego statusu
                        ile += ilosciZnakow[i];
                    }
                    else if (comboBox2.SelectedIndex == 2)
                    {
                        chart1.ChartAreas[0].AxisY.Title = "Ilość słów";
                        chart1.ChartAreas[0].AxisX.Title = "Przedział czasu";
                        // zliczamy slowa - dodaj ilosc slow i-tego statusu
                        ile += ilosciSlow[i];
                    }
                }
                if (ile != 0 && px != -1)
                {
                    chart1.Series[0].Points.AddXY(px, ile); //
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

        public string getTrackKeywords()
        {
            return textBox2.Text;
        }
        public void add(status s)
        {
            int ileSlow = policzSlowa(s.text);
            int sekunda = (int)((double)teraz / 1000.0);

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
                ileZnakowMin = ileZnakowMax = s.text.Length; // do histogramu
            }
            else
            {
                if (ileZnakowMax < s.text.Length)
                    ileZnakowMax = s.text.Length;
                if (ileZnakowMin > s.text.Length)
                    ileZnakowMin = s.text.Length;
            }

            sumaDlugosciStatusow += s.text.Length;
            sumaIlosciSlow += ileSlow;
            ilosciSlow.Add(ileSlow);
            ilosciZnakow.Add(s.text.Length);
            teraz = DateTime.Now.Ticks / (decimal)TimeSpan.TicksPerMillisecond; ;
            teraz = teraz - start;
            czasyNadejscia.Add(teraz);
            odswiezWykres();
            tresciStatusow.Add(s.text);
                
            if (!checkBox1.Checked)
            {
                textBox1.AppendText("[Czas nadejścia (sekunda) " + sekunda + "]: " + "Treść statusu " + s.text + Environment.NewLine);
                textBox1.ScrollBars = ScrollBars.Vertical;
            }

            
 
            label3.Text = "Przeanalizowano " + czasyNadejscia.Count + " statusów"
                +
                ". Program działa " + getCzasDzialaniaStr()
                +
                ". Średnia długość statusu " + ((int)getSredniaDlStatusu()).ToString()
                +
                ". Średnia ilość słów statusu " + ((int)getSredniaIloscSlowStatusu()).ToString();

                
        }
        public void CallToChildThread()
        {
            TwitterStream stream = new TwitterStream();
            stream.Stream2Queue(this);
        }
        private int policzSlowa(String text)
        {
            // Usuwamy wszystkie znaki niealfanumeryczne
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            String t2 = rgx.Replace(text, "");
            string [] slowa = t2.Split(' ');
            foreach(string s in slowa)
            {
                dodajSlowo(s);
            }
            return slowa.Length;
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
       
        public static void swtest(double [] w) //test Shapiro-Wilka (test normalności)
        {
            if(w.Length < 4)
            {
                MessageBox.Show("SWTest wymaga przynajmniej 4 próbek");
                return;
            }
            Accord.Statistics.Testing.ShapiroWilkTest t = new Accord.Statistics.Testing.ShapiroWilkTest(w);
            if(t.Significant == true)
            MessageBox.Show("Test Shapiro Wilk, wynik NEGATYWNY (dane nie pochodzą z rozkładu normalnego), wartość 'p' = " + t.PValue+" < 0,05");
            else if(t.Significant == false)
            MessageBox.Show("Test Shapiro Wilk, wynik POZYTYWNY (dane pochodzą z rozkładu normalnego), wartość 'p' = " + t.PValue+ " > 0,05");
        }
        
        private void start_thread()
        {
            textBox2.Enabled = false;
            button3.Enabled = false;
            ThreadStart childref = new ThreadStart(CallToChildThread);
            Thread childThread = new Thread(childref);
            childThread.Start();
        }
    private void Form1_Load(object sender, EventArgs e)
        {

            comboBox1.Items.Add("Co 1 sekunda");
            comboBox1.Items.Add("Co 30 sekund");
            comboBox1.Items.Add("Co 1 minuta");
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (s == null)
                s = new Slowa(this);
            s.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            start_thread();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            odswiezWykres();
        }
    }
}
