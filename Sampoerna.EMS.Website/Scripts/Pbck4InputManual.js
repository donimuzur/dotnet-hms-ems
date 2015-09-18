function ajaxGetListFaCode(url, formData) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {
                var listMaterial = $('#uploadFaCode');
                listMaterial.empty();

                var list = '<option value>Select</option>';

                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        list += "<option value='" + data[i].FaCode + "'>" + data[i].FaCode + "</option>";
                    }

                }

                listMaterial.html(list);



            }
        });
    }
}


function ajaxGetStickerCodes(url, formData) {
    if (formData.plantId) {
        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (data) {

                if (data != null) {
                    $("#uploadStickerCode").val(data);
                  
                }


            }
        });
    }
}