Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.IO.Compression

Partial Class Order_Rework_Detail
    Inherits Page

    Dim orderClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/order/rework", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("reworkid")) Then
            Response.Redirect("~/order/rework", False)
            Exit Sub
        End If

        lblReworkId.Text = Request.QueryString("reworkid").ToString()
        If Not IsPostBack Then
            AllMessageError(False, String.Empty)
            BindDataRework(lblReworkId.Text)
        End If
    End Sub

    Protected Sub btnSubmitRework_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim itemRework As DataTable = orderClass.GetDataTable("SELECT * FROM OrderReworkDetails WHERE ReworkId='" & lblReworkId.Text & "' AND Active=1 ORDER BY Id ASC")
            If itemRework.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM PRODUCT ORDER !")
            End If
            For i As Integer = 0 To itemRework.Rows.Count - 1
                Dim id As String = itemRework.Rows(i)("Id").ToString()
                Dim itemId As String = itemRework.Rows(i)("ItemId").ToString()
                Dim category As String = itemRework.Rows(i)("Category").ToString()
                Dim description As String = itemRework.Rows(i)("Description").ToString()

                Dim folderPath As String = Server.MapPath(String.Format("~/File/Rework/{0}/{1}", lblReworkId.Text, id))

                If String.IsNullOrEmpty(category) Then
                    MessageError(True, String.Format("CATEGORY IS REQUIRED FOR ITEM ID : {0} !", itemId))
                    Exit For
                End If

                If String.IsNullOrEmpty(description) Then
                    MessageError(True, String.Format("DESCRIPTION IS REQUIRED FOR ITEM ID : {0} !", itemId))
                    Exit For
                End If

                Dim files As String() = Directory.GetFiles(folderPath)
                If files.Length = 0 Then
                    MessageError(True, String.Format("FILE IS REQUIRED FOR ITEM ID : {0}", itemId))
                    Exit For
                End If
            Next

            If msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworks SET Status='Pending Approval', SubmittedDate=GETDATE() WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblReworkId.Text)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim mailingClass As New MailingClass
                mailingClass.ReworkOrder(lblReworkId.Text)

                dataLog = {"OrderReworks", lblReworkId.Text, Session("LoginId").ToString(), "Rework Submitted"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/rework/detail?reworkid={0}", lblReworkId.Text)
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

    Protected Sub btnDeleteRework_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworks SET Active='0' WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblReworkId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim folderPath As String = Path.Combine(Server.MapPath("~/File/Rework"), lblReworkId.Text)
            If Directory.Exists(folderPath) Then
                Directory.Delete(folderPath, True)
            End If

            url = String.Format("~/order/rework/detail?reworkid={0}", lblReworkId.Text)
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

    Protected Sub btnApproveRework_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim newHeaderId As String = orderClass.GetNewOrderHeaderId()

            Dim cashSale As Boolean = orderClass.GetCustomerCashSale(lblCustomerId.Text)
            Dim minSurcharge As Boolean = orderClass.GetCustomerMinimum(lblCustomerId.Text)

            Dim status As String = "New Order"
            If cashSale = True Then status = "Waiting Proforma"

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworks SET Status='Approved', HeaderIdNew=@HeaderIdNew WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblReworkId.Text)
                    myCmd.Parameters.AddWithValue("@HeaderIdNew", newHeaderId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

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
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders SELECT @NewID, @OrderId, CustomerId, NULL, CONVERT(VARCHAR(200), OrderNumber) + ' - ' + 'RW', CONVERT(VARCHAR(200), OrderName) + ' - ' + 'RW', NULL, 'Rework', OrderFactory, @Status, NULL, CreatedBy, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, @InvoiceNumber, NULL, 0, NULL, 0, 'No', NULL, 1 FROM OrderHeaders WHERE Id=@OldId;", thisConn)
                            myCmd.Parameters.AddWithValue("@OldId", lblHeaderId.Text)
                            myCmd.Parameters.AddWithValue("@NewID", newHeaderId)
                            myCmd.Parameters.AddWithValue("@OrderId", orderId)
                            myCmd.Parameters.AddWithValue("@Status", status)
                            myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())
                            myCmd.Parameters.AddWithValue("@InvoiceNumber", orderId)
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

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderQuotes VALUES(@NewID, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00)", thisConn)
                    myCmd.Parameters.AddWithValue("@NewID", newHeaderId)
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderHeaders", newHeaderId, Session("LoginId").ToString(), "Order Created | Rework Approved"}
            orderClass.Logs(dataLog)

            Dim itemRework As DataTable = orderClass.GetDataTable("SELECT ItemId FROM OrderReworkDetails WHERE ReworkId='" & lblReworkId.Text & "'")
            For i As Integer = 0 To itemRework.Rows.Count - 1
                Dim itemId As String = itemRework.Rows(i)("ItemId").ToString()
                Dim newIdDetail As String = orderClass.GetNewOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("sp_CopyOrderDetails", thisConn)
                        myCmd.CommandType = CommandType.StoredProcedure

                        myCmd.Parameters.AddWithValue("@ItemIdOld", itemId)
                        myCmd.Parameters.AddWithValue("@NewId", newIdDetail)
                        myCmd.Parameters.AddWithValue("@HeaderId", newHeaderId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(newHeaderId, newIdDetail)
                orderClass.CalculatePrice(newHeaderId, newIdDetail)
                orderClass.FinalCostItem(newHeaderId, newIdDetail)

                dataLog = {"OrderDetails", newIdDetail, Session("LoginId").ToString(), "Order Item Added | Rework Approved"}
                orderClass.Logs(dataLog)
            Next

            If lblCompanyId.Text = "2" Then
                Dim thisId As String = orderClass.GetNewOrderItemId()
                Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products WHERE Name='Fuel Surcharge' AND (Status='In Stock' OR Status='Limited Stock')")
                Dim productGroupId As String = orderClass.GetItemData("SELECT Id FROM PriceProductGroups WHERE Name='Fuel Surcharge' AND Active=1")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, Qty, Width, [Drop], TotalItems, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, 1, 0, 0, 1, 0, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@HeaderId", newHeaderId)
                        myCmd.Parameters.AddWithValue("@ProductId", If(String.IsNullOrEmpty(productId), CType(DBNull.Value, Object), productId))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(productGroupId), CType(DBNull.Value, Object), productGroupId))

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"OrderDetails", thisId, "2", "Order Item Added"}
                orderClass.Logs(dataLog)

                orderClass.ResetPriceDetail(newHeaderId, thisId)
                orderClass.CalculatePrice(newHeaderId, thisId)
                orderClass.FinalCostItem(newHeaderId, thisId)

                Dim totalItems As Integer = orderClass.GetTotalItemOrder(newHeaderId)
                If minSurcharge = True AndAlso totalItems <= 3 Then
                    thisId = orderClass.GetNewOrderItemId()
                    productId = orderClass.GetItemData("SELECT Id FROM Products WHERE Name='Minimum Order Surcharge' AND (Status='In Stock' OR Status='Limited Stock')")
                    productGroupId = orderClass.GetItemData("SELECT Id FROM PriceProductGroups WHERE Name='Minimum Order Surcharge' AND Active=1")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, Qty, Width, [Drop], TotalItems, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, 1, 0, 0, 1, 0, 1)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@HeaderId", newHeaderId)
                            myCmd.Parameters.AddWithValue("@ProductId", If(String.IsNullOrEmpty(productId), CType(DBNull.Value, Object), productId))
                            myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(productGroupId), CType(DBNull.Value, Object), productGroupId))

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"OrderDetails", thisId, "2", "Order Item Added"}
                    orderClass.Logs(dataLog)

                    orderClass.ResetPriceDetail(newHeaderId, thisId)
                    orderClass.CalculatePrice(newHeaderId, thisId)
                    orderClass.FinalCostItem(newHeaderId, thisId)
                End If
            End If

            orderClass.UpdateOrderFactory(newHeaderId)

            Dim mailingClass As New MailingClass
            mailingClass.ReworkApprove(lblReworkId.Text)

            If cashSale = True Then
                mailingClass.NewOrder_Proforma(newHeaderId)
            End If

            url = String.Format("~/order/detail?orderid={0}", newHeaderId)
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

    Protected Sub btnRejectRework_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworks SET Status='Rejected' WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblReworkId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            url = String.Format("~/order/rework/detail?reworkid={0}", lblReworkId.Text)
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

    Protected Sub btnAddItem_Click(sender As Object, e As EventArgs)
        MessageError_AddItem(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showAddItem(); };"
        Try
            Dim selectedItems As New List(Of Tuple(Of String, String, String, String))

            For Each row As GridViewRow In gvListAddItem.Rows
                Dim chk As CheckBox = CType(row.FindControl("chkSelect"), CheckBox)
                Dim ddlAddCategory As DropDownList = CType(row.FindControl("ddlAddCategory"), DropDownList)
                Dim txtAddInstallDate As TextBox = CType(row.FindControl("txtAddInstallDate"), TextBox)
                Dim txtAddDescription As TextBox = CType(row.FindControl("txtAddDescription"), TextBox)

                If chk IsNot Nothing AndAlso chk.Checked Then
                    Dim itemId As String = gvListAddItem.DataKeys(row.RowIndex).Value.ToString()
                    If ddlAddCategory Is Nothing OrElse String.IsNullOrWhiteSpace(ddlAddCategory.SelectedValue) Then
                        MessageError_AddItem(True, "CATEGORY IS REQUIRED !")
                        ClientScript.RegisterStartupScript(Me.GetType(), "showAddItem", thisScript, True)
                        Exit Sub
                    End If
                    If txtAddInstallDate Is Nothing OrElse String.IsNullOrWhiteSpace(txtAddInstallDate.Text) Then
                        MessageError_AddItem(True, "INSTALL DATE IS REQUIRED !")
                        ClientScript.RegisterStartupScript(Me.GetType(), "showAddItem", thisScript, True)
                        Exit Sub
                    End If
                    If txtAddDescription Is Nothing OrElse String.IsNullOrWhiteSpace(txtAddDescription.Text) Then
                        MessageError_AddItem(True, "DESCRIPTION IS REQUIRED !")
                        ClientScript.RegisterStartupScript(Me.GetType(), "showAddItem", thisScript, True)
                        Exit Sub
                    End If
                    selectedItems.Add(New Tuple(Of String, String, String, String)(itemId, ddlAddCategory.SelectedValue, txtAddInstallDate.Text, txtAddDescription.Text))
                End If
            Next

            If selectedItems.Count = 0 Then
                MessageError_AddItem(True, "ITEM IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showAddItem", thisScript, True)
                Exit Sub
            End If

            For Each item In selectedItems
                Dim reworkDetailId As String = orderClass.GetNewOrderReworkDetailId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderReworkDetails(Id, ReworkId, ItemId, Category, InstallDate, Description, Active) VALUES (@Id, @ReworkId, @ItemId, @Category, @InstallDate, @Description, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", reworkDetailId)
                        myCmd.Parameters.AddWithValue("@ReworkId", lblReworkId.Text)
                        myCmd.Parameters.AddWithValue("@ItemId", item.Item1)
                        myCmd.Parameters.AddWithValue("@Category", item.Item2)
                        myCmd.Parameters.AddWithValue("@InstallDate", Convert.ToDateTime(item.Item3))
                        myCmd.Parameters.AddWithValue("@Description", item.Item4)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Rework/{0}/{1}", lblReworkId.Text, reworkDetailId))

                If Not Directory.Exists(directoryOrder) Then
                    Directory.CreateDirectory(directoryOrder)
                End If
            Next
            url = String.Format("~/order/rework/detail?reworkid={0}", lblReworkId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_AddItem(True, ex.ToString)
            ClientScript.RegisterStartupScript(Me.GetType(), "showAddItem", thisScript, True)
            Exit Sub
        End Try
    End Sub

    Protected Sub UpdateItem_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtDetailId.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworkDetails SET Category=@Category, InstallDate=@InstallDate, Description=@Description WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue)
                    myCmd.Parameters.AddWithValue("@InstallDate", If(String.IsNullOrEmpty(txtInstallDate.Text), CType(DBNull.Value, Object), txtInstallDate.Text))
                    myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            url = String.Format("~/order/rework/detail?reworkid={0}", lblReworkId.Text)
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

    Protected Sub btnDeleteItem_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDeleteItem.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworkDetails SET Active=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim folderPath As String = Server.MapPath(String.Format("~/File/Rework/{0}/{1}", lblReworkId.Text, thisId))
            If Directory.Exists(folderPath) Then
                Directory.Delete(folderPath, True)
            End If

            url = String.Format("~/order/rework/detail?reworkid={0}", lblReworkId.Text)
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

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtUploadId.Text

            Dim folderPath As String = Server.MapPath(String.Format("~/File/Rework/{0}/{1}", lblReworkId.Text, thisId))

            If Not Directory.Exists(folderPath) Then
                Directory.CreateDirectory(folderPath)
            End If

            Dim fileName As String = Path.GetFileName(fuFile.FileName)
            fuFile.SaveAs(Path.Combine(folderPath, fileName))

            url = String.Format("~/order/rework/detail?reworkid={0}", lblReworkId.Text)
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

    Protected Sub DownloadZip_Command(sender As Object, e As CommandEventArgs)
        Try
            Dim itemId As String = e.CommandArgument.ToString()

            Dim stringPath As String = String.Format("~/File/Rework/{0}/{1}", lblReworkId.Text, itemId)
            Dim folderPath As String = Server.MapPath(stringPath)

            If Not Directory.Exists(folderPath) Then
                Exit Sub
            End If

            Dim zipName As String = "ReworkFiles_" & itemId & ".zip"
            Dim tempZip As String = Path.Combine(Path.GetTempPath(), zipName)

            If File.Exists(tempZip) Then
                File.Delete(tempZip)
            End If

            ZipFile.CreateFromDirectory(folderPath, tempZip)

            Response.Clear()
            Response.ContentType = "application/zip"
            Response.AddHeader("content-disposition", "attachment; filename=" & zipName)
            Response.TransmitFile(tempZip)
            Response.End()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub rptRework_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim row As DataRowView = CType(e.Item.DataItem, DataRowView)
                Dim itemId As String = row("Id").ToString()

                Dim gv As GridView = CType(e.Item.FindControl("gvFiles"), GridView)

                Dim stringPath As String = String.Format("~/File/Rework/{0}/{1}", lblReworkId.Text, itemId)

                Dim folderPath As String = Server.MapPath(stringPath)
                Dim dt As New DataTable()
                dt.Columns.Add("FileName")
                dt.Columns.Add("FilePath")

                If Directory.Exists(folderPath) Then
                    For Each filePath As String In Directory.GetFiles(folderPath)
                        Dim dr As DataRow = dt.NewRow()
                        dr("FileName") = Path.GetFileName(filePath)
                        Dim stringFilePath As String = String.Format("~/File/Rework/{0}/{1}/{2}", lblReworkId.Text, itemId, Path.GetFileName(filePath))
                        dr("FilePath") = ResolveUrl(stringFilePath)
                        dt.Rows.Add(dr)
                    Next
                End If

                gv.DataSource = dt
                gv.DataBind()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvFiles_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Try
            If e.CommandName = "DeleteFile" Then
                Dim filePath As String = e.CommandArgument.ToString()
                Dim fullPath As String = Server.MapPath(filePath)

                Try
                    If File.Exists(fullPath) Then
                        File.Delete(fullPath)
                    End If
                Catch ex As Exception
                End Try

                url = String.Format("~/order/rework/detail?reworkid={0}", lblReworkId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub BindDataRework(reworkId As String)
        Try
            Dim reworkData As DataRow = orderClass.GetDataRow("SELECT OrderReworks.*, Logins.FullName AS CreatedFullName, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, OrderHeaders.OrderNumber AS OrderNumber, OrderHeaders.OrderName AS OrderName, OrderHeaders.CustomerId AS CustomerId, OrderHeaders.OrderId FROM OrderReworks LEFT JOIN OrderHeaders ON OrderReworks.HeaderId=OrderHeaders.Id LEFT JOIN Logins ON OrderReworks.CreatedBy=Logins.Id LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderReworks.Id='" & reworkId & "'")
            If reworkData Is Nothing Then
                Response.Redirect("~/order/rework", False)
                Exit Sub
            End If

            lblHeaderId.Text = reworkData("HeaderId").ToString()
            lblCustomerName.Text = reworkData("CustomerName").ToString()
            lblCustomerId.Text = reworkData("CustomerId").ToString()
            lblCompanyId.Text = reworkData("CompanyId").ToString()
            lblOrderId.Text = reworkData("OrderId").ToString()
            lblOrderNumber.Text = reworkData("OrderNumber").ToString()
            lblOrderName.Text = reworkData("OrderName").ToString()
            lblStatus.Text = reworkData("Status").ToString()

            lblCreatedDate.Text = "-"
            If Not String.IsNullOrEmpty(reworkData("CreatedDate").ToString()) Then
                lblCreatedDate.Text = Convert.ToDateTime(reworkData("CreatedDate")).ToString("dd MMM yyyy")
            End If
            lblCreatedBy.Text = reworkData("CreatedFullName").ToString()

            rptRework.DataSource = orderClass.GetDataTable("SELECT OrderReworkDetails.*, 'Item : ' + OrderDetails.Room AS TitleItem FROM OrderReworkDetails LEFT JOIN OrderDetails ON OrderReworkDetails.ItemId=OrderDetails.Id WHERE OrderReworkDetails.ReworkId='" & reworkId & "' AND OrderReworkDetails.Active=1 ORDER BY Id ASC")
            rptRework.DataBind()

            BindAddItem(lblHeaderId.Text)

            aSubmitRework.Visible = False
            aDeleteRework.Visible = False
            aCancelRework.Visible = False
            aApproveRework.Visible = False
            aRejectRework.Visible = False
            aAddItem.Visible = False

            If lblStatus.Text = "Unsubmitted" Then
                If Session("RoleName") = "Developer" Then
                    aDeleteRework.Visible = True
                    aSubmitRework.Visible = True
                    aAddItem.Visible = True
                    If gvListAddItem.Rows.Count = 0 Then aAddItem.Visible = False
                End If
                If Session("RoleName") = "IT" Then
                    aDeleteRework.Visible = True
                    aSubmitRework.Visible = True
                End If
                If Session("RoleName") = "Factory Office" Then
                    aDeleteRework.Visible = True
                    aSubmitRework.Visible = True
                End If
                If Session("RoleName") = "Customer" Then
                    aDeleteRework.Visible = True
                    aSubmitRework.Visible = True
                    aAddItem.Visible = True
                    If gvListAddItem.Rows.Count = 0 Then aAddItem.Visible = False
                End If
                If Session("RoleName") = "Sales" Then
                    aDeleteRework.Visible = True
                    aSubmitRework.Visible = True
                    aAddItem.Visible = True
                    If gvListAddItem.Rows.Count = 0 Then aAddItem.Visible = False
                End If
            End If

            If lblStatus.Text = "Pending Approval" Then
                If Session("RoleName") = "Developer" Then
                    aApproveRework.Visible = True
                    aRejectRework.Visible = True
                End If
                If Session("RoleName") = "IT" Then
                    aApproveRework.Visible = True
                    aRejectRework.Visible = True
                End If
                If Session("RoleName") = "Factory Office" Then
                    aApproveRework.Visible = True
                    aRejectRework.Visible = True
                End If
                If Session("RoleName") = "Data Entry" Then
                    aApproveRework.Visible = True
                    aRejectRework.Visible = True
                End If
                If Session("RoleName") = "Sales" Then
                    aCancelRework.Visible = True
                    aSubmitRework.Visible = True
                End If
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

    Protected Sub BindAddItem(headerId As String)
        If Not String.IsNullOrEmpty(headerId) Then
            gvListAddItem.DataSource = orderClass.GetDataTable("SELECT OrderDetails.* FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN OrderReworks ON OrderReworks.HeaderId=OrderDetails.HeaderId AND OrderReworks.Status='Unsubmitted' AND OrderReworks.Active=1 LEFT JOIN OrderReworkDetails ON OrderReworkDetails.ItemId=OrderDetails.Id AND OrderReworkDetails.ReworkId=OrderReworks.Id AND OrderReworkDetails.Active=1 WHERE OrderDetails.HeaderId=" & headerId & " AND OrderDetails.Active=1 AND (Designs.Type='Blinds' OR Designs.Type='Shutters' OR Designs.Type='Doors' OR Designs.Type='Samples') AND OrderReworkDetails.ItemId IS NULL ORDER BY OrderDetails.Id ASC")
            gvListAddItem.DataBind()

            gvListAddItem.Columns(1).Visible = False ' ID
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then
                gvListAddItem.Columns(1).Visible = True ' ID
            End If
        End If
    End Sub

    Protected Sub AllMessageError(visible As Boolean, message As String)
        MessageError(visible, message)
        MessageError_AddItem(visible, message)
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
        divErrorB.Visible = visible : msgErrorB.InnerText = message
    End Sub

    Protected Sub MessageError_AddItem(visible As Boolean, message As String)
        divErrorAddItem.Visible = visible : msgErrorAddItem.InnerText = message
    End Sub

    Protected Function VisibleDetailRework() As Boolean
        If lblStatus.Text = "Unsubmitted" Then Return True
        Return False
    End Function

    Protected Function VisibleDownloadZip() As Boolean
        If lblStatus.Text = "Pending Approval" OrElse lblStatus.Text = "Approved" Then Return True
        Return False
    End Function

    Protected Function BindProductDescription(itemId As String) As String
        Return orderClass.GetProductDescription(itemId)
    End Function

    Protected Function CreateReworkZip(thisId As String) As String
        Try
            Dim itemRework As DataTable = orderClass.GetDataTable("SELECT * FROM OrderReworkDetails WHERE ReworkId='" & thisId & "' AND Active=1 ORDER BY Id ASC")
            If itemRework Is Nothing OrElse itemRework.Rows.Count = 0 Then
                Return String.Empty
            End If

            Dim zipFileName As String = String.Format("Rework_{0}_{1}.zip", thisId, DateTime.Now.ToString("yyyyMMddHHmmss"))
            Dim zipFilePath As String = HttpContext.Current.Server.MapPath("~/File/Rework/Zip/" & zipFileName)

            Dim zipFolder As String = Path.GetDirectoryName(zipFilePath)
            If Not Directory.Exists(zipFolder) Then
                Directory.CreateDirectory(zipFolder)
            End If

            Dim tempFolder As String = HttpContext.Current.Server.MapPath("~/File/Rework/Temp/" & thisId)
            If Directory.Exists(tempFolder) Then
                Directory.Delete(tempFolder, True)
            End If
            Directory.CreateDirectory(tempFolder)

            For Each row As DataRow In itemRework.Rows
                Dim id As String = row("Id").ToString()
                Dim sourceFolder As String = HttpContext.Current.Server.MapPath(String.Format("~/File/Rework/{0}", id))
                If Directory.Exists(sourceFolder) Then
                    Dim destFolder As String = Path.Combine(tempFolder, id)
                    My.Computer.FileSystem.CopyDirectory(sourceFolder, destFolder)
                End If
            Next

            ZipFile.CreateFromDirectory(tempFolder, zipFilePath, CompressionLevel.Fastest, False)
            Directory.Delete(tempFolder, True)

            Return zipFileName
        Catch ex As Exception
            Return String.Empty
        End Try
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