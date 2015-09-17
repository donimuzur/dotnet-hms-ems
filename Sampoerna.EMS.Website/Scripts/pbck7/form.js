var file_back1 = 0;


$('#upload-file-back1').click(function () {
    
    uploadBack1();
    
});

$('#file-back1-container').on('click', '#add-files-back1', (function () {
    file_back1++;
   
    $('#file-back1-container').append(' <div class="row"><div class="col-sm-9"> ' +
                                 '<input type="file" id="DocumentsPostBack1_' + file_back1 + '_" name="DocumentsPostBack1[' + file_back1 + ']" class="form-control">' +
                             '</div><div class="col-sm-3"></div></div>');
}));

function uploadBack1() {
    var docsback1 = [];
    for (var i = 0; i <= file_back1 ; i++) {
        var fileName = $('#DocumentsPostBack1_' + i + '_').val();
        var name = fileName.replace("C:\\fakepath\\", "");
        docsback1.push(name);
    }
    
  
    for (var k = 0; k < docsback1.length; k++) {
        $('#docback1' + k).remove();
    }
    for (var j = 0; j < docsback1.length; j++) {


        var mm = '<div class="row" id="docback1' + j + '"><div class="col-sm-10">' +
            '<i>' + docsback1[j] + '</i>' +
            '</div>' +
            '<div class="col-sm-2">' +
            '<button class="btn btn-danger full" type="button" onclick="removeBack1UploadButton(' + j + ')"><i class="fa fa-times"></i></button>' +
            '</div>' +
            '</div>';


       
        $('#back1-files').append(mm);
    }
}

function removeBack1UploadButton(index) {
    file_back1--;
        $('#docback1' + index).remove();
        $('#DocumentsBack1_' + index + '_').remove();
       
        $('#file-back1-container').html("");
        $('#file-back1-container').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files">Add Files</button></div></div>');
        for (var i = 0; i <= file_back1; i++) {
            $('#file-back1-container').append('<div class="row"><div class="col-sm-9"> ' +
                            '<input type="file" id="DocumentsPostBack1_' + i + '_" name="DocumentsPostBack1[' + i + ']" class="form-control">' +
                        '</div><div class="col-sm-3"></div></div>');

        }
    
}

function removeDocBack1(id, url, obj) {

   
    
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