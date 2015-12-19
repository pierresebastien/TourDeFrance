using System;

namespace TourDeFrance.Core.Interfaces
{
	public interface IEventListener
	{
		void OnEvent(IEvent ev);

		Type EventType { get; } 
	}
}
