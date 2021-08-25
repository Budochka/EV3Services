using System;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
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
}
