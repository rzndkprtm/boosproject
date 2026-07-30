Imports System.Data

Partial Class Setting_General_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Companys As Integer
    Protected Mailings As Integer
    Protected Newsletters As Integer
    Protected Tutorials As Integer
    Protected Notifications As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Dim dt As DataTable = settingClass.GetDataTableSP("sp_Dashboard_General", Nothing)

            If dt.Rows.Count > 0 Then
                Companys = CInt(dt.Rows(0)("Companys"))
                Mailings = CInt(dt.Rows(0)("Mailings"))
                Newsletters = CInt(dt.Rows(0)("Newsletters"))
                Tutorials = CInt(dt.Rows(0)("Tutorials"))
                Notifications = CInt(dt.Rows(0)("Notifications"))
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
