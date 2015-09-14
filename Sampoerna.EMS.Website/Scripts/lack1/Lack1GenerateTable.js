function generateHeaderTable() {
    var rc =  /*start header*/
        '<thead><tr><th rowspan="2">No<br>Urut</th><th rowspan="2">Saldo<br>Awal</th><th colspan="2">Pemasukan</th><th rowspan="2">Penggunaan</th><th colspan="2">Hasil Produksi BKC</th><th rowspan="2">Saldo<br>Akhir</th><th rowspan="2">Keterangan</th></tr><tr><th>No. & Tgl. CK-5</th><th>Jumlah</th><th>Jenis</th><th>Jumlah</th></tr></thead>';
    /*end of header*/
    return rc;
}

function generateTable(data) {
    console.log(data);
    var rc = '<table border="0" class="table table-bordered">' + generateHeaderTable();
    rc = rc + '<tbody><tr><td>1</td><td>2</td><td>3</td><td>4</td><td>5</td><td>6</td><td>7</td><td>8</td><td>9</td></tr>';

    if (data.IncomeList) {
        var rowIndex = 1;
        var rowCount = data.IncomeList.length;
        rc = rc +
        /*First Record*/
        '<tr><td>' + rowIndex + '</td>' +
        '<td rowspan="' + rowCount + '">' + '<input name="BeginingBalance" type="hidden" value = "' + data.BeginingBalance + '" />' + +(data.BeginingBalance < 0 ? '-' : '') + ThausandSeperator(data.BeginingBalance, 2) + '</td>' +
        '<td><input name="IncomeList[0].RegistrationNumber" type="hidden" value = "' + data.IncomeList[0].RegistrationNumber + '" />' +
            '<input name="IncomeList[0].StringRegistrationDate" type="hidden" value = "' + data.IncomeList[0].StringRegistrationDate + '" />' +
            '<input name="IncomeList[0].Ck5Id" type="hidden" value = "' + data.IncomeList[0].Ck5Id + '" />' +
            '<input name="IncomeList[0].Lack1Id" type="hidden" value = "' + data.IncomeList[0].Lack1Id + '" />'
            + '<input name="IncomeList[0].RegistrationDate" type="hidden" value = "' + data.IncomeList[0].RegistrationDate + '" />'
            + data.IncomeList[0].RegistrationNumber + ' - '
            + data.IncomeList[0].StringRegistrationDate + '</td>' +
        '<td>' +
            '<input name="IncomeList[0].Amount" type="hidden" value = "' + data.IncomeList[0].Amount + '" />'
            + ThausandSeperator(data.IncomeList[0].Amount, 2) + '</td>' +
        '<td rowspan="' + rowCount + '">' + '<input name="TotalUsage" type="hidden" value = "' + parseFloat(data.TotalUsage) + '" />' + (data.TotalUsage < 0 ? '-' : '') + ThausandSeperator(data.TotalUsage, 2) + '</td>' +
        '<td rowspan="' + rowCount + '">' + generateJenisHasilProduksi(data.ProductionList) + '</td>' +
        '<td rowspan="' + rowCount + '">' + generateJumlahHasilProduksi(data.ProductionList) + '</td>' +
        '<td rowspan="' + rowCount + '">' + (data.EndingBalance < 0 ? '-' : '') + ThausandSeperator(data.EndingBalance, 2) + '</td>' +
        '<td rowspan="' + rowCount + '">' + (data.Noted ? data.Noted : '') + '</td></tr>';
        /*loop record*/
        for (var i = 1; i < data.IncomeList.length; i++) {
            rowIndex = rowIndex + 1;
            var item = '<tr><td>' +
                '<input name="IncomeList[' + i + '].Lack1Id" type="hidden" value = "' + data.IncomeList[i].Lack1Id + '" />' +
                '<input name="IncomeList[' + i + '].Ck5Id" type="hidden" value = "' + data.IncomeList[i].Ck5Id + '" />' +
                '<input name="IncomeList[' + i + '].RegistrationDate" type="hidden" value = "' + data.IncomeList[i].RegistrationDate + '" />'
                + '<input name="IncomeList[' + i + '].RegistrationNumber" type="hidden" value = "' + data.IncomeList[i].RegistrationNumber + '" />'
                + rowIndex + '</td><td>' + data.IncomeList[i].RegistrationNumber + ' - ' + data.IncomeList[i].StringRegistrationDate + '</td>' +
                        '<td>' + '<input name="IncomeList[' + i + '].Amount" type="hidden" value = "' + data.IncomeList[i].Amount + '" />' + data.IncomeList[i].Amount + '</td></tr>';
            rc.append(item);
        }
        /*end loop record*/
        
        $('#IncomeListCount').val(rowCount);
    } else {
        rc = rc +
        /*Only 1 Record*/
        '<tr><td>1</td>' +
        '<td>' + (data.BeginingBalance < 0 ? '-' : '') + ThausandSeperator(data.BeginingBalance, 2) + '</td>' +
        '<td></td>' +
        '<td></td>' +
        '<td></td>' +
        '<td>' + generateJenisHasilProduksi(data.ProductionList) + '</td>' +
        '<td>' + generateJumlahHasilProduksi(data.ProductionList) + '</td>' +
        '<td>' + (data.EndingBalance < 0 ? '-' : '') + ThausandSeperator(data.EndingBalance, 2) + '</td>' +
        '<td>' + (data.Noted ? data.Noted : '') + '</td></tr>';
        $('#IncomeListCount').val(0);
    }
    /*footer*/
    rc = rc + '<tr><td></td><td></td><td></td><td>Total : ' + '<input name="TotalIncome" type="hidden" value = "' + data.TotalIncome + '" />'
        + ((data.TotalIncome == 0) ? '-' : (data.TotalIncome < 0 ? '-' : '') + ThausandSeperator(data.TotalIncome, 2)) + '</td><td></td><td></td><td>' + generateSummaryJumlahProduksi(data.SummaryProductionList) + '</td><td colspan="2"></td></tr>' +
        '</tbody></table>';
    
    rc = rc + generatePlant(data.Lack1Plant);
    rc = rc + generatePbck1Mapping(data.Lack1Pbck1Mapping);

    return rc;
}

function generateJenisHasilProduksi(data) {
    var rc = "";
    if (data) {
        rc = '<div id="jumlah-hasil-produksi-data">';
        for (var i = 0; i < data.length; i++) {
            rc += '<input name="ProductionList[' + i + '].Lack1ProductionDetailId" type="hidden" value = "' + data[i].Lack1ProductionDetailId + '" />';
            rc += '<input name="ProductionList[' + i + '].Lack1Id" type="hidden" value = "' + data[i].Lack1Id + '" />';
            rc += '<input name="ProductionList[' + i + '].ProdCode" type="hidden" value = "' + data[i].ProdCode + '" />';
            rc += '<input name="ProductionList[' + i + '].ProductType" type="hidden" value = "' + data[i].ProductType + '" />';
            rc += '<input name="ProductionList[' + i + '].ProductAlias" type="hidden" value = "' + data[i].ProductAlias + '" />';
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
    if (data) {
        for (var i = 0; i < data.length; i++) {
            rc += '<input name="ProductionList[' + i + '].Amount" type="hidden" value = "' + data[i].Amount + '" />';
            rc += '<input name="ProductionList[' + i + '].UomId" type="hidden" value = "' + data[i].UomId + '" />';
            rc += '<input name="ProductionList[' + i + '].UomDesc" type="hidden" value = "' + data[i].UomDesc + '" />';
            if (i == data.length) {
                rc = rc + (data[i].Amount < 0 ? '-' : '') + ThausandSeperator(data[i].Amount, 2);
            } else {
                rc = rc + (data[i].Amount < 0 ? '-' : '') + ThausandSeperator(data[i].Amount, 2) + " " + data[i].UomDesc + "<br/>";
            }
        }
    } else {
        rc = "-";
    }
    return rc;
}

function generateSummaryJumlahProduksi(data) {
    var loopData = '-';
    if (data) {
        var tb = '<table border="0">';
        for (var i = 0; i < data.length; i++) {
            var item = '<tr><td>' + (data[i].Amount < 0 ? '-' : '') + data[i].Amount + '</td><td>' + data[i].UomDesc + '</td></tr>';
            tb = tb + item;
        }
        tb = tb + '</table>';
        loopData = tb;
    }
    var rc = '<table border="0"><tr><td>Total :</td><td>' + loopData + '</td></tr></table>';
    return rc;
}

function generatePlant(data) {
    var rc = "";
    if (data) {
        rc = '<div id="lack1-plant">';
        for (var i = 0; i < data.length; i++) {
            rc += '<input name="Lack1Plant[' + i + '].Lack1PlantId" type="hidden" value = "' + data[i].Lack1PlantId + '" />';
            rc += '<input name="Lack1Plant[' + i + '].Lack1Id" type="hidden" value = "' + data[i].Lack1Id + '" />';
            rc += '<input name="Lack1Plant[' + i + '].Werks" type="hidden" value = "' + data[i].Werks + '" />';
            rc += '<input name="Lack1Plant[' + i + '].Name1" type="hidden" value = "' + data[i].Name1 + '" />';
            rc += '<input name="Lack1Plant[' + i + '].Address" type="hidden" value = "' + data[i].Address + '" />';
        }
        rc = rc + '</div>';
    } else {
        rc = "-";
    }
    return rc;
}

function generatePbck1Mapping(data) {
    var rc = "";
    if (data) {
        rc = '<div id="lack1-pbck1-mapping">';
        for (var i = 0; i < data.length; i++) {
            rc += '<input name="Lack1Pbck1Mapping[' + i + '].LACK1_PBCK1_MAPPING_ID" type="hidden" value = "' + data[i].LACK1_PBCK1_MAPPING_ID + '" />';
            rc += '<input name="Lack1Pbck1Mapping[' + i + '].LACK1_ID" type="hidden" value = "' + data[i].LACK1_ID + '" />';
            rc += '<input name="Lack1Pbck1Mapping[' + i + '].PBCK1_ID" type="hidden" value = "' + data[i].PBCK1_ID + '" />';
            rc += '<input name="Lack1Pbck1Mapping[' + i + '].PBCK1_NUMBER" type="hidden" value = "' + data[i].PBCK1_NUMBER + '" />';
            rc += '<input name="Lack1Pbck1Mapping[' + i + '].DECREE_DATE" type="hidden" value = "' + data[i].DECREE_DATE + '" />';
            rc += '<input name="Lack1Pbck1Mapping[' + i + '].DisplayDecreeDate" type="hidden" value = "' + data[i].DisplayDecreeDate + '" />';
        }
        rc = rc + '</div>';
    } else {
        rc = "-";
    }
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
