
function OnReadyFunction(ck5Type) {
    //alert(ck5Type);
    if (ck5Type == 'PortToImporter' || ck5Type == 'ImporterToPlant') {
        $('#MenuCK5Import').addClass('active');
    }
    else if (ck5Type == 'Export') {
        $('#MenuCK5Export').addClass('active');
    }
    else if (ck5Type == 'Manual') {
        $('#MenuCK5Manual').addClass('active');
    }
    else {
        $('#MenuCK5Domestic').addClass('active');
    }
   

    $('#btnUploadInfo').click(function () {

        $('#home-tab').removeClass('active');
        $('#upload-tab').addClass('active');

        $('#information').removeClass('active');
        $('#upload').addClass('active');

    });

    var total = 0;
    $('#CK5UploadSave').click(function () {
        var datarows = GetTableData($('#Ck5UploadTable'));
        var columnLength = $('#ck5TableItem').find("thead tr:first th").length;
        $('#ck5TableItem tbody').html('');
        for (var i = 0; i < datarows.length; i++) {
            var data = '<tr>';
            if (columnLength > 0) {
                data += '<td> <input name="UploadItemModels[' + i + '].Brand" type="hidden" value = "' + datarows[i][0] + '">' + datarows[i][0] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Qty" type="hidden" value = "' + datarows[i][1] + '">' + datarows[i][1] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Uom" type="hidden" value = "' + datarows[i][2] + '">' + datarows[i][2] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Convertion" type="hidden" value = "' + datarows[i][3] + '">' + datarows[i][3] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].ConvertedQty" type="hidden" value = "' + datarows[i][4] + '">' + datarows[i][4] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].ConvertedUom" type="hidden" value = "' + datarows[i][5] + '">' + datarows[i][5] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Hje" type="hidden" value = "' + datarows[i][6] + '">' + datarows[i][6] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Tariff" type="hidden" value = "' + datarows[i][7] + '">' + datarows[i][7] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].ExciseValue" type="hidden" value = "' + datarows[i][8] + '">' + datarows[i][8] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].UsdValue" type="hidden" value = "' + datarows[i][9] + '">' + datarows[i][9] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Note" type="hidden" value = "' + datarows[i][10] + '">' + datarows[i][10] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Message" type="hidden" value = "' + datarows[i][11] + '">' + datarows[i][11] + '</td>';

                total += parseFloat(datarows[i][1]); //Qty
                if (i == 0) {
                    $("#PackageUomId option").each(function () {
                        if ($(this).text().toLowerCase() == datarows[i][5].toLowerCase()) {
                            $(this).attr('selected', 'selected');
                        }
                    });
                }
            }
            data += '</tr>';
            $('#ck5TableItem tbody').append(data);
        }


        $('#GrandTotalEx').val(total.toFixed(2));

        $('#upload-tab').removeClass('active');
        $('#home-tab').addClass('active');

        $('#information').addClass('active');
        $('#upload').removeClass('active');

        $('#collapse5').addClass('in');
    });
}

function IsValidDataUpload() {

    var datarows = GetTableData($('#Ck5UploadTable'));

    for (var i = 0; i < datarows.length; i++) {
        if (datarows[i][11].length > 0)
            return false;
    }

    return true;
}
$('#CK5UploadSubmitBtn').click(function () {

    var fileName = $('[name="itemExcelFile"]').val().trim();
    var pos = fileName.lastIndexOf('.');
    var extension = (pos <= 0) ? '' : fileName.substring(pos);
    if (extension != '.xlsx') {
        alert('Please browse a correct excel file to upload');
        return false;
    }

    var formData = new FormData();
    var totalFiles = document.getElementById("itemExcelFile").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("itemExcelFile").files[i];

        formData.append("itemExcelFile", file);
        formData.append("plantId", $('#SourcePlantId').val());
    }
    $.ajax({
        type: "POST",
        url: '/CK5/UploadFile',
        data: formData,
        dataType: 'html',
        contentType: false,
        processData: false,
        success: function (response) {
            $('#ProdConvContent').html("");
            $('#ProdConvContent').html(response);
            if (IsValidDataUpload())
                $('#CK5UploadSave').enable();
        },
        error: function (error) {
            alert("errror " + error);
        }
    });
});

//function ajaxCallSave(url, formData) {
//    if (formData.model) {
//        $.ajax({
//            type: 'POST',
//            url: url,
//            data: formData,
//            success: function (data) {
//                $("input[name='pbck1Date']").val(data);
//            }
//        });
//    }
//}

function ajaxGetPbck1Date(url, formData) {
    if (formData.pbck1Id) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                $("input[name='PbckDecreeDate']").val(data);
            }
        });
    }
}

function ajaxGetDestPlantDetails(url, formData) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                $("input[name='DestNpwp']").val(data.PlantNpwp);
                $("input[name='DestNppbkcId']").val(data.NPPBCK_ID);
                $("input[name='DestCompanyName']").val(data.CompanyName);
                $("*[name='DestAddress']").val(data.CompanyAddress);
                $("input[name='DestKppbcName']").val(data.KppBcName);
                $("input[name='DestPlantName']").val(data.PlantName);
            }
        });
    }
}

function RemoveTable() {

    $('#ck5TableItem tbody').html('');
    $('#Ck5UploadTable tbody').html('');

}

function ajaxGetPlantDetails(url, formData) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                $("input[name='SourceNpwp']").val(data.PlantNpwp);
                $("input[name='SourceNppbkcId']").val(data.NPPBCK_ID);
                $("input[name='SourceCompanyName']").val(data.CompanyName);
                $("*[name='SourceAddress']").val(data.CompanyAddress);
                $("input[name='SourceKppbcName']").val(data.KppBcName);
                $("input[name='SourcePlantName']").val(data.PlantName);
                //enable upload
                $('#btnUploadInfo').enable();
                $('#CK5UploadSubmitBtn').enable();

            }
        });
    }
}


function ajaxGetCeOfficeCode(url, formData) {
    if (formData.nppBkcCityId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                $("input[name='CeOfficeCode']").val(data);
            }
        });
    }
}


function ChangeBackSourceMaterial(plantId) {
    if (plantId == plantOriginal) {
        
        var formData = new FormData();
        formData.append("ck5Id", $('#Ck5Id').val());
        $.ajax({
            type: "POST",
            url: '/CK5/GetOriginalPlant',
            data: formData,
            dataType: 'html',
            contentType: false,
            processData: false,
            success: function (response) {
                $('#ck5EditMaterialTable').html("");
                $('#ck5EditMaterialTable').html(response);
               
            },
            error: function (error) {
                alert("errror " + error);
            }
        });
    }
}

function OnSubmitWorkflow(id) {
    
}

function ValidateGovInput() {
    var result = true;
    
    if ($('#RegistrationNumber').val() == '') {
        AddValidationClass(false, 'RegistrationNumber');
        result = false;
    }
    
    if ($('#RegistrationDate').val() == '') {
        AddValidationClass(false, 'RegistrationDate');
        result = false;
    }

    if ($('.ck5Attachment').length == 0) {
        AddValidationClass(false, 'poa-files');
        
        if (result)
            alert("Attach your files");
        result = false;
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