Number.prototype.formatMoney = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

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

// define element as variable
var cAssetsElm = idToElement("CurrentAssetsText");
var cAssetsVal = idToElement("ViewModel_CurrentAssets");
var cDebtsElm = idToElement("CurrentDebtsText");
var cDebtsVal = idToElement("ViewModel_CurrentDebts");
var liquidityRatioElm = idToElement("LiquidityRatioText");

var tAssetsElm = idToElement("TotalAssetsText");
var tAssetsVal = idToElement("ViewModel_TotalAssets");
var tDebtsElm = idToElement("TotalDebtsText");
var tDebtsVal = idToElement("ViewModel_TotalDebts");
var solvabilityRatioElm = idToElement("SolvabilityRatioText");

var nProfitElm = idToElement("NetProfitText");
var nProfitVal = idToElement("ViewModel_NetProfit");
var tCapitalElm = idToElement("TotalCapitalText");
var tCapitalVal = idToElement("ViewModel_TotalCapital");
var rentabiliyRatioElm = idToElement("RentabilityRatioText");

var checkerElm = idToElement("CheckExistanceLabel");
var companyElm = idToElement("CompanySelector");
var periodElm = idToElement("PeriodSelector");

var saveButton = idToElement("SaveButton");

// define helpers
function log(msg) {
    console.log(msg);
}

function idToElement(id) {
    return $("#" + id);
}

function init() {
    $('#MenuFinanceRatio').addClass('active');
    $("#customloader").hide();
    saveButton.prop("disabled", false);
}

function attachEvents() {
    cAssetsElm.on("blur", CalculateLiquidity);
    cDebtsElm.on("blur", CalculateLiquidity);

    tAssetsElm.on("blur", CalculateSolvability);
    tDebtsElm.on("blur", CalculateSolvability);

    nProfitElm.on("blur", CalculateRentability);
    tCapitalElm.on("blur", CalculateRentability);

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

    //companyElm.on("change", CheckRatioExistence);
    //periodElm.on("change", CheckRatioExistence);
}

function inititeAutoNumeric() {
    var temp;
    var cAssets = Number(cAssetsVal.val().replace(',', '.'));
    if (!cAssets || isNaN(cAssets)) {
        temp = toEnCurrency(cAssetsVal.val());
        cAssets = Number(temp);
    }
    cAssetsElm.autoNumeric('init');
    cAssetsElm.autoNumeric('set', cAssets);
    cAssetsVal.val(toIdCurrency(cAssets, 2));

    var cDebts = Number(cDebtsVal.val().replace(',', '.'));
    if (!cDebts || isNaN(cDebts)) {
        var temp = toEnCurrency(cDebtsVal.val());
        cDebts = Number(temp);
    }

    cDebtsElm.autoNumeric('init');
    cDebtsElm.autoNumeric('set', cDebts);
    cDebtsVal.val(toIdCurrency(cDebts, 2));

    var tAssets = Number(tAssetsVal.val().replace(',', '.'));
    if (!tAssets || isNaN(tAssets)) {
        temp = toEnCurrency(tAssetsVal.val());
        tAssets = Number(temp);
    }
    tAssetsElm.autoNumeric('init');
    tAssetsElm.autoNumeric('set', tAssets);
    tAssetsVal.val(toIdCurrency(tAssets, 2));

    var tDebts = Number(tDebtsVal.val().replace(',', '.'));
    if (!tDebts || isNaN(tDebts)) {
        temp = toEnCurrency(tDebtsVal.val());
        tDebts = Number(temp);
    }
    tDebtsElm.autoNumeric('init');
    tDebtsElm.autoNumeric('set', tDebts);
    tDebtsVal.val(toIdCurrency(tDebts, 2));

    var tCapital = Number(tCapitalVal.val().replace(',', '.'));
    if (!tCapital || isNaN(tCapital)) {
        temp = toEnCurrency(tCapitalVal.val());
        tCapital = Number(temp);
    }
    tCapitalElm.autoNumeric('init');
    tCapitalElm.autoNumeric('set', tCapital);
    tCapitalVal.val(toIdCurrency(tCapital, 2));

    var nProfit = Number(nProfitVal.val().replace(',', '.'));
    if (!nProfit || isNaN(nProfit)) {
        temp = toEnCurrency(nProfitVal.val());
        nProfit = Number(temp);
    }
    nProfitElm.autoNumeric('init');
    nProfitElm.autoNumeric('set', nProfit);
    nProfitVal.val(toIdCurrency(nProfit, 2));
}

function detachEvents() {
    cAssetsElm.off("blur", CalculateLiquidity);
    cDebtsElm.off("blur", CalculateLiquidity);

    tAssetsElm.off("blur", CalculateSolvability);
    tDebtsElm.off("blur", CalculateSolvability);

    nProfitElm.off("blur", CalculateRentability);
    tCapitalElm.off("blur", CalculateRentability);

    //companyElm.off("change", CheckRatioExistence);
    //periodElm.off("change", CheckRatioExistence);
}

function CalculateLiquidity() {
    var cAssets = cAssetsElm.autoNumeric('get');
    var cDebts = cDebtsElm.autoNumeric('get');
    cAssetsVal.val(Number(cAssets));
    cDebtsVal.val(Number(cDebts));

    liquidityRatioElm.focus();
    PlaceCalculationResult(cAssetsVal, cDebtsVal, liquidityRatioElm);
    liquidityRatioElm.blur();
}

function CalculateSolvability() {
    var tAssets = tAssetsElm.autoNumeric('get');
    var tDebts = tDebtsElm.autoNumeric('get');
    tAssetsVal.val(Number(tAssets));
    tDebtsVal.val(Number(tDebts));

    solvabilityRatioElm.focus();
    PlaceCalculationResult(tAssetsVal, tDebtsVal, solvabilityRatioElm);
    solvabilityRatioElm.blur();
}

function CalculateRentability() {
    var nProfit = nProfitElm.autoNumeric('get');
    var tCapital = tCapitalElm.autoNumeric('get');
    nProfitVal.val(Number(nProfit));
    tCapitalVal.val(Number(tCapital));

    rentabiliyRatioElm.focus();
    PlaceCalculationResult(nProfitVal, tCapitalVal, rentabiliyRatioElm);
    rentabiliyRatioElm.blur();
}

function PlaceCalculationResult(numeratorElm, denominatorElm, resultElm) {

    var num = numeratorElm.val();
    var denom = denominatorElm.val();
    var result = Calculate(num, denom);
    numeratorElm.val(toIdCurrency(num, 2));
    denominatorElm.val(toIdCurrency(denom, 2));
    console.log("num: " + num);
    console.log("denom: " + denom);
    console.log("#numeratorElm: " + numeratorElm.val());
    console.log("#denominatorElm: " + denominatorElm.val());
    log("result: " + result);
    var resultText = toIdCurrency(result, 2);
    console.log("Calculate: " + resultText);
    if (!isNaN(result))
        resultElm.val(resultText);
    else
        resultElm.val(0);
}

function Calculate(numerator, denominator) {
    // checks null or undefined
    if (!numerator || !denominator) {
        return NaN;
    }

    // checks NaN values
    if (isNaN(numerator) || isNaN(denominator)) {
        return NaN;
    }

    // checks less or equal 0
    if (Number(numerator) <= 0 || Number(denominator) <= 0) {
        return NaN;
    }

    // valid input, begin to calculate
    var result = numerator / denominator;

    // checks if result invalid for some reason
    if (!result || isNaN(result)) {
        return NaN;
    }

    // this one the expected result
    return result;
}
