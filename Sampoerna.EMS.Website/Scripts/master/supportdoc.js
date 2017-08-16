var saveButton = idToElement("SaveButton");
// activate client validation on lost focus
$.validator.setDefaults({
    onfocusout: function (element) {
        $(element).valid();
    }
});

// define element as variable
var checkerElm = idToElement("CheckExistanceLabel");
var companyElm = idToElement("CompanySelector");
var formnameElm = idToElement("FormNameSelector");
var supportdocnameElm = idToElement("SupportDocName");
var activeStateElm = idToElement("ActiveState");
var saveButton = idToElement("SaveButton");

function attachEvents() {

    saveButton.on("click", function () {
        var createForm = document.getElementById("createForm");
        var editForm = document.getElementById("editForm");
        console.log(createForm);
        console.log(editForm);
        if (!createForm)
            editForm.submit();
        else
            createForm.submit();
        $(this).prop("disabled", true);
    });
    $("#ReviseButtonSubmit").on("click", function () {
        $(this).prop("disabled", true);
    })  
}

function log(msg) {
    console.log(msg);
}

function idToElement(id) {
    return $("#" + id);
}

function init() {
    $('#MenuSupportingDocument').addClass('active');
    $("#customloader").hide();
    saveButton.prop("disabled", false);
}