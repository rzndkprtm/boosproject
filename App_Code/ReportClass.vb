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

    Public Sub Logs(data As Object())
        Try
            If data.Length = 4 Then
                Dim type As String = Convert.ToString(data(0))
                Dim dataId As String = Convert.ToString(data(1))
                Dim loginId As String = Convert.ToString(data(2))
                Dim description As String = Convert.ToString(data(3))

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Logs VALUES (NEWID(), @Type, @DataId, @ActionBy, GETDATE(), @Description)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Type", type)
                        thisCmd.Parameters.AddWithValue("@DataId", If(String.IsNullOrEmpty(dataId), CType(DBNull.Value, Object), dataId))
                        thisCmd.Parameters.AddWithValue("@ActionBy", loginId)
                        thisCmd.Parameters.AddWithValue("@Description", description)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    ' START ORDER HEADER
    Public Function OrderHeaderPDF(companyId As String, startDate As Date, endDate As Date) As Byte()
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

                Dim pageTitle As String = "ORDER REPORT"
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
                table.SetWidths(New Single() {0.09F, 0.25F, 0.16F, 0.24F, 0.13F, 0.13F})
                table.HeaderRows = 1
                table.SplitLate = False
                table.KeepTogether = False
                table.SpacingBefore = 5
                table.SpacingAfter = 5

                Dim headerFont As New Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)
                Dim headerBackground As New BaseColor(36, 52, 71)
                Dim borderColor As New BaseColor(226, 232, 240)

                Dim headers As String() = {"ORDER ID", "CUSTOMER NAME", "ORDER NUMBER", "ORDER NAME", "SUBMITTED", "PRODUCTION"}

                For Each header As String In headers
                    Dim cell As New PdfPCell(New Phrase(header, headerFont))
                    cell.BackgroundColor = headerBackground
                    cell.HorizontalAlignment = Element.ALIGN_CENTER
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE
                    cell.PaddingTop = 9
                    cell.PaddingBottom = 9
                    cell.PaddingLeft = 5
                    cell.PaddingRight = 5
                    cell.BorderColor = borderColor
                    cell.BorderWidth = 0.5F
                    cell.Border = Rectangle.BOX
                    table.AddCell(cell)
                Next

                Dim thisData As DataTable = GetDataTableSP("sp_ReportOrder_OrderHeaders", params)

                Dim bodyFont As New Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, New BaseColor(51, 65, 85))
                Dim alternateFont As New Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, New BaseColor(51, 65, 85))
                Dim alternateBackground As New BaseColor(248, 250, 252)
                Dim whiteBackground As BaseColor = BaseColor.WHITE

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

                        Dim rowBackground As BaseColor = If(i Mod 2 = 0, whiteBackground, alternateBackground)

                        Dim values As String() = {orderId, customerName, orderNumber, orderName, submitDate, prodDate}

                        For col As Integer = 0 To values.Length - 1
                            Dim cell As New PdfPCell(New Phrase(If(String.IsNullOrWhiteSpace(values(col)), "-", values(col)), bodyFont))
                            cell.BackgroundColor = rowBackground
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE
                            cell.PaddingTop = 7
                            cell.PaddingBottom = 7
                            cell.PaddingLeft = 6
                            cell.PaddingRight = 6
                            cell.BorderColor = borderColor
                            cell.BorderWidth = 0.5F
                            cell.Border = Rectangle.BOX

                            If col = 0 OrElse col = 2 OrElse col = 4 OrElse col = 5 Then
                                cell.HorizontalAlignment = Element.ALIGN_CENTER
                            Else
                                cell.HorizontalAlignment = Element.ALIGN_LEFT
                            End If

                            table.AddCell(cell)
                        Next
                    Next
                Else
                    Dim emptyCell As New PdfPCell(New Phrase("No order records found for the selected period.", New Font(Font.FontFamily.HELVETICA, 9, Font.ITALIC, New BaseColor(107, 114, 128))))
                    emptyCell.Colspan = 6
                    emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                    emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                    emptyCell.PaddingTop = 18
                    emptyCell.PaddingBottom = 18
                    emptyCell.BackgroundColor = New BaseColor(248, 250, 252)
                    emptyCell.BorderColor = borderColor
                    emptyCell.BorderWidth = 0.5F
                    table.AddCell(emptyCell)
                End If

                doc.Add(table)
                doc.Close()

                Return ms.ToArray()
            End Using
        Catch ex As Exception
            Return New Byte() {}
        End Try
    End Function

    Public Function OrderHeaderExcel(companyId As String, startDate As Date, endDate As Date) As Byte()
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

    Public Function OrderHeaderCSV(companyId As String, startDate As Date, endDate As Date) As Byte()
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

    ' END ORDER HEADER


    ' START ORDER DETAILS

    Public Function OrderDetailPDF(companyId As String, customerId As String, designType As String, startDate As Date, endDate As Date) As Byte()
        Try
            Using ms As New MemoryStream()
                Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@CompanyId", If(String.IsNullOrWhiteSpace(companyId), CType(DBNull.Value, Object), companyId)),
                New SqlParameter("@DesignId", If(String.IsNullOrWhiteSpace(designType), CType(DBNull.Value, Object), designType)),
                New SqlParameter("@CustomerId", If(String.IsNullOrWhiteSpace(customerId), CType(DBNull.Value, Object), customerId)),
                New SqlParameter("@StartDate", startDate),
                New SqlParameter("@EndDate", endDate)
            }

                Dim thisData As DataTable = GetDataTableSP("sp_ReportOrder_OrderDetails", params)

                Dim companyName As String = "ALL COMPANIES"
                If Not String.IsNullOrWhiteSpace(companyId) Then
                    companyName = Convert.ToString(GetItemData("SELECT Alias FROM Companys WHERE Id='" & companyId.Replace("'", "''") & "'"))
                    If String.IsNullOrWhiteSpace(companyName) Then companyName = "COMPANY " & companyId
                End If

                Dim pageTitle As String = "ORDER (PRODUCT) REPORT"
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
                    Dim identityColumns As New List(Of String) From {"OrderId", "CustomerName", "ProductionDate"}
                    Dim productColumns As New List(Of DataColumn)

                    For Each column As DataColumn In thisData.Columns
                        If Not identityColumns.Contains(column.ColumnName, StringComparer.OrdinalIgnoreCase) Then
                            productColumns.Add(column)
                        End If
                    Next

                    Dim maxProductsPerPage As Integer = 6
                    Dim productGroups As New List(Of List(Of DataColumn))

                    For i As Integer = 0 To productColumns.Count - 1 Step maxProductsPerPage
                        productGroups.Add(productColumns.Skip(i).Take(maxProductsPerPage).ToList())
                    Next

                    If productGroups.Count = 0 Then
                        productGroups.Add(New List(Of DataColumn))
                    End If

                    Dim headerFont As New Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE)
                    Dim dataFont As New Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, New BaseColor(45, 55, 72))
                    Dim totalFont As New Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, New BaseColor(31, 41, 55))

                    For groupIndex As Integer = 0 To productGroups.Count - 1
                        If groupIndex > 0 Then
                            doc.NewPage()
                        End If

                        Dim currentColumns As New List(Of DataColumn)

                        For Each identityName As String In identityColumns
                            If thisData.Columns.Contains(identityName) Then
                                currentColumns.Add(thisData.Columns(identityName))
                            End If
                        Next

                        currentColumns.AddRange(productGroups(groupIndex))

                        Dim columnCount As Integer = currentColumns.Count
                        If columnCount = 0 Then Continue For

                        Dim widths(columnCount - 1) As Single

                        For i As Integer = 0 To columnCount - 1
                            Select Case currentColumns(i).ColumnName.ToUpperInvariant()
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

                        For Each column As DataColumn In currentColumns
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
                            headerCell.HorizontalAlignment = Element.ALIGN_CENTER
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

                                For Each column As DataColumn In currentColumns
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

                                    If String.Equals(column.ColumnName, "OrderId", StringComparison.OrdinalIgnoreCase) OrElse
                                   String.Equals(column.ColumnName, "ProductionDate", StringComparison.OrdinalIgnoreCase) OrElse
                                   Not identityColumns.Contains(column.ColumnName, StringComparer.OrdinalIgnoreCase) Then
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
                    Next
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

    Public Function OrderDetailExcel(companyId As String, customerId As String, designType As String, startDate As Date, endDate As Date) As Byte()
        Try
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@CompanyId", If(String.IsNullOrWhiteSpace(companyId), CType(DBNull.Value, Object), companyId)),
                New SqlParameter("@DesignId", If(String.IsNullOrWhiteSpace(designType), CType(DBNull.Value, Object), designType)),
                New SqlParameter("@CustomerId", If(String.IsNullOrWhiteSpace(customerId), CType(DBNull.Value, Object), customerId)),
                New SqlParameter("@StartDate", startDate),
                New SqlParameter("@EndDate", endDate)
            }

            Dim dt As DataTable = GetDataTableSP("sp_ReportOrder_OrderDetails", params)

            Using package As New ExcelPackage()
                Dim ws = package.Workbook.Worksheets.Add("ORDER PRODUCT")

                If dt Is Nothing OrElse dt.Columns.Count = 0 Then
                    ws.Cells(1, 1).Value = "No data available"
                    Return package.GetAsByteArray()
                End If

                Dim identityColumns As New List(Of String) From {"OrderId", "CustomerName", "ProductionDate"}
                Dim columnCount As Integer = dt.Columns.Count

                ' HEADER
                For col As Integer = 0 To columnCount - 1
                    Dim columnName As String = dt.Columns(col).ColumnName

                    Select Case columnName
                        Case "OrderId"
                            ws.Cells(1, col + 1).Value = "ORDER ID"
                        Case "CustomerName"
                            ws.Cells(1, col + 1).Value = "CUSTOMER NAME"
                        Case "ProductionDate"
                            ws.Cells(1, col + 1).Value = "PRODUCTION DATE"
                        Case Else
                            ws.Cells(1, col + 1).Value = columnName.ToUpperInvariant()
                    End Select
                Next

                ' HEADER STYLE
                Using rng = ws.Cells(1, 1, 1, columnCount)
                    rng.Style.Font.Bold = True
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White)
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(36, 52, 71))
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin
                    rng.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(220, 226, 232))
                End Using

                ws.Row(1).Height = 25

                ' DATA
                Dim rowIndex As Integer = 2

                For Each dr As DataRow In dt.Rows
                    Dim isTotal As Boolean = dt.Columns.Contains("OrderId") AndAlso String.Equals(Convert.ToString(dr("OrderId")), "TOTAL", StringComparison.OrdinalIgnoreCase)
                    For col As Integer = 0 To columnCount - 1
                        Dim column As DataColumn = dt.Columns(col)
                        Dim cell = ws.Cells(rowIndex, col + 1)

                        If dr.IsNull(column) Then
                            cell.Value = Nothing
                        ElseIf String.Equals(column.ColumnName, "ProductionDate", StringComparison.OrdinalIgnoreCase) Then
                            Dim productionDate As DateTime
                            If DateTime.TryParse(Convert.ToString(dr(column)), productionDate) Then
                                cell.Value = productionDate
                                cell.Style.Numberformat.Format = "dd MMM yyyy"
                            Else
                                cell.Value = Convert.ToString(dr(column))
                            End If
                        Else
                            cell.Value = dr(column)
                        End If

                        ' ALIGNMENT
                        If String.Equals(column.ColumnName, "CustomerName", StringComparison.OrdinalIgnoreCase) Then
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left
                        Else
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center
                        End If

                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center

                        ' BORDER
                        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin
                        cell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(226, 232, 240))

                        ' ALTERNATING ROW COLORS
                        If isTotal Then
                            cell.Style.Font.Bold = True
                            cell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(31, 41, 55))
                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid
                            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(219, 234, 254))
                        ElseIf rowIndex Mod 2 = 1 Then
                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid
                            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(245, 247, 250))
                        End If
                    Next

                    rowIndex += 1
                Next

                ' COLUMN WIDTHS
                For col As Integer = 0 To columnCount - 1
                    Select Case dt.Columns(col).ColumnName.ToUpperInvariant()
                        Case "ORDERID"
                            ws.Column(col + 1).Width = 15
                        Case "CUSTOMERNAME"
                            ws.Column(col + 1).Width = 30
                        Case "PRODUCTIONDATE"
                            ws.Column(col + 1).Width = 20
                        Case Else
                            ws.Column(col + 1).Width = Math.Max(12, Math.Min(25, dt.Columns(col).ColumnName.Length + 3))
                    End Select
                Next

                ' FREEZE HEADER
                ws.View.FreezePanes(2, 1)

                ' FILTER
                ws.Cells(1, 1, Math.Max(1, rowIndex - 1), columnCount).AutoFilter = True

                ' PRINT SETTINGS
                ws.PrinterSettings.Orientation = eOrientation.Landscape
                ws.PrinterSettings.FitToPage = True
                ws.PrinterSettings.FitToWidth = 1
                ws.PrinterSettings.FitToHeight = 0

                Return package.GetAsByteArray()
            End Using
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("OrderDetailExcel Error: " & ex.ToString())
            Return New Byte() {}
        End Try
    End Function

    Public Function OrderDetailCSV(companyId As String, customerId As String, designType As String, startDate As Date, endDate As Date) As Byte()
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@CompanyId", If(String.IsNullOrWhiteSpace(companyId), CType(DBNull.Value, Object), companyId)),
                New SqlParameter("@DesignId", If(String.IsNullOrWhiteSpace(designType), CType(DBNull.Value, Object), designType)),
                New SqlParameter("@CustomerId", If(String.IsNullOrWhiteSpace(customerId), CType(DBNull.Value, Object), customerId)),
                New SqlParameter("@StartDate", startDate),
                New SqlParameter("@EndDate", endDate)
            }

            Dim dt As DataTable = GetDataTableSP("sp_ReportOrder_OrderDetails", params)
            Dim sb As New StringBuilder()

            If dt Is Nothing OrElse dt.Columns.Count = 0 Then
                Return Encoding.UTF8.GetBytes("")
            End If

            ' HEADER
            Dim headers As New List(Of String)

            For Each column As DataColumn In dt.Columns
                Select Case column.ColumnName
                    Case "OrderId"
                        headers.Add("ORDER ID")
                    Case "CustomerName"
                        headers.Add("CUSTOMER NAME")
                    Case "ProductionDate"
                        headers.Add("PRODUCTION DATE")
                    Case Else
                        headers.Add(column.ColumnName.ToUpperInvariant())
                End Select
            Next

            sb.AppendLine(String.Join(",", headers.Select(Function(h) EscapeCsv(h))))

            ' DATA
            For Each dr As DataRow In dt.Rows
                Dim values As New List(Of String)

                For Each column As DataColumn In dt.Columns
                    Dim cellValue As String = ""

                    If Not dr.IsNull(column) Then
                        If String.Equals(column.ColumnName, "ProductionDate", StringComparison.OrdinalIgnoreCase) Then
                            Dim productionDate As DateTime
                            If DateTime.TryParse(Convert.ToString(dr(column)), productionDate) Then
                                cellValue = productionDate.ToString("dd MMM yyyy")
                            Else
                                cellValue = Convert.ToString(dr(column))
                            End If
                        Else
                            cellValue = Convert.ToString(dr(column))
                        End If
                    End If

                    values.Add(EscapeCsv(cellValue))
                Next

                sb.AppendLine(String.Join(",", values))
            Next

            Return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray()
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("OrderDetailCSV Error: " & ex.ToString())
            Return New Byte() {}
        End Try
    End Function
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
