
Partial Class Order_Archive_Preview
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If String.IsNullOrEmpty(Request.QueryString("orderid")) OrElse String.IsNullOrEmpty(Request.QueryString("filename")) Then
            Response.Redirect("~/order/archive", False)
            Exit Sub
        End If

        Dim url As String = ResolveUrl("~/file/archive/" & Request.QueryString("filename"))

        embPrint.Attributes.Add("src", url)
    End Sub
End Class
