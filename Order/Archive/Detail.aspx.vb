Imports System.Data
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml

Partial Class Order_Archive_Detail
    Inherits Page

    Dim dataMailing As Object() = Nothing
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty

    Dim archiveClass As New ArchiveClass
    Dim mailingClass As New MailingClass

    Dim headerId As String = String.Empty

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Private Property companyGroup As String
        Get
            Return If(ViewState("CompanyGroup"), String.Empty)
        End Get
        Set(value As String)
            ViewState("CompanyGroup") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order/archive", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("orderid")) Then
            Response.Redirect("~/order/archive", False)
            Exit Sub
        End If

        headerId = Request.QueryString("orderid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDataOrder(headerId)
        End If
    End Sub

    Protected Sub btnPreview_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            GeneratePDF()
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

    Protected Sub btnConvert_Click(sender As Object, e As EventArgs)
        'MessageError(False, String.Empty)
        'Try
        '    Dim detailData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID='" & headerId & "' AND Active=1")

        '    If detailData.Tables(0).Rows.Count = 0 Then
        '        MessageError(True, "ERROR")
        '        Exit Sub
        '    End If

        '    Dim orderClass As New OrderClass

        '    Dim thisId As String = orderClass.GetNewOrderHeaderId()

        '    Dim success As Boolean = False
        '    Dim retry As Integer = 0
        '    Dim maxRetry As Integer = 10
        '    Dim orderId As String = ""

        '    Do While Not success
        '        retry += 1
        '        If retry > maxRetry Then
        '            Throw New Exception("FAILED TO GENERATE UNIQUE ORDER ID")
        '        End If

        '        Dim companyAlias As String = orderClass.GetCompanyAliasByCustomer(Session("CustomerId").ToString())

        '        Dim randomCode As String = orderClass.GenerateRandomCode()
        '        orderId = companyAlias & randomCode

        '        Try
        '            Using thisConn As New SqlConnection(myConn)
        '                Using myCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, DownloadBOE, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, @OrderType, 'Unsubmitted', 2, GETDATE(), 0, 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
        '                    myCmd.Parameters.AddWithValue("@Id", thisId)
        '                    myCmd.Parameters.AddWithValue("@OrderId", orderId)
        '                    myCmd.Parameters.AddWithValue("@CustomerId", Session("CustomerId").ToString())
        '                    myCmd.Parameters.AddWithValue("@OrderNumber", lblOrderNumber.Text.Trim())
        '                    myCmd.Parameters.AddWithValue("@OrderName", lblOrderName.Text.Trim())
        '                    myCmd.Parameters.AddWithValue("@OrderNote", String.Empty)
        '                    myCmd.Parameters.AddWithValue("@OrderType", "Builder")

        '                    thisConn.Open()
        '                    myCmd.ExecuteNonQuery()
        '                End Using
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

        '    Using thisConn As New SqlConnection(myConn)
        '        Using myCmd As New SqlCommand("INSERT INTO OrderBuilders(Id) VALUES (@Id)", thisConn)
        '            myCmd.Parameters.AddWithValue("@Id", thisId)
        '            thisConn.Open()
        '            myCmd.ExecuteNonQuery()
        '        End Using
        '    End Using

        '    Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Builder/{0}/", orderId))
        '    If Not Directory.Exists(directoryOrder) Then
        '        Directory.CreateDirectory(directoryOrder)
        '    End If

        '    Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId").ToString(), "Order Created"}
        '    orderClass.Logs(dataLog)

        '    For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
        '        Dim room As String = detailData.Tables(0).Rows(i).Item("Room").ToString()
        '        Dim mounting As String = detailData.Tables(0).Rows(i).Item("Mounting").ToString()
        '        Dim blindType As String = detailData.Tables(0).Rows(i).Item("BlindType").ToString()
        '        Dim colour As String = detailData.Tables(0).Rows(i).Item("Colour").ToString()
        '        Dim subType As String = detailData.Tables(0).Rows(i).Item("SubType").ToString()
        '        Dim width As String = detailData.Tables(0).Rows(i).Item("Width").ToString()
        '        Dim drop As String = detailData.Tables(0).Rows(i).Item("Drop").ToString()
        '        Dim controlPosition As String = detailData.Tables(0).Rows(i).Item("ControlPosition").ToString()
        '        Dim tilterPosition As String = detailData.Tables(0).Rows(i).Item("TilterPosition").ToString()

        '        Dim supply As String = detailData.Tables(0).Rows(i).Item("Supply").ToString()
        '        Dim notes As String = detailData.Tables(0).Rows(i).Item("Notes").ToString()
        '        Dim markUp As String = detailData.Tables(0).Rows(i).Item("MarkUp").ToString()

        '        If blindType = "Ven" Then
        '            Dim widthB As String = "0"
        '            Dim dropB As String = "0"

        '            Dim controlPositionb As String = String.Empty
        '            Dim tilterPositionb As String = String.Empty

        '            Dim controlLength As String = "Custom"
        '            Dim controlLengthB As String = String.Empty

        '            Dim controlLengthValue As String = detailData.Tables(0).Rows(i).Item("PullCordLength").ToString()
        '            Dim controlLengthValueB As String = "0"

        '            Dim wandLength1 As String = String.Empty
        '            Dim wandLength2 As String = String.Empty

        '            Dim wandLengthValue1 As String = detailData.Tables(0).Rows(i).Item("WandLength").ToString()
        '            Dim wandLengthValue2 As String = detailData.Tables(0).Rows(i).Item("Additional").ToString()

        '            Dim wandLength As String = String.Empty
        '            Dim wandLengthB As String = String.Empty

        '            Dim wandLengthValue As String = "0"
        '            Dim wandLengthValueB As String = "0"

        '            If subType = "Single" Then
        '                wandLength = "Custom"
        '                wandLengthValue = wandLengthValue1
        '            End If

        '            Dim designId = "1"
        '            Dim blindId = "2"
        '            Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colour & "'")

        '            Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products WHERE DesignId='" & designId & "' AND BlindId = '" & blindId & "' AND ColourType = '" & colourId & "'")

        '            Dim linearmetre As Decimal = width / 1000
        '            Dim linearmetreB As Decimal = 0D
        '            Dim squaremetre As Decimal = width * drop / 1000000
        '            Dim squaremetreB As Decimal = 0D

        '            Dim totalItems As Integer = 1

        '            Dim newSubType As String = subType

        '            If subType = "2 on 1 Aluminium Left-Left" Then
        '                newSubType = "2 on 1 Left-Left"

        '                widthB = detailData.Tables(0).Rows(i).Item("Width2c").ToString()
        '                dropB = drop

        '                controlPosition = "Left" : tilterPosition = "Left"
        '                controlPositionb = "Left" : tilterPositionb = String.Empty

        '                linearmetre = widthB / 1000
        '                squaremetreB = widthB * dropB / 1000000

        '                totalItems = 2
        '            End If

        '            If subType = "2 on 1 Aluminium Right-Right" Then
        '                newSubType = "2 on 1 Right-Right"

        '                widthB = detailData.Tables(0).Rows(i).Item("Width2c").ToString()
        '                dropB = drop

        '                linearmetre = widthB / 1000
        '                squaremetreB = widthB * dropB / 1000000

        '                totalItems = 2

        '                controlPosition = "Right" : tilterPosition = String.Empty
        '                controlPositionb = "Right" : tilterPositionb = "Right"

        '                wandLength = String.Empty : wandLengthValue = "0"
        '                wandLengthB = "Custom" : wandLengthValue = wandLength1
        '            End If

        '            Dim priceProductGroup As String = orderClass.GetPriceProductGroupId("Aluminium 25mm x 0.21mm", designId)
        '            Dim priceProductGroupB As String = String.Empty

        '            If subType.Contains("2 on 1") Then priceProductGroupB = priceProductGroup

        '            Dim itemId As String = orderClass.GetNewOrderItemId()

        '            Using thisConn As SqlConnection = New SqlConnection(myConn)
        '                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails (Id, HeaderId, ProductId, PriceProductGroupId, PriceProductGroupIdB, SubType, Qty, Room, Mounting, ControlPosition, ControlPositionB, TilterPosition, TilterPositionB, Width, WidthB, [Drop], DropB, Supply, ControlLength, ControlLengthValue, ControlLengthB, ControlLengthValueB, WandLength, WandLengthValue, WandLengthB, WandLengthValueB, Notes, LinearMetre, LinearMetreB, SquareMetre, SquareMetreB, TotalItems, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, @PriceProductGroupIdB, @SubType, 1, @Room, @Mounting, @ControlPosition, @ControlPositionB, @TilterPosition, @TilterPositionB, @Width, @WidthB, @Drop, @DropB, @Supply, @ControlLength, @ControlLengthValue, @ControlLengthB, @ControlLengthValueB, @WandLength, @WandLengthValue, @WandLengthB, @WandLengthValueB, @Notes, @LinearMetre, @LinearMetreB, @SquareMetre, @SquareMetreB, @TotalItems, @MarkUp, 1)", thisConn)
        '                    myCmd.Parameters.AddWithValue("@Id", itemId)
        '                    myCmd.Parameters.AddWithValue("@HeaderId", thisId)
        '                    myCmd.Parameters.AddWithValue("@ProductId", productId)
        '                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
        '                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
        '                    myCmd.Parameters.AddWithValue("@Room", room)
        '                    myCmd.Parameters.AddWithValue("@Mounting", mounting)
        '                    myCmd.Parameters.AddWithValue("@SubType", subType)
        '                    myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
        '                    myCmd.Parameters.AddWithValue("@TilterPosition", tilterPosition)
        '                    myCmd.Parameters.AddWithValue("@Width", width)
        '                    myCmd.Parameters.AddWithValue("@Drop", drop)
        '                    myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
        '                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
        '                    myCmd.Parameters.AddWithValue("@WandLength", wandLength)
        '                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)
        '                    myCmd.Parameters.AddWithValue("@ControlPositionB", controlPositionb)
        '                    myCmd.Parameters.AddWithValue("@TilterPositionB", tilterPositionb)
        '                    myCmd.Parameters.AddWithValue("@Widthb", widthB)
        '                    myCmd.Parameters.AddWithValue("@DropB", dropB)
        '                    myCmd.Parameters.AddWithValue("@ControlLengthB", controlLengthB)
        '                    myCmd.Parameters.AddWithValue("@ControlLengthValueB", controlLengthValueB)
        '                    myCmd.Parameters.AddWithValue("@WandLengthB", wandLengthB)
        '                    myCmd.Parameters.AddWithValue("@WandLengthValueB", wandLengthValueB)
        '                    myCmd.Parameters.AddWithValue("@LinearMetre", linearmetre)
        '                    myCmd.Parameters.AddWithValue("@LinearMetreB", linearmetreB)
        '                    myCmd.Parameters.AddWithValue("@SquareMetre", squaremetre)
        '                    myCmd.Parameters.AddWithValue("@SquareMetreB", squaremetreB)
        '                    myCmd.Parameters.AddWithValue("@Supply", supply)
        '                    myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
        '                    myCmd.Parameters.AddWithValue("@Notes", notes)
        '                    myCmd.Parameters.AddWithValue("@MarkUp", markUp)

        '                    thisConn.Open()
        '                    myCmd.ExecuteNonQuery()
        '                End Using
        '            End Using

        '            orderClass.ResetPriceDetail(thisId, itemId)
        '            orderClass.CalculatePrice(thisId, itemId)
        '            orderClass.FinalCostItem(thisId, itemId)

        '            dataLog = {"OrderDetails", itemId, Session("LoginId"), "Order Item Added"}
        '            orderClass.Logs(dataLog)
        '        End If

        '        If blindType = "Venetian" Then

        '        End If
        '    Next

        '    url = String.Format("~/order/detail?orderid={0}", thisId)
        '    Response.Redirect(url, False)
        'Catch ex As Exception
        '    MessageError(True, ex.ToString())
        '    If Not Session("RoleName") = "Developer" Then
        '        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
        '        If Session("RoleName") = "Customer" Then
        '            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
        '        End If
        '        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataOrder", ex.ToString()}
        '        mailingClass.WebError(dataMailing)
        '    End If
        'End Try
    End Sub

    Protected Sub BindDataOrder(headerId As String)
        Try
            Dim thisQuery As String = "SELECT Order_Header.*, Stores.Name AS CustomerName FROM Order_Header LEFT JOIN Users ON Order_Header.UserLogin=Users.UserName LEFT JOIN Stores ON Users.DebtorCode=Stores.Id WHERE Order_Header.OrdID='" & headerId & "'"

            Dim headerData As DataSet = archiveClass.GetListData(thisQuery)
            If headerData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/order/archive", False)
                Exit Sub
            End If

            lblCustomerName.Text = headerData.Tables(0).Rows(0).Item("CustomerName").ToString()
            lblOrderNumber.Text = headerData.Tables(0).Rows(0).Item("StoreOrderNo").ToString()
            lblOrderName.Text = headerData.Tables(0).Rows(0).Item("StoreCustomer").ToString()

            Dim status As String = headerData.Tables(0).Rows(0).Item("Status").ToString()

            lblCreatedBy.Text = headerData.Tables(0).Rows(0).Item("UserLogin").ToString()
            lblCreatedDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData.Tables(0).Rows(0).Item("CreatedDate").ToString()) Then
                lblCreatedDate.Text = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CreatedDate")).ToString("dd MMM yyyy")
            End If

            companyGroup = archiveClass.GetItemData("SELECT CompanyGroup FROM Users WHERE UserName='" & lblCreatedBy.Text & "'")

            lblSubmittedBy.Text = headerData.Tables(0).Rows(0).Item("SubmittedBy").ToString()
            lblSubmittedDate.Text = "-"
            If Not String.IsNullOrEmpty(headerData.Tables(0).Rows(0).Item("SubmittedDate").ToString()) Then
                lblSubmittedDate.Text = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("SubmittedDate")).ToString("dd MMM yyyy")
            End If

            aConvert.Visible = False
            If Session("RoleName") = "Customer" AndAlso status = "Draft" Then
                aConvert.Visible = True
            End If

            BindDataItem(headerId)
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

    Protected Sub BindDataItem(headerId As String)
        gvListItem.DataSource = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID='" & headerId & "' AND Active=1 ORDER BY OrddID ASC")
        gvListItem.DataBind()
    End Sub

    Protected Function BindProductDescription(itemId As String) As String
        Dim result As String = String.Empty

        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE OrddID='" & itemId & "'")

        Dim blindType As String = thisData.Tables(0).Rows(0).Item("BlindType").ToString()
        Dim orderType As String = thisData.Tables(0).Rows(0).Item("OrderType").ToString()
        Dim subType As String = thisData.Tables(0).Rows(0).Item("SubType").ToString()
        Dim width As String = thisData.Tables(0).Rows(0).Item("Width").ToString()
        Dim drop As String = thisData.Tables(0).Rows(0).Item("Drop").ToString()
        Dim colour As String = thisData.Tables(0).Rows(0).Item("Colour").ToString()

        Dim size As String = String.Format("({0}x{1})", width, drop)

        result = String.Format("{0} {1} {2}", blindType, orderType, colour)

        If blindType = "Ven" Then
            If subType.Contains("2 on 1") Then
                result = "2 on 1 Headrail"
                result &= "<br />"
                result &= String.Format("{0} {1} {2}", blindType, orderType, colour)
            End If
        End If

        Return result
    End Function

    Protected Sub GeneratePDF()
        Dim result As String = ""
        result += Print_HeaderTemplate(headerId)

        result += Print_AluSingle(headerId)
        result += Print_Alu2on1(headerId)
        result += Print_Cellular(headerId)
        result += Print_Linea(headerId)
        result += Print_Panel(headerId)
        result += Print_Pelmet(headerId)
        result += Print_Profile(headerId)
        If companyGroup = "LOCAL" Then
            result += Print_SingleRollerLocal(headerId)
        End If

        If companyGroup = "JPMD" Or companyGroup = "SG" Or companyGroup = "LS" Then
            result += Print_SingleRoller(headerId)
        End If
        result += Print_DBRoller(headerId)
        result += Print_LinkDepRoller(headerId)
        result += Print_LinkIndRoller(headerId)
        result += Print_LinkInd4Roller(headerId)
        result += Print_DBLinkIndRoller_4Blinds(headerId)
        result += Print_DBLinkDepRoller_4Blinds(headerId)
        result += Print_DBLinkDepRoller_6Blinds(headerId)
        result += Print_DBLinkIndRoller_6Blinds(headerId)
        result += Print_Roman(headerId)
        result += Print_Privacy(headerId)
        result += Print_VenetianCordless(headerId)
        result += Print_VenetianSingle(headerId)
        result += Print_Ventian2on1(headerId)
        result += Print_Ventian3on1(headerId)
        result += Print_SaphoraDrape(headerId)
        result += Print_CompleteVertical(headerId)
        result += Print_TrackVertical(headerId)
        result += Print_SlatVertical(headerId)

        result += Print_ShutterOcean(headerId)
        result += Print_ShutterExpress(headerId)
        result += Print_ShutterPanel(headerId)
        result += Print_ShutterFrame(headerId)
        result += Print_ShutterHinged(headerId)
        result += Print_ShutterHingedBiFold(headerId)
        result += Print_ShutterFixed(headerId)
        result += Print_ShutterTrackBiFold(headerId)

        result += Print_Window(headerId)

        result += Print_DoorHinged(headerId)
        result += Print_DoorSliding(headerId)

        result += Print_AustralianCurtainComplete(headerId)
        result += Print_AustralianCurtainOnly(headerId)
        result += Print_CurtainCL(headerId)

        Dim todayString As String = DateTime.Now.ToString("ddMMyyyyHHmmss")
        Dim fileName As String = String.Format("{0}_{1}.pdf", lblOrderNumber.Text, todayString)

        Dim fileDirectory As String = Server.MapPath("~/File/Archive/")

        Using stream As FileStream = New FileStream(fileDirectory + "/" + fileName, FileMode.Create)
            Dim pdfDoc As Document = New Document(PageSize.A4.Rotate)
            Dim writer As PdfWriter = PdfWriter.GetInstance(pdfDoc, stream)
            pdfDoc.Open()
            Dim sr As StringReader = New StringReader(result)
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr)
            pdfDoc.Close()
            stream.Close()
        End Using

        Response.Clear()

        Dim url As String = String.Format("~/order/archive/preview?orderid={0}&filename={1}", headerId, fileName)
        Response.Redirect(url, False)
    End Sub

    Protected Function Print_HeaderTemplate(HeaderId As String) As String
        Dim result As String = ""

        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Header WHERE OrdID = '" + HeaderId + "'")

        Dim vStartTr As String = "<tr>"
        Dim vEndTr As String = "</tr>"
        Dim vEndTd As String = "</td>"

        result += "<table style='width:100%;margin-bottom:10px;margin-top:25px;font-size:smaller;'>"
        result += vStartTr
        result += "<td style='vertical-align:top;width:40%;font-size:small;'>"
        If companyGroup = "JPMD" Then
            result += "<img width='230px' src='https://ordersblindonline.com/image/jpmd_logo_new.jpg' />"
        End If
        If companyGroup = "LOCAL" Then
            result += "<img width='230px' src='https://ordersblindonline.com/image/local.png' />"
        End If
        If companyGroup = "SG" Then
            result += "<img width='230px' src='https://ordersblindonline.com/image/sg_logo.jpg' />"
        End If
        If companyGroup = "LS" Then
            result += "<img width='230px' src='https://ordersblindonline.com/image/lifestyle.png' />"
        End If
        result += "<br />"
        If companyGroup = "JPMD" Then
            result += "<p style='font-size:small;'>"
            result += "<b>JPM Direct Pty Ltd</b>"
            result += "<br />"
            result += "Suite 265 97-99 Bathurst Street"
            result += "<br />"
            result += "Sydney, NSW 2000"
            result += "<br />"
            result += "Phone : 0417 705 109 &nbsp;&nbsp;&nbsp; Email : order@jpmdirect.com.au"
            result += "</p>"
        End If
        result += vEndTd

        result += "<td style='vertical-align:top;width:60%;font-size:small;'>"
        result += "<table style='width:100%;font-size:smaller;'>"
        result += vStartTr
        result += "<td style='vertical-align:top;font-size:small;'>"
        result += "<table style='width:100%;font-size:small;'>"

        result += vStartTr
        result += "<td style='width:190px;font-size:small;'>Debtor / Store Name</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & lblCustomerName.Text & "</td>"
        result += vEndTr

        result += vStartTr
        result += "<td style='width:190px;font-size:small;'>Store Order No (Store Customer)</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & thisData.Tables(0).Rows(0).Item("StoreOrderNo").ToString & " (" & thisData.Tables(0).Rows(0).Item("StoreCustomer").ToString & ")" & "</td>"
        result += vEndTr

        result += vStartTr
        result += "<td style='width:190px;font-size:small;'>User Login & Login Name</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td>" & thisData.Tables(0).Rows(0).Item("UserLogin").ToString & " & " & thisData.Tables(0).Rows(0).Item("LoginName").ToString & "</td>"
        result += vEndTr

        result += vStartTr
        result += "<td style='width:190px;'>Created (Submitted Date)</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & thisData.Tables(0).Rows(0).Item("CreatedDate").ToString & " (Submit : " & thisData.Tables(0).Rows(0).Item("SubmittedDate").ToString & ")" & "</td>"
        result += vEndTr

        result += vStartTr
        result += "<td style='width:190px;'>Total Item Order</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND (BlindType <> 'Surcharge' AND BlindType<>'Fee Installation') AND Active=1") & " Items" & "</td>"
        result += vEndTr
        result += "</table>"
        result += vEndTd
        result += vEndTr

        result += vStartTr
        result += "<td style='vertical-align:top;font-size:smaller;'>"
        result += "<table style='width:100%;font-size:smaller;'>"
        result += vStartTr
        result += "<td>Description Item :</td>"
        result += vEndTr

        result += vStartTr
        result += "<td>" & BindDescOrderItem(HeaderId, companyGroup) & "</td>"
        result += vEndTr

        result += "</table>"
        result += vEndTd
        result += vEndTr

        result += "</table>"
        result += vEndTd
        result += vEndTr
        result += "</table>"

        Return result
    End Function

    Protected Function Print_AluSingle(HeaderId As String) As String
        Dim result As String = ""
        Dim thisQuery As String = "SELECT Products.Name As ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.VenId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Ven' AND Order_Detail.SubType='Single' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC"

        Dim GetUserLogin As String = archiveClass.GetItemData("SELECT UserLogin FROM Order_Header WHERE OrdID = '" + HeaderId + "'")
        If GetUserLogin = "CW_Systems" Then
            thisQuery = "SELECT Products_CWS.Name As ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.VenId LEFT JOIN Products_CWS ON HardwareKits.ProductId = Products_CWS.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Ven' AND Order_Detail.SubType='Single' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC"
        End If

        Dim thisData As DataSet = archiveClass.GetListData(thisQuery)

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>SINGLE ALUMINIUM</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Tilter" & vEndTh
            result += thBorder & "Cord Length" & vEndTh
            result += thBorder & "Wand Length" & vEndTh
            result += thBorder & "Hold Down Clip" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("TilterPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("PullCordLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Sample(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType='Sample' AND Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>SAMPLE</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Type" & vEndTh
            result += thBorder & "Fabric" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrderType").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Alu2on1(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.VenId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType = 'Ven' AND Order_Detail.SubType <> 'Single' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:22px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_rowspan As String = " <td rowspan='2' style='text-align:center;word-wrap: break-word;height:22px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspan As String = " <td colspan='11' style='text-align:left;margin-left:5px;word-wrap: break-word;height:22px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>2 ON 1 ALUMINIUM</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "CTRL & Tilter" & vEndTh
            result += thBorder & "Cord Length" & vEndTh
            result += thBorder & "Wand Length" & vEndTh
            result += thBorder & "Hold Down Clip" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vControl As String = thisData.Tables(0).Rows(i).Item("ControlPosition").ToString()
                Dim vTilter As String = thisData.Tables(0).Rows(i).Item("TilterPosition").ToString()

                Dim vControlB As String = thisData.Tables(0).Rows(i).Item("ControlPositionDB3b").ToString()
                Dim vTilterB As String = thisData.Tables(0).Rows(i).Item("RollDB3b").ToString()
                result += "<tr>"
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & vControl & " " & vTilter & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("PullCordLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & vControlB & " " & vTilterB & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspan & "<b style='margin-left:50px;color:red;'><u>SPECIAL INFORMATION</u></b> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Cellular(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType = 'Cellular Shades' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>CELLULAR SHADES</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "CS Material" & vEndTh
            result += thBorder & "CS Material (Day & Night)" & vEndTh
            result += thBorder & "Cord Position" & vEndTh
            result += thBorder & "Cord Length" & vEndTh
            result += thBorder & "Hold Down Clip" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricType2a").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Curtain(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType = 'Curtain' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>CURTAIN</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Stack" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric" & vEndTh
            result += thBorder & "Lining" & vEndTh
            result += thBorder & "Return (L)" & vEndTh
            result += thBorder & "Return (R)" & vEndTh
            result += thBorder & "Tie Back" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vControl As String = ""
                If thisData.Tables(0).Rows(i).Item("ControlType").ToString() = "Cord" Then
                    vControl = thisData.Tables(0).Rows(i).Item("ControlType").ToString() & " - " & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString()
                End If
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & vControl & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlOperation").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength2").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional1").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_AustralianCurtainComplete(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType LIKE '%Curtain%' AND (OrderType='Single Curtain and Track' OR OrderType='Double Curtain and Track') AND Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = "<td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            Dim tdBorder_colspan As String = "<td colspan='5' style='text-align:center;height:34px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_colspan_notes As String = "<td colspan='19' style='text-align:left;margin-left:5px;word-wrap: break-word;height:22px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 11px;'>COMPLETE SET AUSTRALIAN CURTAIN</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Fitting" & vEndTh
            result += thBorder & "Curtain Type" & vEndTh

            result += thBorder & "Heading" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric" & vEndTh
            result += thBorder & "Lining" & vEndTh
            result += thBorder & "Track" & vEndTh
            result += thBorder & "Track Draw" & vEndTh
            result += thBorder & "Control Colour" & vEndTh
            result += thBorder & "Length" & vEndTh
            result += thBorder & "Stacking" & vEndTh

            result += thBorder & "Bottom HEM" & vEndTh
            result += thBorder & "Tie Back" & vEndTh
            result += thBorder & "Return (L)" & vEndTh
            result += thBorder & "Return (R)" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vTrack As String = thisData.Tables(0).Rows(i).Item("Roll").ToString() & " - " & thisData.Tables(0).Rows(i).Item("TrackColour").ToString()

                Dim vTrackB As String = thisData.Tables(0).Rows(i).Item("Roll2a").ToString() & " - " & thisData.Tables(0).Rows(i).Item("ControlColour").ToString()
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrderType").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Product").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd
                result += tdBorder & vTrack & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlType").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2a").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthdb6").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlOperation").ToString() & vEndTd

                result += tdBorder & thisData.Tables(0).Rows(i).Item("BrColour").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional2").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength2").ToString() & vEndTd
                result += "</tr>"

                If thisData.Tables(0).Rows(i).Item("OrderType").ToString() = "Double Curtain and Track" Then
                    result += "<tr>"
                    result += tdBorder_colspan & "SECOND CURTAIN" & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("AwningType").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricType2a").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional1").ToString() & vEndTd
                    result += tdBorder & vTrackB & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("BottomJoin").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2b").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthdb7").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("ValanceType").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("BrColour").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional2").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & vEndTd
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength2").ToString() & vEndTd
                    result += "</tr>"
                End If

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspan_notes & "<b style='margin-left:50px;color:red;'><u>SPECIAL INFORMATION</u></b> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_AustralianCurtainOnly(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType LIKE '%Curtain%' AND OrderType='Curtain Only' AND Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = "<td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            Dim tdBorder_colspan_notes As String = "<td colspan='13' style='text-align:left;margin-left:5px;word-wrap: break-word;height:22px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 11px;'>AUSTRALIAN CURTAIN (CURTAIN ONLY)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Fitting" & vEndTh

            result += thBorder & "Heading" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric" & vEndTh
            result += thBorder & "Lining" & vEndTh

            result += thBorder & "Bottom HEM" & vEndTh
            result += thBorder & "Tie Back" & vEndTh
            result += thBorder & "Return (L)" & vEndTh
            result += thBorder & "Return (R)" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Product").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd

                result += tdBorder & thisData.Tables(0).Rows(i).Item("BrColour").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional2").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength2").ToString() & vEndTd
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspan_notes & "<b style='margin-left:50px;color:red;'><u>SPECIAL INFORMATION</u></b> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_CurtainCL(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType = 'Curtain' AND Order_Detail.OrderType = 'Curtain CL' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>CURTAIN CL</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Cut Width" & vEndTh
            result += thBorder & "Cut Drop" & vEndTh
            result += thBorder & "Fabric" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Linea(HeaderId As String) As String
        Dim result As String = ""

        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Linea Valance' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap:break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>LINEA VALANCE</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Fabric Insert" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Bracket Type" & vEndTh
            result += thBorder & "Is Blind In" & vEndTh
            result += thBorder & "Valance Return" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vValance As String = ""
                If Not thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() = "" Then
                    vValance = thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() & " - " & thisData.Tables(0).Rows(i).Item("ValanceReturnSize").ToString() & "mm"
                End If
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricInsert").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Product").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & vValance & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Panel(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Panel Glide' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:23px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"


            result += "<span style='font-size: 11px;'>PANEL GLIDE</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Timber Batten" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Layout" & vEndTh
            result += thBorder & "Tracks" & vEndTh
            result += thBorder & "Panels" & vEndTh
            result += thBorder & "Wand Length" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Baton").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("LayoutCode").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Tracks").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Panel").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Pelmet(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND (Order_Detail.BlindType='Pelmet' OR Order_Detail.BlindType='Fabric') AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = "<td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>PELMET</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Timber Batten" & vEndTh
            result += thBorder & "Layout" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "2nd Width" & vEndTh
            result += thBorder & "3rd Width" & vEndTh
            result += thBorder & "Fabric" & vEndTh
            result += thBorder & "Return Position" & vEndTh
            result += thBorder & "Ret Length (L)" & vEndTh
            result += thBorder & "Ret Length (R)" & vEndTh
            result += thBorder & "Dust Cover" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Baton").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("PelmetLayout").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ValanceReturnSize").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength2").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Profile(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Profile' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = "<td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>PROFILE</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Stack Option" & vEndTh
            result += thBorder & "Control Position" & vEndTh
            result += thBorder & "Control Colour" & vEndTh
            result += thBorder & "Control Length" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlOperation").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlColour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                result += "</tr>"
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Roman(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Roman' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:23px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>ROMAN BLINDS</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Timber Batten" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Control Position" & vEndTh
            result += thBorder & "Control Colour" & vEndTh
            result += thBorder & "Control Length" & vEndTh
            result += thBorder & "Valance Option" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Baton").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RomanChainColour").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RomanChainLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RomanValanceOption").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Privacy(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.VenId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Smart Privacy' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = "<td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>SMART PRIVACY</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Control Position" & vEndTh
            result += thBorder & "Tilter Position" & vEndTh
            result += thBorder & "Control Length" & vEndTh
            result += thBorder & "Hold Down Clip" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("TilterPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("PullCordLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_VenetianCordless(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.VenId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Venetian' AND Order_Detail.OrderType='Econo 50mm (Cordless)' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>CORDLESS VENETIAN</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Tilter Position" & vEndTh
            result += thBorder & "Wand Length" & vEndTh
            result += thBorder & "Hold Down Clip" & vEndTh
            result += thBorder & "Valance" & vEndTh
            result += thBorder & "Valance Return" & vEndTh

            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vValance As String = "Type : " & thisData.Tables(0).Rows(i).Item("ValanceType").ToString()
                vValance += "<br />"
                vValance += "Size : " & thisData.Tables(0).Rows(i).Item("ValanceSize").ToString()

                If thisData.Tables(0).Rows(i).Item("ValanceType").ToString() = "No Valance" Or thisData.Tables(0).Rows(i).Item("ValanceType").ToString() = "" Then
                    vValance = ""
                End If

                Dim vValanceReturn As String = thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() & " - " & thisData.Tables(0).Rows(i).Item("ValanceReturnSize").ToString() & "mm"

                If thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() = "" Then
                    vValanceReturn = ""
                End If

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("TilterPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += tdBorder & vValance & vEndTd
                result += tdBorder & vValanceReturn & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_VenetianSingle(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType='Venetian' AND SubType='Single' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:23px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspan_notes As String = " <td colspan='13' style='text-align:left;word-wrap: break-word;height:23px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>SINGLE VENETIAN</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "CTRL - Tilter" & vEndTh
            result += thBorder & "Cord Length" & vEndTh
            result += thBorder & "Hold Down Clip" & vEndTh
            result += thBorder & "Tassel Option" & vEndTh
            result += thBorder & "Valance" & vEndTh
            result += thBorder & "Valance Return" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vProductName As String = thisData.Tables(0).Rows(i).Item("BlindType").ToString() & ", " & thisData.Tables(0).Rows(i).Item("OrderType").ToString() & ", " & thisData.Tables(0).Rows(i).Item("Colour").ToString()
                Dim vValanceS As String = "Type : " & thisData.Tables(0).Rows(i).Item("ValanceType").ToString()

                Dim vValance As String = "Type : " & thisData.Tables(0).Rows(i).Item("ValanceType").ToString()
                vValance += "<br />"
                vValance += "Size : " & thisData.Tables(0).Rows(i).Item("ValanceSize").ToString() & "mm"

                If thisData.Tables(0).Rows(i).Item("ValanceType").ToString() = "No Valance" Or thisData.Tables(0).Rows(i).Item("ValanceType").ToString() = "" Then
                    vValance = "No Valance"
                End If

                Dim vValanceReturn As String = thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() & " - " & thisData.Tables(0).Rows(i).Item("ValanceReturnSize").ToString() & "mm"

                If thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() = "" Then
                    vValanceReturn = ""
                End If

                Dim vControl As String = thisData.Tables(0).Rows(i).Item("ControlPosition").ToString()
                Dim vTilter As String = thisData.Tables(0).Rows(i).Item("TilterPosition").ToString()
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & vProductName & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & vControl & " - " & vTilter & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("PullCordLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlColour").ToString() & vEndTd
                result += tdBorder & vValance & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() & " - " & thisData.Tables(0).Rows(i).Item("ValanceReturnSize").ToString() & "mm" & vEndTd
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspan_notes & "<span style='margin-left:10px;font-size:7px;color:red;'><b><u>SPECIAL INFORMATION : </u></b></span>" & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Ventian2on1(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType = 'Venetian' AND SubType LIKE '%2 on 1%' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:22px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_rowspan As String = " <td rowspan='2' style='text-align:center;word-wrap: break-word;height:22px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspan_notes As String = " <td colspan='13' style='text-align:left;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>2 ON 1 VENETIAN</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "CTRL - Tilter" & vEndTh
            result += thBorder & "Cord Length" & vEndTh
            result += thBorder & "Hold Down Clip" & vEndTh
            result += thBorder & "Tassel Option" & vEndTh
            result += thBorder & "Valance" & vEndTh
            result += thBorder & "Valance Return" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vProductName As String = thisData.Tables(0).Rows(i).Item("BlindType").ToString() & ", " & thisData.Tables(0).Rows(i).Item("OrderType").ToString() & ", " & thisData.Tables(0).Rows(i).Item("Colour").ToString()
                Dim vValance As String = "Type : " & thisData.Tables(0).Rows(i).Item("ValanceType").ToString()
                vValance += "<br />"
                vValance += "Size : " & thisData.Tables(0).Rows(i).Item("ValanceSize").ToString()
                If thisData.Tables(0).Rows(i).Item("ValanceType").ToString() = "No Valance" Or thisData.Tables(0).Rows(i).Item("ValanceType").ToString() = "No Valance" Then
                    vValance = ""
                End If

                Dim vValanceReturn As String = thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() & " - " & thisData.Tables(0).Rows(i).Item("ValanceReturnSize").ToString() & "mm"

                If thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() = "" Then
                    vValanceReturn = ""
                End If

                Dim vControl As String = thisData.Tables(0).Rows(i).Item("ControlPosition").ToString()
                Dim vTilter As String = thisData.Tables(0).Rows(i).Item("TilterPosition").ToString()
                Dim vControlB As String = thisData.Tables(0).Rows(i).Item("ControlPositionDB3b").ToString()
                Dim vTilterB As String = thisData.Tables(0).Rows(i).Item("Additional2").ToString()

                result += "<tr>"
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder_rowspan & vProductName & "</td>"
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & vControl & " - " & vTilter & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("PullCordLength").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlColour").ToString() & "</td>"
                result += tdBorder_rowspan & vValance & "</td>"
                result += tdBorder_rowspan & vValanceReturn & "</td>"
                result += "</tr>"

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2c").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & vControlB & " - " & vTilterB & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthdb4").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlColour").ToString() & "</td>"
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspan_notes & "<span style='margin-left:10px;font-size:7px;color:red;'><b><u>SPECIAL INFORMATION : </u></b></span>" & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Ventian3on1(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType = 'Venetian' AND SubType LIKE '%3 on 1%' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:22px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_rowspan As String = " <td rowspan='3' style='text-align:center;word-wrap: break-word;height:22px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspan As String = "<td colspan='5' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_colspan_notes As String = " <td colspan='13' style='text-align:left;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>3 ON 1 VENETIAN</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "CTRL - Tilter" & vEndTh
            result += thBorder & "Cord Length" & vEndTh
            result += thBorder & "Hold Down Clip" & vEndTh
            result += thBorder & "Tassel Option" & vEndTh
            result += thBorder & "Valance" & vEndTh
            result += thBorder & "Valance Return" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vProductName As String = thisData.Tables(0).Rows(i).Item("BlindType").ToString() & ", " & thisData.Tables(0).Rows(i).Item("OrderType").ToString() & ", " & thisData.Tables(0).Rows(i).Item("Colour").ToString()
                Dim vValance As String = "Type : " & thisData.Tables(0).Rows(i).Item("ValanceType").ToString()
                vValance += "<br />"
                vValance += "Size : " & thisData.Tables(0).Rows(i).Item("ValanceSize").ToString()
                If thisData.Tables(0).Rows(i).Item("ValanceType").ToString() = "No Valance" Or thisData.Tables(0).Rows(i).Item("ValanceType").ToString() = "No Valance" Then
                    vValance = ""
                End If

                Dim vValanceReturn As String = thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() & " - " & thisData.Tables(0).Rows(i).Item("ValanceReturnSize").ToString() & "mm"

                If thisData.Tables(0).Rows(i).Item("ValancePosition").ToString() = "" Then
                    vValanceReturn = ""
                End If

                Dim vControl As String = thisData.Tables(0).Rows(i).Item("ControlPosition").ToString()
                Dim vTilter As String = thisData.Tables(0).Rows(i).Item("TilterPosition").ToString()
                Dim vControlB As String = thisData.Tables(0).Rows(i).Item("ControlPositionDB3b").ToString()
                Dim vTilterB As String = thisData.Tables(0).Rows(i).Item("Additional2").ToString()
                Dim vControlC As String = thisData.Tables(0).Rows(i).Item("Additional").ToString()
                Dim vTilterC As String = thisData.Tables(0).Rows(i).Item("Additional1").ToString()

                result += "<tr>"
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder_rowspan & vProductName & "</td>"
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & vControl & " - " & vTilter & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("PullCordLength").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlColour").ToString() & "</td>"
                result += tdBorder_rowspan & vValance & "</td>"
                result += tdBorder_rowspan & vValanceReturn & "</td>"
                result += "</tr>"

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2c").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & vControlB & " - " & vTilterB & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthdb4").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlColour").ToString() & "</td>"
                result += "</tr>"

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("width3c").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & vControlC & " - " & vTilterC & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlColour").ToString() & "</td>"
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspan_notes & "<span style='margin-left:10px;font-size:7px;color:red;'><b><u>SPECIAL INFORMATION : </u></b></span>" & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_SaphoraDrape(HeaderId As String) As String
        Dim result As String = ""

        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Saphora Drape' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = "<td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>SAPHORA DRAPE</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Stack Option" & vEndTh
            result += thBorder & "Control Position" & vEndTh
            result += thBorder & "Control Length" & vEndTh
            result += thBorder & "Bottom Join" & vEndTh
            result += thBorder & "Ext Bracket" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vControlColour As String = thisData.Tables(0).Rows(i).Item("ChainType").ToString() & thisData.Tables(0).Rows(i).Item("VerWandColour").ToString()
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlOperation").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BottomJoin").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Extbracket").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_CompleteVertical(HeaderId As String) As String
        Dim result As String = ""

        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Vertical' AND Order_Detail.OrderType LIKE '%Complete Set%' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = "<td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>VERTICAL (COMPLETE SET)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Insert" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Stack Option" & vEndTh
            result += thBorder & "Control Position" & vEndTh
            result += thBorder & "Control Colour" & vEndTh
            result += thBorder & "Control Length" & vEndTh
            result += thBorder & "Bottom Join" & vEndTh
            result += thBorder & "Ext Bracket" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vControlColour As String = thisData.Tables(0).Rows(i).Item("ChainType").ToString() & thisData.Tables(0).Rows(i).Item("VerWandColour").ToString()
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricInsert").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlOperation").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & vControlColour & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BottomJoin").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Extbracket").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_TrackVertical(HeaderId As String) As String
        Dim result As String = ""

        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType='Vertical' AND OrderType LIKE '%Track Only%' AND Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = "<td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>VERTICAL (TRACK ONLY)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Blade Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Fabric Insert" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Stack Option" & vEndTh
            result += thBorder & "Control Position" & vEndTh
            result += thBorder & "Control Colour" & vEndTh
            result += thBorder & "Control Length" & vEndTh
            result += thBorder & "Ext Bracket" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vControlColour As String = thisData.Tables(0).Rows(i).Item("ChainType").ToString() & thisData.Tables(0).Rows(i).Item("VerWandColour").ToString()
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OtherQty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & thisData.Tables(0).Rows(i).Item("ControlType").ToString() & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricInsert").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlOperation").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & vControlColour & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Extbracket").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_SlatVertical(HeaderId As String) As String
        Dim result As String = ""

        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Vertical Slat Only' AND Order_Detail.OrderType LIKE '%Slat Only%' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = "<td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>VERTICAL (SLAT ONLY)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Blade Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Bottom Join" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OtherQty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BottomJoin").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_SingleRoller(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Bottoms.Name AS BotName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id LEFT JOIN Bottoms ON Order_Detail.IDBottomRail = Bottoms.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Roller' AND (Order_Detail.OrderType='Regular' OR Order_Detail.OrderType='Regular Chain' OR Order_Detail.OrderType='Full Cassette' OR Order_Detail.OrderType='Semi Cassette' OR Order_Detail.OrderType='Wire Guide' OR Order_Detail.OrderType='Single Motorized') AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            Dim tdBorder_colspanNotes As String = "<td colspan='16' style='text-align:center;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 11px;'>ROLLER REGULAR / MOTOR</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Roll" & vEndTh
            result += thBorder & "Bottom Rail" & vEndTh
            result += thBorder & "Flat Option" & vEndTh
            result += thBorder & "Chain / Remote" & vEndTh
            result += thBorder & "Chain Stopper" & vEndTh
            result += thBorder & "Chain Length" & vEndTh
            result += thBorder & "SB Bracket Size" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vFinalProduct As String = thisData.Tables(0).Rows(i).Item("ProductName").ToString()
                If vFinalProduct = "" Then
                    Dim gearsb As String = thisData.Tables(0).Rows(i).Item("LayoutCode").ToString()
                    Dim acmeda As String = thisData.Tables(0).Rows(i).Item("TrackColour").ToString()
                    Dim tubeType As String = thisData.Tables(0).Rows(i).Item("ValanceType").ToString()

                    vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"

                    If Not String.IsNullOrEmpty(gearsb) And Not gearsb = "No" Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & gearsb & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                    If Not String.IsNullOrEmpty(acmeda) And Not acmeda = "No" Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & gearsb & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                    If Not String.IsNullOrEmpty(tubeType) Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & tubeType & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                End If

                Dim itemId As String = thisData.Tables(0).Rows(i).Item("OrddID").ToString()
                Dim printing As Integer = archiveClass.GetItemData_Integer("SELECT COUNT(*) FROM Printing WHERE ItemId = '" + itemId + "'")
                If printing > 0 Then
                    vFinalProduct += "<br />"
                    vFinalProduct += "<b><u>PRINTING FABRIC</u></b>"
                End If

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & vFinalProduct & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainType").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollerChainLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OtherQty").ToString() & vEndTd
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_SingleRollerLocal(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Bottoms.Name AS BotName, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id LEFT JOIN Bottoms ON Order_Detail.IDBottomRail = Bottoms.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Roller' AND (Order_Detail.OrderType='Regular' OR Order_Detail.OrderType='Regular Chain' OR Order_Detail.OrderType='Full Cassette' OR Order_Detail.OrderType='Semi Cassette' OR Order_Detail.OrderType='Wire Guide' OR Order_Detail.OrderType='Single Motorized') AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = "<td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>ROLLER REGULAR / MOTOR</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Top Track" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Roll" & vEndTh
            result += thBorder & "Bottom Rail" & vEndTh
            result += thBorder & "Flat Option" & vEndTh
            result += thBorder & "Chain / Remote" & vEndTh
            result += thBorder & "Chain Stopper" & vEndTh
            result += thBorder & "Chain Length" & vEndTh
            result += thBorder & "Solar Panel" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                Dim vFinalProduct As String = thisData.Tables(0).Rows(i).Item("ProductName").ToString()
                If vFinalProduct = "" Then
                    vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                End If
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & vFinalProduct & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("PelmetLayout").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainType").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollerChainLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_DBRoller(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Bot.Name AS BotName, BotB.Name AS BotNameB, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id LEFT JOIN Bottoms AS Bot ON Order_Detail.IDBottomRail = Bot.Id LEFT JOIN Bottoms AS BotB ON Order_Detail.IDBottomRail2a = BotB.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Roller' AND Order_Detail.OrderType='Double Bracket' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:32px;font-size: 7px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_rowspan As String = " <td rowspan='2' style='text-align:center;word-wrap: break-word;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspan As String = "<td colspan='5' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspan_notes As String = " <td colspan='16' style='text-align:left;word-wrap: break-word;height:20px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>ROLLER (DOUBLE BRACKET)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Roll" & vEndTh
            result += thBorder & "Bottom Rail" & vEndTh
            result += thBorder & "Flat Option" & vEndTh
            result += thBorder & "Chain Colour" & vEndTh
            result += thBorder & "Chain Stopper" & vEndTh
            result += thBorder & "Chain Length" & vEndTh
            result += thBorder & "Solar Panel" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vFinalProduct As String = thisData.Tables(0).Rows(i).Item("ProductName").ToString()
                If vFinalProduct = "" Then
                    Dim gearsb As String = thisData.Tables(0).Rows(i).Item("LayoutCode").ToString()
                    Dim acmeda As String = thisData.Tables(0).Rows(i).Item("TrackColour").ToString()

                    vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"

                    If Not String.IsNullOrEmpty(gearsb) And Not gearsb = "No" Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & gearsb & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                    If Not String.IsNullOrEmpty(acmeda) And Not acmeda = "No" Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & gearsb & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                End If
                Dim itemId As String = thisData.Tables(0).Rows(i).Item("OrddID").ToString()
                Dim printing As Integer = archiveClass.GetItemData_Integer("SELECT COUNT(*) FROM Printing WHERE ItemId = '" + itemId + "'")
                If printing > 0 Then
                    vFinalProduct += "<br />"
                    vFinalProduct += "<b><u>PRINTING FABRIC</u></b>"
                End If

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & vFinalProduct & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotName").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainType").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollerChainLength").ToString() & "</td>"
                result += tdBorder_rowspan & thisData.Tables(0).Rows(i).Item("Supply").ToString() & "</td>"

                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "SECOND BLIND" & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricType2a").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPositionDB3b").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll2a").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameB").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2a").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Chain2a").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional1").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RolChainLength2c").ToString() & "</td>"
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspan_notes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_LinkDepRoller(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Bot.Name AS BotName, BotB.Name AS BotNameB, BotC.Name AS BotNameC, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id LEFT JOIN Bottoms AS Bot ON Order_Detail.IDBottomRail = Bot.Id LEFT JOIN Bottoms AS BotB ON Order_Detail.IDBottomRail2b = BotB.Id LEFT JOIN Bottoms AS BotC ON Order_Detail.IDBottomRail3b = BotC.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Roller' AND (Order_Detail.OrderType='Link Dep 2 Blinds' OR Order_Detail.OrderType='Link Dep 3 Blinds' OR Order_Detail.OrderType='Link System Dependent 2 Blinds' OR Order_Detail.OrderType='Link System Dependent 3 Blinds') AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspan As String = "<td colspan='5' style='text-align:center;height:20px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspanNotes As String = "<td colspan='15' style='text-align:center;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"

            Dim vEndTd As String = "</td>"

            result += "<span style='font-size: 11px;'>ROLLER (LINK DEPENDENT)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Roll" & vEndTh
            result += thBorder & "Bottom Rail" & vEndTh
            result += thBorder & "Flat Option" & vEndTh
            result += thBorder & "Chain Colour" & vEndTh
            result += thBorder & "Chain Stopper" & vEndTh
            result += thBorder & "Chain Length" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vFinalProduct As String = thisData.Tables(0).Rows(i).Item("ProductName").ToString()
                If vFinalProduct = "" Then
                    Dim gearsb As String = thisData.Tables(0).Rows(i).Item("LayoutCode").ToString()
                    Dim acmeda As String = thisData.Tables(0).Rows(i).Item("TrackColour").ToString()

                    vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"

                    If Not String.IsNullOrEmpty(gearsb) And Not gearsb = "No" Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & gearsb & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                    If Not String.IsNullOrEmpty(acmeda) And Not acmeda = "No" Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & gearsb & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                End If
                Dim itemId As String = thisData.Tables(0).Rows(i).Item("OrddID").ToString()
                Dim printing As Integer = archiveClass.GetItemData_Integer("SELECT COUNT(*) FROM Printing WHERE ItemId = '" + itemId + "'")
                If printing > 0 Then
                    vFinalProduct += "<br />"
                    vFinalProduct += "<b><u>PRINTING FABRIC</u></b>"
                End If

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & vFinalProduct & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotName").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainType").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollerChainLength").ToString() & "</td>"
                result += "</tr>"

                result += "<tr>"
                Dim vOrderType As String = thisData.Tables(0).Rows(i).Item("OrderType").ToString()
                If vOrderType = "Link Dep 2 Blinds" Or vOrderType = "Link System Dependent 2 Blinds" Then
                    result += tdBorder_colspan & "SECOND BLINDS (END)" & "</td>"
                End If

                If vOrderType = "Link Dep 3 Blinds" Or vOrderType = "Link System Dependent 3 Blinds" Then
                    result += tdBorder_colspan & "SECOND BLINDS (MIDDLE)" & "</td>"
                End If

                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2b").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricType2b").ToString() & "</td>"
                result += tdBorder & "" & "</td>"
                result += tdBorder & "" & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameB").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2b").ToString() & "</td>"
                result += tdBorder & "" & "</td>"
                result += tdBorder & "" & "</td>"
                result += tdBorder & "" & "</td>"
                result += "</tr>"

                If vOrderType = "Link Dep 3 Blinds" Or vOrderType = "Link System Dependent 3 Blinds" Then
                    result += "<tr>"
                    result += tdBorder_colspan & "THIRD BLIND (END)" & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("Width3b").ToString() & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricType3b").ToString() & "</td>"
                    result += tdBorder & "" & "</td>"
                    result += tdBorder & "" & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameC").ToString() & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp3b").ToString() & "</td>"
                    result += tdBorder & "" & "</td>"
                    result += tdBorder & "" & "</td>"
                    result += tdBorder & "" & "</td>"
                    result += "</tr>"
                End If

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_LinkIndRoller(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Bot.Name AS BotName, BotB.Name AS BotNameB, BotC.Name AS BotNameC, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id LEFT JOIN Bottoms AS Bot ON Order_Detail.IDBottomRail = Bot.Id LEFT JOIN Bottoms AS BotB ON Order_Detail.IDBottomRail2c = BotB.Id LEFT JOIN Bottoms AS BotC ON Order_Detail.IDBottomRail3c = BotC.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Roller' AND (Order_Detail.OrderType='Link Ind 2 Blinds' OR Order_Detail.OrderType='Link Ind 3 Blinds' OR Order_Detail.OrderType='Link System Independent 2 Blinds' OR Order_Detail.OrderType='Link System Independent 3 Blinds') AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:40px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_rowspan As String = " <td rowspan='2' style='text-align:center;word-wrap: break-word;height:36px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspan As String = "<td colspan='5' style='text-align:center;height:34px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspanNotes As String = "<td colspan='15' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>ROLLER (LINK INDEPENDENT)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Roll" & vEndTh
            result += thBorder & "Bottom Rail" & vEndTh
            result += thBorder & "Flat Option" & vEndTh
            result += thBorder & "Chain Colour" & vEndTh
            result += thBorder & "Chain Stopper" & vEndTh
            result += thBorder & "Chain Length" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vStopper As String = ""
                Dim vStopperB As String = ""
                Dim vStopperC As String = ""
                If thisData.Tables(0).Rows(i).Item("Additional").ToString() = "With Stopper" Then
                    vStopper = "Yes"
                End If
                If thisData.Tables(0).Rows(i).Item("Additional").ToString() = "No Stopper" Then
                    vStopper = "No"
                End If
                If thisData.Tables(0).Rows(i).Item("Additional1").ToString() = "With Stopper" Then
                    vStopperB = "Yes"
                End If
                If thisData.Tables(0).Rows(i).Item("Additional1").ToString() = "No Stopper" Then
                    vStopperB = "No"
                End If
                If thisData.Tables(0).Rows(i).Item("Additional2").ToString() = "With Stopper" Then
                    vStopperC = "Yes"
                End If
                If thisData.Tables(0).Rows(i).Item("Additional2").ToString() = "No Stopper" Then
                    vStopperC = "No"
                End If
                Dim vOrderType As String = thisData.Tables(0).Rows(i).Item("OrderType").ToString()

                Dim vFinalProduct As String = thisData.Tables(0).Rows(i).Item("ProductName").ToString()
                If vFinalProduct = "" Then
                    Dim gearsb As String = thisData.Tables(0).Rows(i).Item("LayoutCode").ToString()
                    Dim acmeda As String = thisData.Tables(0).Rows(i).Item("TrackColour").ToString()

                    vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"

                    If Not String.IsNullOrEmpty(gearsb) And Not gearsb = "No" Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & gearsb & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                    If Not String.IsNullOrEmpty(acmeda) And Not acmeda = "No" Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & gearsb & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                End If
                Dim itemId As String = thisData.Tables(0).Rows(i).Item("OrddID").ToString()
                Dim printing As Integer = archiveClass.GetItemData_Integer("SELECT COUNT(*) FROM Printing WHERE ItemId = '" + itemId + "'")
                If printing > 0 Then
                    vFinalProduct += "<br />"
                    vFinalProduct += "<b><u>PRINTING FABRIC</u></b>"
                End If

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & vFinalProduct & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotName").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainType").ToString() & "</td>"
                result += tdBorder & vStopper & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollerChainLength").ToString() & "</td>"
                result += "</tr>"

                result += "<tr>"
                If vOrderType = "Link Ind 2 Blinds" Or vOrderType = "Link System Independent 2 Blinds" Then
                    result += tdBorder_colspan & "SECOND BLIND (END)" & "</td>"
                End If
                If vOrderType = "Link Ind 3 Blinds" Or vOrderType = "Link System Independent 3 Blinds" Then
                    result += tdBorder_colspan & "SECOND BLIND (MIDDLE)" & "</td>"
                End If

                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2c").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricType2c").ToString() & "</td>"
                If vOrderType = "Link Ind 2 Blinds" Or vOrderType = "Link System Independent 2 Blinds" Then
                    result += tdBorder & "Right" & "</td>"
                End If
                If vOrderType = "Link Ind 3 Blinds" Or vOrderType = "Link System Independent 3 Blinds" Then
                    result += tdBorder & "" & "</td>"
                End If
                result += tdBorder & "" & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameB").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2c").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Chain2c").ToString() & "</td>"
                result += tdBorder & vStopperB & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RolChainLength2c").ToString() & "</td>"
                result += "</tr>"

                If vOrderType = "Link Ind 3 Blinds" Or vOrderType = "Link System Independent 3 Blinds" Then
                    result += "<tr>"
                    result += tdBorder_colspan & "THIRD BLIND (END)" & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("Width3c").ToString() & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricType3c").ToString() & "</td>"
                    result += tdBorder & "" & "</td>"
                    result += tdBorder & "" & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameC").ToString() & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp3c").ToString() & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("Chain3c").ToString() & "</td>"
                    result += tdBorder & vStopperC & "</td>"
                    result += tdBorder & thisData.Tables(0).Rows(i).Item("RolChainLength2c").ToString() & "</td>"
                    result += "</tr>"
                End If

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_LinkInd4Roller(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Bot.Name AS BotName, BotB.Name AS BotNameB, BotC.Name AS BotNameC, BotD.Name AS BotNameD, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id LEFT JOIN Bottoms AS Bot ON Order_Detail.IDBottomRail = Bot.Id LEFT JOIN Bottoms AS BotB ON Order_Detail.IDBottomRail2b = BotB.Id LEFT JOIN Bottoms AS BotC ON Order_Detail.IDBottomRail2c = BotC.Id LEFT JOIN Bottoms AS BotD ON Order_Detail.IDBottomRail3c = BotD.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.OrderType='Link Ind 4 Blinds' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:40px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_rowspan As String = " <td rowspan='2' style='text-align:center;word-wrap: break-word;height:36px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspan As String = "<td colspan='5' style='text-align:center;height:34px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"
            Dim tdBorder_colspanNotes As String = "<td colspan='15' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>ROLLER (LINK INDEPENDENT 4 BLINDS)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Roll" & vEndTh
            result += thBorder & "Bottom Rail" & vEndTh
            result += thBorder & "Flat Option" & vEndTh
            result += thBorder & "Chain Colour" & vEndTh
            result += thBorder & "Chain Stopper" & vEndTh
            result += thBorder & "Chain Length" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vStopper As String = ""
                Dim vStopperB As String = ""

                If thisData.Tables(0).Rows(i).Item("Additional").ToString() = "With Stopper" Then
                    vStopper = "Yes"
                End If
                If thisData.Tables(0).Rows(i).Item("Additional").ToString() = "No Stopper" Then
                    vStopper = "No"
                End If

                If thisData.Tables(0).Rows(i).Item("Additional1").ToString() = "With Stopper" Then
                    vStopperB = "Yes"
                End If
                If thisData.Tables(0).Rows(i).Item("Additional1").ToString() = "No Stopper" Then
                    vStopperB = "No"
                End If

                Dim vFinalProduct As String = thisData.Tables(0).Rows(i).Item("ProductName").ToString()
                If vFinalProduct = "" Then
                    Dim gearsb As String = thisData.Tables(0).Rows(i).Item("LayoutCode").ToString()
                    Dim acmeda As String = thisData.Tables(0).Rows(i).Item("AwningType").ToString()

                    vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"

                    If Not String.IsNullOrEmpty(gearsb) And Not gearsb = "No" Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & gearsb & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                    If Not String.IsNullOrEmpty(acmeda) And Not acmeda = "No" Then
                        vFinalProduct = thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " " & gearsb & " (" & thisData.Tables(0).Rows(i).Item("Colour").ToString() & ")"
                    End If
                End If
                Dim itemId As String = thisData.Tables(0).Rows(i).Item("OrddID").ToString()
                Dim printing As Integer = archiveClass.GetItemData_Integer("SELECT COUNT(*) FROM Printing WHERE ItemId = '" + itemId + "'")
                If printing > 0 Then
                    vFinalProduct += "<br />"
                    vFinalProduct += "<b><u>PRINTING FABRIC</u></b>"
                End If

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & vFinalProduct & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & "</td>"
                result += tdBorder & "Left" & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotName").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainType").ToString() & "</td>"
                result += tdBorder & vStopper & "</td>"
                result += tdBorder & "" & "</td>"
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "SECOND BLIND (MIDDLE)" & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2b").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & "</td>"
                result += tdBorder & "" & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameB").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2b").ToString() & "</td>"
                result += tdBorder & "" & "</td>"
                result += tdBorder & "" & "</td>"
                result += tdBorder & "" & "</td>"
                result += "</tr>"


                result += "<tr>"
                result += tdBorder_colspan & "THIRD BLIND (MIDDLE)" & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2c").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & "</td>"
                result += tdBorder & "" & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameC").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2c").ToString() & "</td>"
                result += tdBorder & "" & "</td>"
                result += tdBorder & "" & "</td>"
                result += tdBorder & "" & "</td>"
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "FOURTH BLIND" & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width3c").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & "</td>"
                result += tdBorder & "Right" & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameD").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp3c").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Chain2c").ToString() & "</td>"
                result += tdBorder & vStopperB & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RolChainLength2c").ToString() & "</td>"
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_DBLinkIndRoller_4Blinds(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Bot.Name AS BotName, BotB.Name AS BotNameB, BotC.Name AS BotNameC, BotD.Name AS BotNameD, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id LEFT JOIN Bottoms AS Bot ON Order_Detail.IDBottomRail = Bot.Id LEFT JOIN Bottoms AS BotB ON Order_Detail.IDBottomRail2c = BotB.Id LEFT JOIN Bottoms AS BotC ON Order_Detail.IDBottomRailDB3c = BotC.Id LEFT JOIN Bottoms AS BotD ON Order_Detail.IDBottomRailDB4c = BotD.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Roller' AND Order_Detail.OrderType='Double and Link System Independent' AND NumOfBlind='4' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            Dim tdBorder_colspan As String = "<td colspan='5' style='text-align:center;height:25px;font-size: 7px;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_colspanNotes As String = "<td colspan='16' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 11px;'>ROLLER - DB LINK IND (4 BLINDS)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Roll" & vEndTh
            result += thBorder & "Bottom Rail" & vEndTh
            result += thBorder & "Flat Option" & vEndTh
            result += thBorder & "Chain / Remote" & vEndTh
            result += thBorder & "Chain Stopper" & vEndTh
            result += thBorder & "Chain Length" & vEndTh
            result += thBorder & "Solar Panel" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ProductName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & "Left" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainType").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollerChainLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "SECOND BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricType2c").ToString() & vEndTd
                result += tdBorder & "Right" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameB").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Chain2c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional1").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RolChainLength2c").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "THIRD BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypeDB3c").ToString() & vEndTd
                result += tdBorder & "Left" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameC").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOpDB3c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainDB3c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional2").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthDB3c").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "FOURTH BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypeDB4c").ToString() & vEndTd
                result += tdBorder & "Right" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameD").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp4c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainDB4c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional3").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthDB4c").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_DBLinkDepRoller_4Blinds(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Bot.Name AS BotName, BotB.Name AS BotNameB, BotC.Name AS BotNameC, BotD.Name AS BotNameD, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id LEFT JOIN Bottoms AS Bot ON Order_Detail.IDBottomRail = Bot.Id LEFT JOIN Bottoms AS BotB ON Order_Detail.IDBottomRail2b = BotB.Id LEFT JOIN Bottoms AS BotC ON Order_Detail.IDBottomRailDB3b = BotC.Id LEFT JOIN Bottoms AS BotD ON Order_Detail.IDBottomRailDB4b = BotD.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Roller' AND Order_Detail.OrderType='Double and Link System Dependent' AND NumOfBlind='4' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")
        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:6px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            Dim tdBorder_colspan As String = "<td colspan='5' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_colspanNotes As String = "<td colspan='16' style='text-align:center;height:20px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 11px;'>ROLLER - DB LINK DEP (4 BLINDS)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Roll" & vEndTh
            result += thBorder & "Bottom Rail" & vEndTh
            result += thBorder & "Flat Option" & vEndTh
            result += thBorder & "Chain / Remote" & vEndTh
            result += thBorder & "Chain Stopper" & vEndTh
            result += thBorder & "Chain Length" & vEndTh
            result += thBorder & "Solar Panel" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vSlimline As String = thisData.Tables(0).Rows(i).Item("Extbracket").ToString()
                Dim vProductName As String = thisData.Tables(0).Rows(i).Item("ProductName").ToString() & " - Slimline (" & vSlimline & ")"

                Dim vMechanism As String = ""
                If InStr(thisData.Tables(0).Rows(i).Item("TrackColour").ToString(), "Acmeda") > 0 Then
                    vMechanism = " - " & thisData.Tables(0).Rows(i).Item("TrackColour").ToString()
                End If

                If InStr(thisData.Tables(0).Rows(i).Item("LayoutCode").ToString(), "Gear Reduction") > 0 Then
                    vMechanism = " - " & thisData.Tables(0).Rows(i).Item("LayoutCode").ToString()
                End If

                If thisData.Tables(0).Rows(i).Item("ProductName").ToString() = "" Then
                    vProductName = "STD " & thisData.Tables(0).Rows(i).Item("Colour").ToString() & " " & thisData.Tables(0).Rows(i).Item("MotorType").ToString() & " - Slimline (" & vSlimline & ")" & vMechanism
                End If

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & vProductName & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainType").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollerChainLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "SECOND BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricType2b").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameB").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2b").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "THIRD BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypeDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameC").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOpDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional1").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthDB3b").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "FOURTH BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypeDB4b").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameD").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp4b").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_DBLinkDepRoller_6Blinds(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Bot.Name AS BotName, BotB.Name AS BotNameB, BotC.Name AS BotNameC, BotD.Name AS BotNameD, BotE.Name AS BotNameE, BotF.Name AS BotNameF, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id LEFT JOIN Bottoms AS Bot ON Order_Detail.IDBottomRail = Bot.Id LEFT JOIN Bottoms AS BotB ON Order_Detail.IDBottomRail2b = BotB.Id LEFT JOIN Bottoms AS BotC ON Order_Detail.IDBottomRail3b = BotC.Id LEFT JOIN Bottoms AS BotD ON Order_Detail.IDBottomRailDB4b = BotD.Id LEFT JOIN Bottoms AS BotE ON Order_Detail.IDBottomRail5 = BotE.Id LEFT JOIN Bottoms AS BotF ON Order_Detail.IDBottomRail6 = BotF.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Roller' AND Order_Detail.OrderType='Double and Link System Dependent' AND NumOfBlind='6' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:6px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            Dim tdBorder_colspan As String = "<td colspan='5' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_colspanNotes As String = "<td colspan='16' style='text-align:center;height:20px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 11px;'>ROLLER - DB LINK DEP (6 BLINDS)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Roll" & vEndTh
            result += thBorder & "Bottom Rail" & vEndTh
            result += thBorder & "Flat Option" & vEndTh
            result += thBorder & "Chain / Remote" & vEndTh
            result += thBorder & "Chain Stopper" & vEndTh
            result += thBorder & "Chain Length" & vEndTh
            result += thBorder & "Solar Panel" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vSlimline As String = thisData.Tables(0).Rows(i).Item("Extbracket").ToString()
                Dim vProductName As String = thisData.Tables(0).Rows(i).Item("ProductName").ToString() & " - Slimline (" & vSlimline & ")"

                Dim vMechanism As String = ""
                If InStr(thisData.Tables(0).Rows(i).Item("TrackColour").ToString(), "Acmeda") > 0 Then
                    vMechanism = " - " & thisData.Tables(0).Rows(i).Item("TrackColour").ToString()
                End If

                If InStr(thisData.Tables(0).Rows(i).Item("LayoutCode").ToString(), "Gear Reduction") > 0 Then
                    vMechanism = " - " & thisData.Tables(0).Rows(i).Item("LayoutCode").ToString()
                End If

                If thisData.Tables(0).Rows(i).Item("ProductName").ToString() = "" Then
                    vProductName = "STD " & thisData.Tables(0).Rows(i).Item("Colour").ToString() & " " & thisData.Tables(0).Rows(i).Item("MotorType").ToString() & " - Slimline (" & vSlimline & ")" & vMechanism
                End If

                Dim vType As String = thisData.Tables(0).Rows(i).Item("AwningType").ToString()
                Dim vControl As String = ""
                Dim vControlDB As String = ""
                If vType = "Type 3" Then
                    vControl = "Right" : vControlDB = "Right"
                End If

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & vProductName & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & vControl & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainType").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollerChainLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "SECOND BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameB").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2b").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "THIRD BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("width3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameC").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp3b").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "FOURTH BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypedb4").ToString() & vEndTd
                result += tdBorder & vControlDB & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameD").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomdb4").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Chaindb4").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("chainst4").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthdb4").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "FIFTH BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypedb4").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameE").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Flatbottomop5").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "SIXTH BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("width3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypedb4").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameF").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomop6").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_DBLinkIndRoller_6Blinds(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT Products.Name AS ProductName, Bot.Name AS BotName, BotB.Name AS BotNameB, BotC.Name AS BotNameC, BotD.Name AS BotNameD, BotE.Name AS BotNameE, BotF.Name AS BotNameF, Order_Detail.* FROM Order_Detail LEFT JOIN HardwareKits ON Order_Detail.IDHK = HardwareKits.KitId LEFT JOIN Products ON HardwareKits.ProductId = Products.Id LEFT JOIN Bottoms AS Bot ON Order_Detail.IDBottomRail = Bot.Id LEFT JOIN Bottoms AS BotB ON Order_Detail.IDBottomRail2c = BotB.Id LEFT JOIN Bottoms AS BotC ON Order_Detail.IDBottomRail3c = BotC.Id LEFT JOIN Bottoms AS BotD ON Order_Detail.IDBottomRailDB4b = BotD.Id LEFT JOIN Bottoms AS BotE ON Order_Detail.IDBottomRail5 = BotE.Id LEFT JOIN Bottoms AS BotF ON Order_Detail.IDBottomRail6 = BotF.Id WHERE Order_Detail.FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Roller' AND Order_Detail.OrderType='Double and Link System Independent' AND NumOfBlind='6' AND Order_Detail.Active=1 ORDER BY Order_Detail.OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:7px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTh As String = "</th>"

            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:25px;font-size:7px;border: 1px solid black; border-collapse: collapse;'>"
            Dim vEndTd As String = "</td>"

            Dim tdBorder_colspan As String = "<td colspan='5' style='text-align:center;height:25px;font-size: 7px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 11px;'>ROLLER - DB LINK IND (6 BLINDS)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:10px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Fabric Name" & vEndTh
            result += thBorder & "Control" & vEndTh
            result += thBorder & "Roll" & vEndTh
            result += thBorder & "Bottom Rail" & vEndTh
            result += thBorder & "Flat Option" & vEndTh
            result += thBorder & "Chain / Remote" & vEndTh
            result += thBorder & "Chain Stopper" & vEndTh
            result += thBorder & "Chain Length" & vEndTh
            result += thBorder & "Solar Panel" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim vSlimline As String = thisData.Tables(0).Rows(i).Item("Extbracket").ToString()
                Dim vProductName As String = thisData.Tables(0).Rows(i).Item("ProductName").ToString() & " - Slimline (" & vSlimline & ")"

                Dim vMechanism As String = ""
                If InStr(thisData.Tables(0).Rows(i).Item("TrackColour").ToString(), "Acmeda") > 0 Then
                    vMechanism = " - " & thisData.Tables(0).Rows(i).Item("TrackColour").ToString()
                End If

                If InStr(thisData.Tables(0).Rows(i).Item("LayoutCode").ToString(), "Gear Reduction") > 0 Then
                    vMechanism = " - " & thisData.Tables(0).Rows(i).Item("LayoutCode").ToString()
                End If

                If thisData.Tables(0).Rows(i).Item("ProductName").ToString() = "" Then
                    vProductName = thisData.Tables(0).Rows(i).Item("Colour").ToString() & " " & thisData.Tables(0).Rows(i).Item("MotorType").ToString() & " - Slimline (" & vSlimline & ")" & vMechanism
                End If

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & vEndTd
                result += tdBorder & vProductName & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Fabric").ToString() & vEndTd
                result += tdBorder & "Left" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotName").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainType").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollerChainLength").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Supply").ToString() & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "SECOND BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricType2c").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameB").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp2c").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "THIRD BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("fabrictype3c").ToString() & vEndTd
                result += tdBorder & "Right" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Roll").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameC").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomOp3c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("chain3c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional1").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("chainlength3c").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "FOURTH BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width2c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypedb4").ToString() & vEndTd
                result += tdBorder & "Left" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameD").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomdb4").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Chaindb4").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("chainst4").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthdb4").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "FIFTH BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("width3c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypedb5").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameE").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomop5").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"

                result += "<tr>"
                result += tdBorder_colspan & "SIXTH BLIND" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("width3c").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypedb6").ToString() & vEndTd
                result += tdBorder & "Right" & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("RollDB3b").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BotNameF").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomop6").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Chaindb6").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("chainst6").ToString() & vEndTd
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ChainLengthdb6").ToString() & vEndTd
                result += tdBorder & "" & vEndTd
                result += "</tr>"
            Next
            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_ShutterPanel(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType='Shutter' AND OrderType='Panel Only' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:40px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>SKYLINE (PANEL ONLY)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Colour" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Midrail Height" & vEndTh
            result += thBorder & "Louvre Size" & vEndTh
            result += thBorder & "Split Blade" & vEndTh
            result += thBorder & "Special Instruction" & vEndTh
            result += thBorder & "Additional Parts" & vEndTh
            result += thBorder & "Additional Length" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Midrail_Height").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Louvre").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional3").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Instruction").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional2").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Winder_Strip_Length").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                result += "</tr>"
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_ShutterFrame(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Shutter' AND OrderType='Frame Only' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:40px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>SKYLINE (FRAME ONLY)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Colour" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Layout" & vEndTh
            result += thBorder & "Panel Qty" & vEndTh
            result += thBorder & "Split Blade" & vEndTh
            result += thBorder & "Frame Type" & vEndTh
            result += thBorder & "Top Frame" & vEndTh
            result += thBorder & "Bottom Frame" & vEndTh
            result += thBorder & "Left Frame" & vEndTh
            result += thBorder & "Right Frame" & vEndTh
            result += thBorder & "1st GAP" & vEndTh
            result += thBorder & "2nd GAP" & vEndTh
            result += thBorder & "3rd GAP" & vEndTh
            result += thBorder & "4th GAP" & vEndTh
            result += thBorder & "5th GAP" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("LayoutCode").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Panel").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional3").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Top").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Bottom").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Left").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Right").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_1st").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_2nd").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_3rd").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_4th").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_5th").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                result += "</tr>"
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_ShutterHinged(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Shutter' AND OrderType='Hinged' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:40px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>SKYLINE (HINGED)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Colour" & vEndTh
            result += thBorder & "Hinge Colour" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Midrail" & vEndTh
            result += thBorder & "Louvre" & vEndTh
            result += thBorder & "Layout" & vEndTh
            result += thBorder & "Panel Qty" & vEndTh
            result += thBorder & "Split Blade" & vEndTh
            result += thBorder & "Frame Type" & vEndTh
            result += thBorder & "Top Frame" & vEndTh
            result += thBorder & "Bottom Frame" & vEndTh
            result += thBorder & "Left Frame" & vEndTh
            result += thBorder & "Right Frame" & vEndTh
            result += thBorder & "1st GAP" & vEndTh
            result += thBorder & "2nd GAP" & vEndTh
            result += thBorder & "3rd GAP" & vEndTh
            result += thBorder & "4th GAP" & vEndTh
            result += thBorder & "5th GAP" & vEndTh
            result += thBorder & "Special Instruction" & vEndTh
            result += thBorder & "Add Parts" & vEndTh
            result += thBorder & "Add Length" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Hinged_Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Midrail_Height").ToString() & " - " & thisData.Tables(0).Rows(i).Item("width_min").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Louvre").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("LayoutCode").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Panel").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional3").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Top").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Bottom").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Left").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Right").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_1st").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_2nd").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_3rd").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_4th").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_5th").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Instruction").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional2").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Winder_Strip_Length").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                result += "</tr>"
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_ShutterFixed(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Shutter' AND (OrderType='Fixed - U Channel' OR OrderType='Fixed - Light Block') AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:36px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>SKYLINE (FIXED)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Colour" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Midrail" & vEndTh
            result += thBorder & "Louvre" & vEndTh
            result += thBorder & "Layout" & vEndTh
            result += thBorder & "Panel Qty" & vEndTh
            result += thBorder & "Split Blade" & vEndTh
            result += thBorder & "Frame Type" & vEndTh
            result += thBorder & "Top Frame" & vEndTh
            result += thBorder & "Bottom Frame" & vEndTh
            result += thBorder & "Left Frame" & vEndTh
            result += thBorder & "Right Frame" & vEndTh
            result += thBorder & "1st GAP" & vEndTh
            result += thBorder & "2nd GAP" & vEndTh
            result += thBorder & "3rd GAP" & vEndTh
            result += thBorder & "4th GAP" & vEndTh
            result += thBorder & "5th GAP" & vEndTh
            result += thBorder & "Special Instruction" & vEndTh
            result += thBorder & "Add Parts" & vEndTh
            result += thBorder & "Add Length" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Midrail_Height").ToString() & " - " & thisData.Tables(0).Rows(i).Item("width_min").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Louvre").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("LayoutCode").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Panel").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional3").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Top").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Bottom").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Left").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Right").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_1st").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_2nd").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_3rd").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_4th").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_5th").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Instruction").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional2").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Winder_Strip_Length").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                result += "</tr>"
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_ShutterHingedBiFold(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Shutter' AND OrderType='Hinged Bi-Fold' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:40px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>SKYLINE (HINGED BI-FOLD)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Colour" & vEndTh
            result += thBorder & "Hinge Colour" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Midrail" & vEndTh
            result += thBorder & "Louvre" & vEndTh
            result += thBorder & "Layout" & vEndTh
            result += thBorder & "Panel Qty" & vEndTh
            result += thBorder & "Split Blade" & vEndTh
            result += thBorder & "Frame Type" & vEndTh
            result += thBorder & "Top Frame" & vEndTh
            result += thBorder & "Bottom Frame" & vEndTh
            result += thBorder & "Left Frame" & vEndTh
            result += thBorder & "Right Frame" & vEndTh
            result += thBorder & "1st GAP" & vEndTh
            result += thBorder & "2nd GAP" & vEndTh
            result += thBorder & "3rd GAP" & vEndTh
            result += thBorder & "4th GAP" & vEndTh
            result += thBorder & "5th GAP" & vEndTh
            result += thBorder & "Special Instruction" & vEndTh
            result += thBorder & "Add Parts" & vEndTh
            result += thBorder & "Add Length" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Hinged_Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Midrail_Height").ToString() & " - " & thisData.Tables(0).Rows(i).Item("width_min").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Louvre").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("LayoutCode").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional3").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Panel").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Top").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Bottom").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Left").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Right").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_1st").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_2nd").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_3rd").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_4th").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_5th").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Instruction").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional2").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Winder_Strip_Length").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                result += "</tr>"
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_ShutterTrackBiFold(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Shutter' AND OrderType='Track Bi-Fold' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:40px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>SKYLINE (TRACK BI-FOLD)</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Mounting" & vEndTh
            result += thBorder & "Colour" & vEndTh
            result += thBorder & "Hinge Colour" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Midrail" & vEndTh
            result += thBorder & "Louvre" & vEndTh
            result += thBorder & "Layout" & vEndTh
            result += thBorder & "Panel Qty" & vEndTh
            result += thBorder & "Split Blade" & vEndTh
            result += thBorder & "Frame Type" & vEndTh
            result += thBorder & "Top Frame" & vEndTh
            result += thBorder & "Bottom Frame" & vEndTh
            result += thBorder & "Left Frame" & vEndTh
            result += thBorder & "Right Frame" & vEndTh
            result += thBorder & "1st GAP" & vEndTh
            result += thBorder & "2nd GAP" & vEndTh
            result += thBorder & "3rd GAP" & vEndTh
            result += thBorder & "4th GAP" & vEndTh
            result += thBorder & "5th GAP" & vEndTh
            result += thBorder & "Bottom Track" & vEndTh
            result += thBorder & "Special Instruction" & vEndTh
            result += thBorder & "Add Parts" & vEndTh
            result += thBorder & "Add Length" & vEndTh
            result += thBorder & "Notes" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Hinged_Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Midrail_Height").ToString() & " - " & thisData.Tables(0).Rows(i).Item("width_min").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Louvre").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("LayoutCode").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Panel").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional3").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Top").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Bottom").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Left").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Frame_Right").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_1st").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_2nd").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_3rd").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_4th").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Gap_5th").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BT_Type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Instruction").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Additional2").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Winder_Strip_Length").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                result += "</tr>"
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_Window(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND Order_Detail.BlindType='Window' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:40px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_colspanNotes As String = "<td colspan='18' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>ALUMINIUM WINDOW</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Opening" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Colour" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Brace / Joiner" & vEndTh
            result += thBorder & "Angle Type" & vEndTh
            result += thBorder & "Angle Length" & vEndTh
            result += thBorder & "Angle Qty" & vEndTh
            result += thBorder & "Screen Port Hole" & vEndTh
            result += thBorder & "Plunger Pin" & vEndTh
            result += thBorder & "Swivel Clip Colour" & vEndTh
            result += thBorder & "Swivel Qty" & vEndTh
            result += thBorder & "Spring Clip" & vEndTh
            result += thBorder & "Top Clip Plastic" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim showSwivel As String = ""
                If InStr(thisData.Tables(0).Rows(i).Item("OrderType").ToString(), "Flyscreen") > 0 Then
                    Dim swivelClip As String = thisData.Tables(0).Rows(i).Item("winder_strip_length").ToString()
                    Dim swivelClip2 As String = thisData.Tables(0).Rows(i).Item("FabricID8").ToString()
                    If swivelClip = "" Then : swivelClip = "-" : End If
                    If swivelClip2 = "" Then : swivelClip2 = "-" : End If

                    showSwivel = "1.6mm : " & swivelClip & " Qty"
                    showSwivel += "<br />"
                    showSwivel += "11mm : " & swivelClip2 & " Qty"
                End If

                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrderType").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("bugseal_alumunium").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("angle_first").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("angle_length_first").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("bolt_patio_lockable").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("winder_strip_colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("chainst7").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("chainst6").ToString() & "</td>"
                result += tdBorder & showSwivel & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("latch_base").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("jamb_adaptor_toplength").ToString() & "</td>"

                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_DoorHinged(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType='Door' AND door_design_type LIKE '%Hinged%' AND Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:40px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_colspanNotes As String = "<td colspan='23' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>ALUMINIUM DOOR</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Measurements" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Colour" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Lock/Handle Height" & vEndTh
            result += thBorder & "Lock Type" & vEndTh
            result += thBorder & "Pet Type" & vEndTh
            result += thBorder & "Pet Position" & vEndTh
            result += thBorder & "Layout Code" & vEndTh
            result += thBorder & "Door Closer" & vEndTh
            result += thBorder & "Midrail" & vEndTh
            result += thBorder & "Bugseal Type" & vEndTh
            result += thBorder & "Flush Bolt" & vEndTh
            result += thBorder & "Jamb Adaptor Type" & vEndTh
            result += thBorder & "Jamb Adaptor Position" & vEndTh
            result += thBorder & "Beading" & vEndTh
            result += thBorder & "Triple Lock" & vEndTh
            result += thBorder & "Angle Type" & vEndTh
            result += thBorder & "Angle Length" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " - " & thisData.Tables(0).Rows(i).Item("door_design_type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "," & thisData.Tables(0).Rows(i).Item("width_middle").ToString() & "," & thisData.Tables(0).Rows(i).Item("width_bottom").ToString() & "</td>"

                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "," & thisData.Tables(0).Rows(i).Item("FabricType2c").ToString() & "," & thisData.Tables(0).Rows(i).Item("Chain2c").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("handle_height").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ValanceType").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("pet_door_type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("pet_door_position").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("frame_type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("chainst4").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("midrail_position").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("PelmetLayout").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("door_closer_colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("jamb_adaptor").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("jamb_adaptor_colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("beading_stop_bead").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("triple_lock").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("angle_first").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("angle_length_first").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("winder_strip_colour").ToString() & "</td>"
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_DoorSliding(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType='Door' AND door_design_type LIKE '%Sliding%' AND Active=1 ORDER BY OrddID ASC")

        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = " <td style='text-align:center;word-wrap: break-word;height:40px;font-size: 8px;border: 1px solid black; border-collapse: collapse;'>"

            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size: 8px;color:white;background-color:#007ACC;border: 1px solid black; border-collapse: collapse;'>"

            Dim tdBorder_colspanNotes As String = "<td colspan='31' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            result += "<span style='font-size: 12px;'>ALUMINIUM DOOR</span>"
            result += "<table style='width:100%;border: 1px solid black; border-collapse: collapse;margin-bottom:15px;'>"
            result += "<tr>"
            result += thBorder & "ID" & vEndTh
            result += thBorder & "Qty" & vEndTh
            result += thBorder & "Location" & vEndTh
            result += thBorder & "Measurements" & vEndTh
            result += thBorder & "Product" & vEndTh
            result += thBorder & "Colour" & vEndTh
            result += thBorder & "Width" & vEndTh
            result += thBorder & "Drop" & vEndTh
            result += thBorder & "Lock/Handle Height" & vEndTh
            result += thBorder & "Lock Type" & vEndTh
            result += thBorder & "Pet Type" & vEndTh
            result += thBorder & "Pet Position" & vEndTh
            result += thBorder & "Layout Code" & vEndTh
            result += thBorder & "Door Closer" & vEndTh
            result += thBorder & "Midrail" & vEndTh
            result += thBorder & "Bugseal Type" & vEndTh
            result += thBorder & "Flush Bolt" & vEndTh
            result += thBorder & "Jamb Adaptor Type" & vEndTh
            result += thBorder & "Jamb Adaptor Position" & vEndTh
            result += thBorder & "Beading" & vEndTh
            result += thBorder & "Triple Lock" & vEndTh
            result += thBorder & "Angle Type" & vEndTh
            result += thBorder & "Angle Length" & vEndTh
            result += thBorder & "Interlock" & vEndTh
            result += thBorder & "Top" & vEndTh
            result += thBorder & "Top Length" & vEndTh
            result += thBorder & "Bottom" & vEndTh
            result += thBorder & "Bottom Length" & vEndTh
            result += thBorder & "Reveiver" & vEndTh
            result += thBorder & "Reveiver Length" & vEndTh
            result += thBorder & "Sliding Roller" & vEndTh
            result += "</tr>"

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += "<tr>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrddID").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Qty").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Room").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("OrderType").ToString() & " - " & thisData.Tables(0).Rows(i).Item("door_design_type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("Width").ToString() & "," & thisData.Tables(0).Rows(i).Item("width_middle").ToString() & "," & thisData.Tables(0).Rows(i).Item("width_bottom").ToString() & "</td>"

                result += tdBorder & thisData.Tables(0).Rows(i).Item("Drop").ToString() & "," & thisData.Tables(0).Rows(i).Item("FabricType2c").ToString() & "," & thisData.Tables(0).Rows(i).Item("Chain2c").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("handle_height").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("ValanceType").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("pet_door_type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("pet_door_position").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("frame_type").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("chainst4").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("midrail_position").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("PelmetLayout").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("door_closer_colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("jamb_adaptor").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("jamb_adaptor_colour").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("beading_stop_bead").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("triple_lock").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("angle_first").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("angle_length_first").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BottomColourdb7").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FlatBottomdb4").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("track_top_double").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("FabricTypedb4").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("track_j").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("BottomTypedb4").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("track_w").ToString() & "</td>"
                result += tdBorder & thisData.Tables(0).Rows(i).Item("beading_stop_bead_colour").ToString() & "</td>"
                result += "</tr>"

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & thisData.Tables(0).Rows(i).Item("Notes").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next

            result += "</table>"
        End If
        Return result
    End Function

    Protected Function Print_ShutterOcean(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" & HeaderId & "' AND Order_Detail.BlindType='Skyline Shutter Ocean' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If thisData.Tables(0).Rows.Count > 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:8px;color:white;background-color:#007ACC;border:1px solid black;border-collapse:collapse;'>"
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = "<td style='text-align:center;word-wrap:break-word;height:40px;font-size:7px;border:1px solid black;border-collapse:collapse;'>"
            Dim vEndTd As String = "</td>"

            result &= "<span style='font-size: 12px;'>SKYLINE SHUTTER OCEAN</span>"
            result &= "<div style='width:100%;'>"

            ' ====== KOLOM KIRI ======
            result &= "<div style='width:100%;display:inline-block;vertical-align:top;'>"
            result &= "<table style='width:100%;border:1px solid black;border-collapse:collapse;margin-bottom:15px;'>"
            result &= "<tr>"
            result &= thBorder & "ID" & vEndTh
            result &= thBorder & "Qty" & vEndTh
            result &= thBorder & "Location" & vEndTh
            result &= thBorder & "Mounting" & vEndTh
            result &= thBorder & "Semi Inside" & vEndTh
            result &= thBorder & "Type" & vEndTh
            result &= thBorder & "Colour" & vEndTh
            result &= thBorder & "Hinge Colour" & vEndTh
            result &= thBorder & "Width" & vEndTh
            result &= thBorder & "Drop" & vEndTh
            result &= thBorder & "Midrail" & vEndTh
            result &= thBorder & "Midrail Critical" & vEndTh
            result &= thBorder & "Louvre Size" & vEndTh
            result &= thBorder & "Louvre Position" & vEndTh
            result &= "</tr>"

            ' ====== KOLOM KANAN ======
            Dim rightTable As String = ""
            Dim tdBorder_colspanNotes As String = "<td colspan='31' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            rightTable &= "<div style='width:100%;display:inline-block;vertical-align:top;'>"
            rightTable &= "<table style='width:100%;border:1px solid black;border-collapse:collapse;margin-bottom:15px;'>"
            rightTable &= "<tr>"
            rightTable &= thBorder & "Layout Code" & vEndTh
            rightTable &= thBorder & "Custom Header Length" & vEndTh
            rightTable &= thBorder & "Frame Type" & vEndTh
            rightTable &= thBorder & "Left Frame" & vEndTh
            rightTable &= thBorder & "Right Frame" & vEndTh
            rightTable &= thBorder & "Top Frame" & vEndTh
            rightTable &= thBorder & "Bottom Frame" & vEndTh
            rightTable &= thBorder & "Bottom Track Type" & vEndTh
            rightTable &= thBorder & "Bottom Track Recess" & vEndTh
            rightTable &= thBorder & "Buildout" & vEndTh
            rightTable &= thBorder & "Buildout Position" & vEndTh
            rightTable &= thBorder & "Panel Qty" & vEndTh
            rightTable &= thBorder & "Same Size Panel" & vEndTh
            rightTable &= thBorder & "1st GAP" & vEndTh
            rightTable &= thBorder & "2nd GAP" & vEndTh
            rightTable &= thBorder & "3rd GAP" & vEndTh
            rightTable &= thBorder & "4th GAP" & vEndTh
            rightTable &= thBorder & "5th GAP" & vEndTh
            rightTable &= thBorder & "Horizontal TPost Height" & vEndTh
            rightTable &= thBorder & "HorizontalTPost" & vEndTh
            rightTable &= thBorder & "Co-Joined Panels" & vEndTh
            rightTable &= thBorder & "Reverse Hinged" & vEndTh
            rightTable &= thBorder & "Pelmet Flat" & vEndTh
            rightTable &= thBorder & "Extra Fascia" & vEndTh
            rightTable &= thBorder & "Hinges Loose" & vEndTh
            rightTable &= thBorder & "Tiltrod Type" & vEndTh
            rightTable &= thBorder & "Tiltrod Split" & vEndTh
            rightTable &= thBorder & "Split Height" & vEndTh
            rightTable &= thBorder & "Door Cut Out" & vEndTh
            rightTable &= thBorder & "Special Shape" & vEndTh
            rightTable &= thBorder & "Template Provided" & vEndTh
            rightTable &= "</tr>"

            ' Loop data sekali untuk mengisi kedua tabel
            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim itemId As String = thisData.Tables(0).Rows(i).Item("OrddID").ToString()
                Dim detailData As DataSet = archiveClass.GetListData_Ocean("SELECT OrderDetails.*, Blinds.Name AS BlindName, ProductColours.Name AS ShutterColour FROM OrderDetails INNER JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id LEFT JOIN ProductColours ON Products.ColourType = ProductColours.Id WHERE OrderDetails.Id = '" & itemId & "'")

                If detailData.Tables(0).Rows.Count = 0 Then Continue For

                Dim dr = detailData.Tables(0).Rows(0)
                Dim midrailHeight As String = "1st : " & dr("MidrailHeight1").ToString() & " | 2nd : " & dr("MidrailHeight2").ToString()
                Dim splitHeight As String = "1st : " & dr("SplitHeight1").ToString() & " | 2nd : " & dr("SplitHeight2").ToString()
                Dim layoutCode As String = dr("LayoutCode").ToString()
                If layoutCode = "Other" Then layoutCode = dr("LayoutCodeCustom").ToString()

                ' === KIRI ===
                result &= "<tr>"
                result &= tdBorder & itemId & vEndTd
                result &= tdBorder & dr("Qty").ToString() & vEndTd
                result &= tdBorder & dr("Room").ToString() & vEndTd
                result &= tdBorder & dr("Mounting").ToString() & vEndTd
                result &= tdBorder & dr("SemiInsideMount").ToString() & vEndTd
                result &= tdBorder & dr("BlindName").ToString() & vEndTd
                result &= tdBorder & dr("ShutterColour").ToString() & vEndTd
                result &= tdBorder & dr("HingeColour").ToString() & vEndTd
                result &= tdBorder & dr("Width").ToString() & vEndTd
                result &= tdBorder & dr("Drop").ToString() & vEndTd
                result &= tdBorder & midrailHeight & vEndTd
                result &= tdBorder & dr("MidrailCritical").ToString() & vEndTd
                result &= tdBorder & dr("LouvreSize").ToString() & vEndTd
                result &= tdBorder & dr("LouvrePosition").ToString() & vEndTd
                result &= "</tr>"

                ' === KANAN ===
                rightTable &= "<tr>"
                rightTable &= tdBorder & layoutCode & vEndTd
                rightTable &= tdBorder & dr("CustomHeaderLength").ToString() & vEndTd
                rightTable &= tdBorder & dr("FrameType").ToString() & vEndTd
                rightTable &= tdBorder & dr("FrameLeft").ToString() & vEndTd
                rightTable &= tdBorder & dr("FrameRight").ToString() & vEndTd
                rightTable &= tdBorder & dr("FrameTop").ToString() & vEndTd
                rightTable &= tdBorder & dr("FrameBottom").ToString() & vEndTd
                rightTable &= tdBorder & dr("BottomTrackType").ToString() & vEndTd
                rightTable &= tdBorder & dr("BottomTrackRecess").ToString() & vEndTd
                rightTable &= tdBorder & dr("Buildout").ToString() & vEndTd
                rightTable &= tdBorder & dr("BuildoutPosition").ToString() & vEndTd
                rightTable &= tdBorder & dr("PanelQty").ToString() & vEndTd
                rightTable &= tdBorder & dr("SameSizePanel").ToString() & vEndTd
                rightTable &= tdBorder & dr("Gap1").ToString() & vEndTd
                rightTable &= tdBorder & dr("Gap2").ToString() & vEndTd
                rightTable &= tdBorder & dr("Gap3").ToString() & vEndTd
                rightTable &= tdBorder & dr("Gap4").ToString() & vEndTd
                rightTable &= tdBorder & dr("Gap5").ToString() & vEndTd
                rightTable &= tdBorder & dr("HorizontalTPostHeight").ToString() & vEndTd
                rightTable &= tdBorder & dr("HorizontalTPost").ToString() & vEndTd
                rightTable &= tdBorder & dr("JoinedPanels").ToString() & vEndTd
                rightTable &= tdBorder & dr("ReverseHinged").ToString() & vEndTd
                rightTable &= tdBorder & dr("PelmetFlat").ToString() & vEndTd
                rightTable &= tdBorder & dr("ExtraFascia").ToString() & vEndTd
                rightTable &= tdBorder & dr("HingesLoose").ToString() & vEndTd
                rightTable &= tdBorder & dr("TiltrodType").ToString() & vEndTd
                rightTable &= tdBorder & dr("TiltrodSplit").ToString() & vEndTd
                rightTable &= tdBorder & splitHeight & vEndTd
                rightTable &= tdBorder & dr("DoorCutOut").ToString() & vEndTd
                rightTable &= tdBorder & dr("SpecialShape").ToString() & vEndTd
                rightTable &= tdBorder & dr("TemplateProvided").ToString() & vEndTd
                rightTable &= "</tr>"

                If Not dr("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & dr("TemplateProvided").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next

            ' Tutup tabel kiri
            result &= "</table></div>"

            ' Tutup tabel kanan
            rightTable &= "</table></div>"
            result &= rightTable

            ' Tutup container
            result &= "</div>"
        End If
        Return result
    End Function

    Protected Function Print_ShutterExpress(HeaderId As String) As String
        Dim result As String = ""
        Dim thisData As DataSet = archiveClass.GetListData("SELECT * FROM Order_Detail WHERE FKOrdID = '" & HeaderId & "' AND Order_Detail.BlindType='Skyline Shutter Express' AND Order_Detail.Active=1 ORDER BY OrddID ASC")

        If thisData.Tables(0).Rows.Count > 0 Then
            Dim thBorder As String = "<th style='text-align:center;height:22px;font-size:8px;color:white;background-color:#007ACC;border:1px solid black;border-collapse:collapse;'>"
            Dim vEndTh As String = "</th>"
            Dim tdBorder As String = "<td style='text-align:center;word-wrap:break-word;height:40px;font-size:7px;border:1px solid black;border-collapse:collapse;'>"
            Dim vEndTd As String = "</td>"

            result &= "<span style='font-size: 12px;'>SKYLINE SHUTTER EXPRESS</span>"
            result &= "<div style='width:100%;'>"

            ' ====== KOLOM KIRI ======
            result &= "<div style='width:100%;display:inline-block;vertical-align:top;'>"
            result &= "<table style='width:100%;border:1px solid black;border-collapse:collapse;margin-bottom:15px;'>"
            result &= "<tr>"
            result &= thBorder & "ID" & vEndTh
            result &= thBorder & "Qty" & vEndTh
            result &= thBorder & "Location" & vEndTh
            result &= thBorder & "Mounting" & vEndTh
            result &= thBorder & "Semi Inside" & vEndTh
            result &= thBorder & "Type" & vEndTh
            result &= thBorder & "Colour" & vEndTh
            result &= thBorder & "Hinge Colour" & vEndTh
            result &= thBorder & "Width" & vEndTh
            result &= thBorder & "Drop" & vEndTh
            result &= thBorder & "Midrail" & vEndTh
            result &= thBorder & "Midrail Critical" & vEndTh
            result &= thBorder & "Louvre Size" & vEndTh
            result &= thBorder & "Louvre Position" & vEndTh
            result &= "</tr>"

            ' ====== KOLOM KANAN ======
            Dim rightTable As String = ""
            Dim tdBorder_colspanNotes As String = "<td colspan='31' style='text-align:center;height:25px;font-size: 6px;border: 1px solid black; border-collapse: collapse;'>"

            rightTable &= "<div style='width:100%;display:inline-block;vertical-align:top;'>"
            rightTable &= "<table style='width:100%;border:1px solid black;border-collapse:collapse;margin-bottom:15px;'>"
            rightTable &= "<tr>"
            rightTable &= thBorder & "Layout Code" & vEndTh
            rightTable &= thBorder & "Custom Header Length" & vEndTh
            rightTable &= thBorder & "Frame Type" & vEndTh
            rightTable &= thBorder & "Left Frame" & vEndTh
            rightTable &= thBorder & "Right Frame" & vEndTh
            rightTable &= thBorder & "Top Frame" & vEndTh
            rightTable &= thBorder & "Bottom Frame" & vEndTh
            rightTable &= thBorder & "Bottom Track Type" & vEndTh
            rightTable &= thBorder & "Bottom Track Recess" & vEndTh
            rightTable &= thBorder & "Buildout" & vEndTh
            rightTable &= thBorder & "Buildout Position" & vEndTh
            rightTable &= thBorder & "Panel Qty" & vEndTh
            rightTable &= thBorder & "Same Size Panel" & vEndTh
            rightTable &= thBorder & "1st GAP" & vEndTh
            rightTable &= thBorder & "2nd GAP" & vEndTh
            rightTable &= thBorder & "3rd GAP" & vEndTh
            rightTable &= thBorder & "4th GAP" & vEndTh
            rightTable &= thBorder & "5th GAP" & vEndTh
            rightTable &= thBorder & "Horizontal TPost Height" & vEndTh
            rightTable &= thBorder & "HorizontalTPost" & vEndTh
            rightTable &= thBorder & "Co-Joined Panels" & vEndTh
            rightTable &= thBorder & "Reverse Hinged" & vEndTh
            rightTable &= thBorder & "Pelmet Flat" & vEndTh
            rightTable &= thBorder & "Extra Fascia" & vEndTh
            rightTable &= thBorder & "Hinges Loose" & vEndTh
            rightTable &= thBorder & "Tiltrod Type" & vEndTh
            rightTable &= thBorder & "Tiltrod Split" & vEndTh
            rightTable &= thBorder & "Split Height" & vEndTh
            rightTable &= thBorder & "Door Cut Out" & vEndTh
            rightTable &= thBorder & "Special Shape" & vEndTh
            rightTable &= thBorder & "Template Provided" & vEndTh
            rightTable &= "</tr>"

            ' Loop data sekali untuk mengisi kedua tabel
            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim itemId As String = thisData.Tables(0).Rows(i).Item("OrddID").ToString()
                Dim detailData As DataSet = archiveClass.GetListData_Ocean("SELECT OrderDetails.*, Blinds.Name AS BlindName, ProductColours.Name AS ShutterColour FROM OrderDetails INNER JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Blinds ON Products.BlindId = Blinds.Id LEFT JOIN ProductColours ON Products.ColourType = ProductColours.Id WHERE OrderDetails.Id = '" & itemId & "'")

                If detailData.Tables(0).Rows.Count = 0 Then Continue For

                Dim dr = detailData.Tables(0).Rows(0)
                Dim midrailHeight As String = "1st : " & dr("MidrailHeight1").ToString() & " | 2nd : " & dr("MidrailHeight2").ToString()
                Dim splitHeight As String = "1st : " & dr("SplitHeight1").ToString() & " | 2nd : " & dr("SplitHeight2").ToString()
                Dim layoutCode As String = dr("LayoutCode").ToString()
                If layoutCode = "Other" Then layoutCode = dr("LayoutCodeCustom").ToString()

                ' === KIRI ===
                result &= "<tr>"
                result &= tdBorder & itemId & vEndTd
                result &= tdBorder & dr("Qty").ToString() & vEndTd
                result &= tdBorder & dr("Room").ToString() & vEndTd
                result &= tdBorder & dr("Mounting").ToString() & vEndTd
                result &= tdBorder & dr("SemiInsideMount").ToString() & vEndTd
                result &= tdBorder & dr("BlindName").ToString() & vEndTd
                result &= tdBorder & dr("ShutterColour").ToString() & vEndTd
                result &= tdBorder & dr("HingeColour").ToString() & vEndTd
                result &= tdBorder & dr("Width").ToString() & vEndTd
                result &= tdBorder & dr("Drop").ToString() & vEndTd
                result &= tdBorder & midrailHeight & vEndTd
                result &= tdBorder & dr("MidrailCritical").ToString() & vEndTd
                result &= tdBorder & dr("LouvreSize").ToString() & vEndTd
                result &= tdBorder & dr("LouvrePosition").ToString() & vEndTd
                result &= "</tr>"

                ' === KANAN ===
                rightTable &= "<tr>"
                rightTable &= tdBorder & layoutCode & vEndTd
                rightTable &= tdBorder & dr("CustomHeaderLength").ToString() & vEndTd
                rightTable &= tdBorder & dr("FrameType").ToString() & vEndTd
                rightTable &= tdBorder & dr("FrameLeft").ToString() & vEndTd
                rightTable &= tdBorder & dr("FrameRight").ToString() & vEndTd
                rightTable &= tdBorder & dr("FrameTop").ToString() & vEndTd
                rightTable &= tdBorder & dr("FrameBottom").ToString() & vEndTd
                rightTable &= tdBorder & dr("BottomTrackType").ToString() & vEndTd
                rightTable &= tdBorder & dr("BottomTrackRecess").ToString() & vEndTd
                rightTable &= tdBorder & dr("Buildout").ToString() & vEndTd
                rightTable &= tdBorder & dr("BuildoutPosition").ToString() & vEndTd
                rightTable &= tdBorder & dr("PanelQty").ToString() & vEndTd
                rightTable &= tdBorder & dr("SameSizePanel").ToString() & vEndTd
                rightTable &= tdBorder & dr("Gap1").ToString() & vEndTd
                rightTable &= tdBorder & dr("Gap2").ToString() & vEndTd
                rightTable &= tdBorder & dr("Gap3").ToString() & vEndTd
                rightTable &= tdBorder & dr("Gap4").ToString() & vEndTd
                rightTable &= tdBorder & dr("Gap5").ToString() & vEndTd
                rightTable &= tdBorder & dr("HorizontalTPostHeight").ToString() & vEndTd
                rightTable &= tdBorder & dr("HorizontalTPost").ToString() & vEndTd
                rightTable &= tdBorder & dr("JoinedPanels").ToString() & vEndTd
                rightTable &= tdBorder & dr("ReverseHinged").ToString() & vEndTd
                rightTable &= tdBorder & dr("PelmetFlat").ToString() & vEndTd
                rightTable &= tdBorder & dr("ExtraFascia").ToString() & vEndTd
                rightTable &= tdBorder & dr("HingesLoose").ToString() & vEndTd
                rightTable &= tdBorder & dr("TiltrodType").ToString() & vEndTd
                rightTable &= tdBorder & dr("TiltrodSplit").ToString() & vEndTd
                rightTable &= tdBorder & splitHeight & vEndTd
                rightTable &= tdBorder & dr("DoorCutOut").ToString() & vEndTd
                rightTable &= tdBorder & dr("SpecialShape").ToString() & vEndTd
                rightTable &= tdBorder & dr("TemplateProvided").ToString() & vEndTd
                rightTable &= "</tr>"

                If Not dr("Notes").ToString() = "" Then
                    result += "<tr>"
                    result += tdBorder_colspanNotes & "<span style='color:red;margin-left:10px;'><b><u>SPECIAL INFORMATION</u></b></span> : " & dr("TemplateProvided").ToString() & "</td>"
                    result += "</tr>"
                End If
            Next

            ' result

            ' Tutup tabel kiri
            result &= "</table></div>"

            ' Tutup tabel kanan
            rightTable &= "</table></div>"
            result &= rightTable

            ' Tutup container
            result &= "</div>"
        End If
        Return result
    End Function

    Protected Function BindDescOrderItem(HeaderId As String, Company As String) As String
        Dim result As String = ""

        Dim titleAlum As String = "<b>Aluminium : </b>" & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID = '" + HeaderId + "' AND BlindType='Ven' AND Order_Detail.Active=1")
        Dim titleCS As String = "<b>Cellular Shades : </b>" & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Cellular Shades' AND Order_Detail.Active=1")
        Dim titleCurtain As String = "<b>Curtain</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType LIKE '%Curtain%' AND Order_Detail.Active=1")
        Dim titleLinea As String = "<b>Linea Valance</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Linea Valance' AND Order_Detail.Active=1")
        Dim titlePanel As String = "<b>Panel Glide</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Panel Glide' AND Order_Detail.Active=1")
        Dim titlePelmet As String = "<b>Pelmet</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND (BlindType='Pelmet' OR BlindType='Fabric') AND Order_Detail.Active=1")
        Dim titleProfie As String = "<b>Profile</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Profile' AND Order_Detail.Active=1")
        Dim titleRoller As String = "<b>Roller</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Roller' AND Order_Detail.Active=1")
        Dim titleRoman As String = "<br /><b>Roman</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Roman' AND Order_Detail.Active=1")
        Dim titlePrivacy As String = "<b>Privacy</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Smart Privacy' AND Order_Detail.Active=1")
        Dim titleVenetian As String = "<b>Venetian</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Venetian' AND Order_Detail.Active=1")
        Dim titleVertical As String = "<b>Vertical</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND (BlindType='Vertical' OR BlindType='Vertical Slat Only') AND Order_Detail.Active=1")
        Dim titleSaphora As String = "<b>Saphora Drape</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Saphora Drape' AND Order_Detail.Active=1")

        Dim titleShutter As String = "<br /><b>Skyline Shutter Express</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Skyline Shutter Express' AND Order_Detail.Active=1")
        Dim titleSkylineOcean As String = "<br /><b>Skyline Shutter Ocean</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Skyline Shutter Ocean' AND Order_Detail.Active=1")
        Dim titleDoor As String = "<b>Aluminium Door</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Door' AND Order_Detail.Active=1")
        Dim titleWindow As String = "<b>Aluminium Window</b> : " & archiveClass.GetItemData("SELECT COUNT(*) FROM Order_Detail WHERE FKOrdID='" + HeaderId + "' AND BlindType='Window' AND Order_Detail.Active=1")

        Dim vSeparted As String = " | "
        Dim vBreak As String = "<br />"

        If Company = "JPMD" Then
            result = titleAlum & vSeparted & titleCS & vSeparted & titleCurtain & vSeparted & titleProfie & vSeparted & titleLinea & vSeparted & titlePelmet & vSeparted & titlePanel & titleRoman & vSeparted & titlePrivacy & vSeparted & titleVenetian & vSeparted & titleVertical & vSeparted & titleSaphora & vSeparted & titleRoller & titleShutter & vSeparted & titleSkylineOcean & vSeparted & titleWindow & vSeparted & titleDoor
        End If

        If Company = "LOCAL" Then
            result = titleAlum & vSeparted & titleCS & vSeparted & titleProfie & vSeparted & titlePanel & titleRoman & vSeparted & titlePrivacy & vSeparted & titleVenetian & vSeparted & titleVertical & vSeparted & titleRoller & titleShutter
        End If

        Return result
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
        'divErrorB.Visible = visible : msgErrorB.InnerText = message
    End Sub

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
