$(document).ready(function() {
	$('.input-group.date').datepicker({
	    toggleActive: true,
	    autoclose: true,
	    format: "dd M yyyy",
	    todayHighlight: true
	});
	$(".action a").tooltip();
	$('#pbck-1').dataTable( {
		"scrollY":        "220px",
		"scrollCollapse": true,
		"order": [[ 1, "asc" ]],
		"paging":         true,
		responsive: true
	} );
	$('#ck-5').dataTable( {
		"scrollY":        "220px",
		"scrollCollapse": true,
		"bFilter": false,
		"order": [[ 1, "asc" ]],
		"paging":         true,
		responsive: true
	});
	$(document).keypress(function (e) {
	    if (e.which == 13) {
	        $('.bs-example-modal-sm').modal('hide');
	        $('.delete-modal').modal('hide');
	    }
	});
	$('.action a , .action-button.new a').click(function (evt) {
	    var page_active = $('.paginate_button.current').text();
	    var url = window.location.href;
	    setCookie("page_active", page_active, 1);
	    setCookie("url_active", url, 1);
	});
	function setCookie(cname, cvalue, exdays) {
	    var d = new Date();
	    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
	    var expires = "expires=" + d.toUTCString();
	    document.cookie = cname + "=" + cvalue + "; " + expires;
	}
	$('.menu-claimable').on( 'keypress', function( e ) {
        $('.menu-claimable').modal({ show: false});
    });
});
$(document).keypress(function (e) {
    if (e.which == 13) {
        $('.bs-example-modal-sm').modal('hide');
        $('.delete-modal').modal('hide');
    }
});