$(document).ready(function () {
    console.log("Document is ready!");
    $("#add").submit(function (event) {
        event.preventDefault();
        $.ajax({
            method: 'POST',
            url: "/add",
        })
    });
});