Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Threading.Tasks
Imports System.Web.Services

Partial Class Order_Default
    Inherits Page

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object() = Nothing


    <WebMethod(EnableSession:=True)>
    Public Shared Sub UpdateSession(value As String)
        HttpContext.Current.Session("selectedTabOrder") = value
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not Session("selectedTabOrder") = "" Then
            selected_tab.Value = Session("selectedTabOrder").ToString()
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_DuplicateOrder(False, String.Empty)
            MessageError_CancelOrder(False, String.Empty)
            MessageError_ShipmentOrder(False, String.Empty)

            BindCompany()
            BindOrderType()

            If Not String.IsNullOrEmpty(Session("OrderCompany")) Then
                ddlCompany.SelectedValue = Session("OrderCompany")
            End If
            ddlState.SelectedValue = Session("OrderState")
            txtSearch.Text = Session("OrderSearch")
            ddlType.SelectedValue = Session("OrderType")
            ddlActive.SelectedValue = Session("OrderActive")

            BindDataOrder(ddlCompany.SelectedValue, ddlState.SelectedValue, txtSearch.Text, ddlType.SelectedValue, ddlActive.SelectedValue)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderState") = ddlState.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderType") = ddlType.SelectedValue
        Session("OrderActive") = ddlActive.SelectedValue

        Response.Redirect("~/order/add", False)
    End Sub

    Protected Sub btnInsert_Click(sender As Object, e As EventArgs)
        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderState") = ddlState.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderType") = ddlType.SelectedValue
        Session("OrderActive") = ddlActive.SelectedValue

        Response.Redirect("~/order/add", False)
    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderState") = ddlState.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderType") = ddlType.SelectedValue
        Session("OrderActive") = ddlActive.SelectedValue

        Response.Redirect("~/order/upload", False)
    End Sub

    Protected Sub btnRework_Click(sender As Object, e As EventArgs)
        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderState") = ddlState.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderType") = ddlType.SelectedValue
        Session("OrderActive") = ddlActive.SelectedValue

        Response.Redirect("~/order/rework", False)
    End Sub

    Protected Sub btnFile_Click(sender As Object, e As EventArgs)
        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderState") = ddlState.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderType") = ddlType.SelectedValue
        Session("OrderActive") = ddlActive.SelectedValue

        Response.Redirect("~/order/file", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(ddlCompany.SelectedValue, ddlState.SelectedValue, txtSearch.Text, ddlType.SelectedValue, ddlActive.SelectedValue)

        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderState") = ddlState.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderType") = ddlType.SelectedValue
        Session("OrderActive") = ddlActive.SelectedValue
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(ddlCompany.SelectedValue, ddlState.SelectedValue, txtSearch.Text, ddlType.SelectedValue, ddlActive.SelectedValue)

        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderState") = ddlState.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderType") = ddlType.SelectedValue
        Session("OrderActive") = ddlActive.SelectedValue
    End Sub

    Protected Sub ddlState_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(ddlCompany.SelectedValue, ddlState.SelectedValue, txtSearch.Text, ddlType.SelectedValue, ddlActive.SelectedValue)

        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderState") = ddlState.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderType") = ddlType.SelectedValue
        Session("OrderActive") = ddlActive.SelectedValue
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(ddlCompany.SelectedValue, ddlState.SelectedValue, txtSearch.Text, ddlType.SelectedValue, ddlActive.SelectedValue)

        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderState") = ddlState.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderType") = ddlType.SelectedValue
        Session("OrderActive") = ddlActive.SelectedValue
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(ddlCompany.SelectedValue, ddlState.SelectedValue, txtSearch.Text, ddlType.SelectedValue, ddlActive.SelectedValue)

        Session("OrderCompany") = ddlCompany.SelectedValue
        Session("OrderState") = ddlState.SelectedValue
        Session("OrderSearch") = txtSearch.Text
        Session("OrderType") = ddlType.SelectedValue
        Session("OrderActive") = ddlActive.SelectedValue
    End Sub

    Protected Sub btnStatusOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtStatusOrderId.Text
            Dim thisStatus As String = txtStatusOrderNew.Text
            Dim thisOldStatus As String = txtStatusOrderOld.Text
            Dim companyId As String = orderClass.GetCompanyIdByOrder(thisId)

            If thisStatus = "Delete Order" Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Active=0, Download='No' WHERE Id=@Id;", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", thisId, Session("LoginId").ToString(), "Order Deleted"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Unsubmit Order" Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET SubmittedDate=NULL, ProductionDate=NULL, OnHoldDate=NULL, Status='Unsubmitted', Download='No', DownloadDate=NULL, ShipmentDate=NULL, ShipmentNumber=NULL, ContainerNumber=NULL, ContainerETA=NULL, Courier=NULL, InvoiceNumber=NULL, Collector=NULL, InvoiceDate=NULL, DueDate=NULL, Payment=0, PaymentDate=NULL, Amount=0 WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim serviceData As DataTable = orderClass.GetDataTable("SELECT OrderDetails.* FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & thisId & "' AND Products.DesignId='16'")
                If serviceData.Rows.Count > 0 Then
                    For i As Integer = 0 To serviceData.Rows.Count - 1
                        Dim serviceId As String = serviceData.Rows(i).Item("Id").ToString()
                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Active=0 WHERE Id=@ItemId; DELETE FROM OrderCostings WHERE HeaderId=@HeaderId AND ItemId=@ItemId", thisConn)
                                thisCmd.Parameters.AddWithValue("@ItemId", serviceId)
                                thisCmd.Parameters.AddWithValue("@HeaderId", thisId)
                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
                            End Using
                        End Using
                    Next
                End If

                orderClass.CalculatePriceByOrder(thisId)

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "Order Unsubmitted"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "New Order" Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='New Order' WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                If thisOldStatus = "Waiting Proforma" Then
                    Dim checkOcean As Integer = orderClass.GetItemData_Integer("SELECT COUNT(OrderDetails.Id) FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & thisId & "' AND OrderDetails.Active=1 AND Products.DesignId='15'")
                    If checkOcean > 0 Then
                        Task.Run(Async Function()
                                     Dim svc As New ShutterOceanService()
                                     Await svc.SendOrderAsync(thisId)
                                 End Function)
                    End If
                End If

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
                    Using thisCmd As SqlCommand = New SqlCommand(stringQuery, thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "Order In Production"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Hold Order" Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='On Hold', OnHoldDate=GETDATE() WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "Order On Hold"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Receive Payment" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()

                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Payment Received' WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.ExecuteNonQuery()
                    End Using

                    Dim amount As Decimal = orderClass.GetItemData_Decimal("SELECT (SUM(SellPrice) * 1.10) AS SumPrice FROM OrderCostings WHERE HeaderId='" & thisId & "' AND Type='Final'")
                    If companyId = "3" Then
                        amount = orderClass.GetItemData_Decimal("SELECT (SUM(SellPrice) * 1.10) AS SumPrice FROM OrderCostings WHERE HeaderId='" & thisId & "' AND Type='Final'")
                    End If

                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET PaymentDate=GETDATE(), DueDate=NULL, Payment=1, Amount=@Amount WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@Amount", amount)
                        thisCmd.ExecuteNonQuery()
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
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Completed', CompletedDate=GETDATE() WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", thisId, Session("LoginId"), "Order Completed"}
                orderClass.Logs(dataLog)

                Response.Redirect("~/order", False)
            End If

            If thisStatus = "Download BOE" Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Download='Yes', DownloadDate=NULL WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
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
                            Using thisCmd As New SqlCommand("sp_OrderHeaders_Copy", thisConn)
                                thisCmd.CommandType = CommandType.StoredProcedure

                                thisCmd.Parameters.AddWithValue("@OldId", thisId)
                                thisCmd.Parameters.AddWithValue("@NewID", newIdHeader)
                                thisCmd.Parameters.AddWithValue("@OrderId", orderId)
                                thisCmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumberNew.Text.Trim())
                                thisCmd.Parameters.AddWithValue("@OrderName", txtOrderNameNew.Text.Trim())
                                thisCmd.Parameters.AddWithValue("@OrderNote", txtOrderNoteNew.Text)
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

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("INSERT INTO OrderQuotes VALUES(@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", newIdHeader)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", newIdHeader, Session("LoginId").ToString(), "Order Created | Copy"}
                orderClass.Logs(dataLog)

                Dim thisHeader As DataTable = orderClass.GetDataTable("SELECT * FROM OrderDetails WHERE HeaderId='" & thisId & "' AND Active=1")
                If thisHeader.Rows.Count > 0 Then
                    For i As Integer = 0 To thisHeader.Rows.Count - 1
                        Dim itemId As String = thisHeader.Rows(i).Item("Id").ToString()
                        Dim newIdDetail As String = orderClass.GetNewOrderItemId()

                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As New SqlCommand("sp_OrderDetails_Copy", thisConn)
                                thisCmd.CommandType = CommandType.StoredProcedure
                                thisCmd.Parameters.AddWithValue("@ItemIdOld", itemId)
                                thisCmd.Parameters.AddWithValue("@NewId", newIdDetail)
                                thisCmd.Parameters.AddWithValue("@HeaderId", newIdHeader)
                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
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
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Shipped Out', ShipmentNumber=@ShipmentNumber, ShipmentDate=@ShipmentDate, ContainerNumber=@ContainerNumber, ContainerETA=@ContainerETA, Courier=@Courier WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@ShipmentNumber", txtShipmentNumber.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@ShipmentDate", If(String.IsNullOrEmpty(txtShipmentDate.Text), CType(DBNull.Value, Object), txtShipmentDate.Text))
                        thisCmd.Parameters.AddWithValue("@ContainerNumber", txtContainerNumber.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@ContainerETA", If(String.IsNullOrEmpty(txtContainerEta.Text), CType(DBNull.Value, Object), txtContainerEta.Text))
                        thisCmd.Parameters.AddWithValue("@Courier", txtCourier.Text.Trim())
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
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
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Canceled', StatusDescription=@StatusDescription, CanceledDate=GETDATE(), ShipmentNumber=NULL, ShipmentDate=NULL, ContainerNumber=NULL, ContainerETA=NULL, Courier=NULL, InvoiceNumber=NULL, Collector=NULL, InvoiceDate=NULL, DueDate=NULL, Payment=0, PaymentDate=NULL, Amount=0 WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@StatusDescription", txtCancelDescription.Text.Trim())
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim descLog As String = String.Format("Order Canceled. Reason : {0}", txtCancelDescription.Text.Trim())
                dataLog = {"OrderHeaders", thisId, Session("LoginId"), descLog}
                orderClass.Logs(dataLog)

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
            Dim thisId As String = txtOceanId.Text

            Task.Run(Async Function()
                         Dim svc As New ShutterOceanService()
                         Await svc.SendOrderAsync(thisId)
                     End Function)

            Response.Redirect("~/order", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataOrder(company As String, state As String, search As String, type As String, active As String)
        Try
            btnAdd.Visible = False
            btnAddOrder.Visible = False
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then
                btnAddOrder.Visible = True
            End If
            If Session("RoleName") = "Data Entry" OrElse Session("RoleName") = "Sales" Then
                btnAdd.Visible = True
            End If
            If Session("RoleName") = "Customer" Then
                btnAdd.Visible = True
                If Session("CustomerId") = "127" Then
                    btnAdd.Visible = False
                    btnAddOrder.Visible = True
                End If
            End If

            btnRework.Visible = LoginAccess("Rework")
            btnFile.Visible = LoginAccess("File")

            divCompany.Visible = LoginAccess("Filter Company")
            divState.Visible = LoginAccess("Filter State")
            divType.Visible = LoginAccess("Filter Type")

            If Session("RoleName") = "Sales" AndAlso Session("LevelName") = "Member" AndAlso Session("UserName") = "felicity" Then
                divType.Visible = True
            End If
            divActive.Visible = LoginAccess("Active")

            If Session("RoleName") = "Customer" Then
                Dim onStop As Boolean = orderClass.GetCustomerOnStop(Session("CustomerId").ToString())
                If onStop = True Then btnAdd.Visible = True
            End If

            listQuote.Visible = False
            If Session("RoleName") = "Developer" Then
                listQuote.Visible = True
            End If
            If Session("RoleName") = "IT" Then
                listQuote.Visible = True
            End If
            If Session("RoleName") = "Factory Office" Then
                listQuote.Visible = True
            End If
            If Session("RoleName") = "Sales" Then
                listQuote.Visible = True
            End If
            If Session("RoleName") = "Account" Then
                listQuote.Visible = True
            End If
            If Session("RoleName") = "Data Entry" Then
                listQuote.Visible = True
            End If
            If Session("RoleName") = "Installer" Then
                listQuote.Visible = True
            End If

            ' UNSUBMIT
            Dim unsubmitParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "Unsubmitted"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListUnsubmit.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", unsubmitParams)
            gvListUnsubmit.DataBind()

            gvListUnsubmit.Columns(1).Visible = LoginAccess("Visible ID")
            gvListUnsubmit.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListUnsubmit.Columns(3).Visible = True
            End If
            gvListUnsubmit.Columns(7).Visible = LoginAccess("Visible Factory")
            gvListUnsubmit.Columns(8).Visible = LoginAccess("Visible BOE")


            ' QUOTE
            Dim quoteParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "Quoted"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListQuote.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", quoteParams)
            gvListQuote.DataBind()

            gvListQuote.Columns(1).Visible = LoginAccess("Visible ID")
            gvListQuote.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListQuote.Columns(3).Visible = True
            End If
            gvListQuote.Columns(8).Visible = LoginAccess("Visible Factory")
            gvListQuote.Columns(9).Visible = LoginAccess("Visible BOE")


            ' WAITING
            Dim waitingParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "Waiting Proforma"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListWaiting.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", waitingParams)
            gvListWaiting.DataBind()

            gvListWaiting.Columns(1).Visible = LoginAccess("Visible ID")
            gvListWaiting.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListWaiting.Columns(3).Visible = True
            End If
            gvListWaiting.Columns(7).Visible = LoginAccess("Visible Factory")
            gvListWaiting.Columns(8).Visible = LoginAccess("Visible BOE")


            ' PROFORMA SENT
            Dim sentParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "Proforma Sent"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListSent.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", sentParams)
            gvListSent.DataBind()

            gvListSent.Columns(1).Visible = LoginAccess("Visible ID")
            gvListSent.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListSent.Columns(3).Visible = True
            End If
            gvListSent.Columns(8).Visible = LoginAccess("Visible Factory")
            gvListSent.Columns(9).Visible = LoginAccess("Visible BOE")


            ' PAYMENT RECEIVED
            Dim receiveParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "Payment Received"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListReceive.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", receiveParams)
            gvListReceive.DataBind()

            gvListReceive.Columns(1).Visible = LoginAccess("Visible ID")
            gvListReceive.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListReceive.Columns(3).Visible = True
            End If
            gvListReceive.Columns(9).Visible = LoginAccess("Visible Factory")
            gvListReceive.Columns(10).Visible = LoginAccess("Visible BOE")


            ' NEW ORDER
            Dim newParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "New Order"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListNew.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", newParams)
            gvListNew.DataBind()

            gvListNew.Columns(1).Visible = LoginAccess("Visible ID")
            gvListNew.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListNew.Columns(3).Visible = True
            End If
            gvListNew.Columns(7).Visible = LoginAccess("Visible Factory")
            gvListNew.Columns(8).Visible = LoginAccess("Visible BOE")


            ' PRODUCTION
            Dim productionParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "In Production"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListProduction.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", productionParams)
            gvListProduction.DataBind()

            gvListProduction.Columns(1).Visible = LoginAccess("Visible ID")
            gvListProduction.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListProduction.Columns(3).Visible = True
            End If
            gvListProduction.Columns(8).Visible = LoginAccess("Visible Factory")
            gvListProduction.Columns(9).Visible = LoginAccess("Visible BOE")


            ' ON HOLD
            Dim holdParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "On Hold"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListHold.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", holdParams)
            gvListHold.DataBind()

            gvListHold.Columns(1).Visible = LoginAccess("Visible ID")
            gvListHold.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListHold.Columns(3).Visible = True
            End If
            gvListHold.Columns(9).Visible = LoginAccess("Visible Factory")
            gvListHold.Columns(10).Visible = LoginAccess("Visible BOE")


            ' SHIPPED OUT
            Dim shippedParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "Shipped Out"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListShipped.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", shippedParams)
            gvListShipped.DataBind()

            gvListShipped.Columns(1).Visible = LoginAccess("Visible ID")
            gvListShipped.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListShipped.Columns(3).Visible = True
            End If
            gvListShipped.Columns(8).Visible = LoginAccess("Visible Factory")
            gvListShipped.Columns(9).Visible = LoginAccess("Visible BOE")


            ' CANCELED
            Dim cancelParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "Canceled"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListCancel.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", cancelParams)
            gvListCancel.DataBind()

            gvListCancel.Columns(1).Visible = LoginAccess("Visible ID")
            gvListCancel.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListCancel.Columns(3).Visible = True
            End If


            ' CANCELED
            Dim unshipmentParams As New List(Of SqlParameter) From {
                New SqlParameter("@Search", search.Trim()),
                New SqlParameter("@Status", "Unshipment"),
                New SqlParameter("@CompanyId", company),
                New SqlParameter("@Active", active),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CustomerLevel", Session("CustomerLevel").ToString()),
                New SqlParameter("@CustomerId", Session("CustomerId").ToString()),
                New SqlParameter("@LoginId", Session("LoginId").ToString()),
                New SqlParameter("@RoleId", Session("RoleId").ToString()),
                New SqlParameter("@OrderType", type),
                New SqlParameter("@CustomerState", state)
            }
            gvListUnshipment.DataSource = orderClass.GetDataTableSP("sp_OrderHeaders_List", unshipmentParams)
            gvListUnshipment.DataBind()

            gvListUnshipment.Columns(1).Visible = LoginAccess("Visible ID")
            gvListUnshipment.Columns(3).Visible = LoginAccess("Visible Customer Name")
            If Session("CustomerLevel") = "Primary" AndAlso Session("LevelName") = "Leader" Then
                gvListUnshipment.Columns(3).Visible = True
            End If
            gvListShipped.Columns(7).Visible = LoginAccess("Visible Factory")
            If gvListProduction.Rows.Count > 0 Then
                selected_tab.Value = "list-production"
            ElseIf gvListNew.Rows.Count > 0 Then
                selected_tab.Value = "list-new"

            ElseIf gvListQuote.Rows.Count > 0 Then
                selected_tab.Value = "list-quote"

            ElseIf gvListWaiting.Rows.Count > 0 Then
                selected_tab.Value = "list-waiting"

            ElseIf gvListSent.Rows.Count > 0 Then
                selected_tab.Value = "list-sent"

            ElseIf gvListReceive.Rows.Count > 0 Then
                selected_tab.Value = "list-receive"

            ElseIf gvListUnsubmit.Rows.Count > 0 Then
                selected_tab.Value = "list-unsubmit"

            ElseIf gvListHold.Rows.Count > 0 Then
                selected_tab.Value = "list-hold"

            ElseIf gvListShipped.Rows.Count > 0 Then
                selected_tab.Value = "list-shipped"

            ElseIf gvListCancel.Rows.Count > 0 Then
                selected_tab.Value = "list-cancel"

            ElseIf gvListUnshipment.Rows.Count > 0 Then
                selected_tab.Value = "list-unshipment"
            Else
                selected_tab.Value = "list-unsubmit"
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
            ddlCompany.Enabled = True
            ddlCompany.DataSource = orderClass.GetDataTable("SELECT Id, Alias FROM Companys WHERE Status='Active' ORDER BY Name ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            ddlCompany.Items.Insert(0, New ListItem("All", ""))
            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Account" Then
                ddlCompany.SelectedValue = Session("CompanyId").ToString()
                ddlCompany.Enabled = False
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
        End Try
    End Sub

    Protected Sub BindOrderType()
        ddlType.Items.Clear()
        Try
            ddlType.Items.Add(New ListItem("All", ""))
            ddlType.Items.Add(New ListItem("Regular", "Regular"))
            ddlType.Items.Add(New ListItem("Builder", "Builder"))
            ddlType.Items.Add(New ListItem("Rework", "Rework"))

            If Session("RoleName") = "Sales" Then
                ddlType.Items.Clear()
                ddlType.Items.Add(New ListItem("Regular", "Regular"))
                ddlType.Items.Add(New ListItem("Rework", "Rework"))
                If Session("LevelName") = "Leader" OrElse Session("UserName") = "felicity" Then
                    ddlType.Items.Add(New ListItem("Builder", "Builder"))
                End If
            End If

            If Session("RoleName") = "Installer" Then
                ddlType.Items.Clear()
                ddlType.Items.Add(New ListItem("Builder", "Builder"))
            End If
        Catch ex As Exception
            ddlType.Items.Clear()
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
        Try
            If Session("RoleName") = "Developer" Then
                If String.IsNullOrWhiteSpace(sales) Then
                    Return customerName
                End If
                Return String.Format("{0}<br /><span style='font-size:13px; color:red;'>(Sales : {1})</span>", customerName, sales)
            End If
            If Session("RoleName") = "IT" Then
                If String.IsNullOrWhiteSpace(sales) Then
                    Return customerName
                End If
                Return String.Format("{0}<br /><span style='font-size:13px; color:red;'>(Sales : {1})</span>", customerName, sales)
            End If
            If Session("RoleName") = "Factory Office" Then
                If String.IsNullOrWhiteSpace(sales) Then
                    Return customerName
                End If
                Return String.Format("{0}<br /><span style='font-size:13px; color:red;'>(Sales : {1})</span>", customerName, sales)
            End If
            If Session("RoleName") = "Account" Then
                If String.IsNullOrWhiteSpace(sales) Then
                    Return customerName
                End If
                Return String.Format("{0}<br /><span style='font-size:13px; color:red;'>(Sales : {1})</span>", customerName, sales)
            End If
            If Session("RoleName") = "Sales" Then
                If String.IsNullOrWhiteSpace(sales) Then
                    Return customerName
                End If
                Return String.Format("{0}<br /><span style='font-size:13px; color:red;'>(Sales : {1})</span>", customerName, sales)
            End If

            Return customerName
        Catch ex As Exception
            Return customerName
        End Try
    End Function

    Protected Function VisibleEdit(data As Object) As Boolean
        Dim active As Boolean = Convert.ToBoolean(data(0))
        Dim status As String = Convert.ToString(data(1))
        Dim createdBy As String = Convert.ToString(data(2))
        Dim createdRole As String = Convert.ToString(data(3))

        If active = True Then
            If Session("RoleName") = "Developer" AndAlso (Not status = "Shipped Out" AndAlso Not status = "Completed" AndAlso Not status = "Canceled") Then Return True
            If Session("RoleName") = "IT" AndAlso (status = "Unsubmitted" OrElse status = "Quoted" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Waiting Proforma" OrElse status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "Factory Office" AndAlso (status = "Unsubmitted" OrElse status = "Quoted" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Waiting Proforma" OrElse status = "New Order" OrElse status = "In Production") Then Return True
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
            If Session("RoleName") = "Developer" AndAlso (Not status = "Shipped Out" AndAlso Not status = "Completed") Then Return True
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
            If Session("RoleName") = "Developer" AndAlso (status = "New Order" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Pending Payment" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "IT" AndAlso (status = "New Order" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Pending Payment") Then Return True
            If Session("RoleName") = "Factory Office" AndAlso (status = "New Order" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Pending Payment") Then Return True
            If Session("RoleName") = "Sales" AndAlso (status = "New Order" OrElse status = "Waiting Proforma") Then Return True
            If Session("RoleName") = "Account" AndAlso (status = "New Order" OrElse status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Pending Payment") Then Return True
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
            If Session("RoleName") = "Developer" AndAlso (status = "Proforma Sent" OrElse status = "Pending Payment") Then Return True
            If Session("RoleName") = "IT" AndAlso (status = "Proforma Sent" OrElse status = "Pending Payment") Then Return True
            If Session("RoleName") = "Factory Office" AndAlso (status = "Proforma Sent" OrElse status = "Pending Payment") Then Return True
            If Session("RoleName") = "Sales" AndAlso (status = "Proforma Sent" OrElse status = "Pending Payment") Then Return True
            If Session("RoleName") = "Account" AndAlso (status = "Proforma Sent" OrElse status = "Pending Payment") Then Return True
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
            If Session("RoleName") = "Developer" AndAlso (status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Pending Payment" OrElse status = "Payment Received" OrElse status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "IT" AndAlso (status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Pending Payment" OrElse status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "Factory Office" AndAlso (status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Pending Payment" OrElse status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "Data Entry" AndAlso (status = "New Order" OrElse status = "In Production" OrElse status = "On Hold") Then Return True
            If Session("RoleName") = "Account" AndAlso (status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "New Order") Then Return True
            If Session("RoleName") = "Sales" AndAlso (status = "Waiting Proforma" OrElse status = "Proforma Sent" OrElse status = "Pending Payment" OrElse status = "New Order") Then Return True
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

            If Session("RoleName") = "IT" AndAlso (status = "Unsubmitted" OrElse status = "Payment Received" OrElse status = "In Production" OrElse status = "On Hold") AndAlso (download = "No" OrElse download = "Done") Then Return True

            If Session("RoleName") = "Factory Office" AndAlso (status = "Unsubmitted" OrElse status = "Payment Received" OrElse status = "In Production" OrElse status = "On Hold") AndAlso (download = "No" OrElse download = "Done") Then Return True

            If Session("RoleName") = "Data Entry" AndAlso (status = "In Production" OrElse status = "On Hold") AndAlso (download = "No" OrElse download = "Done") Then Return True
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
        If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then Return True
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
