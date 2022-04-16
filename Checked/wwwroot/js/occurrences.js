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

const buscarOcorrenciasComFiltro = (query) => {
    fetch(`Occurrences/Filters${query}`)
        .then(response => response.json())
        .then(data => displayDataOccurrences(data))
        .catch(error => console.log(error))
}


function displayDataOccurrences(data, e) {
    const bodyTable = document.getElementById("body-table-occurrence");
    bodyTable.innerHTML = "";
    if (data.length <= 0) {
        const tr = document.createElement("tr")
        tr.innerText = `Não existe ocorrencias para este filtro`
        bodyTable.appendChild(tr)
        return;
    }
    for (let item in data) {

        const tr = document.createElement("tr")

        for (let element in data[item]) {
            tr.innerHTML = `
                <td>${data[item]["Tp_Ocorrencia"]["Name"]}</td>
                <td>${data[item]["Description"]}</td>
                <td>${data[item]["Additional1"] ?? "N/D"}</td>
                <td>${data[item]["Additional2"] ?? "N/D"}</td>
                <td>${data[item]["Harmed"]}</td>
                <td>${data[item]["Document"]}</td>
                <td>${new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(data[item]["Cost"])}</td>
                <td>${data[item]["Appraiser"]["Name"]}</td>
                <td>${data[item]["Origin"]}</td>
                <td><a href="Plans/Index/${data[item]["Id"]}">Plano de ação</a></td>
                <td>${data[item]["StatusActions"]}</td>
                <td>${data[item]["Status"]["Name"]}</td>
                <td>${data[item]["CorrectiveAction"] ?? "N/D"}</td>
                <td>
                    <a href="Occurrences/Edit?idOccurrence=${data[item]["Id"]}">Editar |</a>
                    <a href="Occurrences/Edit?idOccurrence=${data[item]["Id"]}">Detalhes |</a>
                    <a href="Occurrences/Edit?idOccurrence=${data[item]["Id"]}">Delete</a>
                </td>
            `;

        }

        bodyTable.appendChild(tr);
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
        for (let value of form.keys()) {
            if (form.get(value)) {
                if (value === "Cost") {
                    query = query + value + "=" + form.get(value).replace(",", ".") + "&";
                } else {
                    query = query + value + "=" + form.get(value) + "&"
                }
            }
        }
        buscarOcorrenciasComFiltro(query);
        closeForm(e.target)
    })

})



function closeForm(button) {
    let formPesquisa = document.querySelector(".form-pesquisa")
    formPesquisa.remove()

}

const formFilter = () => {
    const form = `
        <form class="form-itens">
        <div style="width: 46%">
        <div class="mb-3 size-box">
            <label for="formGroupExampleInput" class="form-label">Tipo</label>
            <select type="text"
                   class="form-control"
                   id="TP_OcorrenciaId"
                   name="TP_OcorrenciaId"
                   placeholder="Tipo" >
            <option selected></option>
        </select>
        </div>
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Descrição</label>
            <input type="text"
                   class="form-control"
                   id="Description"
                   name="Description"
                   placeholder="Descrição" />
        </div>
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Informação adicional</label>
            <input type="text"
                   class="form-control"
                   id="Additional1"
                   name="Additional1"
                   placeholder="Informação adicional1" />
        </div>
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Informação adicional</label>
            <input type="text"
                   class="form-control"
                   name="Additional2"
                   id="Additional2"
                   placeholder="Informação adicional2" />
        </div>
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Prejudicado</label>
            <input type="text"
                   class="form-control"
                   name="Harmed"
                   id="Harmed"
                   placeholder="Prejudicado" />
        </div>
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Documento</label>
            <input type="text"
                   class="form-control"
                   name="Document"
                   id="Document"
                   placeholder="Documento" />
        </div>
    </div>

    <div style="width: 46%">
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Custo</label>
            <input type="text"
                   class="form-control"
                   id="Cost"
                   name="Cost"
                   placeholder="Custo" />
        </div>
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Avaliador</label>
            <select type="text"
                   class="form-control"
                   id="AppraiserId"
                   name="AppraiserId"
                   placeholder="Avaliador">
        <option selected></option>
        </select>
        </div>
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Origem</label>
            <input type="text"
                   class="form-control"
                   id="Origin"
                   name="Origin"
                   placeholder="Origem" />
        </div>
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Status das ações</label>
            <input type="text"
                   class="form-control"
                   id="StatusActions"
                   name="StatusActions"
                   placeholder="Status das ações" />
        </div>
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Status</label>
            <input type="text"
                   class="form-control"
                   id="StatusId"
                   name="StatusId"
                   placeholder="Status" />
        </div>
        <div class="mb-3">
            <label for="formGroupExampleInput" class="form-label">Ação corretiva</label>
            <input type="text"
                   class="form-control"
                   id="CorrectiveAction"
                   name="CorrectiveAction"
                   placeholder="Ação corretiva" />
        </div>
        <div class="button-search">
            <button id="submitFilter" type="submit" class="btn btn-primary">Procurar</button>
        </div>
    </div>
    <i id="closeForm" onClick=closeForm(this) style="font-size: 2rem;color: red;cursor:pointer;height: fit-content;" class='bx bxs-x-circle'></i>
</form>
    `
    return form;
}

async function carregarSelects() {
    const users = await getDataFromAPI("/Account/GetUsersPerOrganization?organizationId=ac60ba3b-dd8a-45ca-a572-59901b0986ef");
    displaySelectUsers(users);
    const types = await getDataFromAPI("/Occurrences/GetTypesOccurrencesPerOrganization?organizationId=ac60ba3b-dd8a-45ca-a572-59901b0986ef");
    displaySelectTypes(types)
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