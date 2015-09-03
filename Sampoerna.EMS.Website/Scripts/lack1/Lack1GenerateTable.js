function generateHeaderTable() {
    var rc =  /*start header*/
        '<thead><tr><th rowspan="2">No<br>Urut</th><th rowspan="2">Saldo<br>Awal</th><th colspan="2">Pemasukan</th><th rowspan="2">Penggunaan</th><th colspan="2">Hasil Produksi BKC</th><th rowspan="2">Saldo<br>Akhir</th><th rowspan="2">Keterangan</th></tr><tr><th>No. & Tgl. CK-5</th><th>Jumlah</th><th>Jenis</th><th>Jumlah</th></tr></thead>';
    /*end of header*/
    return rc;
}

function generateTable(data) {

    var rc = '<table border="0" class="table table-bordered">' + generateHeaderTable();
    rc = rc + '<tbody><tr><td>1</td><td>2</td><td>3</td><td>4</td><td>5</td><td>6</td><td>7</td><td>8</td><td>9</td></tr>';

    if (data.IncomeList.length > 0) {
        var rowIndex = 1;
        var rowCount = data.IncomeList.length;
        rc = rc +
        /*First Record*/
        '<tr><td>' + rowIndex + '</td>' +
        '<td rowspan="' + rowCount + '">' + ThausandSeperator(data.BeginingBalance, 2) + '</td>' +
        '<td>' + data.IncomeList[0].RegistrationNumber + ' - ' + data.IncomeList[0].StringRegistrationDate + '</td>' +
        '<td>' + ThausandSeperator(data.IncomeList[0].Amount, 2) + '</td>' +
        '<td rowspan="' + rowCount + '">' + ThausandSeperator(data.TotalUsage, 2) + '</td>' +
        '<td rowspan="' + rowCount + '">' + generateJenisHasilProduksi(data.ProductionList) + '</td>' +
        '<td rowspan="' + rowCount + '">' + generateJumlahHasilProduksi(data.ProductionList) + '</td>' +
        '<td rowspan="' + rowCount + '">' + ThausandSeperator(data.EndingBalance, 2) + '</td>' +
        '<td rowspan="' + rowCount + '">' + (data.Noted ? data.Noted : '') + '</td></tr>';
        /*loop record*/
        for (var i = 1; i < data.IncomeList.length; i++) {
            rowIndex = rowIndex + 1;
            var item = '<tr><td>' + rowIndex + '</td><td>' + data.IncomeList[0].RegistrationNumber + ' - ' + data.IncomeList[0].StringRegistrationDate + '</td>' +
                        '<td>' + data.IncomeList[0].Amount + '</td></tr>';
            rc.append(item);
        }
        /*end loop record*/
        $('#IncomeListCount').val(rowCount);
    } else {
        rc = rc +
        /*Only 1 Record*/
        '<tr><td>1</td>' +
        '<td>' + ThausandSeperator(data.BeginingBalance, 2) + '</td>' +
        '<td></td>' +
        '<td></td>' +
        '<td></td>' +
        '<td>' + generateJenisHasilProduksi(data.ProductionList) + '</td>' +
        '<td>' + generateJumlahHasilProduksi(data.ProductionList) + '</td>' +
        '<td>' + ThausandSeperator(data.EndingBalance, 2) + '</td>' +
        '<td>' + (data.Noted ? data.Noted : '') + '</td></tr>';
        $('#IncomeListCount').val(0);
    }
    /*footer*/
    rc = rc + '<tr><td></td><td></td><td></td><td>Total : '
        + ((data.TotalIncome == 0) ? '-' : ThausandSeperator(data.TotalIncome, 2)) + '</td><td></td><td></td><td>' + generateSummaryJumlahProduksi(data.SummaryProductionList) + '</td><td colspan="2"></td></tr>' +
        '</tbody></table>';

    return rc;
}

function generateJenisHasilProduksi(data) {
    var rc = "";
    if (data.length > 0) {
        rc = '<div id="jumlah-hasil-produksi-data">';
        for (var i = 0; i < data.length; i++) {
            if (i == data.length) {
                rc = rc + data[i].ProductAlias;
            } else {
                rc = rc + data[i].ProductAlias + "<br/>";
            }
        }
        rc = rc + '</div>';
    } else {
        rc = "-";
    }
    return rc;
}

function generateJumlahHasilProduksi(data) {
    var rc = "";
    if (data.length > 0) {
        for (var i = 0; i < data.length; i++) {
            if (i == data.length) {
                rc = rc + ThausandSeperator(data[i].Amount, 2);
            } else {
                rc = rc + ThausandSeperator(data[i].Amount, 2) + " " + data[i].UomDesc + "<br/>";
            }
        }
    } else {
        rc = "-";
    }
    return rc;
}

function generateSummaryJumlahProduksi(data) {
    var loopData = '-';
    if (data.length > 0) {
        var tb = '<table border="0">';
        for (var i = 0; i < data.length; i++) {
            var item = '<tr><td>' + data[i].Amount + '</td><td>' + data[i].UomDesc + '</td></tr>';
            tb = tb + item;
        }
        tb = tb + '</table>';
        loopData = tb;
    }
    var rc = '<table border="0"><tr><td>Total :</td><td>' + loopData + '</td></tr></table>';
    return rc;
}

function generateEmptyTable() {
    var rc = '<table border="0" class="table table-bordered">' + generateHeaderTable();
    rc = rc + '<tbody><tr><td>1</td><td>2</td><td>3</td><td>4</td><td>5</td><td>6</td><td>7</td><td>8</td><td>9</td></tr>';
    rc = rc +
        /*Only 1 Record*/
        '<tr><td></td>' +
        '<td></td>' +
        '<td></td>' +
        '<td></td>' +
        '<td></td>' +
        '<td></td>' +
        '<td></td>' +
        '<td></td>' +
        '<td rowspan="2"></td></tr>';
    /*footer*/
    rc = rc + '<tr><td></td><td></td><td></td><td>Total : -</td><td></td><td></td><td>Total: -</td><td></td></tr>' +
        '</tbody></table>';
    return rc;
}
