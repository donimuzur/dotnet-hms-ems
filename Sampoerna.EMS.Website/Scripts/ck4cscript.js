function ValidateGovInput() {
    var result = true;
    AddValidationClass(true, 'Details_PlantId');
    AddValidationClass(true, 'Details_NppbkcId');
    AddValidationClass(true, 'Details_ReportedOn');

    if ($('#Details_CompanyId').val() == '') {
        AddValidationClass(false, 'Details_CompanyId');
        result = false;
    }

    if ($('#Details_ReportedOn').val() == '') {
        AddValidationClass(false, 'Details_ReportedOn');
        result = false;
    }

    if ($('#Details_ReportedPeriod').val() == '') {
        AddValidationClass(false, 'Details_ReportedPeriod');
        result = false;
    }

    if ($('#Details_ReportedMonth').val() == '') {
        AddValidationClass(false, 'Details_ReportedMonth');
        result = false;
    }

    if ($('#Details_ReportedYears').val() == '') {
        AddValidationClass(false, 'Details_ReportedYears');
        result = false;
    }

    if ($('#Details_PlantId').is( ":disabled" )) {
        if ($('#Details_NppbkcId').val() == '') {
            AddValidationClass(false, 'Details_NppbkcId');
            result = false;
        }
    }

    if ($('#Details_NppbkcId').is(":disabled")) {
        if ($('#Details_PlantId').val() == '') {
            AddValidationClass(false, 'Details_PlantId');
            result = false;
        }
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