var requestNo = $("#brandRequestNo");
var pdNumber = $("#PdNumber");
var fileUploadLimitElm = $("#FileUploadLimit");
var docsContainer = $("#SupportingDocs");
var saveButton = $("#SaveButton");
var sErrorDiv = $("#SupportingError");
var otherDocs = [];
var otherDocsName = [];
var supportingDocs = {};
var createForm = $("#ProductDevCreateForm");
var productFormData = {};
var tempAction =$("#TempAction");
var arrayItem = [];
var arrayItemSave = [];
var arrayDetail = [];
var tempItem = [];
var productFormData = {};
var productData = {};
var editButton = $("#EditButton");
var collectOtherDocs = [];
var checkerElm = idToElement("CheckExistanceLabel");
var radioElm = idToElement("CheckRadioLabel");
var listElm = idToElement("CheckListLabel");
var faOldElm = idToElement("CheckFaOldLabel");
var faNewElm = idToElement("CheckFaNewLabel");
var plantElm = idToElement("CheckPlantLabel");
var hlElm = idToElement("CheckHlLabel");
var existSuppDocElm = idToElement("CheckExistSuppDocLabel");
var otherDocElm = idToElement("CheckOtherDocLabel");
var checkModalExcelElm  =idToElement("CheckUploadExcelLabel");
//var existingOtherDocs = {};
//var newOtherDocs = {};
//var newOtherDocsName = {};
//var otherDocMap = {};
//var otherDocMapName = {};

$(document).on('click', '#btn_startimport', function (e) {
   
    console.log("generate");
    var fileName = $('[name="FileImport"]').val().trim();
    var pos = fileName.lastIndexOf('.');
    var extension = (pos <= 0) ? '' : fileName.substring(pos);
    if (extension != '.xlsx' && extension != '.xls') {
        //alert('Please choose or browse a correct Excel File to upload.');     
        checkModalExcelElm.html("<span>Please choose or browse a correct Excel File to upload.</span>");
        checkModalExcelElm.show();
    }
    else {
        checkModalExcelElm.hide();
        CloseAllModal();
    }
   GenerateImportItems();
});

function GenerateImportItems() {
    
    //$("#customloader").show();
    $("#div_error_importitem").empty();
    $("#div_error_importitem").hide();
    var ItemNotIn = "";
    var urut = 1;
    $("#tbody_productlistitem").find("tr").each(function () {
        var ItemIndex = $(this).data("index");
        var ItemId = $("#txt_hd_itemId_" + ItemIndex).val();
        if (urut > 1) {
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
                    var tr = GenerateItemRow(trItemIndex, result[i].Item.Request_No, result[i].Item.Approval_Status, result[i].Item.Bukrs, result[i].Item.Fa_Code_Old, result[i].Item.Fa_Code_Old_Desc, result[i].Item.Fa_Code_New, result[i].Item.Fa_Code_New_Desc, result[i].Item.Hl_Code, result[i].Item.Market_Id, result[i].Item.Werks, result[i].Item.Is_Import);
                    $("#tbody_productlistitem").append(tr);
                    //var hd = GenerateItemInputHidden(trItemIndex, result[i].Item.PD_DETAIL_ID, result[i].Brand_Ce, result[i].Company_Tier, result[i].Prod_Code, result[i].HJEperPack, result[i].HJEperBatang, result[i].Brand_Content, result[i].Unit, result[i].Tariff, result[i].Packaging_Material, result[i].Item.Market.market_desc, result[i].Front_Side, result[i].Back_Side, result[i].Left_Side, result[i].Right_Side, result[i].Top_Side, result[i].Bottom_Side);
                    //$("#tr_item_list_" + trItemIndex).find(".td_inputan").append(hd);
                    //trItemIndex++;
                }
            }
            else {
                //$("#div_error_importitem").show();
                //$("#div_error_importitem").html(err);
            }
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function GenerateItemRow(trItemIndex, request_no, approval_status, company, fa_old, fa_old_Desc, fa_new, fa_new_desc, hl_code, market, plant, is_import) {
    var item = "<tr class='tr_item_list' id='tr_item_list_" + trItemIndex + "' ";
    item += "data-index='" + trItemIndex + "'";
    item += "data-requestnumber='" + request_no + "'";
    item += "data-facodeold='" + fa_old + "'";
    item += "data-facodeolddesc='" + fa_old_Desc + "'";
    item += "data-facodenew='" + fa_new + "'";
    item += "data-facodenewdesc='" + fa_new_desc + "'";
    item += "data-pddetailid='" + pd_detailId + "'";
    item += "data-market='" + market_desc + "'";
    item += "data-proddevnextaction='" + registrationType + "'";
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
    return item;
}


function GenerateItemInputHidden(indexForid, pddetailid, brandname, companytier, prodcode, hjepack, hjebatang, content, unit, tariff, packaging_material, market, front_side, back_side, left_side, right_side, top_side, bottom_side) {
    itemdetailindex = $("#txt_index_listitem").val();
    var item = "<input type='hidden' class='txt_hd_itemId' value='" + pddetailid + "' name='Item[" + itemdetailindex + "].PD_Detail_ID'>";
    item += "<input type='hidden' class='txt_hd_detailId' value='0' name='Item[" + itemdetailindex + "].Registration_Detail_ID'>";
    item += "<input type='hidden' class='txt_hd_isactive' value='true' name='Item[" + itemdetailindex + "].IsActive'>";
    item += "<input type='hidden' class='txt_hd_brandname' value='" + brandname + "' name='Item[" + itemdetailindex + "].Brand_CE'>";
    item += "<input type='hidden' class='txt_hd_latest_skep_no' value='' name='Item[" + itemdetailindex + "].Latest_Skep_No'>";
    item += "<input type='hidden' class='txt_hd_companytier' value='" + companytier + "' name='Item[" + itemdetailindex + "].Company_Tier'>";
    item += "<input type='hidden' class='txt_hd_excisegood' value='" + prodcode + "' name='Item[" + itemdetailindex + "].Prod_Code'>";
    item += "<input type='hidden' class='txt_hd_hjeperpack' value='" + hjepack + "' name='Item[" + itemdetailindex + "].Hje'>";
    item += "<input type='hidden' class='txt_hd_hjeperbatang' value='" + hjebatang + "' name='Item[" + itemdetailindex + "].HJEperBatang'>";
    item += "<input type='hidden' class='txt_hd_content' value='" + content + "' name='Item[" + itemdetailindex + "].Brand_Content'>";
    item += "<input type='hidden' class='txt_hd_unit' value='" + unit + "' name='Item[" + itemdetailindex + "].Unit'>";
    item += "<input type='hidden' class='txt_hd_tariff' value='" + tariff + "' name='Item[" + itemdetailindex + "].Tariff'>";
    item += "<input type='hidden' class='txt_hd_packaging_material' value='" + packaging_material + "' name='Item[" + itemdetailindex + "].Packaging_Material'>";
    item += "<input type='hidden' class='txt_hd_market' value='" + market + "' name='Item[" + itemdetailindex + "].Market'>";
    item += "<input type='hidden' class='txt_hd_front_side' value='" + front_side + "' name='Item[" + itemdetailindex + "].Front_Side'>";
    item += "<input type='hidden' class='txt_hd_back_side' value='" + back_side + "' name='Item[" + itemdetailindex + "].Back_Side'>";
    item += "<input type='hidden' class='txt_hd_left_side' value='" + left_side + "' name='Item[" + itemdetailindex + "].Left_Side'>";
    item += "<input type='hidden' class='txt_hd_right_side' value='" + right_side + "' name='Item[" + itemdetailindex + "].Right_Side'>";
    item += "<input type='hidden' class='txt_hd_top_side' value='" + top_side + "' name='Item[" + itemdetailindex + "].Top_Side'>";
    item += "<input type='hidden' class='txt_hd_bottom_side' value='" + bottom_side + "' name='Item[" + itemdetailindex + "].Bottom_Side'>";

    itemdetailindex++;
    $("#txt_index_listitem").val(itemdetailindex);
    return item;
}

function CloseAllModal() {
    $('.ems-modal').modal('hide');
}

function prodGenerateClick(url)
{
    console.log("generate");
    var fileName = $('[name="ProdExcelfile"]').val().trim();
    var pos = fileName.lastIndexOf('.');
    var extension = (pos <= 0) ? '' : fileName.substring(pos);
    if (extension != '.xlsx' && extension != '.xls') {
        alert('Please browse a correct Excel File to upload.');
        return false;
    }
    var formData = new FormData();
    var totalFiles = document.getElementById("ProdExcelfile").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("ProdExcelfile").files[i];
        formData.append("prodExcelfile", file);
    }
    console.log(formData);
    //$.ajax({
    //    url: url,
    //    type: 'POST',
    //    data: formData,
    //    cache: false,
    //    dataType: 'html',
    //    processData: false, // Don't process the files
    //    contentType: false, // Set content type to false as jQuery will tell the server its a query string request
    //    success: function (response) {
    //        console.log("excel here");
    //        $('#contenttableproduct').html("");
    //        $('#contenttableproduct').html(response);
    //        //if (IsProdPlanValid()) {
    //        //    //valid generated
    //        //    $('#prod-plan-save').removeAttr('disabled');
    //        //} else {
    //        //    //invalid generated
    //        //    $('#prod-plan-save').attr('disabled', 'disabled');
    //        //}
    //        //changeToDecimal('#ProdPlanContent .decimal', 'html');
    //    },
    //    error: function (error) {
    //        // Handle errors here
    //        console.log('ERRORS: ' + error);
    //        // STOP LOADING SPINNER
    //    }
    //});
    //return false;
}

function idToElement(id) {
    return $("#" + id);
}

function attach() {
    $('#MenuProductDevelopment').addClass('active');    
    $("#addOtherDocBtn").on("click", addOtherDoc);
    handleBrowseEvent("browseOtherDoc", "browseOtherDocFile", "browseOtherDocText");
    
  //  init();  
}

function detach() {     
    $("#addOtherDocBtn").off("click", addOtherDoc);    
}

function createDetail(url, pdID) {   
    console.log(pdID);
 
        var detail = arrayItemSave;
        var data = new FormData();        
        $.ajax({
            type: 'POST',
            url: url,
            data: {
                model: JSON.stringify(arrayItemSave),
                pdID: Number(pdID)
            },
            success: function (result) {              
                if (result) {                   
                    console.log("Detail Saved.");
                } else {
                    showErrorDialog("Save Detail failed!");
                    saveButton.prop("disabled", false);
                    $("#customloader").hide();
                }
            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;
                showErrorDialog(err);
                $("#customloader").hide();
            }
        });    
 
}

function addOtherDoc() {
    
    var input = document.getElementById("browseOtherDocFile");
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
            otherDocElm.html("<span>File size is larger than allowed size. Allowed size :" + sizeMB + " MB </span>");
            otherDocElm.show();
            showErrorDialog("File size is larger than allowed size (" + sizeMB + " MB)");
            return;
        }
        if (file.size < allowedFilesize) {
            otherDocElm.hide();
        }

        addToList(file, $("#browseOtherDocFileName").val());
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
    if(!name){
        showErrorDialog("Document name is required.");
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
       
        //var temp1 = file.replace(/(\.[\w\d_-]+)$/i, '_rename$1');
        //var str = file.name;
        //var temp2 = str.substring(0, str.lastIndexOf(".")) + "_rename" + str.substring(str.lastIndexOf("."));
        //file.name = temp2;
        //console.log("file name ori: " + file.name);
      
        //console.log("rename : " + file.name);

        renderFileList();
    }
}

function renderFileList() {
    var container = $("#otherDocsBody");
    var content = '';
    if (!otherDocs || otherDocs.length <= 0) {
        content += '<tr id="noOtherFileMsg">' +
                    '<td colspan="4"><div class="alert alert-info">No Additional Documents</div>' + '</td></tr>';
    }
    for (var i = 0; i < otherDocs.length; i++) {
        content +=
            '<tr>' +
            '<td>' + (i + 1) + '</td>' +
            '<td>' + otherDocs[i].name +
            '<td>' + otherDocsName[i] + '</td>' +
            '<td><button class="btn btn-primary btn-sm" onclick="removeFromList(' + i + ')">Remove</button></td>' +
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

function showErrorDialog(msg) {
    $("#errModalTitle").html("REQUEST FAILED");
    $("#errModalContent").html(msg);
    $("#errModal").modal("show");
}

function loadSupportingDocuments(url, company) {
    //var param = {
    //    formId: 2, bukrs: code
    //};
    $("#customloader").show();
    $.ajax({
        url: url,
        type: 'POST',
        data: {
            company: company
        },
    })
    .success(function (partialResult) {
        $("#customloader").hide();
        docsContainer.html(partialResult);
        sErrorDiv.hide();
      //  saveButton.prop("disabled", false);
   
    })
    .error(function (error) {
        $("#customloader").hide();        
        sErrorDiv.html("<span>Supporting Document Data Not Available!</span>");
        sErrorDiv.show();
      //  saveButton.prop("disabled", true);
       
    });
}

function getValuePDNumber()
{
    var res = pdNumber.val();    
    return res;
}

function ajaxLoadNppbkcData( urlNppbkc, nppbkcId)
{
    console.log(nppbkcId);
    $.ajax({
        url: urlNppbkc,
        type: 'POST',
        data: { id: nppbkcId },
        success: function (response) {
            $("#customloader").hide();
            response = JSON.parse(response);
            if (response.Success) {
                var data = response.Data;
                var cityAlias = data.CityAlias;
                var nppbkcId = data.NppbkcId;
                //FormatDocNumber(countUrl, data.Company.Alias, data.CityAlias);
                console.log(data);
                FormatDocNumberPartial(data.Company.Alias, data.CityAlias);
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

function UpdateDocumentNumbering(importCode) {   
    var reqNumberChange = requestNo.val();    
    var segments = reqNumberChange.split("/");
    segments[1] = importCode;  
    requestNo.val(segments.join("/"));
}

//function FormatDocNumber(nppbkcId, cityAlias) {
//    var date = new Date(); 
//    var months = ["I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII"];
//    var month = date.getMonth(); 
//    var year = date.getYear() + 1900; 
//    var count = 0;
    
//    $("#customloader").show();
//    $.ajax({
//        url: countUrl,
//        type: 'POST',
//        contentType: 'application/json; charset=utf-8',
//        success: function (response) {
//            $("#customloader").hide();         
//            count = parseInt(response) + 1;                                
//            requestNo.val(Pad(count, 10) + "/" + nppbkcId + "/" + cityAlias + "/" + months[month] + "/" + year);
//        },
//        error: function () {
//            $("#customloader").hide();
//        }
//    });
//}

function ajaxFaCodeOldDescription(urlOldDescription, type) {
    $.ajax({
        url: urlOldDescription,
        type: 'POST',
        data: { code: type },
        success: function (data) {
            if (data != null) {
                document.getElementById("FaCodeOldDesc").value = data;
            }
            else {
                document.getElementById("FaCodeOldDesc").value = "";
            }
        }
    });
}

function ajaxFaCodeNewDescription(urlNewDescription, type) {
    $.ajax({
        url: urlNewDescription,
        type: 'POST',
        data: { code: type },
        success: function (data) {
            if (data != null) {
                document.getElementById("FaCodeNewDesc").value = data;
            }
            else {
                document.getElementById("FaCodeNewDesc").value = "";
            }
        }
    });
}
     
function ajaxGetPlant(urlPlantNonImport, type) {
    $.ajax({
        url: urlPlantNonImport,
        type: 'POST',
        data: { bukrs: type },
        success: function (data) {                 
            for (var i = 0; i < data.length; i++) {
                $('#PlantId').append('<option value=' + data[i].Value + '>' + data[i].Text + '</option>');
            }
        }
    });
}

function ajaxGetPlantImport(urlPlantImport, type) {
    $.ajax({
        url: urlPlantImport,
        type: 'POST',
        data: { bukrs: type },
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#PlantId').append('<option value=' + data[i].Value + '>' +  data[i].Text + '</option>');         
            }         
        }
    });
}

function setfilename(val) {
    var fileName = val.substr(val.lastIndexOf("\\") + 1, val.length);
    document.getElementById("mytext[]").value = fileName;
}


function ValidateItem() {
    console.log("Validate Item Here");       
   
    var validCompany = $("#CompanySelector :selected").text();
    var validPlant = $("#PlantId option:selected").text(); // Select
    var validFaOld = $("#FaCodeOldSelector :selected").text(); // Select
    var validFaNew = $("#FaCodeNewSelector :selected").text(); // Select
    var validFaOldDesc = $("#FaCodeOldDesc").val();
    var validFaNewDesc = $("#FaCodeNewDesc").val();
    var validMarket = $("#MarketSelector option:selected").text();
    var validHl = $("#hlCode").val();
    var supportDocsInputs = $("form#ProductDevCreateForm input[type=file]");

    if (!productFormValid) {
        alert("Supporting Document are required.");
        showErrorDialog("Supporting documents are required.");
        existSuppDocElm.html("<span>Supporting Document are required.</span>");
        existSuppDocElm.show();
    }
    else {
        existSuppDocElm.hide();
    }
    if (validPlant == "Select") {
        console.log("Select Plant first")
        plantElm.html("<span>Plant not Selected.</span>");
        plantElm.show();
    }
    else {
        plantElm.hide();
    }

    //if (validFaOld == "Select" ) {
    //    console.log("Fa Code Old not Selected");
    //    faOldElm.html("<span>Fa Code Old not Selected.</span>");
    //    faOldElm.show();
    //}
    //else {
    //    faOldElm.hide();
    //}
     
    //if (validFaNew == "Select") {
    //    console.log("Fa Code New not Selected");
    //    faNewElm.html("<span>Fa Code New not Selected.</span>");
    //    faNewElm.show();
    //}
    //else {
    //    faNewElm.hide();
    //}

    if (validHl == "") {
        console.log("Hl Code empty");
        hlElm.html("<span>Hl Code is Empty.</span>");
        hlElm.show();        
    }
    else {
        hlElm.hide();
    }

  
    //if (supportingDocs == {} || supportingDocs == null) {
    //    console.log("supp doc check :" + supportingDocs);
    //    existSuppDocElm.html("<span>Supporting Document may not be empty.</span>");
    //    existSuppDocElm.show();
    //}
    //else{
    //      existSuppDocElm.hide();
    //}

    //for (var key in supportingDocs) {
    //    if (supportingDocs.hasOwnProperty(key)) {
    //        if (supportingDocs[key] == "" || supportingDocs[key] == null || supportingDocs[key] == "undefined") {
    //            existSuppDocElm.html("<span>Supporting Document may not be empty.</span>");
    //            existSuppDocElm.show();
    //        }
    //        if (supportingDocs[key] != "") {
    //            existSuppDocElm.hide();
    //         //   data.append(key, supportingDocs[key], supportingDocs[key].name);
    //        }
    //    }
    //    else {
    //        console.log("supp doc empty");
    //    }
    //}

    if (validHl != "" &&  validPlant != "Select") {
        $('#myModalItem').modal('hide');
        return true;
    }
}

$('#hlCode').on('input', function (e) {
    var validHl = $("#hlCode").val();
    if (validHl == "") {
        hlElm.html("<span>Hl Code is Empty.</span>");
        hlElm.show();
    }
    else {
        hlElm.hide();
    }

});

$("#addItem").click(function () {
    listElm.hide();
    var checkValidItem = ValidateItem();

    if (checkValidItem == true) {
        console.log("item field valid");
        //console.log('success');
        
        var brandReqNoItem = $("#brandRequestNo").val();
        var companyItem = $("#CompanySelector :selected").text();
        var faCodeOldItem = $("#FaCodeOldSelector :selected").text();
        var faCodeOldDescItem = $("#FaCodeOldDesc").val();
        var faCodeNewItem = $("#FaCodeNewSelector :selected").text();
        var faCodeNewDescItem = $("#FaCodeNewDesc").val();
        var hlCodeItem = $("#hlCode").val();
        var marketItem = $("#MarketSelector option:selected").text();    
        var plantItem = $("#PlantId option:selected").text();
        var isImportItem = document.getElementById("isImport").checked;
        console.log("is import: " + isImportItem);

        var companyItemVal = $("#CompanySelector :selected").val();
        var marketItemVal = $("#MarketSelector option:selected").val();

        var tempItemPlant = $("#PlantId option:selected").val();
        var plantItemVal = ajaxPlantInfo(tempItemPlant);

        var objItem = { brandReqNoItem: brandReqNoItem, companyItem: companyItem, faCodeOldItem: faCodeOldItem, faCodeOldDescItem: faCodeOldDescItem, faCodeNewItem: faCodeNewItem, faCodeNewDescItem: faCodeNewDescItem, hlCodeItem: hlCodeItem, marketItem: marketItem, plantItem: plantItem, isImport: isImportItem  };
        var objItemSave = { brandReqNoItem: brandReqNoItem, companyItem: companyItemVal, faCodeOldItem: faCodeOldItem, faCodeOldDescItem: faCodeOldDescItem, faCodeNewItem: faCodeNewItem, faCodeNewDescItem: faCodeNewDescItem, hlCodeItem: hlCodeItem, marketItem: marketItemVal, plantItem: plantItemVal, isImport: isImportItem };

        uploads();

        arrayItem.push(objItem); // this array accomodated for list view
        arrayItemSave.push(objItemSave); // this array accomodated for saving in table - database                                            
        console.log(arrayItemSave);

        CreateItemList();  
    }
 
});
   

function CreateItemList()
{
    var localCounter = 0;
    //create counter here from last number product detail and generate complete number detail
    localCounter = GetLastNumberItem();
      
    $('#brandTable tbody tr').remove();
    for (var i = 0; i < arrayItem.length; i++) {
        var checkImport = '';
        if (arrayItem[i].isImport == true) {
            checkImport = '<input type="checkbox" checked disabled="disabled" />';
        }
        else {
            checkImport = '<input type="checkbox" disabled="disabled" />';
        }

        var newRow = '<tr><td class="action"><input type="checkbox" onclick="stateCheckItem(' + i + ')" id="checkAll' + i + '" /></td>'+
            i + '<td> <a data-toggle="modal" data-placement="top" title="Edit"  data-id="' + i + '" class="itemView"> <i class="fa fa-pencil-square-o"></i></a></td> ' +
            i + '<td><label id="status">DRAFT NEW</label></td>' + 
            i + '<td>' + checkImport + '</td>' +
            i + '<td nowrap><label id="brandReqNo' + i + '">' + Pad(localCounter + i, 10) + "/" + arrayItem[i].brandReqNoItem + '</label></td>' +
            i + '<td nowrap><label id="companyInf">' + arrayItem[i].companyItem + '</label></td>' +
            i + '<td><label id="faCodeOld">' + arrayItem[i].faCodeOldItem + '</label></td>' +
            i + '<td nowrap><label id="faCodeOldDesc">' + arrayItem[i].faCodeOldDescItem + '</label></td>' +
            i + '<td><label id="faCodeNew">' + arrayItem[i].faCodeNewItem + '</label></td>' +
            i + '<td nowrap><label id="faCodeNewDesc">' + arrayItem[i].faCodeNewDescItem + '</label></td>' +
            i + '<td><label id="hlCode">' + arrayItem[i].hlCodeItem + '</label></td>' +
            i + '<td><label id="marketInf">' + arrayItem[i].marketItem + '</label></td>' +
            i + '<td nowrap><label id="plantInf">' + arrayItem[i].plantItem + '</label></td></tr>' ;
        $('#brandTable tbody').append(newRow);
     
    }

}


function CheckAllItem() {
    console.log("length: "+ arrayItem.length);
    for (var i = 0; i < arrayItem.length; i++) {
        $('#checkAll' + i + '').prop('checked', true);
        
    }
}


//function DefaultTable() {
//    var defaultState = '<tr><td colspan="13" style="text-align:center"><label>No Item Added.</label></td></tr>';
//    $('#brandTable tbody').append(defaultState);
//}

function DeleteItem() {
  
    for (var i = arrayItem.length; i--;) {
        if ($('#checkAll' + i + '').is(':checked')) {           
            arrayItem.splice(i, 1);                    
         }   
    }
    CreateItemList();    
}



function stateCheckItem(i)
{
  //  alert(JSON.stringify(arrayItem[i]));
}

