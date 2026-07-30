Imports System.Data

Partial Class Setting_Job_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected JobSheets As Integer
    Protected OrderJobs As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Dim dt As DataTable = settingClass.GetDataTableSP("sp_Dashboard_Job", Nothing)

            If dt.Rows.Count > 0 Then
                JobSheets = CInt(dt.Rows(0)("JobSheets"))
                OrderJobs = CInt(dt.Rows(0)("OrderJobs"))
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
