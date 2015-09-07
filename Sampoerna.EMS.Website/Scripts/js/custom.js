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

});