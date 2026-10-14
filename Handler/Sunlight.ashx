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
            For Each detail As OrderDetail In orderData.Details
                If detail Is Nothing Then
                    Throw New Exception("Order detail cannot be empty.")
                End If

                If String.IsNullOrWhiteSpace(detail.DesignName) Then
                    Throw New Exception("DesignName is required.")
                End If

                Select Case detail.DesignName.Trim()
                    Case "Aluminium Blinds"
                        InsertAluminium(headerId, detail)
                    Case "Venetian Blinds"
                        InsertVenetian(headerId, detail)
                    Case Else
                        Throw New Exception("DesignName tidak dikenali: " & detail.DesignName)
                End Select
            Next

            mailingClass.NewOrder(headerId)

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
        Dim sql As String = "INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, OrderFactory, OrderContact, OrderAddress, OrderContainer, Status, CreatedBy, CreatedDate, SubmittedDate, Payment, Amount, Download, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, 'Regular', 'BIG', '', '', 'NSW', 'New Order', 1568, GETDATE(), GETDATE(), 0, 0, 'No', 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);"

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

    Private Sub InsertAluminium(headerId As String, detail As OrderDetail)
        Dim designName As String = "Aluminium Blind"
        Dim blindName As String = "0.21mm"
        Dim designId As String = "1"
        Dim priceGroupId As String = "14"

        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds WHERE Name='" & blindName & "' AND DesignId='" & designId & "'")
        Dim productColourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & detail.ColourType & "'")
        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS subCompanyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='9' AND ControlType='17' AND ColourType='" & productColourId & "' AND subCompanyArray.VALUE='7'")

        Dim productGroupName As String = String.Format("{0} - {1}", designName, blindName)
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(productGroupName, designId, priceGroupId)

        Dim controlLength As String = "Custom"
        Dim controlLengthValue As Integer = detail.ControlLength

        If detail.ControlLength = 0 Then
            controlLength = "Standard"
            controlLengthValue = CInt(Math.Ceiling(detail.Drop * 2.0 / 3.0))
            If controlLengthValue < 550 Then
                controlLengthValue = 550
            End If
        End If

        Dim fixControl As String = String.Empty
        If detail.ControlPosition = "LHC" Then
            fixControl = "Left"
        ElseIf detail.ControlPosition = "RHC" Then
            fixControl = "Right"
        End If

        Dim cutOut As String = String.Empty
        If detail.CutOut_LeftTop = 1 OrElse detail.CutOut_RightTop = 1 OrElse detail.CutOut_LeftBottom = 1 OrElse detail.CutOut_RightBottom = 1 Then
            cutOut = "Yes"
        End If

        Dim supply As String = detail.BottomHoldDown
        If supply = "No" Then supply = String.Empty

        Dim linearMetre As Decimal = detail.Width / 1000D
        Dim squareMetre As Decimal = CDec(detail.Width) * CDec(detail.Drop) / 1000000D

        For i As Integer = 1 To detail.Qty
            Dim itemId As String = orderClass.GetNewOrderItemId()
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("sp_OrderDetails_Insert_Aluminium", thisConn)
                    thisCmd.CommandType = CommandType.StoredProcedure

                    thisCmd.Parameters.AddWithValue("@Id", itemId)
                    thisCmd.Parameters.AddWithValue("@HeaderId", headerId)
                    thisCmd.Parameters.AddWithValue("@ProductId", productId)
                    thisCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    thisCmd.Parameters.AddWithValue("@PriceProductGroupIdB", DBNull.Value)
                    thisCmd.Parameters.AddWithValue("@Room", detail.Room)
                    thisCmd.Parameters.AddWithValue("@Mounting", detail.Mounting)
                    thisCmd.Parameters.AddWithValue("@SubType", "Single")
                    thisCmd.Parameters.AddWithValue("@ControlPosition", fixControl)
                    thisCmd.Parameters.AddWithValue("@TilterPosition", fixControl)
                    thisCmd.Parameters.AddWithValue("@Width", detail.Width)
                    thisCmd.Parameters.AddWithValue("@Drop", detail.Drop)
                    thisCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                    thisCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                    thisCmd.Parameters.AddWithValue("@WandLength", controlLength)
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
                    thisCmd.Parameters.AddWithValue("@CutTopLeftWidth", detail.LHSWidth_Top)
                    thisCmd.Parameters.AddWithValue("@CutTopLeftHeight", detail.LHSHeight_Top)
                    thisCmd.Parameters.AddWithValue("@CutTopRightWidth", detail.RHSWidth_Top)
                    thisCmd.Parameters.AddWithValue("@CutTopRightHeight", detail.RHSHeight_Top)
                    thisCmd.Parameters.AddWithValue("@CutBottomLeftWidth", detail.LHSWidth_Bottom)
                    thisCmd.Parameters.AddWithValue("@CutBottomLeftHeight", detail.LHSHeight_Bottom)
                    thisCmd.Parameters.AddWithValue("@CutBottomRightWidth", detail.RHSWidth_Bottom)
                    thisCmd.Parameters.AddWithValue("@CutBottomRightHeight", detail.RHSHeight_Bottom)
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
                    thisCmd.Parameters.AddWithValue("@Notes", If(detail.Notes Is Nothing, "", detail.Notes))
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
    End Sub

    Private Sub InsertVenetian(headerId As String, detail As OrderDetail)
        Dim designName As String = "Venetian Blind"
        Dim blindName As String = String.Empty
        Dim designId As String = "10"
        Dim priceGroupId As String = "14"

        If detail.BlindName = "50mm Wooden" Then
            blindName = "Basswood 50mm"
        End If
        If detail.BlindName = "63mm Wooden" Then
            blindName = "Basswood 63mm"
        End If
        If detail.BlindName = "50mm Mockwood" Then
            blindName = "Econo 50mm"
        End If
        If detail.BlindName = "63mm Mockwood" Then
            blindName = "Econo 63mm"
        End If

        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds WHERE Name='" & blindName & "' AND DesignId='" & designId & "'")

        Dim productColourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & detail.ColourType & "'")
        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS subCompanyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='9' AND ControlType='17' AND ColourType='" & productColourId & "' AND subCompanyArray.VALUE='7'")

        Dim productGroupName As String = String.Format("{0} - {1}", designName, blindName)
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(productGroupName, designId, priceGroupId)

        Dim controlLength As String = "Custom"
        Dim controlLengthValue As Integer = detail.ControlLength

        Dim clvalue As Integer = Math.Ceiling(detail.Drop * 2 / 3)
        If clvalue < 550 Then clvalue = 550

        Dim cutOut As String = String.Empty
        If detail.CutOut_LeftTop = 1 OrElse detail.CutOut_RightTop = 1 OrElse detail.CutOut_LeftBottom = 1 OrElse detail.CutOut_RightBottom = 1 Then
            cutOut = "Yes"
        End If

        Dim valanceType As String = String.Empty
        If blindName.Contains("Basswood") Then
            valanceType = "75mm Valance"
        End If
        If blindName.Contains("Econo") Then
            valanceType = "76mm Valance"
        End If

        Dim valanceSize As Integer = detail.PelmetWidth
        Dim returnPosition As String = String.Empty
        Dim returnLength As String = String.Empty
        Dim returnLengthValue As Integer = 0

        If detail.PelmetType = "With Return" Then
            returnLength = "Custom"
            returnPosition = "Both Sides"
            returnLengthValue = detail.PelmetReturnLeft
        End If
        If detail.PelmetType = "Single Left Return" Then
            returnLength = "Custom"
            returnPosition = "Left"
            returnLengthValue = detail.PelmetReturnLeft
        End If
        If detail.PelmetType = "Single Right Return" Then
            returnLength = "Custom"
            returnPosition = "Right"
            returnLengthValue = detail.PelmetReturnRight
        End If

        Dim supply As String = detail.BottomHoldDown
        If supply = "No" Then supply = String.Empty

        Dim linearMetre As Decimal = detail.Width / 1000D
        Dim squareMetre As Decimal = CDec(detail.Width) * CDec(detail.Drop) / 1000000D

        For i As Integer = 1 To detail.Qty
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
                    thisCmd.Parameters.AddWithValue("@Room", detail.Room)
                    thisCmd.Parameters.AddWithValue("@Mounting", detail.Mounting)
                    thisCmd.Parameters.AddWithValue("@SubType", "Single")

                    thisCmd.Parameters.AddWithValue("@ControlPosition", detail.ControlLift)
                    thisCmd.Parameters.AddWithValue("@ControlPositionB", String.Empty)
                    thisCmd.Parameters.AddWithValue("@ControlPositionC", String.Empty)

                    thisCmd.Parameters.AddWithValue("@TilterPosition", detail.ControlLift)
                    thisCmd.Parameters.AddWithValue("@TilterPositionB", String.Empty)
                    thisCmd.Parameters.AddWithValue("@TilterPositionC", String.Empty)

                    thisCmd.Parameters.AddWithValue("@Width", detail.Width)
                    thisCmd.Parameters.AddWithValue("@WidthB", 0)
                    thisCmd.Parameters.AddWithValue("@WidthC", 0)
                    thisCmd.Parameters.AddWithValue("@Drop", detail.Drop)
                    thisCmd.Parameters.AddWithValue("@DropB", 0)
                    thisCmd.Parameters.AddWithValue("@DropC", 0)

                    thisCmd.Parameters.AddWithValue("@ControlLength", "Standard")
                    thisCmd.Parameters.AddWithValue("@ControlLengthB", String.Empty)
                    thisCmd.Parameters.AddWithValue("@ControlLengthC", String.Empty)
                    thisCmd.Parameters.AddWithValue("@ControlLengthValue", clvalue)
                    thisCmd.Parameters.AddWithValue("@ControlLengthValueB", 0)
                    thisCmd.Parameters.AddWithValue("@ControlLengthValueC", 0)
                    thisCmd.Parameters.AddWithValue("@WandLengthValue", 0)

                    thisCmd.Parameters.AddWithValue("@ValanceType", valanceType)
                    thisCmd.Parameters.AddWithValue("@ValanceSize", "Custom")
                    thisCmd.Parameters.AddWithValue("@ValanceSizeValue", valanceSize)

                    thisCmd.Parameters.AddWithValue("@ReturnPosition", returnPosition)
                    thisCmd.Parameters.AddWithValue("@ReturnLength", "Custom")
                    thisCmd.Parameters.AddWithValue("@ReturnLengthValue", returnLengthValue)

                    thisCmd.Parameters.AddWithValue("@CutOut", cutOut)
                    thisCmd.Parameters.AddWithValue("@CutTopLeftWidth", detail.LHSWidth_Top)
                    thisCmd.Parameters.AddWithValue("@CutTopLeftHeight", detail.LHSHeight_Top)
                    thisCmd.Parameters.AddWithValue("@CutTopRightWidth", detail.RHSWidth_Top)
                    thisCmd.Parameters.AddWithValue("@CutTopRightHeight", detail.RHSHeight_Top)
                    thisCmd.Parameters.AddWithValue("@CutBottomLeftWidth", detail.LHSWidth_Bottom)
                    thisCmd.Parameters.AddWithValue("@CutBottomLeftHeight", detail.LHSHeight_Bottom)
                    thisCmd.Parameters.AddWithValue("@CutBottomRightWidth", detail.RHSWidth_Bottom)
                    thisCmd.Parameters.AddWithValue("@CutBottomRightHeight", detail.RHSHeight_Bottom)
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
                    thisCmd.Parameters.AddWithValue("@Notes", detail.Notes)
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
    Public Property JobId As String
    Public Property CustomerId As String
    Public Property OrderNumber As String
    Public Property OrderName As String
    Public Property OrderNote As String
    Public Property OrderType As String
    Public Property Status As String
    Public Property CreatedBy As String
    Public Property Details As List(Of OrderDetail)

End Class

<Serializable()>
Public Class OrderDetail

    Public Property DesignName As String
    Public Property BlindName As String
    Public Property ColourType As String
    Public Property Qty As Integer
    Public Property Room As String
    Public Property Mounting As String
    Public Property Width As Integer
    Public Property Drop As Integer
    Public Property ControlPosition As String
    Public Property ControlLift As String
    Public Property ControlTilt As String
    Public Property BottomHoldDown As String
    Public Property ControlLength As Integer

    Public Property CutOut_LeftTop As Integer
    Public Property CutOut_RightTop As Integer
    Public Property CutOut_LeftBottom As Integer
    Public Property CutOut_RightBottom As Integer

    Public Property LHSWidth_Top As Integer
    Public Property LHSHeight_Top As Integer
    Public Property RHSWidth_Top As Integer
    Public Property RHSHeight_Top As Integer
    Public Property LHSWidth_Bottom As Integer
    Public Property LHSHeight_Bottom As Integer
    Public Property RHSWidth_Bottom As Integer
    Public Property RHSHeight_Bottom As Integer

    Public Property PelmetType As String
    Public Property PelmetWidth As Integer
    Public Property PelmetReturnLeft As Integer
    Public Property PelmetReturnRight As Integer

    Public Property Notes As String
End Class