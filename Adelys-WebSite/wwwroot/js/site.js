// Bascule entre les deux palettes du site, le choix est gardé dans le navigateur
$(function () {
    var palettes = { nuit: "Nuit", pierre: "Pierre" };
    var $toggle = $(".palette-toggle");

    function showPalette(palette) {
        document.documentElement.dataset.palette = palette;
        $toggle.find(".palette-label").text(palettes[palette]);
        $toggle.attr("aria-label", "Couleurs du site : " + palettes[palette] + ". Changer de couleurs");
    }

    showPalette(document.documentElement.dataset.palette === "pierre" ? "pierre" : "nuit");

    $toggle.on("click", function () {
        var palette = document.documentElement.dataset.palette === "pierre" ? "nuit" : "pierre";
        showPalette(palette);
        try {
            localStorage.setItem("adelys-palette", palette);
        } catch (e) {
            // Stockage indisponible : le choix vaut pour cette page
        }
    });
});
