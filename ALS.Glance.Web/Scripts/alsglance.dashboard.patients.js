var aucBubbleChart = dc.bubbleChart('#aucBubbleChart');

alsglance.dashboard.patients.load = function (data) {
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
        //.width(800) // (optional) define chart width, :default = 200
        //.height(250) // (optional) define chart height, :default = 200
        .transitionDuration(1500) // (optional) define chart transition duration, :default = 750
        .margins({ top: 10, right: 50, bottom: 40, left: 50 })
        .dimension(timeOfDayDimension)
        //Bubble chart expect the groups are reduced to multiple values which would then be used
        //to generate x, y, and radius for each key (bubble) in the group
        .group(timeOfDayGroup)
        .colors(['rgb(49,130,189)', 'rgb(247,104,161)']) // (optional) define color function or array for bubbles
        //.colorDomain([0, 1]) //(optional) define color domain to match your data domain if you want to bind data or
        //color
        //##### Accessors
        //Accessor functions are applied to each value returned by the grouping
        //
        //* `.colorAccessor` The returned value will be mapped to an internal scale to determine a fill color
        //* `.keyAccessor` Identifies the `X` value that will be applied against the `.x()` to identify pixel location
        //* `.valueAccessor` Identifies the `Y` value that will be applied agains the `.y()` to identify pixel location
        //* `.radiusValueAccessor` Identifies the value that will be applied agains the `.r()` determine radius size,
        //*     by default this maps linearly to [0,100]
        // .xUnits(dc.units.ordinal)
        .colorAccessor(function(d) {
            var res = d.key == "M" ? 0 : 1;
            return res;
        })
        .keyAccessor(function(p) {
            return p.value.avgAge;
        })
        .valueAccessor(function(p) {
            return p.value.count;
        })
        .radiusValueAccessor(function(p) {
            return p.value.avgAge;
        })
        .maxBubbleRelativeSize(0.3)
        .x(d3.scale.linear().domain([41, 42]))
        .y(d3.scale.linear().domain([-100, 100]))
        .r(d3.scale.linear().domain([0, 4000]))
        //##### Elastic Scaling
        //`.elasticX` and `.elasticX` determine whether the chart should rescale each axis to fit data.
        //The `.yAxisPadding` and `.xAxisPadding` add padding to data above and below their max values in the same unit
        //domains as the Accessors.
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
        .label(function(p) {
            return p.key;
        })
        .renderTitle(true) // (optional) whether chart should render titles, :default = false
        .title(function(p) {
            return [
                p.key,
                '# Patients: ' + p.value.count,
                'Average Age: ' + numberFormat(p.value.avgAge)
            ].join('\n');
        }).yAxis().tickFormat(function(v) {
            return v;
        });
    //#endregion

    //#### Rendering
    //simply call renderAll() to render all charts on the page
    dc.renderAll();
    $(".loading").remove();

};

alsglance.dashboard.patients.init = function () {
    alsglance.dashboard.init();
};