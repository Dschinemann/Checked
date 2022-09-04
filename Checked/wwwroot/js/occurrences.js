const buttonFiltro = document.querySelector("#filterOccurrence");
import getDataFromAPI from "./modules/network.js";



const buscarOcorrenciasComFiltro = (query, button) => {
    fetch(`Occurrences/Filters${query}`)
        .then(response => response.json())
        .then(data => {            
            closeForm(button)
        })
        .catch(error => console.log(error))
    const navigationPages = document.querySelector("#page-navigation");
    if (navigationPages) {
        navigationPages.remove();
    }
}


buttonFiltro.addEventListener("click", (e) => {

    carregarSelects();
    const homeSection = document.querySelector(".home-section");
    const divPesquisa = document.createElement("div")
    divPesquisa.classList.add("form-pesquisa");
    divPesquisa.innerHTML = formFilter();
    homeSection.appendChild(divPesquisa);

    const buttonFilter = document.querySelector("#submitFilter")
    buttonFilter.addEventListener("click", (e) => {
        e.preventDefault();
        let form = new FormData(document.querySelector(".form-itens"));
        let query = "?";
        for (let key of form.keys()) {
            if (form.get(key)) {
                if (key === "Cost") {
                    query = query + key + "=" + form.get(key).replace(",", ".") + "&";
                } else
                    if (key === "TipoFiltroData") {
                        if (form.get("TipoFiltroData") != 0) {
                            if (form.get("EndDate")) {
                                query = query + key + "=" + form.get(key) + "&StartDate=" + form.get("StartDate") + "&EndDate=" + form.get("EndDate")
                            } else {
                                alert("Informe uma data final valida");
                                return;
                            }
                        } else if (form.get("EndDate") || form.get("StartDate")) {
                            alert("Informe o tipo de filtro de data");
                            return;
                        }
                    } else if (key === "EndDate" || key === "StartDate") {
                        continue;
                    } else {
                        query = query + key + "=" + form.get(key) + "&"
                    }
            }
        }
        buttonFilter.innerHTML = "<span class=\"spinner-grow spinner-grow-sm\" role=\"status\" aria-hidden=\"true\"></span>  Carregando...";
        var link = document.createElement("a")
        link.href = `Occurrences/Filters${query}`
        link.click();
        //buscarOcorrenciasComFiltro(query, e.target);
    })

    const buttonCLoseFormPesquisa = document.querySelector("#closeForm");
    buttonCLoseFormPesquisa.addEventListener("click", () => {
        divPesquisa.remove()
    })

})



function closeForm(button) {
    let formPesquisa = document.querySelector(".form-pesquisa")
    formPesquisa.remove()

}

const formFilter = () => {
    const form = `
              <form class="form-itens">
        <div style="width: 35%">
          <div class="mb-3 size-box">
            <label for="formGroupExampleInput" class="form-label">Tipo</label>
            <select
              type="text"
              class="form-control"
              id="TP_OcorrenciaId"
              name="TP_OcorrenciaId"
            >
              <option selected></option>
            </select>
          </div>
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label"
              >Descrição</label
            >
            <input
              type="text"
              class="form-control"
              id="Description"
              name="Description"
              placeholder="Descrição"
            />
          </div>
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label"
              >Informação adicional</label
            >
            <input
              type="text"
              class="form-control"
              id="Additional1"
              name="Additional1"
              placeholder="Informação adicional1"
            />
          </div>
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label"
              >Informação adicional</label
            >
            <input
              type="text"
              class="form-control"
              name="Additional2"
              id="Additional2"
              placeholder="Informação adicional2"
            />
          </div>
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label"
              >Prejudicado</label
            >
            <input
              type="text"
              class="form-control"
              name="Harmed"
              id="Harmed"
              placeholder="Prejudicado"
            />
          </div>
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label"
              >Documento</label
            >
            <input
              type="text"
              class="form-control"
              name="Document"
              id="Document"
              placeholder="Documento"
            />
          </div>
        </div>

        <div style="width: 35%">
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Custo</label>
            <input
              type="text"
              class="form-control"
              id="Cost"
              name="Cost"
              placeholder="Custo"
            />
          </div>
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label"
              >Avaliador</label
            >
            <select
              type="text"
              class="form-control"
              id="AppraiserId"
              name="AppraiserId"
            >
              <option selected></option>
            </select>
          </div>
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Origem</label>
            <input
              type="text"
              class="form-control"
              id="Origin"
              name="Origin"
              placeholder="Origem"
            />
          </div>
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label"
              >Status das ações</label
            >
            <input
              type="text"
              class="form-control"
              id="StatusActions"
              name="StatusActions"
              placeholder="Status das ações"
            />
          </div>
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Status</label>
            <select
              type="text"
              class="form-control"
              id="StatusId"
              name="StatusId"
            >
              <option selected></option>
            </select>
          </div>
          <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label"
              >Ação corretiva</label
            >
            <input
              type="text"
              class="form-control"
              id="CorrectiveAction"
              name="CorrectiveAction"
              placeholder="Ação corretiva"
            />
          </div>
        </div>
        <div class="datas">
          <div>
            <i id="closeForm" class="bx bxs-x-circle"></i>
          </div>
          <div class="cntr-datas">
            <div>
              <div class="mb-3">
                <label for="tipoFiltroData" class="form-label"
                  >Filtro de data</label
                >
                <select class="form-control" id="TipoFiltroData" name="TipoFiltroData">
                  <option value='0' selected>Selecione um tipo de filtro de data</option>
                  <option value='1'>Por cadastro da ocorrencia</option>
                  <option value='2'>Por data da ocorrência</option>
                </select>
              </div>
            </div>
            <div>
              <div class="mb-3">
                <label for="dataInicial" class="form-label">Data inicial</label>
                <input type="datetime-local" name="StartDate" class="form-control" id="StartDate" />
              </div>
              <div class="mb-3">
                <label for="dataFinal" class="form-label">Data Final</label>
                <input type="datetime-local" name="EndDate" class="form-control" id="EndDate" />
              </div>
            </div>
          </div>
          <div class="button-search">
            <button id="submitFilter" type="submit" class="btn btn-primary">             
                Procurar
            </button>
          </div>
        </div>
      </form>
    `
    return form;
}

async function carregarSelects() {
    const users = await getDataFromAPI("/Account/GetUsersPerOrganization");
    displaySelectUsers(users);
    const types = await getDataFromAPI("/Occurrences/GetTypesOccurrencesPerOrganization");
    displaySelectTypes(types)
    const status = await getDataFromAPI("/Occurrences/GetStatusOcurrence");
    displaySelectStatus(status)
}

function displaySelectUsers(users) {
    const select = document.querySelector("#AppraiserId");
    for (let ele of users) {
        let option = `<option value="${ele["id"]}">${ele["name"]}</option>`
        select.insertAdjacentHTML('beforeend', option);
    }
};

function displaySelectTypes(types) {
    const select = document.querySelector("#TP_OcorrenciaId");
    for (let ele of types) {
        let option = `<option value="${ele["id"]}">${ele["name"]}</option>`
        select.insertAdjacentHTML('beforeend', option);
    }
};

function displaySelectStatus(status) {
    const select = document.querySelector("#StatusId");
    for (let ele of status) {
        let option = `<option value="${ele["id"]}">${ele["name"]}</option>`
        select.insertAdjacentHTML('beforeend', option);
    }
};

//tooltip
function listeners() {
    const td = document.querySelectorAll("td,th");

    td.forEach(element => {
        element.addEventListener("mouseenter", tooltipInfo)
    });

    td.forEach(element => {
        element.addEventListener("mouseout", removeToolTip)
    });
}
listeners();


function tooltipInfo(event) {
    if (event.target.dataset.istooltip === 'false') {
        return;
    }
    let windowWidth = window.innerWidth;
    let LeftOrRight = event.clientX > (windowWidth - 200) ? "right" : "left";
    let position = event.clientX > (windowWidth - 200) ? 16 : event.clientX;

    let text = event.target.innerText;
    const div = document.createElement("div");
    div.setAttribute("id", "infoTip")
    div.innerHTML = `
        <span class="toolTipSpan text-wrap text-break" style="top:${event.clientY}px;${LeftOrRight}:${position}px">${text}</span>
    `
    event.target.appendChild(div);

}

function removeToolTip(event) {
    if (document.querySelector("#infoTip")) {
        document.querySelector("#infoTip").remove();
    }
}

/**
 *
 * ocultar e expandir colunas
 * 
 */

const buttonExpandirOuReduzir = document.querySelectorAll(".arrow")
buttonExpandirOuReduzir.forEach((element, index) => {
    element.addEventListener("click", (elementI) => {
        let armLocal = localStorage.getItem('tabelaOcorrencia');

        let arrayDeColunasDaTabela = [];

        if (armLocal) {
            arrayDeColunasDaTabela = armLocal.split(',')
        }

        let containerColuna = document.querySelector("#colunas-ocultas");

        let newDiv = document.createElement("div");
        newDiv.classList.add("btn", "btn-outline-primary", "coluna-oculta");



        let icon = document.createElement("i");
        icon.classList.add("bx", "bx-x", "bx-sm");
        icon.dataset.position = elementI.target.dataset.position;
        icon.addEventListener("click", elementDiv => {
            mostrarColuna(elementDiv, elementDiv.target.dataset.position)
        })

        let span = document.createElement("span");
        span.innerText = elementI.target.parentElement.innerText;

        newDiv.appendChild(icon);
        newDiv.appendChild(span);

        let coluna = document.querySelector(`.col${elementI.target.dataset.position}`)
        coluna.style.visibility = 'collapse';

        arrayDeColunasDaTabela[elementI.target.dataset.position] = 0;
        containerColuna.appendChild(newDiv);

        salvarColunasOcultasLocalStorage(arrayDeColunasDaTabela);

    })
})

function mostrarColuna(e, index) {

    let armLocal = localStorage.getItem('tabelaOcorrencia');
    let arrayDeColunasDaTabela = [];

    if (armLocal) {
        arrayDeColunasDaTabela = armLocal.split(',');
    }
    arrayDeColunasDaTabela[index] = 1;

    let coluna = document.querySelector(`.col${index}`)
    coluna.style.visibility = 'visible';
    e.target.parentElement.remove();

    localStorage.setItem('tabelaOcorrencia', arrayDeColunasDaTabela.toString())
}

function salvarColunasOcultasLocalStorage(arrayDecolunas) {
    let arrayParaSalvar = [];

    for (let x = 0; x < arrayDecolunas.length; x++) {
        if (arrayDecolunas[x]) {
            arrayParaSalvar[x] = arrayDecolunas[x];
        } else {
            arrayParaSalvar[x] = 0;
        }
    }
    localStorage.setItem('tabelaOcorrencia', arrayParaSalvar.toString())
}

window.addEventListener("load", () => {
    let arrayDeColunasDaTabela = [];
    let cabecalhoTabela = document.querySelectorAll('th');

    let armLocal = localStorage.getItem('tabelaOcorrencia');
    if (armLocal) {
        arrayDeColunasDaTabela = armLocal.split(',')
    }

    let cols = document.querySelectorAll("col");
    cols.forEach((ele, index) => {
        if (arrayDeColunasDaTabela[index] === '0') {
            ele.style.visibility = 'collapse'
            adicionarEtiquetaDaColunaOculta(cabecalhoTabela.item(index).innerText, index)
        }
    })


})

function adicionarEtiquetaDaColunaOculta(texto, index) {
    let containerColuna = document.querySelector("#colunas-ocultas");

    let newDiv = document.createElement("div");
    newDiv.classList.add("btn", "btn-outline-primary", "coluna-oculta");


    let icon = document.createElement("i");
    icon.classList.add("bx", "bx-x", "bx-sm");
    icon.dataset.position = index
    icon.addEventListener("click", e => {
        mostrarColuna(e, index)
    })

    let span = document.createElement("span");
    span.innerText = texto;

    newDiv.appendChild(icon);
    newDiv.appendChild(span);

    containerColuna.appendChild(newDiv);
}

/**
 * adicionar coluna 
 **/

const addComplement = document.querySelector(".addComplement");

addComplement.addEventListener("click", () => {
    let modal = document.querySelector(".modal-complement");
    modal.classList.toggle("display-off")
})

const closemodal = document.querySelector(".close-modal");
closemodal.addEventListener("click", (e) => {
    let modal = document.querySelector(".modal-complement");
    modal.classList.toggle("display-off")
})


/**
 * Adicionar informação condicional
 * */

const addComplementValue = document.querySelectorAll(".addComplement-value");
addComplementValue.forEach((buttonPlus) => {
    buttonPlus.addEventListener("click", (element) => {
        let modal = document.querySelector("#cell-complement")
        modal.classList.toggle("display-off")
        document.querySelector("#name-column").innerHTML = element.target.dataset.coluna;
        document.querySelector("#occurrenceId").value = element.target.dataset.occurrenceid;
        document.querySelector("#columnId").value = element.target.dataset.columnid;
    })
})

const closeCellModal = document.querySelector(".close-modal-cell");
closeCellModal.addEventListener("click", () => {
    let modal = document.querySelector("#cell-complement")
    modal.classList.toggle("display-off")
})

