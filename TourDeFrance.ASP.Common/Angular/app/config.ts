/// <reference path="../references.ts" />

class GravatarConfig {
    constructor(gravatarServiceProvider: any) {
        gravatarServiceProvider.defaults = { "default": "mm" };
    }
}

class ApiSetUp { // Not working if moved into app.ts
    constructor(RestangularProvider: restangular.IProvider) {
        RestangularProvider.setBaseUrl(TourDeFrance.Service.GlobalService.getApiUrl());
    }
}

// TODO: replace toastr by matrial - toast
class RestangularConfig { // Used in a .run since .config does not allow Services
	constructor(Restangular: restangular.IProvider, $state: ng.ui.IStateService, GlobalService: TourDeFrance.Service.IGlobalService, $mdToast: ng.material.IToastService, $location: ng.ILocationService, $window: Window) {
		Restangular.setErrorInterceptor((response: restangular.IResponse, deferred: ng.IDeferred<any>) => {
			var currentState = {
				name: $state.current.name,
				params: $state.params,
				url: $state.href($state.current.name, $state.params)
			};
			switch (response.status) {
			case 400: // Bad Request = TourDeFranceException
				// TODO: check angular toast usage !!!
				//var toastOptions: ng.material.IToastOptions = {};
				//toastOptions.closeButton = true;
				//toastOptions.newestOnTop = true;
				//toastOptions.positionClass = "toast-top-full-width";
				//toastOptions.timeOut = 0;
				//toastOptions.hideDuration = 0;
				//toastOptions.showDuration = 0;
				//$mdToast.options = toastOptions;
				//$mdToast.show(response.data, "Bad Request");
				return true; // The error is not handled and the calling function will execute its error callback
			case 401: // Unauthorized
				var baseInstanceUrl: any = TourDeFrance.Service.GlobalService.getWebUrl();
				$window.location = baseInstanceUrl;
				return false; // The error is considered as handled and the calling function will do nothing
			case 403: // Forbidden
				GlobalService.setError(response.data, currentState);
				$state.go("root.forbidden");
				return false;
			case 404: // Not Found
				GlobalService.setError(response.data, currentState);
				$state.go("root.notfound");
				return false;
			case 500: // Internal Server Error
				GlobalService.setError(response.data);
				$state.go("root.error");
				return false;
			default:
				GlobalService.setError(response.status + " - " + response.data);
				$state.go("root.error");
				return false;
			}
		});

		// Restangular adds the current time to the request to force cache refresh for each API call (needed for IE)
		Restangular.setFullRequestInterceptor((element, operation, what, url, headers, params) => {
			return { headers: headers, params: _.extend(params, { cacheKiller: new Date().getTime() }), element: element };
		});
	}
}

class RouteConfig {
    constructor($stateProvider: ng.ui.IStateProvider, $urlRouterProvider: ng.ui.IUrlRouterProvider) {

        $urlRouterProvider.otherwise("/notfound");

        $stateProvider
            .state('root', {
                url: '',
                abstract: true, // Cannot access directly to the page, must call a child
                resolve: {
                    currentUser: function (GlobalService: TourDeFrance.Service.IGlobalService) {
                        return GlobalService.getCurrentUser();
                    }
                },
                views: {
                    'header': {
                        templateUrl: '/Angular/app/shared/header.tpl.html',
                        controller: HeaderController,
                        controllerAs: 'header'
                    },
                    'menu': {
                        templateUrl: '/Angular/app/shared/menu.tpl.html',
                        controller: MenuController,
                        controllerAs: 'menu'
                    }
                }
            })
            .state('root.home', {
                url: "/home",
                // Default view. Defined here because 'views:' is exclusive with 'templateUrl:' & 'controller:'
                views: {
                    // We need to use the absolute path to use the unnamed ui-view from root (even if there's no view)
                    '@': {
                        templateUrl: "/Angular/app/home/home.tpl.html",
                        controller: HomeController,
                        controllerAs: 'home'
                    }
                }
            })
            .state('root.forbidden', {
                url: "/forbidden",
                views: {
                    '@': {
                        templateUrl: "/Angular/app/shared/forbidden.tpl.html",
                        resolve: {
                            previousState: ($state: ng.ui.IStateService) => {
                                var currentStateData = {
                                    name: $state.current.name,
                                    params: $state.params,
                                    url: $state.href($state.current.name, $state.params)
                                };
                                return currentStateData;
                            }
                        },
                        controller: ErrorController,
                        controllerAs: 'error'
                    }
                }
            })
            .state('root.notfound', {
                url: "/notfound",
                views: {
                    '@': {
                        templateUrl: "/Angular/app/shared/not_found.tpl.html",
                        resolve: {
                            previousState: ($state: ng.ui.IStateService) => {
                                var currentStateData = {
                                    name: $state.current.name,
                                    params: $state.params,
                                    url: $state.href($state.current.name, $state.params)
                                };
                                return currentStateData;
                            }
                        },
                        controller: ErrorController,
                        controllerAs: 'error'
                    }
                }
            })
            .state('root.error', {
                url: "/error",
                views: {
                    '@': {
                        templateUrl: "/Angular/app/shared/error.tpl.html",
                        resolve: {
                            previousState: ($state: ng.ui.IStateService) => {
                                var currentStateData = {
                                    name: $state.current.name,
                                    params: $state.params,
                                    url: $state.href($state.current.name, $state.params)
                                };
                                return currentStateData;
                            }
                        },
                        controller: ErrorController,
                        controllerAs: 'error'
                    }
                }
            });
    }
}

class UiRouterConfig {
    constructor($rootScope: ng.IRootScopeService, usSpinnerService: ISpinnerService) {
        $rootScope.$on('$stateChangeStart', () => {
            usSpinnerService.spin("loader-spinner");
        });
        $rootScope.$on('$stateChangeSuccess', () => {
            usSpinnerService.stop("loader-spinner");
        });
    }
}

class HtmlTemplateHttpInterceptor {
    version: string;

    constructor(applicationVersion: string) {
        this.version = applicationVersion;
    }

    public request = (config: ng.IRequestConfig) => {
        // Checks if it's a template file contained in /Angular/ folder (avoid adding version to a library template) 
        if (config.method === 'GET' && _.endsWith(config.url, '.tpl.html') && config.url.indexOf('/Angular/') >= 0) {
            config.url = config.url + '?v=' + this.version;
        }
        return config;
    }

    public static factory(applicationVersion: string) {
        return new HtmlTemplateHttpInterceptor(applicationVersion);
    }
}