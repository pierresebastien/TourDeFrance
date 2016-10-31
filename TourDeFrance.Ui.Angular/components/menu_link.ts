/// <reference path="../references.ts"/>

module tourdefrance.components {

	export class Page {
		name: string;
		icon: string;
		route: string;
		isSelected: boolean;
	}

	export class MenuLinkComponent implements ng.IComponentOptions {
		public bindings: { [binding: string]: string };
		public controller: Function;
		public controllerAs: string;
		public templateUrl: string;
		public transclude: boolean;

		constructor() {
			this.bindings = {
				page: "=",
				focus: "&"
			}

			this.controller = MenuLinkController;
			this.controllerAs = "menu";
			this.templateUrl = "components/menu_link.tpl.html";
			this.transclude = false;
		}
	}

	export class MenuLinkController implements ng.IComponentController {
		public page: Page;
		public focus: () => any;

		static $inject: string[] = ["$element"];

		constructor(private $element) {}

		public $onInit() {
		}

		public $onChanges(changesObj) {
		}

		public $postLink() {
		}

		public $onDestroy() {}
	}
}