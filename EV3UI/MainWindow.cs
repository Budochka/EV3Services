﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Gtk;

namespace EV3UI
{
    sealed partial class MainWindow : Gtk.Window
    {
        public MainWindow(NLog.Logger log, Config cfg) : base(Gtk.WindowType.Toplevel)
        {
            Build();
            _worker = new Worker(log, cfg);
            _worker.Initialize();
            _worker.Start();

            _worker.Notify += (key, bytes) => ProcessMessage(key, bytes);
        }


        private static Gdk.Pixbuf ImageToPixbuf(System.Drawing.Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(stream);
                return pixbuf;
            }
        }

        private void ProcessMessage(string key, byte[] data)
        {
            if (key == "images.general")
            {
                using (var ms = new MemoryStream(data))
                {
                    var bmp = new Bitmap(new MemoryStream(data));
                    _image.Pixbuf = ImageToPixbuf(bmp);
                }
            }
        }

        private void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            _worker.Stop();
            Application.Quit();
            a.RetVal = true;
        }

        private void SayItButtonClicked(object sender, EventArgs e)
        {
            var text = _textToSay.Text;
            _textToSay.DeleteText(0, text.Length);
        }

        private void RightButtonClicked(object sender, EventArgs e)
        {
        }

        private void LeftButtonClicked(object sender, EventArgs e)
        {
        }

        private void BackwardButtonClicked(object sender, EventArgs e)
        {
        }

        private void ForwardButtonClicked(object sender, EventArgs e)
        {
        }

        

        private readonly Worker _worker;
    }
}