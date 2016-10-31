/// <reference path="../references.ts"/>
var tourdefrance;
(function (tourdefrance) {
    var components;
    (function (components) {
        var Page = (function () {
            function Page() {
            }
            return Page;
        }());
        components.Page = Page;
        var MenuLinkComponent = (function () {
            function MenuLinkComponent() {
                this.bindings = {
                    page: "=",
                    focus: "&"
                };
                this.controller = MenuLinkController;
                this.controllerAs = "menu";
                this.templateUrl = "components/menu_link.tpl.html";
                this.transclude = false;
            }
            return MenuLinkComponent;
        }());
        components.MenuLinkComponent = MenuLinkComponent;
        var MenuLinkController = (function () {
            function MenuLinkController($element) {
                this.$element = $element;
            }
            MenuLinkController.prototype.$onInit = function () {
            };
            MenuLinkController.prototype.$onChanges = function (changesObj) {
            };
            MenuLinkController.prototype.$postLink = function () {
            };
            MenuLinkController.prototype.$onDestroy = function () { };
            MenuLinkController.$inject = ["$element"];
            return MenuLinkController;
        }());
        components.MenuLinkController = MenuLinkController;
    })(components = tourdefrance.components || (tourdefrance.components = {}));
})(tourdefrance || (tourdefrance = {}));
