using System;
using Microsoft.Owin.Hosting;
using Nancy;
using Nancy.Owin;
using Owin;
using TourDeFrance.Core;
using TourDeFrance.Core.Logging;
using TourDeFrance.Core.Processes;

namespace TourDeFrance.Api.Processes
{
    public class ApiSelfHostProcess : ProcessBase
    {
		private static readonly ILog Logger = LogProvider.For<ApiSelfHostProcess>();
		private IDisposable _webApp;

		public ApiSelfHostProcess() : base("API Self Host")
	    {
			WatchdogDelay = TimeSpan.MaxValue;
		    LoopDelay = TimeSpan.MaxValue;
	    }

		// TODO: or based on config key
	    public override bool MustRun => true;

		public override void Starting()
		{
			Logger.Debug("Starting api...");

			StaticConfiguration.DisableErrorTraces = _config.DisableErrorTraces;

			StartOptions options = new StartOptions(_config.Uri);
			_webApp = WebApp.Start(options, x => Startup(x, _config));
			Logger.Info($"Running on {_config.Uri}");
		}

		private void Startup(IAppBuilder app)
		{
			app.Map("/api", x =>
			{
				var options = new NancyOptions {Bootstrapper = new Bootstrapper(Context.Current.Container)};
				x.UseNancy(options);
			});
		}

		public override void Stopping()
		{
			base.Stopping();
			if (_webApp != null)
			{
				_webApp.Dispose();
				_webApp = null;
			}
		}
	}
}
