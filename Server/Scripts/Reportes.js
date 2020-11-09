﻿window.onload = function () {
    PedirDatos();
    id_search.addEventListener('keyup', function () { PedirDatos() });
    id_sensor_search.addEventListener('keyup', function () { PedirDatos() });
    medicion_search.addEventListener('keyup', function () { PedirDatos() });
    longitud_search.addEventListener('keyup', function () { PedirDatos() });
    latitud_search.addEventListener('keyup', function () { PedirDatos() });
    Date_init_search.addEventListener('keyup', function () { PedirDatos() });
    Date_final_search.addEventListener('keyup', function () { PedirDatos() });
    N_items_select.addEventListener('change', function () { PedirDatos() });

    B0.addEventListener('click', function () { PedirDatos(B0.value) });
    B1.addEventListener('click', function () { PedirDatos(B1.value) });
    B2.addEventListener('click', function () { PedirDatos(B2.value) });
    B3.addEventListener('click', function () { PedirDatos(B3.value) });
    B4.addEventListener('click', function () { PedirDatos(B4.value) });
    B5.addEventListener('click', function () { PedirDatos(B5.value) });
    BN.addEventListener('click', function () { PedirDatos(BN.value) });
    Bback.addEventListener('click', function () { PedirDatos(B2.value) });
    Bfollow.addEventListener('click', function () { PedirDatos(B4.value) });
}

function PedirDatos(page) {
    Hide();
    if (page == null) {
        page = 1;
    }
    else {

    }

    var tableHeaderRowCount = 2;
    var table = document.getElementById('myTable');
    var rowCount = table.rows.length;
    for (var i = tableHeaderRowCount; i < rowCount; i++) {
        table.deleteRow(tableHeaderRowCount);
    }

    var parameters = "?id_search=" + id_search.value + "&" + "id_sensor_search=" + id_sensor_search.value +
        "&" + "medicion_search=" + medicion_search.value + "&" + "latitud_search=" + latitud_search.value +
        "&" + "longitud_search=" + longitud_search.value + 
        "&" + "Date_init_search=" + Date_init_search.value + "&" + "Date_final_search=" + Date_final_search.value +
        "&" + "Page=" + page + "&" + "N_items=" + N_items_select.value;
    var xh = new XMLHttpRequest();
    xh.open("GET", "/Reporte/GetRegister" + parameters, true)
    xh.responseType = "json";
    xh.onreadystatechange = function () {
        if (xh.readyState == 4) {
            console.log(xh.status);
            if (xh.status == 200) {
                console.log('OK');
                ListRegister(xh.response)
            }
            else {
                console.log('Error');
            }
        }
    }
    xh.send();

}

function ListRegister(Registros) {

    for (i = 0; i < Registros["Registros"].length; i++) {
        row = myTable.insertRow();
        cell = row.insertCell(0);
        cell.innerHTML = Registros["Registros"][i].ID;
        cell = row.insertCell(1);
        cell.innerHTML = Registros["Registros"][i].SERIALSENSOR;
        cell = row.insertCell(2);
        cell.innerHTML = Registros["Registros"][i].MEDICION.toString();
        cell = row.insertCell(3);
        cell.innerHTML = Registros["Registros"][i].LATITUD.toString();
        cell = row.insertCell(4);
        cell.innerHTML = Registros["Registros"][i].LONGITUD.toString();
        cell = row.insertCell(5);
        var value = new Date(parseInt(Registros["Registros"][i].DATEREPORTED.substr(6)));
        cell.innerHTML = formatted_date(value);
        
        Paginas(parseInt(Registros["PaginaActual"]), parseInt(Registros["TotalPaginas"]));
    }
}

function formatted_date(value) {
    return value.getFullYear() + "/" + ("0" + value.getMonth()).slice(-2) + "/" + ("0" + value.getDate()).slice(-2) + " " + ("0" + value.getHours()).slice(-2) + ":" + ("0" + value.getMinutes()).slice(-2) + ":" + ("0" + value.getSeconds()).slice(-2);
}

function Paginas(PA, PT) {

    B1.value = PA - 2
    B2.value = PA - 1;
    B3.value = PA;
    B4.value = PA + 1;
    B5.value = PA + 2;


    if (PA > 1) {
        Bback.style.display = "inline";
        B2.style.display = "inline";
        if (PA > 2) {
            B1.style.display = "inline";
            if (PA > 3) {
                B0.style.display = "inline";
                B0.value = 1;
            }
        }
    }
    if (PA < PT) {
        Bfollow.style.display = "inline";
        B4.style.display = "inline";
        if (PA < PT - 1) {
            B5.style.display = "inline";
            if (PA < PT - 2) {
                BN.style.display = "inline";
                BN.value = PT;
            }
        }
    }

    B3.style.display = "inline";

}

function Hide() {
    Bback.style.display = "none";
    B0.style.display = "none";
    B1.style.display = "none";
    B2.style.display = "none";
    B3.style.display = "none";
    B4.style.display = "none";
    B5.style.display = "none";
    BN.style.display = "none";
    Bfollow.style.display = "none";
}