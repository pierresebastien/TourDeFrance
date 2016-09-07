﻿/// <reference path="../../references.ts"/>

class HeaderController extends BaseController {
	hasSharedUsers: boolean;

	constructor(Restangular: restangular.IService,
		GlobalService: tourdefrance.services.IGlobalService,
		$state: ng.ui.IStateService,
		$mdToast: ng.material.IToastService,
		currentUser: AuthenticatedUser) {

		super(Restangular, GlobalService, $state, $mdToast, currentUser);
		this.hasSharedUsers = this.currentUser.accessShares.length > 0;
	}
}