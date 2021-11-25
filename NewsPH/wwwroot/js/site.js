// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function onChangeImage(event) {
    var imagePath = event.target.files[0];
    var selectedImage = null;

    if (imagePath) {
        var reader = new FileReader();

        reader.onload = function (readerEvent) {
            //selectedImage = readerEvent.target.result;
            changeImage(readerEvent.target.result);
        }

        reader.readAsDataURL(event.target.files[0]);
        //changeImage(selectedImage);
    }
}

function changeImage(image) {
    var newsImageElement = document.querySelector(".news-image");
    newsImageElement.setAttribute("src", image);
}