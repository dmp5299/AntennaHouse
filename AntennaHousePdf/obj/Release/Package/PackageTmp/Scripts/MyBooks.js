$(window).bind("resize", function () {
    $("#jqGrid").jqGrid("setGridWidth", $("#jqGrid").closest("#grid").width());
}).triggerHandler("resize");
$('#lastStage').click(function () {
    $.ajax({
        url: "/Home/ChangeStage", data: { id: $("#books").val(), nextOrLast: "last" }, success: function (stage) {
            loadGrid($("#books").val(), $("#books option:selected").text(), stage);
        }
    });
});

$('#nextStage').click(function () {
    $.ajax({
        url: "/Home/ChangeStage", data: { id: $("#books").val(), nextOrLast: "next" }, success: function (stage) {
            loadGrid($("#books").val(), $("#books option:selected").text(), stage);
        }
    });
});
$('#tabs, #charts').tabs({
    select: function (event, ui) {
        if ($("#books").val() != 0) {
            $.ajax({
                type: 'get',
                dataType: 'json',
                url: "/Home/GetChartData",
                data: {}, success: function (data) {
                    buildChart(data);
                }
            });
        }

    }
});

function bookChange(title) {
    $.ajax({
        url: "/Home/GetStage", data: { id: title.options[title.selectedIndex].value }, success: function (stage) {
            loadGrid(title.options[title.selectedIndex].value, title.options[title.selectedIndex].text, stage);
        }
    });
    if ($("ul#navtabs li.active").index() == 1) {
        $.ajax({
            type: 'get',
            dataType: 'json',
            url: "/Home/GetChartData",
            data: {}, success: function (data) {
                buildChart(data);
            }
        });
    }

}
function buildChart(data) {
    // Build the chart
    Highcharts.chart('container', {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: " Comment Distribution"
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: 'Comments',
            colorByPoint: true,
            data: [{
                name: 'Valid',
                y: parseInt(data['valid'])
            }, {
                name: 'Invalid',
                y: parseInt(data['invalid'])
            },
{
    name: 'OOS',
    y: parseInt(data['OOS'])
}]
        }]
    });
};
function buildBookChart(data) {
    // Build the chart
    Highcharts.chart('container1', {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: " Comment Distribution"
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false
                },
                showInLegend: true
            }
        },
        series: [{
            name: 'Comments',
            colorByPoint: true,
            data: [{
                name: 'Valid',
                y: parseInt(data['valid'])
            }, {
                name: 'Invalid',
                y: parseInt(data['invalid'])
            },
{
    name: 'OOS',
    y: parseInt(data['OOS'])
}]
        }]
    });
};