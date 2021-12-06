using System;
using DMXEnttecProSharp;

namespace Experiments
{
	class Program
	{
		static void Main(string[] args)
		{
			Controller controller = new Controller("COM3");

			controller.ClearChannels();
			controller.SetChannel(2, 55);
			controller.SetChannel(5, 55);
			controller.Submit();

			Console.WriteLine("Hello World!");
			Console.ReadLine();
		}
	}
}
