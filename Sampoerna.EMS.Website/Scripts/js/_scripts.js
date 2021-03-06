$(document).ready(function() {
	$('.table-container .fix-header').fixedHeaderTable({height: '259', altClass: 'odd', themeClass: 'fancyDarkTable' });
	$(".side-main i.fa").click(function(){
		if($("body").hasClass('hideMenu')){
			$("body").removeClass("hideMenu");			
			$(".side-main i.fa").removeClass("fa-caret-right");
			$(".collapse-menu.active").delay(300).addClass('open');
			$(".collapse-menu.active ul.list").delay(300).fadeIn('fast');
		}else{
			$("body").addClass("hideMenu");
			$(".side-main i.fa").addClass("fa-caret-right");
			$("ul.list").fadeOut('fast');
			$(".collapse-menu").removeClass('open');
		}	
	});
	$(".excise-menu").click(function(){
		$(".excise-menu").removeClass("active");
		$(this).addClass("active");
	});
	$(".movement-menu").click(function(){
		$(".claimable").css("display","none");
		$(".movement").css("display","block");
	});
	$(".claimable-menu").click(function(){
		$(".movement").css("display","none");
		$(".claimable").css("display","block");
	});
	$(".other").mouseenter(function(){
		$(".other .list-group-item").addClass("active");
		$("ul.child.other-menu").fadeIn('fast');
		if($("body").hasClass('hideMenu')){
		} else {
			$(".collapse-menu").removeClass('open');
			$("ul.list").fadeOut('fast');
		}
	});
	$(".other").mouseleave(function(){
		$(".other .list-group-item").removeClass("active");
		$("ul.child.other-menu").fadeOut('fast');
		if($("body").hasClass('hideMenu')){
		} else {
			$(".collapse-menu.active").addClass('open');
			$(".collapse-menu.active ul.list").fadeIn('fast');
		}
	});
	$("a.list-parent").each(function(){
		$(this).mouseenter(function(){
			if($("body").hasClass('hideMenu')){
				$(this).parent().addClass('open');
				$(this).next('ul').fadeIn('fast');
			}
		});
		$(this).parent().mouseleave(function(){
			if($("body").hasClass('hideMenu')){
				$(this).removeClass('open');
				$(this).children('ul').fadeOut('fast');
			}
		});
	});
	$("a.list-parent").each(function(){
		$(this).click(function(){
			if($(this).parent().hasClass('open')){
				$(this).parent().removeClass('open');
				$(this).next('ul').fadeOut('fast');
			}else{
				$("ul.list").fadeOut('fast');
				$(this).parent().addClass('open');
				$(this).next('ul').fadeIn('fast');
			}
		});
	});
	widthHtml = $("html").outerWidth();
	if(widthHtml < 960 ){
		$("body").addClass("hideMenu");
		$(".side-main i.fa").addClass("fa-caret-right");
		$("ul.list").fadeOut('fast');
		$(".collapse-menu").removeClass('open');
	}else{
		$("body").removeClass("hideMenu");			
		$(".side-main i.fa").removeClass("fa-caret-right");
		$(".collapse-menu.active").delay(300).addClass('open');
		$(".collapse-menu.active ul.list").delay(300).fadeIn('fast');
	}
	if(widthHtml < 1180 ){
		$(".nav-tabs").addClass("nav-justified");
	}else{
		$(".nav-tabs").removeClass("nav-justified");
	}
});
$(window).resize(function() {
	widthHtml = $("html").outerWidth();
	if(widthHtml < 960 ){
		$("body").addClass("hideMenu");
		$(".side-main i.fa").addClass("fa-caret-right");
		$("ul.list").fadeOut('fast');
		$(".collapse-menu").removeClass('open');
	}else{
		$("body").removeClass("hideMenu");			
		$(".side-main i.fa").removeClass("fa-caret-right");
		$(".collapse-menu.active").delay(300).addClass('open');
		$(".collapse-menu.active ul.list").delay(300).fadeIn('fast');
	}
	if(widthHtml < 1180 ){
		$(".nav-tabs").addClass("nav-justified");
	}else{
		$(".nav-tabs").removeClass("nav-justified");
	}
});