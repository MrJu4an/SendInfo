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
using Microsoft.VisualBasic;
namespace SendInfo
{
    public partial class Send : Form
    {
        EnvioProtech envioProtech = new EnvioProtech();
        EnvioRSN envioRSN = new EnvioRSN();
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

            //Creamos la hora
            string hora = Strings.Format(DateTime.Now.ToString("HH:mm"));

            //Si es la hora de cierre se realizará el proceso de envio
            if (hora == "02:00")
            {
                envioRSN.ConsultarCierres();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            envioProtech.consultarTasas();
        }
    }
}
