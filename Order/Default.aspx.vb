Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Threading.Tasks

Partial Class Order_Default
    Inherits Page

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_DuplicateOrder(False, String.Empty)
            MessageError_CancelOrder(False, String.Empty)
            MessageError_ShipmentOrder(False, String.Empty)

            BindStatusOrder()
            BindCompany()
            BindOrderType()

            ddlStatus.SelectedValue = Session("OrderStatus")
            If Not String.IsNullOrEmpty(Session("OrderCompany")) Then
                ddlCompany.SelectedValue = Session("OrderCompany")
            End If
            txtSearch.Text = Session("OrderSearch")
            ddlActive.SelectedValue = Session("OrderActive")
            ddlType.SelectedValue = Session("OrderType")

            BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlType.SelectedValue, ddlActive.SelectedValue)
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

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlType.SelectedValue, ddlActive.SelectedValue)

        Session("OrderStatus") = ddlStatus.SelectedValue
        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderActive") = ddlActive.SelectedValue
        Session("OrderType") = ddlType.SelectedValue
    End Sub

    Protected Sub ddlStatus_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlType.SelectedValue, ddlActive.SelectedValue)

        Session("OrderStatus") = ddlStatus.SelectedValue
        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderActive") = ddlActive.SelectedValue
        Session("OrderType") = ddlType.SelectedValue
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlType.SelectedValue, ddlActive.SelectedValue)

        Session("OrderStatus") = ddlStatus.SelectedValue
        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderActive") = ddlActive.SelectedValue
        Session("OrderType") = ddlType.SelectedValue
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlType.SelectedValue, ddlActive.SelectedValue)

        Session("OrderStatus") = ddlStatus.SelectedValue
        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderActive") = ddlActive.SelectedValue
        Session("OrderType") = ddlType.SelectedValue
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlType.SelectedValue, ddlActive.SelectedValue)

        Session("OrderStatus") = ddlStatus.SelectedValue
        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderActive") = ddlActive.SelectedValue
        Session("OrderType") = ddlType.SelectedValue
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName = "Page" Then
                gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
                BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlType.SelectedValue, ddlActive.SelectedValue)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindDataOrder(txtSearch.Text, ddlStatus.SelectedValue, ddlCompany.SelectedValue, ddlType.SelectedValue, ddlActive.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        Try
            BuildPager()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub btnStatusOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtStatusOrderId.Text
            Dim thisStatus As String = txtStatusOrderNew.Text
            Dim thisOldStatus As String = txtStatusOrderOld.Text
            Dim companyId As String = orderClass.GetCompanyIdByOrder(thisId)

            If thisStatus = "Delete Order" Then
                dataLog = {"OrderHeaders", thisId, Session("LoginId").ToString(), "Order Deleted"}
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
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Active=0, Download='No' WHERE Id=@Id; UPDATE OrderDetails SET Active=0 WHERE HeaderId=@Id;", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Unsubmit Order" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET SubmittedDate=NULL, ProductionDate=NULL, OnHoldDate=NULL, Status='Unsubmitted', Download='No', DownloadDate=NULL, ShipmentDate=NULL, ShipmentNumber=NULL, ContainerNumber=NULL, ContainerETA=NULL, Courier=NULL, InvoiceNumber=NULL, Collector=NULL, InvoiceDate=NULL, DueDate=NULL, Payment=0, PaymentDate=NULL, Amount=0 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim salesClass As New SalesClass
                salesClass.RefreshData(companyId)

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "Order Unsubmitted"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "New Order" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='New Order' WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "New Order"}
                orderClass.Logs(dataLog)

                Dim mailingClass As New MailingClass
                mailingClass.NewOrder(thisId)

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Production Order" Then
                Dim stringQuery As String = "UPDATE OrderHeaders SET Status='In Production', OnHoldDate=NULL WHERE Id=@Id;"
                If thisOldStatus = "New Order" OrElse thisOldStatus = "Payment Received" Then
                    stringQuery = "UPDATE OrderHeaders SET Status='In Production', ProductionDate=GETDATE(), OnHoldDate=NULL, Download='Yes' WHERE Id=@Id;"
                End If
                If thisOldStatus = "Shipped Out" Then
                    stringQuery = "UPDATE OrderHeaders SET Status='In Production', ShipmentNumber=NULL, ShipmentDate=NULL, ContainerNumber=NULL, ContainerETA=NULL, Courier=NULL WHERE Id=@Id"
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand(stringQuery, thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim salesClass As New SalesClass
                salesClass.RefreshData(companyId)

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "Order In Production"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Hold Order" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='On Hold', OnHoldDate=GETDATE() WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "Order On Hold"}
                orderClass.Logs(dataLog)

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
                    If companyId = "3" Then
                        amount = orderClass.GetItemData_Decimal("SELECT (SUM(SellPrice) * 1.10) AS SumPrice FROM OrderCostings WHERE HeaderId='" & thisId & "' AND Type='Final'")
                    End If

                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET PaymentDate=GETDATE(), DueDate=NULL, Payment=1, Amount=@Amount WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Amount", amount)
                        myCmd.ExecuteNonQuery()
                    End Using

                    thisConn.Close()
                End Using

                Dim checkOcean As Integer = orderClass.GetItemData_Integer("SELECT COUNT(OrderDetails.Id) FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & thisId & "' AND OrderDetails.Active=1 AND Products.DesignId='15'")
                If checkOcean > 0 Then
                    Task.Run(Async Function()
                                 Dim svc As New ShutterOceanService()
                                 Await svc.SendOrderAsync(thisId)
                             End Function)
                End If

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "Confirm Payment Received"}
                orderClass.Logs(dataLog)

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

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "Order Completed"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Download BOE" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Download='Yes', DownloadDate=NULL WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Surat Jalan" Then
                Dim suratClass As New SuratClass
                Dim pdfBytes As Byte() = suratClass.BindContent(thisId)

                Dim orderId As String = orderClass.GetItemData("SELECT OrderId FROM OrderHeaders WHERE Id='" & thisId & "'")

                Dim fileName As String = String.Format("SURAT JALAN {0}.pdf", orderId)

                Response.Clear()
                Response.ContentType = "application/pdf"
                Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & "")
                Response.BinaryWrite(pdfBytes)
                Response.Flush()
                Response.End()
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

    Protected Sub btnDuplicateOrder_Click(sender As Object, e As EventArgs)
        MessageError_DuplicateOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showDuplicateOrder(); };"
        Try
            Dim thisId As String = txtDuplicateOrderId.Text
            Dim thisCustomerId As String = txtDuplicateOrderCustomerId.Text

            If txtOrderNumberNew.Text = "" Then
                MessageError_DuplicateOrder(True, "ORDER NUMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showDuplicateOrder", thisScript, True)
                Exit Sub
            End If
            If InStr(txtOrderNumberNew.Text, ",") > 0 OrElse InStr(txtOrderNumberNew.Text, "'") > 0 OrElse InStr(txtOrderNumberNew.Text, ";") > 0 Then
                MessageError_DuplicateOrder(True, "PLEASE DON'T USE [ , ], [ ' ] AND [ ; ] !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showDuplicateOrder", thisScript, True)
                Exit Sub
            End If
            If txtOrderNumberNew.Text = orderClass.IsOrderExist(thisCustomerId, txtOrderNumberNew.Text.Trim()) Then
                MessageError_DuplicateOrder(True, "ORDER NUMBER ALREADY EXISTS !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showDuplicateOrder", thisScript, True)
                Exit Sub
            End If
            If txtOrderNameNew.Text = "" Then
                MessageError_DuplicateOrder(True, "ORDER NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showDuplicateOrder", thisScript, True)
                Exit Sub
            End If
            If InStr(txtOrderNameNew.Text, ",") > 0 OrElse InStr(txtOrderNameNew.Text, "'") > 0 OrElse InStr(txtOrderNameNew.Text, ";") > 0 OrElse InStr(txtOrderNameNew.Text, ".") > 0 Then
                MessageError_DuplicateOrder(True, "PLEASE DON'T USE [ , ], [ ' ] AND [ ; ] !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showDuplicateOrder", thisScript, True)
                Exit Sub
            End If

            If msgErrorDuplicateOrder.InnerText = "" Then
                Dim newIdHeader As String = orderClass.GetNewOrderHeaderId()
                Dim companyAlias As String = orderClass.GetCompanyAliasByCustomer(thisCustomerId)

                Dim orderType As String = orderClass.GetItemData("SELECT OrderType FROM OrderHeaders WHERE Id='" & thisId & "'")

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
                            thisConn.Open()

                            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders SELECT @NewID, @OrderId, CustomerId, @OrderNumber, @OrderName, @OrderNote, OrderType, OrderFactory, 'Unsubmitted', NULL, @CreatedBy, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, 0, 'No', NULL, 1 FROM OrderHeaders WHERE Id=@OldId; INSERT INTO OrderQuotes VALUES(@NewID, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
                                myCmd.Parameters.AddWithValue("@OldId", thisId)
                                myCmd.Parameters.AddWithValue("@NewID", newIdHeader)
                                myCmd.Parameters.AddWithValue("@OrderId", orderId)
                                myCmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumberNew.Text.Trim())
                                myCmd.Parameters.AddWithValue("@OrderName", txtOrderNameNew.Text.Trim())
                                myCmd.Parameters.AddWithValue("@OrderNote", txtOrderNoteNew.Text)
                                myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())

                                myCmd.ExecuteNonQuery()
                            End Using

                            If orderType = "Builder" Then
                                Using myCmd As New SqlCommand("INSERT INTO OrderBuilders(Id) VALUES (@Id)", thisConn)
                                    myCmd.Parameters.AddWithValue("@Id", newIdHeader)

                                    myCmd.ExecuteNonQuery()
                                End Using
                            End If

                            thisConn.Close()
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

                dataLog = {"OrderHeaders", newIdHeader, Session("LoginId").ToString(), "Order Created | Copy"}
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
        Catch ex As Exception
            MessageError_DuplicateOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_DuplicateOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showDuplicateOrder", thisScript, True)
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

            If msgErrorShipmentOrder.InnerText = "" Then
                Dim thisId As String = txtShipmentOrderId.Text

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Shipped Out', ShipmentNumber=@ShipmentNumber, ShipmentDate=@ShipmentDate, ContainerNumber=@ContainerNumber, ContainerETA=@ContainerETA, Courier=@Courier WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@ShipmentNumber", txtShipmentNumber.Text.Trim())
                        myCmd.Parameters.AddWithValue("@ShipmentDate", If(String.IsNullOrEmpty(txtShipmentDate.Text), CType(DBNull.Value, Object), txtShipmentDate.Text))
                        myCmd.Parameters.AddWithValue("@ContainerNumber", txtContainerNumber.Text.Trim())
                        myCmd.Parameters.AddWithValue("@ContainerETA", If(String.IsNullOrEmpty(txtContainerEta.Text), CType(DBNull.Value, Object), txtContainerEta.Text))
                        myCmd.Parameters.AddWithValue("@Courier", txtCourier.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "Order Shipped"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If
        Catch ex As Exception
            MessageError_ShipmentOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ShipmentOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showShipmentOrder", thisScript, True)
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
                Dim thisId As String = txtCancelOrderId.Text
                Dim companyId As String = orderClass.GetCompanyIdByOrder(thisId)

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Canceled', StatusDescription=@StatusDescription, CanceledDate=GETDATE(), ShipmentNumber=NULL, ShipmentDate=NULL, ContainerNumber=NULL, ContainerETA=NULL, Courier=NULL, InvoiceNumber=NULL, Collector=NULL, InvoiceDate=NULL, DueDate=NULL, Payment=0, PaymentDate=NULL, Amount=0 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@StatusDescription", txtCancelDescription.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim descLog As String = String.Format("Order Canceled. Reason : {0}", txtCancelDescription.Text.Trim())
                dataLog = {"OrderHeaders", thisId, Session("LoginId"), descLog}
                orderClass.Logs(dataLog)

                Dim salesClass As New SalesClass
                salesClass.RefreshData(companyId)

                Response.Redirect("~/order", False)
            End If
        Catch ex As Exception
            MessageError_CancelOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_CancelOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showCancelOrder", thisScript, True)
        End Try
    End Sub

    Protected Sub btnOcean_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            'Dim thisId As String = txtOceanId.Text

            'Dim checkOcean As Integer = orderClass.GetItemData_Integer("SELECT COUNT(OrderDetails.Id) FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & thisId & "' AND OrderDetails.Active=1 AND Products.DesignId='15'")
            'If checkOcean > 0 Then
            '    Dim svc As New ShutterOceanService()
            '    txtJson.Text = svc.PreviewJsonAsync(thisId).Result
            'End If
            Response.Redirect("~/order", False)
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
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
                ddlStatus.Items.Add(New ListItem("Canceled", "Canceled"))
                ddlStatus.Items.Add(New ListItem("Unshipment", "Unshipment"))
            End If

            If Session("RoleName") = "IT" Then
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
                ddlStatus.Items.Add(New ListItem("Unshipment", "Unshipment"))
            End If

            If Session("RoleName") = "Factory Office" Then
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
                ddlStatus.Items.Add(New ListItem("Unshipment", "Unshipment"))
            End If

            If Session("RoleName") = "Sales" Then
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
                ddlStatus.Items.Add(New ListItem("Unshipment", "Unshipment"))
            End If

            If Session("RoleName") = "Account" Then
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

            If Session("RoleName") = "Data Entry" Then
                ddlStatus.Items.Add(New ListItem("Unsubmitted", "Unsubmitted"))
                ddlStatus.Items.Add(New ListItem("Quoted", "Quoted"))
                ddlStatus.Items.Add(New ListItem("New Order", "New Order"))
                ddlStatus.Items.Add(New ListItem("Payment Received", "Payment Received"))
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
                ddlStatus.Items.Add(New ListItem("Canceled", "Canceled"))
                ddlStatus.Items.Add(New ListItem("Unshipment", "Unshipment"))
            End If

            If Session("RoleName") = "Export" Then
                ddlStatus.Items.Add(New ListItem("In Production", "In Production"))
                ddlStatus.Items.Add(New ListItem("On Hold", "On Hold"))
                ddlStatus.Items.Add(New ListItem("Shipped Out", "Shipped Out"))
                ddlStatus.Items.Add(New ListItem("Completed", "Completed"))
                ddlStatus.Items.Add(New ListItem("Unshipment", "Unshipment"))
            End If

            If Session("RoleName") = "Customer" Then
                ddlStatus.Items.Add(New ListItem("Unsubmitted", "Unsubmitted"))
                ddlStatus.Items.Add(New ListItem("New Order", "New Order"))
                ddlStatus.Items.Add(New ListItem("Waiting Proforma", "Waiting Proforma"))
                ddlStatus.Items.Add(New ListItem("Proforma Sent", "Proforma Sent"))
                ddlStatus.Items.Add(New ListItem("Payment Received", "Payment Received"))
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

    Protected Sub BindOrderType()
        ddlType.Items.Clear()
        Try
            ddlType.Items.Add(New ListItem("All", ""))
            ddlType.Items.Add(New ListItem("Regular", "Regular"))
            ddlType.Items.Add(New ListItem("Builder", "Builder"))
            ddlType.Items.Add(New ListItem("Rework", "Rework"))

            If Session("RoleName") = "Installer" Then
                ddlType.Items.Clear()
                ddlType.Items.Add(New ListItem("Builder", "Builder"))
            End If
        Catch ex As Exception
            ddlType.Items.Clear()
        End Try
    End Sub

    Protected Sub BindDataOrder(search As String, status As String, company As String, orderType As String, active As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", status),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", orderType)
            }

            Dim thisData As DataTable = orderClass.GetDataTableSP("sp_OrderList", params)

            gvList.DataSource = thisData
            gvList.DataBind()

            gvList.Columns(1).Visible = LoginAccess("Visible ID")
            gvList.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Sponsor" AndAlso Session("LevelName") = "Leader" Then
                gvList.Columns(3).Visible = True
            End If
            gvList.Columns(7).Visible = LoginAccess("Visible Created Date")
            gvList.Columns(10).Visible = LoginAccess("Visible Factory")
            gvList.Columns(12).Visible = LoginAccess("Visible BOE")

            btnAdd.Visible = LoginAccess("Add")
            btnRework.Visible = LoginAccess("Rework")
            btnFile.Visible = LoginAccess("File")

            divActive.Visible = LoginAccess("Active")
            divCompany.Visible = LoginAccess("Filter Company")
            divType.Visible = LoginAccess("Filter Type")
            If Session("RoleName") = "Sales" AndAlso Session("LevelName") = "Member" AndAlso Session("UserName") = "felicity" Then
                divType.Visible = True
            End If

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
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = orderClass.GetDataTable("SELECT * FROM Companys WHERE Active=1 ORDER BY Name ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            ddlCompany.Items.Insert(0, New ListItem("All", ""))
            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Account" Then
                ddlCompany.SelectedValue = Session("CompanyId").ToString()
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
        End Try
    End Sub

    Protected Sub BuildPager()
        Try
            If gvList.PageCount <= 1 Then
                navPager.Visible = False
                Return
            End If

            navPager.Visible = True

            Dim currentPage As Integer = gvList.PageIndex
            Dim totalPages As Integer = gvList.PageCount

            Dim pages As New List(Of Object)

            If currentPage > 0 Then
                pages.Add(New With {
                    .Text = "Previous",
                    .PageIndex = currentPage - 1,
                    .CssClass = ""
                })
            End If

            Dim startPage As Integer = Math.Max(0, currentPage - 2)
            Dim endPage As Integer = Math.Min(totalPages - 1, currentPage + 2)

            For i As Integer = startPage To endPage
                pages.Add(New With {
                    .Text = (i + 1).ToString(),
                    .PageIndex = i,
                    .CssClass = If(i = currentPage, "active", "")
                })
            Next

            If currentPage < totalPages - 1 Then
                pages.Add(New With {
                    .Text = "Next",
                    .PageIndex = currentPage + 1,
                    .CssClass = ""
                })
            End If

            rptPager.DataSource = pages
            rptPager.DataBind()
        Catch ex As Exception
            navPager.Visible = False
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub
    Protected Sub MessageError_DuplicateOrder(visible As Boolean, message As String)
        divErrorDuplicateOrder.Visible = visible : msgErrorDuplicateOrder.InnerText = message
    End Sub

    Protected Sub MessageError_CancelOrder(visible As Boolean, message As String)
        divErrorCancelOrder.Visible = visible : msgErrorCancelOrder.InnerText = message
    End Sub

    Protected Sub MessageError_ShipmentOrder(visible As Boolean, message As String)
        divErrorShipmentOrder.Visible = visible : msgErrorShipmentOrder.InnerText = message
    End Sub

    Protected Function BindCustomerText(customerName As String, sales As String) As String
        If Session("RoleName") = "Developer" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Account" OrElse (Session("RoleName") = "Sales" AndAlso Session("LevelName") = "Leader") Then
            If String.IsNullOrWhiteSpace(sales) Then
                Return customerName
            End If
            Return String.Format("{0}<br /><span style='font-size:13px; color:red;'>(Sales : {1})</span>", customerName, sales)
        End If
        Return customerName
    End Function

    Protected Function VisibleEdit(data As Object) As Boolean
        Dim active As Boolean = Convert.ToBoolean(data(0))
        Dim status As String = Convert.ToString(data(1))
        Dim createdBy As String = Convert.ToString(data(2))
        Dim createdRole As String = Convert.ToString(data(3))

        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (Not status = "Shipped Out" AndAlso Not status = "Completed" AndAlso Not status = "Canceled") Then Return True
            If Session("RoleName") = "IT" AndAlso (status = "Unsubmitted" OrElse status = "Quoted" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Waiting Proforma" OrElse status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "Factory Office" AndAlso (status = "Unsubmitted" OrElse status = "Quoted" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Waiting Proforma" OrElse status = "New Order") Then Return True
            If Session("RoleName") = "Sales" AndAlso (status = "Unsubmitted" OrElse status = "Waiting Proforma") Then Return True
            If Session("RoleName") = "Account" AndAlso (status = "Unsubmitted" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Waiting Proforma" OrElse status = "New Order") Then Return True
            If Session("RoleName") = "Data Entry" AndAlso status = "Unsubmitted" AndAlso createdBy = Session("LoginId").ToString() Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleDelete(data As Object) As Boolean
        Dim active As Boolean = Convert.ToBoolean(data(0))
        Dim status As String = Convert.ToString(data(1))
        Dim createdBy As String = Convert.ToString(data(2))
        Dim createdRole As String = Convert.ToString(data(3))

        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (Not status = "Shipped Out" AndAlso Not status = "Completed" AndAlso Not status = "Canceled") Then Return True
            If Session("RoleName") = "IT" AndAlso status = "Unsubmitted" Then Return True
            If Session("RoleName") = "Factory Office" AndAlso status = "Unsubmitted" Then Return True
            If Session("RoleName") = "Sales" AndAlso status = "Unsubmitted" AndAlso createdBy = Session("LoginId").ToString() Then Return True
            If Session("RoleName") = "Data Entry" AndAlso status = "Unsubmitted" AndAlso (createdBy = Session("LoginId").ToString() OrElse createdRole = Session("RoleId")) Then Return True
            If Session("RoleName") = "Customer" AndAlso status = "Unsubmitted" Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleCopy(active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" Then Return True
            If Session("RoleName") = "IT" Then Return True
            If Session("RoleName") = "Factory Office" Then Return True
            If Session("RoleName") = "Sales" Then Return True
            If Session("RoleName") = "Data Entry" Then Return True
            If Session("RoleName") = "Customer" Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleUnsubmitOrder(status As String, active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (status = "New Order" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "IT" AndAlso (status = "New Order" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent") Then Return True
            If Session("RoleName") = "Factory Office" AndAlso (status = "New Order" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent") Then Return True
            If Session("RoleName") = "Sales" AndAlso (status = "New Order" OrElse status = "Waiting Proforma") Then Return True
            If Session("RoleName") = "Account" AndAlso (status = "New Order" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent") Then Return True
            If Session("RoleName") = "Data Entry" AndAlso status = "New Order" Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleNewOrder(status As String, active As String) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso status = "Waiting Proforma" Then Return True
            If Session("RoleName") = "IT" AndAlso status = "Waiting Proforma" Then Return True
            If Session("RoleName") = "Factory Office" AndAlso status = "Waiting Proforma" Then Return True
            If Session("RoleName") = "Account" AndAlso status = "Waiting Proforma" Then Return True
            If Session("RoleName") = "Data Entry" AndAlso status = "Waiting Proforma" Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleReceivePayment(status As String, active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso status = "Proforma Sent" Then Return True
            If Session("RoleName") = "IT" AndAlso status = "Proforma Sent" Then Return True
            If Session("RoleName") = "Factory Office" AndAlso status = "Proforma Sent" Then Return True
            If Session("RoleName") = "Sales" AndAlso status = "Proforma Sent" Then Return True
            If Session("RoleName") = "Account" AndAlso status = "Proforma Sent" Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleProductionOrder(status As String, active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (status = "New Order" OrElse status = "Payment Received" Or status = "On Hold" OrElse status = "Shipped Out") Then Return True
            If Session("RoleName") = "IT" AndAlso (status = "New Order" OrElse status = "Payment Received" Or status = "On Hold") Then Return True
            If Session("RoleName") = "Factory Office" AndAlso (status = "New Order" OrElse status = "Payment Received" Or status = "On Hold") Then Return True
            If Session("RoleName") = "Data Entry" AndAlso (status = "New Order" OrElse status = "Payment Received" Or status = "On Hold") Then Return True
            If Session("RoleName") = "Account" AndAlso (status = "New Order" OrElse status = "Payment Received") Then Return True
            If Session("RoleName") = "Export" AndAlso status = "Shipped Out" Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleHoldOrder(status As String, active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (status = "Payment Received" OrElse status = "New Order" OrElse status = "In Production") Then Return True
            If Session("RoleName") = "IT" AndAlso (status = "Payment Received" OrElse status = "New Order" OrElse status = "In Production") Then Return True
            If Session("RoleName") = "Factory Office" AndAlso (status = "New Order" OrElse status = "In Production") Then Return True
            If Session("RoleName") = "Data Entry" AndAlso (status = "New Order" OrElse status = "In Production") Then Return True
            If Session("RoleName") = "Sales" AndAlso status = "New Order" Then Return True
            If Session("RoleName") = "Account" AndAlso status = "New Order" Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleCancelOrder(status As String, active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Payment Received" OrElse status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "IT" AndAlso (status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "Factory Office" AndAlso (status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "Data Entry" AndAlso (status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "Account" AndAlso (status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "New Order") Then Return True
            If Session("RoleName") = "Sales" AndAlso (status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "New Order") Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleShipmentOrder(status As String, active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "IT" AndAlso status = "In Production" Then Return True
            If Session("RoleName") = "Factory Office" AndAlso status = "In Production" Then Return True
            If Session("RoleName") = "Export" AndAlso status = "In Production" Then Return True
            If Session("RoleName") = "Data Entry" AndAlso status = "In Production" Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleCompleteOrder(status As String, active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso status = "Shipped Out" Then Return True
            If Session("RoleName") = "IT" AndAlso status = "Shipped Out" Then Return True
            If Session("RoleName") = "Factory Office" AndAlso status = "Shipped Out" Then Return True
            If Session("RoleName") = "Export" AndAlso status = "Shipped Out" Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleDownloadBOE(status As String, download As String, active As Boolean) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (status = "Unsubmitted" OrElse status = "Payment Received" OrElse status = "In Production" OrElse status = "On Hold") AndAlso (download = "No" OrElse download = "Done") Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleSurat(status As String, companyId As String, active As Boolean) As Boolean
        If active = True AndAlso companyId = "3" Then
            If Session("RoleName") = "Developer" Then Return True
            If Session("RoleName") = "IT" Then Return True
            If Session("RoleName") = "Factory Office" AndAlso (status = "In Production" OrElse status = "Shipped Out") Then Return True
            If Session("RoleName") = "Export" AndAlso (status = "In Production" OrElse status = "Shipped Out") Then Return True
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleChina(active As Boolean, status As String, factory As String) As Boolean
        If active = True Then
            If Session("RoleName") = "Developer" AndAlso factory.Contains("CHINA") AndAlso (status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then
                Return True
            End If
            Return False
        End If
        Return False
    End Function

    Protected Function VisibleLog() As Boolean
        If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then Return True
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
