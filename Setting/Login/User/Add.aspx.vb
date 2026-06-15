Imports System.Data.SqlClient

Partial Class Setting_Login_User_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/login/user", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindRole()
            BindLeve()
            BindCustomer()

            BindPage(ddlRole.SelectedValue)

            divEmail.Visible = False
            If Session("RoleName") = "Developer" Then divEmail.Visible = True
        End If
    End Sub

    Protected Sub ddlRole_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindPage(ddlRole.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlRole.SelectedValue = "" Then
                MessageError(True, "ROLE IS REQUIRED !")
                Exit Sub
            End If
            If ddlLevel.SelectedValue = "" Then
                MessageError(True, "LEVEL IS REQUIRED !")
                Exit Sub
            End If
            If ddlRole.SelectedValue = "4" OrElse ddlRole.SelectedValue = "5" OrElse ddlRole.SelectedValue = "8" OrElse ddlRole.SelectedValue = "10" Then
                If ddlCustomer.SelectedValue = "" Then
                    MessageError(True, "CUSTOMER ACCOUNT IS REQUIRED !")
                    Exit Sub
                End If
            End If
            If txtAddUserName.Text = "" Then
                MessageError(True, "USERNAME IS REQUIRED !")
                Exit Sub
            End If
            If Not Regex.IsMatch(txtAddUserName.Text, "^[a-zA-Z0-9._-]+$") Then
                MessageError(True, "INVALID USERNAME. ONLY LETTERS, NUMBERS, DOT (.), UNDERSCRORE (_) & HYPHEN (-) ARE ALLOWED !")
                Exit Sub
            End If
            Dim checkUsername As String = settingClass.GetItemData("SELECT UserName FROM Logins WHERE UserName='" + txtAddUserName.Text + "'")
            If txtAddUserName.Text = checkUsername Then
                MessageError(True, "USERNAME ALREADY EXIST !")
                Exit Sub
            End If

            If txtAddPassword.Text = "" Then
                MessageError(True, "PASSWORD IS REQUIRED !")
                Exit Sub
            End If

            Dim isValidEmail As Boolean = False
            Try
                Dim addr As New Net.Mail.MailAddress(txtEmail.Text.Trim())
                isValidEmail = (addr.Address = txtEmail.Text.Trim())
            Catch
                isValidEmail = False
            End Try

            If Not isValidEmail Then
                MessageError(True, "PLEASE ENTER A VALID EMAIL ADDRESS !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If ddlRole.SelectedValue = "1" OrElse ddlRole.SelectedValue = "2" OrElse ddlRole.SelectedValue = "3" OrElse ddlRole.SelectedValue = "7" OrElse ddlRole.SelectedValue = "9" Then
                    ddlCustomer.SelectedValue = ""
                End If
                If txtAddPassword.Text = "" Then txtAddPassword.Text = txtAddUserName.Text
                Dim password As String = settingClass.Encrypt(txtAddPassword.Text)

                If String.IsNullOrEmpty(txtFullName.Text) Then txtFullName.Text = txtAddUserName.Text

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Logins ORDER BY Id DESC")
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Logins VALUES (@Id, @CustomerId, @RoleId, @LevelId, @UserName, @Password, @FullName, @Email, 0, NULL, 1, @Pricing, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@CustomerId", If(String.IsNullOrEmpty(ddlCustomer.SelectedValue), CType(DBNull.Value, Object), ddlCustomer.SelectedValue))
                        myCmd.Parameters.AddWithValue("@RoleId", ddlRole.SelectedValue)
                        myCmd.Parameters.AddWithValue("@LevelId", ddlLevel.SelectedValue)
                        myCmd.Parameters.AddWithValue("@UserName", txtAddUserName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Password", password)
                        myCmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Pricing", ddlPricing.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Logins", thisId, Session("LoginId").ToString(), "Login Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/login/user", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/login/user", False)
    End Sub

    Protected Sub BindPage(roleId As String)
        Try
            divCustomer.Visible = False
            If roleId = "4" OrElse roleId = "5" OrElse roleId = "8" OrElse roleId = "10" Then
                divCustomer.Visible = True
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub BindRole()
        ddlRole.Items.Clear()
        Try
            Dim roleName As String = Convert.ToString(Session("RoleName"))
            Dim levelName As String = Convert.ToString(Session("LevelName"))

            Dim excludeIds As New List(Of String)
            Select Case roleName
                Case "IT"
                    excludeIds.Add("1")

                    If levelName = "Member" Then
                        excludeIds.Add("2")
                    End If
                Case "Factory Office"
                    excludeIds.AddRange({"1", "2"})

                    If levelName = "Member" Then
                        excludeIds.Add("3")
                    End If
                Case "Developer"
            End Select

            Dim sql As String = "SELECT * FROM LoginRoles WHERE Active=1"

            If excludeIds.Count > 0 Then
                sql &= " AND Id NOT IN ('" & String.Join("','", excludeIds) & "')"
            End If
            sql &= " ORDER BY Name ASC"

            ddlRole.DataSource = settingClass.GetDataTable(sql)
            ddlRole.DataTextField = "Name"
            ddlRole.DataValueField = "Id"
            ddlRole.DataBind()

            If ddlRole.Items.Count > 1 Then
                ddlRole.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlRole.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindLeve()
        ddlLevel.Items.Clear()
        Try
            ddlLevel.DataSource = settingClass.GetDataTable("SELECT * FROM LoginLevels WHERE Active=1 ORDER BY Name ASC")
            ddlLevel.DataTextField = "Name"
            ddlLevel.DataValueField = "Id"
            ddlLevel.DataBind()

            If ddlLevel.Items.Count > 1 Then
                ddlLevel.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlLevel.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindCustomer()
        ddlCustomer.Items.Clear()
        Try
            ddlCustomer.DataSource = settingClass.GetDataTable("SELECT * FROM Customers WHERE Active=1 ORDER BY Name ASC")
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
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
