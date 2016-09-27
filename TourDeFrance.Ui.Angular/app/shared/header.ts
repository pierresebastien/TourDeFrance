﻿/// <reference path="../../references.ts"/>

class HeaderController extends BaseController {
	hasSharedUsers: boolean;

	constructor(Restangular: restangular.IService,
		GlobalService: tourdefrance.services.IGlobalService,
		$state: ng.ui.IStateService,
		$mdToast: ng.material.IToastService,
		gettextCatalog: angular.gettext.gettextCatalog,
		currentUser: AuthenticatedUser) {

		super(Restangular, GlobalService, $state, $mdToast, gettextCatalog, currentUser);
	}
}