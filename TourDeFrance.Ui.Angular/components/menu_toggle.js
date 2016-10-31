/// <reference path="../references.ts"/>
var tourdefrance;
(function (tourdefrance) {
    var components;
    (function (components) {
        var Section = (function () {
            function Section() {
            }
            return Section;
        }());
        components.Section = Section;
        var MenuToggleComponent = (function () {
            function MenuToggleComponent() {
                this.bindings = {
                    section: "="
                };
                this.controller = MenuToggleController;
                this.controllerAs = "menu";
                this.templateUrl = "components/menu_toggle.tpl.html";
                this.transclude = false;
            }
            return MenuToggleComponent;
        }());
        components.MenuToggleComponent = MenuToggleComponent;
        var MenuToggleController = (function () {
            //static $inject: string[] = ["$element", "$mdUtil"];
            function MenuToggleController($element, $mdUtil) {
                this.$element = $element;
                this.$mdUtil = $mdUtil;
            }
            MenuToggleController.prototype.$onInit = function () {
            };
            MenuToggleController.prototype.$onChanges = function (changesObj) {
            };
            MenuToggleController.prototype.$postLink = function () {
            };
            MenuToggleController.prototype.$onDestroy = function () { };
            // TODO: review code
            MenuToggleController.prototype.toggle = function () {
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
            };
            MenuToggleController.prototype.getTargetHeight = function ($ul) {
                var targetHeight;
                $ul.addClass('no-transition');
                $ul.css('height', '');
                targetHeight = $ul.prop('clientHeight');
                $ul.css('height', 0);
                $ul.removeClass('no-transition');
                return targetHeight;
            };
            return MenuToggleController;
        }());
        components.MenuToggleController = MenuToggleController;
    })(components = tourdefrance.components || (tourdefrance.components = {}));
})(tourdefrance || (tourdefrance = {}));
