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

function ajaxGetListCk1(url, formData, selectedText) {
    if (formData.nppbkcId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                var listCk1 = $('#uploadCk1No');
                listCk1.empty();

                var list = '<option value>Select</option>';

                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        list += "<option value='" + data[i].Ck1Id + "'>" + data[i].Ck1No + "</option>";
                    }
                }
                
                listCk1.html(list);
                if (selectedText != '') {
                    $("#uploadCk1No").each(function () {
                        $('option', this).each(function () {
                            if ($(this).text() == selectedText) {
                                $(this).attr('selected', 'selected');
                            };
                        });
                    });
                }
            }
        });
    }
}


function ajaxGetListCk1DateByPlantAndFaCode(url, formData, selectedText) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                var listCk1 = $('#uploadCk1No');
                listCk1.empty();

                var list = '<option value>Select</option>';

                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        list += "<option value='" + data[i].Ck1Id + "'>" + data[i].Ck1No + "</option>";
                    }
                }

                listCk1.html(list);
              
                if (selectedText != '') {
                    $("#uploadCk1No").each(function () {
                        $('option', this).each(function () {
                            if ($(this).text() == selectedText) {
                                $(this).attr('selected', 'selected');
                            };
                        });
                    });
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
                    $("#uploadStickerCode").val(data.StickerCode);
                    $("#uploadSeriesCode").val(data.SeriesCode);
                    $("#uploadBrandName").val(data.BrandName);
                    $("#uploadProductAlias").val(data.ProductAlias);
                    $("#uploadContent").val(data.BrandContent);
                    $("#uploadHje").val(data.Hje);
                    $("#uploadTariff").val(data.Tariff);
                    $("#uploadColour").val(data.Colour);
                    $("#uploadBlockedStocked").val(data.BlockedStockRemaining);
                    
                    var listCk1 = $('#uploadCk1No');
                    listCk1.empty();

                    var list = '<option value>Select</option>';

                    if (data.ListCk1Date != null) {
                        for (var i = 0; i < data.ListCk1Date.length; i++) {
                            list += "<option value='" + data.ListCk1Date[i].Ck1Id + "'>" + data.ListCk1Date[i].Ck1No + "</option>";
                        }
                    }
                    
                    listCk1.html(list);
                    
                }


            }
        });
    }
}


function ajaxGetBrandItemsForEdit(url, formData) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {

                if (data != null) {
                    $("#uploadStickerCode").val(data.StickerCode);
                    $("#uploadSeriesCode").val(data.SeriesCode);
                    $("#uploadBrandName").val(data.BrandName);
                    $("#uploadProductAlias").val(data.ProductAlias);
                    $("#uploadContent").val(data.BrandContent);
                    $("#uploadHje").val(data.Hje);
                    $("#uploadTariff").val(data.Tariff);
                    $("#uploadColour").val(data.Colour);
                    $("#uploadBlockedStocked").val(data.BlockedStockRemaining);
                    
                    var listCk1 = $('#uploadCk1No');
                    listCk1.empty();

                    var list = '<option value>Select</option>';

                    if (data.ListCk1Date != null) {
                        for (var i = 0; i < data.ListCk1Date.length; i++) {
                            list += "<option value='" + data.ListCk1Date[i].Ck1Id + "'>" + data.ListCk1Date[i].Ck1No + "</option>";
                        }
                    }

                    listCk1.html(list);
                }


            }
        });
    }
}


function ajaxGetCk1Date(url, formData) {
    if (formData.ck1Id) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {

                if (data != null) {
                    $("#uploadCk1Date").val(data);
                    $("#uploadCk1Id").val(formData.ck1Id);
                }


            }
        });
    }
}

function ValidateManualPbck4() {

    var result = true;

    AddValidationClass(true, 'uploadFaCode');
    if ($('#uploadFaCode').find("option:selected").val() == '') {
        AddValidationClass(false, 'uploadFaCode');
        result = false;
    }

    AddValidationClass(true, 'uploadCk1No');
    if ($('#uploadCk1No').find("option:selected").val() == '') {
        AddValidationClass(false, 'uploadCk1No');
        result = false;
    }

    AddValidationClass(true, 'uploadReqQty');
    if ($.isNumeric($('#uploadReqQty').val()) == false) {
        AddValidationClass(false, 'uploadReqQty');
        result = false;
    } else {
        if (parseFloat($('#uploadReqQty').val()) < 0) {
            AddValidationClass(false, 'uploadReqQty');
            result = false;
        }
    }
    
    if (result) {
        
        if (parseFloat($('#uploadReqQty').val()) > parseFloat($('#uploadBlockedStocked').val())) {
            AddValidationClass(false, 'uploadReqQty');
            result = false;
        }
    }

    AddValidationClass(true, 'uploadApprovedQty');
    if ($.isNumeric($('#uploadApprovedQty').val()) == false) {
        AddValidationClass(false, 'uploadApprovedQty');
        result = false;
    } 
    return result;
}

function RemoveRow(o) {


    var nRow = o.parents('tr');
    nRow.remove();
    $('#Pbck4UploadSave').enable();
    if ($('#Ck5UploadTable tr').length == 1) {
        $('#Pbck4UploadSave').prop('disabled', true);
    }
}


function ClearInputForm() {
    $("#uploadFaCode").val($("#uploadMaterialNumber option:first").val());
    $("#uploadCk1No").val($("#uploadMaterialUom option:first").val());
    
    $("#uploadReqQty").val('0');
    $("#uploadApprovedQty").val('0');
    
    $("#uploadStickerCode").val('');
    $("#uploadCk1Date").val('');
    $("#uploadSeriesCode").val('');
    $("#uploadBrandName").val('');
    
    $("#uploadProductAlias").val('');
    $("#uploadContent").val('');
    $("#uploadHje").val('');
    $("#uploadTariff").val('');
    $("#uploadColour").val('');
    $("#uploadBlockedStocked").val('0');


}

function ClearValidation() {
    AddValidationClass(true, 'uploadFaCode');
    AddValidationClass(true, 'uploadCk1No');
    AddValidationClass(true, 'uploadReqQty');
    AddValidationClass(true, 'uploadApprovedQty');
    
}


function AddRowPbck4() {

    if (ValidateManualPbck4()) {

        $('#Pbck4UploadModal').modal('hide');

        var totalHje = parseFloat($('#uploadHje').val()) * parseFloat($('#uploadReqQty').val());
        var totalTariff = parseFloat($('#uploadTariff').val()) * parseFloat($('#uploadReqQty').val())  * parseFloat($('#uploadContent').val());
        var rowCount = $('#Ck5UploadTable tr').length;


        var classAction = '<td class="action">' +
            '<a href="#" onclick=" RemoveRow($(this)); "data-toggle="tooltip" data-placement="top" title="Delete"><i class="fa fa-trash-o"></i></a>' +
            '<a href="#" onclick=" EditRow($(this)); " data-toggle=" tooltip" data-placement="top" title="Edit"> <i class="fa fa-pencil-square-o"></i></a>' +
            '</td>';

        $("#Ck5UploadTable tbody").append(
            "<tr>" +
                 classAction +
                 "<td style='display: none'>" + (rowCount) + "</td>" +
                "<td>" + $('#uploadFaCode').val() + "</td>" +
                "<td>" + $('#uploadStickerCode').val() + "</td>" +
                "<td>" + $("#uploadCk1No option:selected").text() + "</td>" +
                "<td>" + $('#uploadCk1Date').val() + "</td>" +
                 "<td>" + $('#uploadSeriesCode').val() + "</td>" +
                 "<td>" + $('#uploadBrandName').val() + "</td>" +
                 "<td>" + $('#uploadProductAlias').val() + "</td>" +
                 "<td>" + $('#uploadContent').val() + "</td>" +
                 "<td>" + $('#uploadHje').val() + "</td>" +
                 "<td>" + $('#uploadTariff').val() + "</td>" +
                 "<td>" + $('#uploadColour').val() + "</td>" +

                 "<td>" + $('#uploadReqQty').val() + "</td>" +
                  "<td>" + totalHje.toFixed(2) + "</td>" +
                  "<td>" + totalTariff.toFixed(2) + "</td>" +
                 "<td>" + $('#uploadNoPengawas').val() + "</td>" +
                 "<td>" + $('#uploadApprovedQty').val() + "</td>" +
                 "<td>" + $('#uploadRemarks').val() + "</td>" +

                "<td></td>" +
                "<td style='display: none'>" + $('#uploadCk1Id').val() + "</td>" +
                "<td style='display: none'>" + $('#uploadBlockedStocked').val() + "</td>" +
               
              
                "</tr>");

        $('#Pbck4UploadSave').enable();
    }


}




function UpdateRowPbck4() {
    if (ValidateManualPbck4()) {

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

            }
        });
        $('#Pbck4UploadModal').modal('hide');

        $('#Pbck4UploadSave').enable();
    }
}
