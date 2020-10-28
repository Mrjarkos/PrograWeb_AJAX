window.onload = function () {
    PedirDatos();
    id_search.addEventListener('change', PedirDatos);
    id_sensor_search.addEventListener('change', PedirDatos);
    medicion_search.addEventListener('change', PedirDatos);
    Fecha_search.addEventListener('change', PedirDatos);
}

function PedirDatos() {

    var tableHeaderRowCount = 2;
    var table = document.getElementById('myTable');
    var rowCount = table.rows.length;
    for (var i = tableHeaderRowCount; i < rowCount; i++) {
        table.deleteRow(tableHeaderRowCount);
    }

    var parameters = "?id_search=" + id_search.value + "&" + "id_sensor_search=" + id_sensor_search.value +
        "&" + "medicion_search=" + medicion_search.value + "&" + "Fecha_search=" + Fecha_search.value;
    var xh = new XMLHttpRequest();
    xh.open("GET", "Sensor/GetRegister"+parameters, true)
    xh.responseType = "json";
    xh.onreadystatechange = function () {
        if (xh.readyState == 4) {
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
    
    for (i = 0; i < Registros["sensores"].length; i++) {
        row = myTable.insertRow();
        cell = row.insertCell(0);
        cell.innerHTML = Registros["sensores"][i].ID_REG;
        cell = row.insertCell(1);
        cell.innerHTML = Registros["sensores"][i].ID_SENSOR;
        cell = row.insertCell(2);
        cell.innerHTML = Registros["sensores"][i].MEDICION;
        cell = row.insertCell(3);

        var value = new Date(parseInt(Registros["sensores"][i].FECHAYHORA.substr(6)));
        var formatted_date = value.getFullYear() + "/" + ("0" + value.getMonth()).slice(-2) + "/" + ("0" + value.getDate()).slice(-2) + " " + ("0" + value.getHours()).slice(-2) + ":" + ("0" + value.getMinutes()).slice(-2) + ":" + ("0" + value.getSeconds()).slice(-2)
        console.log(value)
        cell.innerHTML = formatted_date;
    }
}