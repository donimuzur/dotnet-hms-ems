$(document).ready(function() {
	$('.input-group.date').datepicker({
	    toggleActive: true,
	     defaultViewDate: { year: 2015, month: 05, day: 28 }
	});
	$('.table-container .fix-header').fixedHeaderTable({height: '259', altClass: 'odd', themeClass: 'fancyDarkTable' });
	$(".side-main i.fa").click(function(){
		if($("body").hasClass('hideMenu')){
			$(".side-menu .list-group span").css("display", "block");
			$(".side-main h5").css("display", "block");
			$(".navbar-brand img").css("display", "block");
			$(".navbar-brand .logo-small").css("display", "none");
			$("ul.child").css("display", "block");
			$("ul.child.other-menu").css("display", "none");
			$(".side-menu").css("width", "230px");
			$(".navbar-header").css("width", "230px");
			$(".excise-site .side-menu:before").css("width", "230px");
			$(".content").css("padding-left", "229px");
			$("body").removeClass("hideMenu");			
			$(".side-main i.fa").removeClass("fa-caret-right");
		}else{
			$(".side-menu .list-group span").css("display", "none");
			$(".side-main h5").css("display", "none");
			$(".navbar-brand img").css("display", "none");
			$("ul.child").css("display", "none");
			$(".navbar-brand .logo-small").css("display", "block");
			$(".side-menu").css("width", "75px");
			$(".navbar-header").css("width", "75px");
			$(".excise-site .side-menu:before").css("width", "75px");
			$(".content").css("padding-left", "74px");
			$("body").addClass("hideMenu");
			$(".side-main i.fa").addClass("fa-caret-right");
		}	
	});
	$(".other").hover(function(){
		$("ul.child.other-menu").slideToggle();
	});
	$("a.list-parent").click(function(){
		$("ul.list").slideToggle();
	});
	widthHtml = $("html").outerWidth();
	if(widthHtml < 960 ){
		$(".side-menu .list-group span").css("display", "none");
		$(".side-main h5").css("display", "none");
		$(".navbar-brand img").css("display", "none");
		$(".navbar-brand .logo-small").css("display", "block");
		$(".side-menu").css("width", "75px");
		$(".navbar-header").css("width", "75px");
		$(".excise-site .side-menu:before").css("width", "75px");
		$(".content").css("padding-left", "74px");
		$("body").addClass("hideMenu");
		$(".side-main i.fa").addClass("fa-caret-right");
	}else{
		$(".side-menu .list-group span").css("display", "block");
		$(".side-main h5").css("display", "block");
		$(".navbar-brand img").css("display", "block");		
		$(".navbar-brand .logo-small").css("display", "none");
		$(".side-menu").css("width", "230px");
		$(".navbar-header").css("width", "230px");
		$(".excise-site .side-menu:before").css("width", "230px");
		$(".content").css("padding-left", "229px");
		$("body").removeClass("hideMenu");			
		$(".side-main i.fa").removeClass("fa-caret-right");
	}

});
$(window).resize(function() {
	widthHtml = $("html").outerWidth();
	if(widthHtml < 960 ){
		$(".side-menu .list-group span").css("display", "none");
		$(".side-main h5").css("display", "none");
		$(".navbar-brand img").css("display", "none");
		$(".navbar-brand .logo-small").css("display", "block");
		$(".side-menu").css("width", "75px");
		$(".navbar-header").css("width", "75px");
		$(".excise-site .side-menu:before").css("width", "75px");
		$(".content").css("padding-left", "74px");
		$("body").addClass("hideMenu");
		$(".side-main i.fa").addClass("fa-caret-right");
	}else{
		$(".side-menu .list-group span").css("display", "block");
		$(".side-main h5").css("display", "block");
		$(".navbar-brand img").css("display", "block");		
		$(".navbar-brand .logo-small").css("display", "none");
		$(".side-menu").css("width", "230px");
		$(".navbar-header").css("width", "230px");
		$(".excise-site .side-menu:before").css("width", "230px");
		$(".content").css("padding-left", "229px");
		$("body").removeClass("hideMenu");			
		$(".side-main i.fa").removeClass("fa-caret-right");
	}
});