
    const inputStartDate = document.querySelector("#start-date")
    const inputEndDate = document.querySelector("#end-date");
    const spanPerido = document.querySelector("#period");
    const checkDataCadastro = document.querySelector("#data-cadastro")
    const checkDataOcorrencia = document.querySelector("#data-da-ocorrencia")

    checkDataCadastro.addEventListener("change", e => {
        verificaData()
    })
    checkDataOcorrencia.addEventListener("change", e => {
        verificaData()
    })


    inputEndDate.addEventListener("change", e => {
        verificaData()
    })

    inputStartDate.addEventListener("change", e => {
        verificaData();
    })

    function verificaData() {
        if (new Date(inputEndDate.value) < new Date(inputStartDate.value) || inputStartDate.value === "" || inputEndDate.value === "") {
            if (inputStartDate.value === "") {
                spanPerido.innerHTML = `Data inicial inválida`;
                spanPerido.classList.toggle("text-bg-primary")
                return;
            }
            if (inputEndDate.value === "") {
                spanPerido.innerHTML = `Data final inválida`
                spanPerido.classList.toggle("text-bg-primary")
                return;
            }
            spanPerido.innerHTML = `Data inicial não pode ser menor que a data final`
            return;
        }
        spanPerido.innerHTML = `Início de ${new Intl.DateTimeFormat("pt-BR").format(new Date(inputStartDate.value))} até ${new Intl.DateTimeFormat("pt-BR").format(new Date(inputEndDate.value))}`
        atualizaDashboard();
    }

    function atualizaDashboard() {
        let tipoFiltro = checkDataCadastro.checked ? "CreatedAt" : "DateOccurrence";
        let selectTipo = document.querySelector("#options");
        let selectCusto = document.querySelector("#optionsCost");
        let novoArray = dataCharts.filter(ele => {
            if (new Date(ele[tipoFiltro]) >= new Date(inputStartDate.value) && new Date(ele[tipoFiltro]) <= new Date(inputEndDate.value)) {
                return true;
            } else {
                return false;
            }
        }) 
        drawColumnCharts(selectTipo.value, selectCusto.value, novoArray);
        drawTreemapChart(novoArray);
        drawDonutChart(selectTipo.value, selectCusto.value, novoArray)
    }
