Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class InvoiceClass
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
                    Using da As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return dt
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Protected Function GetItemData(thisString As String) As String
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

    Protected Function GetItemData_Decimal(thisString As String) As Decimal
        Dim result As Decimal = 0D
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
            result = 0D
        End Try
        Return result
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

    Private Function CreateCell(text As String, Optional isBold As Boolean = False, Optional alignV As Integer = Element.ALIGN_MIDDLE) As PdfPCell
        Dim style As Integer = If(isBold, Font.BOLD, Font.NORMAL)
        Dim thisFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, style)
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

    Private Function CreateCellTotal(text As String, Optional isBold As Boolean = False) As PdfPCell
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
    End Function

    Public Sub BindContent(headerId As String, filePath As String)
        Using fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
            Dim doc As New Document(PageSize.A4, 36, 36, 110, 180)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)

            Dim headerData As DataRow = GetDataRow("SELECT OrderHeaders.*, OrderInvoices.InvoiceNumber AS InvoiceNumber, OrderInvoices.InvoiceDate AS InvoiceDate, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, Customers.CompanyDetailId AS CompanyDetailId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id LEFT JOIN OrderInvoices ON OrderHeaders.Id=OrderInvoices.Id WHERE OrderHeaders.Id='" & headerId & "'")

            Dim orderId As String = headerData("OrderId").ToString()
            Dim customerId As String = headerData("CustomerId").ToString()
            Dim customerName As String = headerData("CustomerName").ToString()
            Dim orderNumber As String = headerData("OrderNumber").ToString()
            Dim orderName As String = headerData("OrderName").ToString()
            Dim companyId As String = headerData("CompanyId").ToString()
            Dim invoiceNumber As String = headerData("InvoiceNumber").ToString()

            Dim issueDate As String = String.Empty
            Dim dueDate As String = String.Empty

            If Not String.IsNullOrEmpty(headerData("InvoiceDate").ToString()) Then
                Dim invDate As Date = Convert.ToDateTime(headerData("InvoiceDate"))
                issueDate = invDate.ToString("dd MMM yyyy")

                Dim dueDt As Date = invDate.AddDays(30)
                dueDate = dueDt.ToString("dd MMM yyyy")
            End If

            Dim fullAddress As String = String.Empty
            Dim customerAddress As DataRow = GetDataRow("SELECT * FROM CustomerAddress WHERE CustomerId='" & customerId & "' AND [Primary]=1")
            If customerAddress IsNot Nothing Then
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

            Dim customerAbn As String = GetItemData("SELECT ABNNumber FROM CustomerBusiness WHERE CustomerId='" & customerId & "' AND [Primary]=1")

            Dim invoiceTo As String = customerName
            invoiceTo &= vbCrLf
            invoiceTo &= vbCrLf
            invoiceTo &= fullAddress

            Dim invoiceFrom As String = String.Empty

            If companyId = "2" Then
                invoiceFrom = "JPM Direct Pty Ltd"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Attention: Matt McCamey"
                invoiceFrom &= vbCrLf
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Ground Floor, 97-99 Bathrust St,"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Sydney NSW 2000"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Australia"
            End If

            If companyId = "3" Then
                invoiceFrom = "Accent At Home"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Attention: Yudi Tjan"
                invoiceFrom &= vbCrLf
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Ground Floor, 97-99 Bathrust St,"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Sydney NSW 2000"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Indonesia"
            End If

            If companyId = "5" Then
                invoiceFrom = "PT Bumi Indah Global"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Attention: Yudi Tjan"
                invoiceFrom &= vbCrLf
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Ground Floor, 97-99 Bathrust St,"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Sydney NSW 2000"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Indonesia"
            End If

            Dim reference As String = String.Format("{0} - {1}", orderNumber, orderName)

            Dim sumPrice As Decimal = GetItemData_Decimal("SELECT SUM(SellPrice) AS SumPrice FROM OrderCostings WHERE HeaderId='" & headerId & "' AND Type='Final'")
            Dim gst As Decimal = sumPrice * 10 / 100
            If companyId = "1" OrElse companyId = "3" Then
                gst = sumPrice * 11 / 100
            End If
            Dim finaltotal As Decimal = sumPrice + gst

            Dim sumPriceText As String = sumPrice.ToString("N2", enUS)
            Dim gstText As String = gst.ToString("N2", enUS)
            Dim finalTotalText As String = finaltotal.ToString("N2", enUS)

            If companyId = "3" Then
                sumPriceText = sumPrice.ToString("N2", idIDR)
                gstText = gst.ToString("N2", idIDR)
                finalTotalText = finaltotal.ToString("N2", idIDR)
            End If

            writer.PageEvent = New InvoiceEvents(companyId, finalTotalText)
            doc.Open()

            Dim companyTable As New PdfPTable(2)
            companyTable.WidthPercentage = 100
            companyTable.SetWidths(New Single() {0.6F, 0.4F})
            companyTable.DefaultCell.Border = Rectangle.NO_BORDER

            Dim leftTable As New PdfPTable(3)
            leftTable.WidthPercentage = 100
            leftTable.SetWidths({0.27F, 0.03F, 0.7F})
            leftTable.DefaultCell.Border = Rectangle.NO_BORDER

            leftTable.AddCell(CreateCell("To", True, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(""))
            leftTable.AddCell(CreateCell(invoiceTo, False, Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Invoice Number", True))
            leftTable.AddCell(CreateCell(""))
            leftTable.AddCell(CreateCell(invoiceNumber))

            leftTable.AddCell(CreateCell("Reference", True, Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(""))
            leftTable.AddCell(CreateCell(reference))

            leftTable.AddCell(CreateCell("ABN", True))
            leftTable.AddCell(CreateCell(""))
            leftTable.AddCell(CreateCell(customerAbn))

            leftTable.AddCell(CreateCell("Issue Date", True))
            leftTable.AddCell(CreateCell(""))
            leftTable.AddCell(CreateCell(issueDate))

            leftTable.AddCell(CreateCell("Due Date", True))
            leftTable.AddCell(CreateCell(""))
            leftTable.AddCell(CreateCell(dueDate))

            Dim leftCell As New PdfPCell(leftTable)
            leftCell.Border = Rectangle.NO_BORDER
            companyTable.AddCell(leftCell)

            Dim rightTable As New PdfPTable(3)
            rightTable.WidthPercentage = 100
            rightTable.SetWidths({0.2F, 0.03F, 0.77F})
            rightTable.DefaultCell.Border = Rectangle.NO_BORDER

            rightTable.AddCell(CreateCell("From", True, Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(""))
            rightTable.AddCell(CreateCell(invoiceFrom, False, Element.ALIGN_TOP))

            If companyId = "2" Then
                rightTable.AddCell(CreateCell("ABN", True))
                rightTable.AddCell(CreateCell(""))
                rightTable.AddCell(CreateCell("17 143 599 973"))
            End If

            Dim rightCell As New PdfPCell(rightTable)
            rightCell.Border = Rectangle.NO_BORDER
            companyTable.AddCell(rightCell)

            doc.Add(companyTable)

            Dim emptyLine As New Paragraph(" ", New Font(Font.FontFamily.TIMES_ROMAN, 10))
            emptyLine.SpacingBefore = 1
            doc.Add(emptyLine)

            Dim line As New draw.LineSeparator(0.5F, 100.0F, BaseColor.BLACK, Element.ALIGN_CENTER, -2)
            doc.Add(New Chunk(line))

            emptyLine.SpacingAfter = 3
            doc.Add(emptyLine)

            Dim table As New PdfPTable(3)
            table.WidthPercentage = 100
            table.SetWidths(New Single() {0.7F, 0.1F, 0.2F})

            table.AddCell(CreateCellDetail("Description", True))
            table.AddCell(CreateCellDetail("Qty", True, Element.ALIGN_CENTER))
            table.AddCell(CreateCellDetail("Unit Price", True, Element.ALIGN_RIGHT))

            Dim detailData As DataTable = GetDataTable("SELECT OrderDetails.Id, OrderDetails.Qty, Designs.Name AS DesignName, Blinds.Name AS BlindName, OrderDetails.FabricColourId AS FabricColour, OrderDetails.TrackType AS TrackType, OrderDetails.TrackColour AS TrackColour, OrderDetails.Width AS Width, OrderDetails.[Drop] AS Height, OrderDetails.FrameColour AS FrameColour, OrderDetails.DoorCutOut AS DoorCutOut, OrderDetails.SquareMetre AS SQM, OrderDetails.LinearMetre AS LM, Products.Name AS ProductName, Products.InvoiceName AS InvoiceName, 1 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Qty, Designs.Name AS DesignName, Blinds.Name AS BlindName, OrderDetails.FabricColourIdB AS FabricColour, OrderDetails.TrackTypeB AS TrackType, OrderDetails.TrackColourB AS TrackColour, OrderDetails.WidthB AS Width, OrderDetails.DropB AS Height, OrderDetails.FrameColour AS FrameColour, OrderDetails.DoorCutOut AS DoorCutOut, OrderDetails.SquareMetreB AS SQM, OrderDetails.LinearMetreB AS LM, Products.Name AS ProductName, Products.InvoiceName AS InvoiceName, 2 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND (((Designs.Name='Aluminium Blind' OR Designs.Name='Venetian Blind') AND (OrderDetails.SubType LIKE '%2 on 1%' OR OrderDetails.SubType LIKE '%3 on 1%')) OR (Designs.Name = 'Cellular Shades' AND Blinds.Name='Day & Night') OR (Designs.Name='Roller Blind' AND (Blinds.Name='Dual Blinds' OR Blinds.Name='Link 2 Blinds Dependent' OR Blinds.Name='Link 2 Blinds Independent' OR Blinds.Name='Link 3 Blinds Dependent' OR Blinds.Name='Link 3 Blinds Independent with Dependent' OR Blinds.Name='DB Link 2 Blinds Dependent' OR Blinds.Name='DB Link 2 Blinds Independent')) OR (Designs.Name='Curtain' AND Blinds.Name='Double Curtain & Track')) UNION ALL SELECT OrderDetails.Id, OrderDetails.Qty, Designs.Name AS DesignName, Blinds.Name AS BlindName, OrderDetails.FabricColourIdC AS FabricColour, NULL AS TrackType, NULL AS TrackColour, OrderDetails.WidthC AS Width, OrderDetails.DropC AS Height, OrderDetails.FrameColour AS FrameColour, OrderDetails.DoorCutOut AS DoorCutOut, OrderDetails.SquareMetreC AS SQM, OrderDetails.LinearMetreC AS LM, Products.Name AS ProductName, Products.InvoiceName AS InvoiceName, 3 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND (Designs.Name='Roller Blind' AND (Blinds.Name='Link 3 Blinds Dependent' OR Blinds.Name='Link 3 Blinds Independent with Dependent' OR Blinds.Name='DB Link 2 Blinds Dependent' OR Blinds.Name='DB Link 2 Blinds Independent')) UNION ALL SELECT OrderDetails.Id, OrderDetails.Qty, Designs.Name AS DesignName, Blinds.Name AS BlindName, OrderDetails.FabricColourIdD AS FabricColour, NULL AS TrackType, NULL AS TrackColour, OrderDetails.WidthD AS Width, OrderDetails.DropD AS Height, OrderDetails.FrameColour AS FrameColour, OrderDetails.DoorCutOut AS DoorCutOut, OrderDetails.SquareMetreD AS SQM, OrderDetails.LinearMetreD AS LM, Products.Name AS ProductName, Products.InvoiceName AS InvoiceName, 4 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND (Designs.Name='Roller Blind' AND (Blinds.Name='DB Link 2 Blinds Dependent' OR Blinds.Name='DB Link 2 Blinds Independent')) UNION ALL SELECT OrderDetails.Id, OrderDetails.Qty, Designs.Name AS DesignName, Blinds.Name AS BlindName, OrderDetails.FabricColourIdE AS FabricColour, NULL AS TrackType, NULL AS TrackColour, OrderDetails.WidthE AS Width, OrderDetails.DropE AS Height, OrderDetails.FrameColour AS FrameColour, OrderDetails.DoorCutOut AS DoorCutOut, OrderDetails.SquareMetreE AS SQM, OrderDetails.LinearMetreE AS LM, Products.Name AS ProductName, Products.InvoiceName AS InvoiceName, 5 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND (Designs.Name='Roller Blind' AND (Blinds.Name='DB Link 3 Blinds Dependent' OR Blinds.Name='DB Link 3 Blinds Independent with Dependent')) UNION ALL SELECT OrderDetails.Id, OrderDetails.Qty, Designs.Name AS DesignName, Blinds.Name AS BlindName, OrderDetails.FabricColourIdF AS FabricColour, NULL AS TrackType, NULL AS TrackColour, OrderDetails.WidthF AS Width, OrderDetails.DropF AS Height, OrderDetails.FrameColour AS FrameColour, OrderDetails.DoorCutOut AS DoorCutOut, OrderDetails.SquareMetreF AS SQM, OrderDetails.LinearMetreF AS LM, Products.Name AS ProductName, Products.InvoiceName AS InvoiceName, 6 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND (Designs.Name='Roller Blind' AND (Blinds.Name='DB Link 3 Blinds Dependent' OR Blinds.Name='DB Link 3 Blinds Independent with Dependent')) ORDER BY OrderDetails.Id, Item ASC;")
            For i As Integer = 0 To detailData.Rows.Count - 1
                Dim itemId As String = detailData.Rows(i)("Id").ToString()
                Dim itemNumber As Integer = detailData.Rows(i)("Item").ToString()

                Dim designName As String = detailData.Rows(i)("DesignName").ToString()
                Dim blindName As String = detailData.Rows(i)("BlindName").ToString()
                Dim fabricColourId As String = detailData.Rows(i)("FabricColour").ToString()
                Dim width As String = detailData.Rows(i)("Width").ToString()
                Dim drop As String = detailData.Rows(i)("Height").ToString()
                Dim size As String = String.Format("({0}x{1})", width, drop)

                Dim trackType As String = detailData.Rows(i)("TrackType").ToString()
                Dim trackColour As String = detailData.Rows(i)("TrackColour").ToString()

                Dim linearMetre As Decimal = 0D
                Dim squareMetre As Decimal = 0D

                If Not IsDBNull(detailData.Rows(i)("LM")) Then
                    linearMetre = Math.Round(Convert.ToDecimal(detailData.Rows(i)("LM")), 2)
                End If
                If Not IsDBNull(detailData.Rows(i)("SQM")) Then
                    squareMetre = Math.Round(Convert.ToDecimal(detailData.Rows(i)("SQM")), 2)
                End If

                Dim linearMetreText As String = String.Format("{0}lm", linearMetre.ToString("0.##", enUS))
                Dim squareMetreText As String = String.Format("{0}sqm", squareMetre.ToString("0.##", enUS))

                Dim invoiceName As String = detailData.Rows(i)("InvoiceName").ToString()
                Dim itemDescription As String = invoiceName

                If designName = "Aluminium Blind" OrElse designName = "Privacy Venetian" OrElse designName = "Venetian Blind" OrElse designName = "Skyline Shutter Express" OrElse designName = "Skyline Shutter Ocean" Then
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, size, squareMetreText)
                End If

                If designName = "Cellular Shades" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0}", invoiceName)
                    itemDescription &= vbCrLf
                    itemDescription &= String.Format("{0} {1} {2}", fabricColourName, size, squareMetreText)
                End If

                If designName = "Design Shades" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                End If

                If designName = "Roman Blind" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                End If

                If designName = "Roller Blind" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                End If

                If designName = "Curtain" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                    itemDescription &= vbCrLf
                    itemDescription &= String.Format("{0} {1} ({2}) {3}", trackType, trackColour, width, linearMetreText)

                    If blindName = "Curtain Only" Then
                        itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                    End If

                    If blindName = "Track Only" Then
                        itemDescription = String.Format("{0} {1} ({2}) {3}", invoiceName, width, linearMetreText)
                    End If
                End If

                If designName = "Linea Valance" Then
                    itemDescription = String.Format("{0} ({1}mm) {2}", invoiceName, width, linearMetreText)
                End If

                If designName = "Panel Glide" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                    If blindName = "Track Only" Then
                        itemDescription = String.Format("{0} ({1}) {2}", invoiceName, width, linearMetreText)
                    End If
                End If

                If designName = "Pelmet" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, linearMetreText)
                End If

                If designName = "Vertical" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                    If blindName = "Track Only" Then
                        itemDescription = String.Format("{0} ({1}) {2}", invoiceName, width, linearMetreText)
                    End If
                End If

                If designName = "Saphora Drape" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                End If

                Dim pricingData As DataTable = GetDataTable("SELECT * FROM OrderCostings WHERE HeaderId='" & headerId & "' AND ItemId='" & itemId & "' AND Number='" & itemNumber & "' AND Type='Surcharge'")
                If pricingData.Rows.Count > 0 Then
                    For iPricing As Integer = 0 To pricingData.Rows.Count - 1
                        Dim pricingDesc As String = pricingData.Rows(iPricing)("Description").ToString()
                        If itemNumber = "1" Then pricingDesc = pricingDesc.Replace("#1 ", "")
                        If itemNumber = "2" Then pricingDesc = pricingDesc.Replace("#2 ", "")
                        If itemNumber = "3" Then pricingDesc = pricingDesc.Replace("#3 ", "")
                        If itemNumber = "4" Then pricingDesc = pricingDesc.Replace("#4 ", "")
                        If itemNumber = "5" Then pricingDesc = pricingDesc.Replace("#5 ", "")
                        If itemNumber = "6" Then pricingDesc = pricingDesc.Replace("#6 ", "")

                        itemDescription &= vbCrLf
                        itemDescription &= pricingDesc
                    Next
                End If

                Dim finalCost As Decimal = GetItemData_Decimal("SELECT SUM(SellPrice) FROM OrderCostings WHERE HeaderId='" & headerId & "' AND ItemId='" & itemId & "' AND Number='" & itemNumber & "'")

                Dim finalCostText As String = finalCost.ToString("N2", enUS)
                If companyId = "1" OrElse companyId = "3" Then
                    finalCostText = finalCost.ToString("N2", idIDR)
                End If

                table.AddCell(CreateCellDetail(itemDescription))
                table.AddCell(CreateCellDetail(detailData.Rows(i)("Qty").ToString(), False, Element.ALIGN_CENTER))
                table.AddCell(CreateCellDetail(finalCostText, True, Element.ALIGN_RIGHT))
            Next

            table.AddCell(CreateCellTotal("Sub Total"))
            table.AddCell(CreateCellTotal(String.Empty))
            table.AddCell(CreateCellTotal(sumPriceText))

            Dim textGST As String = "Total GST 10%"
            Dim textTotal As String = String.Format("AUD {0}", finalTotalText)

            If companyId = "3" OrElse companyId = "5" Then
                textGST = "Total GST 11%"
                textTotal = String.Format("RP {0}", finalTotalText)
            End If

            table.AddCell(CreateCellTotal(textGST))
            table.AddCell(CreateCellTotal(String.Empty))
            table.AddCell(CreateCellTotal(gstText))

            table.AddCell(CreateCellTotal("TOTAL", isBold:=True))
            table.AddCell(CreateCellTotal(String.Empty))
            table.AddCell(CreateCellTotal(textTotal, isBold:=True))

            doc.Add(table)
            doc.Close()
        End Using
    End Sub
End Class

Public Class InvoiceEvents
    Inherits PdfPageEventHelper

    Private companyId As String
    Private totalPrice As String

    Public Sub New(companyId As String, totalPrice As String)
        Me.companyId = companyId
        Me.totalPrice = totalPrice
    End Sub

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent

        Dim headerTable As New PdfPTable(2)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True
        headerTable.SetWidths(New Single() {0.6F, 0.4F})

        Dim imagePath As String = HttpContext.Current.Server.MapPath("~/assets/images/logo/general.jpg")

        If companyId = "2" Then
            imagePath = HttpContext.Current.Server.MapPath("~/assets/images/logo/jpmdirect.jpg")
        End If
        If companyId = "3" Then
            imagePath = HttpContext.Current.Server.MapPath("~/assets/images/logo/accent.png")
        End If
        If companyId = "5" Then
            imagePath = HttpContext.Current.Server.MapPath("~/assets/images/logo/big.JPG")
        End If

        Dim img As Image = Image.GetInstance(imagePath)
        img.ScaleToFit(150, 60)

        Dim imgCell As New PdfPCell(img)
        imgCell.Border = 0
        imgCell.HorizontalAlignment = Element.ALIGN_LEFT
        imgCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(imgCell)

        Dim mataUang As String = String.Empty
        If companyId = "2" OrElse companyId = "4" Then mataUang = "AUD"
        If companyId = "3" OrElse companyId = "5" Then mataUang = "RP"

        Dim finalPrice As String = String.Format("{0} {1}", Me.totalPrice, mataUang)

        Dim phrase As New Paragraph()
        phrase.Alignment = Element.ALIGN_RIGHT
        phrase.Add(New Chunk("TAX INVOICE", New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)))
        phrase.Add(Chunk.NEWLINE)
        phrase.Add(New Chunk(finalPrice, New Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD, BaseColor.BLACK)))

        Dim headerCell As New PdfPCell(phrase)
        headerCell.Border = 0
        headerCell.HorizontalAlignment = Element.ALIGN_RIGHT
        headerCell.VerticalAlignment = Element.ALIGN_TOP
        headerCell.PaddingTop = 5
        headerTable.AddCell(headerCell)

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

        Dim lineBeforeTermsTable As New PdfPTable(1)
        lineBeforeTermsTable.TotalWidth = document.PageSize.Width - 72
        lineBeforeTermsTable.LockedWidth = True

        Dim hr As New draw.LineSeparator(1.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_CENTER, -1)
        Dim hrChunk As New Chunk(hr)
        Dim hrPhrase As New Phrase(hrChunk)

        Dim hrCell As New PdfPCell(hrPhrase)
        hrCell.Border = 0
        hrCell.PaddingTop = 5
        hrCell.PaddingBottom = 5
        lineBeforeTermsTable.AddCell(hrCell)

        lineBeforeTermsTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(120), cb)

        Dim termsTable As New PdfPTable(1)
        termsTable.TotalWidth = document.PageSize.Width - 72
        termsTable.LockedWidth = True

        Dim termsText As String = String.Empty

        If companyId = "2" Then
            termsText = "Account Name : JPM Direct Pty Ltd" & vbCrLf &
                                  "BSB : 084 209" & vbCrLf &
                                  "Acct :  721 649 678" & vbCrLf &
                                  "Bank : National Australia Bank Limited (NAB)" & vbCrLf & vbCrLf &
                                  "https://pay.b2bpay.com.au/JPMDi"
        End If

        Dim termsPhrase As New Phrase()
        termsPhrase.Add(New Chunk("Terms & Conditions:" & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        termsPhrase.Add(New Chunk(vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10)))
        termsPhrase.Add(New Chunk(termsText & vbCrLf & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10)))

        Dim termsCell As New PdfPCell(termsPhrase)
        termsCell.Border = 0
        termsCell.HorizontalAlignment = Element.ALIGN_LEFT
        termsCell.VerticalAlignment = Element.ALIGN_TOP
        termsCell.PaddingTop = 5
        termsCell.PaddingBottom = 5
        termsTable.AddCell(termsCell)

        termsTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(100), cb)
    End Sub
End Class





