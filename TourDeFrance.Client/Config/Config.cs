using TourDeFrance.Client.Enums;

namespace TourDeFrance.Client.Config
{
	public class Config
	{
		public string Key { get; set; }
		
		public string StringValue { get; set; }

		public object Value { get; set; }
		
		public string DefaultValue { get; set; }
		
		public string DisplayName { get; set; }
		
		public string Description { get; set; }
		
		public string ValidationRegex { get; set; }
		
		public ConfigType Type { get; set; }
		
		public int Order { get; set; }
		
		public bool Dangerous { get; set; }
	}

}
