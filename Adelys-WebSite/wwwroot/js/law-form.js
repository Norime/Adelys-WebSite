// Lignes de l'historique des modifications d'une loi
const amendments = document.getElementById("lawAmendments");
if (amendments) {
    const template = document.getElementById("lawAmendmentTemplate");

    // Les index doivent se suivre pour que le formulaire soit lu correctement
    function renumber() {
        amendments.querySelectorAll(".rules-amendment-row").forEach((row, index) => {
            row.querySelectorAll("input").forEach(input => {
                input.name = input.name.replace(/Amendments\[[^\]]*\]/, `Amendments[${index}]`);
            });
        });
    }

    document.getElementById("lawAmendmentAdd").addEventListener("click", () => {
        amendments.append(template.content.cloneNode(true));
        renumber();
        amendments.lastElementChild.querySelector("input[type='text']").focus();
    });

    amendments.addEventListener("click", event => {
        if (event.target.closest(".rules-amendment-remove")) {
            event.target.closest(".rules-amendment-row").remove();
            renumber();
        }
    });
}
