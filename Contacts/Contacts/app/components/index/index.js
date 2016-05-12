var MainModule = angular.module('MainModule');

MainModule.controller('MainController', MainController);
function MainController($scope, $rootScope)
{
    $rootScope.simplemodaldialog =
        {
            dialog: $('#simplemodaldialog'),
            title: "",
            message: ""
        };

    $scope.$on('$includeContentLoaded', function (event, url)
    {
        switch (url)
        {
            case 'app/components/getlist/getlist.html':
                {
                    break;
                }
        }
    });
};
MainController.$inject = ['$scope', '$rootScope'];