
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
        total = 0;
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
                //alert(total);
                total += parseFloat(datarows[i][1]); //Qty
                if (i == 0) {
                    //alert(datarows[i][2]);
                    $("#PackageUomName option").each(function () {
                        if ($(this).val().toLowerCase() == datarows[i][2].toLowerCase()) {
                            $(this).attr('selected', 'selected');
                        }
                    });
                }
            }
            data += '</tr>';
            $('#ck5TableItem tbody').append(data);
        }

        //alert(total);
        $('#GrandTotalEx').val(total.toFixed(2));

        $('#upload-tab').removeClass('active');
        $('#home-tab').addClass('active');

        $('#information').addClass('active');
        $('#upload').removeClass('active');

        $('#collapse5').addClass('in');
    });
    
    $('#collapseTwo').addClass('in');
    $('#collapseThree').addClass('in');
}

function IsValidDataUpload() {

    var datarows = GetTableData($('#Ck5UploadTable'));

    for (var i = 0; i < datarows.length; i++) {
        if (datarows[i][11].length > 0)
            return false;
    }

    return true;
}

function GenerateXlsCk5Material(url) {
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
        url: url,
        data: formData,
        dataType: 'html',
        contentType: false,
        processData: false,
        success: function (response) {
            $('#ProdConvContent').html("");
            $('#ProdConvContent').html(response);
            if (IsValidDataUpload())
                $('#CK5UploadSave').enable();
        }
        //error: function (error) {
        //    alert("errror " + error);
        //}
    });
}


function ajaxGetDateAndQuotaPbck1(url, formData) {
    if (formData.id) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                $("input[name='PbckDecreeDate']").val(data.Pbck1DecreeDate);
                $("input[name='Pbck1QtyApproved']").val(data.Pbck1QtyApproved);
                $("input[name='Ck5TotalExciseable']").val(data.Ck5TotalExciseable);
                $("input[name='RemainQuota']").val(data.RemainQuota);
            }
        });
    }
}

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
                $("input[name='DestCompanyCode']").val(data.CompanyCode);
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
                $("input[name='SourceCompanyCode']").val(data.CompanyCode);
                $("input[name='SourceCompanyName']").val(data.CompanyName);
                $("*[name='SourceAddress']").val(data.CompanyAddress);
                $("input[name='SourceKppbcName']").val(data.KppBcName);
                $("input[name='SourcePlantName']").val(data.PlantName);
                
                $("input[name='KppBcCity']").val(data.KppbcCity);
                $("input[name='CeOfficeCode']").val(data.KppbcNo);
                
                //enable upload
                $('#btnUploadInfo').enable();
                $('#CK5UploadSubmitBtn').enable();

                $("input[name='PbckDecreeId']").val(data.Pbck1Id);
                $("input[name='PbckDecreeNumber']").val(data.Pbck1Number);
                $("input[name='PbckDecreeDate']").val(data.Pbck1DecreeDate);
                $("input[name='Pbck1QtyApproved']").val(data.Pbck1QtyApproved);
                $("input[name='Ck5TotalExciseable']").val(data.Ck5TotalExciseable);
                $("input[name='RemainQuota']").val(data.RemainQuota);
                
               // PopulateListPbckNumber(data.PbckList);
            }
        });
    }
}

function PopulateListPbckNumber(listPbck1) {
    var selectbox = $("#PbckDecreeId");
    selectbox.empty(); // remove old options
    
    var list = '<option value>Select</option>';

    if (listPbck1 != null) {
        for (var i = 0; i < listPbck1.length; i++) {
            list += "<option value='" + listPbck1[i].PbckId + "'>" + listPbck1[i].PbckNumber + "</option>";
        }

    }
    selectbox.html(list);
    
    //clear rellated field
    $("input[name='PbckDecreeDate']").val('');
    $("input[name='Pbck1QtyApproved']").val('');
    $("input[name='Ck5TotalExciseable']").val('');
    $("input[name='RemainQuota']").val('');
    
    //selectbox.html('refresh', true);
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

function ajaxGetCompanyCode(url, formData) {
    if (formData.nppBkcCityId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                $("input[name='CompanyCode']").val(data);
            }
        });
    }
}

function ChangeBackSourceMaterial(plantId, url) {
    if (plantId == plantOriginal) {
        
        var formData = new FormData();
        formData.append("ck5Id", $('#Ck5Id').val());
        $.ajax({
            type: "POST",
            url: url,
            data: formData,
            dataType: 'html',
            contentType: false,
            processData: false,
            success: function (response) {
                $('#ck5EditMaterialTable').html("");
                $('#ck5EditMaterialTable').html(response);
               
            }
            //,
            //error: function (error) {
            //    alert("errror " + error);
            //}
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
        $('#collapseOne').removeClass('collapse');
        $('#collapseOne').addClass('in');
        $("#collapseOne").css({ height: "auto" });
        $('#RegistrationNumber').focus();
    }
    
    if ($('#RegistrationDate').val() == '') {
        AddValidationClass(false, 'RegistrationDate');
        result = false;
        $('#collapseOne').removeClass('collapse');
        $('#collapseOne').addClass('in');
        $("#collapseOne").css({ height: "auto" });
       
    }
   // alert($('#GovStatus').val());
    
    if ($('#GovStatus').val() == '') {
        AddValidationClass(false, 'GovStatus');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });
        $('#GovStatus').focus();
    } else {
        if ($('#GovStatus').val() == 'GovReject' || $('#GovStatus').val() == 'GovCancel') {
            if ($('#Comment').val() == '') {
                AddValidationClass(false, 'Comment');
                result = false;
                $('#collapseFour').removeClass('collapse');
                $('#collapseFour').addClass('in');
                $("#collapseFour").css({ height: "auto" });
                $('#Comment').focus();
            }
        }

    }
  
    if ($('#poa_sk0').length == 0) {
        AddValidationClass(false, 'poa-files');
        
        if (result) {
            $('#modalBodyMessage').text('Missing attach files');
            $('#ModalCk5ValidateGov').modal('show');
            
            $('#collapseFour').removeClass('collapse');
            $('#collapseFour').addClass('in');
            $("#collapseFour").css({ height: "auto" });
          
        }
        result = false;
    }
    
    if (result) {
        if ($('#RegistrationNumber').val().length < 6) {
            
            AddValidationClass(false, 'RegistrationNumber');
            result = false;
            $('#collapseOne').removeClass('collapse');
            $('#collapseOne').addClass('in');
            $("#collapseOne").css({ height: "auto" });
            $('#RegistrationNumber').focus();
            
            $('#modalBodyMessage').text('Registration Number Length must be 6');
            $('#ModalCk5ValidateGov').modal('show');
            
           
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

function ValidateCk5Form(ck5Type) {
    var result = true;
    var isValidCk5Detail = true;
   
   
    if ($('#KppBcCity').find("option:selected").val() == '') {
        AddValidationClass(false, 'KppBcCity');
        result = false;
        isValidCk5Detail = false;
    }
    
    if ($('#GoodType').find("option:selected").val() == '') {
        AddValidationClass(false, 'GoodType');
        result = false;
        isValidCk5Detail = false;
    }
    
    if ($('#ExciseStatus').find("option:selected").val() == '') {
        AddValidationClass(false, 'ExciseStatus');
        result = false;
        isValidCk5Detail = false;
    }
    
    if ($('#ExciseSettlement').find("option:selected").val() == '') {
        AddValidationClass(false, 'ExciseSettlement');
        result = false;
        isValidCk5Detail = false;
    }
    
    if ($('#RequestType').find("option:selected").val() == '') {
        AddValidationClass(false, 'RequestType');
        result = false;
        isValidCk5Detail = false;
    }
    if ($('#SubmissionDate').val() == '') {
        AddValidationClass(false, 'SubmissionDate');
        result = false;
        isValidCk5Detail = false;
    }
    
    if (!isValidCk5Detail) {
        $('#collapseOne').removeClass('collapse');
        $('#collapseOne').addClass('in');
        $("#collapseOne").css({ height: "auto" });

    }
    
    if ($('#SourcePlantId').find("option:selected").val() == '') {
        AddValidationClass(false, 'SourcePlantId');
        result = false;
        // $('#collapseTwo').addClass('in');
        $('#collapseTwo').removeClass('collapse');
        $('#collapseTwo').addClass('in');
        $("#collapseTwo").css({ height: "auto" });
    }

    if (ck5Type == 'Export') {
        isValidCk5Detail = true;

        if ($('#CountryCode').find("option:selected").val() == '') {
            AddValidationClass(false, 'CountryCode');
            result = false;
            isValidCk5Detail = false;
        }


        if ($('#LoadingPort').val() == '') {
            AddValidationClass(false, 'LoadingPort');
            result = false;
            isValidCk5Detail = false;
        }
        if ($('#LoadingPortName').val() == '') {
            AddValidationClass(false, 'LoadingPortName');
            result = false;
            isValidCk5Detail = false;
        }
        if ($('#LoadingPortId').val() == '') {
            AddValidationClass(false, 'LoadingPortId');
            result = false;
            isValidCk5Detail = false;
        }
        
        if ($('#FinalPort').val() == '') {
            AddValidationClass(false, 'FinalPort');
            result = false;
            isValidCk5Detail = false;
        }
        if ($('#FinalPortName').val() == '') {
            AddValidationClass(false, 'FinalPortName');
            result = false;
            isValidCk5Detail = false;
        }
        if ($('#FinalPortId').val() == '') {
            AddValidationClass(false, 'FinalPortId');
            result = false;
            isValidCk5Detail = false;
        }
        
        if (!isValidCk5Detail) {
            $('#collapseThree').removeClass('collapse');
            $('#collapseThree').addClass('in');
            $("#collapseThree").css({ height: "auto" });

        }
        
    } else {


        if ($('#DestPlantId').find("option:selected").val() == '') {
            AddValidationClass(false, 'DestPlantId');
            result = false;
            $('#collapseThree').removeClass('collapse');
            $('#collapseThree').addClass('in');
            $("#collapseThree").css({ height: "auto" });
        }
    }
    
    if (result) {
        var rowCount = $('#ck5TableItem tr').length;

        if (rowCount <= 1) {
            // alert('Missing CK5 Material');

            $('#ModalCk5Material').modal('show');
            
            $('#home-tab').removeClass('active');
            $('#upload-tab').addClass('active');

            $('#information').removeClass('active');
            $('#upload').addClass('active');
            
            result = false;
        }
        
    }

    return result;
}

