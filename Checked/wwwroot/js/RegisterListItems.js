const country = document.getElementById("CountryId");
const states = document.getElementById("StateId");
const cityes = document.getElementById("CityId")

country.addEventListener("change", (item) => {
    let countryId = country.options[country.selectedIndex].value;
    clearSelectItens("StateId");
    clearSelectItens("CityId");
    fetch(`GetStates?idCountry=${countryId}`)
        .then(response => response.json())
        .then(data => displayOptionsStates(data))
        .catch(error => console.error(error))
})

states.addEventListener("change", (item) => {

    let stateId = states.options[states.selectedIndex].value;
    clearSelectItens("CityId");
    fetch(`GetCityes?IdState=${stateId}`)
        .then(response => response.json())
        .then(data => displayOptionsCityes(data))
        .catch(error => console.error(error))
})

function displayOptionsStates(data) {   
    if (data.length <= 0) {
        let options = document.createElement("option");
        options.value = "-1";
        options.innerHTML = "Não há estados cadastrados para esta região";
        states.appendChild(options);
        return;
    }
    data.forEach(item => {       
        let options = document.createElement("option");
        options.value = parseInt(item.id);
        options.innerHTML= item.name
        states.appendChild(options)
    });
}

function displayOptionsCityes(data) {
    
    if (data.length <= 0) {
        let options = document.createElement("option");
        options.value = "-1";
        options.innerHTML = "Não há cidades cadastradas para esta região"
        cityes.appendChild(options);
        return
    }
    data.forEach(item => {        
        let options = document.createElement("option");
        options.value = parseInt(item.id);
        options.innerHTML = item.name;
        cityes.appendChild(options);
    });
}

function clearSelectItens(id) {
    let node = document.getElementById(id)
    node.options.length = 0;    
}

