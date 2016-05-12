var GetModule = angular.module('GetModule');

GetModule.controller('GetController', GetController);
function GetController($scope, $attrs, DialogService)
{
    $scope.uniqueid = $attrs.uniqueid;
    $scope.isreadonly = true;
    $scope.iscontactcontrolvisible = false;

    $scope.contactidobject = {};
    $scope.contactidobject.id = null;

    $scope.contactobject = {};
    $scope.contactobject.first_name = "";
    $scope.contactobject.last_name = "";
    $scope.contactobject.company_name = "";
    $scope.contactobject.address = "";
    $scope.contactobject.city = "";
    $scope.contactobject.state = "";
    $scope.contactobject.zip = "";
    $scope.contactobject.phone = "";
    $scope.contactobject.work_phone = "";
    $scope.contactobject.email = "";
    $scope.contactobject.url = "";
    $scope.contactobject.dob = null;

    $scope.GetActionClicked = function ()
    {
        $.ajax(
            {
                'type': 'GET',
                'url': constants.Services.ContactsAPI.ContactDetails + "/" + $scope.contactidobject.id,
                success: function (data)
                {
                    $scope.$apply(function ()
                    {
                        $scope.contactobject = data;
                        $scope.iscontactcontrolvisible = true;
                    });
                },
                error: function ()
                {
                    $scope.iscontactcontrolvisible = false;
                    DialogService.showsimplemodaldialog('Error', arguments[2]);
                }
            });
    }
};
GetController.$inject = ['$scope', '$attrs', 'DialogService'];