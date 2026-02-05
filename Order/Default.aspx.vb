Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Partial Class Order_Default
    Inherits Page

    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_CancelOrder(False, String.Empty)
            MessageError_ShipmentOrder(False, String.Empty)

            BindStatusOrder()
            BindCompany()

            ddlStatus.SelectedValue = Session("OrderStatus")
            ddlCompany.SelectedValue = Session("OrderCompany")
            txtSearch.Text = Session("OrderSearch")
            ddlActive.SelectedValue = Session("OrderActive")

            BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlActive.SelectedValue)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlStatus_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlActive.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlActive.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "gvList_PageIndexChanging", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Session("OrderSearch") = txtSearch.Text
                    Session("OrderStatus") = ddlStatus.SelectedValue
                    Session("OrderCompany") = ddlCompany.SelectedValue
                    Session("OrderActive") = ddlActive.SelectedValue

                    url = String.Format("~/order/detail?orderid={0}", dataId)

                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT AT SUPPORT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetail_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order/add", False)
    End Sub

    Protected Sub btnRework_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order/rework", False)
    End Sub

    Protected Sub btnFile_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order/file", False)
    End Sub

    Protected Sub btnStatusOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdStatusOrder.Text
            Dim thisStatus As String = txtStatusOrder.Text

            If thisStatus = "Delete Order" Then
                Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId").ToString(), "Order Deleted"}
                orderClass.Logs(dataLog)

                Dim detailData As DataTable = orderClass.GetDataTable("SELECT * FROM OrderDetails WHERE HeaderId='" & thisId & "' AND Active=1 ORDER BY Id ASC")
                If detailData.Rows.Count > 0 Then
                    For i As Integer = 0 To detailData.Rows.Count - 1
                        Dim itemId As String = detailData.Rows(i)("Id").ToString()
                        dataLog = {"OrderDetails", thisId, Session("LoginId").ToString(), "Order Item Deleted | Order Header Deleted"}
                        orderClass.Logs(dataLog)
                    Next
                End If

                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()

                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Active=0 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.ExecuteNonQuery()
                    End Using

                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Active=0 WHERE HeaderId=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.ExecuteNonQuery()
                    End Using

                    thisConn.Close()
                End Using

                Session("OrderStatus") = ddlStatus.SelectedValue
                Session("OrderCompany") = ddlCompany.SelectedValue
                Session("OrderSearch") = txtSearch.Text
                Session("OrderActive") = ddlActive.SelectedValue

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Copy Order" Then
                Dim newIdHeader As String = orderClass.GetNewOrderHeaderId()

                Dim customerId As String = orderClass.GetCustomerIdByOrder(thisId)
                Dim companyAlias As String = orderClass.GetCompanyAliasByCustomer(customerId)

                Dim success As Boolean = False
                Dim retry As Integer = 0
                Dim maxRetry As Integer = 100
                Dim orderId As String = String.Empty

                Do While Not success
                    retry += 1
                    If retry > maxRetry Then
                        Throw New Exception("FAILED TO GENERATE UNIQUE ORDER ID")
                    End If

                    Dim randomCode As String = orderClass.GenerateRandomCode()
                    orderId = companyAlias & randomCode
                    Try
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders SELECT @NewID, @OrderId, CustomerId, 'Copy ' + CAST(@NewID AS VARCHAR(20)) + ' - ' + OrderNumber, 'Copy ' + CAST(@NewID AS VARCHAR(20)) + ' - ' + OrderName, NULL, OrderType, 'Unsubmitted', NULL, CreatedBy, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, 0, 1 FROM OrderHeaders WHERE Id=@OldId; INSERT INTO OrderQuotes VALUES(@NewID, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
                                myCmd.Parameters.AddWithValue("@OldId", thisId)
                                myCmd.Parameters.AddWithValue("@NewID", newIdHeader)
                                myCmd.Parameters.AddWithValue("@OrderId", orderId)
                                myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
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

                Dim dataLog As Object() = {"OrderHeaders", newIdHeader, Session("LoginId").ToString(), "Order Created | Copy"}
                orderClass.Logs(dataLog)

                Dim thisHeader As DataTable = orderClass.GetDataTable("SELECT * FROM OrderDetails WHERE HeaderId='" & thisId & "' AND Active=1")
                If thisHeader.Rows.Count > 0 Then
                    For i As Integer = 0 To thisHeader.Rows.Count - 1
                        Dim itemId As String = thisHeader.Rows(i).Item("Id").ToString()
                        Dim newIdDetail As String = orderClass.GetNewOrderItemId()

                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As New SqlCommand("sp_CopyOrderDetails", thisConn)
                                myCmd.CommandType = CommandType.StoredProcedure

                                myCmd.Parameters.AddWithValue("@ItemIdOld", itemId)
                                myCmd.Parameters.AddWithValue("@NewId", newIdDetail)
                                myCmd.Parameters.AddWithValue("@HeaderId", newIdHeader)

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using


                        orderClass.ResetPriceDetail(newIdHeader, newIdDetail)
                        orderClass.CalculatePrice(newIdHeader, newIdDetail)
                        orderClass.FinalCostItem(newIdHeader, newIdDetail)

                        dataLog = {"OrderDetails", newIdDetail, Session("LoginId").ToString(), "Order Item Added | Copy"}
                        orderClass.Logs(dataLog)
                    Next
                End If

                Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Order/{0}/", orderId))
                If Not Directory.Exists(directoryOrder) Then
                    Directory.CreateDirectory(directoryOrder)
                End If

                url = String.Format("~/order/detail?orderid={0}", newIdHeader)
                Response.Redirect(url, False)
            End If

            If thisStatus = "Unsubmit Order" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET SubmittedDate=NULL, Status='Unsubmitted', DownloadBOE=0 WHERE Id=@Id; DELETE FROM OrderInvoices WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId"), "Order Unsubmitted"}
                orderClass.Logs(dataLog)

                Session("OrderStatus") = ddlStatus.SelectedValue
                Session("OrderCompany") = ddlCompany.SelectedValue
                Session("OrderSearch") = txtSearch.Text
                Session("OrderActive") = ddlActive.SelectedValue

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Production Order" Then
                Dim companyId As String = orderClass.GetCompanyIdByOrder(thisId)
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='In Production', ProductionDate=GETDATE(), OnHoldDate=NULL, DownloadBOE=1 WHERE Id=@Id; INSERT INTO OrderShipments(Id) VALUES (@Id)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId"), "Order In Production"}
                orderClass.Logs(dataLog)

                ' SALES
                If companyId = "2" Then
                    Dim salesClass As New SalesClass
                    salesClass.RefreshData()
                End If

                mailingClass.ProductionOrder(thisId)

                Session("OrderStatus") = ddlStatus.SelectedValue
                Session("OrderCompany") = ddlCompany.SelectedValue
                Session("OrderSearch") = txtSearch.Text
                Session("OrderActive") = ddlActive.SelectedValue

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Hold Order" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='On Hold', OnHoldDate=GETDATE() WHERE Id=@Id; DELETE FROM OrderShipments WHERE Id=@Id;", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId"), "Order On Hold"}
                orderClass.Logs(dataLog)

                Session("OrderStatus") = ddlStatus.SelectedValue
                Session("OrderCompany") = ddlCompany.SelectedValue
                Session("OrderSearch") = txtSearch.Text
                Session("OrderActive") = ddlActive.SelectedValue

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Receive Payment" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()

                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Payment Received' WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.ExecuteNonQuery()
                    End Using

                    Dim amount As Decimal = orderClass.GetItemData_Decimal("SELECT (SUM(SellPrice) * 1.10) AS SumPrice FROM OrderCostings WHERE HeaderId='" & thisId & "' AND Type='Final'")

                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderInvoices SET PaymentDate=GETDATE(), Payment=1, Amount=@Amount WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Amount", amount)
                        myCmd.ExecuteNonQuery()
                    End Using

                    thisConn.Close()
                End Using

                Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId"), "Confirm Payment Received"}
                orderClass.Logs(dataLog)

                Session("OrderStatus") = ddlStatus.SelectedValue
                Session("OrderCompany") = ddlCompany.SelectedValue
                Session("OrderSearch") = txtSearch.Text
                Session("OrderActive") = ddlActive.SelectedValue

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Complete Order" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Completed', CompletedDate=GETDATE() WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId"), "Order Completed"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "BOE Download" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET DownloadBOE=1 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Response.Redirect("~/order", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnStatusOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnCancelOrder_Click(sender As Object, e As EventArgs)
        MessageError_CancelOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showCancelOrder(); };"
        Try
            If txtCancelDescription.Text = "" Then
                MessageError_CancelOrder(True, "DESCRIPTION IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCancelOrder", thisScript, True)
                Exit Sub
            End If

            If msgErrorCancelOrder.InnerText = "" Then
                Dim thisId As String = txtIdCancelOrder.Text

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Canceled', StatusDescription=@StatusDescription, CanceledDate=GETDATE() WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@StatusDescription", txtCancelDescription.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim descLog As String = String.Format("Order Canceled. Reason : {0}", txtCancelDescription.Text.Trim())
                Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId"), descLog}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If
        Catch ex As Exception
            MessageError_CancelOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_CancelOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnCancelOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showCancelOrder", thisScript, True)
        End Try
    End Sub

    Protected Sub btnShipmentOrder_Click(sender As Object, e As EventArgs)
        MessageError_ShipmentOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showShipmentOrder(); };"
        Try
            If txtShipmentNumber.Text = "" Then
                MessageError_ShipmentOrder(True, "SHIPMENT NUMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showShipmentOrder", thisScript, True)
                Exit Sub
            End If

            If txtShipmentDate.Text = "" Then
                MessageError_ShipmentOrder(True, "SHIPMENT DATE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showShipmentOrder", thisScript, True)
                Exit Sub
            End If

            If txtContainerNumber.Text = "" Then
                MessageError_ShipmentOrder(True, "CONTAINER NUMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showShipmentOrder", thisScript, True)
                Exit Sub
            End If

            If txtCourier.Text = "" Then
                MessageError_ShipmentOrder(True, "COURIER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showShipmentOrder", thisScript, True)
                Exit Sub
            End If

            If msgErrorCancelOrder.InnerText = "" Then
                Dim thisId As String = txtIdShipmentOrder.Text

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Shipped Out' WHERE Id=@Id; UPDATE OrderShipments SET ShipmentNumber=@ShipmentNumber, ShipmentDate=@ShipmentDate, ContainerNumber=@ContainerNumber, Courier=@Courier WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@ShipmentNumber", txtShipmentNumber.Text.Trim())
                        myCmd.Parameters.AddWithValue("@ShipmentDate", txtShipmentDate.Text)
                        myCmd.Parameters.AddWithValue("@ContainerNumber", txtContainerNumber.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Courier", txtCourier.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId"), "Order Shipped"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If

        Catch ex As Exception
            MessageError_ShipmentOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ShipmentOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnShipmentOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showShipmentOrder", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitRestore_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdRestore.Text

            Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId").ToString(), "Order Restored"}
            orderClass.Logs(dataLog)

            Dim detailData As DataTable = orderClass.GetDataTable("SELECT * FROM OrderDetails WHERE HeaderId='" & thisId & "' AND Active=0 ORDER BY Id ASC")
            If detailData.Rows.Count > 0 Then
                For i As Integer = 0 To detailData.Rows.Count - 1
                    Dim itemId As String = detailData.Rows(i).Item("Id").ToString()

                    dataLog = {"OrderDetails", thisId, Session("LoginId").ToString(), "Order Item Restored | Order Header Restored"}
                    orderClass.Logs(dataLog)
                Next
            End If

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Active=1 WHERE HeaderId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Session("OrderStatus") = ddlStatus.SelectedValue
            Session("OrderCompany") = ddlCompany.SelectedValue
            Session("OrderSearch") = txtSearch.Text
            Session("OrderActive") = ddlActive.SelectedValue

            Response.Redirect("~/order", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnSubmitRestore_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnPrintDO_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdPrintDO.Text

            Dim suratJalanClass As New SuratJalanClass
            Dim todayString As String = DateTime.Now.ToString("ddMMyyyyHHmmss")
            Dim fileName As String = String.Format("SJ_{0}.pdf", todayString)
            Dim pdfFilePath As String = Server.MapPath("~/File/SuratJalan/" & fileName)

            suratJalanClass.BindContent(thisId, pdfFilePath)

            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=""" & fileName & """")
            Response.TransmitFile(pdfFilePath)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindStatusOrder()
        ddlStatus.Items.Clear()
        Try
            ddlStatus.Items.Add(New ListItem("All Orders", ""))
            If Session("RoleName") = "Developer" Then
                ddlStatus.Items.Add(New ListItem("Unsubmitted", "Unsubmitted"))
                ddlStatus.Items.Add(New ListItem("Quoted", "Quoted"))
                ddlStatus.Items.Add(New ListItem("New Order", "New Order"))
                ddlStatus.Items.Add(New ListItem("Waiting Proforma", "Waiting Proforma"))
                ddlStatus.Items.Add(New ListItem("Proforma Sent", "Proforma Sent"))
                ddlStatus.Items.Add(New ListItem("Payment Received", "Payment Received"))
                ddlStatus.Items.Add(New ListItem("Approved Rework", "Approved Rework"))
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
                ddlStatus.Items.Add(New ListItem("Canceled", "Canceled"))
            End If

            If Session("RoleName") = "IT" Then
                ddlStatus.Items.Add(New ListItem("Unsubmitted", "Unsubmitted"))
                ddlStatus.Items.Add(New ListItem("Quoted", "Quoted"))
                ddlStatus.Items.Add(New ListItem("New Order", "New Order"))
                ddlStatus.Items.Add(New ListItem("Waiting Proforma", "Waiting Proforma"))
                ddlStatus.Items.Add(New ListItem("Proforma Sent", "Proforma Sent"))
                ddlStatus.Items.Add(New ListItem("Payment Received", "Payment Received"))
                ddlStatus.Items.Add(New ListItem("Approved Rework", "Approved Rework"))
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
                ddlStatus.Items.Add(New ListItem("Canceled", "Canceled"))
            End If

            If Session("RoleName") = "Factory Office" Then
                ddlStatus.Items.Add(New ListItem("Unsubmitted", "Unsubmitted"))
                ddlStatus.Items.Add(New ListItem("Quoted", "Quoted"))
                ddlStatus.Items.Add(New ListItem("New Order", "New Order"))
                ddlStatus.Items.Add(New ListItem("Waiting Proforma", "Waiting Proforma"))
                ddlStatus.Items.Add(New ListItem("Proforma Sent", "Proforma Sent"))
                ddlStatus.Items.Add(New ListItem("Payment Received", "Payment Received"))
                ddlStatus.Items.Add(New ListItem("Approved Rework", "Approved Rework"))
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
                ddlStatus.Items.Add(New ListItem("Canceled", "Canceled"))
            End If

            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Account" OrElse Session("RoleName") = "Customer Service" Then
                ddlStatus.Items.Add(New ListItem("Unsubmitted", "Unsubmitted"))
                ddlStatus.Items.Add(New ListItem("Quoted", "Quoted"))
                ddlStatus.Items.Add(New ListItem("New Order", "New Order"))
                ddlStatus.Items.Add(New ListItem("Waiting Proforma", "Waiting Proforma"))
                ddlStatus.Items.Add(New ListItem("Proforma Sent", "Proforma Sent"))
                ddlStatus.Items.Add(New ListItem("Payment Received", "Payment Received"))
                ddlStatus.Items.Add(New ListItem("Approved Rework", "Approved Rework"))
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
                ddlStatus.Items.Add(New ListItem("Canceled", "Canceled"))
            End If

            If Session("RoleName") = "Customer" Then
                ddlStatus.Items.Add(New ListItem("Unsubmitted", "Unsubmitted"))
                ddlStatus.Items.Add(New ListItem("New Order", "New Order"))
                ddlStatus.Items.Add(New ListItem("Waiting Proforma", "Waiting Proforma"))
                ddlStatus.Items.Add(New ListItem("Proforma Sent", "Proforma Sent"))
                ddlStatus.Items.Add(New ListItem("Payment Received", "Payment Received"))
                ddlStatus.Items.Add(New ListItem("Approved Rework", "Approved Rework"))
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
                ddlStatus.Items.Add(New ListItem("Canceled", "Canceled"))
                If Session("CompanyId") = "3" Then
                    ddlStatus.Items.Add(New ListItem("Unsubmitted", "Unsubmitted"))
                    ddlStatus.Items.Add(New ListItem("New Order", "New Order"))
                    ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                    ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                    ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                    ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
                    ddlStatus.Items.Add(New ListItem("Canceled", "Canceled"))
                End If
            End If

            If Session("RoleName") = "Data Entry" Then
                ddlStatus.Items.Add(New ListItem("Unsubmitted", "Unsubmitted"))
                ddlStatus.Items.Add(New ListItem("Quoted", "Quoted"))
                ddlStatus.Items.Add(New ListItem("New Order", "New Order"))
                ddlStatus.Items.Add(New ListItem("Waiting Proforma", "Waiting Proforma"))
                ddlStatus.Items.Add(New ListItem("Proforma Sent", "Proforma Sent"))
                ddlStatus.Items.Add(New ListItem("Payment Received", "Payment Received"))
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
                ddlStatus.Items.Add(New ListItem("Canceled", "Canceled"))
            End If

            If Session("RoleName") = "Export" Then
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
            End If

            If Session("RoleName") = "Installer" Then
                ddlStatus.Items.Add(New ListItem("Quoted", "Quoted"))
                ddlStatus.Items.Add(New ListItem("New Order", "New Order"))
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
            End If
        Catch ex As Exception
            ddlStatus.Items.Clear()
            ddlStatus.Items.Add(New ListItem("All Order", ""))
        End Try
    End Sub

    Protected Sub BindDataOrder(search As String, status As String, company As String, active As String)
        Session("OrderStatus") = String.Empty
        Session("OrderCompany") = String.Empty
        Session("OrderSearch") = String.Empty
        Session("OrderActive") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search),
                New SqlParameter("@Status", status),
                New SqlParameter("@Company", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@CompanyId", Session("CompanyId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString())
            }

            Dim thisData As DataTable = orderClass.GetDataTableSP("sp_OrderList", params)

            gvList.DataSource = thisData
            gvList.DataBind()

            gvList.Columns(1).Visible = PageAction("Visible ID")
            gvList.Columns(3).Visible = PageAction("Visible Customer Name")
            If Session("CustomerLevel") = "Sponsor" AndAlso Session("LevelName") = "Leader" Then
                gvList.Columns(3).Visible = True
            End If
            gvList.Columns(7).Visible = PageAction("Visible Created Date")

            btnAdd.Visible = PageAction("Add")
            btnRework.Visible = PageAction("Rework")
            btnFile.Visible = PageAction("File")

            divActive.Visible = PageAction("Active")
            divCompany.Visible = PageAction("Filter Company")

            If Session("RoleName") = "Customer" Then
                Dim onStop As Boolean = orderClass.GetCustomerOnStop(Session("CustomerId").ToString())
                If onStop = True Then btnAdd.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataOrder", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = orderClass.GetDataTable("SELECT * FROM Companys WHERE Id<>'1' ORDER BY Name ASC")
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            ddlCompany.Items.Insert(0, New ListItem("All", ""))
        Catch ex As Exception
            ddlCompany.Items.Clear()
            ddlCompany.Items.Add(New ListItem("All", ""))
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_CancelOrder(visible As Boolean, message As String)
        divErrorCancelOrder.Visible = visible : msgErrorCancelOrder.InnerText = message
    End Sub

    Protected Sub MessageError_ShipmentOrder(visible As Boolean, message As String)
        divErrorShipmentOrder.Visible = visible : msgErrorShipmentOrder.InnerText = message
    End Sub

    Protected Function VisibleDelete(data As Object) As Boolean
        Dim active As Boolean = Convert.ToBoolean(data(0))
        Dim status As String = Convert.ToString(data(1))
        Dim createdBy As String = Convert.ToString(data(2))
        Dim createdRole As String = Convert.ToString(data(3))

        If active = True Then
            If Session("RoleName") = "Developer" Then Return True

            If Session("RoleName") = "IT" Then
                If status = "Unsubmitted" Then Return True
            End If

            If Session("RoleName") = "Factory Office" Then
                If status = "Unsubmitted" Then Return True
            End If

            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Customer Service" OrElse Session("RoleName") = "Data Entry" Then
                If status = "Unsubmitted" Then
                    If createdBy = Session("LoginId").ToString() OrElse createdRole = Session("RoleId") Then
                        Return True
                    End If
                End If
            End If

            If Session("RoleName") = "Customer" Then
                If status = "Unsubmitted" Then Return True
            End If
        End If
        Return False
    End Function

    Protected Function VisibleCopy(active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" Then Return True
            If Session("RoleName") = "IT" Then Return True
            If Session("RoleName") = "Factory Office" Then Return True
            If Session("RoleName") = "Customer Service" Then Return True
            If Session("RoleName") = "Data Entry" Then Return True
            If Session("RoleName") = "Customer" Then Return True
        End If
        Return False
    End Function

    Protected Function VisibleRestore(active As Boolean) As Boolean
        If Session("RoleName") = "Developer" AndAlso Session("LevelName") = "Leader" Then
            If active = False Then Return True
        End If
        Return False
    End Function

    Protected Function VisibleUnsubmitOrder(status As String, active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (status = "New Order" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
        End If
        Return False
    End Function

    Protected Function VisibleReceivePayment(status As String, active As Boolean) As Boolean
        If active = True AndAlso status = "Proforma Sent" AndAlso (Session("RoleName") = "Developer" OrElse Session("RoleName") = "Representative" OrElse Session("RoleName") = "Account") Then
            Return True
        End If
        Return False
    End Function

    Protected Function VisibleProductionOrder(status As String, active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (status = "New Order" OrElse status = "Payment Received") Then Return True
        End If
        Return False
    End Function

    Protected Function VisibleHoldOrder(status As String, active As Boolean) As Boolean
        If active = True AndAlso status = "In Production" AndAlso (Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office") Then Return True
        Return False
    End Function

    Protected Function VisibleCancelOrder(status As String, active As Boolean) As Boolean
        If active = True Then
            If status = "New Order" AndAlso (Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Customer Service") Then Return True
            If status = "In Production" AndAlso (Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Customer Service") Then Return True
            If status = "On Hold" AndAlso (Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Customer Service") Then Return True
            If status = "Waiting Proforma" AndAlso (Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Account") Then Return True
            If status = "Proforma Sent" AndAlso (Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Account") Then Return True
        End If
        Return False
    End Function

    Protected Function VisibleShipmentOrder(status As String, active As Boolean) As Boolean
        If active = True AndAlso status = "In Production" Then
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Export" Then
                Return True
            End If
        End If
        Return False
    End Function

    Protected Function VisibleCompleteOrder(status As String, active As Boolean) As Boolean
        If active = True AndAlso status = "Shipped Out" AndAlso Session("RoleName") = "Developer" Then Return True
        Return False
    End Function

    Protected Function VisibleBOEOrder(status As String, active As Boolean) As Boolean
        If active = True AndAlso status = "Unsubmitted" AndAlso Session("RoleName") = "Developer" Then Return True
        Return False
    End Function

    Protected Function VisiblePrintDO(company As String, status As String, active As Boolean) As Boolean
        If active = True AndAlso (company = "3" OrElse company = "5") Then
            If Session("RoleName") = "Developer" Then Return True
            If Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Export" Then
                If status = "New Order" Then Return True
                If status = "In Production" Then Return True
                If status = "On Hold" Then Return True
                If status = "Shipped Out" Then Return True
            End If
        End If
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
