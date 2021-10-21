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

        private void btnForward_Click(object sender, EventArgs e)
        {
            var mc = new MoveCommand(Convert.ToInt32(txtDistance.Text), Convert.ToInt32(txtTorqueMove.Text));
            _worker.Publish("movement.distance", mc.ToByte());
        }

        private void btnBackward_Click(object sender, EventArgs e)
        {
            var mc = new MoveCommand(-Convert.ToInt32(txtDistance.Text), Convert.ToInt32(txtTorqueMove.Text));
            _worker.Publish("movement.distance", mc.ToByte());
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            var tc = new TurnCommand(-Convert.ToInt32(txtDegree.Text), Convert.ToInt32(txtTorqueRotate.Text));
            _worker.Publish("movement.turn", tc.ToByte());
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            var tc = new TurnCommand(Convert.ToInt32(txtDegree.Text), Convert.ToInt32(txtTorqueRotate.Text));
            _worker.Publish("movement.turn", tc.ToByte());
        }
    }
}
