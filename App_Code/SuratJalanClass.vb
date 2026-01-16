Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class SuratJalanClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Public Function GetDataRow(thisString As String) As DataRow
        Using thisConn As New SqlConnection(myConn)
            Using thisCmd As New SqlCommand(thisString, thisConn)
                Using thisAdapter As New SqlDataAdapter(thisCmd)
                    Using dt As New DataTable()
                        thisAdapter.Fill(dt)
                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        End Using
    End Function

    Public Function GetDataTable(thisString As String) As DataTable
        Using thisConn As New SqlConnection(myConn)
            Using thisCmd As New SqlCommand(thisString, thisConn)
                Using da As New SqlDataAdapter(thisCmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        End Using
    End Function

    Public Function GetListData(thisString As String) As DataSet
        Dim thisCmd As New SqlCommand(thisString)
        Using thisConn As New SqlConnection(myConn)
            Using thisAdapter As New SqlDataAdapter()
                thisCmd.Connection = thisConn
                thisAdapter.SelectCommand = thisCmd
                Using thisDataSet As New DataSet()
                    thisAdapter.Fill(thisDataSet)
                    Return thisDataSet
                End Using
            End Using
        End Using
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetItemData_Integer(thisString As String) As Integer
        Dim result As Double = 0
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Private Function CreateCell(text As String, Optional isBold As Boolean = False, Optional alignV As Integer = Element.ALIGN_MIDDLE, Optional alignH As Integer = Element.ALIGN_LEFT, Optional colspan As Integer = 1) As PdfPCell
        Dim style As Integer = If(isBold, Font.BOLD, Font.NORMAL)
        Dim thisFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, style)
        Dim lines As Integer = text.Split({vbLf, vbCrLf}, StringSplitOptions.None).Length
        Dim lineHeight As Single = 13
        Dim calculatedHeight As Single = lines * lineHeight

        Dim cell As New PdfPCell(New Phrase(text, thisFont))
        cell.Border = 0
        cell.HorizontalAlignment = alignH
        cell.VerticalAlignment = alignV
        cell.MinimumHeight = calculatedHeight
        cell.PaddingBottom = 6
        cell.Colspan = colspan

        Return cell
    End Function

    Private Function CreateCellDetail(text As String, Optional isBold As Boolean = False, Optional alignH As Integer = Element.ALIGN_LEFT) As PdfPCell
        Dim style As Integer = If(isBold, Font.BOLD, Font.NORMAL)
        Dim thisFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, style)
        Dim lines As Integer = text.Split({vbLf, vbCrLf}, StringSplitOptions.None).Length
        Dim lineHeight As Single = 16
        Dim calculatedHeight As Single = lines * lineHeight

        Dim cell As New PdfPCell(New Phrase(text, thisFont))
        cell.Border = Rectangle.BOTTOM_BORDER
        cell.BorderWidthBottom = 0.5F
        cell.HorizontalAlignment = alignH
        cell.VerticalAlignment = Element.ALIGN_MIDDLE
        cell.MinimumHeight = calculatedHeight
        cell.PaddingBottom = 6
        Return cell
    End Function

    Public Function GetFabricColourName(fabricColourId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(fabricColourId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("SELECT Name FROM FabricColours WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", fabricColourId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function ConvertMonthToRoman(month As Integer) As String
        Dim romanMonths() As String = {"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII"}
        Return romanMonths(month)
    End Function

    Public Sub BindContent(headerId As String, filePath As String)
        Dim pdfFilePath As String = filePath
        Using fs As New FileStream(pdfFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
            Dim doc As New Document(PageSize.A4, 36, 36, 90, 180)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)

            writer.PageEvent = New SuratJalanEvents()

            doc.Open()

            Dim headerData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")

            Dim orderId As String = headerData("OrderId").ToString()
            Dim customerId As String = headerData("CustomerId").ToString()
            Dim customerName As String = headerData("CustomerName").ToString()

            Dim fullAddress As String = String.Empty
            Dim customerAddress As DataRow = GetDataRow("SELECT * FROM CustomerAddress WHERE CustomerId='" & customerId & "' AND [Primary]=1")
            If Not customerAddress IsNot Nothing Then
                Dim address As String = customerAddress("Address").ToString()
                Dim suburb As String = customerAddress("Suburb").ToString()
                Dim state As String = customerAddress("State").ToString()
                Dim postCode As String = customerAddress("PostCode").ToString()
                Dim country As String = customerAddress("Country").ToString()

                fullAddress = address
                fullAddress &= vbCrLf
                fullAddress &= String.Format("{0}, {1}, {2}", suburb, state, postCode)
                fullAddress &= vbCrLf
                fullAddress &= country
            End If

            Dim rnd As New Random()
            Dim randomCode As String = rnd.Next(10000, 99999).ToString()
            Dim monthRoman As String = ConvertMonthToRoman(Date.Now.Month)
            Dim yearOnly As String = Date.Now.Year.ToString()

            Dim todayString As String = Date.Now.ToString("dd MMMM yyyy")

            Dim nomorSJ As String = String.Format("No: SJ/{0}/{1}/{2}/{3}", orderId, randomCode, monthRoman, yearOnly)
            Dim tanggalSJ As String = String.Format("Tanggal: {0}", todayString)

            Dim titleTable As New PdfPTable(1)
            titleTable.WidthPercentage = 100
            titleTable.DefaultCell.Border = Rectangle.NO_BORDER

            titleTable.AddCell(CreateCell("SURAT JALAN", isBold:=True, alignH:=Element.ALIGN_CENTER))
            titleTable.AddCell(CreateCell(nomorSJ, isBold:=True, alignH:=Element.ALIGN_CENTER))
            titleTable.AddCell(CreateCell(tanggalSJ, isBold:=True, alignH:=Element.ALIGN_CENTER))
            doc.Add(titleTable)

            Dim emptyLine As New Paragraph(" ", New Font(Font.FontFamily.TIMES_ROMAN, 10))
            emptyLine.SpacingBefore = 1
            doc.Add(emptyLine)
            doc.Add(emptyLine)

            Dim companyTable As New PdfPTable(2)
            companyTable.WidthPercentage = 100
            companyTable.SetWidths(New Single() {0.5F, 0.5F})
            companyTable.DefaultCell.Border = Rectangle.NO_BORDER

            Dim leftTable As New PdfPTable(3)
            leftTable.WidthPercentage = 100
            leftTable.SetWidths({0.17F, 0.03F, 0.8F})
            leftTable.DefaultCell.Border = Rectangle.NO_BORDER
            leftTable.AddCell(CreateCell("Data Pengirim", alignV:=Element.ALIGN_TOP, isBold:=True, colspan:=3))

            leftTable.AddCell(CreateCell("Nama", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell("PT Bumi Indah Global", alignV:=Element.ALIGN_TOP))

            Dim alamatPengirim As String = "Jl. Cikande-Rangkas Bitung KM 4.5"
            alamatPengirim &= vbCrLf
            alamatPengirim &= "Kareo, Serang, Banten 42177"

            leftTable.AddCell(CreateCell("Alamat", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(alamatPengirim, alignV:=Element.ALIGN_TOP))

            Dim leftCell As New PdfPCell(leftTable)
            leftCell.Border = Rectangle.NO_BORDER
            companyTable.AddCell(leftCell)

            ' RIGHT SIDE
            Dim rightTable As New PdfPTable(3)
            rightTable.WidthPercentage = 100
            rightTable.SetWidths({0.17F, 0.03F, 0.8F})
            rightTable.DefaultCell.Border = Rectangle.NO_BORDER
            rightTable.AddCell(CreateCell("Data Penerima", alignV:=Element.ALIGN_TOP, isBold:=True, colspan:=3))

            rightTable.AddCell(CreateCell("Nama", alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(customerName, alignV:=Element.ALIGN_TOP))

            rightTable.AddCell(CreateCell("Alamat", alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(fullAddress, alignV:=Element.ALIGN_TOP))

            rightTable.AddCell(CreateCell("Order #", alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(orderId, alignV:=Element.ALIGN_TOP))

            Dim rightCell As New PdfPCell(rightTable)
            rightCell.Border = Rectangle.NO_BORDER
            companyTable.AddCell(rightCell)

            doc.Add(companyTable)

            doc.Add(New Paragraph(" "))
            Dim barangTitle As New Paragraph("Detail Barang :", New Font(Font.FontFamily.HELVETICA, 11, Font.BOLD))
            barangTitle.SpacingBefore = 5
            barangTitle.SpacingAfter = 5
            doc.Add(barangTitle)
            doc.Add(emptyLine)

            Dim table As New PdfPTable(3)
            table.WidthPercentage = 100
            table.SetWidths(New Single() {0.1F, 0.25F, 0.65F})

            table.AddCell(CreateCellDetail("No", True))
            table.AddCell(CreateCellDetail("Ruangan / Lokasi", isBold:=True))
            table.AddCell(CreateCellDetail("Deskripsi", isBold:=True))

            Dim detailData As DataTable = GetDataTable("SELECT OrderDetails.*, Products.Name AS ProductName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND Products.DesignId<>'16' ORDER BY CASE WHEN Designs.Type='Blinds' OR Designs.Type='Shutters' THEN 1 ELSE 2 END, OrderDetails.Id ASC")
            If detailData.Rows.Count > 0 Then
                For i As Integer = 0 To detailData.Rows.Count - 1
                    Dim itemId As String = detailData.Rows(i)("Id").ToString()
                    Dim room As String = detailData.Rows(i)("Room").ToString()

                    Dim productId As String = detailData.Rows(i)("ProductId").ToString()

                    Dim fabricColourId As String = detailData.Rows(i)("FabricColourId").ToString()
                    Dim fabricColourIdB As String = detailData.Rows(i)("FabricColourIdB").ToString()
                    Dim fabricColourIdC As String = detailData.Rows(i)("FabricColourIdC").ToString()
                    Dim fabricColourIdD As String = detailData.Rows(i)("FabricColourIdD").ToString()
                    Dim fabricColourIdE As String = detailData.Rows(i)("FabricColourIdE").ToString()
                    Dim fabricColourIdF As String = detailData.Rows(i)("FabricColourIdF").ToString()

                    Dim width As String = detailData.Rows(i)("Width").ToString()
                    Dim widthB As String = detailData.Rows(i)("WidthB").ToString()
                    Dim widthC As String = detailData.Rows(i)("WidthC").ToString()
                    Dim widthD As String = detailData.Rows(i)("WidthD").ToString()

                    Dim drop As String = detailData.Rows(i)("Drop").ToString()
                    Dim dropB As String = detailData.Rows(i)("DropB").ToString()
                    Dim dropC As String = detailData.Rows(i)("DropC").ToString()
                    Dim dropD As String = detailData.Rows(i)("DropD").ToString()

                    Dim printing As String = detailData.Rows(i)("Printing").ToString()
                    Dim printingB As String = detailData.Rows(i)("PrintingB").ToString()
                    Dim printingC As String = detailData.Rows(i)("PrintingC").ToString()
                    Dim printingD As String = detailData.Rows(i)("PrintingD").ToString()
                    Dim printingE As String = detailData.Rows(i)("PrintingE").ToString()
                    Dim printingF As String = detailData.Rows(i)("PrintingF").ToString()

                    Dim totalItem As String = detailData.Rows(i)("TotalItems").ToString()

                    Dim size As String = String.Format("({0}x{1})", width, drop)
                    Dim sizeB As String = String.Format("({0}x{1})", widthB, dropB)
                    Dim sizeC As String = String.Format("({0}x{1})", widthC, dropC)
                    Dim sizeD As String = String.Format("({0}x{1})", widthD, dropD)

                    Dim linearMetre As Decimal = 0D
                    Dim linearMetreB As Decimal = 0D
                    Dim linearMetreC As Decimal = 0D
                    Dim linearMetreD As Decimal = 0D

                    If Not IsDBNull(detailData.Rows(i)("LinearMetre")) Then
                        linearMetre = Math.Round(Convert.ToDecimal(detailData.Rows(i)("LinearMetre")), 2)
                    End If
                    If Not IsDBNull(detailData.Rows(i)("LinearMetreB")) Then
                        linearMetreB = Math.Round(Convert.ToDecimal(detailData.Rows(i)("LinearMetreB")), 2)
                    End If
                    If Not IsDBNull(detailData.Rows(i)("LinearMetreC")) Then
                        linearMetreC = Math.Round(Convert.ToDecimal(detailData.Rows(i)("LinearMetreC")), 2)
                    End If
                    If Not IsDBNull(detailData.Rows(i)("LinearMetreD")) Then
                        linearMetreD = Math.Round(Convert.ToDecimal(detailData.Rows(i)("LinearMetreD")), 2)
                    End If

                    Dim squareMetre As Decimal = 0D
                    Dim squareMetreB As Decimal = 0D
                    Dim squareMetreC As Decimal = 0D
                    Dim squareMetreD As Decimal = 0D

                    If Not IsDBNull(detailData.Rows(i)("SquareMetre")) Then
                        squareMetre = Math.Round(Convert.ToDecimal(detailData.Rows(i)("SquareMetre")), 2)
                    End If
                    If Not IsDBNull(detailData.Rows(i)("SquareMetreB")) Then
                        squareMetreB = Math.Round(Convert.ToDecimal(detailData.Rows(i)("SquareMetreB")), 2)
                    End If
                    If Not IsDBNull(detailData.Rows(i)("SquareMetreC")) Then
                        squareMetreC = Math.Round(Convert.ToDecimal(detailData.Rows(i)("SquareMetreC")), 2)
                    End If
                    If Not IsDBNull(detailData.Rows(i)("SquareMetreD")) Then
                        squareMetreD = Math.Round(Convert.ToDecimal(detailData.Rows(i)("SquareMetreD")), 2)
                    End If

                    Dim linearMetreText As String = String.Format("{0}lm", linearMetre.ToString("0.##", enUS))
                    Dim linearMetreTextB As String = String.Format("{0}lm", linearMetreB.ToString("0.##", enUS))
                    Dim linearMetreTextC As String = String.Format("{0}lm", linearMetreC.ToString("0.##", enUS))
                    Dim linearMetreTextD As String = String.Format("{0}lm", linearMetreD.ToString("0.##", enUS))

                    Dim squareMetreText As String = String.Format("{0}sqm", squareMetre.ToString("0.##", enUS))
                    Dim squareMetreTextB As String = String.Format("{0}sqm", squareMetreB.ToString("0.##", enUS))
                    Dim squareMetreTextC As String = String.Format("{0}sqm", squareMetreC.ToString("0.##", enUS))
                    Dim squareMetreTextD As String = String.Format("{0}sqm", squareMetreD.ToString("0.##", enUS))

                    Dim productName As String = detailData.Rows(i)("ProductName").ToString()
                    Dim itemDescription As String = productName

                    Dim designId As String = GetItemData("SELECT DesignId FROM Products WHERE Id='" & productId & "'")
                    Dim blindId As String = GetItemData("SELECT BlindId FROM Products WHERE Id='" & productId & "'")

                    Dim designName As String = GetItemData("SELECT Name FROM Designs WHERE Id='" & designId & "'")
                    Dim blindName As String = GetItemData("SELECT Name FROM Blinds WHERE Id='" & blindId & "'")

                    If designName = "Aluminium Blind" Then
                        itemDescription = String.Format("{0} {1} {2}", productName, size, squareMetreText)
                        If totalItem = 2 Then
                            itemDescription = "2 on 1 Headrail"
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("{0} {1} {2}", productName, size, squareMetreText)
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("{0} {1} {2}", productName, sizeB, squareMetreTextB)
                        End If
                    End If

                    If designName = "Cellular Shades" Then
                        Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                        Dim fabricColourNameB As String = GetFabricColourName(fabricColourIdB)

                        itemDescription = String.Format("{0} {1} {2} {3}", productName, fabricColourName, size, squareMetreText)
                        If blindName = "Day & Night" Then
                            itemDescription = productName
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("{0} {1} {2}", fabricColourName, size, squareMetreText)
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("{0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                        End If
                    End If

                    If designName = "Design Shades" Then
                        Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                        itemDescription = String.Format("{0} {1} {2} {3}", productName, fabricColourName, size, squareMetreText)
                    End If

                    If designName = "Linea Valance" Then
                        itemDescription = String.Format("{0} ({1}mm) {2}", productName, width, linearMetreText)
                    End If

                    If designName = "Panel Glide" Then
                        Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                        itemDescription = String.Format("{0} {1} {2} {3}", productName, fabricColourName, size, squareMetreText)
                        If blindName = "Track Only" Then
                            itemDescription = String.Format("{0} ({1}mm) {2}", productName, width, linearMetreText)
                        End If
                    End If

                    If designName = "Privacy Venetian" Then
                        itemDescription = String.Format("{0} {1} {2}", productName, size, squareMetreText)
                    End If

                    If designName = "Pelmet" Then
                        Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                        itemDescription = String.Format("{0} {1} {2} {3}", productName, fabricColourName, size, squareMetreText)
                    End If

                    If designName = "Roller Blind" Then
                        Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                        Dim fabricColourNameB As String = GetFabricColourName(fabricColourIdB)
                        Dim fabricColourNameC As String = GetFabricColourName(fabricColourIdC)
                        Dim fabricColourNameD As String = GetFabricColourName(fabricColourIdD)
                        Dim fabricColourNameE As String = GetFabricColourName(fabricColourIdE)
                        Dim fabricColourNameF As String = GetFabricColourName(fabricColourIdF)

                        itemDescription = String.Format("{0} {1} {2} {3}", productName, fabricColourName, size, squareMetreText)
                        If Not String.IsNullOrEmpty(printing) Then
                            itemDescription &= vbCrLf
                            itemDescription &= "Printed Fabric"
                        End If
                        If blindName = "Dual Blinds" OrElse blindName = "Link 2 Blinds Dependent" OrElse blindName = "Link 2 Blinds Independent" Then
                            itemDescription = productName
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("First Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                            If Not String.IsNullOrEmpty(printing) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                            If Not String.IsNullOrEmpty(printingB) Then
                                itemDescription &= " Printed Fabric"
                            End If
                        End If

                        If blindName = "Link 3 Blinds Dependent" OrElse blindName = "Link 3 Blinds Independent with Dependent" Then
                            itemDescription = productName
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("First Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                            If Not String.IsNullOrEmpty(printing) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                            If Not String.IsNullOrEmpty(printingB) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Third Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                            If Not String.IsNullOrEmpty(printingC) Then
                                itemDescription &= " Printed Fabric"
                            End If
                        End If

                        If blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 2 Blinds Independent" Then
                            itemDescription = productName
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("First Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                            If Not String.IsNullOrEmpty(printing) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                            If Not String.IsNullOrEmpty(printingB) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Third Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                            If Not String.IsNullOrEmpty(printingC) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Fourth Blind : {0} {1} {2}", fabricColourNameD, size, squareMetreText)
                            If Not String.IsNullOrEmpty(printingD) Then
                                itemDescription &= " Printed Fabric"
                            End If
                        End If

                        If blindName = "DB Link 3 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then
                            itemDescription = productName
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("First Blind : {0}", fabricColourName)
                            If Not String.IsNullOrEmpty(printing) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Second Blind : {0}", fabricColourNameB)
                            If Not String.IsNullOrEmpty(printingB) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Third Blind : {0}", fabricColourNameC)
                            If Not String.IsNullOrEmpty(printingC) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Fourth Blind : {0}", fabricColourNameD)
                            If Not String.IsNullOrEmpty(printingD) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Fifth Blind : {0}", fabricColourNameE)
                            If Not String.IsNullOrEmpty(printingE) Then
                                itemDescription &= " Printed Fabric"
                            End If
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("Sixth Blind : {0}", fabricColourNameF)
                            If Not String.IsNullOrEmpty(printingF) Then
                                itemDescription &= " Printed Fabric"
                            End If
                        End If
                    End If

                    If designName = "Roman Blind" Then
                        Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                        itemDescription = String.Format("{0} {1} {2} {3}", productName, fabricColourName, size, squareMetreText)
                    End If

                    If designName = "Venetian Blind" Then
                        itemDescription = String.Format("{0} {1} {2}", productName, size, squareMetreText)
                        If totalItem = 2 Then
                            itemDescription = "2 on 1 Headrail"
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("{0} {1} {2}", productName, size, squareMetreText)
                            itemDescription &= vbCrLf
                            itemDescription &= String.Format("{0} {1} {2}", productName, sizeB, squareMetreTextB)
                        End If
                    End If

                    If designName = "Vertical" Then
                        Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                        itemDescription = String.Format("{0} {1} {2} {3}", productName, fabricColourName, size, squareMetreText)
                        If blindName = "Track Only" Then
                            itemDescription = String.Format("{0} ({1}mm) {2}", productName, width, linearMetreText)
                        End If
                    End If

                    If designName = "Saphora Drape" Then
                        Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                        itemDescription = String.Format("{0} {1} {2} {3}", productName, fabricColourName, size, squareMetreText)
                    End If

                    If designName = "Skyline Shutter Express" OrElse designName = "Skyline Shutter Ocean" Then
                        itemDescription = String.Format("{0} {1} {2}", productName, size, squareMetreText)
                    End If

                    table.AddCell(CreateCellDetail(i + 1))
                    table.AddCell(CreateCellDetail(room))
                    table.AddCell(CreateCellDetail(itemDescription))
                Next
            Else
                table.AddCell(CreateCellDetail(1))
                table.AddCell(CreateCellDetail(String.Empty))
                table.AddCell(CreateCellDetail("DATA TIDAK DITEMUKAN !"))
            End If

            doc.Add(table)

            doc.Add(emptyLine)
            doc.Add(emptyLine)
            doc.Add(emptyLine)

            Dim signatureTable As New PdfPTable(2)
            signatureTable.WidthPercentage = 100
            signatureTable.SetWidths(New Single() {0.5F, 0.5F})
            signatureTable.DefaultCell.Border = Rectangle.NO_BORDER

            Dim leftSignature As New PdfPTable(1)
            leftSignature.WidthPercentage = 100
            leftSignature.DefaultCell.Border = Rectangle.NO_BORDER
            leftSignature.AddCell(CreateCell("Pengirim", alignH:=Element.ALIGN_CENTER, alignV:=Element.ALIGN_TOP, isBold:=True))
            leftSignature.AddCell(CreateCell(vbCrLf & vbCrLf & vbCrLf, alignH:=Element.ALIGN_CENTER))
            leftSignature.AddCell(CreateCell("( ............................................ )", alignH:=Element.ALIGN_CENTER, alignV:=Element.ALIGN_TOP, isBold:=True))

            Dim leftSignatureCell As New PdfPCell(leftSignature)
            leftSignatureCell.Border = Rectangle.NO_BORDER
            signatureTable.AddCell(leftSignatureCell)

            Dim rightSignature As New PdfPTable(1)
            rightSignature.WidthPercentage = 100
            rightSignature.DefaultCell.Border = Rectangle.NO_BORDER
            rightSignature.AddCell(CreateCell("Penerima", alignH:=Element.ALIGN_CENTER, alignV:=Element.ALIGN_TOP, isBold:=True))
            rightSignature.AddCell(CreateCell(vbCrLf & vbCrLf & vbCrLf, alignH:=Element.ALIGN_CENTER))
            rightSignature.AddCell(CreateCell("( ............................................ )", alignH:=Element.ALIGN_CENTER, alignV:=Element.ALIGN_TOP, isBold:=True))

            Dim rightSignatureCell As New PdfPCell(rightSignature)
            rightSignatureCell.Border = Rectangle.NO_BORDER
            signatureTable.AddCell(rightSignatureCell)

            doc.Add(signatureTable)

            doc.Close()
        End Using
    End Sub
End Class

Public Class SuratJalanEvents
    Inherits PdfPageEventHelper

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent

        Dim headerTable As New PdfPTable(1)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True

        Dim imageUrl As String = HttpContext.Current.Server.MapPath("~/assets/images/logo/big.jpg")
        Dim img As Image = Image.GetInstance(imageUrl)

        img.ScaleAbsolute(190, 45)

        Dim imgCell As New PdfPCell(img)
        imgCell.Border = 0
        imgCell.HorizontalAlignment = Element.ALIGN_CENTER
        imgCell.VerticalAlignment = Element.ALIGN_MIDDLE
        imgCell.PaddingBottom = 2
        headerTable.AddCell(imgCell)

        Dim phrase As New Paragraph()
        phrase.Alignment = Element.ALIGN_RIGHT
        phrase.Add(Chunk.NEWLINE)

        Dim headerCell As New PdfPCell(phrase)
        headerCell.Border = 0
        headerCell.HorizontalAlignment = Element.ALIGN_RIGHT
        headerCell.VerticalAlignment = Element.ALIGN_TOP
        headerCell.PaddingTop = 0
        headerTable.AddCell(headerCell)

        Dim headerTopY As Single = document.PageSize.Height - 10
        headerTable.WriteSelectedRows(0, -1, 36, headerTopY, cb)

        Dim headerHeight As Single = headerTable.TotalHeight

        Dim lineTable As New PdfPTable(1)
        lineTable.TotalWidth = document.PageSize.Width - 50
        lineTable.LockedWidth = True

        Dim line As New draw.LineSeparator(0.5F, 100.0F, BaseColor.BLACK, Element.ALIGN_CENTER, -1)
        Dim lineChunk As New Chunk(line)
        Dim linePhrase As New Phrase(lineChunk)

        Dim lineCell As New PdfPCell(linePhrase)
        lineCell.Border = 0
        lineCell.PaddingTop = 0
        lineCell.PaddingBottom = 0
        lineTable.AddCell(lineCell)

        Dim headerBottomY As Single = headerTopY - headerHeight + 5
        lineTable.WriteSelectedRows(0, -1, 36, headerBottomY, cb)

        Dim lineBeforeTermsTable As New PdfPTable(1)
        lineBeforeTermsTable.TotalWidth = document.PageSize.Width - 72
        lineBeforeTermsTable.LockedWidth = True

        Dim hr As New draw.LineSeparator(1.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_CENTER, -1)
        Dim hrChunk As New Chunk(hr)
        Dim hrPhrase As New Phrase(hrChunk)

        Dim hrCell As New PdfPCell(hrPhrase)
        hrCell.Border = 0
        hrCell.PaddingTop = 0
        hrCell.PaddingBottom = 0
        lineBeforeTermsTable.AddCell(hrCell)

        lineBeforeTermsTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(90), cb)

        Dim termsTable As New PdfPTable(1)
        termsTable.TotalWidth = document.PageSize.Width - 72
        termsTable.LockedWidth = True

        Dim termsText As String = ""

        Dim termsPhrase As New Phrase()
        termsPhrase.Add(New Chunk("Catatan : " & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        termsPhrase.Add(New Chunk(vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10)))
        termsPhrase.Add(New Chunk(termsText & vbCrLf & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10)))

        Dim termsCell As New PdfPCell(termsPhrase)
        termsCell.Border = 0
        termsCell.PaddingTop = 0
        termsCell.PaddingBottom = 0
        termsTable.AddCell(termsCell)

        termsTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(65), cb)
    End Sub
End Class