let arrows = document.querySelectorAll(".oc");
for (let i = 0; i < arrows.length; i++) {
    arrows[i].addEventListener("click", e => {
        arrowDown(e);
        if (e.target.style.transform == "rotate(180deg)") {
            e.target.style.transform = ""
            removerLista();
            return;
        }
        fetch(`MyTasks/GetOccurrencePerStatus?status=${e.target.id}`)
            .then(response => response.json())
            .then(data => displayOccurrencesPerStatus(data, e))
            .catch(error => console.error(error))
    })
}

function displayOccurrencesPerStatus(data, element) {
    clock = element.target.parentElement.childNodes[1].classList.value
    text = element.target.parentElement.childNodes[5].innerHTML
    let ul = document.getElementById("subItensOc");
    removerLista();
    element.target.style.transform = "rotate(180deg)";
    if (data.length == 0) {
        let li = document.createElement("li");
        let div1 = document.createElement("div");
        let div2 = document.createElement("div");
        let i = document.createElement("i")
        let h6 = document.createElement("h6");
        let span = document.createElement("span");

        i.classList.value = clock;

        li.classList.add("list")
        li.classList.add("em-tempo")

        h6.innerHTML = text;
        div1.appendChild(i)
        div1.appendChild(h6)
        li.appendChild(div1);


        span.innerHTML = `Não há registros com esse status: ${text}`;
        div2.classList.add("list-details")
        div2.appendChild(span)
        li.appendChild(div2);

        ul.appendChild(li);
    }
    data.forEach(e => {
        let li = document.createElement("li");
        let div1 = document.createElement("div");
        let div2 = document.createElement("div");
        let i = document.createElement("i")
        let h6 = document.createElement("h6");
        let a = document.createElement("a");

        i.classList.value = clock;

        li.classList.add("list")
        li.classList.add("em-tempo")

        h6.innerHTML = text;
        div1.appendChild(i)
        div1.appendChild(h6)
        li.appendChild(div1);

        a.href = `/Occurrences/Details?idOccurrence=${e.id}`
        a.innerHTML = "<span style='font-weight: bold'>Descrição:</span>" +
            e.description + "   |   <span style='font-weight: bold'>Prejudicado:</span>    " +
            e.harmed + "   |     <span style='font-weight: bold'>Custo:</span>    " +
            e.cost + "   |     <span style='font-weight: bold'>Origem:</span> " +
            e.origin;
        div2.classList.add("list-details")
        div2.appendChild(a)
        li.appendChild(div2);

        ul.appendChild(li);
    })
}

function displayPlansPerStatus(data, element) {
    clock = element.target.parentElement.childNodes[1].classList.value
    text = element.target.parentElement.childNodes[5].innerHTML
    let ul = document.getElementById("subItensPlan");
    removerLista();
    element.target.style.transform = "rotate(180deg)";
    if (data.length == 0) {
        let li = document.createElement("li");
        let div1 = document.createElement("div");
        let div2 = document.createElement("div");
        let i = document.createElement("i")
        let h6 = document.createElement("h6");
        let span = document.createElement("span");

        i.classList.value = clock;

        li.classList.add("list")
        li.classList.add("em-tempo")

        h6.innerHTML = text;
        div1.appendChild(i)
        div1.appendChild(h6)
        li.appendChild(div1);


        span.innerHTML = `Não há registros com esse status: ${text}`;
        div2.classList.add("list-details")
        div2.appendChild(span)
        li.appendChild(div2);

        ul.appendChild(li);
    }
    data.forEach(e => {
        let li = document.createElement("li");
        let div1 = document.createElement("div");
        let div2 = document.createElement("div");
        let i = document.createElement("i")
        let h6 = document.createElement("h6");
        let a = document.createElement("a");

        i.classList.value = clock;

        li.classList.add("list")
        li.classList.add("em-tempo")

        h6.innerHTML = text;
        div1.appendChild(i)
        div1.appendChild(h6)
        li.appendChild(div1);

        a.href = `/Plans/Index?planId=${e.Id}`
        a.innerHTML = "<span style='font-weight: bold'>Assunto:</span> " +
            e.Subject + "   |    <span style='font-weight: bold'>Objetivo:</span>    " +
            e.Objective + "   |    <span style='font-weight: bold'>Prazo:</span>   " +
            new Intl.DateTimeFormat("pt-BR").format(new Date(e.Goal))
        div2.classList.add("list-details")
        div2.appendChild(a)
        li.appendChild(div2);

        ul.appendChild(li);
    })
}

let arrowsPlans = document.querySelectorAll(".pl");
for (let i = 0; i < arrowsPlans.length; i++) {
    arrowsPlans[i].addEventListener("click", e => {
        arrowDown(e);
        if (e.target.style.transform == "rotate(180deg)") {
            e.target.style.transform = ""
            removerLista();
            return;
        }
        let status = e.target.dataset["valueStatus"] == undefined ? "" : e.target.dataset["valueStatus"];
        fetch(`MyTasks/GetPlansPerStatus?initial=${e.target.dataset["valueInit"]}&final=${e.target.dataset["valueFinal"]}&status=${status}`)
            .then(response => response.json())
            .then(data => displayPlansPerStatus(data, e))
            .catch(error => console.error(error))
    })
}


function displayActionsPerStatus(data, element) {
    clock = element.target.parentElement.childNodes[1].classList.value
    text = element.target.parentElement.childNodes[5].innerHTML
    let ul = document.getElementById("subItensAc");
    removerLista();
    element.target.style.transform = "rotate(180deg)";
    if (data.length == 0) {
        let li = document.createElement("li");
        let div1 = document.createElement("div");
        let div2 = document.createElement("div");
        let i = document.createElement("i")
        let h6 = document.createElement("h6");
        let span = document.createElement("span");

        i.classList.value = clock;

        li.classList.add("list")
        li.classList.add("em-tempo")

        h6.innerHTML = text;
        div1.appendChild(i)
        div1.appendChild(h6)
        li.appendChild(div1);


        span.innerHTML = `Não há registros com esse status: ${text}`;
        div2.classList.add("list-details")
        div2.appendChild(span)
        li.appendChild(div2);

        ul.appendChild(li);
    }
    data.forEach(e => {
        let li = document.createElement("li");
        let div1 = document.createElement("div");
        let div2 = document.createElement("div");
        let i = document.createElement("i")
        let h6 = document.createElement("h6");
        let a = document.createElement("a");

        i.classList.value = clock;

        li.classList.add("list")
        li.classList.add("em-tempo")

        h6.innerHTML = text;
        div1.appendChild(i)
        div1.appendChild(h6)
        li.appendChild(div1);

        a.href = `/Actions/Details?actionId=${e.id}`
        a.innerHTML = "<span style='font-weight: bold'>What:</span> " +
            e.what + "   |    <span style='font-weight: bold'>Where:</span>     " +
            e.where + "   |    <span style='font-weight: bold'>How:</span>    " +
            e.how + "   |   <span style='font-weight: bold'>Finalizar até:</span> " +
            new Intl.DateTimeFormat("pt-BR").format(new Date(e.newFinish))
        div2.classList.add("list-details")
        div2.appendChild(a)
        li.appendChild(div2);

        ul.appendChild(li);
    })
}

let arrowsActions = document.querySelectorAll(".ac");
for (let i = 0; i < arrowsActions.length; i++) {
    arrowsActions[i].addEventListener("click", e => {
        arrowDown(e);
        if (e.target.style.transform == "rotate(180deg)") {
            e.target.style.transform = ""
            removerLista();
            return;
        }
        let status = e.target.dataset["valueStatus"] == undefined ? "" : e.target.dataset["valueStatus"];
        fetch(`MyTasks/GetActionsPerStatus?initial=${e.target.dataset["valueInit"]}&final=${e.target.dataset["valueFinal"]}&status=${status}`)
            .then(response => response.json())
            .then(data => displayActionsPerStatus(data, e))
            .catch(error => console.error(error))
    })
}

function removerLista() {
    let li = document.querySelectorAll(".list")
    for (let item of li) {
        item.remove();
    }
}
function arrowDown(element) {
    let arrows = document.querySelectorAll(".arrow-down");
    for (let item of arrows) {        
            if (element.target != item) {
                item.style.transform = "";
            }
    }
}
