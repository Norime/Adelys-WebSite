import * as pdfjsLib from "/lib/pdfjs/pdf.min.mjs";

pdfjsLib.GlobalWorkerOptions.workerSrc = "/lib/pdfjs/pdf.worker.min.mjs";

const MAX_PDF_BYTES = 100 * 1024 * 1024;
const COVER_WIDTH = 700;
const COVER_QUALITY = 0.85;

const form = document.getElementById("gazette-form");
const pdfInput = document.getElementById("Pdf");
const coverInput = document.getElementById("Cover");
const submit = document.getElementById("gazette-submit");
const error = document.getElementById("gazette-file-error");
const preview = document.getElementById("gazette-preview");
const previewImage = document.getElementById("gazette-preview-image");
const submitLabel = submit.textContent;

function clearCover() {
    coverInput.value = "";
    preview.hidden = true;
    previewImage.removeAttribute("src");
}

async function createCover(file) {
    const pdf = await pdfjsLib.getDocument({ data: await file.arrayBuffer(), isEvalSupported: false }).promise;
    const page = await pdf.getPage(1);
    const scale = COVER_WIDTH / page.getViewport({ scale: 1 }).width;
    const viewport = page.getViewport({ scale });
    const canvas = document.createElement("canvas");
    canvas.width = Math.floor(viewport.width);
    canvas.height = Math.floor(viewport.height);
    await page.render({ canvasContext: canvas.getContext("2d"), viewport }).promise;
    await pdf.destroy();
    return new Promise((resolve) => canvas.toBlob(resolve, "image/jpeg", COVER_QUALITY));
}

pdfInput.addEventListener("change", async () => {
    const file = pdfInput.files[0];
    error.textContent = "";
    clearCover();
    if (!file) {
        return;
    }
    if (file.size > MAX_PDF_BYTES) {
        error.textContent = "Ce PDF dépasse 100 Mo.";
        pdfInput.value = "";
        return;
    }

    submit.disabled = true;
    submit.textContent = "Préparation de la couverture…";
    try {
        const blob = await createCover(file);
        const transfer = new DataTransfer();
        transfer.items.add(new File([blob], "couverture.jpg", { type: "image/jpeg" }));
        coverInput.files = transfer.files;
        previewImage.src = URL.createObjectURL(blob);
        preview.hidden = false;
    } catch (e) {
        error.textContent = "Ce fichier n'a pas pu être lu comme un PDF.";
        pdfInput.value = "";
    }
    submit.disabled = false;
    submit.textContent = submitLabel;
});

form.addEventListener("submit", () => {
    submit.disabled = true;
    submit.textContent = "Envoi en cours…";
});
