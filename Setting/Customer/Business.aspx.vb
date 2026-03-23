Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Business
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomerBusiness")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchCustomerBusiness") = txtSearch.Text
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Business"

            BindDataCustomer()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
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
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Session("SearchCustomerBusiness") = txtSearch.Text
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Business"

                    Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerBusiness WHERE Id='" & dataId & "'")
                    If thisData Is Nothing Then Exit Sub

                    BindDataCustomer()

                    ddlCustomer.SelectedValue = thisData("CustomerId").ToString()
                    txtNumber.Text = thisData("ABNNumber").ToString()
                    txtName.Text = thisData("RegisteredName").ToString()
                    If Not String.IsNullOrEmpty(thisData("RegisteredDate").ToString()) Then
                        txtRegistered.Text = Convert.ToDateTime(thisData("RegisteredDate")).ToString("yyyy-MM-dd")
                    End If

                    If Not String.IsNullOrEmpty(thisData("ExpiryDate").ToString()) Then
                        txtExpiry.Text = Convert.ToDateTime(thisData("ExpiryDate")).ToString("yyyy-MM-dd")
                    End If

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
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

            If txtNumber.Text = "" Then
                MessageError_Process(True, "ABN NUMBER IS REQURIED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtName.Text = "" Then
                MessageError_Process(True, "REGISTERED NUMBER IS REQURIED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerBusiness ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerBusiness VALUES (@Id, @CustomerId, @ABNNumber, @RegisteredName, @RegisteredDate, @ExpiryDate, 0)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                            myCmd.Parameters.AddWithValue("@ABNNumber", txtNumber.Text)
                            myCmd.Parameters.AddWithValue("@RegisteredName", txtName.Text)
                            myCmd.Parameters.AddWithValue("@RegisteredDate", If(String.IsNullOrEmpty(txtRegistered.Text), CType(DBNull.Value, Object), txtRegistered.Text))
                            myCmd.Parameters.AddWithValue("@ExpiryDate", If(String.IsNullOrEmpty(txtExpiry.Text), CType(DBNull.Value, Object), txtExpiry.Text))

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"CustomerBusiness", thisId, Session("LoginId"), "Customer Business Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchCustomerBusiness") = txtSearch.Text
                    Response.Redirect("~/setting/customer/business", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerBusiness SET  CustomerId=@CustomerId, ABNNumber=@ABNNumber, RegisteredName=@RegisteredName, RegisteredDate=@RegisteredDate, ExpiryDate=@ExpiryDate WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                            myCmd.Parameters.AddWithValue("@ABNNumber", txtNumber.Text)
                            myCmd.Parameters.AddWithValue("@RegisteredName", txtName.Text)
                            myCmd.Parameters.AddWithValue("@RegisteredDate", If(String.IsNullOrEmpty(txtRegistered.Text), CType(DBNull.Value, Object), txtRegistered.Text))
                            myCmd.Parameters.AddWithValue("@ExpiryDate", If(String.IsNullOrEmpty(txtExpiry.Text), CType(DBNull.Value, Object), txtExpiry.Text))

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"CustomerBusiness", lblId.Text, Session("LoginId"), "Customer Business Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchCustomerBusiness") = txtSearch.Text
                    Response.Redirect("~/setting/customer/business", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnPrimary_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdPrimary.Text

            Dim customerId As String = settingClass.GetItemData("SELECT CustomerId FROM CustomerBusiness WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerBusiness SET [Primary]=0 WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", customerId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerBusiness SET [Primary]=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            dataLog = {"CustomerBusiness", thisId, Session("LoginId"), "Set As Primary"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerBusiness") = txtSearch.Text
            Response.Redirect("~/setting/customer/business", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerBusiness WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerBusiness' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Session("SearchCustomerBusiness") = txtSearch.Text
            Response.Redirect("~/setting/customer/business", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(searchText As String)
        Session("SearchCustomerBusiness") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not String.IsNullOrEmpty(searchText) Then
                search = "WHERE Customers.Name LIKE '%" & searchText & "%' OR Customers.DebtorCode LIKE '%" & searchText & "%'"
            End If
            Dim thisQuery As String = String.Format("SELECT CustomerBusiness.*, Customers.Name AS CustomerName, CASE WHEN CustomerBusiness.[Primary]=1 THEN 'Yes' WHEN CustomerBusiness.[Primary]=0 THEN 'No' ELSE 'Error' END AS DataPrimary FROM CustomerBusiness LEFT JOIN Customers ON CustomerBusiness.CustomerId=Customers.Id {0} ORDER BY Customers.Id, CustomerBusiness.Id ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID") ' ID
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDataCustomer()
        ddlCustomer.Items.Clear()
        Try
            ddlCustomer.DataSource = settingClass.GetDataTable("SELECT * FROM Customers ORDER BY Name ASC")
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

    Private Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Private Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Function VisiblePrimary(primary As Boolean) As Boolean
        If primary = False Then Return True
        Return False
    End Function

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
