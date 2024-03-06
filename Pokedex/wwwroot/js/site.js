// Definir una función llamada activateInputTipo2
function activateInputTipo2() {
    // Obtener los elementos con id "check", "select" y guardarlo en variables
    var check = document.getElementById("check");
    var select = document.getElementById("select")
    // Si el elemento check está marcado
    if (check.checked) {
        // Habilitar el elemento select
        select.disabled = false;
    }
    // Si no
    else {
        // Deshabilitar el elemento select
        select.disabled = true;
    }
}

// El siguiente código JavaScript para la función buscar,
// que usa jQuery para simplificar la llamada AJAX:
function getSuggestionsList() {
    // Obtener el valor del input
    var valor = $("#searchPokemon").val();
    var suggestions = $("#suggestionsList");

    // Si el valor está vacío, limpiar las sugerencias
    if (valor == "" || null) {
        suggestions.empty();
        return;
    }
    // Convertir el valor a minúsculas
    valor = valor.toLowerCase();
    // Enviar una solicitud AJAX al método Buscar del controlador Pokedex
    $.ajax({
        url: "/Pokedex/GetSuggestions",
        type: "GET",
        data: { valor: valor },
        success: function (data) {
            // Si la solicitud tiene éxito, agregar los datos recibidos al elemento resultados
            suggestions.html(data);
        },
        error: function (error) {
            // Si la solicitud falla, mostrar un mensaje de error
            suggestions.html("Error al buscar: " + error);
        }
    });
}

// Obtener el valor del elemento clicado
function getSuggestionItem(valor) {
    // Asignar el valor al input
    $("#searchPokemon").val(valor);
    // Limpiar la lista
    $("#suggestionsList").empty();
}

// Añadir escuchador de eventos al documento
$(document).on("click",
    function (e) {
        // Obtener el input y la lista de sugerencias
        var input = $("#searchPokemon");
        var suggestions = $("#suggestionsList");
        // Comprobar si el click fue fuera del input y la lista
        if ((!suggestions.is(e.target) && suggestions.has(e.target).length === 0) && (!input.is(e.target) && input.has(e.target).length === 0)) {
            // Vaciar la lista de sugerencias
            suggestions.empty();
        }
    }
);
