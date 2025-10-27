using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace EV3UIWF
{
    public partial class FrmMain : Form
    {
        private readonly Worker _worker;
        private readonly List<Plugin> _listPlgugins = new();
        private NLog.Logger _logs;

        public FrmMain(NLog.Logger log, Config cfg)
        {
            InitializeComponent();

            _logs = log;

            _worker = new Worker(log, cfg);
            
            // Initialize async in the Load event
            this.Load += async (sender, e) => await InitializeWorkerAsync();

  _worker.Notify += (key, bytes) => ProcessMessage(key, bytes);

            ScanPlugins(cfg.PluginsFolder);
        }

  private async Task InitializeWorkerAsync()
    {
await _worker.InitializeAsync();
     _worker.Start();
     FillPluginsMenu();
 }

        private void FillPluginsMenu()
        {
 foreach (var plugin in _listPlgugins)
   {
              var subMenuItem = new ToolStripMenuItem(plugin.Name, null, OnPluginClick);
            plugin.Data = subMenuItem;
          pluginsToolStripMenuItem.DropDownItems.Add(subMenuItem);
            }
  }

        private void OnPluginClick(object sender, EventArgs e)
        {
            var item = _listPlgugins.Find(i => i.Data == sender);
          if (item != null)
            {
                ExecutePlugin(item);
            }
        }

        private void ExecutePlugin(Plugin item)
  {
            var start = new ProcessStartInfo
   {
     FileName = "python.exe",
 Arguments = $"\"{item.FullFileName}\"",
     UseShellExecute = false,
          CreateNoWindow = true,
    RedirectStandardOutput = true,
  RedirectStandardError = true
  };

          using var process = Process.Start(start);
            if (process == null) return;

            using var reader = process.StandardOutput;
    var stderr = process.StandardError.ReadToEnd();
       var result = reader.ReadToEnd();
        }

    private void ScanPlugins(string folder)
        {
     IEnumerable<string> files;
   try
            {
       files = Directory.EnumerateFiles(folder, "*.py");
}
          catch (DirectoryNotFoundException e)
{
           _logs.Error(e, "No plugins directory found");
      return;
   }
            foreach (var f in files)
            {
      _listPlgugins.Add(new Plugin(name: Path.GetFileNameWithoutExtension(f), fullFileName: f));
            }
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

        private async void btnSayIt_Click(object sender, System.EventArgs e)
        {
     var text = textToSay.Text.ToCharArray();
        var data = new byte[text.Length * sizeof(char)];
       Buffer.BlockCopy(text, 0, data, 0, text.Length * sizeof(char));

      await _worker.PublishAsync("voice.text", data);
            textToSay.Clear();
        }

   private async void btnForward_Click(object sender, EventArgs e)
{
        var mc = new MoveCommand(Convert.ToInt32(txtDistance.Text), Convert.ToInt32(txtTorqueMove.Text));
       await _worker.PublishAsync("movement.distance", mc.ToByte());
        }

        private async void btnBackward_Click(object sender, EventArgs e)
        {
          var mc = new MoveCommand(-Convert.ToInt32(txtDistance.Text), Convert.ToInt32(txtTorqueMove.Text));
await _worker.PublishAsync("movement.distance", mc.ToByte());
        }

        private async void btnLeft_Click(object sender, EventArgs e)
        {
var tc = new TurnCommand(Convert.ToInt32(txtDegree.Text), -Convert.ToInt32(txtTorqueRotate.Text));
await _worker.PublishAsync("movement.turn", tc.ToByte());
        }

        private async void btnRight_Click(object sender, EventArgs e)
        {
       var tc = new TurnCommand(Convert.ToInt32(txtDegree.Text), Convert.ToInt32(txtTorqueRotate.Text));
            await _worker.PublishAsync("movement.turn", tc.ToByte());
        }

      private void FrmMain_Load(object sender, EventArgs e)
     {
       // Worker initialization moved to async handler in constructor
        }

        private async void btnHeadLeft_Click(object sender, EventArgs e)
      {
            var htc = new HeadTurnCommand(Convert.ToInt32(txtDegreeHead.Text), Convert.ToInt32(txtTorqueHead.Text));
    await _worker.PublishAsync("movement.headturn", htc.ToByte());
  }

        private async void btnHeadRight_Click(object sender, EventArgs e)
        {
         var htc = new HeadTurnCommand(Convert.ToInt32(txtDegreeHead.Text), -Convert.ToInt32(txtTorqueHead.Text));
            await _worker.PublishAsync("movement.headturn", htc.ToByte());
        }

   private async void btnSet_Click(object sender, EventArgs e)
    {
    var mode = cmbMode.SelectedIndex;

            switch (mode)
            {
   case 0: //Direct Control
       await _worker.PublishAsync("state.direct", null);
          break;

     case 1: //Explore
           await _worker.PublishAsync("state.explore", null);
         break;

       case 2: //Greeting
      await _worker.PublishAsync("state.greet", null);
        break;
          }
      }
    }
}
