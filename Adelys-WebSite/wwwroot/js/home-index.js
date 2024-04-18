$(document).ready(function () {
    console.log($('#carousel').css('height')); // check the initial height of the carousel;

    // now apply this height as a max-height on all the image items; css will handle the rest;
    $('#carousel').find('.active img').css('max-height', $('#carousel').css('height'))
});