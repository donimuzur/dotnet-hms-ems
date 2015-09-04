var file_sk = 0;


$('#upload-file-sk').click(function () {
    
    uploadSk();
    
});

$('#file-sk-container').on('click', '.add-files', (function () {
    file_sk++;
   
    $('#file-sk-container').append(' <div class="row"><div class="col-sm-9"> ' +
                                 '<input type="file" id="Documents_' + file_sk + '_" name="Documents[' + file_sk + ']" class="form-control">' +
                             '</div><div class="col-sm-3"></div></div>');
}));

function uploadSk() {
    var docs = [];
    for (var i = 0; i <= file_sk ; i++) {
        var fileName = $('#Documents_' + i + '_').val();
        var name = fileName.replace("C:\\fakepath\\", "");
        docs.push(name);
    }
    
  
    for (var k = 0; k < docs.length; k++) {
        $('#doc' + k).remove();
    }
    for (var j = 0; j < docs.length; j++) {


        var mm = '<div class="row" id="doc' + j + '"><div class="col-sm-10">' +
            '<i>' + docs[j] + '</i>' +
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
        $('#doc' + index).remove();
        $('#Lack2Model_Documents_' + index + '_').remove();
       
        $('#file-sk-container').html("");
        $('#file-sk-container').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files">Add Files</button></div></div>');
        for (var i = 0; i <= file_sk; i++) {
            $('#file-sk-container').append('<div class="row"><div class="col-sm-9"> ' +
                            '<input type="file" id="Documents_' + i + '_" name="Documents[' + i + ']" class="form-control">' +
                        '</div><div class="col-sm-3"></div></div>');

        }
    
}

function removeDoc(id, url, obj) {

   
    
        $.ajax({
            type: 'POST',
            url: url,
            data: { docId: id },
            success: function (data) {
                var objParent = ($(obj).parent().parent());
                objParent.remove();
            }
        });
    
}