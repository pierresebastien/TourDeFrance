/// <reference path="../references.ts"/>

module tourdefrance.components {

	export class Section {
		name: string;
		isOpen: boolean;
		pages: Page[];
	}

	export class MenuToggleComponent implements ng.IComponentOptions {
		public bindings: { [binding: string]: string };
		public controller: Function;
		public controllerAs: string;
		public templateUrl: string;
		public transclude: boolean;

		constructor() {
			this.bindings = {
				section: "="
			}

			this.controller = MenuToggleController;
			this.controllerAs = "menu";
			this.templateUrl = "components/menu_toggle.tpl.html";
			this.transclude = false;
		}
	}

	export class MenuToggleController implements ng.IComponentController {
		public section: Section;

		constructor(private $element, private $mdUtil) { }

		public $onInit() {
		}

		public $onChanges(changesObj) {
		}

		public $postLink() {
		}

		public $onDestroy() { }

		// TODO: review code
		public toggle() {
			var $ul = this.$element.find('ul');
			var $li = $ul[0].querySelector('a.active');
			var docsMenuContent = document.querySelector('.site-menu').parentNode;
			var targetHeight = open ? this.getTargetHeight($ul) : 0;
			$ul.css({ height: targetHeight + 'px' });

			// If we are open and the user has not scrolled the content div; scroll the active
			// list item into view.
			if (open && $li && $li.offsetParent && $ul[0].scrollTop === 0) {
				var activeHeight = $li.scrollHeight;
				var activeOffset = $li.offsetTop;
				var parentOffset = $li.offsetParent.offsetTop;

				// Reduce it a bit (2 list items' height worth) so it doesn't touch the nav
				var negativeOffset = activeHeight * 2;
				var newScrollTop = activeOffset + parentOffset - negativeOffset;

				this.$mdUtil.animateScrollTo(docsMenuContent, newScrollTop);
			}

			this.section.isOpen = false;
		}

		public getTargetHeight($ul) {
			var targetHeight;
			$ul.addClass('no-transition');
			$ul.css('height', '');
			targetHeight = $ul.prop('clientHeight');
			$ul.css('height', 0);
			$ul.removeClass('no-transition');
			return targetHeight;
		}
	}
}