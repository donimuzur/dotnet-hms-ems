var file_sk = 0;

$('#MenuPOA').addClass('active');
$('#upload-file-sk').click(function () {
    
    uploadSk();
    
});
$('#Detail_UserId').change(function () {
    OnChangeUser($(this).find("option:selected").val());
});
$('#file-sk-container').on('click', '.add-files', (function () {
    file_sk++;
   
    $('#file-sk-container').append(' <div class="row"><div class="col-sm-9"> ' +
                                 '<input type="file" id="Detail_PoaSKFile_' + file_sk + '_" name="Detail.PoaSKFile[' + file_sk + ']" class="form-control">' +
                             '</div><div class="col-sm-3"></div></div>');
}));

function uploadSk() {
    var poa_sk = [];
    for (var i = 0; i <= file_sk ; i++) {
        var fileName = $('#Detail_PoaSKFile_' + i + '_').val();
        var name = fileName.replace("C:\\fakepath\\", "");
        if (name != '') {
            poa_sk.push(name);
        }
    }
    
    //$('#poa-files').html("");
    for (var k = 0; k < poa_sk.length; k++) {
        $('#poa_sk' + k).remove();
    }
    for (var j = 0; j < poa_sk.length; j++) {


        var mm = '<div class="row" id="poa_sk' + j + '"><div class="col-sm-10">' +
            '<i>'+ poa_sk[j] +'</i>' +
            '</div>' +
            '<div class="col-sm-2">' +
            '<button class="btn btn-danger full" type="button" onclick="removeUploadButton(' + j + ')"><i class="fa fa-times"></i></button>' +
            '</div>' +
            '</div>';


       
        $('#poa-files').append(mm);
    }
}

function removeUploadButton(index) {
       file_sk--;
        $('#poa_sk' + index).remove();
        $('#Detail_PoaSKFile_' + index + '_').remove();
       
        $('#file-sk-container').html("");
        $('#file-sk-container').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files">Add Files</button></div></div>');
        for (var i = 0; i <= file_sk; i++) {
            $('#file-sk-container').append('<div class="row"><div class="col-sm-9"> ' +
                            '<input type="file" id="Detail_PoaSKFile_' + i + '_" name="Detail.PoaSKFile[' + i + ']" class="form-control">' +
                        '</div><div class="col-sm-3"></div></div>');

        }
    
}

function removeSK(id, url, obj) {

   
    
        $.ajax({
            type: 'POST',
            url: url,
            data: { skid: id },
            success: function (data) {
                var objParent = ($(obj).parent().parent());
                objParent.remove();
            }
        });
    
}