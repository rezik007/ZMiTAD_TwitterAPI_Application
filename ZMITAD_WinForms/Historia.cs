using System;
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
    public partial class Historia : Form
    {
        private Form1 f;

        public Historia(Form1 f1)
        {
            this.f = f1;
            InitializeComponent();
        }
        private void odswiezListe()
        {
            listView1.Items.Clear();

            for (int i = 0; i < f.getIleStatusow(); i++)
            {
                double d = (double)f.getCzasStatusu(i);
                int czasSekund = (int)(d / 1000.0);
                ListViewItem it = new ListViewItem(i.ToString());
                it.SubItems.Add(czasSekund.ToString());
                it.SubItems.Add(f.getIloscZnakow(i).ToString());
                it.SubItems.Add(f.getIloscSlow(i).ToString());
                listView1.Items.Add(it);
            }
        }
        private void Historia_Load(object sender, EventArgs e)
        {
            odswiezListe();
        }

        private void Historia_FormClosed(object sender, FormClosedEventArgs e)
        {
            f.zamknietoHistorie();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            odswiezListe();
        }
    }
}
