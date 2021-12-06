using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace DMXEnttecProSharp
{
	public struct Color
	{
		public byte Red;
		public byte Green;
		public byte Blue;
		public byte White;
	}

    /// <summary>
    /// Controller maintains a state and interface for interacting with the Enttec
	/// DMX USB Pro.
	/// 
	/// Key methods include:
	///   `SetChannel(channel, value)` - Sets channel to value
	///   `Submit()` - Send state to device
	///   `Close()` - Close serial connection to device
	/// 
	/// Convenience methods:
	///   `ClearChannels()` - Sets all channels to 0
	///   `AllChannels_on()` -  Sets all channels to 255
	///   `SetAllChannels(value)` - Sets all channels to value
	/// 
	/// </summary>
	public class Controller
	{
		public string Port;
		public int DmxSize;
		public int Baudrate;
		public int Timeout;
		public bool AutoSubmit;
		public byte[] Message;

		const byte SignalStart = 0x7E;
		const byte SignalEnd = 0xE7;

		static SerialPort _serialPort;

		/// <summary>Instantiate Controller.</summary>
		/// <param name="port">COM port to use for communication.</param>
		/// <param name="dmxSize">Number of channels from 24 to 512.</param>
		/// <param name="baudrate">Baudrate for serial connection.</param>
		/// <param name="timeout">Serial connection timeout.</param>
		/// <param name="autoSubmit">Enable or disable default automatic submission.</param>
		public Controller(string port, int dmxSize = 48, int baudrate = 57600, int timeout = 1000, bool autoSubmit = false)
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

			PrepareMessage();
		}

		private void PrepareMessage()
		{
			Message = new byte[DmxSize + 6];
			Message[0] = SignalStart;
			Message[1] = 6;
			Message[2] = (byte)((DmxSize + 1) & 0xFF);
			Message[3] = (byte)(((DmxSize + 1) >> 8) & 0xFF);
			Message[4] = 0;
			Message[Message.Length-1] = SignalEnd;
		}

		private Span<byte> Channels
		{
			get
			{
				return new Span<byte>(Message, 5, DmxSize);
			}
		}

		public void SetChannel(int channel, byte value)
		{
			Channels[channel] = value;
		}

		public void SetColor(int redChannel, Color color)
		{
			var span = Channels.Slice(redChannel, 4);

			span[0] = color.Red;
			span[1] = color.Green;
			span[2] = color.Blue;
			span[3] = color.White;
		}

		/// <summary>
		/// Sets all channels to 0.
		/// </summary>
		public void ClearChannels()
		{
			Channels.Clear();			
		}

		/// <summary>
		/// Sets all channels to 255.
		/// </summary>
		public void AllChannelsOn()
		{
			Channels.Fill(255);
		}

		/// <summary>
		/// Sets all channels to a specific value.
		/// </summary>
		public void SetAllChannels(byte value)
		{
			Channels.Fill(value);
		}

		/// <summary>
		/// Close the connection.
		/// </summary>
		public void Close()
		{
			_serialPort.Close();
		}

		/// <summary>
		/// Send the message to the widget.
		/// </summary>
		public void Submit()
		{
			_serialPort.Write(Message, 0, Message.Length);
		}
	}
}
