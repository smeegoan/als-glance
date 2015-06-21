﻿'use strict';
var aucBubbleChart, muscleChart, quarterChart, timeHourChart, timeOfDayChart, predictionSeriesChart, dateRangeChart, emgChart;

var alsglance = alsglance || {};
alsglance.dashboard = alsglance.dashboard || {
    showPatientsHelpButton: function () {
        $("#patients_filter").attr("data-position", "bottom");
        $("#patients_filter").attr("data-step", "1");
        $("#patients_filter").attr("data-intro", "Click here to filter by the patient name.");
        $("#helpPlaceHolder").html('<a href="javascript:void(0);" onclick="javascript:alsglance.dashboard.showHelp(\'Patients\');">Help</a>');
    },
    showPatientHelpButton: function () {
        $("#helpPlaceHolder").html('<a href="javascript:void(0);" onclick="javascript:alsglance.dashboard.showHelp(\'Patient\');">Help</a>');
    },
    showHelp: function (category) {
        introJs().start();
        analytics.logUiEvent("showHelp", category, "navbar");
    },
    resizeChart:
          function (chart) {
              if (chart == null) {
                  return;
              }
              var parent = $("#" + chart.anchorName()).parent();
              var width = parent.width();
              if (width == null) {
                  return;
              }
              var height = parent.height();
              if (chart.hasOwnProperty("rangeChart")) {
                  var range = chart.rangeChart();
                  if (range != null) {
                      height -= range.height();
                      $("#" + chart.anchorName()).height(height); //fix for ranged charts
                  }
              }
              var children = parent.children().size();
              chart.width(width);
              if (children == 1) {
                  chart.height(height);
              }
              if (chart.hasOwnProperty("radius")) //pie chart
              {
                  chart.radius(Math.min(width, height) / 2.5);
                  chart.innerRadius(chart.radius() / 2);
              }
          },
    resize: function () {
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
        if (emgChart != null)
            emgChart.resize();
    },
    replaceResetBehaviour: function () {
        for (var i = 0; i < dc.chartRegistry.list().length; i++) {
            var chart = dc.chartRegistry.list()[i];
            chart.turnOnControls = alsglance.dashboard.turnOnControls(chart);
            chart.turnOffControls = alsglance.dashboard.turnOffControls(chart);
        }
    },
    turnOnControls: function (chart) {
        return function () {
            var node = chart.root()[0][0];
            if (node != null) {
                var grandparent = node.parentNode.parentNode;
                var filterPrinter = chart.filterPrinter();
                d3.select(grandparent).selectAll('.reset').style('display', null);
                d3.select(grandparent).selectAll('.filter')
                    .text(filterPrinter(chart.filters())).style('display', null);
            }
            return chart;
        };
    },
    init: function () {
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
    },
    turnOffControls: function (chart) {
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
    },
};

alsglance.dashboard.patients = alsglance.dashboard.patients || {
    /**
 * Module for displaying "Waiting for..." dialog using Bootstrap
 *
 * @author Eugene Maslovich <ehpc@em42.ru>
 */

    waitingDialog: (function ($) {

        // Creating modal dialog's DOM
        var $dialog = $(
            '<div class="modal" data-backdrop="false" data-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true" style="padding-top:15%; overflow-y:visible;">' +
            '<div class="modal-dialog modal-m">' +
            '<div class="modal-content">' +
                '<div class="modal-header"><h6 style="margin:0;"></h6></div>' +
                '<div class="modal-body">' +
                    '<div class="progress progress-striped active" style="margin-bottom:0; height:16px;"><div class="progress-bar" style="width: 100%"></div></div>' +
                '</div>' +
            '</div></div></div>');

        return {
            /**
             * Opens our dialog
             * @param message Custom message
             * @param options Custom options:
             * 				  options.dialogSize - bootstrap postfix for dialog size, e.g. "sm", "m";
             * 				  options.progressType - bootstrap postfix for progress bar type, e.g. "success", "warning".
             */
            show: function (message, options) {
                // Assigning defaults
                var settings = $.extend({
                    dialogSize: 'm',
                    progressType: ''
                }, options);
                if (typeof message === 'undefined') {
                    message = 'Loading';
                }
                if (typeof options === 'undefined') {
                    options = {};
                }
                // Configuring dialog
                $dialog.find('.modal-dialog').attr('class', 'modal-dialog').addClass('modal-' + settings.dialogSize);
                $dialog.find('.progress-bar').attr('class', 'progress-bar');
                if (settings.progressType) {
                    $dialog.find('.progress-bar').addClass('progress-bar-' + settings.progressType);
                }
                $dialog.find('h6').html('Loading...<br/><br/><b>' + message + '</b>');
                // Opening dialog
                $dialog.modal();
                analytics.logUiEvent("viewResume", "Patient", "dashboard", message);
            },
            /**
             * Closes dialog
             */
            hide: function () {
                $dialog.modal('hide');
            }
        };

    })(jQuery),
    load: function (data, min, max) {
        aucBubbleChart = dc.bubbleChart('#aucBubbleChart');

        var numberFormat = d3.format('.5f');
        data = data["value"];
        data.forEach(function (d) {
            var date = new Date();
            date.setISO8601(d.BornOn);
            d.age = moment().diff(date, 'years');
        });

        //### Create Crossfilter Dimensions and Groups
        //See the [crossfilter API](https://github.com/square/crossfilter/wiki/API-Reference) for reference.
        var ndx = crossfilter(data);

        var timeOfDayDimension = ndx.dimension(function (d) {
            return d.Sex;
        });
        // maintain running tallies by year as filters are applied or removed
        var timeOfDayGroup = timeOfDayDimension.group().reduce(
            /* callback for when data is added to the current filter results */
            function (p, v) {
                ++p.count;
                p.sumAge += v.age;
                p.avgAge = p.sumAge / p.count;
                return p;
            },
            /* callback for when data is removed from the current filter results */
            function (p, v) {
                --p.count;
                p.sumAge -= v.age;
                p.avgAge = p.count ? p.sumAge / p.count : 0;
                return p;
            },
            /* initialize p */
            function () {
                return {
                    sumAge: 0,
                    avgAge: 0,
                    count: 0
                };
            }
        );

        aucBubbleChart
            .transitionDuration(1500) // (optional) define chart transition duration, :default = 750
            .margins({ top: 10, right: 50, bottom: 40, left: 50 })
            .dimension(timeOfDayDimension)
            //Bubble chart expect the groups are reduced to multiple values which would then be used
            //to generate x, y, and radius for each key (bubble) in the group
            .group(timeOfDayGroup)
            .colors(['rgb(49,130,189)', 'rgb(247,104,161)']) // (optional) define color function or array for bubbles
            .colorAccessor(function (d) {
                var res = d.key == "M" ? 0 : 1;
                return res;
            })
            .keyAccessor(function (p) {
                return p.value.avgAge;
            })
            .valueAccessor(function (p) {
                return p.value.count;
            })
            .radiusValueAccessor(function (p) {
                return p.value.avgAge;
            })
            .maxBubbleRelativeSize(0.3)
            .x(d3.scale.linear().domain([min, max]))
            .y(d3.scale.linear().domain([-100, 100]))
            .r(d3.scale.linear().domain([0, 4000]))
            .elasticY(true)
            //.elasticX(true)
            .yAxisPadding(20)
            .xAxisPadding(50)
            .renderHorizontalGridLines(true) // (optional) render horizontal grid lines, :default=false
            .renderVerticalGridLines(true) // (optional) render vertical grid lines, :default=false
            .xAxisLabel('Average Age') // (optional) render an axis label below the x axis
            .yAxisLabel('# Patients') // (optional) render a vertical axis lable left of the y axis
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
                    '# Patients: ' + p.value.count,
                    'Average Age: ' + numberFormat(p.value.avgAge)
                ].join('\n');
            }).yAxis().tickFormat(function (v) {
                return v;
            });
        //#endregion

        //#### Rendering
        //simply call renderAll() to render all charts on the page
        dc.renderAll();
        $(".loading").remove();

    }
};

alsglance.dashboard.patient = alsglance.dashboard.patient || {
    saveSettings: function () {
        var filters = [];
        for (var i = 0; i < dc.chartRegistry.list().length; i++) {
            var chart = dc.chartRegistry.list()[i];
            for (var j = 0; j < chart.filters().length; j++) {
                filters.push({ ChartID: chart.chartID(), Filter: chart.filters()[j] });
            }
        }
        alsglance.dashboard.settings["P" + alsglance.dashboard.patientId] = filters;
        alsglance.dashboard.settings.colorScheme = selectedScheme;
        var entity = {};
        entity.UserId = alsglance.dashboardUserId;
        entity.ApplicationId = alsglance.applicationId;
        entity.Value = JSON.stringify(alsglance.dashboard.settings);
        $.ajax({
            type: "POST",
            url: alsglance.baseUri + "ApplicationSettings",
            data: JSON.stringify(
                entity
            ),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                toastr.success('Filter changes were successfully saved.', 'ALS Glance');
            },
            failure: function (errMsg) {
                toastr.error(errMsg, 'ALS Glance');
            }
        });
        analytics.logUiEvent("saveSettings", "Patient", "dashboard");
    },
    applyFilters: function (filterObjects) {
        if (filterObjects == null || filterObjects.length == 0)
            return;
        var id, filter;
        for (var j = 0; j < filterObjects.length; j++) {
            id = filterObjects[j].ChartID;
            filter = filterObjects[j].Filter;
            if (id == 1)
                alsglance.dashboard.patient.filterMuscle(filter[0]);
            else if (id == 6) { //dateRangeChart must be handled in a different way
                dc.chartRegistry.list()[id - 1].filterAll();
                dc.chartRegistry.list()[id - 1].filter(dc.filters.RangedFilter(moment(filter[0]).valueOf(), moment(filter[1]).valueOf()));
            } else if (id == 3) {
                dc.chartRegistry.list()[id - 1].filterAll();
                dc.chartRegistry.list()[id - 1].filter(dc.filters.RangedFilter(filter[0], filter[1]));
            }

        }
        for (var i = 0; i < filterObjects.length; i++) {
            id = filterObjects[i].ChartID;
            if (id == 1 || id == 3 || id == 6) {
            }
            else {
                filter = filterObjects[i].Filter;
                dc.chartRegistry.list()[id - 1].filter(filter);
            }

        }
        dc.redrawAll();
    },
    reset: function () {
        dc.filterAll();
        alsglance.dashboard.patient.datePicker();
        alsglance.dashboard.patient.filterMuscle("AT");
        dc.redrawAll();
        if (emgChart != null) {
            emgChart.resetZoom();
        }
        analytics.logUiEvent("reset", "Patient", "dashboard");
    },
    filterMuscle: function (muscle) {
        $('#AT').removeClass("active");
        $('#FCR').removeClass("active");
        $('#SCM').removeClass("active");
        $('#' + muscle).addClass("active");
        muscleChart.filterAll();
        muscleChart.filter([muscle]);
        analytics.logUiEvent("filterMuscle", "Patient", "dashboard", muscle);
    },
    init: function () {
        $("#reset").click(function () {
            alsglance.dashboard.patient.reset();
        });
        $("#save").click(function () {
            alsglance.dashboard.patient.saveSettings();
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
    },
    addPredictions: function (data) {
        var muscles = [];

        //data =JSON.flatten(data);
        data = data.value;
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
                        DateYear: parseInt(startDate.format("YYYY")),
                        DateQuarter: startDate.quarter(),
                        PatientName: "Prediction",
                        DateDayOfWeek: startDate.format("dddd"),
                        TimeTimeOfDay: timeOfDay,
                        TimeHour: 24, //invalid hour so it will be excluded from hour chart
                        MuscleName: muscle,
                        MuscleAbbreviation: muscle
                    });
                }
            };
        };
        return data;
    },
    loadEMG: function (data) {
        emgChart = new Dygraph(document.getElementById("emgChart"), data, {
            // customBars: true,
            //   title: 'Daily Temperatures in New York vs. San Francisco',
            labels: ['Time', 'µV'],
            //xlabel: 'Time',
            //ylabel: 'EMG Signal',
            legend: 'true',
            colors: [colorbrewer[selectedScheme][numClasses][3]],
            labelsDivStyles: {
                'textAlign': 'right'
            }
            //,showRangeSelector: true
        });
        emgChart.anchorName = function () {
            return "emgChart";
        };
    },
    load: function (data, minYear, maxYear) {
        muscleChart = dc.pieChart('#muscleChart');
        quarterChart = dc.pieChart('#quarterChart');
        timeHourChart = dc.barChart('#timeHourChart');
        timeOfDayChart = dc.rowChart('#timeOfDayChart');
        predictionSeriesChart = dc.seriesChart('#predictionSeriesChart');
        dateRangeChart = dc.barChart('#dateRangeChart');
        var dateDimension;
        var predictionDimension;

        alsglance.dashboard.patient.datePicker = function () {
            var minDate = moment("01-01-" + minYear, "MM-DD-YYYY");
            var maxDate = moment("12-31-" + ++maxYear, "MM-DD-YYYY");
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
                dateRangeChart.filterAll();
                dateRangeChart.filter(dc.filters.RangedFilter(start.valueOf(), end.valueOf()));
                dc.redrawAll();
                analytics.logUiEvent("filterDates", "Patient", "dashboard", { start: start.format('MMMM D, YYYY'), end: end.format('MMMM D, YYYY') });
            });
        };





        /* since its a csv file we need to format the data a bit */
        var dateFormat = d3.time.format('%Y/%m/%d');
        var hourFormat = d3.format('.0f');

        data.forEach(function (d) {
            var date = new Date();
            date.setISO8601(d.DateDate);
            d.DateDate = date;
            d.DateMonthInYear = d3.time.month(d.DateDate); // pre-calculate month for better performance

        });

        //### Create Crossfilter Dimensions and Groups
        var ndx = crossfilter(data);
        var all = ndx.groupAll();

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
       //.width(460)
            .height(160)
            //.chart(function(c) { return dc.lineChart(c).interpolate('basis'); })
            .x(d3.time.scale().domain([new Date(minYear, 0, 1), new Date(maxYear + 1, 11, 31)]))
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

            .legend(dc.legend().x(80).y(25).itemHeight(13).gap(5).legendWidth(170).itemWidth(170)).title(function (d) {
                return dateFormat(d.key[1]) + ':\n' + d.value;
            });


        dateRangeChart
            // .width(460)
            .height(100)
            .mouseZoomable(true)
            .margins({ top: 20, right: 50, bottom: 20, left: 60 })
            .dimension(dateMonthInYearDimension)
            .group(volumeByMonthGroup)
            .centerBar(true)
            .gap(1)
            .x(d3.time.scale().domain([new Date(minYear, 0, 1), new Date(maxYear + 1, 11, 31)]))
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
                    ' | <a href=\'javascript:alsglance.dashboard.patient.reset();\'\'>Reset All</a>',
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
                {
                    label: 'Patient',
                    format: function (d) {
                        return d.PatientName;
                    }
                },
                {
                    label: 'Time Of Day',
                    format: function (d) {
                        return d.TimeTimeOfDay;
                    }
                },
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

    }
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
