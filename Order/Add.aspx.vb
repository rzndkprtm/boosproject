Imports System.Data.SqlClient
Imports System.IO

Partial Class Order_Add
    Inherits Page

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        If Session("RoleName") = "Customer" Then
            Dim status As Boolean = orderClass.GetCustomerOnStop(Session("CustomerId").ToString())
            If status = True Then
                Response.Redirect("~/order/", False)
                Exit Sub
            End If
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDataCustomer()
            BindComponentForm(ddlCustomer.SelectedValue)
        End If
    End Sub

    Protected Sub ddlCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindComponentForm(ddlCustomer.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim companyAlias As String = orderClass.GetCompanyAliasByCustomer(ddlCustomer.SelectedValue)
            Dim companyDetailName As String = orderClass.GetCompanyDetailNameByCustomer(ddlCustomer.SelectedValue)

            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                Exit Sub
            End If
            If txtOrderNumber.Text = "" Then
                MessageError(True, "ORDER NUMBER IS REQUIRED !")
                Exit Sub
            End If
            If InStr(txtOrderNumber.Text, ",") > 0 OrElse InStr(txtOrderNumber.Text, "'") > 0 OrElse InStr(txtOrderNumber.Text, ";") > 0 Then
                MessageError(True, "PLEASE DON'T USE [ , ], [ ' ] AND [ ; ] !")
                Exit Sub
            End If
            If Trim(txtOrderNumber.Text).Length > 20 Then
                MessageError(True, "MAXIMUM 20 CHARACTERS FOR RETAILER ORDER NUMBER !")
                Exit Sub
            End If
            If txtOrderName.Text = "" Then
                MessageError(True, "ORDER NAME IS REQUIRED !")
                Exit Sub
            End If
            If InStr(txtOrderName.Text, ",") > 0 OrElse InStr(txtOrderName.Text, "'") > 0 OrElse InStr(txtOrderName.Text, ";") > 0 OrElse InStr(txtOrderName.Text, ".") > 0 Then
                MessageError(True, "PLEASE DON'T USE [ , ], [ ' ] AND [ ; ] !")
                Exit Sub
            End If
            If txtOrderNumber.Text = orderClass.IsOrderExist(ddlCustomer.SelectedValue, txtOrderNumber.Text.Trim()) Then
                MessageError(True, "ORDER NUMBER ALREADY EXISTS !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = orderClass.GetNewOrderHeaderId()

                Dim success As Boolean = False
                Dim retry As Integer = 0
                Dim maxRetry As Integer = 100
                Dim orderId As String = ""

                Do While Not success
                    retry += 1
                    If retry > maxRetry Then
                        Throw New Exception("FAILED TO GENERATE UNIQUE ORDER ID")
                    End If

                    Dim randomCode As String = orderClass.GenerateRandomCode()
                    orderId = companyAlias & randomCode
                    Try
                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, Payment, Amount, Download, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, @OrderType, 'Unsubmitted', @CreatedBy, GETDATE(), 0, 0, 'No', 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
                                thisCmd.Parameters.AddWithValue("@Id", thisId)
                                thisCmd.Parameters.AddWithValue("@OrderId", orderId)
                                thisCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                                thisCmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumber.Text.Trim())
                                thisCmd.Parameters.AddWithValue("@OrderName", txtOrderName.Text.Trim())
                                thisCmd.Parameters.AddWithValue("@OrderNote", txtOrderNote.Text.Trim())
                                thisCmd.Parameters.AddWithValue("@OrderType", ddlOrderType.SelectedValue)
                                thisCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())
                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
                            End Using
                        End Using
                        success = True
                    Catch exSql As SqlException
                        If exSql.Number = 2601 OrElse exSql.Number = 2627 Then
                            success = False
                        Else
                            Throw
                        End If
                    End Try
                Loop

                If ddlOrderType.SelectedValue = "Builder" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As New SqlCommand("INSERT INTO OrderBuilders(Id) VALUES (@Id)", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                End If

                Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Order/{0}/", orderId))
                If Not Directory.Exists(directoryOrder) Then
                    Directory.CreateDirectory(directoryOrder)
                End If

                Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId").ToString(), "Order Created"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", thisId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order/", False)
    End Sub

    Protected Sub BindComponentForm(customerId As String)
        Try
            divCustomer.Visible = False
            divOrderType.Visible = False

            Dim companyDetailName As String = String.Empty
            If Not String.IsNullOrEmpty(customerId) Then
                companyDetailName = orderClass.GetCompanyDetailNameByCustomer(ddlCustomer.SelectedValue)
            End If

            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Sales" OrElse Session("RoleName") = "Data Entry" Then
                divCustomer.Visible = True
                If companyDetailName = "JPMD BP" Then divOrderType.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub BindDataCustomer()
        ddlCustomer.Items.Clear()
        Try
            Dim splitArray As String = String.Empty
            Dim role As String = String.Empty

            If Session("RoleName") = "Customer" Then role = "AND Id='" & Session("CustomerId") & "'"
            If Session("RoleName") = "Sales" Then
                role = "AND CompanyId='" & Session("CompanyId").ToString() & "'"
                If Session("LevelName") = "Member" Then
                    role = "AND (Id = '" & Session("CustomerId") & "' OR EXISTS (SELECT 1 FROM STRING_SPLIT(Operator, ',') WHERE value = '" & Session("LoginId") & "'))"
                End If
            End If

            ddlCustomer.DataSource = orderClass.GetDataTable(String.Format("SELECT * FROM Customers WHERE Active=1 {0} ORDER BY Name ASC", role))
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If

            ddlCustomer.SelectedValue = Session("CustomerId").ToString()
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
