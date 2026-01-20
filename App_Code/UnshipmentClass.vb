Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class UnshipmentClass

    Dim bigConn As String = ConfigurationManager.ConnectionStrings("WebOrder").ConnectionString

    Public Function GetDataRow(thisString As String) As DataRow
        Try
            Using thisConn As New SqlConnection(bigConn)
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
            Using thisConn As New SqlConnection(bigConn)
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
            Using thisConn As New SqlConnection(bigConn)
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

    Public Sub BindContent(filePath As String)
        If Not String.IsNullOrEmpty(filePath) Then
            Dim thisData As DataTable = GetDataTable("SELECT OrderHeaders.*, Customers.Name AS CustomerName, CustomerLogins.FullName AS CreatedName FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id LEFT JOIN CustomerLogins ON OrderHeaders.CreatedBy=CustomerLogins.Id WHERE OrderHeaders.OrderType='Regular' AND Customers.CompanyId='2' AND OrderHeaders.Status='In Production' AND OrderHeaders.ProductionDate<= DATEADD( DAY, - 10, GETDATE())")

            If thisData.Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
                Dim pdfFilePath As String = filePath
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)

                    Dim pageEvent As New MailingEvents() With {
                            .PageTitle = "Unshipment - In Production Order"
                        }
                    writer.PageEvent = pageEvent
                    doc.Open()

                    Dim table As New PdfPTable(9)
                    table.WidthPercentage = 100
                    table.SetWidths(New Single() {0.05F, 0.05F, 0.2F, 0.1F, 0.2F, 0.1F, 0.1F, 0.1F, 0.1F})

                    table.AddCell(HeaderCellOriginal("#"))
                    table.AddCell(HeaderCellOriginal("Order ID"))
                    table.AddCell(HeaderCellOriginal("Customer Name"))
                    table.AddCell(HeaderCellOriginal("Order Number"))
                    table.AddCell(HeaderCellOriginal("Order Name"))
                    table.AddCell(HeaderCellOriginal("Created By"))
                    table.AddCell(HeaderCellOriginal("Created Date"))
                    table.AddCell(HeaderCellOriginal("Submitted Date"))
                    table.AddCell(HeaderCellOriginal("Production Date"))

                    For i As Integer = 0 To thisData.Rows.Count - 1
                        Dim orderId As String = thisData.Rows(i)("OrderId").ToString()
                        Dim customerName As String = thisData.Rows(i)("CustomerName").ToString()
                        Dim orderNumber As String = thisData.Rows(i)("StoreOrderNo").ToString()
                        Dim orderName As String = thisData.Rows(i)("StoreCustomer").ToString()
                        Dim createdName As String = thisData.Rows(i)("CreatedName").ToString()
                        Dim createdDate As DateTime = Convert.ToDateTime(thisData.Rows(i)("CreatedDate"))
                        Dim submittedDate As DateTime = Convert.ToDateTime(thisData.Rows(i)("SubmittedDate"))
                        Dim productionDate As DateTime = Convert.ToDateTime(thisData.Rows(i)("ProductionDate"))

                        table.AddCell(ContentCell(i + 1))
                        table.AddCell(ContentCell(orderId))
                        table.AddCell(ContentCell(customerName))
                        table.AddCell(ContentCell(orderNumber))
                        table.AddCell(ContentCell(orderName))
                        table.AddCell(ContentCell(createdName))
                        table.AddCell(ContentCell(createdDate.ToString("dd MMM yyyy")))
                        table.AddCell(ContentCell(submittedDate.ToString("dd MMM yyyy")))
                        table.AddCell(ContentCell(productionDate.ToString("dd MMM yyyy")))
                    Next
                    doc.Add(table)

                    doc.Close()
                End Using
            End If
        End If
    End Sub

    Public Sub CreatePDF(Files As String)
        'Try
        Dim thisData As DataTable = GetDataTable("SELECT Order_Header.*, B.Name AS BuilderName FROM Order_Header INNER JOIN Users ON Order_Header.UserLogin=Users.UserName INNER JOIN Stores ON Users.DebtorCode=Stores.Id LEFT JOIN Builder_Header ON Order_Header.OrdID=Builder_Header.OrdID LEFT JOIN Stores B ON Builder_Header.DebtorCode=B.Id WHERE Order_Header.Active=1 AND Order_Header.SubmittedDate>='2025-01-01' AND Order_Header.SubmittedDate<=DATEADD(DAY, -10, GETDATE()) AND Order_Header.Status='In Production' AND Order_Header.UserLogin<> 'u_builder' AND (Order_Header.Shipped IS NULL OR Order_Header.Shipped='') AND (Stores.Type='REGULAR' OR Stores.Type='BUILDER') AND Users.CompanyGroup='JPMD' AND (B.Type<>'PROFORMA' OR B.Type IS NULL);")

        If thisData.Rows.Count > 0 Then
            Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
            Dim pdfFilePath As String = Files
            Using fs As New FileStream(pdfFilePath, FileMode.Create)
                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)

                Dim pageEvent As New MailingEvents() With {
                        .PageTitle = "Unshipment - In Production Order"
                    }
                writer.PageEvent = pageEvent
                doc.Open()

                Dim table As New PdfPTable(8)
                table.WidthPercentage = 100
                table.SetWidths(New Single() {0.05F, 0.1F, 0.25F, 0.1F, 0.2F, 0.1F, 0.1F, 0.1F})

                table.AddCell(HeaderCellOriginal("#"))
                table.AddCell(HeaderCellOriginal("WEB ID"))
                table.AddCell(HeaderCellOriginal("Retailer Name"))
                table.AddCell(HeaderCellOriginal("Order Number"))
                table.AddCell(HeaderCellOriginal("Order Name"))
                table.AddCell(HeaderCellOriginal("Created By"))
                table.AddCell(HeaderCellOriginal("Created Date"))
                table.AddCell(HeaderCellOriginal("Submitted Date"))

                For i As Integer = 0 To thisData.Rows.Count - 1
                    Dim headerId As String = thisData.Rows(i)("OrdID").ToString()
                    Dim orderNumber As String = thisData.Rows(i)("StoreOrderNo").ToString()
                    Dim orderName As String = thisData.Rows(i)("StoreCustomer").ToString()
                    Dim createdBy As String = thisData.Rows(i)("UserLogin").ToString()
                    Dim createdDate As DateTime = Convert.ToDateTime(thisData.Rows(i)("CreatedDate"))
                    Dim submittedDate As DateTime = Convert.ToDateTime(thisData.Rows(i)("SubmittedDate"))
                    Dim customerName As String = GetItemData("SELECT Stores.Name FROM Stores INNER JOIN Users ON Stores.Id=Users.DebtorCode WHERE Users.UserName = '" + createdBy + "'")
                    If Not String.IsNullOrEmpty(thisData.Rows(i)("BuilderName").ToString()) Then
                        customerName = String.Format("Builder - {0}", thisData.Rows(i)("BuilderName").ToString())
                    End If

                    table.AddCell(ContentCell(i + 1))
                    table.AddCell(ContentCell(headerId))
                    table.AddCell(ContentCell(customerName))
                    table.AddCell(ContentCell(orderNumber))
                    table.AddCell(ContentCell(orderName))
                    table.AddCell(ContentCell(createdBy))
                    table.AddCell(ContentCell(createdDate.ToString("dd MMM yyyy")))
                    table.AddCell(ContentCell(submittedDate.ToString("dd MMM yyyy")))
                Next
                doc.Add(table)

                doc.Close()
            End Using
        End If
        'Catch ex As Exception
        'End Try
    End Sub

    Private Function HeaderCellOriginal(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 20
        Return thisCell
    End Function

    Private Function ContentCell(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 8)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 15
        Return thisCell
    End Function
End Class

Public Class MailingEvents
    Inherits PdfPageEventHelper

    Public Property PageTitle As String

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent
        Dim font As Font = FontFactory.GetFont("Arial", 12, Font.BOLD)

        Dim headerTable As New PdfPTable(2)
        headerTable.TotalWidth = document.PageSize.Width - 72 ' 72 is the margin
        headerTable.LockedWidth = True

        headerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim phrase As New Phrase()
        Dim chunk1 As New Chunk(UCase(PageTitle).ToString(), New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD))
        phrase.Add(chunk1)
        Dim chunk2 As New Chunk(vbCrLf & vbCrLf)
        phrase.Add(chunk2)

        Dim leftHeaderCell As New PdfPCell(phrase)
        leftHeaderCell.Border = 0
        leftHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(leftHeaderCell)

        Dim rightHeaderCell As New PdfPCell(New Phrase("Date : " & DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"), New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        rightHeaderCell.Border = 0
        rightHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightHeaderCell.VerticalAlignment = Element.ALIGN_BOTTOM
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