/// <reference path="../references.ts"/>

module tourdefrance.components {
	"use strict";

	angular.module("tourdefrance.components", [])
		.component("tdfMenuToggle", new MenuToggleComponent())
		.component("tdfMenuLink", new MenuLinkComponent());
}