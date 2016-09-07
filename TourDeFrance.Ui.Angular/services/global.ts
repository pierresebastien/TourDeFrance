/// <reference path="../references.ts"/>

module tourdefrance.services {
	'use strict';

	export interface IGlobalService {
		accessedState: any;
		errorMessage: string;
		previousState: ng.ui.IState;
		setError(message: string, accessedPage?: any);

		getRealUser(): AuthenticatedUser;
		getCurrentUser(): ng.IPromise<AuthenticatedUser>;

		connectAs(userId: string);
		logBack();
	}

	export class GlobalService implements IGlobalService {
		private currentUser: AuthenticatedUser;
		private realUser: AuthenticatedUser;
		accessedState: any;
		errorMessage: string;
		previousState: ng.ui.IState;

		constructor(private Restangular: restangular.IService,
			private $q: ng.IQService,
			private $stateParams: ng.ui.IStateParamsService,
			private $rootScope: ng.IRootScopeService,
			private $state: ng.ui.IStateService,
			private $window: Window) {
		}

		public setError(message: string, accessedState?: any) {
			this.accessedState = accessedState;
			this.errorMessage = message;
		}

		public connectAs(userId: string) {
			this.Restangular.one('users', userId)
				.one('connect')
				.put()
				.then((newUser: AuthenticatedUser) => {
					// TODO:
					// var url: any = GlobalService.getWebUrl() + '#' + this.$state.get('root.home').url;
					// this.$window.location = url;
					// this.$window.location.reload();
				});
		}

		public logBack() {
			this.connectAs(this.realUser.id);
		}

		public getCurrentUser(): ng.IPromise<AuthenticatedUser> {
			var deferred = this.$q.defer();

			if (this.currentUser != null) {
				deferred.resolve(this.currentUser);
			} else {
				this.initialize(deferred);
			}

			return deferred.promise;
		}

		public getRealUser(): AuthenticatedUser {
			return this.realUser;
		}

		private initialize(deferred: ng.IDeferred<AuthenticatedUser>): void {
			var requests: ng.IPromise<void>[] = [];

			requests.push(this.Restangular
				.all("users")
				.one("me")
				.get<AuthenticatedUser>()
				.then((x: any) => this.currentUser = x));
			requests.push(this.Restangular.all("users")
				.one("realme")
				.get<AuthenticatedUser>()
				.then((x: any) => {
					if (x != null) {
						this.realUser = x;
					}
				}));

			this.$q.all(requests)
				.then(() => {
					deferred.resolve(this.currentUser);
				});
		}
	}
}