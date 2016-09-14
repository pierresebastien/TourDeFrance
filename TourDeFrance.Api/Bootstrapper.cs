using Autofac;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Responses.Negotiation;
using Nancy.Security;
using TourDeFrance.Api.Tools;
using TourDeFrance.Core;
using TourDeFrance.Core.Logging;

namespace TourDeFrance.Api
{
	public class Bootstrapper : AutofacNancyBootstrapper
	{
		private readonly Setup _setup;

		private static readonly ILog Logger = LogProvider.For<Bootstrapper>();

		public Bootstrapper(Setup setup)
		{
			_setup = setup;
		}

		protected override NancyInternalConfiguration InternalConfiguration
		{
			get
			{
				// TODO: to check
				var processors = new[]
				{
					typeof(JsonProcessor),
					typeof(ResponseProcessor)
				};

				return NancyInternalConfiguration.WithOverrides(x =>
				{
					x.ResponseProcessors = processors;
					x.Serializers.Insert(0, typeof(JsonNetSerializer));
				});
			}
		}

		protected override ILifetimeScope GetApplicationContainer()
		{
			return _setup.Container;
		}

		// TODO: to check + some config keys ???
		protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
		{
			Logger.Debug($"RequestStartup: {context.Request.Url}");
			//SSL Behind Proxy
			SSLProxy.RewriteSchemeUsingForwardedHeaders(pipelines);

			// TODO: not the best place to do this => better in session start equivalent (in ASP)
			if (!Context.IsContextInitialized)
			{
				_setup.InitializeContext();
			}

			pipelines.AfterRequest += ctx =>
			{
				ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
				ctx.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, Content-Type, Accept");
				ctx.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,DELETE,OPTIONS");
			};
		}
	}
}
