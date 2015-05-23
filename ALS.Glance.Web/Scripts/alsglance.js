'use strict';
var my;
var yearChart = dc.pieChart('#yearChart');
var muscleChart = dc.pieChart('#muscleChart');
var quarterChart = dc.pieChart('#quarterChart');
var timeHourChart = dc.barChart('#timeHourChart');
var dayOfWeekChart = dc.rowChart('#day-of-week-chart');
var predictionSeriesChart = dc.seriesChart('#predictionSeriesChart');
var dateRangeChart = dc.barChart('#dateRangeChart');
var timeOfDayBubbleChart = dc.bubbleChart('#yearly-bubble-chart');
var lineChart = dc.lineChart("#dc-line-chart");


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

my.LoadPatientData = function (data) {
    /* since its a csv file we need to format the data a bit */
    var dateFormat = d3.time.format('%Y/%m/%d');
    var numberFormat = d3.format('.5f');
    var hourFormat = d3.format('.0f');

    data.forEach(function (d) {
        d.dd = new Date(d.DateDate);
        var date = new Date();
        date.setISO8601(d.DateDate);
        d.dd = date;
        d.DateMonthInYear = d3.time.month(d.dd); // pre-calculate month for better performance
        // d.close = +d.close; // coerce to number
        // d.open = +d.open;
    });

    //### Create Crossfilter Dimensions and Groups
    //See the [crossfilter API](https://github.com/square/crossfilter/wiki/API-Reference) for reference.
    var ndx = crossfilter(data);
    var all = ndx.groupAll();

    // dimension by year
    var timeOfDayDimension = ndx.dimension(function (d) {
        return d.TimeTimeOfDay;
    });
    // maintain running tallies by year as filters are applied or removed
    var timeOfDayGroup = timeOfDayDimension.group().reduce(
        /* callback for when data is added to the current filter results */
        function (p, v) {
            ++p.count;
            p.sumAUC += v.AUC;
            p.avgAUC = p.sumAUC / p.count;
            // p.absGain += v.close - v.open;
            // p.fluctuation += Math.abs(v.close - v.open);
            // p.sumIndex += (v.open + v.close) / 2;
            // p.avgIndex = p.sumIndex / p.count;
            // p.percentageGain = p.avgIndex ? (p.absGain / p.avgIndex) * 100 : 0;
            // p.fluctuationPercentage = p.avgIndex ? (p.fluctuation / p.avgIndex) * 100 : 0;
            return p;
        },
        /* callback for when data is removed from the current filter results */
        function (p, v) {
            --p.count;
            p.sumAUC -= v.AUC;
            p.avgAUC = p.count ? p.sumAUC / p.count : 0;
            // p.absGain -= v.close - v.open;
            // p.fluctuation -= Math.abs(v.close - v.open);
            // p.sumIndex -= (v.open + v.close) / 2;
            // p.avgIndex = p.count ? p.sumIndex / p.count : 0;
            // p.percentageGain = p.avgIndex ? (p.absGain / p.avgIndex) * 100 : 0;
            // p.fluctuationPercentage = p.avgIndex ? (p.fluctuation / p.avgIndex) * 100 : 0;
            return p;
        },
        /* initialize p */
        function () {
            return {
                sumAUC: 0,
                avgAUC: 0,
                count: 0
                // absGain: 0,
                // fluctuation: 0,
                // fluctuationPercentage: 0,
                // sumIndex: 0,
                // avgIndex: 0,
                // percentageGain: 0
            };
        }
    );

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
    var dateDimension = ndx.dimension(function (d) {
        return d.dd;
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

    //#region Year

    var yearDimension = ndx.dimension(function (d) {
        return d.DateYear;
    });
    // produce counts records in the dimension
    var yearGroup = yearDimension.group().reduceCount();

    yearChart
        .dimension(yearDimension) // set dimension
        .group(yearGroup);

    //#endregion

    // determine a histogram of percent changes
    var timeHourDimension = ndx.dimension(function (d) {
        return d.TimeHour;
    });
    var timeHourGroup = timeHourDimension.group();

    // counts per weekday
    var dayOfWeek = ndx.dimension(function (d) {
        return '1' + '.' + d.DateDayOfWeek;
    });
    var dayOfWeekGroup = dayOfWeek.group();

    //### Define Chart Attributes
    //Define chart attributes using fluent methods. See the
    // [dc API Reference](https://github.com/dc-js/dc.js/blob/master/web/docs/api-latest.md) for more information
    //

    //#region Bubble Chart

    timeOfDayBubbleChart
        //.width(800) // (optional) define chart width, :default = 200
        //.height(250) // (optional) define chart height, :default = 200
        .transitionDuration(1500) // (optional) define chart transition duration, :default = 750
        //.margins({ top: 10, right: 50, bottom: 40, left: 40 })
        .dimension(timeOfDayDimension)
        //Bubble chart expect the groups are reduced to multiple values which would then be used
        //to generate x, y, and radius for each key (bubble) in the group
        .group(timeOfDayGroup)
        .colors(colorbrewer.RdYlGn[9]) // (optional) define color function or array for bubbles
        .colorDomain([-500, 500]) //(optional) define color domain to match your data domain if you want to bind data or
        //color
        //##### Accessors
        //Accessor functions are applied to each value returned by the grouping
        //
        //* `.colorAccessor` The returned value will be mapped to an internal scale to determine a fill color
        //* `.keyAccessor` Identifies the `X` value that will be applied against the `.x()` to identify pixel location
        //* `.valueAccessor` Identifies the `Y` value that will be applied agains the `.y()` to identify pixel location
        //* `.radiusValueAccessor` Identifies the value that will be applied agains the `.r()` determine radius size,
        //*     by default this maps linearly to [0,100]
        .colorAccessor(function (d) {
            return d.value.count;
        })
        .keyAccessor(function (p) {
            return p.value.count;
        })
        .valueAccessor(function (p) {
            return p.value.sumAUC;
        })
        .radiusValueAccessor(function (p) {
            return p.value.avgAUC * 10000;
        })
        .maxBubbleRelativeSize(0.3)
        .x(d3.scale.linear().domain([-2500, 2500]))
        .y(d3.scale.linear().domain([-100, 100]))
        .r(d3.scale.linear().domain([0, 4000]))
        //##### Elastic Scaling
        //`.elasticX` and `.elasticX` determine whether the chart should rescale each axis to fit data.
        //The `.yAxisPadding` and `.xAxisPadding` add padding to data above and below their max values in the same unit
        //domains as the Accessors.
        .elasticY(true)
        .elasticX(true)
        .yAxisPadding(20)
        .xAxisPadding(50)
        .renderHorizontalGridLines(true) // (optional) render horizontal grid lines, :default=false
        .renderVerticalGridLines(true) // (optional) render vertical grid lines, :default=false
        .xAxisLabel('Measurements') // (optional) render an axis label below the x axis
        .yAxisLabel('AUC (Sum)') // (optional) render a vertical axis lable left of the y axis
        //#### Labels and  Titles
        //Labels are displaed on the chart for each bubble. Titles displayed on mouseover.
        .renderLabel(true) // (optional) whether chart should render labels, :default = true
        .label(function (p) {
            return p.key;
        })
        .renderTitle(true) // (optional) whether chart should render titles, :default = false
        .title(function (p) {
            return [
                p.key,
                'Total: ' + p.value.count,
                'Avg AUC: ' + numberFormat(p.value.avgAUC),
                'Sum AUC: ' + numberFormat(p.value.sumAUC)
            ].join('\n');
        })
        //#### Customize Axis
        //Set a custom tick format. Note `.yAxis()` returns an axis object, so any additional method chaining applies
        //to the axis, not the chart.
        .yAxis().tickFormat(function (v) {
            return v;
        });
    //#endregion

    //#region Quarter Chart

    var quarter = ndx.dimension(function (d) {
        return 'Q' + d.DateQuarter;
    });
    var quarterGroup = quarter.group().reduceCount();

    quarterChart.width(180)
        .height(180)
        .radius(80)
        .innerRadius(50)
        .dimension(quarter)
        .group(quarterGroup);

    //#endregion

    //#### Row Chart
    dayOfWeekChart.width(180)
        .height(180)
        .margins({ top: 20, left: 30, right: 10, bottom: 20 })
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
    timeHourChart.width(420)
        .height(180)
        .margins({ top: 10, right: 50, bottom: 30, left: 40 })
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
        .x(d3.scale.linear().domain([-1, 22]))
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
    var predictionDimension = ndx.dimension(function (d) {
        return [d.PatientName, d.dd, d.DateYear];
    });

    var predictionGroup = predictionDimension.group().reduceSum(function (d) {
        return +d.AUC;
    });


    predictionSeriesChart
        .width(768)
        .height(480)
        //.chart(function(c) { return dc.lineChart(c).interpolate('basis'); })
        .x(d3.time.scale().domain([new Date(2012, 0, 1), new Date(2016, 11, 31)]))
        .brushOn(false)
        .yAxisLabel("")
        .xAxisLabel("Date")
        .clipPadding(10)
        .elasticY(true)
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
        .legend(dc.legend().x(580).y(300).itemHeight(13).gap(5).legendWidth(170).itemWidth(170)).title(function (d) {
            return dateFormat(d.key[1]) + ':\n' + d.value;
        });

    dateRangeChart.width(800)
        .height(40)
        .margins({ top: 0, right: 50, bottom: 20, left: 40 })
        .dimension(dateMonthInYearDimension)
        .group(volumeByMonthGroup)
        .centerBar(true)
        .gap(1)
        .x(d3.time.scale().domain([new Date(2012, 0, 1), new Date(2016, 11, 31)]))
        .round(d3.time.month.round)
        .alwaysUseRounding(true)
        .xUnits(d3.time.months);

    //#endregion

    lineChart.width(350)
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
    dc.dataCount('.dc-data-count')
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
    dc.dataTable('.dc-data-table')
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
                    return d.DateDate;
                }
            },
            'PatientName', // ...
            'TimeTimeOfDay', // ...
            'MuscleName',
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
            return d.dd;
        })
        // (optional) sort order, :default ascending
        .order(d3.ascending)
        // (optional) custom renderlet to post-process chart using D3
        .on("renderlet.post-process", function (table) {
            table.selectAll('.dc-table-group ').classed('info', true);
        });

    //#endregion

    //#### Rendering
    //simply call renderAll() to render all charts on the page
    dc.renderAll();

    $("#reset").click(function () {
        reset();
    });

    $("#years input[type=checkbox]").change(function () {
        filterYears();
        dc.redrawAll();
    });
    $('#muscles input[type=radio]').change(function () {
        filterMuscle();
        dc.redrawAll();
    });

    function reset() {
        dc.filterAll();
        resetYears();
        $('#AT').prop("checked", true);
        filterMuscle();
        dc.redrawAll();
    }

    function resetYears() {
        $('#years input').each(function (index, value) {
            $(value).prop("checked", true);
        });
        filterYears();
    }

    function filterMuscle() {
        var muscle = $('#muscles input:checked').attr('id');
        muscleChart.filterAll();
        muscleChart.filter([muscle]);
    }

    function filterYears() {
        var years = [];
        $('#years input:checked').each(function (index, value) {
            years.push(parseInt($(value).attr('id')));
        });
        if (years.length == 0) {
            resetYears();
            return;
        }

        yearDimension.filterAll();
        yearDimension.filterFunction(function (d) {
            return years.indexOf(d) != -1;
        });
    }
};
