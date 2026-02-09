Imports System.Data
Imports System.Data.SqlClient

Partial Class Order_Edit
    Inherits Page

    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("orderidedit")) Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        lblHeaderId.Text = Request.QueryString("orderidedit").ToString()
        If Not IsPostBack Then
            BackColor()
            BindDataHeader(lblHeaderId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
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

            If InStr(txtOrderNumber.Text, "\") > 0 Or InStr(txtOrderNumber.Text, "/") > 0 Or InStr(txtOrderNumber.Text, ",") > 0 Or InStr(txtOrderNumber.Text, "&") > 0 Or InStr(txtOrderNumber.Text, ",") > 0 Or InStr(txtOrderNumber.Text, "#") > 0 Or InStr(txtOrderNumber.Text, "'") > 0 Or InStr(txtOrderNumber.Text, ".") > 0 Then
                MessageError(True, "PLEASE DON'T USE [ / ], [ \ ], [ & ], [ # ], [ ' ], [ . ] AND [ , ]")
                txtOrderNumber.BackColor = Drawing.Color.Red
                txtOrderNumber.Focus()
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

            If InStr(txtOrderName.Text, "\") > 0 Or InStr(txtOrderName.Text, "/") > 0 Or InStr(txtOrderName.Text, ",") > 0 Or InStr(txtOrderName.Text, "&") > 0 Or InStr(txtOrderName.Text, ",") > 0 Or InStr(txtOrderName.Text, "#") > 0 Or InStr(txtOrderName.Text, "'") > 0 Or InStr(txtOrderName.Text, ".") > 0 Then
                MessageError(True, "PLEASE DON'T USE [ / ], [ \ ], [ & ], [ # ], [ ' ], [ . ] AND [ , ]")
                txtOrderName.BackColor = Drawing.Color.Red
                txtOrderName.Focus()
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
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET OrderId=@OrderId, CustomerId=@CustomerId, OrderNumber=@OrderNumber, OrderName=@OrderName, OrderNote=@OrderNote, OrderType=@OrderType WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@OrderId", txtOrderId.Text)
                        myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                        myCmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumber.Text.Trim())
                        myCmd.Parameters.AddWithValue("@OrderName", txtOrderName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@OrderNote", txtOrderNote.Text.Trim())
                        myCmd.Parameters.AddWithValue("@OrderType", ddlOrderType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@CreatedBy", ddlCreatedBy.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"OrderHeaders", lblHeaderId.Text, Session("LoginId").ToString(), "Order Updated"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnSubmit_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindDataHeader(headerId As String)
        Try
            Dim thisQuery As String = "SELECT * FROM OrderHeaders WHERE Id='" & headerId & "'"
            If Session("RoleName") = "Customer" Then
                thisQuery = "SELECT * FROM OrderHeaders WHERE Id='" & headerId & "' AND CustomerId='" & Session("CustomerId").ToString() & "'"
            End If
            Dim myData As DataRow = orderClass.GetDataRow(thisQuery)
            If myData Is Nothing Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            Dim statusOrder As String = myData("Status").ToString()
            If (Session("RoleName") = "Customer") AndAlso (statusOrder = "In Production" OrElse statusOrder = "Canceled" Or statusOrder = "Completed") Then
                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
                Exit Sub
            End If

            BindDataCustomer()
            BindDataUser()

            ddlCustomer.SelectedValue = myData("CustomerId").ToString()
            ddlCreatedBy.SelectedValue = myData("CreatedBy").ToString()
            txtCreatedDate.Text = Convert.ToDateTime(myData("CreatedDate")).ToString("yyyy-MM-dd")
            txtOrderNumber.Text = myData("OrderNumber").ToString()
            lblOrderNo.Text = myData("OrderNumber").ToString()

            txtOrderName.Text = myData("OrderName").ToString()
            txtOrderNote.Text = myData("OrderNote").ToString()
            txtOrderId.Text = myData("OrderId").ToString()
            ddlOrderType.SelectedValue = myData("OrderType").ToString()

            divCustomer.Visible = False
            divCreatedBy.Visible = False
            divCreatedDate.Visible = False
            divOrderType.Visible = False

            ddlCustomer.Enabled = False
            ddlCreatedBy.Enabled = False
            txtCreatedDate.Enabled = False

            txtOrderId.Enabled = False
            ddlCustomer.Enabled = False
            ddlCreatedBy.Enabled = False
            txtCreatedDate.Enabled = False

            If Session("RoleName") = "Developer" Then
                divCustomer.Visible = True
                divCreatedBy.Visible = True
                divCreatedDate.Visible = True
                divOrderType.Visible = True

                ddlCustomer.Enabled = True
                ddlCreatedBy.Enabled = True
                txtCreatedDate.Enabled = True

                txtOrderId.Enabled = True
                ddlCustomer.Enabled = True
                ddlCreatedBy.Enabled = True
                txtCreatedDate.Enabled = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataHeader", ex.ToString()}
                mailingClass.WebError(dataMailing)
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
            dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataCustomer", ex.ToString()}
            mailingClass.WebError(dataMailing)
        End Try
    End Sub

    Protected Sub BindDataUser()
        ddlCreatedBy.Items.Clear()
        Try
            ddlCreatedBy.DataSource = orderClass.GetDataTable("SELECT * FROM CustomerLogins ORDER BY UserName ASC")
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
            dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataLogin", ex.ToString()}
            mailingClass.WebError(dataMailing)
        End Try
    End Sub

    Protected Sub BackColor()
        MessageError(False, String.Empty)

        ddlCustomer.BackColor = Drawing.Color.Empty
        txtOrderNumber.BackColor = Drawing.Color.Empty
        txtOrderName.BackColor = Drawing.Color.Empty
        txtOrderNote.BackColor = Drawing.Color.Empty
        ddlOrderType.BackColor = Drawing.Color.Empty
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
