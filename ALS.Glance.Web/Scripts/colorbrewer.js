'use strict';
var schemeNames = {
    sequential: ["BuGn", "BuPu", "GnBu", "OrRd", "PuBu", "PuBuGn", "PuRd", "RdPu", "YlGn", "YlGnBu", "YlOrBr", "YlOrRd"],
    singlehue: ["Blues", "Greens", "Greys", "Oranges", "Purples", "Reds"],
    diverging: ["BrBG", "PiYG", "PRGn", "PuOr", "RdBu", "RdGy", "RdYlBu", "RdYlGn", "Spectral"],
    qualitative: ["Accent", "Dark2", "Paired", "Pastel1", "Pastel2", "Set1", "Set2", "Set3"]
};

var visibleMap,
	selectedScheme = selectedScheme||"BuGn",
	numClasses = 7;

function setNumClasses(n) {
    numClasses = n;
    showSchemes();
}

var selectedSchemeType;
function setSchemeType(type) {
    selectedSchemeType = type;

    $("#num-classes option").removeAttr("disabled");
    switch (selectedSchemeType) {
        case "sequential":
            if ($("#num-classes").val() >= 10) {
                $("#num-classes").val(9);
                numClasses = 9;
            }
            $("#num-classes option[name=10], #num-classes option[name=11], #num-classes option[name=12]").attr("disabled", "disabled");
            break;
        case "diverging":
            if ($("#num-classes").val() >= 12) {
                $("#num-classes").val(11);
                numClasses = 11;
            }
            $("#num-classes option[name=12]").attr("disabled", "disabled");
            break;
    }
    showSchemes();
}

function showSchemes() {
    $("#ramps").empty();
    for (var i in schemeNames[selectedSchemeType]) {
        if (checkFilters(schemeNames[selectedSchemeType][i]) == false) continue;
        var ramp = $("<div class='ramp " + schemeNames[selectedSchemeType][i] + "'></div>"),
			svg = "<svg width='15' height='75'>";
        for (var n = 0; n < 5; n++) {
            svg += "<rect fill=" + colorbrewer[schemeNames[selectedSchemeType][i]][5][n] + " width='15' height='15' y='" + n * 15 + "'/>";
        }
        svg += "</svg>";
        $("#ramps").append(ramp.append(svg).click(function () {
            if ($(this).hasClass("selected")) return;
            setScheme($(this).attr("class").substr(5));
        }));
    }
    if (selectedSchemeType == "sequential") {
        $("#scheme1").css("width", "160px");
        $("#multi").show().text("Multi-hue");
        $("#scheme2").css("width", "90px");
        $("#single").show().text("Single hue");

        $("#singlehue").empty().css("display", "inline-block");
        for (i in schemeNames.singlehue) {
            if (checkFilters(schemeNames.singlehue[i]) == false) continue;
            var ramp = $("<div class='ramp " + schemeNames.singlehue[i] + "'></div>"),
				svg = "<svg width='15' height='75'>";
            for (var n = 0; n < 5; n++) {
                svg += "<rect fill=" + colorbrewer[schemeNames.singlehue[i]][5][n] + " width='15' height='15' y='" + n * 15 + "'/>";
            }
            svg += "</svg>";
            $("#singlehue").append(ramp.append(svg).click(function () {
                if ($(this).hasClass("selected")) return;
                setScheme($(this).attr("class").substr(5));
            }));
        }
    } else {
        $("#scheme1").css("width", "100%");
        $("#multi").hide();
        $("#singlehue").empty();
        $("#single").hide();
    }

    $(".score-icon").show();
    $("#color-system").show();
    if ($(".ramp." + selectedScheme)[0]) {
        setScheme(selectedScheme);
    } else if ($("#ramps").children().length) setScheme($("#ramps .ramp:first-child").attr("class").substr(5));
    else clearSchemes();
}

function clearSchemes() {
    $("#counties g path").css("fill", "#ccc");
    $("#color-chips").empty();
    $("#color-values").empty();
    $("#ramps").css("width", "100%");
    $("#scheme-name").html("");
    $(".score-icon").hide();
    $("#color-system").hide();
    $("#ramps").append("<p>No color schemes match these criteria.</p><p>Please choose fewer data classes, a different data type, and/or fewer filtering options.</p>");
}

function setScheme(s) {
     $(".ramp.selected").removeClass("selected");
    selectedScheme = s;
    $(".ramp." + selectedScheme).addClass("selected");
    $("#scheme-name").html(numClasses + "-class " + selectedScheme);
    applyColors();
    drawColorChips();
  
    var jsonString = "[";
    for (var i = 0; i < numClasses; i++) {
        jsonString += "'" + colorbrewer[selectedScheme][numClasses][i] + "'";
        if (i < numClasses - 1) jsonString += ",";
    }
    jsonString += "]";
    $("#copy-json input").val(jsonString);
    var cssString = "";
    for (var i = 0; i < numClasses; i++) {
        cssString += "." + selectedScheme + " .q" + i + "-" + numClasses + "{fill:" + colorbrewer[selectedScheme][numClasses][i] + "}";
        if (i < numClasses - 1) cssString += " ";
    }
    $("#copy-css input").val(cssString);

    $(".score-icon").attr("class", "score-icon");
    var f = checkColorblind(s);
    $("#blind-icon").addClass(!f ? "bad" : (f == 1 ? "ok" : "maybe")).attr("title", numClasses + "-class " + selectedScheme + " is " + getWord(f) + "color blind friendly");
    f = 1;
    $("#copy-icon").addClass(!f ? "bad" : (f == 1 ? "ok" : "maybe")).attr("title", numClasses + "-class " + selectedScheme + " is " + getWord(f) + "photocopy friendly");
    f = checkScreen(s);
    $("#screen-icon").addClass(!f ? "bad" : (f == 1 ? "ok" : "maybe")).attr("title", numClasses + "-class " + selectedScheme + " is " + getWord(f) + "LCD friendly");
    f = checkPrint(s);
    $("#print-icon").addClass(!f ? "bad" : (f == 1 ? "ok" : "maybe")).attr("title", numClasses + "-class " + selectedScheme + " is " + getWord(f) + "print friendly");

    function getWord(w) {
        if (!w) return "not ";
        if (w == 1) return "";
        if (w == 2) return "possibly not ";
    }
}

function checkFilters(scheme, f) {
    if (!colorbrewer[scheme][numClasses]) return false;
    if (checkColorblind(scheme) != 1) return false;
    //if (checkPrint(scheme) != 1) return false;
    return true;
}
function checkColorblind(scheme) {
    return colorbrewer[scheme].properties.blind.length > 1 ? colorbrewer[scheme].properties.blind[numClasses - 3] : colorbrewer[scheme].properties.blind[0];
}
function checkPrint(scheme) {
    return colorbrewer[scheme].properties.print.length > 1 ? colorbrewer[scheme].properties.print[numClasses - 3] : colorbrewer[scheme].properties.print[0];
}
function checkScreen(scheme) {
    return colorbrewer[scheme].properties.screen.length > 1 ? colorbrewer[scheme].properties.screen[numClasses - 3] : colorbrewer[scheme].properties.screen[0];
}

function drawColorChips() {
    var svg = "<svg width='24' height='270'>";
    for (var i = 0; i < numClasses; i++) {
        svg += "<rect fill=" + colorbrewer[selectedScheme][numClasses][i] + " width='24' height='" + Math.min(24, parseInt(265 / numClasses)) + "' y='" + i * Math.min(24, parseInt(265 / numClasses)) + "'/>";
    }
    $("#color-chips").empty().append(svg);
}


function getColorDisplay(c, s) {
    if (c.indexOf("#") != 0) {
        var arr = c.replace(/[a-z()\s]/g, "").split(",");
        var rgb = { r: arr[0], g: arr[1], b: arr[2] };
    }
    s = s || $("#color-system").val().toLowerCase();
    if (s == "hex") {
        if (rgb) return rgbToHex(rgb.r, rgb.g, rgb.b);
        return c;
    }
    if (s == "rgb") {
        if (!rgb) rgb = hexToRgb(c);
        return rgb.r + "," + rgb.g + "," + rgb.b;
    }
    if (s == "cmyk") {
        if (!rgb) rgb = hexToRgb(c);
        var cmyk = rgb2cmyk(rgb.r, rgb.g, rgb.b);
        return cmyk[0] + "," + cmyk[1] + "," + cmyk[2] + "," + cmyk[3];
    }
}
function getCMYK(scheme, classes, n) {
    return cmyk[scheme][classes][n].toString();
}


function rgb2cmyk(r, g, b) {
    var computedC = 0;
    var computedM = 0;
    var computedY = 0;
    var computedK = 0;

    // BLACK
    if (r == 0 && g == 0 && b == 0) {
        computedK = 1;
        return [0, 0, 0, 100];
    }

    computedC = 1 - (r / 255);
    computedM = 1 - (g / 255);
    computedY = 1 - (b / 255);

    var minCMY = Math.min(computedC,
			  Math.min(computedM, computedY));
    computedC = (computedC - minCMY) / (1 - minCMY);
    computedM = (computedM - minCMY) / (1 - minCMY);
    computedY = (computedY - minCMY) / (1 - minCMY);
    computedK = minCMY;

    return [Math.round(computedC * 100), Math.round(computedM * 100), Math.round(computedY * 100), Math.round(computedK * 100)];
}
function rgbToHex(r, g, b) {
    return "#" + ((1 << 24) | (r << 16) | (g << 8) | b).toString(16).slice(1);
}

function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

function initColors(defaultColor) {
    $("#colorPlaceHolder").replaceWith(' <li class="dropdown">'
        +'   <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">Color Schemes <span class="caret"></span></a>'
        +'    <ul class="dropdown-menu" role="menu">' 
        +'        <li>' 
        +'            <div class="container" style="max-width: 300px">' 
        +'                <div id="scheme" class="form-group">' 
        +'                    <div id="scheme1">' 
        +'                        <label id="multi"></label>' 
        +'                        <div id="ramps"></div>' 
        +'                    </div> <!--end scheme1 div-->' 
        +'                    <div id="scheme2">' 
        +'                        <label id="single"></label>' 
        +'                        <div id="singlehue"></div>' 
        +'                    </div> <!--end scheme2 div-->' 
        +'                </div> <!--end scheme div-->' 
        +'            </div>' 
        +'        </li>' 
        +'    </ul>' 
        +'</li>');
    var scheme = defaultColor||"BuGn";
    var n = 5;
    $("#num-classes").val(n);
    setSchemeType("sequential");
    setNumClasses(n);
    setScheme(scheme);
    showSchemes();
}

function applyColors() {
    if (!colorbrewer[selectedScheme][numClasses]) {
        return;
    }
    var colors = colorbrewer[selectedScheme][numClasses].slice(0); //clone the array
    colors.shift(); //skip the first color because it's to faint
    var colorsDiferential = d3.scale.ordinal().range([colorbrewer[selectedScheme][numClasses][1], colorbrewer[selectedScheme][numClasses][3]]);
    var colorRange = d3.scale.ordinal().range(colors);
    var color2 = colorbrewer[selectedScheme][numClasses][2];
    var color3 = colorbrewer[selectedScheme][numClasses][3];
    timeOfDayChart.ordinalColors(colors).redraw();
    timeHourChart.colors(color2).redraw();
    quarterChart.colors(colorRange).redraw();
    predictionSeriesChart.colors(colorsDiferential).redraw();

    dateRangeChart.colors(color2).redraw();
    if (emgChart != null) {
        emgChart.updateOptions({ colors: [color3] });
    }
}

