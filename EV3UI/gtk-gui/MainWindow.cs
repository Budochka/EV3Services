
namespace EV3UI
{
    sealed partial class MainWindow
	{
		private global::Gtk.Table _table2;

		private global::Gtk.Button _backwardButton;

		private global::Gtk.Entry _textToSay;

		private global::Gtk.ScrolledWindow _gtkScrolledWindow;

		private global::Gtk.TextView _textview;

		private global::Gtk.Image _image;

		private global::Gtk.Label _label1;

		private global::Gtk.Button _leftButton;

		private global::Gtk.Button _rightButton;

		private global::Gtk.Button _sayItButton;

		private global::Gtk.Button _forwardButton;

        private void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget MainWindow
			this.Name = "MainWindow";
			this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child MainWindow.Gtk.Container+ContainerChild
			this._table2 = new global::Gtk.Table(((uint)(5)), ((uint)(3)), false);
			this._table2.Name = "table2";
			this._table2.RowSpacing = ((uint)(6));
			this._table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this._backwardButton = new global::Gtk.Button();
			this._backwardButton.CanFocus = true;
			this._backwardButton.Name = "BackwardButton";
			this._backwardButton.UseUnderline = true;
			this._backwardButton.Label = global::Mono.Unix.Catalog.GetString("Backward");
			this._table2.Add(this._backwardButton);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this._table2[this._backwardButton]));
			w1.TopAttach = ((uint)(3));
			w1.BottomAttach = ((uint)(4));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(2));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this._textToSay = new global::Gtk.Entry();
			this._textToSay.CanFocus = true;
			this._textToSay.Name = "TextToSay";
			this._textToSay.IsEditable = true;
			this._textToSay.InvisibleChar = '●';
			this._table2.Add(this._textToSay);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this._table2[this._textToSay]));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this._gtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this._gtkScrolledWindow.Name = "GtkScrolledWindow";
			this._gtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this._textview = new global::Gtk.TextView();
			this._textview.CanFocus = true;
			this._textview.Name = "textview1";
			this._gtkScrolledWindow.Add(this._textview);
			this._table2.Add(this._gtkScrolledWindow);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this._table2[this._gtkScrolledWindow]));
			w4.TopAttach = ((uint)(4));
			w4.BottomAttach = ((uint)(5));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this._image = new global::Gtk.Image();
			this._image.Name = "image3";
			this._table2.Add(this._image);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this._table2[this._image]));
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.YOptions = ((global::Gtk.AttachOptions)(1));
			// Container child table2.Gtk.Table+TableChild
			this._label1 = new global::Gtk.Label();
			this._label1.Name = "label1";
			this._label1.LabelProp = global::Mono.Unix.Catalog.GetString("Text");
			this._table2.Add(this._label1);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this._table2[this._label1]));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this._leftButton = new global::Gtk.Button();
			this._leftButton.CanFocus = true;
			this._leftButton.Name = "LeftButton";
			this._leftButton.UseUnderline = true;
			this._leftButton.Label = global::Mono.Unix.Catalog.GetString("Left");
			this._table2.Add(this._leftButton);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this._table2[this._leftButton]));
			w7.TopAttach = ((uint)(3));
			w7.BottomAttach = ((uint)(4));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this._rightButton = new global::Gtk.Button();
			this._rightButton.CanFocus = true;
			this._rightButton.Name = "RightButton";
			this._rightButton.UseUnderline = true;
			this._rightButton.Label = global::Mono.Unix.Catalog.GetString("Right");
			this._table2.Add(this._rightButton);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this._table2[this._rightButton]));
			w8.TopAttach = ((uint)(3));
			w8.BottomAttach = ((uint)(4));
			w8.LeftAttach = ((uint)(2));
			w8.RightAttach = ((uint)(3));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this._sayItButton = new global::Gtk.Button();
			this._sayItButton.CanFocus = true;
			this._sayItButton.Name = "SayItButton";
			this._sayItButton.UseUnderline = true;
			this._sayItButton.Label = global::Mono.Unix.Catalog.GetString("Say it!");
			this._table2.Add(this._sayItButton);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this._table2[this._sayItButton]));
			w9.LeftAttach = ((uint)(2));
			w9.RightAttach = ((uint)(3));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this._forwardButton = new global::Gtk.Button();
			this._forwardButton.CanFocus = true;
			this._forwardButton.Name = "ForwardButton";
			this._forwardButton.UseUnderline = true;
			this._forwardButton.Label = global::Mono.Unix.Catalog.GetString("Forward");
			this._table2.Add(this._forwardButton);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this._table2[this._forwardButton]));
			w10.TopAttach = ((uint)(2));
			w10.BottomAttach = ((uint)(3));
			w10.LeftAttach = ((uint)(1));
			w10.RightAttach = ((uint)(2));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			this.Add(this._table2);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 650;
			this.DefaultHeight = 502;
			this.Show();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
			this._forwardButton.Clicked += new global::System.EventHandler(this.ForwardButtonClicked);
			this._sayItButton.Clicked += new global::System.EventHandler(this.SayItButtonClicked);
			this._rightButton.Clicked += new global::System.EventHandler(this.RightButtonClicked);
			this._leftButton.Clicked += new global::System.EventHandler(this.LeftButtonClicked);
			this._backwardButton.Clicked += new global::System.EventHandler(this.BackwardButtonClicked);
		}
	}
}