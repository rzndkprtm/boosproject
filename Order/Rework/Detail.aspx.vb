Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.IO.Compression

Partial Class Order_Rework_Detail
    Inherits Page

    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnCancelRework_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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

                Dim folderPath As String = Server.MapPath(String.Format("~/File/Rework/{0}", id))

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

                Dim filePath As String = "~/File/Rework/Zip/"
                Dim fileName As String = CreateReworkZip(lblReworkId.Text)
                Dim finalFilePath As String = Server.MapPath(filePath & fileName)

                mailingClass.ReworkOrder(lblReworkId.Text, finalFilePath)

                Dim dataLog As Object() = {"OrderReworks", lblReworkId.Text, Session("LoginId").ToString(), "Rework Submitted"}
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnSubmitRework_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnApproveRework_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim newHeaderId As String = orderClass.GetNewOrderHeaderId()

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
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders(Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, Status, CreatedBy, CreatedDate, DownloadBOE, Active) SELECT @NewHeaderId, @OrderId, OrderHeaders.CustomerId, CONVERT(VARCHAR, OrderHeaders.OrderNumber) + ' - RW', CONVERT(VARCHAR, OrderHeaders.OrderName) + ' - RW', NULL, 'Approved Rework', OrderHeaders.CreatedBy, GETDATE(), 0, 1 FROM OrderReworks LEFT JOIN OrderHeaders ON OrderReworks.HeaderId=OrderHeaders.Id WHERE OrderReworks.Id=@ReworkId; INSERT INTO OrderQuotes VALUES(@NewHeaderId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
                            myCmd.Parameters.AddWithValue("@NewHeaderId", newHeaderId)
                            myCmd.Parameters.AddWithValue("@ReworkId", lblReworkId.Text)
                            myCmd.Parameters.AddWithValue("@OrderId", orderId)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using
                Catch exSql As SqlException
                    If exSql.Number = 2601 OrElse exSql.Number = 2627 Then
                        success = False
                    Else
                        Throw
                    End If
                End Try
            Loop



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

            mailingClass.ReworkApprove(lblReworkId.Text)

            url = String.Format("~/order/detail?orderid={0}", newHeaderId)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnSubmitRework_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnRejectRework_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnAddItem_Click(sender As Object, e As EventArgs)
        url = String.Format("~/order/detail?orderid={0}", lblHeaderId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub rptRework_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim row As DataRowView = CType(e.Item.DataItem, DataRowView)
            Dim itemId As String = row("Id").ToString()

            Dim gv As GridView = CType(e.Item.FindControl("gvFiles"), GridView)

            Dim folderPath As String = Server.MapPath("~/File/Rework/" & itemId)
            Dim dt As New DataTable()
            dt.Columns.Add("FileName")
            dt.Columns.Add("FilePath")

            If Directory.Exists(folderPath) Then
                For Each filePath As String In Directory.GetFiles(folderPath)
                    Dim dr As DataRow = dt.NewRow()
                    dr("FileName") = Path.GetFileName(filePath)
                    dr("FilePath") = ResolveUrl("~/File/Rework/" & itemId & "/" & Path.GetFileName(filePath))
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

    Protected Sub btnCategory_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtCategoryId.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworkDetails SET Category=@Category WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue)

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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnCategory_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnDescription_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtDescriptionId.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworkDetails SET Description=@Description WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDescription_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtUploadId.Text

            Dim folderPath As String = Server.MapPath(String.Format("~/File/Rework/{0}", thisId))

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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnUpload_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDeleteItem_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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
            btnAddItem.Visible = False

            If lblStatus.Text = "Unsubmitted" Then
                If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Customer" Then
                    aCancelRework.Visible = True
                    aSubmitRework.Visible = True
                    btnAddItem.Visible = True
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataRework", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub AllMessageError(visible As Boolean, message As String)
        MessageError(visible, message)
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
