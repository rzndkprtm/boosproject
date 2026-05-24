Imports System.Data.SqlClient

Partial Class Setting_General_Mailing_Add
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

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCompany()
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Mailings ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Mailings VALUES (@Id, @CompanyId, @Name, @Server, @Host, @Port, @NetworkCredentials, @DefaultCredentials, @EnableSsl, @Account, @Password, @Alias, @Subject, @To, @Cc, @Bcc, @Description, @Active)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
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
                        myCmd.Parameters.AddWithValue("@Cc", txtCc.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Bcc", txtBcc.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Mailings", thisId, Session("LoginId").ToString(), "Mailing Created"}
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
