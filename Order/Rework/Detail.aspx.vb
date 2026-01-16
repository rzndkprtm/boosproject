Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.IO.Compression

Partial Class Order_Rework_Detail
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
    Dim dataLog As Object() = Nothing

    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

    Dim reworkId As String = String.Empty
    Dim url As String = String.Empty

    Private Property headerId As String
        Get
            Return If(ViewState("headerId"), String.Empty)
        End Get
        Set(value As String)
            ViewState("headerId") = value
        End Set
    End Property

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

        reworkId = Request.QueryString("reworkid").ToString()
        If Not IsPostBack Then
            AllMessageError(False, String.Empty)

            BindDataRework(reworkId)
        End If
    End Sub

    Protected Sub btnCancelRework_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = reworkId

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworks SET Status='Canceled' WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
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
            Dim thisId As String = reworkId

            Dim itemRework As DataSet = orderClass.GetListData("SELECT * FROM OrderReworkDetails WHERE ReworkId='" & thisId & "' AND Active=1 ORDER BY Id ASC")
            For i As Integer = 0 To itemRework.Tables(0).Rows.Count - 1
                Dim id As String = itemRework.Tables(0).Rows(i).Item("Id").ToString()
                Dim itemId As String = itemRework.Tables(0).Rows(i).Item("ItemId").ToString()
                Dim category As String = itemRework.Tables(0).Rows(i).Item("Category").ToString()
                Dim description As String = itemRework.Tables(0).Rows(i).Item("Description").ToString()

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
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim filePath As String = "~/File/Rework/Zip/"
                Dim fileName As String = CreateReworkZip(thisId)
                Dim finalFilePath As String = Server.MapPath(filePath & fileName)

                mailingClass.ReworkOrder(thisId, finalFilePath)

                Dim dataLog As Object() = {"OrderReworks", thisId, Session("LoginId").ToString(), "Rework Submitted"}
                orderClass.Logs(dataLog)

                url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
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
            Dim thisId As String = reworkId
            Dim newHeaderId As String = orderClass.GetNewOrderHeaderId()

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworks SET Status='Approved', HeaderIdNew=@HeaderIdNew WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@HeaderIdNew", newHeaderId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim companyAlias As String = orderClass.GetCompanyAliasByCustomer(lblCustomerId.Text)
            Dim orderId As String = String.Format("{0}-{1}", companyAlias, newHeaderId)

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders(Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, Status, CreatedBy, CreatedDate, DownloadBOE, Active) SELECT @NewHeaderId, @OrderId, OrderHeaders.CustomerId, CONVERT(VARCHAR, OrderHeaders.OrderNumber) + ' - RW', CONVERT(VARCHAR, OrderHeaders.OrderName) + ' - RW', NULL, 'Approved Rework', OrderHeaders.CreatedBy, GETDATE(), 0, 1 FROM OrderReworks LEFT JOIN OrderHeaders ON OrderReworks.HeaderId=OrderHeaders.Id WHERE OrderReworks.Id=@ReworkId; INSERT INTO OrderQuotes VALUES(@NewHeaderId, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
                    myCmd.Parameters.AddWithValue("@NewHeaderId", newHeaderId)
                    myCmd.Parameters.AddWithValue("@ReworkId", reworkId)
                    myCmd.Parameters.AddWithValue("@OrderId", orderId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"OrderHeaders", newHeaderId, Session("LoginId").ToString(), "Order Created | Rework Approved"}
            orderClass.Logs(dataLog)

            Dim itemRework As DataSet = orderClass.GetListData("SELECT ItemId FROM OrderReworkDetails WHERE ReworkId='" & reworkId & "'")
            For i As Integer = 0 To itemRework.Tables(0).Rows.Count - 1
                Dim itemId As String = itemRework.Tables(0).Rows(i).Item("ItemId").ToString()
                Dim newIdDetail As String = orderClass.GetNewOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails SELECT @NewId, @HeaderId, OrderDetails.ProductId, OrderDetails.FabricId, OrderDetails.FabricIdB, OrderDetails.FabricIdC, OrderDetails.FabricIdD, OrderDetails.FabricIdE, OrderDetails.FabricIdF, OrderDetails.FabricIdG, OrderDetails.FabricIdH, OrderDetails.FabricColourId, OrderDetails.FabricColourIdB, OrderDetails.FabricColourIdC, OrderDetails.FabricColourIdD, OrderDetails.FabricColourIdE, OrderDetails.FabricColourIdF, OrderDetails.FabricColourIdG, OrderDetails.FabricColourIdH, OrderDetails.ChainId, OrderDetails.ChainIdB, OrderDetails.ChainIdC, OrderDetails.ChainIdD, OrderDetails.ChainIdE, OrderDetails.ChainIdF, OrderDetails.ChainIdG, OrderDetails.ChainIdH, OrderDetails.BottomId, OrderDetails.BottomIdB, OrderDetails.BottomIdC, OrderDetails.BottomIdD, OrderDetails.BottomIdE, OrderDetails.BottomIdF, OrderDetails.BottomIdG, OrderDetails.BottomIdH, OrderDetails.BottomColourId, OrderDetails.BottomColourIdB, OrderDetails.BottomColourIdC, OrderDetails.BottomColourIdD, OrderDetails.BottomColourIdE, OrderDetails.BottomColourIdF, OrderDetails.BottomColourIdG, OrderDetails.BottomColourIdH, OrderDetails.PriceProductGroupId, OrderDetails.PriceProductGroupIdB, OrderDetails.PriceProductGroupIdC, OrderDetails.PriceProductGroupIdD, OrderDetails.PriceProductGroupIdE, OrderDetails.PriceProductGroupIdF, OrderDetails.PriceProductGroupIdG, OrderDetails.PriceProductGroupIdH, OrderDetails.PriceAdditional, OrderDetails.PriceAdditionalB, OrderDetails.SubType, OrderDetails.Qty, OrderDetails.QtyBlade, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.SemiInsideMount, OrderDetails.Width, OrderDetails.WidthB, OrderDetails.WidthC, OrderDetails.WidthD, OrderDetails.WidthE, OrderDetails.WidthF, OrderDetails.WidthG, OrderDetails.WidthH, OrderDetails.[Drop], OrderDetails.DropB, OrderDetails.DropC, OrderDetails.DropD, OrderDetails.DropE, OrderDetails.DropF, OrderDetails.DropG, OrderDetails.DropH, OrderDetails.Heading, OrderDetails.HeadingB, OrderDetails.StackPosition, OrderDetails.StackPositionB, OrderDetails.ControlPosition, OrderDetails.ControlPositionB, OrderDetails.ControlPositionC, OrderDetails.ControlPositionD, OrderDetails.ControlPositionE, OrderDetails.ControlPositionF, OrderDetails.ControlPositionG, OrderDetails.ControlPositionH, OrderDetails.Roll, OrderDetails.RollB, OrderDetails.RollC, OrderDetails.RollD, OrderDetails.RollE, OrderDetails.RollF, OrderDetails.RollG, OrderDetails.RollH, OrderDetails.TilterPosition, OrderDetails.TilterPositionB, OrderDetails.TilterPositionC, OrderDetails.Charger, OrderDetails.ExtensionCable, OrderDetails.ChainStopper, OrderDetails.ChainStopperB, OrderDetails.ChainStopperC, OrderDetails.ChainStopperD, OrderDetails.ChainStopperE, OrderDetails.ChainStopperF, OrderDetails.ChainStopperG, OrderDetails.ChainStopperH, OrderDetails.FlatOption, OrderDetails.FlatOptionB, OrderDetails.FlatOptionC, OrderDetails.FlatOptionD, OrderDetails.FlatOptionE, OrderDetails.FlatOptionF, OrderDetails.FlatOptionG, OrderDetails.FlatOptionH, OrderDetails.WandColour, OrderDetails.WandColourB, OrderDetails.ControlColour, OrderDetails.ControlColourB, OrderDetails.HingeColour, OrderDetails.HingeQtyPerPanel, OrderDetails.PanelQty, OrderDetails.PanelQtyWithHinge, OrderDetails.SameSizePanel, OrderDetails.TrackType, OrderDetails.TrackTypeB, OrderDetails.TrackColour, OrderDetails.TrackColourB, OrderDetails.TrackDraw, OrderDetails.TrackDrawB, OrderDetails.TopTrack, OrderDetails.BottomTrack, OrderDetails.BottomTrackQty, OrderDetails.TopTrackQty, OrderDetails.TrackQty, OrderDetails.TrackLength, OrderDetails.LayoutCode, OrderDetails.LayoutCodeCustom, OrderDetails.LouvreSize, OrderDetails.LouvrePosition, OrderDetails.FrameType, OrderDetails.FrameLeft, OrderDetails.FrameRight, OrderDetails.FrameTop, OrderDetails.FrameBottom, OrderDetails.BottomTrackType, OrderDetails.BottomTrackRecess, OrderDetails.Buildout, OrderDetails.BuildoutPosition, OrderDetails.ControlLength, OrderDetails.ControlLengthB, OrderDetails.ControlLengthC, OrderDetails.ControlLengthD, OrderDetails.ControlLengthE, OrderDetails.ControlLengthF, OrderDetails.ControlLengthG, OrderDetails.ControlLengthH, OrderDetails.ControlLengthValue, OrderDetails.ControlLengthValueB, OrderDetails.ControlLengthValueC, OrderDetails.ControlLengthValueD, OrderDetails.ControlLengthValueE, OrderDetails.ControlLengthValueF, OrderDetails.ControlLengthValueG, OrderDetails.ControlLengthValueH, OrderDetails.WandLength, OrderDetails.WandLengthValue, OrderDetails.WandLengthB, OrderDetails.WandLengthValueB, OrderDetails.HandleType, OrderDetails.HandleLength, OrderDetails.TripleLock, OrderDetails.MidrailPosition, OrderDetails.MidrailHeight1, OrderDetails.MidrailHeight2, OrderDetails.MidrailCritical, OrderDetails.BugSeal, OrderDetails.PetType, OrderDetails.PetPosition, OrderDetails.DoorCloser, OrderDetails.Brace, OrderDetails.AngleType, OrderDetails.AngleLength, OrderDetails.AngleQty, OrderDetails.FabricInsert, OrderDetails.Supply, OrderDetails.Tassel, OrderDetails.CustomHeaderLength, OrderDetails.BracketType, OrderDetails.BracketSize, OrderDetails.BracketExtension, OrderDetails.Sloping, OrderDetails.IsBlindIn, OrderDetails.Batten, OrderDetails.BattenB, OrderDetails.BottomJoining, OrderDetails.BottomHem, OrderDetails.ValanceOption, OrderDetails.ValanceType, OrderDetails.ValanceSize, OrderDetails.ValanceSizeValue, OrderDetails.ReturnPosition, OrderDetails.ReturnLength, OrderDetails.ReturnLengthValue, OrderDetails.ReturnLengthB, OrderDetails.ReturnLengthValueB, OrderDetails.SpringAssist, OrderDetails.Adjusting, NULL, NULL, NULL, OrderDetails.PrintingD, NULL, NULL, NULL, NULL, OrderDetails.Gap1, OrderDetails.Gap2, OrderDetails.Gap3, OrderDetails.Gap4, OrderDetails.Gap5, OrderDetails.Beading, OrderDetails.JambType, OrderDetails.JambPosition, OrderDetails.FlushBold, OrderDetails.PortHole, OrderDetails.InterlockType, OrderDetails.Receiver, OrderDetails.ReceiverLength, OrderDetails.SlidingQty, OrderDetails.PlungerPin, OrderDetails.SwivelColour, OrderDetails.SwivelQty, OrderDetails.SwivelQtyB, OrderDetails.SpringQty, OrderDetails.TopPlasticQty, OrderDetails.HorizontalTPost, OrderDetails.HorizontalTPostHeight, OrderDetails.JoinedPanels, OrderDetails.ReverseHinged, OrderDetails.PelmetFlat, OrderDetails.ExtraFascia, OrderDetails.HingesLoose, OrderDetails.TiltrodType, OrderDetails.TiltrodSplit, OrderDetails.SplitHeight1, OrderDetails.SplitHeight2, OrderDetails.DoorCutOut, OrderDetails.SpecialShape, OrderDetails.TemplateProvided, OrderDetails.LinearMetre, OrderDetails.LinearMetreB, OrderDetails.LinearMetreC, OrderDetails.LinearMetreD, OrderDetails.LinearMetreE, OrderDetails.LinearMetreF, OrderDetails.LinearMetreG, OrderDetails.SquareMetre, OrderDetails.SquareMetreB, OrderDetails.SquareMetreC, OrderDetails.SquareMetreD, OrderDetails.SquareMetreE, OrderDetails.SquareMetreF, OrderDetails.SquareMetreG, OrderDetails.SquareMetreH, OrderDetails.TotalItems, OrderDetails.Notes, OrderDetails.MarkUp, OrderDetails.Active FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id=@ItemIdOld AND Products.DesignId<>'16';", thisConn)
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

            mailingClass.ReworkApprove(thisId)

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
            Dim thisId As String = reworkId

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderReworks SET Status='Rejected' WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
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
        url = String.Format("~/order/detail?orderid={0}", headerId)
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

            url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
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

            url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
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

            url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
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

            url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
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

            url = String.Format("~/order/rework/detail?reworkid={0}", reworkId)
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
            Dim reworkData As DataSet = orderClass.GetListData("SELECT OrderReworks.*, CustomerLogins.FullName AS CreatedFullName, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, OrderHeaders.OrderNumber AS OrderNumber, OrderHeaders.OrderName AS OrderName, OrderHeaders.CustomerId AS CustomerId, OrderHeaders.OrderId FROM OrderReworks LEFT JOIN OrderHeaders ON OrderReworks.HeaderId=OrderHeaders.Id LEFT JOIN CustomerLogins ON OrderReworks.CreatedBy=CustomerLogins.Id LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderReworks.Id='" & reworkId & "'")
            If reworkData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/order/rework", False)
                Exit Sub
            End If

            headerId = reworkData.Tables(0).Rows(0).Item("HeaderId").ToString()
            lblCustomerName.Text = reworkData.Tables(0).Rows(0).Item("CustomerName").ToString()
            lblCustomerId.Text = reworkData.Tables(0).Rows(0).Item("CustomerId").ToString()
            lblCompanyId.Text = reworkData.Tables(0).Rows(0).Item("CompanyId").ToString()
            lblOrderId.Text = reworkData.Tables(0).Rows(0).Item("OrderId").ToString()
            lblOrderNumber.Text = reworkData.Tables(0).Rows(0).Item("OrderNumber").ToString()
            lblOrderName.Text = reworkData.Tables(0).Rows(0).Item("OrderName").ToString()
            lblStatus.Text = reworkData.Tables(0).Rows(0).Item("Status").ToString()

            lblCreatedDate.Text = "-"
            If Not String.IsNullOrEmpty(reworkData.Tables(0).Rows(0).Item("CreatedDate").ToString()) Then
                lblCreatedDate.Text = Convert.ToDateTime(reworkData.Tables(0).Rows(0).Item("CreatedDate")).ToString("dd MMM yyyy")
            End If
            lblCreatedBy.Text = reworkData.Tables(0).Rows(0).Item("CreatedFullName").ToString()

            rptRework.DataSource = orderClass.GetListData("SELECT OrderReworkDetails.*, 'Item ID ' + CONVERT(VARCHAR, OrderDetails.Id) + ' - ' + OrderDetails.Room AS TitleItem FROM OrderReworkDetails LEFT JOIN OrderDetails ON OrderReworkDetails.ItemId=OrderDetails.Id WHERE OrderReworkDetails.ReworkId='" & reworkId & "' AND OrderReworkDetails.Active=1 ORDER BY Id ASC")
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
            Dim itemRework As DataSet = orderClass.GetListData("SELECT * FROM OrderReworkDetails WHERE ReworkId='" & thisId & "' AND Active=1 ORDER BY Id ASC")
            If itemRework Is Nothing OrElse itemRework.Tables(0).Rows.Count = 0 Then
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

            For Each row As DataRow In itemRework.Tables(0).Rows
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
