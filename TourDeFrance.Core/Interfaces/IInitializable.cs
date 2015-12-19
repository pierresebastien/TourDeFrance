namespace TourDeFrance.Core.Interfaces
{
	public interface IInitializable
	{
		int Order { get; }

		void Initialize();
	}
}
