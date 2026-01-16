
Partial Class Error_Default
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Response.Redirect("~/", False)
    End Sub
End Class
