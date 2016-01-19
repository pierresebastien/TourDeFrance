using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Web.Hosting;
using ServiceStack.Net30.Collections.Concurrent;
using TourDeFrance.Core;

namespace TourDeFrance.ASP.Common.Providers
{
	public class DebugVirtualPathProvider : VirtualPathProvider
	{
		private const string GitModulePath = ".gitmodules";

		private static readonly IDictionary<string, string> PathMappings;
		private static readonly string ApplicationPath;
		private static readonly string AspCommonPath;

		static DebugVirtualPathProvider()
		{
			ApplicationPath = Path.GetDirectoryName(ApplicationConfig.FromFile().ApplicationPath);
			PathMappings = new ConcurrentDictionary<string, string>();
			AspCommonPath = string.Empty;

			string gitModuleFilePath = Path.Combine(ApplicationPath, "..", GitModulePath);
			if (!File.Exists(gitModuleFilePath))
			{
				AspCommonPath = Path.Combine(ApplicationPath, "..", "TourDeFrance.ASP.Common");
			}
			else
			{
				Regex subModulePathRegex = new Regex("^\\tpath = (?<path>.+)$");
				foreach (var line in File.ReadAllLines(gitModuleFilePath))
				{
					if (subModulePathRegex.IsMatch(line))
					{
						string subModulePath = subModulePathRegex.Match(line).Groups["path"].Value.Trim();
						AspCommonPath = Path.Combine(ApplicationPath, "..", subModulePath, "TourDeFrance.ASP.Common");
						break;
					}
				}
				if (string.IsNullOrEmpty(AspCommonPath))
				{
					throw new Exception("Could not parse sub module path");
				}
			}
		}

		public override bool FileExists(string virtualPath)
		{
			return !string.IsNullOrEmpty(GePhysicalPath(virtualPath));
		}

		public override VirtualFile GetFile(string virtualPath)
		{
			string physicalPath = GePhysicalPath(virtualPath);
			if (!string.IsNullOrEmpty(physicalPath))
			{
				return new TourDeFranceVirtualFile(virtualPath, physicalPath);
			}
			return base.GetFile(virtualPath);
		}

		public override string CombineVirtualPaths(string basePath, string relativePath)
		{
			string physicalPath = GePhysicalPath(relativePath);
			if (!string.IsNullOrEmpty(physicalPath))
			{
				return GePhysicalPath(relativePath);
			}
			return base.CombineVirtualPaths(basePath, relativePath);
		}

		public override string GetCacheKey(string virtualPath)
		{
			string physicalPath = GePhysicalPath(virtualPath);
			if (!string.IsNullOrEmpty(physicalPath))
			{
				return physicalPath.GetHashCode().ToString();
			}
			return base.GetCacheKey(virtualPath);
		}

		public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
		{
			string physicalPath = GePhysicalPath(virtualPath);
			if (!string.IsNullOrEmpty(physicalPath))
			{
				return new CacheDependency(physicalPath, utcStart);
			}
			return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
		}

		public string GePhysicalPath(string virtualPath)
		{
			virtualPath = virtualPath.Replace("~", string.Empty);
			if (PathMappings.ContainsKey(virtualPath))
			{
				return PathMappings[virtualPath];
			}

			string formattedVirtualPath = virtualPath;
			if (formattedVirtualPath.StartsWith("/"))
			{
				formattedVirtualPath = formattedVirtualPath.Substring(1, formattedVirtualPath.Length - 1);
			}
			formattedVirtualPath = formattedVirtualPath.Replace('/', Path.DirectorySeparatorChar);

			string physicalPath = string.Empty;
			string temporaryPath = Path.Combine(ApplicationPath, formattedVirtualPath);
			// Overidded by ASP project
			if (File.Exists(temporaryPath))
			{
				physicalPath = temporaryPath;
			}
			// ASP Common files
			else
			{
				temporaryPath = Path.Combine(ApplicationPath, AspCommonPath, formattedVirtualPath);
				if (File.Exists(temporaryPath))
				{
					physicalPath = temporaryPath;
				}
			}

			PathMappings.Add(virtualPath, physicalPath);
			return physicalPath;
		}
	}

	public class TourDeFranceVirtualFile : VirtualFile
	{
		private readonly string _physicalPath;

		public TourDeFranceVirtualFile(string virtualPath, string physicalPath) : base(virtualPath)
		{
			_physicalPath = physicalPath;
		}

		public override Stream Open()
		{
			return File.OpenRead(_physicalPath);
		}
	}
}