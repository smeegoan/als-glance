var dateDimension;
var predictionDimension;
var muscleChart = dc.pieChart('#muscleChart');
var quarterChart = dc.pieChart('#quarterChart');
var timeHourChart = dc.barChart('#timeHourChart');
var timeOfDayChart = dc.rowChart('#timeOfDayChart');
var predictionSeriesChart = dc.seriesChart('#predictionSeriesChart');
var dateRangeChart = dc.barChart('#dateRangeChart');
var averageAucChart = dc.lineChart("#averageAucChart");

alsglance.dashboard.patient.reset = function () {
    dc.filterAll();
    alsglance.dashboard.patient.datePicker();
    $('#AT').addClass("active");
    $('#FCR').removeClass("active");
    $('#SCM').removeClass("active");
    alsglance.dashboard.patient.filterMuscle("AT");
    dc.redrawAll();
};


alsglance.dashboard.patient.filterMuscle = function (muscle) {
    muscleChart.filterAll();
    muscleChart.filter([muscle]);
};

alsglance.dashboard.patient.datePicker = function () {
    var minDate = moment().subtract(4, 'year');
    var maxDate = moment().add(1, 'year');
    $('#reportrange span').html(minDate.format('MMMM D, YYYY') + ' - ' + maxDate.format('MMMM D, YYYY'));
    $('#reportrange').daterangepicker({
        format: 'MM/DD/YYYY',
        startDate: minDate,
        endDate: moment(),
        minDate: minDate,
        maxDate: maxDate,
        showDropdowns: true,
        showWeekNumbers: false,
        timePicker: false,
        timePickerIncrement: 1,
        timePicker12Hour: true,
        ranges: {
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
            'Last Year': [moment().subtract(1, 'year'), moment()]
        },
        opens: 'left',
        drops: 'down',
        buttonClasses: ['btn', 'btn-sm'],
        applyClass: 'btn-primary',
        cancelClass: 'btn-default',
        separator: ' to ',
        locale: {
            applyLabel: 'Submit',
            cancelLabel: 'Cancel',
            fromLabel: 'From',
            toLabel: 'To',
            customRangeLabel: 'Custom',
            daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
            monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            firstDay: 1
        }
    }, function (start, end, label) {

        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
        predictionDimension.filter(null);
        predictionDimension.filter(function (d) {
            var date = d[1].valueOf();
            return date > start.valueOf() && date < end.valueOf();
        });
        dc.redrawAll();
    });
};

alsglance.dashboard.patient.init = function () {
    $("#reset").click(function () {
        alsglance.dashboard.patient.reset();
    });
    $.each($('#muscles .btn'), function (index, value) {
        var id = $(value).attr('id');
        $(value).click(id, function () {
            alsglance.dashboard.patient.filterMuscle(id);
            dc.redrawAll();
        });
    });
    alsglance.dashboard.init();
    alsglance.dashboard.patient.reset();
};


alsglance.dashboard.patient.addPredictions = function (data) {
    var muscles = [];

    //data =JSON.flatten(data);
    data = data["value"];
    data.forEach(function (entry) {
        if (muscles[entry.MuscleAbbreviation] == null)
            muscles[entry.MuscleAbbreviation] = [];
        if (muscles[entry.MuscleAbbreviation][entry.TimeTimeOfDay] == null)
            muscles[entry.MuscleAbbreviation][entry.TimeTimeOfDay] = [];
        muscles[entry.MuscleAbbreviation][entry.TimeTimeOfDay].push([new Date(entry.DateDate).getTime(), entry.AUC]);
    });
    for (var muscle in muscles) {
        for (var timeOfDay in muscles[muscle]) {
            var measurements = muscles[muscle][timeOfDay];
            var equation = regression('linear', measurements).equation;
            var startDate = moment(measurements[0][0]);
            for (var i = 1; i < 36; i++) {
                startDate = startDate.add(1, "months");
                var ticks = startDate.valueOf();
                data.push({
                    DateDate: startDate.format("YYYY-MM-DD HH:mm"),
                    AUC: equation[0] * ticks + equation[1],
                    DateMonthName: startDate.format("MMMM"),
                    DateMonth: startDate.format("M"),
                    DateYear: parseInt(startDate.format("YYYY")),
                    DateQuarter: startDate.quarter(),
                    PatientName: "Prediction",
                    DateDayOfWeek: startDate.format("dddd"),
                    TimeTimeOfDay: timeOfDay,
                    MuscleName: muscle,
                    MuscleAbbreviation: muscle
                });
            }
        };
    };
    return data;
};

alsglance.dashboard.patient.load = function (data) {
    /* since its a csv file we need to format the data a bit */
    var dateFormat = d3.time.format('%Y/%m/%d');
    var numberFormat = d3.format('.5f');
    var hourFormat = d3.format('.0f');

    data.forEach(function (d) {
        var date = new Date();
        date.setISO8601(d.DateDate);
        d.DateDate = date;
        d.DateMonthInYear = d3.time.month(d.DateDate); // pre-calculate month for better performance
    });

    //### Create Crossfilter Dimensions and Groups
    //See the [crossfilter API](https://github.com/square/crossfilter/wiki/API-Reference) for reference.
    var ndx = crossfilter(data);
    var all = ndx.groupAll();

    var hourDimension = ndx.dimension(function (d) {
        return d.TimeHour;
    });
    var hourAverageGroup = hourDimension.group().reduce(
        function (p, v) {
            ++p.count;
            p.total += v.AUC;
            p.avg = p.total / p.count;
            return p;
        },
        function (p, v) {
            --p.count;
            p.total -= v.AUC;
            p.avg = p.count ? p.total / p.count : 0;
            return p;
        },
        function () {
            return { count: 0, total: 0, avg: 0 };
        }
    );


    // dimension by full date
    dateDimension = ndx.dimension(function (d) {
        return d.DateDate;
    });

    // dimension by month
    var dateMonthInYearDimension = ndx.dimension(function (d) {
        return d.DateMonthInYear;
    });
    // group by total volume within move, and scale down result
    var volumeByMonthGroup = dateMonthInYearDimension.group().reduceSum(function (d) {
        return d.AUC;
    });

    //#region Muscle

    var muscleDimension = ndx.dimension(function (d) {
        return d.MuscleAbbreviation;
    });
    // produce counts records in the dimension
    var muscleGroup = muscleDimension.group().reduceCount();


    muscleChart
        .dimension(muscleDimension) // set dimension
        .group(muscleGroup);

    //#endregion

    // determine a histogram of percent changes
    var timeHourDimension = ndx.dimension(function (d) {
        return d.TimeHour;
    });
    var timeHourGroup = timeHourDimension.group();

    // counts per weekday
    var dayOfWeek = ndx.dimension(function (d) {
        return '1' + '.' + d.TimeTimeOfDay;
    });
    var dayOfWeekGroup = dayOfWeek.group();


    //#region Quarter Chart

    var quarter = ndx.dimension(function (d) {
        return 'Q' + d.DateQuarter;
    });
    var quarterGroup = quarter.group().reduceCount();

    quarterChart
        .radius(90)
        .innerRadius(50)
        .dimension(quarter)
        .group(quarterGroup);

    //#endregion

    //#### Row Chart
    timeOfDayChart.width(180)
      //  .height(180)
        //.margins({ top: 20, left: 30, right: 10, bottom: 20 })
        .group(dayOfWeekGroup)
        .dimension(dayOfWeek)
        .label(function (d) {
            return d.key.split('.')[1];
        })
        // title sets the row text
        .title(function (d) {
            return d.value;
        })
        .elasticX(true)
        .xAxis().ticks(4);

    //#### Bar Chart
    // Create a bar chart and use the given css selector as anchor. You can also specify
    // an optional chart group for this chart to be scoped within. When a chart belongs
    // to a specific group then any interaction with such chart will only trigger redraw
    // on other charts within the same chart group.
    /* dc.barChart('#volume-month-chart') */
    timeHourChart//.width(420)
        //.height(180)
        //.margins({ top: 10, right: 50, bottom: 30, left: 40 })
        .dimension(timeHourDimension)
        .group(timeHourGroup)
        .elasticY(true)
        // (optional) whether bar should be center to its x value. Not needed for ordinal chart, :default=false
        .centerBar(true)
        // (optional) set gap between bars manually in px, :default=2
        .gap(1)
        // (optional) set filter brush rounding
        .round(dc.round.floor)
        .alwaysUseRounding(true)
        .x(d3.scale.linear().domain([0, 23]))
        .renderHorizontalGridLines(true)
        // customize the filter displayed in the control span
        .filterPrinter(function (filters) {
            var filter = filters[0], s = '';
            s += hourFormat(filter[0]) + 'h -> ' + hourFormat(filter[1]) + 'h';
            return s;
        });

    // Customize axis
    timeHourChart.xAxis().tickFormat(
        function (v) { return v + 'h'; });
    timeHourChart.yAxis().ticks(6);

    //#region Prediction Chart
    predictionDimension = ndx.dimension(function (d) {
        return [d.PatientName, d.DateDate, d.DateYear];
    });

    var predictionGroup = predictionDimension.group().reduceSum(function (d) {
        return +d.AUC;
    });


    predictionSeriesChart
           .margins({ top: 20, right: 30, bottom: 20, left: 60 })
   .width(460)
        .height(180)
        //.chart(function(c) { return dc.lineChart(c).interpolate('basis'); })
        .x(d3.time.scale().domain([new Date(2012, 0, 1), new Date(2016, 11, 31)]))
        .y(d3.scale.linear().domain([0.009, 0.03]))
        .brushOn(false)
        //.yAxisLabel("")
        //.xAxisLabel("Date")
        //.clipPadding(10)
        // .elasticY(true)
        .dimension(predictionDimension)
        .group(predictionGroup)
        .rangeChart(dateRangeChart)
        .seriesAccessor(function (d) {
            return d.key[0];
        })
        .keyAccessor(function (d) {
            return +d.key[1];
        })
        .valueAccessor(function (d) {
            return +d.value;
        })
        .legend(dc.legend().x(280).y(20).itemHeight(13).gap(5).legendWidth(170).itemWidth(170)).title(function (d) {
            return dateFormat(d.key[1]) + ':\n' + d.value;
        });

    dateRangeChart
        .width(460)
       .height(60)
           .mouseZoomable(true)
     .margins({ top: 10, right: 50, bottom: 20, left: 60 })
        .dimension(dateMonthInYearDimension)
        .group(volumeByMonthGroup)
        .centerBar(true)
        .gap(1)
        .x(d3.time.scale().domain([new Date(2012, 0, 1), new Date(2016, 11, 31)]))
        .round(d3.time.month.round)
        .alwaysUseRounding(true)
        .xUnits(d3.time.months).on("filtered", function (chart) {
            var filters = chart.filters();
            if (filters.length > 0) {
                var range = filters[0];
                $('#reportrange span').html(moment(range[0]).format('MMMM D, YYYY') + ' - ' + moment(range[1]).format('MMMM D, YYYY'));
            }
        });

    //#endregion

    averageAucChart.width(350)
        .margins({ top: 30, right: 50, bottom: 25, left: 40 })
        .height(200)
        .dimension(hourDimension)
        .group(hourAverageGroup)
        .x(d3.scale.linear().domain([0, 23]))
        .valueAccessor(function (d) {
            return d.value.avg;
        })
        .renderHorizontalGridLines(true)
        .elasticY(true)
        .elasticX(true)
		     .brushOn(false)
        .filterPrinter(function (filters) {
            var filter = filters[0], s = '';
            s += filter[0] + ' -> ' + filter[1];
            return s;
        }).title(function (d) {
            var value = d.value.avg ? d.value.avg : d.value;
            if (isNaN(value)) {
                value = 0;
            }
            return d.key + 'h\n' + numberFormat(value);
        });



    //#region DataTable
    /*
    //#### Data Count
    // Create a data count widget and use the given css selector as anchor. You can also specify
    // an optional chart group for this chart to be scoped within. When a chart belongs
    // to a specific group then any interaction with such chart will only trigger redraw
    // on other charts within the same chart group.
    <div id='data-count'>
        <span class='filter-count'></span> selected out of <span class='total-count'></span> records
    </div>
    */
    dc.dataCount('#dc-data-count')
        .dimension(ndx)
        .group(all)
        // (optional) html, for setting different html for some records and all records.
        // .html replaces everything in the anchor with the html given using the following function.
        // %filter-count and %total-count are replaced with the values obtained.
        .html({
            some: '<strong>%filter-count</strong> selected out of <strong>%total-count</strong> records' +
                ' | <a href=\'javascript:dc.filterAll(); dc.renderAll();\'\'>Reset All</a>',
            all: 'All records selected. Please click on the graph to apply filters.'
        });

    /*
    //#### Data Table
    // Create a data table widget and use the given css selector as anchor. You can also specify
    // an optional chart group for this chart to be scoped within. When a chart belongs
    // to a specific group then any interaction with such chart will only trigger redraw
    // on other charts within the same chart group.
    <!-- anchor div for data table -->
    <div id='data-table'>
        <!-- create a custom header -->
        <div class='header'>
            <span>Date</span>
            <span>Open</span>
            <span>Close</span>
            <span>Change</span>
            <span>Volume</span>
        </div>
        <!-- data rows will filled in here -->
    </div>
    */
    dc.dataTable('#dc-data-table')
        .dimension(dateDimension)
        // data table does not use crossfilter group but rather a closure
        // as a grouping function
        .group(function (d) {
            return d.DateYear + '/' + d.DateMonthName;
        })
        .size(100) // (optional) max number of records to be shown, :default = 25
        // There are several ways to specify the columns; see the data-table documentation.
        // This code demonstrates generating the column header automatically based on the columns.
        .columns([
            {
                label: 'Date',
                format: function (d) {
                    return moment(d.DateDate).format("YYYY-MM-DD");
                }
            },
            'PatientName', // ...
            'TimeTimeOfDay', // ...
            {
                label: 'Time (h)', // desired format of column name 'Change' when used as a label with a function.
                format: function (d) {
                    return d.TimeHour;
                }
            },
            'AUC' // d['volume'], ie, a field accessor; capitalized automatically
        ])

        // (optional) sort using the given field, :default = function(d){return d;}
        .sortBy(function (d) {
            return d.DateDate;
        })
        // (optional) sort order, :default ascending
        .order(d3.ascending)
        // (optional) custom renderlet to post-process chart using D3
        .on("renderlet.post-process", function (table) {
            //table.selectAll('.dc-table-group ').classed('info', true);
        });

    //#endregion

    //#### Rendering
    //simply call renderAll() to render all charts on the page
    dc.renderAll();
    $(".loading").remove();

};
