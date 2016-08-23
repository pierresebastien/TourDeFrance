/// <reference path="../references.ts" />

module TourDeFrance {
	'use strict';

	angular.module('TourDeFrance',
		[
			'ui.router', 'restangular', 'infinite-scroll', 'ui.gravatar', 'angularSpinner', 'ngMaterial', 'tourdefrance.services'
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