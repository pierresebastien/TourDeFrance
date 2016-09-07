/// <reference path="../references.ts" />

module tourdefrance {
	'use strict';

	angular.module('tourdefrance',
		[
			'tourdefrance.services', 'ui.router', 'restangular', 'infinite-scroll', 'ui.gravatar', 'angularSpinner', 'ngMaterial', 'gettext'
		])
		.config([
			'$httpProvider', ($httpProvider: ng.IHttpProvider) => {
				$httpProvider.interceptors.push(HtmlTemplateHttpInterceptor.factory);
			}
		])
		.config(RouteConfig)
		.config(ApiSetUp)
		.config(GravatarConfig)
		.run(RestangularConfig)
		.run(UiRouterConfig);
}