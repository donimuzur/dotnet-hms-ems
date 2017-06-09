
var DetManufactIndex = 0;
var otherdocIndex = 0;
var BAdocIndex = 0;
var HdDocIndex = 0;

function setupDataTables() {
    var table = null;
    if ($.fn.dataTable.isDataTable('#kppbctable')) {
        table = $('#kppbctable').DataTable();
        table.destroy();txt
    }

    table = $('#kppbctable').DataTable(
        {
            "sDom": "Rlfrtip",
            "language": {
                "zeroRecords": "No records found",
                "infoFiltered": "(filtered from _MAX_ total records)"
            }
        });

    // Setup - add a text input to each footer cell
    $('#kppbctable .filters th').each(function () {
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
}

function CloseAllModal() {
    $('.ems-modal').modal('hide');
}

function OptCompanyList(NppbkcId) {
    $.ajax({
        type: 'POST',        
        url: getUrl("GetCompanyList"),
        data: { NppbkcId: NppbkcId },
        success: function (data) {
            var result = data;
            $("#opt_company").empty();
            $("#opt_company").append("<option value='' selected='selected'>Select</option>");
            for (var i = 0; i < result.length; i++) {
                $("#opt_company").append("<option data-npwp='" + result[i].Npwp + "' data-add='" + result[i].Company_Address + "' value='" + result[i].Company_ID + "'>" + result[i].Company_Name + "</option>");
            }
        }
    });
}

function FillCompanyDetail(npwp, address) {
    $("#Npwp").val(npwp);
    $("#Company_Address").val(address);
}

function AddDetailManufacture() {
    $("#customloader").show();
    $.ajax({
        type: 'POST',
        url: getUrl("AddDetailManufactureForm"),
        data: { Index: DetManufactIndex },
        cache: false,
        success: function (html) {
            DetManufactIndex++;
            $("#div_body_interviewdetail").append(html);
        }
    });
    $("#customloader").hide();
}

function SupportingDocumentList(CompanyId) {
    var LRId = $("#txt_hd_id").val();
    var isReadonly = $("#txt_isformreadonly").val();
    var statuskey = $("#txt_hd_statuskey").val();
    var param = {
        CompanyId: CompanyId,
        LRId: LRId,
        IsReadonly: isReadonly,
        StatusKey: statuskey
    };
    $.ajax({
        url: getUrl("GetSupportingDocuments"),
        type: 'POST',
        data: JSON.stringify(param),
        contentType: 'application/json; charset=utf-8',
    })
    .success(function (partialResult) {
        $("#div_body_supportingdoc").html(partialResult);
    })
    .error(function (error) {
    });
}

function GetFileSize(fileid) {
    try {
        fileSize = $("#" + fileid)[0].files[0].size //size in kb
        fileSize = fileSize / 1048576; //size in mb
        return fileSize;
    }
    catch (e) {
        //alert("Error is :" + e);
        return 0;
    }
}

//function FillDetailForm() {    
//    $("#customloader").show();
//    var IRId = $("#txt_hd_id").val();
//    $.ajax({
//        type: 'POST',
//        url: getUrl("GetInterviewRequestDetail"),
//        data: { IRId: IRId },
//        cache: false,
//        success: function (data) {
//            var result = data;
//            for (var i = 0; i < result.length; i++) {
//                AddDetailManufacture();
//                var _index = (DetManufactIndex - 1).toString();
//                alert(_index);
//                $("#interviewRequestDetail_" + _index + "__Manufacture_Address").val(result[i].Manufacture_Address);
//                $("#opt_city_" + _index).val(result[i].City_Id);
//                $("#txt_hd_stateid_" + _index).val(result[i].Province_Id);
//                $("#txt_state_" + _index).val(result[i].Province_Name);
//                $("#interviewRequestDetail_" + _index + "__Sub_District").val(result[i].Sub_District);
//                $("#interviewRequestDetail_" + _index + "__Village").val(result[i].Village);
//                var phone = result[i].Phone;
//                if(phone != "")
//                {
//                    var arrphone = phone.split("-");
//                    $("#interviewRequestDetail_" + _index + "__Phone_Area_Code").val(arrphone[0]);
//                    if (arrphone[1] != null) {
//                        $("#interviewRequestDetail_" + _index + "__Phone").val(arrphone[1]);
//                    }
//                }
//                var fax = result[i].Fax;
//                if (fax != "") {
//                    var arrfax = fax.split("-");
//                    $("#interviewRequestDetail_" + _index + "__Fax_Area_Code").val(arrfax[0]);
//                    if (arrfax[1] != null) {
//                        $("#interviewRequestDetail_" + _index + "__Fax").val(arrfax[1]);
//                    }
//                }
//            }
//        }
//    });
//    $("#customloader").hide();
//}

function ReIndexingDetailForm()
{
    var index = 0;
    $('.sub_panel_parentbody').each(function () {
        index++;
        $(this).find(".label_header_irdet").val("Detail Manufacture " + index.toString());
    });
}

function CompletingDetailForm() {
    $('.sub_panel_parentbody').each(function () {
        DetManufactIndex++;

        var ObjPhone = $(this).find(".txt_phone");
        var phone = ObjPhone.val().split('-');
        if(phone.length > 1)
        {
            $(this).find(".txt_phonecode").val(phone[0]);
            ObjPhone.val(phone[1]);
        }

        var ObjFax = $(this).find(".txt_fax");
        var fax = ObjFax.val().split('-');
        if (fax.length > 1) {
            $(this).find(".txt_faxcode").val(fax[0]);
            ObjFax.val(fax[1]);
        }
    });
    var companyid = $("#opt_company").find(':selected').val();    
    SupportingDocumentList(companyid);
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
        if (FileName == $(this).data("filename"))
        {
            isNotExist = false;
        }
    });
    return isNotExist;
}

function DeleteCheckedDetailManufact()
{
    $("#div_body_interviewdetail").find(".div_body_detmanufactform").each(function () {
        if($(this).find(".cb_panel_detailmanufacture").is(":checked"))
        {
            $(this).find(".txt_manufact_address").val("");
            $(this).hide();
        }
    });
}

function renderManufactureDetailNumber()
{
    var i = 1;
    $("#div_body_interviewdetail").find(".div_body_detmanufactform").each(function () {
        if ($(this).is(":visible")) {            
            $(this).find(".label_header_irdet").html("Detail Manufacture " + i.toString());
            i++;
        }
    });
}

function ToggleCheckAll()
{
    var isChecked = false;
    $("#div_body_interviewdetail").find(".div_body_detmanufactform").each(function () {
        if ($(this).is(":visible")) {
            if ($(this).find(".cb_panel_detailmanufacture").is(":checked") == false) {
                isChecked = true;
            }            
        }
    });

    $("#div_body_interviewdetail").find(".div_body_detmanufactform").each(function () {
        if ($(this).is(":visible")) {
            var checkbox = $(this).find(".cb_panel_detailmanufacture");
            if (isChecked) {                
                checkbox.attr('checked', true);
                checkbox.prop("checked", true);                
            }
            else {                
                checkbox.attr('checked', false);
                checkbox.prop("checked", false);                
            }
        }
    });
}

function AddRemovedFileToList(FileId)
{
    if (FileId != 0 && FileId != "" && FileId != null && FileId != "0") {
        var html = "<input type='hidden' name='RemovedFilesId' value='" + FileId.toString() + "' />";
        $(".div_licenserequest").append(html);
    }
}

function ChangeStatus(Action, Status, Comment)
{
    CloseAllModal();
    var IRId = $("#txt_req_id").val();
    
    $.ajax({
        type: 'POST',
        url: getUrl("ChangeStatus"),
        data: {
            LRId: IRId,
            Status: Status,
            Comment: Comment,
            Action: Action
        },
        success: function (data) {
            var errmessage = data;
            //alert(errmessage);
            if(data == "")
            {
                window.location.replace(getUrl(""));
            }
        }
    });
    
}

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

///////////////////////////////// END OF FUNCTIONS //////////////////////////////////

$(document).on("click", "#btn_kppbclist", function () {
    $.ajax({
        type: 'POST',
        url: getUrl("GetKPPBCList"),
        data: {},
        success: function (data) {
            var result = data;
            $("#tbody_kppbclist").empty();
            for (var i = 0; i < result.length; i++) {
                var address = result[i].ADDR1 + " " + result[i].ADDR2;
                var items = "";
                items += "<tr>";
                items += "<td><input type='button' class='btn btn-sm btn-default btn_add_kppbc_from_list' value='Add' data-kppbcid='" + result[i].KPPBC_ID + "' data-nppbkcid='" + result[i].NPPBKC_ID + "' data-kppbcadd='" + address + "' /></td>";
                items += "<td>" + result[i].KPPBC_ID + "</td>";
                items += "<td>" + result[i].NPPBKC_ID + "</td>";
                items += "<td>" + result[i].TEXT_TO + "</td>";
                items += "<td class='last'>" + address + "</td>";
                items += "</tr>";
                $("#tbody_kppbclist").append(items);
            }
            setupDataTables();
        },
        error: function (data) {
            ////////
        },
        complete: function () {
            ////////
        }
    });
});

$(document).on("click", ".btn_add_kppbc_from_list", function () {
    var kppbc_id = $(this).data("kppbcid");
    var nppbkc_id = $(this).data("nppbkcid");
    var kppbc_address = $(this).data("kppbcadd");
    $("#KPPBC_ID").val(kppbc_id);
    $("#KPPBC_Address").val(kppbc_address);
    $("#txt_hd_nppbkc_id").val(nppbkc_id);

    OptCompanyList(nppbkc_id);

    CloseAllModal();
});

$(document).on("change", "#opt_company", function () {
    var opt_selected = $(this).find(':selected');
    var npwp = opt_selected.data('npwp');
    var address = opt_selected.data('add');
    FillCompanyDetail(npwp, address);
    SupportingDocumentList(opt_selected.val());    
});

$(document).on("click", "#btn_add_interviewdetail", function () {        
    AddDetailManufacture();
    ReIndexingDetailForm();
    renderManufactureDetailNumber();
});

$(document).on("click", "#btn_add_ba_doc", function () {
    var items = "";
    items += "<div class='row div_group_doc'>";
    items += "<div class='col-md-8'>";
    items += "<input type='file' class='form-control file_BAdoc' id='file_BAdoc_" + BAdocIndex + "' name='File_BA' accept='application/pdf' />";
    items += "</div>";
    items += "<input type='button' class='btn btn-blue btn_del_doc' value='Delete' />";
    items += "</div>";
    $("#div_body_ba_doc").append(items);
    BAdocIndex++;
});

$(document).on("click", ".btn_del_doc", function () {
    $(this).parents(".div_group_doc").remove();
});

$(document).on("change", ".file_otherdoc", function () {
    var id = $(this).attr("id");
    var filesize = GetFileSize(id);
    
    if (filesize != null && filesize != undefined && filesize > 0) {
        var maxsize = parseInt($("#txt_hd_filesize").val());
        if(filesize > maxsize)
        {
            $(this).empty();
            $("Max file size is " +  String(maxsize) + " Mb");
        }
    }
});

$(document).on("change", ".opt_city", function () {
    var opt_selected = $(this).find(':selected');
    var stateId = opt_selected.data("stateid");
    var stateName = opt_selected.data("statename");
    var parent = $(this).parents(".sub_panel_parentbody");
    parent.find(".txt_hd_stateid").val(stateId);
    parent.find(".txt_state").val(stateName);
});

$(document).on("click", "#btn_browseOtherDoc_file", function () {
    var div_parent = $(this).closest(".div_file");    
    var htmlInput = "<input id='txt_otherdoc_" + HdDocIndex + "' type='file' name='File_Other' class='hidden txt_file' accept='application/pdf' data-index='other_" + HdDocIndex + "' />";
    htmlInput += "<input type='hidden' name='File_Other_Name' id='txt_otherdoc_filename_other_" + HdDocIndex + "' />";
    div_parent.append(htmlInput);
    $("#txt_otherdoc_" + HdDocIndex).click();
    HdDocIndex++;
});

//$(document).on("change", ".txt_file", function () {
//    var txtFileName = $(this).closest(".div_file").find(".txt_file_name");
//    var fileName = $(this).val().replace(/C:\\fakepath\\/i, '');
//    txtFileName.val(fileName);    
//});

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
            fileName = fileName.replace(/^.*[\\\/]/, '')
            txtFileName.val(fileName);
        }
    }
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
                AddOtherFileList(filename, thefilename);
                txtfilename.val(filename + "^" + thefilename);
            }
            else
            {
                alert("File is already added");
            }
        }
        else {
            $("#txt_otherdoc_filename_" + txtfilenameindex).remove();
            txtfilename.remove();
        }
    }
    else {
        alert("File name field is required");
    }
});

$(document).on("click", ".btn_remove_otherdoc", function () {
    var thisTr = $(this).closest("tr");
    var fileuploadid = $(this).data("fileuploadid");
    var Removingfilename = thisTr.find(".td_filename").data("filename");
    
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


$(document).on("click", "#btn_browsebaDoc_file", function () {
    var div_parent = $(this).closest(".div_file");
    var htmlInput = "<input id='txt_badoc_" + BAdocIndex + "' type='file' name='File_BA' class='hidden txt_file' accept='application/pdf' data-index='ba_" + BAdocIndex + "' />";
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
            else {
                alert("File is already added");
            }
        }
       
    }
    else {
        alert("File name field is required");
    }
});

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

$(document).on("click", ".btn_browsesupportdoc_file", function () {
    var index = $(this).data("index");
    $("#txt_supportdoc_file" + index).click();
});

$(document).on("click", "#btn_delete_manufacturedetail", function () {
    DeleteCheckedDetailManufact();
    renderManufactureDetailNumber();
});

$(document).on("click", "#btn_checkall_manufacturedetail", function () {
    ToggleCheckAll();
});

$(document).on("click", ".btn_remove_supportdoc", function () {
    var fileuploadid = $(this).data("fileuploadid");
    var parent = $(this).closest(".div_file");
    parent.find(".div_file_href").remove();
    parent.find(".div_supportdoc_name").show();
    parent.find(".span_browsesupportdoc").show();
    AddRemovedFileToList(fileuploadid);
});

//$(document).on("click", "#btnSave", function () {
//    var fieldEmpty = CheckFieldEmpty();
//    if (fieldEmpty == 0) {
//        $("#customloader").show();
//        $("#SaveLRequestForms").submit();
//    }
//});

$(document).on("click", "#btnSave", function () {
    $("#customloader").show();
    var fieldEmpty = CheckFieldEmpty();
    if (fieldEmpty == 0) {
        CleaningUnUseFileOther("div_otherDocsBody", "div_file_otherdoc");
        $("#SaveLRequestForms").submit();
    }
    else {
        $("#customloader").hide();
    }
});

$(document).on("click", "#btnSubmit", function () {
    CloseAllModal();
    //var fieldEmpty = CheckFieldEmpty();
    //if (fieldEmpty == 0) {
        //$("#customloader").show();
        //$("#SaveLRequestForms").submit();
        //
        ChangeStatus("submit", "WAITING_POA_APPROVAL", "");
    //}
});

$(document).on("click", "#btnSubmitSkep_check", function () {
    var fieldEmpty = CheckFieldBAEmpty();    
    if (fieldEmpty == 0) {
        $("#btnSubmitSkep_showmodal").click();
    }
});

$(document).on("click", "#btnSubmitSkep", function () {
    $("#customloader").show();
    var fieldEmpty = CheckFieldBAEmpty();
    
    if (fieldEmpty == 0) {
        //$("#customloader").show();
        //CleaningUnUseFileOther("div_body_ba_doc", "div_file_badoc");
        CloseAllModal();
        $("#SaveLRequestForms").submit();
    }
     else
    {
        $("#customloader").hide();
    }
});

/*
$(document).on("click", "#btnSubmit", function () {
    $("#customloader").show();
    $("#txt_hd_status").val("WAITING FOR POA APPROVAL");
    $("#SaveIRequestForms").submit();
});
*/
$(document).on("click", ".btn_showmodal_changestatus", function () {
    var action = $(this).data("action");
    if (action == "Revise") {
        $("#txt_label_forchangestatus").text("Revise Form Confirmation");
        $("#btn_changestatus_revise").show();
    }
    else if (action == "ReviseSkep") {
        $("#txt_label_forchangestatus").text("Revise Form Confirmation");
        $("#btn_changestatus_reject").hide();
        $("#btn_changestatus_reviseskep").show();
    }
    else if (action == "Reject") {
        $("#txt_label_forchangestatus").text("Reject Form Confirmation");
        $("#txt_label_forchangestatus").text("Reject");
        $("#btn_changestatus_reviseskep").hide();
        $("#btn_changestatus_reject").show();
    }
});

$(document).on("click", ".btn_changestatus_submit", function () {
    
    var action = $(this).data("action");
    
    var status = "";
    if (action == "revise") {
        status = "DRAFT_EDIT_STATUS";
    }
    else if (action == "reviseskep") {
        status = "WAITING_GOVERNMENT_APPROVAL";
    }
    else{
        status = "CANCELED";
    }
    var comment = $("#txt_changestatus_comment").val();
    if (comment == "" && (action == "revise" || action == "reviseskep")) {
        $("#div_alert_revise").show();
        $("#div_alert_revise").html("* Note/Comment cannot be empty.");
    }
    else {
        $("#customloader").show();
        ChangeStatus(action, status, comment);
    }
});

$(document).on("click", "#btnCancel", function () {
    ChangeStatus("cancel", "CANCELED", "");
});

$(document).on("click", "#btnApprove", function () {
    ChangeStatus("approve", "WAITING_GOVERNMENT_APPROVAL", "");
});

$(document).on("click", "#btnApproveFinal", function () {
    ChangeStatus("approve", "COMPLETED", "");
});

$(document).on("click", "#btnWithdraw", function () {
    ChangeStatus("withdraw", "DRAFT_EDIT_STATUS", "");
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

    var jml = $('#supportdoc_index').val();
    if (jml == undefined)
    {
        var message = "* At least 1 Supporting Document is exist.";
        $("#div_alert").append(message + "<br>");
    }

    var pt = 0;
    $('.prodtype_list').each(function () {
        if ($(this).is(':checked'))
        {
            pt++;
        }
    });
    if (pt == 0)
    {
        var message = "* At least 1 product type field is selected.";
        $("#div_alert").append(message + "<br>");
    }

    var detmanCount = 0;
    $('.div_body_detmanufactform').each(function () {
        detmanCount++;
    });
    //if (detmanCount == 0) {
    //    emptyCount++;
    //    var message = "* At least 1 Interview & Location Visit is required.";
    //    $("#div_alert").append(message + "<br/>");
    //}
    if (emptyCount == 0) {
        $("#div_alert").hide();
    }
    else {
        $("#div_alert").show();
    }
    return emptyCount;
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
    if ($("#decree_number").val() == "") {        
        emptyCount++;
        message += "* The Decree Number field is required.<br/>";
    }    
    if ($("#decree_date").val() == "") {
        emptyCount++;        
        message += "* The Decree Date field is required.<br/>";
    }    
    if ($("#nppbkc_id").val() == "") {
        emptyCount++;        
        message += "* The NPPBKC field is required.<br/>";
    }    
    var ba_doc_count = 0;
    $("#div_body_ba_doc").find("tr").each(function () {        
        ba_doc_count++;
    });    
    if (ba_doc_count == 0) {        
        emptyCount++;
        message += "* Atleast 1 BA Document is required.<br/>";
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

$(document).on("click", ".btn_loader", function () {
    $("#customloader").show();
});
/*
$(document).on("click", "#btn-tab-print-layout", function () {
    $("#div-print-layout").empty();

    var companyaddress = $(this).find(".txt_manufact_address").val();

    var companycity = $(this).find(".txt_manufact_district").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GetPrintOutLayout"),
        data: {},
        success: function (data) {
            
            var requestdate = $("#RequestDate").val();
            var formnumber = $("#div_form_number").text();            
            var perihal = $("#opt_perihal").val();
            var kppbc = $("#KPPBC_ID").val();
            if (kppbc == undefined)
            {
                kppbc = $("#KPPBC").val();
            }
            var kppbcaddress = $("#KPPBCAddress").val();
            if (kppbcaddress == undefined)
            {
                kppbcaddress = $("#KppbcAddress").val();
            }
            var POAName = $("#POAName").val();
            var POAPosition = $("#POAPosition").val();
            if (POAPosition == undefined)
            {
                POAPosition = $("#Position").val();
            }
            var POAAddress = $("#POAAddress").text();            
            var companytype = $("#CompType").val();
            var companyname = $("#Company").val();
            var prodtypelist = $("#product_type_list").val();
            
            var manufact_info = $("#Mnflocationlist").val();
            var prodtype_info = $("#product_type_list").val();
            var otherprodtype_info = $("#otherproducttype_list").val();
            var supportdoc_info = $("#supportdoc_list").val();

            var mnfcity_info = $("#Mnfcitylist").val();
            var mnfaddr_info = $("#Mnfaddresslist").val();
            var lampirancount_info = $("#txt_lampirancount").val();

            var html = data;
            
            html = html.replace(/#REQUEST_DATE/g, requestdate);
            html = html.replace(/#FORM_NUMBER/g, formnumber);
            html = html.replace(/#LAMPIRAN_COUNT/g, '0');
            html = html.replace(/#PERIHAL/g, perihal);
            html = html.replace(/#KPPBC/g, kppbc);
            html = html.replace(/#ADDRESS_KPPBC/g, kppbcaddress);
            html = html.replace(/#POA_NAME/g, POAName);
            html = html.replace(/#POA_ROLE/g, POAPosition);
            html = html.replace(/#POA_ADDRESS/g, POAAddress);
            html = html.replace(/#COMPANY_TYPE/g, companytype);
            html = html.replace(/#COMPANY_NAME/g, companyname);

            html = html.replace(/#MANUFACTURE_INFO/g, manufact_info);
            html = html.replace(/#PROD_TYPE_LIST/g, prodtype_info);
            html = html.replace(/#OTHERPRODTYPE_LIST/g, otherprodtype_info);
            html = html.replace(/#SUPPORTDOC_LIST/g, supportdoc_info);
            html = html.replace(/#MNFCITY_LIST/g, mnfcity_info);
            html = html.replace(/#MNFADDR_LIST/g, mnfaddr_info);
            html = html.replace(/#LAMPIRAN_COUNT/g, lampirancount_info);

            $("#div-print-layout").append(html);
        }
    });
});

$(document).on("click", "#btn_download_printout", function () {
    var html = tinyMCE.get('txt_printoutlayout').getContent();
    $.ajax({
        type: 'POST',
        url: getUrl("DownloadPrintOut"),
        data: { htmlText: html },
        success: function (data) {
        }
    });
});

$(document).on("click", "#btn_show_printouteditor", function () {
    var html = $("#div-print-layout").html().toString();
    tinyMCE.activeEditor.setContent(html);
});
*/
$(document).on("click", "#btn-tab-print-layout", function () {
    
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

function GeneratePrintOut() {
    $("#customloader").show();
    $("#div-print-layout").empty();
    var ID = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GeneratePrintout"),
        data: { MnfRequestID: ID },
        success: function (data) {
            $("#div-print-layout").html(data);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function GeneratePrintOutPDF() {
    //$("#customloader").show();
    //var InterviewID = $("#txt_hd_id").val();
    //var FormNumber = $("#FormNumber").val();
    //$.ajax({
    //    type: 'POST',
    //    url: getUrl("DownloadPrintOut"),
    //    data: {
    //        InterviewID: InterviewID,
    //        FormNumber: FormNumber
    //    },
    //    complete: function () {
    //        $("#customloader").hide();
    //    }
    //});
    $("#form_DownloadPrintout").submit();
}

function GetPrintoutLayout() {
    $("#customloader").show();
    var Creator = $("#txt_hd_createdby").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GetPrintOutLayout"),
        data: { Creator: Creator },
        success: function (html) {
            tinyMCE.activeEditor.setContent(html);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
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
            CreatedBy: $("#txt_hd_createdby").val()
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


$(document).on("click", "#btn-tab-changeslog", function () {
    var isalreadyloaded = $("#txt_ischangelog_loaded").val();
    if (isalreadyloaded == "0") {
        var crid = $("#txt_hd_id").val();
        $("#customloader").show();
        $.ajax({
            //url: '/MLLicenseRequest/ChangeLog',
            url: getUrl("ChangeLog"),
            dataType: 'html',
            data: { CRID: crid },
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
            },
            complete: function () {
            $("#txt_ischangelog_loaded").val("1");
            $("#customloader").hide();
        }
    });
        //}).done(function () {
        //    $("#customloader").hide();
        //});

    }
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

function RestoreDefaultPrintout() {
    $("#customloader").show();
    $.ajax({
        type: 'POST',
        url: getUrl("RestorePrintoutToDefault"),
        data: {},
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

$(document).on('keydown', '.numeric-form', function (e) {
    { -1 !== $.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) || /65|67|86|88/.test(e.keyCode) && (!0 === e.ctrlKey || !0 === e.metaKey) || 35 <= e.keyCode && 40 >= e.keyCode || (e.shiftKey || 48 > e.keyCode || 57 < e.keyCode) && (96 > e.keyCode || 105 < e.keyCode) && e.preventDefault() }
});

function CloseAllModal() {
    $('.ems-modal').modal('hide');
}
