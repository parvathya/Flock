﻿//TODO: This controller should be in real controllers folder?
flockApp.controller('autoUpdateUserTagsController', function ($scope) {
    //TODO: connect this with server side code.
    $scope.users = ['VenCKi', 'Venki', 'Venkat']; //it should return most used tags
    $scope.quackText = "";

    $scope.$watch('quackText', function (param) {
        $scope.$emit('quackTextChanged', $scope.quackText);
    });
});

flockApp.directive('autoUpdateUserTags', function () {
    return {
        restrict: 'A',
        //TODO: move this to a template file.
        template: '<div><div id="suggessions" ng-repeat="user in filteredUsers"> <span>{{user}}</span>  </div></div>',
        scope: {
            listOfUsers: '='            
        }, 
        controller: function ($rootScope, $scope) {
            var self = this;            
            self.scope = $scope;
            self.isNewUserTagAttempted = false;

            $rootScope.$on('quackTextChanged', function (event, quackText) {
                $scope.filteredUsers = [];                
                if (quackText && quackText.indexOf('@') > 0) {

                    if (quackText.lastIndexOf('@') === (quackText.length - 1)) {
                        self.isNewUserTagAttempted = true;                        
                    }

                    if (self.isNewUserTagAttempted && (quackText.lastIndexOf('@') !== (quackText.length - 1)) && quackText.lastIndexOf(' ') === (quackText.length - 2)) {
                        self.isNewUserTagAttempted = false;                        
                    }
                    
                    if (self.isNewUserTagAttempted) {
                        quackText = quackText.substring(quackText.lastIndexOf('@'), quackText.length);
                        //TODO: remove this log
                        console.log(quackText);
                    }

                    var textFieldValue = quackText.slice(1);
                    if (textFieldValue) {
                        //TODO: implement more to show all tags?
                        _.find(_.last(self.scope.listOfUsers, 10), function (aUser) {
                            if (aUser.toLowerCase().indexOf(textFieldValue.toLowerCase()) === 0) {
                                self.scope.filteredUsers.push("Tag user " + aUser);
                            }
                        });
                    }
                }
            });
        }        
    };
});