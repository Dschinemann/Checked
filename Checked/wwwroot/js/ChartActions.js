google.charts.load('current', { 'packages': ['corechart'] });
google.charts.setOnLoadCallback(drawChart);
const windowHight = window.screen.availHeight;
const windowWhidth = window.screen.availWidth;

function drawChart() {

    const actionsArray = [
        ["Ações", "Status"]
    ]
    action.forEach(element => {
        actionsArray.push([element.Status, element.Quantidade])
    });

    var data = google.visualization.arrayToDataTable(actionsArray);

    var options = {
        title: 'Ações por status',
        is3D: true,
        //width: '100%',
        height: `${(windowHight * 20) / 100}`,
        backgroundColor: '#2F4F4F',
        legend: {
            textStyle: {
                color: 'white',
                fontSize: 12,
                bold: true
            }
        },
        titleTextStyle:
        {
            color: 'white',
            fontSize: 12,
            bold: true
        }
    };

    var chart = new google.visualization.PieChart(document.getElementById('piechart'));

    chart.draw(data, options);
}