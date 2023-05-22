﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function insertSuccessMessage(message) {
    $('main').prepend("<div class='alert alert-success alert-dismissible fade show' role='alert'>" + message + "<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>")
}

function insertErrorMessage(message) {
    $('main').prepend("<div class='alert alert-warning alert-dismissible fade show' role='alert'>" + message + "<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>");
}