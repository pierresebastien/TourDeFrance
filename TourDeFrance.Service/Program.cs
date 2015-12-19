using ASK.ServEasy;
using log4net.Config;

namespace TourDeFrance.Service
{
	public class Program
	{
		public static void Main(string[] args)
		{
			XmlConfigurator.Configure();
			Loader.LoadModule(new TourDeFranceModule());
		}
	}
}
