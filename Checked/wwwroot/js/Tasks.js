let iconFilter = document.getElementById("iconFilter");
const menuNav = document.getElementById("menuNav");
const mascara = document.getElementById("mascara");
const header = document.querySelectorAll(".header-section");
const panels = document.querySelectorAll(".panels-section");

panels.forEach(ele => {
    ele.addEventListener("click", consultarDetalhes)
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
    ele.addEventListener('change', getData)
})


function getData(event) {
    let titleHeader = document.querySelector("#titulo-header");

    if (event.target.id === "others") {
        fetch(`MyTasks/GetTaskPerType?type=${event.target.id}`)
            .then(response => response.json())
            .then(data => displayUserTasks(data))
            .catch(error => console.log(error));
        titleHeader.innerText = setTitle(event.target.id);
        return;
    }
    fetch(`MyTasks/GetTaskPerType?type=${event.target.id}`)
        .then(response => response.json())
        .then(data => displayData(data, event))
        .catch(error => console.log(error))

    header.forEach(c => {
        c.style.position = "sticky";
    })
    menuNav.style.display = "none";
    mascara.style.display = "none";

    titleHeader.innerText = setTitle(event.target.id);   
    
}

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
        sections.setAttribute("ondrop", "drop_handler(event)");
        const headerSection = document.createElement("div");
        headerSection.className = "header-section";
        const h3 = document.createElement("h3");
        const h3Count = document.createElement("h3");
        h3.innerText = prop;
        h3.className = "text-capitalize";

        h3Count.innerText = countPanels;
        h3Count.classList.add("countElements")
        headerSection.appendChild(h3);
        headerSection.appendChild(h3Count);

        sections.appendChild(headerSection);


        if (countPanels) {
            for (let x = 0; x < countPanels; x++) {
                let boxes = data[prop][x].split(",");

                const formPanels = document.createElement("form");
                formPanels.className = "panels-section";
                formPanels.setAttribute("id", boxes[2]);
                formPanels.setAttribute("data-type", boxes[3]);
                formPanels.setAttribute("action", `/${boxes[3]}/Edit`);

                const inputhidden = document.createElement("input");
                inputhidden.setAttribute("value", boxes[2])
                inputhidden.setAttribute("type", "hidden");
                inputhidden.setAttribute("name", `${boxes[4]}`)
                formPanels.appendChild(inputhidden)

                const divTitulo = document.createElement("div");
                divTitulo.className = "title";

                const h4 = document.createElement("h4");
                h4.innerText = boxes[0]

                divTitulo.appendChild(h4);
                formPanels.appendChild(divTitulo)

                const divDescricao = document.createElement("div");
                const p = document.createElement("p");
                p.innerText = boxes[1]
                divDescricao.appendChild(p);
                formPanels.appendChild(divDescricao);
                sections.appendChild(formPanels);
                formPanels.addEventListener("click", consultarDetalhes)
            }
        }

        panels.appendChild(sections);
    }
    dragAndDrop()
}

function consultarDetalhes(event) {
    if (event.target.tagName === "FORM") {
        event.target.submit();
        return;
    }
}

/*Drag and drop*/

function dragAndDrop() {
    const panelSections = document.querySelectorAll(".panels-section");
    panelSections.forEach(element => {
        element.addEventListener("dragstart", (event) => {
            event.dataTransfer.setData("id", event.target.id)
            updateHeaderSection(event, "dragstart")
        })
    })

    panelSections.forEach(element => {
        element.addEventListener("dragend", (event) => {            
            updateHeaderSection(event, "dragend")
        })
    })

    const sections = document.querySelectorAll("section");
    sections.forEach(section => {
        section.addEventListener("dragover", (event) => {
            event.preventDefault();
        })
    })
}



function drop_handler(event) {
    event.preventDefault();
    let data = event.dataTransfer.getData("id");
    if (event.toElement.localName === "section") {
        event.target.appendChild(document.getElementById(data));
        updateItem(data, event);
    }
}

function updateItem(id, node) {
    let taskId = id;
    let statusTaskId = node.target.id;
    let formTask = new FormData();
    let inputToken = document.querySelector('input[name="__RequestVerificationToken"]')

    formTask.append("Id", taskId)
    formTask.append("StatusID", statusTaskId)
    formTask.append("__RequestVerificationToken", inputToken.value)
    CreateOrUpdateTask(formTask);
    updateHeaderSection(node);
}


//tasks do usuário
function displayUserTasks(data) {
    const panels = document.getElementById("panels");
    panels.innerHTML = "";
    header.forEach(c => {
        c.style.position = "sticky";
    })
    menuNav.style.display = "none";
    mascara.style.display = "none";


    //Form nova task
    const sectionForm = document.createElement("section");    
    //sectionForm.style.padding = "10px";
    const formTask = document.createElement("div")
    const errorSpan = document.createElement("span");
    errorSpan.className = "text-danger";
    formTask.innerHTML = `
    <div class="header-section">
        <h3>Nova tarefa</h3>
    </div>
    <form id="createTaskForm" method="POST" style="border: 1px solid #0000006b;border-radius: 10px;margin: 10px;">
        <input name="Id" type="hidden" value=""/>
         <input name="StatusID" type="hidden" value=""/>
        <div style="padding: 10px;">          
          <div class="mb-3">
            <label for="Title" class="form-label"
              >Título</label
            >
            <input
              type="text"
              class="form-control"
              id="Title"
              name="Title"
              placeholder="Título"
            />
          </div>
          <div class="mb-3">
            <label for="Description" class="form-label"
              >Descrição</label
            >
            <textarea
              type="text"
              class="form-control"
              id="Description"
              name="Description"
              placeholder="Descrição"
              rows="10"
            ></textarea>
          </div>
            <input form="createTaskForm" id="submitTask" type="submit" class="btn btn-primary" value="Salvar"/>
        </div>
    </form>`;

    sectionForm.appendChild(errorSpan);
    sectionForm.appendChild(formTask);
    panels.appendChild(sectionForm);

    const button = document.querySelector("#submitTask")

    button.addEventListener("click", e => {
        e.preventDefault();
        submitNewTask();
    })

    if (data.error) {        
        errorSpan.innerText = data.error;
        return;
    }
    let IdSection = 1;
    for (const prop in data) {

        let countPanels = data[prop].length;

        const sections = document.createElement("section");
        sections.setAttribute("ondrop", "drop_handler(event)");
        sections.setAttribute("Id", IdSection );
        const headerSection = document.createElement("div");
        headerSection.className = "header-section";
        const h3 = document.createElement("h3");
        const h3Count = document.createElement("h3");
        h3Count.classList.add("countElements");
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
                divPanels.setAttribute("id", boxes[2]);
                divPanels.setAttribute("data-type", "task")
                divPanels.setAttribute("draggable",true)


                divPanels.innerHTML = `
                    <div class="title">
                        <h4>${boxes[0]}</h4>
                    </div>
                    <div>
                        <p>${boxes[1]}</p>
                    </div>
                    <div style="text-align: end;font-size: 1.2rem;">
                        <i data-id=${boxes[2]} style="color: red;padding: 0 5px 0 5px;" class="bx bx-trash"></i>
                    	<i data-id=${boxes[2]} style="padding: 0 5px 0 5px;" class="bx bxs-edit-alt edit-task"></i>
                    </div>
                    <input name="Status" type="hidden" value=${boxes[4]} />
                `
                sections.appendChild(divPanels);                
            }
        }        
        panels.appendChild(sections);
        IdSection = IdSection + 1;
    }
    const trash = document.querySelectorAll(".bx-trash")
    trash.forEach(element => element.addEventListener("click", deleteItem))
    

    const editTask = document.querySelectorAll(".edit-task")
    editTask.forEach(element => element.addEventListener("click", buttonEditTask));

    dragAndDrop();
}

function submitNewTask() {
    let title = document.querySelector("#Title");
    let description = document.querySelector("#Description");
    if (title.value.length === 0 || title.value === undefined) {
        let message = {
            error:"Campo titulo esta vazio"
        }
        displayUserTasks(message)
        return;
    }
    if (description.value.length === 0 || description.value === undefined) {
        let message = {
            error: "Campo Descrição esta vazio"
        }
        displayUserTasks(message)
        return;
    }

    let inputToken = document.querySelector('input[name="__RequestVerificationToken"]')
    let inputId = document.querySelector("input[name='Id']");
    let inputStattusId = document.querySelector("input[name='StatusID']");

    let formData = new FormData();
    formData.append("Title", title.value )
    formData.append("Description", description.value)
    formData.append("Id", inputId.value)
    formData.append("StatusID", inputStattusId.value)
    formData.append("__RequestVerificationToken", inputToken.value)

    fetch("/MyTasks/createOrUpdate", {
        method: "POST",
        body: formData
    })
        .then(response => response.json())
        .then(data => displayUserTasks(data))
        .catch(error => console.log(error))
}

//
function deleteItem(element) {
    let id = element.target.dataset.id;
    let formData = new FormData();
    let inputToken = document.querySelector('input[name="__RequestVerificationToken"]')
    formData.append("id", id)
    formData.append("__RequestVerificationToken", inputToken.value)

    fetch("/MyTasks/DeleteTask", {
        body: formData,
        method:"POST"
    }).then(res => res.json())
        .then(data => displayUserTasks(data))
    .catch(error => console.log(error))
}

function buttonEditTask(element) {
    let parent = element.target.parentNode.parentNode;
    let title = parent.querySelector("h4").innerText;
    let description = parent.querySelector("p").innerText;
    let Status = parent.querySelector("input[name='Status']");

    let inputTitle = document.querySelector("input[id='Title']");
    let inputDescription = document.querySelector("textarea[id='Description']");
    let inputId = document.querySelector("input[name='Id']");
    let inputStattusId = document.querySelector("input[name='StatusID']");

    inputTitle.value = title;
    inputDescription.value = description;
    inputId.value = event.target.dataset.id;
    inputStattusId.value = Status.value;
}

function CreateOrUpdateTask(task) {
    fetch("/MyTasks/createOrUpdate", {
        method: "POST",
        body: task
    })
        .catch(error => console.log(error))
}

function updateHeaderSection(node, type) {    
    if (type === "dragend") {
        let headerCounter = node.target.parentNode.querySelector(".countElements");        
        headerCounter.innerText = node.target.parentNode.childElementCount - 1;
        return;
    }

    if (type === "dragstart") {
        let headerCounter = node.target.parentNode.querySelector(".countElements");
        headerCounter.innerText = node.target.parentNode.childElementCount - 2;
        return;
    }
    let headerCounter = node.target.querySelector(".countElements");
    headerCounter.innerText = node.target.childElementCount - 1
}

function setTitle(title) {
    switch (title) {
        case "plans":
            return "Planos"
        case "actions":
            return "Ações"
        case "others":
            return "Minhas Tarefas"
        default:
            return "";
    }
}
