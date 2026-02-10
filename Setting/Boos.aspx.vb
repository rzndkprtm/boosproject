Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Partial Class Setting_Boos
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If String.IsNullOrEmpty(Request.QueryString("action")) Then
            Response.Redirect("~/additional", False)
            Exit Sub
        End If

        Dim thisAction As String = Request.QueryString("action").ToString()
        If thisAction = "salescosting" Then
            CreateSalesCosting()
        End If
        If thisAction = "resetcashsaleorder" Then
            ResetDataCashSaleOrder()
        End If
        If thisAction = "unshipment" Then
            UnshipmentOrder()
        End If
        If thisAction = "orderactioncontext" Then
            DeleteContext()
        End If
        If thisAction = "login" Then
            If String.IsNullOrEmpty(Request.QueryString("user")) Then
                Response.Redirect("~/", False)
                Exit Sub
            End If
            GetTemporary(Request.QueryString("user").ToString())
        End If
        If thisAction = "resetpassword" Then
            ResetPassword()
        End If
        If thisAction = "productionorder" Then
            ProductionOrder()
        End If
        If thisAction = "clearsession" Then
            ClearSession()
        End If
        If thisAction = "builder" Then
            If String.IsNullOrEmpty(Request.QueryString("status")) Then
                Response.Redirect("~/", False)
                Exit Sub
            End If
            GenerateBuilder(Request.QueryString("status").ToString())
        End If
        If thisAction = "shipment" Then
            If String.IsNullOrEmpty(Request.QueryString("id")) Then
                Exit Sub
            End If
            If String.IsNullOrEmpty(Request.QueryString("status")) Then
                Exit Sub
            End If
            Dim id As String = Request.QueryString("OrdID").ToString()
            Dim status As String = Request.QueryString("Status").ToString()
            Dim shipmentNumber As String = Request.QueryString("ShipmentNo").ToString()
            Dim containerNumber As String = Request.QueryString("ContainerNo").ToString()
            Dim courier As String = Request.QueryString("Courier").ToString()
            Dim invoiceNumber As String = Request.QueryString("InvoiceNo").ToString()

            Dim shipDateStr As String = Request.QueryString("ShipDate")
            Dim shipDate As DateTime

            If String.IsNullOrEmpty(shipDateStr) OrElse Not DateTime.TryParse(shipDateStr, shipDate) Then

            End If

            UpdateShipment()
        End If
    End Sub

    Protected Sub GenerateBuilder(status As String)
        If Not String.IsNullOrEmpty(status) Then
            Dim archiveClass As New ArchiveClass
            Dim orderClass As New OrderClass

            Dim headerData As DataTable = archiveClass.GetDataTable("SELECT TOP 10 * FROM Builders WHERE Active=1 AND Status='" & status & "' ORDER BY OrdID DESC")

            If headerData.Rows.Count > 0 Then
                For i As Integer = 0 To headerData.Rows.Count - 1
                    Dim ordId As String = headerData.Rows(i)("OrdID").ToString()
                    Dim debtorCode As String = headerData.Rows(i)("DebtorCode").ToString()
                    Dim loginName As String = headerData.Rows(i)("LoginName").ToString().ToLower()
                    Dim customerId As String = settingClass.GetItemData("SELECT Id FROM Customers WHERE DebtorCode='" & debtorCode.Trim() & "'")

                    Dim createdBy As String = "26"
                    If loginName = "lia" Then createdBy = "12"
                    If loginName = "ros mawati" OrElse loginName = "rosm" OrElse loginName = "rosma" OrElse loginName = "rosmawati" Then
                        createdBy = "27"
                    End If
                    If loginName = "santi" Then createdBy = "14"
                    If loginName = "watik" Then createdBy = "13"

                    Dim orderId As String = "JPMD" & ordId

                    Using thisConn As New SqlConnection(myConn)
                        thisConn.Open()

                        Using myCmd As New SqlCommand("INSERT INTO bigblindsorder.dbo.OrderHeaders SELECT @Id, @OrderId, @CustomerId, StoreOrderNo, StoreCustomer, NULL, 'Builder', Status, NULL, @CreatedBy, CreatedDate, QuotedDate, SubmittedDate, NULL, NULL, NULL, NULL, 0, 1 FROM db4700_weborder.dbo.Builders WHERE Status='" & status & "' AND Active=1 AND OrdID=@OrdID", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", ordId)
                            myCmd.Parameters.AddWithValue("@OrderId", orderId)
                            myCmd.Parameters.AddWithValue("@CustomerId", customerId)
                            myCmd.Parameters.AddWithValue("@CreatedBy", createdBy)
                            myCmd.Parameters.AddWithValue("@OrdID", ordId)

                            myCmd.ExecuteNonQuery()
                        End Using

                        Using myCmd As New SqlCommand("INSERT INTO bigblindsorder.dbo.OrderBuilders SELECT @Id, Estimator, Supervisor, Address, CallUpDate, CMDueDate, BeInstalledDate, CompletedDate FROM db4700_weborder.dbo.Builders WHERE Status='" & status & "' AND Active=1 AND OrdID=@OrdID", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", ordId)
                            myCmd.Parameters.AddWithValue("@OrderId", orderId)
                            myCmd.Parameters.AddWithValue("@CustomerId", customerId)
                            myCmd.Parameters.AddWithValue("@CreatedBy", createdBy)
                            myCmd.Parameters.AddWithValue("@OrdID", ordId)
                            myCmd.ExecuteNonQuery()
                        End Using

                        Using myCmd As New SqlCommand("INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", ordId)

                            myCmd.ExecuteNonQuery()
                        End Using

                        thisConn.Close()
                    End Using

                    Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Order/{0}/", orderId))
                    If Not Directory.Exists(directoryOrder) Then
                        Directory.CreateDirectory(directoryOrder)
                    End If

                    'ORDER DETAIL
                    Dim detailData As DataTable = archiveClass.GetDataTable("SELECT * FROM Order_Detail WHERE FKOrdID='" & ordId & "' AND Active=1 ORDER BY OrddID ASC")
                    If detailData.Rows.Count > 0 Then
                        For iDetail As Integer = 0 To detailData.Rows.Count - 1
                            Dim orddId As String = detailData.Rows(iDetail)("OrddID").ToString()
                            Dim blindType As String = detailData.Rows(iDetail)("BlindType").ToString()
                            Dim orderType As String = detailData.Rows(iDetail)("OrderType").ToString()

                            If blindType = "Roller" Then
                                Dim bracketColour As String = detailData.Rows(iDetail)("Colour").ToString()
                                Dim fabric As String = detailData.Rows(iDetail)("Fabric").ToString()
                                Dim chaintype As String = detailData.Rows(iDetail)("ChainType").ToString()
                                Dim brtype As String = detailData.Rows(iDetail)("BrType").ToString()
                                Dim brcolour As String = detailData.Rows(iDetail)("BrColour").ToString()
                                Dim tubeType As String = detailData.Rows(iDetail)("LayoutCode").ToString()

                                Dim designId As String = "12"
                                Dim blindId As String = String.Empty
                                Dim tubeId As String = "1"
                                Dim controlId As String = "1"
                                Dim colourId As String = String.Empty
                                Dim productId As String = String.Empty

                                If orderType = "Regular Chain" Then
                                    Dim fabricId As String = String.Empty
                                    Dim fabricColourId As String = String.Empty
                                    Dim chainId As String = String.Empty
                                    Dim bottomId As String = String.Empty
                                    Dim bottomColourId As String = String.Empty

                                    Dim priceProductGroupId As String = String.Empty

                                    If tubeType = "Gear Reduction 38mm Tube" Then tubeId = "2"
                                    If tubeType = "Gear Reduction 45mm Tube" Then tubeId = "3"
                                    If tubeType = "Gear Reduction 49mm Tube" Then tubeId = "4"

                                    blindId = "28"
                                    colourId = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name = '" & bracketColour & "'")
                                    productId = orderClass.GetItemData("SELECT Id FROM Products WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "'")

                                    If fabric.Contains("Essential") Then fabricId = "31"
                                    If fabric.Contains("Echo BO") Then fabricId = "29"
                                    If fabric.Contains("Echo TL") Then fabricId = "30"
                                    If fabric.Contains("Havana BO") Then fabricId = "35"

                                    Dim fabricColourName As String = fabric.Split("("c)(1).Split(")"c)(0).Trim()
                                    fabricColourId = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColourName & "'")

                                    chainId = orderClass.GetItemData("SELECT Id FROM Chains WHERE Name = '" & Replace(chaintype, "Continuous", "Cont") & "'")

                                    If brtype = "Oval" Then bottomId = "5"
                                    If brtype = "Round" Then bottomId = "6"

                                    bottomColourId = orderClass.GetItemData("SELECT Id FROM BottomColours WHERE BottomId='" & bottomId & "' AND Colour='" & brcolour & "'")

                                    Dim groupFabric As String = orderClass.GetFabricGroup(fabricId)

                                    Dim tubeIstilah As String = "Standard"
                                    If tubeType.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"

                                    Dim productGroupName As String = String.Format("Roller Blind - {0} - {1}", tubeIstilah, groupFabric)
                                    priceProductGroupId = orderClass.GetPriceProductGroupId(productGroupName, designId)

                                    Using thisConn As SqlConnection = New SqlConnection(myConn)
                                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO bigblindsorder.dbo.OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, ChainId, BottomId, BottomColourId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], Roll, ControlPosition, ControlLength, ControlLengthValue, ChainStopper, FlatOption, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) SELECT @Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @ChainId, @BottomId, @BottomColourId, @PriceProductGroupId, 1, Room, Mounting, Width, [Drop], Roll, ControlPosition, 'Custom', RollerChainLength, Additional, FlatBottomOp, CAST(Width AS DECIMAL(18,4)) / 1000, (CAST(Width AS DECIMAL(18,4)) * CAST([Drop] AS DECIMAL(18,4))) / 1000000, 1, Notes, 0, 1 FROM db4700_weborder.dbo.Order_Detail WHERE OrddID=@OrddID", thisConn)
                                            myCmd.Parameters.AddWithValue("@OrddID", orddId)
                                            myCmd.Parameters.AddWithValue("@Id", orddId)
                                            myCmd.Parameters.AddWithValue("@HeaderId", ordId)
                                            myCmd.Parameters.AddWithValue("@ProductId", productId)
                                            myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                            myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                            myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                            myCmd.Parameters.AddWithValue("@BottomId", If(String.IsNullOrEmpty(bottomId), CType(DBNull.Value, Object), bottomId))
                                            myCmd.Parameters.AddWithValue("@BottomColourId", If(String.IsNullOrEmpty(bottomColourId), CType(DBNull.Value, Object), bottomColourId))
                                            myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroupId), CType(DBNull.Value, Object), priceProductGroupId))

                                            thisConn.Open()
                                            myCmd.ExecuteNonQuery()
                                        End Using
                                    End Using

                                    orderClass.ResetPriceDetail(ordId, orddId)
                                    orderClass.CalculatePrice(ordId, orddId)
                                    orderClass.FinalCostItem(ordId, orddId)
                                End If

                                If orderType = "Double Bracket" Then
                                    Dim fabricb As String = detailData.Rows(iDetail)("FabricType2a").ToString()

                                    Dim fabricId As String = String.Empty
                                    Dim fabricIdB As String = String.Empty
                                    Dim fabricColourId As String = String.Empty
                                    Dim fabricColourIdB As String = String.Empty
                                    Dim chainId As String = String.Empty
                                    Dim bottomId As String = String.Empty
                                    Dim bottomColourId As String = String.Empty

                                    Dim priceProductGroupId As String = String.Empty
                                    Dim priceProductGroupIdB As String = String.Empty

                                    If tubeType = "Gear Reduction 38mm Tube" Then tubeId = "2"
                                    If tubeType = "Gear Reduction 45mm Tube" Then tubeId = "3"
                                    If tubeType = "Gear Reduction 49mm Tube" Then tubeId = "4"

                                    blindId = "29"
                                    colourId = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name = '" & bracketColour & "'")
                                    productId = orderClass.GetItemData("SELECT Id FROM Products WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "'")

                                    If fabric.Contains("Viewscreen") Then fabricId = "50"
                                    If fabricb.Contains("Essential") Then fabricId = "31"

                                    Dim fabricColourName As String = fabric.Split("("c)(1).Split(")"c)(0).Trim()
                                    fabricColourId = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColourName & "'")

                                    Dim fabricColourNameB As String = fabric.Split("("c)(1).Split(")"c)(0).Trim()
                                    fabricColourIdB = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColourNameB & "'")

                                    chainId = orderClass.GetItemData("SELECT Id FROM Chains WHERE Name = '" & Replace(chaintype, "Continuous", "Cont") & "'")

                                    If brtype = "Oval" Then bottomId = "5"
                                    If brtype = "Round" Then bottomId = "6"

                                    bottomColourId = orderClass.GetItemData("SELECT Id FROM BottomColours WHERE BottomId='" & bottomId & "' AND Colour='" & brcolour & "'")

                                    Dim groupFabric As String = orderClass.GetFabricGroup(fabricId)
                                    Dim groupFabricDB As String = orderClass.GetFabricGroup(fabricIdB)

                                    Dim tubeIstilah As String = "Standard"
                                    If tubeType.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"

                                    Dim productGroupName As String = String.Format("Roller Blind - {0} - {1}", tubeIstilah, groupFabric)
                                    Dim productGroupNameDB As String = String.Format("Roller Blind - {0} - {1}", tubeIstilah, groupFabricDB)

                                    priceProductGroupId = orderClass.GetPriceProductGroupId(productGroupName, designId)
                                    priceProductGroupIdB = orderClass.GetPriceProductGroupId(productGroupNameDB, designId)

                                    Using thisConn As SqlConnection = New SqlConnection(myConn)
                                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO bigblindsorder.dbo.OrderDetails(Id, HeaderId, ProductId, FabricId, FabricIdB, FabricColourId, FabricColourIdB, ChainId, ChainIdB, BottomId, BottomIdB, BottomColourId, BottomColourIdB, PriceProductGroupId, PriceProductGroupIdB, Qty, Room, Mounting, Width, WidthB, [Drop], DropB, Roll, RollB, ControlPosition, ControlPositionB, ControlLength, ControlLengthB, ControlLengthValue, ControlLengthValueB, ChainStopper, ChainStopperB, FlatOption, FlatOptionB, LinearMetre, LinearMetreB, SquareMetre, SquareMetreB, TotalItems, Notes, MarkUp, Active) SELECT @Id, @HeaderId, @ProductId, @FabricId, @FabricIdB, @FabricColourId, @FabricColourIdB, @ChainId, @ChainIdB, @BottomId, @BottomIdB, @BottomColourId, @BottomColourIdB, @PriceProductGroupId, @PriceProductGroupIdB, 1, Room, Mounting, Width, Width, [Drop], [Drop], Roll, Roll2a, ControlPosition, ControlPositionDB3b, 'Custom', 'Custom', RollerChainLength, RollerChainLength, Additional, Additional1, FlatBottomOp, FlatBottomOp, CAST(Width AS DECIMAL(18,4)) / 1000, CAST(Width AS DECIMAL(18,4)) / 1000, (CAST(Width AS DECIMAL(18,4)) * CAST([Drop] AS DECIMAL(18,4))) / 1000000, (CAST(Width AS DECIMAL(18,4)) * CAST([Drop] AS DECIMAL(18,4))) / 1000000, 2, Notes, 0, 1 FROM db4700_weborder.dbo.Order_Detail WHERE OrddID=@OrddID", thisConn)
                                            myCmd.Parameters.AddWithValue("@OrddID", orddId)
                                            myCmd.Parameters.AddWithValue("@Id", orddId)
                                            myCmd.Parameters.AddWithValue("@HeaderId", ordId)
                                            myCmd.Parameters.AddWithValue("@ProductId", productId)
                                            myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                            myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                            myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                            myCmd.Parameters.AddWithValue("@FabricColourIdB", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                            myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                            myCmd.Parameters.AddWithValue("@ChainIdB", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                            myCmd.Parameters.AddWithValue("@BottomId", If(String.IsNullOrEmpty(bottomId), CType(DBNull.Value, Object), bottomId))
                                            myCmd.Parameters.AddWithValue("@BottomIdB", If(String.IsNullOrEmpty(bottomId), CType(DBNull.Value, Object), bottomId))
                                            myCmd.Parameters.AddWithValue("@BottomColourId", If(String.IsNullOrEmpty(bottomColourId), CType(DBNull.Value, Object), bottomColourId))
                                            myCmd.Parameters.AddWithValue("@BottomColourIdB", If(String.IsNullOrEmpty(bottomColourId), CType(DBNull.Value, Object), bottomColourId))

                                            myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroupId), CType(DBNull.Value, Object), priceProductGroupId))
                                            myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupIdB), CType(DBNull.Value, Object), priceProductGroupIdB))

                                            thisConn.Open()
                                            myCmd.ExecuteNonQuery()
                                        End Using
                                    End Using

                                    orderClass.ResetPriceDetail(ordId, orddId)
                                    orderClass.CalculatePrice(ordId, orddId)
                                    orderClass.FinalCostItem(ordId, orddId)
                                End If

                                If orderType = "Link System Independent 2 Blinds" Then
                                    Dim fabricId As String = String.Empty
                                    Dim fabricColourId As String = String.Empty
                                    Dim chainId As String = String.Empty
                                    Dim bottomId As String = String.Empty
                                    Dim bottomColourId As String = String.Empty

                                    Dim priceProductGroupId As String = String.Empty
                                    Dim priceProductGroupIdB As String = String.Empty

                                    If tubeType = "Gear Reduction 38mm Tube" Then tubeId = "2"
                                    If tubeType = "Gear Reduction 45mm Tube" Then tubeId = "3"
                                    If tubeType = "Gear Reduction 49mm Tube" Then tubeId = "4"

                                    blindId = "31"
                                    colourId = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name = '" & bracketColour & "'")
                                    productId = orderClass.GetItemData("SELECT Id FROM Products WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "'")

                                    If fabric.Contains("Essential") Then fabricId = "31"
                                    If fabric.Contains("Echo BO") Then fabricId = "29"
                                    If fabric.Contains("Echo TL") Then fabricId = "30"
                                    If fabric.Contains("Havana BO") Then fabricId = "35"

                                    Dim fabricColourName As String = fabric.Split("("c)(1).Split(")"c)(0).Trim()
                                    fabricColourId = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColourName & "'")

                                    chainId = orderClass.GetItemData("SELECT Id FROM Chains WHERE Name = '" & Replace(chaintype, "Continuous", "Cont") & "'")

                                    If brtype = "Oval" Then bottomId = "5"
                                    If brtype = "Round" Then bottomId = "6"

                                    bottomColourId = orderClass.GetItemData("SELECT Id FROM BottomColours WHERE BottomId='" & bottomId & "' AND Colour='" & brcolour & "'")

                                    Dim groupFabric As String = orderClass.GetFabricGroup(fabricId)

                                    Dim tubeIstilah As String = "Standard"
                                    If tubeType.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"

                                    Dim productGroupName As String = String.Format("Roller Blind - {0} - {1}", tubeIstilah, groupFabric)

                                    priceProductGroupId = orderClass.GetPriceProductGroupId(productGroupName, designId)
                                    priceProductGroupIdB = orderClass.GetPriceProductGroupId(productGroupName, designId)

                                    Using thisConn As SqlConnection = New SqlConnection(myConn)
                                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO bigblindsorder.dbo.OrderDetails(Id, HeaderId, ProductId, FabricId, FabricIdB, FabricColourId, FabricColourIdB, ChainId, ChainIdB, BottomId, BottomIdB, BottomColourId, BottomColourIdB, PriceProductGroupId, PriceProductGroupIdB, Qty, Room, Mounting, Width, WidthB, [Drop], DropB, Roll, RollB, ControlPosition, ControlPositionB, ControlLength, ControlLengthB, ControlLengthValue, ControlLengthValueB, ChainStopper, ChainStopperB, FlatOption, FlatOptionB, LinearMetre, LinearMetreB, SquareMetre, SquareMetreB, TotalItems, Notes, MarkUp, Active) SELECT @Id, @HeaderId, @ProductId, @FabricId, @FabricIdB, @FabricColourId, @FabricColourId, @ChainId, @ChainId, @BottomId, @BottomId, @BottomColourId, @BottomColourId, @PriceProductGroupId, @PriceProductGroupIdB, 1, Room, Mounting, Width, Width2C, [Drop], [Drop], Roll, Roll, 'Left', 'Right', 'Custom', 'Custom', RollerChainLength, RollerChainLength, Additional, Additional1, FlatBottomOp, FlatBottomOp, CAST(Width AS DECIMAL(18,4)) / 1000, CAST(Width2c AS DECIMAL(18,4)) / 1000, (CAST(Width AS DECIMAL(18,4)) * CAST([Drop] AS DECIMAL(18,4))) / 1000000, (CAST(Width2c AS DECIMAL(18,4)) * CAST([Drop] AS DECIMAL(18,4))) / 1000000, 2, Notes, 0, 1 FROM db4700_weborder.dbo.Order_Detail WHERE OrddID=@OrddID", thisConn)
                                            myCmd.Parameters.AddWithValue("@OrddID", orddId)
                                            myCmd.Parameters.AddWithValue("@Id", orddId)
                                            myCmd.Parameters.AddWithValue("@HeaderId", ordId)
                                            myCmd.Parameters.AddWithValue("@ProductId", productId)
                                            myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                            myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                            myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                            myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                            myCmd.Parameters.AddWithValue("@BottomId", If(String.IsNullOrEmpty(bottomId), CType(DBNull.Value, Object), bottomId))
                                            myCmd.Parameters.AddWithValue("@BottomColourId", If(String.IsNullOrEmpty(bottomColourId), CType(DBNull.Value, Object), bottomColourId))
                                            myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroupId), CType(DBNull.Value, Object), priceProductGroupId))
                                            myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupIdB), CType(DBNull.Value, Object), priceProductGroupIdB))

                                            thisConn.Open()
                                            myCmd.ExecuteNonQuery()
                                        End Using
                                    End Using

                                    orderClass.ResetPriceDetail(ordId, orddId)
                                    orderClass.CalculatePrice(ordId, orddId)
                                    orderClass.FinalCostItem(ordId, orddId)
                                End If
                            End If

                            If blindType = "Vertical" Then
                                Dim controlType As String = detailData.Rows(iDetail)("ControlType").ToString()
                                Dim colourType As String = detailData.Rows(iDetail)("VerHRColour").ToString()
                                Dim fabric As String = detailData.Rows(iDetail)("Fabric").ToString()
                                Dim wandColour As String = detailData.Rows(iDetail)("VerHRColour").ToString()

                                Dim designId As String = "11"
                                Dim blindId As String = "25"
                                Dim tubeId As String = ""
                                If orderType.Contains("127mm") Then tubeId = "5"
                                If orderType.Contains("89mm") Then tubeId = "6"
                                Dim controlId As String = ""
                                If controlType = "Chain" Then controlId = "1"
                                If controlType = "Wand" Then controlId = "3"

                                Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name = '" & colourType & "'")

                                Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "'")

                                Dim fabricId As String = ""
                                If orderType.Contains("127mm") Then fabricId = "52"
                                If orderType.Contains("89mm") Then fabricId = "53"

                                If controlType = "Chain" Then wandColour = ""

                                Dim controlLengthValue As Integer = 0
                                Dim wandLengthValue As Integer = 0
                                If controlType = "Chain" Then controlLengthValue = detailData.Rows(iDetail)("ControlLength").ToString()
                                If controlType = "Wand" Then wandLengthValue = detailData.Rows(iDetail)("ControlLength").ToString()

                                Dim fabricColourName As String = fabric.Split("("c)(1).Split(")"c)(0).Trim()
                                Dim fabricColourId As String = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColourName & "'")

                                Dim chainId As String = ""
                                If controlType = "Chain" Then chainId = "17"

                                Dim priceProductGroupId As String = ""

                                Using thisConn As SqlConnection = New SqlConnection(myConn)
                                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO bigblindsorder.dbo.OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, ChainId, PriceProductGroupId, Qty, QtyBlade, Room, Mounting, Width, [Drop], StackPosition, ControlPosition, ControlLength, ControlLengthValue, WandColour, WandLengthValue, FabricInsert, BottomJoining, BracketExtension, Sloping, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) SELECT @Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @ChainId, @PriceProductGroupId, 1, 1, Room, Mounting, Width, [Drop], ControlOperation, ControlPosition, 'Custom', @ControlLengthValue, @WandColour, @WandLengthValue, FabricInsert, BottomJoin, Extbracket, '', CAST(Width AS DECIMAL(18,4)) / 1000, (CAST(Width2c AS DECIMAL(18,4)) * CAST([Drop] AS DECIMAL(18,4))) / 1000000, 1, Notes, 0, 1 FROM db4700_weborder.dbo.Order_Detail WHERE OrddID=@OrddID", thisConn)
                                        myCmd.Parameters.AddWithValue("@Id", orddId)
                                        myCmd.Parameters.AddWithValue("@OrddID", orddId)
                                        myCmd.Parameters.AddWithValue("@HeaderId", ordId)
                                        myCmd.Parameters.AddWithValue("@ProductId", productId)
                                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                        myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroupId), CType(DBNull.Value, Object), priceProductGroupId))
                                        myCmd.Parameters.AddWithValue("@WandColour", wandColour)
                                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                        myCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)

                                        thisConn.Open()
                                        myCmd.ExecuteNonQuery()
                                    End Using
                                End Using

                                orderClass.ResetPriceDetail(ordId, orddId)
                                orderClass.CalculatePrice(ordId, orddId)
                                orderClass.FinalCostItem(ordId, orddId)
                            End If
                        Next
                    End If
                Next
            End If
        End If
    End Sub

    Protected Sub CreateSalesCosting()
        Try
            Dim salesClass As New SalesClass
            Dim checkData As Integer = salesClass.GetItemData_Integer("SELECT COUNT(*) FROM Sales WHERE SummaryDate=GETDATE()")
            If checkData = 0 Then
                Dim thisId As String = salesClass.CreateId("SELECT TOP 1 Id FROM Sales ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Sales(Id, SummaryDate, TotalCostPrice, TotalSellingPrice, TotalPaidAmount) VALUES(@Id, GETDATE(), 0, 0, 0)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ResetDataCashSaleOrder()
        Try
            Dim orderClass As New OrderClass
            Dim thisData As DataTable = orderClass.GetDataTable("SELECT OrderHeaders.* FROM OrderHeaders LEFT JOIN OrderInvoices ON OrderHeaders.Id=OrderInvoices.Id WHERE OrderHeaders.Status='Proforma Sent' AND OrderInvoices.DueDate=CAST(GETDATE() AS DATE)")

            If thisData.Rows.Count > 0 Then
                For i As Integer = 0 To thisData.Rows.Count - 1
                    Dim thisId As String = thisData.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        thisConn.Open()

                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='Unsubmitted', SubmittedDate=NULL WHERE Id=@Id; DELETE FROM OrderInvoices WHERE Id=@Id;", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)

                            myCmd.ExecuteNonQuery()
                        End Using

                        Dim serviceData As DataTable = orderClass.GetDataTable("SELECT OrderDetails.* FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.HeaderId='" & thisId & "' AND Products.DesignId='16'")
                        If serviceData.Rows.Count > 0 Then
                            For iDetail As Integer = 0 To serviceData.Rows.Count - 1
                                Dim serviceId As String = serviceData.Rows(iDetail)("Id").ToString()

                                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Active=0 WHERE Id=@ItemId; DELETE FROM OrderCostings WHERE HeaderId=@HeaderId AND ItemId=@ItemId", thisConn)
                                    myCmd.Parameters.AddWithValue("@ItemId", serviceId)
                                    myCmd.Parameters.AddWithValue("@HeaderId", thisId)

                                    myCmd.ExecuteNonQuery()
                                End Using
                            Next
                        End If

                        thisConn.Close()
                    End Using
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub UnshipmentOrder()
        Try
            If Now.DayOfWeek >= DayOfWeek.Monday AndAlso Now.DayOfWeek <= DayOfWeek.Friday Then
                Dim mailClass As New MailingClass
                Dim unshipmentClass As New UnshipmentClass

                Dim fileName As String = Trim("Unshipment - In Production Order " & Now.ToString("dd MMm yyyy") & ".pdf")

                Dim pdfFilePath As String = Server.MapPath("~/File/Report/" & fileName)
                unshipmentClass.BindContent(pdfFilePath)

                mailClass.MailUnshipment(pdfFilePath)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub DeleteContext()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderActionContext", thisConn)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub GetTemporary(user As String)
        Try
            Dim thisId As String = String.Empty

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Temporarys OUTPUT INSERTED.Id VALUES (NEWID(), @UserName)", thisConn)
                    myCmd.Parameters.AddWithValue("@UserName", user)

                    thisConn.Open()
                    thisId = myCmd.ExecuteScalar().ToString()
                End Using
            End Using
            Dim url As String = String.Format("~/information?uid={0}", thisId)
            Response.Redirect(url, False)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ResetPassword()
        Dim loginData As DataTable = settingClass.GetDataTable("SELECT CustomerLogins.* FROM CustomerLogins LEFT JOIN Customers ON CustomerLogins.CustomerId=Customers.Id WHERE Customers.CompanyId='2'")
        If loginData.Rows.Count > 0 Then
            For iLogin As Integer = 0 To loginData.Rows.Count - 1
                Dim loginId As String = loginData.Rows(iLogin)("Id").ToString()
                Dim userName As String = loginData.Rows(iLogin)("UserName").ToString()

                Dim newPassword As String = settingClass.Encrypt(userName)

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET Password=@Password, FailedCount=0, LastLogin=NULL, ResetLogin=1 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", loginId)
                        myCmd.Parameters.AddWithValue("@Password", newPassword)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using
            Next
        End If
    End Sub

    Protected Sub ProductionOrder()
        Try
            Dim thisData As DataTable = settingClass.GetDataTable("SELECT * FROM OrderHeaders WHERE ProductionDate=CAST(GETDATE() AS DATE) AND Active=1")
            If thisData.Rows.Count > 0 Then
                Dim mailingClass As New MailingClass
                For i As Integer = 0 To thisData.Rows.Count - 1
                    Dim headerId As String = thisData.Rows(i)("Id").ToString()

                    mailingClass.ProductionOrder(headerId)
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ClearSession()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Sessions", thisConn)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub UpdateShipment()

    End Sub
End Class
