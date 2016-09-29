/// <reference path="../../references.ts"/>

class MenuController extends BaseController {
	sections: any[];

	constructor($mdToast: ng.material.IToastService,
		gettextCatalog: angular.gettext.gettextCatalog,
		currentUser: AuthenticatedUser) {

		super($mdToast, gettextCatalog, currentUser);

		this.sections = [
			{
				name: "Home",
				subSections: [
					{ name: this.gettextCatalog.getString("Games"), icon: "bicycle", route: "root.games" },
					{ name: this.gettextCatalog.getString("History"), icon: "trophy", route: "root.history" }
				]
			},
			{
				name: "Management",
				subSections: [
					{ name: this.gettextCatalog.getString("Drinks"), icon: "beer", route: "root.management.drinks" },
					{ name: this.gettextCatalog.getString("Stages"), icon: "map-marker", route: "root.management.stages" },
					{ name: this.gettextCatalog.getString("Races"), icon: "map-o", route: "root.management.races" },
					{ name: this.gettextCatalog.getString("Riders"), icon: "user", route: "root.management.riders" },
					{ name: this.gettextCatalog.getString("Teams"), icon: "users", route: "root.management.teams" }
				]
			},
			{
				name: "Administration",
				subSections: [
					{ name: this.gettextCatalog.getString("Users"), icon: "user", route: "root.admin.users" },
					{ name: this.gettextCatalog.getString("Config"), icon: "wrench", route: "root.admin.config" }
				]
			}
		];
	}
}