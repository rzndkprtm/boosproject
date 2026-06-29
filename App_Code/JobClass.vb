Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class JobClass
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

    Public Function GetDataRowSP(spName As String, params As List(Of SqlParameter)) As DataRow
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(spName, thisConn)
                    thisCmd.CommandType = CommandType.StoredProcedure
                    thisCmd.Parameters.AddRange(params.ToArray())
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim thisTable As New DataTable()
                        thisAdapter.Fill(thisTable)
                        If thisTable.Rows.Count > 0 Then
                            Return thisTable.Rows(0)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return Nothing
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

    Public Function GetItemData_Integer(thisString As String) As Integer
        Dim result As Integer = 0
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
            result = 0
        End Try
        Return result
    End Function

    Public Function GetItemData_Decimal(thisString As String) As Decimal
        Dim result As Double = 0D
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

    Public Function GetItemData_Boolean(thisString As String) As Boolean
        Dim result As Boolean = False
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
            result = False
        End Try
        Return result
    End Function

    Public Function GetNewJobSheetId() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT TOP 1 Id FROM JobSheets ORDER BY Id DESC", thisConn)
                    thisConn.Open()
                    Dim lastId As Object = thisCmd.ExecuteScalar()
                    If lastId IsNot Nothing AndAlso Not IsDBNull(lastId) Then
                        result = (CInt(lastId) + 1).ToString()
                    Else
                        result = "1"
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetNewJobSheetDetailId() As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT TOP 1 Id FROM JobSheetDetails ORDER BY Id DESC", thisConn)
                    thisConn.Open()
                    Dim lastId As Object = thisCmd.ExecuteScalar()
                    If lastId IsNot Nothing AndAlso Not IsDBNull(lastId) Then
                        result = (CInt(lastId) + 1).ToString()
                    Else
                        result = "1"
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetNewSortOrder(jobSheetId As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("SELECT TOP 1 SortOrder FROM JobSheetDetails WHERE JobSheetId='" & jobSheetId & "' ORDER BY Id DESC", thisConn)
                    thisConn.Open()
                    Dim lastId As Object = thisCmd.ExecuteScalar()
                    If lastId IsNot Nothing AndAlso Not IsDBNull(lastId) Then
                        result = (CInt(lastId) + 1).ToString()
                    Else
                        result = "1"
                    End If
                End Using
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
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

    Public Function BindContent(headerId As String) As Byte()
        Using ms As New MemoryStream()

            Dim headerData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Companys.Name AS CompanyName, OrderJobs.JobNumber, OrderJobs.WorkOrder, OrderJobs.JobNote, OrderJobs.CreatedDate AS ConvertDate, Logins.FullName AS ConvertBy FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId = Customers.Id LEFT JOIN Companys ON Customers.CompanyId = Companys.Id LEFT JOIN OrderJobs ON OrderHeaders.OrderJobId = OrderJobs.Id LEFT JOIN Logins ON OrderJobs.CreatedBy = Logins.Id WHERE OrderHeaders.Id='" & headerId & "'")

            Dim customerName As String = headerData("CustomerName").ToString()
            Dim companyName As String = headerData("CompanyName").ToString()
            Dim orderId As String = headerData("OrderId").ToString()
            Dim orderJobId As String = headerData("OrderJobId").ToString()
            Dim orderNumber As String = headerData("OrderNumber").ToString()
            Dim orderName As String = headerData("OrderName").ToString()
            Dim orderNote As String = headerData("OrderNote").ToString()
            Dim jobNumber As String = headerData("JobNumber").ToString()
            Dim workOrder As String = headerData("WorkOrder").ToString()
            Dim jobNote As String = headerData("JobNote").ToString()
            Dim convertBy As String = headerData("ConvertBy").ToString()
            Dim convertDate As String = Convert.ToDateTime(headerData("ConvertDate")).ToString("dd MMM yyyy")

            Dim doc As New Document(PageSize.A4, 20, 20, 120, 50)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)

            Dim pageEvent As New JobEvents() With {
                .PageOrderId = orderId,
                .PageCustomerName = customerName,
                .PageOrderNumber = orderNumber,
                .PageOrderName = orderName,
                .pageCompany = companyName,
                .pageJobNumber = jobNumber,
                .pageWorkOrder = workOrder,
                .pageJobNote = jobNote,
                .pageConvertBy = convertBy,
                .pageConvertDate = convertDate
            }
            writer.PageEvent = pageEvent

            doc.Open()

            Dim jobSheetData As DataTable = GetDataTable("SELECT JobSheetId FROM OrderJobDetails WHERE OrderJobId='" & orderJobId & "' GROUP BY JobSheetId ORDER BY JobSheetId ASC")
            If jobSheetData.Rows.Count > 0 Then
                For i As Integer = 0 To jobSheetData.Rows.Count - 1
                    If i > 0 Then
                        doc.NewPage()
                    End If

                    Dim jobSheetId As String = jobSheetData.Rows(i)("JobSheetId").ToString()
                    Dim jobSheetName As String = GetItemData("SELECT Name FROM JobSheets WHERE Id='" & jobSheetId & "'")
                    Dim jobSheetAlias As String = GetItemData("SELECT Alias FROM JobSheets WHERE Id='" & jobSheetId & "'")
                    pageEvent.pageDesignType = jobSheetName
                    pageEvent.PageTitle = jobSheetAlias

                    Dim jobSheetDetail As DataTable = GetDataTable("SELECT OrderJobDetails.ItemNumber, JobSheetDetails.Name AS JobSheetDetailName, OrderJobDetails.JobValue FROM OrderJobDetails INNER JOIN JobSheets ON OrderJobDetails.JobSheetId = JobSheets.Id INNER JOIN JobSheetDetails ON OrderJobDetails.JobSheetDetailId = JobSheetDetails.Id WHERE OrderJobDetails.OrderJobId = '" & orderJobId & "' AND OrderJobDetails.JobSheetId = '" & jobSheetId & "' ORDER BY OrderJobDetails.ItemId ASC, OrderJobDetails.ItemNumber ASC, JobSheetDetails.SortOrder ASC")
                    If jobSheetDetail.Rows.Count = 0 Then
                        Continue For
                    End If

                    Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 5, Font.BOLD)
                    Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 5)

                    Dim headers As List(Of String) = jobSheetDetail.AsEnumerable().Select(Function(r) r("JobSheetDetailName").ToString()).Distinct().ToList()
                    Dim itemNumbers As List(Of Integer) = jobSheetDetail.AsEnumerable().Select(Function(r) Convert.ToInt32(r("ItemNumber"))).Distinct().OrderBy(Function(x) x).ToList()
                    Dim data As New Dictionary(Of Integer, Dictionary(Of String, String))

                    For Each row As DataRow In jobSheetDetail.Rows
                        Dim itemNo As Integer = Convert.ToInt32(row("ItemNumber"))
                        Dim header As String = row("JobSheetDetailName").ToString()
                        Dim value As String = row("JobValue").ToString()
                        If Not data.ContainsKey(itemNo) Then
                            data(itemNo) = New Dictionary(Of String, String)
                        End If
                        data(itemNo)(header) = value
                    Next

                    For startCol As Integer = 0 To itemNumbers.Count - 1 Step 6
                        Dim table As New PdfPTable(7)
                        table.WidthPercentage = 100
                        Dim itemHeader As New PdfPCell(New Phrase("Item :", fontHeader))
                        itemHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                        itemHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                        itemHeader.BackgroundColor = New BaseColor(200, 200, 200)
                        itemHeader.MinimumHeight = 14
                        table.AddCell(itemHeader)

                        Dim displayed As Integer = 0
                        For col As Integer = startCol To Math.Min(startCol + 5, itemNumbers.Count - 1)
                            displayed += 1

                            Dim itemCell As New PdfPCell(New Phrase(itemNumbers(col).ToString(), fontHeader))
                            itemCell.HorizontalAlignment = Element.ALIGN_CENTER
                            itemCell.VerticalAlignment = Element.ALIGN_MIDDLE
                            itemCell.BackgroundColor = New BaseColor(200, 200, 200)
                            itemCell.MinimumHeight = 14

                            table.AddCell(itemCell)
                        Next

                        While displayed < 6
                            Dim emptyCell As New PdfPCell(New Phrase("", fontHeader))
                            emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                            emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                            emptyCell.BackgroundColor = New BaseColor(200, 200, 200)
                            emptyCell.MinimumHeight = 14
                            table.AddCell(emptyCell)
                            displayed += 1
                        End While

                        For Each header As String In headers
                            Dim cellHeader As New PdfPCell(New Phrase(header, fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 14
                            table.AddCell(cellHeader)

                            displayed = 0
                            For col As Integer = startCol To Math.Min(startCol + 5, itemNumbers.Count - 1)
                                displayed += 1
                                Dim itemNo As Integer = itemNumbers(col)
                                Dim value As String = ""
                                If data(itemNo).ContainsKey(header) Then
                                    value = data(itemNo)(header)
                                End If
                                Dim cellContent As New PdfPCell(New Phrase(value, fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 14
                                table.AddCell(cellContent)
                            Next

                            While displayed < 6
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 14
                                table.AddCell(emptyCell)
                                displayed += 1
                            End While
                        Next
                        doc.Add(table)
                        If startCol + 6 < itemNumbers.Count Then
                            doc.NewPage()
                        End If
                    Next
                Next
            End If
            doc.Close()
            Return ms.ToArray()
        End Using
    End Function
End Class

Public Class JobEvents
    Inherits PdfPageEventHelper

    Public Property PageTitle As String
    Public Property PageCustomerName As String
    Public Property PageOrderId As String
    Public Property PageOrderNumber As String
    Public Property PageOrderName As String
    Public Property PageTotalDoc As Integer
    Public Property pageCompany As String

    Public Property pageJobNumber As String
    Public Property pageWorkOrder As String
    Public Property pageJobNote As String
    Public Property pageConvertBy As String
    Public Property pageConvertDate As String
    Public Property pageDesignType As String


    Private baseFont As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
    Private template As PdfTemplate

    Public Overrides Sub OnOpenDocument(writer As PdfWriter, document As Document)
        template = writer.DirectContent.CreateTemplate(50, 50)
    End Sub

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent

        If template Is Nothing Then
            template = cb.CreateTemplate(50, 50)
        End If

        Dim headerTable As New PdfPTable(3)
        headerTable.TotalWidth = document.PageSize.Width - 40
        headerTable.LockedWidth = True
        headerTable.SetWidths(New Single() {0.4F, 0.4F, 0.2F})

        Dim firstCellTable As New PdfPTable(3)
        firstCellTable.TotalWidth = headerTable.TotalWidth * 0.5F
        firstCellTable.SetWidths(New Single() {0.25F, 0.05F, 0.7F})

        Dim firstLabels As String() = {"Company", "Customer", "Order #", "Order Number", "Order Name"}
        Dim firstValues As String() = {pageCompany, PageCustomerName, PageOrderId, PageOrderNumber, PageOrderName}
        For i As Integer = 0 To firstLabels.Length - 1
            firstCellTable.AddCell(New PdfPCell(New Phrase(firstLabels(i), New Font(Font.FontFamily.TIMES_ROMAN, 7, Font.BOLD))) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_LEFT})
            firstCellTable.AddCell(New PdfPCell(New Phrase(":", New Font(Font.FontFamily.TIMES_ROMAN, 7, Font.BOLD))) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_CENTER})
            firstCellTable.AddCell(New PdfPCell(New Phrase(firstValues(i), New Font(Font.FontFamily.TIMES_ROMAN, 7))) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_LEFT})
        Next
        Dim firstHeaderCell As New PdfPCell(firstCellTable) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_LEFT}
        headerTable.AddCell(firstHeaderCell)

        Dim secondCellTable As New PdfPTable(3)
        secondCellTable.TotalWidth = headerTable.TotalWidth * 0.5F
        secondCellTable.SetWidths(New Single() {0.25F, 0.05F, 0.7F})

        Dim secondLabels As String() = {"Job Number", "Design Type", "WO No", "Convert By", "Convert Date"}
        Dim secondValues As String() = {pageJobNumber, pageDesignType, pageWorkOrder, pageConvertBy, pageConvertDate, PageOrderName}
        For i As Integer = 0 To secondLabels.Length - 1
            secondCellTable.AddCell(New PdfPCell(New Phrase(secondLabels(i), New Font(Font.FontFamily.TIMES_ROMAN, 7, Font.BOLD))) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_LEFT})
            secondCellTable.AddCell(New PdfPCell(New Phrase(":", New Font(Font.FontFamily.TIMES_ROMAN, 7, Font.BOLD))) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_CENTER})
            secondCellTable.AddCell(New PdfPCell(New Phrase(secondValues(i), New Font(Font.FontFamily.TIMES_ROMAN, 7))) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_LEFT})
        Next
        Dim secondHeaderCell As New PdfPCell(secondCellTable) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_LEFT}
        headerTable.AddCell(secondHeaderCell)

        Dim phraseThird As New Phrase()
        phraseThird.Add(New Chunk(PageTitle, New Font(Font.FontFamily.TIMES_ROMAN, 14, Font.BOLD)))

        Dim thirdHeaderCell As New PdfPCell(phraseThird)
        thirdHeaderCell.Border = 0
        thirdHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        thirdHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(thirdHeaderCell)

        headerTable.WriteSelectedRows(0, -1, 20, document.PageSize.Height - 20, cb)

        Dim noteTable As New PdfPTable(1)
        noteTable.TotalWidth = document.PageSize.Width - 40
        noteTable.LockedWidth = True

        Dim phraseNote As New Phrase()
        phraseNote.Add(New Chunk("Note : ", New Font(Font.FontFamily.TIMES_ROMAN, 7, Font.BOLD)))
        phraseNote.Add(New Chunk(pageJobNote, New Font(Font.FontFamily.TIMES_ROMAN, 7)))

        Dim noteCell As New PdfPCell(phraseNote)
        With noteCell
            .Border = Rectangle.NO_BORDER
            .HorizontalAlignment = Element.ALIGN_LEFT
            .VerticalAlignment = Element.ALIGN_TOP
            .PaddingTop = 2
            .PaddingBottom = 4
        End With

        noteTable.AddCell(noteCell)

        Dim noteY As Single = document.PageSize.Height - 90
        noteTable.WriteSelectedRows(0, -1, 20, noteY, cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True
        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(New Phrase("Print Date: " & Now.ToString("dd MMM yyyy HH:mm"), New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD))) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_LEFT}
        footerTable.AddCell(leftFooterCell)

        Dim pageText As String = "Page " & writer.PageNumber.ToString() & " of "
        Dim pageFont As New Font(Font.FontFamily.TIMES_ROMAN, 8)
        Dim rightFooterCell As New PdfPCell(New Phrase(pageText, pageFont)) With {
            .Border = 0,
            .HorizontalAlignment = Element.ALIGN_RIGHT
        }
        footerTable.AddCell(rightFooterCell)

        Dim footerY As Single = document.PageSize.GetBottom(30)
        footerTable.WriteSelectedRows(0, -1, 36, footerY, cb)

        Dim baseFont As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
        Dim textWidth As Single = baseFont.GetWidthPoint(pageText, 8)
        Dim xPos As Single = document.PageSize.Width - textWidth - 1
        Dim yPos As Single = footerY - 10.0F

        cb.AddTemplate(template, xPos, yPos)
    End Sub

    Public Overrides Sub OnCloseDocument(writer As PdfWriter, document As Document)
        template.BeginText()
        template.SetFontAndSize(baseFont, 8)
        template.SetTextMatrix(0, 0)
        template.ShowText((writer.PageNumber - 1).ToString())
        template.EndText()

        PageTotalDoc = writer.PageNumber

        MyBase.OnCloseDocument(writer, document)
    End Sub
End Class