﻿function ResetPbck1Link() {
    $('#IsReducePbck1Ck5Trial').prop('checked', false);
    $("input[name='PbckDecreeId']").val('');
    $("input[name='PbckDecreeNumber']").val('');
    $("input[name='PbckDecreeDate']").val('');
    $("input[name='Pbck1QtyApproved']").val('0');
    $("input[name='Ck5TotalExciseable']").val('0');
    $("input[name='RemainQuota']").val('0');
    $("input[name='PbckUom']").val('');
}

function OnReadyFunction(ck5Type) {

    $('#ck5TrialPbck1Reduce').hide();

    if (ck5Type == 'Completed' || ck5Type == 'Cancelled') {
        $('#MenuCK5Completed').addClass('active');

    } else if (ck5Type == 'PortToImporter' || ck5Type == 'ImporterToPlant') {
        $('#MenuCK5Import').addClass('active');
    } else if (ck5Type == 'Export') {
        $('#MenuCK5Export').addClass('active');
    } else if (ck5Type == 'Manual') {
        $('#ck5TrialPbck1Reduce').show();
        $('#MenuCK5Manual').addClass('active');
    } else if (ck5Type == 'Waste') {
        $('#MenuCK5Waste').addClass('active');
    } else if (ck5Type == 'MarketReturn') {
        $('#liCK5MarketReturnOpenDocument').addClass('active');
    }
    else if (ck5Type == 'Return') {
        $('#MenuCK5TriggerSto').addClass('active');
    }
    else {
        $('#MenuCK5Domestic').addClass('active');
    }

    //$('#GrandTotalEx').prop("readonly", true);



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
        var isSamePbck1Uom = true;
        //====fixing bug no. 111 CK5====
        var isExistPbck1 = true;
        //==============================

        var data = "";
        for (var i = 0; i < datarows.length; i++) {
            data += '<tr>';
            if (columnLength > 0) {

                if (datarows[i][13].length > 0) {
                    $('#modalBodyMessage').text('Item Material Not Valid');
                    $('#ModalCk5Material').modal('show');
                    return;
                }
                data += '<td> <input name="UploadItemModels[' + i + '].Brand" type="hidden" value = "' + datarows[i][2] + '">' + datarows[i][2] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Qty" type="hidden" value = "' + datarows[i][3] + '">' + datarows[i][3] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Uom" type="hidden" value = "' + datarows[i][4] + '">' + datarows[i][4] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Convertion" type="hidden" value = "' + datarows[i][5] + '">' + datarows[i][5] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].ConvertedQty" type="hidden" value = "' + datarows[i][6] + '">' + datarows[i][6] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].ConvertedUom" type="hidden" value = "' + datarows[i][7] + '">' + datarows[i][7] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Hje" type="hidden" value = "' + datarows[i][8] + '">' + datarows[i][8] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Tariff" type="hidden" value = "' + datarows[i][9] + '">' + datarows[i][9] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].ExciseValue" type="hidden" value = "' + datarows[i][10] + '">' + datarows[i][10] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].UsdValue" type="hidden" value = "' + datarows[i][11] + '">' + datarows[i][11] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Note" type="hidden" value = "' + datarows[i][12] + '">' + datarows[i][12] + '</td>';
                if (ck5Type == 'Waste') {
                    data += '<td> <input name="UploadItemModels[' + i + '].WasteStock" type="hidden" value = "' + datarows[i][20] + '">' + datarows[i][20] + '</td>';
                } else {
                    data += '<td> <input name="UploadItemModels[' + i + '].Message" type="hidden" value = "' + datarows[i][13] + '">' + datarows[i][13] + '</td>';
                }
                
                data += '<input name="UploadItemModels[' + i + '].MaterialDesc" type="hidden" value = "' + datarows[i][15] + '">';
                data += '<input name="UploadItemModels[' + i + '].CK5_MATERIAL_ID" type="hidden" value = "' + datarows[i][18] + '">';
                
                total += parseFloat(removeCharacter(datarows[i][6],",")); //Qty
                if (i == 0) {
                    $("#PackageUomName").val(datarows[i][7]);
                }
            }
            data += '</tr>';
            //console.log($('#PbckDecreeId').val());
            // console.log($(datarows[i][4]));

            if ($('#PbckDecreeId').val() == '')
                isExistPbck1 = false;

            if (datarows[i][17] != ($('#PbckUom').val()))
                isSamePbck1Uom = false;

            //$('#ck5TableItem tbody').append(data);
        }

        //alert(total);

        $('#GrandTotalEx').val(total.toFixed(2));
        $('#GrandTotalExDisplay').val(ThausandSeperator(total.toFixed(2), 2));


        $('#upload-tab').removeClass('active');
        $('#home-tab').addClass('active');

        $('#information').addClass('active');
        $('#upload').removeClass('active');

        $('#collapse5').addClass('in');



        if (ck5Type == 'Export' || (ck5Type == 'Manual' && $('#IsReducePbck1Ck5Trial').is(':checked') == false)
            || ck5Type == 'MarketReturn' || ck5Type == 'PortToImporter' || ck5Type == 'Return' || ck5Type == 'Waste') {
            $('#ck5TableItem tbody').append(data);
            return;
        }
        if (ck5Type == 'Domestic' && ($('#SourceNppbkcId').val() == $('#DestNppbkcId').val())) {
            $('#ck5TableItem tbody').append(data);
            return;
        }


        //if pbck is not exist
        //====fixing bug no. 111 CK5====
        if (!isExistPbck1) {
            $('#modalBodyMessage').text('Pbck1 Not Found');
            $('#ModalCk5Material').modal('show');
            return;
            //==============================
        } else {
            //if pbck1 uom not same not append the data
            if (!isSamePbck1Uom) {
                var pbck1Uom = $('#PbckUom').val();
                $('#modalBodyMessage').text('Pbck1 Uom Not Same, pbck1 Uom : ' + pbck1Uom);
                $('#ModalCk5Material').modal('show');
                return;

            }
        }

        if (ValidateRemainQuota(total))
            $('#ck5TableItem tbody').append(data);

        

    });

    $('#collapseTwo').addClass('in');
    $('#collapseThree').addClass('in');
    $('#collapse6').addClass('in');
}


function ValidateRemainQuota(total) {
    // var total = parseFloat($('#GrandTotalEx').val());

    var pbck1QtyApproved = parseFloat($('#Pbck1QtyApproved').val().replace(/\,/g,''));
    var totalCk5 = parseFloat($('#Ck5TotalExciseable').val().replace(/\,/g, ''));
    var remainQuota = pbck1QtyApproved - totalCk5;
    if (total > remainQuota) {
        $('#collapseThree').removeClass('collapse');
        $('#collapseThree').addClass('in');
        $("#collapseThree").css({ height: "auto" });

        $('#modalBodyMessage').text('CK5 Quota Exceeded');
        $('#ModalCk5Material').modal('show');

        AddValidationClass(false, 'GrandTotalEx');


        return false;
    }
    
    return true;
}

function IsValidDataUpload(ck5Type) {

    var datarows = GetTableData($('#Ck5UploadTable'));

    for (var i = 0; i < datarows.length; i++) {
        if (ck5Type == 'MarketReturn') {
            if (datarows[i][14].length > 0) {
                return false;
            }
        }
        else {
            if (datarows[i][13].length > 0) {
                return false;
            }
        }
    }

    return true;
}

function GenerateXlsCk5Material(url, ck5Type) {
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
        if (ck5Type == 'PortToImporter' || ck5Type == 'DomesticAlcohol')
            formData.append("plantId", $('#DestPlantId').val());
        else if (ck5Type == 'Manual') {
            if ($('#IsFreeTextSource').is(':checked')) {
                formData.append("plantId", $('#DestPlantId').val());
            } else {
                formData.append("plantId", $('#SourcePlantId').val());
            }
        }
        else
            formData.append("plantId", $('#SourcePlantId').val());

    }


    var goodTypeGroupId = $("#GoodType").val();
    if (goodTypeGroupId == "") {
        $('#modalBodyMessage').text('Please fill the excisable good types for this CK-5 document');
        $('#ModalCk5Material').modal('show');
    } else {
        formData.append("groupType", $('#GoodType').val());
    }

    formData.append("ck5Type", ck5Type);
    
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
            if (IsValidDataUpload(''))
                $('#CK5UploadSave').enable();
        }
      
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
    if (formData.destPlantId) {
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

                $("input[name='PbckDecreeId']").val(data.Pbck1Id);
                $("input[name='PbckDecreeNumber']").val(data.Pbck1Number);
                $("input[name='PbckDecreeDate']").val(data.Pbck1DecreeDate);
                $("input[name='Pbck1QtyApproved']").val(data.Pbck1QtyApproved);
                $("input[name='Ck5TotalExciseable']").val(data.Ck5TotalExciseable);
                $("input[name='RemainQuota']").val(data.RemainQuota);
                //alert(data.PbckUom);
                $("input[name='PbckUom']").val(data.PbckUom);

                if (formData.ck5Type != null && (formData.ck5Type == "PortToImporter" || formData.ck5Type == "DomesticAlcohol")) {
                    $("input[name='KppBcCity']").val(data.KppbcCity);
                    $("input[name='CeOfficeCode']").val(data.KppbcNo);
                  
                }
                else if (formData.ck5Type != null && (formData.ck5Type == "Manual")) {
                    if ($('#IsFreeTextSource').is(':checked')) {
                        $("input[name='KppBcCity']").val(data.KppbcCity);
                        $("input[name='CeOfficeCode']").val(data.KppbcNo);
                        
                        //enable upload
                        $('#btnUploadInfo').enable();
                        $('#CK5UploadSubmitBtn').enable();
                    }
                }
           
            }
        });
    }
}

function ajaxGetDestPlantDetailsPortToImporter(url, formData) {
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


                $("input[name='KppBcCity']").val(data.KppbcCity);
                $("input[name='CeOfficeCode']").val(data.KppbcNo);

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
            
                //reset destination plant
                $("input[name='DestNpwp']").val("");
                $("input[name='DestNppbkcId']").val("");
                $("input[name='DestCompanyCode']").val("");
                $("input[name='DestCompanyName']").val("");
                $("*[name='DestAddress']").val("");
                $("input[name='DestKppbcName']").val("");
                $("input[name='DestPlantName']").val("");

                //data plant destination
                $("#DestPlantId").html("");
                $("#DestPlantId").append("<option value=\"\">Select</option>");
                $.each(data.CorrespondingPlantList, function (index, optiondata) {
                    $("#DestPlantId").append("<option value=\"" + optiondata.Value + "\">" + optiondata.Text + "</option>");

                });
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
          
        });
    }
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

function ValidateGovMarketReturn() {
    var result = true;


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

    if ($('#Back1Number').val() != '') {

        if ($('#Back1Date').val() == '') {
            AddValidationClass(false, 'Back1Date');
            result = false;
            $('#collapseOne').removeClass('collapse');
            $('#collapseOne').addClass('in');
            $("#collapseOne").css({ height: "auto" });
        }
    }

    if ($('#Back1Date').val() != '') {

        if ($('#Back1Number').val() == '') {
            AddValidationClass(false, 'Back1Number');
            result = false;
            $('#collapseOne').removeClass('collapse');
            $('#collapseOne').addClass('in');
            $("#collapseOne").css({ height: "auto" });
            $('#Back1Number').focus();
        }
    }

    if ($('#RegistrationNumber').val() == '') {
        AddValidationClass(false, 'RegistrationNumber');
        result = false;
        $('#collapseOne').removeClass('collapse');
        $('#collapseOne').addClass('in');
        $("#collapseOne").css({ height: "auto" });
        $('#RegistrationNumber').focus();
        
        $('#modalBodyMessage').text('Registration Number Can Not be Empty');
        $('#ModalCk5ValidateGov').modal('show');
    }
    
    if (result) {
        if ($('#RegistrationNumber').val() != '') {

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

    if ($('#CarriageMethod').find("option:selected").val() == '') {
        AddValidationClass(false, 'CarriageMethod');
        result = false;
        $('#collapse6').removeClass('collapse');
        $('#collapse6').addClass('in');
        $("#collapse6").css({ height: "auto" });
    }
    
    if (!isValidCk5Detail) {
        $('#collapseOne').removeClass('collapse');
        $('#collapseOne').addClass('in');
        $("#collapseOne").css({ height: "auto" });

    }

      
    else if (ck5Type != 'MarketReturn')
        {
        if ($('#SourcePlantId').find("option:selected").val() == '') {
            AddValidationClass(false, 'SourcePlantId');
            result = false;
            // $('#collapseTwo').addClass('in');
            $('#collapseTwo').removeClass('collapse');
            $('#collapseTwo').addClass('in');
            $("#collapseTwo").css({ height: "auto" });
        }
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

    if (ck5Type == 'Manual') {
        if ($('#Ck5ManualType').val() == '') {
            AddValidationClass(false, 'Ck5ManualType');
            result = false;
            //isValidCk5Detail = false;
        }

        if ($('#IsFreeTextSource').is(':checked')) {
            if ($('#SourcePlantId').val() == '') {
                AddValidationClass(false, 'SourcePlantId');
                result = false;

                $('#collapseTwo').removeClass('collapse');
                $('#collapseTwo').addClass('in');
                $("#collapseTwo").css({ height: "auto" });
            }

            if ($('#SourcePlantNameManual').val() == '') {
                AddValidationClass(false, 'SourcePlantNameManual');
                result = false;

                $('#collapseTwo').removeClass('collapse');
                $('#collapseTwo').addClass('in');
                $("#collapseTwo").css({ height: "auto" });
            }
        }

        if ($('#IsFreeTextDestination').is(':checked')) {
            if ($('#DestPlantId').val() == '') {
                AddValidationClass(false, 'DestPlantId');
                result = false;

                $('#collapseThree').removeClass('collapse');
                $('#collapseThree').addClass('in');
                $("#collapseThree").css({ height: "auto" });
            }

            if ($('#DestPlantNameManual').val() == '') {
                AddValidationClass(false, 'DestPlantNameManual');
                result = false;

                $('#collapseThree').removeClass('collapse');
                $('#collapseThree').addClass('in');
                $("#collapseThree").css({ height: "auto" });
            }
        }
    }

    if (ck5Type == 'PortToImporter') {
        if ($('#SourcePlantName').val() == '') {
            AddValidationClass(false, 'SourcePlantName');
            result = false;

            $('#collapseTwo').removeClass('collapse');
            $('#collapseTwo').addClass('in');
            $("#collapseTwo").css({ height: "auto" });
        }
    }
    

    if (result) {
        var rowCount = $('#ck5TableItem tr').length;

        if (rowCount <= 1) {
            // alert('Missing CK5 Material');
            $('#modalBodyMessage').text('Missing CK5 Materials');
            $('#ModalCk5Material').modal('show');

            $('#home-tab').removeClass('active');
            $('#upload-tab').addClass('active');

            $('#information').removeClass('active');
            $('#upload').addClass('active');

            result = false;
        }

    }

    if (result) {

        if (ck5Type == 'Export' || ck5Type == "Manual" || ck5Type == 'PortToImporter'
            || ck5Type == 'MarketReturn' || ck5Type == 'Return'
            || ck5Type == 'Waste')
            return result;

        if (ck5Type == 'Domestic' && ($('#SourceNppbkcId').val() == $('#DestNppbkcId').val()))
            return result;

        var pbck1QtyApproved = parseFloat($('#Pbck1QtyApproved').val().replace(/\,/g, ''));
        var totalCk5 = parseFloat($('#Ck5TotalExciseable').val().replace(/\,/g, ''));
        var total = parseFloat($('#GrandTotalEx').val().replace(/\,/g, ''));
        var remainQuota = pbck1QtyApproved - totalCk5;
        if (total > remainQuota) {
            $('#collapseThree').removeClass('collapse');
            $('#collapseThree').addClass('in');
            $("#collapseThree").css({ height: "auto" });

            $('#modalBodyMessage').text('CK5 Quota Exceeded');
            $('#ModalCk5Material').modal('show');

            AddValidationClass(false, 'GrandTotalEx');
            result = false;
        }
    }
    return result;
}

function ValidateGiCreated() {
    var result = true;

    if ($('#SealingNotifNumber').val() == '') {
        AddValidationClass(false, 'SealingNotifNumber');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });
        $('#SealingNotifNumber').focus();
    }

    if ($('#SealingNotifDate').val() == '') {
        AddValidationClass(false, 'SealingNotifDate');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });

    }
    return result;
}

function ValidateGRCreated() {
    var result = true;

    if ($('#UnSealingNotifNumber').val() == '') {
        AddValidationClass(false, 'UnSealingNotifNumber');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });
        $('#UnSealingNotifNumber').focus();
    }

    if ($('#UnsealingNotifDate').val() == '') {
        AddValidationClass(false, 'UnsealingNotifDate');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });

    }
    return result;
}

function ValidateGoodIssue() {
    var result = true;

    if ($('#GiDate').val() == '') {
        AddValidationClass(false, 'GiDate');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });

    }

    if ($('#SealingNotifNumber').val() == '') {
        AddValidationClass(false, 'SealingNotifNumber');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });
        $('#SealingNotifNumber').focus();
    }

    if ($('#SealingNotifDate').val() == '') {
        AddValidationClass(false, 'SealingNotifDate');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });

    }
    return result;
}

function ValidateGoodReceive() {
    var result = true;

    if ($('#GrDate').val() == '') {
        AddValidationClass(false, 'GrDate');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });
        $('#UnSealingNotifNumber').focus();
    }

    if ($('#UnSealingNotifNumber').val() == '') {
        AddValidationClass(false, 'UnSealingNotifNumber');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });
        $('#UnSealingNotifNumber').focus();
    }

    if ($('#UnsealingNotifDate').val() == '') {
        AddValidationClass(false, 'UnsealingNotifDate');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });

    }
    return result;
}

function ValidatePOReceive() {
    var result = true;

    if ($('#DnNumber').val() == '') {
        AddValidationClass(false, 'DnNumber');
        result = false;
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });
        $('#DnNumber').focus();
    }

    return result;
}

function ajaxGetListMaterial(url, formData, materialid) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                //debugger;
                var listMaterial = $('#uploadMaterialNumber');
                listMaterial.empty();

                var list = '<option value>Select</option>';

                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        if (materialid == data[i].MaterialNumber) {
                            list += "<option value='" + data[i].MaterialNumber + "' selected='true'>" + data[i].MaterialNumber + "</option>";
                        } else {
                            list += "<option value='" + data[i].MaterialNumber + "'>" + data[i].MaterialNumber + "</option>";
                        }

                    }

                }

                listMaterial.html(list);

                listMaterial.selectpicker('refresh');
               
            }
        });
    }
}


function ajaxGetMaterialHjeAndTariff(url, formData) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {

                if (data != null) {
                    $("#uploadMaterialHje").val(data.Hje);
                    $("#uploadMaterialTariff").val(data.Tariff);
                    $("#uploadMaterialDesc").val(data.MaterialDesc);
                    
                    $("#uploadWasteStock").val(data.StockRemaining);
                }


            }
        });
    }
}

function removeExistingDocument(id) {
    // alert(id);
    $(id).parent().parent().parent().remove();
}

function ajaxValidateMaterial(url, formData, callback) {
    //if (formData.plantId) {
    $.ajax({
        type: 'POST',
        url: url,
        data: formData,
        success: function (data) {
            callback(data);


        }
    });
    //}
}


function GenerateXlsCk5MarketReturnMaterial(url) {
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
        formData.append("plantId", $('#DestPlantId').val());
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
            if (IsValidDataUpload('MarketReturn'))
                $('#CK5UploadSave').enable();
        }

    });
}

function MoveUploadToTableMarketReturn() {
    var datarows = GetTableData($('#Ck5UploadTable'));
    var columnLength = $('#ck5TableItem').find("thead tr:first th").length;
    $('#ck5TableItem tbody').html('');
    total = 0;


    var data = "";
    for (var i = 0; i < datarows.length; i++) {
        data += '<tr>';
        if (columnLength > 0) {

            if (datarows[i][14].length > 0) {
                $('#modalBodyMessage').text('Item Material Not Valid');
                $('#ModalCk5Material').modal('show');
                return;
            }
            data += '<td> <input name="UploadItemModels[' + i + '].Brand" type="hidden" value = "' + datarows[i][2] + '">' + datarows[i][2] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].StickerCode" type="hidden" value = "' + datarows[i][3] + '">' + datarows[i][3] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].Qty" type="hidden" value = "' + datarows[i][4] + '">' + datarows[i][4] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].Uom" type="hidden" value = "' + datarows[i][5] + '">' + datarows[i][5] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].Convertion" type="hidden" value = "' + datarows[i][6] + '">' + datarows[i][6] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].ConvertedQty" type="hidden" value = "' + datarows[i][7] + '">' + datarows[i][7] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].ConvertedUom" type="hidden" value = "' + datarows[i][8] + '">' + datarows[i][8] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].Hje" type="hidden" value = "' + datarows[i][9] + '">' + datarows[i][9] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].Tariff" type="hidden" value = "' + datarows[i][10] + '">' + datarows[i][10] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].ExciseValue" type="hidden" value = "' + datarows[i][11] + '">' + datarows[i][11] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].UsdValue" type="hidden" value = "' + datarows[i][12] + '">' + datarows[i][12] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].Note" type="hidden" value = "' + datarows[i][13] + '">' + datarows[i][13] + '</td>';
            data += '<td> <input name="UploadItemModels[' + i + '].Message" type="hidden" value = "' + datarows[i][14] + '">' + datarows[i][14] + '</td>';
            data += '<input name="UploadItemModels[' + i + '].MaterialDesc" type="hidden" value = "' + datarows[i][16] + '">';
            data += '<input name="UploadItemModels[' + i + '].CK5_MATERIAL_ID" type="hidden" value = "' + datarows[i][19] + '">';

            total += parseFloat(datarows[i][17]); //Qty
            if (i == 0) {
                $("#PackageUomName").val(datarows[i][18]);
            }
        }
        data += '</tr>';

    }

    //alert(total);
    $('#GrandTotalExDisplay').val(ThausandSeperator(total.toFixed(2), 2));
    $('#GrandTotalEx').val(total.toFixed(2));



    $('#upload-tab').removeClass('active');
    $('#home-tab').addClass('active');

    $('#information').addClass('active');
    $('#upload').removeClass('active');

    $('#collapse5').addClass('in');




    $('#ck5TableItem tbody').append(data);

}


function ajaxGetListMaterialMarketReturn(url, formData, materialid) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                //debugger;
                var listMaterial = $('#uploadMaterialNumber');
                listMaterial.empty();

                var list = '<option value>Select</option>';

                var faCode = materialid.split(" - ")[0];
                var stickerCode = materialid.split(" - ")[1];

                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        if (faCode == data[i].MaterialNumber && stickerCode == data[i].StickerCode) {
                            list += "<option value='" + data[i].MaterialNumber + " - " + data[i].StickerCode + "' selected='true'>" + data[i].MaterialNumber + " - " + data[i].StickerCode + "</option>";
                        } else {
                            list += "<option value='" + data[i].MaterialNumber + " - " + data[i].StickerCode + "'>" + data[i].MaterialNumber + " - " + data[i].StickerCode + "</option>";
                        }

                    }

                }

                listMaterial.html(list);
                listMaterial.selectpicker('refresh');
            }
        });
    }
}

function ajaxGetMaterialHjeAndTariffMarketReturn(url, formData) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {

                if (data != null) {
                    $("#uploadMaterialHje").val(data.Hje);
                    $("#uploadMaterialTariff").val(data.Tariff);
                    $("#uploadMaterialDesc").val(data.MaterialDesc);
                    $("#uploadConvertedUom").val(data.Uom);
                }
            }
        });
    }
}


function ajaxGetDestPlantDetailsMarketReturn(url, formData) {
    if (formData.destPlantId) {
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

                $("input[name='SourceNpwp']").val(data.PlantNpwp);
                $("input[name='SourceNppbkcId']").val(data.NPPBCK_ID);
                $("input[name='SourceCompanyCode']").val(data.CompanyCode);
                $("input[name='SourceCompanyName']").val(data.CompanyName);
                $("*[name='SourceAddress']").val(data.CompanyAddress);
                $("input[name='SourceKppbcName']").val(data.KppBcName);
                $("input[name='plantname']").val(formData.destPlantId + "-" + data.PlantName);

                $("input[name='KppBcCity']").val(data.KppbcCity);
                $("input[name='CeOfficeCode']").val(data.KppbcNo);

                //enable upload
                $('#btnUploadInfo').enable();
                $('#CK5UploadSubmitBtn').enable();
            }
        });
    }
}

function OnSuccess(data) {
    $('.loading').hide();
    $("#contenttable").html(data);
}

function OnSuccessIntercompany(data) {
    $('.loading').hide();
    $("#contenttableIntercompany").html(data);
}

function OnSuccessDomesticAlcohol(data) {
    $('.loading').hide();
    $("#contenttableDomesticAlcohol").html(data);
}

function OnFailure(xhr, status) {
    $('.loading').hide();
    // alert('Error: ' + xhr.statusText);
}

function OnFilter() {
    $('.loading').show();
    //$('#ajaxForm').submit();
    
    var formId = $('.tab-pane.active').attr('id');
    //console.log(formId);
    if (formId == 'domestic') {
        $('#ajaxForm').submit();
    } else if (formId == 'intercompany') {
        $('#ajaxForm1').submit();
    } else if (formId == 'domesticAlcohol') {
        $('#ajaxForm2').submit();
    }
    else
        $('#ajaxForm').submit();
}


function getExGroupTypeVal(exgrouptypeId) {
    if (exgrouptypeId == "HasilTembakau") {
        return 3;
    } else if(exgrouptypeId == "EtilAlkohol") {
        return 1;
    }else if (exgrouptypeId == "MMEA") {
        return 2;
    } else {
        return 4;
    }
}