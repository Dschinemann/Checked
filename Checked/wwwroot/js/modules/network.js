'use strict'

async function getDataFromAPI(url,obj) {
    try {
        let response = await fetch(url);
        return response.json();
    } catch (err) {
        return {
            err
        }
    }          
}

export default getDataFromAPI;