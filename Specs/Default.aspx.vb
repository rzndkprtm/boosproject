
Partial Class Specs_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Dim pageAccess As Boolean = LoginAccess("Load")
        'If pageAccess = False Then
        '    Response.Redirect("~/", False)
        '    Exit Sub
        'End If

        If Not IsPostBack Then

        End If
    End Sub

    Protected Function LoginAccess(Action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, Action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
