Imports System.Data

Partial Class Setting_Login_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Logins As Integer
    Protected LoginRoles As Integer
    Protected LoginLevels As Integer
    Protected LoginAccessC As Integer
    Protected Online As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Dim dt As DataTable = settingClass.GetDataTableSP("sp_Dashboard_Login", Nothing)

            If dt.Rows.Count > 0 Then
                Logins = CInt(dt.Rows(0)("Logins"))
                LoginRoles = CInt(dt.Rows(0)("LoginRoles"))
                LoginLevels = CInt(dt.Rows(0)("LoginLevels"))
                LoginAccessC = CInt(dt.Rows(0)("LoginAccessC"))
                Online = CInt(dt.Rows(0)("Online"))
            End If
        End If
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
