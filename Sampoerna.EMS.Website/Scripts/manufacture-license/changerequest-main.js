// Init Components
var kppbc = $("#KppbcText");
var nppbkcCity = $("#NPPBKCCityText");
var nppbkcRegion = $("#NPPBKCRegionText");
var nppbkcAddress = $("#NPPBKCAddressText");
var company = $("#CompanyText");
var companyId = $("#NPPBKC_Company_Id");
var nppbkc = $("#NppbkcSelector");
var companyAlias = $("#CompanyAlias");
var cityAlias = $("#CityAlias");
var submitDate = $("#SubmissionDate");
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
var otherDocs = [];
var supportingDocs = [];
var notesIndex = 0;
var BAdocIndex = 0;
var HdDocIndex = 0;


function attachEvents() {
    nppbkc.on("change", nppbkcSelectionChanges);
    submitDate.on("change", submitDateSelectionChanges);
    saveButton.on("click", uploads);
    $("#addOtherDocBtn").on("click", addOtherDoc);
    $("#saveCalcResult").on("click", placeResult);
    handleBrowseEvent("browseOtherDoc", "browseOtherDocFile", "browseOtherDocText");
    nppbkcSelectionChanges();

    saveButton.on("click", function () {
        var createForm = document.getElementById("createForm");
        var editForm = document.getElementById("editForm");
        console.log(createForm);
        console.log(editForm);
        if (!createForm)
            editForm.submit();
        else
            createForm.submit();
        $(this).prop("disabled", true);
    });
    //$("#ReviseButtonSubmit").on("click", function () {
    //    $(this).prop("disabled", true);
    //})
}


function Cleanup() {
    nppbkc.off("change", nppbkcSelectionChanges);
    submitDate.off("change", submitDateSelectionChanges);
    $("#saveCalcResult").off("click", placeResult);
    $("#addOtherDocBtn").off("click", addOtherDoc);
    saveButton.off("click", uploads);
}

function init() {
    errorDiv.hide();
    sErrorDiv.hide();

    $('#MenuManufactureLicenseChangeRequestOpenDocument').addClass('active');
    $('#MenuManufactureLicenseLicenseRequestOpenDocument').removeClass('active');
    $('#MenuManufactureLicenseLicenseRequestOpenDocument').removeClass('open');
    $('#MenuManufactureLicenseInterviewRequestOpenDocument').removeClass('active');
    $('#MenuManufactureLicenseInterviewRequestOpenDocument').removeClass('open');

    $("#customloader").hide();
    //saveButton.prop("disabled", false);
} 
    


function addOtherDoc() {
    var input = input = document.getElementById('browseOtherDocFile');
    console.log(input);
    if (!input) {
        alert("Um, couldn't find the fileinput element.");
    }
    else if (!input.files) {
        alert("This browser doesn't seem to support the `files` property of file inputs.");
    }
    else if (!input.files[0]) {
        alert("Please select a file before clicking 'Add'");
    }
    else {
        var file = input.files[0];
        addToList(file);
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

function loadNppbkc(url, id, sdUrl) {
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
                companyAlias.val(data.CompanyAlias);
                cityAlias.val(data.CityAlias);
                kppbc.val(data.KppbcId);
                nppbkcCity.val(data.City);
                nppbkcRegion.val(data.Region);
                nppbkcAddress.text(data.Address);
                company.val(data.Company.Name);
                companyId.val(data.Company.Id);
                
                $("#divManufactureLocation").empty();

                $.each(data.Plants, function (i, item) {
                    var item = "";
                    var red_mark = "";
                    if (data.Plants[i].IsMainPlant)
                    {
                        red_mark = "<span class='red'>*</span>";
                    }
                    item += "<div class='row'>";
                    item += "<div class='col-md-6'>";
                    item += "<div class='row'>";
                    item += "<label class='col-md-4'>Manufacture Address" + red_mark + "</label>";
                    item += "<div class='col-md-8'>";
                    item += "<textarea class = 'form-control' rows = '4' readonly = 'True'>" + data.Plants[i].PlantAddress + "</textarea>";
                    item += "</div>";
                    item += "</div>";
                    item += "</div>";
                    item += "<div class='col-md-6'>";
                    item += "<div class='row'>";
                    item += "<label class='col-md-4'>District / City" + red_mark + "</label>";
                    item += "<div class='col-md-8'>";
                    item += "<input type='text' class = 'form-control' readonly = 'True' value='" + data.Plants[i].PlantCity + "'></input>";
                    item += "</div>";
                    item += "</div>";
                    item += "</div>";
                    item += "</div>";

                    $("#divManufactureLocation").append(item);
                });

                var year = new Date(submitDate.val()).getYear() + 1900;
                formatDocNumber(data.Company.Alias, data.CityAlias, new Date(submitDate.val()));
                SupportingDocumentList(sdUrl, companyId.val());
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

$(document).on("click", ".btn_remove_supportdoc", function () {
    var fileuploadid = $(this).data("fileuploadid");
    var parent = $(this).closest(".div_file");
    parent.find(".div_file_href").remove();
    parent.find(".div_supportdoc_name").show();
    parent.find(".span_browsesupportdoc").show();
    parent.find(".supporting_document").prop('required', true);
    AddRemovedFileToList(fileuploadid);
});


function loadSupportingDocuments(url, company) {
    var param = {
        CompanyId: company
    };
    $("#customloader").show();
    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(param),
        contentType: 'application/json; charset=utf-8',
    })
    .success(function(partialResult){
        $("#customloader").hide();
        docsContainer.html(partialResult);
        sErrorDiv.hide();
        saveButton.prop("disabled", false);
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

function uploads() {
    var files = otherDocs;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            var data = new FormData();
            var others = [];
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x], files[x].name);
            }
            data.append("doc_number", docNumberContainer.html());

            $.ajax({
                type: "POST",
                url: uploadUrl,
                contentType: false,
                processData: false,
                data: data,
                success: function (result) {
                    console.log(result);
                    create();
                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                }
            });
        } else {
            alert("This browser doesn't support HTML5 file uploads!");
        }
    }
}

function removeFromList(index) {
    if (otherDocs.length > index)
    {
        otherDocs.splice(index, 1);
        renderFileList();
    }
}

function addToList(file) {
    if (otherDocs.indexOf(file) < 0) {
        otherDocs.push(file);
        renderFileList();
    }
}

function renderFileList() {
    var container = $("#otherDocsBody");
    var content = "";
    for (var i = 0; i < otherDocs.length; i++) {
        content +=
            '<tr>' +
            '<td>' + (i + 1) + '</td>' +
            '<td>' + otherDocs[i].name +
            '<td><button class="btn btn-blue" onclick="removeFromList(' + i + ')">Remove</button></td>' +
            '</tr>';
    }
    container.html(content);
}

function handleBrowseEvent(id, fileId, textId) {
    $("#" + id).on('click', function () {
        var file = $("#" + fileId);
        file.trigger('click');
    });
    $("#" + fileId).on('change', function () {
        $("#" + textId).val($(this).val().replace(/C:\\fakepath\\/i, ''));
    });
}
var NWin = null;

function placeResult() {
}


function create() {
    alert("Create!");
}

$(document).on("click", "#btn_add_notes", function () {
    notesIndex = $("#hd_detail_index").val();
    var items = "";
    items += "<div class='row div_group_notes'>";
    items += "<div class='col-md-12>";
    items += "<div class='row'>";
    items += "<div class='col-md-6'>";
    items += "<input class='txt_manufactdet_id' id='updateNote_DetailId' name='ListOfUpdateNotes[" + notesIndex + "].DetailId' value='0' type='hidden'>"
    items += "<input class='notes_isactive' id='updateNote_DetailId' name='ListOfUpdateNotes[" + notesIndex + "].IsActive' value='1' type='hidden'>"
    items += "<input type='text' class='form-control file_notes' id='file_notes_" + notesIndex + "' name='ListOfUpdateNotes[" + notesIndex + "].UpdateNotes'  />";
    items += "</div>";
    items += "<div class='col-md-6'>";
    items += "<input type='button' class='btn btn-blue btn_del_notes' data-detailid='0'  value='Delete' />";
    items += "</div>";
    items += "</div>";
    items += "</div>";
    items += "</div>";
    $("#div_body_notes").append(items);
    notesIndex++;
    $("#hd_detail_index").val(notesIndex);
});

$(document).on("click", ".btn_del_notes", function () {
    var detailId = $(this).data("detailid");
    if (detailId != "0")
    {
        $("#div_body_notes").append("<input type='hidden' name='RemovedDetailId' value='" + detailId + "' />");
    }
    var div_parent = $(this).parents(".div_group_notes");
    div_parent.find(".notes_isactive").val("0");
    div_parent.hide();
    //$(this).parents(".div_group_notes").remove();

});


//$(document).on("click", "#btn_add_otherdoc", function () {
//    var items = "";
//    items += "<div class='row div_group_doc'>";
//    items += "<div class='col-md-12>";
//    items += "<div class='row'>";
//    items += "<div class='col-md-6'>";
//    items += "<input type='file' class='form-control' />";
//    items += "</div>";
//    items += "<div class='col-md-6'>";
//    items += "<input type='button' class='btn btn-danger btn_del_doc' value='Delete' />";
//    items += "</div>";
//    items += "</div>";
//    items += "</div>";
//    items += "</div>";
//    $("#div_body_otherdoc").append(items);
//});

//$(document).on("click", "#btn_add_ba_doc", function () {
//    var items = "";
//    items += "<div class='row div_group_doc'>";
//    items += "<div class='col-md-8'>";
//    items += "<input type='file' class='form-control' name='File_BA' />";
//    items += "</div>";
//    items += "<input type='button' class='btn btn-blue btn_del_doc' value='Delete' />";
//    items += "</div>";
//    $("#div_body_ba_doc").append(items);
//});

$(document).on("click", "#btn_browsebaDoc_file", function () {
    var div_parent = $(this).closest(".div_file");
    var htmlInput = "<input id='txt_badoc_" + BAdocIndex + "' type='file' name='File_BA' class='hidden txt_file' accept='application/pdf' data-thefieldnameid='txt_otherdoc_filename_ba_" + BAdocIndex + "'  data-index='ba_" + BAdocIndex + "' />";
    htmlInput += "<input type='hidden' name='File_Other_Name' id='txt_otherdoc_filename_ba_" + BAdocIndex + "' />";
    div_parent.append(htmlInput);
    $("#txt_badoc_" + BAdocIndex).click();
    BAdocIndex++;
});

$(document).on("click", "#btn_add_badoc", function () {
    var filename = $("#txt_badocfile_name").val();
    var thefilename = $("#txt_bafile_name").val();
    if (thefilename != "") {
        var txtfilenameindex = $("#hd_toggle_otherdoc").val();
        var txtfilename = $("#txt_otherdoc_filename_" + txtfilenameindex);
        $("#txt_badocfile_name").val("Select file");
        $("#txt_bafile_name").val("");
        if (filename != "" && filename != "Select file") {
            if (CheckIsFileNotExist("#div_body_ba_doc", filename)) {
                AddBAFileList(filename, thefilename);
                txtfilename.val(filename + "^" + thefilename);
            }
        }
    }
    else {
        alert("File name field is required");
    }
});

function AddBAFileList(filename, name) {
    var container = $("#div_body_ba_doc");
    var content = '<tr>' +
            '<td class="td_filename_number"></td>' +
            '<td class="td_filename" data-filename="' + filename + '" data-fileuploadid="0">' + filename + '</td>' +
            '<td>' + name + '</td>' +
            '<td><button type="button" class="btn btn-blue btn_remove_badoc" data-fileuploadid="0">Delete</button></td>' +
            '</tr>';
    container.append(content);
    renderFileNumberList("#div_body_ba_doc");
}


$(document).on("click", ".btn_remove_badoc", function () {
    var thisTr = $(this).closest("tr");
    var fileuploadid = $(this).data("fileuploadid");
    var Removingfilename = thisTr.find(".td_filename").html();
    if (fileuploadid == "0") {
        $(".div_file_badoc").find('.txt_file').each(function () {
            var fileName = $(this).val().replace(/C:\\fakepath\\/i, '');
            fileName = fileName.replace(/^.*[\\\/]/, '')
            if (Removingfilename == fileName) {
                $(this).remove();
                thisTr.remove();
            }
        });
    }
    else {
        thisTr.remove();
    }
    AddRemovedFileToList(fileuploadid);
    renderFileNumberList("#div_body_ba_doc");
});

$(document).on("click", ".btn_del_doc", function () {
    $(this).parents(".div_group_doc").remove();
});

function SupportingDocumentList(sdUrl, CompanyId) {
    var crid = $("#txt_hd_id").val();
    var isReadonly = $("#txt_hd_isformreadonly").val();
    var param = {
        CompanyId: CompanyId,
        CRId: crid,
        IsReadonly: isReadonly
    };
    $.ajax({
        url: sdUrl,
        type: 'POST',
        data: JSON.stringify(param),
        contentType: 'application/json; charset=utf-8',
    })
    .success(function (partialResult) {
        sErrorDiv.hide();
        $("#div_body_supportingdoc").html(partialResult);
    })
    .error(function (error) {
        ////
    });
}

function AddOtherFileList(filename, name) {
    var container = $("#div_otherDocsBody");
    var content = '<tr>' +
            '<td class="td_filename_number"></td>' +
            '<td class="td_filename" data-filename="' + filename + '" data-fileuploadid="0">' + filename + '</td>' +
            '<td>' + name + '</td>' +
            '<td><button type="button" class="btn btn-blue btn_remove_otherdoc" data-fileuploadid="0">Delete</button></td>' +
            '</tr>';
    container.append(content);
    renderFileNumberList("#div_otherDocsBody");
}

function renderFileNumberList(DivId) {
    var i = 1;
    var td_list = $(DivId).find('.td_filename_number').each(function () {
        $(this).html(i);
        i++
    });
}

function CheckIsFileNotExist(DivId, FileName) {
    var isNotExist = true;
    var td_list = $(DivId).find('.td_filename').each(function () {
        if (FileName == $(this).html()) {
            isNotExist = false;
        }
    });
    return isNotExist;
}


$(document).on("click", ".btn_browsesupportdoc_file", function () {
    var index = $(this).data("index");
    $("#txt_supportdoc_file" + index).click();
});

$(document).on("change", ".txt_file", function () {
    var id = $(this).attr("id");
    var filename_index = $(this).data("index");
    $("#hd_toggle_otherdoc").val(filename_index);
    var isSupporting = $(this).hasClass("supporting_document");
    var filesize = GetFileSize(id);
    if (filesize != null && filesize != undefined && filesize > 0) {
        var maxsize = parseInt($("#txt_hd_filesize").val());
        if (filesize > maxsize && !isSupporting) {
            $("#" + id).remove();
            alert("Max file size is " + String(maxsize) + " Mb");
        }
        else {
            var txtFileName = $(this).closest(".div_file").find(".txt_file_name");
            var fileName = $(this).val().replace(/C:\\fakepath\\/i, '');
            fileName = fileName.replace(/^.*[\\\/]/, '');
            txtFileName.val(fileName);
        }
    }
});

function GetFileSize(fileid) {
    try {
        var fileSize = 0;
        fileSize = $("#" + fileid)[0].files[0].size //size in kb
        fileSize = fileSize / 1048576; //size in mb 
        return fileSize;
    }
    catch (e) {
        return 0;
    }
}
$(document).on("click", "#btn_browseOtherDoc_file", function () {
    var div_parent = $(this).closest(".div_file");
    var htmlInput = "<input id='txt_otherdoc_" + HdDocIndex + "' type='file' name='File_Other' class='hidden txt_file' accept='application/pdf' data-thefieldnameid='txt_otherdoc_filename_other_" + HdDocIndex + "'  data-index='other_" + HdDocIndex + "' />";
    htmlInput += "<input type='hidden' name='File_Other_Name' id='txt_otherdoc_filename_other_" + HdDocIndex + "' />";
    div_parent.append(htmlInput);
    $("#txt_otherdoc_" + HdDocIndex).click();
    HdDocIndex++;
});

$(document).on("click", "#btn_add_otherdoc", function () {
    var filename = $("#txt_otherdocfile_name").val();
    var thefilename = $("#txt_otherfile_name").val();
    if (thefilename != "") {
        var txtfilenameindex = $("#hd_toggle_otherdoc").val();
        var txtfilename = $("#txt_otherdoc_filename_" + txtfilenameindex);
        $("#txt_otherdocfile_name").val("Select file");
        $("#txt_otherfile_name").val("");
        if (filename != "" && filename != "Select file") {
            if (CheckIsFileNotExist("#div_otherDocsBody", filename)) {
                $("#div_alert_additional").html("");
                $("#div_alert_additional").hide();
                AddOtherFileList(filename, thefilename);
                txtfilename.val(filename + "^" + thefilename);
            }
            else
            {
                $("#div_alert_additional").html("File already exist");
                $("#div_alert_additional").show();
            }
        }
        else {
            $("#txt_otherdoc_" + txtfilenameindex).remove();
            txtfilename.remove();
        }
    }
    else {
        alert("File name field is required");
    }

    //var filename = $("#txt_otherdocfile_name").val();
    //var thefilename = $("#txt_otherfile_name").val();
    //if (thefilename != "") {
    //    var txtfilenameindex = $("#hd_toggle_otherdoc").val();
    //    var txtfilename = $("#txt_otherdoc_filename_" + txtfilenameindex);
    //    $("#txt_otherdocfile_name").val("Select file");
    //    $("#txt_otherfile_name").val("");
    //    if (filename != "" && filename != "Select file") {
    //        if (CheckIsFileNotExist("#div_otherDocsBody", filename)) {
    //            AddOtherFileList(filename, thefilename);
    //            txtfilename.val(filename + "^" + thefilename);

    //        }
    //    }
    //}
    //else {
    //    alert("File name field is required");
    //}
});

$(document).on("click", ".btn_remove_otherdoc", function () {
    var thisTr = $(this).closest("tr");
    var fileuploadid = $(this).data("fileuploadid");
    var Removingfilename = thisTr.find(".td_filename").html();
    if (fileuploadid == "0") {
        $(".div_file_otherdoc").find('.txt_file').each(function () {
            var fileName = $(this).val().replace(/C:\\fakepath\\/i, '');
            fileName = fileName.replace(/^.*[\\\/]/, '')
            if (Removingfilename == fileName) {
                $(this).remove();
                thisTr.remove();
            }
        });
    }
    else {
        thisTr.remove();
    }
    AddRemovedFileToList(fileuploadid);
    renderFileNumberList("#div_otherDocsBody");
});

$(document).on("change", ".file_otherdoc", function () {
    var id = $(this).attr("id");
    var filesize = GetFileSize(id);
    if (filesize != null && filesize != undefined && filesize > 0) {
        var maxsize = parseInt($("#txt_hd_filesize").val());
        if (filesize > maxsize) {
            $(this).empty();
            $("Max file size is " + String(maxsize) + " Mb");
        }
    }
});

$(document).on("click", "#btnSubmit", function () {
    CloseAllModal();
    $("#customloader").show();
    ChangeStatus("submit", "WAITING FOR POA APPROVAL", "");

});


$(document).on("click", "#btnApprove", function () {
    CloseAllModal();
    $("#customloader").show();
    ChangeStatus("approve", "WAITING FOR GOVERNMENT APPROVAL", "");
});


$(document).on("click", "#btnApproveFinal", function () {
    CloseAllModal();
    $("#customloader").show();
    ChangeStatus("approve", "COMPLETED", "");
});

$(document).on("click", "#btnCancel", function () {
    CloseAllModal();
    $("#customloader").show();
    ChangeStatus("cancel", "CANCELED", "");
});

$(document).on("click", "#btnWithdraw", function () {
    CloseAllModal();
    $("#customloader").show();
    ChangeStatus("withdraw", "DRAFT EDIT", "");
});



function ChangeStatus(Action, Status, Comment) {
    var CRId = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("ChangeStatus"),
        data: {
            CRId: CRId,
            Status: Status,
            Comment: Comment,
            Action: Action
        },
        success: function (data) {
            var errmessage = data;
            if (data == "") {
                window.location.replace(getUrl(""));
            }
        }
    });
}

$(document).on("click", ".btn_showmodal_changestatus", function () {
    var action = $(this).data("action");
    if (action == "Revise") {
        $("#txt_label_forchangestatus").text("Revise Form Confirmation");
        $("#btn_changestatus_reject").hide();
        $("#btn_changestatus_revise").show();
    }
    else if (action == "Reject") {
        $("#txt_label_forchangestatus").text("Reject Form Confirmation");
        $("#btn_changestatus_revise").hide();
        $("#btn_changestatus_reject").show();
    }
    else if (action == "FinalReject") {
        $("#txt_label_forchangestatus").text("Reject");
        $("#btn_finalreject").show();
    }
    else if (action == "Withdraw") {
        $("#txt_label_forchangestatus").text("Withdraw Form Confirmation");
        $("#txt_label_forchangestatus").text("Withdraw");
        $("#btn_changestatus_reviseskep").hide();
        $("#btn_changestatus_reject").hide();
        $("#btn_changestatus_withdraw").show();
    }

});

$(document).on("click", ".btn_changestatus_submit", function () {
    //CloseAllModal();
    //$("#customloader").show();
    var action = $(this).data("action");
    var status = "";

    switch(action)
    {
        case "revise":
            status = "DRAFT EDIT";
            break;

        case "reject":
            status = "REJECTED";
            break;

        case "finalreject":
            status = "WAITING FOR GOVERNMENT APPROVAL";
            break;

        case "withdraw":
            status = "DRAFT EDIT";
            break;
    }

    var comment = $("#txt_changestatus_comment").val();
    if (comment == "" && (action == "revise" || action == "reviseskep" || action == "withdraw")) {
        $("#div_alert_revise").show();
        $("#div_alert_revise").html("* Note/Comment cannot be empty.");
    }
    else
    {
        CloseAllModal();
        $("#customloader").show();
        ChangeStatus(action, status, comment);
    }

    //ChangeStatus(action, status, comment);
});

function CheckFieldEmpty() {
    var emptyCount = 0;
    $("#div_alert").empty();
    var arr = [];
    $(':input[required]:visible').each(function () {
        if ($(this).val() == "" || $(this).val() == null) {
            emptyCount++;
            var message = "* The " + $(this).data("fieldname") + " field is required.";
            if (arr.indexOf(message) < 0) {
                arr.push(message);
                $("#div_alert").append(message + "<br/>");
            }
        }
    });
    var nppbkc_selected = $("#NppbkcSelector").val();
    if (nppbkc_selected == "")
    {
        emptyCount++;
        var message = "* The NPPBKC field is required.";
        if (arr.indexOf(message) < 0) {
            arr.push(message);
            $("#div_alert").append(message + "<br/>");
        }
    }

    var count_item = $(".file_notes").length;
    if (count_item == 0) {
        emptyCount++;
        var message = "* Must have at least 1 Item Updates";
        if (arr.indexOf(message) < 0) {
            arr.push(message);
            $("#div_alert").append(message + "<br/>");
        }
    }
    else
    {
        var emptyNotes = 0;
        $(".file_notes").each(function () {
            if ($(this).is(':visible'))
            {
                if ($(this).val() == "" || $(this).val() == null) {
                    emptyNotes++;
                }
                if (emptyNotes > 0) {
                    emptyCount++;
                    var message = "* The Item Updates field is required";
                    if (arr.indexOf(message) < 0) {
                        arr.push(message);
                        $("#div_alert").append(message + "<br/>");
                    }
                }
            }
            else
            {
                //alert('not visible ' + $(this).val());
            }

        })

    }



    if (emptyCount == 0) {
        $("#div_alert").hide();
    }
    else {
        $("#div_alert").show();
    }
    return emptyCount;
}

$(document).on("click", "#btnSave", function () {
    $("#customloader").show();
    var fieldEmpty = CheckFieldEmpty();
    if (fieldEmpty == 0) {
        CleaningUnUseFileOther("div_otherDocsBody", "div_file_otherdoc");
        $("#ChangeRequestForm").submit();
    }
    else {
        $("#customloader").hide();
    }
});

$(document).on("click", "#btnSubmitSkep", function () {
    CloseAllModal();
    $("#customloader").show();
    var fieldEmpty = CheckFieldBAEmpty();
    if (fieldEmpty == 0) {
        CleaningUnUseFileOther("div_body_ba_doc", "div_file_badoc");
        $("#ChangeRequestForm").submit();
    }
    else {
        $("#customloader").hide();
    }
});


function GeneratePrintOut() {
    $("#customloader").show();
    $("#div-printout").empty();
    var ID = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GeneratePrintout"),
        data: { ChangeID: ID },
        success: function (data) {
            $("#div-printout").html(data);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function GeneratePrintOut_license2() {
    $("#customloader").show();
    $("#div-printout").empty();
    var ID = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GeneratePrintout_License2"),
        data: { ChangeID: ID },
        success: function (data) {
            $("#div-printout").html(data);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function GeneratePrintOut_license3() {
    $("#customloader").show();
    $("#div-printout").empty();
    var ID = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GeneratePrintout_License3"),
        data: { ChangeID: ID },
        success: function (data) {
            $("#div-printout").html(data);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function GeneratePrintOutPDF() {
    $("#form_DownloadPrintout").submit();
}

function GetPrintoutLayout() {
    $("#customloader").show();
    $.ajax({
        type: 'POST',
        url: getUrl("GetPrintOutLayout"),
        data: {
            CreatedBy : $("#txt_hd_createdby").val()
        },
        success: function (html) {
            tinyMCE.activeEditor.setContent(html);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}
function CloseAllModal() {
    $('.ems-modal').modal('hide');
}


function UpdatePrintoutLayout() {
    $("#customloader").show();
    CloseAllModal();
    var layout = tinyMCE.get('txt_printoutlayout').getContent();
    $.ajax({
        type: 'POST',
        url: getUrl("UpdatePrintOutLayout"),
        data: {
            NewPrintout: layout,
            CreatedBy: $("#txt_hd_createdby").val(),
            Id: $("#txt_hd_id").val()
        },
        success: function (result) {
            if (result == "") {
                GeneratePrintOut();
            }
            else {
                alert(result);
            }
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

$(document).on("click", "#btn-tab-printout", function () {
    GeneratePrintOut();
});

$(document).on("click", "#btn_download_printout", function () {
    GeneratePrintOutPDF();
});

$(document).on("click", "#btn_show_printouteditor", function () {
    GetPrintoutLayout();
});

$(document).on("click", "#btn_change_printoutlayout", function () {
    UpdatePrintoutLayout();
});

$(document).on("click", "#btn-tab-changelog", function () {
    var crid = $("#txt_hd_id").val();
    var now = new Date();
    var Token = now.getDate().toString() + now.getDate().toString() + (now.getMonth() + 1).toString()
                + now.getFullYear().toString()
                + now.getHours().toString()
                + now.getMinutes().toString()
                + now.getSeconds().toString();

    $("#customloader").show();

    $.ajax({
        url: getUrl("ChangeLog"),
        dataType: 'html',
        cache: false,
        data: {
            CRID: crid,
            Token: Token
        },
        success: function (data) {
            $('#divChangeLog').html(data);

            // DataTable
            var table = null;
            if ($.fn.dataTable.isDataTable('#changeRequestTable')) {
                table = $('#changeRequestTable').DataTable();
                table.destroy();
            }

            table = $('#changesHistoryTable').DataTable(
                {
                    "sDom": "Rlfrtip",
                    "language": {
                        "zeroRecords": "No records found",
                        "infoFiltered": "(filtered from _MAX_ total records)"
                    }
                });



            // Setup - add a text input to each footer cell
            $('#changesHistoryTable .filters th').each(function () {
                var idx = $(this).index();
                if (idx != 0) {
                    $(this).html('<input type="text" placeholder="Search" style="width:100%" />');
                }
            });

            // Apply the search
            table.columns().eq(0).each(function (colIdx) {
                $('input', $('.filters th')[colIdx]).on('keyup change', function () {
                    table
                        .column(colIdx)
                        .search(this.value)
                        .draw();
                });
            });
        },
        error: function () {
        $("#customloader").hide();
    }

    }).done(function () {
        $("#customloader").hide();
    });


});

function RestoreDefaultPrintout() {
    $("#customloader").show();
    $.ajax({
        type: 'POST',
        url: getUrl("RestorePrintoutToDefault"),
        data: {
            CreatedBy: $("#txt_hd_createdby").val(),
            Id: $("#txt_hd_id").val()
        },
        success: function (data) {
            if (data != "") {
                alert(data);
            }
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

$(document).on("click", "#btnRestorePrintoutToDefault", function () {
    CloseAllModal();
    RestoreDefaultPrintout();
    GeneratePrintOut();
});

function CleaningUnUseFileOther(TBodyId, DivId) {
    $("." + DivId).find('.txt_file').each(function () {
        var fileName = $(this).val().replace(/C:\\fakepath\\/i, '');
        fileName = fileName.replace(/^.*[\\\/]/, '')
        var filenameFromTable = "";
        var isAva = 0;
        $("#" + TBodyId).find(".td_filename").each(function () {
            if ($(this).data("fileuploadid") == 0) {
                filenameFromTable = $(this).data("filename");
                if (fileName == filenameFromTable) {
                    isAva = 1;
                }
            }
        });
        if (isAva == 0) {
            $(this).remove();
            var txtfilename = $("#" + $(this).data("thefieldnameid"));
            txtfilename.remove();
        }
    });
}

function CheckFieldBAEmpty() {
    var message = "";
    var emptyCount = 0;
    $("#div_alert").empty();
    var aaa = $("#opt_govstatus").val();
    if ($("#opt_govstatus").val() == "") {
        emptyCount++;
        message += "* The Government Status field is required.<br/>";
    }
    if ($("#txt_decree_number").val() == "") {
        emptyCount++;
        message += "* The SKep Number field is required.<br/>";
    }
    var ba_doc_count = 0;
    $("#div_body_ba_doc").find(".td_filename").each(function () {
        ba_doc_count++;
    });
    if (ba_doc_count == 0) {
        emptyCount++;
        message += "* At least 1 Attachment Document is required.<br/>";
    }
    if (emptyCount == 0) {
        $("#div_alert").hide();
    }
    else {
        $("#div_alert").show();
        $("#div_alert").append(message);
    }
    return emptyCount;
}

function AddRemovedFileToList(FileId) {
    if (FileId != 0 && FileId != "" && FileId != null && FileId != "0") {
        var html = "<input type='hidden' name='RemovedFilesId' value='" + FileId.toString() + "' />";
        $(".div_changerequest_utama").append(html);
    }
}
