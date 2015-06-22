//
//    Main script of DevOOPS v1.0 Bootstrap Theme
//
"use strict";

//
//  Function maked all .box selector is draggable, to disable for concrete element add class .no-drop
//
function WinMove() {
    $("div.box").not('.no-drop')
		.draggable({
		    revert: true,
		    zIndex: 2000,
		    cursor: "crosshair",
		    handle: '.box-name',
		    opacity: 0.8
		})
		.droppable({
		    tolerance: 'pointer',
		    drop: function (event, ui) {
		        var draggable = ui.draggable;
		        var droppable = $(this);
		        var dragPos = draggable.position();
		        var dropPos = droppable.position();
		        draggable.swap(droppable);
		        setTimeout(function () {
		            var dropmap = droppable.find('[id^=map-]');
		            var dragmap = draggable.find('[id^=map-]');
		            if (dragmap.length > 0 || dropmap.length > 0) {
		                dragmap.resize();
		                dropmap.resize();
		            }
		            else {
		                draggable.resize();
		                droppable.resize();
		                alsglance.dashboard.resize();
		            }
		        }, 50);
		        setTimeout(function () {
		            draggable.find('[id^=map-]').resize();
		            droppable.find('[id^=map-]').resize();
		        }, 250);
		    }
		});
}
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
//  Function set min-height of window (required for this theme)
//
function SetMinBlockHeight(elem) {
    elem.css('min-height', window.innerHeight - 49)
}
//
//  Helper for correct size of Messages page
//
function MessagesMenuWidth() {
    var W = window.innerWidth;
    var W_menu = $('#sidebar-left').outerWidth();
    var w_messages = (W - W_menu) * 16.666666666666664 / 100;
    $('#messages-menu').width(w_messages);
}

//
//  Helper for open ModalBox with requested header, content and bottom
//
//
function OpenModalBox(header, inner, bottom) {
    var modalbox = $('#modalbox');
    modalbox.find('.modal-header-name span').html(header);
    modalbox.find('.devoops-modal-inner').html(inner);
    modalbox.find('.devoops-modal-bottom').html(bottom);
    modalbox.fadeIn('fast');
    $('body').addClass("body-expanded");
}
//
//  Close modalbox
//
//
function CloseModalBox() {
    var modalbox = $('#modalbox');
    modalbox.fadeOut('fast', function () {
        modalbox.find('.modal-header-name span').children().remove();
        modalbox.find('.devoops-modal-inner').children().remove();
        modalbox.find('.devoops-modal-bottom').children().remove();
        $('body').removeClass("body-expanded");
    });
}
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
//
// Beauty Hover Plugin (backlight row and col when cell in mouseover)
//
//
(function ($) {
    $.fn.beautyHover = function () {
        var table = this;
        table.on('mouseover', 'td', function () {
            var idx = $(this).index();
            var rows = $(this).closest('table').find('tr');
            rows.each(function () {
                $(this).find('td:eq(' + idx + ')').addClass('beauty-hover');
            });
        })
		.on('mouseleave', 'td', function (e) {
		    var idx = $(this).index();
		    var rows = $(this).closest('table').find('tr');
		    rows.each(function () {
		        $(this).find('td:eq(' + idx + ')').removeClass('beauty-hover');
		    });
		});
    };
})(jQuery);
//
//  Function convert values of inputs in table to JSON data
//
//
function Table2Json(table) {
    var result = {};
    table.find("tr").each(function () {
        var oneRow = [];
        var varname = $(this).index();
        $("td", this).each(function (index) { if (index != 0) { oneRow.push($("input", this).val()); } });
        result[varname] = oneRow;
    });
    var result_json = JSON.stringify(result);
    OpenModalBox('Table to JSON values', result_json);
}



/*-------------------------------------------
	Scripts for DataTables page (tables_datatables.html)
---------------------------------------------*/

//
// Function for table, located in element with id = datatable-2
//
function TestTable2() {
    var asInitVals = [];
    var oTable = $('#datatable-2').dataTable({
        "aaSorting": [[0, "asc"]],
        "sDom": "<'box-content'<'col-sm-6'f><'col-sm-6 text-right'l><'clearfix'>>rt<'box-content'<'col-sm-6'i><'col-sm-6 text-right'p><'clearfix'>>",
        "sPaginationType": "bootstrap",
        "oLanguage": {
            "sSearch": "",
            "sLengthMenu": '_MENU_'
        },
        bAutoWidth: false
    });
    var header_inputs = $("#datatable-2 thead input");
    header_inputs.on('keyup', function () {
        /* Filter on the column (the index) of this element */
        oTable.fnFilter(this.value, header_inputs.index(this));
    })
	.on('focus', function () {
	    if (this.className == "search_init") {
	        this.className = "";
	        this.value = "";
	    }
	})
	.on('blur', function (i) {
	    if (this.value == "") {
	        this.className = "search_init";
	        this.value = asInitVals[header_inputs.index(this)];
	    }
	});
    header_inputs.each(function (i) {
        asInitVals[i] = this.value;
    });
}

/*-------------------------------------------
	Functions for Dashboard page (dashboard.html)
---------------------------------------------*/
//
// Helper for random change data (only test data for Sparkline plots)
//
function SmallChangeVal(val) {
    var new_val = Math.floor(100 * Math.random());
    var plusOrMinus = Math.random() < 0.5 ? -1 : 1;
    var result = val[0] + new_val * plusOrMinus;
    if (parseInt(result) > 1000) {
        return [val[0] - new_val]
    }
    if (parseInt(result) < 0) {
        return [val[0] + new_val]
    }
    return [result];
}


/*-------------------------------------------
	Function for Form Layout page (form layouts.html)
---------------------------------------------*/

//
// Function for Dynamically Change input size on Form Layout page
//
function FormLayoutExampleInputLength(selector) {
    var steps = [
		"col-sm-1",
		"col-sm-2",
		"col-sm-3",
		"col-sm-4",
		"col-sm-5",
		"col-sm-6",
		"col-sm-7",
		"col-sm-8",
		"col-sm-9",
		"col-sm-10",
		"col-sm-11",
		"col-sm-12"
    ];
    selector.slider({
        range: 'min',
        value: 1,
        min: 0,
        max: 11,
        step: 1,
        slide: function (event, ui) {
            if (ui.value < 1) {
                return false;
            }
            var input = $("#form-styles");
            var f = input.parent();
            f.removeClass();
            f.addClass(steps[ui.value]);
            input.attr("placeholder", '.' + steps[ui.value]);
        }
    });
}


/*-------------------------------------------
	Function for jQuery-UI page (ui_jquery-ui.html)
---------------------------------------------*/
//
// Function for make all Date-Time pickers on page
//
function AllTimePickers() {
    $('#datetime_example').datetimepicker({});
    $('#time_example').timepicker({
        hourGrid: 4,
        minuteGrid: 10,
        timeFormat: 'hh:mm tt'
    });
    $('#date3_example').datepicker({ numberOfMonths: 3, showButtonPanel: true });
    $('#date3-1_example').datepicker({ numberOfMonths: 3, showButtonPanel: true });
    $('#date_example').datepicker({});
}

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


