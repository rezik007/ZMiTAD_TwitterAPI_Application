using FinMath.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZMITAD_WinForms
{
    public partial class Slowa : Form
    {
        private Form1 f;

        public Slowa(Form1 f1)
        {
            this.f = f1;
            InitializeComponent();
        }
        private void Slowa_Load(object sender, EventArgs e)
        {
            odswiezSlowa();
        }

        private void Slowa_FormClosed(object sender, FormClosedEventArgs e)
        {
            f.zamknietoSlowa();
        }
        private void odswiezSlowa()
        {
            listView1.Items.Clear();
            Hashtable slowa = f.getHashTableSlowa();
            List<DictionaryEntry> list = slowa.Cast<DictionaryEntry>().OrderByDescending(entry => entry.Value).ToList();
            foreach(DictionaryEntry de in list)
            {
                ListViewItem it = new ListViewItem();
                it.Text = de.Key.ToString();
                it.SubItems.Add(de.Value.ToString());
                listView1.Items.Add(it);
            }
           
        }
        private void button1_Click(object sender, EventArgs e)
        {
            odswiezSlowa();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                odswiezSlowa();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<double> tab = new List<double>();
            Hashtable slowa = f.getHashTableSlowa();
            List<DictionaryEntry> list = slowa.Cast<DictionaryEntry>().OrderByDescending(entry => entry.Value).ToList();
            foreach (DictionaryEntry de in list)
            {
                int wartosc = (int)de.Value;
                tab.Add(wartosc);
            }
            double[] ar = tab.ToArray();
            Vector dane = new Vector(ar);
            Form1.ttest(dane);
        }
    }
}
