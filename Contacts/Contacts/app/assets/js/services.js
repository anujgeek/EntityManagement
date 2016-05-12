var ServicesModule = angular.module('ServicesModule');

ServicesModule.factory('DialogService', ['$rootScope',
    function ($rootScope)
    {
        var factory = {};

        factory.showsimplemodaldialog = function (title, message)
        {
            $rootScope.$apply(function ()
            {
                $rootScope.simplemodaldialog.title = title;
                $rootScope.simplemodaldialog.message = message;
            });

            $rootScope.simplemodaldialog.dialog.modal('show');
        }

        return factory;
    }]);