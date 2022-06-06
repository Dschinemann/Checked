const buttonIncrement = document.querySelector("#increment");
const buttonDecrement = document.querySelector("#decrement");
const elements = document.querySelectorAll(".number-page");
const buttonFiltro = document.querySelector("#filterOccurrence");
import getDataFromAPI from "./modules/network.js";


elements.forEach((element, index) => {
    element.addEventListener("click", element => {
        buscarOcorrencias(element);
    })
})

buttonIncrement.addEventListener("click", e => {
    e.preventDefault();
    if (elements[0].innerText > 1) {
        elements.forEach((element, index) => {
            const page = Number(elements[index].innerText) - 3;
            element.innerText = page
            element.setAttribute("data-page", page)

        })
    }
})

buttonDecrement.addEventListener("click", e => {
    e.preventDefault();
    if (numeroDePaginas < 3) return;
    elements.forEach((element, index) => {
        if (elements[2].innerText >= numeroDePaginas) return
        const page = Number(elements[index].innerHTML) + 3;
        element.innerHTML = page;
        element.setAttribute("data-page", page)
    })
})

function buscarOcorrencias(e) {
    if (numeroDePaginas <= 2) {
        return;
    }
    fetch(`Occurrences/GetOccurrencesPerPage?pagina=${e.target.getAttribute("data-page")}`)
        .then(response => response.json())
        .then(data => displayDataOccurrences(data, e))
        .catch(error => console.log(error))
}

const buscarOcorrenciasComFiltro = (query, button) => {
    fetch(`Occurrences/Filters${query}`)
        .then(response => response.json())
        .then(data => {
            displayDataOccurrences(data);
            closeForm(button)
        })
        .catch(error => console.log(error))
    const navigationPages = document.querySelector("#page-navigation");
    if (navigationPages) {
        navigationPages.remove();
    }
}


function displayDataOccurrences(data, e) {
    console.log(data)
    const bodyTable = document.getElementById("body-table-occurrence");    
    const trClone = bodyTable.children.item(0).cloneNode(true);
    bodyTable.innerHTML = "";
    if (data.length <= 0) {
        const tr = document.createElement("tr")
        tr.innerText = `Não existe ocorrencias para este filtro`
        bodyTable.appendChild(tr)
        return;
    }
    const options = {
        year: '2-digit', month: '2-digit', day: '2-digit',
        hour: '2-digit', minute: '2-digit',
        hour12: false        
    };
    data.forEach(item => {
        let newClone = trClone.cloneNode(true);
        newClone.children.item(15).remove();

        let linkEdit = document.createElement("a");
        linkEdit.href = `Occurrences/Edit?idOccurrence=${item.Id}`;
        linkEdit.innerText = "Editar | "

        let linkDelete = document.createElement("a");
        linkDelete.href = `Occurrences/Delete?idOccurrence=${item.Id}`;
        linkDelete.innerText = "Delete";

        let linkDetail = document.createElement("a");
        linkDetail.href = `Occurrences/Details?idOccurrence=${item.Id}`;
        linkDetail.innerText = "Detalhes | ";

        let tdLinks = document.createElement("td");
        tdLinks.setAttribute("data-istooltip", "false")
        tdLinks.appendChild(linkEdit);
        tdLinks.appendChild(linkDetail);
        tdLinks.appendChild(linkDelete);

        newClone.children.item(0).innerText = item.Tp_Ocorrencia.Name;
        newClone.children.item(1).innerText = new Intl.DateTimeFormat('pt-BR', options).format(new Date(item.CreatedAt));
        newClone.children.item(2).innerText = new Intl.DateTimeFormat('pt-BR', options).format(new Date(item.DateOccurrence));
        newClone.children.item(3).innerText = item.Description;
        newClone.children.item(4).innerText = item.Additional1 ?? "N/D";
        newClone.children.item(5).innerText = item.Additional2 ?? "N/D";
        newClone.children.item(6).innerText = item.Harmed
        newClone.children.item(7).innerText = item.Document
        newClone.children.item(8).innerText = new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(item.Cost);
        newClone.children.item(9).innerText = item.Appraiser.Name
        newClone.children.item(10).innerText = item.Origin
        newClone.children.item(11).innerHtml = `<a href="Plans/Index/${item.Id}"> Plano de ação</a>`;
        newClone.children.item(12).innerText = item.StatusActions;
        newClone.children.item(13).innerText = item.Status.Name;
        newClone.children.item(14).innerText = item.CorrectiveAction;

        newClone.appendChild(tdLinks)

        bodyTable.appendChild(newClone)
    })
    listeners();
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
        buttonFilter.innerHTML = "<span class=\"spinner-grow spinner-grow-sm\" role=\"status\" aria-hidden=\"true\"></span>  Carregando..."
        buscarOcorrenciasComFiltro(query, e.target);
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