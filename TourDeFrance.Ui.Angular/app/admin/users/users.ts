﻿﻿/// <reference path="../../../references.ts"/>

class UserController extends BaseController {
	constructor($mdToast: ng.material.IToastService,
		gettextCatalog: angular.gettext.gettextCatalog,
		currentUser: AuthenticatedUser) {

		super($mdToast, gettextCatalog, currentUser);
	}
}