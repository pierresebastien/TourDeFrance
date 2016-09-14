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
		private readonly Config _config;
		private static readonly ILog Logger = LogProvider.For<ApiSelfHostProcess>();
		private IDisposable _webApp;

		public ApiSelfHostProcess(Config config) : base("API Self Host")
		{
			_config = config;
			WatchdogDelay = TimeSpan.MaxValue;
			LoopDelay = TimeSpan.MaxValue;
		}

		public override bool MustRun => !string.IsNullOrWhiteSpace(_config.ApiUri);

		public override void Starting()
		{
			Logger.Debug("Starting api...");

			// TODO: check usage and other configurations
			StaticConfiguration.DisableErrorTraces = _config.DisableTraceInApi;

			StartOptions options = new StartOptions(_config.ApiUri);
			_webApp = WebApp.Start(options, Startup);
			Logger.Info($"Running on {_config.ApiPort}");
		}

		private void Startup(IAppBuilder app)
		{
			app.Map("/api", x =>
			{
				var options = new NancyOptions {Bootstrapper = new Bootstrapper(Setup)};
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
