
Partial Class Setting_Validation_Detail_Default
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Response.Redirect("~/setting/validation", False)
    End Sub

End Class
