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

namespace ZMITAD_WinForms
{
    public partial class Form1 : Form, MojDataStore
    {
        decimal start;
        public Form1()
        {
            InitializeComponent();

            start = DateTime.Now.Ticks / (decimal)TimeSpan.TicksPerMillisecond; ;
        }

        private int ilosc = 0;
        private int popMin = -1;
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
            textBox1.Text += "Tresc statusu " + s.text + Environment.NewLine;
            textBox1.AppendText(s.text);
            


            decimal teraz = DateTime.Now.Ticks / (decimal)TimeSpan.TicksPerMillisecond; ;
            teraz = teraz - start;
            double minuta = ((double)teraz / 1000.0) / 10.0;
            int imin = (int)minuta;
            if(popMin == -1)
            {
                //test
                popMin = imin;
                ilosc++;
            }
            else
            {
                if(popMin != imin)
                {
                    chart1.Series[0].Points.AddXY(popMin, ilosc);
                    popMin = imin;
                    ilosc = 0;
                }
                else
                {

                }
                ilosc++;
            }
        }
        public void CallToChildThread()
        {
            TwitterStream stream = new TwitterStream();
            stream.Stream2Queue(this);
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            ThreadStart childref = new ThreadStart(CallToChildThread);
            Thread childThread = new Thread(childref);
            childThread.Start();
        }
    }
}
