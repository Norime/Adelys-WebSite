// Chaque carte charge sa partie du règlement, l'ancre de l'adresse garde la partie ouverte
const sections = [...document.querySelectorAll("[data-rules-section]")];
const display = document.getElementById("displayInfo");

async function loadSection(name) {
    const link = sections.find(s => s.dataset.rulesSection === name) ?? sections[0];
    sections.forEach(s => s.closest(".card").classList.toggle("is-selected", s === link));
    try {
        const response = await fetch(link.dataset.url);
        display.innerHTML = response.ok ? await response.text() : "<p>Impossible de charger cette partie du règlement.</p>";
    } catch {
        display.innerHTML = "<p>Impossible de charger cette partie du règlement.</p>";
    }
}

window.addEventListener("hashchange", () => loadSection(location.hash.slice(1)));
loadSection(location.hash.slice(1));
