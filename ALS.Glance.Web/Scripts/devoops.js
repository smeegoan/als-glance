//
//    Main script of DevOOPS v1.0 Bootstrap Theme
//
"use strict";


//
// Swap 2 elements on page. Used by WinMove function
//
jQuery.fn.swap = function (b) {
    b = jQuery(b)[0];
    var a = this[0];
    var t = a.parentNode.insertBefore(document.createTextNode(''), a);
    b.parentNode.insertBefore(a, b);
    t.parentNode.insertBefore(b, t);
    t.parentNode.removeChild(t);
    return this;
};

//
//  Beauty tables plugin (navigation in tables with inputs in cell)
//  Created by DevOOPS.
//
(function ($) {
    $.fn.beautyTables = function () {
        var table = this;
        var string_fill = false;
        this.on('keydown', function (event) {
            var target = event.target;
            var tr = $(target).closest("tr");
            var col = $(target).closest("td");
            if (target.tagName.toUpperCase() == 'INPUT') {
                if (event.shiftKey === true) {
                    switch (event.keyCode) {
                        case 37: // left arrow
                            col.prev().children("input[type=text]").focus();
                            break;
                        case 39: // right arrow
                            col.next().children("input[type=text]").focus();
                            break;
                        case 40: // down arrow
                            if (string_fill == false) {
                                tr.next().find('td:eq(' + col.index() + ') input[type=text]').focus();
                            }
                            break;
                        case 38: // up arrow
                            if (string_fill == false) {
                                tr.prev().find('td:eq(' + col.index() + ') input[type=text]').focus();
                            }
                            break;
                    }
                }
                if (event.ctrlKey === true) {
                    switch (event.keyCode) {
                        case 37: // left arrow
                            tr.find('td:eq(1)').find("input[type=text]").focus();
                            break;
                        case 39: // right arrow
                            tr.find('td:last-child').find("input[type=text]").focus();
                            break;
                        case 40: // down arrow
                            if (string_fill == false) {
                                table.find('tr:last-child td:eq(' + col.index() + ') input[type=text]').focus();
                            }
                            break;
                        case 38: // up arrow
                            if (string_fill == false) {
                                table.find('tr:eq(1) td:eq(' + col.index() + ') input[type=text]').focus();
                            }
                            break;
                    }
                }
                if (event.keyCode == 13 || event.keyCode == 9) {
                    event.preventDefault();
                    col.next().find("input[type=text]").focus();
                }
                if (string_fill == false) {
                    if (event.keyCode == 34) {
                        event.preventDefault();
                        table.find('tr:last-child td:last-child').find("input[type=text]").focus();
                    }
                    if (event.keyCode == 33) {
                        event.preventDefault();
                        table.find('tr:eq(1) td:eq(1)').find("input[type=text]").focus();
                    }
                }
            }
        });
        table.find("input[type=text]").each(function () {
            $(this).on('blur', function (event) {
                var target = event.target;
                var col = $(target).parents("td");
                if (table.find("input[name=string-fill]").prop("checked") == true) {
                    col.nextAll().find("input[type=text]").each(function () {
                        $(this).val($(target).val());
                    });
                }
            });
        })
    };
})(jQuery);



//////////////////////////////////////////////////////
//////////////////////////////////////////////////////
//
//      MAIN DOCUMENT READY SCRIPT OF DEVOOPS THEME
//
//      In this script main logic of theme
//
//////////////////////////////////////////////////////
//////////////////////////////////////////////////////
$(document).ready(function () {
    $('.main-menu').on('click', 'a', function (e) {
        var parents = $(this).parents('li');
        var li = $(this).closest('li.dropdown');
        var another_items = $('.main-menu li').not(parents);
        another_items.find('a').removeClass('active');
        another_items.find('a').removeClass('active-parent');
        if ($(this).hasClass('dropdown-toggle') || $(this).closest('li').find('ul').length == 0) {
            $(this).addClass('active-parent');
            var current = $(this).next();
            if (current.is(':visible')) {
                li.find("ul.dropdown-menu").slideUp('fast');
                li.find("ul.dropdown-menu a").removeClass('active');
            }
            else {
                another_items.find("ul.dropdown-menu").slideUp('fast');
                current.slideDown('fast');
            }
        }
        else {
            if (li.find('a.dropdown-toggle').hasClass('active-parent')) {
                var pre = $(this).closest('ul.dropdown-menu');
                pre.find("li.dropdown").not($(this).closest('li')).find('ul.dropdown-menu').slideUp('fast');
            }
        }
        if ($(this).hasClass('active') == false) {
            $(this).parents("ul.dropdown-menu").find('a').removeClass('active');
            $(this).addClass('active');
        }        
        if ($(this).attr('href') == '#') {
            e.preventDefault();
        }
    });
    var height = window.innerHeight - 49;
    $('#main').css('min-height', height)
		.on('click', '.expand-link', function (e) {
		    var body = $('body');
		    e.preventDefault();
		    var box = $(this).closest('div.box');
		    var button = $(this).find('i');
		    button.toggleClass('fa-expand').toggleClass('fa-compress');
		    box.toggleClass('expanded');
		    var currentHeight = box.find('.box-content')
	            .height();
		    var maxHeight = $(window).height()-100;
		    if (maxHeight == currentHeight)
		        maxHeight = "220px";
		    box.find('.box-content')
                .height(maxHeight)
                .find('.dc-chart')
                .height(maxHeight); //expand height
		    body.toggleClass('body-expanded');
		    var timeout = 0;
		    if (body.hasClass('body-expanded')) {
		        timeout = 100;
		    }
		    setTimeout(function () {
		        box.toggleClass('expanded-padding');
		    }, timeout);
		    setTimeout(function () {
		        box.resize();
		        box.find('[id^=map-]').resize();
		    }, timeout + 50);
		})
		.on('click', '.collapse-link', function (e) {
		    e.preventDefault();
		    var box = $(this).closest('div.box');
		    var button = $(this).find('i');
		    var content = box.find('div.box-content');
		    content.slideToggle('fast');
		    button.toggleClass('fa-chevron-up').toggleClass('fa-chevron-down');
		    setTimeout(function () {
		        box.resize();
		        box.find('[id^=map-]').resize();
		    }, 50);
		})
		.on('click', '.close-link', function (e) {
		    e.preventDefault();
		    var content = $(this).closest('div.box');
		    content.remove();
		});   
    $('body').on('click', 'a.close-link', function (e) {
        e.preventDefault();
        CloseModalBox();
    });
});


