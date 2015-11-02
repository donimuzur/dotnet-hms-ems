
function ClearInputForm() {
    $("#uploadMaterialNumber").val($("#uploadMaterialNumber option:first").val());
    $("#uploadMaterialUom").val($("#uploadMaterialUom option:first").val());
    $("#uploadConvertedUom").val($("#uploadConvertedUom option:first").val());
    $("#uploadMaterialQty").val('0');
    $("#uploadMaterialHje").val('0');
    $("#uploadMaterialTariff").val('0');
    $("#uploadMaterialConvertion").val('0');
    $("#uploadUsdValue").val('0');
    $("#uploadNote").val('');

   

}

function ClearValidation() {
    AddValidationClass(true, 'uploadMaterialNumber');
    AddValidationClass(true, 'uploadMaterialUom');
    AddValidationClass(true, 'uploadConvertedUom');
    
    AddValidationClass(true, 'uploadMaterialQty');
    AddValidationClass(true, 'uploadMaterialConvertion');
}

function EditRow(o,url) {
    //debugger;
    var nRow = o.parents('tr');
    var plant = nRow.find("td").eq(19).html();
    var materialnumber = nRow.find("td").eq(2).html();

    //if ('@Model.Ck5Type.ToString()' == "PortToImporter" || '@Model.Ck5Type.ToString()' == "DomesticAlcohol") {
    //    plant = $("#DestPlantId").find("option:selected").val();
    //}

    
    var goodTypeGroupId = $("#GoodType").val();
    ajaxGetListMaterial(url, { plantId: plant, goodTypeGroup: goodTypeGroupId }, materialnumber);
    
    $('#btnSaveMaterial').hide();
    $('#btnUpdateMaterial').show();

    //set value
    //if ('@Model.Ck5Type.ToString()' == "PortToImporter" || '@Model.Ck5Type.ToString()' == "DomesticAlcohol") {
    //    plant = $("#DestPlantId").find("option:selected").val();
    //}
    
   // $("#uploadMaterialPlant").val(nRow.find("td").eq(19).html());
    $('#uploadMaterialRow').val(nRow.find("td").eq(1).html());
    
    $('#uploadMaterialNumber').find('option:not(:first)').remove();
    //===Fixing Bug CK5 no. 110====
    $('#uploadMaterialNumber').append('<option>' + nRow.find("td").eq(2).html() + '</option>');
    //=============================

    $('#uploadMaterialNumber').val(materialnumber);
    $('#uploadMaterialQty').val(nRow.find("td").eq(3).html());

    $('#uploadMaterialUom').val(nRow.find("td").eq(4).html());
    $('#uploadMaterialConvertion').val(nRow.find("td").eq(5).html());
    
    $('#uploadConvertedUom').val(nRow.find("td").eq(7).html());
    
    $('#uploadMaterialHje').val(nRow.find("td").eq(8).html());
    $('#uploadMaterialTariff').val(nRow.find("td").eq(9).html());
    
    $('#uploadUsdValue').val(nRow.find("td").eq(11).html());
    
    $('#uploadNote').val(nRow.find("td").eq(12).html());
    
    $('#uploadMaterialDesc').val(nRow.find("td").eq(15).html());

    //===Fixing Bug CK5 no. 110====
    $('#uploadMaterialPlant').val(nRow.find("td").eq(19).html());
    //=============================
    
    $('#uploadMaterialId').val(nRow.find("td").eq(18).html());
    
    ClearValidation();
    
    $("#lblTitleInputManual").text('Edit Input');
    
    $('#Ck5UploadModal').modal('show');


}

function RemoveRow(o) {


    var nRow = o.parents('tr');
    nRow.remove();
    $('#CK5UploadSave').enable();
    if ($('#Ck5UploadTable tr').length == 1) {
        $('#CK5UploadSave').prop('disabled', true);
    } 
}

function ValidateManual() {

    var result = true;
   
    AddValidationClass(true, 'uploadMaterialNumber');
    if ($('#uploadMaterialNumber').find("option:selected").val() == '') {
        AddValidationClass(false, 'uploadMaterialNumber');
        result = false;
    } 

    AddValidationClass(true, 'uploadMaterialUom');
    if ($('#uploadMaterialUom').find("option:selected").val() == '') {
        AddValidationClass(false, 'uploadMaterialUom');
        result = false;
    }

    AddValidationClass(true, 'uploadConvertedUom');
    if ($('#uploadConvertedUom').find("option:selected").val() == '') {
        AddValidationClass(false, 'uploadConvertedUom');
        result = false;
    }

   
    AddValidationClass(true, 'uploadMaterialQty');
    if ($.isNumeric($('#uploadMaterialQty').val()) == false) {
        AddValidationClass(false, 'uploadMaterialQty');
        result = false;
    }
    
    AddValidationClass(true, 'uploadMaterialConvertion');
    if ($.isNumeric($('#uploadMaterialConvertion').val()) == false) {
        AddValidationClass(false, 'uploadMaterialConvertion');
        result = false;
    }
   
    if ($.isNumeric($('#uploadUsdValue').val()) == false) {
        $('#uploadUsdValue').val('0');
    }
    
    return result;
}


function UpdateRow(data) {
    //if (ValidateManual()) {

        var row = $('#uploadMaterialRow').val();

        var convertedQty = parseFloat($('#uploadMaterialQty').val()) * parseFloat($('#uploadMaterialConvertion').val());
        var total = parseFloat($('#uploadMaterialTariff').val()) * convertedQty;
        var exciseQty = convertedQty;
        var exciseUOM = $('#uploadConvertedUom').val();

        var materialId = $('#uploadMaterialId').val();
        if ($.isNumeric(materialId) == false) {
            materialId = 0;
        }
        
        if (exciseUOM == "KG") {
            exciseUOM = "G";
            exciseQty = convertedQty * 1000;
        } 
        
        var dataerror = "";
        if (data.error != null) {
            dataerror = data.error;
        }

        $('#Ck5UploadTable tr').each(function() {

            if ($(this).find('td').eq(1).text() == row) {

                $(this).find('td').eq(2).text($('#uploadMaterialNumber').val());
                $(this).find('td').eq(3).text($('#uploadMaterialQty').val());
                $(this).find('td').eq(4).text($('#uploadMaterialUom').val());
                $(this).find('td').eq(5).text($('#uploadMaterialConvertion').val());
                $(this).find('td').eq(6).text(convertedQty.toFixed(3));
                $(this).find('td').eq(7).text($('#uploadConvertedUom').val());
                $(this).find('td').eq(8).text($('#uploadMaterialHje').val());
                $(this).find('td').eq(9).text($('#uploadMaterialTariff').val());
                $(this).find('td').eq(10).text(total.toFixed(3));
                $(this).find('td').eq(11).text($('#uploadUsdValue').val());
                $(this).find('td').eq(12).text($('#uploadNote').val());
                $(this).find('td').eq(13).text(dataerror);
                $(this).find('td').eq(15).text($('#uploadMaterialDesc').val());
                
                
                $(this).find('td').eq(16).text(exciseQty);
                $(this).find('td').eq(17).text(exciseUOM);
                $(this).find('td').eq(18).text(materialId);
                $(this).find('td').eq(19).text($('#uploadMaterialPlant').val());
               
            }
        });
        $('#Ck5UploadModal').modal('hide');
        if (data.success) {
            $('#CK5UploadSave').prop('disabled', false);
        } else {
            $('#CK5UploadSave').prop('disabled', true);
        }
    //}
}
function AddRow(url,data) {

    //if (ValidateManual()) {
        
        $('#Ck5UploadModal').modal('hide');

        var convertedQty = parseFloat($('#uploadMaterialQty').val()) * parseFloat($('#uploadMaterialConvertion').val());
        var total = parseFloat($('#uploadMaterialTariff').val()) * convertedQty;
        var rowCount = $('#Ck5UploadTable tr').length;
        
        var exciseQty = convertedQty;
        var exciseUOM = $('#uploadConvertedUom').val();



        if (exciseUOM == "KG") {
            exciseUOM = "G";
            exciseQty = convertedQty * 1000;
        }

        var classAction = '<td class="action">' +
            '<a href="#" onclick=" RemoveRow($(this)); "data-toggle="tooltip" data-placement="top" title="Delete"><i class="fa fa-trash-o"></i></a>' +
            '<a href="#" onclick=" EditRow($(this),\''+url+'\'); " data-toggle=" tooltip" data-placement="top" title="Edit"> <i class="fa fa-pencil-square-o"></i></a>' +
            '</td>';
        var dataerror = "";
        if (data.error != null) {
            dataerror = data.error;
        }
        $("#Ck5UploadTable tbody").append(
            "<tr>" +
                 classAction +
                 "<td style='display: none'>" + (rowCount) + "</td>" +
                "<td>" + $('#uploadMaterialNumber').val() + "</td>" +
                "<td>" + $('#uploadMaterialQty').val() + "</td>" +
                "<td>" + $('#uploadMaterialUom').val() + "</td>" +
                "<td>" + $('#uploadMaterialConvertion').val() + "</td>" +
                "<td>" + convertedQty.toFixed(3) + "</td>" +
                "<td>" + $('#uploadConvertedUom').val() + "</td>" +
                "<td>" + $('#uploadMaterialHje').val() + "</td>" +
                "<td>" + $('#uploadMaterialTariff').val() + "</td>" +
                "<td>" + total.toFixed(3) + "</td>" +
                "<td>" + $('#uploadUsdValue').val() + "</td>" +
                "<td>" + $('#uploadNote').val() + "</td>" +
                "<td>" + dataerror + "</td>" +
                "<td style='display: none'></td>" +
                "<td style='display: none'>" + $('#uploadMaterialDesc').val() + "</td>" +
                "<td style='display: none'>" + exciseQty + "</td>" +
                "<td style='display: none'>" + exciseUOM + "</td>" +
                "<td style='display: none'>0</td>" +
                "<td style='display: none'>" + $('#uploadMaterialPlant').val() + "</td>" +
                "</tr>");
        
        if (data.success) {
            $('#CK5UploadSave').prop('disabled', false);
        } else {
            $('#CK5UploadSave').prop('disabled', true);
        }
        
    //}
    
  
}