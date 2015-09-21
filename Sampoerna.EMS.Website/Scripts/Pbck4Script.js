function printPreview(url) {

    window.open(url, 'popup', 'width=800,height=600,toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,resizable=no');

}

function OnReadyFunction() {
    
    $('#btnUploadInfo').click(function () {

        $('#home-tab').removeClass('active');
        $('#upload-tab').addClass('active');

        $('#information').removeClass('active');
        $('#upload').addClass('active');

    });
    
    $('#Pbck4UploadSave').click(function () {
        var datarows = GetTableData($('#Ck5UploadTable'));
        var columnLength = $('#pbck4TableItem').find("thead tr:first th").length;
        $('#pbck4TableItem tbody').html('');
       
        var data = "";
        for (var i = 0; i < datarows.length; i++) {
            data += '<tr>';
            if (columnLength > 0) {
                data += '<td> <input name="UploadItemModels[' + i + '].FaCode" type="hidden" value = "' + datarows[i][2] + '">' + datarows[i][2] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].StickerCode" type="hidden" value = "' + datarows[i][3] + '">' + datarows[i][3] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Ck1No" type="hidden" value = "' + datarows[i][4] + '">' + datarows[i][4] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Ck1Date" type="hidden" value = "' + datarows[i][5] + '">' + datarows[i][5] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].SeriesCode" type="hidden" value = "' + datarows[i][6] + '">' + datarows[i][6] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].BrandName" type="hidden" value = "' + datarows[i][7] + '">' + datarows[i][7] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].ProductAlias" type="hidden" value = "' + datarows[i][8] + '">' + datarows[i][8] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Content" type="hidden" value = "' + datarows[i][9] + '">' + datarows[i][9] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Hje" type="hidden" value = "' + datarows[i][10] + '">' + datarows[i][10] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Tariff" type="hidden" value = "' + datarows[i][11] + '">' + datarows[i][11] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Colour" type="hidden" value = "' + datarows[i][12] + '">' + datarows[i][12] + '</td>';
                
                data += '<td> <input name="UploadItemModels[' + i + '].ReqQty" type="hidden" value = "' + datarows[i][13] + '">' + datarows[i][13] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].TotalHje" type="hidden" value = "' + datarows[i][14] + '">' + datarows[i][14] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].TotalStamps" type="hidden" value = "' + datarows[i][15] + '">' + datarows[i][15] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].NoPengawas" type="hidden" value = "' + datarows[i][16] + '">' + datarows[i][16] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].ApprovedQty" type="hidden" value = "' + datarows[i][17] + '">' + datarows[i][17] + '</td>';
                data += '<td> <input name="UploadItemModels[' + i + '].Remark" type="hidden" value = "' + datarows[i][18] + '">' + datarows[i][18] + '</td>';
                //data += '<td> <input name="UploadItemModels[' + i + '].Message" type="hidden" value = "' + datarows[i][19] + '">' + datarows[i][19] + '</td>';
                data += '<input name="UploadItemModels[' + i + '].CK1_ID" type="hidden" value = "' + datarows[i][20] + '">';
                
            }
            data += '</tr>';
          
        }

       

        $('#upload-tab').removeClass('active');
        $('#home-tab').addClass('active');

        $('#information').addClass('active');
        $('#upload').removeClass('active');
        
        $('#pbck4TableItem tbody').append(data);

     

    });
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

function ValidatePbck4Form() {
    var result = true;
    
    if ($('#PlantId').find("option:selected").val() == '') {

        AddValidationClass(false, 'PlantId');
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

    if (result) {
        var rowCount = $('#pbck4TableItem tr').length;

        if (rowCount <= 1) {

            $('#modalBodyMessage').text('Missing PBCK-4 Items');
            $('#ModalPbck4Items').modal('show');

            $('#home-tab').removeClass('active');
            $('#upload-tab').addClass('active');

            $('#information').removeClass('active');
            $('#upload').addClass('active');

            result = false;
        }
    }

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
                $("input[name='CompanyId']").val(data.CompanyId);
                $("*[name='PlantDesc']").val(data.PlantDesc);
                $("input[name='Poa']").val(data.Poa);
                $("input[name='NppbkcId']").val(data.NppbkcId);
                $("input[name='NppbkcDesc']").val(data.NppbkcDescription);
                
                $('#btnUploadInfo').enable();
                $('#Pbck4UploadSubmitBtn').enable();
                
            }
        });
    }
}

function GenerateXlsPbck4Items(url) {
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
        formData.append("plantId", $('#PlantId').val());

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
                $('#Pbck4UploadSave').enable();
        }
      
    });
}


    function IsValidDataUpload() {

        var datarows = GetTableData($('#Ck5UploadTable'));
        
        for (var i = 0; i < datarows.length; i++) {
           
            if (datarows[i][19].length > 0)
                return false;
        }

        return true;
    }


    function RemovePbck4ItemsTable() {

        $('#pbck4TableItem tbody').html('');
        $('#Ck5UploadTable tbody').html('');

    }


    function ValidateGovInput() {
        var result = true;

        if ($('#BACK1_NO').val() == '') {
            AddValidationClass(false, 'BACK1_NO');
            result = false;
           
        }

        if ($('#BACK1_DATE').val() == '') {
            AddValidationClass(false, 'BACK1_DATE');
            result = false;
        }
        
        if ($('#CK3_NO').val() == '') {
            AddValidationClass(false, 'CK3_NO');
            result = false;
        }
        
        if ($('#CK3_DATE').val() == '') {
            AddValidationClass(false, 'CK3_DATE');
            result = false;
        }
        
        //if ($('#CK3_OFFICE_VALUE').val() == '') {
        //    AddValidationClass(false, 'CK3_OFFICE_VALUE');
        //    result = false;
        //}
        if ($.isNumeric($('#CK3_OFFICE_VALUE').val()) == false) {
            AddValidationClass(false, 'CK3_OFFICE_VALUE');
            result = false;
        }
        
        // alert($('#GovStatus').val());

        if ($('#GovStatus').val() == '') {
            AddValidationClass(false, 'GovStatus');
            result = false;
         
            $('#GovStatus').focus();
        } else {
            if ($('#GovStatus').val() == 'Rejected') {
                if ($('#Comment').val() == '') {
                    AddValidationClass(false, 'Comment');
                    result = false;
                   
                    $('#Comment').focus();
                }
            }

        }

        if (result == false) {
            $('#collapseFour').removeClass('collapse');
            $('#collapseFour').addClass('in');
            $("#collapseFour").css({ height: "auto" });

        }
        
        if ($('#poa_sk0').length == 0) {
            AddValidationClass(false, 'poa-files');

            if (result) {
                $('#modalBodyMessage').text('Missing attach files BACK-1 Doc');
                $('#ModalPbck4ValidateGov').modal('show');

                $('#collapseFour').removeClass('collapse');
                $('#collapseFour').addClass('in');
                $("#collapseFour").css({ height: "auto" });

            }
            result = false;
        }

       
        if ($('#poa_sk20').length == 0) {
            AddValidationClass(false, 'poa-files2');

            if (result) {
                $('#modalBodyMessage').text('Missing attach files CK-3 Doc');
                $('#ModalPbck4ValidateGov').modal('show');

                $('#collapseFour').removeClass('collapse');
                $('#collapseFour').addClass('in');
                $("#collapseFour").css({ height: "auto" });

            }
            result = false;
        }

        return result;
    }