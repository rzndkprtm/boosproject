Imports System.Data.SqlClient

Partial Class Setting_Log
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub tmrRefresh_Tick(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim()))
            }
            gvList.DataSource = settingClass.GetDataTableSP("sp_GetLogs", params)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function GetDataName(type As String, dataId As String, desc As String) As String
        Try
            If Not String.IsNullOrEmpty(type) AndAlso Not String.IsNullOrEmpty(dataId) Then
                Dim thisQuery As String = String.Format("SELECT Name FROM {0} WHERE Id={1}", type, dataId)
                If type = "Logins" Then
                    thisQuery = String.Format("SELECT UserName FROM {0} WHERE Id={1}", type, dataId)
                End If
                If type = "OrderHeaders" Then
                    thisQuery = String.Format("SELECT OrderId FROM {0} WHERE Id={1}", type, dataId)
                End If
                If type = "OrderDetails" Then
                    thisQuery = String.Format("SELECT Id FROM {0} WHERE Id={1}", type, dataId)
                End If

                Dim dataName As String = settingClass.GetItemData(thisQuery)

                Dim thisDes As String = String.Format("{0} -> {1}", dataName, desc)
                Return thisDes
            End If

            Return String.Empty
        Catch ex As Exception
            Return "Error"
        End Try
        Return String.Empty
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
