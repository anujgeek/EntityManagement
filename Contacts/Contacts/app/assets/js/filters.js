var FiltersModule = angular.module('FiltersModule');

FiltersModule.filter('DateFormatter', function ()
{
    return function (input)
    {
        return kendo.toString(kendo.parseDate(input, "yyyy-MM-ddTHH:mm:ss"), "d");
    };
});