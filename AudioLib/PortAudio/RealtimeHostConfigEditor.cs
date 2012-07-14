using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AudioLib
{
	
	public partial class RealtimeHostConfigEditor : Form
	{
		private class HostInfo
		{
			public PortAudio.PaHostApiInfo value;

			public override string ToString()
			{
				return value.name;
			}

			public HostInfo(PortAudio.PaHostApiInfo val)
			{
				value = val;
			}
		}
		private class DeviceInfo
		{
			public PortAudio.PaDeviceInfo value;

			public override string ToString()
			{
				return value.name;
			}

			public DeviceInfo(PortAudio.PaDeviceInfo val)
			{
				value = val;
			}
		}

		public RealtimeHostConfigEditor()
		{
			InitializeComponent();
			populateHostAPIs();

			labelASIO.Visible = false;
		}

		public List<PortAudio.PaHostApiInfo> hostInfos = new List<PortAudio.PaHostApiInfo>();
		public List<PortAudio.PaDeviceInfo> deviceInfos = new List<PortAudio.PaDeviceInfo>();

		public void populateHostAPIs()
		{
			int hostCount = PortAudio.Pa_GetHostApiCount();
			for (int i = 0; i < hostCount; i++)
			{
				hostInfos.Add(PortAudio.Pa_GetHostApiInfo(i));
			}

			int deviceCount = PortAudio.Pa_GetDeviceCount();
			for (int i = 0; i < deviceCount; i++)
			{
				deviceInfos.Add(PortAudio.Pa_GetDeviceInfo(i));
			}

			foreach (var info in hostInfos)
			{
				comboBoxHost.Items.Add(info.name);
			}
		}

		public bool OK;

		public int SelectedHostAPI
		{
			get
			{
				for (int i = 0; i < hostInfos.Count; i++)
					if (hostInfos[i].name == comboBoxHost.Text)
						return i;
				return -1;
			}
		}

		public int Samplerate
		{
			get { return Convert.ToInt32(textBoxSamplerate.Text); }
			set { textBoxSamplerate.Text = value.ToString(); }
		}

		public int Latency
		{
			get { return Convert.ToInt32(textBoxLatency.Text); }
			set { textBoxLatency.Text = value.ToString(); }
		}

		public int InputChannels
		{
			get { return Convert.ToInt32(textBoxInputChannels.Text); }
			set { textBoxInputChannels.Text = value.ToString(); }
		}

		public int OutputChannels
		{
			get { return Convert.ToInt32(textBoxOutputChannels.Text); }
			set { textBoxOutputChannels.Text = value.ToString(); }
		}

		public bool isASIO
		{
			get { return comboBoxHost.Text.ToUpper().Contains("ASIO"); }
		}

		public int InputDeviceID
		{
			get
			{
				for (int i = 0; i < deviceInfos.Count; i++)
					if (deviceInfos[i].Equals(((DeviceInfo)comboBoxInput.SelectedItem).value))
						return i;
				return -1;
			}

			set
			{
				var device = deviceInfos[value];
				comboBoxHost.SelectedItem = hostInfos[device.hostApi].name;
				// ... now it loads comboboxinput and output values in event...

				for (int i = 0; i < comboBoxInput.Items.Count; i++)
				{
					if (comboBoxInput.Items[i].ToString() == device.name)
					{
						comboBoxInput.SelectedIndex = i;
						break;
					}
				}
			}
		}

		public int OutputDeviceID
		{
			get
			{
				for (int i = 0; i < deviceInfos.Count; i++)
					if (deviceInfos[i].Equals(((DeviceInfo)comboBoxOutput.SelectedItem).value))
						return i;
				return -1;
			}

			set
			{
				var device = deviceInfos[value];
				comboBoxHost.SelectedItem = hostInfos[device.hostApi].name;
				// ... now it loads comboboxinput and output values in event...

				for (int i = 0; i < comboBoxInput.Items.Count; i++)
				{
					if (comboBoxOutput.Items[i].ToString() == device.name)
					{
						comboBoxOutput.SelectedIndex = i;
						break;
					}
				}
			}
		}

		private void comboBoxHost_SelectedIndexChanged(object sender, EventArgs e)
		{
			var HostDevices = deviceInfos.Where(x => x.hostApi == SelectedHostAPI).ToList();

			comboBoxInput.Items.Clear();
			comboBoxOutput.Items.Clear();

			foreach (var info in HostDevices)
			{
				if(info.maxInputChannels > 0)
					comboBoxInput.Items.Add(new DeviceInfo(info));

				if(info.maxOutputChannels > 0)
					comboBoxOutput.Items.Add(new DeviceInfo(info));
			}

			if (comboBoxInput.Items.Count > 0)
				comboBoxInput.SelectedIndex = 0;

			if (comboBoxOutput.Items.Count > 0)
				comboBoxOutput.SelectedIndex = 0;

			Samplerate = (HostDevices.Count > 0) ? ((int)HostDevices[0].defaultSampleRate) : 44100;
			Latency = (HostDevices.Count > 0) ? ((int)(HostDevices[0].defaultLowOutputLatency * Samplerate)) : 512;
			if (Latency < 64)
				Latency = 64;

			if (isASIO)
				labelASIO.Visible = true;
			else
				labelASIO.Visible = false;
		}

		private void comboBoxInput_SelectedIndexChanged(object sender, EventArgs e)
		{
			InputChannels = (comboBoxInput.SelectedItem != null) ? ((DeviceInfo)comboBoxInput.SelectedItem).value.maxInputChannels : 0;
			if (isASIO)
				comboBoxOutput.SelectedIndex = comboBoxInput.SelectedIndex;
		}

		private void comboBoxOutput_SelectedIndexChanged(object sender, EventArgs e)
		{
			OutputChannels = (comboBoxOutput.SelectedItem != null) ? ((DeviceInfo)comboBoxOutput.SelectedItem).value.maxOutputChannels : 0;
			if (isASIO)
				comboBoxInput.SelectedIndex = comboBoxOutput.SelectedIndex;
		}

		private void labelASIO_Click(object sender, EventArgs e)
		{
			PortAudio.PaAsio_ShowControlPanel(InputDeviceID, this.Handle);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			OK = true;
			this.Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			OK = false;
			this.Close();
		}
	}
}
