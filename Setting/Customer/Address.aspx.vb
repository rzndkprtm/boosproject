Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Address
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
            txtSearch.Text = Session("SearchCustomerAddress")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchCustomerAddress") = txtSearch.Text
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Address"

            BindDataCustomer()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
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
                Session("SearchCustomerAddress") = txtSearch.Text
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Address"

                    Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerAddress WHERE Id='" & dataId & "'")
                    If thisData Is Nothing Then Exit Sub

                    BindDataCustomer()

                    ddlCustomer.SelectedValue = thisData("CustomerId").ToString()
                    txtDescription.Text = thisData("Description").ToString()
                    txtAddress.Text = thisData("Address").ToString()
                    txtSuburb.Text = thisData("Suburb").ToString()
                    txtState.Text = thisData("State").ToString()
                    txtPostCode.Text = thisData("PostCode").ToString()
                    txtNote.Text = thisData("Note").ToString()

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
                MessageError_Process(True, "CUSTOMER ACCOUNT IS REQURIED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtAddress.Text = "" Then
                MessageError_Process(True, "ADDRESS IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtAddress.Text.Contains(",") OrElse txtAddress.Text.Contains(";") Then
                MessageError_Process(True, "ADDRESS CANNOT CONTAIN COMMA (,) OR SEMICOLON (;) !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtSuburb.Text = "" Then
                MessageError_Process(True, "SUBURB IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtState.Text = "" Then
                MessageError_Process(True, "STATE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtPostCode.Text = "" Then
                MessageError_Process(True, "POST CODE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerAddress ORDER BY Id DESC")
                    Dim checkData As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) FROM CustomerAddress WHERE CustomerId='" & lblId.Text & "'")
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

                    dataLog = {"CustomerAddress", thisId, Session("LoginId").ToString(), "Customer Address Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchCustomerAddress") = txtSearch.Text
                    Response.Redirect("~/setting/customer/address", False)
                End If

                If lblAction.Text = "Edit" Then
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

                    dataLog = {"CustomerAddress", lblId.Text, Session("LoginId"), "Customer Address Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchCustomerAddress") = txtSearch.Text
                    Response.Redirect("~/setting/customer/address", False)
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

    Protected Sub btnPrimary_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdPrimary.Text

            Dim customerId As String = settingClass.GetItemData("SELECT CustomerId FROM CustomerAddress WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET [Primary]=0 WHERE CustomerId=@CustomerId; UPDATE CustomerAddress SET [Primary]=1 WHERE Id=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@CustomerId", customerId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"CustomerAddress", thisId, Session("LoginId"), "Reset As Primary"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerAddress") = txtSearch.Text
            Response.Redirect("~/setting/customer/address", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Dim fullAddress As String = settingClass.GetItemData("SELECT CONCAT('Description: ', ISNULL(Description, ''), ', ', 'Address: ', ISNULL(Address, ''), ', ', 'Suburb: ', ISNULL(Suburb, ''), ', ', 'State: ', ISNULL(State, ''), ', ', 'PostCode: ', ISNULL(PostCode, '')) AS FullDescription FROM CustomerAddress WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerAddress' AND DataId=@Id; DELETE FROM CustomerAddress WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim stringLog As String = String.Format("Customer Address Deleted | {0}", fullAddress)
            dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), stringLog}
            settingClass.Logs(dataLog)

            Session("SearchCustomerAddress") = txtSearch.Text
            Response.Redirect("~/setting/customer/address", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerAddress") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim())),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing, CType(DBNull.Value, Object), Session("CompanyId"))),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            gvList.DataSource = settingClass.GetDataTableSP("sp_CustomerAddress", params)
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

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Function BindDetailAddress(addressId As String) As String
        Dim result As String = String.Empty

        If Not String.IsNullOrEmpty(addressId) Then
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerAddress WHERE Id='" & addressId & "'")

            If thisData IsNot Nothing Then
                Dim address As String = thisData("Address").ToString()
                Dim suburb As String = thisData("Suburb").ToString()
                Dim state As String = thisData("State").ToString()
                Dim postCode As String = thisData("PostCode").ToString()

                result = address & ", " & suburb & ", " & state & " " & postCode
            End If
        End If
        Return result
    End Function

    Protected Function VisiblePrimary(primary As Boolean) As Boolean
        If primary = False Then Return True
        Return False
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
