/// <reference path="../../references.ts"/>

// TODO: to review
abstract class BaseController {
	baseWebUrl: string;
	baseApiUrl: string;
	currentUser: AuthenticatedUser;

	constructor(protected Restangular: restangular.IService,
		protected GlobalService: tourdefrance.services.IGlobalService,
		protected $state: ng.ui.IStateService,
		protected $mdToast: ng.material.IToastService,
		currentUser: AuthenticatedUser) {

		this.currentUser = currentUser;
	}
}
