$('#MenuCk4cDocumentList').addClass('active');

function ValidateGovInput() {
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

function ajaxSelectPlant(url, formData) {
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
                }

            }
        });
    }
}

function ajaxGetNppbkcByCompany(urlFunction, company) {
    $.ajax({
        type: 'POST',
        url: urlFunction,
        data: { companyId: company },
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#Details_NppbkcId').append('<option value=' + data[i].NPPBKC_ID + '>' + data[i].NPPBKC_ID + '</option>');
            }
        }
    });
}

function getProductionData(urlFunction) {
    var comp = $('#Details_CompanyId').find("option:selected").val();
    var plant = $('#Details_PlantId').find("option:selected").val();
    var nppbkc = $('#Details_NppbkcId').find("option:selected").val();
    var period = $('#Details_ReportedPeriod').find("option:selected").val();
    var month = $('#Details_ReportedMonth').find("option:selected").val();
    var year = $('#Details_ReportedYears').find("option:selected").val();
    var isPlant = $('#Details_PlantId').is(":disabled");
    var isNppbkc = $('#Details_NppbkcId').is(":disabled");
    if (comp == "" || (plant == "" && isNppbkc) || (nppbkc == "" && isPlant) || period == "" || month == "" || year == "") {
        $('#tb-body-ck4c').html("");
        $('#tb-body-ck4c').append('<tr><td style="text-align:center" colspan="9">no data<td></tr>');
    }
    else {
        $.ajax({
            type: 'POST',
            url: urlFunction,
            data: { comp: comp, plant: plant, nppbkc: nppbkc, period: period, month: month, year: year },
            success: function (data) {
                if (data.length > 0) {
                    $('#tb-body-ck4c').html("");
                    for (var i = 0; i < data.length; i++) {
                        var dt = new Date(parseInt(data[i].ProductionDate.substr(6)));
                        var dtStr = dt.toString();
                        var date = dtStr.substr(8, 2) + " " + (dtStr.substr(4, 3)) + " " + dtStr.substr(11, 4);
                        var str = dt.getDate() + '/' + (dt.getMonth() + 1) + '/' + dt.getFullYear();

                        var tableProdItem = '<tr><td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_ProdDate" name="Details.Ck4cItemData[' + i + '].ProdDate" value=' + str + '></input>' + date + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_FaCode" name="Details.Ck4cItemData[' + i + '].FaCode" value=' + data[i].FaCode + '></input>' + data[i].FaCode + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_PackedQty" name="Details.Ck4cItemData[' + i + '].PackedQty" value=' + data[i].QtyPacked + '></input>' + data[i].BrandDescription + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_Werks" name="Details.Ck4cItemData[' + i + '].Werks" value=' + data[i].PlantWerks + '></input>' + data[i].PlantName + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_UnpackedQty" name="Details.Ck4cItemData[' + i + '].UnpackedQty" value=' + data[i].QtyUnpacked + '></input>' + data[i].TobaccoProductType + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_HjeIdr" name="Details.Ck4cItemData[' + i + '].HjeIdr" value=' + data[i].Hje + '></input>' + data[i].Hje + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_Tarif" name="Details.Ck4cItemData[' + i + '].Tarif" value=' + data[i].Tarif + '></input>' + data[i].Tarif + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_ProdQty" name="Details.Ck4cItemData[' + i + '].ProdQty" value=' + data[i].QtyProduced + '></input>' + data[i].QtyProduced + '</td>' +
                            '<td><input type="hidden" id="Details_Ck4cItemData[' + i + ']_ProdQtyUom" name="Details.Ck4cItemData[' + i + '].ProdQtyUom" value=' + data[i].UomProd + '></input><input type="hidden" id="Details_Ck4cItemData[' + i + ']_ProdCode" name="Details.Ck4cItemData[' + i + '].ProdCode" value=' + data[i].ProdCode + '></input>' + data[i].Uom + '</td></tr>';
                        $('#tb-body-ck4c').append(tableProdItem);
                    }
                } else {
                    $('#tb-body-ck4c').html("");
                    $('#tb-body-ck4c').append('<tr><td style="text-align:center" colspan="9">no data<td></tr>');
                }
            }
        });
    }
}