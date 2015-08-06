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
	$('.menu-claimable').on( 'keypress', function( e ) {
        $('.menu-claimable').modal({ show: false});
    });
});