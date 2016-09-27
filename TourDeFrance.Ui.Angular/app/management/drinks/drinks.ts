﻿/// <reference path="../../../references.ts"/>

class DrinkController extends BaseController {
	drinks: Drink[];

	constructor(Restangular: restangular.IService,
		GlobalService: tourdefrance.services.IGlobalService,
		$state: ng.ui.IStateService,
		$mdToast: ng.material.IToastService,
		gettextCatalog: angular.gettext.gettextCatalog,
		currentUser: AuthenticatedUser) {

		super(Restangular, GlobalService, $state, $mdToast, gettextCatalog, currentUser);

		this.Restangular.all("drinks")
			.getList<Drink>()
			.then((results: Drink[]) => {
				this.drinks = results;
			});
	}
}

class CreateDrinkController extends BaseController {
	drink: Drink;
	drinks: Drink[];
	isComposedDrink: boolean;

	constructor(Restangular: restangular.IService,
		GlobalService: tourdefrance.services.IGlobalService,
		$state: ng.ui.IStateService,
		$mdToast: ng.material.IToastService,
		gettextCatalog: angular.gettext.gettextCatalog,
		currentUser: AuthenticatedUser) {

		super(Restangular, GlobalService, $state, $mdToast, gettextCatalog, currentUser);

		this.isComposedDrink = false;
		this.drink = new Drink();

		this.Restangular.all("drinks")
			.getList<Drink>()
			.then((results: Drink[]) => {
				this.drinks = results;
			});
	}
}