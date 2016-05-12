//Main Module

angular.module('MainModule', ['ngRoute', 'DirectivesModule', 'FiltersModule', 'ServicesModule', 'AboutModule', 'AddModule', 'GetModule', 'GetListModule', 'UpdateModule', 'DeleteModule']);

//Common Modules

angular.module('DirectivesModule', []);
angular.module('FiltersModule', ['kendo.directives']);
angular.module('ServicesModule', []);

//Component Modules

angular.module('AboutModule', []);
angular.module('AddModule', ['DirectivesModule', 'ServicesModule']);
angular.module('GetListModule', ['DirectivesModule', 'FiltersModule', 'ServicesModule']);
angular.module('GetModule', ['DirectivesModule', 'FiltersModule', 'ServicesModule']);
angular.module('UpdateModule', ['DirectivesModule', 'ServicesModule']);
angular.module('DeleteModule', ['DirectivesModule', 'ServicesModule']);