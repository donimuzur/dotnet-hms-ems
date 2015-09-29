var file_sk = 0;

$('#upload-file-sk').click(function () {

    var poa_sk = [];
    for (var i = 0; i <= file_sk ; i++) {
        var fileName = $('#Pbck4FileUploadFileList_' + i + '_').val();
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
  
    file_sk++;

    $('#file-sk-container').append(' <div class="row"><div class="col-sm-9"> ' +
                            '<a class="file-input-wrapper btn btn-grey  file-inputs">' +
                            ' <span>Browse</span>' +
                               '<input type="file" id="Pbck4FileUploadFileList_' + file_sk + '_" name="Pbck4FileUploadFileList[' + file_sk + ']" class="file-inputs" data-filename-placement="inside" style="left: -169px; top: 14px;"></a>' +
                           '</div><div class="col-sm-3"></div></div>');



}));

function removeUploadButton(index) {
    file_sk--;
    $('#poa_sk' + index).remove();
    $('#Pbck4FileUploadFileList_' + index + '_').remove();

    $('#file-sk-container').html("");
    $('#file-sk-container').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files">Add Files</button></div></div>');
    for (var i = 0; i <= file_sk; i++) {
        $('#file-sk-container').append(' <div class="row"><div class="col-sm-9"> ' +
                            '<a class="file-input-wrapper btn btn-grey  file-inputs">' +
                            ' <span>Browse</span>' +
                               '<input type="file" id="Pbck4FileUploadFileList_' + file_sk + '_" name="Pbck4FileUploadFileList[' + file_sk + ']" class="file-inputs" data-filename-placement="inside" style="left: -169px; top: 14px;"></a>' +
                           '</div><div class="col-sm-3"></div></div>');

    }

}

//==========================================================
//ck3
//==========================================================
var file_sk2 = 0;

$('#upload-file-sk2').click(function () {

    var poa_sk2 = [];
    for (var i = 0; i <= file_sk2 ; i++) {
        var fileName = $('#Pbck4FileUploadFileList2_' + i + '_').val();
        var name = fileName.replace("C:\\fakepath\\", "");
        poa_sk2.push(name);
    }



    $('#poa-files2').html("");
    for (var j = 0; j < poa_sk2.length; j++) {
        var attachmentDiv = '<div class="row">' +
       '<div class="col-xs-8 col-sm-10 col-md-8 col-lg-10"> ' +
       '<a href="#" id="poa_sk2' + j + '">' + poa_sk2[j] + '</a>' +
       '</div>' +
       '<div class="col-xs-4 col-sm-2 col-md-4 col-lg-2">' +
       '<button type="button" class="btn btn-danger full" onclick="removeUploadButton2(' + j + ')" ><i class="fa fa-times"></i></button>	' +
       '</div>' +
       '</div>';

        $('#poa-files2').append(attachmentDiv);
    }


});


$('#file-sk-container2').on('click', '.add-files', (function () {

    file_sk2++;

    $('#file-sk-container2').append(' <div class="row"><div class="col-sm-9"> ' +
                            '<a class="file-input-wrapper btn btn-grey  file-inputs">' +
                            ' <span>Browse</span>' +
                               '<input type="file" id="Pbck4FileUploadFileList2_' + file_sk2 + '_" name="Pbck4FileUploadFileList2[' + file_sk2 + ']" class="file-inputs" data-filename-placement="inside" style="left: -169px; top: 14px;"></a>' +
                           '</div><div class="col-sm-3"></div></div>');



}));

function removeUploadButton2(index) {
    file_sk2--;
    $('#poa_sk2' + index).remove();
    $('#Pbck4FileUploadFileList2_' + index + '_').remove();

    $('#file-sk-container2').html("");
    $('#file-sk-container2').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files">Add Files</button></div></div>');
    for (var i = 0; i <= file_sk2; i++) {
        $('#file-sk-container').append(' <div class="row"><div class="col-sm-9"> ' +
                            '<a class="file-input-wrapper btn btn-grey  file-inputs">' +
                            ' <span>Browse</span>' +
                               '<input type="file" id="Pbck4FileUploadFileList2_' + file_sk2 + '_" name="Pbck4FileUploadFileList2[' + file_sk2 + ']" class="file-inputs" data-filename-placement="inside" style="left: -169px; top: 14px;"></a>' +
                           '</div><div class="col-sm-3"></div></div>');

    }

}
