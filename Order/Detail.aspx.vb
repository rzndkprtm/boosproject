Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Order_Detail
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim dataMailing As Object() = Nothing
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty

    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim idIDR As New CultureInfo("id-ID")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("orderid")) Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        lblHeaderId.Text = Request.QueryString("orderid").ToString()
        If Not IsPostBack Then
            AllMessageError(False, String.Empty)
            BindDataOrder(lblHeaderId.Text)
            BindBlindTypeService()
        End If
    End Sub

    Protected Sub btnPreview_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showPreview(); };"
        Try
            If gvListItem.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM ORDER !")
                Exit Sub
            End If

            Dim previewClass As New PreviewClass
            Dim filePath As String = "~/File/Preview/"
            Dim todayString As String = DateTime.Now.ToString("ddMMyyyyHHmmss")
            Dim fileName As String = String.Format("{0}_{1}.pdf", lblOrderId.Text, todayString)

            Dim finalFilePath As String = Server.MapPath(filePath & fileName)
            previewClass.BindContent(lblHeaderId.Text, finalFilePath)

            Dim documentFile As String = "~/File/Preview/" & fileName
            framePreview.Attributes("src") = "../Handler/PDF.ashx?document=" & documentFile

            ClientScript.RegisterStartupScript(Me.GetType(), "showPreview", thisScript, True)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnPreview_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showPreview", thisScript, True)
        End Try
    End Sub

    Protected Sub btnEditHeader_Click(sender As Object, e As EventArgs)
        url = String.Format("~/order/edit?orderidedit={0}", lblHeaderId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnDeleteOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = lblHeaderId.Text

            dataLog = {"OrderHeaders", thisId, Session("LoginId").ToString(), "Order Deleted"}
            orderClass.Logs(dataLog)

            Dim detailData As DataTable = orderClass.GetDataTable("SELECT * FROM OrderDetails WHERE HeaderId='" & thisId & "' AND Active=1 ORDER BY Id ASC")
            If detailData.Rows.Count > 0 Then
                For i As Integer = 0 To detailData.Rows.Count - 1
                    Dim detailId As String = detailData.Rows(i)("Id").ToString()

                    dataLog = {"OrderDetails", detailId, Session("LoginId").ToString(), "Order Item Deleted | Order Header Deleted"}
                    orderClass.Logs(dataLog)
                Next
            End If

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Active=0, DownloadBOE=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Active=0 WHERE HeaderId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDeleteOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnQuoteOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If gvListItem.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM ORDER !")
                Exit Sub
            End If

            If lblCompanyDetailId.Text = "3" Then
                Dim thisData As DataRow = orderClass.GetDataRow("SELECT Est*imator FROM OrderBuilders WHERE Id='" & lblHeaderId.Text & "'")
                Dim estimator As String = thisData("Estimator").ToString()
                Dim supervisor As String = thisData("Supervisor").ToString()
                Dim address As String = thisData("Address").ToString()

                If String.IsNullOrEmpty(estimator) Then
                    MessageError(True, "ESTIMATOR IS REQUIRED !")
                    Exit Sub
                End If

                If String.IsNullOrEmpty(supervisor) Then
                    MessageError(True, "SUPERVISOR IS REQUIRED !")
                    Exit Sub
                End If

                If String.IsNullOrEmpty(address) Then
                    MessageError(True, "ADDRESS IS REQUIRED !")
                    Exit Sub
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET QuotedDate=GETDATE(), Status='Quoted' WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Quote Order"}
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnQuoteOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnSubmitOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If gvListItem.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM ORDER !")
                Exit Sub
            End If

            Dim cashSale As Boolean = orderClass.GetCustomerCashSale(lblCustomerId.Text)

            Dim orderStatus As String = "New Order"
            If cashSale = True Then orderStatus = "Waiting Proforma"

            Dim invoiceNumber As String = lblOrderId.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET SubmittedDate=GETDATE(), Status=@Status WHERE Id=@Id; INSERT OrderInvoices(Id, InvoiceNumber, Payment, Amount) VALUES (@Id, @InvoiceNumber, @Payment, 0)", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@Status", orderStatus)
                    myCmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber)
                    myCmd.Parameters.AddWithValue("@Payment", False)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim totalItems As Integer = orderClass.GetTotalItemOrder(lblHeaderId.Text)
            If lblCompanyId.Text = "2" AndAlso totalItems <= 3 Then
                Dim thisId As String = orderClass.GetNewOrderItemId()
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, Qty, Width, [Drop], TotalItems, MarkUp, Active) VALUES (@Id, @HeaderId, 2986, 112, 1, 0, 0, 1, 0, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderDetails", thisId, "2", "Order Item Added"}
                orderClass.Logs(dataLog)

                orderClass.ResetPriceDetail(lblHeaderId.Text, thisId)
                orderClass.CalculatePrice(lblHeaderId.Text, thisId)
                orderClass.FinalCostItem(lblHeaderId.Text, thisId)
            End If

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Order Submitted"}
            orderClass.Logs(dataLog)

            Dim previewClass As New PreviewClass
            Dim filePath As String = "~/File/Preview/"
            Dim todayString As String = DateTime.Now.ToString("ddMMyyyyHHmmss")
            Dim fileName As String = String.Format("{0}_{1}.pdf", lblOrderId.Text, todayString)

            Dim finalFilePath As String = Server.MapPath(filePath & fileName)
            previewClass.BindContent(lblHeaderId.Text, finalFilePath)

            If cashSale = False Then
                mailingClass.NewOrder(lblHeaderId.Text, finalFilePath)
            End If

            If cashSale = True Then
                mailingClass.NewOrder_Proforma(lblHeaderId.Text, finalFilePath)
            End If

            Dim checkPrinting As Integer = orderClass.GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails WHERE HeaderId='" & lblHeaderId.Text & "' AND (NULLIF(LTRIM(RTRIM(Printing)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingB)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingC)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingD)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingE)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingF)),'') IS NOT NULL)")

            If checkPrinting > 0 Then
                mailingClass.SubmitOrder_PrintingFabric(lblHeaderId.Text, finalFilePath)
            End If

            'ShutterOcean()

            Dim berhasil As String = String.Format("showSuccessSwal('{0}')", lblHeaderId.Text)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "swalSuccess", berhasil, True)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnSubmitOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Async Sub ShutterOcean()
        Dim api = Await ShutterOceanClass.SendOrderAsync(lblHeaderId.Text)
    End Sub

    Protected Sub btnUnsubmitOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET SubmittedDate=NULL, Status='Unsubmitted' WHERE Id=@Id; DELETE FROM OrderInvoices WHERE Id=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    myCmd.ExecuteNonQuery()
                End Using

                Dim serviceData As DataTable = orderClass.GetDataTable("SELECT OrderDetails.* FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & lblHeaderId.Text & "' AND Products.DesignId='16'")
                If serviceData.Rows.Count > 0 Then
                    For i As Integer = 0 To serviceData.Rows.Count - 1
                        Dim serviceId As String = serviceData.Rows(i).Item("Id").ToString()

                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Active=0 WHERE Id=@ItemId; DELETE FROM OrderCostings WHERE HeaderId=@HeaderId AND ItemId=@ItemId", thisConn)
                            myCmd.Parameters.AddWithValue("@ItemId", serviceId)
                            myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)

                            myCmd.ExecuteNonQuery()
                        End Using
                    Next
                End If

                thisConn.Close()
            End Using

            orderClass.CalculatePriceByOrder(lblHeaderId.Text)

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Order Unsubmitted"}
            orderClass.Logs(dataLog)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnUnsubmitOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnCancelOrder_Click(sender As Object, e As EventArgs)
        MessageError_CancelOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showCancelOrder(); };"
        Try
            If txtCancelDescription.Text = "" Then
                MessageError_CancelOrder(True, "CANCEL DESCRIPTION IS REQURIED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCancelOrder", thisScript, True)
                Exit Sub
            End If

            If msgErrorCancelOrder.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Canceled', CanceledDate=GETDATE(), ProductionDate=NULL, OnHoldDate=NULL, DownloadBOE=0 WHERE Id=@Id; UPDATE OrderShipments SET ShipmentNumber=NULL, ShipmentDate=NULL, ContainerNumber=NULL, Courier=NULL WHERE Id=@Id; UPDATE OrderInvoices SET InvoiceNumber=NULL, Collector=NULL, InvoiceDate=NULL, DueDate=NULL, Payment=0, PaymentDate=NULL, Amount=0 WHERE Id=@Id;", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@StatusDescription", txtCancelDescription.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                If lblCompanyId.Text = "2" Then
                    Dim salesClass As New SalesClass
                    salesClass.RefreshData()
                End If

                dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Order Canceled"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
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

    Protected Sub btnProductionOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='In Production', ProductionDate=GETDATE(), DownloadBOE=1 WHERE Id=@Id; INSERT INTO OrderShipments(Id) VALUES (@Id)", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Order In Production"}
            orderClass.Logs(dataLog)

            ' SALES
            If lblCompanyId.Text = "2" Then
                Dim salesClass As New SalesClass
                salesClass.RefreshData()
            End If

            mailingClass.ProductionOrder(lblHeaderId.Text)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnProductionOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnHoldOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='On Hold', OnHoldDate=GETDATE() WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Order On Hold"}
            orderClass.Logs(dataLog)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnHoldOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnUnHoldOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='In Production', OnHoldDate=NULL, ProductionDate=GETDATE() WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Order In Production (Unhold Order)"}
            orderClass.Logs(dataLog)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnUnHoldOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnShippedOrder_Click(sender As Object, e As EventArgs)
        MessageError_ShippedOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showShippedOrder(); };"
        Try
            If txtShipmentNumber.Text = "" Then
                MessageError_ShippedOrder(True, "SHIPMENT NUMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showShippedOrder", thisScript, True)
                Exit Sub
            End If

            If txtShipmentDate.Text = "" Then
                MessageError_ShippedOrder(True, "SHIPMENT DATE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showShippedOrder", thisScript, True)
                Exit Sub
            End If

            If txtContainerNumber.Text = "" Then
                MessageError_ShippedOrder(True, "CONTAINER NUMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showShippedOrder", thisScript, True)
                Exit Sub
            End If

            If txtCourier.Text = "" Then
                MessageError_ShippedOrder(True, "COURIER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showShippedOrder", thisScript, True)
                Exit Sub
            End If

            If msgErrorShippedOrder.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Shipped Out' WHERE Id=@Id; UPDATE OrderShipments SET ShipmentNumber=@ShipmentNumber, ShipmentDate=@ShipmentDate, ContainerNumber=@ContainerNumber, Courier=@Courier WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@ShipmentNumber", txtShipmentNumber.Text.Trim())
                        myCmd.Parameters.AddWithValue("@ShipmentDate", txtShipmentDate.Text)
                        myCmd.Parameters.AddWithValue("@ContainerNumber", txtContainerNumber.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Courier", txtCourier.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Order Shipped"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_ShippedOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ShippedOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnShippedOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showShippedOrder", thisScript, True)
        End Try
    End Sub

    Protected Sub btnCompleteOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Completed', CompletedDate=GETDATE() WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Order Completed"}
            orderClass.Logs(dataLog)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnCompleteOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnReworkOrder_Click(sender As Object, e As EventArgs)
        MessageError_ReworkOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showReworkOrder(); };"
        Try
            Dim selectedIds As New List(Of String)()

            For Each row As GridViewRow In gvListItemRework.Rows
                Dim chk As CheckBox = TryCast(row.FindControl("chkSelect"), CheckBox)
                If chk IsNot Nothing AndAlso chk.Checked Then
                    Dim id As String = gvListItemRework.DataKeys(row.RowIndex).Value.ToString()

                    selectedIds.Add(id)
                End If
            Next

            If selectedIds.Count = 0 Then
                MessageError_ReworkOrder(True, "PLEASE ADD MINIMAL 1 ITEM FOR REWORK !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showReworkOrder", thisScript, True)
                Exit Sub
            End If

            Dim action As String = "Add"
            Dim reworkId As String = orderClass.GetNewOrderReworkId()

            Dim getReworkId As String = orderClass.GetItemData("SELECT Id FROM OrderReworks WHERE HeaderId='" & lblHeaderId.Text & "' AND Status='Unsubmitted'")
            If Not String.IsNullOrEmpty(getReworkId) Then
                action = "Update" : reworkId = getReworkId
            End If

            If action = "Add" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderReworks VALUES (@Id, @HeaderId, NULL, 'Unsubmitted', @CreatedBy, GETDATE(), NULL, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", reworkId)
                        myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())
                        myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderReworks", reworkId, Session("LoginId").ToString(), "Order Rework Created"}
                orderClass.Logs(dataLog)
            End If

            For Each selectedId As String In selectedIds
                Dim reworkDetailId As String = orderClass.GetNewOrderReworkDetailId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderReworkDetails VALUES (@Id, @ReworkId, @ItemId, NULL, NULL, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", reworkDetailId)
                        myCmd.Parameters.AddWithValue("@ReworkId", reworkId)
                        myCmd.Parameters.AddWithValue("@ItemId", selectedId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Rework/{0}/", reworkDetailId))
                If Not IO.Directory.Exists(directoryOrder) Then
                    IO.Directory.CreateDirectory(directoryOrder)
                End If
            Next

            url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_ReworkOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ReworkOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnShippedOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showReworkOrder", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDetailQuote_Click(sender As Object, e As EventArgs)
        MessageError_DetailQuote(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showDetailQuote(); };"
        Try
            Dim discount As Decimal
            Dim installation As Decimal
            Dim checkMeasure As Decimal
            Dim freight As Decimal

            If Not txtQuoteDiscount.Text = "" Then
                If InStr(txtQuoteDiscount.Text, ",") > 0 Then
                    MessageError_DetailQuote(True, "PLEASE DON'T USE COMMA (,) FOR SEPARATOR ON DISCOUNT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailQuote", thisScript, True)
                    Exit Sub
                End If
                If Not Decimal.TryParse(txtQuoteDiscount.Text, discount) OrElse discount < 0 Then
                    MessageError_DetailQuote(True, "PLEASE CHECK YOUR DISCOUNT FORMAT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailQuote", thisScript, True)
                    Exit Sub
                End If
            End If

            If Not txtQuoteInstallation.Text = "" Then
                If InStr(txtQuoteInstallation.Text, ",") > 0 Then
                    MessageError_DetailQuote(True, "PLEASE DON'T USE COMMA (,) FOR SEPARATOR ON INSTALLATION !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailQuote", thisScript, True)
                    Exit Sub
                End If

                If Not Decimal.TryParse(txtQuoteInstallation.Text, installation) OrElse installation < 0 Then
                    MessageError_DetailQuote(True, "PLEASE CHECK YOUR INSTALLATION FORMAT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailQuote", thisScript, True)
                    Exit Sub
                End If
            End If

            If Not txtQuoteCheckMeasure.Text = "" Then
                If InStr(txtQuoteCheckMeasure.Text, ",") > 0 Then
                    MessageError_DetailQuote(True, "PLEASE DON'T USE COMMA (,) FOR SEPARATOR ON CHECK MEASURE !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailQuote", thisScript, True)
                    Exit Sub
                End If

                If Not Decimal.TryParse(txtQuoteCheckMeasure.Text, checkMeasure) OrElse checkMeasure < 0 Then
                    MessageError_DetailQuote(True, "PLEASE CHECK YOUR CHECK MEASURE FORMAT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailQuote", thisScript, True)
                    Exit Sub
                End If
            End If

            If Not txtQuoteFreight.Text = "" Then
                If InStr(txtQuoteFreight.Text, ",") > 0 Then
                    MessageError_DetailQuote(True, "PLEASE DON'T USE COMMA (,) FOR SEPARATOR ON FREIGHT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailQuote", thisScript, True)
                    Exit Sub
                End If

                If Not Decimal.TryParse(txtQuoteFreight.Text, freight) OrElse freight < 0 Then
                    MessageError_DetailQuote(True, "PLEASE CHECK YOUR CHECK FREIGHT FORMAT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailQuote", thisScript, True)
                    Exit Sub
                End If
            End If

            If msgErrorDetailQuote.InnerText = "" Then
                Dim address As String = txtQuoteAddress.Text.Trim()
                If txtQuoteDiscount.Text = "" Then : txtQuoteDiscount.Text = 0 : End If
                If txtQuoteCheckMeasure.Text = "" Then : txtQuoteCheckMeasure.Text = 0 : End If
                If txtQuoteInstallation.Text = "" Then : txtQuoteInstallation.Text = 0 : End If
                If txtQuoteFreight.Text = "" Then : txtQuoteFreight.Text = 0 : End If

                txtQuoteDiscount.Text.Replace(".", ",")
                txtQuoteCheckMeasure.Text.Replace(".", ",")
                txtQuoteInstallation.Text.Replace(".", ",")
                txtQuoteFreight.Text.Replace(".", ",")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderQuotes SET Email=@Email, Phone=@Phone, Address=@Address, Suburb=@Suburb, City=@City, State=@State, PostCode=@PostCode, Country=@Country, Discount=@Discount, Installation=@Installation, CheckMeasure=@CheckMeasure, Freight=@Freight WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@Email", txtQuoteEmail.Text)
                        myCmd.Parameters.AddWithValue("@Phone", txtQuotePhone.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Address", address)
                        myCmd.Parameters.AddWithValue("@Suburb", txtQuoteSuburb.Text.Trim())
                        myCmd.Parameters.AddWithValue("@City", txtQuoteCity.Text.Trim())
                        myCmd.Parameters.AddWithValue("@State", txtQuoteState.Text.Trim())
                        myCmd.Parameters.AddWithValue("@PostCode", txtQuotePostCode.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Country", ddlQuoteCountry.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Discount", txtQuoteDiscount.Text)
                        myCmd.Parameters.AddWithValue("@CheckMeasure", txtQuoteCheckMeasure.Text)
                        myCmd.Parameters.AddWithValue("@Installation", txtQuoteInstallation.Text)
                        myCmd.Parameters.AddWithValue("@Freight", txtQuoteFreight.Text)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Quote Details Updated"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_DetailQuote(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_DetailQuote(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError_DetailQuote(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDetailQuote_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showDetailQuote", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDownloadQuote_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If gvListItem.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM ORDER !")
                Exit Sub
            End If

            Dim quoteClass As New QuoteClass
            Dim todayString As String = DateTime.Now.ToString("ddMMyyyyHHmmss")
            Dim fileName As String = String.Format("Quote_{0}_{1}.pdf", lblOrderId.Text, todayString)
            Dim pdfFilePath As String = Server.MapPath("~/File/Quote/" & fileName)

            quoteClass.BindContentCustomer(lblHeaderId.Text, pdfFilePath)

            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=""" & fileName & """")
            Response.TransmitFile(pdfFilePath)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDownloadQuote_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnSendInvoice_Click(sender As Object, e As EventArgs)
        MessageError_SendInvoice(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showSendInvoice(); };"
        Try
            If String.IsNullOrEmpty(txtSendInvoiceTo.Text) Then
                MessageError_SendInvoice(True, "CUSTOMER EMAIL TO IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showSendInvoice", thisScript, True)
                Exit Sub
            End If

            Dim isValidEmail As Boolean = orderClass.IsValidEmail(txtSendInvoiceTo.Text)
            If IsValid = False Then
                MessageError_SendInvoice(True, "PLEASE CHECK YOUR CUSTOMER EMAIL TO !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showSendInvoice", thisScript, True)
                Exit Sub
            End If

            Dim ccCustomer As String = String.Empty

            If Not String.IsNullOrEmpty(txtSendInvoiceCCCustomer.Text) Then
                Dim raw As String = txtSendInvoiceCCCustomer.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                ccCustomer = String.Join(";", cleanedEmails)
            End If

            If msgErrorSendInvoice.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()

                    If lblOrderStatus.Text = "Waiting Proforma" Then
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Proforma Sent' WHERE Id=@Id;", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                            myCmd.ExecuteNonQuery()
                        End Using
                    End If

                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderInvoices SET Collector=@Collector, InvoiceDate=GETDATE(), DueDate=DATEADD(DAY, 14, GETDATE()) WHERE Id=@Id;", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@Collector", Session("LoginId").ToString())
                        myCmd.ExecuteNonQuery()
                    End Using

                    thisConn.Close()
                End Using

                Dim todayString As String = DateTime.Now.ToString("ddMMyyyyHHmmss")

                Dim previewClass As New PreviewClass
                Dim pathPreview As String = "~/File/Preview/"
                Dim namePreview As String = String.Format("{0}_{1}.pdf", lblOrderId.Text, todayString)

                Dim pdfPreview As String = Server.MapPath(pathPreview & namePreview)
                previewClass.BindContent(lblHeaderId.Text, pdfPreview)

                Dim invoiceClass As New InvoiceClass
                Dim nameInvoice As String = String.Format("{0}_{1}.pdf", lblInvoiceNumber.Text, todayString)

                Dim pdfInvoice As String = Server.MapPath(String.Format("~/File/Invoice/{0}", nameInvoice))
                invoiceClass.BindContent(lblHeaderId.Text, pdfInvoice)

                mailingClass.SendInvoice(lblHeaderId.Text, pdfPreview, pdfInvoice, Session("LoginId").ToString(), txtSendInvoiceTo.Text, ccCustomer, txtSendInvoiceCCStaff.Text)

                dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Send Invoice"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_SendInvoice(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_SendInvoice(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnSendInvoice_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showSendInvoice", thisScript, True)
        End Try
    End Sub

    Protected Sub btnReceivePayment_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                If lblOrderStatus.Text = "Proforma Sent" Then
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Payment Received' WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.ExecuteNonQuery()
                    End Using
                End If

                Dim amount As Decimal = orderClass.GetItemData_Decimal("SELECT (SUM(SellPrice) * 1.10) AS SumPrice FROM OrderCostings WHERE HeaderId='" & lblHeaderId.Text & "' AND Type='Final'")

                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderInvoices SET PaymentDate=GETDATE(), Payment=1, Amount=@Amount WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@Amount", amount)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Confirm Payment Received"}
            orderClass.Logs(dataLog)

            If lblCompanyId.Text = "2" Then
                Dim salesClass As New SalesClass
                salesClass.RefreshData()
            End If

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnReceivePayment_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnDownloadInvoice_Click(sender As Object, e As EventArgs)
        MessageError_Preview(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showPreview(); };"
        Try
            Dim invoiceClass As New InvoiceClass
            Dim todayString As String = DateTime.Now.ToString("ddMMyyyyHHmmss")
            Dim fileName As String = String.Format("{0}_{1}.pdf", lblInvoiceNumber.Text, todayString)
            Dim pdfFilePath As String = Server.MapPath("~/File/Invoice/" & fileName)

            invoiceClass.BindContent(lblHeaderId.Text, pdfFilePath)

            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=""" & fileName & """")
            Response.TransmitFile(pdfFilePath)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            MessageError_Preview(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Preview(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError_Preview(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDownloadInvoice_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showPreview", thisScript, True)
        End Try
    End Sub

    Protected Sub btnInvoiceNumber_Click(sender As Object, e As EventArgs)
        MessageError_InvoiceNumber(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showInvoiceNumber(); };"
        Try
            If txtUpdateInvoiceNumber.Text.ToLower.Contains("inv") Then
                MessageError_InvoiceNumber(True, "SILAHKAN GUNAKAN INVOICE NUMBER YANG LAIN !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showInvoiceNumber", thisScript, True)
                Exit Sub
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderInvoices SET InvoiceNumber=@InvoiceNumber WHERE Id=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@InvoiceNumber", txtUpdateInvoiceNumber.Text.Trim())

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Update Invoice Number"}
            orderClass.Logs(dataLog)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_InvoiceNumber(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_InvoiceNumber(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnInvoiceNumber_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showInvoiceNumber", thisScript, True)
        End Try
    End Sub

    Protected Sub btnInvoiceData_Click(sender As Object, e As EventArgs)
        MessageError_InvoiceData(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showInvoiceData(); };"
        Try
            Dim amount As Decimal = 0D
            If ddlPayment.SelectedValue = "1" Then
                amount = orderClass.GetItemData_Decimal("SELECT (SUM(SellPrice) * 1.10) AS SumPrice FROM OrderCostings WHERE HeaderId='" & lblHeaderId.Text & "' AND Type='Final'")
            End If
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderInvoices SET InvoiceNumber=@InvoiceNumber, Collector=@Collector, InvoiceDate=@InvoiceDate, PaymentDate=@PaymentDate, Payment=@Payment, Amount=@Amount WHERE Id=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@InvoiceNumber", txtInvoiceNumber.Text.Trim())
                    myCmd.Parameters.AddWithValue("@Collector", ddlCollector.SelectedValue)
                    myCmd.Parameters.AddWithValue("@InvoiceDate", If(String.IsNullOrEmpty(txtInvoiceDate.Text), CType(DBNull.Value, Object), txtInvoiceDate.Text))
                    myCmd.Parameters.AddWithValue("@PaymentDate", If(String.IsNullOrEmpty(txtPaymentDate.Text), CType(DBNull.Value, Object), txtPaymentDate.Text))
                    myCmd.Parameters.AddWithValue("@Payment", ddlPayment.SelectedValue)
                    myCmd.Parameters.AddWithValue("@Amount", amount)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Update Invoice Data"}
            orderClass.Logs(dataLog)

            If lblCompanyId.Text = "2" Then
                Dim salesClass As New SalesClass
                salesClass.RefreshData()
            End If

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_InvoiceData(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_InvoiceData(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnInvoiceData_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showInvoiceData", thisScript, True)
        End Try
    End Sub

    Protected Sub gvListOrderFile_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Try
            Dim fileName As String = e.CommandArgument.ToString()
            Dim directoryPath As String = Server.MapPath(String.Format("~/File/Builder/{0}/", lblOrderId.Text))
            Dim filePath As String = IO.Path.Combine(directoryPath, fileName)

            Select Case e.CommandName
                Case "DownloadFile"
                    If IO.File.Exists(filePath) Then
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
                        Response.TransmitFile(filePath)
                        Response.End()
                    End If

                Case "DeleteFile"
                    If IO.File.Exists(filePath) Then
                        IO.File.Delete(filePath)
                    End If
                    BindDataFile(lblOrderId.Text)
            End Select
        Catch ex As Exception
            MessageError_FileOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_FileOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                Dim dataMailing As Object() = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "gvListBuilderFile_RowCommand", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnBuilderDetail_Click(sender As Object, e As EventArgs)
        MessageError_BuilderDetail(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showBuilderDetail(); };"
        Try
            Dim updates As New Dictionary(Of String, String)()

            For Each row As GridViewRow In gvBuilderDetail.Rows
                If row.RowType = DataControlRowType.DataRow Then
                    Dim fieldName As String = row.Cells(0).Text.Trim()
                    Dim txtValue As TextBox = CType(row.FindControl("txtEditValue"), TextBox)

                    If txtValue IsNot Nothing Then
                        updates(fieldName) = txtValue.Text.Trim()
                    End If
                End If
            Next

            Dim estimatorValue As String = If(updates.ContainsKey("Estimator"), updates("Estimator"), "")
            Dim supervisorValue As String = If(updates.ContainsKey("Supervisor"), updates("Supervisor"), "")
            Dim addressValue As String = If(updates.ContainsKey("Address"), updates("Address"), "")
            Dim callUpRaw As String = If(updates.ContainsKey("Call Up"), updates("Call Up"), "")
            Dim measureRaw As String = If(updates.ContainsKey("Check Measure"), updates("Check Measure"), "")
            Dim installationRaw As String = If(updates.ContainsKey("Installation"), updates("Installation"), "")

            Dim callUpValue As Object = DBNull.Value
            If Not String.IsNullOrWhiteSpace(callUpRaw) Then
                Dim parsedDate As DateTime
                If DateTime.TryParse(callUpRaw, parsedDate) Then
                    callUpValue = parsedDate.ToString("yyyy-MM-dd")
                End If
            End If

            Dim measureValue As Object = DBNull.Value
            If Not String.IsNullOrWhiteSpace(measureRaw) Then
                Dim parsedDate As DateTime
                If DateTime.TryParse(measureRaw, parsedDate) Then
                    measureValue = parsedDate.ToString("yyyy-MM-dd")
                End If
            End If

            Dim installationValue As Object = DBNull.Value
            If Not String.IsNullOrWhiteSpace(installationRaw) Then
                Dim parsedDate As DateTime
                If DateTime.TryParse(installationRaw, parsedDate) Then
                    installationValue = parsedDate.ToString("yyyy-MM-dd")
                End If
            End If

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE OrderBuilders SET Estimator=@Estimator, Supervisor=@Supervisor, Address=@Address, CallUpDate=@CallUp, CheckMeasureDate=@CheckMeasure, InstallationDate=@Installation WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    thisCmd.Parameters.AddWithValue("@Estimator", estimatorValue)
                    thisCmd.Parameters.AddWithValue("@Supervisor", supervisorValue)
                    thisCmd.Parameters.AddWithValue("@Address", addressValue)
                    thisCmd.Parameters.AddWithValue("@CallUp", callUpValue)
                    thisCmd.Parameters.AddWithValue("@CheckMeasure", measureValue)
                    thisCmd.Parameters.AddWithValue("@Installation", installationValue)

                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_BuilderDetail(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_BuilderDetail(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnBuilderDetail_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showBuilderDetail", thisScript, True)
        End Try
    End Sub

    Protected Sub btnUploadFileOrder_Click(sender As Object, e As EventArgs)
        MessageError_FileOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showFileOrder(); };"
        Try
            If Not fuOrderFile.HasFile Then
                MessageError_FileOrder(True, "FILE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showFileOrder", thisScript, True)
                Exit Sub
            End If

            Dim folderPath As String = Server.MapPath(String.Format("~/File/Order/{0}", lblOrderId.Text))
            If Not IO.Directory.Exists(folderPath) Then
                IO.Directory.CreateDirectory(folderPath)
            End If

            Dim fileName As String = IO.Path.GetFileName(fuOrderFile.FileName)
            fuOrderFile.SaveAs(IO.Path.Combine(folderPath, fileName))

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_FileOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_FileOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnUploadFileOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showFileOrder", thisScript, True)
        End Try
    End Sub

    Protected Sub btnMoreDownloadQuote_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If gvListItem.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM ORDER !")
                Exit Sub
            End If

            Dim quoteClass As New QuoteClass
            Dim todayString As String = DateTime.Now.ToString("ddMMyyyyHHmmss")
            Dim fileName As String = String.Format("Quote_{0}_{1}.pdf", lblOrderId.Text, todayString)
            Dim pdfFilePath As String = Server.MapPath("~/File/Quote/" & fileName)

            quoteClass.BindContent(lblHeaderId.Text, pdfFilePath)

            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=""" & fileName & """")
            Response.TransmitFile(pdfFilePath)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnMoreDownloadQuote_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnMoreEmailQuote_Click(sender As Object, e As EventArgs)
        MessageError_MoreEmailQuote(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showEmailQuote(); };"
        Try
            If String.IsNullOrEmpty(txtEmailQuoteTo.Text) Then
                MessageError_MoreEmailQuote(True, "CUSTOMER EMAIL TO IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showEmailQuote", thisScript, True)
                Exit Sub
            End If

            Dim isValidEmail As Boolean = orderClass.IsValidEmail(txtEmailQuoteTo.Text)
            If IsValid = False Then
                MessageError_MoreEmailQuote(True, "PLEASE CHECK YOUR CUSTOMER EMAIL TO !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showEmailQuote", thisScript, True)
                Exit Sub
            End If

            Dim ccCustomer As String = String.Empty
            If Not String.IsNullOrEmpty(txtEmailQuoteCCCustomer.Text) Then
                Dim raw As String = txtEmailQuoteCCCustomer.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                ccCustomer = String.Join(";", cleanedEmails)
            End If

            If msgErrorMoreEmailQuote.InnerText = "" Then
                Dim todayString As String = DateTime.Now.ToString("ddMMyyyyHHmmss")

                Dim previewClass As New PreviewClass
                Dim pathPreview As String = "~/File/Preview/"
                Dim namePreview As String = String.Format("{0}_{1}.pdf", lblOrderId.Text, todayString)

                Dim pdfPreview As String = Server.MapPath(pathPreview & namePreview)
                previewClass.BindContent(lblHeaderId.Text, pdfPreview)

                Dim quoteClass As New QuoteClass
                Dim quoteName As String = String.Format("QUOTE-{0}_{1}.pdf", lblOrderId.Text, todayString)

                Dim pdfQuote As String = Server.MapPath(String.Format("~/File/Quote/{0}", quoteName))
                quoteClass.BindContent(lblHeaderId.Text, pdfQuote)

                mailingClass.SentQuote(lblHeaderId.Text, pdfPreview, pdfQuote, Session("LoginId").ToString(), txtEmailQuoteTo.Text, ccCustomer, txtEmailQuoteCCStaff.Text)

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_MoreEmailQuote(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_MoreEmailQuote(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnMoreEmailQuote_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showEmailQuote", thisScript, True)
        End Try
    End Sub

    Protected Sub btnAddNote_Click(sender As Object, e As EventArgs)
        MessageError_AddNote(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showAddNote(); };"
        Try
            If txtAddNote.Text = "" Then
                MessageError_AddNote(True, "YOUR NOTE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAddNote", thisScript, True)
                Exit Sub
            End If

            If msgErrorAddNote.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderInternalNotes VALUES (NEWID(), @HeaderId, GETDATE(), @CreatedBy, @Note)", thisConn)
                        myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())
                        myCmd.Parameters.AddWithValue("@Note", txtAddNote.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_AddNote(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_AddNote(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnAddNote_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showAddNote", thisScript, True)
        End Try
    End Sub

    Protected Sub gvListItem_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvListItem.PageIndex = e.NewPageIndex
            BindDataItem(lblOrderStatus.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "gvList_PageIndexChanging", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub gvListItem_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Dim thisData As DataRow = orderClass.GetDataRow("SELECT Products.DesignId FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & dataId & "' AND OrderDetails.Active=1")

                    Dim designId As String = thisData("DesignId").ToString()
                    Dim page As String = orderClass.GetDesignPage(designId)

                    If designId = "16" Then
                        MessageError(True, "ACCESS DENIED. YOU ARE NOT AUTHORIZED TO PERFORM THIS ACTION !")
                        Exit Sub
                    End If

                    Dim itemAction As String = "view"
                    If lblOrderStatus.Text = "Unsubmitted" Then
                        itemAction = "edit"
                    End If

                    If lblOrderStatus.Text = "Waiting Proforma" AndAlso (Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office") Then
                        itemAction = "edit"
                    End If

                    If lblOrderStatus.Text = "Quoted" AndAlso (Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Installer" OrElse Session("RoleName") = "Customer Service" OrElse Session("RoleName") = "Sales") Then
                        itemAction = "edit"
                    End If

                    If lblOrderStatus.Text = "New Order" OrElse lblOrderStatus.Text = "In Production" OrElse lblOrderStatus.Text = "On Hold" Then
                        If Session("RoleName") = "Developer" Then
                            itemAction = "edit"
                        End If
                    End If

                    Dim queryString As String = String.Format("do={0}&orderid={1}&itemid={2}&dtype={3}&uid={4}", itemAction, lblHeaderId.Text, dataId, designId, Session("LoginId").ToString())

                    Dim contextId As String = InsertContext(queryString)
                    url = String.Format("{0}?boos={1}", page, contextId)

                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetail_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                End Try
            ElseIf e.CommandName = "Copy" Then
                MessageError(False, String.Empty)
                Try
                    Dim thisData As DataRow = orderClass.GetDataRow("SELECT Products.DesignId FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderDetails.Id='" & dataId & "' AND OrderDetails.Active=1")

                    Dim designId As String = thisData("DesignId").ToString()
                    Dim page As String = orderClass.GetDesignPage(designId)

                    Dim queryString As String = String.Format("do={0}&orderid={1}&itemid={2}&dtype={3}&uid={4}", "copy", lblHeaderId.Text, dataId, designId, Session("LoginId").ToString())

                    Dim contextId As String = InsertContext(queryString)

                    url = String.Format("{0}?boos={1}", page, contextId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkCopy_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                End Try
            ElseIf e.CommandName = "Printing" Then
                MessageError(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showPrinting(); };"
                Try
                    Dim queryString As String = String.Format("headerid={0}&itemid={1}", lblHeaderId.Text, dataId)
                    Dim contextId As String = InsertContext(queryString)
                    url = String.Format("~/order/printing?boos={0}", contextId)
                    Response.Redirect(url)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkPrinting_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showPrinting", thisScript, True)
                End Try
            ElseIf e.CommandName = "EditCosting" Then
                MessageError_EditCosting(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showEditCosting(); };"
                Try
                    Dim queryDetailsPrice As String = "SELECT *, FORMAT(BuyPrice, 'C', 'en-US') AS BuyPricing, FORMAT(SellPrice, 'C', 'en-US') AS SellPricing FROM OrderCostings WHERE ItemId='" & dataId & "' AND Type<>'Final' AND Number<>0 ORDER BY Number, CASE WHEN Type='Base' THEN 1 WHEN Type='Surcharge' THEN 2 ELSE 3 END ASC"
                    If lblCompanyId.Text = "3" OrElse lblCompanyId.Text = "5" Then
                        queryDetailsPrice = "SELECT *, FORMAT(BuyPrice, 'C', 'id-ID') AS BuyPricing, FORMAT(SellPrice, 'C', 'en-US') AS SellPricing FROM OrderCostings WHERE ItemId='" & dataId & "' AND Type<>'Final' AND Number<>0 ORDER BY Number, CASE WHEN Type='Base' THEN 1 WHEN Type='Surcharge' THEN 2 ELSE 3 END ASC"
                    End If

                    lblItemId.Text = dataId

                    gvListEditCosting.DataSource = orderClass.GetDataTable(queryDetailsPrice)
                    gvListEditCosting.DataBind()

                    gvListEditCosting.Columns(3).Visible = False
                    gvListEditCosting.Columns(4).Visible = False

                    If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse (Session("RoleName") = "Sales" AndAlso Session("LevelName") = "Leader") Then
                        gvListEditCosting.Columns(3).Visible = True
                        gvListEditCosting.Columns(4).Visible = True
                    End If

                    ClientScript.RegisterStartupScript(Me.GetType(), "showEditCosting", thisScript, True)
                Catch ex As Exception
                    MessageError_EditCosting(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_EditCosting(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkEditCosting_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showEditCosting", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddItem_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDesign.SelectedValue = "" Then
                Response.Redirect("~/order/detail", False)
                Exit Sub
            End If

            Dim page As String = orderClass.GetDesignPage(ddlDesign.SelectedValue)
            Dim queryString As String = String.Format("do={0}&orderid={1}&itemid={2}&dtype={3}&uid={4}", "create", lblHeaderId.Text, String.Empty, ddlDesign.SelectedValue, Session("LoginId").ToString())
            Dim contextId As String = InsertContext(queryString)

            url = String.Format("{0}?boos={1}", page, contextId)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnAddItem_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnService_Click(sender As Object, e As EventArgs)
        MessageError_Service(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showService(); };"
        Try
            If ddlBlindService.SelectedValue = "" Then
                MessageError_Service(True, "TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showService", thisScript, True)
                Exit Sub
            End If

            Dim checkData As Integer = orderClass.GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails WHERE HeaderId='" & lblHeaderId.Text & "' AND ProductId='" & ddlBlindService.SelectedValue & "' AND Active=1")
            If checkData > 0 Then
                MessageError_Service(True, "SERVICE ALREADY EXISTS !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showService", thisScript, True)
                Exit Sub
            End If

            If msgErrorService.InnerText = "" Then
                Dim newItemId As String = orderClass.GetNewOrderItemId()

                Dim groupName As String = orderClass.GetItemData("SELECT Name FROM Products WHERE Id='" & ddlBlindService.SelectedValue & "'")
                Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, "16")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, Qty, Width, [Drop], TotalItems, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, 1, 0, 0, 1, 0, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", newItemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@ProductId", ddlBlindService.SelectedValue)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(lblHeaderId.Text, newItemId)
                orderClass.CalculatePrice(lblHeaderId.Text, newItemId)
                orderClass.FinalCostItem(lblHeaderId.Text, newItemId)

                If lblCompanyId.Text = "2" AndAlso (lblOrderStatus.Text = "In Production" OrElse lblOrderStatus.Text = "On Hold") Then
                    Dim salesClass As New SalesClass
                    salesClass.RefreshData()
                End If

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_Service(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Service(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnService_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showService", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDeleteItem_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtDeleteItemId.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Active=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderCostings WHERE HeaderId=@HeaderId AND ItemId=@ItemId", thisConn)
                    myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@ItemId", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Dim folderPath As String = Server.MapPath("~/File/Printing/" & thisId)
            If IO.Directory.Exists(folderPath) Then
                IO.Directory.Delete(folderPath, True)
            End If

            Dim dataLog As Object() = {"OrderDetails", thisId, Session("LoginId"), "Delete Order Item"}
            orderClass.Logs(dataLog)

            If lblCompanyId.Text = "2" AndAlso (lblOrderStatus.Text = "In Production" OrElse lblOrderStatus.Text = "On Hold") Then
                Dim salesClass As New SalesClass
                salesClass.RefreshData()
            End If

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDeleteItem_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnEditCosting_Click(sender As Object, e As EventArgs)
        MessageError_EditCosting(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showEditCosting(); };"
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using tran As SqlTransaction = thisConn.BeginTransaction()
                    Try
                        Using delFinal As New SqlCommand("DELETE FROM OrderCostings WHERE ItemId=@ItemId AND Type='Final'", thisConn, tran)
                            delFinal.Parameters.Add("@ItemId", SqlDbType.Int).Value = lblItemId.Text
                            delFinal.ExecuteNonQuery()
                        End Using

                        For Each row As GridViewRow In gvListEditCosting.Rows
                            If row.RowType = DataControlRowType.DataRow Then
                                Dim costingId As String = gvListEditCosting.DataKeys(row.RowIndex).Values("Id").ToString()

                                Dim txtNewBuyPrice As TextBox = CType(row.FindControl("txtNewBuyPrice"), TextBox)
                                Dim txtNewSellPrice As TextBox = CType(row.FindControl("txtNewSellPrice"), TextBox)

                                Dim newBuy As Decimal = 0
                                Dim newSell As Decimal = 0

                                Decimal.TryParse(txtNewBuyPrice.Text, NumberStyles.Any, CultureInfo.CurrentCulture, newBuy)
                                Decimal.TryParse(txtNewSellPrice.Text, NumberStyles.Any, CultureInfo.CurrentCulture, newSell)

                                Dim oldBuy As Decimal = 0
                                Dim oldSell As Decimal = 0

                                Using cmdOld As New SqlCommand("SELECT BuyPrice, SellPrice FROM OrderCostings WHERE Id=@Id", thisConn, tran)
                                    cmdOld.Parameters.AddWithValue("@Id", costingId)

                                    Using rd = cmdOld.ExecuteReader()
                                        If rd.Read() Then
                                            oldBuy = If(IsDBNull(rd("BuyPrice")), 0D, Convert.ToDecimal(rd("BuyPrice")))
                                            oldSell = If(IsDBNull(rd("SellPrice")), 0D, Convert.ToDecimal(rd("SellPrice")))
                                        End If
                                    End Using
                                End Using

                                If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse (Session("RoleName") = "Sales" AndAlso Session("LevelName") = "Leader") Then
                                    If oldBuy <> newBuy Then
                                        Using cmd As New SqlCommand("UPDATE OrderCostings SET BuyPrice=@BuyPrice WHERE Id=@Id", thisConn, tran)
                                            cmd.Parameters.AddWithValue("@Id", costingId)
                                            cmd.Parameters.Add("@BuyPrice", SqlDbType.Decimal).Value = newBuy
                                            cmd.ExecuteNonQuery()
                                        End Using
                                    End If
                                End If

                                If oldSell <> newSell Then
                                    Using cmd As New SqlCommand("UPDATE OrderCostings SET SellPrice=@SellPrice WHERE Id=@Id", thisConn, tran)
                                        cmd.Parameters.AddWithValue("@Id", costingId)
                                        cmd.Parameters.Add("@SellPrice", SqlDbType.Decimal).Value = newSell
                                        cmd.ExecuteNonQuery()
                                    End Using
                                End If
                            End If
                        Next

                        Dim buyPrice As Decimal = 0
                        Dim sellPrice As Decimal = 0

                        Using cmdSum As New SqlCommand("SELECT ISNULL(SUM(CASE WHEN Type='Base' THEN BuyPrice WHEN Type='Discount' THEN -BuyPrice WHEN Type='Surcharge' THEN BuyPrice ELSE 0 END),0) AS TotalBuy, ISNULL(SUM(CASE WHEN Type='Base' THEN SellPrice WHEN Type='Discount' THEN -SellPrice WHEN Type='Surcharge' THEN SellPrice ELSE 0 END),0) AS TotalSell FROM OrderCostings WHERE ItemId=@ItemId", thisConn, tran)
                            cmdSum.Parameters.Add("@ItemId", SqlDbType.Int).Value = lblItemId.Text

                            Using rd = cmdSum.ExecuteReader()
                                If rd.Read() Then
                                    buyPrice = Convert.ToDecimal(rd("TotalBuy"))
                                    sellPrice = Convert.ToDecimal(rd("TotalSell"))
                                End If
                            End Using
                        End Using

                        Dim dataCosting As Object() = {lblHeaderId.Text, lblItemId.Text, 0, "Final", "Final Cost This Item", buyPrice, sellPrice}
                        orderClass.OrderCostings(dataCosting)

                        dataLog = {"OrderDetails", lblItemId.Text, Session("LoginId"), "Update Price"}
                        orderClass.Logs(dataLog)

                        tran.Commit()
                    Catch
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            If lblOrderStatus.Text = "In Production" OrElse lblOrderStatus.Text = "On Hold" Then
                Dim salesClass As New SalesClass
                salesClass.RefreshData()
            End If

            Response.Redirect(String.Format("~/order/detail?orderid={0}", lblHeaderId.Text), False)
            Context.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            MessageError_EditCosting(True, ex.Message)
            If Not Session("RoleName") = "Developer" Then
                MessageError_EditCosting(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnEditCosting_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showEditCosting", thisScript, True)
        End Try
    End Sub

    Protected Sub BindDataOrder(headerId As String)
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

            Dim headerData As DataRow = orderClass.GetDataRowSP("sp_GetHeaderOnDetail", params)
            If headerData Is Nothing Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            lblCustomerId.Text = headerData("CustomerId").ToString()
            lblCompanyId.Text = headerData("CompanyId").ToString()
            lblCompanyDetailId.Text = headerData("CompanyDetailId").ToString()
            lblPriceGroupId.Text = headerData("PriceGroupId").ToString()

            lblCustomerName.Text = headerData("CustomerName").ToString()
            lblOrderId.Text = headerData("OrderId").ToString()
            lblOrderNumber.Text = headerData("OrderNumber").ToString()
            lblOrderName.Text = headerData("OrderName").ToString()
            lblOrderNote.Text = If(String.IsNullOrEmpty(headerData("OrderNote").ToString()), "-", headerData("OrderNote").ToString())
            lblOrderStatus.Text = headerData("Status").ToString()
            lblOrderStatusDescription.Text = headerData("StatusDescription").ToString()
            lblOrderType.Text = headerData("OrderType").ToString()
            lblInternalNote.Text = orderClass.GetItemData("SELECT TOP 1 'Noted By ' + ISNULL(CustomerLogins.FullName, '') + ' | ' + ISNULL(OrderInternalNotes.Note, '') AS NoteDetail FROM OrderInternalNotes LEFT JOIN CustomerLogins ON OrderInternalNotes.CreatedBy=CustomerLogins.Id WHERE OrderInternalNotes.HeaderId='" & headerId & "' ORDER BY OrderInternalNotes.CreatedDate DESC;")
            If lblInternalNote.Text = "" Then lblInternalNote.Text = "-"
            lblCreatedBy.Text = headerData("CreatedBy").ToString()
            lblCreatedName.Text = headerData("CreatedFullName").ToString()
            lblCreatedRole.Text = headerData("CreatedRole").ToString()

            lblCreatedDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("CreatedDate").ToString()) Then
                lblCreatedDate.Text = Convert.ToDateTime(headerData("CreatedDate")).ToString("dd MMM yyyy")
            End If

            lblQuotedDate.Text = String.Empty
            If Not String.IsNullOrEmpty(headerData("QuotedDate").ToString()) Then
                lblQuotedDate.Text = Convert.ToDateTime(headerData("QuotedDate")).ToString("dd MMM yyyy")
            End If

            lblSubmittedDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("SubmittedDate").ToString()) Then
                lblSubmittedDate.Text = Convert.ToDateTime(headerData("SubmittedDate")).ToString("dd MMM yyyy")
            End If

            lblProductionDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("ProductionDate").ToString()) Then
                lblProductionDate.Text = Convert.ToDateTime(headerData("ProductionDate")).ToString("dd MMM yyyy")
            End If

            lblOnHoldDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("OnHoldDate").ToString()) Then
                lblOnHoldDate.Text = Convert.ToDateTime(headerData("OnHoldDate")).ToString("dd MMM yyyy")
            End If

            lblCanceledDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("CanceledDate").ToString()) Then
                lblCanceledDate.Text = Convert.ToDateTime(headerData("CanceledDate")).ToString("dd MMM yyyy")
            End If

            lblCompletedDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("CompletedDate").ToString()) Then
                lblCompletedDate.Text = Convert.ToDateTime(headerData("CompletedDate")).ToString("dd MMM yyyy")
            End If

            BindDesignType()
            BindDataQuote()
            BindDataItem(lblOrderStatus.Text)
            BindDataCosting(gvListItem.Rows.Count)
            BindDataInvoice()
            BindDataShipment()
            If lblOrderType.Text = "Builder" Then
                BindDataBuilder()
            End If
            If lblOrderStatus.Text = "Shipped Out" OrElse lblOrderStatus.Text = "Completed" Then
                BindDataRework(headerId)
            End If
            BindCollector()
            BindEmailQuote()
            BindEmailInvoice()
            BindDataFile(lblOrderId.Text)

            divInternalNote.Visible = False
            secBuilder.Visible = False
            btnEditHeader.Visible = False
            aDeleteOrder.Visible = False
            aQuoteOrder.Visible = False
            aSubmitOrder.Visible = False
            aUnsubmitOrder.Visible = False
            aCancelOrder.Visible = False
            aProductionOrder.Visible = False
            aHoldOrder.Visible = False
            aUnHoldOrder.Visible = False
            aShippedOrder.Visible = False
            aCompleteOrder.Visible = False
            aReworkOrder.Visible = False

            btnQuoteAction.Visible = False

            btnInvoice.Visible = False
            aSendInvoice.Visible = False : aReceivePayment.Visible = False
            liDividerInvoice.Visible = False : liUpdateInvoiceNumber.Visible = False
            liUpdateInvoiceData.Visible = False

            aBuilder.Visible = False
            aFileOrder.Visible = False

            btnMoreAction.Visible = False
            liMoreDownloadQuote.Visible = False
            liMoreEmailQuote.Visible = False
            liMoreDividerQuote.Visible = False
            liMoreAddNote.Visible = False
            liMoreHistoryNote.Visible = False

            aAddItem.Visible = False
            aService.Visible = False

            Dim isReworkOrder As Boolean = orderClass.IsReworkOrder(headerId)

            Dim checkRework As Integer = orderClass.CheckRework(headerId)
            Dim roleName As String = orderClass.GetUserRoleName(lblCreatedBy.Text)

            If Session("RoleName") = "Developer" Then
                divInternalNote.Visible = True

                btnMoreAction.Visible = True
                liMoreAddNote.Visible = True
                liMoreHistoryNote.Visible = True

                aFileOrder.Visible = True

                If lblOrderType.Text = "Regular" Then btnQuoteAction.Visible = True
                If lblOrderType.Text = "Builder" Then
                    aBuilder.Visible = True : secBuilder.Visible = True
                End If

                If lblOrderStatus.Text = "Unsubmitted" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True
                    liMoreDividerQuote.Visible = True

                    btnEditHeader.Visible = True : aDeleteOrder.Visible = True

                    If lblOrderType.Text = "Regular" Then aSubmitOrder.Visible = True
                    If lblOrderType.Text = "Builder" Then aQuoteOrder.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "Quoted" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True
                    liMoreDividerQuote.Visible = True

                    btnEditHeader.Visible = True
                    aDeleteOrder.Visible = True
                    aSubmitOrder.Visible = True

                    aAddItem.Visible = True
                End If

                If lblOrderStatus.Text = "Waiting Proforma" Then
                    btnEditHeader.Visible = True
                    aUnsubmitOrder.Visible = True : aCancelOrder.Visible = True

                    btnInvoice.Visible = True : aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceNumber.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "Proforma Sent" Then
                    btnEditHeader.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True : aReceivePayment.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceNumber.Visible = True

                    aAddItem.Visible = True
                    aService.Visible = True
                End If

                If lblOrderStatus.Text = "Payment Received" Then
                    btnEditHeader.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aProductionOrder.Visible = True
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "Approved Rework" Then
                    btnEditHeader.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aProductionOrder.Visible = True
                    aCancelOrder.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "New Order" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True
                    liMoreDividerQuote.Visible = True

                    btnEditHeader.Visible = True

                    aUnsubmitOrder.Visible = True : aCancelOrder.Visible = True
                    aHoldOrder.Visible = True : aProductionOrder.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "In Production" Then
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True
                    aShippedOrder.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "On Hold" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aUnHoldOrder.Visible = True : aCancelOrder.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "Shipped Out" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aProductionOrder.Visible = True : aCompleteOrder.Visible = True

                    If isReworkOrder = False Then
                        aReworkOrder.Visible = True
                    End If
                End If

                If lblOrderStatus.Text = "Completed" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    If isReworkOrder = False Then
                        aReworkOrder.Visible = True
                    End If
                End If
            End If

            If Session("RoleName") = "IT" Then
                divInternalNote.Visible = True

                btnMoreAction.Visible = True
                liMoreAddNote.Visible = True
                liMoreHistoryNote.Visible = True

                aFileOrder.Visible = True

                If lblOrderType.Text = "Regular" Then btnQuoteAction.Visible = True
                If lblOrderType.Text = "Builder" Then aBuilder.Visible = True : secBuilder.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True

                    liMoreDividerQuote.Visible = True
                    If lblOrderType.Text = "Regular" Then aSubmitOrder.Visible = True
                    If lblOrderType.Text = "Builder" Then aQuoteOrder.Visible = True

                    btnEditHeader.Visible = True
                    aDeleteOrder.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "Waiting Proforma" Then
                    btnInvoice.Visible = True : aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceNumber.Visible = True

                    btnEditHeader.Visible = True
                    aUnsubmitOrder.Visible = True : aCancelOrder.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "Proforma Sent" Then
                    btnInvoice.Visible = True : aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceNumber.Visible = True

                    aUnsubmitOrder.Visible = True : aCancelOrder.Visible = True
                End If

                If lblOrderStatus.Text = "Payment Received" Then
                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aProductionOrder.Visible = True
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "New Order" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True
                    liMoreDividerQuote.Visible = True

                    aUnsubmitOrder.Visible = True : aCancelOrder.Visible = True
                    aProductionOrder.Visible = True : aHoldOrder.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aAddItem.Visible = True : aService.Visible = True
                    End If
                End If

                If lblOrderStatus.Text = "In Production" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True
                    liMoreDividerQuote.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aHoldOrder.Visible = True : aCancelOrder.Visible = True
                    aShippedOrder.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aAddItem.Visible = True : aService.Visible = True
                    End If
                End If

                If lblOrderStatus.Text = "On Hold" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True
                    liMoreDividerQuote.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aProductionOrder.Visible = True : aCancelOrder.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aAddItem.Visible = True : aService.Visible = True
                    End If
                End If

                If lblOrderStatus.Text = "Shipped Out" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aCompleteOrder.Visible = True
                    If isReworkOrder = False Then
                        aReworkOrder.Visible = True
                    End If
                End If

                If lblOrderStatus.Text = "Completed" Then
                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    If isReworkOrder = False Then
                        aReworkOrder.Visible = True
                    End If
                End If
            End If

            If Session("RoleName") = "Factory Office" Then
                divInternalNote.Visible = True

                btnMoreAction.Visible = True
                liMoreAddNote.Visible = True
                liMoreHistoryNote.Visible = True

                aFileOrder.Visible = True

                If lblOrderType.Text = "Builder" Then aBuilder.Visible = True : secBuilder.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True
                    liMoreDividerQuote.Visible = True

                    If lblOrderType.Text = "Regular" Then aSubmitOrder.Visible = True
                    If lblOrderType.Text = "Builder" Then aQuoteOrder.Visible = True

                    btnEditHeader.Visible = True : aDeleteOrder.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "Waiting Proforma" Then
                    liMoreDividerQuote.Visible = True
                    liMoreAddNote.Visible = True
                    liMoreHistoryNote.Visible = True

                    btnInvoice.Visible = True : aSendInvoice.Visible = True

                    btnEditHeader.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True

                    aAddItem.Visible = True
                    aService.Visible = True
                End If

                If lblOrderStatus.Text = "Proforma Sent" Then
                    liMoreDividerQuote.Visible = True
                    liMoreAddNote.Visible = True
                    liMoreHistoryNote.Visible = True

                    btnInvoice.Visible = True : aSendInvoice.Visible = True : aReceivePayment.Visible = True
                End If

                If lblOrderStatus.Text = "Payment Received" Then
                    liMoreDividerQuote.Visible = True
                    liMoreAddNote.Visible = True
                    liMoreHistoryNote.Visible = True

                    btnInvoice.Visible = True
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True
                End If

                If lblOrderStatus.Text = "New Order" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True
                    liMoreDividerQuote.Visible = True

                    aCancelOrder.Visible = True : aHoldOrder.Visible = True : aProductionOrder.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aAddItem.Visible = True : aService.Visible = True
                    End If
                End If

                If lblOrderStatus.Text = "In Production" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aHoldOrder.Visible = True : aCancelOrder.Visible = True : aShippedOrder.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aAddItem.Visible = True : aService.Visible = True
                    End If
                End If

                If lblOrderStatus.Text = "On Hold" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aProductionOrder.Visible = True : aCancelOrder.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aAddItem.Visible = True : aService.Visible = True
                    End If
                End If

                If lblOrderStatus.Text = "Shipped Out" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aCompleteOrder.Visible = True
                    If isReworkOrder = False Then
                        aReworkOrder.Visible = True
                    End If
                End If

                If lblOrderStatus.Text = "Completed" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    If isReworkOrder = False Then
                        aReworkOrder.Visible = True
                    End If
                End If
            End If

            If Session("RoleName") = "Sales" Then
                divInternalNote.Visible = True

                btnMoreAction.Visible = True
                liMoreAddNote.Visible = True
                liMoreHistoryNote.Visible = True

                aFileOrder.Visible = True

                If lblOrderType.Text = "Builder" Then aBuilder.Visible = True : secBuilder.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    liMoreDividerQuote.Visible = True
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True

                    If lblOrderType.Text = "Regular" Then
                        If Session("LoginId") = lblCreatedBy.Text Then
                            btnEditHeader.Visible = True
                            aDeleteOrder.Visible = True
                        End If
                        aSubmitOrder.Visible = True
                        aAddItem.Visible = True
                    End If

                    If lblOrderType.Text = "Builder" Then
                        btnEditHeader.Visible = True
                        aQuoteOrder.Visible = True
                        aAddItem.Visible = True
                    End If
                End If

                If lblOrderStatus.Text = "Waiting Proforma" Then
                    btnInvoice.Visible = True : aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceNumber.Visible = True

                    aUnsubmitOrder.Visible = True : aCancelOrder.Visible = True

                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "Proforma Sent" Then
                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True : aReceivePayment.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceNumber.Visible = True
                End If

                If lblOrderStatus.Text = "Payment Received" Then
                    btnInvoice.Visible = True
                End If

                If lblOrderStatus.Text = "New Order" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True
                End If

                If lblOrderStatus.Text = "In Production" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True
                End If

                If lblOrderStatus.Text = "On Hold" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True
                End If

                If lblOrderStatus.Text = "Shipped Out" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True
                End If

                If lblOrderStatus.Text = "Completed" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True
                End If
            End If

            If Session("RoleName") = "Account" Then
                divInternalNote.Visible = True

                btnMoreAction.Visible = True
                liMoreAddNote.Visible = True
                liMoreHistoryNote.Visible = True

                aFileOrder.Visible = True

                If lblOrderType.Text = "Builder" Then aBuilder.Visible = True : secBuilder.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    liMoreDividerQuote.Visible = True
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True

                    If Session("LoginId") = lblCreatedBy.Text Then
                        btnEditHeader.Visible = True
                        aDeleteOrder.Visible = True
                    End If

                    aService.Visible = True
                End If

                If lblOrderStatus.Text = "Waiting Proforma" Then
                    aCancelOrder.Visible = True : aUnsubmitOrder.Visible = True

                    btnInvoice.Visible = True : aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceNumber.Visible = True

                    aUnsubmitOrder.Visible = True : aCancelOrder.Visible = True

                    aService.Visible = True
                End If

                If lblOrderStatus.Text = "Proforma Sent" Then
                    aCancelOrder.Visible = True : aUnsubmitOrder.Visible = True
                    btnInvoice.Visible = True : aSendInvoice.Visible = True : aReceivePayment.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceNumber.Visible = True
                End If

                If lblOrderStatus.Text = "Payment Received" Then
                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True
                End If

                If lblOrderStatus.Text = "New Order" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True
                    liMoreDividerQuote.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aService.Visible = True
                End If

                If lblOrderStatus.Text = "In Production" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aService.Visible = True
                End If

                If lblOrderStatus.Text = "On Hold" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aService.Visible = True
                End If

                If lblOrderStatus.Text = "Shipped Out" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aService.Visible = True
                End If

                If lblOrderStatus.Text = "Completed" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True : liUpdateInvoiceData.Visible = True

                    aService.Visible = True
                End If
            End If

            If Session("RoleName") = "Customer Service" Then
                btnMoreAction.Visible = True
                liMoreAddNote.Visible = True
                liMoreHistoryNote.Visible = True

                aFileOrder.Visible = True

                If lblOrderType.Text = "Builder" Then aBuilder.Visible = True : secBuilder.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    If lblOrderType.Text = "Builder" Then aQuoteOrder.Visible = True

                    aAddItem.Visible = True
                End If

                If lblOrderStatus.Text = "Quoted" Then
                    liMoreDownloadQuote.Visible = True
                    liMoreEmailQuote.Visible = True
                    liMoreDividerQuote.Visible = True

                    btnEditHeader.Visible = True
                    aDeleteOrder.Visible = True

                    aAddItem.Visible = True
                End If

                If lblOrderStatus.Text = "New Order" Then
                    aUnsubmitOrder.Visible = True : aCancelOrder.Visible = True
                    aHoldOrder.Visible = True : aProductionOrder.Visible = True

                    aAddItem.Visible = True
                End If

                If lblOrderStatus.Text = "Waiting Proforma" Then
                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "Payment Received" Then
                    aAddItem.Visible = True : aService.Visible = True
                End If

                If lblOrderStatus.Text = "In Production" Then
                    aAddItem.Visible = True : aService.Visible = True

                    aHoldOrder.Visible = True : aCancelOrder.Visible = True
                End If

                If lblOrderStatus.Text = "On Hold" Then
                    aProductionOrder.Visible = True
                End If

                If lblOrderStatus.Text = "Shipped Out" OrElse lblOrderStatus.Text = "Completed" Then
                    aCompleteOrder.Visible = True
                    aReworkOrder.Visible = True
                End If
            End If

            If Session("RoleName") = "Export" Then
                If lblOrderStatus.Text = "Unsubmitted" OrElse lblOrderStatus.Text = "Waiting Proforma" OrElse lblOrderStatus.Text = "Proforma Sent" OrElse lblOrderStatus.Text = "Payment Received" OrElse lblOrderStatus.Text = "New Order" OrElse lblOrderStatus.Text = "On Hold" OrElse lblOrderStatus.Text = "Canceled" Then
                    Response.Redirect("~/order", False)
                    Exit Sub
                End If

                divInternalNote.Visible = True

                btnMoreAction.Visible = True
                liMoreAddNote.Visible = True
                liMoreHistoryNote.Visible = True

                If lblOrderStatus.Text = "In Production" Then
                    aShippedOrder.Visible = True
                End If

                If lblOrderStatus.Text = "Shipped Out" Then
                    aShippedOrder.Visible = True
                    aCompleteOrder.Visible = True
                End If
            End If

            If Session("RoleName") = "Customer" Then
                btnQuoteAction.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    btnEditHeader.Visible = True
                    aDeleteOrder.Visible = True
                    aSubmitOrder.Visible = True
                    aAddItem.Visible = True
                End If

                If lblOrderStatus.Text = "Shipped Out" OrElse lblOrderStatus.Text = "Completed" Then
                    aReworkOrder.Visible = True
                End If
            End If

            If Session("RoleName") = "Data Entry" Then
                If lblOrderType.Text = "Regular" Then btnQuoteAction.Visible = True
                If lblOrderType.Text = "Builder" Then aBuilder.Visible = True : secBuilder.Visible = True

                divInternalNote.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    If lblCreatedRole.Text = Session("RoleId") Then
                        btnEditHeader.Visible = True
                        aDeleteOrder.Visible = True

                        aAddItem.Visible = True

                        If lblOrderType.Text = "Regular" Then aSubmitOrder.Visible = True
                        If lblOrderType.Text = "Builder" Then aQuoteOrder.Visible = True
                    End If

                End If

                If lblOrderStatus.Text = "Quoted" Then
                    btnMoreAction.Visible = True
                    liMoreAddNote.Visible = True
                    liMoreHistoryNote.Visible = True
                End If
            End If

            If Session("RoleName") = "Installer" Then
                If lblOrderStatus.Text = "Quoted" Then
                    aSubmitOrder.Visible = True
                End If
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

    Protected Sub BindDesignType()
        ddlDesign.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT Designs.Id, Designs.Name AS NameText FROM CustomerProductAccess CROSS APPLY STRING_SPLIT(CustomerProductAccess.DesignId, ',') AS designArray INNER JOIN Designs ON designArray.VALUE=Designs.Id WHERE CustomerProductAccess.Id='" & lblCustomerId.Text & "' AND Designs.Type<>'Additional' AND Designs.Active=1 ORDER BY Designs.Name ASC"

            ddlDesign.DataSource = orderClass.GetDataTable(thisQuery)
            ddlDesign.DataTextField = "NameText"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDesignType", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindBlindTypeService()
        ddlBlindService.Items.Clear()
        Try
            ddlBlindService.DataSource = orderClass.GetDataTable("SELECT * FROM Products WHERE DesignId='16' ORDER BY Name ASC")
            ddlBlindService.DataTextField = "Name"
            ddlBlindService.DataValueField = "Id"
            ddlBlindService.DataBind()

            If ddlBlindService.Items.Count > 0 Then
                ddlBlindService.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDesignType", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindCollector()
        ddlCollector.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT * FROM CustomerLogins"
            If Session("RoleName") = "Account" OrElse Session("RoleName") = "Sales" Then
                thisQuery = "SELECT * FROM CustomerLogins WHERE RoleId='4' OR RoleId='5'"
            End If

            ddlCollector.DataSource = orderClass.GetDataTable(thisQuery)
            ddlCollector.DataTextField = "FullName"
            ddlCollector.DataValueField = "Id"
            ddlCollector.DataBind()

            If ddlCollector.Items.Count > 0 Then
                ddlCollector.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCollector.Items.Add(New ListItem(ex.ToString(), ""))
        End Try
    End Sub

    Protected Sub BindDataItem(status As String)
        divMinimumOrderSurcharge.Visible = False
        Try
            Dim param As New List(Of SqlParameter) From {
                New SqlParameter("@HeaderId", Convert.ToInt32(lblHeaderId.Text))
            }

            gvListItem.DataSource = orderClass.GetDataTableSP("sp_GetOrderItemsByHeader", param)
            gvListItem.DataBind()

            gvListItem.Columns(1).Visible = PageAction("Visible ID")
            gvListItem.Columns(2).Visible = PageAction("Visible Product ID")

            gvListItem.Columns(4).Visible = PageAction("Visible Buy Price")
            gvListItem.Columns(5).Visible = PageAction("Visible Sell Price")
            gvListItem.Columns(6).Visible = PageAction("Visible Price")

            gvListItem.Columns(7).Visible = False
            If Session("PriceAccess") = "Yes" Then gvListItem.Columns(7).Visible = True

            Dim totalItems As Integer = orderClass.GetTotalItemOrder(lblHeaderId.Text)
            If status = "Unsubmitted" AndAlso lblPriceGroupId.Text = "1" AndAlso totalItems > 0 AndAlso totalItems <= 3 Then
                divMinimumOrderSurcharge.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataItem", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDataQuote()
        Dim quoteData As DataRow = orderClass.GetDataRow("SELECT * FROM OrderQuotes WHERE Id='" & lblHeaderId.Text & "'")

        If quoteData Is Nothing Then
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderQuotes VALUES(@Id, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00)", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Response.Redirect("~/order/detail", False)
        End If

        imgDetailQuote.ImageUrl = "~/assets/images/quotation/quotefordetail.jpg"
        If lblCompanyId.Text = "3" OrElse lblCompanyId.Text = "5" Then imgDetailQuote.ImageUrl = "~/assets/images/quotation/quotefordetail_local.jpg"

        divQuoteCity.Visible = False

        spanDiscount.InnerText = "$"
        spanMeasure.InnerText = "$"
        spanInstall.InnerText = "$"
        spanFreight.InnerText = "$"

        If lblCompanyId.Text = "3" OrElse lblCompanyId.Text = "5" Then
            divQuoteCity.Visible = True

            spanDiscount.InnerText = "Rp"
            spanMeasure.InnerText = "Rp"
            spanInstall.InnerText = "Rp"
            spanFreight.InnerText = "Rp"
        End If

        ddlQuoteCountry.Items.Clear()
        ddlQuoteCountry.Items.Add(New ListItem("", ""))
        If lblCompanyId.Text = "2" OrElse lblCompanyId.Text = "4" Then
            ddlQuoteCountry.Items.Add(New ListItem("Australia", "Australia"))
        End If
        If lblCompanyId.Text = "3" OrElse lblCompanyId.Text = "5" Then
            ddlQuoteCountry.Items.Add(New ListItem("Indonesia", "Indonesia"))
        End If

        txtQuoteEmail.Text = quoteData("Email").ToString()
        txtQuotePhone.Text = quoteData("Phone").ToString()
        txtQuoteAddress.Text = quoteData("Address").ToString()
        txtQuoteSuburb.Text = quoteData("Suburb").ToString()
        txtQuoteCity.Text = quoteData("City").ToString()
        txtQuoteState.Text = quoteData("State").ToString()
        txtQuotePostCode.Text = quoteData("PostCode").ToString()
        ddlQuoteCountry.SelectedValue = quoteData("Country").ToString()

        txtQuoteDiscount.Text = quoteData("Discount").ToString().Replace(",", ".")
        txtQuoteCheckMeasure.Text = quoteData("CheckMeasure").ToString().Replace(",", ".")
        txtQuoteInstallation.Text = quoteData("Installation").ToString().Replace(",", ".")
        txtQuoteFreight.Text = quoteData("Freight").ToString().Replace(",", ".")
    End Sub

    Protected Sub BindDataCosting(itemCount As Integer)
        divPricing.Visible = False
        If Session("PriceAccess") = "Yes" Then divPricing.Visible = True
        lblPriceOrder.Text = "-"
        lblGst.Text = "-"
        lblFinalPriceOrder.Text = "-"

        lblPriceOrderTitle.Text = "Total excl. GST"
        lblGstTitle.Text = "GST 10%"
        lblFinalPriceOrderTitle.Text = "TOTAL incl. GST"
        If lblCompanyId.Text = "3" OrElse lblCompanyId.Text = "5" Then
            lblPriceOrderTitle.Text = "Total excl. PPN"
            lblGstTitle.Text = "PPN 11%"
            lblFinalPriceOrderTitle.Text = "TOTAL incl. PPN"
        End If

        If itemCount > 0 Then
            Dim sumPrice As Decimal = orderClass.GetItemData_Decimal("SELECT SUM(SellPrice) AS SumPrice FROM OrderCostings WHERE HeaderId='" & lblHeaderId.Text & "' AND Type='Final'")
            Dim gst As Decimal = sumPrice * 10 / 100
            If lblCompanyId.Text = "3" OrElse lblCompanyId.Text = "5" Then
                gst = sumPrice * 11 / 100
            End If
            Dim finaltotal As Decimal = sumPrice + gst

            lblPriceOrder.Text = "$ " & sumPrice.ToString("N2", enUS)
            lblGst.Text = "$ " & gst.ToString("N2", enUS)
            lblFinalPriceOrder.Text = "$ " & finaltotal.ToString("N2", enUS)

            If lblCompanyId.Text = "3" OrElse lblCompanyId.Text = "5" Then
                lblPriceOrder.Text = "Rp " & sumPrice.ToString("N2", idIDR)
                lblGst.Text = "Rp " & gst.ToString("N2", idIDR)
                lblFinalPriceOrder.Text = "Rp " & finaltotal.ToString("N2", idIDR)
            End If
        End If
    End Sub

    Protected Sub BindDataInvoice()
        Dim param As New List(Of SqlParameter) From {
            New SqlParameter("@InvoiceId", Convert.ToInt32(lblHeaderId.Text))
        }

        Dim invoiceData As DataRow = orderClass.GetDataRowSP("sp_GetInvoiceById", param)

        lblInvoiceNumber.Text = "-"
        lblInvoiceDate.Text = "-"
        lblCollector.Text = "-"
        lblPaymentDate.Text = "-"

        If invoiceData IsNot Nothing Then

            If Not String.IsNullOrEmpty(invoiceData("InvoiceNumber").ToString()) Then
                lblInvoiceNumber.Text = invoiceData("InvoiceNumber").ToString()
                txtUpdateInvoiceNumber.Text = invoiceData("InvoiceNumber").ToString()
                txtInvoiceNumber.Text = invoiceData("InvoiceNumber").ToString()
            End If

            If Not String.IsNullOrEmpty(invoiceData("InvoiceDate").ToString()) Then
                lblInvoiceDate.Text = Convert.ToDateTime(invoiceData("InvoiceDate")).ToString("dd MMM yyyy")
                txtInvoiceDate.Text = Convert.ToDateTime(invoiceData("InvoiceDate")).ToString("yyyy-MM-dd")
            End If

            If Not String.IsNullOrEmpty(invoiceData("CollectorName").ToString()) Then
                lblCollector.Text = invoiceData("CollectorName").ToString()
                ddlCollector.SelectedValue = invoiceData("Collector").ToString()
            End If

            If Not String.IsNullOrEmpty(invoiceData("PaymentDate").ToString()) Then
                lblPaymentDate.Text = Convert.ToDateTime(invoiceData("PaymentDate")).ToString("dd MMM yyyy")
                txtPaymentDate.Text = Convert.ToDateTime(invoiceData("PaymentDate")).ToString("yyyy-MM-dd")
            End If

            ddlPayment.SelectedValue = Convert.ToInt32(invoiceData("Payment"))
            lblOrderPaid.Text = invoiceData("OrderPaid").ToString()
        End If
    End Sub

    Protected Sub BindDataShipment()
        Dim param As New List(Of SqlParameter) From {
            New SqlParameter("@ShipmentId", Convert.ToInt32(lblHeaderId.Text))
        }

        Dim shipmentData As DataRow = orderClass.GetDataRowSP("sp_GetShipmentById", param)

        lblShipmentNumber.Text = "-"
        lblShipmentDate.Text = "-"
        lblContainerNumber.Text = "-"
        lblCourier.Text = "-"

        If shipmentData IsNot Nothing Then

            If Not String.IsNullOrEmpty(shipmentData("ShipmentNumber").ToString()) Then
                lblShipmentNumber.Text = shipmentData("ShipmentNumber").ToString()
                txtShipmentNumber.Text = shipmentData("ShipmentNumber").ToString()
            End If

            If Not String.IsNullOrEmpty(shipmentData("ShipmentDate").ToString()) Then
                lblShipmentDate.Text = Convert.ToDateTime(shipmentData("ShipmentDate")).ToString("dd MMM yyyy")
                txtShipmentDate.Text = Convert.ToDateTime(shipmentData("ShipmentDate")).ToString("yyyy-MM-dd")
            End If

            If Not String.IsNullOrEmpty(shipmentData("ContainerNumber").ToString()) Then
                lblContainerNumber.Text = shipmentData("ContainerNumber").ToString()
                txtContainerNumber.Text = shipmentData("ContainerNumber").ToString()
            End If

            If Not String.IsNullOrEmpty(shipmentData("Courier").ToString()) Then
                lblCourier.Text = shipmentData("Courier").ToString()
                txtCourier.Text = shipmentData("Courier").ToString()
            End If

        End If
    End Sub

    Protected Sub BindDataBuilder()
        Try
            Dim dataBuilder As DataRow = orderClass.GetDataRow("SELECT * FROM OrderBuilders WHERE Id='" & lblHeaderId.Text & "'")

            If dataBuilder IsNot Nothing Then
                lblEstimator.Text = dataBuilder("Estimator").ToString()
                lblSupervisor.Text = dataBuilder("Supervisor").ToString()
                lblAddress.Text = dataBuilder("Address").ToString()

                Dim dtDisplay As New DataTable()
                dtDisplay.Columns.Add("Label", GetType(String))
                dtDisplay.Columns.Add("EditValue", GetType(String))
                dtDisplay.Columns.Add("FieldType", GetType(String))

                dtDisplay.Rows.Add("Estimator", dataBuilder("Estimator").ToString(), "text")
                dtDisplay.Rows.Add("Supervisor", dataBuilder("Supervisor").ToString(), "text")
                dtDisplay.Rows.Add("Address", dataBuilder("Address").ToString(), "text")

                Dim callUpEdit As String = ""
                lblCallUpDate.Text = String.Empty
                If Not IsDBNull(dataBuilder("CallUpDate")) AndAlso Not String.IsNullOrEmpty(dataBuilder("CallUpDate").ToString()) Then
                    Dim tmp As DateTime
                    If DateTime.TryParse(dataBuilder("CallUpDate").ToString(), tmp) Then
                        callUpEdit = tmp.ToString("yyyy-MM-dd")
                        lblCallUpDate.Text = tmp.ToString("dd MMM yyyy")
                    End If
                End If
                dtDisplay.Rows.Add("Call Up", callUpEdit, "date")

                Dim measureEdit As String = ""
                lblMeasure.Text = String.Empty
                If Not IsDBNull(dataBuilder("CheckMeasureDate")) AndAlso Not String.IsNullOrEmpty(dataBuilder("CheckMeasureDate").ToString()) Then
                    Dim tmp As DateTime
                    If DateTime.TryParse(dataBuilder("CheckMeasureDate").ToString(), tmp) Then
                        measureEdit = tmp.ToString("yyyy-MM-dd")
                        lblMeasure.Text = tmp.ToString("dd MMM yyyy")
                    End If
                End If
                dtDisplay.Rows.Add("Check Measure", measureEdit, "date")

                Dim installationEdit As String = ""
                lblInstallation.Text = String.Empty
                If Not IsDBNull(dataBuilder("InstallationDate")) AndAlso Not String.IsNullOrEmpty(dataBuilder("InstallationDate").ToString()) Then
                    Dim tmp As DateTime
                    If DateTime.TryParse(dataBuilder("InstallationDate").ToString(), tmp) Then
                        installationEdit = tmp.ToString("yyyy-MM-dd")
                        lblInstallation.Text = tmp.ToString("dd MMM yyyy")
                    End If
                End If
                dtDisplay.Rows.Add("Installation", installationEdit, "date")

                gvBuilderDetail.DataSource = dtDisplay
                gvBuilderDetail.DataBind()
            End If
        Catch ex As Exception
            MessageError_BuilderDetail(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_BuilderDetail(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataBuilder", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDataFile(orderId As String)
        Try
            Dim path As String = Server.MapPath("~/File/Order/" & lblOrderId.Text)

            Dim dt As New DataTable
            dt.Columns.Add("FolderName")
            dt.Columns.Add("FileName")
            dt.Columns.Add("FileUrl")

            If IO.Directory.Exists(path) Then
                For Each file As String In IO.Directory.GetFiles(path)
                    Dim fileName = IO.Path.GetFileName(file)
                    Dim row = dt.NewRow()

                    row("FolderName") = lblOrderId.Text
                    row("FileName") = fileName
                    row("FileUrl") = ResolveUrl("~/File/Order/" & lblOrderId.Text & "/" & fileName)
                    dt.Rows.Add(row)
                Next
            End If

            gvListOrderFile.DataSource = dt
            gvListOrderFile.DataBind()

            divUploadAction.Visible = False
            If lblOrderType.Text = "Builder" AndAlso (lblOrderStatus.Text = "Unsubmitted" OrElse lblOrderStatus.Text = "Quoted") Then
                divUploadAction.Visible = True
            End If

        Catch ex As Exception
            MessageError_FileOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_FileOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                Dim dataMailing As Object() = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindBuilderFile", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDataRework(headerId As String)
        If Not String.IsNullOrEmpty(headerId) Then
            gvListItemRework.DataSource = orderClass.GetDataTable("SELECT OrderDetails.* FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN OrderReworks ON OrderReworks.HeaderId=OrderDetails.HeaderId AND OrderReworks.Status='Unsubmitted' AND OrderReworks.Active=1 LEFT JOIN OrderReworkDetails ON OrderReworkDetails.ItemId=OrderDetails.Id AND OrderReworkDetails.ReworkId=OrderReworks.Id AND OrderReworkDetails.Active=1 WHERE OrderDetails.HeaderId=" & headerId & " AND OrderDetails.Active=1 AND (Designs.Type='Blinds' OR Designs.Type='Shutters') AND OrderReworkDetails.ItemId IS NULL ORDER BY OrderDetails.Id ASC"
)
            gvListItemRework.DataBind()

            gvListItemRework.Columns(1).Visible = False ' ID
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then
                gvListItemRework.Columns(1).Visible = True ' ID
            End If
        End If
    End Sub

    Protected Sub BindEmailQuote()
        Try
            txtEmailQuoteTo.Text = orderClass.GetCustomerPrimaryEmail(lblCustomerId.Text)

            Dim dataEmailCustomer As DataTable = orderClass.GetDataTable("SELECT Email FROM CustomerContacts CROSS APPLY STRING_SPLIT(Tags, ',') AS thisArray WHERE CustomerId='" & lblCustomerId.Text & "' AND LTRIM(RTRIM(Email)) <> '' AND Email IS NOT NULL AND thisArray.VALUE='Quoting' AND [Primary]=0")
            If dataEmailCustomer.Rows.Count > 0 Then
                Dim listEmail As New List(Of String)

                For Each row As DataRow In dataEmailCustomer.Rows
                    listEmail.Add(row("Email").ToString())
                Next

                txtEmailQuoteCCCustomer.Text = String.Join(vbCrLf, listEmail)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindEmailInvoice()
        Try
            txtSendInvoiceTo.Text = orderClass.GetCustomerPrimaryEmail(lblCustomerId.Text)

            Dim dataEmailCustomer As DataTable = orderClass.GetDataTable("SELECT Email FROM CustomerContacts CROSS APPLY STRING_SPLIT(Tags, ',') AS thisArray WHERE CustomerId='" & lblCustomerId.Text & "' AND LTRIM(RTRIM(Email)) <> '' AND Email IS NOT NULL AND thisArray.VALUE='Invoicing' AND [Primary]=0")
            If dataEmailCustomer.Rows.Count > 0 Then
                Dim listEmail As New List(Of String)

                For Each row As DataRow In dataEmailCustomer.Rows
                    listEmail.Add(row("Email").ToString())
                Next

                txtSendInvoiceCCCustomer.Text = String.Join(vbCrLf, listEmail)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function BindProductDescription(itemId As Integer) As String
        Dim result As String = String.Empty

        Dim param As New List(Of SqlParameter) From {
            New SqlParameter("@ItemId", Convert.ToInt32(itemId))
        }

        Dim thisData As DataRow = orderClass.GetDataRowSP("sp_GetOrderDetailForDescription", param)
        If thisData Is Nothing Then
            Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
        End If

        Dim productId As String = thisData("ProductId").ToString()
        Dim productName As String = thisData("ProductName").ToString()
        Dim designName As String = thisData("DesignName").ToString()
        Dim blindName As String = thisData("BlindName").ToString()

        Dim subType As String = thisData("SubType").ToString()
        Dim heading As String = thisData("Heading").ToString()
        Dim headingB As String = thisData("HeadingB").ToString()

        Dim trackType As String = thisData("TrackType").ToString()
        Dim trackTypeB As String = thisData("TrackTypeB").ToString()

        Dim fabricColourId As String = thisData("FabricColourId").ToString()
        Dim fabricColourIdB As String = thisData("FabricColourIdB").ToString()
        Dim fabricColourIdC As String = thisData("FabricColourIdC").ToString()
        Dim fabricColourIdD As String = thisData("FabricColourIdD").ToString()
        Dim fabricColourIdE As String = thisData("FabricColourIdE").ToString()
        Dim fabricColourIdF As String = thisData("FabricColourIdF").ToString()

        Dim width As String = thisData("Width").ToString()
        Dim widthB As String = thisData("WidthB").ToString()
        Dim widthC As String = thisData("WidthC").ToString()
        Dim widthD As String = thisData("WidthD").ToString()
        Dim widthE As String = thisData("WidthE").ToString()
        Dim widthF As String = thisData("WidthF").ToString()

        Dim drop As String = thisData("Drop").ToString()
        Dim dropB As String = thisData("DropB").ToString()
        Dim dropC As String = thisData("DropC").ToString()
        Dim dropD As String = thisData("DropD").ToString()
        Dim dropE As String = thisData("DropE").ToString()
        Dim dropF As String = thisData("DropF").ToString()

        Dim printing As String = thisData("Printing").ToString()
        Dim printingB As String = thisData("PrintingB").ToString()
        Dim printingC As String = thisData("PrintingC").ToString()
        Dim printingD As String = thisData("PrintingD").ToString()
        Dim printingE As String = thisData("PrintingE").ToString()
        Dim printingF As String = thisData("PrintingF").ToString()

        Dim layoutCode As String = thisData("LayoutCode").ToString()
        Dim frameColour As String = thisData("FrameColour").ToString()

        Dim size As String = String.Format("({0}x{1})", width, drop)
        Dim sizeB As String = String.Format("({0}x{1})", widthB, dropB)
        Dim sizeC As String = String.Format("({0}x{1})", widthC, dropC)
        Dim sizeD As String = String.Format("({0}x{1})", widthD, dropD)
        Dim sizeE As String = String.Format("({0}x{1})", widthE, dropE)
        Dim sizeF As String = String.Format("({0}x{1})", widthF, dropF)

        Dim fabricColourName As String = orderClass.GetFabricColourName(fabricColourId)
        Dim fabricColourNameB As String = orderClass.GetFabricColourName(fabricColourIdB)
        Dim fabricColourNameC As String = orderClass.GetFabricColourName(fabricColourIdC)
        Dim fabricColourNameD As String = orderClass.GetFabricColourName(fabricColourIdD)
        Dim fabricColourNameE As String = orderClass.GetFabricColourName(fabricColourIdE)
        Dim fabricColourNameF As String = orderClass.GetFabricColourName(fabricColourIdF)

        Dim squareMetre As Decimal = 0D
        Dim squareMetreB As Decimal = 0D
        Dim squareMetreC As Decimal = 0D
        Dim squareMetreD As Decimal = 0D
        Dim squareMetreE As Decimal = 0D
        Dim squareMetreF As Decimal = 0D

        If Not IsDBNull(thisData("SquareMetre")) Then
            squareMetre = Math.Round(Convert.ToDecimal(thisData("SquareMetre")), 2)
        End If
        If Not IsDBNull(thisData("SquareMetreB")) Then
            squareMetreB = Math.Round(Convert.ToDecimal(thisData("SquareMetreB")), 2)
        End If
        If Not IsDBNull(thisData("SquareMetreC")) Then
            squareMetreC = Math.Round(Convert.ToDecimal(thisData("SquareMetreC")), 2)
        End If
        If Not IsDBNull(thisData("SquareMetreD")) Then
            squareMetreD = Math.Round(Convert.ToDecimal(thisData("SquareMetreD")), 2)
        End If
        If Not IsDBNull(thisData("SquareMetreE")) Then
            squareMetreE = Math.Round(Convert.ToDecimal(thisData("SquareMetreE")), 2)
        End If
        If Not IsDBNull(thisData("SquareMetreF")) Then
            squareMetreF = Math.Round(Convert.ToDecimal(thisData("SquareMetreF")), 2)
        End If

        Dim squareMetreText As String = String.Format("{0}sqm", squareMetre.ToString("0.##", enUS))
        Dim squareMetreTextB As String = String.Format("{0}sqm", squareMetreB.ToString("0.##", enUS))
        Dim squareMetreTextC As String = String.Format("{0}sqm", squareMetreC.ToString("0.##", enUS))
        Dim squareMetreTextD As String = String.Format("{0}sqm", squareMetreD.ToString("0.##", enUS))
        Dim squareMetreTextE As String = String.Format("{0}sqm", squareMetreE.ToString("0.##", enUS))
        Dim squareMetreTextF As String = String.Format("{0}sqm", squareMetreF.ToString("0.##", enUS))

        Dim linearMetre As Decimal = 0D
        Dim linearMetreB As Decimal = 0D
        Dim linearMetreC As Decimal = 0D

        If Not IsDBNull(thisData("LinearMetre")) Then
            linearMetre = Math.Round(Convert.ToDecimal(thisData("LinearMetre")), 2)
        End If
        If Not IsDBNull(thisData("LinearMetreB")) Then
            linearMetreB = Math.Round(Convert.ToDecimal(thisData("LinearMetreB")), 2)
        End If
        If Not IsDBNull(thisData("LinearMetreC")) Then
            linearMetreC = Math.Round(Convert.ToDecimal(thisData("LinearMetreC")), 2)
        End If

        Dim linearMetreText As String = String.Format("{0}lm", linearMetre.ToString("0.##", enUS))
        Dim linearMetreTextB As String = String.Format("{0}lm", linearMetreB.ToString("0.##", enUS))
        Dim linearMetreTextC As String = String.Format("{0}lm", linearMetreC.ToString("0.##", enUS))

        Dim room As String = thisData("Room").ToString()
        Dim itemDescription As String = String.Format("<b>{0}</b>, {1}", room, productName)

        If designName = "Aluminium Blind" Then
            result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
            If subType.Contains("2 on 1") Then
                result = String.Format("<b>{0}</b>, 2 on 1 Headrail", room)
                result &= "<br />"
                result &= String.Format("{0} {1} {2}", productName, size, squareMetreText)
                result &= "<br />"
                result &= String.Format("{0} {1} {2}", productName, sizeB, squareMetreTextB)
            End If
        End If

        If designName = "Cellular Shades" Then
            result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            If blindName = "Day & Night" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("{0} {1} {2}", fabricColourName, size, squareMetreText)
                result &= "<br />"
                result &= String.Format("{0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
            End If
        End If

        If designName = "Curtain" Then
            result = itemDescription
            If blindName = "Curtain Only" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            End If
            If blindName = "Fabric Only" Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            End If
            If blindName = "Track Only" Then
                result = String.Format("{0} {1} ({2}) {3}", itemDescription, trackType, width, linearMetreText)
            End If
            If blindName = "Single Curtain & Track" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("{0} {1} {2} | {3} ({4}) {5}", fabricColourName, size, squareMetreText, trackType, width, linearMetreText)
            End If
            If blindName = "Double Curtain & Track" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("1st Curtain : {0} {1} {2} | {3} ({4}) {5}", fabricColourName, size, squareMetreText, trackType, width, linearMetreText)
                result &= "<br />"
                result &= String.Format("2nd Curtain : {0} {1} {2} | {3} ({4}) {5}", fabricColourNameB, sizeB, squareMetreTextB, trackTypeB, widthB, linearMetreTextB)
            End If
        End If

        If designName = "Design Shades" Then
            result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
        End If

        If designName = "Linea Valance" Then
            result = String.Format("{0} ({1}mm) {2}", itemDescription, width, linearMetreText)
        End If

        If designName = "Panel Glide" Then
            result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            If blindName = "Track Only" Then
                result = String.Format("{0} ({1}mm) {2}", itemDescription, width, linearMetreText)
            End If
        End If

        If designName = "Pelmet" Then
            result = String.Format("{0} {1} ({2}mm) {3}", itemDescription, fabricColourName, width, linearMetreText)
            If layoutCode = "B" OrElse layoutCode = "C" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("{0} ({1}mm) {2}", fabricColourName, width, linearMetreText)
                result &= "<br />"
                result &= String.Format("{0} ({1}mm) {2}", fabricColourName, widthB, linearMetreTextB)
            End If

            If layoutCode = "D" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("{0} ({1}mm) {2}", fabricColourName, width, linearMetreText)
                result &= "<br />"
                result &= String.Format("{0} ({1}mm) {2}", fabricColourName, widthB, linearMetreTextB)
                result &= "<br />"
                result &= String.Format("{0} ({1}mm) {2}", fabricColourName, widthC, linearMetreTextC)
            End If
        End If

        If designName = "Privacy Venetian" Then
            result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
        End If

        If designName = "Roller Blind" Then
            result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            If Not String.IsNullOrEmpty(printing) Then
                result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
                result &= "<br />"
                result &= "<b><u>Printed Fabric</b></u>"
            End If
            If blindName = "Dual Blinds" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("First Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                If Not String.IsNullOrEmpty(printingB) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
            End If
            If blindName = "Link 2 Blinds Dependent" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("First & Control Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                If Not String.IsNullOrEmpty(printingB) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
            End If
            If blindName = "Link 2 Blinds Independent" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("Left Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Right Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                If Not String.IsNullOrEmpty(printingB) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
            End If
            If blindName = "Link 3 Blinds Dependent" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("Control Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                If Not String.IsNullOrEmpty(printingB) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Third Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                If Not String.IsNullOrEmpty(printingC) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
            End If

            If blindName = "Link 3 Blinds Independent with Dependent" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("Independent Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                If Not String.IsNullOrEmpty(printingB) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Control Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                If Not String.IsNullOrEmpty(printingC) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
            End If

            If blindName = "Link 4 Blinds Independent with Dependent" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("Left Control Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Middle Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                If Not String.IsNullOrEmpty(printingB) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Middle Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                If Not String.IsNullOrEmpty(printingC) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Right Control Blind : {0} {1} {2}", fabricColourNameD, sizeD, squareMetreTextD)
                If Not String.IsNullOrEmpty(printingD) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
            End If

            If blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 2 Blinds Independent" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("First Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                If Not String.IsNullOrEmpty(printingB) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Third Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                If Not String.IsNullOrEmpty(printingC) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Fourth Blind : {0} {1} {2}", fabricColourNameD, sizeD, squareMetreTextD)
                If Not String.IsNullOrEmpty(printingD) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
            End If

            If blindName = "DB Link 3 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then
                result = itemDescription
                result &= "<br />"
                result &= String.Format("First Blind : {0} {1} {2}", fabricColourName, size, squareMetreText)
                If Not String.IsNullOrEmpty(printing) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Second Blind : {0} {1} {2}", fabricColourNameB, sizeB, squareMetreTextB)
                If Not String.IsNullOrEmpty(printingB) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Third Blind : {0} {1} {2}", fabricColourNameC, sizeC, squareMetreTextC)
                If Not String.IsNullOrEmpty(printingC) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Fourth Blind : {0} {1} {2}", fabricColourNameD, sizeD, squareMetreTextD)
                If Not String.IsNullOrEmpty(printingD) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Fifth Blind : {0} {1} {2}", fabricColourNameE, sizeE, squareMetreTextE)
                If Not String.IsNullOrEmpty(printingE) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
                result &= "<br />"
                result &= String.Format("Sixth Blind : {0} {1} {2}", fabricColourNameF, sizeF, squareMetreTextF)
                If Not String.IsNullOrEmpty(printingF) Then
                    result &= " (<b><u>Printed Fabric</b></u>)"
                End If
            End If
        End If

        If designName = "Roman Blind" Then
            result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            If Not String.IsNullOrEmpty(printing) Then
                result &= "<br />"
                result &= "<b><u>Printed Fabric</b></u>"
            End If
        End If

        If designName = "Sample" Then
            result = productName
        End If

        If designName = "Skyline Shutter Express" Then
            result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
        End If

        If designName = "Skyline Shutter Ocean" Then
            result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
        End If

        If designName = "Evolve Shutter Ocean" Then
            result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
        End If

        If designName = "Venetian Blind" Then
            result = String.Format("{0} {1} {2}", itemDescription, size, squareMetreText)
            If subType.Contains("2 on 1") Then
                result = String.Format("<b>{0}</b>, 2 on 1 Headrail", room)
                result &= "<br />"
                result &= String.Format("{0} {1} {2}", productName, size, squareMetreText)
                result &= "<br />"
                result &= String.Format("{0} {1} {2}", productName, sizeB, squareMetreTextB)
            End If
            If subType.Contains("3 on 1") Then
                result = String.Format("<b>{0}</b>, 3 on 1 Headrail", room)
                result &= "<br />"
                result &= String.Format("{0} {1} {2}", productName, size, squareMetreText)
                result &= "<br />"
                result &= String.Format("{0} {1} {2}", productName, sizeB, squareMetreTextB)
                result &= "<br />"
                result &= String.Format("{0} {1} {2}", productName, sizeC, squareMetreTextC)
            End If
        End If

        If designName = "Vertical" Then
            result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
            If blindName = "Track Only" Then
                result = String.Format("{0} ({1}mm) {2}", itemDescription, width, linearMetreText)
            End If
        End If

        If designName = "Saphora Drape" Then
            result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
        End If

        If designName = "Window" OrElse designName = "Door" Then
            result = String.Format("{0} - {1} {2} {3}", itemDescription, frameColour, size, squareMetreText)
        End If

        If designName = "Outdoor" Then
            result = String.Format("{0} {1} {2} {3}", itemDescription, fabricColourName, size, squareMetreText)
        End If

        If designName = "Additional" Then
            result = productName
        End If
        Return result
    End Function

    Protected Function ItemCosting(itemId As String, type As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(itemId) Then
                Dim thisQuery As String = String.Format("SELECT {0} FROM OrderCostings WHERE ItemId='{1}' AND Type='Final'", type, itemId)
                Dim thisPrice As Decimal = orderClass.GetItemData_Decimal(thisQuery)

                result = String.Format("${0}", thisPrice.ToString("N2", enUS))
                If lblCompanyId.Text = "3" OrElse lblCompanyId.Text = "5" Then
                    result = String.Format("Rp{0}", thisPrice.ToString("N2", idIDR))
                End If
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Protected Function BindMarkUp(markUp As Decimal) As String
        Dim result As String = String.Empty
        If markUp > 0 Then : result = markUp & "%" : End If
        Return result
    End Function

    Protected Function InsertContext(queryString As String) As String
        Try
            Dim thisId As String = String.Empty
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("sp_InsertOrderActionContext", thisConn)
                    myCmd.CommandType = CommandType.StoredProcedure
                    myCmd.Parameters.Add("@Query", SqlDbType.NVarChar).Value = queryString

                    thisConn.Open()
                    Return myCmd.ExecuteScalar().ToString()
                End Using
            End Using
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Protected Sub AllMessageError(visible As Boolean, message As String)
        MessageError(visible, message)
        MessageError_Preview(visible, message)

        MessageError_BuilderDetail(visible, message)
        MessageError_FileOrder(visible, message)

        MessageError_DetailQuote(visible, message)

        MessageError_AddNote(visible, message)

        MessageError_SendInvoice(visible, message)
        MessageError_InvoiceNumber(visible, message)
        MessageError_InvoiceData(visible, message)

        MessageError_CancelOrder(visible, message)
        MessageError_ShippedOrder(visible, message)
        MessageError_ReworkOrder(visible, message)
        MessageError_MoreEmailQuote(visible, message)

        MessageError_Service(visible, message)
        MessageError_EditCosting(visible, message)
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
        divErrorB.Visible = visible : msgErrorB.InnerText = message
    End Sub

    Protected Sub MessageError_BuilderDetail(visible As Boolean, message As String)
        divErrorBuilderDetail.Visible = visible : msgErrorBuilderDetail.InnerText = message
    End Sub

    Protected Sub MessageError_FileOrder(visible As Boolean, message As String)
        divErrorFileOrder.Visible = visible : msgErrorFileOrder.InnerText = message
    End Sub

    Protected Sub MessageError_Preview(visible As Boolean, message As String)
        divErrorPreview.Visible = visible : msgErrorPreview.InnerText = message
    End Sub

    Protected Sub MessageError_DetailQuote(visible As Boolean, message As String)
        divErrorDetailQuote.Visible = visible : msgErrorDetailQuote.InnerText = message
    End Sub

    Protected Sub MessageError_AddNote(visible As Boolean, message As String)
        divErrorAddNote.Visible = visible : msgErrorAddNote.InnerText = message
    End Sub

    Protected Sub MessageError_CancelOrder(visible As Boolean, message As String)
        divErrorCancelOrder.Visible = visible : msgErrorCancelOrder.InnerText = message
    End Sub

    Protected Sub MessageError_MoreEmailQuote(visible As Boolean, message As String)
        divErrorMoreEmailQuote.Visible = visible : msgErrorMoreEmailQuote.InnerText = message
    End Sub

    Protected Sub MessageError_SendInvoice(visible As Boolean, message As String)
        divErrorSendInvoice.Visible = visible : msgErrorSendInvoice.InnerText = message
    End Sub

    Protected Sub MessageError_InvoiceNumber(visible As Boolean, message As String)
        divErrorInvoiceNumber.Visible = visible : msgErrorInvoiceNumber.InnerText = message
    End Sub

    Protected Sub MessageError_InvoiceData(visible As Boolean, message As String)
        divErrorInvoiceData.Visible = visible : msgErrorInvoiceData.InnerText = message
    End Sub

    Protected Sub MessageError_ShippedOrder(visible As Boolean, message As String)
        divErrorShippedOrder.Visible = visible : msgErrorShippedOrder.InnerText = message
    End Sub

    Protected Sub MessageError_ReworkOrder(visible As Boolean, message As String)
        divErrorReworkOrder.Visible = visible : msgErrorReworkOrder.InnerText = message
    End Sub

    Protected Sub MessageError_EditCosting(visible As Boolean, message As String)
        divErrorEditCosting.Visible = visible : msgErrorEditCosting.InnerText = message
    End Sub

    Protected Sub MessageError_Service(visible As Boolean, message As String)
        divErrorService.Visible = visible : msgErrorService.InnerText = message
    End Sub

    Protected Function VisibleCopy(productId As String) As Boolean
        Dim result As Boolean = False
        Try
            If Not String.IsNullOrEmpty(productId) Then
                If Session("RoleName") = "Developer" Then
                    result = True
                End If
                If Session("RoleName") = "IT" Then
                    If lblOrderStatus.Text = "Unsubmitted" Then result = True
                    If lblOrderStatus.Text = "Quoted" Then result = True
                    If lblOrderStatus.Text = "New Order" AndAlso lblOrderPaid.Text = "" Then result = True
                    If lblOrderStatus.Text = "In Production" AndAlso lblOrderPaid.Text = "" Then result = True
                    If lblOrderStatus.Text = "On Hold" AndAlso lblOrderPaid.Text = "" Then result = True
                    If lblOrderStatus.Text = "Waiting Proforma" AndAlso lblOrderPaid.Text = "" Then result = True
                    If lblOrderStatus.Text = "Proforma Sent" AndAlso lblOrderPaid.Text = "" Then result = True
                    If lblOrderStatus.Text = "Payment Received" AndAlso lblOrderPaid.Text = "" Then result = True
                End If

                If Session("RoleName") = "Factory Office" Then
                    If lblOrderStatus.Text = "Unsubmitted" Then result = True
                    If lblOrderStatus.Text = "New Order" AndAlso lblOrderPaid.Text = "" Then result = True
                    If lblOrderStatus.Text = "Waiting Proforma" AndAlso lblOrderPaid.Text = "" Then result = True
                End If

                If Session("RoleName") = "Sales" Then
                    If lblOrderStatus.Text = "Unsubmitted" Then result = True
                    If lblOrderStatus.Text = "Quoted" Then result = True
                    If lblOrderStatus.Text = "Waiting Proforma" Then result = True
                End If

                If Session("RoleName") = "Customer Service" Then
                    If lblOrderStatus.Text = "Unsubmitted" Then result = True
                    If lblOrderStatus.Text = "New Order" Then result = True
                    If lblOrderStatus.Text = "Quoted" Then result = True
                    If lblOrderStatus.Text = "Waiting Proforma" Then result = True
                End If

                If Session("RoleName") = "Data Entry" AndAlso lblCreatedRole.Text = Session("RoleId") Then
                    If lblOrderStatus.Text = "Unsubmitted" Then result = True
                    If lblOrderStatus.Text = "Quoted" Then result = True
                    If lblOrderStatus.Text = "Waiting Proforma" Then result = True
                End If

                If Session("RoleName") = "Customer" AndAlso lblOrderStatus.Text = "Unsubmitted" Then result = True

                Dim designId As String = orderClass.GetItemData("SELECT DesignId FROM Products WHERE Id='" & productId & "'")

                If designId = "16" Then result = False
            End If
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Protected Function VisibleDelete(productId As String) As Boolean
        Dim result As Boolean = False
        Try
            Dim designId As String = orderClass.GetItemData("SELECT DesignId FROM Products WHERE Id='" & productId & "'")

            If Session("RoleName") = "Developer" Then Return True

            If Session("RoleName") = "IT" Then
                If lblOrderStatus.Text = "Unsubmitted" Then result = True
                If lblOrderStatus.Text = "Quoted" Then result = True
                If lblOrderStatus.Text = "New Order" Then result = True
                If lblOrderStatus.Text = "In Production" Then result = True
                If lblOrderStatus.Text = "On Hold" Then result = True
                If lblOrderStatus.Text = "Waiting Proforma" Then result = True
                If lblOrderStatus.Text = "Proforma Sent" Then result = True
                If lblOrderStatus.Text = "Payment Received" Then result = True
            End If

            If Session("RoleName") = "Factory Office" Then
                If lblOrderStatus.Text = "Unsubmitted" Then result = True
                If lblOrderStatus.Text = "New Order" Then result = True
                If lblOrderStatus.Text = "Waiting Proforma" Then result = True
            End If

            If Session("RoleName") = "Sales" Then
                If lblOrderStatus.Text = "Unsubmitted" Then result = True
                If lblOrderStatus.Text = "Quoted" Then result = True
                If lblOrderStatus.Text = "Waiting Proforma" Then result = True
            End If

            If Session("RoleName") = "Account" Then
                If lblOrderStatus.Text = "Unsubmitted" Then result = True
                If lblOrderStatus.Text = "Quoted" Then result = True
                If lblOrderStatus.Text = "Waiting Proforma" Then result = True
            End If

            If Session("RoleName") = "Customer Service" Then
                If lblOrderStatus.Text = "Unsubmitted" Then result = True
                If lblOrderStatus.Text = "Quoted" Then result = True
                If lblOrderStatus.Text = "Waiting Proforma" Then result = True
            End If

            If Session("RoleName") = "Data Entry" AndAlso lblCreatedRole.Text = Session("RoleId") Then
                If lblOrderStatus.Text = "Unsubmitted" Then result = True
                If lblOrderStatus.Text = "Quoted" Then result = True
                If lblOrderStatus.Text = "Waiting Proforma" Then result = True

                If designId = "16" Then result = False
            End If

            If Session("RoleName") = "Customer" AndAlso lblOrderStatus.Text = "Unsubmitted" Then
                result = True
                If designId = "16" Then result = False
            End If
        Catch ex As Exception
            result = False
        End Try

        Return result
    End Function

    Protected Function VisiblePrinting(itemId As String) As Boolean
        Try
            If Session("RoleName") = "Installer" Then Return False

            Dim thisData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Designs.Name AS DesignName, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.Id='" & itemId & "'")

            Dim designname As String = thisData("DesignName").ToString()
            Dim blindname As String = thisData("BlindName").ToString()
            Dim width As Integer = thisData("Width")
            Dim drop As Integer = thisData("Drop")

            If designname = "Roller Blind" Then
                If blindname = "Single Blind" OrElse blindname = "Dual Blinds" Then
                    If width <= 1510 OrElse drop <= 1510 Then Return True
                End If
                If blindname = "Link 2 Blinds Dependent" OrElse blindname = "Link 2 Blinds Independent" Then
                    Dim widthb As Integer = thisData("WidthB")
                    If width <= 1510 OrElse widthb <= 1510 Then Return True
                End If
                If blindname = "Link 3 Blinds Dependent" OrElse blindname = "Link 3 Blinds Independent with Dependent" Then
                    Dim widthb As Integer = thisData("WidthB")
                    Dim widthc As Integer = thisData("WidthC")

                    If width <= 1510 OrElse widthb <= 1510 OrElse widthc <= 1510 Then Return True
                End If
                If blindname = "Link 4 Blinds Independent with Dependent" OrElse blindname = "DB Link 2 Blinds Dependent" OrElse blindname = "DB Link 2 Blinds Independent" Then
                    Dim widthb As Integer = thisData("WidthB")
                    Dim widthc As Integer = thisData("WidthC")
                    Dim widthd As Integer = thisData("WidthD")

                    If width <= 1510 OrElse widthb <= 1510 OrElse widthc <= 1510 OrElse widthd <= 1510 Then Return True
                End If
            End If

            If designname = "Roman Blind" Then
                If width <= 1510 OrElse drop <= 1510 Then Return True
            End If
        Catch ex As Exception
            Return False
        End Try
        Return False
    End Function

    Protected Function VisibleCosting() As Boolean
        Dim result As Boolean = False

        If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Sales" OrElse Session("RoleName") = "Customer Service" OrElse Session("RoleName") = "Account" OrElse Session("RoleName") = "Customer" Then result = True

        Return result
    End Function

    Protected Function VisibleEditPrice() As Boolean
        Dim result As Boolean = False

        If Session("RoleName") = "Developer" Then result = True

        If Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Account" Then
            If lblOrderPaid.Text = "" Then result = True
        End If

        If Session("RoleName") = "Sales" Then
            If lblOrderStatus.Text = "Unsubmitted" Then result = True
            If lblOrderStatus.Text = "Quoted" Then result = True
            If lblOrderStatus.Text = "New Order" AndAlso lblOrderPaid.Text = "" Then result = True
            If lblOrderStatus.Text = "Waiting Proforma" Then result = True
        End If

        If Session("RoleName") = "Customer Service" Then
            If lblOrderStatus.Text = "Quoted" Then result = True
        End If

        Return result
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
