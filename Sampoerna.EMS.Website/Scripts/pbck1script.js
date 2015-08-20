function setSupplierPlantEmpty() {
    $('#Detail_SupplierPortName').val('');
    $('#Detail_SupplierNppbkcId').val('');
    $('#Detail_SupplierKppbcId').val('');
    $('#Detail_SupplierPhone').val('');
    $('#Detail_SupplierAddress').val('');
    $('#Detail_HiddendSupplierAddress').val('');
    $('#Detail_HiddenSupplierKppbcId').val('');
    $('#Detail_HiddenSupplierNppbkcId').val('');
    $('#Detail_SupplierPlant').val('');
    $('#Detail_SupplierPlantWerks').val('');

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
                $('#Detail_HiddenSupplierKppbcId').val(data.KPPBC_NO);
                $('#Detail_SupplierKppbcId').val(data.KPPBC_NO);
                $('#Detail_SupplierPhone').val('');
                $('#Detail_SupplierAddress').val(data.Address);
                $('#Detail_HiddendSupplierAddress').val(data.Address);
                $('#Detail_SupplierPlant').val(data.Name1);
            } else {
                setSupplierPlantEmpty();
            }
        }
    });
}

function disableSupplierFormInput(isDisable) {
    $('#Detail_SupplierNppbkcId').prop('disabled', isDisable);
    $('#Detail_SupplierKppbcId').prop('disabled', isDisable);
    $('#Detail_SupplierAddress').prop('disabled', isDisable);
}

function supplierChange(url) {
    if ($("#Detail_SupplierPlant_ddl").length) {
        var plantid = $("#Detail_SupplierPlant_ddl").find("option:selected").val();
        console.log(plantid);
        ajaxLoadDetailSupplierPlant({ plantid: plantid }, url);
    }
}

function goodTypeOnChange() {
    if ($("#Detail_Pbck1Type").length) {
        var goodTypeName = $("#Detail_GoodType").find("option:selected").text();
        $('#Detail_GoodTypeDesc').val(goodTypeName);
    }

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
        if (datarows[i][6].length > 0)
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
                + '].ConverterOutput" type="hidden" value = "' + datarows[i][4] + '" />' + datarows[i][4] + '</td>';
            data += '<td><input name="Detail.Pbck1ProdConverter[' + i
                + '].ConverterUomId" type="hidden" value = "' + datarows[i][5] + '" />' + datarows[i][5] + '</td>';

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
            data += '<td><input name="Detail.Pbck1ProdPlan[' + i + '].Amount" type="hidden" value = "'
                + datarows[i][5] + '" />' + datarows[i][5] + '</td>';
            data += '<td><input name="Detail.Pbck1ProdPlan[' + i + '].BkcRequired" type="hidden" value = "'
                + datarows[i][6] + '" />' + datarows[i][6] + '</td>';
            data += '<td><input name="Detail.Pbck1ProdPlan[' + i + '].BkcRequiredUomId" type="hidden" value = "'
                + datarows[i][7] + '" />' + datarows[i][7] + '</td>';
            data += '<td style="display:none"><input name="Detail.Pbck1ProdPlan[' + i + '].BkcRequiredUomName" type="hidden" value = "'
                + datarows[i][8] + '" />' + datarows[i][8] + '</td>';
            
            total += parseFloat(datarows[i][6]);
            uom = datarows[i][7];
        }
        data += '</tr>';
        $('#Detail_Pbck1ProdPlan tbody').append(data);
    }
    $("input[name='Detail.RequestQty']").val(total);
    $("select[name='Detail.RequestQtyUomId']").val(uom);
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
        formData.append("prodPlanExcelFile", file);
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
            $('#ProdPlanContent').html("");
            $('#ProdPlanContent').html(response);
            if (IsProdPlanValid()) {
                //valid generated
                $('#prod-plan-save').removeAttr('disabled');
            } else {
                //invalid generated
                $('#prod-plan-save').attr('disabled', 'disabled');
            }
        },
        error: function (error) {
            // Handle errors here
            console.log('ERRORS: ' + error);
            // STOP LOADING SPINNER
        }
    });
    return false;
}

function pbck1TypeOnchange() {
    if ($("#Detail_Pbck1Type").length) {
        var pbck1Type = $("#Detail_Pbck1Type").find("option:selected").val();
        if (pbck1Type == '' || pbck1Type.toLowerCase() == 'new') {
            $('#Detail_Pbck1Reference').prop('disabled', true);
        } else {
            $('#Detail_Pbck1Reference').prop('disabled', false);
        }
    }
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