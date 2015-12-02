function generateHeaderTable() {
    var rc =  /*start header*/
        '<thead><tr><th rowspan="2">No<br>Urut</th><th rowspan="2">Saldo<br>Awal</th><th colspan="2">Pemasukan</th><th rowspan="2">Penggunaan</th><th colspan="2">Hasil Produksi BKC</th><th rowspan="2">Saldo<br>Akhir</th><th rowspan="2">Keterangan</th></tr><tr><th>No. & Tgl. CK-5</th><th>Jumlah</th><th>Jenis</th><th>Jumlah</th></tr></thead>';
    /*end of header*/
    return rc;
}

function generateTable(data) {
    console.log('Only Tis To Fa');
    var rc = '<table border="0" class="table table-bordered">' + generateHeaderTable();
    rc = rc + '<tbody><tr><td>1</td><td>2</td><td>3</td><td>4</td><td>5</td><td>6</td><td>7</td><td>8</td><td>9</td></tr>';

    if (data.IncomeList != null && data.IncomeList.length > 0) {
        if (data.IncomeList) {
            var rowIndex = 1;
            var rowCount = data.IncomeList.length;
            rc = rc +
            /*First Record*/
            '<tr><td>' + rowIndex + '</td>' +
            '<td rowspan="' + rowCount + '">' + (data.BeginingBalance < 0 ? '-' : '') + ThausandSeperator(data.BeginingBalance, 2) + '</td>' +
            '<td>' + data.IncomeList[0].RegistrationNumber + ' - '
                + data.IncomeList[0].StringRegistrationDate + '</td>' +
            '<td>' +
               ThausandSeperator(data.IncomeList[0].Amount, 2) + '</td>' +
            '<td rowspan="' + rowCount + '">' + (data.TotalUsage < 0 ? '-' : '') + ThausandSeperator(data.TotalUsage, 2) + '</td>' +
            '<td rowspan="' + rowCount + '">' + generateJenisHasilProduksi(data.InventoryProductionTisToFa.ProductionData) + '</td>' +
            '<td rowspan="' + rowCount + '">' + generateJumlahHasilProduksi(data.InventoryProductionTisToFa.ProductionData) + '</td>' +
            '<td rowspan="' + rowCount + '">' + (data.EndingBalance < 0 ? '-' : '') + ThausandSeperator(data.EndingBalance, 2) + '</td>' +
            '<td rowspan="' + rowCount + '">' + (data.DocumentNoted ? data.DocumentNoted : '') + '</td></tr>';
            /*loop record*/
            for (var i = 1; i < data.IncomeList.length; i++) {
                rowIndex = rowIndex + 1;
                var item = '<tr><td>' + rowIndex + '</td><td>' + data.IncomeList[i].RegistrationNumber + ' - ' + data.IncomeList[i].StringRegistrationDate + '</td>' +
                            '<td>' + ThausandSeperator(data.IncomeList[i].Amount) + '</td></tr>';
                /*rc.append(item);*/
                rc = rc + item;
            }
            /*end loop record*/

            $('#IncomeListCount').val(rowCount);
        } else {
            rc = rc +
            /*Only Tis To Fa Record*/
            '<tr><td>1</td>' +
            '<td>' + (data.BeginingBalance < 0 ? '-' : '') + ThausandSeperator(data.BeginingBalance, 2) + '</td>' +
            '<td></td>' +
            '<td></td>' +
            '<td></td>' +
            '<td>' + generateJenisHasilProduksi(data.InventoryProductionTisToFa.ProductionData) + '</td>' +
            '<td>' + generateJumlahHasilProduksi(data.InventoryProductionTisToFa.ProductionData) + '</td>' +
            '<td>' + (data.EndingBalance < 0 ? '-' : '') + ThausandSeperator(data.EndingBalance, 2) + '</td>' +
            '<td>' + (data.DocumentNoted ? data.DocumentNoted : '') + '</td></tr>';
            
            $('#IncomeListCount').val(0);
        }
    }
    /*footer*/
    rc = rc + '<tr><td></td><td></td><td></td><td>Total : ' + '<input name="TotalIncome" type="hidden" value = "' + data.TotalIncome + '" />'
        + ((data.TotalIncome == 0) ? '-' : (data.TotalIncome < 0 ? '-' : '') + ThausandSeperator(data.TotalIncome, 2)) + '</td><td></td><td></td><td>' + generateSummaryJumlahProduksi(data.FusionSummaryProductionList) + '</td><td colspan="2"></td></tr>' +
        '</tbody></table>';

    //rc = rc + generatePlant(data.Lack1Plant);
    //if (data.Lack1Pbck1Mapping) {
    //    rc = rc + generatePbck1Mapping(data.Lack1Pbck1Mapping);
    //}

    return rc;
}

function generateTableWithTisToTis(data) {
    console.log('With Tis To Tis');
    console.log(data);
    var rc = '<table border="0" class="table table-bordered">' + generateHeaderTable();
    rc = rc + '<tbody><tr><td>1</td><td>2</td><td>3</td><td>4</td><td>5</td><td>6</td><td>7</td><td>8</td><td>9</td></tr>';

    if (data.IncomeList != null && data.IncomeList.length > 0) {
        if (data.IncomeList) {
            var rowCount = data.IncomeList.length;
            /*if only one record for income list from CK5*/
            if (rowCount == 1) {
                rc = rc + generateContentTableLack1WithTisToTisOnlyOneIncomeList(data);
            } else {
                rc = rc + generateContentTableLack1WithTisToTis(data);
            }
        } else {
            
            rc = rc +
            /*Only 1 Record*/
            '<tr><td rowspan="2">1</td>' +
            '<td rowspan="2">' + (data.BeginingBalance < 0 ? '-' : '') + ThausandSeperator(data.BeginingBalance, 2) + '</td>' +
            '<td rowspan="2"></td>' +
            '<td rowspan="2"></td>' +
            '<td rowspan="2"></td>' +
            '<td>' + generateJenisHasilProduksi(data.InventoryProductionTisToFa.ProductionData) + '</td>' +
            '<td>' + generateJumlahHasilProduksi(data.InventoryProductionTisToFa.ProductionData) + '</td>' +
            '<td rowspan="2">' + (data.EndingBalance < 0 ? '-' : '') + ThausandSeperator(data.EndingBalance, 2) + '</td>' +
            '<td rowspan="2">' + (data.DocumentNoted ? data.DocumentNoted : '') + '</td></tr>';
            
            /*Second record*/
            rc = rc + '<tr><td style="border-bottom:none">' + (data.TotalUsageTisToTis < 0 ? '-' : ThausandSeperator(data.TotalUsageTisToTis, 2)) + '</td>' +
                    '<td style="border-top:none">' + generateJenisHasilProduksi(data.InventoryProductionTisToTis.ProductionData) + '</td>' +
                    '<td style="border-top:none">' + generateJumlahHasilProduksi(data.InventoryProductionTisToTis.ProductionData) + '</td></tr>';

            $('#IncomeListCount').val(0);
        }
    }
    /*footer*/
    rc = rc + '<tr><td></td><td></td><td></td><td>Total : ' + '<input name="TotalIncome" type="hidden" value = "' + data.TotalIncome + '" />'
        + ((data.TotalIncome == 0) ? '-' : (data.TotalIncome < 0 ? '-' : '') + ThausandSeperator(data.TotalIncome, 2)) + '</td><td></td><td></td><td>' + generateSummaryJumlahProduksi(data.FusionSummaryProductionList) + '</td><td colspan="2"></td></tr>' +
        '</tbody></table>';
    
    return rc;
}

function generateContentTableLack1WithTisToTisOnlyOneIncomeList(data) {
    var rc = '';
    
    /*First Record*/
    rc = rc +
    '<tr><td rowspan="2">' + rowIndex + '</td>' +
    '<td rowspan="2">' + (data.BeginingBalance < 0 ? '-' : '') + ThausandSeperator(data.BeginingBalance, 2) + '</td>' +
    '<td rowspan="2">' + data.IncomeList[0].RegistrationNumber + ' - '
        + data.IncomeList[0].StringRegistrationDate + '</td>' +
    '<td rowspan="2">' +
       ThausandSeperator(data.IncomeList[0].Amount, 2) + '</td>' +
    '<td style="border-bottom:none">' + (data.TotalUsage < 0 ? '-' : '') + ThausandSeperator(data.TotalUsage, 2) + '</td>' +
    '<td style="border-bottom:none">' + generateJenisHasilProduksi(data.InventoryProductionTisToFa.ProductionData) + '</td>' +
    '<td style="border-bottom:none">' + generateJumlahHasilProduksi(data.InventoryProductionTisToFa.ProductionData) + '</td>' +
    '<td rowspan="2">' + (data.EndingBalance < 0 ? '-' : '') + ThausandSeperator(data.EndingBalance, 2) + '</td>' +
    '<td rowspan="2">' + (data.DocumentNoted ? data.DocumentNoted : '') + '</td></tr>';
    
    /*Second record*/
    rc = rc + '<tr><td style="border-bottom:none">' + (data.TotalUsageTisToTis < 0 ? '-' : ThausandSeperator(data.TotalUsageTisToTis, 2)) + '</td>' +
        '<td style="border-top:none">' + generateJenisHasilProduksi(data.InventoryProductionTisToTis.ProductionData) + '</td>' +
        '<td style="border-top:none">' + generateJumlahHasilProduksi(data.InventoryProductionTisToTis.ProductionData) + '</td></tr>';

    $('#IncomeListCount').val(1);

    return rc;
}

function generateContentTableLack1WithTisToTis(data) {
    var rc = '';
    var rowIndex = 1;
    var rowCount = data.IncomeList.length;
    var rowSpan1 = Math.round(rowCount / 2);
    var rowSpan2 = rowCount - rowSpan1;

    /*First Record*/
    rc = rc +
    '<tr><td>' + rowIndex + '</td>' +
    '<td rowspan="' + rowCount + '">' + (data.BeginingBalance < 0 ? '-' : '') + ThausandSeperator(data.BeginingBalance, 2) + '</td>' +
    '<td>' + data.IncomeList[0].RegistrationNumber + ' - '
        + data.IncomeList[0].StringRegistrationDate + '</td>' +
    '<td>' +
       ThausandSeperator(data.IncomeList[0].Amount, 2) + '</td>' +
    '<td rowspan="' + rowSpan1 + '" style="border-bottom:none">' + (data.TotalUsage < 0 ? '-' : '') + ThausandSeperator(data.TotalUsage, 2) + '</td>' +
    '<td rowspan="' + rowSpan1 + '" style="border-bottom:none">' + generateJenisHasilProduksi(data.InventoryProductionTisToFa.ProductionData) + '</td>' +
    '<td rowspan="' + rowSpan1 + '" style="border-bottom:none">' + generateJumlahHasilProduksi(data.InventoryProductionTisToFa.ProductionData) + '</td>' +
    '<td rowspan="' + rowCount + '">' + (data.EndingBalance < 0 ? '-' : '') + ThausandSeperator(data.EndingBalance, 2) + '</td>' +
    '<td rowspan="' + rowCount + '">' + (data.DocumentNoted ? data.DocumentNoted : '') + '</td></tr>';

    /*loop record first block*/
    for (var j = 1; j < rowSpan1; j++) {
        rowIndex = rowIndex + 1;
        var item1 = '<tr><td>' + rowIndex + '</td><td>' + data.IncomeList[j].RegistrationNumber + ' - ' + data.IncomeList[j].StringRegistrationDate + '</td>' +
                    '<td>' + ThausandSeperator(data.IncomeList[j].Amount) + '</td></tr>';
        /*rc.append(item);*/
        rc = rc + item1;
    }
    /*end loop record*/
    rowIndex = rowIndex + 1;
    rc = rc + '<tr><td>' + rowIndex + '</td><td>' + data.IncomeList[rowSpan1].RegistrationNumber + ' - ' + data.IncomeList[rowSpan1].StringRegistrationDate + '</td>' +
                    '<td>' + ThausandSeperator(data.IncomeList[rowSpan1].Amount) + '</td>' +
                    '<td rowspan="' + rowSpan2 + '" style="border-top:none">' + (data.TotalUsageTisToTis < 0 ? '-' : '') + ThausandSeperator(data.TotalUsageTisToTis, 2) + '</td>' +
                    '<td rowspan="' + rowSpan2 + '" style="border-top:none">' + generateJenisHasilProduksi(data.InventoryProductionTisToTis.ProductionData) + '</td>' +
                    '<td rowspan="' + rowSpan2 + '" style="border-top:none">' + generateJumlahHasilProduksi(data.InventoryProductionTisToTis.ProductionData) + '</td></tr>';

    /*loop record second block*/
    for (var i = rowSpan1 + 1; i < rowCount; i++) {
        rowIndex = rowIndex + 1;
        var item = '<tr><td>' + rowIndex + '</td><td>' + data.IncomeList[i].RegistrationNumber + ' - ' + data.IncomeList[i].StringRegistrationDate + '</td>' +
                    '<td>' + ThausandSeperator(data.IncomeList[i].Amount) + '</td></tr>';
        /*rc.append(item);*/
        rc = rc + item;
    }
    /*end loop record*/
    $('#IncomeListCount').val(rowCount);

    return rc;
}

function generateJenisHasilProduksi(data) {
    var rc = "";
    if (data && data.ProductionSummaryByProdTypeList) {
        rc = '<div id="jumlah-hasil-produksi-data">';
        if (data.ProductionSummaryByProdTypeList.length == 0) {
            //there is no record found
            rc = '-';
        } else {
            for (var i = 0; i < data.ProductionSummaryByProdTypeList.length; i++) {
                if (i == data.ProductionSummaryByProdTypeList.length) {
                    rc = rc + data.ProductionSummaryByProdTypeList[i].ProductAlias;
                } else {
                    rc = rc + data.ProductionSummaryByProdTypeList[i].ProductAlias + "<br/>";
                }
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
    if (data && data.ProductionSummaryByProdTypeList) {
        if (data.ProductionSummaryByProdTypeList.length == 0) {
            //there is no record found
            rc = '-';
        } else {
            for (var i = 0; i < data.ProductionSummaryByProdTypeList.length; i++) {
                if (i == data.ProductionSummaryByProdTypeList.length) {
                    rc = rc + (data.ProductionSummaryByProdTypeList[i].TotalAmount < 0 ? '-' : '') + ThausandSeperator(data.ProductionSummaryByProdTypeList[i].TotalAmount, 3);
                } else {
                    rc = rc + (data.ProductionSummaryByProdTypeList[i].TotalAmount < 0 ? '-' : '') + ThausandSeperator(data.ProductionSummaryByProdTypeList[i].TotalAmount, 3) + " "
                        + data.ProductionSummaryByProdTypeList[i].UomDesc + "<br/>";
                }
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
            var item = '<tr><td>' + (data[i].Amount < 0 ? '-' : '') + ThausandSeperator(data[i].Amount, 3) + '</td><td>' + data[i].UomDesc + '</td></tr>';
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
