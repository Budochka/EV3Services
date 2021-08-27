using System;
using Gtk;
using EV3UI;


namespace EV3UI
{
    partial class MainWindow : Gtk.Window
    {
        public MainWindow(NLog.Logger log, Config cfg) : base(Gtk.WindowType.Toplevel)
        {
            Build();
            _worker = new Worker(log, cfg);
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }

        protected void SayItButtonClicked(object sender, EventArgs e)
        {
        }

        protected void RightButtonClicked(object sender, EventArgs e)
        {
        }

        protected void LeftButtonClicked(object sender, EventArgs e)
        {
        }

        protected void DownButtonClicked(object sender, EventArgs e)
        {
        }

        protected void UpButtonClicked(object sender, EventArgs e)
        {
        }

        Worker _worker;
    }
}