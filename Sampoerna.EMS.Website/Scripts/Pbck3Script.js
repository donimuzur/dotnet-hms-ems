﻿function ValidateGovInput() {
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