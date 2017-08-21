function toIdCurrency(val, precision) {
    var data = Number(val).toFixed(precision).toString();

    //data = data.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
    //data = data.split(".").join("_");
    //data = data.split(",").join(".");
    //data = data.split("_").join(",");

    return data;
}

function toEnCurrency(data) {
    data = data.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
    data = data.split(".").join("");
    data = data.split(",").join(".");

    return data;
}
// activate client validation on lost focus
$.validator.setDefaults({
    onfocusout: function (element) {
        $(element).valid();
    }
});

// define helpers
function log(msg) {
    console.log(msg);
}

function idToElement(id) {
    return $("#" + id);
}

function init() {
    $('#MenuMasterTariff').addClass('active');
    $("#customloader").hide();
    $('#ApproveButtonConfirm').prop("disabled", true);
    $('#ReviseButtonSubmit').prop("disabled", true);
}

function inititeAutoNumeric() {
    var temp;
    var hjeMin = Number(hjeFromVal.val().replace(',', '.'));

    if (!hjeMin || isNaN(hjeMin)) {
        temp = toEnCurrency(hjeFromVal.val());
        hjeMin = Number(temp);
    }
    hjeFromElm.autoNumeric('init');
    hjeFromElm.autoNumeric('set', hjeMin);
    hjeFromVal.val(toIdCurrency(hjeMin, 2));

    var hjeMax = Number(hjeToVal.val().replace(',', '.'));
    if (!hjeMax || isNaN(hjeMax)) {
        temp = toEnCurrency(hjeToVal.val());
        hjeMax = Number(temp);
    }
    hjeToElm.autoNumeric('init');
    hjeToElm.autoNumeric('set', hjeMax);
    hjeToVal.val(toIdCurrency(hjeMax, 2));

    var tariff = Number(tariffVal.val().replace(',', '.'));
    if (!tariff || isNaN(tariff)) {
        temp = toEnCurrency(tariffVal.val());
        tariff = Number(temp);
    }
    tariffElm.autoNumeric('init');
    tariffElm.autoNumeric('set', tariff);
    tariffVal.val(toIdCurrency(tariff, 2));
}

function mapValues() {
    var hjeFrom = hjeFromElm.autoNumeric('get');
    hjeFromVal.val(toIdCurrency(Number(hjeFrom), 2));
    var hjeTo = hjeToElm.autoNumeric('get');
    hjeToVal.val(toIdCurrency(Number(hjeTo), 2));
    var tariff = tariffElm.autoNumeric('get');
    tariffVal.val(toIdCurrency(Number(tariff), 2));
    //console.log("HJE From: " + hjeFrom);
    //console.log("HJE From Val: " + hjeFromVal.val());
    //console.log("HJE To: " + hjeTo);
    //console.log("Tariff: " + tariff);
    
}

function attachEvents() {
    hjeFromElm.on('blur', mapValues);
    hjeToElm.on('blur', mapValues);
    tariffElm.on('blur', mapValues);
    saveButton.on("click", function (e) {
        e.preventDefault();
        mapValues();
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
}

function detachEvents() {
    hjeFromElm.off('blur', mapValues);
    hjeToElm.off('blur', mapValues);
    tariffElm.off('blur', mapValues);

}

var hjeFromElm = idToElement("MinimumHje");
var hjeToElm = idToElement("MaximumHje");
var tariffElm = idToElement("Tariff");
var saveButton = idToElement("SaveButton");

var hjeFromVal = idToElement("ViewModel_MinimumHJE");
var hjeToVal = idToElement("ViewModel_MaximumHJE");
var tariffVal = idToElement("ViewModel_Tariff");



