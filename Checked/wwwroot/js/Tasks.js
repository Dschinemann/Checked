let arrows = document.querySelectorAll(".oc");
for (let i = 0; i < arrows.length; i++) {
    arrows[i].addEventListener("click", e => {
        fetch(`MyTasks/GetOccurrencePerStatus?status=${e.target.id}`)
            .then(response => response.json())
            .then(data => displayOccurrencesPerStatus(data,e))
            .catch(error => console.error(error))
    })
}

function displayOccurrencesPerStatus(data, element) {
    clock = element.target.parentElement.childNodes[1].classList.value
    text = element.target.parentElement.childNodes[5].innerHTML
    let ul = document.getElementById("subItensOc");
    let ul1 = document.getElementById("subItensPlan");
    let ul2 = document.getElementById("subItensAc");
    const remove = $(ul)
    const remove1 = $(ul1)
    const remove2 = $(ul2)
    remove.empty();
    remove1.empty();
    remove2.empty();
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
        a.innerHTML = "Descrição: " + e.description + "   |    Prejudicado:    " + e.harmed + "   |    Custo:    " + e.cost + "   |    Origem: "+ e.origin;
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
    let ul1 = document.getElementById("subItensAc");
    let ul2 = document.getElementById("subItensOc");
    const remove = $(ul)
    const remove1 = $(ul1)
    const remove2 = $(ul2)
    remove.empty();
    remove1.empty();
    remove2.empty();

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
        a.innerHTML = "Assunto: " + e.Subject + "   |    Objetivo:    " + e.Objective + "   |    Prazo:    " + e.Goal
        div2.classList.add("list-details")
        div2.appendChild(a)
        li.appendChild(div2);

        ul.appendChild(li);
    })
}

let arrowsPlans = document.querySelectorAll(".pl");
for (let i = 0; i < arrowsPlans.length; i++) {
    arrowsPlans[i].addEventListener("click", e => {
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
    let ul1 = document.getElementById("subItensOc");
    let ul2 = document.getElementById("subItensPlan");

    const remove = $(ul)
    const remove1 = $(ul1)
    const remove2 = $(ul2)
    remove.empty();
    remove1.empty();
    remove2.empty();


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
        a.innerHTML = "What: " + e.what + "   |    Where:    " + e.where + "   |    How:    " + e.how + "   |   Finalizar até:" + e.newFinish
        div2.classList.add("list-details")
        div2.appendChild(a)
        li.appendChild(div2);

        ul.appendChild(li);
    })
}

let arrowsActions = document.querySelectorAll(".ac");
for (let i = 0; i < arrowsActions.length; i++) {
    arrowsActions[i].addEventListener("click", e => {
        let status = e.target.dataset["valueStatus"] == undefined ? "" : e.target.dataset["valueStatus"];
        fetch(`MyTasks/GetActionsPerStatus?initial=${e.target.dataset["valueInit"]}&final=${e.target.dataset["valueFinal"]}&status=${status}`)
            .then(response => response.json())
            .then(data => displayActionsPerStatus(data, e))
            .catch(error => console.error(error))
    })
}
