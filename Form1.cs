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

namespace DownLoadDemo
{
    public partial class Form1 : Form
    {
        private HttpFileRequest fileRequest = null;
        private delegate void AsynUpdateUI();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fileRequest = new HttpFileRequest();
            fileRequest.MainUpdateProcess += Process;
            fileRequest.MainLoadSuccess += DownLoadSuccess;
            Task task = new Task(DownLoad);
            task.Start();
        }

        public void DownLoad()
        {
            string fileUrl = "d://1.zip";            
            fileRequest.DownLoad("http://192.168.1.107:301/1.zip", fileUrl);
        }

        public void Process(double value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate()
                {
                    progressBar1.Value = Convert.ToInt32(value);
                    label1.Text = progressBar1.Value + @"%";              
                }));
            }
        }

        public void DownLoadSuccess()
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate()
                {
                    progressBar1.Value = 100;
                    label1.Text = @"100%";
                    label2.Text = @"已下载完成";
                }));
            }
        }
    }
}
