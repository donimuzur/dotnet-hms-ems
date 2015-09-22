var file_sk = 0;

$('#upload-file-sk').click(function () {

    var poa_sk = [];
    for (var i = 0; i <= file_sk ; i++) {
        var fileName = $('#Ck5FileUploadFileList_' + i + '_').val();
        var name = fileName.replace("C:\\fakepath\\", "");
        poa_sk.push(name);
    }

   
		
    $('#poa-files').html("");
    for (var j = 0; j < poa_sk.length; j++) {
        var attachmentDiv = '<div class="row">' +
       '<div class="col-xs-8 col-sm-10 col-md-8 col-lg-10"> ' +
       '<a href="#" id="poa_sk' + j + '">' + poa_sk[j] + '</a>' +
       '</div>' +
       '<div class="col-xs-4 col-sm-2 col-md-4 col-lg-2">' +
       '<button type="button" class="btn btn-danger full" onclick="removeUploadButton(' + j + ')" ><i class="fa fa-times"></i></button>	' +
       '</div>' +
       '</div>';
        
        $('#poa-files').append(attachmentDiv);
    }
 

});


$('#file-sk-container').on('click', '.add-files', (function () {
   // $('.file-inputs').bootstrapFileInput();
    
    file_sk++;

    //$('#file-sk-container').append(' <div class="row"><div class="col-sm-9"> ' +
    //                             '<input type="file" id="Ck5FileUploadFileList_' + file_sk + '_" name="Ck5FileUploadFileList[' + file_sk + ']" class="file-inputs" data-filename-placement="inside" >' +
    //                         '</div><div class="col-sm-3"></div></div>');
   
    $('#file-sk-container').append(' <div class="row"><div class="col-sm-9"> ' +
                            '<a class="file-input-wrapper btn btn-grey  file-inputs">' +
                            ' <span>Browse</span>' +
                               '<input type="file" id="Ck5FileUploadFileList_' + file_sk + '_" name="Ck5FileUploadFileList[' + file_sk + ']" class="file-inputs" data-filename-placement="inside" style="left: -169px; top: 14px;"></a>' +
                           '</div><div class="col-sm-3"></div></div>');
    
   
        
}));

function removeUploadButton(index) {
    file_sk--;
    $('#poa_sk' + index).remove();
    $('#Ck5FileUploadFileList_' + index + '_').remove();

    $('#file-sk-container').html("");
    $('#file-sk-container').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files">Add Files</button></div></div>');
    for (var i = 0; i <= file_sk; i++) {
        $('#file-sk-container').append(' <div class="row"><div class="col-sm-9"> ' +
                            '<a class="file-input-wrapper btn btn-grey  file-inputs">' +
                            ' <span>Browse</span>' +
                               '<input type="file" id="Ck5FileUploadFileList_' + file_sk + '_" name="Ck5FileUploadFileList[' + file_sk + ']" class="file-inputs" data-filename-placement="inside" style="left: -169px; top: 14px;"></a>' +
                           '</div><div class="col-sm-3"></div></div>');

    }

}

