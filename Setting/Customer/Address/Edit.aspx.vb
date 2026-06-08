Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Address_Edit
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

        If String.IsNullOrEmpty(Request.QueryString("addressid")) Then
            Response.Redirect("~/setting/customer/address/", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblId.Text = Request.QueryString("addressid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "CUSTOMER ACCOUNT IS REQURIED !")
                Exit Sub
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
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET CustomerId=@CustomerId, Description=@Description, Address=@Address, Suburb=@Suburb, State=@State, PostCode=@PostCode, Note=@Note WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                        myCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Suburb", txtSuburb.Text.Trim())
                        myCmd.Parameters.AddWithValue("@State", txtState.Text.Trim())
                        myCmd.Parameters.AddWithValue("@PostCode", txtPostCode.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Note", txtNote.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"CustomerAddress", lblId.Text, Session("LoginId").ToString(), "Customer Address Updated"}
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

    Protected Sub BindData(addressId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerAddress WHERE Id='" & addressId & "'")
            If thisData Is Nothing Then Exit Sub

            BindCustomer()

            ddlCustomer.SelectedValue = thisData("CustomerId").ToString()
            txtDescription.Text = thisData("Description").ToString()
            txtAddress.Text = thisData("Address").ToString()
            txtSuburb.Text = thisData("Suburb").ToString()
            txtState.Text = thisData("State").ToString()
            txtPostCode.Text = thisData("PostCode").ToString()
            txtNote.Text = thisData("Note").ToString()

            ddlCustomer.Enabled = False
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCustomer()
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
