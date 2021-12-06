using System;
using System.Threading;
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

			byte val = 0;
			int count = 0;
			while (count < 3)
			{
				if (val < 255)
					val++;
				else
				{
					val = 0;
					count++;
				}
				controller.SetChannel(2, val);

				controller.SetChannel(5, val);
				controller.Submit();
				Thread.Sleep(5);
			}

			controller.ClearChannels();
			controller.Submit();
			controller.Close();

			Console.WriteLine("Press any key to exit...");
			Console.ReadLine();
		}
	}
}
