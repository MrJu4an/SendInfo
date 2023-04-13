using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SendInfo.Procesos;
namespace SendInfo
{
    public partial class Send : Form
    {
        EnvioProtech envioProtech = new EnvioProtech();
        public Send()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblHora.Text = DateTime.Now.ToString("HH");
            lblMinutos.Text = DateTime.Now.ToString("mm");
            lblSegundos.Text = DateTime.Now.ToString("ss");
            PBar.Value = Convert.ToInt32(DateTime.Now.ToString("ss"));
            envioProtech.consultarTasas();
        }
    }
}
