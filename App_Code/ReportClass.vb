Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports OfficeOpenXml
Imports OfficeOpenXml.Style

Public Class ReportClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetDataRow(thisString As String) As DataRow
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim thisTable As New DataTable()
                        thisAdapter.Fill(thisTable)
                        If thisTable.Rows.Count > 0 Then
                            Return thisTable.Rows(0)
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
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(spName, thisConn)
                    thisCmd.CommandType = CommandType.StoredProcedure
                    If params IsNot Nothing AndAlso params.Count > 0 Then
                        thisCmd.Parameters.AddRange(params.ToArray())
                    End If
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        thisAdapter.Fill(thisTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            thisTable = New DataTable()
        End Try
        Return thisTable
    End Function

    Protected Function GetItemData(thisString As String) As String
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

    Private Function EscapeCsv(value As String) As String
        Try
            If value.Contains(",") OrElse value.Contains("""") Then
                value = value.Replace("""", """""")
                Return """" & value & """"
            End If
            Return value
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Private Function CellData(text As String, Optional isBold As Boolean = False, Optional alignH As Integer = Element.ALIGN_LEFT) As PdfPCell
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

            cell.Border = Rectangle.BOX
            cell.BorderWidth = 0.5F

            cell.HorizontalAlignment = alignH
            cell.VerticalAlignment = Element.ALIGN_MIDDLE

            cell.PaddingTop = 4
            cell.PaddingBottom = 6
            cell.PaddingLeft = 4
            cell.PaddingRight = 4

            Return cell
        Catch ex As Exception
            Dim fallbackCell As New PdfPCell(New Phrase("Error"))
            fallbackCell.Border = Rectangle.BOX
            fallbackCell.BorderWidth = 0.5F

            Return fallbackCell
        End Try
    End Function


    ' START JOB ORDER
    Public Function JobOrderPDF(companyId As String, startDate As Date, endDate As Date) As Byte()
        Try
            Using ms As New MemoryStream()
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
                    New SqlParameter("@StartDate", startDate),
                    New SqlParameter("@EndDate", endDate)
                }

                Dim companyName As String = "ALL COMPANIES"
                If Not String.IsNullOrWhiteSpace(companyId) Then
                    companyName = Convert.ToString(GetItemData("SELECT Alias FROM Companys WHERE Id='" & companyId.Replace("'", "''") & "'"))
                    If String.IsNullOrWhiteSpace(companyName) Then companyName = "COMPANY " & companyId
                End If

                Dim pageTitle As String = "ORDER DETAIL REPORT"

                Dim doc As New Document(PageSize.A4.Rotate(), 25, 25, 125, 55)
                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)

                Dim pageEvent As New ReportDataOrderEvents() With {
                    .PageTitle = pageTitle,
                    .PageCompany = companyName,
                    .PageStartDate = startDate,
                    .PageEndDate = endDate
                }

                writer.PageEvent = pageEvent
                doc.Open()

                Dim table As New PdfPTable(6)

                table.WidthPercentage = 100
                table.SetWidths(New Single() {0.1F, 0.3F, 0.2F, 0.2F, 0.1F, 0.1F})

                table.HeaderRows = 1
                table.SplitLate = False
                table.KeepTogether = False

                table.AddCell(CellData("ORDER ID", isBold:=True))
                table.AddCell(CellData("CUSTOMER NAME", isBold:=True))
                table.AddCell(CellData("ORDER NUMBER", isBold:=True))
                table.AddCell(CellData("ORDER NAME", isBold:=True))
                table.AddCell(CellData("SUBMITTED", isBold:=True))
                table.AddCell(CellData("PRODUCTION", isBold:=True))

                Dim thisData As DataTable = GetDataTableSP("sp_ReportOrder_OrderHeaders", params)
                If thisData.Rows.Count > 0 Then
                    For i As Integer = 0 To thisData.Rows.Count - 1
                        Dim orderId As String = thisData.Rows(i)("OrderId").ToString()
                        Dim customerName As String = thisData.Rows(i)("CustomerName").ToString()
                        Dim orderNumber As String = thisData.Rows(i)("OrderNumber").ToString()
                        Dim orderName As String = thisData.Rows(i)("OrderName").ToString()

                        Dim submitDate As String = String.Empty
                        If Not IsDBNull(thisData.Rows(i)("SubmittedDate")) Then
                            submitDate = Convert.ToDateTime(thisData.Rows(i)("SubmittedDate")).ToString("dd MMM yyyy")
                        End If

                        Dim prodDate As String = String.Empty
                        If Not IsDBNull(thisData.Rows(i)("ProductionDate")) Then
                            prodDate = Convert.ToDateTime(thisData.Rows(i)("ProductionDate")).ToString("dd MMM yyyy")
                        End If

                        table.AddCell(CellData(orderId))
                        table.AddCell(CellData(customerName))
                        table.AddCell(CellData(orderNumber))
                        table.AddCell(CellData(orderName))
                        table.AddCell(CellData(submitDate))
                        table.AddCell(CellData(prodDate))
                    Next
                End If

                doc.Add(table)
                doc.Close()

                Return ms.ToArray()
            End Using
        Catch ex As Exception
            Return New Byte() {}
        End Try
    End Function

    Public Function JobOrderExcel(companyId As String, startDate As Date, endDate As Date) As Byte()
        Try
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
                New SqlParameter("@StartDate", startDate),
                New SqlParameter("@EndDate", endDate)
            }

            Dim dt As DataTable = GetDataTableSP("sp_ReportOrder_OrderHeaders", params)

            Using package As New ExcelPackage()
                Dim ws = package.Workbook.Worksheets.Add("JOB ORDER")

                ws.Cells(1, 1).Value = "ORDER ID"
                ws.Cells(1, 2).Value = "CUSTOMER NAME"
                ws.Cells(1, 3).Value = "ORDER NUMBER"
                ws.Cells(1, 4).Value = "ORDER NAME"
                ws.Cells(1, 5).Value = "SUBMITTED"
                ws.Cells(1, 6).Value = "PRODUCTION"

                Using rng = ws.Cells(1, 1, 1, 6)
                    rng.Style.Font.Bold = True
                End Using

                Dim row As Integer = 2
                For Each dr As DataRow In dt.Rows
                    ws.Cells(row, 1).Value = dr("OrderId").ToString()
                    ws.Cells(row, 2).Value = dr("CustomerName").ToString()
                    ws.Cells(row, 3).Value = dr("OrderNumber").ToString()
                    ws.Cells(row, 4).Value = dr("OrderName").ToString()

                    If Not IsDBNull(dr("SubmittedDate")) Then
                        ws.Cells(row, 5).Value = Convert.ToDateTime(dr("SubmittedDate"))
                        ws.Cells(row, 5).Style.Numberformat.Format = "dd MMM yyyy"
                    End If

                    If Not IsDBNull(dr("ProductionDate")) Then
                        ws.Cells(row, 6).Value = Convert.ToDateTime(dr("ProductionDate"))
                        ws.Cells(row, 6).Style.Numberformat.Format = "dd MMM yyyy"
                    End If

                    row += 1
                Next
                ws.Cells.AutoFitColumns()

                Return package.GetAsByteArray()
            End Using
        Catch ex As Exception
            Return New Byte() {}
        End Try
    End Function

    Public Function JobOrderCSV(companyId As String, startDate As Date, endDate As Date) As Byte()
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
                New SqlParameter("@StartDate", startDate),
                New SqlParameter("@EndDate", endDate)
            }

            Dim dt As DataTable = GetDataTableSP("sp_ReportOrder_OrderHeaders", params)

            Dim sb As New StringBuilder()

            sb.AppendLine("ORDER ID,CUSTOMER NAME,ORDER NUMBER,ORDER NAME,SUBMITTED,PRODUCTION")

            For Each dr As DataRow In dt.Rows
                Dim submitted As String = ""
                If Not IsDBNull(dr("SubmittedDate")) Then
                    submitted = Convert.ToDateTime(dr("SubmittedDate")).ToString("dd MMM yyyy")
                End If

                Dim prod As String = ""
                If Not IsDBNull(dr("ProductionDate")) Then
                    prod = Convert.ToDateTime(dr("ProductionDate")).ToString("dd MMM yyyy")
                End If

                Dim line As String = dr("OrderId").ToString() & "," & EscapeCsv(dr("CustomerName").ToString()) & "," & EscapeCsv(dr("OrderNumber").ToString()) & "," & EscapeCsv(dr("OrderName").ToString()) & "," & submitted & "," & prod

                sb.AppendLine(line)
            Next

            Return Encoding.UTF8.GetBytes(sb.ToString())
        Catch ex As Exception
            Return New Byte() {}
        End Try
    End Function

    ' END JOB ORDER

    ' START ORDER DETAILS
    Public Function OrderDetailPDF(companyId As String, designType As String, startDate As Date, endDate As Date) As Byte()
        Try
            Using ms As New MemoryStream()
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@CompanyId", If(String.IsNullOrWhiteSpace(companyId), CType(DBNull.Value, Object), companyId)),
                    New SqlParameter("@DesignId", If(String.IsNullOrWhiteSpace(designType), CType(DBNull.Value, Object), designType)),
                    New SqlParameter("@StartDate", startDate),
                    New SqlParameter("@EndDate", endDate)
                }

                Dim thisData As DataTable = GetDataTableSP("sp_ReportOrder_OrderDetails", params)

                Dim companyName As String = "ALL COMPANIES"
                If Not String.IsNullOrWhiteSpace(companyId) Then
                    companyName = Convert.ToString(GetItemData("SELECT Alias FROM Companys WHERE Id='" & companyId.Replace("'", "''") & "'"))
                    If String.IsNullOrWhiteSpace(companyName) Then companyName = "COMPANY " & companyId
                End If

                Dim pageTitle As String = "ORDER DETAIL REPORT"

                Dim doc As New Document(PageSize.A4.Rotate(), 25, 25, 125, 55)
                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)

                Dim pageEvent As New ReportDataOrderEvents() With {
                    .PageTitle = pageTitle,
                    .PageCompany = companyName,
                    .PageStartDate = startDate,
                    .PageEndDate = endDate
                }

                writer.PageEvent = pageEvent
                doc.Open()

                If thisData IsNot Nothing AndAlso thisData.Columns.Count > 0 Then
                    Dim columnCount As Integer = thisData.Columns.Count
                    Dim widths(columnCount - 1) As Single

                    For i As Integer = 0 To columnCount - 1
                        Dim columnName As String = thisData.Columns(i).ColumnName

                        Select Case columnName.ToUpperInvariant()
                            Case "ORDERID"
                                widths(i) = 1.5F
                            Case "CUSTOMERNAME"
                                widths(i) = 2.5F
                            Case "PRODUCTIONDATE"
                                widths(i) = 1.8F
                            Case Else
                                widths(i) = 1.0F
                        End Select
                    Next

                    Dim table As New PdfPTable(columnCount)
                    table.WidthPercentage = 100
                    table.SetWidths(widths)
                    table.HeaderRows = 1
                    table.SplitLate = False
                    table.KeepTogether = False
                    table.SpacingBefore = 5
                    table.SpacingAfter = 5

                    Dim headerFont As New Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)
                    Dim dataFont As New Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, New BaseColor(45, 55, 72))
                    Dim totalFont As New Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, New BaseColor(31, 41, 55))

                    For Each column As DataColumn In thisData.Columns
                        Dim columnTitle As String = column.ColumnName

                        Select Case column.ColumnName
                            Case "OrderId"
                                columnTitle = "Order ID"
                            Case "CustomerName"
                                columnTitle = "Customer Name"
                            Case "ProductionDate"
                                columnTitle = "Production Date"
                        End Select

                        Dim headerCell As New PdfPCell(New Phrase(columnTitle, headerFont))
                        headerCell.BackgroundColor = New BaseColor(36, 52, 71)
                        headerCell.HorizontalAlignment = Element.ALIGN_LEFT
                        headerCell.VerticalAlignment = Element.ALIGN_MIDDLE
                        headerCell.PaddingTop = 8
                        headerCell.PaddingBottom = 8
                        headerCell.PaddingLeft = 5
                        headerCell.PaddingRight = 5
                        headerCell.BorderColor = New BaseColor(220, 226, 232)
                        headerCell.BorderWidth = 0.5F
                        table.AddCell(headerCell)
                    Next

                    If thisData.Rows.Count > 0 Then
                        Dim rowIndex As Integer = 0

                        For Each row As DataRow In thisData.Rows
                            Dim isTotal As Boolean = thisData.Columns.Contains("OrderId") AndAlso String.Equals(Convert.ToString(row("OrderId")), "TOTAL", StringComparison.OrdinalIgnoreCase)
                            Dim rowBackground As BaseColor

                            If isTotal Then
                                rowBackground = New BaseColor(219, 234, 254)
                            ElseIf rowIndex Mod 2 = 0 Then
                                rowBackground = BaseColor.WHITE
                            Else
                                rowBackground = New BaseColor(245, 247, 250)
                            End If

                            For Each column As DataColumn In thisData.Columns
                                Dim cellValue As String = If(row.IsNull(column), String.Empty, Convert.ToString(row(column)))

                                If String.Equals(column.ColumnName, "ProductionDate", StringComparison.OrdinalIgnoreCase) AndAlso Not row.IsNull(column) Then
                                    Dim productionDate As DateTime
                                    If DateTime.TryParse(Convert.ToString(row(column)), productionDate) Then
                                        cellValue = productionDate.ToString("dd MMM yyyy")
                                    End If
                                End If

                                Dim cellFont As Font = If(isTotal, totalFont, dataFont)
                                Dim dataCell As New PdfPCell(New Phrase(cellValue, cellFont))
                                dataCell.BackgroundColor = rowBackground
                                dataCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                dataCell.PaddingTop = 6
                                dataCell.PaddingBottom = 6
                                dataCell.PaddingLeft = 5
                                dataCell.PaddingRight = 5
                                dataCell.BorderColor = New BaseColor(226, 232, 240)
                                dataCell.BorderWidth = 0.5F

                                If String.Equals(column.ColumnName, "OrderId", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(column.ColumnName, "ProductionDate", StringComparison.OrdinalIgnoreCase) Then
                                    dataCell.HorizontalAlignment = Element.ALIGN_CENTER
                                Else
                                    dataCell.HorizontalAlignment = Element.ALIGN_LEFT
                                End If

                                table.AddCell(dataCell)
                            Next

                            rowIndex += 1
                        Next
                    Else
                        Dim emptyCell As New PdfPCell(New Phrase("No data available", New Font(Font.FontFamily.HELVETICA, 9, Font.ITALIC, New BaseColor(107, 114, 128))))
                        emptyCell.Colspan = columnCount
                        emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                        emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                        emptyCell.Padding = 15
                        emptyCell.BorderColor = New BaseColor(226, 232, 240)
                        table.AddCell(emptyCell)
                    End If

                    doc.Add(table)
                Else
                    Dim emptyParagraph As New Paragraph("No data available", New Font(Font.FontFamily.HELVETICA, 10, Font.ITALIC, New BaseColor(107, 114, 128)))
                    emptyParagraph.Alignment = Element.ALIGN_CENTER
                    emptyParagraph.SpacingBefore = 20
                    doc.Add(emptyParagraph)
                End If

                doc.Close()
                Return ms.ToArray()
            End Using
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("OrderDetailPDF Error: " & ex.ToString())
            Return New Byte() {}
        End Try
    End Function

    ' END ORDER DETAILS

    ' START CUSTOMER (LIST)

    Public Function CustomerListPDF(companyId As String, roleName As String) As Byte()
        'Try
        '    Using ms As New MemoryStream()
        '        Dim params As New List(Of SqlParameter) From {
        '            New SqlParameter("@Active", 1),
        '            New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
        '            New SqlParameter("@RoleName", roleName)
        '        }

        '        Dim companyName As String = "ALL"
        '        If Not String.IsNullOrWhiteSpace(companyId) Then
        '            companyName = GetItemData("SELECT Alias FROM Companys WHERE Id='" & companyName & "'")
        '        End If

        '        Dim doc As New Document(PageSize.A4.Rotate(), 20, 20, 80, 50)
        '        Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)

        '        Dim pageEvent As New ReportDataOrderEvents() With {
        '            .PageTitle = "", .PageCompany = companyName, .PageStartDate = startDate, .PageEndDate = endDate
        '        }
        '        writer.PageEvent = pageEvent
        '        doc.Open()

        '        Dim table As New PdfPTable(8)

        '        table.WidthPercentage = 100
        '        table.SetWidths(New Single() {0.1F, 0.2F, 0.1F, 0.1F, 0.16F, 0.1F, 0.14F, 0.1F})

        '        table.HeaderRows = 1
        '        table.SplitLate = False
        '        table.KeepTogether = False

        '        table.AddCell(CellData("Debtor Code", isBold:=True))
        '        table.AddCell(CellData("Account Name", isBold:=True))
        '        table.AddCell(CellData("Company", isBold:=True))
        '        table.AddCell(CellData("Area", isBold:=True))
        '        table.AddCell(CellData("Sales Name", isBold:=True))
        '        table.AddCell(CellData("Pricing", isBold:=True))
        '        table.AddCell(CellData("Shutter Pricing", isBold:=True))
        '        table.AddCell(CellData("Door Pricing", isBold:=True))

        '        Dim thisData As DataTable = GetDataTableSP("sp_Customers_List", params)
        '        If thisData.Rows.Count > 0 Then
        '            For i As Integer = 0 To thisData.Rows.Count - 1
        '                table.AddCell(CellData(thisData.Rows(i)("DebtorCode").ToString()))
        '                table.AddCell(CellData(thisData.Rows(i)("Name").ToString()))
        '                table.AddCell(CellData(thisData.Rows(i)("CompanyDetailName").ToString()))
        '                table.AddCell(CellData(thisData.Rows(i)("Area").ToString()))
        '                table.AddCell(CellData(thisData.Rows(i)("OperatorName").ToString()))
        '                table.AddCell(CellData(thisData.Rows(i)("PriceBlind").ToString()))
        '                table.AddCell(CellData(thisData.Rows(i)("PriceShutter").ToString()))
        '                table.AddCell(CellData(thisData.Rows(i)("PriceDoor").ToString()))
        '            Next
        '        End If

        '        doc.Add(table)
        '        doc.Close()

        '        Return ms.ToArray()
        '    End Using
        'Catch ex As Exception
        '    Return New Byte() {}
        'End Try
    End Function

    Public Function CustomerListExcel(companyId As String, roleName As String) As Byte()
        Try
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Active", 1),
                New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
                New SqlParameter("@RoleName", roleName)
            }

            Dim dt As DataTable = GetDataTableSP("sp_Customers_List", params)

            Using package As New ExcelPackage()
                Dim ws = package.Workbook.Worksheets.Add("CUSTOMER LIST")

                ws.Cells(1, 1).Value = "DEBTOR CODE"
                ws.Cells(1, 2).Value = "ACCOUNT NAME"
                ws.Cells(1, 3).Value = "COMPANY"
                ws.Cells(1, 4).Value = "AREA"
                ws.Cells(1, 5).Value = "SALES NAME"
                ws.Cells(1, 6).Value = "PRICING"
                ws.Cells(1, 7).Value = "SHUTTER PRICING"
                ws.Cells(1, 8).Value = "DOOR PRICING"

                Using rng = ws.Cells(1, 1, 1, 7)
                    rng.Style.Font.Bold = True
                End Using

                Dim row As Integer = 2
                For Each dr As DataRow In dt.Rows
                    ws.Cells(row, 1).Value = dr("DebtorCode").ToString()
                    ws.Cells(row, 2).Value = dr("Name").ToString()
                    ws.Cells(row, 3).Value = dr("CompanyDetailName").ToString()
                    ws.Cells(row, 4).Value = dr("Area").ToString()
                    ws.Cells(row, 5).Value = dr("OperatorName").ToString()
                    ws.Cells(row, 6).Value = dr("PriceBlind").ToString()
                    ws.Cells(row, 7).Value = dr("PriceShutter").ToString()
                    ws.Cells(row, 8).Value = dr("PriceDoor").ToString()

                    row += 1
                Next
                ws.Cells.AutoFitColumns()

                Return package.GetAsByteArray()
            End Using
        Catch ex As Exception
            Return New Byte() {}
        End Try
    End Function

    Public Function CustomerListCSV(companyId As String, roleName As String) As Byte()
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Active", 1),
                New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
                New SqlParameter("@RoleName", roleName)
            }

            Dim dt As DataTable = GetDataTableSP("sp_Customers_List", params)

            Dim sb As New StringBuilder()

            sb.AppendLine("DEBTOR CODE,ACCOUNT NAME,COMPANY,AREA,SALES NAME,PRICING,SHUTTER PRICING,DOOR PRICING")

            For Each dr As DataRow In dt.Rows
                Dim line As String = dr("DebtorCode").ToString() & "," & EscapeCsv(dr("Name").ToString()) & "," & EscapeCsv(dr("CompanyDetailName").ToString()) & "," & EscapeCsv(dr("Area").ToString()) & "," & EscapeCsv(dr("OperatorName").ToString()) & "," & EscapeCsv(dr("PriceBlind").ToString()) & "," & EscapeCsv(dr("PriceShutter").ToString()) & "," & EscapeCsv(dr("PriceDoor").ToString())

                sb.AppendLine(line)
            Next

            Return Encoding.UTF8.GetBytes(sb.ToString())
        Catch ex As Exception
            Return New Byte() {}
        End Try
    End Function

    ' END CUSTOMER (LIST)

    ' START CUSTOMER (ORDER)
    Public Function CustomerOrderPDF(dt As DataTable, title As String) As Byte()
        'Try
        '    Using ms As New MemoryStream()
        '        Dim visibleColumns As New List(Of DataColumn)
        '        For Each col As DataColumn In dt.Columns
        '            Dim colName As String = col.ColumnName.Trim()
        '            If String.Equals(colName, "SortOrder", StringComparison.OrdinalIgnoreCase) Then Continue For
        '            If String.Equals(colName, "Total Order", StringComparison.OrdinalIgnoreCase) Then Continue For
        '            If String.Equals(colName, "No", StringComparison.OrdinalIgnoreCase) Then Continue For
        '            If String.IsNullOrWhiteSpace(colName) Then Continue For

        '            visibleColumns.Add(col)
        '        Next

        '        Dim companyName As String = "ALL"
        '        'If Not String.IsNullOrWhiteSpace(companyId) Then
        '        '    companyName = GetItemData("SELECT Alias FROM Companys WHERE Id='" & companyName & "'")
        '        'End If

        '        Dim doc As New Document(PageSize.A4.Rotate(), 20, 20, 80, 50)
        '        Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)

        '        Dim pageEvent As New ReportDataOrderEvents() With {
        '            .PageTitle = "", .PageCompany = companyName, .PageStartDate = , .PageEndDate = DBNull.Value
        '        }
        '        writer.PageEvent = pageEvent

        '        doc.Open()

        '        Dim table As New PdfPTable(visibleColumns.Count)

        '        table.WidthPercentage = 100
        '        table.HeaderRows = 1
        '        table.SplitLate = False
        '        table.KeepTogether = False

        '        Dim widths(visibleColumns.Count - 1) As Single

        '        For i As Integer = 0 To visibleColumns.Count - 1
        '            widths(i) = 1.0F
        '        Next

        '        table.SetWidths(widths)

        '        For Each col As DataColumn In visibleColumns
        '            table.AddCell(CellData(col.ColumnName.ToUpper(), isBold:=True))
        '        Next

        '        For Each dr As DataRow In dt.Rows
        '            For Each col As DataColumn In visibleColumns
        '                Dim value As String = ""

        '                If Not IsDBNull(dr(col.ColumnName)) Then
        '                    value = dr(col.ColumnName).ToString()
        '                End If
        '                table.AddCell(CellData(value))
        '            Next
        '        Next

        '        doc.Add(table)
        '        doc.Close()

        '        Return ms.ToArray()
        '    End Using
        'Catch ex As Exception
        '    Return New Byte() {}
        'End Try
    End Function

    Public Function CustomerOrderExcel(dt As DataTable, title As String) As Byte()
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial
        Try
            Dim visibleColumns As New List(Of DataColumn)

            For Each col As DataColumn In dt.Columns
                Dim colName As String = col.ColumnName.Trim()

                If String.Equals(colName, "SortOrder", StringComparison.OrdinalIgnoreCase) Then Continue For
                If String.Equals(colName, "Total Order", StringComparison.OrdinalIgnoreCase) Then Continue For
                If String.Equals(colName, "No", StringComparison.OrdinalIgnoreCase) Then Continue For
                If String.IsNullOrWhiteSpace(colName) Then Continue For

                visibleColumns.Add(col)

            Next

            Using package As New ExcelPackage()
                Dim ws As ExcelWorksheet = package.Workbook.Worksheets.Add("Report")
                ws.Cells(1, 1).Value = title

                ws.Cells(1, 1).Style.Font.Bold = True
                ws.Cells(1, 1).Style.Font.Size = 14
                ws.Cells(1, 1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center

                If visibleColumns.Count > 0 Then
                    ws.Cells(1, 1, 1, visibleColumns.Count).Merge = True
                End If

                '========================================
                ' HEADER
                '========================================
                Dim rowStart As Integer = 3
                Dim colIndex As Integer = 1

                For Each col As DataColumn In visibleColumns

                    ws.Cells(rowStart, colIndex).Value = col.ColumnName.ToUpper()

                    ws.Cells(rowStart, colIndex).Style.Font.Bold = True
                    ws.Cells(rowStart, colIndex).Style.Fill.PatternType = ExcelFillStyle.Solid
                    ws.Cells(rowStart, colIndex).Style.Border.BorderAround(ExcelBorderStyle.Thin)

                    colIndex += 1

                Next

                '========================================
                ' DATA
                '========================================
                Dim rowIndex As Integer = rowStart + 1

                For Each dr As DataRow In dt.Rows

                    colIndex = 1

                    For Each col As DataColumn In visibleColumns

                        Dim value As Object = dr(col.ColumnName)

                        If IsDBNull(value) Then

                            ws.Cells(rowIndex, colIndex).Value = ""

                        Else

                            Dim cleanText As String = CleanExcelText(value.ToString())

                            ws.Cells(rowIndex, colIndex).Value = cleanText

                        End If

                        ws.Cells(rowIndex, colIndex).Style.Border.BorderAround(ExcelBorderStyle.Thin)

                        colIndex += 1

                    Next

                    rowIndex += 1

                Next

                '========================================
                ' AUTOFIT
                '========================================
                If ws.Dimension IsNot Nothing Then
                    ws.Cells(ws.Dimension.Address).AutoFitColumns()
                End If

                '========================================
                ' RETURN FILE
                '========================================
                Return package.GetAsByteArray()

            End Using

        Catch ex As Exception
            Throw New Exception("CustomerOrderExcel Error : " & ex.Message)
        End Try
    End Function

    Private Function CleanExcelText(text As String) As String
        Try
            If String.IsNullOrEmpty(text) Then Return ""
            Return Regex.Replace(text, "[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", "")
        Catch ex As Exception
            Return ""
        End Try
    End Function
    ' END CUSTOMER (ORDER)
End Class

Public Class ReportDataOrderEvents
    Inherits PdfPageEventHelper

    Public PageTitle As String
    Public PageCompany As String
    Public PageStartDate As Date?
    Public PageEndDate As Date?

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent

        Dim titleFont As New Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, New BaseColor(36, 52, 71))
        Dim labelFont As New Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, New BaseColor(107, 114, 128))
        Dim valueFont As New Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, New BaseColor(31, 41, 55))
        Dim smallFont As New Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, New BaseColor(107, 114, 128))
        Dim footerFont As New Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, New BaseColor(107, 114, 128))

        Dim leftMargin As Single = document.LeftMargin
        Dim rightMargin As Single = document.PageSize.Width - document.RightMargin
        Dim contentWidth As Single = rightMargin - leftMargin

        ' HEADER TITLE
        Dim titleTable As New PdfPTable(2)
        titleTable.TotalWidth = contentWidth
        titleTable.LockedWidth = True
        titleTable.SetWidths(New Single() {0.68F, 0.32F})

        Dim titleCell As New PdfPCell(New Phrase(If(String.IsNullOrWhiteSpace(PageTitle), "REPORT", PageTitle), titleFont))
        titleCell.Border = Rectangle.NO_BORDER
        titleCell.HorizontalAlignment = Element.ALIGN_LEFT
        titleCell.VerticalAlignment = Element.ALIGN_MIDDLE
        titleCell.PaddingBottom = 5
        titleTable.AddCell(titleCell)

        Dim generatedCell As New PdfPCell(New Phrase("GENERATED ON" & vbLf & DateTime.Now.ToString("dd MMM yyyy HH:mm"), smallFont))
        generatedCell.Border = Rectangle.NO_BORDER
        generatedCell.HorizontalAlignment = Element.ALIGN_RIGHT
        generatedCell.VerticalAlignment = Element.ALIGN_MIDDLE
        generatedCell.PaddingBottom = 5
        titleTable.AddCell(generatedCell)

        titleTable.WriteSelectedRows(0, -1, leftMargin, document.PageSize.Height - 22, cb)

        ' HEADER DIVIDER
        Dim lineY As Single = document.PageSize.Height - 57

        cb.SaveState()
        cb.SetColorStroke(New BaseColor(37, 99, 235))
        cb.SetLineWidth(2.0F)
        cb.MoveTo(leftMargin, lineY)
        cb.LineTo(rightMargin, lineY)
        cb.Stroke()
        cb.RestoreState()

        ' COMPANY AND REPORT PERIOD
        Dim infoTable As New PdfPTable(2)
        infoTable.TotalWidth = contentWidth
        infoTable.LockedWidth = True
        infoTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim companyCell As New PdfPCell()
        companyCell.Border = Rectangle.NO_BORDER
        companyCell.HorizontalAlignment = Element.ALIGN_LEFT
        companyCell.VerticalAlignment = Element.ALIGN_TOP
        companyCell.PaddingTop = 9
        companyCell.PaddingBottom = 5
        companyCell.PaddingLeft = 0
        companyCell.PaddingRight = 10
        companyCell.AddElement(New Paragraph("COMPANY", labelFont))
        companyCell.AddElement(New Paragraph(If(String.IsNullOrWhiteSpace(PageCompany), "ALL COMPANIES", PageCompany), valueFont))
        infoTable.AddCell(companyCell)

        Dim periodText As String = "All Dates"

        If PageStartDate.HasValue AndAlso PageEndDate.HasValue Then
            periodText = PageStartDate.Value.ToString("dd MMM yyyy") & " - " & PageEndDate.Value.ToString("dd MMM yyyy")
        ElseIf PageStartDate.HasValue Then
            periodText = "From " & PageStartDate.Value.ToString("dd MMM yyyy")
        ElseIf PageEndDate.HasValue Then
            periodText = "Until " & PageEndDate.Value.ToString("dd MMM yyyy")
        End If

        Dim periodCell As New PdfPCell()
        periodCell.Border = Rectangle.NO_BORDER
        periodCell.HorizontalAlignment = Element.ALIGN_LEFT
        periodCell.VerticalAlignment = Element.ALIGN_TOP
        periodCell.PaddingTop = 9
        periodCell.PaddingBottom = 5
        periodCell.PaddingLeft = 0
        periodCell.PaddingRight = 10
        periodCell.AddElement(New Paragraph("REPORT PERIOD", labelFont))
        periodCell.AddElement(New Paragraph(periodText, valueFont))
        infoTable.AddCell(periodCell)

        infoTable.WriteSelectedRows(0, -1, leftMargin, lineY - 3, cb)

        ' FOOTER DIVIDER
        Dim footerY As Single = document.BottomMargin - 5

        cb.SaveState()
        cb.SetColorStroke(New BaseColor(226, 232, 240))
        cb.SetLineWidth(0.7F)
        cb.MoveTo(leftMargin, footerY + 15)
        cb.LineTo(rightMargin, footerY + 15)
        cb.Stroke()
        cb.RestoreState()

        ' FOOTER
        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = contentWidth
        footerTable.LockedWidth = True
        footerTable.SetWidths(New Single() {0.75F, 0.25F})

        Dim leftFooterCell As New PdfPCell(New Phrase("CONFIDENTIAL  |  All information within this report is private and confidential.", footerFont))
        leftFooterCell.Border = Rectangle.NO_BORDER
        leftFooterCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftFooterCell.VerticalAlignment = Element.ALIGN_MIDDLE
        leftFooterCell.PaddingTop = 5
        footerTable.AddCell(leftFooterCell)

        Dim rightFooterCell As New PdfPCell(New Phrase("Page " & writer.PageNumber, New Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, New BaseColor(36, 52, 71))))
        rightFooterCell.Border = Rectangle.NO_BORDER
        rightFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightFooterCell.VerticalAlignment = Element.ALIGN_MIDDLE
        rightFooterCell.PaddingTop = 5
        footerTable.AddCell(rightFooterCell)

        footerTable.WriteSelectedRows(0, -1, leftMargin, footerY + 10, cb)
    End Sub
End Class
