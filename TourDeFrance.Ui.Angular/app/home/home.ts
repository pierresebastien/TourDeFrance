﻿class HomeController extends BaseController {
	constructor(Restangular: restangular.IService,
		GlobalService: TourDeFrance.Service.IGlobalService,
		$state: ng.ui.IStateService,
		$mdToast: ng.material.IToastService,
		protected $q: ng.IQService,
		currentUser: AuthenticatedUser) {

		super(Restangular, GlobalService, $state, $mdToast, currentUser);
	}
}