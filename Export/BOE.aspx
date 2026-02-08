<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.IO.Compression" %>
<%@ Page Language="VB" Title="Export BOE Result" ContentType="text/xml" Debug="true" %>

<script runat="server">
    
    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetDataTable(thisString As String) As DataTable
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using da As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return dt
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand(thisString, thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0).ToString()
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs)
        Dim type As String = Request.QueryString("type").ToString()
        Dim company As String = Request.QueryString("company").ToString()
        Dim action As String = String.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("action")) Then
            action = Request.QueryString("action").ToString()
        End If

        Dim status As String = String.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("status")) Then
            status = Request.QueryString("status").ToString()
        End If

        Dim stringCompany As String = String.Empty
        stringCompany = "AND Customers.CompanyId='" & company & "'"

        If company = "jpmd" Then
            stringCompany = "AND Customers.CompanyId='2'"
        End If
        If company = "local" Then
            stringCompany = "AND (Customers.CompanyId='3' OR Customers.CompanyId='5')"
        End If

        Dim stringStatus As String = String.Empty
        If String.IsNullOrEmpty(status) Then
            stringStatus = "AND OrderHeaders.Status='" & status & "'"
        End If

        If action = "download" Then
            stringStatus = "AND (OrderHeaders.Status='New Order' OR OrderHeaders.Status='Payment Received')"
        End If

        If type = "header" Then
            If action = "download" Then
                Dim thisQuery As String = String.Format("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.DebtorCode AS DebtorCode, CustomerLogins.UserName AS UserName FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id INNER JOIN CustomerLogins ON OrderHeaders.CreatedBy=CustomerLogins.Id WHERE OrderHeaders.Active=1 {0} {1} ORDER BY OrderHeaders.Id ASC", stringCompany, stringStatus)

                Dim thisData As DataTable = GetDataTable(thisQuery)
                If thisData.Rows.Count > 0 Then
                    For i As Integer = 0 To thisData.Rows.Count - 1
                        Dim headerId As String = thisData.Rows(i)("Id").ToString()
                        Dim orderId As String = thisData.Rows(i)("OrderId").ToString()
                        Dim debtorCode As String = thisData.Rows(i)("DebtorCode").ToString()

                        If String.IsNullOrEmpty(debtorCode) Then
                            Continue For
                        End If

                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Status='In Production', ProductionDate=GETDATE(), DownloadBOE=1 WHERE Id=@Id; INSERT INTO OrderShipments(Id) VALUES (@Id)", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", headerId)

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        Dim dataLog As Object() = {"OrderHeaders", headerId, "2", "Order In Production"}
                        orderClass.Logs(dataLog)

                        If company = "2" OrElse company = "jpmd" Then
                            Dim salesClass As New SalesClass
                            salesClass.RefreshData()
                        End If

                        'Dim checkPrinting As Integer = orderClass.GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails WHERE HeaderId='" & headerId & "' AND (NULLIF(LTRIM(RTRIM(Printing)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingB)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingC)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingD)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingE)),'') IS NOT NULL OR NULLIF(LTRIM(RTRIM(PrintingF)),'') IS NOT NULL)")

                        'If checkPrinting > 0 Then
                        '    Dim sourceFolder As String = Server.MapPath(String.Format("~/File/Printing/{0}", headerId))
                        '    Dim zipPath As String = Server.MapPath(String.Format("~/File/Printing/{0}.zip", orderId))

                        '    If IO.File.Exists(zipPath) Then IO.File.Delete(zipPath)
                        '    ZipFile.CreateFromDirectory(sourceFolder, zipPath, CompressionLevel.Fastest, True)

                        '    mailingClass.SubmitOrder_PrintingFabric(headerId, finalFilePath, zipPath)
                        'End If
                        mailingClass.ProductionOrder(headerId)
                    Next
                End If
            End If

            Dim headerQuery As String = String.Format("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.DebtorCode AS DebtorCode, CustomerLogins.UserName AS UserName FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id INNER JOIN CustomerLogins ON OrderHeaders.CreatedBy=CustomerLogins.Id WHERE OrderHeaders.Active=1 {0} AND OrderHeaders.DownloadBOE=1 AND CAST(OrderHeaders.CreatedDate AS DATE)=CAST(GETDATE() AS DATE) ORDER BY OrderHeaders.Id ASC", stringCompany)

            If action = "download" Then
                headerQuery = String.Format("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.DebtorCode AS DebtorCode, CustomerLogins.UserName AS UserName FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id INNER JOIN CustomerLogins ON OrderHeaders.CreatedBy=CustomerLogins.Id WHERE OrderHeaders.Active=1 {0} AND OrderHeaders.DownloadBOE=1 AND CAST(OrderHeaders.ProductionDate AS DATE) = CAST(GETDATE() AS DATE) ORDER BY OrderHeaders.Id ASC", stringCompany)
            End If

            Dim headerData As DataTable = GetDataTable(headerQuery)
            DataHeader(headerData)
        End If

        If type = "detail" Then
            Dim detailQuery As String = String.Format("SELECT OrderDetails.*, Products.DesignId AS DesignId, Products.BlindId AS BlindId, Designs.Name AS DesignName, Blinds.Name AS BlindName, Products.Name AS ProductName, ProductTubes.Name AS TubeName, ProductControls.Name AS ControlName, ProductColours.Name AS ColourName FROM OrderDetails INNER JOIN OrderHeaders ON OrderDetails.HeaderId=OrderHeaders.Id INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductTubes ON Products.TubeType=ProductTubes.Id LEFT JOIN ProductControls ON Products.ControlType=ProductControls.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.Active=1 AND OrderHeaders.Active=1 AND OrderHeaders.DownloadBOE=1 {0}", stringCompany)

            If action = "download" Then
                detailQuery = String.Format("SELECT OrderDetails.*, Products.DesignId AS DesignId, Products.BlindId AS BlindId, Designs.Name AS DesignName, Blinds.Name AS BlindName, Products.Name AS ProductName, ProductTubes.Name AS TubeName, ProductControls.Name AS ControlName, ProductColours.Name AS ColourName FROM OrderDetails INNER JOIN OrderHeaders ON OrderDetails.HeaderId=OrderHeaders.Id INNER JOIN Customers ON OrderHeaders.CustomerId=Customers.Id LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductTubes ON Products.TubeType=ProductTubes.Id LEFT JOIN ProductControls ON Products.ControlType=ProductControls.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.Active=1 AND OrderHeaders.Active=1 AND OrderHeaders.DownloadBOE=1 AND CAST(OrderHeaders.ProductionDate AS DATE) = CAST(GETDATE() AS DATE) {0}", stringCompany)
            End If

            Dim detailData As DataTable = GetDataTable(detailQuery)
            DataDetail(detailData)
        End If
    End Sub

    Protected Sub DataHeader(myData As DataTable)
        Dim writer As New XmlTextWriter(Response.OutputStream, Encoding.ASCII)
        writer.WriteStartDocument()
        writer.WriteStartElement("Orders")

        For i As Integer = 0 To myData.Rows.Count - 1
            writer.WriteStartElement("OrderHeader")
            writer.WriteAttributeString("Id", myData.Rows(i)("Id").ToString())
            writer.WriteAttributeString("OrderId", myData.Rows(i)("OrderId").ToString())
            writer.WriteAttributeString("CustomerId", myData.Rows(i)("CustomerId").ToString())
            writer.WriteAttributeString("OrderNumber", myData.Rows(i)("OrderNumber").ToString())
            writer.WriteAttributeString("OrderName", myData.Rows(i)("OrderName").ToString())
            writer.WriteAttributeString("OrderNote", myData.Rows(i)("OrderNote").ToString())

            writer.WriteAttributeString("OrdID", myData.Rows(i)("Id").ToString())
            writer.WriteAttributeString("StoreOrderNo", myData.Rows(i)("OrderNumber").ToString())
            writer.WriteAttributeString("StoreCustomer", myData.Rows(i)("OrderName").ToString())
            writer.WriteAttributeString("DebtorCode", myData.Rows(i)("DebtorCode").ToString())

            Dim prodDate As DateTime
            If DateTime.TryParse(myData.Rows(i)("ProductionDate").ToString(), prodDate) Then
                writer.WriteAttributeString("SubmittedDate", prodDate.ToString("dd/MM/yyyy"))
            Else
                writer.WriteAttributeString("SubmittedDate", "")
            End If
            writer.WriteAttributeString("Production", "BIG")
            writer.WriteAttributeString("SubProduction", "Universal")
            writer.WriteAttributeString("Status", myData.Rows(i)("Status").ToString())
            writer.WriteEndElement()
        Next

        writer.WriteEndElement()
        writer.WriteEndDocument()
        writer.Close()
    End Sub

    Protected Sub DataDetail(thisData As DataTable)
        Dim writer As New XmlTextWriter(Response.OutputStream, Encoding.ASCII)
        writer.WriteStartDocument()
        writer.WriteStartElement("Orders")

        For i As Integer = 0 To thisData.Rows.Count - 1
            Dim productId As String = thisData.Rows(i)("ProductId").ToString()
            Dim productName As String = thisData.Rows(i)("ProductName").ToString()

            Dim designName As String = thisData.Rows(i)("DesignName").ToString()
            Dim blindName As String = thisData.Rows(i)("BlindName").ToString()

            Dim tubeName As String = thisData.Rows(i)("TubeName").ToString()
            Dim controlName As String = thisData.Rows(i)("ControlName").ToString()
            Dim colourName As String = thisData.Rows(i)("ColourName").ToString()

            If designName = "Aluminium Blind" Then
                Dim subType As String = thisData.Rows(i)("SubType").ToString()
                Dim venId As String = GetItemData("SELECT VenId FROM ProductKits WHERE ProductId='" & productId & "'")

                Dim venIdB As String = String.Empty
                If subType.Contains("2 on 1") Then venIdB = venId

                Dim newSubType As String = "Single"
                If subType = "2 on 1 Left-Left" Then newSubType = "2 on 1 Aluminium Left-Left"
                If subType = "2 on 1 Right-Right" Then newSubType = "2 on 1 Aluminium Right-Right"
                If subType = "2 on 1 Left-Right" Then newSubType = "2 on 1 Aluminium Left-Right"

                If String.IsNullOrEmpty(venId) Then Continue For

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", "Ven")
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("SubType", newSubType)
                writer.WriteAttributeString("IDHK", venId)
                writer.WriteAttributeString("IDHK2", venIdB)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Width2c", thisData.Rows(i)("WidthB").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("Drop2c", thisData.Rows(i)("DropB").ToString())
                writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                writer.WriteAttributeString("ControlPosition_LSDouble3rd", thisData.Rows(i)("ControlPositionB").ToString())
                writer.WriteAttributeString("TilterPosition", thisData.Rows(i)("TilterPosition").ToString())
                writer.WriteAttributeString("Roll_LSDouble3rd", thisData.Rows(i)("TilterPositionB").ToString())
                writer.WriteAttributeString("PullCordLength", thisData.Rows(i)("ControlLengthValue").ToString())
                writer.WriteAttributeString("Chain_LSDouble3rd", thisData.Rows(i)("ControlLengthValueB").ToString())
                writer.WriteAttributeString("WandLength", thisData.Rows(i)("WandLengthValue").ToString())
                writer.WriteAttributeString("Additional", thisData.Rows(i)("WandLengthValueB").ToString())
                writer.WriteAttributeString("Supply", thisData.Rows(i)("Supply").ToString())

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Cellular Shades" Then
                Dim kitId As String = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")
                Dim webFabricId As String = thisData.Rows(i)("FabricColourId").ToString()
                Dim webFabricIdB As String = thisData.Rows(i)("FabricColourIdB").ToString()

                Dim boeFabricId As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricId & "'")
                Dim boeFabricIdB As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricIdB & "'")

                If String.IsNullOrEmpty(kitId) Then Continue For
                If String.IsNullOrEmpty(boeFabricId) Then Continue For

                Dim controlPosition As String = thisData.Rows(i)("ControlPosition").ToString()
                If controlPosition = "Both Sides" Then controlPosition = "Left and Right"

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", "Cellular Shades")
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("IDHK", kitId)
                writer.WriteAttributeString("FabricID", boeFabricId)
                writer.WriteAttributeString("FabricID_DoubleBracket", boeFabricIdB)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("ControlPosition", controlPosition)
                writer.WriteAttributeString("ControlLength", thisData.Rows(i)("ControlLengthValue").ToString())
                writer.WriteAttributeString("Supply", thisData.Rows(i)("Supply").ToString())

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Curtain" Then
                Dim heading As String = thisData.Rows(i)("Heading").ToString()
                Dim headingB As String = thisData.Rows(i)("HeadingB").ToString()

                Dim kitName As String = blindName & " " & heading
                Dim kitId As String = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "'")
                If blindName = "Track Only" Then
                    kitId = String.Empty
                End If
                If blindName = "Fabric Only" Then
                    kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")
                End If
                Dim kitIdB As String = String.Empty

                If blindName = "Double Curtain & Track" Then kitIdB = kitId

                Dim webFabricId As String = thisData.Rows(i)("FabricColourId").ToString()
                Dim boeFabricId As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricId & "'")

                If String.IsNullOrEmpty(kitId) Then Continue For
                If String.IsNullOrEmpty(boeFabricId) Then Continue For

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", "Curtain")
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("IDHK", kitId)
                writer.WriteAttributeString("FabricID", boeFabricId)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("StackOption", thisData.Rows(i)("StackPosition").ToString())
                writer.WriteAttributeString("ControlType", "Hand")
                writer.WriteAttributeString("BottomRailColour", thisData.Rows(i)("BottomHem").ToString())
                writer.WriteAttributeString("Additional1", thisData.Rows(i)("Supply").ToString())
                writer.WriteAttributeString("ControlLength", thisData.Rows(i)("ReturnLengthValue").ToString())
                writer.WriteAttributeString("ControlLength2", thisData.Rows(i)("ReturnLengthValueB").ToString())

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Design Shades" Then
                Dim kitId As String = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")
                Dim webFabricId As String = thisData.Rows(i)("FabricColourId").ToString()
                Dim boeFabricId As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricId & "'")

                Dim chainId As String = thisData.Rows(i)("ChainId").ToString()
                Dim boeChainId As String = String.Empty
                Dim controlColour As String = GetItemData("SELECT Name FROM Chains WHERE Id='" & chainId & "'")
                Dim controlLength As String = thisData.Rows(i)("ControlLengthValue").ToString()
                If controlName = "Wand" Then
                    controlColour = thisData.Rows(i)("WandColour").ToString()
                    controlLength = thisData.Rows(i)("WandLengthValue").ToString()
                End If
                If controlName = "Chain" Then
                    boeChainId = GetItemData("SELECT BoeId FROM Chains WHERE Id='" & chainId & "'")
                End If

                If String.IsNullOrEmpty(kitId) Then Continue For
                If String.IsNullOrEmpty(boeFabricId) Then Continue For

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", "Profile")
                writer.WriteAttributeString("OrderType", "Profile")
                writer.WriteAttributeString("IDHK", kitId)
                writer.WriteAttributeString("IDChain", boeChainId)
                writer.WriteAttributeString("FabricID", boeFabricId)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                writer.WriteAttributeString("StackOption", thisData.Rows(i)("StackPosition").ToString())
                writer.WriteAttributeString("ControlLength", controlLength)
                writer.WriteAttributeString("ControlColour", controlColour)

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Linea Valance" Then
                Dim kitId As String = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")
                Dim webFabricId As String = thisData.Rows(i)("FabricColourId").ToString()
                Dim boeFabricId As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricId & "'")

                Dim valanceSize As String = String.Empty
                If tubeName = "Valance 100mm" Then valanceSize = "100"
                If tubeName = "Valance 140mm" Then valanceSize = "140"

                Dim valancePosition As String = thisData.Rows(i)("ReturnPosition").ToString()
                If valancePosition = "Both Sides" Then valancePosition = "RL"

                If String.IsNullOrEmpty(kitId) Then Continue For

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", "Linea Valance")
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("IDHK", kitId)
                writer.WriteAttributeString("FabricInsert", thisData.Rows(i)("FabricInsert").ToString())
                writer.WriteAttributeString("FabricID", boeFabricId)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("LineaValance_BracketType", thisData.Rows(i)("BracketType").ToString())
                writer.WriteAttributeString("RollDirection", thisData.Rows(i)("IsBlindIn").ToString())
                writer.WriteAttributeString("Colour", "25")
                writer.WriteAttributeString("ValanceSize", valanceSize)
                writer.WriteAttributeString("ValancePosition", valancePosition)
                writer.WriteAttributeString("ValanceReturnSize", thisData.Rows(i)("ReturnLengthValue").ToString())

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Panel Glide" Then
                Dim idhk As String = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")
                Dim webFabricId As String = thisData.Rows(i)("FabricColourId").ToString()
                Dim boeFabricId As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricId & "'")

                Dim layoutCode As String = thisData.Rows(i)("LayoutCode").ToString()
                If layoutCode = "S" Then
                    layoutCode = thisData.Rows(i)("LayoutCodeCustom").ToString()
                End If

                writer.WriteAttributeString("BlindType", "Panel Glide")
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("TubeName", tubeName)
                writer.WriteAttributeString("IDHK", idhk)
                writer.WriteAttributeString("FabricID", boeFabricId)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("Baton", thisData.Rows(i)("Batten").ToString())
                writer.WriteAttributeString("LayoutCode", layoutCode)
                writer.WriteAttributeString("Tracks", thisData.Rows(i)("TrackType").ToString())
                writer.WriteAttributeString("Panel", thisData.Rows(i)("TrackType").ToString())
                writer.WriteAttributeString("WandLength", thisData.Rows(i)("WandLengthValue").ToString())
            End If

            If designName = "Pelmet" Then
                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
            End If

            If designName = "Roman Blind" Then
                Dim kitId As String = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")
                Dim webFabricId As String = thisData.Rows(i)("FabricColourId").ToString()
                Dim boeFabricId As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricId & "'")

                Dim chainId As String = thisData.Rows(i)("ChainId").ToString()
                Dim boeChainId As String = String.Empty
                Dim mechanismeOption As String = String.Empty
                Dim controlColour As String = GetItemData("SELECT Name FROM Chains WHERE Id='" & chainId & "'")
                If controlName = "Chain" Then
                    boeChainId = GetItemData("SELECT BoeId FROM Chains WHERE Id='" & chainId & "'")
                    mechanismeOption = "Covered"
                End If

                If String.IsNullOrEmpty(kitId) Then Continue For
                If String.IsNullOrEmpty(boeFabricId) Then Continue For

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", "Roman")
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("TubeName", tubeName)
                writer.WriteAttributeString("IDHK", kitId)
                writer.WriteAttributeString("FabricID", boeFabricId)
                writer.WriteAttributeString("IDChain", boeChainId)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("Baton", thisData.Rows(i)("Batten").ToString())
                writer.WriteAttributeString("RomanHR", "Timber")
                writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                writer.WriteAttributeString("RomanMechanismType", controlName)
                writer.WriteAttributeString("RomanChainColour", controlColour)
                writer.WriteAttributeString("RomanChainOption", mechanismeOption)
                writer.WriteAttributeString("RomanChainLength", thisData.Rows(i)("ControlLengthValue").ToString())
                writer.WriteAttributeString("RomanValanceOption", thisData.Rows(i)("ValanceOption").ToString())

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Privacy Venetian" Then
                Dim venId As String = GetItemData("SELECT VenId FROM ProductKits WHERE ProductId='" & productId & "'")

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", "Smart Privacy")
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("IDHK", venId)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                writer.WriteAttributeString("TilterPosition", thisData.Rows(i)("TilterPosition").ToString())
                writer.WriteAttributeString("PullCordLength", thisData.Rows(i)("ControlLengthValue").ToString())
                writer.WriteAttributeString("Supply", thisData.Rows(i)("Supply").ToString())

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Venetian Blind" Then
                Dim subType As String = thisData.Rows(i)("SubType").ToString()
                Dim tassel As String = thisData.Rows(i)("Tassel").ToString()
                Dim venId As String = GetItemData("SELECT VenId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='Semi Metal'")
                If tassel = "Gold" OrElse tassel = "Antique Brass" Then
                    venId = GetItemData("SELECT VenId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='Metal'")
                End If
                Dim venIdB As String = String.Empty
                Dim venIdC As String = String.Empty
                If subType.Contains("2 on 1") Then venIdB = venId
                If subType.Contains("3 on 1") Then venIdB = venId : venIdC = venId

                Dim newSubType As String = "Single"
                If subType = "2 on 1 Left-Left" Then newSubType = "2 on 1 Venetian Left-Left"
                If subType = "2 on 1 Right-Right" Then newSubType = "2 on 1 Venetian Right-Right"
                If subType = "2 on 1 Left-Right" Then newSubType = "2 on 1 Venetian Left-Right"

                Dim valancePosition As String = thisData.Rows(i)("ReturnPosition").ToString()
                If valancePosition = "Both Sides" Then valancePosition = "Right and Left"

                If String.IsNullOrEmpty(venId) Then Continue For

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", "Venetian")
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("SubType", newSubType)
                writer.WriteAttributeString("IDHK", venId)
                writer.WriteAttributeString("IDHK2", venIdB)
                writer.WriteAttributeString("IDHK3", venIdC)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Width2c", thisData.Rows(i)("WidthB").ToString())
                writer.WriteAttributeString("Width_LSIdp3rd", thisData.Rows(i)("WidthC").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                writer.WriteAttributeString("ControlPosition_LSDouble3rd", thisData.Rows(i)("ControlPositionB").ToString())
                writer.WriteAttributeString("Additional", thisData.Rows(i)("ControlPositionC").ToString())
                writer.WriteAttributeString("TilterPosition", thisData.Rows(i)("TilterPosition").ToString())
                writer.WriteAttributeString("Additional2", thisData.Rows(i)("TilterPositionB").ToString())
                writer.WriteAttributeString("Additional1", thisData.Rows(i)("TilterPositionC").ToString())
                writer.WriteAttributeString("PullCordLength", thisData.Rows(i)("ControlLengthValue").ToString())
                writer.WriteAttributeString("ChainLengthdb4", thisData.Rows(i)("ControlLengthValueB").ToString())
                writer.WriteAttributeString("ControlLength", thisData.Rows(i)("ControlLengthValueC").ToString())
                writer.WriteAttributeString("ControlColour", thisData.Rows(i)("Tassel").ToString())
                writer.WriteAttributeString("ValanceType", thisData.Rows(i)("ValanceType").ToString())
                writer.WriteAttributeString("ValanceSize", thisData.Rows(i)("ValanceSizeValue").ToString())
                writer.WriteAttributeString("ValancePosition", valancePosition)
                writer.WriteAttributeString("ValanceReturnSize", thisData.Rows(i)("ReturnLengthValue").ToString())
                writer.WriteAttributeString("Supply", thisData.Rows(i)("Supply").ToString())
                writer.WriteAttributeString("WandLength", thisData.Rows(i)("WandLengthValue").ToString())

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Vertical" Then
                Dim kitId As String = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")

                Dim webFabricId As String = thisData.Rows(i)("FabricColourId").ToString()
                Dim boeFabricId As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricId & "'")

                Dim chainId As String = thisData.Rows(i)("ChainId").ToString()
                Dim boeChainId As String = String.Empty
                If controlName = "Chain" Then boeChainId = GetItemData("SELECT BoeId FROM Chains WHERE Id='" & chainId & "'")
                Dim controlLength As String = thisData.Rows(i)("ControlLengthValue").ToString()
                If controlName = "Wand" Then controlLength = thisData.Rows(i)("WandLengthValue").ToString()

                Dim orderType As String = "Vertical"
                If blindName = "Slat Only" Then orderType = "Vertical Slat Only"

                If String.IsNullOrEmpty(kitId) Then Continue For

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", orderType)
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("IDHK", kitId)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("ControlType", controlName)
                writer.WriteAttributeString("StackOption", thisData.Rows(i)("StackPosition").ToString())
                writer.WriteAttributeString("ControlLength", controlLength)
                writer.WriteAttributeString("IDChain", boeChainId)
                writer.WriteAttributeString("FabricID", boeFabricId)
                writer.WriteAttributeString("FabricInsert", thisData.Rows(i)("FabricInsert").ToString())
                writer.WriteAttributeString("BottomJoin", thisData.Rows(i)("BottomJoining").ToString())
                writer.WriteAttributeString("Vertical_HeadrailColour", colourName)
                writer.WriteAttributeString("Vertical_WandCordColour", colourName)
                writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                writer.WriteAttributeString("Extbracket", thisData.Rows(i)("BracketExtension").ToString())

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Saphora Drape" Then
                Dim kitId As String = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")

                Dim webFabricId As String = thisData.Rows(i)("FabricColourId").ToString()
                Dim boeFabricId As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricId & "'")

                Dim chainId As String = thisData.Rows(i)("ChainId").ToString()
                Dim boeChainId As String = String.Empty
                If controlName = "Chain" Then
                    boeChainId = GetItemData("SELECT BoeId FROM Chains WHERE Id='" & chainId & "'")
                End If
                Dim controlLength As String = thisData.Rows(i)("ControlLengthValue").ToString()
                If controlName = "Wand" Then
                    controlLength = thisData.Rows(i)("WandLengthValue").ToString()
                End If

                If String.IsNullOrEmpty(kitId) Then Continue For
                If String.IsNullOrEmpty(boeFabricId) Then Continue For

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", designName)
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("IDHK", kitId)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("ControlType", controlName)
                writer.WriteAttributeString("StackOption", thisData.Rows(i)("StackPosition").ToString())
                writer.WriteAttributeString("ControlLength", controlLength)
                writer.WriteAttributeString("IDChain", boeChainId)
                writer.WriteAttributeString("FabricID", boeFabricId)
                writer.WriteAttributeString("FabricInsert", thisData.Rows(i)("FabricInsert").ToString())
                writer.WriteAttributeString("BottomJoin", thisData.Rows(i)("BottomJoining").ToString())
                writer.WriteAttributeString("Vertical_HeadrailColour", colourName)
                writer.WriteAttributeString("Vertical_WandCordColour", colourName)
                writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                writer.WriteAttributeString("Extbracket", thisData.Rows(i)("BracketExtension").ToString())

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Roller Blind" Then
                Dim webFabricId As String = thisData.Rows(i)("FabricColourId").ToString()
                Dim boeFabricId As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricId & "'")

                Dim chainId As String = thisData.Rows(i)("ChainId").ToString()
                Dim boeChainId As String = GetItemData("SELECT BoeId FROM Chains WHERE Id='" & chainId & "'")

                Dim bottomColourId As String = thisData.Rows(i)("BottomColourId").ToString()
                Dim boeBottomId As String = GetItemData("SELECT BoeId FROM BottomColours WHERE Id='" & bottomColourId & "'")

                Dim orderType As String = String.Empty
                If blindName = "Single Blind" Then orderType = "Regular Chain"
                If blindName = "Dual Blinds" Then orderType = "Double Bracket"
                If blindName = "Link 2 Blinds Dependent" Then orderType = "Link System Dependent 2 Blinds"
                If blindName = "Link 2 Blinds Independent" Then orderType = "Link System Independent 2 Blinds"
                If blindName = "Link 3 Blinds Dependent" Then orderType = "Link System Dependent 3 Blinds"
                If blindName = "Link 3 Blinds Independent with Dependent" Then orderType = "Link System Independent 3 Blinds"

                Dim kitId As String = String.Empty
                Dim kitIdB As String = String.Empty
                Dim kitIdC As String = String.Empty
                Dim kitIdD As String = String.Empty
                Dim kitIdE As String = String.Empty
                Dim kitIdF As String = String.Empty
                Dim kitIdG As String = String.Empty
                Dim kitIdH As String = String.Empty

                Dim springAssist As String = thisData.Rows(i)("SpringAssist").ToString()

                If blindName = "Single Blind" OrElse blindName = "Full Cassette" OrElse blindName = "Semi Cassette" OrElse blindName = "Wire Guide" Then
                    kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")
                    Dim kitName As String = String.Empty

                    If tubeName = "Standard" AndAlso controlName = "Chain" Then
                        kitName = String.Format("{0} (LD)", productName)

                        Dim width As Integer = thisData.Rows(i)("Width")
                        If width > 1810 Then kitName = String.Format("{0} (HD)", productName)

                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "'")
                    End If

                    If tubeName = "Acmeda 49mm" AndAlso controlName = "Chain" Then
                        kitName = productName
                        If springAssist = "Yes" Then
                            kitName = String.Format("{0} (Spring Assist)", productName)
                        End If
                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "'")
                    End If

                    If String.IsNullOrEmpty(kitId) Then Continue For
                    If String.IsNullOrEmpty(boeFabricId) Then Continue For

                    Dim flatOption As String = thisData.Rows(i)("FlatOption").ToString()
                    If flatOption = "Fabric on Back" Then flatOption = "Fabric on back"
                    If flatOption = "Fabric on Front" Then flatOption = "Fabric on front"

                    writer.WriteStartElement("OrderDetails")
                    writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                    writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                    writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                    writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                    writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())
                    writer.WriteAttributeString("BlindType", "Roller")
                    writer.WriteAttributeString("OrderType", orderType)
                    writer.WriteAttributeString("IDHK", kitId)
                    writer.WriteAttributeString("IDChain", boeChainId)
                    writer.WriteAttributeString("RollerChainLength", thisData.Rows(i)("ControlLengthValue").ToString())
                    writer.WriteAttributeString("Additional", thisData.Rows(i)("ChainStopper").ToString())
                    writer.WriteAttributeString("FabricID", boeFabricId)
                    writer.WriteAttributeString("IDBottomRail", boeBottomId)
                    writer.WriteAttributeString("FlatBottomOp", flatOption)
                    writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                    writer.WriteAttributeString("RollDirection", thisData.Rows(i)("Roll").ToString())
                    writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                    writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                    writer.WriteAttributeString("Panel", thisData.Rows(i)("BracketSize").ToString())
                    writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                    writer.WriteEndElement()
                End If

                If blindName = "Dual Blinds" Then
                    Dim webFabricIdB As String = thisData.Rows(i)("FabricColourIdB").ToString()
                    Dim boeFabricIdB As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricIdB & "'")

                    Dim chainIdB As String = thisData.Rows(i)("ChainIdB").ToString()
                    Dim boeChainIdB As String = GetItemData("SELECT BoeId FROM Chains WHERE Id='" & chainIdB & "'")

                    Dim bottomColourIdB As String = thisData.Rows(i)("BottomColourIdB").ToString()
                    Dim boeBottomIdB As String = GetItemData("SELECT BoeId FROM BottomColours WHERE Id='" & bottomColourIdB & "'")

                    kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")
                    kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")

                    Dim kitName As String = String.Empty

                    If tubeName = "Standard" AndAlso controlName = "Chain" Then
                        kitName = String.Format("{0} (LD)", productName)

                        Dim width As Integer = thisData.Rows(i)("Width")
                        Dim widthB As Integer = thisData.Rows(i)("WidthB")
                        If width > 1810 OrElse widthB > 1810 Then kitName = String.Format("{0} (HD)", productName)

                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "'")
                        kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "'")
                    End If

                    If tubeName = "Acmeda 49mm" AndAlso controlName = "Chain" Then
                        kitName = productName
                        If springAssist = "Yes" Then
                            kitName = String.Format("{0} (Spring Assist)", productName)
                        End If
                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "'")
                        kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "'")
                    End If

                    If String.IsNullOrEmpty(kitId) OrElse String.IsNullOrEmpty(kitIdB) Then Continue For

                    Dim flatOption As String = thisData.Rows(i)("FlatOption").ToString()
                    Dim flatOptionB As String = thisData.Rows(i)("FlatOptionB").ToString()

                    If flatOption = "Fabric on Back" Then flatOption = "Fabric on back"
                    If flatOption = "Fabric on Front" Then flatOption = "Fabric on front"

                    If flatOptionB = "Fabric on Back" Then flatOptionB = "Fabric on back"
                    If flatOptionB = "Fabric on Front" Then flatOptionB = "Fabric on front"

                    writer.WriteStartElement("OrderDetails")
                    writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                    writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                    writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                    writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                    writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())
                    writer.WriteAttributeString("Extbracket", "Yes")
                    writer.WriteAttributeString("BlindType", "Roller")
                    writer.WriteAttributeString("OrderType", orderType)
                    writer.WriteAttributeString("IDHK", kitId)
                    writer.WriteAttributeString("IDHK2", kitIdB)
                    writer.WriteAttributeString("IDChain", boeChainId)
                    writer.WriteAttributeString("RollerChainLength", thisData.Rows(i)("ControlLengthValue").ToString())
                    writer.WriteAttributeString("Additional", thisData.Rows(i)("ChainStopper").ToString())
                    If controlName = "Chain" Then
                        writer.WriteAttributeString("IDChain_DoubleBracket", boeChainIdB)
                        writer.WriteAttributeString("RolChainLength2c", thisData.Rows(i)("ControlLengthValueB").ToString())
                        writer.WriteAttributeString("Additional1", thisData.Rows(i)("ChainStopperB").ToString())
                    End If

                    writer.WriteAttributeString("FabricID", boeFabricId)
                    writer.WriteAttributeString("FabricID_DoubleBracket", boeFabricIdB)
                    writer.WriteAttributeString("IDBottomRail", boeBottomId)
                    writer.WriteAttributeString("FlatBottomOp", flatOption)
                    writer.WriteAttributeString("IDBottomRail_DoubleBracket", boeBottomIdB)
                    writer.WriteAttributeString("FlatBottomOp2a", flatOptionB)
                    writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                    writer.WriteAttributeString("RollDirection", thisData.Rows(i)("Roll").ToString())
                    writer.WriteAttributeString("ControlPosition_LSDouble3rd", thisData.Rows(i)("ControlPositionB").ToString())
                    writer.WriteAttributeString("Roll2a", thisData.Rows(i)("RollB").ToString())
                    writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                    writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                    writer.WriteAttributeString("Panel", thisData.Rows(i)("BracketSize").ToString())
                    writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                    writer.WriteAttributeString("ProductId", productId)
                    writer.WriteEndElement()
                End If

                If blindName = "Link 2 Blinds Dependent" Then
                    kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='Control'")
                    kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='End'")

                    Dim kitName As String = String.Empty

                    If tubeName = "Standard" AndAlso controlName = "Chain" Then
                        kitName = String.Format("{0} (LD)", productName)

                        Dim width As Integer = thisData.Rows(i)("Width")
                        Dim widthB As Integer = thisData.Rows(i)("WidthB")

                        If width > 1810 OrElse widthB > 1810 Then kitName = String.Format("{0} (HD)", productName)

                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Control'")
                        kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='End'")
                    End If

                    If tubeName = "Acmeda 49mm" AndAlso controlName = "Chain" Then
                        kitName = productName
                        If springAssist = "Yes" Then
                            kitName = String.Format("{0} (Spring Assist)", productName)
                        End If
                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Control'")
                        kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='End'")
                    End If

                    If String.IsNullOrEmpty(kitId) OrElse String.IsNullOrEmpty(kitIdB) Then Continue For

                    Dim flatOption As String = thisData.Rows(i)("FlatOption").ToString()
                    Dim flatOptionB As String = thisData.Rows(i)("FlatOptionB").ToString()

                    If flatOption = "Fabric on Back" Then flatOption = "Fabric on back"
                    If flatOption = "Fabric on Front" Then flatOption = "Fabric on front"

                    If flatOptionB = "Fabric on Back" Then flatOptionB = "Fabric on back"
                    If flatOptionB = "Fabric on Front" Then flatOptionB = "Fabric on front"

                    Dim webFabricIdB As String = thisData.Rows(i)("FabricColourIdB").ToString()
                    Dim boeFabricIdB As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricIdB & "'")

                    Dim bottomColourIdB As String = thisData.Rows(i)("BottomColourIdB").ToString()
                    Dim boeBottomIdB As String = GetItemData("SELECT BoeId FROM BottomColours WHERE Id='" & bottomColourIdB & "'")

                    writer.WriteStartElement("OrderDetails")
                    writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                    writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                    writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                    writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                    writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())
                    writer.WriteAttributeString("BlindType", "Roller")
                    writer.WriteAttributeString("OrderType", orderType)
                    writer.WriteAttributeString("IDHK", kitId)
                    writer.WriteAttributeString("IDHK2", kitIdB)
                    writer.WriteAttributeString("IDChain", boeChainId)
                    writer.WriteAttributeString("RollerChainLength", thisData.Rows(i)("ControlLengthValue").ToString())
                    writer.WriteAttributeString("Additional", thisData.Rows(i)("ChainStopper").ToString())
                    writer.WriteAttributeString("FabricID", boeFabricId)
                    writer.WriteAttributeString("FabricID_LinkSys", boeFabricIdB)
                    writer.WriteAttributeString("IDBottomRail", boeBottomId)
                    writer.WriteAttributeString("FlatBottomOp", flatOption)
                    writer.WriteAttributeString("IDBottomRail_LinkSys", boeBottomIdB)
                    writer.WriteAttributeString("FlatBottomOp2b", flatOptionB)
                    writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                    writer.WriteAttributeString("RollDirection", thisData.Rows(i)("Roll").ToString())
                    writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                    writer.WriteAttributeString("Width2b", thisData.Rows(i)("WidthB").ToString())
                    writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                    writer.WriteAttributeString("Panel", thisData.Rows(i)("BracketSize").ToString())
                    writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                    writer.WriteEndElement()
                End If

                If blindName = "Link 3 Blinds Dependent" Then
                    kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='Control'")
                    kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='Middle'")
                    kitIdC = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='End'")

                    Dim kitName As String = String.Empty

                    If tubeName = "Standard" AndAlso controlName = "Chain" Then
                        kitName = String.Format("{0} (LD)", productName)

                        Dim width As Integer = thisData.Rows(i)("Width")
                        Dim widthB As Integer = thisData.Rows(i)("WidthB")
                        Dim widthC As Integer = thisData.Rows(i)("WidthC")

                        If width > 1810 OrElse widthB > 1810 OrElse widthC > 1810 Then kitName = String.Format("{0} (HD)", productName)

                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Control'")
                        kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Middle'")
                        kitIdC = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='End'")
                    End If

                    If tubeName = "Acmeda 49mm" AndAlso controlName = "Chain" Then
                        kitName = productName
                        If springAssist = "Yes" Then
                            kitName = String.Format("{0} (Spring Assist)", productName)
                        End If
                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Control'")
                        kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Middle'")
                        kitIdC = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='End'")
                    End If

                    If String.IsNullOrEmpty(kitId) OrElse String.IsNullOrEmpty(kitIdB) OrElse String.IsNullOrEmpty(kitIdC) Then Continue For

                    Dim webFabricIdB As String = thisData.Rows(i)("FabricColourIdB").ToString()
                    Dim webFabricIdC As String = thisData.Rows(i)("FabricColourIdC").ToString()

                    Dim boeFabricIdB As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricIdB & "'")
                    Dim boeFabricIdC As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricIdC & "'")

                    Dim bottomColourIdB As String = thisData.Rows(i)("BottomColourIdB").ToString()
                    Dim bottomColourIdC As String = thisData.Rows(i)("BottomColourIdC").ToString()

                    Dim boeBottomIdB As String = GetItemData("SELECT BoeId FROM BottomColours WHERE Id='" & bottomColourIdB & "'")
                    Dim boeBottomIdC As String = GetItemData("SELECT BoeId FROM BottomColours WHERE Id='" & bottomColourIdC & "'")

                    Dim flatOption As String = thisData.Rows(i)("FlatOption").ToString()
                    Dim flatOptionB As String = thisData.Rows(i)("FlatOptionB").ToString()
                    Dim flatOptionC As String = thisData.Rows(i)("FlatOptionC").ToString()

                    If flatOption = "Fabric on Back" Then flatOption = "Fabric on back"
                    If flatOption = "Fabric on Front" Then flatOption = "Fabric on front"

                    If flatOptionB = "Fabric on Back" Then flatOptionB = "Fabric on back"
                    If flatOptionB = "Fabric on Front" Then flatOptionB = "Fabric on front"

                    If flatOptionC = "Fabric on Back" Then flatOptionC = "Fabric on back"
                    If flatOptionC = "Fabric on Front" Then flatOptionC = "Fabric on front"

                    writer.WriteStartElement("OrderDetails")
                    writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                    writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                    writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                    writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                    writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())
                    writer.WriteAttributeString("BlindType", "Roller")
                    writer.WriteAttributeString("OrderType", orderType)
                    writer.WriteAttributeString("IDHK", kitId)
                    writer.WriteAttributeString("IDHK2", kitIdB)
                    writer.WriteAttributeString("IDHK3", kitIdC)
                    writer.WriteAttributeString("IDChain", boeChainId)
                    writer.WriteAttributeString("RollerChainLength", thisData.Rows(i)("ControlLengthValue").ToString())
                    writer.WriteAttributeString("Additional", thisData.Rows(i)("ChainStopper").ToString())
                    writer.WriteAttributeString("FabricID", boeFabricId)
                    writer.WriteAttributeString("FabricID_LinkSys", boeFabricIdB)
                    writer.WriteAttributeString("FabricID_LS3rd", boeFabricIdC)
                    writer.WriteAttributeString("IDBottomRail", boeBottomId)
                    writer.WriteAttributeString("FlatBottomOp", flatOption)
                    writer.WriteAttributeString("IDBottomRail_LinkSys", boeBottomIdB)
                    writer.WriteAttributeString("FlatBottomOp2b", flatOptionB)
                    writer.WriteAttributeString("IDBottomRail_LinkSys_3rd", boeBottomIdC)
                    writer.WriteAttributeString("FlatBottomOp3b", flatOptionC)
                    writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                    writer.WriteAttributeString("RollDirection", thisData.Rows(i)("Roll").ToString())
                    writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                    writer.WriteAttributeString("Width2b", thisData.Rows(i)("WidthB").ToString())
                    writer.WriteAttributeString("Width_LS3rd", thisData.Rows(i)("WidthC").ToString())
                    writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                    writer.WriteAttributeString("Panel", thisData.Rows(i)("BracketSize").ToString())
                    writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                    writer.WriteEndElement()
                End If

                If blindName = "Link 2 Blinds Independent" Then
                    kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='Control'")
                    kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='End'")

                    Dim kitName As String = String.Empty

                    If tubeName = "Standard" AndAlso controlName = "Chain" Then
                        kitName = String.Format("{0} (LD)", productName)

                        Dim width As Integer = thisData.Rows(i)("Width")
                        Dim widthB As Integer = thisData.Rows(i)("WidthB")

                        If width > 1810 OrElse widthB > 1810 Then kitName = String.Format("{0} (HD)", productName)

                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Control'")
                        kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='End'")
                    End If

                    If tubeName = "Acmeda 49mm" AndAlso controlName = "Chain" Then
                        kitName = productName
                        If springAssist = "Yes" Then
                            kitName = String.Format("{0} (Spring Assist)", productName)
                        End If
                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Control'")
                        kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='End'")
                    End If

                    If String.IsNullOrEmpty(kitId) OrElse String.IsNullOrEmpty(kitIdB) Then Continue For

                    Dim webFabricIdB As String = thisData.Rows(i)("FabricColourIdB").ToString()
                    Dim boeFabricIdB As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricIdB & "'")

                    Dim chainIdB As String = thisData.Rows(i)("ChainIdB").ToString()
                    Dim boeChainIdB As String = GetItemData("SELECT BoeId FROM Chains WHERE Id='" & chainIdB & "'")

                    Dim bottomColourIdB As String = thisData.Rows(i)("BottomColourIdB").ToString()
                    Dim boeBottomIdB As String = GetItemData("SELECT BoeId FROM BottomColours WHERE Id='" & bottomColourIdB & "'")

                    Dim flatOption As String = thisData.Rows(i)("FlatOption").ToString()
                    Dim flatOptionB As String = thisData.Rows(i)("FlatOptionB").ToString()

                    If flatOption = "Fabric on Back" Then flatOption = "Fabric on back"
                    If flatOption = "Fabric on Front" Then flatOption = "Fabric on front"

                    If flatOptionB = "Fabric on Back" Then flatOptionB = "Fabric on back"
                    If flatOptionB = "Fabric on Front" Then flatOptionB = "Fabric on front"

                    writer.WriteStartElement("OrderDetails")
                    writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                    writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                    writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                    writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                    writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())
                    writer.WriteAttributeString("BlindType", "Roller")
                    writer.WriteAttributeString("OrderType", orderType)
                    writer.WriteAttributeString("IDHK", kitId)
                    writer.WriteAttributeString("IDHK2", kitIdB)
                    writer.WriteAttributeString("IDChain", boeChainId)
                    writer.WriteAttributeString("RollerChainLength", thisData.Rows(i)("ControlLengthValue").ToString())
                    writer.WriteAttributeString("Additional", thisData.Rows(i)("ChainStopper").ToString())
                    If controlName = "Chain" Then
                        writer.WriteAttributeString("IDChain_LinkSysIdp", boeChainIdB)
                        writer.WriteAttributeString("RolChainLength2c", thisData.Rows(i)("ControlLengthValueB").ToString())
                        writer.WriteAttributeString("Additional1", thisData.Rows(i)("ChainStopperB").ToString())
                    End If
                    writer.WriteAttributeString("FabricID", boeFabricId)
                    writer.WriteAttributeString("FabricID_LinkSysIdp", boeFabricIdB)
                    writer.WriteAttributeString("IDBottomRail", boeBottomId)
                    writer.WriteAttributeString("FlatBottomOp", flatOption)
                    writer.WriteAttributeString("IDBottomRail_LinkSysIdp", boeBottomIdB)
                    writer.WriteAttributeString("FlatBottomOp2c", flatOption)
                    writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                    writer.WriteAttributeString("RollDirection", thisData.Rows(i)("Roll").ToString())
                    writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                    writer.WriteAttributeString("Width2c", thisData.Rows(i)("WidthB").ToString())
                    writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                    writer.WriteAttributeString("Panel", thisData.Rows(i)("BracketSize").ToString())
                    writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                    writer.WriteEndElement()
                End If

                If blindName = "Link 3 Blinds Independent width Independent" Then
                    kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='Control'")
                    kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='Middle'")
                    kitIdC = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND BlindStatus='End'")

                    Dim kitName As String = String.Empty

                    If tubeName = "Standard" AndAlso controlName = "Chain" Then
                        kitName = String.Format("{0} (LD)", productName)

                        Dim width As Integer = thisData.Rows(i)("Width")
                        Dim widthB As Integer = thisData.Rows(i)("WidthB")
                        Dim widthC As Integer = thisData.Rows(i)("WidthC")

                        If width > 1810 OrElse widthB > 1810 OrElse widthC > 1810 Then kitName = String.Format("{0} (HD)", productName)

                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Control'")
                        kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Middle'")
                        kitIdC = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='End'")
                    End If

                    If tubeName = "Acmeda 49mm" AndAlso controlName = "Chain" Then
                        kitName = productName
                        If springAssist = "Yes" Then
                            kitName = String.Format("{0} (Spring Assist)", productName)
                        End If
                        kitId = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Control'")
                        kitIdB = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='Middle'")
                        kitIdC = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "' AND Name='" & kitName & "' AND BlindStatus='End'")
                    End If

                    If String.IsNullOrEmpty(kitId) OrElse String.IsNullOrEmpty(kitIdB) Then Continue For

                    Dim webFabricIdB As String = thisData.Rows(i)("FabricColourIdB").ToString()
                    Dim webFabricIdC As String = thisData.Rows(i)("FabricColourIdC").ToString()

                    Dim boeFabricIdB As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricIdB & "'")
                    Dim boeFabricIdC As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricIdC & "'")

                    Dim chainIdB As String = thisData.Rows(i)("ChainIdB").ToString()
                    Dim chainIdC As String = thisData.Rows(i)("ChainIdC").ToString()

                    Dim boeChainIdB As String = GetItemData("SELECT BoeId FROM Chains WHERE Id='" & chainIdB & "'")
                    Dim boeChainIdC As String = GetItemData("SELECT BoeId FROM Chains WHERE Id='" & chainIdC & "'")

                    Dim bottomColourIdB As String = thisData.Rows(i)("BottomColourIdB").ToString()
                    Dim bottomColourIdC As String = thisData.Rows(i)("BottomColourIdC").ToString()

                    Dim boeBottomIdB As String = GetItemData("SELECT BoeId FROM BottomColours WHERE Id='" & bottomColourIdB & "'")
                    Dim boeBottomIdC As String = GetItemData("SELECT BoeId FROM BottomColours WHERE Id='" & bottomColourIdC & "'")

                    Dim flatOption As String = thisData.Rows(i)("FlatOption").ToString()
                    Dim flatOptionB As String = thisData.Rows(i)("FlatOptionB").ToString()
                    Dim flatOptionC As String = thisData.Rows(i)("FlatOptionC").ToString()

                    If flatOption = "Fabric on Back" Then flatOption = "Fabric on back"
                    If flatOption = "Fabric on Front" Then flatOption = "Fabric on front"

                    If flatOptionB = "Fabric on Back" Then flatOptionB = "Fabric on back"
                    If flatOptionB = "Fabric on Front" Then flatOptionB = "Fabric on front"

                    If flatOptionC = "Fabric on Back" Then flatOptionC = "Fabric on back"
                    If flatOptionC = "Fabric on Front" Then flatOptionC = "Fabric on front"

                    writer.WriteStartElement("OrderDetails")
                    writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                    writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                    writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                    writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                    writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())
                    writer.WriteAttributeString("BlindType", "Roller")
                    writer.WriteAttributeString("OrderType", orderType)
                    writer.WriteAttributeString("IDHK", kitId)
                    writer.WriteAttributeString("IDHK2", kitIdB)
                    writer.WriteAttributeString("IDHK3", kitIdC)
                    writer.WriteAttributeString("IDChain", boeChainId)
                    writer.WriteAttributeString("RollerChainLength", thisData.Rows(i)("ControlLengthValue").ToString())
                    writer.WriteAttributeString("Additional", thisData.Rows(i)("ChainStopper").ToString())
                    If controlName = "Chain" Then
                        writer.WriteAttributeString("IDChain_LSIdp3rd", boeChainIdC)
                        writer.WriteAttributeString("ChainLength_LSIdp3rd", thisData.Rows(i)("ControlLengthValueC").ToString())
                        writer.WriteAttributeString("Additional2", thisData.Rows(i)("ChainStopperC").ToString())
                    End If
                    writer.WriteAttributeString("FabricID", boeFabricId)
                    writer.WriteAttributeString("FabricID_LinkSysIdp", boeFabricIdB)
                    writer.WriteAttributeString("FabricID_LSIdp3rd", boeFabricIdC)
                    writer.WriteAttributeString("IDBottomRail", boeBottomId)
                    writer.WriteAttributeString("FlatBottomOp", flatOption)
                    writer.WriteAttributeString("IDBottomRail_LinkSysIdp", boeBottomIdB)
                    writer.WriteAttributeString("FlatBottomOp2c", flatOptionB)
                    writer.WriteAttributeString("IDBottomRail_LinkSysIdp_3rd", boeBottomIdC)
                    writer.WriteAttributeString("FlatBottomOp3c", flatOptionC)
                    writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                    writer.WriteAttributeString("RollDirection", thisData.Rows(i)("Roll").ToString())
                    writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                    writer.WriteAttributeString("Width2c", thisData.Rows(i)("WidthB").ToString())
                    writer.WriteAttributeString("Width_LSIdp3rd", thisData.Rows(i)("WidthC").ToString())
                    writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                    writer.WriteAttributeString("Panel", thisData.Rows(i)("BracketSize").ToString())
                    writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                    writer.WriteEndElement()
                End If
            End If

            If designName = "Outdoor" Then
                Dim kitId As String = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")

                Dim webFabricId As String = thisData.Rows(i)("FabricColourId").ToString()
                Dim boeFabricId As String = GetItemData("SELECT BoeId FROM FabricColours WHERE Id='" & webFabricId & "'")

                If String.IsNullOrEmpty(kitId) Then Continue For
                If String.IsNullOrEmpty(boeFabricId) Then Continue For

                writer.WriteStartElement("OrderDetails")
                writer.WriteAttributeString("OrddID", thisData.Rows(i)("Id").ToString())
                writer.WriteAttributeString("FKOrdID", thisData.Rows(i)("HeaderId").ToString())
                writer.WriteAttributeString("Qty", thisData.Rows(i)("Qty").ToString())
                writer.WriteAttributeString("Room", thisData.Rows(i)("Room").ToString())
                writer.WriteAttributeString("Mounting", thisData.Rows(i)("Mounting").ToString())

                writer.WriteAttributeString("BlindType", designName)
                writer.WriteAttributeString("OrderType", blindName)
                writer.WriteAttributeString("IDHK", kitId)
                writer.WriteAttributeString("Width", thisData.Rows(i)("Width").ToString())
                writer.WriteAttributeString("Drop", thisData.Rows(i)("Drop").ToString())
                writer.WriteAttributeString("ControlType", controlName)
                writer.WriteAttributeString("ControlPosition", thisData.Rows(i)("ControlPosition").ToString())
                writer.WriteAttributeString("ControlLength", thisData.Rows(i)("ControlLength").ToString())
                writer.WriteAttributeString("FabricID", boeFabricId)

                writer.WriteAttributeString("TotalItems", thisData.Rows(i)("TotalItems").ToString())
                writer.WriteAttributeString("MarkUp", thisData.Rows(i)("MarkUp").ToString())
                writer.WriteAttributeString("Notes", thisData.Rows(i)("Notes").ToString())
                writer.WriteEndElement()
            End If

            If designName = "Sample" Then
                Dim kitId As String = GetItemData("SELECT KitId FROM ProductKits WHERE ProductId='" & productId & "'")

                If String.IsNullOrEmpty(kitId) Then Continue For
            End If
        Next
        writer.WriteEndElement()
        writer.WriteEndDocument()
        writer.Close()
    End Sub
</script>
