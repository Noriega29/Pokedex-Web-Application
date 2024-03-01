var check = document.getElementById("check");
var select = document.getElementById("select")

check.addEventListener("change",
    function () {
        if (check.checked) {
            select.disabled = false;
        }
        else {
            select.disabled = true;
        }
    }
);
