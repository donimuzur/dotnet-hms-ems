$(document).ready(function() {
	$('.input-group.date').datepicker({
	    toggleActive: true,
	    autoclose: true,
	    format: "dd MMM yyyy",
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
	} );
});
$(document).keypress(function (e) {
    if (e.which == 13) {
        $('.bs-example-modal-sm').modal('hide');
        $('.delete-modal').modal('hide');
    }
});