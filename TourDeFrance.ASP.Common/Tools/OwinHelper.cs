using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Integration.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleStack.Orm;
using SimpleStack.Orm.PostgreSQL;
using TourDeFrance.Core;
using Autofac.Integration.WebApi;
using Microsoft.Owin.Security.Cookies;
using Owin;
using TourDeFrance.ASP.Common.Providers;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.ASP.Common.Tools
{
	public class OwinHelper
	{
		protected static readonly Assembly Assembly = Assembly.GetAssembly(typeof (OwinHelper));

		protected readonly IAppBuilder App;

		public static Setup ApplicationSetup;

		public OwinHelper(IAppBuilder app)
		{
			App = app;
		}

		public virtual void Inititialize(ApplicationConfig config)
		{
			#if DEBUG
			HostingEnvironment.RegisterVirtualPathProvider(new DebugVirtualPathProvider());
			#endif

			IDialectProvider dialectProvider;
			switch (config.DatabaseType)
			{
				case DatabaseType.PostgreSQL:
					dialectProvider = new PostgreSQLDialectProvider();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Assembly applicationAssembly = GetApplicationAssembly();
			ContainerBuilder builder = new ContainerBuilder();
			builder.RegisterControllers(applicationAssembly);
			if (Assembly != applicationAssembly)
			{
				IContainer container = builder.Build();
				builder = new ContainerBuilder();

				IList<IComponentRegistration> components =
					container.ComponentRegistry.Registrations.Where(cr => cr.Activator.LimitType != typeof(LifetimeScope)).ToList();
				foreach (var component in components)
				{
					builder.RegisterComponent(component);
				}

				//register the mvc controller of the TourDFrance.ASP.Common project only if there isn't a controller with the same name already registered
				builder.RegisterControllers(Assembly)
					.Where(x => {
						return components.FirstOrDefault(y => y.Activator.LimitType.Name == x.Name) == null;
					});

				builder.RegisterApiControllers(Assembly);
			}
			builder.RegisterApiControllers(applicationAssembly);

			Setup setup = new Setup(dialectProvider);
			setup.Initialize(config, builder);
			ApplicationSetup = setup;

			App.UseAutofacMiddleware(ApplicationSetup.Container);
		}

		protected virtual Assembly GetApplicationAssembly()
		{
			return Assembly;
		}

		public virtual void RegisterAuthentication()
		{
			App.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = "Cookies",
			});

			// TODO: openid, google, ... ?
			// !!! tour de france membership provider
		}

		public virtual void RegisterMvcApplication()
		{
			// Remove WebFormViewEngine
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new RazorViewEngine());

			MvcHandler.DisableMvcResponseHeader = true;
			AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;

			RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			RouteTable.Routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional},
				namespaces: GetPrioritizedNamespaces()
				);

			DependencyResolver.SetResolver(new AutofacDependencyResolver(ApplicationSetup.Container));
			App.UseAutofacMvc();
		}

		// TODO: check usage ?? -> if register all controllers, even overriden???
		protected virtual string[] GetPrioritizedNamespaces()
		{
			return new string[] { };
		}

		public virtual void RegisterWebApiApplication()
		{
			HttpConfiguration config = new HttpConfiguration();

			config.MapHttpAttributeRoutes();
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new {id = RouteParameter.Optional}
				);

			// Force JSON to return object in camelCase
			var settings = config.Formatters.JsonFormatter.SerializerSettings;
			settings.Formatting = Formatting.Indented;
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			config.DependencyResolver = new AutofacWebApiDependencyResolver(ApplicationSetup.Container);
			App.UseAutofacWebApi(config);
			App.UseWebApi(config);
		}
	}
}