Imports System.Data

Partial Class Setting_Database_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Tables As Integer
    Protected Views As Integer
    Protected Procedures As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Dim dt As DataTable = settingClass.GetDataTableSP("sp_Dashboard_Database", Nothing)

            If dt.Rows.Count > 0 Then
                Tables = CInt(dt.Rows(0)("Tables"))
                Views = CInt(dt.Rows(0)("Views"))
                Procedures = CInt(dt.Rows(0)("Procedures"))
            End If
        End If
    End Sub

    Protected Function GetSumData(params As String) As String
        Try
            If Not String.IsNullOrEmpty(params) Then
                Dim thisQuery As String = String.Format("SELECT COUNT(*) FROM {0}", params)
                Dim sumData As Integer = settingClass.GetItemData_Integer(thisQuery)
                Return sumData & " Data"
            End If
            Return String.Empty
        Catch ex As Exception
            Return ex.ToString()
        End Try
    End Function

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
