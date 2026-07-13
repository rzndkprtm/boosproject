Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class PackingListClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim idIDR As New CultureInfo("id-ID")

    Public Function GetDataRow(thisString As String) As DataRow
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        thisAdapter.Fill(dt)
                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetDataTable(thisString As String) As DataTable
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim thisTable As New DataTable()
                        thisAdapter.Fill(thisTable)
                        Return thisTable
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetDataTableSP(spName As String, params As List(Of SqlParameter)) As DataTable
        Dim thisTable As New DataTable()
        Using thisConn As New SqlConnection(myConn)
            Using thisCmd As New SqlCommand(spName, thisConn)
                thisCmd.CommandType = CommandType.StoredProcedure
                thisCmd.Parameters.AddRange(params.ToArray())
                Using thisAdapter As New SqlDataAdapter(thisCmd)
                    thisAdapter.Fill(thisTable)
                End Using
            End Using
        End Using
        Return thisTable
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader
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

    Protected Function GetItemData_Decimal(thisString As String) As Decimal
        Dim result As Decimal = 0D
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 0D
        End Try
        Return result
    End Function

    Public Function GetFabricColourName(fabricColourId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(fabricColourId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("SELECT Name FROM FabricColours WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", fabricColourId)
                        thisConn.Open()
                        Dim obj = thisCmd.ExecuteScalar()
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

    Private Function CreateCell(text As String, Optional isBold As Boolean = False, Optional alignV As Integer = Element.ALIGN_MIDDLE) As PdfPCell
        Try
            Dim style As Integer = If(isBold, Font.BOLD, Font.NORMAL)
            Dim thisFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, style)

            If text Is Nothing Then text = String.Empty

            Dim lines As Integer = text.Split({vbLf, vbCrLf}, StringSplitOptions.None).Length
            Dim lineHeight As Single = 13
            Dim calculatedHeight As Single = lines * lineHeight

            Dim cell As New PdfPCell(New Phrase(text, thisFont))
            cell.Border = 0
            cell.HorizontalAlignment = Element.ALIGN_LEFT
            cell.VerticalAlignment = alignV
            cell.MinimumHeight = calculatedHeight
            cell.PaddingBottom = 6

            Return cell
        Catch ex As Exception
            Dim fallbackFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.NORMAL)
            Dim fallbackCell As New PdfPCell(New Phrase("", fallbackFont))
            fallbackCell.Border = 0
            fallbackCell.HorizontalAlignment = Element.ALIGN_LEFT
            fallbackCell.VerticalAlignment = alignV
            fallbackCell.MinimumHeight = 13
            fallbackCell.PaddingBottom = 6

            Return fallbackCell
        End Try
    End Function

    Private Function CreateCellDetail(text As String, Optional isBold As Boolean = False, Optional alignH As Integer = Element.ALIGN_LEFT) As PdfPCell
        Try
            If text Is Nothing Then text = String.Empty

            Dim normalFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, If(isBold, Font.BOLD, Font.NORMAL))
            Dim italicFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.ITALIC)

            Dim paragraph As New Paragraph()

            Dim currentIndex As Integer = 0

            While currentIndex < text.Length
                Dim startItalic As Integer = text.IndexOf("<i>", currentIndex)

                If startItalic = -1 Then
                    paragraph.Add(New Chunk(text.Substring(currentIndex), normalFont))
                    Exit While
                End If

                If startItalic > currentIndex Then
                    paragraph.Add(New Chunk(text.Substring(currentIndex, startItalic - currentIndex), normalFont))
                End If

                Dim endItalic As Integer = text.IndexOf("</i>", startItalic)

                If endItalic = -1 Then
                    paragraph.Add(New Chunk(text.Substring(startItalic), normalFont))
                    Exit While
                End If

                Dim italicText As String = text.Substring(startItalic + 3, endItalic - (startItalic + 3))
                paragraph.Add(New Chunk(italicText, italicFont))

                currentIndex = endItalic + 4
            End While

            Dim cell As New PdfPCell(paragraph)
            cell.Border = Rectangle.BOTTOM_BORDER
            cell.BorderWidthBottom = 0.5F
            cell.HorizontalAlignment = alignH
            cell.VerticalAlignment = Element.ALIGN_MIDDLE
            cell.PaddingBottom = 6

            Return cell

        Catch ex As Exception
            Dim font As New Font(Font.FontFamily.TIMES_ROMAN, 10, If(isBold, Font.BOLD, Font.NORMAL))
            Dim cell As New PdfPCell(New Phrase(If(text, String.Empty), font))

            cell.Border = Rectangle.BOTTOM_BORDER
            cell.BorderWidthBottom = 0.5F
            cell.HorizontalAlignment = alignH
            cell.VerticalAlignment = Element.ALIGN_MIDDLE
            cell.PaddingBottom = 6

            Return cell
        End Try
    End Function

    Private Function CreateCellTotal(text As String, Optional isBold As Boolean = False) As PdfPCell
        Try
            If text Is Nothing Then text = String.Empty

            Dim style As Integer = If(isBold, Font.BOLD, Font.NORMAL)
            Dim thisFont As New Font(Font.FontFamily.TIMES_ROMAN, 12, style)

            Dim lines As Integer = Regex.Split(text, "\r\n|\r|\n").Length
            Dim lineHeight As Single = 22
            Dim calculatedHeight As Single = lines * lineHeight

            Dim cell As New PdfPCell(New Phrase(text, thisFont))
            cell.Border = Rectangle.NO_BORDER
            cell.BorderWidthBottom = 0.15F
            cell.HorizontalAlignment = Element.ALIGN_RIGHT
            cell.VerticalAlignment = Element.ALIGN_MIDDLE
            cell.MinimumHeight = calculatedHeight
            cell.PaddingBottom = 8

            Return cell
        Catch ex As Exception
            Dim fallbackFont As New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.NORMAL)
            Dim fallbackCell As New PdfPCell(New Phrase("", fallbackFont))
            fallbackCell.Border = Rectangle.NO_BORDER
            fallbackCell.HorizontalAlignment = Element.ALIGN_RIGHT
            fallbackCell.VerticalAlignment = Element.ALIGN_MIDDLE
            fallbackCell.MinimumHeight = 22
            fallbackCell.PaddingBottom = 8

            Return fallbackCell
        End Try
    End Function

    Private Function CreateCellTotal_Local(text As String, Optional isBold As Boolean = False, Optional alignH As Integer = Element.ALIGN_LEFT) As PdfPCell
        Try
            If text Is Nothing Then text = String.Empty

            Dim style As Integer = If(isBold, Font.BOLD, Font.NORMAL)
            Dim thisFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, style)

            Dim lines As Integer = Regex.Split(text, "\r\n|\r|\n").Length
            Dim lineHeight As Single = 22
            Dim calculatedHeight As Single = lines * lineHeight

            Dim cell As New PdfPCell(New Phrase(text, thisFont))
            cell.Border = Rectangle.NO_BORDER
            cell.BorderWidthBottom = 0.15F
            cell.HorizontalAlignment = alignH
            cell.VerticalAlignment = Element.ALIGN_MIDDLE
            cell.MinimumHeight = calculatedHeight
            cell.PaddingBottom = 8

            Return cell
        Catch ex As Exception
            Dim fallbackFont As New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.NORMAL)
            Dim fallbackCell As New PdfPCell(New Phrase("", fallbackFont))
            fallbackCell.Border = Rectangle.NO_BORDER
            fallbackCell.HorizontalAlignment = alignH
            fallbackCell.VerticalAlignment = Element.ALIGN_MIDDLE
            fallbackCell.MinimumHeight = 22
            fallbackCell.PaddingBottom = 8

            Return fallbackCell
        End Try
    End Function

    Public Function BindContent_Local(headerId As String) As Byte()
        Using ms As New MemoryStream()
            Dim headerData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, Customers.CompanyDetailId AS CompanyDetailId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")

            Dim orderId As String = headerData("OrderId").ToString()
            Dim customerId As String = headerData("CustomerId").ToString()
            Dim customerName As String = headerData("CustomerName").ToString()
            Dim orderNumber As String = headerData("OrderNumber").ToString()
            Dim orderName As String = headerData("OrderName").ToString()
            Dim invoiceNumber As String = headerData("InvoiceNumber").ToString()

            Dim invoiceDate As String = String.Empty

            If Not String.IsNullOrEmpty(headerData("InvoiceDate").ToString()) Then
                If Not IsDBNull(headerData("InvoiceDate")) Then
                    Dim invDate As Date = Convert.ToDateTime(headerData("InvoiceDate"))
                    invoiceDate = invDate.ToString("dd MMM yyyy")
                End If
            End If

            Dim fullAddress As String = String.Empty
            Dim customerAddress As DataRow = GetDataRow("SELECT * FROM CustomerAddress WHERE CustomerId='" & customerId & "' AND [Primary]=1")
            If customerAddress IsNot Nothing Then
                Dim address As String = customerAddress("Address").ToString()
                Dim suburb As String = customerAddress("Suburb").ToString()
                Dim state As String = customerAddress("State").ToString()
                Dim postCode As String = customerAddress("PostCode").ToString()
                Dim country As String = "Indonesia"

                fullAddress = address
                fullAddress &= vbCrLf
                fullAddress &= String.Format("{0}, {1}, {2}", suburb, state, postCode)
                fullAddress &= vbCrLf
                fullAddress &= country
            End If

            Dim customerAbn As String = GetItemData("SELECT ABNNumber FROM CustomerBusiness WHERE CustomerId='" & customerId & "' AND [Primary]=1")

            Dim doc As New Document(PageSize.A4, 36, 36, 80, 50)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)

            writer.PageEvent = New PackingLocalEvents()
            doc.Open()

            Dim emptyLine As New Paragraph(" ", New Font(Font.FontFamily.TIMES_ROMAN, 10))
            emptyLine.SpacingBefore = 1

            Dim emptyLine2 As New Paragraph(" ", New Font(Font.FontFamily.TIMES_ROMAN, 10))
            emptyLine.SpacingBefore = 1

            Dim line As New draw.LineSeparator(0.5F, 100.0F, BaseColor.BLACK, Element.ALIGN_CENTER, -2)

            Dim itemTable As New PdfPTable(8)
            itemTable.WidthPercentage = 100
            itemTable.SetWidths(New Single() {0.05F, 0.15F, 0.51F, 0.05F, 0.05F, 0.052F, 0.052F, 0.05F})

            itemTable.AddCell(CreateCellDetail("No", True))
            itemTable.AddCell(CreateCellDetail("Kode Barang", True))
            itemTable.AddCell(CreateCellDetail("Nama Barang", True))
            itemTable.AddCell(CreateCellDetail("Qty", True, Element.ALIGN_CENTER))
            itemTable.AddCell(CreateCellDetail("CTN", True, Element.ALIGN_RIGHT))
            itemTable.AddCell(CreateCellDetail("NW (KG)", True, Element.ALIGN_RIGHT))
            itemTable.AddCell(CreateCellDetail("GW (KG)", True, Element.ALIGN_RIGHT))
            itemTable.AddCell(CreateCellDetail("VOL (M3)", True, Element.ALIGN_RIGHT))

            Dim commodityList As New List(Of String)

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@HeaderId", headerId)
            }
            Dim detailData As DataTable = GetDataTableSP("sp_OrderDetails_List_Invoice", params)
            For i As Integer = 0 To detailData.Rows.Count - 1
                Dim itemId As String = detailData.Rows(i)("Id").ToString()
                Dim itemNumber As Integer = detailData.Rows(i)("Item").ToString()

                Dim designName As String = detailData.Rows(i)("DesignName").ToString()
                Dim designType As String = detailData.Rows(i)("DesignType").ToString()
                Dim blindName As String = detailData.Rows(i)("BlindName").ToString()
                Dim fabricColourId As String = detailData.Rows(i)("FabricColour").ToString()
                Dim width As String = detailData.Rows(i)("Width").ToString()
                Dim drop As String = detailData.Rows(i)("Height").ToString()
                Dim size As String = String.Format("({0}x{1})", width, drop)

                Dim trackType As String = detailData.Rows(i)("TrackType").ToString()
                Dim trackColour As String = detailData.Rows(i)("TrackColour").ToString()

                Dim itemNote As String = detailData.Rows(i)("Notes").ToString()

                Dim invoiceName As String = detailData.Rows(i)("InvoiceName").ToString()
                Dim itemDescription As String = invoiceName

                Dim namaBarang As String = String.Empty

                If designName = "Service" Then
                    itemDescription = String.Format("{0}", invoiceName)
                    If Not String.IsNullOrEmpty(itemNote) Then
                        itemDescription &= vbCrLf
                        itemDescription &= itemNote
                    End If
                End If

                If designName = "Aluminium Blind" Then
                    itemDescription = String.Format("{0} {1}", invoiceName, size)
                    namaBarang = "Aluminium Venetian Blind"
                End If

                If designName = "Privacy Venetian" Then
                    itemDescription = String.Format("{0} {1}", invoiceName, size)
                    namaBarang = "Smart Privacy"
                End If

                If designName = "Venetian Blind" Then
                    itemDescription = String.Format("{0} {1}", invoiceName, size)
                    If blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" Then
                        namaBarang = "Ecowood Blind"
                    End If
                    If blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" Then
                        namaBarang = "Basswood Blind"
                    End If
                End If

                If designName = "Skyline Shutter Express" Then
                    itemDescription = String.Format("{0} {1}", invoiceName, size)
                    namaBarang = "PVC Shutter"
                End If

                If designName = "Cellular Shades" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1}", invoiceName, size)
                    If blindName = "Day & Night" Then
                        Dim secondFabricColour As String = GetItemData("SELECT FabricColourIdB FROM OrderDetails WHERE Id='" & itemId & "'")
                        Dim fabricColourNameB As String = GetFabricColourName(secondFabricColour)

                        itemDescription &= vbCrLf
                        itemDescription &= fabricColourName
                        itemDescription &= vbCrLf
                        itemDescription &= fabricColourNameB
                    End If

                    namaBarang = designName
                End If

                If designName = "Design Shades" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, fabricColourName, size)
                    namaBarang = designName
                End If

                If designName = "Roman Blind" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, fabricColourName, size)
                    namaBarang = designName
                End If

                If designName = "Soft Roman" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, fabricColourName, size)
                    namaBarang = "Roman Blind"
                End If

                If designName = "Roller Blind" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, fabricColourName, size)
                    namaBarang = designName
                End If

                If designName = "Curtain" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, fabricColourName, size)
                    itemDescription &= vbCrLf
                    itemDescription &= String.Format("{0} {1} ({2})", trackType, trackColour, width)
                    If blindName = "Curtain Only" Then
                        itemDescription = String.Format("{0} {1} {2}", invoiceName, fabricColourName, size)
                    End If
                    If blindName = "Track Only" Then
                        itemDescription = String.Format("{0} ({1})", invoiceName, width)
                    End If
                    namaBarang = "Roman Tailored"
                End If

                If designName = "Linea Valance" Then
                    itemDescription = String.Format("{0} ({1}mm)", invoiceName, width)
                    namaBarang = designName
                End If

                If designName = "Panel Glide" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, fabricColourName, size)
                    If blindName = "Track Only" Then
                        itemDescription = String.Format("{0} ({1})", invoiceName, width)
                    End If
                    namaBarang = designName
                End If

                If designName = "Pelmet" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, fabricColourName, size)
                    namaBarang = "Roller Pelmet"
                End If

                If designName = "Vertical" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    fabricColourName = fabricColourName.Replace("127mm ", "").Replace("89mm ", "").Trim()
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, fabricColourName, size)
                    If blindName = "Track Only" Then
                        itemDescription = String.Format("{0} ({1})", invoiceName, width)
                    End If
                    namaBarang = designName
                End If

                If designName = "Saphora Drape" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, fabricColourName, size)
                    namaBarang = "Vertical"
                End If

                Dim kodeBarang As String = String.Empty
                If Not String.IsNullOrEmpty(namaBarang) Then
                    kodeBarang = GetItemData("SELECT ItemCode FROM AKZero_KodeBarang WHERE Name='" & namaBarang & "' AND Active=1")
                    If Not commodityList.Contains(namaBarang) Then
                        commodityList.Add(namaBarang)
                    End If
                End If

                Dim qty As String = detailData.Rows(i)("Qty").ToString()

                itemTable.AddCell(CreateCellDetail(i + 1))
                itemTable.AddCell(CreateCellDetail(kodeBarang))
                itemTable.AddCell(CreateCellDetail(itemDescription))
                itemTable.AddCell(CreateCellDetail(qty, False, Element.ALIGN_CENTER))
                itemTable.AddCell(CreateCellDetail(qty, False, Element.ALIGN_CENTER))
                itemTable.AddCell(CreateCellDetail(qty, False, Element.ALIGN_CENTER))
                itemTable.AddCell(CreateCellDetail(qty, False, Element.ALIGN_CENTER))
                itemTable.AddCell(CreateCellDetail(qty, False, Element.ALIGN_CENTER))
            Next

            Dim commoditiy As String = String.Join(", ", commodityList)

            Dim space As New Paragraph(" ")
            space.SpacingBefore = 5

            Dim footerTable As New PdfPTable(2)
            footerTable.WidthPercentage = 100
            footerTable.SetWidths(New Single() {0.6F, 0.4F})

            Dim paymentCell As New PdfPCell()
            paymentCell.Border = Rectangle.NO_BORDER
            paymentCell.VerticalAlignment = Element.ALIGN_TOP

            Dim normalFont As New Font(Font.FontFamily.TIMES_ROMAN, 10)
            Dim boldFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)

            Dim totalTable As New PdfPTable(2)
            totalTable.WidthPercentage = 100
            totalTable.SetWidths(New Single() {0.65F, 0.35F})

            Dim totalQty As Integer = detailData.Rows.Count

            Dim totalQtyText As String = String.Format("{0} Set", totalQty.ToString())

            totalTable.AddCell(CreateCellTotal_Local("Total Qty (Set)"))
            totalTable.AddCell(CreateCellTotal_Local(totalQtyText, alignH:=Element.ALIGN_RIGHT))

            totalTable.AddCell(CreateCellTotal_Local("Total Cartoon"))
            totalTable.AddCell(CreateCellTotal_Local(totalQtyText, alignH:=Element.ALIGN_RIGHT))

            totalTable.AddCell(CreateCellTotal_Local("Total Net Weight Qty"))
            totalTable.AddCell(CreateCellTotal_Local(totalQtyText, alignH:=Element.ALIGN_RIGHT))

            totalTable.AddCell(CreateCellTotal_Local("Total Gross Weight Qty"))
            totalTable.AddCell(CreateCellTotal_Local(totalQtyText, alignH:=Element.ALIGN_RIGHT))

            totalTable.AddCell(CreateCellTotal_Local("Total Volume", isBold:=True))
            totalTable.AddCell(CreateCellTotal_Local(totalQtyText, alignH:=Element.ALIGN_RIGHT))

            Dim totalCell As New PdfPCell(totalTable)
            totalCell.Border = Rectangle.NO_BORDER
            totalCell.Padding = 0

            footerTable.AddCell(paymentCell)
            footerTable.AddCell(totalCell)

            Dim signSpace As New Paragraph(" ")
            signSpace.SpacingBefore = 6
            doc.Add(signSpace)

            Dim signTable As New PdfPTable(2)
            signTable.WidthPercentage = 100
            signTable.SetWidths(New Single() {0.6F, 0.4F})

            Dim blankCell As New PdfPCell()
            blankCell.Border = Rectangle.NO_BORDER

            Dim signCell As New PdfPCell()
            signCell.Border = Rectangle.NO_BORDER
            signCell.HorizontalAlignment = Element.ALIGN_RIGHT

            Dim sign As New Paragraph()
            sign.Alignment = Element.ALIGN_RIGHT
            sign.Add(New Chunk("Serang, " & Date.Today.ToString("dd MMMM yyyy", idIDR), New Font(Font.FontFamily.TIMES_ROMAN, 11, Font.BOLD)))
            sign.Add(Chunk.NEWLINE)
            sign.Add(Chunk.NEWLINE)
            sign.Add(Chunk.NEWLINE)
            sign.Add(Chunk.NEWLINE)
            sign.Add(New Chunk("Saifullah Toyyib", New Font(Font.FontFamily.TIMES_ROMAN, 11, Font.BOLD)))

            signCell.AddElement(sign)

            signTable.AddCell(blankCell)
            signTable.AddCell(signCell)

            Dim headerTable As New PdfPTable(2)
            headerTable.WidthPercentage = 100
            headerTable.SetWidths(New Single() {0.6F, 0.4F})
            headerTable.DefaultCell.Border = Rectangle.NO_BORDER

            Dim leftTable As New PdfPTable(3)
            leftTable.WidthPercentage = 100
            leftTable.SetWidths({0.27F, 0.03F, 0.7F})
            leftTable.DefaultCell.Border = Rectangle.NO_BORDER

            leftTable.AddCell(CreateCell("Kepada", True, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", False, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(customerName, False, Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Nama", True, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", False, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(customerName, False, Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Alamat", True, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", False, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(fullAddress, False, Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("", True, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell("", False, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(String.Empty))

            leftTable.AddCell(CreateCell("NPWP", True))
            leftTable.AddCell(CreateCell(":", False, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(customerAbn, False, Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Komoditas", True))
            leftTable.AddCell(CreateCell(":", False, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(commoditiy, False, Element.ALIGN_TOP))

            Dim leftCell As New PdfPCell(leftTable)
            leftCell.Border = Rectangle.NO_BORDER
            headerTable.AddCell(leftCell)

            Dim rightTable As New PdfPTable(3)
            rightTable.WidthPercentage = 100
            rightTable.SetWidths({0.4F, 0.03F, 0.57F})
            rightTable.DefaultCell.Border = Rectangle.NO_BORDER

            rightTable.AddCell(CreateCell("Nomor PO", True, Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", False, Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(orderNumber, False, Element.ALIGN_TOP))

            rightTable.AddCell(CreateCell("Nomor PL", True, Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", False, Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(invoiceNumber, False, Element.ALIGN_TOP))

            rightTable.AddCell(CreateCell("Tanggal PL", True, Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", False, Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(invoiceDate, False, Element.ALIGN_TOP))

            rightTable.AddCell(CreateCell("Tgl Pengiriman", True, Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", False, Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(String.Empty, False, Element.ALIGN_TOP))

            Dim rightCell As New PdfPCell(rightTable)
            rightCell.Border = Rectangle.NO_BORDER
            headerTable.AddCell(rightCell)

            doc.Add(headerTable)
            doc.Add(emptyLine)
            doc.Add(New Chunk(line))
            doc.Add(emptyLine2)
            doc.Add(itemTable)
            doc.Add(space)
            doc.Add(footerTable)
            doc.Add(signSpace)
            doc.Add(signTable)

            doc.Close()

            Return ms.ToArray()
        End Using
    End Function
End Class

Public Class PackingLocalEvents
    Inherits PdfPageEventHelper

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent

        Dim headerTable As New PdfPTable(2)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True
        headerTable.SetWidths(New Single() {0.8F, 0.2F})

        Dim imagePath As String = HttpContext.Current.Server.MapPath("~/assets/images/logo/big.jpg")

        Dim img As Image = Image.GetInstance(imagePath)
        Dim availableWidth As Single = (document.PageSize.Width - 72) * 0.7F
        Dim availableHeight As Single = 45

        img.ScaleToFit(availableWidth, availableHeight)

        Dim imgCell As New PdfPCell(img)
        imgCell.Border = Rectangle.NO_BORDER
        imgCell.PaddingLeft = 5
        imgCell.PaddingRight = 5
        imgCell.HorizontalAlignment = Element.ALIGN_LEFT
        imgCell.VerticalAlignment = Element.ALIGN_MIDDLE

        headerTable.AddCell(imgCell)

        Dim invoiceCell As New PdfPCell(New Phrase("PACKING LIST", New Font(Font.FontFamily.TIMES_ROMAN, 20, Font.BOLD)))

        invoiceCell.Border = Rectangle.NO_BORDER
        invoiceCell.HorizontalAlignment = Element.ALIGN_RIGHT
        invoiceCell.VerticalAlignment = Element.ALIGN_BOTTOM
        invoiceCell.PaddingBottom = 2

        headerTable.AddCell(invoiceCell)

        Dim headerTopY As Single = document.PageSize.Height - 20
        headerTable.WriteSelectedRows(0, -1, 36, headerTopY, cb)

        Dim headerHeight As Single = headerTable.TotalHeight

        Dim lineTable As New PdfPTable(1)
        lineTable.TotalWidth = document.PageSize.Width - 72
        lineTable.LockedWidth = True

        Dim line As New draw.LineSeparator(0.5F, 100.0F, BaseColor.BLACK, Element.ALIGN_CENTER, -1)
        Dim lineChunk As New Chunk(line)
        Dim linePhrase As New Phrase(lineChunk)

        Dim lineCell As New PdfPCell(linePhrase)
        lineCell.Border = 0
        lineCell.PaddingTop = 2
        lineCell.PaddingBottom = 2
        lineTable.AddCell(lineCell)

        Dim headerBottomY As Single = headerTopY - headerHeight - 5
        lineTable.WriteSelectedRows(0, -1, 36, headerBottomY, cb)
    End Sub
End Class