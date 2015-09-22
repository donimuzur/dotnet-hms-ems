var file_back1 = 0;
var file_back3 = 0;
var file_ck2 = 0;

$('#upload-file-back1').click(function () {
    
    uploadBack1();
    
});


$('#upload-file-back3').click(function () {

    uploadBack3();

});

$('#upload-file-ck2').click(function () {

    uploadCk2();

});

$('#file-back1-container').on('click', '#add-files-back1', (function () {
    file_back1++;
   
    $('#file-back1-container').append(' <div class="row"><div class="col-sm-9"> ' +
                                 '<input type="file" id="DocumentsPostBack1_' + file_back1 + '_" name="DocumentsPostBack1[' + file_back1 + ']" class="form-control">' +
                             '</div><div class="col-sm-3"></div></div>');
}));
$('#file-back3-container').on('click', '#add-files-back3', (function () {
    file_back3++;

    $('#file-back3-container').append(' <div class="row"><div class="col-sm-9"> ' +
                                 '<input type="file" id="DocumentsPostBack3_' + file_back3 + '_" name="DocumentsPostBack3[' + file_back3 + ']" class="form-control">' +
                             '</div><div class="col-sm-3"></div></div>');
}));

$('#file-ck2-container').on('click', '#add-files-ck2', (function () {
    file_ck2++;

    $('#file-ck2-container').append(' <div class="row"><div class="col-sm-9"> ' +
                                 '<input type="file" id="DocumentsPostCk2_' + file_ck2 + '_" name="DocumentsPostCk2[' + file_ck2 + ']" class="form-control">' +
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

function uploadBack3() {
    var docsback3 = [];
    for (var i = 0; i <= file_back3 ; i++) {
        var fileName = $('#DocumentsPostBack3_' + i + '_').val();
        var name = fileName.replace("C:\\fakepath\\", "");
        docsback3.push(name);
    }


    for (var k = 0; k < docsback3.length; k++) {
        $('#docback1' + k).remove();
    }
    for (var j = 0; j < docsback3.length; j++) {


        var mm = '<div class="row" id="docback3' + j + '"><div class="col-sm-10">' +
            '<i>' + docsback3[j] + '</i>' +
            '</div>' +
            '<div class="col-sm-2">' +
            '<button class="btn btn-danger full" type="button" onclick="removeBack3UploadButton(' + j + ')"><i class="fa fa-times"></i></button>' +
            '</div>' +
            '</div>';



        $('#back3-files').append(mm);
    }
}


function uploadCk2() {
    var docsck2 = [];
    for (var i = 0; i <= file_ck2 ; i++) {
        var fileName = $('#DocumentsPostCk2_' + i + '_').val();
        var name = fileName.replace("C:\\fakepath\\", "");
        docsck2.push(name);
    }


    for (var k = 0; k < docsck2.length; k++) {
        $('#docsck2' + k).remove();
    }
    for (var j = 0; j < docsck2.length; j++) {


        var mm = '<div class="row" id="docsck2' + j + '"><div class="col-sm-10">' +
            '<i>' + docsck2[j] + '</i>' +
            '</div>' +
            '<div class="col-sm-2">' +
            '<button class="btn btn-danger full" type="button" onclick="removeCk2UploadButton(' + j + ')"><i class="fa fa-times"></i></button>' +
            '</div>' +
            '</div>';



        $('#ck2-files').append(mm);
    }
}

function removeBack1UploadButton(index) {
    file_back1--;
        $('#docback1' + index).remove();
        $('#DocumentsBack1_' + index + '_').remove();
       
        $('#file-back1-container').html("");
        $('#file-back1-container').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files" id="add-files-back1">Add Files</button></div></div>');
        for (var i = 0; i <= file_back1; i++) {
            $('#file-back1-container').append('<div class="row"><div class="col-sm-9"> ' +
                            '<input type="file" id="DocumentsPostBack1_' + i + '_" name="DocumentsPostBack1[' + i + ']" class="form-control">' +
                        '</div><div class="col-sm-3"></div></div>');

        }
    
}
function removeBack3UploadButton(index) {
    file_back3--;
    $('#docback3' + index).remove();
    $('#DocumentsBack3_' + index + '_').remove();

    $('#file-back3-container').html("");
    $('#file-back3-container').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files" id="add-files-back3">Add Files</button></div></div>');
    for (var i = 0; i <= file_back3; i++) {
        $('#file-back3-container').append('<div class="row"><div class="col-sm-9"> ' +
                        '<input type="file" id="DocumentsPostBack3_' + i + '_" name="DocumentsPostBack3[' + i + ']" class="form-control">' +
                    '</div><div class="col-sm-3"></div></div>');

    }

}
function removeCk2UploadButton(index) {
    file_ck2--;
    $('#docsck2'+ index).remove();
    $('#DocumentsCk2_' + index + '_').remove();

    $('#file-ck2-container').html("");
    $('#file-ck2-container').html('<div class="row"><div class="col-sm-4"><button type="button"  class="btn btn-primary full add-files" id="add-files-ck2">Add Files</button></div></div>');
    for (var i = 0; i <= file_ck2; i++) {
        $('#file-ck2-container').append('<div class="row"><div class="col-sm-9"> ' +
                        '<input type="file" id="DocumentsPostCk2_' + i + '_" name="DocumentsPostCk2[' + i + ']" class="form-control">' +
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