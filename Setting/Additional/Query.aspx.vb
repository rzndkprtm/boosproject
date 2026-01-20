Imports System.Data.SqlClient

Partial Class Setting_Additional_Query
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/additional", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlAction.SelectedValue = "" Then
                MessageError(True, "QUERY ACTION IS REQUIRED !")
                Exit Sub
            End If

            If txtQuery.Text = "" Then
                MessageError(True, "YOUR QUERY IS REQUIRED !")
                Exit Sub
            End If

            If ddlAction.SelectedValue = "Create" Or ddlAction.SelectedValue = "Update" Or ddlAction.SelectedValue = "Delete" Then
                Dim result As Integer = 0
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand(txtQuery.Text.Trim(), thisConn)
                        myCmd.Connection = thisConn

                        thisConn.Open()
                        result = myCmd.ExecuteNonQuery()
                    End Using
                End Using
                If result = 1 Then

                End If
            End If

            If ddlAction.SelectedValue = "Read" Then
                gvList.DataSource = settingClass.GetDataTable(txtQuery.Text.Trim())
                gvList.DataBind()
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/additional", False)
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
