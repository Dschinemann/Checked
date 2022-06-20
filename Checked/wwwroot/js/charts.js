let screenWidth = window.screen.width;
let screenHeight = window.screen.height;

let selectTipo = document.querySelector("#options");
let selectCusto = document.querySelector("#optionsCost");

selectTipo.addEventListener("change", (e) => {
    let inputStartDate = document.querySelector("#start-date")
    let inputEndDate = document.querySelector("#end-date");

    if (inputStartDate.value === "" || inputEndDate.value === "") {
        drawColumnCharts(e.target.value, selectCusto.value);
        drawDonutChart(e.target.value, selectCusto.value);
    } else {
        verificaData()
    }

})

selectCusto.addEventListener("change", (e) => {
    let inputStartDate = document.querySelector("#start-date")
    let inputEndDate = document.querySelector("#end-date");

    if (inputStartDate.value === "" || inputEndDate.value === "") {
        drawColumnCharts(selectTipo.value, e.target.value);
        drawDonutChart(selectTipo.value, e.target.value);
    } else {
        verificaData()
    }

})

let selectDate = document.querySelector("#options-date");
let selectCustoPorMes = document.querySelector("#optionsSumMonth");

selectDate.addEventListener("change", (e) => {
    drawColumnChartMonth(e.target.value, selectCustoPorMes.value)
})

selectCustoPorMes.addEventListener("change", (e) => {
    drawColumnChartMonth(selectDate.value, e.target.value)
})

/***
 * 
 * Column chart Custo mensal
 * /
 * */



drawColumnChartMonth()
function drawColumnChartMonth(tipoData = "CreatedAt", tipoCalculo = "Somar", listaDeOcorrencias = dataCharts) {
    let custoPorMes = []
    listaDeOcorrencias.forEach(ocorrencia => {
        if (tipoCalculo === "Somar") {
            if (custoPorMes[new Date(ocorrencia[tipoData]).getMonth()]) {
                custoPorMes[new Date(ocorrencia[tipoData]).getMonth()] = custoPorMes[new Date(ocorrencia[tipoData]).getMonth()] + ocorrencia.Cost;
            } else {
                custoPorMes[new Date(ocorrencia[tipoData]).getMonth()] = ocorrencia.Cost;
            }
        } else {
            if (custoPorMes[new Date(ocorrencia[tipoData]).getMonth()]) {
                custoPorMes[new Date(ocorrencia[tipoData]).getMonth()] = custoPorMes[new Date(ocorrencia[tipoData]).getMonth()] + 1;
            } else {
                custoPorMes[new Date(ocorrencia[tipoData]).getMonth()] = 1;
            }
        }
        //
    })
    for (let index = 0; index < 12; index++) {
        const element = custoPorMes[index];
        if (!element) {
            custoPorMes[index] = 0;
        }
    }
    atualizarColumnChartMonth(custoPorMes)
}

function atualizarColumnChartMonth(custoPorMes) {
    let ehContagem = selectCustoPorMes.value === "Somar" ? "R$" : "";
    let tituloDoGrafico = selectCustoPorMes.value === "Somar" ? "Soma" : "Contagem";
    let columnData = {
        "graphset": [{
            "type": "bar3d",
            "title": {
                "text": `${tituloDoGrafico} Mensal de ocorrências`
            },
            tooltip: {
                text: `${ehContagem} %node-value`
            },
            "3d-aspect": {
                "true3d": false,
                depth: "15px"
            },
            plot: {
                decimals: 2,
                facets: {
                    front: {
                        'background-color': "#3EA4F9 #0055BF"
                    },
                    right: {
                        'background-color': "#3EA4F9 #0055BF"
                    },
                    left: {
                        'background-color': "#3EA4F9 #0055BF"
                    },
                    top: {
                        'background-color': "white"
                    },
                    bottom: {
                        'background-color': "white"
                    }
                }
            },
            'scale-x': {
                labels: ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"] /* Scale Labels */
            },
            "series": [{
                "values": custoPorMes
            }
            ]
        }]
    };

    zingchart.render({
        id: 'myChart3',
        data: columnData,
        height: "90%",
        width: "100%"
    });
}

/***
 * 
 * Donut chart
 * 
 */
drawDonutChart();
function drawDonutChart(tipoFiltro = "Tp_Ocorrencia", somarOuContar = "Somar", listaDeOcorrencias = dataCharts) {
    let arrayDeTiposDeOcorrencia = [];
    listaDeOcorrencias.forEach(ocorrencia => {
        if (tipoFiltro === "Tp_Ocorrencia" || tipoFiltro === "Status") {
            if (arrayDeTiposDeOcorrencia.length <= 0) {
                arrayDeTiposDeOcorrencia.push({
                    values: [ocorrencia.Cost],
                    text: ocorrencia[tipoFiltro].Name
                })
            } else {
                let index = arrayDeTiposDeOcorrencia.findIndex(ele => {
                    return ele.text === ocorrencia[tipoFiltro].Name;
                })
                if (index !== -1) {
                    arrayDeTiposDeOcorrencia[index].values = [parseFloat(arrayDeTiposDeOcorrencia[index].values) + ocorrencia.Cost]
                } else {
                    arrayDeTiposDeOcorrencia.push({
                        values: [ocorrencia.Cost],
                        text: ocorrencia[tipoFiltro].Name
                    })
                }
            }
        }
        if (tipoFiltro === "Harmed" || tipoFiltro === "Origin") {
            if (arrayDeTiposDeOcorrencia.length <= 0) {
                if (somarOuContar === "Somar") {
                    arrayDeTiposDeOcorrencia.push({
                        values: [ocorrencia.Cost],
                        text: ocorrencia[tipoFiltro]
                    })
                } else {
                    arrayDeTiposDeOcorrencia.push({
                        values: [1],
                        text: ocorrencia[tipoFiltro]
                    })
                }

            } else {
                let index = arrayDeTiposDeOcorrencia.findIndex(ele => {
                    return ele.text === ocorrencia[tipoFiltro];
                })
                if (index !== -1) {
                    if (somarOuContar === "Somar") {
                        arrayDeTiposDeOcorrencia[index].values = [parseFloat(arrayDeTiposDeOcorrencia[index].values) + ocorrencia.Cost]
                    } else {
                        arrayDeTiposDeOcorrencia[index].values = [parseFloat(arrayDeTiposDeOcorrencia[index].values) + 1]
                    }
                } else {
                    if (somarOuContar === "Somar") {
                        arrayDeTiposDeOcorrencia.push({
                            values: [ocorrencia.Cost],
                            text: ocorrencia[tipoFiltro]
                        })
                    } else {
                        arrayDeTiposDeOcorrencia.push({
                            values: [1],
                            text: ocorrencia[tipoFiltro]
                        })
                    }
                }
            }
        }
    })
    atualizarDonutChart(arrayDeTiposDeOcorrencia);
}
function atualizarDonutChart(arrayDeTiposDeOcorrencia) {
    let dataDonut = {
        type: "ring3d",
        plot: {
            "value-box": {
                "font-size": 10,
                "font-weight": "normal",
                "placement": "out"
            },
            tooltip: {
                fontSize: '10',
                fontFamily: "Open Sans",
                padding: "5 10",
                text: "%plot-text%t %node-percent-value%"
            },
            animation: {
                effect: 2,
                method: 5,
                speed: 900,
                sequence: 1,
                delay: 3000
            }
        },
        scale: {
            "size-factor": 0.5
        },
        plotarea: {
            margin: "20 20 20 20"
        },
        series: arrayDeTiposDeOcorrencia
    };

    zingchart.render({
        id: 'myChart2',
        data: dataDonut,
        height: "90%",
        width: "100%"
    });
}

drawTreemapChart()
function drawTreemapChart(listaDeOcorrencias = dataCharts) {
    let tiposDeOcorrencia = [];

    listaDeOcorrencias.forEach(ocorrencia => {
        if (tiposDeOcorrencia.length === 0) {
            tiposDeOcorrencia.push({
                text: ocorrencia.Tp_Ocorrencia.Name,
                children: [{
                    text: ocorrencia.Harmed,
                    value: 1
                }]
            })
        } else {
            let index = tiposDeOcorrencia.findIndex(ele => ele.text === ocorrencia.Tp_Ocorrencia.Name)
            if (index === -1) {
                tiposDeOcorrencia.push({
                    text: ocorrencia.Tp_Ocorrencia.Name,
                    children: [{
                        text: ocorrencia.Harmed,
                        value: 1
                    }]
                })
            } else {
                let childIndex = tiposDeOcorrencia[index].children.findIndex(ele => ele.text === ocorrencia.Harmed)
                if (childIndex === -1) {
                    tiposDeOcorrencia[index].children.push({
                        text: ocorrencia.Harmed,
                        value: 1
                    });
                } else {
                    tiposDeOcorrencia[index].children[childIndex].value = tiposDeOcorrencia[index].children[childIndex].value + 1;
                }
            }
        }
    })

    let treemapChart = {
        graphset: [{
            type: "treemap",
            plotarea: {
                margin: "0 0 30 0"
            },
            options: {
                "aspect-type": "transition",
                "color-start": "#c00",
                "color-end": "#300"
            },
            series: tiposDeOcorrencia
        }]
    }

    zingchart.render({
        id: 'myChart5',
        data: treemapChart,
        height: ((screenHeight * 100) / 100),
        width: "100%"
    });
}

/********************
 * 
 * 
 * 
 * 
 * 
 */


drawColumnCharts("Tp_Ocorrencia", "Somar")

function drawColumnCharts(tipoFiltro = "Tp_Ocorrencia", somarOuContar = "Somar", listaDeOcorrencias = dataCharts) {
    let arrayDeTiposDeOcorrencia = [];
    let series = [];
    listaDeOcorrencias.forEach(ocorrencia => {
        if (tipoFiltro === "Status" || tipoFiltro === "Tp_Ocorrencia") {
            if (arrayDeTiposDeOcorrencia.length === 0) {
                arrayDeTiposDeOcorrencia.push(ocorrencia[tipoFiltro].Name)
                if (somarOuContar === "Somar") {
                    series.push({ "values": [ocorrencia.Cost] })
                } else {
                    series.push({ "values": [1] })
                }

            } else {
                let existeEsteTipo = arrayDeTiposDeOcorrencia.findIndex(tipo => tipo === ocorrencia[tipoFiltro].Name)
                if (existeEsteTipo !== -1) {
                    if (somarOuContar === "Somar") {
                        series[0].values[existeEsteTipo] = (series[0].values[existeEsteTipo] + ocorrencia.Cost)
                    } else {
                        series[0].values[existeEsteTipo] = (series[0].values[existeEsteTipo] + 1)
                    }
                } else {
                    arrayDeTiposDeOcorrencia.push(ocorrencia[tipoFiltro].Name)
                    if (somarOuContar === "Somar") {
                        series[0].values.push(ocorrencia.Cost)
                    } else {
                        series[0].values.push(1)
                    }
                }
            }
            return;
        };
        if (tipoFiltro === "Harmed" || tipoFiltro === "Origin") {
            if (arrayDeTiposDeOcorrencia.length === 0) {
                arrayDeTiposDeOcorrencia.push(ocorrencia[tipoFiltro])
                if (somarOuContar === "Somar") {
                    series.push({ "values": [ocorrencia.Cost] })
                } else {
                    series.push({ "values": [1] })
                }
            } else {
                let existeEsteTipo = arrayDeTiposDeOcorrencia.findIndex(tipo => tipo === ocorrencia[tipoFiltro])
                if (existeEsteTipo !== -1) {
                    if (somarOuContar === "Somar") {
                        series[0].values[existeEsteTipo] = (series[0].values[existeEsteTipo] + ocorrencia.Cost)
                    } else {
                        series[0].values[existeEsteTipo] = (series[0].values[existeEsteTipo] + 1)
                    }
                } else {
                    arrayDeTiposDeOcorrencia.push(ocorrencia[tipoFiltro])
                    if (somarOuContar === "Somar") {
                        series[0].values.push(ocorrencia.Cost)
                    } else {
                        series[0].values.push(1)
                    }
                }
            }
        };
    })
    atualizarChart(arrayDeTiposDeOcorrencia, series)
}


function atualizarChart(arrayDeTiposDeOcorrencia, series) {
    let ehContagem = selectCusto.value === "Somar" ? "R$" : "";
    let tituloDoGrafico = selectCusto.value === "Somar" ? "Soma de Custo": "Contagem"
    let data = {
        "graphset": [{
            "type": "bar3d",
            "title": {
                "text": `${tituloDoGrafico} de Ocorrências`
            },
            "3d-aspect": {
                "true3d": false,
                depth: "15px"
            },
            tooltip: {
                text: `${ehContagem} %node-value`
            },
            plot: {
                decimals: 2,
                facets: {
                    front: {
                        'background-color': "#3EA4F9 #0055BF"
                    },
                    right: {
                        'background-color': "#3EA4F9 #0055BF"
                    },
                    left: {
                        'background-color': "#3EA4F9 #0055BF"
                    },
                    top: {
                        'background-color': "white"
                    },
                    bottom: {
                        'background-color': "white"
                    }
                }
            },
            'scale-x': {
                labels: arrayDeTiposDeOcorrencia
            },
            "series": series
        }]
    };
    zingchart.render({
        id: 'myChart',
        data: data,
        height: "90%",
        width: "100%"
    });
}