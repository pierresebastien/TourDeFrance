using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Interfaces;

namespace TourDeFrance.Core.Tools
{
	public class EventDispatcher : IInitializable
	{
		private IList<IEventListener> _listeners; 

		public void DispatchEvent(IEvent ev)
		{
			foreach (IEventListener listener in _listeners)
			{
				Type eventType = ev.GetType();
				if (eventType.ImplementsInterface(listener.EventType) || eventType.IsSameOrSubclass(listener.EventType))
				{
					listener.OnEvent(ev);
				}
			}
		}

		public int Order => 10;

		public void Initialize()
		{
			_listeners = Context.Current.Container.Resolve<IEnumerable<IEventListener>>().ToList();
		}
	}
}
