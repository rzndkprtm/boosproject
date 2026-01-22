
Partial Class Mailing_Unshipment
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim type As String = Request.QueryString("type").ToString()

        If type = "unshipment" Then
            Dim fileName As String = Trim("Unshipment - In Production Order " & Now.ToString("dd MMm yyyy") & ".pdf")
            Dim pdfFilePath As String = Server.MapPath("~/File/Preview/" & fileName)

            Dim thisClass As New UnshipmentClass
            thisClass.CreatePDF(pdfFilePath)


            Dim mailClass As New MailingClass
            mailClass.MailUnshipment(pdfFilePath)
        End If
    End Sub
End Class
