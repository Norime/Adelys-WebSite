const shopItems = document.getElementById('shopItems');
const shopItemTemplate = document.getElementById('shopItemTemplate');
const MAX_SUGGESTIONS = 30;
let itemCatalog = null;

// Le serveur lit les items par index : ils doivent rester continus après un ajout ou un retrait
function renumberShopItems() {
    shopItems.querySelectorAll('.shop-item-row').forEach((row, index) => {
        row.querySelectorAll('[data-field]').forEach(input => {
            input.name = `Items[${index}].${input.dataset.field}`;
        });
    });
}

document.getElementById('shopItemAdd').addEventListener('click', () => {
    shopItems.append(shopItemTemplate.content.cloneNode(true));
    renumberShopItems();
    shopItems.lastElementChild.querySelector('.item-picker-input').focus();
});

shopItems.addEventListener('click', event => {
    if (event.target.classList.contains('shop-item-remove')) {
        event.target.closest('.shop-item-row').remove();
        renumberShopItems();
    }
});

// « epee » doit trouver « Épée »
function normalizeItemName(text) {
    return text.normalize('NFD').replace(/\p{Diacritic}/gu, '').toLowerCase().trim();
}

async function loadItemCatalog() {
    if (!itemCatalog) {
        const response = await fetch(shopItems.dataset.catalogUrl);
        const items = response.ok ? await response.json() : [];
        itemCatalog = items.map(item => ({ ...item, search: normalizeItemName(item.name) }));
    }
    return itemCatalog;
}

function findSuggestions(catalog, query) {
    const search = normalizeItemName(query);
    if (!search) {
        return [];
    }
    const startsWith = catalog.filter(item => item.search.startsWith(search));
    const contains = catalog.filter(item => !item.search.startsWith(search) && item.search.includes(search));
    return startsWith.concat(contains).slice(0, MAX_SUGGESTIONS);
}

function setPickerItem(picker, item) {
    const icon = picker.querySelector('.item-picker-icon');
    picker.querySelector('[data-field="ItemId"]').value = item ? item.id : '';
    if (item) {
        picker.querySelector('.item-picker-input').value = item.name;
    }
    icon.hidden = !item || !item.icon;
    if (item && item.icon) {
        icon.src = item.icon;
    }
}

function closePicker(picker) {
    picker.querySelector('.item-picker-list').hidden = true;
    picker.querySelector('.item-picker-input').setAttribute('aria-expanded', 'false');
}

function renderSuggestions(picker, suggestions) {
    const list = picker.querySelector('.item-picker-list');
    list.replaceChildren(...suggestions.map((item, index) => {
        const option = document.createElement('li');
        option.setAttribute('role', 'option');
        option.dataset.index = index;
        if (item.icon) {
            const icon = document.createElement('img');
            icon.src = item.icon;
            icon.alt = '';
            icon.loading = 'lazy';
            option.append(icon);
        }
        option.append(item.name);
        return option;
    }));
    picker.suggestions = suggestions;
    picker.activeIndex = -1;
    list.hidden = suggestions.length === 0;
    picker.querySelector('.item-picker-input').setAttribute('aria-expanded', String(!list.hidden));
}

function setActiveSuggestion(picker, index) {
    const options = picker.querySelectorAll('.item-picker-list li');
    if (options.length === 0) {
        return;
    }
    picker.activeIndex = (index + options.length) % options.length;
    options.forEach((option, i) => option.setAttribute('aria-selected', String(i === picker.activeIndex)));
    options[picker.activeIndex].scrollIntoView({ block: 'nearest' });
}

shopItems.addEventListener('input', async event => {
    if (!event.target.classList.contains('item-picker-input')) {
        return;
    }
    const picker = event.target.closest('.item-picker');
    // Le texte a changé : l'item choisi avant n'est plus valable
    setPickerItem(picker, null);
    renderSuggestions(picker, findSuggestions(await loadItemCatalog(), event.target.value));
});

shopItems.addEventListener('keydown', event => {
    if (!event.target.classList.contains('item-picker-input')) {
        return;
    }
    const picker = event.target.closest('.item-picker');
    const isOpen = !picker.querySelector('.item-picker-list').hidden;
    if (event.key === 'ArrowDown' || event.key === 'ArrowUp') {
        event.preventDefault();
        setActiveSuggestion(picker, (picker.activeIndex ?? -1) + (event.key === 'ArrowDown' ? 1 : -1));
    } else if (event.key === 'Enter' && isOpen) {
        // Entrée choisit l'item au lieu d'envoyer le formulaire
        event.preventDefault();
        const item = picker.suggestions[Math.max(picker.activeIndex, 0)];
        setPickerItem(picker, item);
        closePicker(picker);
    } else if (event.key === 'Escape') {
        closePicker(picker);
    }
});

// mousedown passe avant la perte du focus, qui fermerait la liste avant le clic
shopItems.addEventListener('mousedown', event => {
    const option = event.target.closest('.item-picker-list li');
    if (option) {
        event.preventDefault();
        const picker = option.closest('.item-picker');
        setPickerItem(picker, picker.suggestions[Number(option.dataset.index)]);
        closePicker(picker);
    }
});

// Un nom tapé en entier sans passer par la liste est accepté s'il correspond exactement à un item
shopItems.addEventListener('focusout', async event => {
    if (!event.target.classList.contains('item-picker-input')) {
        return;
    }
    const picker = event.target.closest('.item-picker');
    closePicker(picker);
    if (!picker.querySelector('[data-field="ItemId"]').value && event.target.value.trim()) {
        const search = normalizeItemName(event.target.value);
        setPickerItem(picker, (await loadItemCatalog()).find(item => item.search === search) ?? null);
    }
});
