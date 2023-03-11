$(document).ready(function () {
    loadLawsPage();
})

function loadLawsPage() {
    GetLawsView().promise().done(function (partialView) {
        $('#displayInfo').empty();
        $('#displayInfo').append(partialView);
    });
}

function GetLawsView() {
    return $.ajax({
        url: '/Rules/GetLawsPartialView',
        success: function (partialView) {
            return partialView;
        }
    });
}

function loadExceptionsPage() {
    GetExceptionsView().promise().done(function (partialView) {
        $('#displayInfo').empty();
        $('#displayInfo').append(partialView);
    });
}

function GetExceptionsView() {
    return $.ajax({
        url: '/Rules/GetExceptionsPartialView',
        success: function (partialView) {
            return partialView;
        }
    });
}

function loadFactoriesPage() {
    GetFactoriesView().promise().done(function (partialView) {
        $('#displayInfo').empty();
        $('#displayInfo').append(partialView);
    });
}

function GetFactoriesView() {
    return $.ajax({
        url: '/Rules/GetFactoriesPartialView',
        success: function (partialView) {
            return partialView;
        }
    });
}

function loadMonopoliesPage() {
    GetMonopoliesView().promise().done(function (partialView) {
        $('#displayInfo').empty();
        $('#displayInfo').append(partialView);
    });
}

function GetMonopoliesView() {
    return $.ajax({
        url: '/Rules/GetMonopoliesPartialView',
        success: function (partialView) {
            return partialView;
        }
    });
}



