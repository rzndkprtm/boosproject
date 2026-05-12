Partial Class Boos_Default
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Response.Redirect("~/", False)
        Exit Sub
    End Sub
End Class
