let arrow = document.querySelectorAll(".arrow");
for (var i = 0; i < arrow.length; i++) {
    arrow[i].addEventListener("click", (e) => {
        let arrowParent = e.target.parentElement.parentElement;//selecting main parent of arrow
        arrowParent.classList.toggle("showMenu");
    });
}
let sidebar = document.querySelector(".sidebar");
let sidebarBtn = document.querySelector(".bx-menu");
let classCircle = document.querySelectorAll(".circle");

sidebarBtn.addEventListener("click", () => {
    sidebar.classList.toggle("close");
    for (i = 0; i < classCircle.length; i++) {
        classCircle[i].classList.toggle("discard-circle")
    }
});
