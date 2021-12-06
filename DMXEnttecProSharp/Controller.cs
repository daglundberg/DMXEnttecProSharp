using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace DMXEnttecProSharp
{
    /// <summary>
    /// Controller maintains a state and interface for interacting with the Enttec
	/// DMX USB Pro.
	/// 
	/// Key methods include:
	///   `set_channel(channel, value)` - Sets channel to value
	///   `submit()` - Send state to device
	///   `Close()` - Close serial connection to device
	/// 
	/// Convenience methods:
	///   `clear_channels()` - Sets all channels to 0
	///   `all_channels_on()` -  Sets all channels to 255
	///   `set_all_channels(value)` - Sets all channels to value
	/// 
	/// Automatic submission of state changes configurable with `auto_submit`
	/// argument.Usage of `submit_after` argument in state-changing methods takes
	/// precedence over this default.
	/// </summary>
	public class Controller
	{
		public string Port;
		public int DmxSize;
		public int Baudrate;
		public int Timeout;
		public bool AutoSubmit;
		public byte[] Channels;
		private byte[] _signalStart, _signalEnd;

		static SerialPort _serialPort;

		/// <summary>Instantiate Controller.</summary>
		/// <param name="port">COM port to use for communication.</param>
		/// <param name="dmxSize">Number of channels from 24 to 512.</param>
		/// <param name="baudrate">Baudrate for serial connection.</param>
		/// <param name="timeout">Serial connection timeout.</param>
		/// <param name="autoSubmit">Enable or disable default automatic submission.</param>
		public Controller(string port, int dmxSize = 512, int baudrate = 57600, int timeout = 500, bool autoSubmit = false)
		{
			String[] PortNames = SerialPort.GetPortNames();

			Console.WriteLine("Available Ports:");
			foreach (string s in PortNames)
			{
				Console.WriteLine("   {0}", s);
			}

			Port = port;
			DmxSize = dmxSize;
			Baudrate = baudrate;
			Timeout = timeout;
			AutoSubmit = autoSubmit;

			if (DmxSize > 512 || DmxSize < 24)
			{
				Console.WriteLine("Size of DMX channel frame must be between 24 and 512! Defaulting to 512.");
				DmxSize = 512;
			}

			// Create a new SerialPort object with default settings.
			_serialPort = new SerialPort(port, baudrate);
			_serialPort.Handshake = Handshake.XOnXOff;

			// Set the read/write timeouts
			_serialPort.ReadTimeout = timeout;
			_serialPort.WriteTimeout = timeout;

			_serialPort.Open();

			Channels = new byte[DmxSize];
			_signalStart = new byte[] { 0x7E };
			_signalEnd = new byte[] { 0xE7 };
		}

		public void SetChannel(int channel, byte value)
		{
			Channels[channel] = value;
		}

		/// <summary>
		/// Sets all channels to 0.
		/// </summary>
		public void ClearChannels()
		{
			Array.Clear(Channels, 0, Channels.Length);
		}

		/// <summary>
		/// Sets all channels to 255.
		/// </summary>
		public void AllChannelsOn()
		{
			Array.Fill(Channels, (byte)255);
		}

		/// <summary>
		/// Sets all channels to a specific value.
		/// </summary>
		public void SetAllChannels(byte value)
		{
			Array.Fill(Channels, value);
		}

		public void Close()
		{
			_serialPort.Close();
		}

		public void Submit()
		{
			List<byte> message = new List<byte>();

			message.AddRange(_signalStart);
			message.AddRange(new byte[]
			{
				6,
				(byte)((Channels.Length+1) & 0xFF),
				(byte)(((Channels.Length+1) >> 8) & 0xFF),
				0,
			});

			message.AddRange(Channels);
			message.AddRange(_signalEnd);

			_serialPort.Write(message.ToArray(), 0, message.Count);
		}

	}
}
