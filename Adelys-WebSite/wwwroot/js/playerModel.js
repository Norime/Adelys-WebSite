//window.onload = (event) => {
//    var uuid = "";
//    $.ajax({
//        url: "/Players/GetUserSkin",
//        type: "GET",
//        datatype: "json",
//        data: {
//            //'UUID' : uuid,
//        },
//        beforeSend: function () {

//        },
//        success: function (result) {
//            //createModel(playerSkinUrl, imgElementId);
//        },
//        error: function (xmlHttpRequest, textStatus, errorThrown) {

//        },
//        complete: function () {

//        }
//    })
//};

function createModel(playerSkinUrl, imgElementId) {
    // Chargement de l'image du skin
    var textureLoader = new THREE.TextureLoader();
    var skinTexture = textureLoader.load(playerSkinUrl);

    // Création du modèle du personnage
    var geometry = new THREE.BoxGeometry(1, 2, 0.5);
    var material = new THREE.MeshBasicMaterial({ map: skinTexture });
    var characterModel = new THREE.Mesh(geometry, material);
    scene.add(characterModel);

    // Récupération de l'élément <img> par son identifiant
    var imgElement = document.getElementById(imgElementId);

    // Remplacement de l'élément <img> par le modèle 3D
    imgElement.parentNode.replaceChild(renderer.domElement, imgElement);
    //animate();
};


function animate() {
    // Boucle de rendu
    requestAnimationFrame(animate);
    characterModel.rotation.x += 0.01;
    characterModel.rotation.y += 0.01;
    renderer.render(scene, camera);
}

