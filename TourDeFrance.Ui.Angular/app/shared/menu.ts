/// <reference path="../../references.ts"/>

class MenuController extends BaseController {
	constructor(Restangular: restangular.IService,
		GlobalService: tourdefrance.services.IGlobalService,
		$state: ng.ui.IStateService,
		$mdToast: ng.material.IToastService,
		protected $rootScope: ng.IRootScopeService,
		protected $scope: ng.IScope,
		currentUser: AuthenticatedUser) {

		super(Restangular, GlobalService, $state, $mdToast, currentUser);
	}
}