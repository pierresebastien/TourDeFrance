/// <reference path="../references.ts"/>

module tourdefrance.services {
	'use strict';

	export interface IGlobalService {
		accessedState: any;
		errorMessage: string;
		previousState: ng.ui.IState;
		setError(message: string, accessedPage?: any);

		getCurrentUser(): ng.IPromise<AuthenticatedUser>;
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

		public getCurrentUser(): ng.IPromise<AuthenticatedUser> {
			var deferred = this.$q.defer();

			if (this.currentUser != null) {
				deferred.resolve(this.currentUser);
			} else {
				this.initialize(deferred);
			}

			return deferred.promise;
		}

		private initialize(deferred: ng.IDeferred<AuthenticatedUser>): void {
			this.Restangular
				.all("users")
				.one("me")
				.get<AuthenticatedUser>()
				.then((x: any) => {
					this.currentUser = x;
					deferred.resolve(this.currentUser);
				});
		}
	}
}