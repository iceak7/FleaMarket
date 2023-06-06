// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function insertSuccessMessage(message) {
    $('main').prepend("<div class='alert alert-success alert-dismissible fade show' role='alert'>" + message + "<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>")
}

function insertErrorMessage(message) {
    $('main').prepend("<div class='alert alert-warning alert-dismissible fade show' role='alert'>" + message + "<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>");
}

function previewImage(url) {
    console.log(url);

    $('body').prepend(`<div class='image-preview-modal' id='image-preview-modal'> 
        <div class='image-preview-image-container'>
            <span class='close-modal'><button type='button' onClick='onCloseImagePreview()' class='btn-close' aria-label='Close'></button></span>
            <img src='${url}'>
        </div>
    </div>`);
}

function onCloseImagePreview(){
    $('#image-preview-modal').remove();
}

function updateMarketItems() {
    $('#market-items-form').submit();
}