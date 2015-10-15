var file_sk = 0;
alert(1);
$('#upload-file-sk').click(function () {

    uploadSk();

});

$('#file-decree-doc-container').on('click', '.add-files', (function () {
    file_sk++;
    $('#file-decree-doc-container').append(' <div class="row"><div class="col-sm-9"> ' +
                                 '<input type="file" id="Detail_Pbck1DecreeFiles_' + file_sk + '_" name="Detail.Pbck1DecreeFiles[' + file_sk + ']" class="form-control">' +
                             '</div><div class="col-sm-3"><button type="button" class="btn btn-danger full" onclick="removeUploadButton(' + file_sk + ')">Remove</button>' +
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
var active = true;
function removeUploadButton(index, obj) {
    if (!active)
        return;
    active = false;
    $('#Detail_Pbck1DecreeFiles_' + index + '_').closest(".row").remove();
    $("#poa_sk"+index).remove();
    var i = 0;
    $("div[id^=poa_sk]").each(function () {
        $(this).attr("id", "poa_sk" + i);
        $(this).find("button").attr("onclick", "removeUploadButton(" + i + ")");
        i++;
    });

    var j = 0;
    $("input[id^=Detail_Pbck1DecreeFiles_]").each(function () {
        $(this).attr("id", "Detail_Pbck1DecreeFiles_" + j + "_");
        $(this).attr("name", "Detail.Pbck1DecreeFiles[" + j + "]");
        $(this).closest(".row").find(".col-sm-3 button").attr("onclick", "removeUploadButton(" + j + ")");
        j++;
    });

    file_sk--;
    active = true;
}