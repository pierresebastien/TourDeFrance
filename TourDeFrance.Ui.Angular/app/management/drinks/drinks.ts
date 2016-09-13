﻿/// <reference path="../../../references.ts"/>

class DrinkController extends BaseController {
	drinks: Drink[];

	constructor(Restangular: restangular.IService,
		GlobalService: tourdefrance.services.IGlobalService,
		$state: ng.ui.IStateService,
		$mdToast: ng.material.IToastService,
		currentUser: AuthenticatedUser) {

		super(Restangular, GlobalService, $state, $mdToast, currentUser);

		this.Restangular.all("drinks")
			.getList<Drink>()
			.then((results: Drink[]) => {
				this.drinks = results;
			});
	}
}