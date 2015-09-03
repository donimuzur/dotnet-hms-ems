function generateDataClick(lackLevel, url) {
    if (!generateInputValidation()) {
        $('#ModalValidation').modal('show');
        return;
    }

    var param = {};
    param.CompanyCode = $('#Bukrs').find("option:selected").val();
    param.Lack1Level = lackLevel;
    param.NppbkcId = $('#NppbkcId').find("option:selected").val();
    param.ExcisableGoodsType = $('#ExGoodsTypeId').find("option:selected").val();
    param.ExcisableGoodsTypeDesc = $('#ExGoodsType').find("option:selected").text();
    param.SupplierPlantId = $('#SupplierPlantId').find("option:selected").val();
    param.WasteAmount = parseFloat($('#WasteQty').val());
    param.WasteAmountUom = $('#WasteUom').find("option:selected").val();
    param.ReturnAmount = parseFloat($('#ReturnQty').val());
    param.ReturnAmountUom = $('#ReturnUom').find("option:selected").val();
    param.Noted = $('#Noted').val();
    param.CompanyName = $('#Bukrs').find("option:selected").text();

    var plantAttr = $('#LevelPlantId').attr('disabled');
    /* For some browsers, `attr` is undefined; for others,
     `attr` is false.  Check for both.
    load plant if enable*/
    if (!(typeof plantAttr !== typeof undefined && plantAttr !== false)) {
        param.ReceivedPlantId = $('#LevelPlantId').find("option:selected").val();
    }

    param.PeriodMonth = parseInt($('#PeriodMonth').find("option:selected").val());
    param.PeriodYear = parseInt($('#PeriodYears').find("option:selected").val());

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ param: param }),
        success: function (response) {
            if (response.Success) {
                $('#generated-data-container').html("");
                var data = response.Data;
                var tableGenerated = generateTable(data);
                /*console.log(tableGenerated);*/
                $('#generated-data-container').append(tableGenerated);
            } else {
                /*alert(response.ErrorMessage);*/
                $('#modalBodyMessage').text(response.ErrorMessage);
                $('#ModalValidation').modal('show');
            }
        }
    });
}

function generateInputValidation() {
    var rc = true;
    if ($('#Bukrs').find("option:selected").val() == '') {
        rc = false;
        $('#Bukrs').addClass('input-validation-error');
    }
    if ($('#NppbkcId').find("option:selected").val() == '') {
        rc = false;
        $('#NppbkcId').addClass('input-validation-error');
    }
    if ($('#ExGoodsTypeId').find("option:selected").val() == '') {
        rc = false;
        $('#ExGoodsTypeId').addClass('input-validation-error');
    }
    if ($('#SupplierPlantId').find("option:selected").val() == '') {
        rc = false;
        $('#SupplierPlantId').addClass('input-validation-error');
    }
    if ($('#PeriodMonth').find("option:selected").val() == '') {
        rc = false;
        $('#PeriodMonth').addClass('input-validation-error');
    }
    if ($('#PeriodYears').find("option:selected").val() == '') {
        rc = false;
        $('#PeriodYears').addClass('input-validation-error');
    }
    if ($('#WasteUom').find("option:selected").val() == '') {
        rc = false;
        $('#WasteUom').addClass('input-validation-error');
    }
    if ($('#ReturnUom').find("option:selected").val() == '') {
        rc = false;
        $('#ReturnUom').addClass('input-validation-error');
    }

    var plantAttr = $('#LevelPlantId').attr('disabled');
    /* For some browsers, `attr` is undefined; for others,
     `attr` is false.  Check for both.
    load plant if enable*/
    if (!(typeof plantAttr !== typeof undefined && plantAttr !== false)) {
        if ($('#LevelPlantId').find("option:selected").val() == '') {
            rc = false;
            $('#LevelPlantId').addClass('input-validation-error');
        }
    }
    if (!rc) {
        $('#modalBodyMessage').text('Please input the required field on selection criteria input.');
    }
    return rc;
}

function saveClick() {
    var isValid = false;
    if (generateInputValidation()) {
        isValid = true;
    }
    if (isValid) {
        isValid = saveInputValidation();
    }
    if (isValid) {
        isValid = checkJumlahProduksi();
    }
    if (isValid) {
        isValid = checkIncomeListItem();
    }
    if (isValid) {
        $('#CreateLack1Form').submit();
    } else {
        $('#ModalValidation').modal('show');
    }
}

function saveInputValidation() {
    var rc = true;
    if ($('#SubmissionDate').val() == '')
        rc = false;
    $('#SubmissionDate').addClass('input-validation-error');
    if (!rc) {
        $('#modalBodyMessage').text('Please input the required field on selection criteria input.');
    }
    return rc;
}

function checkJumlahProduksi() {
    if ($('#jumlah-hasil-produksi-data').length == 0) {
        $('#modalBodyMessage').text('Missing Hasil Produksi BKC');
        return false;
    }
    return true;
}

function checkIncomeListItem() {
    if ($('#IncomeListCount').length == 0 || $('#IncomeListCount').val() == '0') {
        $('#modalBodyMessage').text('Missing Income List Item');
        return false;
    }
    return true;
}