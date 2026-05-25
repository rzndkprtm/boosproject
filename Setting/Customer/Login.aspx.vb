Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Login
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomerLogin")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Login"

            BindDataCustomer()
            BindRole()
            BindLevel()

            divPassword.Visible = True
            divEmail.Visible = False
            If Session("RoleName") = "Developer" Then divEmail.Visible = True

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
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Session("SearchCustomerLogin") = txtSearch.Text
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Login"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Logins WHERE Id='" & lblId.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    BindDataCustomer()
                    BindRole()
                    BindLevel()

                    ddlCustomer.SelectedValue = myData("CustomerId").ToString()
                    ddlRole.SelectedValue = myData("RoleId").ToString()
                    ddlLevel.SelectedValue = myData("LevelId").ToString()
                    txtUserName.Text = myData("UserName").ToString()
                    lblUserName.Text = myData("UserName").ToString()
                    txtFullName.Text = myData("FullName").ToString()
                    txtEmail.Text = myData("Email").ToString()

                    divPassword.Visible = False
                    divEmail.Visible = False
                    If Session("RoleName") = "Developer" Then divEmail.Visible = True

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

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError_Process(True, "CUSTOMER ACCOUNT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlRole.SelectedValue = "" Then
                MessageError_Process(True, "ROLE MEMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlLevel.SelectedValue = "" Then
                MessageError_Process(True, "LEVEL MEMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtFullName.Text = "" Then
                MessageError_Process(True, "FULL NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtUserName.Text = "" Then
                MessageError_Process(True, "USERNAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If Not Regex.IsMatch(txtUserName.Text, "^[a-zA-Z0-9._-]+$") Then
                MessageError_Process(True, "INVALID USERNAME. ONLY LETTERS, NUMBERS, DOT (.), UNDERSCRORE (_) & HYPHEN (-) ARE ALLOWED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            Dim checkUsername As String = settingClass.GetItemData("SELECT UserName FROM Logins WHERE UserName='" + txtUserName.Text + "'")

            If lblAction.Text = "Add" Then
                If txtUserName.Text = checkUsername Then
                    MessageError_Process(True, "USERNAME ALREADY EXIST !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                    Exit Sub
                End If
            End If

            If lblAction.Text = "Edit" And txtUserName.Text <> lblUserName.Text Then
                If txtUserName.Text = checkUsername Then
                    MessageError_Process(True, "USERNAME ALREADY EXIST !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                    Exit Sub
                End If
            End If

            If msgErrorProcess.InnerText = "" Then
                If txtPassword.Text = "" Then txtPassword.Text = txtUserName.Text
                Dim password As String = settingClass.Encrypt(txtPassword.Text)

                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Logins ORDER BY Id DESC")
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Logins VALUES (@Id, @CustomerId, @RoleId, @LevelId, @UserName, @Password, @FullName, @Email, 0, NULL, 1, @Pricing, 1)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", If(String.IsNullOrEmpty(ddlCustomer.SelectedValue), CType(DBNull.Value, Object), ddlCustomer.SelectedValue))
                            myCmd.Parameters.AddWithValue("@RoleId", ddlRole.SelectedValue)
                            myCmd.Parameters.AddWithValue("@LevelId", ddlLevel.SelectedValue)
                            myCmd.Parameters.AddWithValue("@UserName", txtUserName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Password", password)
                            myCmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Pricing", ddlPricing.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Login Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchCustomerLogin") = txtSearch.Text
                    Response.Redirect("~/setting/customer/login", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET CustomerId=@CustomerId, RoleId=@RoleId, LevelId=@LevelId, UserName=@UserName, FullName=@FullName, Email=@Email WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", If(String.IsNullOrEmpty(ddlCustomer.SelectedValue), CType(DBNull.Value, Object), ddlCustomer.SelectedValue))
                            myCmd.Parameters.AddWithValue("@RoleId", ddlRole.SelectedValue)
                            myCmd.Parameters.AddWithValue("@LevelId", ddlLevel.SelectedValue)
                            myCmd.Parameters.AddWithValue("@UserName", txtUserName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Pricing", ddlPricing.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"Logins", lblId.Text, Session("LoginId").ToString(), "Login Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchCustomerLogin") = txtSearch.Text
                    Response.Redirect("~/setting/customer/login", False)
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

    Protected Sub btnActive_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdActive.Text

            Dim active As Integer = 1
            If txtActive.Text = "1" Then : active = 0 : End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Active=@Active WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Active", active)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim activeDesc As String = "Login Has Been Activated"
            If active = 0 Then activeDesc = "Login Has Been Deactivated"

            dataLog = {"Logins", thisId, Session("LoginId").ToString(), activeDesc}
            settingClass.Logs(dataLog)

            Session("SearchCustomerLogin") = txtSearch.Text
            Response.Redirect("~/setting/customer/login", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnChangePassword_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If Not String.IsNullOrEmpty(txtChangePassword.Text) Then
                Dim thisId As String = txtIdChangePassword.Text
                Dim newPassword As String = settingClass.Encrypt(txtChangePassword.Text)

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Password=@Password WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Password", newPassword)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Change Password Login"}
                settingClass.Logs(dataLog)

                Session("SearchCustomerLogin") = txtSearch.Text
                Response.Redirect("~/setting/customer/login", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnResetPass_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdResetPass.Text
            Dim newPassword As String = settingClass.Encrypt(txtNewResetPass.Text)

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Password=@Password, ResetLogin=1 WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Password", newPassword)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Login Reset Password"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerLogin") = txtSearch.Text
            Response.Redirect("~/setting/customer/login", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerLogin") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim())),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing, CType(DBNull.Value, Object), Session("CompanyId"))),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            gvList.DataSource = settingClass.GetDataTableSP("sp_CustomerLogins", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID") ' ID
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataCustomer()
        ddlCustomer.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM Customers ORDER BY Name ASC"
            If Session("RoleName") = "Account" Then
                thisString = "SELECT * FROM Customers WHERE CompanyId='" & Session("CompanyId").ToString() & "' ORDER BY Name ASC"
            End If
            If Session("RoleName") = "Sales" Then
                thisString = "SELECT * FROM Customers WHERE CompanyId='" & Session("CompanyId").ToString() & "' ORDER BY Name ASC"
                If Session("LevelName") = "Member" Then
                    thisString = "SELECT * FROM Customers CROSS APPLY STRING_SPLIT(Operator, ',') AS operatorArray WHERE CompanyId='" & Session("CompanyId").ToString() & "' AND operatorArray.VALUE='" & Session("LoginId").ToString() & "' ORDER BY Name ASC"
                End If
            End If

            ddlCustomer.DataSource = settingClass.GetDataTable(thisString)
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
        End Try
    End Sub

    Protected Sub BindRole()
        ddlRole.Items.Clear()
        Try
            ddlRole.DataSource = settingClass.GetDataTable("SELECT * FROM LoginRoles WHERE Id='8' ORDER BY Name ASC")
            ddlRole.DataTextField = "Name"
            ddlRole.DataValueField = "Id"
            ddlRole.DataBind()

            ddlRole.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            ddlRole.Items.Clear()
        End Try
    End Sub

    Protected Sub BindLevel()
        ddlLevel.Items.Clear()
        Try
            ddlLevel.DataSource = settingClass.GetDataTable("SELECT * FROM LoginLevels ORDER BY Name ASC")
            ddlLevel.DataTextField = "Name"
            ddlLevel.DataValueField = "Id"
            ddlLevel.DataBind()

            ddlLevel.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            ddlLevel.Items.Clear()
        End Try
    End Sub

    Protected Function DencryptPassword(password As String) As String
        Dim result As String = settingClass.Decrypt(password)
        Return result
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Function TextActive(active As Boolean) As String
        Dim result As String = "Enable"
        If active = True Then : Return "Disable" : End If
        Return result
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
