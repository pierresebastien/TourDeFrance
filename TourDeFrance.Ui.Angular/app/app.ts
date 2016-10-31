﻿/// <reference path="../references.ts" />

module tourdefrance {
	'use strict';

	angular.module('tourdefrance',
		[
			'tourdefrance.services', 'tourdefrance.components', 'ui.router', 'restangular', 'infinite-scroll', 'ui.gravatar',
			'angularSpinner', 'ngMaterial', 'ngMessages', 'gettext'
		])
		.config([
			'$httpProvider', ($httpProvider: ng.IHttpProvider) => {
				$httpProvider.interceptors.push(HtmlTemplateHttpInterceptor.factory);
			}
		])
		.config(RouteConfig)
		.config(ApiSetUp)
		.config(GravatarConfig)
		.config(ThemeConfig)
		.run(RestangularConfig)
		.run(UiRouterConfig);
}