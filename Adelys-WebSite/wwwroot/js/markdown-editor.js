// Éditeur markdown du règlement et des events : mise en forme, aperçu et envoi d'images
document.querySelectorAll(".md-editor").forEach(editor => {
    const input = editor.querySelector(".md-input");
    const preview = editor.querySelector(".md-preview");
    const status = editor.querySelector(".md-status");
    const fileInput = editor.querySelector(".md-file");
    const uploadUrl = editor.dataset.uploadUrl;
    const token = editor.closest("form").querySelector("input[name='__RequestVerificationToken']").value;

    function replaceSelection(before, after, placeholder) {
        const start = input.selectionStart;
        const end = input.selectionEnd;
        const selected = input.value.slice(start, end) || placeholder;
        input.setRangeText(before + selected + after, start, end, "end");
        input.setSelectionRange(start + before.length, start + before.length + selected.length);
        input.focus();
    }

    function prefixLines(prefix) {
        const lineStart = input.value.lastIndexOf("\n", input.selectionStart - 1) + 1;
        const end = input.selectionEnd;
        const lines = input.value.slice(lineStart, end).split("\n").map(line => prefix + line).join("\n");
        input.setRangeText(lines, lineStart, end, "end");
        input.focus();
    }

    function insertText(text) {
        input.focus();
        input.setRangeText(text, input.selectionStart, input.selectionEnd, "end");
        input.dispatchEvent(new Event("input"));
    }

    const actions = {
        bold: () => replaceSelection("**", "**", "texte en gras"),
        italic: () => replaceSelection("*", "*", "texte en italique"),
        heading: () => prefixLines("## "),
        list: () => prefixLines("- "),
        link: () => replaceSelection("[", "](https://)", "texte du lien"),
        image: () => fileInput.click()
    };

    editor.querySelectorAll("[data-md]").forEach(button => {
        button.addEventListener("click", () => actions[button.dataset.md]());
    });

    editor.querySelectorAll("[data-md-tab]").forEach(tab => {
        tab.addEventListener("click", async () => {
            const showPreview = tab.dataset.mdTab === "preview";
            editor.querySelectorAll("[data-md-tab]").forEach(t => t.classList.toggle("is-active", t === tab));
            editor.querySelector(".md-actions").hidden = showPreview;
            input.hidden = showPreview;
            preview.hidden = !showPreview;
            if (!showPreview) {
                return;
            }

            preview.textContent = "Chargement de l'aperçu…";
            const body = new FormData();
            body.append("markdown", input.value);
            try {
                const response = await fetch(editor.dataset.previewUrl, { method: "POST", body, headers: { RequestVerificationToken: token } });
                // Le HTML vient du rendu serveur, qui désactive le HTML brut et filtre les liens
                preview.innerHTML = response.ok ? await response.text() : "";
                if (!response.ok) {
                    preview.textContent = "Aperçu indisponible";
                }
            } catch {
                preview.textContent = "Aperçu indisponible";
            }
        });
    });

    if (!uploadUrl) {
        return;
    }

    let pending = 0;
    const submitButton = editor.closest("form").querySelector("[type='submit']");

    async function upload(file) {
        const marker = `![Envoi de ${file.name || "l'image"}…]()`;
        insertText(`\n${marker}\n`);
        pending++;
        submitButton.disabled = true;
        status.textContent = "";

        const body = new FormData();
        body.append("image", file);
        let replacement = "";
        try {
            const response = await fetch(uploadUrl, { method: "POST", body, headers: { RequestVerificationToken: token } });
            const result = await response.json().catch(() => ({}));
            if (response.ok) {
                const alt = (file.name || "image").replace(/\.[^.]+$/, "").replace(/[[\]]/g, "");
                replacement = `![${alt}](${result.url})`;
            } else {
                status.textContent = result.error || "L'image n'a pas pu être envoyée";
            }
        } catch {
            status.textContent = "L'image n'a pas pu être envoyée";
        }

        input.value = input.value.replace(marker, replacement);
        pending--;
        submitButton.disabled = pending > 0;
    }

    function uploadAll(files) {
        [...files].filter(file => file.type.startsWith("image/")).forEach(upload);
    }

    input.addEventListener("paste", event => {
        const files = [...event.clipboardData.files];
        if (files.some(file => file.type.startsWith("image/"))) {
            event.preventDefault();
            uploadAll(files);
        }
    });

    input.addEventListener("dragover", event => {
        if (event.dataTransfer.types.includes("Files")) {
            event.preventDefault();
            input.classList.add("is-dragging");
        }
    });
    input.addEventListener("dragleave", () => input.classList.remove("is-dragging"));
    input.addEventListener("drop", event => {
        input.classList.remove("is-dragging");
        if (event.dataTransfer.files.length > 0) {
            event.preventDefault();
            uploadAll(event.dataTransfer.files);
        }
    });

    fileInput.addEventListener("change", () => {
        uploadAll(fileInput.files);
        fileInput.value = "";
    });
});
