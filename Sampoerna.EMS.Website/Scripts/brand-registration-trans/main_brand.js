var docsContainer = $("#SupportingDocs");
var brNumber = $("#BrNumber");
var regNumber = $("#RegNumber");
var kppbc = $("#KppbcText");
var nppbkc = $("#NppbkcSelector");
var company = $("#CompanyText");
var companyId = $("#NPPBKC_Company_Id");
var companyAlias = $("#ViewModel_Company_Alias");
var companyCity = $("#ViewModel_Company_City");

var npwp = $("#NpwpText");
var addressPlant = $("#AddressPlant");
var createForm = $("#BrandCreateForm");
var otherDocs = [];
var supportingDocs = [];
var sErrorDiv = $("#SupportingError");
var submitDate = $("#ViewModel_Submission_Date");
var effectiveDate = $("#ViewModel_Effective_Date");
var saveButton = $("#SaveButton");
var brandFormData = {};

var trItemIndex = 0;
var itemdetailindex = 0;
var BAdocIndex = 0;
var HdDocIndex = 0;


function populateFormData() {
    console.log(submitDate.val());
    console.log(new Date(submitDate.val()));
    brandFormData = {
        Registration_Type: parseInt($("#TempAction").val()),
        Registration_No: brNumber.val(),
        Nppbkc_ID: nppbkc.val(),
        Submission_Date: submitDate.val(),
        Effective_Date: effectiveDate.val()
    };
    console.log(JSON.stringify(brandFormData));
}

function showErrorDialog(msg) {
    $("#errModalTitle").html("REQUEST FAILED");
    $("#errModalContent").html(msg);
    $("#errModal").modal("show");
}

function submitDateSelectionChanges() {

    brandFormData.Submission_Date = new Date(submitDate.val());
    brandFormData.Effective_Date = new Date(effectiveDate.val());

}

function attach()
{
    nppbkc.on("change", nppbkcSelectionChanges);
    submitDate.on("change", submitDateSelectionChanges);
    effectiveDate.on("change", submitDateSelectionChanges);
   // submitDateSelectionChanges();
    nppbkcSelectionChanges();
    $("#addOtherDocBtn").on("click", addOtherDoc);
    handleBrowseEvent("browseOtherDoc", "browseOtherDocFile", "browseOtherDocText");
}

function detach() {
    $("#addOtherDocBtn").off("click", addOtherDoc);
    // saveButton.off("click", uploads);
}

function brandGenerateClick() {
    console.log("generate");
    var fileName = $('[name="BrandExcelfile"]').val().trim();
    var pos = fileName.lastIndexOf('.');
    var extension = (pos <= 0) ? '' : fileName.substring(pos);
    if (extension != '.xlsx' && extension != '.xls') {
        alert('Please browse a correct Excel File to upload.');
        return false;
    }
    var formData = new FormData();
    var totalFiles = document.getElementById("BrandExcelfile").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("BrandExcelfile").files[i];
        formData.append("brandExcelfile", file);
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
                url: uploadUrlBrand,
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


//function loadSupportingDocuments(url, company) {
  
//    $("#customloader").show();
//    $.ajax({
//        url: url,
//        type: 'POST',
//        data: {
//            company: company
//        },
//    })
//    .success(function (partialResult) {
//        $("#customloader").hide();
//        docsContainer.html(partialResult);
//        sErrorDiv.hide();     
//    })
//    .error(function (error) {
//        $("#customloader").hide();        
//        sErrorDiv.html("<span>Supporting Document Data Not Available!</span>");
//        sErrorDiv.show();     
//    });
//}

function formatDocNumber(urlBrNumber, companyAlias, cityAlias, submitDate) {
    var date = new Date();
    var months = ["I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII"];
    var month = date.getMonth();
    var year = date.getYear() + 1900;
    var count = 0;

    $("#customloader").show();
    $.ajax({
        url: urlBrNumber,
        type: 'POST',
        success: function (response) {
            $("#customloader").hide();
            count = parseInt(response) + 1;
            var resultNumber = Pad(count, 10) + "/" + companyAlias + "/" + cityAlias + "/" + months[month] + "/" + year;
            brNumber.html(resultNumber);
            brNumber.val(resultNumber);
            console.log(resultNumber);
        },
        error: function () {
            $("#customloader").hide();
        }
    });
}


function ajaxNppbkcInfo(urlBrNumber,url, id) {
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
                company.val(data.Company.Name);
                companyId.val(data.Company.Id);
                companyAlias.val(data.Company.Alias);
                companyCity.val(data.CityAlias);

                npwp.val(data.Company.Npwp);
          //      var year = new Date(submitDate.val()).getYear() + 1900;
                //console.log(new Date(submitDate.val()).getYear());
            
                //formatDocNumber(urlBrNumber, data.Company.Alias, data.CityAlias, new Date(submitDate.val()));
            } else {
                kppbc.val("");              
                company.val("");
                companyId.val("");
                npwp.val("");
            }
        },
        error: function () {
            $("#customloader").hide();
        }
    });
}

function ajaxPlantInfo(urlPlant, nppbkcId) {
    $("#customloader").show();
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
                                          
            } else {
                addressPlant.val("");
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


function GetItemList() {
    $("#customloader").show();
    var ItemNotIn = [];
    var RegistrationID = $("#txt_hd_id").val();
    $("#tbody_skeplistitem").find("tr").each(function () {
        var ItemId = $(this).find(".txt_hd_itemid").val();
        ItemNotIn.push(ItemId);
    });
    var registrationType = $("#divRegistrationType input[type='radio']:checked").val();
    var nppbkc_selected = $("#NppbkcSelector").val();
    $.ajax({
        url: getUrl("GetProductDevelopmentItemList"),
        type: 'POST',
        data: { ItemNotIn: ItemNotIn, RegistrationType: registrationType, nppbkc: nppbkc_selected, RegId: RegistrationID },
        success: function (data) {
            $("#tbody_optitemlist").empty();
            if (data != null) {
                var item = "";
                for (var i = 0; i < data.length; i++) {
                    item += "<tr class='tr_item_list' id='tr_item_list_" + trItemIndex + "' ";
                    item += "data-index='" + trItemIndex + "'";
                    item += "data-requestnumber='" + data[i].Request_No + "'";
                    item += "data-facodeold='" + data[i].Fa_Code_Old + "'";
                    item += "data-facodeolddesc='" + data[i].Fa_Code_Old_Desc + "'";
                    item += "data-facodenew='" + data[i].Fa_Code_New + "'";
                    item += "data-facodenewdesc='" + data[i].Fa_Code_New_Desc + "'";
                    item += "data-pddetailid='" + data[i].PD_DETAIL_ID + "'";
                    item += "data-market='" + data[i].Market.Market_Desc+ "'";
                    item += "data-prodevnextaction='" + data[i].ProdDevNextAction + "'";
                    item += ">";
                    item += "<td class='td_inputan'>";
                    item += "<input type='checkbox' class='cb_itemlist' />";
                    item += "<input type='hidden' class='txt_hd_itemId' value='0'>";
                    item += "<input type='hidden' class='txt_hd_companytier' value='" + data[i].CompanyTier + "'>";
                    item += "<input type='hidden' class='txt_hd_brandname' value='" + data[i].BrandName + "'>";
                    item += "<input type='hidden' class='txt_hd_hjeperpack' value='" + data[i].HJE + "'>";
                    item += "<input type='hidden' class='txt_hd_hjeperbatang' value='" + data[i].HJEperBatang + "'>";
                    item += "<input type='hidden' class='txt_hd_content' value='" + data[i].BrandContent + "'>";
                    item += "<input type='hidden' class='txt_hd_unit' value='" + data[i].Unit + "'>";
                    item += "<input type='hidden' class='txt_hd_tariff' value='" + data[i].Tariff + "'>";
                    item += "<input type='hidden' class='txt_hd_packaging_material' value='" + data[i].PackagingMaterial + "'>";
                    item += "<input type='hidden' class='txt_hd_excisegood' value='" + data[i].ExciseGoodType + "'>";
                    item += "<input type='hidden' class='txt_hd_market' value=''>";
                    item += "<input type='hidden' class='txt_hd_front_side' value='" + data[i].FrontSide + "'>";
                    item += "<input type='hidden' class='txt_hd_back_side' value='" + data[i].BackSide + "'>";
                    item += "<input type='hidden' class='txt_hd_left_side' value='" + data[i].LeftSide + "'>";
                    item += "<input type='hidden' class='txt_hd_right_side' value='" + data[i].RightSide + "'>";
                    item += "<input type='hidden' class='txt_hd_top_side' value='" + data[i].TopSide + "'>";
                    item += "<input type='hidden' class='txt_hd_bottom_side' value='" + data[i].BottomSide + "'>";
                    item += "</td>";
                    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + data[i].Request_No + "</td>";
                    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + data[i].Fa_Code_Old + "</td>";
                    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + data[i].Fa_Code_Old_Desc + "</td>";
                    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + data[i].Fa_Code_New + "</td>";
                    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + data[i].Fa_Code_New_Desc + "</td>";
                    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + data[i].Company.Name + "</td>";
                    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + data[i].Hl_Code + "</td>";
                    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + data[i].Market.Market_Desc + "</td>";
                    item += "<td class='btn_showmodal_detail' style='cursor:pointer' >" + data[i].ProductionCenter + "</td>";
                    item += "</tr>";
                    trItemIndex++;
                }
                $("#tbody_optitemlist").append(item);
            }
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function ClearModalDetail() {
    $("#txt_modal_detail_requestnumber").val("");
    $("#txt_modal_detail_brandname").val("");
    //$("#txt_modal_detail_brandname").selectpicker("refresh");
    $("#txt_modal_detail_facodeold").val("");
    $("#txt_modal_detail_facodeolddesc").val("");
    $("#txt_modal_detail_facodenew").val("");
    $("#txt_modal_detail_facodenewdesc").val("");
    $("#txt_modal_detail_excisegoodtype").val("");
    $("#txt_modal_detail_excisegoodtype").selectpicker("refresh");
    $("#txt_modal_detail_companytier").val("");
    $("#txt_modal_detail_companytier").selectpicker("refresh");
    $("#txt_modal_detail_hjeperpack").val("");
    $("#txt_modal_detail_hjeperbatang").val("");
    $("#txt_modal_detail_content").val("");
    $("#txt_modal_detail_unit").val("");
    $("#txt_modal_detail_unit").selectpicker("refresh");
    $("#txt_modal_detail_tariff").val("");
    $("#txt_modal_detail_packaging_material").val("");
    $("#txt_modal_detail_market").val("");
    $("#txt_modal_detail_front_side").val("");
    $("#txt_modal_detail_back_side").val("");
    $("#txt_modal_detail_left_side").val("");
    $("#txt_modal_detail_right_side").val("");
    $("#txt_modal_detail_top_side").val("");
    $("#txt_modal_detail_bottom_side").val("");

}

$(document).on("change", ".change_hjeperbatang", function () {
    CountHJEperBatang();
});

$(document).on("change", ".change_tariff", function () {
    SetTariff();
});


function CountHJEperBatang() {
    var hje = NumberWithoutComma($("#txt_modal_detail_hjeperpack").val());
    var content = NumberWithoutComma($("#txt_modal_detail_content").val());
    var hjePerBatang = hje / content;
    $("#txt_modal_detail_hjeperbatang").val(NumberWithComma(hjePerBatang));
}

function SetTariff() {
    var hje = NumberWithoutComma($("#txt_modal_detail_hjeperpack").val());
    var startDate = $("#ViewModel_Submission_Date").val();
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
            $("#txt_modal_detail_tariff").val("0");
            if (data != null) {
                $("#txt_modal_detail_tariff").val(NumberWithComma(data.attribute.Tariff));
            }
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}



$(document).on("click", "#btn_myModalListItem", function () {
    GetItemList();
});

$(document).on("click", "#btn_addItem", function () {    
    $("#tbody_optitemlist").find("tr").each(function () {
        if ($(this).find(".cb_itemlist").is(":checked")) {
            itemdetailindex = $("#txt_index_listitem").val();
            var pddetailid = $(this).data("pddetailid");
            var market = $(this).data("market");
            var frontSide = $(".txt_hd_front_side").val();

            var item = "<input type='hidden' class='txt_hd_itemId' value='" + pddetailid + "' name='Item[" + itemdetailindex + "].PD_Detail_ID'>";
            item += "<input type='hidden' class='txt_hd_detailId' value='0' name='Item[" + itemdetailindex + "].Registration_Detail_ID'>";
            item += "<input type='hidden' class='txt_hd_isactive' value='true' name='Item[" + itemdetailindex + "].IsActive'>";
            item += "<input type='hidden' class='txt_hd_brandname' value='" + $(".txt_hd_brandname").val() + "' name='Item[" + itemdetailindex + "].Brand_CE'>";
            item += "<input type='hidden' class='txt_hd_latest_skep_no' value='' name='Item[" + itemdetailindex + "].Latest_Skep_No'>";
            item += "<input type='hidden' class='txt_hd_companytier' value='" + $(".txt_hd_companytier").val() + "' name='Item[" + itemdetailindex + "].Company_Tier'>";
            item += "<input type='hidden' class='txt_hd_excisegood' value='" + $(".txt_hd_excisegood").val() + "' name='Item[" + itemdetailindex + "].Prod_Code'>";
            item += "<input type='hidden' class='txt_hd_hjeperpack' value='" + $(".txt_hd_hjeperpack").val() + "' name='Item[" + itemdetailindex + "].Hje'>";
            item += "<input type='hidden' class='txt_hd_hjeperbatang' value='" + $(".txt_hd_hjeperbatang").val() + "' name='Item[" + itemdetailindex + "].HJEperBatang'>";
            item += "<input type='hidden' class='txt_hd_content' value='" + $(".txt_hd_content").val() + "' name='Item[" + itemdetailindex + "].Brand_Content'>";
            item += "<input type='hidden' class='txt_hd_unit' value='" + $(".txt_hd_unit").val() + "' name='Item[" + itemdetailindex + "].Unit'>";
            item += "<input type='hidden' class='txt_hd_tariff' value='" + $(".txt_hd_tariff").val() + "' name='Item[" + itemdetailindex + "].Tariff'>";
            item += "<input type='hidden' class='txt_hd_packaging_material' value='" + $(".txt_hd_packaging_material").val() + "' name='Item[" + itemdetailindex + "].Packaging_Material'>";
            item += "<input type='hidden' class='txt_hd_market' value='" + market + "' name='Item[" + itemdetailindex + "].Market'>";
            item += "<input type='hidden' class='txt_hd_front_side' value='" + $(".txt_hd_front_side").val() + "' name='Item[" + itemdetailindex + "].Front_Side'>";
            item += "<input type='hidden' class='txt_hd_back_side' value='" + $(".txt_hd_back_side").val() + "' name='Item[" + itemdetailindex + "].Back_Side'>";
            item += "<input type='hidden' class='txt_hd_left_side' value='" + $(".txt_hd_left_side").val() + "' name='Item[" + itemdetailindex + "].Left_Side'>";
            item += "<input type='hidden' class='txt_hd_right_side' value='" + $(".txt_hd_right_side").val() + "' name='Item[" + itemdetailindex + "].Right_Side'>";
            item += "<input type='hidden' class='txt_hd_top_side' value='" + $(".txt_hd_top_side").val() + "' name='Item[" + itemdetailindex + "].Top_Side'>";
            item += "<input type='hidden' class='txt_hd_bottom_side' value='" + $(".txt_hd_bottom_side").val() + "' name='Item[" + itemdetailindex + "].Bottom_Side'>";
            $(this).find(".td_inputan").append(item);
            $(this).appendTo("#tbody_skeplistitem");
            itemdetailindex++;
            $("#txt_index_listitem").val(itemdetailindex);

        }
    });
});

$(document).on("click", ".btn_showmodal_detail", function () {
    var tr = $(this).closest("tr");
    if ($(this).closest("tbody").attr("id") != "tbody_optitemlist") {

        ClearModalDetail();
        var index = tr.data("index");
        var market = tr.data("market");
        var requestnumber = tr.data("requestnumber");
        var brandname = tr.find(".txt_hd_brandname").val();
        var facodeold = tr.data("facodeold");
        var facodeolddesc = tr.data("facodeolddesc");
        var facodenew = tr.data("facodenew");
        var facodenewdesc = tr.data("facodenewdesc");
        var companytier = tr.find(".txt_hd_companytier").val();
        var excisegood = tr.find(".txt_hd_excisegood").val();
        var hjeperpack = tr.find(".txt_hd_hjeperpack").val();
        hjeperpack = NumberWithComma(NumberWithoutComma(hjeperpack));
        var unit = tr.find(".txt_hd_unit").val();
        var hjeperbatang = tr.find(".txt_hd_hjeperbatang").val();
        hjeperbatang = NumberWithComma(NumberWithoutComma(hjeperbatang));
        var content = tr.find(".txt_hd_content").val();
        content = NumberWithComma(NumberWithoutComma(content));
        var strTariff = tr.find(".txt_hd_tariff").val();
        var tariff = 0;
        if (strTariff != "") {
            strTariff = NumberWithoutComma(strTariff);
            tariff = parseInt(strTariff);
        }
        tariff = NumberWithComma(tariff);

        var packaging_material = tr.find(".txt_hd_packaging_material").val();
        var front_side = tr.find(".txt_hd_front_side").val();
        var back_side = tr.find(".txt_hd_back_side").val();
        var left_side = tr.find(".txt_hd_left_side").val();
        var right_side = tr.find(".txt_hd_right_side").val();
        var top_side = tr.find(".txt_hd_top_side").val();
        var bottom_side = tr.find(".txt_hd_bottom_side").val();

        $("#txt_hd_index").val(index);
        $("#txt_modal_detail_requestnumber").val(requestnumber);
        $("#txt_modal_detail_facodeold").val(facodeold);
        $("#txt_modal_detail_facodeolddesc").val(facodeolddesc);
        $("#txt_modal_detail_facodenew").val(facodenew);
        $("#txt_modal_detail_facodenewdesc").val(facodenewdesc);
        $("#txt_modal_detail_companytier").val(companytier);
        $("#txt_modal_detail_excisegoodtype").val(excisegood);
        $("#txt_modal_detail_excisegoodtype").selectpicker("refresh");
        $("#txt_modal_detail_companytier").selectpicker("refresh");

        $("#txt_modal_detail_hjeperpack").val(hjeperpack);
        $("#txt_modal_detail_hjeperbatang").val(hjeperbatang);
        $("#txt_modal_detail_unit").val(unit);
        $("#txt_modal_detail_unit").selectpicker("refresh");
        $("#txt_modal_detail_content").val(content);
        $("#txt_modal_detail_tariff").val(tariff);
        $("#txt_modal_detail_packaging_material").val(packaging_material);
        $("#txt_modal_detail_market").val(market);
        $("#txt_modal_detail_front_side").val(front_side);
        $("#txt_modal_detail_back_side").val(back_side);
        $("#txt_modal_detail_left_side").val(left_side);
        $("#txt_modal_detail_right_side").val(right_side);
        $("#txt_modal_detail_top_side").val(top_side);
        $("#txt_modal_detail_bottom_side").val(bottom_side);

        var registrationType = $("#divRegistrationType input[type='radio']:checked").val();
        $("#txt_modal_detail_brandname").val(brandname);
        if (brandname == "") {
            $("#txt_modal_detail_brandname").attr("readonly", false);
        }
        else {
            $("#txt_modal_detail_brandname").attr("readonly", true);
        }

        if (registrationType == "2") {
            $(".div_new_brand").hide();
        }
        else {
            $(".div_new_brand").show();
        }


        $("#myModalDetailItem").modal();
    }
    else
    {
        var cb = tr.find(".cb_itemlist");
        if (cb.is(':checked')) {
            cb.prop('checked', false);
        }
        else {
            cb.prop('checked', true);
        }
    }

});

function CheckFieldItemEmpty() {
    var emptyCount = 0;
    $("#div_alert_item").empty();
    var arr = [];
    
    var brand_name = $("#txt_modal_detail_brandname").val();
    if (brand_name == "") {
        emptyCount++;
        var message = "* The Brand Name Registration is required.";
        if (arr.indexOf(message) < 0) {
            arr.push(message);
            $("#div_alert_item").append(message + "<br/>");
        }
    }
 
    var product_type = $("#txt_modal_detail_excisegoodtype").val();
    if (product_type == "") {
        emptyCount++;
        var message = "* The Excise Good Type is required.";
        if (arr.indexOf(message) < 0) {
            arr.push(message);
            $("#div_alert_item").append(message + "<br/>");
        }
    }

    var company_tier = $("#txt_modal_detail_companytier").val();
    if (company_tier == "") {
        emptyCount++;
        var message = "* The Company Tier is required.";
        if (arr.indexOf(message) < 0) {
            arr.push(message);
            $("#div_alert_item").append(message + "<br/>");
        }
    }

    var hje_per_pack = $("#txt_modal_detail_hjeperpack").val();
    if (hje_per_pack == "") {
        emptyCount++;
        var message = "* The HJE Per Pack is required.";
        if (arr.indexOf(message) < 0) {
            arr.push(message);
            $("#div_alert_item").append(message + "<br/>");
        }
    }

    var content = $("#txt_modal_detail_content").val();
    if (content == "") {
        emptyCount++;
        var message = "* The HJE Per Pack is required.";
        if (arr.indexOf(message) < 0) {
            arr.push(message);
            $("#div_alert_item").append(message + "<br/>");
        }
    }

    var unit = $("#txt_modal_detail_unit").val();
    if (unit == "") {
        emptyCount++;
        var message = "* The Unit is required.";
        if (arr.indexOf(message) < 0) {
            arr.push(message);
            $("#div_alert_item").append(message + "<br/>");
        }
    }



    if (emptyCount == 0) {
        $("#div_alert_item").hide();
    }
    else {
        $("#div_alert_item").show();
    }
    return emptyCount;
}


$(document).on("click", "#btn_saveDetailItem", function () {
    var fieldEmpty = CheckFieldItemEmpty();
    if (fieldEmpty == 0)
    {
        var brandname = $("#txt_modal_detail_brandname").val();
        var unit = $("#txt_modal_detail_unit").val();
        var companytier = $("#txt_modal_detail_companytier").val();
        var excisegood = $("#txt_modal_detail_excisegoodtype").val();
        var hjeperpack = $("#txt_modal_detail_hjeperpack").val();
        var hjeperbatang = $("#txt_modal_detail_hjeperbatang").val();
        var content = $("#txt_modal_detail_content").val();
        var tariff = $("#txt_modal_detail_tariff").val();
        var packaging_material = $("#txt_modal_detail_packaging_material").val();
        var market = $("#txt_modal_detail_packaging_market").val();
        var front_side = $("#txt_modal_detail_front_side").val();
        var back_side = $("#txt_modal_detail_back_side").val();
        var left_side = $("#txt_modal_detail_left_side").val();
        var right_side = $("#txt_modal_detail_right_side").val();
        var top_side = $("#txt_modal_detail_top_side").val();
        var bottom_side = $("#txt_modal_detail_bottom_side").val();

        var index = $("#txt_hd_index").val();
        var tr = $("#tr_item_list_" + index);
        tr.find(".txt_hd_brandname").val(brandname);
        tr.find(".txt_hd_unit").val(unit);
        tr.find(".txt_hd_companytier").val(companytier);
        tr.find(".txt_hd_excisegood").val(excisegood);
        tr.find(".txt_hd_hje").val(hjeperpack);
        tr.find(".txt_hd_hjeperpack").val(hjeperpack);
        tr.find(".txt_hd_hjeperbatang").val(hjeperbatang);
        tr.find(".txt_hd_content").val(content);
        tr.find(".txt_hd_tariff").val(tariff);
        tr.find(".txt_hd_packaging_material").val(packaging_material);
        tr.find(".txt_hd_market").val(market);
        tr.find(".txt_hd_front_side").val(front_side);
        tr.find(".txt_hd_back_side").val(back_side);
        tr.find(".txt_hd_left_side").val(left_side);
        tr.find(".txt_hd_right_side").val(right_side);
        tr.find(".txt_hd_top_side").val(top_side);
        tr.find(".txt_hd_bottom_side").val(bottom_side);

        ClearModalDetail();
        $('#btn_closeDetailItem').click();
    }
    else
    {
    }

});



$(document).on("click", ".btn_browsesupportdoc_file", function () {
    var index = $(this).data("index");
    $("#txt_supportdoc_file" + index).click();
});

$(document).on("change", ".txt_file", function () {
    var id = $(this).attr("id");
    var filename_index = $(this).data("index");
    $("#hd_toggle_otherdoc").val(filename_index);
    var filesize = GetFileSize(id);
    if (filesize != null && filesize != undefined && filesize > 0) {
        var maxsize = parseInt($("#txt_hd_filesize").val());
        if (filesize > maxsize) {
            $(this).empty();
            alert("Max file size is " + String(maxsize) + " Mb");
        }
        else {
            var txtFileName = $(this).closest(".div_file").find(".txt_file_name");
            var fileName = $(this).val().replace(/C:\\fakepath\\/i, '');
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
    var htmlInput = "<input id='txt_otherdoc_" + HdDocIndex + "' type='file' name='File_Other' class='hidden txt_file' accept='application/pdf' data-index='other_" + HdDocIndex + "' />";
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
            else {
                $("#div_alert_additional").html("File already exist");
                $("#div_alert_additional").show();
            }

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

function loadSupportingDocuments(nppbkc) {
    $("#customloader").show();
    docsContainer.html("");
    var crid = $("#txt_hd_id").val();
    var isReadonly = $("#txt_hd_isformreadonly").val();

    $.ajax({
        url: getUrl("GetSupportingDocuments"),
        type: 'POST',
        data: {
            nppbkc: nppbkc,
            RegId: crid,
            IsReadonly: isReadonly
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

function SupportingDocumentList(sdUrl, CompanyId) {
    var crid = $("#txt_hd_id").val();
    var isReadonly = $("#txt_hd_isformreadonly").val();
    var param = {
        CompanyId: CompanyId,
        RegId: crid,
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
            '<td class="td_filename">' + filename + '</td>' +
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

function AddRemovedFileToList(FileId) {
    if (FileId != 0 && FileId != "" && FileId != null && FileId != "0") {
        var html = "<input type='hidden' name='RemovedFilesId' value='" + FileId.toString() + "' />";
        $(".div_interviewrequest").append(html);
    }
}


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
    if (nppbkc_selected == "") {
        emptyCount++;
        var message = "* The NPPBKC field is required.";
        if (arr.indexOf(message) < 0) {
            arr.push(message);
            $("#div_alert").append(message + "<br/>");
        }
    }
    var message = "";
    var detCount = 0;
    $('.tr_item_list:visible').each(function () {
        detCount++;
    });
    if (detCount == 0) {
        emptyCount++;
        message += "* At least 1 Item is required.<br/>";
    }
    //var other_doc_count = 0;
    //$("#div_otherDocsBody").find(".td_filename_number").each(function () {
    //    other_doc_count++;
    //});
    //if (other_doc_count == 0) {
    //    emptyCount++;
    //    message += "* At least 1 Attachment is required.<br/>";
    //}
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

    if (detailempty > 0) {
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

//$(document).on("click", "#btnSave", function () {
//    //var fieldEmpty = CheckFieldEmpty();
//    //if (fieldEmpty == 0) {
//    alert('asasa');
//    $("#BrandRegCreateForm").submit();
//    alert('xxxx');

//    //}
//});

function GeneratePrintOut() {
    $("#customloader").show();
    $("#div-printout").empty();
    var ID = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GeneratePrintout"),
        data: { ID: ID },
        success: function (data) {
            $("#div-printout").html(data);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function GeneratePrintOutSuratPernyataan() {
    $("#customloader").show();
    $("#div-printout_surat_pernyataan").empty();
    var ID = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GeneratePrintoutSuratPernyataan"),
        data: { ID: ID },
        success: function (data) {
            $("#div-printout_surat_pernyataan").html(data);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}



function GeneratePrintOutItem() {
    $("#customloader").show();
    $("#div-printout-item").empty();
    var ID = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GeneratePrintoutItem"),
        data: { ID: ID },
        success: function (data) {
            $("#div-printout-item").html(data);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}

function GeneratePrintOutExport() {
    $("#customloader").show();
    $("#div-printout-export").empty();
    var ID = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("GeneratePrintoutExport"),
        data: { ID: ID },
        success: function (data) {
            $("#div-printout-export").html(data);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}


function GeneratePrintOutPDF() {
    $("#form_DownloadPrintout").submit();
}

function GetPrintoutLayout(layout) {
    $("#customloader").show();
    var registrationType = $("#divRegistrationType input[type='radio']:checked").val();
    $("#PrintLayoutName").val(layout);
    $.ajax({
        type: 'POST',
        url: getUrl("GetPrintOutLayout"),
        data: {
            RegistrationType: registrationType,
            CreatedBy: $('#txt_hd_createdby').val(),
            DocExport: $('#txt_hd_docexport').val(),
            LayoutName: layout
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

function GetPrintoutLayoutSuratPernyataan() {
    $("#customloader").show();
    var registrationType = $("#divRegistrationType input[type='radio']:checked").val();

    $.ajax({
        type: 'POST',
        url: getUrl("GetPrintOutLayoutSuratPernyataan"),
        data: {
            RegistrationType: registrationType,
            CreatedBy: $('#txt_hd_createdby').val(),
            DocExport: $('#txt_hd_docexport').val(),
        },
        success: function (html) {
            tinyMCE.activeEditor.setContent(html);
        },
        complete: function () {
            $("#customloader").hide();
        }
    });
}


function UpdatePrintoutLayout(layoutName) {
    $("#customloader").show();
    CloseAllModal();
    var layout = tinyMCE.get('txt_printoutlayout').getContent();
    var registrationType = $("#divRegistrationType input[type='radio']:checked").val();

    $.ajax({
        type: 'POST',
        url: getUrl("UpdatePrintOutLayout"),
        data: {
            NewPrintout: layout,
            RegistrationType: registrationType,
            CreatedBy: $('#txt_hd_createdby').val(),
            DocExport: $('#txt_hd_docexport').val(),
            LayoutName: layoutName
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
    GeneratePrintOutSuratPernyataan();
    GeneratePrintOutItem();

    if($("#txt_hd_docexport").val() == "True")
    {
        GeneratePrintOutExport();

    }
});

$(document).on("click", "#btn_download_printout", function () {
    GeneratePrintOutPDF();
});

$(document).on("click", "#btn_show_printouteditor", function () {
    var layout = $(this).data("layout");

    GetPrintoutLayout(layout);
});

$(document).on("click", "#btn_show_printouteditor_suratpernyataan", function () {
    GetPrintoutLayoutSuratPernyataan();
});


$(document).on("click", "#btn_change_printoutlayout", function () {
    UpdatePrintoutLayout($("#PrintLayoutName").val());
});

$(document).on("click", "#btnSave", function () {
    CloseAllModal();
    $("#customloader").show();
    CleaningComma();
    var fieldEmpty = CheckFieldEmpty();
    if (fieldEmpty == 0) {
        $("#txt_hd_next_status").val($("#statusDraftEdit").val());
        $("#BrandRegCreateForm").submit();
    }
    else {
        $("#customloader").hide();
    }
});


$(document).on("click", "#btnSubmit", function () {
    CloseAllModal();
    $("#customloader").show();
    CleaningComma();
    var url = $("#txt_hd_baseurl").val() + "BRBrandRegistration/ChangeStatus";
    $("#BrandRegCreateForm").attr("action", url);
    $("#txt_hd_current_action").val("submit");
    $("#txt_hd_next_status").val($("#statusWaitingforPOAApproval").val());
    $("#BrandRegCreateForm").submit();
});

$(document).on("click", "#btnSubmitSkep", function () {
    CloseAllModal();
    $("#customloader").show();
    $("#txt_hd_next_status").val($("#statusWaitingforPOASKEPApproval").val());
    $("#BrandRegCreateForm").submit();
});

$(document).on("click", "#btnCancel", function () {
    CloseAllModal();
    $("#customloader").show();
    var url = $("#txt_hd_baseurl").val() + "BRBrandRegistration/ChangeStatus";
    $("#BrandRegCreateForm").attr("action", url);
    var last_approval_status = $("#txt_hd_lastapproved_status").val();
    $("#txt_hd_current_action").val("cancel");
    $("#txt_hd_next_status").val($("#statusCanceled").val());
    $("#BrandRegCreateForm").submit();
});


$(document).on("click", "#btnApprove", function () {
    CloseAllModal();
    $("#customloader").show();
    var last_approval_status = $("#txt_hd_lastapproved_status").val();
    $("#txt_hd_current_action").val("approve");

    if (last_approval_status == $("#statusWaitingforPOAApproval").val())
    {
        $("#txt_hd_next_status").val($("#statusWaitingforGovApproval").val());

    }
    else
    {
        if (last_approval_status == $("#statusWaitingforPOASKEPApproval").val())
        {
            $("#txt_hd_next_status").val($("#statusCompleted").val());
        }
    }

    $("#BrandRegCreateForm").submit();
});

$(document).on("click", ".btn_showmodal_changestatus", function () {
    var action = $(this).data("action");
    if (action == "Revise") {
        $("#txt_label_forchangestatus").text("Revise Form Confirmation");
        $("#btn_changestatus_reject").hide();
        $("#btn_changestatus_revise").show();
    }
    else if (action == "FinalRevise") {
        $("#txt_label_forchangestatus").text("Revise");
        $("#btn_finalrevise").show();
    }
    else if (action == "Withdraw") {
        $("#txt_label_forchangestatus").text("Withdraw Form Confirmation");
        $("#btn_changestatus_reject").hide();
        $("#btn_changestatus_revise").hide();
        $("#btn_finalrevise").hide();
        $("#btn_changestatus_withdraw").show();
    }

});

$(document).on("click", ".btn_changestatus_submit", function () {
    var action = $(this).data("action");
    var status = "";
    switch(action)
    {
        case "revise":
            status = $("#statusDraftEdit").val();
            break;

        case "finalrevise":
            status = $("#statusWaitingforGovApproval").val();
            break;

        case "withdraw":
            status = $("#statusCanceled").val();
            break;
    }
    var comment = $("#txt_changestatus_comment").val();

    if (comment != "") {
        CloseAllModal();
        $("#customloader").show();
        ReviseBrand(action, status, comment);
    }
    else {
        $("#div_alert_revise").show();
        $("#div_alert_revise").html("* Note/Comment cannot be empty.");
    }
});


function ReviseBrand(Action, Status, Comment) {
    var CRId = $("#txt_hd_id").val();
    $.ajax({
        type: 'POST',
        url: getUrl("ReviseBrand"),
        data: {
            ID: CRId,
            NextStatus: Status,
            Comment: Comment
        },
        success: function (data) {
            var errmessage = data;
            window.location.replace(getUrl("IndexBrandRegistration"));
            //$("#customloader").hide();
        }
    });
    $("#customloader").show();

}

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

function NumberWithComma(number) {
    if (number != "") {
        //number = number.replace(/\.00/g, '');
        number = number.toString().replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
    return number;
}

function NumberWithoutComma(number) {
    if (number != "") {
        number = number.replace(/\,/g, '');
        //number = number.replace(/\.00/g, '');
    }
    return number;
}

function ToggleCheckAll() {
    var isNowChecked = false;
    $("#tbody_skeplistitem").find(".tr_item_list").each(function () {
        if ($(this).is(":visible")) {
            var checkbox = $(this).find(".td_inputan").find(".cb_itemlist");
            if (checkbox.is(":checked") == true) {
                isNowChecked = true;
            }
        }
    });
    $("#tbody_skeplistitem").find(".tr_item_list").each(function () {
        if ($(this).is(":visible")) {
            var checkbox = $(this).find(".td_inputan").find(".cb_itemlist");
            if (isNowChecked == true) {
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

$("input[name='ViewModel.Registration_Type']").change(function () {
    var registrationType = $("#divRegistrationType input[type='radio']:checked").val();

    if (registrationType == 1)
    {
        $("#divEffectiveDate").hide();
    }
    else
    {
        $("#divEffectiveDate").show();
    }

    $("#tbody_skeplistitem").find(".tr_item_list").each(function () {
        var proddevnextaction = $(this).data("proddevnextaction");
        if (registrationType != proddevnextaction)
        {
            $(this).hide();
            $(this).find(".td_inputan").find(".txt_hd_itemId").val("-1");
        }
    });

});

$(document).on("click", "#btn-tab-changelog", function () {
    var crid = $("#txt_hd_id").val();

    $("#customloader").show();

    $.ajax({
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
        }

    }).done(function () {
        $("#customloader").hide();
    });


});

$(document).on("click", "#btn_showimport_dialog", function (e) {
    $('#txt_FileImport').replaceWith($('#txt_FileImport').clone());
    $("#txt_startdate_import").val("");    
});

$(document).on('click', '#btn_startimport', function (e) {
    CloseAllModal();
    $("#txt_submissiondate_import").val($("#ViewModel_Submission_Date").val());
    var radioValue = $("input[name='ViewModel.Registration_Type']:checked").val();
    $("#txt_hdregistration_type").val(radioValue);
    GenerateImportItems();
});

function GenerateImportItems() {
    var radioValue = $("input[name='ViewModel.Registration_Type']:checked").val();
    $("#customloader").show();
    $("#div_error_importitem").empty();
    $("#div_error_importitem").hide();
    var ItemNotIn = "";
    var urut = 1;
    $("#tbody_skeplistitem").find("tr").each(function () {
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
                    var tr = GenerateItemRow(trItemIndex, result[i].Item.Request_No, result[i].Item.Fa_Code_Old, result[i].Item.Fa_Code_Old_Desc, result[i].Item.Fa_Code_New, result[i].Item.Fa_Code_New_Desc, result[i].Item.PD_DETAIL_ID, result[i].Item.Company.Name, result[i].Item.Hl_Code, result[i].Item.Market.Market_Desc, result[i].Item.ProductionCenter, radioValue);
                    $("#tbody_skeplistitem").append(tr);
                    var hd = GenerateItemInputHidden(trItemIndex, result[i].Item.PD_DETAIL_ID, result[i].Brand_Ce, result[i].Company_Tier, result[i].Prod_Code, result[i].HJEperPack, result[i].HJEperBatang, result[i].Brand_Content, result[i].Unit, result[i].Tariff, result[i].Packaging_Material, result[i].Item.Market.market_desc, result[i].Front_Side, result[i].Back_Side, result[i].Left_Side, result[i].Right_Side, result[i].Top_Side, result[i].Bottom_Side);
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

function GenerateItemRow(trItemIndex, request_no, fa_old, fa_old_Desc, fa_new, fa_new_desc, pd_detailId, company_name, hl_code, market_desc, plant, registrationType) {
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

function RestoreDefaultPrintout() {
    $("#customloader").show();
    var registrationType = $("#divRegistrationType input[type='radio']:checked").val();
    var layoutName = $("#PrintLayoutName").val();
    $.ajax({
        type: 'POST',
        url: getUrl("RestorePrintoutToDefault"),
        data: {
            RegistrationType: registrationType,
            CreatedBy: $('#txt_hd_createdby').val(),
            DocExport: $('#txt_hd_docexport').val(),
            LayoutName: layoutName
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

$(document).on("click", "#btn_browseSkepDoc_file", function () {
    var div_parent = $(this).closest(".div_file");
    var htmlInput = "<input id='txt_badoc_" + BAdocIndex + "' type='file' name='File_SKEP' class='hidden txt_file' accept='application/pdf' data-thefieldnameid='txt_otherdoc_filename_ba_" + BAdocIndex + "' data-index='ba_" + BAdocIndex + "' />";
    htmlInput += "<input type='hidden' name='File_SKEP_Name' id='txt_otherdoc_filename_ba_" + BAdocIndex + "' />";
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

$(document).on("click", ".btn_remove_badoc", function () {
    var thisTr = $(this).closest("tr");
    var fileuploadid = $(this).data("fileuploadid");
    var Removingfilename = thisTr.find(".td_filename").data("filename");
    if (fileuploadid == "0") {
        $(".div_file_badoc").find('.txt_file').each(function () {
            var fileName = $(this).val().replace(/C:\\fakepath\\/i, '');
            fileName = fileName.replace(/^.*[\\\/]/, '')
            if (Removingfilename == fileName) {
                var index = $(this).data("index");
                var txtfilenameId = "#txt_otherdoc_filename_" + index;
                $(txtfilenameId).remove();
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

$(document).on("click", ".btn_remove_supportdoc", function () {
    var fileuploadid = $(this).data("fileuploadid");
    var parent = $(this).closest(".div_file");
    parent.find(".div_file_href").remove();
    parent.find(".div_supportdoc_name").show();
    parent.find(".span_browsesupportdoc").show();
    parent.find(".supporting_document").prop('required', true);
    AddRemovedFileToList(fileuploadid);
});

function CleaningComma() {
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
