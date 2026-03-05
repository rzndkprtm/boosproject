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
        Dim pageAccess As Boolean = PageAction("Load")
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

    Protected Sub btnCancelRework_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworks SET Status='Canceled' WHERE Id=@Id", thisConn)
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

    Protected Sub btnSubmitRework_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim itemRework As DataTable = orderClass.GetDataTable("SELECT * FROM OrderReworkDetails WHERE ReworkId='" & lblReworkId.Text & "' AND Active=1 ORDER BY Id ASC")
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

            Dim orderType As String = orderClass.GetItemData("SELECT OrderType FROM OrderHeaders WHERE Id='" & lblHeaderId.Text & "'")

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

                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders SELECT @NewID, @OrderId, CustomerId, CONVERT(VARCHAR(200), OrderNumber) + ' - ' + 'RW', CONVERT(VARCHAR(200), OrderName) + ' - ' + 'RW', NULL, OrderType, @Status, NULL, CreatedBy, GETDATE(), NULL, NULL, NULL, NULL, NULL, NULL, 0, 1 FROM OrderHeaders WHERE Id=@OldId;", thisConn)
                            myCmd.Parameters.AddWithValue("@OldId", lblHeaderId.Text)
                            myCmd.Parameters.AddWithValue("@NewID", newHeaderId)
                            myCmd.Parameters.AddWithValue("@OrderId", orderId)
                            myCmd.Parameters.AddWithValue("@Status", status)
                            myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())

                            myCmd.ExecuteNonQuery()
                        End Using

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

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderQuotes VALUES(@NewID, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00); INSERT OrderInvoices(Id, InvoiceNumber, Payment, Amount) VALUES (@NewID, @InvoiceNumber, @Payment, 0);", thisConn)
                    myCmd.Parameters.AddWithValue("@NewID", newHeaderId)
                    myCmd.Parameters.AddWithValue("@InvoiceNumber", orderId)
                    myCmd.Parameters.AddWithValue("@Payment", False)

                    myCmd.ExecuteNonQuery()
                End Using

                If orderType = "Builder" Then
                    Using myCmd As New SqlCommand("INSERT INTO OrderBuilders(Id) VALUES (@Id)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", newHeaderId)

                        myCmd.ExecuteNonQuery()
                    End Using
                End If

                thisConn.Close()
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

            Dim totalItems As Integer = orderClass.GetTotalItemOrder(newHeaderId)
            If lblCompanyId.Text = "2" AndAlso minSurcharge = True AndAlso totalItems <= 3 Then
                Dim thisId As String = orderClass.GetNewOrderItemId()
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, Qty, Width, [Drop], TotalItems, MarkUp, Active) VALUES (@Id, @HeaderId, 2986, 112, 1, 0, 0, 1, 0, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@HeaderId", newHeaderId)

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

    Protected Sub rptRework_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
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
    End Sub

    Protected Sub gvFiles_RowCommand(sender As Object, e As GridViewCommandEventArgs)
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
    End Sub

    Protected Sub UpdateItem_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtDetailId.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworkDetails SET Category=@Category, Description=@Description WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue)
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

            Dim folderPath As String = Server.MapPath("~/File/Rework/" & thisId)
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

    Protected Sub BindDataRework(reworkId As String)
        Try
            Dim reworkData As DataRow = orderClass.GetDataRow("SELECT OrderReworks.*, CustomerLogins.FullName AS CreatedFullName, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, OrderHeaders.OrderNumber AS OrderNumber, OrderHeaders.OrderName AS OrderName, OrderHeaders.CustomerId AS CustomerId, OrderHeaders.OrderId FROM OrderReworks LEFT JOIN OrderHeaders ON OrderReworks.HeaderId=OrderHeaders.Id LEFT JOIN CustomerLogins ON OrderReworks.CreatedBy=CustomerLogins.Id LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderReworks.Id='" & reworkId & "'")
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

            rptRework.DataSource = orderClass.GetDataTable("SELECT OrderReworkDetails.*, 'Item ID ' + CONVERT(VARCHAR, OrderDetails.Id) + ' - ' + OrderDetails.Room AS TitleItem FROM OrderReworkDetails LEFT JOIN OrderDetails ON OrderReworkDetails.ItemId=OrderDetails.Id WHERE OrderReworkDetails.ReworkId='" & reworkId & "' AND OrderReworkDetails.Active=1 ORDER BY Id ASC")
            rptRework.DataBind()

            aCancelRework.Visible = False
            aSubmitRework.Visible = False
            aApproveRework.Visible = False
            aRejectRework.Visible = False

            If lblStatus.Text = "Unsubmitted" Then
                If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Customer" Then
                    aCancelRework.Visible = True
                    aSubmitRework.Visible = True
                End If
            End If

            If lblStatus.Text = "Pending Approval" Then
                If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then
                    aApproveRework.Visible = True
                    aRejectRework.Visible = True
                End If
                If Session("RoleName") = "Customer Service" Then
                    aApproveRework.Visible = True
                    aRejectRework.Visible = True
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

    Protected Sub AllMessageError(visible As Boolean, message As String)
        MessageError(visible, message)
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
        divErrorB.Visible = visible : msgErrorB.InnerText = message
    End Sub

    Protected Function VisibleDetailRework() As Boolean
        If lblStatus.Text = "Unsubmitted" Then Return True
        Return False
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
