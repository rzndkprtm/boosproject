Partial Class Setting_Additional_Email
    Inherits Page

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

            BindCompany()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCompany.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If

            If txtSendTo.Text = "" Then
                MessageError(True, "SEND TO IS REQUIRED !")
                Exit Sub
            End If

            If txtMessage.Text = "" Then
                MessageError(True, "MESSAGE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                mailingClass.TestEmail(ddlCompany.SelectedValue, txtSendTo.Text, txtMessage.Text)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/additional", False)
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = settingClass.GetListData("SELECT * FROM Companys ORDER BY Id ASC")
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 1 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
        End Try
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
