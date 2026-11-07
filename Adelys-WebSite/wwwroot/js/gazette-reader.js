import * as pdfjsLib from "/lib/pdfjs/pdf.min.mjs";

pdfjsLib.GlobalWorkerOptions.workerSrc = "/lib/pdfjs/pdf.worker.min.mjs";

const TWO_PAGES_MIN_WIDTH = 900;
const ZOOM_LEVELS = [1, 1.5, 2, 3];
const SWIPE_DISTANCE = 50;

const reader = document.querySelector(".reader");
const stage = document.getElementById("reader-stage");
const pagesElement = document.getElementById("reader-pages");
const message = document.getElementById("reader-message");
const position = document.getElementById("reader-position");
const buttons = {
    prev: document.getElementById("reader-prev"),
    next: document.getElementById("reader-next"),
    zoomOut: document.getElementById("reader-zoom-out"),
    zoomIn: document.getElementById("reader-zoom-in"),
    fullscreen: document.getElementById("reader-fullscreen")
};

let pdf = null;
let currentPage = 1;
let zoomIndex = 0;
let renderId = 0;
let renderTasks = [];

function showsTwoPages() {
    return stage.clientWidth >= TWO_PAGES_MIN_WIDTH;
}

// La couverture est seule, puis les pages vont par deux comme dans un journal ouvert
function getSpread(page) {
    if (!showsTwoPages() || page === 1) {
        return [page];
    }
    const left = page % 2 === 0 ? page : page - 1;
    return left < pdf.numPages ? [left, left + 1] : [left];
}

async function render() {
    const id = ++renderId;
    renderTasks.forEach((task) => task.cancel());
    renderTasks = [];

    const spread = getSpread(currentPage);
    const pages = await Promise.all(spread.map((number) => pdf.getPage(number)));
    if (id !== renderId) {
        return;
    }

    const stageStyle = getComputedStyle(stage);
    const availableWidth = stage.clientWidth - parseFloat(stageStyle.paddingLeft) * 2;
    const availableHeight = stage.clientHeight - parseFloat(stageStyle.paddingTop) * 2;
    const viewports = pages.map((page) => page.getViewport({ scale: 1 }));
    const spreadWidth = viewports.reduce((total, viewport) => total + viewport.width, 0);
    const spreadHeight = Math.max(...viewports.map((viewport) => viewport.height));
    const scale = Math.min(availableWidth / spreadWidth, availableHeight / spreadHeight) * ZOOM_LEVELS[zoomIndex];
    const pixelRatio = Math.min(window.devicePixelRatio || 1, 2);

    const canvases = pages.map((page) => {
        const viewport = page.getViewport({ scale: scale * pixelRatio });
        const canvas = document.createElement("canvas");
        canvas.width = Math.floor(viewport.width);
        canvas.height = Math.floor(viewport.height);
        canvas.style.width = `${Math.floor(viewport.width / pixelRatio)}px`;
        canvas.style.height = `${Math.floor(viewport.height / pixelRatio)}px`;
        const task = page.render({ canvasContext: canvas.getContext("2d"), viewport });
        renderTasks.push(task);
        return { canvas, task };
    });

    try {
        await Promise.all(canvases.map((item) => item.task.promise));
    } catch (error) {
        if (error instanceof pdfjsLib.RenderingCancelledException) {
            return;
        }
        throw error;
    }
    if (id !== renderId) {
        return;
    }

    pagesElement.replaceChildren(...canvases.map((item) => item.canvas));
    message.hidden = true;
    stage.scrollTo(0, 0);
    updateControls(spread);
    prefetch(spread[spread.length - 1] + 1);
}

function updateControls(spread) {
    const first = spread[0];
    const last = spread[spread.length - 1];
    const label = spread.length > 1 ? `Pages ${first}-${last}` : `Page ${first}`;
    position.textContent = `${label} sur ${pdf.numPages}`;
    buttons.prev.disabled = first <= 1;
    buttons.next.disabled = last >= pdf.numPages;
    buttons.zoomOut.disabled = zoomIndex === 0;
    buttons.zoomIn.disabled = zoomIndex === ZOOM_LEVELS.length - 1;
    history.replaceState(null, "", `#page=${first}`);
}

// Télécharge la page suivante pendant la lecture de la page affichée
function prefetch(number) {
    if (number <= pdf.numPages) {
        pdf.getPage(number).then((page) => page.getOperatorList()).catch(() => {});
    }
}

function showError() {
    message.hidden = false;
    message.textContent = "Ce numéro n'a pas pu s'ouvrir. Utilise le bouton Télécharger pour le lire sur ton appareil.";
    position.textContent = "";
}

function refresh() {
    render().catch(showError);
}

function goToNext() {
    const spread = getSpread(currentPage);
    const next = spread[spread.length - 1] + 1;
    if (next <= pdf.numPages) {
        currentPage = next;
        refresh();
    }
}

function goToPrevious() {
    const previous = getSpread(currentPage)[0] - 1;
    if (previous >= 1) {
        currentPage = previous;
        refresh();
    }
}

function changeZoom(delta) {
    const next = zoomIndex + delta;
    if (next >= 0 && next < ZOOM_LEVELS.length) {
        zoomIndex = next;
        refresh();
    }
}

function readPageFromAddress() {
    const match = location.hash.match(/^#page=(\d+)$/);
    const page = match ? parseInt(match[1], 10) : 1;
    return Math.min(Math.max(page, 1), pdf.numPages);
}

function bindControls() {
    buttons.next.addEventListener("click", goToNext);
    buttons.prev.addEventListener("click", goToPrevious);
    buttons.zoomIn.addEventListener("click", () => changeZoom(1));
    buttons.zoomOut.addEventListener("click", () => changeZoom(-1));

    if (reader.requestFullscreen) {
        buttons.fullscreen.addEventListener("click", () => {
            if (document.fullscreenElement) {
                document.exitFullscreen();
            } else {
                reader.requestFullscreen();
            }
        });
    } else {
        buttons.fullscreen.hidden = true;
    }

    document.addEventListener("keydown", (event) => {
        if (event.key === "ArrowRight") {
            goToNext();
        } else if (event.key === "ArrowLeft") {
            goToPrevious();
        }
    });

    let touchStartX = null;
    stage.addEventListener("touchstart", (event) => {
        touchStartX = event.touches.length === 1 ? event.touches[0].clientX : null;
    }, { passive: true });
    stage.addEventListener("touchend", (event) => {
        if (touchStartX === null || zoomIndex !== 0) {
            return;
        }
        const distance = event.changedTouches[0].clientX - touchStartX;
        if (distance <= -SWIPE_DISTANCE) {
            goToNext();
        } else if (distance >= SWIPE_DISTANCE) {
            goToPrevious();
        }
    }, { passive: true });

    let resizeTimer = null;
    new ResizeObserver(() => {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(refresh, 150);
    }).observe(stage);
}

try {
    // Les pages sont chargées à la demande : un numéro de 46 Mo n'est jamais téléchargé d'un coup
    pdf = await pdfjsLib.getDocument({
        url: reader.dataset.url,
        disableAutoFetch: true,
        disableStream: true,
        rangeChunkSize: 1024 * 1024,
        isEvalSupported: false
    }).promise;
    currentPage = readPageFromAddress();
    bindControls();
    await render();
} catch (error) {
    showError();
}
