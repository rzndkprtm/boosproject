Partial Class Order_Preview
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim fileName As String = Request.QueryString("file")
        Dim isView As Boolean = Request.QueryString("view") = "true"

        If String.IsNullOrEmpty(fileName) Then
            Response.Write("<p style='color:red;'>Nama file tidak diberikan.</p>")
            Return
        End If

        fileName = IO.Path.GetFileName(fileName)
        Dim filePath As String = Server.MapPath("~/File/Preview/" & fileName)

        If Not IO.File.Exists(filePath) Then
            Response.Write("<p style='color:red;'>File tidak ditemukan.</p>")
            Return
        End If

        ' Mode 1: tampilkan halaman viewer
        If Not isView Then
            Dim fileUrl As String = ResolveUrl("~/Order/Preview.aspx?file=" & fileName & "&view=true")
            litIframe.Text = String.Format("<iframe src='{0}#toolbar=0' width='100%' height='900px' style='border:1px solid #ccc; border-radius:8px;'></iframe>", fileUrl)
            Return
        End If

        ' Mode 2: kirim file PDF inline agar tidak auto-download
        Response.Clear()
        Response.ContentType = "application/pdf"
        Response.AddHeader("Content-Disposition", "inline; filename=" & fileName)
        Response.TransmitFile(filePath)
        Response.End()
    End Sub
End Class
