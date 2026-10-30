<%@ WebHandler Language="VB" Class="Sunlight" %>

Imports System
Imports System.Data
Imports System.IO
Imports System.Web
Imports System.Web.Configuration
Imports System.Web.Script.Serialization
Imports System.Data.SqlClient

Public Class Sunlight : Implements IHttpHandler

    Private ReadOnly orderClass As New OrderClass
    Private ReadOnly mailingClass As New MailingClass

    Private ReadOnly myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"
        Dim serializer As New JavaScriptSerializer()
        Try
            Dim apiKeyHeader As String = context.Request.Headers("X-API-KEY")
            Dim validApiKey As String = WebConfigurationManager.AppSettings("ApiKey")

            If String.IsNullOrEmpty(apiKeyHeader) OrElse String.IsNullOrEmpty(validApiKey) OrElse apiKeyHeader <> validApiKey Then
                WriteJson(context, 401, New With {.status = "error", .message = "Invalid or missing API key"})
                Return
            End If

            If context.Request.HttpMethod <> "POST" Then
                WriteJson(context, 405, New With {.status = "error", .message = "Method not allowed"})
                Return
            End If

            Dim jsonString As String
            Using reader As New StreamReader(context.Request.InputStream)
                jsonString = reader.ReadToEnd()
            End Using

            If String.IsNullOrWhiteSpace(jsonString) Then
                WriteJson(context, 400, New With {.status = "error", .message = "Request body is empty"})
                Return
            End If

            Dim orderData As OrderData = serializer.Deserialize(Of OrderData)(jsonString)
            If orderData Is Nothing Then
                WriteJson(context, 400, New With {.status = "error", .message = "Invalid JSON data"})
                Return
            End If

            If orderData.Details Is Nothing OrElse orderData.Details.Count = 0 Then
                WriteJson(context, 400, New With {.status = "error", .message = "Order details are required"})
                Return
            End If

            Dim headerId As String = orderClass.GetNewOrderHeaderId()

            InsertOrderHeader(headerId, orderData)
            For Each detail As Dictionary(Of String, Object) In orderData.Details
                If detail Is Nothing Then
                    Throw New Exception("Order detail cannot be empty.")
                End If

                InsertOrderDetail(headerId, detail)
            Next

            'PINDAHKAN DARI ORDERDETAILS_SUNLIGHT KE ORDERDETAILS
            Dim itemData As DataTable = orderClass.GetDataTable("SELECT * FROM OrderDetails_Sunlight WHERE HeaderId='" & headerId & "' ORDER BY Id ASC")
            For iItem As Integer = 0 To itemData.Rows.Count - 1
                Dim kitName As String = itemData.Rows(iItem)("KitName").ToString()
                Dim bracketType As String = itemData.Rows(iItem)("BracketType").ToString()
                Dim tubeType As String = itemData.Rows(iItem)("TubeType").ToString()
                Dim controlType As String = itemData.Rows(iItem)("ControlType").ToString()
                Dim colourType As String = itemData.Rows(iItem)("ColourType").ToString()
                Dim designName As String = itemData.Rows(iItem)("DesignName").ToString()
                Dim blindName As String = itemData.Rows(iItem)("BlindName").ToString()
                Dim fabricType As String = itemData.Rows(iItem)("FabricType").ToString()
                Dim fabricColour As String = itemData.Rows(iItem)("FabricColour").ToString()
                Dim uniqueId As String = itemData.Rows(iItem)("UniqueId").ToString()
                Dim blindNo As String = itemData.Rows(iItem)("BlindNo").ToString()
                Dim qty As Integer = If(IsDBNull(itemData.Rows(iItem)("Qty")), 0, Convert.ToInt32(itemData.Rows(iItem)("Qty")))
                Dim room As String = itemData.Rows(iItem)("Room").ToString()
                Dim mounting As String = itemData.Rows(iItem)("Mounting").ToString()
                Dim width As Integer = If(IsDBNull(itemData.Rows(iItem)("Width")), 0, Convert.ToInt32(itemData.Rows(iItem)("Width")))
                Dim drop As Integer = If(IsDBNull(itemData.Rows(iItem)("Drop")), 0, Convert.ToInt32(itemData.Rows(iItem)("Drop")))
                Dim layout As String = itemData.Rows(iItem)("Layout").ToString()
                Dim numOfPanel As Integer = If(IsDBNull(itemData.Rows(iItem)("NumOfPanel")), 0, Convert.ToInt32(itemData.Rows(iItem)("NumOfPanel")))
                Dim twoOnOneHeadreal As String = itemData.Rows(iItem)("TwoOnOneHeadreal").ToString()
                Dim controlPosition As String = itemData.Rows(iItem)("ControlPosition").ToString()
                Dim controlLength As Integer = If(IsDBNull(itemData.Rows(iItem)("ControlLength")), 0, Convert.ToInt32(itemData.Rows(iItem)("ControlLength")))
                Dim controlLift As String = itemData.Rows(iItem)("ControlLift").ToString()
                Dim controlTilt As String = itemData.Rows(iItem)("ControlTilt").ToString()
                Dim tilterPosition As String = itemData.Rows(iItem)("TilterPosition").ToString()
                Dim chainLength As Integer = If(IsDBNull(itemData.Rows(iItem)("ChainLength")), 0, Convert.ToInt32(itemData.Rows(iItem)("ChainLength")))
                Dim materialChain As String = itemData.Rows(iItem)("MaterialChain").ToString()
                Dim chainColour As String = itemData.Rows(iItem)("ChainColour").ToString()
                Dim trackType As String = itemData.Rows(iItem)("TrackType").ToString()
                Dim trackColour As String = itemData.Rows(iItem)("TrackColour").ToString()
                Dim numOfWand As Integer = If(IsDBNull(itemData.Rows(iItem)("NumOfWand")), 0, Convert.ToInt32(itemData.Rows(iItem)("NumOfWand")))
                Dim wandPosition As String = itemData.Rows(iItem)("WandPosition").ToString()
                Dim wandColour As String = itemData.Rows(iItem)("WandColour").ToString()
                Dim wandLength As Integer = If(IsDBNull(itemData.Rows(iItem)("WandLength")), 0, Convert.ToInt32(itemData.Rows(iItem)("WandLength")))
                Dim cordColour As String = itemData.Rows(iItem)("CordColour").ToString()
                Dim cordLength As Integer = If(IsDBNull(itemData.Rows(iItem)("CordLength")), 0, Convert.ToInt32(itemData.Rows(iItem)("CordLength")))
                Dim acornPlasticColour As String = itemData.Rows(iItem)("AcornPlasticColour").ToString()
                Dim batten As String = itemData.Rows(iItem)("Batten").ToString()
                Dim battenColour As String = itemData.Rows(iItem)("BattenColour").ToString()
                Dim bracketOption As String = itemData.Rows(iItem)("BracketOption").ToString()
                Dim bracketColour As String = itemData.Rows(iItem)("BracketColour").ToString()
                Dim fitting As String = itemData.Rows(iItem)("Fitting").ToString()
                Dim cleat As String = itemData.Rows(iItem)("Cleat").ToString()
                Dim bottomHoldDown As String = itemData.Rows(iItem)("BottomHoldDown").ToString()
                Dim pelmetType As String = itemData.Rows(iItem)("PelmetType").ToString()
                Dim pelmetWidth As Integer = If(IsDBNull(itemData.Rows(iItem)("PelmetWidth")), 0, Convert.ToInt32(itemData.Rows(iItem)("PelmetWidth")))
                Dim pelmetReturnLeft As Integer = If(IsDBNull(itemData.Rows(iItem)("PelmetReturnLeft")), 0, Convert.ToInt32(itemData.Rows(iItem)("PelmetReturnLeft")))
                Dim pelmetReturnRight As Integer = If(IsDBNull(itemData.Rows(iItem)("PelmetReturnRight")), 0, Convert.ToInt32(itemData.Rows(iItem)("PelmetReturnRight")))
                Dim cutOut_LeftTop As Integer = If(IsDBNull(itemData.Rows(iItem)("CutOut_LeftTop")), 0, Convert.ToInt32(itemData.Rows(iItem)("CutOut_LeftTop")))
                Dim cutOut_RightTop As Integer = If(IsDBNull(itemData.Rows(iItem)("CutOut_RightTop")), 0, Convert.ToInt32(itemData.Rows(iItem)("CutOut_RightTop")))
                Dim cutOut_LeftBottom As Integer = If(IsDBNull(itemData.Rows(iItem)("CutOut_LeftBottom")), 0, Convert.ToInt32(itemData.Rows(iItem)("CutOut_LeftBottom")))
                Dim cutOut_RightBottom As Integer = If(IsDBNull(itemData.Rows(iItem)("CutOut_RightBottom")), 0, Convert.ToInt32(itemData.Rows(iItem)("CutOut_RightBottom")))
                Dim lhsWidth_Top As Integer = If(IsDBNull(itemData.Rows(iItem)("LHSWidth_Top")), 0, Convert.ToInt32(itemData.Rows(iItem)("LHSWidth_Top")))
                Dim lhsHeight_Top As Integer = If(IsDBNull(itemData.Rows(iItem)("LHSHeight_Top")), 0, Convert.ToInt32(itemData.Rows(iItem)("LHSHeight_Top")))
                Dim rhsWidth_Top As Integer = If(IsDBNull(itemData.Rows(iItem)("RHSWidth_Top")), 0, Convert.ToInt32(itemData.Rows(iItem)("RHSWidth_Top")))
                Dim rhsHeight_Top As Integer = If(IsDBNull(itemData.Rows(iItem)("RHSHeight_Top")), 0, Convert.ToInt32(itemData.Rows(iItem)("RHSHeight_Top")))
                Dim lhsWidth_Bottom As Integer = If(IsDBNull(itemData.Rows(iItem)("LHSWidth_Bottom")), 0, Convert.ToInt32(itemData.Rows(iItem)("LHSWidth_Bottom")))
                Dim lhsHeight_Bottom As Integer = If(IsDBNull(itemData.Rows(iItem)("LHSHeight_Bottom")), 0, Convert.ToInt32(itemData.Rows(iItem)("LHSHeight_Bottom")))
                Dim rhsWidth_Bottom As Integer = If(IsDBNull(itemData.Rows(iItem)("RHSWidth_Bottom")), 0, Convert.ToInt32(itemData.Rows(iItem)("RHSWidth_Bottom")))
                Dim rhsHeight_Bottom As Integer = If(IsDBNull(itemData.Rows(iItem)("RHSHeight_Bottom")), 0, Convert.ToInt32(itemData.Rows(iItem)("RHSHeight_Bottom")))
                Dim notes As String = itemData.Rows(iItem)("Notes").ToString()

                If designName = "Aluminium Blinds" Then
                    Dim designName_Fix = "Aluminium Blind"
                    Dim blindName_Fix As String = "0.21mm"
                    Dim priceGroup As String = "SP"

                    Dim designId As String = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='" & designName_Fix & "'")
                    Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds WHERE Name='" & blindName_Fix & "' AND DesignId='" & designId & "'")
                    Dim productColourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colourType & "'")
                    Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS subCompanyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='9' AND ControlType='17' AND ColourType='" & productColourId & "' AND subCompanyArray.VALUE='7'")

                    Dim productGroupName As String = String.Format("{0} - {1}", designName_Fix, blindName_Fix)
                    Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(productGroupName, designId, "14")

                    Dim controlLength_Fix As String = String.Empty
                    Dim controlLengthValue As Integer = 0
                    If controlLength = 0 Then
                        controlLength_Fix = "Standard"
                        controlLengthValue = CInt(Math.Ceiling(drop * 2.0 / 3.0))
                        If controlLengthValue < 550 Then controlLengthValue = 550
                    End If
                    If controlLength > 0 Then
                        controlLength_Fix = "Custom"
                        controlLengthValue = controlLength
                    End If

                    Dim controlPosition_Fix As String = String.Empty
                    If controlPosition = "LHC" Then
                        controlPosition_Fix = "Left"
                    ElseIf controlPosition = "RHC" Then
                        controlPosition_Fix = "Right"
                    End If

                    Dim cutOut As String = String.Empty
                    If cutOut_LeftTop = 1 OrElse cutOut_RightTop = 1 OrElse cutOut_LeftBottom = 1 OrElse cutOut_RightBottom = 1 Then
                        cutOut = "Yes"
                    End If

                    Dim supply As String = bottomHoldDown
                    If supply = "No" Then supply = String.Empty

                    Dim linearMetre As Decimal = width / 1000D
                    Dim squareMetre As Decimal = CDec(width) * CDec(drop) / 1000000D

                    For i As Integer = 1 To qty
                        Dim itemId As String = orderClass.GetNewOrderItemId()
                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As New SqlCommand("sp_OrderDetails_Insert_Aluminium", thisConn)
                                thisCmd.CommandType = CommandType.StoredProcedure

                                thisCmd.Parameters.AddWithValue("@Id", itemId)
                                thisCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                thisCmd.Parameters.AddWithValue("@ProductId", productId)
                                thisCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                thisCmd.Parameters.AddWithValue("@PriceProductGroupIdB", DBNull.Value)
                                thisCmd.Parameters.AddWithValue("@Room", room)
                                thisCmd.Parameters.AddWithValue("@Mounting", mounting)
                                thisCmd.Parameters.AddWithValue("@SubType", "Single")
                                thisCmd.Parameters.AddWithValue("@ControlPosition", controlPosition_Fix)
                                thisCmd.Parameters.AddWithValue("@TilterPosition", controlPosition_Fix)
                                thisCmd.Parameters.AddWithValue("@Width", width)
                                thisCmd.Parameters.AddWithValue("@Drop", drop)
                                thisCmd.Parameters.AddWithValue("@ControlLength", controlLength_Fix)
                                thisCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                thisCmd.Parameters.AddWithValue("@WandLength", controlLength_Fix)
                                thisCmd.Parameters.AddWithValue("@WandLengthValue", controlLengthValue)
                                thisCmd.Parameters.AddWithValue("@ControlPositionB", String.Empty)
                                thisCmd.Parameters.AddWithValue("@TilterPositionB", String.Empty)
                                thisCmd.Parameters.AddWithValue("@WidthB", 0)
                                thisCmd.Parameters.AddWithValue("@DropB", 0)
                                thisCmd.Parameters.AddWithValue("@ControlLengthB", String.Empty)
                                thisCmd.Parameters.AddWithValue("@ControlLengthValueB", 0)
                                thisCmd.Parameters.AddWithValue("@WandLengthB", String.Empty)
                                thisCmd.Parameters.AddWithValue("@WandLengthValueB", 0)
                                thisCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                                thisCmd.Parameters.AddWithValue("@LinearMetreB", 0)
                                thisCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                                thisCmd.Parameters.AddWithValue("@SquareMetreB", 0)
                                thisCmd.Parameters.AddWithValue("@CutOut", cutOut)
                                thisCmd.Parameters.AddWithValue("@CutTopLeftWidth", lhsWidth_Top)
                                thisCmd.Parameters.AddWithValue("@CutTopLeftHeight", lhsHeight_Top)
                                thisCmd.Parameters.AddWithValue("@CutTopRightWidth", rhsWidth_Top)
                                thisCmd.Parameters.AddWithValue("@CutTopRightHeight", rhsHeight_Top)
                                thisCmd.Parameters.AddWithValue("@CutBottomLeftWidth", lhsWidth_Bottom)
                                thisCmd.Parameters.AddWithValue("@CutBottomLeftHeight", lhsHeight_Bottom)
                                thisCmd.Parameters.AddWithValue("@CutBottomRightWidth", rhsWidth_Bottom)
                                thisCmd.Parameters.AddWithValue("@CutBottomRightHeight", rhsHeight_Bottom)
                                thisCmd.Parameters.AddWithValue("@CutOutB", String.Empty)
                                thisCmd.Parameters.AddWithValue("@CutTopLeftWidthB", 0)
                                thisCmd.Parameters.AddWithValue("@CutTopLeftHeightB", 0)
                                thisCmd.Parameters.AddWithValue("@CutTopRightWidthB", 0)
                                thisCmd.Parameters.AddWithValue("@CutTopRightHeightB", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomLeftWidthB", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomLeftHeightB", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomRightWidthB", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomRightHeightB", 0)
                                thisCmd.Parameters.AddWithValue("@Supply", supply)
                                thisCmd.Parameters.AddWithValue("@TotalItems", 1)
                                thisCmd.Parameters.AddWithValue("@Notes", If(notes Is Nothing, "", notes))
                                thisCmd.Parameters.AddWithValue("@MarkUp", 0)
                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
                            End Using
                        End Using
                        orderClass.ResetPriceDetail(headerId, itemId)
                        orderClass.CalculatePrice(headerId, itemId)
                        orderClass.FinalCostItem(headerId, itemId)

                        Dim dataLog As Object() = {"OrderDetails", itemId, 2, "Order Item Added"}
                        orderClass.Logs(dataLog)
                    Next
                    orderClass.UpdateOrderFactory(headerId)
                End If

                If designName = "Venetian Blinds" Then
                    Dim designName_Fix As String = "Venetian Blind"
                    Dim designId As String = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='" & designName_Fix & "'")

                    Dim blindName_Fix As String = String.Empty
                    If blindName = "50mm Wooden" Then blindName_Fix = "Basswood 50mm"
                    If blindName = "63mm Wooden" Then blindName_Fix = "Basswood 63mm"
                    If blindName = "50mm Mockwood" Then blindName_Fix = "Econo 50mm"
                    If blindName = "50mm Mockwood (Cordless)" Then blindName_Fix = "Econo 50mm (Cordless)"
                    If blindName = "63mm Mockwood" Then blindName_Fix = "Econo 63mm"
                    Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds WHERE Name='" & blindName_Fix & "' AND DesignId='" & designId & "'")

                    Dim productColourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colourType & "'")
                    Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS subCompanyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='9' AND ControlType='17' AND ColourType='" & productColourId & "' AND subCompanyArray.VALUE='7'")

                    Dim productGroupName As String = String.Format("{0} - {1}", designName_Fix, blindName_Fix)
                    Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(productGroupName, designId, "14")

                    Dim controlPosition_Fix As String = controlLift
                    Dim tilterPosition_Fix As String = controlTilt
                    If blindName_Fix = "Econo 50mm (Cordless)" Then
                        controlPosition_Fix = String.Empty
                        tilterPosition_Fix = tilterPosition
                    End If

                    Dim controlLength_Fix As String = "Custom"
                    Dim controlLengthValue As Integer = 0
                    Dim wandLengthValue As Integer = 0
                    If controlLength = 0 Then
                        controlLength_Fix = "Standard"
                        controlLengthValue = Math.Ceiling(drop * 2 / 3)
                        If controlLengthValue < 550 Then controlLengthValue = 550
                    End If
                    If controlLength > 0 Then
                        controlLength_Fix = "Custom"
                        controlLengthValue = controlLength
                    End If
                    If blindName_Fix = "Econo 50mm (Cordless)" Then
                        controlLength_Fix = "Custom"
                        controlLengthValue = wandLength
                        wandLengthValue = wandLength
                    End If

                    Dim valanceType As String = String.Empty
                    If blindName_Fix = "Basswood 50mm" OrElse blindName_Fix = "Basswood 63mm" Then
                        valanceType = "75mm Valance"
                    End If
                    If blindName_Fix = "Econo 50mm" OrElse blindName_Fix = "Econo 63mm" Then
                        valanceType = "76mm Valance"
                    End If
                    If blindName_Fix = "Econo 50mm (Cordless)" Then
                        valanceType = "Standard"
                    End If

                    Dim valanceSize As String = String.Empty
                    Dim valanceSizeValue As Integer = pelmetWidth

                    Dim returnPosition As String = String.Empty
                    Dim returnLength As String = String.Empty
                    Dim returnLengthValue As Integer = 0

                    If pelmetType = "With Return" Then
                        returnPosition = "Both Sides"

                        If pelmetReturnLeft = 0 Then
                            returnLength = "Standard"
                            returnLengthValue = 70
                            If blindName.Contains("Econo") Then returnLengthValue = 77
                            If mounting = "Opening Size Reveal Fit" OrElse mounting = "Make Size Reveal Fit" Then
                                returnLengthValue = 20
                            End If
                        End If
                        If pelmetReturnLeft > 0 Then
                            returnLength = "Standard"
                            returnLengthValue = pelmetReturnLeft
                        End If
                    End If
                    If pelmetType = "Single Left Return" Then
                        returnPosition = "Left"
                        If pelmetReturnLeft = 0 Then
                            returnLength = "Standard"
                            returnLengthValue = 70
                            If blindName.Contains("Econo") Then returnLengthValue = 77
                            If mounting = "Opening Size Reveal Fit" OrElse mounting = "Make Size Reveal Fit" Then
                                returnLengthValue = 20
                            End If
                        End If
                        If pelmetReturnLeft > 0 Then
                            returnLength = "Standard"
                            returnLengthValue = pelmetReturnLeft
                        End If
                    End If
                    If pelmetType = "Single Right Return" Then
                        returnPosition = "Right"

                        If pelmetReturnRight = 0 Then
                            returnLength = "Standard"
                            returnLengthValue = 70
                            If blindName.Contains("Econo") Then returnLengthValue = 77
                            If mounting = "Opening Size Reveal Fit" OrElse mounting = "Make Size Reveal Fit" Then
                                returnLengthValue = 20
                            End If
                        End If
                        If pelmetReturnRight > 0 Then
                            returnLength = "Standard"
                            returnLengthValue = pelmetReturnRight
                        End If
                    End If

                    If pelmetWidth = 0 Then
                        valanceSize = "Standard"

                        If mounting = "Opening Size Reveal Fit" Then valanceSizeValue = width - 1
                        If mounting = "Make Size Reveal Fit" Then valanceSizeValue = width + 9
                        If mounting = "Opening Size Face Fit" Then valanceSizeValue = width + 20
                        If mounting = "Make Size Face Fit" Then valanceSizeValue = width + 20
                    End If
                    If pelmetWidth > 0 Then
                        valanceSize = "Custom"
                        valanceSizeValue = pelmetWidth
                    End If

                    Dim cutOut As String = String.Empty
                    If cutOut_LeftTop = 1 OrElse cutOut_RightTop = 1 OrElse cutOut_LeftBottom = 1 OrElse cutOut_RightBottom = 1 Then
                        cutOut = "Yes"
                    End If

                    Dim supply As String = bottomHoldDown
                    If supply = "No" Then supply = String.Empty

                    Dim linearMetre As Decimal = width / 1000D
                    Dim squareMetre As Decimal = CDec(width) * CDec(drop) / 1000000D

                    For i As Integer = 1 To qty
                        Dim itemId As String = orderClass.GetNewOrderItemId()

                        Using thisConn As SqlConnection = New SqlConnection(myConn)
                            Using thisCmd As New SqlCommand("sp_OrderDetails_Insert_Venetian", thisConn)
                                thisCmd.CommandType = CommandType.StoredProcedure

                                thisCmd.Parameters.AddWithValue("@Id", itemId)
                                thisCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                thisCmd.Parameters.AddWithValue("@ProductId", productId)
                                thisCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                thisCmd.Parameters.AddWithValue("@PriceProductGroupIdB", CType(DBNull.Value, Object))
                                thisCmd.Parameters.AddWithValue("@PriceProductGroupIdC", CType(DBNull.Value, Object))
                                thisCmd.Parameters.AddWithValue("@Room", room)
                                thisCmd.Parameters.AddWithValue("@Mounting", mounting)
                                thisCmd.Parameters.AddWithValue("@SubType", "Single")

                                thisCmd.Parameters.AddWithValue("@ControlPosition", controlPosition_Fix)
                                thisCmd.Parameters.AddWithValue("@ControlPositionB", String.Empty)
                                thisCmd.Parameters.AddWithValue("@ControlPositionC", String.Empty)

                                thisCmd.Parameters.AddWithValue("@TilterPosition", tilterPosition_Fix)
                                thisCmd.Parameters.AddWithValue("@TilterPositionB", String.Empty)
                                thisCmd.Parameters.AddWithValue("@TilterPositionC", String.Empty)

                                thisCmd.Parameters.AddWithValue("@Width", width)
                                thisCmd.Parameters.AddWithValue("@WidthB", 0)
                                thisCmd.Parameters.AddWithValue("@WidthC", 0)
                                thisCmd.Parameters.AddWithValue("@Drop", drop)
                                thisCmd.Parameters.AddWithValue("@DropB", 0)
                                thisCmd.Parameters.AddWithValue("@DropC", 0)

                                thisCmd.Parameters.AddWithValue("@ControlLength", controlLength_Fix)
                                thisCmd.Parameters.AddWithValue("@ControlLengthB", String.Empty)
                                thisCmd.Parameters.AddWithValue("@ControlLengthC", String.Empty)
                                thisCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                thisCmd.Parameters.AddWithValue("@ControlLengthValueB", 0)
                                thisCmd.Parameters.AddWithValue("@ControlLengthValueC", 0)
                                thisCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)

                                thisCmd.Parameters.AddWithValue("@ValanceType", valanceType)
                                thisCmd.Parameters.AddWithValue("@ValanceSize", valanceSize)
                                thisCmd.Parameters.AddWithValue("@ValanceSizeValue", valanceSizeValue)

                                thisCmd.Parameters.AddWithValue("@ReturnPosition", returnPosition)
                                thisCmd.Parameters.AddWithValue("@ReturnLength", returnLength)
                                thisCmd.Parameters.AddWithValue("@ReturnLengthValue", returnLengthValue)

                                thisCmd.Parameters.AddWithValue("@CutOut", cutOut)
                                thisCmd.Parameters.AddWithValue("@CutTopLeftWidth", lhsWidth_Top)
                                thisCmd.Parameters.AddWithValue("@CutTopLeftHeight", lhsHeight_Top)
                                thisCmd.Parameters.AddWithValue("@CutTopRightWidth", rhsWidth_Top)
                                thisCmd.Parameters.AddWithValue("@CutTopRightHeight", rhsHeight_Top)
                                thisCmd.Parameters.AddWithValue("@CutBottomLeftWidth", lhsWidth_Bottom)
                                thisCmd.Parameters.AddWithValue("@CutBottomLeftHeight", lhsHeight_Bottom)
                                thisCmd.Parameters.AddWithValue("@CutBottomRightWidth", rhsWidth_Bottom)
                                thisCmd.Parameters.AddWithValue("@CutBottomRightHeight", rhsHeight_Bottom)
                                thisCmd.Parameters.AddWithValue("@CutOutB", String.Empty)
                                thisCmd.Parameters.AddWithValue("@CutTopLeftWidthB", 0)
                                thisCmd.Parameters.AddWithValue("@CutTopLeftHeightB", 0)
                                thisCmd.Parameters.AddWithValue("@CutTopRightWidthB", 0)
                                thisCmd.Parameters.AddWithValue("@CutTopRightHeightB", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomLeftWidthB", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomLeftHeightB", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomRightWidthB", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomRightHeightB", 0)
                                thisCmd.Parameters.AddWithValue("@CutOutC", String.Empty)
                                thisCmd.Parameters.AddWithValue("@CutTopLeftWidthC", 0)
                                thisCmd.Parameters.AddWithValue("@CutTopLeftHeightC", 0)
                                thisCmd.Parameters.AddWithValue("@CutTopRightWidthC", 0)
                                thisCmd.Parameters.AddWithValue("@CutTopRightHeightC", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomLeftWidthC", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomLeftHeightC", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomRightWidthC", 0)
                                thisCmd.Parameters.AddWithValue("@CutBottomRightHeightC", 0)

                                thisCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                                thisCmd.Parameters.AddWithValue("@LinearMetreB", 0)
                                thisCmd.Parameters.AddWithValue("@LinearMetreC", 0)

                                thisCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                                thisCmd.Parameters.AddWithValue("@SquareMetreB", 0)
                                thisCmd.Parameters.AddWithValue("@SquareMetreC", 0)

                                thisCmd.Parameters.AddWithValue("@Tassel", "Plastic")
                                thisCmd.Parameters.AddWithValue("@Supply", supply)
                                thisCmd.Parameters.AddWithValue("@TotalItems", 1)
                                thisCmd.Parameters.AddWithValue("@Notes", notes)
                                thisCmd.Parameters.AddWithValue("@MarkUp", 0)

                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        orderClass.ResetPriceDetail(headerId, itemId)
                        orderClass.CalculatePrice(headerId, itemId)
                        orderClass.FinalCostItem(headerId, itemId)

                        Dim dataLog As Object() = {"OrderDetails", itemId, 2, "Order Item Added"}
                        orderClass.Logs(dataLog)
                    Next
                    orderClass.UpdateOrderFactory(headerId)
                End If

                If designName = "Global Roller Blinds" Then
                    If Not blindNo = "Blind 1" Then
                        Dim designName_Fix As String = "Roller Blind"
                        Dim blindName_Fix As String = bracketType
                        Dim tubeName_Fix As String = "Gear Reduction " & tubeType & "mm"
                        Dim controlName_Fix As String = controlType
                        Dim colourName_Fix As String = colourType

                        Dim designId As String = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='" & designName_Fix & "'")
                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds WHERE Name='" & blindName_Fix & "' AND DesignId='" & designId & "'")
                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & tubeName_Fix & "'")
                        Dim controlid As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlName_Fix & "'")
                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colourName_Fix & "'")
                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS subCompanyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlid & "' AND ColourType='" & colourId & "' AND subCompanyArray.VALUE='7'")

                        Dim fabricId As String = orderClass.GetItemData("SELECT Id FROM Fabrics WHERE Name='" & fabricType & "'")
                        Dim fabricColourId As String = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Name='" & fabricId & "'")

                        Dim fabricIdB As String = String.Empty
                        Dim fabricColourIdB As String = String.Empty

                        Dim fabricIdC As String = String.Empty
                        Dim fabricColourIdC As String = String.Empty

                        Dim fabricIdD As String = String.Empty
                        Dim fabricColourIdD As String = String.Empty

                        Dim fabricIdE As String = String.Empty
                        Dim fabricColourIdE As String = String.Empty

                        Dim fabricIdF As String = String.Empty
                        Dim fabricColourIdF As String = String.Empty

                        Dim chainId As String = String.Empty
                        Dim chainIdB As String = String.Empty
                        Dim chainIdC As String = String.Empty
                        Dim chainIdD As String = String.Empty
                        Dim chainIdE As String = String.Empty
                        Dim chainIdF As String = String.Empty

                        Dim bottomId As String = String.Empty
                        Dim bottomIdB As String = String.Empty
                        Dim bottomIdC As String = String.Empty
                        Dim bottomIdD As String = String.Empty
                        Dim bottomIdE As String = String.Empty
                        Dim bottomIdF As String = String.Empty

                        Dim bottomColourId As String = String.Empty
                        Dim bottomColourIdB As String = String.Empty
                        Dim bottomColourIdC As String = String.Empty
                        Dim bottomColourIdD As String = String.Empty
                        Dim bottomColourIdE As String = String.Empty
                        Dim bottomColourIdF As String = String.Empty

                        Dim priceProductGroup As String = String.Empty
                        Dim priceProductGroupB As String = String.Empty
                        Dim priceProductGroupC As String = String.Empty
                        Dim priceProductGroupD As String = String.Empty
                        Dim priceProductGroupE As String = String.Empty
                        Dim priceProductGroupF As String = String.Empty

                        Dim linearMetre As Decimal = width / 1000
                        Dim squareMetre As Decimal = width * drop / 1000000

                        For i As Integer = 1 To qty
                            Dim itemId As String = orderClass.GetNewOrderItemId()

                            Using thisConn As New SqlConnection(myConn)
                                Using thisCmd As New SqlCommand("sp_OrderDetails_Insert_Roller", thisConn)
                                    thisCmd.CommandType = CommandType.StoredProcedure

                                    thisCmd.Parameters.AddWithValue("@Id", itemId)
                                    thisCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                    thisCmd.Parameters.AddWithValue("@ProductId", productId)

                                    thisCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                    thisCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(fabricIdB), CType(DBNull.Value, Object), fabricIdB))
                                    thisCmd.Parameters.AddWithValue("@FabricIdC", If(String.IsNullOrEmpty(fabricIdC), CType(DBNull.Value, Object), fabricIdC))
                                    thisCmd.Parameters.AddWithValue("@FabricIdD", If(String.IsNullOrEmpty(fabricIdD), CType(DBNull.Value, Object), fabricIdD))
                                    thisCmd.Parameters.AddWithValue("@FabricIdE", If(String.IsNullOrEmpty(fabricIdE), CType(DBNull.Value, Object), fabricIdE))
                                    thisCmd.Parameters.AddWithValue("@FabricIdF", If(String.IsNullOrEmpty(fabricIdF), CType(DBNull.Value, Object), fabricIdF))

                                    thisCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                    thisCmd.Parameters.AddWithValue("@FabricColourIdB", If(String.IsNullOrEmpty(fabricColourIdB), CType(DBNull.Value, Object), fabricColourIdB))
                                    thisCmd.Parameters.AddWithValue("@FabricColourIdC", If(String.IsNullOrEmpty(fabricColourIdC), CType(DBNull.Value, Object), fabricColourIdC))
                                    thisCmd.Parameters.AddWithValue("@FabricColourIdD", If(String.IsNullOrEmpty(fabricColourIdD), CType(DBNull.Value, Object), fabricColourIdD))
                                    thisCmd.Parameters.AddWithValue("@FabricColourIdE", If(String.IsNullOrEmpty(fabricColourIdE), CType(DBNull.Value, Object), fabricColourIdE))
                                    thisCmd.Parameters.AddWithValue("@FabricColourIdF", If(String.IsNullOrEmpty(fabricColourIdF), CType(DBNull.Value, Object), fabricColourIdF))

                                    thisCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                    thisCmd.Parameters.AddWithValue("@ChainIdB", If(String.IsNullOrEmpty(chainIdB), CType(DBNull.Value, Object), chainIdB))
                                    thisCmd.Parameters.AddWithValue("@ChainIdC", If(String.IsNullOrEmpty(chainIdC), CType(DBNull.Value, Object), chainIdC))
                                    thisCmd.Parameters.AddWithValue("@ChainIdD", If(String.IsNullOrEmpty(chainIdD), CType(DBNull.Value, Object), chainIdD))
                                    thisCmd.Parameters.AddWithValue("@ChainIdE", If(String.IsNullOrEmpty(chainIdE), CType(DBNull.Value, Object), chainIdE))
                                    thisCmd.Parameters.AddWithValue("@ChainIdF", If(String.IsNullOrEmpty(chainIdF), CType(DBNull.Value, Object), chainIdF))

                                    thisCmd.Parameters.AddWithValue("@BottomId", If(String.IsNullOrEmpty(bottomId), CType(DBNull.Value, Object), bottomId))
                                    thisCmd.Parameters.AddWithValue("@BottomIdB", If(String.IsNullOrEmpty(bottomIdB), CType(DBNull.Value, Object), bottomIdB))
                                    thisCmd.Parameters.AddWithValue("@BottomIdC", If(String.IsNullOrEmpty(bottomIdC), CType(DBNull.Value, Object), bottomIdC))
                                    thisCmd.Parameters.AddWithValue("@BottomIdD", If(String.IsNullOrEmpty(bottomIdD), CType(DBNull.Value, Object), bottomIdD))
                                    thisCmd.Parameters.AddWithValue("@BottomIdE", If(String.IsNullOrEmpty(bottomIdE), CType(DBNull.Value, Object), bottomIdE))
                                    thisCmd.Parameters.AddWithValue("@BottomIdF", If(String.IsNullOrEmpty(bottomIdF), CType(DBNull.Value, Object), bottomIdF))

                                    thisCmd.Parameters.AddWithValue("@BottomColourId", If(String.IsNullOrEmpty(bottomColourId), CType(DBNull.Value, Object), bottomColourId))
                                    thisCmd.Parameters.AddWithValue("@BottomColourIdB", If(String.IsNullOrEmpty(bottomColourIdB), CType(DBNull.Value, Object), bottomColourIdB))
                                    thisCmd.Parameters.AddWithValue("@BottomColourIdC", If(String.IsNullOrEmpty(bottomColourIdC), CType(DBNull.Value, Object), bottomColourIdC))
                                    thisCmd.Parameters.AddWithValue("@BottomColourIdD", If(String.IsNullOrEmpty(bottomColourIdD), CType(DBNull.Value, Object), bottomColourIdD))
                                    thisCmd.Parameters.AddWithValue("@BottomColourIdE", If(String.IsNullOrEmpty(bottomColourIdE), CType(DBNull.Value, Object), bottomColourIdE))
                                    thisCmd.Parameters.AddWithValue("@BottomColourIdF", If(String.IsNullOrEmpty(bottomColourIdF), CType(DBNull.Value, Object), bottomColourIdF))

                                    thisCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                    thisCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                                    thisCmd.Parameters.AddWithValue("@PriceProductGroupIdC", If(String.IsNullOrEmpty(priceProductGroupC), CType(DBNull.Value, Object), priceProductGroupC))
                                    thisCmd.Parameters.AddWithValue("@PriceProductGroupIdD", If(String.IsNullOrEmpty(priceProductGroupD), CType(DBNull.Value, Object), priceProductGroupD))
                                    thisCmd.Parameters.AddWithValue("@PriceProductGroupIdE", If(String.IsNullOrEmpty(priceProductGroupE), CType(DBNull.Value, Object), priceProductGroupE))
                                    thisCmd.Parameters.AddWithValue("@PriceProductGroupIdF", If(String.IsNullOrEmpty(priceProductGroupF), CType(DBNull.Value, Object), priceProductGroupF))

                                    thisCmd.Parameters.AddWithValue("@Qty", 1)
                                    thisCmd.Parameters.AddWithValue("@Room", room)
                                    thisCmd.Parameters.AddWithValue("@Mounting", mounting)

                                    thisCmd.Parameters.AddWithValue("@Width", width)
                                    thisCmd.Parameters.AddWithValue("@WidthB", 0)
                                    thisCmd.Parameters.AddWithValue("@WidthC", 0)
                                    thisCmd.Parameters.AddWithValue("@WidthD", 0)
                                    thisCmd.Parameters.AddWithValue("@WidthE", 0)
                                    thisCmd.Parameters.AddWithValue("@WidthF", 0)

                                    thisCmd.Parameters.AddWithValue("@Drop", drop)
                                    thisCmd.Parameters.AddWithValue("@DropB", 0)
                                    thisCmd.Parameters.AddWithValue("@DropC", 0)
                                    thisCmd.Parameters.AddWithValue("@DropD", 0)
                                    thisCmd.Parameters.AddWithValue("@DropE", 0)
                                    thisCmd.Parameters.AddWithValue("@DropF", 0)

                                    thisCmd.Parameters.AddWithValue("@Roll", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@RollB", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@RollC", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@RollD", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@RollE", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@RollF", String.Empty)

                                    thisCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                    thisCmd.Parameters.AddWithValue("@ControlPositionB", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@ControlPositionC", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@ControlPositionD", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@ControlPositionE", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@ControlPositionF", String.Empty)

                                    thisCmd.Parameters.AddWithValue("@ControlLength", "Standard")
                                    thisCmd.Parameters.AddWithValue("@ControlLengthB", "Standard")
                                    thisCmd.Parameters.AddWithValue("@ControlLengthC", "Standard")
                                    thisCmd.Parameters.AddWithValue("@ControlLengthD", "Standard")
                                    thisCmd.Parameters.AddWithValue("@ControlLengthE", "Standard")
                                    thisCmd.Parameters.AddWithValue("@ControlLengthF", "Standard")

                                    thisCmd.Parameters.AddWithValue("@ControlLengthValue", controlLength)
                                    thisCmd.Parameters.AddWithValue("@ControlLengthValueB", 0)
                                    thisCmd.Parameters.AddWithValue("@ControlLengthValueC", 0)
                                    thisCmd.Parameters.AddWithValue("@ControlLengthValueD", 0)
                                    thisCmd.Parameters.AddWithValue("@ControlLengthValueE", 0)
                                    thisCmd.Parameters.AddWithValue("@ControlLengthValueF", 0)

                                    thisCmd.Parameters.AddWithValue("@ChainStopper", "No Stopper")
                                    thisCmd.Parameters.AddWithValue("@ChainStopperB", "No Stopper")
                                    thisCmd.Parameters.AddWithValue("@ChainStopperC", "No Stopper")
                                    thisCmd.Parameters.AddWithValue("@ChainStopperD", "No Stopper")
                                    thisCmd.Parameters.AddWithValue("@ChainStopperE", "No Stopper")
                                    thisCmd.Parameters.AddWithValue("@ChainStopperF", "No Stopper")

                                    thisCmd.Parameters.AddWithValue("@FlatOption", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@FlatOptionB", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@FlatOptionC", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@FlatOptionD", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@FlatOptionE", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@FlatOptionF", String.Empty)

                                    thisCmd.Parameters.AddWithValue("@TopTrack", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@BracketSize", CType(DBNull.Value, Object))
                                    thisCmd.Parameters.AddWithValue("@BracketExtension", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@SpringAssist", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@Adjusting", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@DryContact", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@Charger", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@ExtensionCable", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@Supply", String.Empty)

                                    thisCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                                    thisCmd.Parameters.AddWithValue("@LinearMetreB", 0)
                                    thisCmd.Parameters.AddWithValue("@LinearMetreC", 0)
                                    thisCmd.Parameters.AddWithValue("@LinearMetreD", 0)
                                    thisCmd.Parameters.AddWithValue("@LinearMetreE", 0)
                                    thisCmd.Parameters.AddWithValue("@LinearMetreF", 0)

                                    thisCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                                    thisCmd.Parameters.AddWithValue("@SquareMetreB", 0)
                                    thisCmd.Parameters.AddWithValue("@SquareMetreC", 0)
                                    thisCmd.Parameters.AddWithValue("@SquareMetreD", 0)
                                    thisCmd.Parameters.AddWithValue("@SquareMetreE", 0)
                                    thisCmd.Parameters.AddWithValue("@SquareMetreF", 0)

                                    thisCmd.Parameters.AddWithValue("@Printing", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@PrintingB", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@PrintingC", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@PrintingD", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@PrintingE", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@PrintingF", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@PrintingG", String.Empty)
                                    thisCmd.Parameters.AddWithValue("@PrintingH", String.Empty)

                                    thisCmd.Parameters.AddWithValue("@TotalItems", 1)
                                    thisCmd.Parameters.AddWithValue("@Notes", notes)
                                    thisCmd.Parameters.AddWithValue("@MarkUp", 0)

                                    thisConn.Open()
                                    thisCmd.ExecuteNonQuery()
                                End Using
                            End Using

                            orderClass.ResetPriceDetail(headerId, itemId)
                            orderClass.CalculatePrice(headerId, itemId)
                            orderClass.FinalCostItem(headerId, itemId)

                            Dim dataLog As Object() = {"OrderDetails", itemId, "2", "Order Item Added"}
                            orderClass.Logs(dataLog)
                        Next
                        orderClass.UpdateOrderFactory(headerId)
                    End If
                End If

                If designName = "Global Roman Blinds" Then
                    If Not blindNo = "Blind 1" Then
                        Continue For
                    End If
                End If
            Next

            WriteJson(context, 200, New With {.status = "success", .message = "Data diterima dan disimpan dengan sukses", .headerId = headerId})
        Catch ex As Exception
            System.Diagnostics.Trace.TraceError("Sunlight API Error: " & ex.ToString())
            WriteJson(context, 500, New With {.status = "error", .message = "Database operation failed", .details = ex.Message})
        End Try
    End Sub

    Private Sub WriteJson(context As HttpContext, statusCode As Integer, data As Object)
        context.Response.StatusCode = statusCode
        context.Response.TrySkipIisCustomErrors = True
        context.Response.Write(New JavaScriptSerializer().Serialize(data))
        context.ApplicationInstance.CompleteRequest()
    End Sub

    Private Sub InsertOrderHeader(headerId As String, orderData As OrderData)
        Dim sql As String = "INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, OrderFactory, OrderDelivery, OrderContact, OrderAddress, OrderContainer, Status, CreatedBy, CreatedDate, SubmittedDate, Payment, Amount, Download, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, 'Regular', 'BIG', 'Default', '', '', 'NSW', 'New Order', 1568, GETDATE(), GETDATE(), 0, 0, 'No', 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);"

        Using thisConn As New SqlConnection(myConn)
            Using thisCmd As New SqlCommand(sql, thisConn)
                thisCmd.Parameters.Add("@Id", SqlDbType.NVarChar).Value = headerId
                thisCmd.Parameters.Add("@OrderId", SqlDbType.NVarChar).Value = If(orderData.OrderId Is Nothing, "", orderData.OrderId)
                thisCmd.Parameters.Add("@CustomerId", SqlDbType.NVarChar).Value = "1538"
                thisCmd.Parameters.Add("@OrderNumber", SqlDbType.NVarChar).Value = If(orderData.OrderNumber Is Nothing, "", orderData.OrderNumber)
                thisCmd.Parameters.Add("@OrderName", SqlDbType.NVarChar).Value = If(orderData.OrderName Is Nothing, "", orderData.OrderName)
                thisCmd.Parameters.Add("@OrderNote", SqlDbType.NVarChar).Value = If(orderData.OrderNote Is Nothing, "", orderData.OrderNote)
                thisConn.Open()
                thisCmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub


    Private Sub InsertOrderDetail(headerId As String, detail As Dictionary(Of String, Object))
        Dim allowedColumns As String() = {
            "KitName", "BracketType", "TubeType", "ControlType", "ColourType",
            "DesignName", "BlindName", "FabricType", "FabricColour", "UniqueId",
            "BlindNo", "Qty", "Room", "Mounting", "Width", "Drop", "Layout",
            "NumOfPanel", "TwoOnOneHeadreal", "ControlPosition", "ControlLength",
            "ControlLift", "ControlTilt", "TilterPosition", "ChainLength", "MaterialChain",
            "ChainColour", "TrackType", "TrackColour", "NumOfWand", "WandPosition",
            "WandColour", "WandLength", "CordColour", "CordLength",
            "AcornPlasticColour", "Batten", "BattenColour", "BracketOption",
            "BracketColour", "Fitting", "Cleat", "BottomHoldDown", "PelmetType",
            "PelmetWidth", "PelmetReturnLeft", "PelmetReturnRight",
            "CutOut_LeftTop", "CutOut_RightTop", "CutOut_LeftBottom",
            "CutOut_RightBottom", "LHSWidth_Top", "LHSHeight_Top",
            "RHSWidth_Top", "RHSHeight_Top", "LHSWidth_Bottom",
            "LHSHeight_Bottom", "RHSWidth_Bottom", "RHSHeight_Bottom", "Notes"
        }

        Dim columns As New List(Of String)
        Dim parameters As New List(Of String)

        Using thisConn As New SqlConnection(myConn)
            Using thisCmd As New SqlCommand()
                thisCmd.Connection = thisConn

                columns.Add("[HeaderId]")
                parameters.Add("@HeaderId")
                thisCmd.Parameters.Add("@HeaderId", SqlDbType.NVarChar).Value = headerId

                Dim index As Integer = 0

                For Each column As String In allowedColumns
                    If detail.ContainsKey(column) Then

                        Dim parameterName As String = "@p" & index
                        Dim value As Object = detail(column)

                        columns.Add("[" & column & "]")
                        parameters.Add(parameterName)

                        thisCmd.Parameters.AddWithValue(parameterName, If(value Is Nothing, DBNull.Value, value))

                        index += 1
                    End If
                Next

                If columns.Count = 1 Then
                    Throw New Exception("Order detail has no valid fields.")
                End If

                thisCmd.CommandText = "INSERT INTO OrderDetails_Sunlight (" & String.Join(", ", columns) & ") VALUES (" & String.Join(", ", parameters) & ")"

                thisConn.Open()
                thisCmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class

<Serializable()>
Public Class OrderData
    Public Property Id As Integer
    Public Property OrderId As String
    Public Property OrderNumber As String
    Public Property OrderName As String
    Public Property OrderNote As String
    Public Property Details As List(Of Dictionary(Of String, Object))
End Class