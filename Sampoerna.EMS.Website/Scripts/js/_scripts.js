$(document).ready(function () {
    $('.input-group.date').datepicker({
        toggleActive: true,
        defaultViewDate: { year: 2015, month: 05, day: 28 }
    });
    $('.table-container .fix-header').fixedHeaderTable({ height: '259', altClass: 'odd', themeClass: 'fancyDarkTable' });
    $(".side-main i.fa").click(function () {
        if ($("body").hasClass('hideMenu')) {
            $("body").removeClass("hideMenu");
            $(".side-main i.fa").removeClass("fa-caret-right");
            $(".collapse-menu.active").addClass('open');
            $(".collapse-menu.active ul.list").fadeIn('fast');
        } else {
            $("body").addClass("hideMenu");
            $(".side-main i.fa").addClass("fa-caret-right");
            $("ul.list").fadeOut('fast');
            $(".collapse-menu").removeClass('open');
        }
    });
    $(".other").mouseenter(function () {
        $(".other .list-group-item").addClass("active");
        $("ul.child.other-menu").fadeIn('fast');
    });
    $(".other").mouseleave(function () {
        $(".other .list-group-item").removeClass("active");
        $("ul.child.other-menu").fadeOut('fast');
    });
    $("a.list-parent").each(function () {
        $(this).mouseenter(function () {
            if ($("body").hasClass('hideMenu')) {
                $(this).parent().addClass('open');
                $(this).next('ul').fadeIn('fast');
            }
        });
        $(this).parent().mouseleave(function () {
            if ($("body").hasClass('hideMenu')) {
                $(this).removeClass('open');
                $(this).children('ul').fadeOut('fast');
            }
        });
    });
    $("a.list-parent").each(function () {
        $(this).click(function () {
            if ($(this).parent().hasClass('open')) {
                $(this).parent().removeClass('open');
                $(this).next('ul').fadeOut('fast');
            } else {
                $(this).parent().addClass('open');
                $(this).next('ul').fadeIn('fast');
            }
        });
    });
});
$(window).resize(function () {
    widthHtml = $("html").outerWidth();
    if (widthHtml < 960) {

    } else {

    }
});