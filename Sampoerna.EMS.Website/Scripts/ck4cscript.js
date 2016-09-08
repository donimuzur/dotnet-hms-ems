﻿$(document).ready(function () {
    if ($('#btnCancelCompleted').length)
    {
        $('#MenuCk4cCompleted').addClass('active');
    }
    else {
        $('#MenuCk4cDocumentList').addClass('active');
    }
});

function ValidateInput() {
    var result = true;
    AddValidationClass(true, 'Details_PlantId');
    AddValidationClass(true, 'Details_NppbkcId');
    AddValidationClass(true, 'Details_ReportedOn');

    if ($('#Details_CompanyId').val() == '') {
        AddValidationClass(false, 'Details_CompanyId');
        result = false;
    }

    if ($('#Details_ReportedOn').val() == '') {
        AddValidationClass(false, 'Details_ReportedOn');
        result = false;
    }

    if ($('#Details_ReportedPeriod').val() == '') {
        AddValidationClass(false, 'Details_ReportedPeriod');
        result = false;
    }

    if ($('#Details_ReportedMonth').val() == '') {
        AddValidationClass(false, 'Details_ReportedMonth');
        result = false;
    }

    if ($('#Details_ReportedYears').val() == '') {
        AddValidationClass(false, 'Details_ReportedYears');
        result = false;
    }

    if ($('#Details_PlantId').is( ":disabled" )) {
        if ($('#Details_NppbkcId').val() == '') {
            AddValidationClass(false, 'Details_NppbkcId');
            result = false;
        }
    }

    if ($('#Details_NppbkcId').is(":disabled")) {
        if ($('#Details_PlantId').val() == '') {
            AddValidationClass(false, 'Details_PlantId');
            result = false;
        }
    }

    return result;
}

function ValidateGovInput() {
    var result = true;
    var govStatus = $('#Details_StatusGoverment').find("option:selected").val();
    AddValidationClass(true, 'Details_ReportedOn');

    if ($('#Details_ReportedOn').val() == '') {
        AddValidationClass(false, 'Details_ReportedOn');
        result = false;
    }

    if (govStatus == '') {
        AddValidationClass(false, 'Details_StatusGoverment');
        result = false;
    }

    if ($('#Details_DecreeDate').val() == '') {
        AddValidationClass(false, 'Details_DecreeDate');
        result = false;
    }

    if ($('#Details_StatusGoverment').val() == 'Rejected') {
        if ($('#Details_Comment').val() == '') {
            AddValidationClass(false, 'Details_Comment');
            result = false;
        }
    }

    if ($("#poa-files .row").length == 0) {
        $('#ModalCk4cDoc').modal('show');
        result = false;
    }

    return result;
}

function ValidateCompletedInput() {
    var result = true;
    var govStatus = $('#Details_StatusGoverment').find("option:selected").val();

    if (govStatus == '' && $('#Details_DecreeDate').val() == '' && $("#poa-files .row").length == 0) {
        return true;
    }

    if (govStatus == '') {
        AddValidationClass(false, 'Details_StatusGoverment');
        result = false;
    }

    if ($('#Details_DecreeDate').val() == '') {
        AddValidationClass(false, 'Details_DecreeDate');
        result = false;
    }

    if ($('#Details_StatusGoverment').val() == 'Rejected') {
        if ($('#Details_Comment').val() == '') {
            AddValidationClass(false, 'Details_Comment');
            result = false;
        }
    }

    if ($("#poa-files .row").length == 0) {
        $('#ModalCk4cDoc').modal('show');
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

function ChangeBasedOn(value) {
    var IsDisabledPlant = false;
    var IsDisabledNppbkc = true;

    if (value == 2) {
        IsDisabledPlant = true;
        IsDisabledNppbkc = false;
    }

    $('#Details_PlantId').val('');
    $('#Details_NppbkcId').val('');
    $('#Details_PlantId').prop('disabled', IsDisabledPlant);
    $('#Details_NppbkcId').prop('disabled', IsDisabledNppbkc);
}

function ajaxSelectPlant(url, formData, plant) {
    if (formData.companyId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                var listPlant = data.PlanList;
                if (listPlant.length > 0) {
                    for (var i = 0; i < listPlant.length; i++) {
                        $('#Details_PlantId').append('<option value=' + listPlant[i].Value + '>' + listPlant[i].Text + '</option>');
                    }

                    $('#Details_PlantId').val(plant);
                }
            }
        });
    }
}

function ajaxGetNppbkcByCompany(urlFunction, company, nppbkc) {
    $.ajax({
        type: 'POST',
        url: urlFunction,
        data: { companyId: company },
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#Details_NppbkcId').append('<option value=' + data[i].NPPBKC_ID + '>' + data[i].NPPBKC_ID + '</option>');
            }

            $('#Details_NppbkcId').val(nppbkc);
        }
    });
}

function getProductionData(urlFunction) {
    $('.loading').show();
    $(".wipHead").css("display", "none");

    var comp = $('#Details_CompanyId').find("option:selected").val();
    var plant = $('#Details_PlantId').find("option:selected").val();
    var nppbkc = $('#Details_NppbkcId').find("option:selected").val();
    var period = $('#Details_ReportedPeriod').find("option:selected").val();
    var month = $('#Details_ReportedMonth').find("option:selected").val();
    var year = $('#Details_ReportedYears').find("option:selected").val();
    var isNppbkc = $('#Details_PlantId').is(":disabled");
    var isPlant = $('#Details_NppbkcId').is(":disabled");

    $('#CompanyId').val(comp);
    $('#PlantId').val(plant);
    $('#NppbkcId').val(nppbkc);
    $('#PeriodId').val(period);
    $('#MonthId').val(month);
    $('#YearId').val(year);

    if (comp == "" || (plant == "" && isPlant) || (nppbkc == "" && isNppbkc) || period == "" || month == "" || year == "") {
        $('.loading').hide();
        $('#tb-body-ck4c').html("");
        $("#WipLabel").css("visibility", "hidden");
        $('#tb-body-ck4c').append('<tr><td style="text-align:center" colspan="18">no data<td></tr>');
    }
    else {
        $.ajax({
            type: 'POST',
            url: urlFunction,
            data: { comp: comp, plant: plant, nppbkc: nppbkc, period: period, month: month, year: year, isNppbkc: isNppbkc },
            success: function (data) {
                $('.loading').hide();
                if (data.length > 0) {
                    $('#tb-body-ck4c').html("");
                    $("#WipLabel").css("visibility", "visible");
                    for (var i = 0; i < data.length; i++) {
                        var dt = new Date(parseInt(data[i].ProductionDate.substr(6)));
                        var dtStr = dt.toString();
                        var date = dtStr.substr(8, 2) + " " + (dtStr.substr(4, 3)) + " " + dtStr.substr(11, 4);
                        var str = (dt.getMonth() + 1) + '/' + dt.getDate() + '/' + dt.getFullYear();

                        var classAction = '<td class="action">' +
                                            '<a href="#" onclick=" EditRow($(this)); " data-toggle=" tooltip" data-placement="top" title="Edit"> <i class="fa fa-pencil-square-o"></i></a>' +
                                            '</td>';

                        var rowCount = (i + 1);
                        var classRemarks = "Remarks" + rowCount;
                        var classWip = "Unpack" + rowCount;

                        var tableProdItem = '<tr>' +
                            classAction +
                            "<td style='display: none'>" + rowCount + "</td>" +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_ProdDate" name="Details.Ck4cItemData[' + i + '].ProdDate" value=' + str + '></input>' + date + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_FaCode" name="Details.Ck4cItemData[' + i + '].FaCode" value=' + data[i].FaCode + '></input>' + data[i].FaCode + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_PackedQty" name="Details.Ck4cItemData[' + i + '].PackedQty" value=' + data[i].QtyPacked + '></input>' + data[i].BrandDescription + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_Werks" name="Details.Ck4cItemData[' + i + '].Werks" value=' + data[i].PlantWerks + '></input>' + data[i].PlantWerks + "-" + data[i].PlantName + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_UnpackedQty" name="Details.Ck4cItemData[' + i + '].UnpackedQty" value=' + data[i].QtyUnpacked + ' class=' + classWip + '></input>' + data[i].TobaccoProductType + '</td>' +
                            '<td class="number"><input type="hidden" id="Details_Ck4cItemData[' + i + ']_HjeIdr" name="Details.Ck4cItemData[' + i + '].HjeIdr" value=' + data[i].Hje + '></input>' + ThausandSeperator(data[i].Hje, 2) + '</td>' +
                            '<td class="number" class="number"><input type="hidden" id="Details_Ck4cItemData[' + i + ']_Tarif" name="Details.Ck4cItemData[' + i + '].Tarif" value=' + data[i].Tarif + '></input>' + ThausandSeperator(data[i].Tarif, 2) + '</td>' +
                            '<td class="number"><input type="hidden" id="Details_Ck4cItemData[' + i + ']_ContentPerPack" name="Details.Ck4cItemData[' + i + '].ContentPerPack" value=' + data[i].ContentPerPack + '></input>' + ThausandSeperator(data[i].ContentPerPack, 2) + '</td>' +
                            '<td class="number"><input type="hidden" id="Details_Ck4cItemData[' + i + ']_PackedQty" name="Details.Ck4cItemData[' + i + '].PackedQty" value=' + data[i].QtyPacked + '></input>' + ThausandSeperator(data[i].QtyPacked, 2) + '</td>' +
                            '<td class="number"><input type="hidden" id="Details_Ck4cItemData[' + i + ']_Zb" name="Details.Ck4cItemData[' + i + '].Zb" value=' + data[i].Zb + '></input>' + ThausandSeperator(data[i].Zb, 2) + '</td>' +
                            '<td class="number"><input type="hidden" id="Details_Ck4cItemData[' + i + ']_PackedAdjusted" name="Details.Ck4cItemData[' + i + '].PackedAdjusted" value=' + data[i].PackedAdjusted + '></input>' + ThausandSeperator(data[i].PackedAdjusted, 2) + '</td>' +
                            '<td class="number"><input type="hidden" id="Details_Ck4cItemData[' + i + ']_PackedInPack" name="Details.Ck4cItemData[' + i + '].PackedInPack" value=' + data[i].PackedInPack + '></input>' + ThausandSeperator(data[i].PackedInPack, 2) + '</td>' +
                            '<td class="number">' + ThausandSeperator(data[i].QtyUnpacked, 2) + '</td>' +
                            '<td class="number"><input type="hidden" id="Details_Ck4cItemData[' + i + ']_ProdQty" name="Details.Ck4cItemData[' + i + '].ProdQty" value=' + data[i].QtyProduced + '></input>' + ThausandSeperator(data[i].QtyProduced, 2) + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_ProdQtyUom" name="Details.Ck4cItemData[' + i + '].ProdQtyUom" value=' + data[i].Uom + '></input><input type="hidden" id="Details_Ck4cItemData[' + i + ']_ProdCode" name="Details.Ck4cItemData[' + i + '].ProdCode" value=' + data[i].ProdCode + '></input>' + data[i].Uom + '</td>' +
                            '<td></td>' +
                            '<td style="display: none"><input type="hidden" id="Details_Ck4cItemData[' + i + ']_Remarks" name="Details.Ck4cItemData[' + i + '].Remarks" value="" class=' + classRemarks + '></input></td>' +
                            '<td style="display: none">' + data[i].IsEditable + '</td>' +
                            '<td style="display: none">' + ThausandSeperator(data[i].QtyUnpacked, 2) + '</td>' +
                            '</tr>';
                        $('#tb-body-ck4c').append(tableProdItem);
                    }
                } else {
                    $('#tb-body-ck4c').html("");
                    $("#WipLabel").css("visibility", "hidden");
                    $('#tb-body-ck4c').append('<tr><td style="text-align:center" colspan="18">no data<td></tr>');
                }
            }
        });
    }
}

function EditRow(o) {
    var nRow = o.parents('tr');

    //set value
    $('#uploadItemRow').val(nRow.find("td").eq(1).html());

    $('#uploadRemarks').val(nRow.find("td").eq(18).text());
    
    $('#uploadWip').val(nRow.find("td").eq(21).text());
    $('#uploadValidate').val('1');//need validate
    $("#rowUnpack").css("visibility", "visible");
    
    $('#Ck4cUploadModal').modal('show');
    
    $('#uploadWip').removeClass('input-validation-error');
    $('#uploadWip').addClass('valid');
    
    if (nRow.find("td").eq(20).text() == 'False'
        || nRow.find("td").eq(20).text() == 'false')
    {
        $("#rowUnpack").css("visibility", "hidden");
        $('#uploadValidate').val('0');
    }
    
    
}

function UpdateRow() {
    var row = $('#uploadItemRow').val();
    var validated = true;
    
   

    if ($('#uploadValidate').val() == '1') {
        if ($('#uploadWip').val() == '') {
           
            $('#uploadWip').removeClass('valid');
            $('#uploadWip').addClass('input-validation-error');
            validated = false;
        }
    }

    if (validated) {
        $('#tb-body-ck4c tr').each(function() {
            if ($(this).find('td').eq(1).text() == row) {
                var classRemarks = "Remarks" + row;
                var classUnpack = "Unpack" + row;

                $(this).find('td').eq(18).text($('#uploadRemarks').val());
                $("." + classRemarks).val($('#uploadRemarks').val());

                $(this).find('td').eq(12).text($('#uploadWip').val());
                $(this).find('td').eq(21).text($('#uploadWip').val());

                var unpackValue = $('#uploadWip').val().replace(/\,/g, '');
                $("." + classUnpack).val(unpackValue);

            }
        });


        $('#Ck4cUploadModal').modal('hide');
    }
}

function ajaxLoadPoa(formData, url) {
    $.ajax({
        type: 'POST',
        url: url,
        data: formData,
        success: function (data) {
            var list = data.PoaList;
            if (list.length > 0) {
                var poalist = '';
                for (var i = 0; i < list.length; i++) {
                    poalist = poalist + ', ' + list[i].Text;
                }
                poalist = poalist.slice(2);
                $('#Details_PoaList').val(poalist);
                $('#displayPoaList').val(poalist);
            } else {
                $('#Details_PoaList').val('');
                $('#displayPoaList').val('');
            }
        }
    });
}

function nppbkcIdOnChange(url) {
    $('#Details_PoaList').val('');
    $('#displayPoaList').val('');

    if ($("#Details_NppbkcId").length) {
        var nppbkcid = $('#Details_NppbkcId').find("option:selected").val();
        if (nppbkcid != '') {
            ajaxSelectNppbck({ nppbkcid: nppbkcid }, url);
        }
    }
}

function plantIdOnChange(url) {
    $('#Details_PoaList').val('');
    $('#displayPoaList').val('');

    if ($("#Details_PlantId").length) {
        var plant = $('#Details_PlantId').find("option:selected").val();
        if (plant != '') {
            ajaxSelectNppbck({ plantId: plant }, url);
        }
    }
}

function ajaxSelectNppbck(formData, url) {
    //debugger;
    if (formData) {
        //Load POA
        ajaxLoadPoa(formData, url);
    }
}

function CheckSameData() {
    var comp = $('#Details_CompanyId').find("option:selected").val();
    var plant = $('#Details_PlantId').find("option:selected").val();
    var nppbkc = $('#Details_NppbkcId').find("option:selected").val();
    var period = $('#Details_ReportedPeriod').find("option:selected").val();
    var month = $('#Details_ReportedMonth').find("option:selected").val();
    var year = $('#Details_ReportedYears').find("option:selected").val();

    var compX = $('#CompanyId').val();
    var plantX = $('#PlantId').val();
    var nppbkcX = $('#NppbkcId').val();
    var periodX = $('#PeriodId').val();
    var monthX = $('#MonthId').val();
    var yearX = $('#YearId').val();

    if (comp != compX || plant != plantX || nppbkc != nppbkcX || period != periodX || month != monthX || year != yearX) {
        $('#tb-body-ck4c').html("");
        $('#tb-body-ck4c').append('<tr><td style="text-align:center" colspan="18">no data<td></tr>');
    }
}