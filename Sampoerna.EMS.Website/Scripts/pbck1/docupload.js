var file_sk = 0;

$('#upload-file-sk').click(function () {

    uploadSk();

});

$('#file-decree-doc-container').on('click', '.add-files', (function () {
    file_sk++;
    $('#file-decree-doc-container').append(' <div class="row"><div class="col-sm-9"> ' +
                                 '<input type="file" id="Detail_Pbck1DecreeFiles_' + file_sk + '_" name="Detail.Pbck1DecreeFiles[' + file_sk + ']" class="form-control">' +
                             '</div><div class="col-sm-3"><button type="button" class="btn btn-danger full" onclick="removeUploadButton(' + file_sk + ', $(this))">Remove</button>' +
                             '</div></div>');
}));

function uploadSk() {
    var poa_sk = [];
    for (var i = 0; i <= file_sk ; i++) {
        var fileName = $('#Detail_Pbck1DecreeFiles_' + i + '_').val();
        var name = fileName.replace("C:\\fakepath\\", "");
        poa_sk.push(name);
    }

    //$('#poa-files').html("");
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
}

function removeUploadButton(index, obj) {
    if (index > 0) {
        file_sk--;

        $('#Detail_Pbck1DecreeFiles_' + index + '_').remove();
        obj.remove();
        $('#file-decree-doc-container').html("");
        $('#file-decree-doc-container').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files">Add Files</button></div></div>');
        for (var i = 0; i <= file_sk; i++) {
            $('#file-decree-doc-container').append('<div class="row"><div class="col-sm-9"> ' +
                            '<input type="file" id="Detail_Pbck1DecreeFiles_' + i + '_" name="Detail.Pbck1DecreeFiles[' + i + ']" class="form-control">' +
                        '</div><div class="col-sm-3"><button type="button" class="btn btn-danger full" onclick="removeUploadButton(' + i + ', $(this))">Remove</button>' +
                        '</div></div>');

        }
    }
}