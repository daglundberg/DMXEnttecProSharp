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
			Console.WriteLine("Init success.");
			controller.ClearChannels();
			controller.Submit();

			Console.WriteLine("First submit.");

			byte val = 0;
			int count = 0;
			while (count < 5)
			{
				if (val < 200)
					val++;
				else
				{
					val = 0;
					count++;
					Console.WriteLine($"Count {count}.");
				}

				controller.SetColor(0, new Color() { Blue = val, Red = 255, Green = 0, White = 0 });

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
