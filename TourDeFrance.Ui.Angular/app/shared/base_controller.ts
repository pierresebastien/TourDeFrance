/// <reference path="../../references.ts"/>

// TODO: to review
abstract class BaseController {

	constructor(protected  $mdToast: ng.material.IToastService,
		protected gettextCatalog: angular.gettext.gettextCatalog,
		protected currentUser: AuthenticatedUser) {
	}
}
