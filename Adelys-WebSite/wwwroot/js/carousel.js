// Carrousel de l'accueil : flèches, points, glissement au doigt et défilement automatique
document.querySelectorAll("[data-carousel]").forEach(carousel => {
    const track = carousel.querySelector(".carousel-track");
    const slides = [...track.children];
    const dots = [...carousel.querySelectorAll(".carousel-dot")];
    if (slides.length < 2) {
        return;
    }

    const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    let current = 0;
    let timer = null;

    function goTo(index) {
        current = (index + slides.length) % slides.length;
        track.scrollTo({ left: slides[current].offsetLeft, behavior: reduceMotion ? "auto" : "smooth" });
    }

    function showDot(index) {
        current = index;
        dots.forEach((dot, i) => {
            dot.classList.toggle("is-active", i === index);
            dot.setAttribute("aria-current", i === index ? "true" : "false");
        });
    }

    // Le point actif suit l'image visible, y compris quand on glisse au doigt
    const observer = new IntersectionObserver(entries => {
        entries.filter(entry => entry.isIntersecting).forEach(entry => showDot(slides.indexOf(entry.target)));
    }, { root: track, threshold: 0.6 });
    slides.forEach(slide => observer.observe(slide));

    carousel.querySelector(".carousel-prev").addEventListener("click", () => goTo(current - 1));
    carousel.querySelector(".carousel-next").addEventListener("click", () => goTo(current + 1));
    dots.forEach((dot, i) => dot.addEventListener("click", () => goTo(i)));

    track.addEventListener("keydown", event => {
        if (event.key === "ArrowLeft" || event.key === "ArrowRight") {
            event.preventDefault();
            goTo(current + (event.key === "ArrowRight" ? 1 : -1));
        }
    });

    function start() {
        if (!reduceMotion && timer === null) {
            timer = setInterval(() => goTo(current + 1), 6000);
        }
    }

    function stop() {
        clearInterval(timer);
        timer = null;
    }

    // Pas de défilement pendant qu'on regarde ou qu'on manipule le carrousel
    carousel.addEventListener("mouseenter", stop);
    carousel.addEventListener("mouseleave", start);
    carousel.addEventListener("focusin", stop);
    carousel.addEventListener("focusout", start);
    track.addEventListener("touchstart", stop, { passive: true });
    document.addEventListener("visibilitychange", () => document.hidden ? stop() : start());

    showDot(0);
    start();
});
