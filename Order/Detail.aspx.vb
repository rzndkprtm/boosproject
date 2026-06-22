Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Threading.Tasks

Partial Class Order_Detail
    Inherits Page

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty
    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim idIDR As New CultureInfo("id-ID")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
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

            btnPreview.OnClientClick = "window.open('view?action=jobsheet&boosid=" & lblHeaderId.Text & "','_blank'); return false;"
            btnSuratJalan.OnClientClick = "window.open('view?action=suratjalan&boosid=" & lblHeaderId.Text & "','_blank'); return false;"
            btnPreviewInvoice.OnClientClick = "window.open('view?action=invoice&boosid=" & lblHeaderId.Text & "','_blank'); return false;"

            Dim quoteString As String = "window.open('view?action=quote&boosid=" & lblHeaderId.Text & "','_blank'); return false;"
            If lblOrderType.Text = "Builder" Then
                quoteString = "window.open('view?action=quotebuilder&boosid=" & lblHeaderId.Text & "','_blank'); return false;"
            End If
            If Session("RoleName") = "Customer" Then
                quoteString = "window.open('view?action=quotes&boosid=" & lblHeaderId.Text & "','_blank'); return false;"
            End If
            btnPreviewQuote.OnClientClick = quoteString
        End If
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
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showFileOrder", thisScript, True)
        End Try
    End Sub

    Protected Sub btnRecalculate_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            orderClass.CalculatePriceByOrder(lblHeaderId.Text)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
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
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showAddNote", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDownloadBOE_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Download='Yes', DownloadDate=NULL WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDateOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET CreatedDate=@CreatedDate, SubmittedDate=@SubmittedDate, ProductionDate=@ProductionDate, OnHoldDate=@OnHoldDate, CanceledDate=@CanceledDate, CompletedDate=@CompletedDate WHERE Id=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@CreatedDate", If(String.IsNullOrEmpty(txtCreatedDate.Text), CType(DBNull.Value, Object), txtCreatedDate.Text))
                    myCmd.Parameters.AddWithValue("@SubmittedDate", If(String.IsNullOrEmpty(txtSubmittedDate.Text), CType(DBNull.Value, Object), txtSubmittedDate.Text))
                    myCmd.Parameters.AddWithValue("@ProductionDate", If(String.IsNullOrEmpty(txtProductionDate.Text), CType(DBNull.Value, Object), txtProductionDate.Text))
                    myCmd.Parameters.AddWithValue("@OnHoldDate", If(String.IsNullOrEmpty(txtHoldDate.Text), CType(DBNull.Value, Object), txtHoldDate.Text))
                    myCmd.Parameters.AddWithValue("@CanceledDate", If(String.IsNullOrEmpty(txtCanceledDate.Text), CType(DBNull.Value, Object), txtCanceledDate.Text))
                    myCmd.Parameters.AddWithValue("@CompletedDate", If(String.IsNullOrEmpty(txtCompletedDate.Text), CType(DBNull.Value, Object), txtCompletedDate.Text))

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim salesClass As New SalesClass
            salesClass.RefreshData(lblCompanyId.Text)

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Date Order Updated"}
            orderClass.Logs(dataLog)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDownload_Click(sender As Object, e As EventArgs)
        Try
            If gvListItem.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM ORDER !")
                Exit Sub
            End If

            Dim previewClass As New PreviewClass
            Dim pdfBytes As Byte() = previewClass.BindContent(lblHeaderId.Text)

            Dim fileName As String = String.Format("ORDER {0} {1}.pdf", lblOrderId.Text, lblCustomerName.Text.ToUpper())

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & "")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
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

    Protected Sub btnEditOrder_Click(sender As Object, e As EventArgs)
        url = String.Format("~/order/edit?orderid={0}&returnpage=detail", lblHeaderId.Text)
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
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Active=0, Download='No' WHERE Id=@Id; UPDATE OrderDetails SET Active=0 WHERE HeaderId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
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
            End If
        End Try
    End Sub

    Protected Sub btnDuplicateOrder_Click(sender As Object, e As EventArgs)
        MessageError_DuplicateOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showDuplicateOrder(); };"
        Try
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
            If txtOrderNumberNew.Text = orderClass.IsOrderExist(lblCustomerId.Text, txtOrderNumberNew.Text.Trim()) Then
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
                Dim companyAlias As String = orderClass.GetCompanyAliasByCustomer(lblCustomerId.Text)

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
                                myCmd.Parameters.AddWithValue("@OldId", lblHeaderId.Text)
                                myCmd.Parameters.AddWithValue("@NewID", newIdHeader)
                                myCmd.Parameters.AddWithValue("@OrderId", orderId)
                                myCmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumberNew.Text.Trim())
                                myCmd.Parameters.AddWithValue("@OrderName", txtOrderNameNew.Text.Trim())
                                myCmd.Parameters.AddWithValue("@OrderNote", txtOrderNoteNew.Text)
                                myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())

                                myCmd.ExecuteNonQuery()
                            End Using

                            If lblOrderType.Text = "Builder" Then
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

                Dim thisHeader As DataTable = orderClass.GetDataTable("SELECT * FROM OrderDetails WHERE HeaderId='" & lblHeaderId.Text & "' AND Active=1")
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
                If Not IO.Directory.Exists(directoryOrder) Then
                    IO.Directory.CreateDirectory(directoryOrder)
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

        'MessageError(False, String.Empty)
        'Try
        '    Dim newIdHeader As String = orderClass.GetNewOrderHeaderId()
        '    Dim customerId As String = orderClass.GetCustomerIdByOrder(lblHeaderId.Text)
        '    Dim companyAlias As String = orderClass.GetCompanyAliasByCustomer(customerId)

        '    Dim orderType As String = orderClass.GetItemData("SELECT OrderType FROM OrderHeaders WHERE Id='" & lblHeaderId.Text & "'")

        '    Dim success As Boolean = False
        '    Dim retry As Integer = 0
        '    Dim maxRetry As Integer = 100
        '    Dim orderId As String = String.Empty

        '    Do While Not success
        '        retry += 1
        '        If retry > maxRetry Then
        '            Throw New Exception("FAILED TO GENERATE UNIQUE ORDER ID")
        '        End If

        '        Dim randomCode As String = orderClass.GenerateRandomCode()
        '        orderId = companyAlias & randomCode
        '        Try
        '            Using thisConn As New SqlConnection(myConn)
        '                thisConn.Open()

        '                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders SELECT @NewID, @OrderId, CustomerId, 'Copy ' + CAST(@NewID AS VARCHAR(20)) + ' - ' + OrderNumber, 'Copy ' + CAST(@NewID AS VARCHAR(20)) + ' - ' + OrderName, NULL, OrderType, OrderFactory, 'Unsubmitted', NULL, @CreatedBy, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, 0, 'No', NULL, 1 FROM OrderHeaders WHERE Id=@OldId; INSERT INTO OrderQuotes VALUES(@NewID, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
        '                    myCmd.Parameters.AddWithValue("@OldId", lblHeaderId.Text)
        '                    myCmd.Parameters.AddWithValue("@NewID", newIdHeader)
        '                    myCmd.Parameters.AddWithValue("@OrderId", orderId)
        '                    myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())

        '                    myCmd.ExecuteNonQuery()
        '                End Using

        '                If orderType = "Builder" Then
        '                    Using myCmd As New SqlCommand("INSERT INTO OrderBuilders(Id) VALUES (@Id)", thisConn)
        '                        myCmd.Parameters.AddWithValue("@Id", newIdHeader)

        '                        myCmd.ExecuteNonQuery()
        '                    End Using
        '                End If

        '                thisConn.Close()
        '            End Using

        '            success = True
        '        Catch exSql As SqlException
        '            If exSql.Number = 2601 OrElse exSql.Number = 2627 Then
        '                success = False
        '            Else
        '                Throw
        '            End If
        '        End Try
        '    Loop

        '    dataLog = {"OrderHeaders", newIdHeader, Session("LoginId").ToString(), "Order Created | Copy"}
        '    orderClass.Logs(dataLog)

        '    Dim thisHeader As DataTable = orderClass.GetDataTable("SELECT * FROM OrderDetails WHERE HeaderId='" & lblHeaderId.Text & "' AND Active=1")
        '    If thisHeader.Rows.Count > 0 Then
        '        For i As Integer = 0 To thisHeader.Rows.Count - 1
        '            Dim itemId As String = thisHeader.Rows(i).Item("Id").ToString()
        '            Dim newIdDetail As String = orderClass.GetNewOrderItemId()

        '            Using thisConn As New SqlConnection(myConn)
        '                Using myCmd As New SqlCommand("sp_CopyOrderDetails", thisConn)
        '                    myCmd.CommandType = CommandType.StoredProcedure

        '                    myCmd.Parameters.AddWithValue("@ItemIdOld", itemId)
        '                    myCmd.Parameters.AddWithValue("@NewId", newIdDetail)
        '                    myCmd.Parameters.AddWithValue("@HeaderId", newIdHeader)

        '                    thisConn.Open()
        '                    myCmd.ExecuteNonQuery()
        '                End Using
        '            End Using

        '            orderClass.ResetPriceDetail(newIdHeader, newIdDetail)
        '            orderClass.CalculatePrice(newIdHeader, newIdDetail)
        '            orderClass.FinalCostItem(newIdHeader, newIdDetail)

        '            dataLog = {"OrderDetails", newIdDetail, Session("LoginId").ToString(), "Order Item Added | Copy"}
        '            orderClass.Logs(dataLog)
        '        Next
        '    End If

        '    Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Order/{0}/", orderId))
        '    If Not IO.Directory.Exists(directoryOrder) Then
        '        IO.Directory.CreateDirectory(directoryOrder)
        '    End If

        '    url = String.Format("~/order/detail?orderid={0}", newIdHeader)
        '    Response.Redirect(url, False)
        'Catch ex As Exception
        '    MessageError(True, ex.ToString())
        '    If Not Session("RoleName") = "Developer" Then
        '        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
        '        If Session("RoleName") = "Customer" Then
        '            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
        '        End If
        '    End If
        'End Try
    End Sub

    Protected Sub btnQuoteOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If gvListItem.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM ORDER !")
                Exit Sub
            End If

            If lblCompanyDetailId.Text = "3" AndAlso lblOrderType.Text = "Builder" Then
                Dim thisData As DataRow = orderClass.GetDataRow("SELECT * FROM OrderBuilders WHERE Id='" & lblHeaderId.Text & "'")
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

            Dim checkProduct As DataTable = orderClass.GetDataTable("SELECT ROW_NUMBER() OVER (ORDER BY OrderDetails.Id ASC) AS [Number], OrderDetails.Id, Products.Status FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & lblHeaderId.Text & "' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

            If checkProduct.Rows.Count > 0 Then
                Dim sb As New StringBuilder()

                For i As Integer = 0 To checkProduct.Rows.Count - 1
                    Dim number As String = checkProduct.Rows(i)("Number").ToString()
                    Dim status As String = checkProduct.Rows(i)("Status").ToString()

                    If status = "" Then
                        Dim thisString As String = "- ITEM " & number & ". THIS PRODUCT IS CURRENTLY UNAVAILABLE. PLEASE CHECK AND CHANGE IT.<br />"
                    End If
                    If status = "Out of Stock" Then
                        Dim thisString As String = "- ITEM " & number & ". THIS PRODUCT IS CURRENTLY " & status.ToUpper() & ". PLEASE CHECK AND CHANGE IT.<br />"
                        sb.AppendLine()
                    End If
                    If status = "Discontinued" Then
                        Dim thisString As String = "- ITEM " & number & ". THIS PRODUCT IS HAS BEEN " & status.ToUpper() & ". PLEASE CHECK AND CHANGE IT.<br />"
                        sb.AppendLine()
                    End If
                Next

                If sb.Length > 0 Then
                    Dim thisMessage As String = sb.ToString()
                    MessageError(True, thisMessage)
                    Exit Sub
                Else
                    MessageError(False, String.Empty)
                End If
            End If

            Dim cashSale As Boolean = orderClass.GetCustomerCashSale(lblCustomerId.Text)
            Dim minSurcharge As Boolean = orderClass.GetCustomerMinimum(lblCustomerId.Text)

            Dim orderStatus As String = "New Order"
            If cashSale = True Then orderStatus = "Waiting Proforma"

            Dim invoiceNumber As String = lblOrderId.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET SubmittedDate=GETDATE(), Status=@Status, InvoiceNumber=@InvoiceNumber, Payment=0, Amount=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@Status", orderStatus)
                    myCmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber)
                    myCmd.Parameters.AddWithValue("@Payment", False)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            If lblCompanyId.Text = "2" AndAlso lblOrderType.Text = "Regular" Then
                Dim thisId As String = orderClass.GetNewOrderItemId()

                Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products WHERE Name='Fuel Surcharge' AND (Status='In Stock' OR Status='Limited Stock')")
                Dim productGroupId As String = orderClass.GetItemData("SELECT Id FROM PriceProductGroups WHERE Name='Fuel Surcharge' AND Active=1")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, Qty, Width, [Drop], TotalItems, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, 1, 0, 0, 1, 0, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@ProductId", If(String.IsNullOrEmpty(productId), CType(DBNull.Value, Object), productId))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(productGroupId), CType(DBNull.Value, Object), productGroupId))

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderDetails", thisId, "2", "Order Item Added"}
                orderClass.Logs(dataLog)

                orderClass.ResetPriceDetail(lblHeaderId.Text, thisId)
                orderClass.CalculatePrice(lblHeaderId.Text, thisId)
                orderClass.FinalCostItem(lblHeaderId.Text, thisId)

                Dim totalItems As Integer = orderClass.GetTotalItemOrder(lblHeaderId.Text)
                If minSurcharge = True AndAlso totalItems <= 3 Then
                    thisId = orderClass.GetNewOrderItemId()
                    productId = orderClass.GetItemData("SELECT Id FROM Products WHERE Name='Minimum Order Surcharge' AND (Status='In Stock' OR Status='Limited Stock')")
                    productGroupId = orderClass.GetItemData("SELECT Id FROM PriceProductGroups WHERE Name='Minimum Order Surcharge' AND Active=1")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, Qty, Width, [Drop], TotalItems, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, 1, 0, 0, 1, 0, 1)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)
                            myCmd.Parameters.AddWithValue("@ProductId", If(String.IsNullOrEmpty(productId), CType(DBNull.Value, Object), productId))
                            myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(productGroupId), CType(DBNull.Value, Object), productGroupId))

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
            End If

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Order Submitted"}
            orderClass.Logs(dataLog)

            If chkSendEmail.Checked = True Then
                Dim mailingClass As New MailingClass
                If lblCompanyId.Text = "2" Then
                    If cashSale = False Then mailingClass.NewOrder(lblHeaderId.Text)
                    If cashSale = True Then mailingClass.NewOrder_Proforma(lblHeaderId.Text)
                End If
                If lblCompanyId.Text = "3" Then mailingClass.NewOrder(lblHeaderId.Text)

                Dim checkPrinting As Integer = orderClass.GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails WHERE HeaderId='" & lblHeaderId.Text & "' AND (NULLIF(LTRIM(RTRIM(Printing)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingB)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingC)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingD)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingE)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingF)),'') IS NOT NULL)")
                If checkPrinting > 0 Then
                    mailingClass.NewOrder_PrintingFabric(lblHeaderId.Text)
                End If
            End If

            Dim checkOcean As Integer = orderClass.GetItemData_Integer("SELECT COUNT(OrderDetails.Id) FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & lblHeaderId.Text & "' AND OrderDetails.Active=1 AND Products.DesignId='15'")
            If cashSale = False AndAlso checkOcean > 0 Then
                Dim thisId As String = lblHeaderId.Text

                Task.Run(Async Function()
                             Dim svc As New ShutterOceanService()
                             Await svc.SendOrderAsync(thisId)
                         End Function)

            End If

            Dim berhasil As String = String.Format("showSuccessSwal('{0}')", lblHeaderId.Text)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "swalSuccess", berhasil, True)
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

    Protected Sub btnNewOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='New Order' WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim mailingClass As New MailingClass
            mailingClass.NewOrder(lblHeaderId.Text)

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "New Order"}
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
            End If
        End Try
    End Sub

    Protected Sub btnUnsubmitOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET SubmittedDate=NULL, ProductionDate=NULL, OnHoldDate=NULL, Status='Unsubmitted', Download='No', DownloadDate=NULL, ShipmentDate=NULL, ShipmentNumber=NULL, ContainerNumber=NULL, ContainerETA=NULL, Courier=NULL, InvoiceNumber=NULL, Collector=NULL, InvoiceDate=NULL, DueDate=NULL, Payment=0, PaymentDate=NULL, Amount=0 WHERE Id=@Id", thisConn)
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
            End If
        End Try
    End Sub

    Protected Sub btnProductionOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim stringQuery As String = "UPDATE OrderHeaders SET Status='In Production', OnHoldDate=NULL WHERE Id=@Id;"
            If lblOrderStatus.Text = "New Order" OrElse lblOrderStatus.Text = "Payment Received" Then
                stringQuery = "UPDATE OrderHeaders SET Status='In Production', ProductionDate=GETDATE(), OnHoldDate=NULL, Download='Yes' WHERE Id=@Id"
            End If
            If lblOrderStatus.Text = "Shipped Out" Then
                stringQuery = "UPDATE OrderHeaders SET Status='In Production', ShipmentNumber=NULL, ShipmentDate=NULL, ContainerNumber=NULL, ContainerETA=NULL, Courier=NULL WHERE Id=@Id"
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand(stringQuery, thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Order In Production"}
            orderClass.Logs(dataLog)

            Dim salesClass As New SalesClass
            salesClass.RefreshData(lblCompanyId.Text)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
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
            End If
        End Try
    End Sub

    Protected Sub btnShippedOrder_Click(sender As Object, e As EventArgs)
        MessageError_ShippedOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showShippedOrder(); };"
        Try
            If msgErrorShippedOrder.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Shipped Out', ShipmentNumber=@ShipmentNumber, ShipmentDate=@ShipmentDate, ContainerNumber=@ContainerNumber, ContainerETA=@ContainerETA, Courier=@Courier WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@ShipmentNumber", txtShipmentNumber.Text.Trim())
                        myCmd.Parameters.AddWithValue("@ShipmentDate", If(String.IsNullOrEmpty(txtShipmentDate.Text), CType(DBNull.Value, Object), txtShipmentDate.Text))
                        myCmd.Parameters.AddWithValue("@ContainerNumber", txtContainerNumber.Text.Trim())
                        myCmd.Parameters.AddWithValue("@ContainerETA", If(String.IsNullOrEmpty(txtContainerEta.Text), CType(DBNull.Value, Object), txtContainerEta.Text))
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
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Canceled', CanceledDate=GETDATE(), ShipmentNumber=NULL, ShipmentDate=NULL, ContainerNumber=NULL, ContainerETA=NULL, Courier=NULL, InvoiceNumber=NULL, Collector=NULL, InvoiceDate=NULL, DueDate=NULL, Payment=0, PaymentDate=NULL, Amount=0, Download='No' WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@StatusDescription", txtCancelDescription.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim salesClass As New SalesClass
                salesClass.RefreshData(lblCompanyId.Text)

                Dim descLog As String = String.Format("Order Canceled | {0}", txtCancelDescription.Text.Trim())

                dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), descLog}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_CancelOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_CancelOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showCancelOrder", thisScript, True)
        End Try
    End Sub

    Protected Sub btnReworkOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim reworkId As String = orderClass.GetNewOrderReworkId()

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderReworks VALUES (@Id, @HeaderId, NULL, 'Unsubmitted', @CreatedBy, GETDATE(), NULL, 1)", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", reworkId)
                    myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())
                    myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim directory As String = Server.MapPath(String.Format("~/File/Rework/{0}/", reworkId))
            If Not IO.Directory.Exists(directory) Then
                IO.Directory.CreateDirectory(directory)
            End If

            dataLog = {"OrderReworks", reworkId, Session("LoginId").ToString(), "Order Rework Created"}
            orderClass.Logs(dataLog)

            url = String.Format("~/order/rework/detail?reworkid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
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
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderQuotes SET Email=@Email, Phone=@Phone, Address=@Address, Suburb=@Suburb, City=@City, State=@State, PostCode=@PostCode, Discount=@Discount, Installation=@Installation, CheckMeasure=@CheckMeasure, Freight=@Freight WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@Email", txtQuoteEmail.Text)
                        myCmd.Parameters.AddWithValue("@Phone", txtQuotePhone.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Address", address)
                        myCmd.Parameters.AddWithValue("@Suburb", txtQuoteSuburb.Text.Trim())
                        myCmd.Parameters.AddWithValue("@City", txtQuoteCity.Text.Trim())
                        myCmd.Parameters.AddWithValue("@State", txtQuoteState.Text.Trim())
                        myCmd.Parameters.AddWithValue("@PostCode", txtQuotePostCode.Text.Trim())
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
            Dim pdfBytes As Byte() = quoteClass.BindContent(lblHeaderId.Text)
            If lblOrderType.Text = "Builder" Then
                pdfBytes = quoteClass.BindContentBuilder(lblHeaderId.Text)
            End If
            If Session("RoleName") = "Customer" Then
                pdfBytes = quoteClass.BindContentCustomer(lblHeaderId.Text)
            End If

            Dim fileName As String = String.Format("QUOTE-{0}-{1}", lblOrderNumber.Text, lblOrderName.Text)

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".pdf")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
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

    Protected Sub btnSendQuote_Click(sender As Object, e As EventArgs)
        MessageError_SendQuote(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showSendQuote(); };"
        Try
            If String.IsNullOrEmpty(txtSendQuoteTo.Text) Then
                MessageError_SendQuote(True, "CUSTOMER EMAIL TO IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showSendQuote", thisScript, True)
                Exit Sub
            End If

            Dim isValidEmail As Boolean = orderClass.IsValidEmail(txtSendQuoteTo.Text)
            If IsValid = False Then
                MessageError_SendQuote(True, "PLEASE CHECK YOUR CUSTOMER EMAIL TO !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showSendQuote", thisScript, True)
                Exit Sub
            End If

            Dim ccCustomer As String = String.Empty
            If Not String.IsNullOrEmpty(txtSendQuoteCCCustomer.Text) Then
                Dim raw As String = txtSendQuoteCCCustomer.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                ccCustomer = String.Join(";", cleanedEmails)
            End If

            Dim ccStaff As String = String.Empty
            If Not String.IsNullOrEmpty(txtSendQuoteCCStaff.Text) Then
                Dim raw As String = txtSendQuoteCCStaff.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                ccStaff = String.Join(";", cleanedEmails)
            End If

            If msgErrorSendQuote.InnerText = "" Then
                Dim mailingClass As New MailingClass
                mailingClass.SentQuote(lblHeaderId.Text, Session("LoginId").ToString(), txtSendQuoteTo.Text, ccCustomer, ccStaff)

                dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Send Quote"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_SendQuote(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_SendQuote(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showSendQuote", thisScript, True)
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

            Dim ccStaff As String = String.Empty
            If Not String.IsNullOrEmpty(txtSendInvoiceCCStaff.Text) Then
                Dim raw As String = txtSendInvoiceCCStaff.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                ccStaff = String.Join(";", cleanedEmails)
            End If

            If msgErrorSendInvoice.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    If lblOrderStatus.Text = "Waiting Proforma" Then
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Proforma Sent', Collector=@Collector, InvoiceDate=GETDATE(), DueDate=DATEADD(DAY, 14, GETDATE()) WHERE Id=@Id;", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                            myCmd.Parameters.AddWithValue("@Collector", Session("LoginId").ToString())

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End If
                End Using

                Dim mailingClass As New MailingClass
                mailingClass.SendInvoice(lblHeaderId.Text, Session("LoginId").ToString(), txtSendInvoiceTo.Text, ccCustomer, ccStaff)

                dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Send Invoice"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_SendInvoice(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_SendInvoice(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
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
                If lblCompanyId.Text = "3" Then
                    amount = orderClass.GetItemData_Decimal("SELECT (SUM(SellPrice) * 1.11) AS SumPrice FROM OrderCostings WHERE HeaderId='" & lblHeaderId.Text & "' AND Type='Final'")
                End If

                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET PaymentDate=GETDATE(), DueDate=NULL, Payment=1, Amount=@Amount WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@Amount", amount)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            If lblOrderStatus.Text = "Proforma Sent" Then
                Dim mailingClass As New MailingClass
                mailingClass.NewOrder(lblHeaderId.Text)

                Dim checkOcean As Integer = orderClass.GetItemData_Integer("SELECT COUNT(OrderDetails.Id) FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & lblHeaderId.Text & "' AND OrderDetails.Active=1 AND Products.DesignId='15'")
                If checkOcean > 0 Then
                    Dim thisId As String = lblHeaderId.Text

                    Task.Run(Async Function()
                                 Dim svc As New ShutterOceanService()
                                 Await svc.SendOrderAsync(thisId)
                             End Function)

                End If
            End If

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Confirm Payment Received"}
            orderClass.Logs(dataLog)

            Dim salesClass As New SalesClass
            salesClass.RefreshData(lblCompanyId.Text)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDownloadInvoice_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim invoiceClass As New InvoiceClass
            Dim pdfBytes As Byte() = invoiceClass.BindContent(lblHeaderId.Text)

            Dim fileName As String = String.Format("INVOICE {0}.pdf", lblInvoiceNumber.Text.ToUpper())

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & "")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
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

    Protected Sub btnDownloadInvoiceCSV_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim invoiceClass As New InvoiceClass

            Dim data As String = invoiceClass.BindXero(lblHeaderId.Text)

            Dim fileName As String = String.Format("{0}.csv", lblInvoiceNumber.Text)

            Response.Clear()
            Response.Buffer = True

            Response.ContentType = "text/csv"
            Response.ContentEncoding = Encoding.UTF8

            Response.AddHeader("content-disposition", "attachment;filename=" & fileName)

            Dim bom As Byte() = Encoding.UTF8.GetPreamble()
            Response.OutputStream.Write(bom, 0, bom.Length)

            Dim bytes As Byte() = Encoding.UTF8.GetBytes(data)

            Response.OutputStream.Write(bytes, 0, bytes.Length)

            Response.Flush()
            Response.End()

        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
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
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET InvoiceNumber=@InvoiceNumber WHERE Id=@Id;", thisConn)
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
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET InvoiceNumber=@InvoiceNumber, Collector=@Collector, InvoiceDate=@InvoiceDate, PaymentDate=@PaymentDate, DueDate=@DueDate, Payment=@Payment, Amount=@Amount WHERE Id=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@InvoiceNumber", txtInvoiceNumber.Text.Trim())
                    myCmd.Parameters.AddWithValue("@Collector", If(String.IsNullOrEmpty(ddlCollector.SelectedValue), CType(DBNull.Value, Object), ddlCollector.SelectedValue))
                    myCmd.Parameters.AddWithValue("@InvoiceDate", If(String.IsNullOrEmpty(txtInvoiceDate.Text), CType(DBNull.Value, Object), txtInvoiceDate.Text))
                    myCmd.Parameters.AddWithValue("@PaymentDate", If(String.IsNullOrEmpty(txtPaymentDate.Text), CType(DBNull.Value, Object), txtPaymentDate.Text))
                    myCmd.Parameters.AddWithValue("@DueDate", If(String.IsNullOrEmpty(txtDueDate.Text), CType(DBNull.Value, Object), txtDueDate.Text))
                    myCmd.Parameters.AddWithValue("@Payment", ddlPayment.SelectedValue)
                    myCmd.Parameters.AddWithValue("@Amount", amount)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderHeaders", lblHeaderId.Text, Session("LoginId"), "Update Invoice Data"}
            orderClass.Logs(dataLog)

            Dim salesClass As New SalesClass
            salesClass.RefreshData(lblCompanyId.Text)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_InvoiceData(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_InvoiceData(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showInvoiceData", thisScript, True)
        End Try
    End Sub

    Protected Sub btnBuilderDetail_Click(sender As Object, e As EventArgs)
        MessageError_BuilderDetail(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showBuilderDetail(); };"
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE OrderBuilders SET Estimator=@Estimator, Supervisor=@Supervisor, Address=@Address, CallForCheckMeasure=@CallForCheckMeasure, CheckMeasureDue=@CheckMeasureDue, ToBeInstalled=@ToBeInstalled, Installed=@Installed WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    thisCmd.Parameters.AddWithValue("@Estimator", txtEstimator.Text.Trim())
                    thisCmd.Parameters.AddWithValue("@Supervisor", txtSupervisor.Text.Trim())
                    thisCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim())
                    thisCmd.Parameters.AddWithValue("@CallForCheckMeasure", If(String.IsNullOrEmpty(txtCallForCheckMeasure.Text), CType(DBNull.Value, Object), txtCallForCheckMeasure.Text))
                    thisCmd.Parameters.AddWithValue("@CheckMeasureDue", If(String.IsNullOrEmpty(txtCheckMeasureDue.Text), CType(DBNull.Value, Object), txtCheckMeasureDue.Text))
                    thisCmd.Parameters.AddWithValue("@ToBeInstalled", If(String.IsNullOrEmpty(txtToBeInstalled.Text), CType(DBNull.Value, Object), txtToBeInstalled.Text))
                    thisCmd.Parameters.AddWithValue("@Installed", If(String.IsNullOrEmpty(txtInstalled.Text), CType(DBNull.Value, Object), txtInstalled.Text))

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
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showBuilderDetail", thisScript, True)
        End Try
    End Sub

    Protected Sub btnConvertOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            'Using thisConn As New SqlConnection(myConn)
            '    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status=@Status, ProductionDate=GETDATE() WHERE Id=@Id;", thisConn)
            '        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
            '        thisConn.Open()
            '        myCmd.ExecuteNonQuery()
            '    End Using
            'End Using

            Dim detailData As DataTable = orderClass.GetDataTable("SELECT OrderDetails.Id, OrderDetails.TotalItems, Products.JobSheetId FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & lblHeaderId.Text & "'")
            If detailData.Rows.Count > 0 Then
                For i As Integer = 0 To detailData.Rows.Count - 1
                    Dim itemId As String = detailData.Rows(i).Item("Id").ToString()
                    Dim totalItems As Integer = CInt(detailData.Rows(i).Item("TotalItems"))
                    Dim jobSheetId As String = detailData.Rows(i).Item("JobSheetId").ToString()

                    If String.IsNullOrEmpty(jobSheetId) Then
                        Continue For
                    End If

                    For j As Integer = 1 To totalItems
                        Dim formulaColumn As String = "Formula1"
                        If j = 2 Then formulaColumn = "Formula2"
                        Dim params As New List(Of SqlParameter) From {
                            New SqlParameter("@JobSheetId", jobSheetId),
                            New SqlParameter("@JobNumber", "Reza"),
                            New SqlParameter("@ItemId", itemId),
                            New SqlParameter("@ItemNumber", j),
                            New SqlParameter("@FormulaColumn", formulaColumn)
                        }
                        orderClass.ExecuteSP("sp_InsertJobOrders", params)
                    Next
                Next
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnReConvertOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try

        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvListOrderFile_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Try
            Dim fileName As String = e.CommandArgument.ToString()
            Dim directoryPath As String = Server.MapPath(String.Format("~/File/Order/{0}/", lblOrderId.Text))
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
            End If
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
            End If
        End Try
    End Sub

    Protected Sub gvListItem_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Dim thisData As DataRow = orderClass.GetDataRow("SELECT Designs.Id AS DesignId, Designs.Type AS DesignType, Designs.Page AS DesignPage FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id WHERE OrderDetails.Id='" & dataId & "' AND OrderDetails.Active=1")

                    Dim designId As String = thisData("DesignId").ToString()
                    Dim designType As String = thisData("DesignType").ToString()
                    Dim designPage As String = thisData("DesignPage").ToString()

                    Dim itemAction As String = String.Empty
                    If Session("RoleName") = "Developer" Then itemAction = "edit"

                    If Session("RoleName") = "IT" Then
                        itemAction = "edit"
                        If lblOrderStatus.Text = "Canceled" OrElse lblOrderStatus.Text = "Shipped Out" OrElse lblOrderStatus.Text = "Completed" Then itemAction = "view"
                    End If

                    If Session("RoleName") = "Factory Office" Then
                        itemAction = "edit"
                        If lblOrderStatus.Text = "Canceled" OrElse lblOrderStatus.Text = "Completed" OrElse lblOrderStatus.Text = "Shipped Out" Then
                            itemAction = "view"
                        End If
                    End If

                    If Session("RoleName") = "Sales" Then
                        itemAction = "view"
                        If lblOrderStatus.Text = "Unsubmitted" OrElse lblOrderStatus.Text = "Quoted" OrElse lblOrderStatus.Text = "Waiting Proforma" Then
                            itemAction = "edit"
                        End If
                    End If

                    If Session("RoleName") = "Account" Then
                        itemAction = "view"
                        If lblOrderStatus.Text = "Unsubmitted" OrElse lblOrderStatus.Text = "Quoted" OrElse lblOrderStatus.Text = "Waiting Proforma" OrElse lblOrderStatus.Text = "Proforma Sent" OrElse lblOrderStatus.Text = "Payment Received" Then
                            itemAction = "edit"
                        End If
                        If designId = "16" Then itemAction = "edit"
                    End If

                    If Session("RoleName") = "Data Entry" Then
                        itemAction = "view"
                        If lblOrderStatus.Text = "Unsubmitted" OrElse lblOrderStatus.Text = "Quoted" OrElse lblOrderStatus.Text = "Waiting Proforma" OrElse lblOrderStatus.Text = "Proforma Sent" OrElse lblOrderStatus.Text = "Payment Received" Then
                            itemAction = "edit"
                        End If
                    End If

                    If Session("RoleName") = "Installer" Then
                        itemAction = "view"
                        If lblOrderStatus.Text = "Quoted" Then itemAction = "edit"
                    End If

                    If Session("RoleName") = "Export" Then
                        itemAction = "view"
                    End If

                    If Session("RoleName") = "Customer" Then
                        itemAction = "view"
                        If lblOrderStatus.Text = "Unsubmitted" Then itemAction = "edit"
                    End If

                    If designType = "Services" Then
                        MessageError(True, "ACCESS DENIED. YOU ARE NOT AUTHORIZED TO PERFORM THIS ACTION !")
                        Exit Sub
                    End If

                    If String.IsNullOrEmpty(itemAction) Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        Exit Sub
                    End If

                    Dim queryString As String = String.Format("do={0}&orderid={1}&itemid={2}&dtype={3}&uid={4}", itemAction, lblHeaderId.Text, dataId, designId, Session("LoginId").ToString())

                    Dim contextId As String = InsertContext(queryString)
                    url = String.Format("{0}?boos={1}", designPage, contextId)

                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
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
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showPrinting", thisScript, True)
                End Try
            ElseIf e.CommandName = "EditCosting" Then
                MessageError(False, String.Empty)
                Try
                    Dim queryString As String = String.Format("headerid={0}&itemid={1}", lblHeaderId.Text, dataId)
                    Dim contextId As String = InsertContext(queryString)
                    url = String.Format("~/order/editcosting?boos={0}", contextId)
                    Response.Redirect(url)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
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
            End If
        End Try
    End Sub

    Protected Sub btnAddService_Click(sender As Object, e As EventArgs)
        MessageError_AddService(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showService(); };"
        Try
            If ddlAddService.SelectedValue = "" Then
                MessageError_AddService(True, "ITEM SERVICE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showService", thisScript, True)
                Exit Sub
            End If
            If msgErrorAddService.InnerText = "" Then
                Dim designId As String = "16"

                Dim productName As String = orderClass.GetProductName(ddlAddService.SelectedValue)
                Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(productName, designId, lblCompanyDetailId.Text)

                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails (Id, HeaderId, ProductId, PriceProductGroupId, Qty, Width, [Drop], LinearMetre, SquareMetre, TotalItems, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, 1, 0, 0, 0, 0, 1, 0, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@ProductId", ddlAddService.SelectedValue)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(lblHeaderId.Text, itemId)

                If Not String.IsNullOrEmpty(txtNoteService.Text.Trim()) Then
                    Dim costingArray As Object() = {lblHeaderId.Text, itemId, 0, "Note", txtNoteService.Text, 0, 0}
                    orderClass.OrderCostings(costingArray)
                    orderClass.FinalCostItem(lblHeaderId.Text, itemId)
                End If

                orderClass.CalculatePrice(lblHeaderId.Text, itemId)
                orderClass.UpdateServiceItem(lblHeaderId.Text, itemId, txtBuyService.Text, txtSellService.Text)
                orderClass.FinalCostItem(lblHeaderId.Text, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, Session("LoginId"), "Order Item Added"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_AddService(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_AddService(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
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

            dataLog = {"OrderDetails", thisId, Session("LoginId"), "Delete Order Item"}
            orderClass.Logs(dataLog)

            If lblOrderStatus.Text = "In Production" OrElse lblOrderStatus.Text = "On Hold" Then
                Dim salesClass As New SalesClass
                salesClass.RefreshData(lblCompanyId.Text)
            End If

            orderClass.UpdateOrderFactory(lblHeaderId.Text)

            url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
            Response.Redirect(url, False)
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
            lblOrderFactory.Text = headerData("OrderFactory").ToString()
            lblInternalNote.Text = orderClass.GetItemData("SELECT TOP 1 'Noted By ' + ISNULL(Logins.FullName, '') + ' | ' + ISNULL(OrderInternalNotes.Note, '') AS NoteDetail FROM OrderInternalNotes LEFT JOIN Logins ON OrderInternalNotes.CreatedBy=Logins.Id WHERE OrderInternalNotes.HeaderId='" & headerId & "' ORDER BY OrderInternalNotes.CreatedDate DESC;")
            If lblInternalNote.Text = "" Then lblInternalNote.Text = "-"
            lblCreatedBy.Text = headerData("CreatedBy").ToString()
            lblCreatedName.Text = headerData("CreatedFullName").ToString()
            lblCreatedRole.Text = headerData("CreatedRole").ToString()
            lblDownloadBoe.Text = headerData("Download").ToString()
            Dim customerOnStop As String = headerData("CustomerOnStop").ToString()

            lblCreatedDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("CreatedDate").ToString()) Then
                lblCreatedDate.Text = Convert.ToDateTime(headerData("CreatedDate")).ToString("dd MMM yyyy")
                txtCreatedDate.Text = Convert.ToDateTime(headerData("CreatedDate")).ToString("yyyy-MM-dd")
            End If

            lblQuotedDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("QuotedDate").ToString()) Then
                lblQuotedDate.Text = Convert.ToDateTime(headerData("QuotedDate")).ToString("dd MMM yyyy")
                txtQuotedDate.Text = Convert.ToDateTime(headerData("QuotedDate")).ToString("yyyy-MM-dd")
            End If

            lblSubmittedDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("SubmittedDate").ToString()) Then
                lblSubmittedDate.Text = Convert.ToDateTime(headerData("SubmittedDate")).ToString("dd MMM yyyy")
                txtSubmittedDate.Text = Convert.ToDateTime(headerData("SubmittedDate")).ToString("yyyy-MM-dd")
            End If

            lblProductionDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("ProductionDate").ToString()) Then
                lblProductionDate.Text = Convert.ToDateTime(headerData("ProductionDate")).ToString("dd MMM yyyy")
                txtProductionDate.Text = Convert.ToDateTime(headerData("ProductionDate")).ToString("yyyy-MM-dd")
            End If

            lblOnHoldDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("OnHoldDate").ToString()) Then
                lblOnHoldDate.Text = Convert.ToDateTime(headerData("OnHoldDate")).ToString("dd MMM yyyy")
                txtHoldDate.Text = Convert.ToDateTime(headerData("OnHoldDate")).ToString("yyyy-MM-dd")
            End If

            lblCanceledDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("CanceledDate").ToString()) Then
                lblCanceledDate.Text = Convert.ToDateTime(headerData("CanceledDate")).ToString("dd MMM yyyy")
                txtCanceledDate.Text = Convert.ToDateTime(headerData("CanceledDate")).ToString("yyyy-MM-dd")
            End If

            lblCompletedDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("CompletedDate").ToString()) Then
                lblCompletedDate.Text = Convert.ToDateTime(headerData("CompletedDate")).ToString("dd MMM yyyy")
                txtCompletedDate.Text = Convert.ToDateTime(headerData("CompletedDate")).ToString("yyyy-MM-dd")
            End If

            BindDesignType()
            BindService()
            BindDataQuote()
            BindDataItem(lblOrderStatus.Text)
            BindDataCosting(gvListItem.Rows.Count)

            'BIND INVOICE
            lblInvoiceNumber.Text = "-"
            lblInvoiceDate.Text = "-"
            lblCollector.Text = "-"
            lblPaymentDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData("InvoiceNumber").ToString()) Then
                lblInvoiceNumber.Text = headerData("InvoiceNumber").ToString()
                txtUpdateInvoiceNumber.Text = headerData("InvoiceNumber").ToString()
                txtInvoiceNumber.Text = headerData("InvoiceNumber").ToString()
            End If

            If Not String.IsNullOrEmpty(headerData("InvoiceDate").ToString()) Then
                lblInvoiceDate.Text = Convert.ToDateTime(headerData("InvoiceDate")).ToString("dd MMM yyyy")
                txtInvoiceDate.Text = Convert.ToDateTime(headerData("InvoiceDate")).ToString("yyyy-MM-dd")
            End If

            If Not String.IsNullOrEmpty(headerData("DueDate").ToString()) Then
                txtDueDate.Text = Convert.ToDateTime(headerData("DueDate")).ToString("yyyy-MM-dd")
            End If

            If Not String.IsNullOrEmpty(headerData("CollectorName").ToString()) Then
                lblCollector.Text = headerData("CollectorName").ToString()
                ddlCollector.SelectedValue = headerData("Collector").ToString()
            End If

            If Not String.IsNullOrEmpty(headerData("PaymentDate").ToString()) Then
                lblPaymentDate.Text = Convert.ToDateTime(headerData("PaymentDate")).ToString("dd MMM yyyy")
                txtPaymentDate.Text = Convert.ToDateTime(headerData("PaymentDate")).ToString("yyyy-MM-dd")
            End If

            ddlPayment.SelectedValue = Convert.ToInt32(headerData("Payment"))
            lblOrderPaid.Text = headerData("OrderPaid").ToString()

            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then
                divInvoicing.Attributes.Add("onclick", "showInvoiceData()")
                divInvoicing.Style.Add("cursor", "pointer")
            End If

            ' BIND SHIPMENT
            lblShipmentNumber.Text = "-"
            lblShipmentDate.Text = "-"
            lblContainerNumber.Text = "-"
            lblContainerEta.Text = "-"
            lblCourier.Text = "-"
            If Not String.IsNullOrEmpty(headerData("ShipmentNumber").ToString()) Then
                lblShipmentNumber.Text = headerData("ShipmentNumber").ToString()
                txtShipmentNumber.Text = headerData("ShipmentNumber").ToString()
            End If

            If Not String.IsNullOrEmpty(headerData("ShipmentDate").ToString()) Then
                lblShipmentDate.Text = Convert.ToDateTime(headerData("ShipmentDate")).ToString("dd MMM yyyy")
                txtShipmentDate.Text = Convert.ToDateTime(headerData("ShipmentDate")).ToString("yyyy-MM-dd")
            End If

            If Not String.IsNullOrEmpty(headerData("ContainerEta").ToString()) Then
                lblContainerEta.Text = Convert.ToDateTime(headerData("ContainerEta")).ToString("dd MMM yyyy")
                txtContainerEta.Text = Convert.ToDateTime(headerData("ContainerEta")).ToString("yyyy-MM-dd")
            End If

            If Not String.IsNullOrEmpty(headerData("ContainerNumber").ToString()) Then
                lblContainerNumber.Text = headerData("ContainerNumber").ToString()
                txtContainerNumber.Text = headerData("ContainerNumber").ToString()
            End If

            If Not String.IsNullOrEmpty(headerData("Courier").ToString()) Then
                lblCourier.Text = headerData("Courier").ToString()
                txtCourier.Text = headerData("Courier").ToString()
            End If

            If lblOrderType.Text = "Builder" Then BindDataBuilder()

            BindCollector()
            BindEmailQuote()
            BindEmailInvoice()
            BindDataFile(lblOrderId.Text)

            btnEditOrder.Visible = False
            aDeleteOrder.Visible = False
            aQuoteOrder.Visible = False
            aSubmitOrder.Visible = False : chkSendEmail.Visible = False
            aDuplicateOrder.Visible = False

            btnUpdateStatus.Visible = False
            aNewOrder.Visible = False
            aUnsubmitOrder.Visible = False
            aProductionOrder.Visible = False
            aHoldOrder.Visible = False
            aShippedOrder.Visible = False
            aCompleteOrder.Visible = False
            aCancelOrder.Visible = False

            aReworkOrder.Visible = False

            btnQuoteAction.Visible = False
            aQuoteCustomer.Visible = False
            aSendQuote.Visible = False

            btnInvoice.Visible = False
            aSendInvoice.Visible = False
            aReceivePayment.Visible = False
            liDividerInvoice.Visible = False
            liUpdateInvoiceNumber.Visible = False
            liUpdateInvoiceData.Visible = False

            aBuilderData.Visible = False
            'btnJob.Visible = False
            aFile.Visible = False
            aMoreDownloadBOE.Visible = False
            btnSuratJalan.Visible = False
            aRePrice.Visible = False

            aLog.Visible = False : aLog.Attributes("data-id") = headerId

            divOrderType.Visible = False
            divOrderFactory.Visible = False
            divInternalNote.Visible = False

            aAddItem.Visible = False
            aAddService.Visible = False

            Dim isReworkOrder As Boolean = orderClass.IsReworkOrder(headerId)

            If Session("RoleName") = "Developer" Then
                aDuplicateOrder.Visible = True

                aFile.Visible = True
                aLog.Visible = True
                aRePrice.Visible = True
                If lblCompanyId.Text = "3" Then btnSuratJalan.Visible = True

                If lblOrderType.Text = "Builder" Then aBuilderData.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True
                    aDeleteOrder.Visible = True
                    aSubmitOrder.Visible = True : chkSendEmail.Visible = True
                    If lblOrderType.Text = "Builder" Then aQuoteOrder.Visible = True

                    btnQuoteAction.Visible = True
                    aSendQuote.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True

                    If lblDownloadBoe.Text = "No" OrElse lblDownloadBoe.Text = "Done" Then
                        aMoreDownloadBOE.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Quoted" Then
                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True
                    aDeleteOrder.Visible = True
                    aSubmitOrder.Visible = True : chkSendEmail.Visible = True

                    btnQuoteAction.Visible = True
                    aSendQuote.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Waiting Proforma" Then
                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True
                    aNewOrder.Visible = True

                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceNumber.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Proforma Sent" Then
                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True
                    aReceivePayment.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceNumber.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Payment Received" Then
                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aProductionOrder.Visible = True
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True

                    If lblDownloadBoe.Text = "No" OrElse lblDownloadBoe.Text = "Done" Then
                        aMoreDownloadBOE.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "New Order" Then
                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True
                    aHoldOrder.Visible = True
                    aProductionOrder.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "In Production" Then
                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True
                    aShippedOrder.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True

                    If lblDownloadBoe.Text = "No" OrElse lblDownloadBoe.Text = "Done" Then
                        aMoreDownloadBOE.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "On Hold" Then
                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aProductionOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True

                    If lblDownloadBoe.Text = "No" OrElse lblDownloadBoe.Text = "Done" Then
                        aMoreDownloadBOE.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Shipped Out" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                    End If
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    btnUpdateStatus.Visible = True
                    aProductionOrder.Visible = True
                    aCompleteOrder.Visible = True

                    If isReworkOrder = False Then
                        aReworkOrder.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Completed" Then
                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If isReworkOrder = False Then aReworkOrder.Visible = True
                End If

                divInternalNote.Visible = True
                divOrderType.Visible = True
                divOrderFactory.Visible = True

                divDateAction.Attributes.Add("onclick", "showDateOrder()")
                divDateAction.Style.Add("cursor", "pointer")

                divShipmentOrder.Attributes.Add("onclick", "showShippedOrder()")
                divShipmentOrder.Style.Add("cursor", "pointer")

                divInvoicing.Attributes.Add("onclick", "showInvoiceData()")
                divInvoicing.Style.Add("cursor", "pointer")
            End If
            If Session("RoleName") = "IT" Then
                aDuplicateOrder.Visible = True
                aFile.Visible = True
                aLog.Visible = True

                If lblCompanyId.Text = "3" Then btnSuratJalan.Visible = True
                If lblOrderType.Text = "Builder" Then aBuilderData.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True
                    aSubmitOrder.Visible = True : chkSendEmail.Visible = True
                    aRePrice.Visible = True

                    btnQuoteAction.Visible = True
                    aSendQuote.Visible = True

                    If lblOrderType.Text = "Builder" Then aQuoteOrder.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Quoted" Then
                    btnEditOrder.Visible = True
                    aSubmitOrder.Visible = True
                    aRePrice.Visible = True

                    btnQuoteAction.Visible = True
                    aSendQuote.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Waiting Proforma" Then
                    btnEditOrder.Visible = True
                    aRePrice.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True
                    aNewOrder.Visible = True

                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceNumber.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Proforma Sent" Then
                    btnEditOrder.Visible = True
                    aRePrice.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True
                    aReceivePayment.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceNumber.Visible = True
                End If
                If lblOrderStatus.Text = "Payment Received" Then
                    btnEditOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aProductionOrder.Visible = True
                    aHoldOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "New Order" Then
                    btnEditOrder.Visible = True
                    aRePrice.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aProductionOrder.Visible = True
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                        aRePrice.Visible = True
                        aAddItem.Visible = True
                        aAddService.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "In Production" Then
                    btnEditOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True
                    aShippedOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                        aRePrice.Visible = True
                        aAddItem.Visible = True
                        aAddService.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "On Hold" Then
                    btnEditOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aProductionOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aRePrice.Visible = True
                        aSendInvoice.Visible = True
                        aAddItem.Visible = True
                        aAddService.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Shipped Out" Then
                    btnUpdateStatus.Visible = True
                    aCompleteOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aRePrice.Visible = True
                        aSendInvoice.Visible = True
                    End If

                    If isReworkOrder = False AndAlso lblOrderType.Text = "Regular" Then
                        aReworkOrder.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Completed" Then
                    If isReworkOrder = False Then
                        aReworkOrder.Visible = True
                    End If
                End If

                divInternalNote.Visible = True
                divOrderType.Visible = True
                divOrderFactory.Visible = True

                divDateAction.Attributes.Add("onclick", "showDateOrder()")
                divDateAction.Style.Add("cursor", "pointer")
            End If
            If Session("RoleName") = "Factory Office" Then
                aDuplicateOrder.Visible = True
                aFile.Visible = True
                aLog.Visible = True
                If lblCompanyId.Text = "3" Then btnSuratJalan.Visible = True
                If lblOrderType.Text = "Builder" Then aBuilderData.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    aSubmitOrder.Visible = True : chkSendEmail.Visible = True
                    If lblOrderType.Text = "Builder" Then aQuoteOrder.Visible = True

                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True
                    aRePrice.Visible = True

                    btnQuoteAction.Visible = True
                    aSendQuote.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Quoted" Then
                    btnEditOrder.Visible = True
                    aSubmitOrder.Visible = True
                    aRePrice.Visible = True

                    btnQuoteAction.Visible = True
                    aSendQuote.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Waiting Proforma" Then
                    btnEditOrder.Visible = True
                    aRePrice.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True
                    aNewOrder.Visible = True

                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Proforma Sent" Then
                    btnEditOrder.Visible = True
                    aRePrice.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True
                    aReceivePayment.Visible = True
                End If
                If lblOrderStatus.Text = "Payment Received" Then
                    btnEditOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aProductionOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True
                End If
                If lblOrderStatus.Text = "New Order" Then
                    btnEditOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aCancelOrder.Visible = True
                    aHoldOrder.Visible = True
                    aUnsubmitOrder.Visible = True
                    aProductionOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                        aRePrice.Visible = True
                        aAddItem.Visible = True
                        aAddService.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "In Production" Then
                    btnUpdateStatus.Visible = True
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True
                    aShippedOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                        aRePrice.Visible = True
                        aAddItem.Visible = True
                        aAddService.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "On Hold" Then
                    btnUpdateStatus.Visible = True
                    aProductionOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                        aRePrice.Visible = True
                        aAddItem.Visible = True
                        aAddService.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Shipped Out" Then
                    btnUpdateStatus.Visible = True
                    aCompleteOrder.Visible = True

                    btnInvoice.Visible = True
                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                    End If
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If isReworkOrder = False AndAlso lblOrderType.Text = "Regular" Then aReworkOrder.Visible = True
                End If
                If lblOrderStatus.Text = "Completed" Then
                    If isReworkOrder = False Then aReworkOrder.Visible = True
                End If

                divInternalNote.Visible = True
                divOrderType.Visible = True
                divOrderFactory.Visible = True
            End If
            If Session("RoleName") = "Sales" Then
                aDuplicateOrder.Visible = True
                aFile.Visible = True

                If lblOrderType.Text = "Builder" Then aBuilderData.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    btnEditOrder.Visible = True

                    btnQuoteAction.Visible = True
                    aSendQuote.Visible = True
                    If lblOrderType.Text = "Regular" Then
                        If Session("LoginId") = lblCreatedBy.Text Then aDeleteOrder.Visible = True
                        aSubmitOrder.Visible = True
                        aAddItem.Visible = True
                    End If
                    If lblOrderType.Text = "Builder" Then
                        aQuoteOrder.Visible = True
                        aAddItem.Visible = True
                    End If

                End If
                If lblOrderStatus.Text = "Quoted" Then
                    btnEditOrder.Visible = True
                    aSubmitOrder.Visible = True

                    btnQuoteAction.Visible = True
                    aSendQuote.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Waiting Proforma" Then
                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceNumber.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Proforma Sent" Then
                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True
                    aReceivePayment.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceNumber.Visible = True

                    btnUpdateStatus.Visible = True
                    aCancelOrder.Visible = True
                End If
                If lblOrderStatus.Text = "Payment Received" Then
                    btnInvoice.Visible = True
                End If
                If lblOrderStatus.Text = "New Order" Then
                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aProductionOrder.Visible = True
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "In Production" Then
                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "On Hold" Then
                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Shipped Out" Then
                    btnInvoice.Visible = True
                    If isReworkOrder = False AndAlso lblOrderType.Text = "Regular" AndAlso customerOnStop = "No" Then
                        aReworkOrder.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Completed" Then
                    btnInvoice.Visible = True
                End If

                divInternalNote.Visible = True
                divOrderType.Visible = True
                divOrderFactory.Visible = True
            End If
            If Session("RoleName") = "Account" Then
                aFile.Visible = True
                If lblOrderType.Text = "Builder" Then aBuilderData.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    btnEditOrder.Visible = True
                    aRePrice.Visible = True

                    btnQuoteAction.Visible = True
                    aSendQuote.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Quoted" Then
                    aRePrice.Visible = True

                    btnQuoteAction.Visible = True
                    aSendQuote.Visible = True
                End If
                If lblOrderStatus.Text = "Waiting Proforma" Then
                    btnEditOrder.Visible = True
                    aRePrice.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aCancelOrder.Visible = True
                    aNewOrder.Visible = True

                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceNumber.Visible = True

                    aAddItem.Visible = True
                    aAddService.Visible = True
                End If
                If lblOrderStatus.Text = "Proforma Sent" Then
                    btnEditOrder.Visible = True
                    aRePrice.Visible = True

                    btnUpdateStatus.Visible = True
                    aCancelOrder.Visible = True
                    aUnsubmitOrder.Visible = True

                    btnInvoice.Visible = True
                    aSendInvoice.Visible = True
                    aReceivePayment.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceNumber.Visible = True
                End If
                If lblOrderStatus.Text = "Payment Received" Then
                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True
                End If
                If lblOrderStatus.Text = "New Order" Then
                    btnEditOrder.Visible = True

                    btnUpdateStatus.Visible = True
                    aUnsubmitOrder.Visible = True
                    aProductionOrder.Visible = True
                    aHoldOrder.Visible = True
                    aCancelOrder.Visible = True

                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                        aRePrice.Visible = True
                        aAddService.Visible = True
                        aAddItem.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "In Production" Then
                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                        aRePrice.Visible = True
                        aAddItem.Visible = True
                        aAddService.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "On Hold" Then
                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                        aRePrice.Visible = True
                        aAddItem.Visible = True
                        aAddService.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Shipped Out" Then
                    btnInvoice.Visible = True
                    liDividerInvoice.Visible = True
                    liUpdateInvoiceData.Visible = True

                    btnUpdateStatus.Visible = True
                    aCompleteOrder.Visible = True

                    If lblOrderPaid.Text = "" Then
                        aSendInvoice.Visible = True
                        aRePrice.Visible = True
                        aAddService.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Completed" Then
                    btnInvoice.Visible = True
                End If

                divInternalNote.Visible = True
                divOrderType.Visible = True
                divOrderFactory.Visible = True
            End If
            If Session("RoleName") = "Data Entry" Then
                aDuplicateOrder.Visible = True

                If lblOrderType.Text = "Builder" Then aBuilderData.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    btnEditOrder.Visible = True
                    If lblCreatedRole.Text = Session("RoleId") Then
                        aDeleteOrder.Visible = True

                        If lblOrderType.Text = "Regular" Then aSubmitOrder.Visible = True
                    End If
                    If lblOrderType.Text = "Builder" Then aQuoteOrder.Visible = True
                    aAddItem.Visible = True
                End If
                If lblOrderStatus.Text = "Quoted" Then
                    btnQuoteAction.Visible = True
                    aFile.Visible = True
                    aAddItem.Visible = True
                End If
                If lblOrderStatus.Text = "New Order" Then
                    btnUpdateStatus.Visible = True
                    aCancelOrder.Visible = True
                    aHoldOrder.Visible = True
                    aProductionOrder.Visible = True
                End If
                If lblOrderStatus.Text = "Payment Received" Then
                    btnUpdateStatus.Visible = True
                    aProductionOrder.Visible = True
                    aHoldOrder.Visible = True
                End If
                If lblOrderStatus.Text = "In Production" Then
                    btnUpdateStatus.Visible = True
                    aHoldOrder.Visible = True
                    aShippedOrder.Visible = True
                    aCancelOrder.Visible = True
                End If
                If lblOrderStatus.Text = "On Hold" Then
                    btnUpdateStatus.Visible = True
                    aProductionOrder.Visible = True
                    aCancelOrder.Visible = True
                End If
                If lblOrderStatus.Text = "Shipped Out" Then
                    If isReworkOrder = False AndAlso lblOrderType.Text = "Regular" Then
                        aReworkOrder.Visible = True
                    End If
                End If

                divInternalNote.Visible = True
                divOrderType.Visible = True
                divOrderFactory.Visible = True
            End If
            If Session("RoleName") = "Export" Then
                aFile.Visible = True

                If lblCompanyId.Text = "3" Then btnSuratJalan.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" OrElse lblOrderStatus.Text = "Waiting Proforma" OrElse lblOrderStatus.Text = "Proforma Sent" OrElse lblOrderStatus.Text = "Payment Received" OrElse lblOrderStatus.Text = "New Order" OrElse lblOrderStatus.Text = "Canceled" Then
                    Response.Redirect("~/order", False)
                    Exit Sub
                End If

                If lblOrderStatus.Text = "In Production" Then
                    btnUpdateStatus.Visible = True
                    aShippedOrder.Visible = True
                End If
                If lblOrderStatus.Text = "Shipped Out" Then
                    btnUpdateStatus.Visible = True
                    aCompleteOrder.Visible = True

                    divShipmentOrder.Attributes.Add("onclick", "showShippedOrder()")
                    divShipmentOrder.Style.Add("cursor", "pointer")
                End If

                divInternalNote.Visible = True
                divOrderType.Visible = True
                divOrderFactory.Visible = True
            End If
            If Session("RoleName") = "Customer" Then
                aDuplicateOrder.Visible = True

                btnQuoteAction.Visible = True
                aQuoteCustomer.Visible = True

                If lblOrderStatus.Text = "Unsubmitted" Then
                    btnEditOrder.Visible = True
                    aDeleteOrder.Visible = True
                    aSubmitOrder.Visible = True
                    aAddItem.Visible = True
                End If

                If lblOrderStatus.Text = "Shipped Out" Then
                    If isReworkOrder = False AndAlso customerOnStop = "No" Then
                        aReworkOrder.Visible = True
                    End If
                End If
                If lblOrderStatus.Text = "Completed" Then
                    If isReworkOrder = False AndAlso customerOnStop = "No" AndAlso lblOrderType.Text = "Regular" Then
                        aReworkOrder.Visible = True
                    End If
                End If
            End If
            If Session("RoleName") = "Installer" AndAlso lblOrderStatus.Text = "Quoted" Then
                aSubmitOrder.Visible = True
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

    Protected Sub BindDesignType()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = orderClass.GetDataTable("SELECT Designs.Id, Designs.Name AS NameText FROM CustomerProductAccess CROSS APPLY STRING_SPLIT(CustomerProductAccess.DesignId, ',') AS designArray INNER JOIN Designs ON designArray.VALUE=Designs.Id WHERE CustomerProductAccess.Id='" & lblCustomerId.Text & "' AND Designs.Type<>'Services' AND Designs.Active=1 ORDER BY Designs.Name ASC")
            ddlDesign.DataTextField = "NameText"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlDesign.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindService()
        ddlAddService.Items.Clear()
        Try
            ddlAddService.DataSource = orderClass.GetDataTable("SELECT Id, Name FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId=16 AND BlindId=96 AND companyArray.VALUE='" & lblCompanyDetailId.Text & "' AND Status='In Stock' ORDER BY Name ASC")
            ddlAddService.DataTextField = "Name"
            ddlAddService.DataValueField = "Id"
            ddlAddService.DataBind()

            If ddlAddService.Items.Count > 0 Then
                ddlAddService.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlAddService.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindCollector()
        ddlCollector.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT * FROM Logins"
            If Session("RoleName") = "Account" OrElse Session("RoleName") = "Sales" Then
                thisQuery = "SELECT * FROM Logins WHERE RoleId='4' OR RoleId='5'"
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
        divMinimumOrderSurcharge.Visible = False : divFuelSurcharge.Visible = False
        Try
            Dim param As New List(Of SqlParameter) From {
                New SqlParameter("@HeaderId", Convert.ToInt32(lblHeaderId.Text))
            }

            gvListItem.DataSource = orderClass.GetDataTableSP("sp_GetOrderItemsByHeader", param)
            gvListItem.DataBind()

            gvListItem.Columns(1).Visible = LoginAccess("Visible ID")
            gvListItem.Columns(2).Visible = LoginAccess("Visible Product ID")

            gvListItem.Columns(4).Visible = LoginAccess("Visible Buy Price")
            gvListItem.Columns(5).Visible = LoginAccess("Visible Sell Price")
            gvListItem.Columns(6).Visible = LoginAccess("Visible Price")

            gvListItem.Columns(7).Visible = False
            If Session("PriceAccess") = "Yes" Then gvListItem.Columns(7).Visible = True

            Dim totalItems As Integer = orderClass.GetTotalItemOrder(lblHeaderId.Text)
            If status = "Unsubmitted" AndAlso lblCompanyId.Text = "2" AndAlso totalItems > 0 Then
                divFuelSurcharge.Visible = True
                If totalItems <= 3 Then divMinimumOrderSurcharge.Visible = True
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

    Protected Sub BindDataQuote()
        Dim quoteData As DataRow = orderClass.GetDataRow("SELECT * FROM OrderQuotes WHERE Id='" & lblHeaderId.Text & "'")

        If quoteData Is Nothing Then
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderQuotes VALUES(@Id, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00)", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
            Response.Redirect("~/order/detail", False)
        End If

        divQuoteCity.Visible = False

        spanDiscount.InnerText = "$"
        spanMeasure.InnerText = "$"
        spanInstall.InnerText = "$"
        spanFreight.InnerText = "$"

        If lblCompanyId.Text = "3" Then
            divQuoteCity.Visible = True

            spanDiscount.InnerText = "Rp"
            spanMeasure.InnerText = "Rp"
            spanInstall.InnerText = "Rp"
            spanFreight.InnerText = "Rp"
        End If

        txtQuoteEmail.Text = quoteData("Email").ToString()
        txtQuotePhone.Text = quoteData("Phone").ToString()
        txtQuoteAddress.Text = quoteData("Address").ToString()
        txtQuoteSuburb.Text = quoteData("Suburb").ToString()
        txtQuoteCity.Text = quoteData("City").ToString()
        txtQuoteState.Text = quoteData("State").ToString()
        txtQuotePostCode.Text = quoteData("PostCode").ToString()

        txtQuoteDiscount.Text = quoteData("Discount").ToString().Replace(",", ".")
        txtQuoteCheckMeasure.Text = quoteData("CheckMeasure").ToString().Replace(",", ".")
        txtQuoteInstallation.Text = quoteData("Installation").ToString().Replace(",", ".")
        txtQuoteFreight.Text = quoteData("Freight").ToString().Replace(",", ".")
    End Sub

    Protected Sub BindDataCosting(itemCount As Integer)
        secPricing.Visible = False
        If Session("PriceAccess") = "Yes" Then secPricing.Visible = True
        lblPriceOrder.Text = "-"
        lblGst.Text = "-"
        lblFinalPriceOrder.Text = "-"

        lblPriceOrderTitle.Text = "Total excl. GST"
        lblGstTitle.Text = "GST 10%"
        lblFinalPriceOrderTitle.Text = "TOTAL incl. GST"
        If lblCompanyId.Text = "3" Then
            lblPriceOrderTitle.Text = "Total excl. PPN"
            lblGstTitle.Text = "PPN 11%"
            lblFinalPriceOrderTitle.Text = "TOTAL incl. PPN"
        End If

        If itemCount > 0 Then
            Dim priceSell As Decimal = orderClass.GetItemData_Decimal("SELECT SUM(SellPrice) AS SumPrice FROM OrderCostings WHERE HeaderId='" & lblHeaderId.Text & "' AND Type='Final'")
            Dim gstSell As Decimal = priceSell * 10 / 100
            If lblCompanyId.Text = "3" Then gstSell = priceSell * 11 / 100
            Dim totalSell As Decimal = priceSell + gstSell

            lblPriceOrder.Text = "$ " & priceSell.ToString("N2", enUS)
            lblGst.Text = "$ " & gstSell.ToString("N2", enUS)
            lblFinalPriceOrder.Text = "$ " & totalSell.ToString("N2", enUS)

            If lblCompanyId.Text = "3" Then
                lblPriceOrder.Text = "Rp " & priceSell.ToString("N2", idIDR)
                lblGst.Text = "Rp " & gstSell.ToString("N2", idIDR)
                lblFinalPriceOrder.Text = "Rp " & totalSell.ToString("N2", idIDR)
            End If

            If lblCompanyId.Text = "2" Then
                Dim priceBuy As Decimal = orderClass.GetItemData_Decimal("SELECT SUM(BuyPrice) AS BuyPrice FROM OrderCostings WHERE HeaderId='" & lblHeaderId.Text & "' AND Type='Final'")

                Dim gstBuy As Decimal = priceBuy * 10 / 100
                Dim totalBuy As Decimal = priceBuy + gstBuy

                spanOrderBuy.InnerText = "$ " & priceBuy.ToString("N2", enUS)
                spanGstBuy.InnerText = "$ " & gstBuy.ToString("N2", enUS)
                spanTotalBuy.InnerText = "$ " & totalBuy.ToString("N2", enUS)
            End If
        End If

        If lblCompanyId.Text = "2" Then
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse (Session("RoleName") = "Sales" AndAlso Session("LevelName") = "Leader") Then
                divCosting.Attributes.Add("onclick", "showCostingBuy()")
                divCosting.Style.Add("cursor", "pointer")
            End If
        End If
    End Sub

    Protected Sub BindDataBuilder()
        Try
            Dim dataBuilder As DataRow = orderClass.GetDataRow("SELECT * FROM OrderBuilders WHERE Id='" & lblHeaderId.Text & "'")
            If dataBuilder IsNot Nothing Then
                txtEstimator.Text = dataBuilder("Estimator").ToString()
                txtSupervisor.Text = dataBuilder("Supervisor").ToString()
                txtAddress.Text = dataBuilder("Address").ToString()

                txtCallForCheckMeasure.Text = String.Empty
                txtCheckMeasureDue.Text = String.Empty
                txtToBeInstalled.Text = String.Empty
                txtInstalled.Text = String.Empty

                If Not String.IsNullOrEmpty(dataBuilder("CallForCheckMeasure").ToString()) Then
                    txtCallForCheckMeasure.Text = Convert.ToDateTime(dataBuilder("CallForCheckMeasure")).ToString("yyyy-MM-dd")
                End If
                If Not String.IsNullOrEmpty(dataBuilder("CheckMeasureDue").ToString()) Then
                    txtCheckMeasureDue.Text = Convert.ToDateTime(dataBuilder("CheckMeasureDue")).ToString("yyyy-MM-dd")
                End If
                If Not String.IsNullOrEmpty(dataBuilder("ToBeInstalled").ToString()) Then
                    txtToBeInstalled.Text = Convert.ToDateTime(dataBuilder("ToBeInstalled")).ToString("yyyy-MM-dd")
                End If
                If Not String.IsNullOrEmpty(dataBuilder("Installed").ToString()) Then
                    txtInstalled.Text = Convert.ToDateTime(dataBuilder("Installed")).ToString("yyyy-MM-dd")
                End If
            End If
        Catch ex As Exception
            MessageError_BuilderDetail(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_BuilderDetail(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
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
            If lblOrderType.Text = "Builder" Then divUploadAction.Visible = True
        Catch ex As Exception
            MessageError_FileOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_FileOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindEmailQuote()
        Try
            txtSendQuoteTo.Text = orderClass.GetCustomerPrimaryEmail(lblCustomerId.Text)

            Dim dataEmailCustomer As DataTable = orderClass.GetDataTable("SELECT Email FROM CustomerContacts CROSS APPLY STRING_SPLIT(Tags, ',') AS thisArray WHERE CustomerId='" & lblCustomerId.Text & "' AND LTRIM(RTRIM(Email)) <> '' AND Email IS NOT NULL AND thisArray.VALUE='Invoicing' AND [Primary]=0")
            If dataEmailCustomer.Rows.Count > 0 Then
                Dim listEmail As New List(Of String)

                For Each row As DataRow In dataEmailCustomer.Rows
                    listEmail.Add(row("Email").ToString())
                Next

                txtSendQuoteCCCustomer.Text = String.Join(vbCrLf, listEmail)
            End If

            Dim dataCCMailing As DataTable = orderClass.GetDataTable("SELECT * FROM Mailings WHERE CompanyId='" & lblCompanyId.Text & "' AND Name='Send Quote' AND Active=1")
            If dataCCMailing.Rows.Count > 0 Then
                Dim listEmail As New List(Of String)

                For Each row As DataRow In dataCCMailing.Rows
                    Dim emails As String() = row("Cc").ToString().Split(";"c)

                    For Each email As String In emails
                        If Not String.IsNullOrWhiteSpace(email) Then
                            listEmail.Add(email.Trim())
                        End If
                    Next
                Next

                txtSendQuoteCCStaff.Text = String.Join(vbCrLf, listEmail)
            End If

            Dim operatorEmail As String = orderClass.GetItemData("SELECT ISNULL(STRING_AGG(Logins.Email, ';'), '') FROM Customers OUTER APPLY STRING_SPLIT(Customers.Operator, ',') operatorArray LEFT JOIN Logins ON Logins.Id = TRY_CAST(operatorArray.value AS INT) WHERE Customers.Id='" & lblCustomerId.Text & "';")
            If Not String.IsNullOrEmpty(operatorEmail) Then
                Dim listEmail As New List(Of String)
                If Not String.IsNullOrWhiteSpace(txtSendQuoteCCStaff.Text) Then
                    listEmail.AddRange(txtSendQuoteCCStaff.Text.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries))
                End If
                Dim operatorEmails = operatorEmail.Split(";"c)
                For Each email As String In operatorEmails
                    If Not String.IsNullOrWhiteSpace(email) Then
                        listEmail.Add(email.Trim())
                    End If
                Next
                listEmail = listEmail.Distinct().ToList()
                txtSendQuoteCCStaff.Text = String.Join(vbCrLf, listEmail)
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

            Dim dataCCMailing As DataTable = orderClass.GetDataTable("SELECT * FROM Mailings WHERE CompanyId='" & lblCompanyId.Text & "' AND Name='Send Invoice' AND Active=1")
            If dataCCMailing.Rows.Count > 0 Then
                Dim listEmail As New List(Of String)

                For Each row As DataRow In dataCCMailing.Rows
                    Dim emails As String() = row("Cc").ToString().Split(";"c)

                    For Each email As String In emails
                        If Not String.IsNullOrWhiteSpace(email) Then
                            listEmail.Add(email.Trim())
                        End If
                    Next
                Next

                txtSendInvoiceCCStaff.Text = String.Join(vbCrLf, listEmail)
            End If

            Dim operatorEmail As String = orderClass.GetItemData("SELECT ISNULL(STRING_AGG(Logins.Email, ';'), '') FROM Customers OUTER APPLY STRING_SPLIT(Customers.Operator, ',') operatorArray LEFT JOIN Logins ON Logins.Id = TRY_CAST(operatorArray.value AS INT) WHERE Customers.Id='" & lblCustomerId.Text & "';")
            If Not String.IsNullOrEmpty(operatorEmail) Then
                Dim listEmail As New List(Of String)
                If Not String.IsNullOrWhiteSpace(txtSendInvoiceCCStaff.Text) Then
                    listEmail.AddRange(txtSendInvoiceCCStaff.Text.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries))
                End If
                Dim operatorEmails = operatorEmail.Split(";"c)
                For Each email As String In operatorEmails
                    If Not String.IsNullOrWhiteSpace(email) Then
                        listEmail.Add(email.Trim())
                    End If
                Next
                listEmail = listEmail.Distinct().ToList()
                txtSendInvoiceCCStaff.Text = String.Join(vbCrLf, listEmail)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function BindProductDescription(itemId As String) As String
        Return orderClass.GetProductDescription(itemId)
    End Function

    Protected Function ItemCosting(itemId As String, type As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(itemId) Then
                Dim thisQuery As String = String.Format("SELECT {0} FROM OrderCostings WHERE ItemId='{1}' AND Type='Final'", type, itemId)
                Dim thisPrice As Decimal = orderClass.GetItemData_Decimal(thisQuery)

                result = String.Format("${0}", thisPrice.ToString("N2", enUS))
                If lblCompanyId.Text = "3" Then
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
        MessageError_DuplicateOrder(visible, message)

        MessageError_BuilderDetail(visible, message)
        MessageError_FileOrder(visible, message)

        MessageError_DetailQuote(visible, message)
        MessageError_SendQuote(visible, message)

        MessageError_AddService(visible, message)
        MessageError_AddNote(visible, message)

        MessageError_SendInvoice(visible, message)
        MessageError_InvoiceNumber(visible, message)
        MessageError_InvoiceData(visible, message)

        MessageError_CancelOrder(visible, message)
        MessageError_ShippedOrder(visible, message)
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerHtml = message
        divErrorB.Visible = visible : msgErrorB.InnerHtml = message
    End Sub

    Protected Sub MessageError_DuplicateOrder(visible As Boolean, message As String)
        divErrorDuplicateOrder.Visible = visible : msgErrorDuplicateOrder.InnerHtml = message
    End Sub

    Protected Sub MessageError_BuilderDetail(visible As Boolean, message As String)
        divErrorBuilderDetail.Visible = visible : msgErrorBuilderDetail.InnerText = message
    End Sub

    Protected Sub MessageError_FileOrder(visible As Boolean, message As String)
        divErrorFileOrder.Visible = visible : msgErrorFileOrder.InnerText = message
    End Sub

    Protected Sub MessageError_DetailQuote(visible As Boolean, message As String)
        divErrorDetailQuote.Visible = visible : msgErrorDetailQuote.InnerText = message
    End Sub

    Protected Sub MessageError_SendQuote(visible As Boolean, message As String)
        divErrorSendQuote.Visible = visible : msgErrorSendQuote.InnerText = message
    End Sub

    Protected Sub MessageError_AddService(visible As Boolean, message As String)
        divErrorAddService.Visible = visible : msgErrorAddService.InnerText = message
    End Sub

    Protected Sub MessageError_AddNote(visible As Boolean, message As String)
        divErrorAddNote.Visible = visible : msgErrorAddNote.InnerText = message
    End Sub

    Protected Sub MessageError_CancelOrder(visible As Boolean, message As String)
        divErrorCancelOrder.Visible = visible : msgErrorCancelOrder.InnerText = message
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

                If designId = "16" Then Return True
            End If

            If Session("RoleName") = "Factory Office" Then
                If lblOrderStatus.Text = "Unsubmitted" Then result = True
                If lblOrderStatus.Text = "New Order" Then result = True
                If lblOrderStatus.Text = "Waiting Proforma" Then result = True

                If designId = "16" Then Return True
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

                If designId = "16" Then Return True
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
            If Session("RoleName") = "Export" Then Return False

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

        If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Sales" OrElse Session("RoleName") = "Data Entry" OrElse Session("RoleName") = "Account" OrElse Session("RoleName") = "Customer" OrElse Session("RoleName") = "Export" Then result = True

        Return result
    End Function

    Protected Function VisibleEditPrice() As Boolean
        Dim result As Boolean = False

        If Session("RoleName") = "Developer" Then result = True

        If Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" OrElse Session("RoleName") = "Account" Then
            If lblOrderPaid.Text = "" AndAlso Not lblOrderStatus.Text = "Canceled" Then result = True
        End If

        If Session("RoleName") = "Sales" Then
            If lblOrderStatus.Text = "Unsubmitted" Then result = True
            If lblOrderStatus.Text = "Quoted" Then result = True
            If lblOrderStatus.Text = "New Order" AndAlso lblOrderPaid.Text = "" Then result = True
            If lblOrderStatus.Text = "Waiting Proforma" Then result = True
        End If

        If Session("RoleName") = "Data Entry" AndAlso lblOrderType.Text = "Builder" Then
            result = True
        End If

        Return result
    End Function

    Protected Function VisibleLog() As Boolean
        If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then
            Return True
        End If
        Return False
    End Function

    Protected Function VisibleFileDelete() As Boolean
        If Session("RoleName") = "Developer" Then Return True
        If Session("RoleName") = "IT" Then Return True
        If Session("RoleName") = "Factory Office" Then Return True
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
