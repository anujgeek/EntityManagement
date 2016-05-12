var UpdateModule = angular.module('UpdateModule');

UpdateModule.controller('UpdateController', UpdateController);
function UpdateController($scope, $attrs, DialogService)
{
    $scope.uniqueid = $attrs.uniqueid;
    $scope.isreadonly = false;
    $scope.isdirty = false;
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

    $scope.contactobjectbackup = angular.toJson($scope.contactobject);

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
                        $scope.contactobject = angular.copy(data);
                        $scope.contactobjectbackup = angular.toJson($scope.contactobject);
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

    $scope.$watchCollection('contactobject', function ()
    {
        if (angular.toJson($scope.contactobject) == $scope.contactobjectbackup)
        {
            $scope.isdirty = false;
        }
        else
        {
            $scope.isdirty = true;
        }
    });

    $scope.cancelbuttonclicked = function ()
    {
        $scope.contactobject = angular.copy(angular.fromJson($scope.contactobjectbackup));
    }

    $scope.updatebuttonclicked = function ()
    {
        $.ajax(
            {
                'type': 'PUT',
                'url': constants.Services.ContactsAPI.ContactUpdate + "/" + $scope.contactidobject.id,
                'data':
                    {
                        id: $scope.contactidobject.id,
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
                    if (arguments[1] == "success")
                    {
                        $scope.contactobjectbackup = angular.toJson($scope.contactobject);
                        $scope.isdirty = false;

                        DialogService.showsimplemodaldialog('Success', "Contact updated successfully (Id: " + $scope.contactobject.id + ")");
                    }
                    else
                    {
                        DialogService.showsimplemodaldialog('Error', "Contact update failed");
                    }
                },
                error: function ()
                {
                    DialogService.showsimplemodaldialog('Error', arguments[2]);
                }
            });
    }
};
UpdateController.$inject = ['$scope', '$attrs', 'DialogService'];