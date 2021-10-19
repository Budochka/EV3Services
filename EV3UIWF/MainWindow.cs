using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EV3UIWF
{
    public partial class frmMain : Form
    {
        private readonly Worker _worker;

        public frmMain(NLog.Logger log, Config cfg)
        {
            InitializeComponent();

            _worker = new Worker(log, cfg);
            _worker.Initialize();
            _worker.Start();

            _worker.Notify += (key, bytes) => ProcessMessage(key, bytes);
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            _worker.Stop();
        }
        private void ProcessMessage(string key, byte[] data)
        {
            if (key == "images.general")
            {
                imgCameraView.Image = Image.FromStream(new MemoryStream(data));
            }
        }

        private void btnSayIt_Click(object sender, System.EventArgs e)
        {
            var text = textToSay.Text.ToCharArray();
            var data = new byte[text.Length * sizeof(char)];
            Buffer.BlockCopy(text, 0, data, 0, text.Length * sizeof(char));

            _worker.Publish("voice.text", data);
            textToSay.Clear();
        }
    }
}
