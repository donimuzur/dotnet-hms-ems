$(document).ready(function() {
	$('.input-group.date').datepicker({
	    toggleActive: true,
	    defaultViewDate: { year: 2015, month: 06, day: 28 }
	});
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