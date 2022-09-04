/*
    modal edit
 */

const closeModalEdit = document.querySelector("#close-form");
closeModalEdit.addEventListener("click", () => {
    let modal = document.querySelector(".modal-complement-edit");
    modal.classList.toggle("display-off")
})

const opeModal = document.querySelector("#open-modal")
opeModal.addEventListener("click", () => {
    let modal = document.querySelector(".modal-complement-edit");
    modal.classList.toggle("display-off")
})