Imports System.Data
Imports System.Data.SqlClient

Partial Class Order_Edit
    Inherits Page

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim returnPage As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("orderid")) Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            returnPage = Request.QueryString("returnpage").ToString()
        End If

        lblHeaderId.Text = Request.QueryString("orderid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDataHeader(lblHeaderId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                ddlCustomer.BackColor = Drawing.Color.Red
                ddlCustomer.Focus()
                Exit Sub
            End If
            If txtOrderNumber.Text = "" Then
                MessageError(True, "ORDER NUMBER IS REQUIRED !")
                txtOrderNumber.BackColor = Drawing.Color.Red
                txtOrderNumber.Focus()
                Exit Sub
            End If
            If InStr(txtOrderNumber.Text, ",") > 0 OrElse InStr(txtOrderNumber.Text, "'") > 0 OrElse InStr(txtOrderNumber.Text, ";") > 0 Then
                MessageError(True, "PLEASE DON'T USE [ , ], [ ' ] AND [ ; ] !")
                Exit Sub
            End If
            If Trim(txtOrderNumber.Text).Length > 20 Then
                MessageError(True, "MAXIMUM 20 CHARACTERS FOR RETAILER ORDER NUMBER !")
                txtOrderNumber.BackColor = Drawing.Color.Red
                txtOrderNumber.Focus()
                Exit Sub
            End If
            If txtOrderName.Text = "" Then
                MessageError(True, "ORDER NAME IS REQUIRED !")
                txtOrderName.BackColor = Drawing.Color.Red
                txtOrderName.Focus()
                Exit Sub
            End If
            If InStr(txtOrderName.Text, ",") > 0 OrElse InStr(txtOrderName.Text, "'") > 0 OrElse InStr(txtOrderName.Text, ";") > 0 Then
                MessageError(True, "PLEASE DON'T USE [ , ], [ ' ] AND [ ; ] !")
                Exit Sub
            End If
            If txtOrderNumber.Text <> lblOrderNo.Text Then
                If txtOrderNumber.Text = orderClass.IsOrderExist(ddlCustomer.SelectedValue, txtOrderNumber.Text.Trim()) Then
                    MessageError(True, "ORDER NUMBER ALREADY EXISTS !")
                    txtOrderNumber.BackColor = Drawing.Color.Red
                    txtOrderNumber.Focus()
                    Exit Sub
                End If
            End If

            If msgError.InnerText = "" Then
                Dim orderFactory As String = String.Empty
                If Not String.IsNullOrEmpty(lbOrderFactory.SelectedValue) Then
                    orderFactory = String.Join(",", lbOrderFactory.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET OrderId=@OrderId, CustomerId=@CustomerId, OrderNumber=@OrderNumber, OrderName=@OrderName, OrderNote=@OrderNote, OrderType=@OrderType, OrderFactory=@OrderFactory, CreatedBy=@CreatedBy WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        thisCmd.Parameters.AddWithValue("@OrderId", txtOrderId.Text)
                        thisCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumber.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@OrderName", txtOrderName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@OrderNote", txtOrderNote.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@OrderType", ddlOrderType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@OrderFactory", orderFactory)
                        thisCmd.Parameters.AddWithValue("@CreatedBy", ddlCreatedBy.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"OrderHeaders", lblHeaderId.Text, Session("LoginId").ToString(), "Order Updated"}
                orderClass.Logs(dataLog)

                url = "~/order/"
                If returnPage = "detail" Then
                    url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                End If
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
        url = "~/order/"
        If returnPage = "detail" Then
            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindDataHeader(headerId As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@HeaderId", CInt(headerId)),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", If(Session("LevelName"), DBNull.Value)),
                New SqlParameter("@CompanyId", If(Session("CompanyId"), DBNull.Value)),
                New SqlParameter("@LoginId", If(Session("LoginId"), DBNull.Value)),
                New SqlParameter("@CustomerId", If(Session("CustomerId"), DBNull.Value)),
                New SqlParameter("@CustomerLevel", If(Session("CustomerLevel"), DBNull.Value))
            }
            Dim headerData As DataRow = orderClass.GetDataRowSP("sp_GetOrderHeaderById", params)
            If headerData Is Nothing Then
                url = "~/order/"
                If returnPage = "detail" Then
                    url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                End If
                Response.Redirect(url, False)
                Exit Sub
            End If

            Dim statusOrder As String = headerData("Status").ToString()
            If Session("RoleName") = "Customer" AndAlso Not statusOrder = "Unsubmitted" Then
                url = "~/order/"
                If returnPage = "detail" Then
                    url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                End If
                Response.Redirect(url, False)
            End If

            If Session("RoleName") = "IT" AndAlso (statusOrder = "Shipped Out" OrElse statusOrder = "Completed" OrElse statusOrder = "Canceled") Then
                url = "~/order/"
                If returnPage = "detail" Then
                    url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                End If
                Response.Redirect(url, False)
                Exit Sub
            End If

            If Session("RoleName") = "Factory Office" AndAlso (statusOrder = "Shipped Out" OrElse statusOrder = "Completed" OrElse statusOrder = "Canceled") Then
                url = "~/order/"
                If returnPage = "detail" Then
                    url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                End If
                Response.Redirect(url, False)
                Exit Sub
            End If

            If Session("RoleName") = "Account" AndAlso (statusOrder = "Unsubmitted" OrElse statusOrder = "In Production" OrElse statusOrder = "On Hold" OrElse statusOrder = "Shipped Out" OrElse statusOrder = "Completed" OrElse statusOrder = "Canceled") Then
                url = "~/order/"
                If returnPage = "detail" Then
                    url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                End If
                Response.Redirect(url, False)
                Exit Sub
            End If

            If Session("RoleName") = "Sales" AndAlso Not statusOrder = "Unsubmitted" Then
                url = "~/order/"
                If returnPage = "detail" Then
                    url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                End If
                Response.Redirect(url, False)
                Exit Sub
            End If

            BindDataCustomer()
            BindDataUser()

            ddlCustomer.SelectedValue = headerData("CustomerId").ToString()
            ddlCreatedBy.SelectedValue = headerData("CreatedBy").ToString()
            txtOrderNumber.Text = headerData("OrderNumber").ToString()
            lblOrderNo.Text = headerData("OrderNumber").ToString()

            txtOrderName.Text = headerData("OrderName").ToString()
            txtOrderNote.Text = headerData("OrderNote").ToString()
            txtOrderId.Text = headerData("OrderId").ToString()
            ddlOrderType.SelectedValue = headerData("OrderType").ToString()
            If Not headerData("OrderFactory").ToString() = "" Then
                Dim factoryArray() As String = headerData("OrderFactory").ToString().Split(",")
                For Each i In factoryArray
                    If Not String.IsNullOrEmpty(i) Then
                        Dim item = lbOrderFactory.Items.FindByValue(i)
                        If item IsNot Nothing Then
                            item.Selected = True
                        End If
                    End If
                Next
            End If

            divCustomer.Visible = False
            divCreatedBy.Visible = False
            divOrderType.Visible = False
            divOrderFactory.Visible = False

            ddlCustomer.Enabled = False
            ddlCreatedBy.Enabled = False

            txtOrderId.Enabled = False
            ddlCustomer.Enabled = False
            ddlCreatedBy.Enabled = False

            If Session("RoleName") = "Developer" Then
                divCustomer.Visible = True
                divCreatedBy.Visible = True
                divOrderType.Visible = True
                divOrderFactory.Visible = True

                ddlCustomer.Enabled = True
                ddlCreatedBy.Enabled = True

                txtOrderId.Enabled = True
                ddlCustomer.Enabled = True
                ddlCreatedBy.Enabled = True
            End If

            If Session("RoleName") = "IT" Then
                divCustomer.Visible = True
                divCreatedBy.Visible = True
                divOrderType.Visible = True
                divOrderFactory.Visible = True

                ddlCustomer.Enabled = True
                ddlCreatedBy.Enabled = True

                txtOrderId.Enabled = True
                If statusOrder = "In Production" OrElse statusOrder = "On Hold" Then
                    txtOrderId.Enabled = False
                End If
                ddlCustomer.Enabled = False
                ddlCreatedBy.Enabled = False
            End If

            If Session("RoleName") = "Factory Office" Then
                divCustomer.Visible = True
                divCreatedBy.Visible = True
                ddlCustomer.Enabled = False
                ddlCreatedBy.Enabled = False
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
            Dim byRole As String = String.Empty
            If Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Production" Then byRole = " AND Id<>'1' AND Id<>'2'"
            If Session("RoleName") = "Sales" Then
                byRole = "AND Customers.CompanyId='" & Session("CompanyId") & "'"
                If Session("LevelName") = "Member" Then
                    byRole = "AND Customers.Operator='" & Session("LoginId") & "'"
                End If
            End If
            If Session("RoleName") = "Account" Then
                byRole = "AND Customers.CompanyId='" + Session("CompanyId") + "'"
            End If

            Dim thisQuery As String = String.Format("SELECT * FROM Customers WHERE Active=1 {0} ORDER BY Name ASC", byRole)

            ddlCustomer.DataSource = orderClass.GetDataTable(thisQuery)
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

    Protected Sub BindDataUser()
        ddlCreatedBy.Items.Clear()
        Try
            ddlCreatedBy.DataSource = orderClass.GetDataTable("SELECT * FROM Logins ORDER BY UserName ASC")
            ddlCreatedBy.DataTextField = "UserName"
            ddlCreatedBy.DataValueField = "Id"
            ddlCreatedBy.DataBind()

            If ddlCreatedBy.Items.Count > 0 Then
                ddlCreatedBy.Items.Insert(0, New ListItem("", ""))
            End If

            ddlCreatedBy.SelectedValue = Session("LoginId")
        Catch ex As Exception
            ddlCreatedBy.Items.Clear()
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
