$(function () {
    $(".dropdown-toggle").on("click", function (event) {
        event.preventDefault();
        var $menu = $(this).siblings(".dropdown-menu");
        $(".dropdown-menu").not($menu).hide();
        $menu.toggle();
    });

    $(document).on("click", function (event) {
        if (!$(event.target).closest(".dropdown-toggle, .dropdown-menu").length) {
            $(".dropdown-menu").hide();
        }
    });

    $(".navbar-toggler").on("click", function () {
        var isOpen = $(".nav-links").toggleClass("is-open").hasClass("is-open");
        $(this).attr("aria-expanded", isOpen);
    });
});
