const MAP_ID = 'adelys';
const DEFAULT_DISTANCE = 300;
const MIN_DISTANCE = 50;
const MAX_DISTANCE = 1500;
const ZOOM_FACTOR = 1.5;

const shopMapPanel = document.getElementById('shopMapPanel');
const shopMapLabel = document.getElementById('shopMapLabel');
const shopMap = document.getElementById('shopMap');
const shopMapFrame = document.getElementById('shopMapFrame');
let selectedShop = null;
let distance = DEFAULT_DISTANCE;

// La carte n'est chargée qu'au premier clic, ensuite seule l'ancre change et BlueMap déplace sa caméra sans recharger
function showSelectedShop() {
    const { x, y, z } = selectedShop;
    shopMapPanel.hidden = false;
    shopMapFrame.src = `${shopMap.dataset.mapUrl}#${MAP_ID}:${x}:${y}:${z}:${distance}:0:0:0:1:flat`;
}

function zoom(newDistance) {
    if (selectedShop) {
        distance = Math.round(Math.min(MAX_DISTANCE, Math.max(MIN_DISTANCE, newDistance)));
        showSelectedShop();
    }
}

function selectShop(card) {
    document.querySelector('.shop-card-selected')?.classList.remove('shop-card-selected');
    card.classList.add('shop-card-selected');
    selectedShop = card.dataset;
    distance = DEFAULT_DISTANCE;
    shopMapLabel.textContent = card.dataset.name;
    showSelectedShop();
    shopMapPanel.scrollIntoView({ block: 'nearest', behavior: 'smooth' });
}

document.querySelectorAll('.shop-card-map').forEach(card => {
    card.addEventListener('click', event => {
        // Les liens Modifier et Supprimer des admins gardent leur propre action
        if (!event.target.closest('a, button, form')) {
            selectShop(card);
        }
    });
    card.addEventListener('keydown', event => {
        if (event.target === card && (event.key === 'Enter' || event.key === ' ')) {
            event.preventDefault();
            selectShop(card);
        }
    });
});

document.getElementById('shopMapZoomIn').addEventListener('click', () => zoom(distance / ZOOM_FACTOR));
document.getElementById('shopMapZoomOut').addEventListener('click', () => zoom(distance * ZOOM_FACTOR));
