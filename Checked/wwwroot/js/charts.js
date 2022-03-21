google.charts.load('current', { 'packages': ['corechart'] });
google.charts.load('current', { 'packages': ['corechart', 'bar'] });
google.charts.setOnLoadCallback(drawChartPie);
google.charts.setOnLoadCallback(drawChartArea);
google.charts.setOnLoadCallback(drawChartComboChart);

const windowWidth = window.screen.availWidth;
const windowHeight = window.screen.availHeight;

function drawChartPie() {

    let arrayChart = [['Status', 'Ocorrencias por Status']];
    chartOccurrencePerStatus.forEach(c => {
        arrayChart.push([c.status, c.Quantidade])
    });

    var data = google.visualization.arrayToDataTable(arrayChart);

    var options = {
        title: 'Ocorrências por Status',
        legend: {
            position: 'bottom',
            textStyle: {
                fontSize: 10,
                bold: true
            }
        },
        backgroundColor: "#e5e5e5",
        is3D: true,
        width: (windowWidth * 18) / 100,
        chartArea: {
            left: 20, top: 0, width: '90%', height: '75%'
        }
    };

    var chart = new google.visualization.PieChart(document.getElementById('piechart'));
    chart.draw(data, options);
}

function drawChartArea() {
    const costPerWeek = [
        ["Semana", "Custo"]
    ];
    chartCostPerWeek.forEach(c => {
        costPerWeek.push([c.Week, c.Cost]);
    })

    var data = google.visualization.arrayToDataTable(costPerWeek);

    var options = {
        title: 'Total de gastos com ocorrencias',
        //hAxis: { title: 'Year', titleTextStyle: { color: '#333' } },
        vAxis: { minValue: 0 },        
        chartArea: {
            //width: '0%'
        },
        height: `${(windowHeight * 40) / 100}`
    };
    var chart = new google.visualization.AreaChart(document.getElementById('chartArea'));
    chart.draw(data, options);
}

function drawChartComboChart() {
    var data = google.visualization.arrayToDataTable(arrayForComboChart())
    var materialOptions = {
        width: (windowWidth * 50) / 100,        
        chart: {
            title: 'Ocorrências',
            subtitle: 'Custo das Ocorrências dos últimos seis meses'
        },
        series: {
            0: { axis: 'distance' }, // Bind series 0 to an axis named 'distance'.
            1: { axis: 'brightness' } // Bind series 1 to an axis named 'brightness'.
        },
        axes: {
            y: {
                distance: { label: 'Prejuízo em R$' }, // Left y-axis.                
            }
        },
        bar: { groupWidth: "95%" },
        legend: {
            position: 'bottom',
            textStyle: { color: 'blue', fontSize: 10 },
        }
    }
    var chart = new google.charts.Bar(document.getElementById('columnChart'));
    chart.draw(data, google.charts.Bar.convertOptions(materialOptions))
};


const arrayWithOccurrencesPerName = [];
function arrayForComboChart() {
    const occurrenceName = ['Mês'];

    chartOccurrencePerNames.forEach(e => {
        if (occurrenceName.length == 0) {
            occurrenceName.push(e.Name)
        } else {
            if (occurrenceName.findIndex(ele => ele == e.Name) == -1) {
                occurrenceName.push(e.Name)
            }
        }
    });
    arrayWithOccurrencesPerName.push(occurrenceName);

    chartOccurrencePerNames.forEach((e) => {
        const occurrenceDetail = [];
        const typeIndex = arrayWithOccurrencesPerName[0].findIndex(ele => ele == e.Name)
        const indexMonth = searchIndeMonth(month(e.Month));


        if (indexMonth != -1) {
            arrayWithOccurrencesPerName[indexMonth][typeIndex] = e.Cost;
        } else {
            occurrenceDetail[0] = month(e.Month);
            for (i = 1; i < arrayWithOccurrencesPerName[0].length; i++) {
                if (i != typeIndex) {
                    occurrenceDetail[i] = 0;
                } else {
                    occurrenceDetail[i] = e.Cost
                }
            }
            arrayWithOccurrencesPerName.push(occurrenceDetail);
        }
    })
    return arrayWithOccurrencesPerName;
}
function searchIndeMonth(month) {
    let index = -1;
    arrayWithOccurrencesPerName.forEach((e, ind) => {
        if (e.findIndex(c => c == month) != -1) {
            index = ind;
        }
    })
    return index;
}

function month(intMonth) {
    switch (intMonth) {
        case 1:
            return 'Jan'
        case 2:
            return 'Fev'
        case 3:
            return 'Mar'
        case 4:
            return 'Abr'
        case 5:
            return 'Mai'
        case 6:
            return 'Jun'
        case 7:
            return 'Jul'
        case 8:
            return 'Ago'
        case 9:
            return 'Set'
        case 10:
            return 'Out'
        case 11:
            return 'Nov'
        case 12:
            return 'Dex'

        default:
            break;
    }
};