using Autofac;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Responses.Negotiation;
using Nancy.Security;
using TourDeFrance.Api.Tools;
using TourDeFrance.Core.Logging;

namespace TourDeFrance.Api
{
	public class Bootstrapper : AutofacNancyBootstrapper
	{
		private readonly IContainer _container;

		private static readonly ILog Logger = LogProvider.For<Bootstrapper>();

		public Bootstrapper(IContainer container)
		{
			_container = container;
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

		protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
		{
			Logger.Debug("Configuring Application Container");
			ContainerBuilder builder = new ContainerBuilder();
			builder.Update(existingContainer.ComponentRegistry);
			builder.Update(_container);
		}

		// TODO: to check + soem config keys ???
		protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
		{
			Logger.Debug($"RequestStartup: {context.Request.Url}");
			//SSL Behind Proxy
			SSLProxy.RewriteSchemeUsingForwardedHeaders(pipelines);

			pipelines.AfterRequest += ctx =>
			{
				ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
				ctx.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, Content-Type, Accept");
				ctx.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,DELETE,OPTIONS");
			};
		}
	}
}
