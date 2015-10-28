/**
Timeslider for jquery
param include some class:
number: number of point
pointClass: class of points
containerClass: class of container
sliderClass: class of slider
completed: number of done tasks
renderLabel: function render label below 
*/
$.fn.timeslide = function (params) {
    var points = [];
    var pointLabels = [];
    var percent = (100 / (params.number - 1));
    for (var i = 0; i < params.number; i++) {
        var point = $("<div>", {
            "class": "point",
            "data-toggle": "popover",
            "data-content": "Content",
            "data-container": "document",
            "data-placement": "top",
            "title": "Title"
        }).css("left", (percent * (i) - 2) + "%")
            .addClass(params.pointClass);
        if (params.renderLabel != undefined) {
            var label = $("<div>", {
                "class": "point-label",
                html: params.renderLabel(i)
            })
                .css("left", (percent * (i - 1)) + "%")
                .css("width", (percent * 2) + "%")
                .css("display", "inline-block")
                .css("font-size", "13px");
            pointLabels.push(label);
        }
        points.push(point);
    }
    var donePercent = 100 * params.completed / (1 + params.number) + 2;

    var slider = $("<div>", {
        "class": "timeslide-slider " + params.sliderClass
    }).css("width", donePercent + "%");
    var container = $("<div>", {
        "class": "timeslide-container"
    }).addClass(params.containerClass)
        .append(slider)
        .append(points);
    this.append(container).css("position", "relative");
    if (params.renderLabel != undefined) {
        this.append(pointLabels);
    }
    return this;
}

/*Example*/
$(document).ready(function () {
    var data = [
        "abc",
        "def"
    ]
    $("#abc").timeslide({
        number: data.length,
        completed: data.length - 1,
        renderLabel: function (i) {
            return "<b>" + data[i] + "</b>";
        }
    });
})