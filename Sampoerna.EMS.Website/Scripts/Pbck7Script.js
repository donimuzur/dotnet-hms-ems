
function uploadXmlFile(url) {

    var postUrl = url;
    var fileName = $('[name="itemExcelFile"]').val().trim();
    var pos = fileName.lastIndexOf('.');
    var extension = (pos <= 0) ? '' : fileName.substring(pos);
    if (extension != '.xlsx') {
        alert('Please browse a correct excel file to upload');
        return false;
    }

    var formData = new FormData();
    var totalFiles = document.getElementById("itemExcelFile").files.length;
    var plantId = $('#PlantId').val();
    if (plantId == '') {
        alert('please select plant');
        return false;
    }
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("itemExcelFile").files[i];
        formData.append("itemExcelFile", file);
        formData.append("plantId", plantId);

    }
    $.ajax({
        type: "POST",
        url: postUrl,
        data: formData,
        dataType: 'html',
        contentType: false,
        processData: false,

        success: function (response) {
            $('#tb-upload-excel').html('');

            if (response == -1) {
                $('#btn-save-upload').attr("disabled", "disabled");
                return false;
            }
            var uploaditems = [];
            
            uploaditems = JSON.parse(response);
            if (uploaditems.length == 0) {
                $('#btn-save-upload').attr("disabled", "disabled");
                return false;
            }

            var classAction = '<td class="action">' +
           '<a href="#" onclick=" RemoveRow($(this)); "data-toggle="tooltip" data-placement="top" title="Delete"><i class="fa fa-trash-o"></i></a>' +
           '<a href="#" onclick=" EditRow($(this)); " data-toggle=" tooltip" data-placement="top" title="Edit"> <i class="fa fa-pencil-square-o"></i></a>' +
           '</td>';
            
            for (var i = 0; i < uploaditems.length; i++) {
                var tr = '<tr>' +
                    classAction +
                    createColumn(i + 1) +
                    createColumn(uploaditems[i].FaCode) +
                    createColumn(uploaditems[i].ProdTypeAlias) +
                    createColumn(uploaditems[i].Brand) +
                    createColumn(uploaditems[i].Content) +
                    createColumn(uploaditems[i].Pbck7Qty) +
                    createColumn('') +
                    createColumn(uploaditems[i].SeriesValue) +
                    createColumn(uploaditems[i].Hje) +
                    createColumn(uploaditems[i].Tariff) +
                    createColumn(uploaditems[i].FiscalYear) +
                    createColumn(uploaditems[i].ExciseValue) + 
                createColumn(uploaditems[i].Message) + '</tr>';
                $('#tb-upload-excel').append(tr);
            }
            
            if (IsValidDataUploadPbck7())
                $('#btn-save-upload').enable();
        }
    });
}

function IsValidDataUploadPbck7() {

    var datarows = GetTableData($('#Pbck7UploadTable'));

    for (var i = 0; i < datarows.length; i++) {

        if (datarows[i][13].length > 0)
            return false;
    }

    return true;
}


function createColumn(text) {

    return '<td>' + text + '</td>';
}

function createColumnWithHiddenField(text, name) {
    var displayText = text;

    if (text == null) {
        displayText = '';
    }
    return '<td><input type="hidden" name="' + name + '" value="' + text + '">' + displayText + '</td>';
}


function ajaxGetPoaByNppbkc(nppbkc, url) {

    $.ajax({
        type: 'POST',
        url: url,
        data: { nppbkcid: nppbkc },
        success: function (data) {
            $('#displayPoaList').html('');
            var poalist = '';
            for (var i = 0; i < data.length; i++) {

                poalist += data[i].PRINTED_NAME;
                if (data.length - 1 != i) {
                    poalist += ', ';
                }
            }
            $('#displayPoaList').html(poalist);
        }
    });

}


$('#btn-save-upload').click(function () {

    var uploaditems = GetTableData($('#Pbck7UploadTable'));
    
    //$('#tab-information').addClass('active');
    //$('#information').addClass('active');
    //$('#tab-upload').removeClass("active");
    //$('#upload').removeClass("active");
    
    $('#upload-tab').removeClass('active');
    $('#home-tab').addClass('active');

    $('#information').addClass('active');
    $('#upload').removeClass('active');
    

    $('#body-tb-upload').html('');
    //$('#table-upload tbody').html('');
    
    for (var i = 0; i < uploaditems.length; i++) {
      
        var tr = '<tr>' +
            createColumn(i + 1) +
            createColumnWithHiddenField(uploaditems[i][2], 'UploadItems[' + i + '].FaCode') +
            createColumnWithHiddenField(uploaditems[i][3], 'UploadItems[' + i + '].ProdTypeAlias') +
            createColumnWithHiddenField(uploaditems[i][4], 'UploadItems[' + i + '].Brand') +
            createColumnWithHiddenField(uploaditems[i][5], 'UploadItems[' + i + '].Content') +
            createColumnWithHiddenField(uploaditems[i][6], 'UploadItems[' + i + '].Pbck7Qty') +
            createColumnWithHiddenField(uploaditems[i][7], 'UploadItems[' + i + '].Back1Qty') +
            createColumnWithHiddenField(uploaditems[i][8], 'UploadItems[' + i + '].SeriesValue') +
            createColumnWithHiddenField(uploaditems[i][9], 'UploadItems[' + i + '].Hje') +
            createColumnWithHiddenField(uploaditems[i][10], 'UploadItems[' + i + '].Tariff') +
            createColumnWithHiddenField(uploaditems[i][11], 'UploadItems[' + i + '].FiscalYear') +
            createColumnWithHiddenField(uploaditems[i][12], 'UploadItems[' + i + '].ExciseValue') + '</tr>';
        $('#body-tb-upload').append(tr);
        //$('#table-upload tbody').append(tr);
    }
});

function toUploadTab() {

    $('#tab-information').removeClass('active');
    $('#information').removeClass('active');
    $('#tab-upload').addClass("active");
    $('#upload').addClass("active");
   
}


function ajaxGetPlantByNppbkc(nppbkc, url) {

    $.ajax({
        type: 'POST',
        url: url,
        data: { nppbkcid: nppbkc },
        success: function (data) {
            $('#PlantId').html('');

            for (var i = 0; i < data.length; i++) {
                $('#PlantId').append('<option value=' + data[i].Value + '>' + data[i].Value + '-' + data[i].Text + '</option>');

            }

        }
    }
    );

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

function ValidateBack1() {
    var status = '@Model.Pbck7Status';

    if (status == 'GovApproved') {
        var validCount = 0;
        var back1No = $('#Back1Dto_Back1Number').val();
        if (back1No == '') {
            AddValidationClass(false, 'Back1Dto_Back1Number');
            validCount++;
        }
        var back1Date = $('#Back1Dto_Back1Date').val();
        if (back1Date == '') {
            AddValidationClass(false, 'Back1Dto_Back1Date');
            validCount++;
        }
        if (validCount > 0) {
            return false;
        }
    }

    return true;
}

function ValidatePbck3() {
    var value = $('#Pbck3Dto_Pbck3Date').val();
    var status = '@Model.Pbck3Dto.Pbck3Status';

    var validCount = 0;
    if (value == '') {
        AddValidationClass(false, 'Pbck3Dto_Pbck3Date');
        validCount++;
    }
    if (status == 'GovApproved') {

        var back3No = $('#Back3Dto_Back3Number').val();

        if (back3No == '') {
            AddValidationClass(false, 'Back3Dto_Back3Number');
            validCount++;
        }
        var back3Date = $('#Back3Dto_Back3Date').val();

        if (back3Date == '') {
            AddValidationClass(false, 'Back3Dto_Back3Date');
            validCount++;
        }
        var ck2No = $('#Ck2Dto_Ck2Number').val();

        if (ck2No == '') {
            AddValidationClass(false, 'Ck2Dto_Ck2Number');
            validCount++;
        }
        var ck2Date = $('#Ck2Dto_Ck2Date').val();

        if (ck2Date == '') {
            AddValidationClass(false, 'Ck2Dto_Ck2Date');
            validCount++;
        }
        var ck2Val = $('#Ck2Dto_Ck2Value').val();

        if (ck2Val == '') {
            AddValidationClass(false, 'Ck2Dto_Ck2Value');
            validCount++;
        }

    }

    if (validCount > 0) {
        return false;
    }
    return true;
}

function ValidateGovInput() {
    var result = true;
    var govStatus = $('#Pbck7GovStatus').find("option:selected").val();


    if (govStatus == '') {
        AddValidationClass(false, 'Pbck7GovStatus');
        result = false;
    }


    if ($('#Pbck7GovStatus').val() == 'Rejected') {
        if ($('#Comment').val() == '') {
            AddValidationClass(false, 'Comment');
            result = false;
        }
    }

    return result;
}


function ValidatePbck7Form() {
    var result = true;

    if ($('#PlantId').find("option:selected").val() == '') {

        AddValidationClass(false, 'PlantId');
        result = false;
    }
    
    if ($('#Pbck7Date').val() == '') {
        AddValidationClass(false, 'Pbck7Date');
        result = false;
    }
    
    if ($('#DocumentType').find("option:selected").val() == '') {

        AddValidationClass(false, 'DocumentType');
        result = false;
    }

    if ($('#ExecDateFrom').val() == '') {
        AddValidationClass(false, 'ExecDateFrom');
        result = false;
    }
    
    if ($('#ExecDateTo').val() == '') {
        AddValidationClass(false, 'ExecDateTo');
        result = false;
    }

    if ($('#NppbkcId').find("option:selected").val() == '') {

        AddValidationClass(false, 'NppbkcId');
        result = false;
    }
    
    if ($('#PlantId').find("option:selected").val() == '') {

        AddValidationClass(false, 'PlantId');
        result = false;

      
    }
    
    if (!result) {
        $('#collapseOne').removeClass('collapse');
        $('#collapseOne').addClass('in');
        $("#collapseOne").css({ height: "auto" });
    }
    
    if (result) {
        var rowCount = $('#table-upload tr').length;

        if (rowCount <= 1) {

            $('#modalBodyMessage').text('Missing PBCK-7 Items');
            $('#ModalPbck7Items').modal('show');

            $('#home-tab').removeClass('active');
            $('#upload-tab').addClass('active');

            $('#information').removeClass('active');
            $('#upload').addClass('active');

            result = false;
        }
    }

    if (result) {
        rowCount = $('#table-upload-preview tr').length;

        if (rowCount <= 1) {
            // alert('Missing CK5 Material');
            $('#modalBodyMessage').text('Missing PBCK-7 Items');
            $('#ModalPbck7Items').modal('show');

            $('#home-tab').removeClass('active');
            $('#upload-tab').addClass('active');

            $('#information').removeClass('active');
            $('#upload').addClass('active');

            result = false;
        }

    }


    return result;
}

function ValidateGovInput() {
    var result = true;
    
    if ($('#Pbck7GovStatus').val() == '') {
        AddValidationClass(false, 'Pbck7GovStatus');
        result = false;

        $('#Pbck7GovStatus').focus();
    } else {
        if ($('#Pbck7GovStatus').val() == 'Rejected') {
            if ($('#Comment').val() == '') {
                AddValidationClass(false, 'Comment');
                result = false;

                $('#Comment').focus();
            }
        } 
    }

    if ($('#Pbck7GovStatus').val() != 'Rejected') {

        if ($('#Back1Dto_Back1Number').val() == '') {
            AddValidationClass(false, 'Back1Dto_Back1Number');
            result = false;

        }

        if ($('#Back1Dto_Back1Date').val() == '') {
            AddValidationClass(false, 'Back1Dto_Back1Date');
            result = false;
        }

        if ($('#poa_sk0').length == 0) {
            AddValidationClass(false, 'poa-files');

            if (result) {
                $('#modalBodyMessage').text('Missing attach files BACK-1 Doc');
                $('#ModalPbck7ValidateGov').modal('show');

                $('#collapseFour').removeClass('collapse');
                $('#collapseFour').addClass('in');
                $("#collapseFour").css({ height: "auto" });

            }
            result = false;
        }
    }
    if (result == false) {
        $('#collapseFour').removeClass('collapse');
        $('#collapseFour').addClass('in');
        $("#collapseFour").css({ height: "auto" });

    }

   

    return result;
}