let iconFilter = document.getElementById("iconFilter");
const menuNav = document.getElementById("menuNav");
const mascara = document.getElementById("mascara");
const header = document.querySelectorAll(".header-section");
const panels = document.querySelectorAll(".panels-section");

panels.forEach(ele => {
    ele.addEventListener("click", consultarDetalhes )
})

iconFilter.addEventListener("click", (e) => {

    if (menuNav.style.display == "none") {
        header.forEach(c => {
            c.style.position = "initial";
        })
        mascara.style.display = "block"
        menuNav.style.display = "block"
    } else {
        header.forEach(c => {
            c.style.position = "sticky";
        })
        menuNav.style.display = "none";
        mascara.style.display = "none";
    }
})

let radios = document.querySelectorAll(".form-check-input");

radios.forEach(ele => {
    ele.addEventListener('change', e => {

        fetch(`MyTasks/GetTaskPerType?type=${e.target.id}`)
            .then(response => response.json())
            .then(data => displayData(data, e))
            .catch(error => console.log(error))

        header.forEach(c => {
            c.style.position = "sticky";
        })
        menuNav.style.display = "none";
        mascara.style.display = "none";
    })
})

function displayData(data, e) {
    if (data == null) return;

    const panels = document.getElementById("panels");
    panels.innerHTML = "";

    if (data.error) {
        const h3 = document.createElement("h3");
        h3.innerText = data.error;
        h3.className = "text-error";
        panels.appendChild(h3);
        return
    }

    for (const prop in data) {

        let countPanels = data[prop].length;

        const sections = document.createElement("section");
        const headerSection = document.createElement("div");
        headerSection.className = "header-section";
        const h3 = document.createElement("h3");
        const h3Count = document.createElement("h3");
        h3.innerText = prop;
        h3.className = "text-capitalize";

        h3Count.innerText = countPanels;
        headerSection.appendChild(h3);
        headerSection.appendChild(h3Count);

        sections.appendChild(headerSection);        
        

        if (countPanels) {
            for (let x = 0; x < countPanels; x++) {
                let boxes = data[prop][x].split(",");

                const divPanels = document.createElement("div");
                divPanels.className = "panels-section";
                divPanels.setAttribute("data-id", boxes[2])

                const divTitulo = document.createElement("div");
                divTitulo.className = "title";

                const h4 = document.createElement("h4");
                h4.innerText = boxes[0]

                divTitulo.appendChild(h4);
                divPanels.appendChild(divTitulo)

                const divDescricao = document.createElement("div");
                const p = document.createElement("p");
                p.innerText = boxes[1]
                divDescricao.appendChild(p);
                divPanels.appendChild(divDescricao);
                sections.appendChild(divPanels);
                divPanels.addEventListener("click", consultarDetalhes)
            }
        }

        panels.appendChild(sections);
    }

}

function consultarDetalhes(event) {
    fetch("/Occurrences/Details")    
}

/*Drag and drop*/

const panelSections = document.querySelectorAll(".panels-section");
panelSections.forEach(element => {
    element.addEventListener("dragstart", (event) => {        
        event.dataTransfer.setData("id",event.target.id)
    })
})

const sections = document.querySelectorAll("section");
sections.forEach(section => {
    section.addEventListener("dragover", (event) => {
        event.preventDefault();
    })
})

function drop_handler(event) {
    event.preventDefault();
    let data = event.dataTransfer.getData("id");
    if (event.toElement.localName === "section") {
        event.target.appendChild(document.getElementById(data));
        updateItem(data, event);
    }
}

function updateItem(id, node) {
    let status = node.target.querySelector(".text-capitalize").innerText;
    let tipoElemento = node.target.dataset.type;    
}