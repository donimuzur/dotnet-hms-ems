USE [EMSDMS]
GO

UPDATE [dbo].[PRINTOUT_LAYOUT]
   SET [LAYOUT] = '<div style="font-family: Arial; font-size: 11px;">
<h2 style="text-transform: uppercase; text-align: center;">PERHITUNGAN BESARNYA PENUNDAAN NILAI CUKAI</h2>
<br />
<table style="width: 100%; background-color: white!important; ">
<tbody>
<tr>
<td style="width: 30%;">Nama Pengusaha/Importir</td>
<td style="width: 10px;">:</td>
<td colspan="3">#POA</td>
</tr>
<tr>
<td style="width: 30%;">Nama Perusahaan</td>
<td style="width: 10px;">:</td>
<td colspan="3">#COMPANY_NAME ( NPPBKC #NPPBKC_ID )</td>
<td ></td>
<td ></td>
</tr>
<tr>
<td style="width: 30%;">Alamat Perusahaan</td>
<td style="width: 10px;">:</td>
<td colspan="3">#COMPANY_ADDRESS</td>
</tr>
</tbody>
</table>
<br />
<p>Daftar pemesanan pita cukai dalam 6 (enam) bulan terakhir:</p>
#CK1_DETAIL_TABLE
<p>Pesanan pita cukai rata-rata per bulan:</p>
<br />
<p>1. Dalam 6 (enam) bulan terakhir:<br /> #CK1_AVG_6</p>
<p>2. Dalam 3 (enam) bulan terakhir:<br /> #CK1_AVG_3</p>
<p>Nilai cukai yang dimintakan penundaan:<br /> #PRODUCT_AMOUNT_CALCULATION</p>
<p>Tambahan nilai cukai yang dimintakan penundaan:<br /> #PRODUCT_ADDITIONAL_AMOUNT</p>
<table style="width: 100%; background-color: white!important;">
<tbody>
<tr>
<td style="width: 70%;" colspan="2">Total nilai cukai yang dimintakan penundaan</td>
<td>#TOTAL_AMOUNT</td>
</tr>
</tbody>
</table>
<p>Pemohon,<br />
#POA
</p>
</div>'
 WHERE NAME = 'EXCISE_CREDIT_NEW_REQUEST'
GO





UPDATE [dbo].[PRINTOUT_LAYOUT]
   SET [LAYOUT] = '<div style="font-family: Arial; font-size: 11px;">
<table style="width: 100%; background-color: white!important;">
<tbody>
<tr>
<td style="width: 30%;">Nama Perusahaan</td>
<td style="width: 10px;">:&nbsp;</td>
<td colspan="3">#COMPANY_NAME</td>
</tr>
<tr>
<td style="width: 30%;">NPPBKC</td>
<td style="width: 10px;">:&nbsp;</td>
<td colspan="3">#NPPBKC</td>
</tr>
<tr>
<td style="width: 30%;">Alamat Perusahaan</td>
<td style="width: 10px;">:&nbsp;</td>
<td colspan="3">#COMPANY_ADDRESS</td>
</tr>
</tbody>
</table>
<br />
<h2 style="text-transform: uppercase; text-align: center;">daftar pesanan pita cukai 6 (ENAM) bulan terakhir <br /> periode: #CK1_PERIOD</h2>
<br /> #CK1_DETAIL_TABLE <br />
<p>Pemohon,<br /> #POA</p>
</div>'
 WHERE NAME = 'EXCISE_CREDIT_DETAIL_CALCULATION'
GO