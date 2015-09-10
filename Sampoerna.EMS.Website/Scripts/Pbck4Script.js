function AddValidationClass(isValid, objName) {
    if (isValid) {
        $('#' + objName).removeClass('input-validation-error');
        $('#' + objName).addClass('valid');
    } else {
        $('#' + objName).removeClass('valid');
        $('#' + objName).addClass('input-validation-error');
    }
}

function ValidatePbck4Form() {
    var result = true;
    if ($('#PlantId').find("option:selected").val() == '') {
        AddValidationClass(false, 'SourcePlantId');
        result = false;
     
        $('#collapseOne').removeClass('collapse');
        $('#collapseOne').addClass('in');
        $("#collapseOne").css({ height: "auto" });
    }

    if ($('#ReportedOn').val() == '') {
        AddValidationClass(false, 'ReportedOn');
        result = false;

        $('#collapseOne').removeClass('collapse');
        $('#collapseOne').addClass('in');
        $("#collapseOne").css({ height: "auto" });

    }
   
    //if (result) {
    //    var rowCount = $('#ck5TableItem tr').length;

    //    if (rowCount <= 1) {
    //        // alert('Missing CK5 Material');
    //        $('#modalBodyMessage').text('Missing CK5 Materials');
    //        $('#ModalCk5Material').modal('show');

    //        $('#home-tab').removeClass('active');
    //        $('#upload-tab').addClass('active');

    //        $('#information').removeClass('active');
    //        $('#upload').addClass('active');

    //        result = false;
    //    }

    //    rowCount = $('#Ck5UploadTable tr').length;

    //    if (rowCount <= 1) {
    //        // alert('Missing CK5 Material');
    //        $('#modalBodyMessage').text('Missing CK5 Materials');
    //        $('#ModalCk5Material').modal('show');

    //        $('#home-tab').removeClass('active');
    //        $('#upload-tab').addClass('active');

    //        $('#information').removeClass('active');
    //        $('#upload').addClass('active');

    //        result = false;
    //    }

    //}
   
   
    return result;
}

function ajaxGetPlantDetails(url, formData) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                $("input[name='CompanyName']").val(data.CompanyName);
                $("input[name='CompanyId']").val(data.CompanyName);
                $("*[name='PlantDesc']").val(data.PlantDesc);
                $("input[name='Poa']").val(data.Poa);
                $("input[name='NppbkcId']").val(data.NppbkcId);
                $("input[name='NppbkcDesc']").val(data.NppbkcDescription);
                
            }
        });
    }
}
