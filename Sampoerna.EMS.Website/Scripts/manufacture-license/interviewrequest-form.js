var DetManufactIndex = 0;
var otherdocIndex = 0;
var BAdocIndex = 0;
var HdDocIndex = 0;

function setupDataTables() {
    var _table = null;
    if ($.fn.dataTable.isDataTable('#kppbctable')) {
        _table = $('#kppbctable').DataTable();
        _table.destroy();
    }

    _table = $('#kppbctable').DataTable(
        {
            "sDom": "Rlfrtip","language": {"zeroRecords": "No records found","infoFiltered": "(filtered from _MAX_ total records)"
            }
        });
    
    $('#kppbctable .filters th').each(function () {
        var idx = $(this).index();
        if (idx != 0) {
            $(this).html('<input class="txtsearchinputcol" type="text" placeholder="Search" style="width:100%" />');
        }
    });

    $('.txtsearchinputcol').on('keyup change', function () {
        _table.search(this.value).draw();
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
            var value = "";
            var npwp = "";
            var address = "";
            for (var i = 0; i < result.length; i++) {
                $("#opt_company").append("<option data-npwp='" + result[i].Npwp + "' data-add='" + result[i].Company_Address + "' value='" + result[i].Company_ID + "'>" + result[i].Company_Name + "</option>");
                value = result[i].Company_ID;
                npwp = result[i].Npwp;
                address = result[i].Company_Address;
            }
            if (result.length == 1) {
                $('#opt_company').val(value);
                FillCompanyDetail(npwp, address);
                SupportingDocumentList(value);
            }
            $('#opt_company').selectpicker('refresh');
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
            $("#div_body_interviewdetail").append(html);
            $('#opt_city_' + DetManufactIndex.toString()).attr("data-live-search", "true");
            $('#opt_city_' + DetManufactIndex.toString()).selectpicker("refresh");                        
            DetManufactIndex++;
        },
        complete: function()
        {
            renderManufactureDetailNumber();
            $("#customloader").hide();
        }
    });    
}

function SupportingDocumentList(CompanyId) {
    var irid = $("#txt_hd_id").val();
    var statusKey = $("#txt_hd_statuskey").val();
    var isReadonly = $("#txt_hd_isformreadonly").val();
    var param = {
        CompanyId: CompanyId,
        IRId: irid,
        IsReadonly: isReadonly,
        StatusKey: statusKey
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
        ////
    });
}

function GetFileSize(fileid) {
    try {
        var fileSize = 0;
        //if (navigator.userAgent.match(/msie/i) || navigator.userAgent.match(/trident/i)) {
        //    var objFSO = new ActiveXObject("Scripting.FileSystemObject");
        //    var filePath = $("#" + fileid)[0].value;
        //    var objFile = objFSO.getFile(filePath);
        //    var fileSize = objFile.size; //size in kb
        //    fileSize = fileSize / 1048576; //size in mb 
        //}
        //else {        
            fileSize = $("#" + fileid)[0].files[0].size //size in kb
            fileSize = fileSize / 1048576; //size in mb 
        //}
        return fileSize;
    }
    catch (e) {
        //alert("Error is :" + e);
        return 0;
    }
}

//function ReIndexingDetailForm()
//{
//    var index = 0;
//    $('.sub_panel_parentbody').each(function () {
//        index++;
//        $(this).find(".label_header_irdet").val("Detail Manufacture " + index.toString());
//    });
//}

function CompletingDetailForm() {
    $('.sub_panel_parentbody').each(function () {
        DetManufactIndex++;

        //var cityid = $(this).find(".txt_hd_cityid").val();
        //$(this).find(".opt_city").val(cityid);
        $(this).find(".opt_city").attr("data-live-search", "true");
        $(this).find(".opt_city").selectpicker('refresh');

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
            var ManFactId = $(this).find(".txt_manufactdet_id").val();
            if (ManFactId > 0) {
                $("#div_body_interviewdetail").append("<input type='hidden' name='RemovedDetailId' value='" + ManFactId + "' />");
            }
            $(this).find(".txt_manufact_address").val("");
            $(this).removeClass("stillActive");
            $(this).hide();
        }
    });
}

function renderManufactureDetailNumber()
{
    var i = 1;
    $(".div_body_detmanufactform.stillActive").each(function () {        
        $(this).find(".label_header_irdet").html("Detail Manufacture " + i.toString());
        i++;
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
        $(".div_interviewrequest").append(html);
    }
}

function ChangeStatus(Action, Status, Comment)
{
    CloseAllModal();
    var IRId = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("ChangeStatus"),
        data: {
            IRId: IRId,
            Status: Status,
            Comment: Comment,
            Action: Action
        },
        success: function (data) {
            window.location.replace(getUrl(""));
        }
    });
}

function SaveNewKppbc() {
    var IRId = $("#txt_hd_id").val();
    var KppbcId = $("#txt_new_kppbcid").val();
    var KppbcAdd = $("#txt_new_kppbcadd").val();
    var City = $("#txt_new_kppbccity").val();
    var CityAlias = $("#txt_new_kppbccityalias").val();
    var TextTo = $("#txt_new_kppbctextto").val();
    $.ajax({
        type: 'POST',
        url: getUrl("AddNewKppbc"),
        data: {
            IRId:IRId,
            KPPBCId: KppbcId
        },
        success: function (data) {            
            if (data != null) {
                if (data != "") {
                    $("#err_new_kppbc").hide();
                    var nppbkcid = "";
                    $("#KPPBC_ID").val(KppbcId);
                    $("#KPPBC_Address").val(KppbcAdd);
                    $("#txt_hd_nppbkc_id").val(nppbkcid);
                    $("#txt_hd_city").val(City);
                    $("#txt_hd_cityalias").val(CityAlias);
                    $("#txt_hd_textto").val(TextTo);
                    OptCompanyList("0");
                    SupportingDocumentList(0);
                    CloseAllModal();
                }
                else {
                    $("#err_new_kppbc").show();
                    $("#err_new_kppbc").html("KPPBC already exist");
                }
            }
            else {
                alert("Something error happened, failed to add new KPPBC.");
            }
        }
    });
}

function CheckFieldEmpty()
{
    var emptyCount = 0;
    $("#div_alert").empty();
    var arr = [];
    if ($('#opt_perihal').val() == "")
    {
        emptyCount++;
        var message = "* The Perihal field is required.";
        $("#div_alert").append(message + "<br/>");
    }
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
    if ($('#opt_company').val() == "") {
        emptyCount++;
        var message = "* The Company field is required.";
        $("#div_alert").append(message + "<br/>");
    }
    var detmanCount = 0;
    $('.div_body_detmanufactform:visible').each(function () {
        detmanCount++;
    });
    if (detmanCount == 0)
    {
        emptyCount++;
        var message = "* Atleast 1 Interview & Location Visit is required.";
        $("#div_alert").append(message + "<br/>");
    }
    if (emptyCount == 0)
    {
        $("#div_alert").hide();
    }
    else
    {
        $("#div_alert").show();
    }
    return emptyCount;
}

function CheckFieldBAEmpty()
{
    var message = "";
    var emptyCount = 0;
    $("#div_alert").empty();
    var aaa = $("#opt_govstatus").val();
    if ($("#opt_govstatus").val() == "") {
        emptyCount++;
        message += "* The Government Status field is required.<br/>";
    }    
    if ($("#BANumber").val() == "") {
        emptyCount++;
        message += "* The BA Number field is required.<br/>";
    }    
    if ($("#BADate").val() == "") {
        emptyCount++;
        message += "* The BA Date field is required.<br/>";
    }
    var ba_doc_count = 0;
    $("#div_body_ba_doc").find(".td_filename").each(function () {
        ba_doc_count++;
    });
    if (ba_doc_count == 0)
    {
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

function GeneratePrintOut() {
    $("#customloader").show();
    $("#div-printout").empty();
    var ID = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GeneratePrintout"),
        data: { InterviewID: ID },
        success: function (data) {
            var _html = "";
            for (var i = 0; i < data.length; i++) {
                _html += "<div class='panel-group' id='accordionOne" + i + "' role='tablist' aria-multiselectable='true'>";
                _html += "<div class='panel panel-default'>";
                _html += "<div class='panel-heading' role='tab' id='headingOne" + i + "'>";
                _html += "<h4 class='panel-title'>";
                _html += "<a data-toggle='collapse' data-parent='#accordion" + i + "' href='#collapseOne" + i + "' aria-expanded='true' aria-controls='collapseOne" + i + "'>";
                _html += "Interview &amp; Location Visit " + (i + 1).toString();
                _html += "<i class='fa fa-caret-down'></i>";
                _html += "</a>";
                _html += "</h4>";
                _html += "</div>";
                _html += "<div id='collapseOne" + i + "' class='panel-collapse collapse in' role='tabpanel' aria-labelledby='headingOne" + i + "'>";
                _html += "<div class='panel-body'>";
                _html += "<div class='form-excise' role='form'>";
                _html += data[i];
                _html += "</div>";
                _html += "</div>";
                _html += "</div>";
                _html += "</div>";
                _html += "</div>";
            }
            $("#div-printout").html(_html);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function GeneratePrintOutPDF()
{    
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
    $("#txt_ischangelog_loaded").val("0");
    $("#form_DownloadPrintout").submit();    
}

function GetPrintoutLayout()
{    
    $("#customloader").show();
    var Creator = $("#txt_hd_creator").val();
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
    var Creator = $("#txt_hd_creator").val();
    var IrId = $("#txt_hd_id").val();
    CloseAllModal();
    var layout = tinyMCE.get('txt_printoutlayout').getContent();
    $.ajax({
        type: 'POST',
        url: getUrl("UpdatePrintOutLayout"),
        data: {
            NewPrintout: layout,
            Creator: Creator,
            IRID: IrId
        },
        success: function (result) {
            if (result == "") {
                GeneratePrintOut();
                $("#txt_ischangelog_loaded").val("0");
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

function CleaningUnUseFileOther(TBodyId, DivId)
{    
    $("." + DivId).find('.txt_file').each(function () {        
        var fileName = $(this).val().replace(/C:\\fakepath\\/i, '');
        fileName = fileName.replace(/^.*[\\\/]/, '')
        var filenameFromTable = "";
        var isAva = 0;
        $("#" + TBodyId).find(".td_filename").each(function () {            
            if ($(this).data("fileuploadid") == 0) {                
                filenameFromTable = $(this).data("filename");
                if(fileName == filenameFromTable)
                {                    
                    isAva = 1;
                }
            }
        });        
        if(isAva == 0)
        {            
            $(this).remove();            
            var txtfilename = $("#" + $(this).data("thefieldnameid"));
            txtfilename.remove();
        }        
    });
}

function RestoreDefaultPrintout() {
    $("#customloader").show();
    var Creator = $("#txt_hd_creator").val();
    var IrId = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("RestorePrintoutToDefault"),
        data: {
            Creator: Creator,
            IRID: IrId
        },
        success: function (data) {
            if (data != "") {
                alert(data);
            }
            else
            {
                $("#txt_ischangelog_loaded").val("0");
            }
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

///////////////////////////////// END OF FUNCTIONS //////////////////////////////////

$(document).on('keydown', '.numeric-form', function (e) {
    { -1 !== $.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) || /65|67|86|88/.test(e.keyCode) && (!0 === e.ctrlKey || !0 === e.metaKey) || 35 <= e.keyCode && 40 >= e.keyCode || (e.shiftKey || 48 > e.keyCode || 57 < e.keyCode) && (96 > e.keyCode || 105 < e.keyCode) && e.preventDefault() }
});

$(document).on("click", "#btn_kppbclist", function () {
    $.ajax({
        type: 'POST',
        url: getUrl("GetKPPBCList"),
        data: {},
        success: function (data) {
            var result = data;
            $("#tbody_kppbclist").empty();
            for (var i = 0; i < result.length; i++) {
                var address = result[i].ADDR1;
                var items = "";
                items += "<tr>";
                items += "<td><input type='button' class='btn btn-sm btn-default btn_add_kppbc_from_list' value='Add' data-kppbcid='" + result[i].KPPBC_ID + "' data-nppbkcid='" + result[i].NPPBKC_ID + "' data-kppbcadd='" + address + "' data-city='" + result[i].CITY + "' data-cityalias='" + result[i].CITY_ALIAS + "' data-textto='" + result[i].TEXT_TO + "' /></td>";
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
    var City = $(this).data("city");
    var CityAlias = $(this).data("cityalias");
    var TextTo = $(this).data("textto");
    $("#KPPBC_ID").val(kppbc_id);
    $("#KPPBC_Address").val(kppbc_address);
    $("#txt_hd_nppbkc_id").val(nppbkc_id);
    $("#txt_hd_city").val(City);
    $("#txt_hd_cityalias").val(CityAlias);
    $("#txt_hd_textto").val(TextTo);
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
    $("#customloader").show();
    AddDetailManufacture();
    //ReIndexingDetailForm();    
    $("#customloader").hide();
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

$(document).on("click", "#btn_browseOtherDoc_file", function () {
    var div_parent = $(this).closest(".div_file");
    var htmlInput = "<input id='txt_otherdoc_" + HdDocIndex + "' type='file' name='File_Other' class='hidden txt_file' accept='application/pdf' data-thefieldnameid='txt_otherdoc_filename_other_" + HdDocIndex + "' data-index='other_" + HdDocIndex + "' />";
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
                AddOtherFileList(filename, thefilename);
                txtfilename.val(filename + "^" + thefilename);
            }
            else
            {
                alert("File is already added");
            }
        }
        else
        {
            $("#txt_otherdoc_" + txtfilenameindex).remove();
            txtfilename.remove();
        }
    }
    else
    {
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
    else
    {        
        thisTr.remove();
    }
    AddRemovedFileToList(fileuploadid);
    renderFileNumberList("#div_otherDocsBody");
});

$(document).on("click", "#btn_browsebaDoc_file", function () {
    var div_parent = $(this).closest(".div_file");
    var htmlInput = "<input id='txt_badoc_" + BAdocIndex + "' type='file' name='File_BA' class='hidden txt_file' accept='application/pdf' data-thefieldnameid='txt_otherdoc_filename_ba_" + BAdocIndex + "' data-index='ba_" + BAdocIndex + "' />";
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
    else
    {
        alert("File name field is required");
    }
});

$(document).on("click", ".btn_remove_badoc", function () {
    var thisTr = $(this).closest("tr");
    var fileuploadid = $(this).data("fileuploadid");
    var Removingfilename = thisTr.find(".td_filename").data("filename");
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
    parent.find(".supporting_document").prop('required', true);
    AddRemovedFileToList(fileuploadid);
});

$(document).on("click", "#btnSubmit", function () {
    CloseAllModal();
    //CleaningUnUseFileOther("div_otherDocsBody", "div_file_otherdoc");    
    //$("#txt_hd_status").val("WAITING FOR POA APPROVAL");
    //$("#SaveIRequestForms").submit();
    ChangeStatus("submit", "WAITING FOR POA APPROVAL", "");
});

$(document).on("click", ".btn_showmodal_changestatus", function () {
    var action = $(this).data("action");
    if(action == "Revise")
    {
        $("#txt_label_forchangestatus").text("Revise Form Confirmation");
        $("#btn_changestatus_revise").show();
    }
    else if (action == "ReviseSkep")
    {
        $("#txt_label_forchangestatus").text("Revise Form Confirmation");
        $("#btn_changestatus_reject").hide();
        $("#btn_changestatus_reviseskep").show();
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
    var action = $(this).data("action");
    var status = "";
    if (action == "revise") {
        status = "DRAFT EDIT";
    }
    else if(action == "reviseskep")
    {
        status = "WAITING FOR GOVERNMENT APPROVAL";
    }    
    else {
        status = "CANCELED";
    }
    var comment = $("#txt_changestatus_comment").val();
    if (comment == "" && (action == "revise" || action == "reviseskep" || action == "withdraw")) {
        $("#div_alert_revise").show();
        $("#div_alert_revise").html("* Note/Comment cannot be empty.");
    }
    else
    {
        $("#customloader").show();
        ChangeStatus(action, status, comment);
    }
});

$(document).on("click", "#btnCancel", function () {    
    ChangeStatus("cancel", "CANCELED", "");
});

$(document).on("click", "#btnApprove", function () {
    ChangeStatus("approve", "WAITING FOR GOVERNMENT APPROVAL", "");
});

$(document).on("click", "#btnApproveFinal", function () {    
    ChangeStatus("approve_final", "COMPLETED", "");
});

$(document).on("click", "#btn_add_newkppbc", function () {    
    var msg = "";
    if ($("#txt_new_kppbcid").val() == "")
    {
        msg += "* KPPBC Id Cannot be empty <br/>";
    }
    if ($("#txt_new_kppbcadd").val() == "")
    {
        msg += "* KPPBC Address Cannot be empty <br/>";
    }
    if ($("#txt_new_kppbccity").val() == "")
    {
        msg += "* City Cannot be empty <br/>";
    }
    if ($("#txt_new_kppbccityalias").val() == "")
    {
        msg += "* City Alias Cannot be empty <br/>";
    }
    if ($("#txt_new_kppbctextto").val() == "")
    {
        msg += "* Text To Cannot be empty <br/>";
    }
    if (msg == "") {
        SaveNewKppbc();
    }
    else
    {
        $("#err_new_kppbc").show();
        $("#err_new_kppbc").html(msg);
    }
});

$(document).on("click", "#btnSave", function () {
    $("#customloader").show();
    var fieldEmpty = CheckFieldEmpty();
    if(fieldEmpty == 0)
    {
        CleaningUnUseFileOther("div_otherDocsBody", "div_file_otherdoc");
        $("#SaveIRequestForms").submit();
    }
    else
    {
        $("#customloader").hide();
    }
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
        CleaningUnUseFileOther("div_body_ba_doc", "div_file_badoc");
        CloseAllModal();
        $("#SaveIRequestForms").submit();
    }
    else
    {
        $("#customloader").hide();
    }
});

//$(document).on("click", "#btn_changestatus_withdraw", function () {
//    ChangeStatus("withdraw", "DRAFT EDIT", "");
//});

$(document).on("click", ".btn_loader", function () {
    $("#customloader").show();
});

$(document).on("click", "#btn-tab-printout", function () {
    var ID = $("#txt_hd_id").val();
    if (ID != 0) {
        $("#print").show();
        GeneratePrintOut();
    }
    else
    {
        $("#print").hide();
    }
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

$(document).on("click", "#btn-tab-changeslog", function () {
    var isalreadyloaded = $("#txt_ischangelog_loaded").val();
    var now = new Date();
    var Token = now.getDate().toString() + now.getDate().toString() + (now.getMonth() + 1).toString()
                + now.getFullYear().toString()
                + now.getHours().toString()
                + now.getMinutes().toString()
                + now.getSeconds().toString();
    if (isalreadyloaded == "0") {
        var ID = $("#txt_hd_id").val();
        $("#customloader").show();
        $.ajax({
            url: getUrl("ChangeLog"),
            dataType: 'html',
            data: {
                ID: ID,
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
            },
            complete: function () {
                $("#txt_ischangelog_loaded").val("1");
                $("#customloader").hide();
            }
        });
    }
});

$(document).on("click", "#btnRestorePrintoutToDefault", function () {
    CloseAllModal();
    RestoreDefaultPrintout();
    GeneratePrintOut();
});

//$(document).on("change", ".opt_city", function () {
//    $("#customloader").show();
//    var ID = $(this).val();
//    var parent = $(this).parents(".sub_panel_parentbody");    
//    $.ajax({
//        type: 'POST',
//        url: getUrl("GetCityDetailById"),
//        data: { ID: ID },
//        success: function (data) {
//            var result = data;
//            if (result != null) {
//                var stateId = result.StateId;
//                var stateName = result.StateName;
//                parent.find(".txt_hd_stateid").val(stateId);
//                parent.find(".txt_state").val(stateName);
//            }
//        },
//        complete: function () {
//            $("#customloader").hide();
//        }
//    });
//});