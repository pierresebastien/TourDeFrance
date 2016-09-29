﻿﻿/// <reference path="../../references.ts"/>

class HomeController extends BaseController {
	constructor($mdToast: ng.material.IToastService,
		gettextCatalog: angular.gettext.gettextCatalog,
		currentUser: AuthenticatedUser) {

		super($mdToast, gettextCatalog, currentUser);
	}
}