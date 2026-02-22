
Partial Class Order_View
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim action As String = Request("action")

            Dim headerId As String = Request("boosid")
            If String.IsNullOrEmpty(headerId) OrElse String.IsNullOrEmpty(action) Then Exit Sub
            If action = "jobsheet" Then JobSheet(headerId)
            If action = "invoice" Then JobSheet(headerId)

        End If
    End Sub

    Protected Sub JobSheet(headerId As String)
        Try
            Dim previewClass As New PreviewClass
            Dim pdfBytes As Byte() = previewClass.BindContent(headerId)

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "inline; filename=ORDER-" & headerId & ".pdf")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Invoice(headerId As String)
        Try
            Dim invoiceClass As New InvoiceClass
            Dim pdfBytes As Byte() = invoiceClass.BindContent(headerId)

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "inline; filename=INVOICE-" & headerId & ".pdf")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception

        End Try
    End Sub
End Class
