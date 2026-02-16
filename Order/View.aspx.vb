
Partial Class Order_View
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim headerId As String = Request("boosid")
            If String.IsNullOrEmpty(headerId) Then Exit Sub

            Dim previewClass As New PreviewClass
            Dim pdfBytes As Byte() = previewClass.BindContent(headerId)

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "inline; filename=Order" & headerId & ".pdf")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        End If
    End Sub
End Class
