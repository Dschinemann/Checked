google.charts.load('current', { 'packages': ['corechart'] });
google.charts.setOnLoadCallback(drawChartPie);
google.charts.setOnLoadCallback(drawChartArea);
google.charts.setOnLoadCallback(drawChartComboChart);

function drawChartPie() {

    var data = google.visualization.arrayToDataTable([
        ['Task', 'Hours per Day'],
        ['Work', 11],
        ['Eat', 2],
        ['Commute', 2],
        ['Watch TV', 2],
        ['Sleep', 7]
    ]);

    var options = {
        title: 'My Daily Activities',
        legend: {
            position: 'bottom'
        },
        height: 300
    };

    var chart = new google.visualization.PieChart(document.getElementById('piechart'));

    chart.draw(data, options);
}

function drawChartArea() {
    var data = google.visualization.arrayToDataTable([
        ['Semana', 'Custo'],
        ['Semana 1', 1000],
        ['Semana 2', 1170],
        ['Semana 3 ', 660],
        ['Semana 4', 1030]
    ]);

    var options = {
        title: 'Company Performance',
        hAxis: { title: 'Year', titleTextStyle: { color: '#333' } },
        vAxis: { minValue: 0 },     
        legend: {
            position: 'bottom'
        },
        height:'280'
    };
    var chart = new google.visualization.AreaChart(document.getElementById('chartArea'));
    chart.draw(data, options);
}

function drawChartComboChart() {
    // Some raw data (not necessarily accurate)
    var data = google.visualization.arrayToDataTable([
        ['Falta', 'Avaria', 'Fefo', 'Entrega errada', 'Acuracidade', 'Fora do Prazo', 'Falta de manutenção'],
        ['2004/06', 165, 938, 522, 998, 450, 614.6],
        ['2005/07', 135, 1120, 599, 1268, 288, 682],
        ['2006/08', 157, 1167, 587, 807, 397, 623],
        ['2007/09', 139, 1110, 615, 968, 215, 609.4],
        ['2008/10', 136, 691, 629, 1026, 366, 569.6],
        ['2008/11', 136, 691, 629, 1026, 366, 569.6]
    ]);

    var options = {
        title: 'Custo das Ocorrencias por mês',
        //vAxis: { title: 'Cups' },
        hAxis: { title: 'Month' },
        seriesType: 'bars',
        series: { 5: { type: 'line' } },
        legend: {
            position: 'bottom'
        }
    };

    var chart = new google.visualization.ComboChart(document.getElementById('comboChart'));
    chart.draw(data, options);
}