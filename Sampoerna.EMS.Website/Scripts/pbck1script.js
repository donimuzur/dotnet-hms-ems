jQuery.validator.addMethod("greaterThan",
function (value, element, params) {
    if (!/Invalid|NaN/.test(new Date(value))) {
        return new Date(value) > new Date($(params).val());
    }

    return isNaN(value) && isNaN($(params).val())
        || (Number(value) > Number($(params).val()));
}, 'Must be greater than {0}.');

function setUpload() {
    if ($("#Detail_GoodType").val() == "") {
        $("#btn-prod-conv-upload").prop("disabled", true);
        $("#ProdConvExcelfile").prop("disabled", true);
        $("#btn-prod-plan-upload").prop("disabled", true);
        $("#ProdPlanExcelfile").prop("disabled", true);
    } else {
        $("#btn-prod-conv-upload").prop("disabled", false);
        $("#ProdConvExcelfile").prop("disabled", false);
        $("#btn-prod-plan-upload").prop("disabled", false);
        $("#ProdPlanExcelfile").prop("disabled", false);
    }
}

function setSupplierPlantEmpty() {
    $('#Detail_SupplierNppbkcId').val('');
    $('#Detail_SupplierKppbcId').val('');
    $('#Detail_SupplierPhone').val('');
    $('#Detail_SupplierAddress').val('');
    $('#Detail_HiddendSupplierAddress').val('');
    $('#Detail_HiddenSupplierKppbcId').val('');
    $('#Detail_HiddenSupplierNppbkcId').val('');
    $('#Detail_SupplierPlant').val('');
    $('#Detail_SupplierPlantWerks').val('');
    $('#Detail_SupplierKppbcName').val('');
    $('#Detail_SupplierCompany').val('');
}

function ajaxLoadDetailSupplierPlant(formData, url) {
    $.ajax({
        type: 'POST',
        data: formData,
        url: url,
        success: function (data) {

            if (data != null) {
                //load to form
                $('#Detail_SupplierPlantWerks').val(data.Werks);
                $('#Detail_SupplierNppbkcId').val(data.NPPBKC_ID);
                $('#Detail_HiddenSupplierNppbkcId').val(data.NPPBKC_ID);
                $('#Detail_HiddenSupplierKppbcId').val(data.KPPBC_NAME);
                $('#Detail_SupplierKppbcId').val(data.KPPBC_NO);
                $('#Detail_SupplierKppbcName').val(data.KPPBC_NAME);
                $('#Detail_SupplierPhone').val(data.Phone);
                $('#Detail_SupplierAddress').val(data.Address);
                $('#Detail_HiddendSupplierAddress').val(data.Address);
                $('#Detail_SupplierPlant').val(data.Name1);
                $('#Detail_SupplierCompany').val(data.SUPPLIER_COMPANY);
            } else {
                setSupplierPlantEmpty();
            }
            getReference();
        }
    });
}

function disableSupplierFormInput(isDisable) {
    $('#Detail_SupplierNppbkcId').prop('readonly', isDisable);
    $('#Detail_SupplierKppbcName').prop('readonly', isDisable);
    $('#Detail_SupplierAddress').prop('readonly', isDisable);
    $('#Detail_SupplierPhone').prop('readonly', isDisable);
    $('#Detail_SupplierCompany').prop('readonly', isDisable);
}

function supplierChange(isNppbkcImport, url) {
    if ($("#Detail_SupplierPlant_ddl").length) {
        var plantid = $("#Detail_SupplierPlant_ddl").find("option:selected").val();
        var plantidFirst = $("#Detail_SupplierPlant_ddl").find("option:first").val();
        console.log(plantid);
        if (plantid == undefined)
            plantid = plantidFirst;
        ajaxLoadDetailSupplierPlant({ plantid: plantid, isNppbkcImport: isNppbkcImport }, url);
    }
}

function goodTypeOnChange() {
    if ($("#Detail_GoodType").length) {
        var goodTypeName = $("#Detail_GoodType").find("option:selected").text();
        goodTypeName = goodTypeName.substr(3);
        $('#Detail_GoodTypeDesc').val(goodTypeName);
    }
    prodPlanClear();
    getReference();
}

function IsProdPlanValid() {

    var datarows = GetTableData($('#prod-plan-table'));

    for (var i = 0; i < datarows.length; i++) {
        if (datarows[i][9].length > 0)
            return false;
    }

    return true;
}

function IsProdConverterValid() {

    var datarows = GetTableData($('#prod-conv-table'));

    for (var i = 0; i < datarows.length; i++) {
        if (datarows[i][7].length > 0)
            return false;
    }

    return true;
}

function prodConvSaveClick() {
    var datarows = GetTableData($('#prod-conv-table'));
    var columnLength = $('#Detail_Pbck1ProdConverter').find("thead tr:first th").length;
    $('#Detail_Pbck1ProdConverter tbody').html('');
    
    for (var i = 0; i < datarows.length; i++) {
        var data = '<tr>';
        if (columnLength > 0) {
            data += '<td style="display:none"><input name="Detail.Pbck1ProdConverter[' + i
                + '].ProductCode" type="hidden" value = "' + datarows[i][0] + '" />' + datarows[i][0] + '</td>';
            data += '<td style="display:none"><input name="Detail.Pbck1ProdConverter[' + i
                + '].ConverterUom" type="hidden" value = "' + datarows[i][1] + '" />' + datarows[i][1] + '</td>';
            data += '<td><input name="Detail.Pbck1ProdConverter[' + i
                + '].ProdTypeAlias" type="hidden" value = "' + datarows[i][2] + '" />' + datarows[i][2] + '</td>';
            data += '<td><input name="Detail.Pbck1ProdConverter[' + i
                + '].ProdTypeName" type="hidden" value = "' + datarows[i][3] + '" />' + datarows[i][3] + '</td>';
            data += '<td><input name="Detail.Pbck1ProdConverter[' + i
                + '].BrandCE" type="hidden" value = "' + datarows[i][4] + '" />' + datarows[i][4] + '</td>';
            data += '<td class="number"><input name="Detail.Pbck1ProdConverter[' + i
                + '].ConverterOutput" type="hidden" value = "' + changeToNumber(datarows[i][5]) + '" />' + datarows[i][5] + '</td>';
            data += '<td><input name="Detail.Pbck1ProdConverter[' + i
                + '].ConverterUomId" type="hidden" value = "' + datarows[i][6] + '" />' + datarows[i][6] + '</td>';

        }
        
        $('#Detail_Pbck1ProdConverter tbody').append(data);
    }
    
    $('#prod-conv-upload-tab').removeClass('active');
    $('#home-tab').addClass('active');
    $('#home').addClass('active');
    $('#upload').removeClass('active');
    $('#collapse6').addClass("in");
}

function prodPlanSaveClick() {
    var datarows = GetTableData($('#prod-plan-table'));
    var columnLength = $('#Detail_Pbck1ProdPlan').find("thead tr:first th").length;
    $('#Detail_Pbck1ProdPlan tbody').html('');

    var total = 0;
    var uom = '';

    for (var i = 0; i < datarows.length; i++) {
        var data = '<tr>';
        if (columnLength > 0) {
            data += '<td style="display:none"><input name="Detail.Pbck1ProdPlan[' + i + '].ProductCode" type="hidden" value = "'
                + datarows[i][0] + '" />' + datarows[i][0] + '</td>';
            data += '<td style="display:none"><input name="Detail.Pbck1ProdPlan[' + i + '].ProdTypeName" type="hidden" value = "'
                + datarows[i][1] + '" />' + datarows[i][1] + '</td>';
            data += '<td style="display:none"><input name="Detail.Pbck1ProdPlan[' + i + '].Month" type="hidden" value = "'
                + datarows[i][2] + '" />' + datarows[i][2] + '</td>';
            data += '<td><input name="Detail.Pbck1ProdPlan[' + i + '].MonthName" type="hidden" value = "'
                + datarows[i][3] + '" />' + datarows[i][3] + '</td>';
            data += '<td><input name="Detail.Pbck1ProdPlan[' + i + '].ProdTypeAlias" type="hidden" value = "'
                + datarows[i][4] + '" />' + datarows[i][4] + '</td>';
            data += '<td class="number"><input name="Detail.Pbck1ProdPlan[' + i + '].Amount" type="hidden" value = "'
                + changeToNumber(datarows[i][5]) + '" />' + datarows[i][5] + '</td>';
            data += '<td class="number"><input name="Detail.Pbck1ProdPlan[' + i + '].BkcRequired" type="hidden" value = "'
                + changeToNumber(datarows[i][6]) + '" />' + datarows[i][6] + '</td>';
            data += '<td><input name="Detail.Pbck1ProdPlan[' + i + '].BkcRequiredUomId" type="hidden" value = "'
                + datarows[i][7] + '" />' + datarows[i][7] + '</td>';
            data += '<td style="display:none"><input name="Detail.Pbck1ProdPlan[' + i + '].BkcRequiredUomName" type="hidden" value = "'
                + datarows[i][8] + '" />' + datarows[i][8] + '</td>';
            
            total += changeToNumber(datarows[i][6]);
            uom = datarows[i][7];
        }
        data += '</tr>';
        $('#Detail_Pbck1ProdPlan tbody').append(data);
    }
    $("input[name='Detail.RequestQty']:hidden").val(total);

    var request = parseFloat(Math.round(total * 100) / 100).toFixed(2);
    $("input[name='Detail.RequestQty']:text").val(ThausandSeperator(request, 2));

    $("select[name='Detail.RequestQtyUomId']").val(uom);
    $("select[name='Detail.LatestSaldoUomId']").val(uom);
    $("input[name='Detail.RequestQtyUomId']").val(uom);
    $("input[name='Detail.LatestSaldoUomId']").val(uom);
    $('#prod-plan-upload-tab').removeClass('active');
    $('#home-tab').addClass('active');
    $('#home').addClass('active');
    $('#messages').removeClass('active');
    $('#collapse5').addClass("in");
}

function prodConvGenerateClick(url) {
    var fileName = $('[name="ProdConvExcelfile"]').val().trim();
    var pos = fileName.lastIndexOf('.');
    var extension = (pos <= 0) ? '' : fileName.substring(pos);
    if (extension != '.xlsx') {
        alert('Please browse a correct excel file to upload');
        return false;
    }
    var formData = new FormData();
    var totalFiles = document.getElementById("ProdConvExcelfile").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("ProdConvExcelfile").files[i];
        formData.append("prodConvExcelFile", file);
    }

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        cache: false,
        dataType: 'html',
        processData: false, // Don't process the files
        contentType: false, // Set content type to false as jQuery will tell the server its a query string request
        success: function (response) {
            $('#ProdConvContent').html("");
            $('#ProdConvContent').html(response);
            if (IsProdConverterValid()) {
                //valid generated
                $('#prod-conv-save').removeAttr('disabled');
            } else {
                //invalid generated
                $('#prod-conv-save').attr('disabled', 'disabled');
            }
            changeToDecimalMaxFour('#ProdConvContent .decimal', 'html');
        },
        error: function (error) {
            // Handle errors here
            console.log('ERRORS: ' + error);
            // STOP LOADING SPINNER
        }
    });
    return false;
}

function prodPlanGenerateClick(url) {
    var fileName = $('[name="ProdPlanExcelfile"]').val().trim();
    var pos = fileName.lastIndexOf('.');
    var extension = (pos <= 0) ? '' : fileName.substring(pos);
    if (extension != '.xlsx') {
        alert('Please browse a correct excel file to upload');
        return false;
    }

    var formData = new FormData();
    var totalFiles = document.getElementById("ProdPlanExcelfile").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("ProdPlanExcelfile").files[i];
        console.log(file);
        formData.append("prodPlanExcelFile", file);
    }
    formData.append("goodType", $("#Detail_GoodType").val());

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        cache: false,
        dataType: 'html',
        processData: false, // Don't process the files
        contentType: false, // Set content type to false as jQuery will tell the server its a query string request
        success: function (response) {
            $('#ProdPlanContent').html("");
            $('#ProdPlanContent').html(response);
            if (IsProdPlanValid()) {
                //valid generated
                $('#prod-plan-save').removeAttr('disabled');
            } else {
                //invalid generated
                $('#prod-plan-save').attr('disabled', 'disabled');
            }
            changeToDecimal('#ProdPlanContent .decimal', 'html');
        },
        error: function (error) {
            // Handle errors here
            console.log('ERRORS: ' + error);
            // STOP LOADING SPINNER
        }
    });
    return false;
}

function prodPlanClear() {
    var html_upload = "<table id=\"prod-plan-table\" class=\"table table-bordered table-striped js-options-table\"> \
    <thead> \
        <tr> \
            <th style=\"display: none\"></th> \
            <th style=\"display: none\"></th> \
            <th style=\"display: none\"></th> \
            <th>Month</th> \
            <th>Product Type Alias</th> \
            <th>Amount</th> \
            <th>BKC Required</th> \
            <th>BKC Required Uom</th> \
            <th style=\"display: none\"></th> \
            <th>Message Error</th> \
        </tr> \
    </thead>";

    $('#prod-plan-save').attr('disabled', 'disabled');
    
    $('input[name="Detail.RequestQty"]:text').val("0.00");
    $('input[name="Detail.RequestQty"]:hidden').val("");
    $('#ProdPlanContent').html("");
    $('#ProdPlanContent').html(html_upload);
    $('#Detail_Pbck1ProdPlan tbody').html('');
}

function pbck1TypeOnchange() {
    if ($("select#Detail_Pbck1Type").length) {
        var pbck1Type = $("#Detail_Pbck1Type").find("option:selected").val();
        if (pbck1Type == '' || pbck1Type.toLowerCase() == 'new') {
            $('input[name="Detail.Pbck1ReferenceNumber"]:text').val("");
            $('input[name="Detail.Pbck1Reference"]:hidden').val("");
            $('input[name="Detail.Pbck1Reference"]:hidden').prop('disabled', true);
        } else {
            $('input[name="Detail.Pbck1Reference"]:hidden').prop('disabled', false);
        }
        getReference();
    }
}

function checkReference() {
    var pbck1Type = $("#Detail_Pbck1Type").find("option:selected").val();
    if (($(pbck1Type == '' || pbck1Type.toLowerCase() == 'new') && 'input[name="Detail.Pbck1Reference"]:hidden').val() == "")
        return false;
    return true;
}

function btnProdConvUploadClick() {
    $('#home-tab').removeClass('active');
    $('#home').removeClass('active');
    $('#prod-conv-upload-tab').addClass('active');
    $('#upload').addClass('active');
}

function btnProdPlanUploadClick() {
    $('#home-tab').removeClass('active');
    $('#home').removeClass('active');
    $('#prod-plan-upload-tab').addClass('active');
    $('#messages').addClass('active');
}

function ajaxLoadPoa(formData, url) {
    $.ajax({
        type: 'POST',
        url: url,
        data: formData,
        success: function (data) {
            var list = data.SearchInput.PoaList;
            if (list.length > 0) {
                var poalist = '';
                for (var i = 0; i < list.length; i++) {
                    poalist = poalist + ', ' + list[i].Text;
                }
                poalist = poalist.slice(2);
                $('#Detail_PoaList').val(poalist);
                $('#displayPoaList').val(poalist);
            } else {
                $('#Detail_PoaList').val('');
                $('#displayPoaList').val('');
            }
        }
    });
}

function ajaxLoadCompany(formData, url) {
    if (formData.nppbkcid) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                if (data != null) {
                    $('#Detail_NppbkcCompanyName').val(data.CompanyName);
                    $('#Detail_NppbkcCompanyCode').val(data.CompanyCode);
                    $('#Detail_NppbkcKppbcId').val(data.KppbcNo);
                    $('#displayCompanyName').val(data.CompanyName);
                } else {
                    $('#Detail_NppbkcCompanyName').val('');
                    $('#Detail_NppbkcCompanyCode').val('');
                    $('#Detail_NppbkcKppbcId').val('');
                    $('#displayCompanyName').val('');
                }
            }
        });
    }
}

function ValidateGovInput() {
    var result = true;
    var requestQty = parseInt($("input[name='Detail.RequestQty']:hidden").val());
    var approvedQty = parseInt($('#Detail_QtyApproved').val());
    var govStatus = $('#Detail_StatusGov').find("option:selected").val();

    if (approvedQty > requestQty) {
        $('#modalBodyMessage').text('PBCK1 Quota Exceeded');
        $('#ModalPbck1ValidateGov').modal('show');

        AddValidationClass(false, 'Detail_QtyApproved');
        result = false;
    }

    if (govStatus == '') {
        AddValidationClass(false, 'Detail_StatusGov');
        result = false;
    }

    if ($('#Detail_DecreeDate').val() == '') {
        AddValidationClass(false, 'Detail_DecreeDate');
        result = false;
    }

    if ($('#Detail_StatusGov').val() == 'Rejected') {
        if ($('#Detail_Comment').val() == '') {
            AddValidationClass(false, 'Detail_Comment');
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

function changeToDecimal(selector, type) {
    $(selector).each(function () {
        if (type == "val") {
            var val = $(this).val();
            val = parseFloat(Math.round(val * 100) / 100).toFixed(2);
            $(this).val(ThausandSeperator(val, 2));

        } else {
            var val = $(this).html();
            val = parseFloat(Math.round(val * 100) / 100).toFixed(2);
            $(this).html(ThausandSeperator(val, 2));
        }
    });
}

function changeToNumber(dec) {
    var find = ',';
    var re = new RegExp(find, 'g');

    dec = dec.replace(re, '');
    dec = parseFloat(dec);
    return dec;
}

function getReference() {
    if ($('select[name="Detail.Pbck1Type"]').val().toLowerCase() != "additional") {
        return;
    }

    $('input[name="Detail.Pbck1ReferenceNumber"]:text').val("");
    $('input[name="Detail.Pbck1Reference"]:hidden').prop("disabled", true);
    $('input[name="Detail.Pbck1Reference"]:hidden').val("");

    if ($('select[name="Detail.NppbkcId"]').val() == "" || $('input[name="Detail.PeriodFrom"]').val() == "" || $('input[name="Detail.PeriodTo"]').val() == "" || ($('input[name="Detail.SupplierPlantWerks"]').val() == "" && $('input[name="Detail.SupplierPlant"]').val() == "") || $('select[name="Detail.GoodType"]').val() == "")
    {
        return false;
    }

    var data = {
        nppbkcId: $('select[name="Detail.NppbkcId"]').val(),
        periodFrom: $('input[name="Detail.PeriodFrom"]').val(),
        periodTo: $('input[name="Detail.PeriodTo"]').val(),
        supplierNppbkcId: $('input[name="Detail.SupplierNppbkcId"]').val(),
        supplierPlantWerks: $('input[name="Detail.SupplierPlantWerks"]').val(),
        supplierPlant:$('input[name="Detail.SupplierPlant"]').val(),
        goodType: $('select[name="Detail.GoodType"]').val()

    }
    $.ajax({
        type: 'POST',
        url: referenceURL,
        data: data,
        success: function (data) {
            if (data == false) {
                $('input[name="Detail.Pbck1ReferenceNumber"]:text').val("");
                $('input[name="Detail.Pbck1Reference"]:hidden').prop("disabled", true);
                $('input[name="Detail.Pbck1Reference"]:hidden').val("");
            } else {
                $('input[name="Detail.Pbck1ReferenceNumber"]:text').val(data.refereceNumber);
                $('input[name="Detail.Pbck1Reference"]:hidden').val(data.referenceId);
                $('input[name="Detail.Pbck1Reference"]:hidden').prop("disabled", false);

            }
        }
    });
}

function getKPPBCByNPPBKC(id) {
    if ($("#Detail_IsExternalSupplier").is(':checked')) {
        $.ajax({
            type: 'POST',
            url: kppbcUrl,
            data: { nppbkcid: id },
            success: function (data) {
                $('input[name="Detail.SupplierKppbcName"]:text').val(data.kppbcname);
                $('input[name="Detail.HiddenSupplierKppbcId"]:hidden').val(data.kppbcname);
                $('input[name="Detail.SupplierKppbcId"]:hidden').val(data.kppbcid);
            }
        });
    }

}

function changeToDecimalMaxFour(selector, type) {
    $(selector).each(function () {
        if (type == "val") {
            var val = $(this).val();
            val = parseFloat(Math.round(val * 100) / 100).toFixed(5);
            $(this).val(ThausandSeparatorMaxFour(val, 5));
        } else {
            var val = $(this).html();
            //val = parseFloat(Math.round(val * 100) / 100).toFixed(5);
            $(this).html(ThausandSeparatorMaxFour(val, 5));
        }
    });
}

function setLackYear() {
    var date = $("#Detail_PeriodFrom").datepicker('getDate');
    if (date == null)
        return;

    var year = date.getFullYear();

    $("#Detail_Lack1FormYear").html("");
    $("#Detail_Lack1ToYear").html("");
    for (var i = 0; i < 4 ; i++) {
        $("#Detail_Lack1FormYear").append("<option value='" + (year - i) + "' " + (i == 1 ? "selected='selected'" : "") + ">" + (year - i) + "</option>");
        $("#Detail_Lack1ToYear").append("<option value='" + (year - i) + "' " + (i == 1 ? "selected='selected'" : "") + ">" + (year - i) + "</option>");
    }
}