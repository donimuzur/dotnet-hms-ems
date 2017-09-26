$(document).ready(function () {
    if ($('#btnCancelCompleted').length) {
        $('#MenuLack10CompletedDocument').addClass('active');
    }
    else {
        $('#MenuLack10OpenDocument').addClass('active');
    }
});

function ValidateInput() {
    var result = true;
    AddValidationClass(true, 'Details_PlantId');
    AddValidationClass(true, 'Details_NppbkcId');
    AddValidationClass(true, 'Details_SubmissionDate');

    if ($('#Details_CompanyId').val() == '') {
        AddValidationClass(false, 'Details_CompanyId');
        result = false;
    }

    if ($('#Details_ReportType').val() == '') {
        AddValidationClass(false, 'Details_ReportType');
        result = false;
    }

    if ($('#Details_SubmissionDate').val() == '') {
        AddValidationClass(false, 'Details_SubmissionDate');
        result = false;
    }

    if ($('#Details_PeriodMonth').val() == '') {
        AddValidationClass(false, 'Details_PeriodMonth');
        result = false;
    }

    if ($('#Details_PeriodYears').val() == '') {
        AddValidationClass(false, 'Details_PeriodYears');
        result = false;
    }

    if ($('#Details_PlantId').is(":disabled")) {
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

    if ($('#Details_Reason').val() == '') {
        AddValidationClass(false, 'Details_Reason');
        result = false;
    }

    if ($('#Details_Remark').val() == '') {
        AddValidationClass(false, 'Details_Remark');
        result = false;
    }

    return result;
}

function ValidateGovInput() {
    var result = true;
    var govStatus = $('#Details_StatusGoverment').find("option:selected").val();
    AddValidationClass(true, 'Details_SubmissionDate');

    if ($('#Details_SubmissionDate').val() == '') {
        AddValidationClass(false, 'Details_SubmissionDate');
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
        $('#ModalLack10Doc').modal('show');
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
        $('#ModalLack10Doc').modal('show');
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

function getWasteData(urlFunction) {
    $('.loading').show();

    var comp = $('#Details_CompanyId').find("option:selected").val();
    var plant = $('#Details_PlantId').find("option:selected").val();
    var nppbkc = $('#Details_NppbkcId').find("option:selected").val();
    var month = $('#Details_PeriodMonth').find("option:selected").val();
    var year = $('#Details_PeriodYears').find("option:selected").val();
    var isNppbkc = $('#Details_PlantId').is(":disabled");
    var isPlant = $('#Details_NppbkcId').is(":disabled");

    $('#CompanyId').val(comp);
    $('#PlantId').val(plant);
    $('#NppbkcId').val(nppbkc);
    $('#MonthId').val(month);
    $('#YearId').val(year);

    if (comp == "" || (plant == "" && isPlant) || (nppbkc == "" && isNppbkc) || month == "" || year == "") {
        $('.loading').hide();
        $('#tb-body-lack10').html("");
        $('#tb-body-lack10').append('<tr><td style="text-align:center" colspan="7">no data<td></tr>');
    }
    else {
        $.ajax({
            type: 'POST',
            url: urlFunction,
            data: { comp: comp, plant: plant, nppbkc: nppbkc, month: month, year: year, isNppbkc: isNppbkc },
            success: function (data) {
                $('.loading').hide();
                if (data.length > 0) {
                    $('#tb-body-lack10').html("");
                    for (var i = 0; i < data.length; i++) {
                        var rowCount = (i + 1);
                        
                        var tableProdItem = '<tr>' +
                            '<td>' + rowCount + '</td>' +
                            '<td><input type="hidden" id="Details_Lack10ItemData[' + i + ']_FaCode" name="Details.Lack10ItemData[' + i + '].FaCode" value=' + data[i].FaCode + '></input>' + data[i].FaCode + '</td>' +
                            '<td><input type="hidden" id="Details_Lack10ItemData[' + i + ']_BrandDescription" name="Details.Lack10ItemData[' + i + '].BrandDescription" value="' + data[i].BrandDescription + '"></input>' + data[i].BrandDescription + '</td>' +
                            '<td><input type="hidden" id="Details_Lack10ItemData[' + i + ']_Werks" name="Details.Lack10ItemData[' + i + '].Werks" value=' + data[i].Werks + '></input><input type="hidden" id="Details_Lack10ItemData[' + i + ']_PlantName" name="Details.Lack10ItemData[' + i + '].PlantName" value="' + data[i].PlantName + '"></input>' + data[i].Werks + "-" + data[i].PlantName + '</td>' +
                            '<td><input type="hidden" id="Details_Lack10ItemData[' + i + ']_Type" name="Details.Lack10ItemData[' + i + '].Type" value="' + data[i].Type + '"></input>' + data[i].Type + '</td>' +
                            '<td><input type="hidden" id="Details_Lack10ItemData[' + i + ']_WasteValue" name="Details.Lack10ItemData[' + i + '].WasteValue" value=' + data[i].WasteValue + '></input>' + ThausandSeperator(data[i].WasteValue, 3) + '</td>' +
                            '<td><input type="hidden" id="Details_Lack10ItemData[' + i + ']_Uom" name="Details.Lack10ItemData[' + i + '].Uom" value=' + data[i].Uom + '></input>' + data[i].Uom + '</td>' +
                            '</tr>';

                        $('#tb-body-lack10').append(tableProdItem);
                    }
                } else {
                    $('#tb-body-lack10').html("");
                    $('#tb-body-lack10').append('<tr><td style="text-align:center" colspan="7">no data<td></tr>');
                }
            }
        });
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
    var month = $('#Details_PeriodMonth').find("option:selected").val();
    var year = $('#Details_PeriodYears').find("option:selected").val();

    var compX = $('#CompanyId').val();
    var plantX = $('#PlantId').val();
    var nppbkcX = $('#NppbkcId').val();
    var monthX = $('#MonthId').val();
    var yearX = $('#YearId').val();

    if (comp != compX || plant != plantX || nppbkc != nppbkcX || month != monthX || year != yearX) {
        $('#tb-body-lack10').html("");
        $('#tb-body-lack10').append('<tr><td style="text-align:center" colspan="7">no data<td></tr>');
    }
}