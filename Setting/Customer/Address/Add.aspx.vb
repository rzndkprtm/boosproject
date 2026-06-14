Imports System.Data.SqlClient

Partial Class Setting_Customer_Address_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/address/", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("custid")) Then
            lblCustomerId.Text = Request.QueryString("custid").ToString()
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCustomer(lblCustomerId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "ACCOUNT IS REQURIED !")
                Exit Sub
            End If
            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Account" Then
                If Session("CompanyId") = ddlCustomer.SelectedValue Then
                    MessageError(True, "ACCESS DENIED !")
                    Exit Sub
                End If
            End If
            If txtAddress.Text = "" Then
                MessageError(True, "ADDRESS IS REQUIRED !")
                Exit Sub
            End If
            If txtAddress.Text.Contains(",") OrElse txtAddress.Text.Contains(";") Then
                MessageError(True, "ADDRESS CANNOT CONTAIN COMMA (,) OR SEMICOLON (;) !")
                Exit Sub
            End If
            If txtSuburb.Text = "" Then
                MessageError(True, "SUBURB IS REQUIRED !")
                Exit Sub
            End If
            If txtSuburb.Text.Contains(",") OrElse txtAddress.Text.Contains(";") Then
                MessageError(True, "SUBURB CANNOT CONTAIN COMMA (,) OR SEMICOLON (;) !")
                Exit Sub
            End If
            If txtState.Text = "" Then
                MessageError(True, "STATE IS REQUIRED !")
                Exit Sub
            End If
            If txtState.Text.Contains(",") OrElse txtAddress.Text.Contains(";") Then
                MessageError(True, "STATE CANNOT CONTAIN COMMA (,) OR SEMICOLON (;) !")
                Exit Sub
            End If
            If txtPostCode.Text = "" Then
                MessageError(True, "POST CODE IS REQUIRED !")
                Exit Sub
            End If
            If txtPostCode.Text.Contains(",") OrElse txtAddress.Text.Contains(";") Then
                MessageError(True, "POST CODE CANNOT CONTAIN COMMA (,) OR SEMICOLON (;) !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerAddress ORDER BY Id DESC")
                Dim checkData As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) FROM CustomerAddress WHERE CustomerId='" & ddlCustomer.SelectedValue & "'")
                Dim primaryData As Integer = 0
                If checkData = 0 Then primaryData = 1

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerAddress VALUES (@Id, @CustomerId, @Description, @Address, @Suburb, @State, @PostCode, @Note, @Primary)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                        myCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Suburb", txtSuburb.Text.Trim())
                        myCmd.Parameters.AddWithValue("@State", txtState.Text.Trim())
                        myCmd.Parameters.AddWithValue("@PostCode", txtPostCode.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Note", txtNote.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Primary", primaryData)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"CustomerAddress", thisId, Session("LoginId").ToString(), "Customer Address Created"}
                settingClass.Logs(dataLog)

                url = "~/setting/customer/address"
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
        url = "~/setting/customer/address"
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

            ddlCustomer.DataSource = settingClass.GetDataTable(String.Format("SELECT * FROM Customers WHERE Active=1 {0} ORDER BY Name ASC", role))
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
