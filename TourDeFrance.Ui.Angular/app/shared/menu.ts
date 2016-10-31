/// <reference path="../../references.ts"/>

class MenuController extends BaseController {
	sections: tourdefrance.components.Section[];

	constructor($mdToast: ng.material.IToastService,
		gettextCatalog: angular.gettext.gettextCatalog,
		currentUser: AuthenticatedUser) {

		super($mdToast, gettextCatalog, currentUser);

		this.sections = [
			{
				name: "Home",
				isOpen: true,
				pages: [
					{ name: this.gettextCatalog.getString("Games"), icon: "bicycle", route: "root.games", isSelected: true },
					{ name: this.gettextCatalog.getString("History"), icon: "trophy", route: "root.history", isSelected: false }
				]
			},
			{
				name: "Management",
				isOpen: false,
				pages: [
					{ name: this.gettextCatalog.getString("Drinks"), icon: "beer", route: "root.management.drinks", isSelected: false },
					{ name: this.gettextCatalog.getString("Stages"), icon: "map-marker", route: "root.management.stages", isSelected: false },
					{ name: this.gettextCatalog.getString("Races"), icon: "map-o", route: "root.management.races", isSelected: false },
					{ name: this.gettextCatalog.getString("Riders"), icon: "user", route: "root.management.riders", isSelected: false },
					{ name: this.gettextCatalog.getString("Teams"), icon: "users", route: "root.management.teams", isSelected: false }
				]
			},
			{
				name: "Administration",
				isOpen: false,
				pages: [
					{ name: this.gettextCatalog.getString("Users"), icon: "user", route: "root.admin.users", isSelected: false },
					{ name: this.gettextCatalog.getString("Config"), icon: "wrench", route: "root.admin.config", isSelected: false }
				]
			}
		];
	}
}