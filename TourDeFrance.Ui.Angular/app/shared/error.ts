﻿/// <reference path="../../references.ts"/>

// TODO: to review (extends base controller?)
class ErrorController {
	errorMessage: string;
	accessedState: any;
	show: boolean;

	constructor(Restangular: restangular.IService,
		GlobalService: tourdefrance.services.IGlobalService,
		public previousState: any) {

		this.show = false;
		this.errorMessage = GlobalService.errorMessage;
		this.accessedState = GlobalService.accessedState;
	}
}