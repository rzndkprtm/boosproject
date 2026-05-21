Imports System.Data.SqlClient

Partial Class Setting_Online
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_SendNotif(False, String.Empty)
            BindData(txtSearch.Text, ddlMinute.SelectedValue)
        End If
    End Sub

    Protected Sub tmrRefresh_Tick(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        MessageError_SendNotif(False, String.Empty)
        BindData(txtSearch.Text, ddlMinute.SelectedValue)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        MessageError_SendNotif(False, String.Empty)
        BindData(txtSearch.Text, ddlMinute.SelectedValue)
    End Sub

    Protected Sub ddlMinute_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        MessageError_SendNotif(False, String.Empty)
        BindData(txtSearch.Text, ddlMinute.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text, ddlMinute.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSendNotif_Click(sender As Object, e As EventArgs)
        MessageError_SendNotif(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showSendNotif(); };"
        Try
            Dim htmlContent As String = fieldMessage.Value
            If htmlContent = "" Then
                MessageError_SendNotif(True, "MESSAGE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showSendNotif", thisScript, True)
                Exit Sub
            End If

            If msgErrorSendNotif.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Notifications ORDER BY Id DESC")
                Dim loginId As String = txtLoginId.Text
                Dim roleId As String = txtRoleId.Text

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Notifications VALUES (@Id, @RoleId, @LoginId, @Title, @Message, GETDATE(), GETDATE(), 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@RoleId", roleId)
                        myCmd.Parameters.AddWithValue("@LoginId", loginId)
                        myCmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Message", htmlContent)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Notifications", thisId, Session("LoginId").ToString(), "Notification Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/online", False)
            End If
        Catch ex As Exception
            MessageError_SendNotif(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showSendNotif", thisScript, True)
        End Try
    End Sub

    Protected Sub BindData(searchText As String, minuteText As String)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", searchText),
                New SqlParameter("@Minutes", minuteText)
            }
            gvList.DataSource = settingClass.GetDataTableSP("sp_GetActiveCustomerLogin", paramsItem)
            gvList.DataBind()

            divMinute.Visible = PageAction("Sort Minute")
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_SendNotif(visible As Boolean, message As String)
        divErrorSendNotif.Visible = visible : msgErrorSendNotif.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
