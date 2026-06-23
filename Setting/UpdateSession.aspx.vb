Partial Class Setting_UpdateSession
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Session("KeepAlive") = 1
    End Sub
End Class
