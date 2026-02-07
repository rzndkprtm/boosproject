Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_General_Mailing
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchMailing")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchMailing") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Mailing"

            BindCompany()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
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

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("SearchMailing") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Mailing"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Mailings WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

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
                    txtCc.Text = myData("Cc").ToString()
                    txtBcc.Text = myData("Bcc").ToString()
                    txtDescription.Text = myData("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Mailings WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='Mailings' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Session("SearchMailing") = txtSearch.Text
            Response.Redirect("~/setting/general/mailing", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Mailings ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Mailings VALUES (@Id, @CompanyId, @Name, @Server, @Host, @Port, @NetworkCredentials, @DefaultCredentials, @EnableSsl, @Account, @Password, @Alias, @Subject, @To, @Cc, @Bcc, @Description, @Active)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CompanyId", ddlCompanyId.SelectedValue)
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

                    Dim dataLog As Object() = {"Mailings", thisId, Session("LoginId").ToString(), "Mailing Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchMailing") = txtSearch.Text
                    Response.Redirect("~/setting/general/mailing", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Mailings SET CompanyId=@CompanyId, Name=@Name, Server=@Server, Host=@Host, Port=@Port, NetworkCredentials=@NetworkCredentials, DefaultCredentials=@DefaultCredentials, EnableSSL=@EnableSSL, Account=@Account, Password=@Password, Alias=@Alias, Subject=@Subject, [To]=@To, Cc=@Cc, Bcc=@Bcc, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@CompanyId", ddlCompanyId.SelectedValue)
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

                    Dim dataLog As Object() = {"Mailings", lblId.Text, Session("LoginId").ToString(), "Mailing Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchMailing") = txtSearch.Text
                    Response.Redirect("~/setting/general/mailing", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnCopy_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdCopy.Text

            Dim newId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Mailings ORDER BY Id DESC")
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Mailings SELECT @NewId, CompanyId, Name + ' - Copy', Server, Host, Port, NetworkCredentials, DefaultCredentials, EnableSSL, Account, Password, Alias, Subject, [To], Cc, Bcc, Description, Active FROM Mailings WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@NewId", newId)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"Mailings", newId, Session("LoginId").ToString(), "Mailing Created | Duplicated of " & lblId.Text}
            settingClass.Logs(dataLog)

            Session("SearchMailing") = txtSearch.Text
            Response.Redirect("~/setting/general/mailing", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchMailing") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "WHERE Mailings.Name LIKE '%" & searchText.Trim() & "%' OR Mailings.Name LIKE '%" & searchText.Trim() & "%' OR Companys.Name LIKE '%" & searchText.Trim() & "%'"
            End If
            Dim thisQuery As String = String.Format("SELECT Mailings.*, Companys.Name AS CompanyName, CASE WHEN Mailings.Active=1 THEN 'Yes' WHEN Mailings.Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Mailings LEFT JOIN Companys ON Mailings.CompanyId=Companys.Id {0} ORDER BY Companys.Id, Mailings.Name ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID")

            btnAdd.Visible = PageAction("Add")
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
            ddlCompanyId.DataTextField = "Name"
            ddlCompanyId.DataValueField = "Id"
            ddlCompanyId.DataBind()

            If ddlCompanyId.Items.Count > 1 Then
                ddlCompanyId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
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
