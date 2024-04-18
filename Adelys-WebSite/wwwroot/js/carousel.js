// Récupère le nombre d'enfant de slideshow
// Affiche un enfant du slideshow à la fois

//$(document).ready(function () {
//    // Itération sur chaque élément possédant la classe "carousel"
//    $(".carousel").each(function () {
//        var $carousel = $(this);
//        var $slideshow = $carousel.find(".slideshow");
//        var nbSliderItemInCarousel = $slideshow.children().length;
//        var $carouselIndicators = $carousel.find(".carousel-indicators");

//        for (var x = 0; x < nbSliderItemInCarousel; x++) {
//            var $button = $("<button></button>", {
//                "data-slide-to": x,
//                text: x + 1,
//                click: function () {
//                    $slideshow.carousel(parseInt($(this).attr("data-slide-to")));
//                }
//            });

//            $carouselIndicators.append($button);
//        }
//        $carousel.append($carouselIndicators);
//    });
//});

//$(".slideshow-item").addClass("active");

$(document).ready(function () {
    $(".carousel").each(function () {
        var $carousel = $(this);
        var $slideshow = $carousel.find(".slideshow");
        var nbSliderItemInCarousel = $slideshow.children().length;
        var $carouselIndicators = $carousel.find(".carousel-indicators");

        for (var x = 0; x < nbSliderItemInCarousel; x++) {
            var $button = $("<button></button>", {
                "data-slide-to": x,
                text: x + 1,
                click: function () {
                    $slideshow.carousel(parseInt($(this).attr("data-slide-to")));
                }
            });

            $carouselIndicators.append($button);
        }
        $carousel.append($carouselIndicators);

        // Ajout de la classe "active" au premier élément
        $slideshow.find('.slideshow-item').first().addClass('active');

        // Modification de l'élément actif toutes les 5 secondes
        setInterval(function () {
            var currentActiveSlide = $slideshow.find('.slideshow-item.active');
            var nextActiveSlideIndex = currentActiveSlide.index() + 1;
            if (nextActiveSlideIndex >= nbSliderItemInCarousel) {
                nextActiveSlideIndex = 0;
            }
            var nextActiveSlide = $slideshow.find('.slideshow-item').eq(nextActiveSlideIndex);
            currentActiveSlide.removeClass('active');
            nextActiveSlide.addClass('active');
        }, 5000);

        // Activation de l'élément souhaité lorsqu'on clique sur un indicateur
        $carouselIndicators.on('click', 'button[data-slide-to]', function () {
            var slideToIndex = parseInt($(this).attr('data-slide-to'));
            $slideshow.find('.slideshow-item.active').removeClass('active');
            $slideshow.find('.slideshow-item').eq(slideToIndex).addClass('active');
        });
    });
});