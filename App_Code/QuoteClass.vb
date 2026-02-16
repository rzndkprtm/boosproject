Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class QuoteClass
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

    Public Function GetDataTableSP(spName As String, params As List(Of SqlParameter)) As DataTable
        Dim dt As New DataTable()
        Using conn As New SqlConnection(myConn)
            Using cmd As New SqlCommand(spName, conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddRange(params.ToArray())

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
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

    Protected Function CreateCellDetail(text As String, Optional isBold As Boolean = False, Optional alignH As Integer = Element.ALIGN_LEFT) As PdfPCell
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

    Protected Function CreateCellTotal(text As String, Optional isBold As Boolean = False) As PdfPCell
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

    Public Function BindContent(headerId As String) As Byte()
        Using ms As New MemoryStream()
            Dim doc As New Document(PageSize.A4, 36, 36, 110, 180)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)

            Dim headerData As DataRow = GetDataRow("SELECT OrderHeaders.*, OrderInvoices.InvoiceNumber AS InvoiceNumber, OrderInvoices.InvoiceDate AS InvoiceDate, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, Customers.CompanyDetailId AS CompanyDetailId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id LEFT JOIN OrderInvoices ON OrderHeaders.Id=OrderInvoices.Id WHERE OrderHeaders.Id='" & headerId & "'")

            Dim orderId As String = headerData("OrderId").ToString()
            Dim customerId As String = headerData("CustomerId").ToString()
            Dim customerName As String = headerData("CustomerName").ToString()
            Dim orderNumber As String = headerData("OrderNumber").ToString()
            Dim orderName As String = headerData("OrderName").ToString()
            Dim orderType As String = headerData("OrderType").ToString()
            Dim companyId As String = headerData("CompanyId").ToString()
            Dim companyDetailId As String = headerData("CompanyDetailId").ToString()

            Dim estimator As String = String.Empty
            Dim supervisor As String = String.Empty
            If orderType = "Builder" Then
                Dim builderData As DataRow = GetDataRow("SELECT * FROM OrderBuilders WHERE Id='" & headerId & "'")
                If builderData IsNot Nothing Then
                    estimator = builderData("Estimator").ToString()
                    supervisor = builderData("Supervisor").ToString()
                End If
            End If

            Dim invoiceNumber As String = headerData("InvoiceNumber").ToString()

            Dim issueDate As String = Now.ToString("dd MMM yyyy")
            Dim dueDate As String = String.Empty

            Dim daysRemaining As Integer = 0

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
                invoiceFrom &= "Ruko De Mansion Blok D No 9"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Kunciran, Kota Tangerang"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Banten 15143"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Indonesia"
            End If

            If companyId = "5" Then
                invoiceFrom = "PT Bumi Indah Global"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Attention: Yudi Tjan"
                invoiceFrom &= vbCrLf
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Jl. Cikande-Rangkas Bitung KM 4,5, Kareo, Junti, Kec. Jawilan,"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Kabupaten Serang, Banten 42177"
                invoiceFrom &= vbCrLf
                invoiceFrom &= "Indonesia"
            End If

            Dim reference As String = String.Format("{0} - {1}", orderNumber, orderName)

            Dim sumPrice As Decimal = GetItemData_Decimal("SELECT SUM(SellPrice) AS SumPrice FROM OrderCostings WHERE HeaderId='" & headerId & "' AND Type='Final'")
            Dim gst As Decimal = 0D
            If companyId = "2" OrElse companyId = "4" Then gst = sumPrice * 10 / 100
            If companyId = "3" OrElse companyId = "5" Then gst = sumPrice * 11 / 100

            Dim finaltotal As Decimal = sumPrice + gst

            Dim sumPriceText As String = sumPrice.ToString("N2", enUS)
            Dim gstText As String = gst.ToString("N2", enUS)
            Dim finalTotalText As String = finaltotal.ToString("N2", enUS)

            If companyId = "3" OrElse companyId = "5" Then
                sumPriceText = sumPrice.ToString("N2", idIDR)
                gstText = gst.ToString("N2", idIDR)
                finalTotalText = finaltotal.ToString("N2", idIDR)
            End If

            writer.PageEvent = New QuoteEvents(companyId, daysRemaining, finalTotalText)
            doc.Open()

            Dim companyTable As New PdfPTable(2)
            companyTable.WidthPercentage = 100
            companyTable.SetWidths(New Single() {0.6F, 0.4F})
            companyTable.DefaultCell.Border = Rectangle.NO_BORDER

            Dim leftTable As New PdfPTable(3)
            leftTable.WidthPercentage = 100
            leftTable.SetWidths({0.25F, 0.03F, 0.72F})
            leftTable.DefaultCell.Border = Rectangle.NO_BORDER

            leftTable.AddCell(CreateCell("To", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(invoiceTo, alignV:=Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Order ID", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(orderId, alignV:=Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Order Number", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(orderNumber, alignV:=Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Order Name", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(orderName, alignV:=Element.ALIGN_TOP))

            If orderType = "Builder" Then
                leftTable.AddCell(CreateCell("Estimator", isBold:=True, alignV:=Element.ALIGN_TOP))
                leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
                leftTable.AddCell(CreateCell(estimator, alignV:=Element.ALIGN_TOP))

                leftTable.AddCell(CreateCell("Supervisor", isBold:=True, alignV:=Element.ALIGN_TOP))
                leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
                leftTable.AddCell(CreateCell(supervisor, alignV:=Element.ALIGN_TOP))
            End If

            leftTable.AddCell(CreateCell("Issue Date", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":"))
            leftTable.AddCell(CreateCell(issueDate))

            Dim leftCell As New PdfPCell(leftTable)
            leftCell.Border = Rectangle.NO_BORDER
            companyTable.AddCell(leftCell)

            Dim rightTable As New PdfPTable(3)
            rightTable.WidthPercentage = 100
            rightTable.SetWidths({0.2F, 0.03F, 0.77F})
            rightTable.DefaultCell.Border = Rectangle.NO_BORDER

            rightTable.AddCell(CreateCell("From", isBold:=True, alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(invoiceFrom, alignV:=Element.ALIGN_TOP))

            If companyId = "2" Then
                rightTable.AddCell(CreateCell("ABN", isBold:=True, alignV:=Element.ALIGN_TOP))
                rightTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
                rightTable.AddCell(CreateCell("17 143 599 973", alignV:=Element.ALIGN_TOP))
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

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@HeaderId", headerId)
            }
            Dim detailData As DataTable = GetDataTableSP("sp_GetOrderDetailData_Quote", params)

            For i As Integer = 0 To detailData.Rows.Count - 1
                Dim itemId As String = detailData.Rows(i)("Id").ToString()
                Dim itemNumber As String = detailData.Rows(i)("Item").ToString()

                Dim designName As String = detailData.Rows(i)("DesignName").ToString()
                Dim blindName As String = detailData.Rows(i)("BlindName").ToString()
                Dim fabricColourId As String = detailData.Rows(i)("FabricColour").ToString()
                Dim width As String = detailData.Rows(i)("Width").ToString()
                Dim drop As String = detailData.Rows(i)("Height").ToString()
                Dim frameColour As String = detailData.Rows(i)("FrameColour").ToString()
                Dim doorCutOut As String = detailData.Rows(i)("DoorCutOut").ToString()

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

                If designName = "Aluminium Blind" OrElse designName = "Privacy Venetian" OrElse designName = "Venetian Blind" OrElse designName = "Skyline Shutter Express" Then
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, size, squareMetreText)
                End If

                If designName = "Design Shades" OrElse designName = "Roller Blind" OrElse designName = "Roman Blind" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                End If

                If designName = "Cellular Shades" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                    If blindName = "Day & Night" Then
                        If itemNumber = "1" Then
                            itemDescription = String.Format("{0} {1} {2} {3}", invoiceName.Replace("& Night ", ""), fabricColourName, size, squareMetreText)
                        End If
                        If itemNumber = "2" Then
                            itemDescription = String.Format("{0} {1} {2} {3}", invoiceName.Replace("Day & ", ""), fabricColourName, size, squareMetreText)
                        End If
                    End If
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
                        itemDescription = String.Format("{0} ({1}mm) {2}", invoiceName, width, linearMetreText)
                    End If
                End If

                If designName = "Linea Valance" Then
                    itemDescription = String.Format("{0} ({1}mm) {2}", invoiceName, width, linearMetreText)
                End If

                If designName = "Panel Glide" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                    If blindName = "Track Only" Then
                        itemDescription = String.Format("{0} ({1}mm) {2}", invoiceName, width, linearMetreText)
                    End If
                End If

                If designName = "Pelmet" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, width, linearMetreText)
                End If

                If designName = "Vertical" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                    If blindName = "Track Only" Then
                        itemDescription = String.Format("{0} ({1}mm) {2}", invoiceName, width, linearMetreText)
                    End If
                End If

                If designName = "Saphora Drape" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, fabricColourName, size, squareMetreText)
                End If

                If designName = "Skyline Shutter Ocean" Then
                    itemDescription = String.Format("{0} {1} {2}", invoiceName, size, squareMetreText)
                    If doorCutOut = "Yes" Then
                        itemDescription = String.Format("{0} - French Door Cut-Out {1} {2}", invoiceName, size, squareMetreText)
                    End If
                End If

                If designName = "Window" Then
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, frameColour, size, squareMetreText)
                End If

                If designName = "Door" Then
                    itemDescription = String.Format("{0} {1} {2} {3}", invoiceName, frameColour, size, squareMetreText)
                End If

                If Not (designName = "Door") AndAlso Not (designName = "Window") Then
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
                End If

                Dim finalCost As Decimal = GetItemData_Decimal("SELECT SUM(SellPrice) FROM OrderCostings WHERE HeaderId='" & headerId & "' AND ItemId='" & itemId & "' AND Number='" & itemNumber & "'")

                Dim finalCostText As String = finalCost.ToString("N2", enUS)
                If companyId = "3" OrElse companyId = "5" Then
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

            Return ms.ToArray()
        End Using
    End Function

    Public Function BindContentCustomer(headerId As String) As Byte()
        Using ms As New MemoryStream()
            Dim doc As New Document(PageSize.A4, 36, 36, 110, 180)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)

            Dim headerData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")

            Dim orderId As String = headerData("OrderId").ToString()
            Dim customerId As String = headerData("CustomerId").ToString()
            Dim customerName As String = headerData("CustomerName").ToString()
            Dim orderNumber As String = headerData("OrderNumber").ToString()
            Dim orderName As String = headerData("OrderName").ToString()
            Dim companyId As String = headerData("CompanyId").ToString()

            Dim reference As String = String.Format("{0} - {1}", orderNumber, orderName)

            Dim issueDate As String = DateTime.Now.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)

            Dim fullAddress As String = String.Empty
            Dim quoteEmail As String = String.Empty
            Dim quotePhone As String = String.Empty

            Dim quoteDiscount As Decimal = 0D
            Dim quoteCheckMeasure As Decimal = 0D
            Dim quoteInstallation As Decimal = 0D
            Dim quoteFreight As Decimal = 0D

            Dim orderQuote As DataRow = GetDataRow("SELECT * FROM OrderQuotes WHERE Id='" & headerId & "'")
            If orderQuote IsNot Nothing Then
                Dim address As String = orderQuote("Address").ToString().Trim()
                Dim suburb As String = orderQuote("Suburb").ToString().Trim()
                Dim city As String = orderQuote("City").ToString().Trim()
                Dim state As String = orderQuote("State").ToString().Trim()
                Dim postCode As String = orderQuote("PostCode").ToString().Trim()
                Dim country As String = orderQuote("Country").ToString().Trim()

                quoteEmail = orderQuote("Email").ToString()
                quotePhone = orderQuote("Phone").ToString()

                If Not IsDBNull(orderQuote("Discount")) Then
                    quoteDiscount = Math.Round(Convert.ToDecimal(orderQuote("Discount")), 2)
                End If
                If Not IsDBNull(orderQuote("CheckMeasure")) Then
                    quoteCheckMeasure = Math.Round(Convert.ToDecimal(orderQuote("CheckMeasure")), 2)
                End If
                If Not IsDBNull(orderQuote("Installation")) Then
                    quoteInstallation = Math.Round(Convert.ToDecimal(orderQuote("Installation")), 2)
                End If
                If Not IsDBNull(orderQuote("Freight")) Then
                    quoteFreight = Math.Round(Convert.ToDecimal(orderQuote("Freight")), 2)
                End If

                If Not String.IsNullOrWhiteSpace(address & suburb & city & state & postCode & country) Then
                    Dim lines As New List(Of String)

                    If Not String.IsNullOrEmpty(address) Then
                        lines.Add(address)
                    End If

                    Dim areaParts As New List(Of String)

                    If Not String.IsNullOrEmpty(suburb) Then areaParts.Add(suburb)
                    If companyId = "3" AndAlso Not String.IsNullOrEmpty(city) Then areaParts.Add(city)
                    If Not String.IsNullOrEmpty(state) Then areaParts.Add(state)
                    If Not String.IsNullOrEmpty(postCode) Then areaParts.Add(postCode)

                    If areaParts.Count > 0 Then lines.Add(String.Join(", ", areaParts))
                    If Not String.IsNullOrEmpty(country) Then lines.Add(country)

                    fullAddress = String.Join(vbCrLf, lines)
                End If
            End If

            Dim quoteFrom As String = customerName
            Dim quoteFrom_Email As String = String.Empty
            Dim quoteFrom_Phone As String = String.Empty

            Dim customerLogo As String = String.Empty
            Dim customerTerms As String = String.Empty

            Dim quoteData As DataRow = GetDataRow("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")
            If quoteData IsNot Nothing Then
                Dim address As String = quoteData("Address").ToString().Trim()
                Dim suburb As String = quoteData("Suburb").ToString().Trim()
                Dim state As String = quoteData("State").ToString().Trim()
                Dim postCode As String = quoteData("PostCode").ToString().Trim()
                Dim country As String = quoteData("Country").ToString().Trim()

                customerLogo = quoteData("Logo").ToString()
                customerTerms = quoteData("Terms").ToString()
                quoteFrom_Email = quoteData("Email").ToString()
                quoteFrom_Phone = quoteData("Phone").ToString()

                If Not String.IsNullOrWhiteSpace(address & suburb & state & postCode & country) Then
                    Dim lines As New List(Of String)

                    Dim addressLine As String = String.Empty
                    If Not String.IsNullOrEmpty(address) Then addressLine &= If(Not String.IsNullOrEmpty(addressLine), " ", "") & address

                    If Not String.IsNullOrEmpty(addressLine) Then lines.Add(addressLine)

                    Dim areaParts As New List(Of String)

                    If Not String.IsNullOrEmpty(suburb) Then areaParts.Add(suburb)
                    If Not String.IsNullOrEmpty(state) Then areaParts.Add(state)
                    If Not String.IsNullOrEmpty(postCode) Then areaParts.Add(postCode)

                    If areaParts.Count > 0 Then lines.Add(String.Join(", ", areaParts))

                    If Not String.IsNullOrEmpty(country) Then lines.Add(country)

                    quoteFrom &= vbCrLf & vbCrLf & String.Join(vbCrLf, lines)
                End If
            End If

            doc.Open()

            Dim companyTable As New PdfPTable(2)
            companyTable.WidthPercentage = 100
            companyTable.SetWidths(New Single() {0.6F, 0.4F})
            companyTable.DefaultCell.Border = Rectangle.NO_BORDER

            Dim leftTable As New PdfPTable(3)
            leftTable.WidthPercentage = 100
            leftTable.SetWidths({0.22F, 0.03F, 0.75F})
            leftTable.DefaultCell.Border = Rectangle.NO_BORDER

            leftTable.AddCell(CreateCell("Order #", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(orderId, alignV:=Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Reference", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(reference, alignV:=Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Address", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(fullAddress, alignV:=Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Email", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(quoteEmail, alignV:=Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Phone", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(quotePhone, alignV:=Element.ALIGN_TOP))

            leftTable.AddCell(CreateCell("Issue Date", isBold:=True, alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            leftTable.AddCell(CreateCell(issueDate, alignV:=Element.ALIGN_TOP))

            Dim leftCell As New PdfPCell(leftTable)
            leftCell.Border = Rectangle.NO_BORDER
            companyTable.AddCell(leftCell)

            Dim rightTable As New PdfPTable(3)
            rightTable.WidthPercentage = 100
            rightTable.SetWidths({0.17F, 0.03F, 0.8F})
            rightTable.DefaultCell.Border = Rectangle.NO_BORDER

            rightTable.AddCell(CreateCell("From", isBold:=True, alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(quoteFrom, alignV:=Element.ALIGN_TOP))

            rightTable.AddCell(CreateCell("Email", isBold:=True, alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(quoteFrom_Email))

            rightTable.AddCell(CreateCell("Phone", isBold:=True, alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(":", alignV:=Element.ALIGN_TOP))
            rightTable.AddCell(CreateCell(quoteFrom_Phone, alignV:=Element.ALIGN_TOP))

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

            Dim table As New PdfPTable(4)
            table.WidthPercentage = 100
            table.SetWidths(New Single() {0.15F, 0.6F, 0.05F, 0.2F})

            table.AddCell(CreateCellDetail("Location", isBold:=True))
            table.AddCell(CreateCellDetail("Description", isBold:=True))
            table.AddCell(CreateCellDetail("Qty", isBold:=True, alignH:=Element.ALIGN_CENTER))
            table.AddCell(CreateCellDetail("Unit Price", isBold:=True, alignH:=Element.ALIGN_RIGHT))

            Dim sumItemPrice As Decimal = 0D

            Dim detailData As DataTable = GetDataTable("SELECT OrderDetails.*, Products.Name AS ProductName, Designs.Name AS DesignName, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND Products.DesignId<>'16' ORDER BY CASE WHEN Designs.Type='Blinds' OR Designs.Type='Shutters' THEN 1 ELSE 2 END, OrderDetails.Id ASC")
            For i As Integer = 0 To detailData.Rows.Count - 1
                Dim itemId As String = detailData.Rows(i)("Id").ToString()
                Dim room As String = detailData.Rows(i)("Room").ToString()

                Dim markUp As Integer = detailData.Rows(i)("MarkUp")

                Dim productId As String = detailData.Rows(i)("ProductId").ToString()

                Dim fabricColourId As String = detailData.Rows(i)("FabricColourId").ToString()
                Dim fabricColourIdB As String = detailData.Rows(i)("FabricColourIdB").ToString()
                Dim fabricColourIdC As String = detailData.Rows(i)("FabricColourIdC").ToString()
                Dim fabricColourIdD As String = detailData.Rows(i)("FabricColourIdD").ToString()
                Dim fabricColourIdE As String = detailData.Rows(i)("FabricColourIdE").ToString()
                Dim fabricColourIdF As String = detailData.Rows(i)("FabricColourIdF").ToString()

                Dim printing As String = detailData.Rows(i)("Printing").ToString()
                Dim printingB As String = detailData.Rows(i)("PrintingB").ToString()
                Dim printingC As String = detailData.Rows(i)("PrintingC").ToString()
                Dim printingD As String = detailData.Rows(i)("PrintingD").ToString()
                Dim printingE As String = detailData.Rows(i)("PrintingE").ToString()
                Dim printingF As String = detailData.Rows(i)("PrintingF").ToString()

                Dim width As String = detailData.Rows(i)("Width").ToString()
                Dim widthB As String = detailData.Rows(i)("WidthB").ToString()
                Dim widthC As String = detailData.Rows(i)("WidthC").ToString()
                Dim widthD As String = detailData.Rows(i)("WidthD").ToString()
                Dim widthE As String = detailData.Rows(i)("WidthE").ToString()
                Dim widthF As String = detailData.Rows(i)("WidthF").ToString()

                Dim drop As String = detailData.Rows(i)("Drop").ToString()
                Dim dropB As String = detailData.Rows(i)("DropB").ToString()
                Dim dropC As String = detailData.Rows(i)("DropC").ToString()
                Dim dropD As String = detailData.Rows(i)("DropD").ToString()
                Dim dropE As String = detailData.Rows(i)("DropE").ToString()
                Dim dropF As String = detailData.Rows(i)("DropF").ToString()

                Dim layoutCode As String = detailData.Rows(i)("LayoutCode").ToString()
                Dim frameColour As String = detailData.Rows(i)("FrameColour").ToString()
                Dim doorCutOut As String = detailData.Rows(i)("DoorCutOut").ToString()

                Dim size As String = String.Format("({0}x{1})", width, drop)
                Dim sizeB As String = String.Format("({0}x{1})", widthB, dropB)
                Dim sizeC As String = String.Format("({0}x{1})", widthC, dropC)
                Dim sizeD As String = String.Format("({0}x{1})", widthD, dropD)
                Dim sizeE As String = String.Format("({0}x{1})", widthE, dropE)
                Dim sizeF As String = String.Format("({0}x{1})", widthF, dropF)

                Dim linearMetre As Decimal = 0D
                Dim linearMetreB As Decimal = 0D
                Dim linearMetreC As Decimal = 0D
                Dim linearMetreD As Decimal = 0D
                Dim linearMetreE As Decimal = 0D
                Dim linearMetreF As Decimal = 0D

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
                If Not IsDBNull(detailData.Rows(i)("LinearMetreE")) Then
                    linearMetreE = Math.Round(Convert.ToDecimal(detailData.Rows(i)("LinearMetreE")), 2)
                End If
                If Not IsDBNull(detailData.Rows(i)("LinearMetreF")) Then
                    linearMetreF = Math.Round(Convert.ToDecimal(detailData.Rows(i)("LinearMetreF")), 2)
                End If

                Dim squareMetre As Decimal = 0D
                Dim squareMetreB As Decimal = 0D
                Dim squareMetreC As Decimal = 0D
                Dim squareMetreD As Decimal = 0D
                Dim squareMetreE As Decimal = 0D
                Dim squareMetreF As Decimal = 0D

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
                If Not IsDBNull(detailData.Rows(i)("SquareMetreE")) Then
                    squareMetreE = Math.Round(Convert.ToDecimal(detailData.Rows(i)("SquareMetreE")), 2)
                End If
                If Not IsDBNull(detailData.Rows(i)("SquareMetreF")) Then
                    squareMetreF = Math.Round(Convert.ToDecimal(detailData.Rows(i)("SquareMetreF")), 2)
                End If

                Dim linearMetreText As String = String.Format("{0}lm", linearMetre.ToString("0.##", enUS))
                Dim linearMetreTextB As String = String.Format("{0}lm", linearMetreB.ToString("0.##", enUS))
                Dim linearMetreTextC As String = String.Format("{0}lm", linearMetreC.ToString("0.##", enUS))
                Dim linearMetreTextD As String = String.Format("{0}lm", linearMetreD.ToString("0.##", enUS))
                Dim linearMetreTextE As String = String.Format("{0}lm", linearMetreE.ToString("0.##", enUS))
                Dim linearMetreTextF As String = String.Format("{0}lm", linearMetreF.ToString("0.##", enUS))

                Dim squareMetreText As String = String.Format("{0}sqm", squareMetre.ToString("0.##", enUS))
                Dim squareMetreTextB As String = String.Format("{0}sqm", squareMetreB.ToString("0.##", enUS))
                Dim squareMetreTextC As String = String.Format("{0}sqm", squareMetreC.ToString("0.##", enUS))
                Dim squareMetreTextD As String = String.Format("{0}sqm", squareMetreD.ToString("0.##", enUS))
                Dim squareMetreTextE As String = String.Format("{0}sqm", squareMetreE.ToString("0.##", enUS))
                Dim squareMetreTextF As String = String.Format("{0}sqm", squareMetreF.ToString("0.##", enUS))

                Dim subType As String = detailData.Rows(i)("SubType").ToString()
                Dim totalItem As String = detailData.Rows(i)("TotalItems").ToString()

                Dim trackType As String = detailData.Rows(i)("TrackType").ToString()
                Dim trackTypeB As String = detailData.Rows(i)("TrackTypeB").ToString()

                Dim productName As String = detailData.Rows(i)("ProductName").ToString()
                Dim designName As String = detailData.Rows(i)("DesignName").ToString()
                Dim blindName As String = detailData.Rows(i)("BlindName").ToString()
                Dim itemDescription As String = productName

                If designName = "Aluminium Blind" Then
                    itemDescription = productName
                    If totalItem = 2 Then
                        itemDescription = "2 on 1 Headrail"
                        itemDescription &= vbCrLf
                        itemDescription &= productName
                    End If
                End If

                If designName = "Cellular Shades" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    Dim fabricColourNameB As String = GetFabricColourName(fabricColourIdB)

                    itemDescription = String.Format("{0} {1}", productName, fabricColourName)
                    If blindName = "Day & Night" Then
                        itemDescription = productName
                        itemDescription &= vbCrLf
                        itemDescription &= fabricColourName
                        itemDescription &= vbCrLf
                        itemDescription &= fabricColourNameB
                    End If
                End If

                If designName = "Curtain" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    Dim fabricColourNameB As String = GetFabricColourName(fabricColourIdB)

                    itemDescription = productName

                    If blindName = "Single Curtain & Track" Then
                        itemDescription = String.Format("{0} {1} {2}", productName, fabricColourName, trackType)
                    End If
                    If blindName = "Double Curtain & Track" Then
                        itemDescription = productName
                        itemDescription &= vbCrLf
                        itemDescription &= String.Format("1st Curtain : {0} | {1}", fabricColourName, trackType)
                        itemDescription &= vbCrLf
                        itemDescription &= String.Format("2nd Curtain : {0} | {1}", fabricColourNameB, trackTypeB)
                    End If
                    If blindName = "Curtain Only" Then
                        itemDescription = String.Format("{0} {1}", productName, fabricColourName)
                    End If
                    If blindName = "Track Only" Then
                        itemDescription = String.Format("{0} {1}", productName, trackType)
                    End If
                End If

                If designName = "Design Shades" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)

                    itemDescription = String.Format("{0} {1}", productName, fabricColourName)
                End If

                If designName = "Linea Valance" Then
                    itemDescription = productName
                End If

                If designName = "Panel Glide" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)

                    itemDescription = String.Format("{0} {1}", productName, fabricColourName)
                    If blindName = "Track Only" Then
                        itemDescription = productName
                    End If
                End If

                If designName = "Privacy Venetian" Then
                    itemDescription = productName
                End If

                If designName = "Pelmet" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)

                    itemDescription = String.Format("{0} {1}", productName, fabricColourName)
                End If

                If designName = "Roller Blind" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)
                    Dim fabricColourNameB As String = GetFabricColourName(fabricColourIdB)
                    Dim fabricColourNameC As String = GetFabricColourName(fabricColourIdC)
                    Dim fabricColourNameD As String = GetFabricColourName(fabricColourIdD)
                    Dim fabricColourNameE As String = GetFabricColourName(fabricColourIdE)
                    Dim fabricColourNameF As String = GetFabricColourName(fabricColourIdF)

                    itemDescription = String.Format("{0} {1}", productName, fabricColourName)
                    If Not String.IsNullOrEmpty(printing) Then
                        itemDescription &= vbCrLf
                        itemDescription &= "Printed Fabric"
                    End If

                    If blindName = "Dual Blinds" OrElse blindName = "Link 2 Blinds Dependent" OrElse blindName = "Link 2 Blinds Independent" Then
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
                    End If

                    If blindName = "Link 3 Blinds Dependent" OrElse blindName = "Link 3 Blinds Independent with Dependent" Then
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
                    End If

                    If blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 2 Blinds Independent" Then
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

                    itemDescription = String.Format("{0} {1}", productName, fabricColourName)
                End If

                If designName = "Venetian Blind" Then
                    itemDescription = productName
                    If totalItem = 2 Then
                        itemDescription = "2 on 1 Headrail"
                        itemDescription &= vbCrLf
                        itemDescription &= productName
                    End If
                End If

                If designName = "Vertical" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)

                    itemDescription = String.Format("{0} {1}", productName, fabricColourName)
                    If blindName = "Track Only" Then
                        itemDescription = productName
                    End If
                End If

                If designName = "Saphora Drape" Then
                    Dim fabricColourName As String = GetFabricColourName(fabricColourId)

                    itemDescription = String.Format("{0} {1}", productName, fabricColourName)
                End If

                If designName = "Window" Then
                    itemDescription = String.Format("{0} {1}", productName, frameColour)
                End If

                If designName = "Skyline Shutter Express" Then
                    itemDescription = productName
                End If

                If designName = "Skyline Shutter Ocean" Then
                    itemDescription = productName
                    If doorCutOut = "Yes" Then
                        itemDescription = String.Format("{0} - French Door Cut-Out", productName)
                    End If
                End If

                Dim itemCost As Decimal = GetItemData_Decimal("SELECT SellPrice FROM OrderCostings WHERE HeaderId='" & headerId & "' AND ItemId='" & itemId & "' AND Type='Final'")

                Dim itemCostMarkUp As Decimal = Math.Round(itemCost + (itemCost * markUp / 100), 2)

                sumItemPrice = sumItemPrice + itemCostMarkUp

                Dim unitPriceText As String = itemCostMarkUp.ToString("N2", enUS)
                If companyId = "3" OrElse companyId = "5" Then
                    unitPriceText = itemCostMarkUp.ToString("N2", idIDR)
                End If

                table.AddCell(CreateCellDetail(room))
                table.AddCell(CreateCellDetail(itemDescription))
                table.AddCell(CreateCellDetail(detailData.Rows(i)("Qty").ToString(), alignH:=Element.ALIGN_CENTER))
                table.AddCell(CreateCellDetail(unitPriceText, isBold:=True, alignH:=Element.ALIGN_RIGHT))
            Next

            Dim sumPrice As Decimal = GetItemData_Decimal("SELECT SUM(SellPrice) AS SumPrice FROM OrderCostings WHERE HeaderId='" & headerId & "' AND Type='Final'")

            Dim quoteDiscountText As String = String.Format("- {0}", quoteDiscount.ToString("N2", enUS))
            Dim quoteMeasureText As String = quoteCheckMeasure.ToString("N2", enUS)
            Dim quoteInstallationText As String = quoteInstallation.ToString("N2", enUS)
            Dim quoteFreightText As String = quoteFreight.ToString("N2", enUS)
            If companyId = "3" OrElse companyId = "5" Then
                quoteDiscountText = String.Format("- {0}", quoteDiscount.ToString("N2", idIDR))
                quoteMeasureText = quoteCheckMeasure.ToString("N2", idIDR)
                quoteInstallationText = quoteInstallation.ToString("N2", idIDR)
                quoteFreightText = quoteFreight.ToString("N2", idIDR)
            End If

            Dim gst As Decimal = (sumItemPrice - quoteDiscount + quoteCheckMeasure + quoteInstallation + quoteFreight) * 10 / 100
            If companyId = "3" OrElse companyId = "5" Then
                gst = (sumItemPrice - quoteDiscount + quoteCheckMeasure + quoteInstallation + quoteFreight) * 11 / 100
            End If
            Dim finaltotal As Decimal = sumItemPrice - quoteDiscount + quoteCheckMeasure + quoteInstallation + quoteFreight + gst

            Dim subTotalText As String = sumItemPrice.ToString("N2", enUS)
            Dim gstText As String = gst.ToString("N2", enUS)
            Dim finalTotalText As String = finaltotal.ToString("N2", enUS)
            If companyId = "3" OrElse companyId = "5" Then
                subTotalText = sumItemPrice.ToString("N2", idIDR)
                gstText = gst.ToString("N2", idIDR)
                finalTotalText = finaltotal.ToString("N2", idIDR)
            End If

            table.AddCell(CreateCellTotal(String.Empty))
            table.AddCell(CreateCellTotal("Sub Total"))
            table.AddCell(CreateCellTotal(String.Empty))
            table.AddCell(CreateCellTotal(subTotalText))

            If quoteDiscount > 0 Then
                table.AddCell(CreateCellTotal(String.Empty))
                table.AddCell(CreateCellTotal("Discount"))
                table.AddCell(CreateCellTotal(String.Empty))
                table.AddCell(CreateCellTotal(quoteDiscountText))
            End If

            If quoteCheckMeasure > 0 Then
                table.AddCell(CreateCellTotal(String.Empty))
                table.AddCell(CreateCellTotal("Check Measure"))
                table.AddCell(CreateCellTotal(String.Empty))
                table.AddCell(CreateCellTotal(quoteMeasureText))
            End If

            If quoteInstallation > 0 Then
                table.AddCell(CreateCellTotal(String.Empty))
                table.AddCell(CreateCellTotal("Installation"))
                table.AddCell(CreateCellTotal(String.Empty))
                table.AddCell(CreateCellTotal(quoteInstallationText))
            End If

            If quoteFreight > 0 Then
                table.AddCell(CreateCellTotal(String.Empty))
                table.AddCell(CreateCellTotal("Freight"))
                table.AddCell(CreateCellTotal(String.Empty))
                table.AddCell(CreateCellTotal(quoteFreightText))
            End If

            Dim gstTitle As String = "Total GST 10%"
            If companyId = "3" OrElse companyId = "5" Then
                gstTitle = "Total GST 11%"
            End If

            table.AddCell(CreateCellTotal(String.Empty))
            table.AddCell(CreateCellTotal(gstTitle))
            table.AddCell(CreateCellTotal(String.Empty))
            table.AddCell(CreateCellTotal(gstText))

            table.AddCell(CreateCellTotal(String.Empty))
            table.AddCell(CreateCellTotal("TOTAL", isBold:=True))
            table.AddCell(CreateCellTotal(String.Empty))
            table.AddCell(CreateCellTotal(finalTotalText, isBold:=True))

            writer.PageEvent = New QuoteCustomerEvents(companyId, customerLogo, finalTotalText, customerTerms)

            doc.Add(table)
            doc.Close()

            Return ms.ToArray()
        End Using
    End Function
End Class

Public Class QuoteEvents
    Inherits PdfPageEventHelper

    Private companyId As String
    Private totalPrice As String

    Public Sub New(companyId As String, dayRemaining As String, totalPrice As String)
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
        'If Me.companyId = "1" OrElse Me.companyId = "3" Then
        '    img.ScaleToFit(180, 100)
        'End If

        Dim imgCell As New PdfPCell(img)
        imgCell.Border = 0
        imgCell.HorizontalAlignment = Element.ALIGN_LEFT
        imgCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(imgCell)

        Dim mataUang As String = String.Empty
        If companyId = "2" Then mataUang = "AUD"
        If companyId = "3" Then mataUang = "RP"

        Dim finalPrice As String = String.Format("{0} {1}", Me.totalPrice, mataUang)

        Dim phrase As New Paragraph()
        phrase.Alignment = Element.ALIGN_RIGHT
        phrase.Add(New Chunk("QUOTE ORDER", New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)))
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

Public Class QuoteCustomerEvents
    Inherits PdfPageEventHelper

    Private companyId As String
    Private logoText As String
    Private totalPrice As String
    Private termsText As String

    Public Sub New(companyId As String, logo As String, totalPrice As String, terms As String)
        Me.companyId = companyId
        Me.totalPrice = totalPrice
        Me.logoText = logo
        Me.termsText = terms
    End Sub

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent

        Dim headerTable As New PdfPTable(2)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True
        headerTable.SetWidths(New Single() {0.6F, 0.4F})

        If String.IsNullOrEmpty(Me.logoText) Then
            Me.logoText = "yourlogo.png"
        End If

        Dim imagePath As String = HttpContext.Current.Server.MapPath(String.Format("~/assets/images/logo/customers/{0}", Me.logoText))

        If Not File.Exists(imagePath) Then
            imagePath = HttpContext.Current.Server.MapPath("~/assets/images/logo/jpmdirect.jpg")
        End If

        Dim img As Image = Image.GetInstance(imagePath)
        img.ScaleToFit(150, 60)

        Dim imgCell As New PdfPCell(img)
        imgCell.Border = 0
        imgCell.HorizontalAlignment = Element.ALIGN_LEFT
        imgCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(imgCell)

        Dim mataUang As String = String.Empty
        If companyId = "2" Then mataUang = "AUD"
        If companyId = "3" Then mataUang = "RP"

        Dim finalPrice As String = String.Format("{0} {1}", Me.totalPrice, mataUang)

        Dim phrase As New Paragraph()
        phrase.Alignment = Element.ALIGN_RIGHT
        phrase.Add(New Chunk("QUOTE ORDER", New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)))
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

        Dim termsPhrase As New Phrase()
        termsPhrase.Add(New Chunk("Terms & Conditions:" & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        termsPhrase.Add(New Chunk(vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10)))
        termsPhrase.Add(New Chunk(Me.termsText & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10)))

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