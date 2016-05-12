var DeleteModule = angular.module('DeleteModule');

DeleteModule.controller('DeleteController', DeleteController);
function DeleteController($scope, $attrs, DialogService)
{
    $scope.uniqueid = $attrs.uniqueid;

    $scope.contactidobject = {};
    $scope.contactidobject.id = null;

    $scope.DeleteActionClicked = function ()
    {
        $.ajax(
            {
                'type': 'DELETE',
                'url': constants.Services.ContactsAPI.ContactDelete + "/" + $scope.contactidobject.id,
                success: function (data)
                {
                    if (data.id == $scope.contactidobject.id)
                    {
                        DialogService.showsimplemodaldialog('Success', "Contact (Id: " + data.id + ") deleted successfully");
                    }
                    else
                    {
                        DialogService.showsimplemodaldialog('Error', "Contact deletion failed");
                    }
                },
                error: function ()
                {
                    DialogService.showsimplemodaldialog('Error', arguments[2]);
                }
            });
    }
};
DeleteController.$inject = ['$scope', '$attrs', 'DialogService'];