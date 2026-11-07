const playerDialog = document.getElementById('playerDialog');

document.querySelectorAll('.cardPlayer').forEach(card => {
    card.addEventListener('click', () => playerDialog.showModal());
    card.addEventListener('keydown', event => {
        if (event.key === 'Enter' || event.key === ' ') {
            event.preventDefault();
            playerDialog.showModal();
        }
    });
});
