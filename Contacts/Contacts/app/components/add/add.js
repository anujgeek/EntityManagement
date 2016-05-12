var AddModule = angular.module('AddModule');

AddModule.controller('AddController', AddController);
function AddController($scope, $attrs, DialogService)
{
    $scope.uniqueid = $attrs.uniqueid;
    $scope.isreadonly = false;

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

    $scope.addbuttonclicked = function ()
    {
        $.ajax(
            {
                'type': 'POST',
                'url': constants.Services.ContactsAPI.ContactAdd,
                'data':
                    {
                        first_name: $scope.contactobject.first_name,
                        last_name: $scope.contactobject.last_name,
                        company_name: $scope.contactobject.company_name,
                        address: $scope.contactobject.address,
                        city: $scope.contactobject.city,
                        state: $scope.contactobject.state,
                        zip: $scope.contactobject.zip,
                        phone: $scope.contactobject.phone,
                        work_phone: $scope.contactobject.work_phone,
                        email: $scope.contactobject.email,
                        url: $scope.contactobject.url,
                        dob: $scope.contactobject.dob
                    },
                success: function (data)
                {
                    if (data.id != null)
                    {
                        DialogService.showsimplemodaldialog('Success', "Contact added successfully (Id: " + data.id + ")");
                    }
                    else
                    {
                        DialogService.showsimplemodaldialog('Error', "Contact addition failed");
                    }
                },
                error: function ()
                {
                    DialogService.showsimplemodaldialog('Error', arguments[2]);
                }
            });
    }
};
AddController.$inject = ['$scope', '$attrs', 'DialogService'];