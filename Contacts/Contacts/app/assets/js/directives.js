var DirectivesModule = angular.module('DirectivesModule');

DirectivesModule.directive('contactidcontrol', function ()
{
    var directive = {};
    directive.restrict = 'E';
    directive.templateUrl = '/app/common/contact_id/contact_id.html';

    directive.scope =
        {
            uniqueid: "=uniqueid",
            contactidobject: "=contactidobject",
            actionlabel: "=actionlabel",
            actionbuttonclicked: "=actionbuttonclicked"
        }

    return directive;
});

DirectivesModule.directive('contactcontrol', function ()
{
    var directive = {};
    directive.restrict = 'E';
    directive.templateUrl = '/app/common/contact/contact.html';

    directive.scope =
        {
            uniqueid: "=uniqueid",
            isreadonly: "=isreadonly",
            contactobject: "=contactobject"
        }

    return directive;
});