$("#btnSaveMaterial").click(function () {
    AddRowPbck7();
});

$("#btnUpdateMaterial").click(function () {

    UpdateRowPbck7();
});

function RemovePbck7ItemsTable() {

    //$('#pbck4TableItem tbody').html('');
    $('#body-tb-upload').html('');
    $('#Pbck7UploadTable tbody').html('');

}

function ajaxGetListFaCode2(url, formData, facode) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                var listMaterial = $('#uploadFaCode');
                listMaterial.empty();

                var list = '<option value>Select</option>';

                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        list += "<option value='" + data[i].FaCode + "'>" + data[i].FaCode + "</option>";
                    }

                }

                listMaterial.html(list);

                $('#uploadFaCode').val(facode);

            }
        });
    }
}

function ajaxGetListFaCode(url, formData, selectedValue) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                var listMaterial = $('#uploadFaCode');
                listMaterial.empty();

                var list = '<option value>Select</option>';

                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        list += "<option value='" + data[i].FaCode + "'>" + data[i].FaCode + "</option>";
                    }

                }

                listMaterial.html(list);
                if (selectedValue != '') {
                    $('#uploadFaCode').val(selectedValue);

                }


            }
        });
    }
}


function ajaxGetBrandItems(url, formData) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {

                if (data != null) {
                    $("#uploadProductTypeAlias").val(data.ProductAlias);
                    $("#uploadBrand").val(data.BrandName);
                    $("#uploadContent").val(data.BrandContent);
                    $("#uploadSeriesValue").val(data.SeriesValue);
                    $("#uploadHje").val(data.Hje);
                    $("#uploadTariff").val(data.Tariff);
                    $("#uploadBlockedStocked").val(data.BlockedStockRemaining);
                }


            }
        });
    }
}



function ValidateManualPbck7() {

    var result = true;

    AddValidationClass(true, 'uploadFaCode');
    if ($('#uploadFaCode').find("option:selected").val() == '') {
        AddValidationClass(false, 'uploadFaCode');
        result = false;
    }

   

    AddValidationClass(true, 'uploadPbck7Qty');
    if ($.isNumeric($('#uploadPbck7Qty').val()) == false) {
        AddValidationClass(false, 'uploadPbck7Qty');
        result = false;
    } else {
        if (parseFloat($('#uploadPbck7Qty').val()) <= 0) {
            AddValidationClass(false, 'uploadPbck7Qty');
            result = false;
        }
    }

    if (result) {

        if (parseFloat($('#uploadPbck7Qty').val()) > parseFloat($('#uploadBlockedStocked').val())) {
            AddValidationClass(false, 'uploadPbck7Qty');
            result = false;
        }
    }
    
    AddValidationClass(true, 'uploadFiscalYear');
    if ($.isNumeric($('#uploadFiscalYear').val()) == false) {
        AddValidationClass(false, 'uploadFiscalYear');
        result = false;
    } else {
        if (parseFloat($('#uploadFiscalYear').val()) < 0) {
            AddValidationClass(false, 'uploadFiscalYear');
            result = false;
        }
    }
   
    return result;
}

function RemoveRow(o) {


    var nRow = o.parents('tr');
    nRow.remove();
    $('#Pbck7UploadSave').enable();
    if ($('#Ck5UploadTable tr').length == 1) {
        $('#Pbck4UploadSave').prop('disabled', true);
    }
}


function ClearInputForm() {
    $("#uploadFaCode").val($("#uploadMaterialNumber option:first").val());
   
    $("#uploadProductTypeAlias").val('');
    $("#uploadBrand").val('');

    $("#uploadContent").val('');
    $("#uploadPbck7Qty").val('0');
    $("#uploadBack1Qty").val('');
    $("#uploadSeriesValue").val('');

    $("#uploadHje").val('');
    $("#uploadTariff").val('');
    $("#uploadFiscalYear").val('');
    $("#uploadExciseValue").val('');
    $("#uploadBlockedStocked").val('0');


}

function ClearValidation() {
    AddValidationClass(true, 'uploadFaCode');
    AddValidationClass(true, 'uploadPbck7Qty');
    AddValidationClass(true, 'uploadFiscalYear');
}


function AddRowPbck7() {

    if (ValidateManualPbck7()) {

        $('#Pbck7UploadModal').modal('hide');

       
        var totalExcise = parseFloat($('#uploadTariff').val()) * parseFloat($('#uploadPbck7Qty').val()) * parseFloat($('#uploadContent').val());
        var rowCount = $('#Pbck7UploadTable tr').length;


        var classAction = '<td class="action">' +
            '<a href="#" onclick=" RemoveRow($(this)); "data-toggle="tooltip" data-placement="top" title="Delete"><i class="fa fa-trash-o"></i></a>' +
            '<a href="#" onclick=" EditRow($(this)); " data-toggle=" tooltip" data-placement="top" title="Edit"> <i class="fa fa-pencil-square-o"></i></a>' +
            '</td>';

        $("#Pbck7UploadTable tbody").append(
            "<tr>" +
                 classAction +
                 "<td>" + (rowCount) + "</td>" +
                "<td>" + $('#uploadFaCode').val() + "</td>" +
                "<td>" + $('#uploadProductTypeAlias').val() + "</td>" +
              
                "<td>" + $('#uploadBrand').val() + "</td>" +
                 "<td>" + $('#uploadContent').val() + "</td>" +
                 "<td>" + $('#uploadPbck7Qty').val() + "</td>" +
                 "<td></td>" +
                 "<td>" + $('#uploadSeriesValue').val() + "</td>" +
                 "<td>" + $('#uploadHje').val() + "</td>" +
                 "<td>" + $('#uploadTariff').val() + "</td>" +
                 "<td>" + $('#uploadFiscalYear').val() + "</td>" +
                  "<td>" + totalExcise.toFixed(2) + "</td>" +
                "<td></td>" +
               "<td style='display: none'>" + $('#uploadBlockedStocked').val() + "</td>" +

                "</tr>");

        $('#btn-save-upload').enable();
    }


}




function UpdateRowPbck7() {
    if (ValidateManualPbck7()) {

        var row = $('#uploadMaterialRow').val();

        var totalExcise = parseFloat($('#uploadTariff').val()) * parseFloat($('#uploadPbck7Qty').val()) * parseFloat($('#uploadContent').val());
        

        $('#Pbck7UploadTable tr').each(function () {

            if ($(this).find('td').eq(1).text() == row) {

                $(this).find('td').eq(2).text($('#uploadFaCode').val());
                $(this).find('td').eq(3).text($('#uploadProductTypeAlias').val());
                $(this).find('td').eq(4).text($('#uploadBrand').val());
                $(this).find('td').eq(5).text($('#uploadContent').val());
                $(this).find('td').eq(6).text($('#uploadPbck7Qty').val());
               //back-1 qty 7
                $(this).find('td').eq(8).text($('#uploadSeriesValue').val());
                $(this).find('td').eq(9).text($('#uploadHje').val());
                $(this).find('td').eq(10).text($('#uploadTariff').val());
                
                $(this).find('td').eq(11).text($('#uploadFiscalYear').val());
             
                $(this).find('td').eq(12).text(totalExcise.toFixed(2));
                $(this).find('td').eq(13).text('');
                $(this).find('td').eq(14).text($('#uploadBlockedStocked').val());

            }
        });
        $('#Pbck7UploadModal').modal('hide');

        $('#btn-save-upload').enable();
    }
}

function UpdateRowPbck7Detail() {

    AddValidationClass(true, 'uploadApprovedQty');
    if ($.isNumeric($('#uploadApprovedQty').val()) == false) {
        AddValidationClass(false, 'uploadApprovedQty');
        return;
    } else {
        if (parseFloat($('#uploadApprovedQty').val()) < 0) {
            AddValidationClass(false, 'uploadApprovedQty');
            return;
        }
    }

    var row = $('#uploadMaterialRow').val();

    var totalHje = parseFloat($('#uploadHje').val()) * parseFloat($('#uploadReqQty').val());
    var totalTariff = parseFloat($('#uploadTariff').val()) * parseFloat($('#uploadReqQty').val()) * parseFloat($('#uploadContent').val());



    $('#Ck5UploadTable tr').each(function () {

        if ($(this).find('td').eq(1).text() == row) {

            $(this).find('td').eq(2).text($('#uploadFaCode').val());
            $(this).find('td').eq(3).text($('#uploadStickerCode').val());
            //$(this).find('td').eq(4).text($('#uploadCk1No').val());
            $(this).find('td').eq(4).text($("#uploadCk1No option:selected").text());

            $(this).find('td').eq(5).text($('#uploadCk1Date').val());
            $(this).find('td').eq(6).text($('#uploadSeriesCode').val());
            $(this).find('td').eq(7).text($('#uploadBrandName').val());
            $(this).find('td').eq(8).text($('#uploadProductAlias').val());
            $(this).find('td').eq(9).text($('#uploadContent').val());
            $(this).find('td').eq(10).text($('#uploadHje').val());
            $(this).find('td').eq(11).text($('#uploadTariff').val());
            $(this).find('td').eq(12).text($('#uploadColour').val());
            $(this).find('td').eq(13).text($('#uploadReqQty').val());


            $(this).find('td').eq(14).text(totalHje.toFixed(3));
            $(this).find('td').eq(15).text(totalTariff.toFixed(3));

            $(this).find('td').eq(16).text($('#uploadNoPengawas').val());
            $(this).find('td').eq(17).text($('#uploadApprovedQty').val());
            $(this).find('td').eq(18).text($('#uploadRemarks').val());

            $(this).find('td').eq(20).text($('#uploadCk1Id').val());
            $(this).find('td').eq(21).text($('#uploadBlockedStocked').val());
            $(this).find('td').eq(22).text($('#uploadPbck4ItemId').val());
            $(this).find('td').eq(23).text('True');

        }
    });
    $('#Pbck7UploadModal').modal('hide');

    $('#Pbck7UploadSave').enable();

}

function ajaxGetBrandItemsForEdit(url, formData) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {

                if (data != null) {
                    $("#uploadProductTypeAlias").val(data.ProductAlias);
                    $("#uploadBrand").val(data.BrandName);
                    $("#uploadContent").val(data.BrandContent);
                    $("#uploadSeriesValue").val(data.SeriesValue);
                    $("#uploadHje").val(data.Hje);
                    $("#uploadTariff").val(data.Tariff);
                    $("#uploadBlockedStocked").val(data.BlockedStockRemaining);
                    
                }


            }
        });
    }
}