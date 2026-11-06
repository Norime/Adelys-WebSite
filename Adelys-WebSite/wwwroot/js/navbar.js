$(function () {
    $('.dropdown-menu').hide();
    $(".dropdown-toggle").on("click", function (event) {
        event.preventDefault();
        $(this).siblings('.dropdown-menu').toggle();
    });
});

function openDropdownMenu() {
    var x = document.getElementByClassName("dropdown-menu");
    if (x.style.display === "block") {
        x.style.display = "none";
    } else {
        x.style.display = "block";
    }
}

function openNavbarMenu() {
    var x = document.getElementById("nav-links");
    if (x.style.display === "block") {
        x.style.display = "none";
    } else {
        x.style.display = "block";
    }
}



//function toggleMenu() {
//    const navLinks = document.getElementById('nav-links');
//    const togglerIcon = document.querySelector('.navbar-toggler-icon');

//    if (!isOpen) {
//        navLinks.classList.remove('hide-on-large');
//        isOpen = true;
//        togglerIcon.setAttribute('aria-expanded', 'true');
//    } else {
//        navLinks.classList.add('hide-on-large');
//        isOpen = false;
//        togglerIcon.setAttribute('aria-expanded', 'false');
//    }
//}

//document.addEventListener('DOMContentLoaded', () => {
//    const navLinks = document.getElementById('nav-links');
//    const togglerIcon = document.querySelector('.navbar-toggler-icon');

//    // Fermez le menu lorsque vous cliquez sur un lien
//    Array.from(document.getElementsByClassName('nav-link')).forEach((element) => {
//        element.addEventListener('click', () => {
//            navLinks.classList.add('hide-on-large');
//            isOpen = false;
//            togglerIcon.setAttribute('aria-expanded', 'false');
//        });
//    });
//});