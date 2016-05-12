var GetListModule = angular.module('GetListModule');

GetListModule.controller('GetListController', GetListController);
function GetListController($scope, $filter, DialogService)
{
    $scope.initialize = function ()
    {
        $(document).ready(function ()
        {
            $scope.table = $('#table').DataTable(
                {
                    'processing': true,
                    'serverSide': true,
                    'info': true,
                    'stateSave': false,
                    'lengthMenu': [[10, 20, 50, -1], [10, 20, 50, "All"]],
                    'language':
                        {
                            zeroRecords: 'Nothing found',
                            emptyTable: 'Nothing found',
                            search: '_INPUT_',
                            searchPlaceholder: 'Search by name...'
                        },
                    'columns':
                        [
                            { 'data': 'id', 'orderable': true },
                            { 'data': 'first_name', 'orderable': true },
                            { 'data': 'last_name', 'orderable': true },
                            { 'data': 'company_name', 'orderable': true },
                            { 'data': 'address', 'orderable': true },
                            { 'data': 'city', 'orderable': true },
                            { 'data': 'state', 'orderable': true },
                            { 'data': 'zip', 'orderable': true },
                            { 'data': 'phone', 'orderable': true },
                            { 'data': 'work_phone', 'orderable': true },
                            { 'data': 'email', 'orderable': true },
                            { 'data': 'url', 'orderable': true },
                            { 'data': 'dob', 'orderable': true }
                        ],
                    'ordering': true,
                    'orderMulti': false,
                    'order': [[0, 'asc']],
                    'autoWidth': true,
                    'ajax': function (data, callback, settings)
                    {
                        $.ajax(
                            {
                                'type': 'GET',
                                'url': constants.Services.ContactsAPI.ContactsProcessed,
                                'data':
                                    {
                                        draw: data.draw,
                                        start: data.start,
                                        length: data.length,
                                        searchvalue: data.search.value,
                                        ordercolumn: data.order[0].column,
                                        orderdir: data.order[0].dir
                                    },
                                success: function (d)
                                {
                                    d.data.forEach(function (element, index, array)
                                    {
                                        element.dob = $filter('DateFormatter')(element.dob);
                                    });

                                    callback(d);
                                },
                                error: function ()
                                {
                                    DialogService.showsimplemodaldialog('Error', arguments[2]);
                                }
                            });
                    },
                    'deferLoading': 0
                });

            $scope.table.ajax.reload();

            $scope.table.on('click', 'tr', function ()
            {
                if ($(this).hasClass('selected'))
                {
                    $(this).removeClass('selected');
                }
                else
                {
                    $scope.table.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });
        });
    };
};
GetListController.$inject = ['$scope', '$filter', 'DialogService'];