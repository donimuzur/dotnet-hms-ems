var file_sk = 0;

$('#upload-file-sk').click(function () {

    var poa_sk = [];
    for (var i = 0; i <= file_sk ; i++) {
        var fileName = $('#Pbck4FileUploadFileList_' + i + '_').val();
        var name = fileName.replace("C:\\fakepath\\", "");
        if (name != '') {
            poa_sk.push(name);
        }
    }



    //$('#poa-files').html("");
    //for (var j = 0; j < poa_sk.length; j++) {
    //    var attachmentDiv = '<div class="row">' +
    //   '<div class="col-xs-8 col-sm-10 col-md-8 col-lg-10"> ' +
    //   '<a href="#" id="poa_sk' + j + '">' + poa_sk[j] + '</a>' +
    //   '</div>' +
    //   '<div class="col-xs-4 col-sm-2 col-md-4 col-lg-2">' +
    //   '<button type="button" class="btn btn-danger full" onclick="removeUploadButton(' + j + ')" ><i class="fa fa-times"></i></button>	' +
    //   '</div>' +
    //   '</div>';

    //    $('#poa-files').append(attachmentDiv);
    //}
    for (var k = 0; k < poa_sk.length; k++) {
        $('#poa_sk' + k).remove();
    }
    for (var j = 0; j < poa_sk.length; j++) {


        var mm = '<div class="row" id="poa_sk' + j + '"><div class="col-sm-10">' +
            '<i>' + poa_sk[j] + '</i>' +
            '</div>' +
            '<div class="col-sm-2">' +
            '<button class="btn btn-danger full" type="button" onclick="removeUploadButton(' + j + ')"><i class="fa fa-times"></i></button>' +
            '</div>' +
            '</div>';



        $('#poa-files').append(mm);
    }

});


$('#file-sk-container').on('click', '.add-files', (function () {
  
    file_sk++;

    //$('#file-sk-container').append(' <div class="row"><div class="col-sm-9"> ' +
    //                        '<a class="file-input-wrapper btn btn-grey  file-inputs">' +
    //                        ' <span>Browse</span>' +
    //                           '<input type="file" id="Pbck4FileUploadFileList_' + file_sk + '_" name="Pbck4FileUploadFileList[' + file_sk + ']" class="file-inputs" data-filename-placement="inside" style="left: -169px; top: 14px;"></a>' +
    //                       '</div><div class="col-sm-3"></div></div>');
    $('#file-sk-container').append(' <div class="row"><div class="col-sm-9"> ' +
                               '<input type="file" id="Pbck4FileUploadFileList_' + file_sk + '_" name="Pbck4FileUploadFileList_[' + file_sk + ']" class="form-control">' +
                           '</div><div class="col-sm-3"><button type="button" class="btn btn-danger full" onclick="removeUploadButton(' + file_sk + ')">Remove</button>' +
                           '</div></div>');


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
        if (name != '') {
            poa_sk2.push(name);
        }
    }



    //$('#poa-files2').html("");
    //for (var j = 0; j < poa_sk2.length; j++) {
    //    var attachmentDiv = '<div class="row">' +
    //   '<div class="col-xs-8 col-sm-10 col-md-8 col-lg-10"> ' +
    //   '<a href="#" id="poa_sk2' + j + '">' + poa_sk2[j] + '</a>' +
    //   '</div>' +
    //   '<div class="col-xs-4 col-sm-2 col-md-4 col-lg-2">' +
    //   '<button type="button" class="btn btn-danger full" onclick="removeUploadButton2(' + j + ')" ><i class="fa fa-times"></i></button>	' +
    //   '</div>' +
    //   '</div>';

    //    $('#poa-files2').append(attachmentDiv);
    //}
    for (var k = 0; k < poa_sk2.length; k++) {
        $('#poa_sk2' + k).remove();
    }
    for (var j = 0; j < poa_sk2.length; j++) {


        var mm = '<div class="row" id="poa_sk2' + j + '"><div class="col-sm-10">' +
            '<i>' + poa_sk2[j] + '</i>' +
            '</div>' +
            '<div class="col-sm-2">' +
            '<button class="btn btn-danger full" type="button" onclick="removeUploadButton(' + j + ')"><i class="fa fa-times"></i></button>' +
            '</div>' +
            '</div>';


        $('#poa-files2').append(mm);

    }
});


$('#file-sk-container2').on('click', '.add-files', (function () {

    file_sk2++;

    //$('#file-sk-container2').append(' <div class="row"><div class="col-sm-9"> ' +
    //                        '<a class="file-input-wrapper btn btn-grey  file-inputs">' +
    //                        ' <span>Browse</span>' +
    //                           '<input type="file" id="Pbck4FileUploadFileList2_' + file_sk2 + '_" name="Pbck4FileUploadFileList2[' + file_sk2 + ']" class="file-inputs" data-filename-placement="inside" style="left: -169px; top: 14px;"></a>' +
    //                       '</div><div class="col-sm-3"></div></div>');
    $('#file-sk-container2').append(' <div class="row"><div class="col-sm-9"> ' +
                             '<input type="file" id="Pbck4FileUploadFileList2_' + file_sk + '_" name="Pbck4FileUploadFileList2_[' + file_sk2 + ']" class="form-control">' +
                         '</div><div class="col-sm-3"><button type="button" class="btn btn-danger full" onclick="removeUploadButton(' + file_sk2 + ')">Remove</button>' +
                         '</div></div>');



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


function removeUploadDocPbck4(obj) {
    $(obj).closest(".row").remove();
}

function removeUploadDocPbck4(obj, modelName, i) {
   
    //$('#Pbck4FileUploadModelList_' + i + '__IsDeleted').val('True');
    $('#' + modelName + i + '__IsDeleted').val('True');
    $(obj).closest(".row").remove();
}

//function removeUploadDocPbck42(obj) {
//    $(obj).closest(".row").remove();
//}

//function removeUploadDocPbck42(obj, i) {
//    // alert(i);
//    $('#Pbck4FileUploadModelList_' + i + '__IsDeleted').val('True');
//    $(obj).closest(".row").remove();
//}