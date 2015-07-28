$(document).ready(function() {
	//$('.input-group.date').datepicker({
	//    toggleActive: true,
	//    autoclose: true,
	//    format: "dd/M/yyyy",
	//    todayHighlight: true
	//});
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
    
	//if ($('#workflowHistoryTable') != null) {
	//    $('#workflowHistoryTable .filters th').each(function () {
	//        var idx = $(this).index();
	//        var title = $('#workflowHistoryTable thead th').eq($(this).index()).text();
	//        $(this).html('<input type="text" placeholder="Search" style="width:100%" >');


	//    });
	//    var tableHistory = $('#workflowHistoryTable').DataTable();
	//    // Setup - add a text input to each footer cell
	//    if (tableHistory.columns().eq(0) != null) {
	//        tableHistory.columns().eq(0).each(function (colIdx) {
	//            $('input', $('.filters th')[colIdx]).on('keyup change', function () {
	//                tableHistory
    //                    .column(colIdx)
    //                    .search(this.value)
    //                    .draw();
	//            });
	//        });
	//    }

	//}
});