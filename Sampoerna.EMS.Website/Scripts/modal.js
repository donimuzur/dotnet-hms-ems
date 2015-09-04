$(function () {
    $.ajaxSetup({ cache: false });
    initEmsModal();
});

function initEmsModal() {
    $("a[data-modal]").on("click", function (e) {
        var modalTrigger = this;
        $('#emsModalContent').load(this.href, function () {
            $('#emsModal').modal({
                keyboard: true
            }, 'show');

            bindForm(this, $(modalTrigger).data("oncomplete"));
        });
        return false;
    });

    //$("tr[data-modal]").on("click", function (e) {
    //    var modalTrigger = this;
    //    $('#emsModalContent').load($(this).data("url"), function () {
    //        $('#emsModal').modal({
    //            keyboard: true
    //        }, 'show');

    //        bindForm(this, $(modalTrigger).data("oncomplete"));
    //    });
    //    return false;
    //});
}

function bindForm(dialog, callback) {
    $('form', dialog).submit(function () {
        console.log(callback);
        $('#progress').show();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $('#emsModal').modal('hide');
                    $('#progress').hide();
                    if (callback) eval(callback)();
                    //location.reload();
                } else {
                    $('#progress').hide();
                    $('#emsModalContent').html(result);
                    bindForm();
                }
            }
        });
        return false;
    });
}