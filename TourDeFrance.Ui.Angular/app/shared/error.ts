﻿/// <reference path="../../references.ts"/>

// TODO: to review
class ErrorController {
	errorMessage: string;
	accessedState: any;
	show: boolean;
	adminMail: string;

	constructor(Restangular: restangular.IService,
		GlobalService: tourdefrance.services.IGlobalService,
		public previousState: any) {

		Restangular.one('configs', 'AdminMail')
			.get<Config>()
			.then((config: Config) => {
				this.adminMail = config.value != null ? config.value : config.defaultValue;
			});
		this.show = false;
		this.errorMessage = GlobalService.errorMessage;
		this.accessedState = GlobalService.accessedState;
	}
}