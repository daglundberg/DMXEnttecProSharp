using System;
using DMXEnttecProSharp;

namespace Experiments
{
	class Program
	{
		static void Main(string[] args)
		{
			Controller controller = new Controller("COM3");

			controller.AllChannelsOn();
			controller.Submit();


			Console.WriteLine("Hello World!");
			Console.ReadLine();
		}
	}
}
