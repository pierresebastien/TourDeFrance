/// <reference path="../references.ts"/>
var tourdefrance;
(function (tourdefrance) {
    var components;
    (function (components) {
        "use strict";
        angular.module("tourdefrance.components", [])
            .component("tdfMenuToggle", new components.MenuToggleComponent())
            .component("tdfMenuLink", new components.MenuLinkComponent());
    })(components = tourdefrance.components || (tourdefrance.components = {}));
})(tourdefrance || (tourdefrance = {}));
