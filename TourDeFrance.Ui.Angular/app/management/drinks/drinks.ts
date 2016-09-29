﻿/// <reference path="../../../references.ts"/>

class DrinkController extends BaseController {
	drinks: Drink[];

	constructor($mdToast: ng.material.IToastService,
		gettextCatalog: angular.gettext.gettextCatalog,
		currentUser: AuthenticatedUser,
		private DrinkService: tourdefrance.services.IDrinkService) {

		super($mdToast, gettextCatalog, currentUser);

		this.DrinkService.getDrinks()
			.then((results: Drink[]) => {
				this.drinks = results;
			});
	}

	public deleteDrink(id: string) {
		this.DrinkService.deleteDrink(id)
			.then((Drink: Drink) => {
				_.remove(this.drinks, x => x.id === id);
				this.$mdToast.show(
					this.$mdToast.simple()
						.textContent(this.gettextCatalog.getString("Drink deleted"))
						.theme("success-toast")
						.position('top right')
						.hideDelay(3000)
				);
			});
	}
}

class CreateDrinkController extends BaseController {
	drink: Drink;
	drinks: Drink[];
	isComposedDrink: boolean;

	constructor($mdToast: ng.material.IToastService,
		gettextCatalog: angular.gettext.gettextCatalog,
		currentUser: AuthenticatedUser,
		private DrinkService: tourdefrance.services.IDrinkService,
		private $state: ng.ui.IStateService) {

		super($mdToast, gettextCatalog, currentUser);

		this.isComposedDrink = false;
		this.drink = new Drink();

		this.DrinkService.getDrinks()
			.then((results: Drink[]) => {
				this.drinks = results;
			});
	}

	public sendRequest() {
		this.DrinkService.createDrink(this.drink.name, this.drink.volume, this.drink.alcoholByVolume, null).then((drink: Drink) => {
			this.$mdToast.show(
				this.$mdToast.simple()
					.textContent(this.gettextCatalog.getString("Drink created"))
					.theme("success-toast")
					.position('top right')
					.hideDelay(3000)
			);
			this.$state.go("root.management.drinks");
		});
	}
}