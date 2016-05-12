var AboutModule = angular.module('AboutModule');

AboutModule.controller('AboutController', AboutController);
function AboutController($scope)
{
    $scope.FrontEnd = "HTML5, CSS3, JS, AngularJS, jQ, jQ UI, Bootstrap, Kendo UI, DataTables";
    $scope.BackEnd = "C#, Web API, LINQ, EF, SQL, SQL Server 2014";
};
AboutController.$inject = ['$scope'];