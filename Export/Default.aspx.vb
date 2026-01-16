Partial Class Export_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
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
            If msgError.InnerText = "" Then
                Dim url As String = String.Format("boe?company={0}&status={1}&type={2}", ddlOrderCompany.SelectedValue, ddlOrderStatus.SelectedValue, ddlOrderType.SelectedValue)
                Dim script As String = "window.open('" & ResolveUrl(url) & "', '_blank');"
                ClientScript.RegisterStartupScript(Me.GetType(), "exportTab", script, True)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Protected Sub BindCompany()
        ddlOrderCompany.Items.Clear()
        Try
            ddlOrderCompany.DataSource = settingClass.GetListData("SELECT * FROM Companys ORDER BY Name ASC")
            ddlOrderCompany.DataTextField = "Name"
            ddlOrderCompany.DataValueField = "Id"
            ddlOrderCompany.DataBind()

            If ddlOrderCompany.Items.Count > 0 Then
                ddlOrderCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlOrderCompany.Items.Clear()
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function PageAction(Action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, Action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
