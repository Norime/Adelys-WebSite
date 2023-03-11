$(document).ready(function () {
    loadShopsPage();
})

function loadShopsPage(cityName) {
    GetShopsCityView(cityName = "Adelys").promise().done(function (partialView) {
        $('#displayInfo').empty();
        $('#displayInfo').append(partialView);
    });
}

function GetShopsCityView(cityNameToSend) {
    return $.ajax({
        url: '/Rules/GetShopsFromCityPartialView',
        data: { cityName: cityNameToSend },
        success: function (partialView) {
            return partialView;
        }
    });
}