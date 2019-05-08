using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.tod.gui;
using TODGUI = com.tod.gui.GUI;
using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Timeline {

	class GUI : TODGUI {

		public ManagedTextBox log;
		public ManagedTextBox streamLog;
        public Image sourceImage;
		public Image faceRecognition;
		public Image candidateImage;
		public Image debugPreview;

		public ImageGallery processGallery;

		private Dictionary<IImage, ImageGallery.Item> m_FaceEntries;

		public GUI() {

			Instance = this;
			m_FaceEntries = new Dictionary<IImage, ImageGallery.Item>();
		}

		public override void Silent(string message) {
			Console.WriteLine(message);
		}

		public override void Write(string message) {
			Console.WriteLine(message);
			log.AppendText(string.Format("{0}{1}", message, Environment.NewLine));
		}

		public override void Notification(string message) {
			Console.WriteLine(message);
			MessageBox.Show(message);
			log.AppendText(string.Format("{0}{1}", message, Environment.NewLine));
		}

		public override void Exception(string message) {
			Notification(message);
		}

        public override void Stream(string message) {
            Console.WriteLine(message);
            streamLog.AppendText(string.Format("{0}{1}", message, Environment.NewLine));
        }

        public void SetProcessImage(IImage image) {

			ImageGallery.Item item;
			if (m_FaceEntries.TryGetValue(image, out item) == false) {
                try
                {
                    item = processGallery.AddImage(image);
                    m_FaceEntries.Add(image, item);
                }
                catch (Exception ex) { }
			}
			else {
				item.control.Source = image;
			}
		}
	}

	class ManagedTextBox : TextBox {

		private delegate void AppendTextCallback(TextBox box, string text);

		public TextBox box;

		public ManagedTextBox(TextBox box) {
			this.box = box;
		}

		public new void AppendText(string text) {
			try {
				if (box.InvokeRequired)
					box.FindForm().Invoke(new AppendTextCallback(AppendText), box, text);
				else
					AppendText(box, text);
			}
			catch (ObjectDisposedException) { }
		}

		private static void AppendText(TextBox box, string text) {
			box.AppendText(text);
		}
	}

	class Image : IGUIImage {

		private delegate void SetSourceCallback(ImageBox container, IImage mat);

		public ImageBox container;

		public Image(ImageBox container) {
			this.container = container;
		}

		public IImage Source {
			set {
				try {
					if (container.InvokeRequired) 
						container.FindForm().Invoke(new SetSourceCallback(SetSource), container, value);
					else
						SetSource(container, value);
				}
				catch (ObjectDisposedException) { }
				catch (System.ComponentModel.InvalidAsynchronousStateException) { }
			}
		}

		public Image Clone() {

			Image clone = new Image(new ImageBox());
			clone.container.Width = container.Width;
			clone.container.Height = container.Height;
			clone.container.BorderStyle = container.BorderStyle;
			clone.container.Dock = container.Dock;
			clone.container.Anchor = container.Anchor;
			clone.container.FunctionalMode = container.FunctionalMode;
			clone.container.SizeMode = container.SizeMode;
			clone.container.InterpolationMode = container.InterpolationMode;

			return clone;
		}

		private static void SetSource(ImageBox container, IImage image) {
			container.Image = image;
		}
	}

	class ImageGallery {

		private delegate void AddImageCallback(FlowLayoutPanel panel, Item item);

		public class Item {

			public Image control;

			public Item(Image template) {

				control = template.Clone();
				control.container.Visible = true;
				control.container.Show();
			}

			public void Update(IImage image) {
				control.Source = image;
			}
		}

		public Image template;
		public FlowLayoutPanel panel;

		public ImageGallery(FlowLayoutPanel panel, Image template) {
			this.panel = panel;
			//this.panel.HorizontalScroll.Enabled = false;
			//this.panel.HorizontalScroll.Visible = false;
			this.template = template;
		}

		public Item AddImage(IImage image) {

			Item item = new Item(template);
			item.control.Source = image;

			try {
				if (panel.InvokeRequired)
					panel.FindForm().Invoke(new AddImageCallback(AddImage), panel, item);
				else
					AddImage(panel, item);
			}
			catch (ObjectDisposedException) { }

			return item;
		}

		public static void AddImage(FlowLayoutPanel panel, Item item) {
			panel.Controls.Add(item.control.container);
			item.control.container.BringToFront();
		}
	}
}
