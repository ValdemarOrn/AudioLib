using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AudioLib.UI
{
	public class SplashData
	{
		public string Plugin;
		public string Version;
		public string Developer;
		public string Website;
	}

	public class Splash : Panel
	{
		Bitmap Resource;
		public SplashData Data;

		public Splash(Bitmap resource, SplashData data)
		{
			Data = data;
			Resource = resource;
			ClientSize = new Size(Resource.Width, resource.Height);
			this.Click += Close;

			Controls.Add(new Label()
			{
				Text = "Plugin",
				TextAlign = ContentAlignment.MiddleRight,
				AutoSize = false,
				Width = 110,
				Height = 25,
				Left = 0,
				Top = 20
			});

			Controls.Add(new Label()
			{
				Text = "Version",
				TextAlign = ContentAlignment.MiddleRight,
				AutoSize = false,
				Width = 110,
				Height = 25,
				Left = 0,
				Top = 40
			});


			Controls.Add(new Label()
			{
				Text = "Developer",
				TextAlign = ContentAlignment.MiddleRight,
				AutoSize = false,
				Width = 110,
				Height = 25,
				Left = 0,
				Top = 70
			});

			Controls.Add(new Label()
			{
				Text = "Website",
				TextAlign = ContentAlignment.MiddleRight,
				AutoSize = false,
				Width = 110,
				Height = 25,
				Left = 0,
				Top = 90
			});



			Controls.Add(new Label()
			{
				Text = Data.Plugin,
				TextAlign = ContentAlignment.MiddleLeft,
				AutoSize = false,
				Width = 150,
				Height = 25,
				Left = 135,
				Top = 20
			});

			Controls.Add(new Label()
			{
				Text = Data.Version,
				TextAlign = ContentAlignment.MiddleLeft,
				AutoSize = false,
				Width = 150,
				Height = 25,
				Left = 135,
				Top = 40
			});

			Controls.Add(new Label()
			{
				Text = Data.Developer,
				TextAlign = ContentAlignment.MiddleLeft,
				AutoSize = false,
				Width = 150,
				Height = 25,
				Left = 135,
				Top = 70,
			});

			var website = new Label()
			{
				Text = Data.Website,
				TextAlign = ContentAlignment.MiddleLeft,
				AutoSize = false,
				Width = 150,
				Height = 25,
				Left = 135,
				Top = 90,
				Cursor = Cursors.Hand
			};

			Controls.Add(website);

			website.Click += website_Click;
			website.MouseEnter += (s, e) => { ((Label)s).ForeColor = Color.CornflowerBlue; };
			website.MouseLeave += (s, e) => { ((Label)s).ForeColor = Color.White; };

			foreach(var ctrl in Controls)
			{
				if (!(ctrl is Label))
					continue;

				var label = (Label)ctrl;

				label.BackColor = Color.Transparent;
				label.Font = new Font("Arial", this.Font.Size);
				label.ForeColor = Color.White;

				if (label != website)
					label.Click += Close;
			}
		}

		void Close(object sender, EventArgs e)
		{
			this.Hide(); 
			this.Parent.Controls.Remove(this); 
			this.Dispose();
		}

		void website_Click(object sender, EventArgs e)
		{
			var url = Data.Website;
			if (url.StartsWith("www"))
				url = "http://" + url;

			System.Diagnostics.Process.Start(url);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawImage(Resource, 0, 0, ClientSize.Width, ClientSize.Height);
			base.OnPaint(e);
		}
	}
}
