var docsContainer = $("#SupportingDocs");
var skepNumber = $("#SKEPNumber");
var kppbc = $("#KppbcText");
var nppbkc = $("#NppbkcSelector");
var company = $("#CompanyText");
var companyId = $("#NPPBKC_Company_Id");
var npwp = $("#NpwpText");
var addressPlant = $("#AddressPlant");
var createForm = $("#SKEPCreateForm");
var otherDocs = [];
var supportingDocs = [];
var sErrorDiv = $("#SupportingError");
var submitDate = $("#SubmissionDate");
var trItemIndex = 0;
var itemdetailindex = 0;
var HdDocIndex = 0;

function CloseAllModal() {
    $('.ems-modal').modal('hide');
}

function attach() {
    nppbkc.on("change", nppbkcSelectionChanges);
    nppbkcSelectionChanges();
    $("#addOtherDocBtn").on("click", addOtherDoc);
    handleBrowseEvent("browseOtherDoc", "browseOtherDocFile", "browseOtherDocText");
}

function detach() {
    $("#addOtherDocBtn").off("click", addOtherDoc);    
}

function skepGenerateClick() {
    console.log("generate");
    var fileName = $('[name="SkepExcelfile"]').val().trim();
    var pos = fileName.lastIndexOf('.');
    var extension = (pos <= 0) ? '' : fileName.substring(pos);
    if (extension != '.xlsx' && extension != '.xls') {
        alert('Please browse a correct Excel File to upload.');
        return false;
    }
    var formData = new FormData();
    var totalFiles = document.getElementById("SkepExcelfile").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("SkepExcelfile").files[i];
        formData.append("skepExcelfile", file);
    }
    console.log(formData);
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
                url: uploadUrlSKEP,
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
    if (otherDocs.length > index) {
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
            '<td><button class="btn btn-danger" onclick="removeFromList(' + i + ')">Remove</button></td>' +
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

function loadSupportingDocuments(nppbkc) {
    $("#customloader").show();
    var ID = $("#txt_hd_id").val();
    var isEnable = $("#txt_hd_isformenable").val()    
    docsContainer.html("");
    $.ajax({
        url: getUrl("GetSupportingDocumentsSKEP"),
        type: 'POST',
        data: {
            ID: ID,
            nppbkc: nppbkc,
            isEnable: isEnable
        },
        success: function (partialResult) {
            docsContainer.html(partialResult);            
            sErrorDiv.hide();
        },
        error: function () {            
            sErrorDiv.html("<span>Supporting Document Data Not Available!</span>");
            sErrorDiv.show();
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function ajaxNppbkcInfo(url, id) {
    var param = {
        id: id
    };
    //skepNumber.html("");
    //$("#txt_SKEPNumber").val("");
    kppbc.val("");
    company.val("");
    companyId.val("");
    npwp.val("");
    $("#customloader").show();
    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(param),
        contentType: 'application/json; charset=utf-8',
        success: function (response) {            
            response = JSON.parse(response);
      
            var valid = false;
            if (response.Success) {
                var data = response.Data;
                kppbc.val(data.KppbcId);
                company.val(data.Company.Name);
                companyId.val(data.Company.Id);
                npwp.val(data.Company.Npwp);
                //      var year = new Date(submitDate.val()).getYear() + 1900;
                //console.log(new Date(submitDate.val()).getYear());

                //formatDocNumber(data.Company.Alias, data.CityAlias, new Date(submitDate.val()));
            }
        },
        error: function () {
            $("#customloader").hide();
        },
        complete: function () {            
            $("#customloader").hide();
        }
    });
}

function ajaxPlantInfo(urlPlant, nppbkcId) {
    $("#customloader").show();
    addressPlant.val("");
    $.ajax({
        url: urlPlant,
        type: 'POST',
        data: { nppbkcId: nppbkcId },
        success: function (response) {
            $("#customloader").hide();
            response = JSON.parse(response);
            if (response.Success) {
                var data = response.Data;
                addressPlant.val(data.Address);

            }
        },
        error: function () {
            $("#customloader").hide();
        }
    });
}

function Pad(num, size) {
    var s = "000000000" + num;
    return s.substr(s.length - size);
}

function GenerateItemRow(trItemIndex, request_no, fa_old, fa_old_Desc, fa_new, fa_new_desc, pd_detailId, company_name, hl_code, market_desc, plant)
{
    var item = "<tr class='tr_item_list' id='tr_item_list_" + trItemIndex + "' ";
    item += "data-index='" + trItemIndex + "'";
    item += "data-requestnumber='" + request_no + "'";
    item += "data-facodeold='" + fa_old + "'";
    item += "data-facodeolddesc='" + fa_old_Desc + "'";
    item += "data-facodenew='" + fa_new + "'";
    item += "data-facodenewdesc='" + fa_new_desc + "'";
    item += "data-pddetailid='" + pd_detailId + "'";
    item += ">";
    item += "<td class='td_inputan'>";
    item += "<input type='checkbox' class='cb_itemlist' />";
    item += "</td>";
    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + request_no + "</td>";
    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + fa_old + "</td>";
    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + fa_old_Desc + "</td>";
    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + fa_new + "</td>";
    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + fa_new_desc + "</td>";
    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + company_name + "</td>";
    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + hl_code + "</td>";
    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + market_desc + "</td>";
    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + plant + "</td>";
    item += "</tr>";
    return  item;
}

function GetItemList()
{
    $("#customloader").show();
    $("#tbody_optitemlist").empty();
    var ItemNotIn = [];    
    $("#tbody_skeplistitem").find("tr").each(function () {
        var ItemIndex = $(this).data("index");
        var ItemId = $("#txt_hd_itemId_" + ItemIndex).val();
        ItemNotIn.push(ItemId);        
    });
    $.ajax({
        url: getUrl("GetProductDevelopmentItemList"),
        type: 'POST',        
        data: JSON.stringify({ 'ItemNotIn': ItemNotIn }),
        async: false,
        contentType: 'application/json',
        success: function (data) {
            $("#tbody_optitemlist").empty();
            if(data != null)
            {
                var item = "";
                trItemIndex = $("#txt_index_listitem").val();
                for (var i = 0; i < data.length; i++) {
                    var _request_no = data[i].Request_No;
                    var _fa_old = data[i].Fa_Code_Old;
                    var _fa_old_Desc = data[i].Fa_Code_Old_Desc;
                    var _fa_new = data[i].Fa_Code_New;
                    var _fa_new_desc = data[i].Fa_Code_New_Desc;
                    var _pd_detailId = data[i].PD_DETAIL_ID;
                    var _company_name = data[i].Company.Name;
                    var _hl_code = data[i].Hl_Code;
                    var _market_desc = data[i].Market.Market_Desc;
                    var _plant = data[i].Werks;
                    item += GenerateItemRow(trItemIndex, _request_no, _fa_old, _fa_old_Desc, _fa_new, _fa_new_desc, _pd_detailId, _company_name, _hl_code, _market_desc, _plant);
                    trItemIndex++;
                }
                $("#tbody_optitemlist").append(item);
                $("#hd_islistitemloaded").val("1");
            }
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function ClearModalDetail()
{
    $("#txt_modal_detail_requestnumber").val("");
    $("#txt_modal_detail_brandname").val("");
    $("#txt_modal_detail_facodeold").val("");
    $("#txt_modal_detail_facodeolddesc").val("");
    $("#txt_modal_detail_facodenew").val("");
    $("#txt_modal_detail_facodenewdesc").val("");
    $("#txt_modal_detail_excisegoodtype").val("");
    $("#txt_modal_detail_companytier").val("");
    $("#txt_modal_detail_hjeperpack").val("");
    $("#txt_modal_detail_hjeperbatang").val("");
    $("#txt_modal_detail_content").val("");
    $("#txt_modal_detail_unit").val("");
    $("#txt_modal_detail_tariff").val("");
    $('.selectpicker').selectpicker('refresh');
}

function CountHJEperBatang()
{    
    var hje = NumberWithoutComma($("#txt_modal_detail_hjeperpack").val());    
    var content = NumberWithoutComma($("#txt_modal_detail_content").val());    
    var hjePerBatang = hje / content;    
    var result = NumberWithComma(hjePerBatang);
    $("#txt_modal_detail_hjeperbatang").val(result);
}

function SetTariff()
{    
    var hje = NumberWithoutComma($("#txt_modal_detail_hjeperpack").val());    
    var startDate = $("#ViewModel_Decree_StartDate").val();
    var GoodType = $("#txt_modal_detail_excisegoodtype").val();    
    $("#customloader").show();
    $.ajax({
        url: getUrl("GetTariff"),
        type: 'POST',
        data: {
            HJE: hje,
            StartDate: startDate,
            GoodType: GoodType
        },
        success: function (data) {
            $("#customloader").hide();
            $("#txt_modal_detail_tariff").val("0");            
            if (data != null)
            {
                $("#txt_modal_detail_tariff").val(NumberWithComma(data.attribute.Tariff));
            }            
        },
        error: function () {
            $("#customloader").hide();
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

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

function CheckIsFileNotExist(DivId, FileName) {
    var isNotExist = true;
    var td_list = $(DivId).find('.td_filename').each(function () {        
        if (FileName == $(this).data("filename")) {
            isNotExist = false;
        }
    });
    return isNotExist;
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

function AddRemovedFileToList(FileId) {
    if (FileId != 0 && FileId != "" && FileId != null && FileId != "0") {
        var html = "<input type='hidden' name='RemovedFilesId' value='" + FileId.toString() + "' />";
        $(".div_interviewrequest").append(html);
    }
}

function ChangeStatus(Action, Comment) {
    var ID = $("#txt_hd_id").val();
    $("#customloader").show();
    $.ajax({
        url: getUrl("ChangeStatus"),
        type: 'POST',
        data: {
            ReceiveID: ID,
            Action: Action,
            Comment: Comment
        },
        success: function (data) {
            var errmessage = data;
            if(data == "")
            {
                window.location.replace(getUrl(""));
            }
        }
    });
}

function CheckFieldEmpty() {
    var emptyCount = 0;
    $("#div_alert").empty();
    var arr = [];
    var message = "";
    if ($('#NppbkcSelector').val() == "") {
        emptyCount++;
        message += "* The NPPBKC field is required.<br/>";        
    }
    $(':input[required]:visible').each(function () {
        if ($(this).val() == "" || $(this).val() == null) {
            emptyCount++;
            message += "* The " + $(this).data("fieldname") + " field is required.<br/>";
            if (arr.indexOf(message) < 0) {
                arr.push(message);                
            }
        }
    });
    var detCount = 0;
    $('.tr_item_list:visible').each(function () {
        detCount++;
    });
    if (detCount == 0) {
        emptyCount++;
        message += "* At least 1 Item is required.<br/>";
    }
    var other_doc_count = 0;
    $("#div_otherDocsBody").find(".td_filename_number").each(function () {
        other_doc_count++;
    });
    if (other_doc_count == 0) {
        emptyCount++;
        message += "* At least 1 Attachment is required.<br/>";
    }
    var detailempty = 0;
    if (detCount > 0) {        
        $("#tbody_skeplistitem").find("tr").each(function () {            
            var _isactive = $(this).find(".txt_hd_isactive").val();
            if (_isactive == "true") {
                var txt_hd_brandname = $(this).find(".txt_hd_brandname").val();                
                var txt_hd_companytier = $(this).find(".txt_hd_companytier").val();
                var txt_hd_excisegood = $(this).find(".txt_hd_excisegood").val();
                var txt_hd_hjeperpack = $(this).find(".txt_hd_hjeperpack").val();
                var txt_hd_hjeperbatang = $(this).find(".txt_hd_hjeperbatang").val();
                var txt_hd_content = $(this).find(".txt_hd_content").val();
                var txt_hd_unit = $(this).find(".txt_hd_unit").val();
                var txt_hd_tariff = $(this).find(".txt_hd_tariff").val();
                if (txt_hd_brandname == "") {
                    detailempty++;
                }
                if (txt_hd_companytier == "" || txt_hd_companytier == "0") {
                    detailempty++;
                }
                if (txt_hd_excisegood == "") {
                    detailempty++;
                }
                if (txt_hd_hjeperbatang == "" || txt_hd_hjeperbatang == "0") {
                    detailempty++;
                }
                if (txt_hd_hjeperpack == "" || txt_hd_hjeperpack == "0") {
                    detailempty++;
                }
                if (txt_hd_content == "" || txt_hd_content == "0") {
                    detailempty++;
                }
                if (txt_hd_unit == "") {
                    detailempty++;
                }
                if (txt_hd_tariff == "" || txt_hd_tariff == "0") {
                    detailempty++;
                }
            }
        });
    }

    if (detailempty > 0)
    {
        emptyCount++;
        message += "* Please make sure product details are not empty.<br/>";
    }
    if (emptyCount == 0) {
        $("#div_alert").hide();
    }
    else {
        $("#div_alert").append(message);
        $("#div_alert").show();
    }
    return emptyCount;
}

function ToggleCheckAll() {    
    $("#tbody_skeplistitem").find(".tr_item_list").each(function () {        
        if ($(this).is(":visible")) {            
            var checkbox = $(this).find(".td_inputan").find(".cb_itemlist");
            if (checkbox.is(":checked") == true) {                
                checkbox.attr('checked', false);
                checkbox.prop("checked", false);
            }
            else {
                checkbox.attr('checked', true);
                checkbox.prop("checked", true);
            }
        }
    });
}

function DeleteCheckedItem() {
    $("#tbody_skeplistitem").find(".tr_item_list").each(function () {
        if ($(this).is(":visible")) {
            var checkbox = $(this).find(".td_inputan").find(".cb_itemlist");            
            if (checkbox.is(":checked") == true) {
                $(this).hide();
                $(this).find(".td_inputan").find(".txt_hd_itemId").val("-1");
            }
        }
    });
}

function CleaningUnUseFileOther() {
    $(".div_file_otherdoc").find('.txt_file').each(function () {
        var fileName = $(this).val().replace(/C:\\fakepath\\/i, '');
        fileName = fileName.replace(/^.*[\\\/]/, '')
        var filenameFromTable = "";
        var isAva = 0;
        $("#div_otherDocsBody").find(".td_filename").each(function () {
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

function GenerateItemInputHidden(indexForid, pddetailid, brandname, companytier, prodcode, hjepack, hjebatang, content, unit, tariff) {
    itemdetailindex = $("#txt_index_listitem").val();
    var item = "<input type='hidden' id='txt_hd_itemId_" + indexForid + "' class='txt_hd_itemId' value='" + pddetailid + "' name='Item[" + itemdetailindex + "].PD_Detail_ID'>";
    item += "<input type='hidden' class='txt_hd_detailId' value='0' name='Item[" + itemdetailindex + "].Received_Detail_ID'>";
    item += "<input type='hidden' class='txt_hd_isactive' value='true' name='Item[" + itemdetailindex + "].IsActive'>";
    item += "<input type='hidden' class='txt_hd_brandname' value='" + brandname + "' name='Item[" + itemdetailindex + "].Brand_CE'>";
    item += "<input type='hidden' class='txt_hd_companytier' value='" + companytier + "' name='Item[" + itemdetailindex + "].Company_Tier'>";
    item += "<input type='hidden' class='txt_hd_excisegood' value='" + prodcode + "' name='Item[" + itemdetailindex + "].Prod_Code'>";
    item += "<input type='hidden' class='txt_hd_hjeperpack' value='" + hjepack + "' name='Item[" + itemdetailindex + "].HJEperPack'>";
    item += "<input type='hidden' class='txt_hd_hjeperbatang' value='" + hjebatang + "' name='Item[" + itemdetailindex + "].HJEperBatang'>";
    item += "<input type='hidden' class='txt_hd_content' value='" + content + "' name='Item[" + itemdetailindex + "].Brand_Content'>";
    item += "<input type='hidden' class='txt_hd_unit' value='" + unit + "' name='Item[" + itemdetailindex + "].Unit'>";
    item += "<input type='hidden' class='txt_hd_tariff' value='" + tariff + "' name='Item[" + itemdetailindex + "].Tariff'>";
    itemdetailindex++;
    $("#txt_index_listitem").val(itemdetailindex);
    return item;
}

function GenerateImportItems()
{
    $("#customloader").show();
    $("#div_error_importitem").empty();
    $("#div_error_importitem").hide();
    var ItemNotIn = "";
    var urut = 1;
    $("#tbody_skeplistitem").find("tr").each(function () {
        var ItemIndex = $(this).data("index");
        var ItemId = $("#txt_hd_itemId_" + ItemIndex).val();
        if (urut > 1)
        {
            ItemNotIn += ",";
        }
        ItemNotIn += ItemId;
        urut++;
    });
    $("#txt_hditemnotin_import").val(ItemNotIn);
    var formData = new FormData($("#form_ImportItems")[0]);
    $.ajax({
        url: getUrl("ImportItems"),
        type: 'POST',
        data: formData,
        async: false,
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            var result = data.data;
            var err = data.attribute.ErrorMessage;
            if (err == "") {
                trItemIndex = $("#txt_index_listitem").val();                
                for (var i = 0; i < result.length; i++) {                    
                    var tr = GenerateItemRow(trItemIndex, result[i].Item.Request_No, result[i].Item.Fa_Code_Old, result[i].Item.Fa_Code_Old_Desc, result[i].Item.Fa_Code_New, result[i].Item.Fa_Code_New_Desc, result[i].Item.PD_DETAIL_ID, result[i].Item.Company.Name, result[i].Item.Hl_Code, result[i].Item.Market.Market_Desc, result[i].Item.Werks);
                    $("#tbody_skeplistitem").append(tr);
                    var hd = GenerateItemInputHidden(trItemIndex, result[i].Item.PD_DETAIL_ID, result[i].Brand_CE, result[i].Company_Tier, result[i].Prod_Code, result[i].HJEperPack, result[i].HJEperBatang, result[i].Brand_Content, result[i].Unit, result[i].Tariff);                    
                    $("#tr_item_list_" + trItemIndex).find(".td_inputan").append(hd);
                    trItemIndex++;
                }
            }
            else {
                $("#div_error_importitem").show();
                $("#div_error_importitem").html(err);
            }
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function CleaningComma()
{
    $("#tbody_skeplistitem").find("tr").each(function () {
        var _isactive = $(this).find(".txt_hd_isactive").val();
        if (_isactive == "true") {                                    
            var txt_hd_hjeperpack = $(this).find(".txt_hd_hjeperpack").val();
            var txt_hd_hjeperbatang = $(this).find(".txt_hd_hjeperbatang").val();
            var txt_hd_content = $(this).find(".txt_hd_content").val();            
            var txt_hd_tariff = $(this).find(".txt_hd_tariff").val();
            $(this).find(".txt_hd_hjeperpack").val(NumberWithoutComma(txt_hd_hjeperpack));
            $(this).find(".txt_hd_hjeperbatang").val(NumberWithoutComma(txt_hd_hjeperbatang));
            $(this).find(".txt_hd_content").val(NumberWithoutComma(txt_hd_content));
            $(this).find(".txt_hd_tariff").val(NumberWithoutComma(txt_hd_tariff));
        }
    });
}

function NumberWithComma(number)
{    
    return number.toString().replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

function NumberWithoutComma(number)
{
    return number.replace(/\./g, '');
}

///////////////////////////////////////////////////////////////////////////////////

$(document).on("click", "#btn_myModalListItem", function () {
    //if ($("#hd_islistitemloaded").val() == "0") {
        GetItemList();
    //}
});

$(document).on("click", "#btn_addItem", function () {
    $("#div_error_importitem").empty();
    $("#div_error_importitem").hide();
    $("#tbody_optitemlist").find("tr").each(function () {
        if ($(this).find(".cb_itemlist").is(":checked")) {
            var pddetailid = $(this).data("pddetailid");
            var index = $(this).data("index");
            var item = GenerateItemInputHidden(index, pddetailid, "", "", "", "", "", "", "", "");
            $(this).find(".td_inputan").append(item);
            $(this).appendTo("#tbody_skeplistitem");            
        }
    });
});

$(document).on("click", ".btn_showmodal_detail", function () {
    var tr = $(this).closest("tr");
    if ($(this).closest("tbody").attr("id") != "tbody_optitemlist") {
        ClearModalDetail();
        var index = tr.data("index");
        var requestnumber = tr.data("requestnumber");
        var facodeold = tr.data("facodeold");
        var facodeolddesc = tr.data("facodeolddesc");
        var facodenew = tr.data("facodenew");
        var facodenewdesc = tr.data("facodenewdesc");
        var companytier = tr.find(".txt_hd_companytier").val();
        var excisegood = tr.find(".txt_hd_excisegood").val();        
        var hjeperpack = tr.find(".txt_hd_hjeperpack").val();        
        hjeperpack = NumberWithComma(NumberWithoutComma(hjeperpack));        
        var hjeperbatang = tr.find(".txt_hd_hjeperbatang").val();
        hjeperbatang = NumberWithComma(NumberWithoutComma(hjeperbatang));        
        var content = tr.find(".txt_hd_content").val();
        content = NumberWithComma(NumberWithoutComma(content));        
        var unit = tr.find(".txt_hd_unit").val();
        var strTariff = tr.find(".txt_hd_tariff").val();
        var tariff = 0;        
        if (strTariff != "")
        {
            strTariff = NumberWithoutComma(strTariff);            
            tariff = parseInt(strTariff);            
        }
        tariff = NumberWithComma(tariff);
        var brandname = tr.find(".txt_hd_brandname").val();
        $("#txt_hd_index").val(index);
        $("#txt_modal_detail_requestnumber").val(requestnumber);
        $("#txt_modal_detail_facodeold").val(facodeold);
        $("#txt_modal_detail_facodeolddesc").val(facodeolddesc);
        $("#txt_modal_detail_facodenew").val(facodenew);
        $("#txt_modal_detail_facodenewdesc").val(facodenewdesc);
        $("#txt_modal_detail_companytier").val(companytier);
        $("#txt_modal_detail_excisegoodtype").val(excisegood);
        $("#txt_modal_detail_hjeperpack").val(hjeperpack);
        $("#txt_modal_detail_hjeperbatang").val(hjeperbatang);
        $("#txt_modal_detail_content").val(content);
        $("#txt_modal_detail_unit").val(unit);
        $("#txt_modal_detail_tariff").val(tariff);
        $("#txt_modal_detail_brandname").val(brandname);
        $('.selectpicker').selectpicker('refresh');
        $("#myModalDetailItem").modal();
    }
    else
    {
        var cb = tr.find(".cb_itemlist");
        if(cb.is(':checked'))
        {
            cb.prop('checked', false);
        }
        else
        {
            cb.prop('checked', true);
        }
    }
});

$(document).on("click", "#btn_saveDetailItem", function () {
    var brandname = $("#txt_modal_detail_brandname").val();
    var companytier = $("#txt_modal_detail_companytier").val();
    var excisegood = $("#txt_modal_detail_excisegoodtype").val();
    var hjeperpack = $("#txt_modal_detail_hjeperpack").val();
    var hjeperbatang = $("#txt_modal_detail_hjeperbatang").val();    
    var content = $("#txt_modal_detail_content").val();
    var unit = $("#txt_modal_detail_unit").val();
    var tariff = $("#txt_modal_detail_tariff").val();
    var index = $("#txt_hd_index").val();
    var tr = $("#tr_item_list_" + index);
    tr.find(".txt_hd_brandname").val(brandname);
    tr.find(".txt_hd_companytier").val(companytier);
    tr.find(".txt_hd_excisegood").val(excisegood);
    tr.find(".txt_hd_hjeperpack").val(hjeperpack);
    tr.find(".txt_hd_hjeperbatang").val(hjeperbatang);
    tr.find(".txt_hd_content").val(content);
    tr.find(".txt_hd_unit").val(unit);
    tr.find(".txt_hd_tariff").val(tariff);
});

$(document).on("change", ".change_hjeperbatang", function () {
    CountHJEperBatang();
});

$(document).on("change", ".change_tariff", function () {
    SetTariff();
});

$(document).on("change", ".txt_file", function () {
    var id = $(this).attr("id");
    var filename_index = $(this).data("index");
    $("#hd_toggle_otherdoc").val(filename_index);
    var filesize = GetFileSize(id);
    if (filesize != null && filesize != undefined && filesize > 0) {
        var maxsize = parseInt($("#txt_hd_filesize").val());
        var isSupport = $(this).hasClass("supportingdocument");
        if (filesize > maxsize && !isSupport) {
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
    var htmlInput = "<input id='txt_otherdoc_other_" + HdDocIndex + "' type='file' name='File_Other' class='hidden txt_file' accept='application/pdf' data-index='other_" + HdDocIndex + "' />";
    htmlInput += "<input type='hidden' name='File_Other_Name' id='txt_otherdoc_filename_other_" + HdDocIndex + "' />";
    div_parent.append(htmlInput);
    $("#txt_otherdoc_other_" + HdDocIndex).click();
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
            else {
                alert("File is already added");
                $("#txt_otherdoc_" + txtfilenameindex).remove();
                txtfilename.remove();
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

$(document).on("click", ".btn_browsesupportdoc_file", function () {
    var index = $(this).data("index");
    $("#txt_supportdoc_file" + index).click();
});

$(document).on("click", ".btn_remove_supportdoc", function () {
    var fileuploadid = $(this).data("fileuploadid");
    var parent = $(this).closest(".div_file");
    parent.find(".div_file_href").remove();
    parent.find(".div_supportdoc_name").show();
    parent.find(".span_browsesupportdoc").show();
    AddRemovedFileToList(fileuploadid);
});

$(document).on("click", "#SubmitButtonConfirm", function () {
    if (CheckFieldEmpty() == 0) {
        CleaningComma();
        $("#txt_hd_action").val("submit");
        $("#SavePenetapanSkepForms").submit();
    }
});

$(document).on("click", "#ReviseButton", function () {
    var comment = $("#txt_revise_comment").val();
    if (comment != "") {
        ChangeStatus("revise", comment);
    }
    else
    {
        $("#div_alert_revise").show();
        $("#div_alert_revise").html("* Note/Comment cannot be empty.");
    }
});

$(document).on("click", "#ApproveButtonConfirm", function () {    
    ChangeStatus("approve", "");
});

$(document).on("click", "#CancelButtonConfirm", function () {
    ChangeStatus("cancel", "");
});

$(document).on("click", "#SaveButton", function () {
    $("#customloader").show();
    if (CheckFieldEmpty() == 0) {
        CleaningUnUseFileOther();
        CleaningComma();
        $("#SavePenetapanSkepForms").submit();
    } else {
        $("#customloader").hide();
    }
});

$(document).on("click", "#checkallitem", function () {
    ToggleCheckAll();
});

$(document).on("click", "#delItem", function () {
    DeleteCheckedItem();
});

$(document).on('keydown', '.numeric-form', function (e) {
    { -1 !== $.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) || /65|67|86|88/.test(e.keyCode) && (!0 === e.ctrlKey || !0 === e.metaKey) || 35 <= e.keyCode && 40 >= e.keyCode || (e.shiftKey || 48 > e.keyCode || 57 < e.keyCode) && (96 > e.keyCode || 105 < e.keyCode) && e.preventDefault() }    
});

$(document).on("keyup", ".numeric-form", function () {
    var number = NumberWithComma($(this).val());
    $(this).val(number);
});

$(document).on("click", "#btn_showimport_dialog", function (e) {
    $('#txt_FileImport').replaceWith($('#txt_FileImport').clone());
    $("#txt_startdate_import").val("");    
});

$(document).on('click', '#btn_startimport', function (e) {
    CloseAllModal();
    $("#txt_startdate_import").val($("#ViewModel_Decree_StartDate").val());
    GenerateImportItems();
});