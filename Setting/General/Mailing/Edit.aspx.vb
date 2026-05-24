Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_General_Mailing_Edit
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general/mailing", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("mailingid")) Then
            Response.Redirect("~/setting/general/mailing", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("mailingid").ToString()

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Mailings SET CompanyId=@CompanyId, Name=@Name, Server=@Server, Host=@Host, Port=@Port, NetworkCredentials=@NetworkCredentials, DefaultCredentials=@DefaultCredentials, EnableSSL=@EnableSSL, Account=@Account, Password=@Password, Alias=@Alias, Subject=@Subject, [To]=@To, Cc=@Cc, Bcc=@Bcc, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@CompanyId", If(String.IsNullOrEmpty(ddlCompanyId.SelectedValue), CType(DBNull.Value, Object), ddlCompanyId.SelectedValue))
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Server", txtServer.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Host", txtHost.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Port", txtPort.Text.Trim())
                        myCmd.Parameters.AddWithValue("@NetworkCredentials", ddlNetworkCredentials.SelectedValue)
                        myCmd.Parameters.AddWithValue("@DefaultCredentials", ddlDefaultCredentials.SelectedValue)
                        myCmd.Parameters.AddWithValue("@EnableSsl", ddlEnableSSL.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Account", txtAccount.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Alias", txtAlias.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Subject", txtSubject.Text.Trim())
                        myCmd.Parameters.AddWithValue("@To", txtTo.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Cc", txtCC.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Bcc", txtBcc.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Mailings", lblId.Text, Session("LoginId").ToString(), "Mailing Updated"}
                settingClass.Logs(dataLog)
                Response.Redirect("~/setting/general/mailing", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/general/mailing", False)
        Exit Sub
    End Sub

    Protected Sub BindData(mailingId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Mailings WHERE Id='" & mailingId & "'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/general/mailing", False)
                Exit Sub
            End If

            BindCompany()

            ddlCompanyId.SelectedValue = myData("CompanyId").ToString()
            txtName.Text = myData("Name").ToString()
            txtServer.Text = myData("Server").ToString()
            txtHost.Text = myData("Host").ToString()
            txtPort.Text = myData("Port").ToString()

            ddlNetworkCredentials.SelectedValue = Convert.ToInt32(myData("NetworkCredentials"))
            ddlDefaultCredentials.SelectedValue = Convert.ToInt32(myData("DefaultCredentials"))
            ddlEnableSSL.SelectedValue = Convert.ToInt32(myData("EnableSSL"))

            txtAccount.Text = myData("Account").ToString()
            txtPassword.Text = myData("Password").ToString()
            txtAlias.Text = myData("Alias").ToString()
            txtSubject.Text = myData("Subject").ToString()
            txtTo.Text = myData("To").ToString()
            txtCC.Text = myData("Cc").ToString()
            txtBcc.Text = myData("Bcc").ToString()
            txtDescription.Text = myData("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompanyId.Items.Clear()
        Try
            ddlCompanyId.DataSource = settingClass.GetDataTable("SELECT * FROM Companys ORDER BY Name ASC")
            ddlCompanyId.DataTextField = "Alias"
            ddlCompanyId.DataValueField = "Id"
            ddlCompanyId.DataBind()

            If ddlCompanyId.Items.Count > 1 Then
                ddlCompanyId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

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
