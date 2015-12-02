function ChangeNumber(value) {
    var realValue = value;
    var addOns = "";

    if (value > 9999 && value < 10000000) {
        realValue = (value / 1000);
        addOns = "K";
    }
    else if (value > 9999999) {
        realValue = (value / 1000000);
        addOns = "M";
    }

    var total = parseInt(realValue, 10);
    realValue = ThausandSeperator(total, 0) + addOns;

    return realValue;
}

function ChangeSeparatorTooltip() {
    var tooltipDraft = $('#divDraft').attr("data-original-title");
    var tooltipWaitA = $('#divWaitA').attr("data-original-title");
    var tooltipWaitG = $('#divWaitG').attr("data-original-title");
    var tooltipComp = $('#divComp').attr("data-original-title");

    $('#divDraft').attr("data-original-title", ThausandSeperator(tooltipDraft, 0));
    $('#divWaitA').attr("data-original-title", ThausandSeperator(tooltipWaitA, 0));
    $('#divWaitG').attr("data-original-title", ThausandSeperator(tooltipWaitG, 0));
    $('#divComp').attr("data-original-title", ThausandSeperator(tooltipComp, 0));
}

function ChangeNumberStatus() {
    $('.changeNumber1').text(ChangeNumber($('.changeNumber1').text()));
    $('.changeNumber2').text(ChangeNumber($('.changeNumber2').text()));
    $('.changeNumber3').text(ChangeNumber($('.changeNumber3').text()));
    $('.changeNumber4').text(ChangeNumber($('.changeNumber4').text()));
}