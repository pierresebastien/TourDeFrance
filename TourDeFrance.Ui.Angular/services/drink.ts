/// <reference path="../references.ts"/>

module tourdefrance.services {
	'use strict';

	export interface IDrinkService {
		getDrinks(): ng.IPromise<Drink[]>;
		getDrinkById(id: string): ng.IPromise<Drink>;
		// TODO: create type for subDrink def
		createDrink(name: string, volume: number, alcoholByVolume: number, subDrinks: any): ng.IPromise<Drink>;
		updateDrink(id: string, name: string, volume: number, alcoholByVolume: number, subDrinks: any): ng.IPromise<Drink>;
		deleteDrink(id: string): ng.IPromise<Drink>;
	}

	export class DrinkService implements IDrinkService {
		private baseApiRoute: string;

		constructor(private Restangular: restangular.IService) {
			this.baseApiRoute = "drinks";
		}

		public getDrinks(): ng.IPromise<Drink[]> {
			return this.Restangular.all(this.baseApiRoute).getList<Drink>();
		}

		public getDrinkById(id: string): ng.IPromise<Drink> {
			return this.Restangular.one(this.baseApiRoute, id).get<Drink>();
		}

		public createDrink(name: string, volume: number, alcoholByVolume: number, subDrinks: any): ng.IPromise<Drink> {
			var request = { Name: name, Volume: volume, AlcoholByVolume: alcoholByVolume, SubDrinkDefinitions: subDrinks };
			return this.Restangular.all(this.baseApiRoute).customPOST(request);
		}

		public updateDrink(id: string, name: string, volume: number, alcoholByVolume: number, subDrinks: any): ng.IPromise<Drink> {
			var request = { Name: name, Volume: volume, AlcoholByVolume: alcoholByVolume, SubDrinkDefinitions: subDrinks };
			return this.Restangular.one(this.baseApiRoute, id).customPOST(request);
		}

		public deleteDrink(id: string): ng.IPromise<Drink> {
			return this.Restangular.all(this.baseApiRoute).customDELETE(id);
		}
	}
}