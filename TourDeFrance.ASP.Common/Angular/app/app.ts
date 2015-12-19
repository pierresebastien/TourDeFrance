/// <reference path="../references.ts" />
/// <reference path="./config.ts" />

module TourDeFrance {
    'use strict';

    angular.module('TourDeFrance', [
            'ui.router', 'restangular', 'infinite-scroll', 'ui.gravatar', 'angularSpinner', 'ngMaterial', 'tourdefrance.services'
        ])
        .config(RouteConfig)
        .config(ApiSetUp)
        .config(GravatarConfig)
        .run(RestangularConfig)
        .run(UiRouterConfig)
        .controller('HeaderController', HeaderController)
        .controller('MenuController', MenuController)
        .controller('ErrorController', ErrorController)
        .controller('HomeController', HomeController);
}