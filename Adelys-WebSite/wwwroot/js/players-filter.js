const groupFilter = document.getElementById('playersGroupFilter');
const searchInput = document.getElementById('playersSearch');

function applyPlayersFilters() {
    const group = groupFilter.value;
    const search = searchInput.value.trim().toLowerCase();

    document.querySelectorAll('.players-grid').forEach(grid => {
        let visibleCards = 0;

        grid.querySelectorAll('.cardPlayer').forEach(card => {
            const isVisible = (!group || card.dataset.rank === group) && card.dataset.search.toLowerCase().includes(search);
            card.hidden = !isVisible;
            if (isVisible) {
                visibleCards++;
            }
        });

        grid.nextElementSibling.hidden = visibleCards > 0;
    });
}

groupFilter.addEventListener('change', applyPlayersFilters);
searchInput.addEventListener('input', applyPlayersFilters);
// Le navigateur peut restaurer les champs au retour sur la page
window.addEventListener('pageshow', applyPlayersFilters);
