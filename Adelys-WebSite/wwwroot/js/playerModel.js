function createModel(playerSkinUrl) {
    // Création de la scène
    var scene = new THREE.Scene();

    // Création de la caméra
    var camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
    camera.position.z = 5;

    // Chargement de l'image du skin
    var textureLoader = new THREE.TextureLoader();
    var skinUrl = "http://textures.minecraft.net/texture/855c0e2d7c8c28755206390c209be7e6708a51f3744464f8283bcf20ac6c859f";
    var skinTexture = textureLoader.load(skinUrl);

    // Création du modèle du personnage
    var geometry = new THREE.BoxGeometry(1, 2, 0.5);
    var material = new THREE.MeshBasicMaterial({ map: skinTexture });
    var characterModel = new THREE.Mesh(geometry, material);
    scene.add(characterModel);

    // Configuration du moteur de rendu
    var renderer = new THREE.WebGLRenderer();
    renderer.setSize(window.innerWidth, window.innerHeight);
    document.body.appendChild(renderer.domElement);

    // Boucle de rendu
    function animate() {
        requestAnimationFrame(animate);
        characterModel.rotation.x += 0.01;
        characterModel.rotation.y += 0.01;
        renderer.render(scene, camera);
    }
    animate();
}

