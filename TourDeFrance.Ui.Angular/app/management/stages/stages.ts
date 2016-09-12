﻿﻿/// <reference path="../../../references.ts"/>

class StageController extends BaseController {
	constructor(Restangular: restangular.IService,
		GlobalService: tourdefrance.services.IGlobalService,
		$state: ng.ui.IStateService,
		$mdToast: ng.material.IToastService,
		currentUser: AuthenticatedUser) {

		super(Restangular, GlobalService, $state, $mdToast, currentUser);
	}
}