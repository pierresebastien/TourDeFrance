/// <reference path="../../references.ts"/>

abstract class BaseController {
	baseWebUrl: string;
	baseApiUrl: string;
	currentUser: AuthenticatedUser;
	realUser: AuthenticatedUser;

	constructor(protected Restangular: restangular.IService,
		protected GlobalService: tourdefrance.services.IGlobalService,
		protected $state: ng.ui.IStateService,
		protected $mdToast: ng.material.IToastService,
		currentUser: AuthenticatedUser) {

		this.currentUser = currentUser;
		this.realUser = GlobalService.getRealUser();
	}

	public generateWebUrl(controller: string, action: string, parameters: any) {
		return this.generateUrl(this.baseWebUrl, controller, action, parameters);
	}

	protected generateUrl(baseUrl: string, controller: string, action: string, parameters: any) {
		var params: string = '';
		if (parameters != null) {
			var cpt = 0;
			for (var param in parameters) {
				if (cpt === 0) {
					params += '?';
				}
				if (cpt > 0) {
					params += '&';
				}
				params += param + '=' + parameters[param];
				cpt++;
			}
		}
		return baseUrl + controller + '/' + action + params;
	}
}
