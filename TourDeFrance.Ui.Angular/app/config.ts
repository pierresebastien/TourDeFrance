/// <reference path="../references.ts" />

// Url of API
var CONST_API_URL = location.protocol.concat("//").concat(window.location.hostname).concat("/api");

class GravatarConfig {
	constructor(gravatarServiceProvider: any) {
		gravatarServiceProvider.defaults = { "default": "mm" };
	}
}

class ApiSetUp {
	constructor(RestangularProvider: restangular.IProvider) {
		RestangularProvider.setBaseUrl(CONST_API_URL);
	}
}

class RestangularConfig { // Used in a .run since .config does not allow Services
	constructor(Restangular: restangular.IProvider, $state: ng.ui.IStateService, GlobalService: tourdefrance.services.IGlobalService, $mdToast: ng.material.IToastService) {
		Restangular.setErrorInterceptor((response: restangular.IResponse, deferred: ng.IDeferred<any>) => {
			var currentState = {
				name: $state.current.name,
				params: $state.params,
				url: $state.href($state.current.name, $state.params)
			};
			switch (response.status) {
			case 400: // Bad Request = TourDeFranceException
				//toastOptions.closeButton = true;
				//toastOptions.timeOut = 0;
				//toastOptions.hideDuration = 0;
				//toastOptions.showDuration = 0;
				//toastr.show(response.data, "Bad Request");
				$mdToast.show(
					$mdToast.simple()
					.textContent(response.data)
					.position('top right')
					.hideDelay(3000)
				);
				return true; // The error is not handled and the calling function will execute its error callback
			case 401: // Unauthorized
				$state.go("login");
				return false; // The error is considered as handled and the calling function will do nothing
			case 403: // Forbidden
				GlobalService.setError(response.data, currentState);
				$state.go("root.error.forbidden");
				return false;
			case 404: // Not Found
				GlobalService.setError(response.data, currentState);
				$state.go("root.error.not_found");
				return false;
			case 500: // Internal Server Error
				GlobalService.setError(response.data);
				$state.go("root.error.internal");
				return false;
			case 502: // Bad gateway
			case 503:
				$state.go("api_stopped");
				return false;
			default:
				GlobalService.setError(response.status + " - " + response.data);
				$state.go("root.error.internal");
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

		$urlRouterProvider.otherwise("/login");

		$stateProvider
			.state("login",
			{
				url: "/login",
				views: {
					'@': {
						templateUrl: "app/login/login.tpl.html",
						controller: LoginController,
						controllerAs: "login"
					}
				}
			})
			.state("api_stopped",
			{
				url: "/stopped",
				views: {
					'@': {
						templateUrl: "app/shared/api_stopped.tpl.html"
					}
				}
			})
			.state("root",
			{
				url: "",
				abstract: true, // Cannot access directly to the page, must call a child
				resolve: {
					currentUser: function(GlobalService: tourdefrance.services.IGlobalService) {
						return GlobalService.getCurrentUser();
					}
				},
				views: {
					'header': {
						templateUrl: "app/shared/header.tpl.html",
						controller: HeaderController,
						controllerAs: "header"
					},
					'menu': {
						templateUrl: "app/shared/menu.tpl.html",
						controller: MenuController,
						controllerAs: "menu"
					}
				}
			})
			.state("root.home",
			{
				url: "/home",
				// Default view. Defined here because 'views:' is exclusive with 'templateUrl:' & 'controller:'
				views: {
					// We need to use the absolute path to use the unnamed ui-view from root (even if there's no view)
					'@': {
						templateUrl: "app/home/home.tpl.html",
						controller: HomeController,
						controllerAs: "home"
					}
				}
			})
			.state("root.error",
			{
				url: "/error",
				abstract: true,
				resolve: {
					previousState: ($state: ng.ui.IStateService) => {
						var currentStateData = {
							name: $state.current.name,
							params: $state.params,
							url: $state.href($state.current.name, $state.params)
						};
						return currentStateData;
					}
				}
			})
			.state("root.error.forbidden",
			{
				url: "/forbidden",
				views: {
					'@': {
						templateUrl: "app/shared/forbidden.tpl.html",
						controller: ErrorController,
						controllerAs: "error"
					}
				}
			})
			.state("root.error.not_found",
			{
				url: "/notfound",
				views: {
					'@': {
						templateUrl: "app/shared/not_found.tpl.html",
						controller: ErrorController,
						controllerAs: "error"
					}
				}
			})
			.state("root.error.internal",
			{
				url: "/internal",
				views: {
					'@': {
						templateUrl: "app/shared/error.tpl.html",
						controller: ErrorController,
						controllerAs: "error"
					}
				}
			})
			.state("root.admin",
			{
				url: "/admin",
				abstract: true
			})
			.state("root.admin.config",
			{
				url: "/config",
				views: {
					'@': {
						templateUrl: "app/admin/config/config.tpl.html",
						controller: ConfigController,
						controllerAs: "config"
					}
				}
			})
			.state("root.admin.users",
			{
				url: "/users",
				views: {
					'@': {
						templateUrl: "app/admin/users/users.tpl.html",
						controller: UserController,
						controllerAs: "user"
					}
				}
			})
			.state("root.management",
			{
				url: "/management",
				abstract: true
			})
			.state("root.management.drinks",
			{
				url: "/drinks",
				views: {
					'@': {
						templateUrl: "app/management/drinks/drinks.tpl.html",
						controller: DrinkController,
						controllerAs: "drink"
					}
				}
			})
			.state("root.management.drinks.create",
			{
				url: "/create",
				views: {
					'@': {
						templateUrl: "app/management/drinks/create.tpl.html",
						controller: CreateDrinkController,
						controllerAs: "create"
					}
				}
			})
			.state("root.management.stages",
			{
				url: "/stages",
				views: {
					'@': {
						templateUrl: "app/management/stages/stages.tpl.html",
						controller: StageController,
						controllerAs: "stage"
					}
				}
			})
			.state("root.management.races",
			{
				url: "/races",
				views: {
					'@': {
						templateUrl: "app/management/races/races.tpl.html",
						controller: RaceController,
						controllerAs: "race"
					}
				}
			})
			.state("root.management.riders",
			{
				url: "/riders",
				views: {
					'@': {
						templateUrl: "app/management/riders/riders.tpl.html",
						controller: RiderController,
						controllerAs: "rider"
					}
				}
			})
			.state("root.management.teams",
			{
				url: "/teams",
				views: {
					'@': {
						templateUrl: "app/management/teams/teams.tpl.html",
						controller: TeamController,
						controllerAs: "team"
					}
				}
			});
	}
}

class UiRouterConfig {
	constructor($rootScope: ng.IRootScopeService, usSpinnerService: ISpinnerService) {
		$rootScope.$on("$stateChangeStart",
		() => {
			usSpinnerService.spin("loader-spinner");
		});
		$rootScope.$on("$stateChangeSuccess",
		() => {
			usSpinnerService.stop("loader-spinner");
		});
	}
}

class HtmlTemplateHttpInterceptor {
	version: string;

	constructor() {
		var v: string;
		$.ajax({
				url: CONST_API_URL + "/infos/version",
				method: "GET",
				success(data: Info) {
					v = data.message;
				}
			})
			.done(() => {
				this.version = v;
			});
	}
	
	public request = (config: ng.IRequestConfig) => {
		if (config.method === "GET" && _.endsWith(config.url, ".tpl.html") && config.url.indexOf("app/") >= 0) {
			config.url = config.url + "?v=" + this.version;
		}
		return config;
	}

	public static factory() {
		return new HtmlTemplateHttpInterceptor();
	}
}