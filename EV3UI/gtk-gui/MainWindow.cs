
namespace EV3UI
{
	partial class MainWindow
	{
		private global::Gtk.Table table2;

		private global::Gtk.Button DownButton;

		private global::Gtk.Entry entry1;

		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.TextView textview1;

		private global::Gtk.Image image3;

		private global::Gtk.Label label1;

		private global::Gtk.Button LeftButton;

		private global::Gtk.Button RightButton;

		private global::Gtk.Button SayItButton;

		private global::Gtk.Button UpButton;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget MainWindow
			this.Name = "MainWindow";
			this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child MainWindow.Gtk.Container+ContainerChild
			this.table2 = new global::Gtk.Table(((uint)(5)), ((uint)(3)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.DownButton = new global::Gtk.Button();
			this.DownButton.CanFocus = true;
			this.DownButton.Name = "DownButton";
			this.DownButton.UseUnderline = true;
			this.DownButton.Label = global::Mono.Unix.Catalog.GetString("Down");
			this.table2.Add(this.DownButton);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table2[this.DownButton]));
			w1.TopAttach = ((uint)(3));
			w1.BottomAttach = ((uint)(4));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(2));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entry1 = new global::Gtk.Entry();
			this.entry1.CanFocus = true;
			this.entry1.Name = "entry1";
			this.entry1.IsEditable = true;
			this.entry1.InvisibleChar = '●';
			this.table2.Add(this.entry1);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table2[this.entry1]));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.textview1 = new global::Gtk.TextView();
			this.textview1.CanFocus = true;
			this.textview1.Name = "textview1";
			this.GtkScrolledWindow.Add(this.textview1);
			this.table2.Add(this.GtkScrolledWindow);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table2[this.GtkScrolledWindow]));
			w4.TopAttach = ((uint)(4));
			w4.BottomAttach = ((uint)(5));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.image3 = new global::Gtk.Image();
			this.image3.Name = "image3";
			this.table2.Add(this.image3);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table2[this.image3]));
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.YOptions = ((global::Gtk.AttachOptions)(1));
			// Container child table2.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString("Text");
			this.table2.Add(this.label1);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table2[this.label1]));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.LeftButton = new global::Gtk.Button();
			this.LeftButton.CanFocus = true;
			this.LeftButton.Name = "LeftButton";
			this.LeftButton.UseUnderline = true;
			this.LeftButton.Label = global::Mono.Unix.Catalog.GetString("Left");
			this.table2.Add(this.LeftButton);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table2[this.LeftButton]));
			w7.TopAttach = ((uint)(3));
			w7.BottomAttach = ((uint)(4));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.RightButton = new global::Gtk.Button();
			this.RightButton.CanFocus = true;
			this.RightButton.Name = "RightButton";
			this.RightButton.UseUnderline = true;
			this.RightButton.Label = global::Mono.Unix.Catalog.GetString("Right");
			this.table2.Add(this.RightButton);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table2[this.RightButton]));
			w8.TopAttach = ((uint)(3));
			w8.BottomAttach = ((uint)(4));
			w8.LeftAttach = ((uint)(2));
			w8.RightAttach = ((uint)(3));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.SayItButton = new global::Gtk.Button();
			this.SayItButton.CanFocus = true;
			this.SayItButton.Name = "SayItButton";
			this.SayItButton.UseUnderline = true;
			this.SayItButton.Label = global::Mono.Unix.Catalog.GetString("Say it!");
			this.table2.Add(this.SayItButton);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table2[this.SayItButton]));
			w9.LeftAttach = ((uint)(2));
			w9.RightAttach = ((uint)(3));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.UpButton = new global::Gtk.Button();
			this.UpButton.CanFocus = true;
			this.UpButton.Name = "UpButton";
			this.UpButton.UseUnderline = true;
			this.UpButton.Label = global::Mono.Unix.Catalog.GetString("Up");
			this.table2.Add(this.UpButton);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table2[this.UpButton]));
			w10.TopAttach = ((uint)(2));
			w10.BottomAttach = ((uint)(3));
			w10.LeftAttach = ((uint)(1));
			w10.RightAttach = ((uint)(2));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			this.Add(this.table2);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 650;
			this.DefaultHeight = 502;
			this.Show();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
			this.UpButton.Clicked += new global::System.EventHandler(this.UpButtonClicked);
			this.SayItButton.Clicked += new global::System.EventHandler(this.SayItButtonClicked);
			this.RightButton.Clicked += new global::System.EventHandler(this.RightButtonClicked);
			this.LeftButton.Clicked += new global::System.EventHandler(this.LeftButtonClicked);
			this.DownButton.Clicked += new global::System.EventHandler(this.DownButtonClicked);
		}
	}
}