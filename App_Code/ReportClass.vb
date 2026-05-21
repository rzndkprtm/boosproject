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
        Try
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand(spName, conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    If params IsNot Nothing AndAlso params.Count > 0 Then
                        cmd.Parameters.AddRange(params.ToArray())
                    End If

                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            dt = New DataTable()
        End Try
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

    Private Function EscapeCsv(value As String) As String
        If value.Contains(",") OrElse value.Contains("""") Then
            value = value.Replace("""", """""")
            Return """" & value & """"
        End If

        Return value
    End Function

    Private Function CellDataOrder(text As String, Optional isBold As Boolean = False, Optional alignH As Integer = Element.ALIGN_LEFT) As PdfPCell
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

    Public Function DataOrderPDF(companyId As String, startDate As Date, endDate As Date) As Byte()
        Try
            Using ms As New MemoryStream()

                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
                    New SqlParameter("@StartDate", startDate),
                    New SqlParameter("@EndDate", endDate)
                }

                Dim doc As New Document(PageSize.A4.Rotate, 20, 20, 80, 50)
                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)

                writer.PageEvent = New ReportDataOrderEvents("REPORT DATA ORDER")
                doc.Open()

                Dim table As New PdfPTable(6)

                table.WidthPercentage = 100
                table.SetWidths(New Single() {0.1F, 0.3F, 0.2F, 0.2F, 0.1F, 0.1F})

                table.HeaderRows = 1
                table.SplitLate = False
                table.KeepTogether = False

                table.AddCell(CellDataOrder("ORDER ID", isBold:=True))
                table.AddCell(CellDataOrder("CUSTOMER NAME", isBold:=True))
                table.AddCell(CellDataOrder("ORDER NUMBER", isBold:=True))
                table.AddCell(CellDataOrder("ORDER NAME", isBold:=True))
                table.AddCell(CellDataOrder("SUBMITTED", isBold:=True))
                table.AddCell(CellDataOrder("PRODUCTION", isBold:=True))

                Dim thisData As DataTable = GetDataTableSP("sp_GetOrderHeadersForExport", params)
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

                        table.AddCell(CellDataOrder(orderId))
                        table.AddCell(CellDataOrder(customerName))
                        table.AddCell(CellDataOrder(orderNumber))
                        table.AddCell(CellDataOrder(orderName))
                        table.AddCell(CellDataOrder(submitDate))
                        table.AddCell(CellDataOrder(prodDate))
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

    Public Function DataOrderExcel(companyId As String, startDate As Date, endDate As Date) As Byte()
        Try
            Using ms As New MemoryStream()
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
                    New SqlParameter("@StartDate", startDate),
                    New SqlParameter("@EndDate", endDate)
                }

                Dim dt As DataTable = GetDataTableSP("sp_GetOrderHeadersForExport", params)

                Using package As New ExcelPackage()
                    Dim ws = package.Workbook.Worksheets.Add("Data Order")

                    ' HEADER
                    ws.Cells(1, 1).Value = "ORDER ID"
                    ws.Cells(1, 2).Value = "CUSTOMER NAME"
                    ws.Cells(1, 3).Value = "ORDER NUMBER"
                    ws.Cells(1, 4).Value = "ORDER NAME"
                    ws.Cells(1, 5).Value = "SUBMITTED"
                    ws.Cells(1, 6).Value = "PRODUCTION"

                    ' STYLE HEADER
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
                            ws.Cells(row, 5).Value = Convert.ToDateTime(dr("SubmittedDate")).ToString("dd MMM yyyy")
                        End If

                        If Not IsDBNull(dr("ProductionDate")) Then
                            ws.Cells(row, 6).Value = Convert.ToDateTime(dr("ProductionDate")).ToString("dd MMM yyyy")
                        End If

                        row += 1
                    Next

                    ws.Cells(ws.Dimension.Address).AutoFitColumns()

                    package.SaveAs(ms)
                End Using

                Return ms.ToArray()
            End Using
        Catch ex As Exception
            Return New Byte() {}
        End Try
    End Function

    Public Function DataOrderCSV(companyId As String, startDate As Date, endDate As Date) As Byte()
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
                New SqlParameter("@StartDate", startDate),
                New SqlParameter("@EndDate", endDate)
            }

            Dim dt As DataTable = GetDataTableSP("sp_GetOrderHeadersForExport", params)

            Dim sb As New StringBuilder()

            ' HEADER
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

    Public Function StatisticPDF(dt As DataTable, title As String) As Byte()
        Try
            Using ms As New MemoryStream()
                Dim visibleColumns As New List(Of DataColumn)
                For Each col As DataColumn In dt.Columns

                    Dim colName As String = col.ColumnName.Trim()
                    If String.Equals(colName, "SortOrder", StringComparison.OrdinalIgnoreCase) Then Continue For
                    If String.Equals(colName, "Total Order", StringComparison.OrdinalIgnoreCase) Then Continue For
                    If String.Equals(colName, "No", StringComparison.OrdinalIgnoreCase) Then Continue For
                    If String.IsNullOrWhiteSpace(colName) Then Continue For

                    visibleColumns.Add(col)
                Next

                Dim doc As New Document(PageSize.A4.Rotate, 20, 20, 80, 50)

                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)
                writer.PageEvent = New ReportDataOrderEvents(title)

                doc.Open()

                Dim table As New PdfPTable(visibleColumns.Count)

                table.WidthPercentage = 100
                table.HeaderRows = 1
                table.SplitLate = False
                table.KeepTogether = False

                Dim widths(visibleColumns.Count - 1) As Single

                For i As Integer = 0 To visibleColumns.Count - 1
                    widths(i) = 1.0F
                Next

                table.SetWidths(widths)

                For Each col As DataColumn In visibleColumns
                    table.AddCell(CellDataOrder(col.ColumnName.ToUpper(), isBold:=True))
                Next

                For Each dr As DataRow In dt.Rows
                    For Each col As DataColumn In visibleColumns
                        Dim value As String = ""

                        If Not IsDBNull(dr(col.ColumnName)) Then
                            value = dr(col.ColumnName).ToString()
                        End If
                        table.AddCell(CellDataOrder(value))
                    Next
                Next

                doc.Add(table)
                doc.Close()

                Return ms.ToArray()
            End Using
        Catch ex As Exception
            Return New Byte() {}
        End Try
    End Function

    Public Function StatisticExcel(dt As DataTable, title As String) As Byte()
        Dim visibleColumns As New List(Of DataColumn)

        For Each col As DataColumn In dt.Columns
            Dim colName As String = col.ColumnName.Trim()

            If String.Equals(colName, "SortOrder", StringComparison.OrdinalIgnoreCase) Then Continue For
            If String.Equals(colName, "Total Order", StringComparison.OrdinalIgnoreCase) Then Continue For
            If String.Equals(colName, "No", StringComparison.OrdinalIgnoreCase) Then Continue For
            If String.IsNullOrWhiteSpace(colName) Then Continue For

            visibleColumns.Add(col)
        Next

        Using package As New OfficeOpenXml.ExcelPackage()
            Dim ws = package.Workbook.Worksheets.Add("Report")

            ws.Cells(1, 1).Value = title
            ws.Cells(1, 1, 1, visibleColumns.Count).Merge = True
            ws.Cells(1, 1).Style.Font.Bold = True

            Dim rowStart As Integer = 3
            Dim colIndex As Integer = 1

            For Each col As DataColumn In visibleColumns
                ws.Cells(rowStart, colIndex).Value = col.ColumnName.ToUpper()
                ws.Cells(rowStart, colIndex).Style.Font.Bold = True
                colIndex += 1
            Next

            Dim rowIndex As Integer = rowStart + 1

            For Each dr As DataRow In dt.Rows
                colIndex = 1

                For Each col As DataColumn In visibleColumns
                    ws.Cells(rowIndex, colIndex).Value =
                    If(IsDBNull(dr(col.ColumnName)), "", dr(col.ColumnName))
                    colIndex += 1
                Next

                rowIndex += 1
            Next

            ws.Cells(ws.Dimension.Address).AutoFitColumns()

            Return package.GetAsByteArray()
        End Using
    End Function
End Class

Public Class ReportDataOrderEvents
    Inherits PdfPageEventHelper

    Private pageTitle As String

    Public Sub New(pageTitle As String)
        Me.pageTitle = pageTitle
    End Sub

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent
        Dim headerTable As New PdfPTable(2)

        headerTable.TotalWidth = document.PageSize.Width - 40
        headerTable.LockedWidth = True

        headerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim phrase As New Phrase()

        Dim chunk1 As New Chunk(
            pageTitle.ToUpper(),
            New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)
        )

        phrase.Add(chunk1)

        Dim leftHeaderCell As New PdfPCell(phrase)

        leftHeaderCell.Border = 0
        leftHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftHeaderCell.VerticalAlignment = Element.ALIGN_TOP

        headerTable.AddCell(leftHeaderCell)

        Dim rightHeaderCell As New PdfPCell(
            New Phrase(
                "Date : " & DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"),
                New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)
            )
        )

        rightHeaderCell.Border = 0
        rightHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightHeaderCell.VerticalAlignment = Element.ALIGN_BOTTOM

        headerTable.AddCell(rightHeaderCell)

        headerTable.WriteSelectedRows(0, -1, 20, document.PageSize.Height - 20, cb)

        Dim footerTable As New PdfPTable(2)

        footerTable.TotalWidth = document.PageSize.Width - 40
        footerTable.LockedWidth = True

        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(
            New Phrase(
                "All information within this report is private and confidential.",
                New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)
            )
        )

        leftFooterCell.Border = 0
        leftFooterCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM

        footerTable.AddCell(leftFooterCell)

        Dim rightFooterCell As New PdfPCell(
            New Phrase(
                "Page " & writer.PageNumber,
                New Font(Font.FontFamily.TIMES_ROMAN, 10)
            )
        )

        rightFooterCell.Border = 0
        rightFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM

        footerTable.AddCell(rightFooterCell)

        footerTable.WriteSelectedRows(0, -1, 20, document.PageSize.GetBottom(36), cb)
    End Sub
End Class
