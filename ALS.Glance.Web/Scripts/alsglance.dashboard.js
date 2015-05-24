'use strict';
var alsglance = alsglance || {};
alsglance.dashboard = alsglance.dashboard || {};
alsglance.dashboard.patient = alsglance.dashboard.patient || {};
alsglance.dashboard.patients = alsglance.dashboard.patients || {};

alsglance.dashboard.resizeChart = function (chart) {
    var parent = $("#" + chart.anchorName()).parent();
    var newWidth = parent.width();
    var height = parent.height();
    var children = parent.children().size();
    chart.width(newWidth);
    if (children == 1) {
        chart.height(height);
    }
};

alsglance.dashboard.resize = function () {
    var i, chart;
    for (i = 0; i < dc.chartRegistry.list().length; i++) {
        chart = dc.chartRegistry.list()[i];
        chart.transitionDuration(0);
        alsglance.dashboard.resizeChart(chart);
    }
    dc.renderAll();
    for (i = 0; i < dc.chartRegistry.list().length; i++) {
        chart = dc.chartRegistry.list()[i];
        chart.transitionDuration(500);
    }
};

alsglance.dashboard.replaceResetBehaviour = function () {
    for (var i = 0; i < dc.chartRegistry.list().length; i++) {
        var chart = dc.chartRegistry.list()[i];
        chart.turnOnControls = alsglance.dashboard.turnOnControls(chart);
        chart.turnOffControls = alsglance.dashboard.turnOffControls(chart);
    }
};

alsglance.dashboard.turnOnControls = function (chart) {
    return function () {
        var node = chart.root()[0][0];
        if (node != null) {
            var grandparent = node.parentNode.parentNode;
            var _filterPrinter = chart.filterPrinter();
            d3.select(grandparent).selectAll('.reset').style('display', null);
            d3.select(grandparent).selectAll('.filter')
                .text(_filterPrinter(chart.filters())).style('display', null);
        }
        return chart;
    };
};

alsglance.dashboard.turnOffControls = function (chart) {
    return function () {
        var node = chart.root()[0][0];
        if (node != null) {
            var grandparent = node.parentNode.parentNode;
            d3.select(grandparent).selectAll('.reset').style('display', 'none');
            d3.select(grandparent).selectAll('.filter')
                .style('display', 'none').text(chart.filter());
        }
        return chart;
    };
};

alsglance.dashboard.init = function () {
    alsglance.dashboard.replaceResetBehaviour();
    $(window).resize(function () {
        if (this.resizeTO) clearTimeout(this.resizeTO);
        this.resizeTO = setTimeout(function () {
            $(this).trigger('resizeEnd');
        }, 500);
    });
    $(window).bind('resizeEnd', function () {
        //window hasn't changed size in 500ms
        alsglance.dashboard.resize();
    });
    alsglance.dashboard.resize();
};


Date.prototype.setISO8601 = function (string) {
    var regexp = "([0-9]{4})(-([0-9]{2})(-([0-9]{2})" +
        "(T([0-9]{2}):([0-9]{2})(:([0-9]{2})(\.([0-9]+))?)?" +
        "(Z|(([-+])([0-9]{2}):([0-9]{2})))?)?)?)?";
    var d = string.match(new RegExp(regexp));

    var offset = 0;
    var date = new Date(d[1], 0, 1);

    if (d[3]) {
        date.setMonth(d[3] - 1);
    }
    if (d[5]) {
        date.setDate(d[5]);
    }
    if (d[7]) {
        date.setHours(d[7]);
    }
    if (d[8]) {
        date.setMinutes(d[8]);
    }
    if (d[10]) {
        date.setSeconds(d[10]);
    }
    if (d[12]) {
        date.setMilliseconds(Number("0." + d[12]) * 1000);
    }
    if (d[14]) {
        offset = (Number(d[16]) * 60) + Number(d[17]);
        offset *= ((d[15] == '-') ? 1 : -1);
    }

    offset -= date.getTimezoneOffset();
    var time = (Number(date) + (offset * 60 * 1000));
    this.setTime(Number(time));
};

