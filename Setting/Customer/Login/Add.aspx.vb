Imports System.Data.SqlClient

Partial Class Setting_Customer_Login_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/login/", False)
            Exit Sub
        End If

        ddlCustomer.Enabled = True

        If Not String.IsNullOrEmpty(Request.QueryString("custid")) Then
            lblCustomerId.Text = Request.QueryString("custid").ToString()
            txtUserName.Text = settingClass.GenerateUsername(GetCustomerName(lblCustomerId.Text))
            txtFullName.Text = txtUserName.Text
            ddlCustomer.Enabled = False
            If Session("RoleNme") = "Developer" Then ddlCustomer.Enabled = True
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCustomer(lblCustomerId.Text)
            BindRole()
            BindLevel()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "ACCOUNT IS REQUIRED !")
                Exit Sub
            End If
            If ddlRole.SelectedValue = "" Then
                MessageError(True, "ROLE MEMBER IS REQUIRED !")
                Exit Sub
            End If
            If ddlLevel.SelectedValue = "" Then
                MessageError(True, "LEVEL MEMBER IS REQUIRED !")
                Exit Sub
            End If
            If txtFullName.Text = "" Then
                MessageError(True, "FULL NAME IS REQUIRED !")
                Exit Sub
            End If
            If txtUserName.Text = "" Then
                MessageError(True, "USERNAME IS REQUIRED !")
                Exit Sub
            End If
            If Not Regex.IsMatch(txtUserName.Text, "^[a-zA-Z0-9._-]+$") Then
                MessageError(True, "INVALID USERNAME. ONLY LETTERS, NUMBERS, DOT (.), UNDERSCRORE (_) & HYPHEN (-) ARE ALLOWED !")
                Exit Sub
            End If

            If settingClass.IsUsernameExists(txtUserName.Text.Trim()) Then
                MessageError(True, "USERNAME ALREADY EXIST !")
                Exit Sub
            End If
            Dim checkRole As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) FROM Logins WHERE CustomerId='" & ddlCustomer.SelectedValue & "' AND RoleId='" & ddlRole.SelectedValue & "' AND LevelId='" & ddlLevel.SelectedValue & "'")
            If checkRole > 0 Then
                MessageError(True, "LEVEL ROLE PADA PELANGGAN INI SUDAH TERDAFTAR. SILAHKAN GUNAKAN LEVEL LAIN !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If txtPassword.Text = "" Then txtPassword.Text = txtUserName.Text
                Dim password As String = settingClass.Encrypt(txtPassword.Text)

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Logins ORDER BY Id DESC")
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Logins VALUES (@Id, @CustomerId, @RoleId, @LevelId, @UserName, @Password, @FullName, NULL, 0, NULL, 1, @Pricing, 1)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@CustomerId", If(String.IsNullOrEmpty(ddlCustomer.SelectedValue), CType(DBNull.Value, Object), ddlCustomer.SelectedValue))
                        thisCmd.Parameters.AddWithValue("@RoleId", ddlRole.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@LevelId", ddlLevel.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@UserName", txtUserName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Password", password)
                        thisCmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Pricing", ddlPricing.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Logins", thisId, Session("LoginId").ToString(), "Login Created"}
                settingClass.Logs(dataLog)

                url = "~/setting/customer/login"
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
                End If
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = "~/setting/customer/login"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindCustomer(customerId As String)
        ddlCustomer.Items.Clear()
        Try
            Dim role As String = String.Empty
            If Session("RoleName") = "Sales" Then
                role = "AND CompanyId='" & Session("CompanyId").ToString() & "'"
                If Session("LevelName") = "Member" Then
                    role = "AND (Id = '" & Session("CustomerId") & "' OR EXISTS (SELECT 1 FROM STRING_SPLIT(Operator, ',') WHERE value = '" & Session("LoginId") & "'))"
                End If
            End If

            Dim thisQuery As String = String.Format("SELECT * FROM Customers WHERE Active=1 {0} ORDER BY Name ASC", role)

            ddlCustomer.DataSource = settingClass.GetDataTable(thisQuery)
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
            ddlCustomer.SelectedValue = customerId

            ddlCustomer.Enabled = False
            If String.IsNullOrEmpty(customerId) Then ddlCustomer.Enabled = True
        Catch ex As Exception
            ddlCustomer.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
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

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function GetCustomerName(customerId As String) As String
        Try
            Return settingClass.GetItemData("SELECT Name FROM Customers WHERE Id='" & customerId & "'")
        Catch ex As Exception
            Return "ERROR"
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
