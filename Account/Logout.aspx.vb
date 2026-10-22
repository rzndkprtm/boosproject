
Partial Class Account_Logout
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim sessionId As String = String.Empty

        If Request.Cookies("deviceId") IsNot Nothing Then
            sessionId = Request.Cookies("deviceId").Value
            settingClass.DeleteSession(sessionId)
        End If

        Session.Clear()
        Response.Redirect("~/account/login", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub
End Class
