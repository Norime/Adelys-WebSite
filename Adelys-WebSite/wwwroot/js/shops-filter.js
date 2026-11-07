const shopTypeFilter = document.getElementById('shopTypeFilter');
const shopOwnerFilter = document.getElementById('shopOwnerFilter');
const shopGroupFilter = document.getElementById('shopGroupFilter');
const shopItemSearch = document.getElementById('shopItemSearch');

// « epee » doit trouver « Épée »
function normalizeShopText(text) {
    return text.normalize('NFD').replace(/\p{Diacritic}/gu, '').toLowerCase();
}

function applyShopsFilters() {
    const type = shopTypeFilter.value;
    const owner = shopOwnerFilter.value;
    // Le groupe est celui du propriétaire dans LuckPerms : choisir pristo garde les commerces de tous les joueurs de pristo
    const group = shopGroupFilter.value;
    const search = normalizeShopText(shopItemSearch.value.trim());

    document.querySelectorAll('.shops-city').forEach(city => {
        let visibleShops = 0;

        city.querySelectorAll('.shop-card').forEach(card => {
            let matchingItems = 0;

            // Pendant une recherche, la carte ne garde que les items trouvés et leur prix
            card.querySelectorAll('[data-item]').forEach(item => {
                const isMatch = normalizeShopText(item.dataset.item).includes(search);
                item.hidden = !isMatch;
                if (isMatch) {
                    matchingItems++;
                }
            });

            const isVisible = (!type || card.dataset.types.split('|').includes(type))
                && (!owner || card.dataset.owner === owner)
                && (!group || card.dataset.group === group)
                && (!search || matchingItems > 0);
            card.hidden = !isVisible;
            if (isVisible) {
                visibleShops++;
            }
        });

        city.querySelector('.shops-empty').hidden = visibleShops > 0;
    });
}

shopTypeFilter.addEventListener('change', applyShopsFilters);
shopOwnerFilter.addEventListener('change', applyShopsFilters);
shopGroupFilter.addEventListener('change', applyShopsFilters);
shopItemSearch.addEventListener('input', applyShopsFilters);
// Le navigateur peut restaurer les champs au retour sur la page
window.addEventListener('pageshow', applyShopsFilters);
