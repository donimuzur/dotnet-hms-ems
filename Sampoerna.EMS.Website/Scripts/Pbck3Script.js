
function ValidatePbck3Form() {
    var result = true;

 
    if ($('#PBCK3_DATE').val() == '') {
        AddValidationClass(false, 'PBCK3_DATE');
        result = false;
    }

 
    if ($('#EXEC_DATE_FROM').val() == '') {
        AddValidationClass(false, 'EXEC_DATE_FROM');
        result = false;
    }

    if ($('#EXEC_DATE_TO').val() == '') {
        AddValidationClass(false, 'EXEC_DATE_TO');
        result = false;
    }


    if (!result) {
        $('#collapseSix').removeClass('collapse');
        $('#collapseSix').addClass('in');
        $("#collapseSix").css({ height: "auto" });
    }


    return result;
}


function ValidateGovInput() {
    var result = true;

  
    if ($('#Pbck3GovStatus').val() == '') {
        AddValidationClass(false, 'Pbck3GovStatus');
        result = false;

        $('#Pbck3GovStatus').focus();
    } else {
        if ($('#Pbck3GovStatus').val() == 'Rejected') {
            if ($('#Comment').val() == '') {
                AddValidationClass(false, 'Comment');
                result = false;

                $('#Comment').focus();
            }
        }

    }

    if (result == false) {
        $('#collapseSix').removeClass('collapse');
        $('#collapseSix').addClass('in');
        $("#collapseSix").css({ height: "auto" });

    }

    if ($('#Ck2Value').val() != '') {
        if ($.isNumeric($('#Ck2Value').val()) == false) {
            AddValidationClass(false, 'Ck2Value');
            result = false;
        }

        var ck2Value = parseFloat($('#Ck2Value').val());
        if (ck2Value <= 0) {
            AddValidationClass(false, 'Ck2Value');
            result = false;
        }
    }
    if (result == false) {
        $('#collapseFive').removeClass('collapse');
        $('#collapseFive').addClass('in');
        $("#collapseFive").css({ height: "auto" });

    }
    
    return result;
}

function AddValidationClass(isValid, objName) {
    if (isValid) {
        $('#' + objName).removeClass('input-validation-error');
        $('#' + objName).addClass('valid');
    } else {
        $('#' + objName).removeClass('valid');
        $('#' + objName).addClass('input-validation-error');
    }
}