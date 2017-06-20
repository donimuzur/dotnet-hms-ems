// Init Components
var kppbc = $("#KppbcText");
var nppbkcCity = $("#NppbkcCityText");
var nppbkcRegion = $("#NppbkcRegionText");
var nppbkcAddress = $("#NppbkcAddressText");
var company = $("#CompanyText");
var companyId = $("#NPPBKC_Company_Id");
var nppbkc = $("#NppbkcSelector");
var submitDate = $("#SubmissionDate");
var requestType = $("#ViewModel_RequestTypeID");
var guarantee = $("#GuaranteeSelector option:selected");
var errorDiv = $("#ExciseDataError");
var fErrorDiv = $("#FinancialRatioError");
var ckErrorDiv = $("#CK1Error");
var sErrorDiv = $("#SupportingError");
var fYear1 = $("#fYear1");
var fYear2 = $("#fYear2");
var fLiquidity1 = $("#fLiquidity1");
var fLiquidity2 = $("#fLiquidity2");
var fSolvency1 = $("#fSolvency1");
var fSolvency2 = $("#fSolvency2");
var fRentability1 = $("#fRentability1");
var fRentability2 = $("#fRentability2");
var docsContainer = $("#SupportingDocs");
var modalContainer = $("#calcDetailContainer");
var saveButton = $("#SaveButton");
var calculateButton = $("#calcDetailButton");
var docNumberContainer = $("#docNumber");
var createForm = $("#ExciseCreateForm");
var fileUploadLimitElm = $("#FileUploadLimit");
var otherDocs = [];
var otherDocsName = [];
var supportingDocs = {};
var exciseFormData = {};
var exciseAdjustmentFormData = [];
var parseObject = {};
var adjItem = {};
var adjfacode = [];

function populateFormData() {
    console.log(submitDate.val());
    console.log(new Date(submitDate.val()));
    var nominal = 0;
    console.log($("#Adjustment").val());
    if ($("#Adjustment").val() === undefined || $("#Adjustment").val() === null) {
        nominal = parseFloat($('#ViewModel_AmountDisplay').val());
    } else {
        nominal = parseInt($("#Adjustment").val().split(',').join('.'));
    }
    for (var i = 0; i <= Object.keys(adjItem).length - 1; i++) {
        for (var j = 0; j <= adjItem[Object.keys(adjItem)[i]].length - 1; j++) {
            console.log("sucess");
            exciseAdjustmentFormData.push({
                BRAND_CE: adjItem[Object.keys(adjItem)[i]][j].BRAND,
                OLD_TARIFF: adjItem[Object.keys(adjItem)[i]][j].OLDTARIFF,
                NEW_TARIFF: adjItem[Object.keys(adjItem)[i]][j].NEWTARIFF,
                INCREASE_TARIFF: adjItem[Object.keys(adjItem)[i]][j].INCREASE,
                CK1_AMOUNT: adjItem[Object.keys(adjItem)[i]][j].CK12MONTH,
                WEIGHTED_INCREASE: adjItem[Object.keys(adjItem)[i]][j].WEIGHTEDINCREASE,
                PRODUCT_CODE: adjItem[Object.keys(adjItem)[i]][j].PRODUCTCODE
            });
        }
    }
    exciseFormData = {
        NppbkcId: nppbkc.val(),
        SubmissionDate: submitDate.val(),
        RequestTypeId: parseInt(requestType.val()),
        Guarantee: $("#GuaranteeSelector :selected").text(),
        FinancialRatioIds: $("#ViewModel_FinancialRatioIds").val(),
        Amount: Number($("#ViewModel_Amount").val().split(',').join('.')),
        POA: $("#POA_Id").val(),
        CalculatedAdjustment: nominal,
    };
    parseObject.Master = exciseFormData;
    parseObject.Detail = exciseAdjustmentFormData;

    console.log(console.log(exciseFormData));
}

function attach() {
    $('#MenuExciseOpenListDocument').addClass('active');
    nppbkc.on("change", nppbkcSelectionChanges);
    submitDate.on("change", submitDateSelectionChanges);
    requestType.on("change", exciseTypeSelectionChanges);
    saveButton.on("click", create);
    $("#addOtherDocBtn").on("click", addOtherDoc);
    $("#saveCalcResult").off("click", placeResult);
    $("#saveCalcResult").on("click", placeResult);
    $('#ExciseCreditAlpha').terbilang({
        lang: 'id',
        output: $('#ExciseCreditAlphaText')
    });
    handleBrowseEvent("browseOtherDoc", "browseOtherDocFile", "browseOtherDocText");
    init();
    nppbkcSelectionChanges();
}

function detach() {
    nppbkc.off("change", nppbkcSelectionChanges);
    submitDate.off("change", submitDateSelectionChanges);
    requestType.off("change", exciseTypeSelectionChanges);
    //$("#saveCalcResult").off("click", placeResult);
    $("#addOtherDocBtn").off("click", addOtherDoc);
    saveButton.off("click", create);
}

function init() {
    $.validator.setDefaults({
        onfocusout: function (element) {
            $(element).valid();
        }
    });
    
    //disableMonthNavigation();
    errorDiv.hide();
    exciseFormData.SubmissionDate = new Date(submitDate.val());
    exciseFormData.RequestTypeId = parseInt(requestType.val());
    saveButton.prop("disabled", true);


}

function disableMonthNavigation() {
    var dateStr = $('#ViewModel_SubmissionDate').val();
    var parts = dateStr.split(" ");
    parts = parts[0].split("/");
    console.log(parts);
    var now = new Date(parts[2] + "-" + parts[0] + "-" + parts[1]);

    console.log(now);
    $('#SubmissionDate').each(function (index) {
        $(this).datepicker('remove');
    });
    $('#SubmissionDate').datepicker({
        toggleActive: true,
        autoclose: true,
        format: "dd M yyyy",
        todayHighlight: true,
        startDate: new Date(now.getFullYear(), now.getMonth(), 1),
        endDate: new Date(now.getFullYear(), now.getMonth() + 1, 0)
    });
}

function addOtherDoc() {
    var input = document.getElementById('browseOtherDocFile');
    //console.log(input);
    if (!input) {
        showErrorDialog("Couldn't find the fileinput element.");
    }
    else if (!input.files) {
        showErrorDialog("This browser doesn't seem to support the `files` property of file inputs.");
    }
    else if (!input.files[0]) {
        showErrorDialog("Please select a file before clicking 'Add'");
    }
    else {
        var file = input.files[0];
        var sizeMB = fileUploadLimitElm.val();
        console.log("Size Limit: " + sizeMB);
        var allowedFilesize = Number(sizeMB) * 1024 * 1024;
        if (file.size > allowedFilesize) {
            showErrorDialog("File size is larger than allowed size (" + sizeMB + " MB)");
            return;
        }
        addToList(file, $("#browseOtherDocFileName").val());
    }
}

function pad(num, size) {
    var s = "000000000" + num;
    return s.substr(s.length - size);
}

function formatDocNumber(companyAlias, cityAlias, submitDate) {
    var months = ["I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII"];
    var year = submitDate.getYear() + 1900;
    var month = submitDate.getMonth();
    var count = 0;
    $("#customloader").show();
    $.ajax({
        url: countUrl,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            $("#customloader").hide();
            count = parseInt(response) + 1;
            docNumberContainer.html(pad(count, 10) + "/" + companyAlias + "/" + cityAlias + "/" + months[month] + "/" + year);
        },
        error: function () {
            $("#customloader").hide();
        }
    });

}

function updateDocumentNumbering(submitDate) {
    var docNumber = docNumberContainer.html();
    var months = ["I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII"];
    var year = submitDate.getYear() + 1900;
    var month = submitDate.getMonth();
    var segments = docNumber.split("/");
    segments[3] = months[month];
    segments[4] = year;
    docNumberContainer.html(segments.join("/"));
}

function loadNppbkc(url, id, fsUrl) {
    exciseFormData.NppbkcId = id;
    var param = {
        id: id
    };
    $("#customloader").show();
    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(param),
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            $("#customloader").hide();
            response = JSON.parse(response);
            //console.log(response);
            var valid = false;
            if (response.Success) {
                var data = response.Data;

                kppbc.val(data.KppbcId);
                nppbkcCity.val(data.City);
                nppbkcRegion.val(data.Region);
                nppbkcAddress.val(data.Address);
                if (data.Company) {
                    company.val(data.Company.Name);
                    companyId.val(data.Company.Id);
                }
                var year = new Date(submitDate.val()).getYear() + 1900;
                //console.log(new Date(submitDate.val()).getYear());
                loadFinancialStatement(fsUrl, companyId.val(), year);
                loadSupportingDocuments(sdUrl, companyId.val());
                if (data.Company)
                    formatDocNumber(data.Company.Alias, data.CityAlias, new Date(submitDate.val()));
            } else {
                kppbc.val("");
                nppbkcCity.val("");
                nppbkcRegion.val("");
                nppbkcAddress.val("");
                company.val("");
                companyId.val("");
            }
        },
        error: function () {
            $("#customloader").hide();
        }
    });
}

function loadFinancialStatement(url, _company, year) {
    if (!_company) {
        _company = '0';
    }
    var param = {
        company: _company,
        year: year
    };
    $("#customloader").show();
    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(param),
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            $("#customloader").hide();
            response = JSON.parse(response);
            //console.log(response);
            var valid = false;
            if (response.Success) {
                var data = response.Data;
                if (!data || data.length < 2) {
                    errorDiv.html("<span>Financial statement last 2 years data for " + company.val() + " not available!</span>");
                    fErrorDiv.show();
                    saveButton.prop("disabled", true);
                    calculateButton.prop("disabled", true);
                }
                else {
                    $("#ViewModel_FinancialRatioIds").val(data[0].Id + "_" + data[1].Id);
                    fYear1.val(data[0].YearPeriod);
                    fYear2.val(data[1].YearPeriod);
                    fLiquidity1.val(data[0].LiquidityRatio);
                    fLiquidity2.val(data[1].LiquidityRatio);
                    fSolvency1.val(data[0].SolvencyRatio);
                    fSolvency2.val(data[1].SolvencyRatio);
                    fRentability1.val(data[0].RentabilityRatio);
                    fRentability2.val(data[1].RentabilityRatio);
                    //saveButton.prop("disabled", false);
                    calculateButton.prop("disabled", false);
                    fErrorDiv.hide();
                    if ($('.requestType').val() === '1') {
                        console.log("load Calculation Normal", ckUrl);
                    loadCk1Calculation(ckUrl);
                    } else {
                        console.log("load calculation adj", adjUrl);
                        loadCk1Calculation(adjUrl);
                    }
                }
            } else {
                fErrorDiv.html("<span>Financial statement last 2 years data for " + company.val() + " not available!</span>");
                ckErrorDiv.html("<span>CK1 Data not available</span>");
                ckErrorDiv.show();
                fErrorDiv.show();
            }
        },
        error: function () {
            $("#customloader").hide();
            fErrorDiv.html("<span>Financial statement last 2 years data for " + company.val() + " not available!</span>");
            ckErrorDiv.html("<span>CK1 Data not available</span>");
            ckErrorDiv.show();
            fErrorDiv.show();
        }
    });
}

function loadSupportingDocuments(url, _company) {
    if (!_company) {
        _company = "0";
    }
    var param = {
        company: _company
    };
    $("#customloader").show();
    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(param),
        contentType: 'application/json; charset=utf-8',
    })
    .success(function (partialResult) {
        $("#customloader").hide();
        docsContainer.html(partialResult);
        sErrorDiv.hide();
        //saveButton.prop("disabled", false);
        calculateButton.prop("disabled", false);
    })
    .error(function (error) {
        $("#customloader").hide();
        //alert("Supporting Document Data Not Available!");
        sErrorDiv.html("<span>Supporting Document Data Not Available!</span>");
        sErrorDiv.show();
        saveButton.prop("disabled", true);
        calculateButton.prop("disabled", true);
    });
}

function loadCk1Calculation(url) {
    var sd = new Date(submitDate.val());
    var year = sd.getYear() + 1900;
    var month = sd.getMonth() + 1;
    var date = sd.getDate();
    var dateStr = year + "-" + month + "-" + date;
    var param = {
        nppbkc: nppbkc.val(),
        submit: dateStr,
        liquidity: fLiquidity1.val()
    };

    $("#customloader").show();
    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(param),
        contentType: 'application/json; charset=utf-8',
    }).success(function (partialResult) {
        $("#customloader").hide();
        modalContainer.html(partialResult);
        $("#ckListButton").off("click", openCkListPopUp);
        $("#ckListButton").on("click", openCkListPopUp);
        $(".btnAddAdj").off("click", addToAdjList);
        $(".btnAddAdj").on("click", addToAdjList);
        ckErrorDiv.hide();
        //saveButton.prop("disabled", false);
        calculateButton.prop("disabled", false);
        $("#saveCalcResult").off("click", placeResult);
        $("#saveCalcResult").on("click", placeResult);

    }).error(function (error) {
        console.log("error calculation");
        $("#customloader").hide();
        ckErrorDiv.html("<span>CK1 Data not available</span>");
        ckErrorDiv.show();
        saveButton.prop("disabled", true);
        calculateButton.prop("disabled", true);
    });
}

function uploads(id) {
    //console.log("Uploading");
    var files = otherDocs;
    if(!files) {
        files = [];
    }
    $("#customloader").show();
    if (window.FormData !== undefined) {
            var data = new FormData();
            var others = [];
            for (var x = 0; x < files.length; x++) {
                
                data.append("other_" + otherDocsName[x], files[x], files[x].name);
            }
            for (var key in supportingDocs) {
                if (supportingDocs.hasOwnProperty(key)) {
                    data.append(key, supportingDocs[key], supportingDocs[key].name);
                }
            }
            data.append("doc_number", docNumberContainer.html());
            data.append("excise_id", id);
        console.log(data);
            $.ajax({
                type: "POST",
                url: uploadUrl,
                contentType: false,
                processData: false,
                data: data,
                success: function (result) {
                    $("#customloader").hide();
                    document.location.href = homeUrl;
                    //create();
                },
                error: function (xhr, status, p3, p4) {
                    $("#customloader").hide();
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                    showErrorDialog(err);
                }
            });
        } else {
            showErrorDialog("This browser doesn't support HTML5 file uploads!");
        }
    if (files && files.length > 0) {
        
    } else {
        //create();
    }
}

function removeFromList(index) {
    if (otherDocs.length > index) {
        otherDocs.splice(index, 1);
        otherDocsName.splice(index, 1);
        renderFileList();
    }


}

function addToList(file, name) {
    if (!name) {
        showErrorDialog("Document name is required!");
        return;
    }
    var idx = otherDocs.indexOf(file);
    var updated = true;
    for (var i = 0; i < otherDocs.length; i++) {
        if (file.name == otherDocs[i].name) {
            updated = false;
            break;
        }
    }
    if (idx < 0 && updated) {
        otherDocs.push(file);
        otherDocsName.push(name);
        renderFileList();
    }
}

function renderFileList() {
    var container = $("#otherDocsBody");
    var content = '';
    if (!otherDocs || otherDocs.length <= 0) {
        content += '<tr id="noOtherFileMsg">' +
                    '<td colspan="4"><div class="alert alert-info">No Addional Documents</div>' + '</td></tr>';
    }
    for (var i = 0; i < otherDocs.length; i++) {
        content +=
            '<tr>' +
            '<td>' + (i + 1) + '</td>' +
            '<td>' + otherDocs[i].name + '</td>' +
            '<td>' + otherDocsName[i] + '</td>' +
            '<td><button class="btn btn-danger" onclick="removeFromList(' + i + ')">Remove</button></td>' +
            '</tr>';
    }
    container.html(content);
}

function handleBrowseEvent(id, fileId, textId) {
    //console.log("registered: " + id);
    $("#" + id).on('click', function () {
        var file = $("#" + fileId);
        file.trigger('click');
    });
    $("#" + fileId).on('change', function () {
        $("#" + textId).val($(this).val().replace(/C:\\fakepath\\/i, ''));
    });
}
var NWin = null;

function openCkListPopUp() {
    console.log("open ck list popup");
    var nppbkcId = nppbkc.val();
    var sd = new Date(submitDate.val());
    var year = sd.getYear() + 1900;
    var month = sd.getMonth() + 1;
    var date = sd.getDate();
    var dateStr = year + "-" + month + "-" + date;

    if (!NWin || NWin.closed) {
        if ($('.requestType').val() === '1') {
            console.log("open detail");
        NWin = window.open(ck1DetaiUrl + '?submit=' + dateStr + '&nppbkc=' + nppbkcId, '', 'titlebar=0,width=800,height=800,scrollbars=yes');
        } else {
            console.log("open detail adjustment");
            NWin = window.open(ck1DetailAdjustmentUrl + '?submit=' + dateStr + '&nppbkc=' + nppbkcId, '', 'titlebar=0,width=800,height=800,scrollbars=yes');
        }
        NWin.focus();
        $(NWin.document).ready(function () {
            NWin.window.title = "Test 1";
        });
    } else {
        NWin.window.title = "Test 1";
        NWin.focus();
    }
    return false;
}

function addToAdjList() {
    console.log(adjfacode[0]);
    var updated = true;
    var sd = new Date(submitDate.val());
    var year = sd.getYear() + 1900;
    var month = sd.getMonth() + 1;
    var date = sd.getDate();
    var dateStr = year + "-" + month + "-" + date;
    var param = {
        nppbkc: nppbkc.val(),
        submit: dateStr,
        facode: $(this).closest('.row').find('.itemid').val()
    };
    var facode = $(this).closest('.row').find('.itemid').val();
    var table = $(this).closest('.row').find('.tbladjBody');
    var tableId = $(this).closest('.row').find('.tblId').val();
    var pctweightincrease = $(this).closest('.row').find('.pctweightincrease');
    var subtotal = $(this).closest('.row').find('.subtotal');
    var subtotal2 = $(this).closest('.row').find('.subtotal2');
    var SKEPCreditTariff = $(this).closest('.row').find('.SKEPCreditTariff');
    var wTariff = $(this).closest('.row').find('.wTariff');
    for (var i = 0; i < adjfacode.length; i++) {
        if ($(this).closest('.row').find('.itemid').val() === adjfacode[0]) {
            updated = false;
            break;
        }
    }
    $.ajax({
        url: adjItemUrl,
        type: 'POST',
        data: JSON.stringify(param),
        contentType: 'application/json; charset=utf-8',
    })
        .success(function (item) {
            
            var idx = adjfacode.indexOf(facode);
            var subtotalck12month = 0;
            var subtotalweightincreased = 0;
            var latestSkep = item.LATESTSKEP;
            if (idx < 0 && updated) {
                var addItem = {
                    BRAND: item.BRAND,
                    OLDTARIFF: item.OLDTARIFF,
                    NEWTARIFF: item.NEWTARIFF,
                    INCREASE: item.INCREASE,
                    CK12MONTH: item.CK12MONTH,
                    WEIGHTEDINCREASE: item.WEIGHTEDINCREASE,
                    PRODUCTCODE: item.PRODUCTCODE,
                };
                adjfacode.push(facode);
                console.log("show hasil", adjItem, adjItem[tableId]);
                if (adjItem.hasOwnProperty(tableId)) {
                    adjItem[tableId].push(addItem);
                } else {
                    adjItem[tableId] = [addItem];
                }
                console.log("show hasil", adjItem);
                renderAdjList(table, tableId);
                for (var i = 0; i < adjItem[tableId].length; i++) {
                    subtotalck12month += adjItem[tableId][i].CK12MONTH;
                    subtotalweightincreased += adjItem[tableId][i].WEIGHTEDINCREASE;
                }
                console.log("result", (subtotalweightincreased / subtotalck12month * 100));
                pctweightincrease.val((subtotalweightincreased / subtotalck12month * 100).toFixed(2).replace(".", ","));
                SKEPCreditTariff.val(latestSkep.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));
                wTariff.val(((subtotalweightincreased / subtotalck12month) * subtotalweightincreased).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));
                
                subtotal.val((subtotalck12month).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));
                if (subtotalweightincreased === 0) {
                    //subtotal.val(0);
                    subtotal2.val(0);
                } else {
                    
                    subtotal2.val(subtotalweightincreased.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));
                }
                var summaryId = '.' + tableId + 'summary';
                console.log("summaryid", summaryId);
                var summaryfield = $(summaryId);
                summaryfield.find(".summaryweightedtariff").val(wTariff.val());
                summaryfield.find(".summaryskep").val(SKEPCreditTariff.val());
                summaryfield.find(".summaryTotal").val((subtotalck12month + subtotalweightincreased));
                summaryfield.find(".summaryTotalTemp").val((subtotalck12month + subtotalweightincreased).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));
                var sum = 0;
                $('.summaryTotal').each(function () {
                    sum += parseFloat(this.value);
                });
                $('#grandtotalValue').val(sum);
                $('.grandtotalValue').val(sum.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));
            }
        })
        .error(function (error) {
            console.log("error", error);
        });
}
function renderAdjList(table, tableId) {
    var container = table;
    var content = '';
    console.log("in renderadjlist", adjfacode);
    if (!adjfacode || adjfacode.length <= 0) {
        content += '<tr id="noOtherFileMsg">' +
            '<td colspan="8"><div class="alert alert-info">No Addional Documents</div>' + '</td></tr>';
    }
    console.log(adjItem[tableId].length, adjItem[tableId][0].BRAND);
    for (var i = 0; i < adjItem[tableId].length; i++) {
        content +=
            '<tr>' +
            '<td>' + (i + 1) + '</td>' +
            '<td>' + adjItem[tableId][i].BRAND + '</td>' +
            '<td style="text-align:right">' + adjItem[tableId][i].OLDTARIFF.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,') + '</td>' +
            '<td style="text-align:right">' + adjItem[tableId][i].NEWTARIFF.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,') + '</td>' +
            '<td style="text-align:right">' + adjItem[tableId][i].INCREASE.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,') + '</td>' +
            '<td style="text-align:right">' + adjItem[tableId][i].CK12MONTH.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,') + '</td>' +
            '<td style="text-align:right">' + adjItem[tableId][i].WEIGHTEDINCREASE.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,') + '</td>' +
            '<td><button class="btn btn-danger" onclick="removeFromList(' + i + ')">Remove</button></td>' +
            '</tr>';
    }
    container.html(content);
    console.log(container);
}
function placeResult() {
    //alert("Place result!");
    console.log($("#grandTotal").val());
    $("#ViewModel_AmountDisplay").val($("#grandTotal").val());
    $("#ViewModel_Amount").val($("#grandTotalValue").val());
    var amount = Number($("#ViewModel_Amount").val().split(',').join('.'));
    if (!amount || amount == 0) {
        var currency = $("#grandTotal").val();
        amount = Number(currency.replace(/[^0-9\.]+/g, ""));
    }
    console.log(amount);
    var _terbilang = terbilang(amount.toString()) + " Rupiah"
        .replace(/^([a-z\u00E0-\u00FC])|\s+([a-z\u00E0-\u00FC])/g, function ($1) {
            return $1.toUpperCase();
        });
    //$("#ExciseCreditAlpha").val(amount);
    $("#ExciseCreditAlphaText").html(_terbilang);
    $("#ViewModel_Amount").val(amount);
    console.log("Grand Total: " + $("#ViewModel_Amount").val());
    saveButton.prop("disabled", false);
}

function showErrorDialog(msg) {
    $("#errModalTitle").html("REQUEST FAILED");
    $("#errModalContent").html(msg);
    $("#errModal").modal("show");
}


function create() {
    //$("#supportDocForm").validate()
    //var supportDocValid = $("#supportDocForm").valid();
    //console.log("SupportDoc: " + supportDocValid);
    createForm.validate();
    var exciseFormValid = createForm.valid();
    var supportDocsInputs = $("form#ExciseCreateForm input[type=file]");
    //console.log(supportDocsInputs);
    //console.log("Form: " + exciseFormValid);
    if (!exciseFormValid) {
        showErrorDialog("Supporting documents are required");
        return;
    }
    var sizeMB = fileUploadLimitElm.val();
    //for (var key in supportingDocs) {
    //    if (supportingDocs.hasOwnProperty(key)) {
    //        if (supportingDocs[key].size > Number(sizeMB) * 1024 * 1024) {
    //            showErrorDialog("File size is larger than allowed size (" + sizeMB + " MB)");
    //            return;
    //        }
    //    }
    //}
    if (exciseFormValid) {
        var token = $('input[name="__RequestVerificationToken"]', createForm).val();
        console.log("Token: " + token);
        saveButton.prop("disabled", true);
        populateFormData();
        $("#customloader").show();
        $.ajax({
            type: "POST",
            url: createUrl,
            data: {
                __RequestVerificationToken: token,
                model: JSON.stringify(parseObject)
            },
            success: function (result) {
                console.log(result);
                if (result) {
                    console.log(result);
                    uploads(result);
                    document.location.href = homeUrl;

                } else {
                    showErrorDialog("Save failed!");
                    saveButton.prop("disabled", false);
                    $("#customloader").hide();
                    //document.location.href = '/ExciseCredit';
                }
                //create();
            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;
                showErrorDialog(err);
                $("#customloader").hide();
            }
        });
        //saveButton.prop("disabled", true);
    }


}

function edit() {
    var editForm = $("#ExciseEditForm");
    var token = $('input[name="__RequestVerificationToken"]', editForm).val();
    var id = $("#ViewModel_Id").val();
    var docNumber = $("#ViewModel_DocumentNumber").val();
    //console.log("Uploading");
    var files = otherDocMap;
    console.log(files);
    $("#customloader").show();
    
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (var key in otherDocMap) {
                if (otherDocMap.hasOwnProperty(key)) {
                    data.append("other_" + otherDocMapName[key], otherDocMap[key], otherDocMap[key].name);
                }
            }
            for (var key in supportingDocs) {
                if (supportingDocs.hasOwnProperty(key)) {
                    data.append(key, supportingDocs[key], supportingDocs[key].name);
                }
            }
            data.append("submit_date", submitDate.val());
            data.append("guarantee", $("#GuaranteeSelector :selected").text());
            data.append("doc_number", docNumber);
            data.append("excise_id", id);
            data.append("deleted_docs", JSON.stringify(existingOtherDocs));
            data.append("__RequestVerificationToken", token);
            $.ajax({
                type: "POST",
                url: editUrl,
                contentType: false,
                processData: false,
                data: data,
                success: function (result) {
                    $("#customloader").hide();
                    //console.log(result);
                    document.location.href = homeUrl;
                },
                error: function (xhr, status, p3, p4) {
                    $("#customloader").hide();
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                    showErrorDialog(err);
                }
            });
        } else {
            showErrorDialog("This browser doesn't support HTML5 file uploads!");
        }
    
}

function TinyMceInit(selector) {
    tinyMCE.init({
        mode: "specific_textareas",
        editor_selector: selector,
        theme: "modern",
        plugins: 'code nonbreaking pagebreak',
        menubar: false,
        statusbar: false,
        toolbar: "undo redo | removeformat | pagebreak nonbreaking | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | justifyleft justifycenter justifyright justifyfull | bullist numlist | outdent indent",
        pagebreak_separator: "<br />",
        width: '100%',
        height: "500",
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        theme_advanced_resizing: true,
        setup: function (ed) {
            ed.on('keydown', function (e) {
                e.stopPropagation();
                e.preventDefault();
                window.event.cancelBubble = true
            });
        }
    });
}