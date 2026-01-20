Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

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

    Public Function GetReportData(myCmd As SqlCommand) As DataSet
        Using thisConn As New SqlConnection(myConn)
            Using thisAdapter As New SqlDataAdapter()
                myCmd.Connection = thisConn
                thisAdapter.SelectCommand = myCmd

                Dim thisDataSet As New DataSet()
                thisAdapter.Fill(thisDataSet)
                Return thisDataSet
            End Using
        End Using
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
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
        Return result
    End Function

    Public Sub Blinds(filePath As String, type As String, startDate As DateTime, endDate As DateTime)
        Using fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
            Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)

            Dim pageEvent As New ReportEvents() With {
                .PageTitle = type,
                .PageStartDate = startDate,
                .PageEndDate = endDate
            }
            writer.PageEvent = pageEvent

            doc.Open()

            Dim emptyLine As New Paragraph(" ", New Font(Font.FontFamily.TIMES_ROMAN, 10))
            emptyLine.SpacingBefore = 1
            doc.Add(emptyLine)

            doc.Close()
        End Using
    End Sub
End Class

Public Class ReportEvents
    Inherits PdfPageEventHelper

    Public Property PageTitle As String
    Public Property PageStartDate As DateTime
    Public Property PageEndDate As DateTime

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent
        Dim font As Font = FontFactory.GetFont("Arial", 12, Font.BOLD)

        Dim headerTable As New PdfPTable(2)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True

        headerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim phrase As New Phrase()
        Dim chunk1 As New Chunk(UCase(PageTitle).ToString(), New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD))
        phrase.Add(chunk1)

        Dim leftHeaderCell As New PdfPCell(phrase)
        leftHeaderCell.Border = 0
        leftHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftHeaderCell.VerticalAlignment = Element.ALIGN_MIDDLE
        headerTable.AddCell(leftHeaderCell)

        Dim periodText As String

        If PageStartDate.Date = PageEndDate.Date Then
            periodText = "Period : " & PageStartDate.ToString("dd MMM yyyy")
        Else
            periodText = "Period : " & PageStartDate.ToString("dd MMM yyyy") & " - " & PageEndDate.ToString("dd MMM yyyy")
        End If

        Dim rightHeaderCell As New PdfPCell(New Phrase(periodText, New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        rightHeaderCell.Border = 0
        rightHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightHeaderCell.VerticalAlignment = Element.ALIGN_MIDDLE
        headerTable.AddCell(rightHeaderCell)

        headerTable.WriteSelectedRows(0, -1, 36, document.PageSize.Height - 20, cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True

        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(New Phrase("All information within this report is private and confidential.", New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        leftFooterCell.Border = 0
        leftFooterCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(leftFooterCell)

        Dim rightFooterCell As New PdfPCell(New Phrase("Page " & writer.PageNumber, New Font(Font.FontFamily.TIMES_ROMAN, 10)))
        rightFooterCell.Border = 0
        rightFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(rightFooterCell)

        footerTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(36), cb)
    End Sub
End Class
