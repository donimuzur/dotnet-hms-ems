﻿var file_sk = 0;

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
                             '</div><div class="col-sm-3"><button type="button" class="btn btn-danger full" onclick="removeUploadButton(' + file_sk + ', $(this))">Remove</button>' +
                             '</div></div>');
}));

function uploadSk() {
    var poa_sk = [];
    for (var i = 0; i <= file_sk ; i++) {
        var fileName = $('#Detail_PoaSKFile_' + i + '_').value;
        console.log(fileName);
        var name = fileName[0].val().replace("C:\\fakepath\\", "");
        poa_sk.push(name);
    }
    
    for (var j = 0; j < poa_sk.length; j++) {
        $('#poa-files').append('<p>' + poa_sk[i] + '</p>');
    }
}

function removeUploadButton(index, obj) {
    if (index > 0) {
        file_sk--;

        $('#Detail_PoaSKFile_' + index + '_').remove();
        obj.remove();
        $('#file-sk-container').html("");
        $('#file-sk-container').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files">Add Files</button></div></div>');
        for (var i = 0; i <= file_sk; i++) {
            $('#file-sk-container').append('<div class="row"><div class="col-sm-9"> ' +
                            '<input type="file" id="Detail_PoaSKFile_' + i + '_" name="Detail.PoaSKFile[' + i + ']" class="form-control">' +
                        '</div><div class="col-sm-3"><button type="button" class="btn btn-danger full" onclick="removeUploadButton(' + i + ', $(this))">Remove</button>' +
                        '</div></div>');

        }
    }
}