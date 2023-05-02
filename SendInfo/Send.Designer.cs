namespace SendInfo
{
    partial class Send
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Send));
            this.lblMinutos = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.BunifuCustomLabel3 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.lblHora = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.lblSegundos = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.PBar = new Bunifu.Framework.UI.BunifuCircleProgressbar();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.BunifuCustomLabel1 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMinutos
            // 
            this.lblMinutos.AutoSize = true;
            this.lblMinutos.Font = new System.Drawing.Font("Century Gothic", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMinutos.ForeColor = System.Drawing.Color.White;
            this.lblMinutos.Location = new System.Drawing.Point(111, 125);
            this.lblMinutos.Name = "lblMinutos";
            this.lblMinutos.Size = new System.Drawing.Size(79, 58);
            this.lblMinutos.TabIndex = 18;
            this.lblMinutos.Text = "00";
            // 
            // BunifuCustomLabel3
            // 
            this.BunifuCustomLabel3.AutoSize = true;
            this.BunifuCustomLabel3.Font = new System.Drawing.Font("Century Gothic", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BunifuCustomLabel3.ForeColor = System.Drawing.Color.White;
            this.BunifuCustomLabel3.Location = new System.Drawing.Point(90, 125);
            this.BunifuCustomLabel3.Name = "BunifuCustomLabel3";
            this.BunifuCustomLabel3.Size = new System.Drawing.Size(38, 58);
            this.BunifuCustomLabel3.TabIndex = 17;
            this.BunifuCustomLabel3.Text = ":";
            // 
            // lblHora
            // 
            this.lblHora.AutoSize = true;
            this.lblHora.Font = new System.Drawing.Font("Century Gothic", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHora.ForeColor = System.Drawing.Color.White;
            this.lblHora.Location = new System.Drawing.Point(26, 125);
            this.lblHora.Name = "lblHora";
            this.lblHora.Size = new System.Drawing.Size(79, 58);
            this.lblHora.TabIndex = 16;
            this.lblHora.Text = "00";
            // 
            // lblSegundos
            // 
            this.lblSegundos.AutoSize = true;
            this.lblSegundos.Font = new System.Drawing.Font("Century Gothic", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSegundos.ForeColor = System.Drawing.Color.Aqua;
            this.lblSegundos.Location = new System.Drawing.Point(84, 75);
            this.lblSegundos.Name = "lblSegundos";
            this.lblSegundos.Size = new System.Drawing.Size(53, 39);
            this.lblSegundos.TabIndex = 15;
            this.lblSegundos.Text = "00";
            // 
            // PBar
            // 
            this.PBar.animated = false;
            this.PBar.animationIterval = 5;
            this.PBar.animationSpeed = 300;
            this.PBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PBar.BackgroundImage")));
            this.PBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F);
            this.PBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PBar.LabelVisible = true;
            this.PBar.LineProgressThickness = 8;
            this.PBar.LineThickness = 15;
            this.PBar.Location = new System.Drawing.Point(0, 43);
            this.PBar.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.PBar.MaxValue = 60;
            this.PBar.Name = "PBar";
            this.PBar.ProgressBackColor = System.Drawing.Color.Gainsboro;
            this.PBar.ProgressColor = System.Drawing.Color.DarkCyan;
            this.PBar.Size = new System.Drawing.Size(215, 215);
            this.PBar.TabIndex = 14;
            this.PBar.Value = 0;
            // 
            // Panel1
            // 
            this.Panel1.BackColor = System.Drawing.Color.Gold;
            this.Panel1.Controls.Add(this.BunifuCustomLabel1);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.Panel1.Location = new System.Drawing.Point(0, 0);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(215, 36);
            this.Panel1.TabIndex = 13;
            // 
            // BunifuCustomLabel1
            // 
            this.BunifuCustomLabel1.AutoSize = true;
            this.BunifuCustomLabel1.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BunifuCustomLabel1.ForeColor = System.Drawing.Color.Black;
            this.BunifuCustomLabel1.Location = new System.Drawing.Point(32, 9);
            this.BunifuCustomLabel1.Name = "BunifuCustomLabel1";
            this.BunifuCustomLabel1.Size = new System.Drawing.Size(146, 20);
            this.BunifuCustomLabel1.TabIndex = 0;
            this.BunifuCustomLabel1.Text = "ENVIO CONDUCES";
            // 
            // bunifuElipse1
            // 
            this.bunifuElipse1.ElipseRadius = 5;
            this.bunifuElipse1.TargetControl = this;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1500;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Send
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(215, 258);
            this.Controls.Add(this.lblMinutos);
            this.Controls.Add(this.BunifuCustomLabel3);
            this.Controls.Add(this.lblHora);
            this.Controls.Add(this.lblSegundos);
            this.Controls.Add(this.PBar);
            this.Controls.Add(this.Panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Send";
            this.Text = "Form1";
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal Bunifu.Framework.UI.BunifuCustomLabel lblMinutos;
        internal Bunifu.Framework.UI.BunifuCustomLabel BunifuCustomLabel3;
        internal Bunifu.Framework.UI.BunifuCustomLabel lblHora;
        internal Bunifu.Framework.UI.BunifuCustomLabel lblSegundos;
        internal Bunifu.Framework.UI.BunifuCircleProgressbar PBar;
        internal System.Windows.Forms.Panel Panel1;
        internal Bunifu.Framework.UI.BunifuCustomLabel BunifuCustomLabel1;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}

