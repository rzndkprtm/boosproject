Imports System.Data
Imports System.Data.SqlClient

Partial Class Order_Default
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
    Dim url As String = String.Empty

    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

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
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = orderClass.GetDataTable("SELECT * FROM Logs WHERE Type='OrderHeaders' AND DataId='" & dataId & "' ORDER BY ActionDate ASC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError_Log(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkLog_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
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
                Dim orderId As String = ""

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
                            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails SELECT @NewId, @HeaderId, OrderDetails.ProductId, OrderDetails.FabricId, OrderDetails.FabricIdB, OrderDetails.FabricIdC, OrderDetails.FabricIdD, OrderDetails.FabricIdE, OrderDetails.FabricIdF, OrderDetails.FabricIdG, OrderDetails.FabricIdH, OrderDetails.FabricColourId, OrderDetails.FabricColourIdB, OrderDetails.FabricColourIdC, OrderDetails.FabricColourIdD, OrderDetails.FabricColourIdE, OrderDetails.FabricColourIdF, OrderDetails.FabricColourIdG, OrderDetails.FabricColourIdH, OrderDetails.ChainId, OrderDetails.ChainIdB, OrderDetails.ChainIdC, OrderDetails.ChainIdD, OrderDetails.ChainIdE, OrderDetails.ChainIdF, OrderDetails.ChainIdG, OrderDetails.ChainIdH, OrderDetails.BottomId, OrderDetails.BottomIdB, OrderDetails.BottomIdC, OrderDetails.BottomIdD, OrderDetails.BottomIdE, OrderDetails.BottomIdF, OrderDetails.BottomIdG, OrderDetails.BottomIdH, OrderDetails.BottomColourId, OrderDetails.BottomColourIdB, OrderDetails.BottomColourIdC, OrderDetails.BottomColourIdD, OrderDetails.BottomColourIdE, OrderDetails.BottomColourIdF, OrderDetails.BottomColourIdG, OrderDetails.BottomColourIdH, OrderDetails.PriceProductGroupId, OrderDetails.PriceProductGroupIdB, OrderDetails.PriceProductGroupIdC, OrderDetails.PriceProductGroupIdD, OrderDetails.PriceProductGroupIdE, OrderDetails.PriceProductGroupIdF, OrderDetails.PriceProductGroupIdG, OrderDetails.PriceProductGroupIdH, OrderDetails.PriceAdditional, OrderDetails.PriceAdditionalB, OrderDetails.SubType, OrderDetails.Qty, OrderDetails.QtyBlade, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.SemiInsideMount, OrderDetails.Width, OrderDetails.WidthB, OrderDetails.WidthC, OrderDetails.WidthD, OrderDetails.WidthE, OrderDetails.WidthF, OrderDetails.WidthG, OrderDetails.WidthH, OrderDetails.[Drop], OrderDetails.DropB, OrderDetails.DropC, OrderDetails.DropD, OrderDetails.DropE, OrderDetails.DropF, OrderDetails.DropG, OrderDetails.DropH, OrderDetails.Heading, OrderDetails.HeadingB, OrderDetails.StackPosition, OrderDetails.StackPositionB, OrderDetails.ControlPosition, OrderDetails.ControlPositionB, OrderDetails.ControlPositionC, OrderDetails.ControlPositionD, OrderDetails.ControlPositionE, OrderDetails.ControlPositionF, OrderDetails.ControlPositionG, OrderDetails.ControlPositionH, OrderDetails.Roll, OrderDetails.RollB, OrderDetails.RollC, OrderDetails.RollD, OrderDetails.RollE, OrderDetails.RollF, OrderDetails.RollG, OrderDetails.RollH, OrderDetails.TilterPosition, OrderDetails.TilterPositionB, OrderDetails.TilterPositionC, OrderDetails.Charger, OrderDetails.ExtensionCable, OrderDetails.ChainStopper, OrderDetails.ChainStopperB, OrderDetails.ChainStopperC, OrderDetails.ChainStopperD, OrderDetails.ChainStopperE, OrderDetails.ChainStopperF, OrderDetails.ChainStopperG, OrderDetails.ChainStopperH, OrderDetails.FlatOption, OrderDetails.FlatOptionB, OrderDetails.FlatOptionC, OrderDetails.FlatOptionD, OrderDetails.FlatOptionE, OrderDetails.FlatOptionF, OrderDetails.FlatOptionG, OrderDetails.FlatOptionH, OrderDetails.WandColour, OrderDetails.WandColourB, OrderDetails.ControlColour, OrderDetails.ControlColourB, OrderDetails.HingeColour, OrderDetails.HingeQtyPerPanel, OrderDetails.PanelQty, OrderDetails.PanelQtyWithHinge, OrderDetails.SameSizePanel, OrderDetails.TrackType, OrderDetails.TrackTypeB, OrderDetails.TrackColour, OrderDetails.TrackColourB, OrderDetails.TrackDraw, OrderDetails.TrackDrawB, OrderDetails.TopTrack, OrderDetails.TopTrackLength, OrderDetails.BottomTrack, OrderDetails.BottomTrackLength, OrderDetails.TrackQty, OrderDetails.TrackLength, OrderDetails.LayoutCode, OrderDetails.LayoutCodeCustom, OrderDetails.LouvreSize, OrderDetails.LouvrePosition, OrderDetails.MeshType, OrderDetails.FrameType, OrderDetails.FrameColour, OrderDetails.FrameLeft, OrderDetails.FrameRight, OrderDetails.FrameTop, OrderDetails.FrameBottom, OrderDetails.BottomTrackType, OrderDetails.BottomTrackRecess, OrderDetails.Buildout, OrderDetails.BuildoutPosition, OrderDetails.ControlLength, OrderDetails.ControlLengthB, OrderDetails.ControlLengthC, OrderDetails.ControlLengthD, OrderDetails.ControlLengthE, OrderDetails.ControlLengthF, OrderDetails.ControlLengthG, OrderDetails.ControlLengthH, OrderDetails.ControlLengthValue, OrderDetails.ControlLengthValueB, OrderDetails.ControlLengthValueC, OrderDetails.ControlLengthValueD, OrderDetails.ControlLengthValueE, OrderDetails.ControlLengthValueF, OrderDetails.ControlLengthValueG, OrderDetails.ControlLengthValueH, OrderDetails.WandLength, OrderDetails.WandLengthValue, OrderDetails.WandLengthB, OrderDetails.WandLengthValueB, OrderDetails.HandleType, OrderDetails.HandleLength, OrderDetails.TripleLock, OrderDetails.MidrailPosition, OrderDetails.MidrailHeight1, OrderDetails.MidrailHeight2, OrderDetails.MidrailCritical, OrderDetails.BugSeal, OrderDetails.PetType, OrderDetails.PetPosition, OrderDetails.DoorCloser, OrderDetails.Brace, OrderDetails.AngleType, OrderDetails.AngleLength, OrderDetails.AngleQty, OrderDetails.FabricInsert, OrderDetails.Supply, OrderDetails.Tassel, OrderDetails.CustomHeaderLength, OrderDetails.BracketType, OrderDetails.BracketSize, OrderDetails.BracketExtension, OrderDetails.Sloping, OrderDetails.IsBlindIn, OrderDetails.Batten, OrderDetails.BattenB, OrderDetails.BottomJoining, OrderDetails.BottomHem, OrderDetails.ValanceOption, OrderDetails.ValanceType, OrderDetails.ValanceSize, OrderDetails.ValanceSizeValue, OrderDetails.ReturnPosition, OrderDetails.ReturnLength, OrderDetails.ReturnLengthValue, OrderDetails.ReturnLengthB, OrderDetails.ReturnLengthValueB, OrderDetails.SpringAssist, OrderDetails.Adjusting, NULL, NULL, NULL, OrderDetails.PrintingD, NULL, NULL, NULL, NULL, OrderDetails.Gap1, OrderDetails.Gap2, OrderDetails.Gap3, OrderDetails.Gap4, OrderDetails.Gap5, OrderDetails.Beading, OrderDetails.JambType, OrderDetails.JambPosition, OrderDetails.FlushBold, OrderDetails.PortHole, OrderDetails.InterlockType, OrderDetails.Receiver, OrderDetails.ReceiverLength, OrderDetails.SlidingQty, OrderDetails.PlungerPin, OrderDetails.SwivelColour, OrderDetails.SwivelQty, OrderDetails.SwivelQtyB, OrderDetails.SpringQty, OrderDetails.TopPlasticQty, OrderDetails.HorizontalTPost, OrderDetails.HorizontalTPostHeight, OrderDetails.JoinedPanels, OrderDetails.ReverseHinged, OrderDetails.PelmetFlat, OrderDetails.ExtraFascia, OrderDetails.HingesLoose, OrderDetails.TiltrodType, OrderDetails.TiltrodSplit, OrderDetails.SplitHeight1, OrderDetails.SplitHeight2, OrderDetails.DoorCutOut, OrderDetails.SpecialShape, OrderDetails.TemplateProvided, OrderDetails.LinearMetre, OrderDetails.LinearMetreB, OrderDetails.LinearMetreC, OrderDetails.LinearMetreD, OrderDetails.LinearMetreE, OrderDetails.LinearMetreF, OrderDetails.LinearMetreG, OrderDetails.SquareMetre, OrderDetails.SquareMetreB, OrderDetails.SquareMetreC, OrderDetails.SquareMetreD, OrderDetails.SquareMetreE, OrderDetails.SquareMetreF, OrderDetails.SquareMetreG, OrderDetails.SquareMetreH, OrderDetails.TotalItems, OrderDetails.Notes, OrderDetails.MarkUp, OrderDetails.Active FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id=@ItemIdOld AND Products.DesignId<>'16';", thisConn)
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
            'Dim byActive As String = "WHERE OrderHeaders.Active='" & active & "'"
            'Dim byRole As String = String.Empty
            'Dim byCompany As String = String.Empty
            'Dim byInstaller As String = String.Empty
            'Dim byStatus As String = String.Empty
            'Dim byText As String = String.Empty
            'Dim byOrder As String = "ORDER BY OrderHeaders.Id DESC"

            'If Not String.IsNullOrEmpty(company) Then
            '    byCompany = "AND Customers.CompanyId = '" & company & "'"
            'End If

            'If Not String.IsNullOrEmpty(status) Then
            '    byStatus = "AND OrderHeaders.Status='" & status & "'"
            'End If

            'If Not search = "" Then
            '    byText = "AND (OrderHeaders.Id LIKE '%" & search.Trim() & "%' OR OrderHeaders.OrderId LIKE '%" & search.Trim() & "%' OR OrderHeaders.OrderNumber LIKE '%" & search.Trim() & "%' OR OrderHeaders.OrderName LIKE '%" & search.Trim() & "%' OR OrderHeaders.CustomerId LIKE '%" & search.Trim() & "%' OR Customers.Name LIKE '%" & search.Trim() & "%')"
            'End If

            'If Session("RoleName") = "Factory Office" Then
            '    byRole = String.Empty
            '    byStatus = String.Empty
            '    byOrder = "ORDER BY CASE WHEN OrderHeaders.Status='New Order' THEN 1 WHEN OrderHeaders.Status='In Production' THEN 2 WHEN OrderHeaders.Status='On Hold' THEN 3 ELSE 4 END, OrderHeaders.Id DESC"
            '    If Not status = "" Then
            '        byStatus = "AND OrderHeaders.Status='" & status & "'"
            '        byOrder = "ORDER BY OrderHeaders.Id DESC"
            '    End If
            'End If

            'If Session("RoleName") = "Customer" Then
            '    byRole = "AND OrderHeaders.CustomerId='" & Session("CustomerId") & "'"
            '    If Session("LevelName") = "Member" Then
            '        byRole = "AND OrderHeaders.CreatedBy='" & Session("LoginId") & "'"
            '    End If
            '    If Session("CustomerLevel") = "Sponsor" AndAlso Session("LevelName") = "Leader" Then
            '        byRole = "AND (OrderHeaders.CustomerId='" & Session("CustomerId") & "' OR Customers.SponsorId='" & Session("CustomerId") & "')"
            '    End If
            '    byStatus = String.Empty
            '    If Not status = "" Then
            '        byStatus = "AND OrderHeaders.Status='" & status & "'"
            '    End If
            'End If

            'If Session("RoleName") = "Sales" Then
            '    byRole = "AND Customers.CompanyId='" & Session("CompanyId") & "'"
            '    If Session("LevelName") = "Member" Then
            '        byRole = "AND Customers.Operator='" & Session("LoginId") & "'"
            '    End If
            '    byStatus = String.Empty
            '    byOrder = "ORDER BY CASE WHEN OrderHeaders.Status='New Order' THEN 1 WHEN OrderHeaders.Status='In Production' THEN 2 WHEN OrderHeaders.Status='On Hold' THEN 3 ELSE 4 END, OrderHeaders.Id ASC"
            '    If Not status = "" Then
            '        byStatus = "AND OrderHeaders.Status='" & status & "'"
            '        byOrder = "ORDER BY OrderHeaders.Id DESC"
            '    End If
            'End If

            'If Session("RoleName") = "Account" Then
            '    byRole = "AND Customers.CompanyId='" & Session("CompanyId") & "'"
            '    byStatus = String.Empty
            '    byOrder = "ORDER BY CASE WHEN OrderHeaders.Status='Waiting Proforma' THEN 1 WHEN OrderHeaders.Status='Proforma Sent' THEN 2 WHEN OrderHeaders.Status='New Order' THEN 3 ELSE 4 END, OrderHeaders.Id ASC"
            '    If Not status = "" Then
            '        byStatus = "AND OrderHeaders.Status='" & status & "'"
            '        byOrder = "ORDER BY OrderHeaders.Id ASC"
            '    End If
            'End If

            'If Session("RoleName") = "Customer Service" Then
            '    byRole = "AND Customers.CompanyId='" & Session("CompanyId") & "'"
            '    byStatus = String.Empty
            '    byOrder = "ORDER BY CASE WHEN OrderHeaders.Status='New Order' THEN 1 WHEN OrderHeaders.Status='In Production' THEN 2 WHEN OrderHeaders.Status='On Hold' THEN 3 ELSE 4 END, OrderHeaders.Id ASC"
            '    If Not status = "" Then
            '        byStatus = "AND OrderHeaders.Status='" & status & "'"
            '        byOrder = "ORDER BY OrderHeaders.Id DESC"
            '    End If
            'End If

            'If Session("RoleName") = "Data Entry" Then
            '    byRole = "AND Customers.CompanyId='" & Session("CompanyId") & "' AND CustomerLogins.RoleId='" & Session("RoleId") & "'"
            '    If Session("CompanyId") = "1" Then byRole = String.Empty
            '    byStatus = String.Empty
            '    If Not status = "" Then
            '        byStatus = "AND OrderHeaders.Status='" & status & "'"
            '    End If
            'End If

            'If Session("RoleName") = "Export" Then
            '    byRole = String.Empty
            '    byStatus = "AND (OrderHeaders.Status='In Production' OR OrderHeaders.Status='Shipped Out' OR OrderHeaders.Status='Completed')"
            '    If Not status = "" Then
            '        byStatus = "AND OrderHeaders.Status='" & status & "'"
            '    End If
            'End If

            'If Session("RoleName") = "Installer" Then
            '    byRole = "AND OrderHeaders.OrderType='Builder'"
            '    Dim area As String = orderClass.GetItemData("SELECT Area FROM InstallerAccess WHERE Id='" & Session("LoginId").ToString() & "'")
            '    byInstaller = "AND Customers.Area IN ('" & area & "')"
            '    byStatus = "AND OrderHeaders.Status='Quoted'"
            '    If Not status = "" Then
            '        byStatus = "AND OrderHeaders.Status='" & status & "'"
            '    End If
            '    byOrder = "ORDER BY OrderHeaders.Id, CASE WHEN Status='New Order' THEN 1 WHEN Status='In Production' THEN 2 WHEN Status='Completed' THEN 3 WHEN Status='Unsubmitted' THEN 4 WHEN Status='Canceled' THEN 5 END DESC"
            'End If

            'Dim thisQuery As String = String.Format("SELECT OrderHeaders.*, OrderShipments.ShipmentNumber, OrderShipments.ShipmentDate, OrderShipments.ContainerNumber, OrderShipments.Courier, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, Companys.Name AS CompanyName, CustomerLogins.RoleId AS CreatedRole FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id LEFT JOIN Companys ON Customers.CompanyId=Companys.Id LEFT JOIN OrderShipments ON OrderHeaders.Id=OrderShipments.Id LEFT JOIN CustomerLogins ON OrderHeaders.CreatedBy=CustomerLogins.Id LEFT JOIN CustomerLoginRoles ON CustomerLogins.RoleId=CustomerLoginRoles.Id {0} {1} {2} {3} {4} {5} {6}", byActive, byRole, byCompany, byInstaller, byStatus, byText, byOrder)

            'gvList.DataSource = orderClass.GetDataTable_Orders(thisQuery)
            'gvList.DataBind()

            Dim thisCmd As New SqlCommand("sp_GetOrders")
            thisCmd.CommandType = CommandType.StoredProcedure

            thisCmd.Parameters.AddWithValue("@Search", search)
            thisCmd.Parameters.AddWithValue("@Status", status)
            thisCmd.Parameters.AddWithValue("@Company", company)
            thisCmd.Parameters.AddWithValue("@Active", active)
            thisCmd.Parameters.AddWithValue("@RoleName", Session("RoleName"))
            thisCmd.Parameters.AddWithValue("@CustomerId", Session("CustomerId"))
            thisCmd.Parameters.AddWithValue("@LoginId", Session("LoginId"))
            thisCmd.Parameters.AddWithValue("@LevelName", Session("LevelName"))
            thisCmd.Parameters.AddWithValue("@CustomerLevel", Session("CustomerLevel"))
            thisCmd.Parameters.AddWithValue("@CompanyId", Session("CompanyId"))

            gvList.DataSource = orderClass.GetDataTable_Orders(thisCmd)
            gvList.DataBind()

            gvList.Columns(1).Visible = PageAction("Visible ID")
            gvList.Columns(3).Visible = PageAction("Visible Customer Name")
            If Session("CustomerLevel") = "Sponsor" AndAlso Session("LevelName") = "Leader" Then
                gvList.Columns(3).Visible = True
            End If
            gvList.Columns(7).Visible = PageAction("Visible Created Date")

            btnAdd.Visible = PageAction("Add")
            btnRework.Visible = PageAction("Rework")
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

    Protected Sub MessageError_Log(visible As Boolean, message As String)
        divErrorLog.Visible = visible : msgErrorLog.InnerText = message
    End Sub

    Protected Function BindTextLog(logId As String) As String
        Return orderClass.GetTextLog(logId)
    End Function

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
        If active = True AndAlso status = "In Production" AndAlso Session("RoleName") = "Developer" Then Return True
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
