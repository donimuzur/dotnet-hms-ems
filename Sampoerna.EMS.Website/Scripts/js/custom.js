$(document).ready(function() {
	$('.input-group.date').datepicker({
	    toggleActive: true,
	    autoclose: true,
	    format: "dd M yyyy",
	    todayHighlight: true
	});
	$('.disabled.date .input-group-addon').click(function() {
		$(".datepicker").css('opacity','0');	  
		$(".datepicker").css('width','1px');	  
		$(".datepicker").css('height','1px');	  
		$(".datepicker").css('overflow','hidden');	  
	});
  
	$(".action a").tooltip();
	$('#pbck-1').DataTable( {
		"scrollY":        "220px",
		"scrollCollapse": true,
		"order": [[ 1, "asc" ]],
		"paging":         true,
		responsive: true
	} );
	$('#ck-5').DataTable({
		"scrollY":        "220px",
		"scrollCollapse": true,
		"bFilter": false,
		"order": [[ 1, "asc" ]],
		"paging":         true,
		responsive: true
	} );
});