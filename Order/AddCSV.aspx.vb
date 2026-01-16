Imports System.Data.SqlClient
Imports System.IO
Imports OfficeOpenXml

Partial Class Order_AddCSV
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing
    Dim url As String = String.Empty

    Dim orderClass As New OrderClass
    Dim mailingClass As New MailingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDataCustomer()
            BindComponent(ddlMethod.SelectedValue)
        End If
    End Sub

    Protected Sub ddlMethod_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindComponent(ddlMethod.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "CUSTOMER ACCOUNT IS REQUIRED !")
                Exit Sub
            End If
            If ddlMethod.SelectedValue = "" Then
                MessageError(True, "METHOD IS REQUIRED !")
                Exit Sub
            End If
            If Not fuFile.HasFiles Then
                MessageError(True, "NO FILE SELECTED. PLEASE SELECT A FILE TO UPLOAD !")
                Exit Sub
            End If
            If ddlMethod.SelectedValue = "API" Then
                If txtApiId.Text = "" Then
                    MessageError(True, "YOUR ORDER ID IS REQUIRED !")
                    Exit Sub
                End If
            End If
            If ddlMethod.SelectedValue = "XLSX" Then
                If fuFile.HasFiles Then
                    Dim fileExtension As String = Path.GetExtension(fuFile.FileName).ToLower()
                    If fileExtension = ".xls" Or fileExtension = ".xlsx" Then
                        Dim fileName As String = fuFile.FileName

                        Dim savePath As String = Server.MapPath(String.Format("~/file/cws/{0}", fileName))
                        fuFile.SaveAs(savePath)
                        ReadExcelData(savePath)
                    End If
                End If
            End If

        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnSubmit_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order", False)
    End Sub

    Protected Sub BindComponent(method As String)
        divApi.Visible = False : divXls.Visible = False
        If method = "API" Then divApi.Visible = True
        If method = "XLSX" Then divXls.Visible = True
    End Sub

    Protected Sub BindDataCustomer()
        ddlCustomer.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT * FROM Customers WHERE Active=1 ORDER BY Name ASC"
            If Session("RoleName") = "IT" Then
                thisQuery = "SELECT * FROM Customers WHERE Active=1 AND Id<>'1' ORDER BY Name ASC"
            End If
            ddlCustomer.DataSource = orderClass.GetListData(thisQuery)
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If

            ddlCustomer.SelectedValue = Session("CustomerId").ToString()
        Catch ex As Exception
            ddlCustomer.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
            dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataCustomer", ex.ToString()}
            mailingClass.WebError(dataMailing)
        End Try
    End Sub

    Protected Sub ReadExcelData(filePath As String)
        Using package As New ExcelPackage(New FileInfo(filePath))
            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)

            Dim companyAlias As String = orderClass.GetCompanyAliasByCustomer(ddlCustomer.SelectedValue)
            Dim companyDetailId As String = orderClass.GetCompanyDetailIdByCustomer(ddlCustomer.SelectedValue)

            Dim headerId As String = orderClass.GetNewOrderHeaderId

            Dim orderNumber As String = worksheet.Cells(2, 1).Text
            Dim orderName As String = worksheet.Cells(2, 2).Text
            Dim orderNote As String = worksheet.Cells(2, 5).Text

            If orderNumber = orderClass.IsOrderExist(ddlCustomer.SelectedValue, orderNumber.Trim()) Then
                MessageError(True, "ORDER NUMBER ALREADY EXISTS !")
                Exit Sub
            End If

            Dim success As Boolean = False
            Dim retry As Integer = 0
            Dim maxRetry As Integer = 10
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
                        Using myCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, DownloadBOE, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, 'Regular', 'Unsubmitted', @CreatedBy, GETDATE(), 0, 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", headerId)
                            myCmd.Parameters.AddWithValue("@OrderId", orderId)
                            myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                            myCmd.Parameters.AddWithValue("@OrderNumber", orderNumber)
                            myCmd.Parameters.AddWithValue("@OrderName", orderName)
                            myCmd.Parameters.AddWithValue("@OrderNote", orderNote)
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

            Dim dataLog As Object() = {"OrderHeaders", headerId, Session("LoginId").ToString(), "Order Created | CSV"}
            orderClass.Logs(dataLog)

            Using secDetail As New ExcelPackage(New FileInfo(filePath))
                Dim sheetDetail As ExcelWorksheet = secDetail.Workbook.Worksheets(0)
                Dim startRow As Integer = 3
                Dim lastRow As Integer = sheetDetail.Dimension.End.Row

                For row As Integer = startRow To lastRow
                    Dim designId As String = String.Empty
                    Dim designType As String = If(sheetDetail.Cells(row, 1).Text IsNot Nothing, sheetDetail.Cells(row, 1).Text, "")
                    Dim blindType As String = If(sheetDetail.Cells(row, 2).Text IsNot Nothing, sheetDetail.Cells(row, 2).Text, "")

                    If designType = "Venetian" Then
                        Dim blindLower As String = blindType.ToLower()

                        If blindLower = "aluminium" Then
                            designId = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='Aluminium Blind'")

                            Dim subType As String = "Single"
                            Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 3).Text), 0, CInt(sheetDetail.Cells(row, 3).Text))
                            Dim room As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                            Dim mounting As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                            Dim colour As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                            Dim controlPosition As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                            Dim tilterPosition As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                            Dim width As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 9).Text), 0, CInt(sheetDetail.Cells(row, 9).Text))
                            Dim drop As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 10).Text), 0, CInt(sheetDetail.Cells(row, 10).Text))
                            Dim cordLength As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                            Dim wandLengthText As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                            Dim supply As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                            Dim notes As String = (sheetDetail.Cells(row, 19).Text & "").Trim()

                            If String.IsNullOrEmpty(designId) Then
                                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                                Exit For
                            End If

                            Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='Aluminium 25mm x 0.21mm' AND CompanyArray.VALUE='" & companyDetailId & "'")

                            If String.IsNullOrEmpty(blindId) Then
                                MessageError(True, "PLEASE CHECK YOUR ORDER TYPE !")
                                Exit For
                            End If

                            If qty <> "1" Then
                                MessageError(True, "PLEASE CHECK YOUR QTY ORDER ! ITEM : " & row.ToString())
                                Exit For
                            End If

                            Dim validMounting As String() = {"Face Fit", "Reveal Fit", "Make Size Face Fit", "Make Size Reveal Fit"}
                            If Not validMounting.Contains(mounting) Then
                                MessageError(True, "PLEASE CHECK YOUR MOUNTING DATA !")
                                Exit For
                            End If

                            If String.IsNullOrEmpty(colour) Then
                                MessageError(True, "COLOUR IS REQUIRED !")
                                Exit For
                            End If

                            Dim tubeId As String = "0" : Dim controlId As String = "0"
                            Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colour & "'")

                            If String.IsNullOrEmpty(colourId) Then
                                MessageError(True, "COLOUR TIDAK TERDAFTAR. SILAHKAN HUBUNGI IT SUPPORT REZA@BIGBLINDS.CO.ID !")
                                Exit For
                            End If

                            Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.value='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "' AND Active=1")

                            If String.IsNullOrEmpty(productId) Then
                                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                                Exit For
                            End If

                            Dim validCP As String() = {"Left", "Right", "No Control"}
                            If Not validCP.Contains(controlPosition) Then
                                MessageError(True, "PLEASE CHECK YOUR CONTROL POSITION DATA !")
                                Exit For
                            End If

                            Dim validTP As String() = {"Left", "Right", "Center", "Centre"}
                            If Not validTP.Contains(tilterPosition) Then
                                MessageError(True, "PLEASE CHECK YOUR TILTER POSITION DATA !")
                                Exit For
                            End If

                            If width < 270 Then
                                MessageError(True, "PLEASE CHECK YOUR WIDTH ORDER & MINIMUM WIDTH IS 270MM !")
                                Exit For
                            End If

                            If width <= 300 Then
                                If controlPosition = "No Control" Then
                                    MessageError(True, "YOUR WIDTH ORDER UNDER 300MM. PLEASE CHANGE PULL CORD POSITION TO NO CONTROL !")
                                    Exit For
                                End If

                                If tilterPosition <> "Center" OrElse tilterPosition <> "Centre" Then
                                    MessageError(True, "YOUR WIDTH ORDER UNDER 300MM. PLEASE CHANGE TILTER POSITION TO CENTER !")
                                    Exit For
                                End If
                            End If

                            If drop < 200 OrElse drop > 3200 Then
                                MessageError(True, "DROP MUST BE BETWEEN 200MM - 3200MM !")
                                Exit For
                            End If

                            Dim controlLength As String = String.Empty
                            Dim controlLengthValue As Integer = 0

                            Dim wandLength As String = String.Empty
                            Dim wandLengthValue As Integer = 0

                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)
                            If controlLengthValue < 450 Then controlLengthValue = 450

                            If Not String.IsNullOrEmpty(cordLength) AndAlso Not cordLength.ToLower().Contains("standard") AndAlso Not cordLength.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                cordLength = cordLength.Replace("mm", "")
                                If Not Integer.TryParse(cordLength, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    MessageError(True, "PLEASE CHECK YOUR CORD LENGTH !")
                                    Exit For
                                End If
                            End If

                            wandLength = "Standard"
                            wandLengthValue = Math.Ceiling(drop * 2 / 3)
                            If wandLengthValue < 450 Then wandLengthValue = 450

                            If Not String.IsNullOrEmpty(wandLengthText) AndAlso Not wandLengthText.ToLower().Contains("standard") AndAlso Not wandLengthText.ToLower().Contains("std") Then
                                wandLength = "Custom"
                                wandLengthText = wandLengthText.Replace("mm", "")
                                If Not Integer.TryParse(wandLengthText, wandLengthValue) OrElse wandLengthValue < 0 Then
                                    MessageError(True, "PLEASE CHECK YOUR CORD LENGTH !")
                                    Exit For
                                End If
                            End If

                            Dim validSupply As String() = {"No", "Yes"}
                            If Not validSupply.Contains(supply) Then
                                MessageError(True, "PLEASE CHECK YOUR HOLD DOWN CLIP !")
                                Exit For
                            End If

                            If supply = "No" Then supply = String.Empty

                            If msgError.InnerText = "" Then
                                Dim groupName As String = "Aluminium 25mm x 0.21mm"

                                Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId)

                                Dim linearMetre As Decimal = width / 10000
                                Dim squareMetre As Decimal = width * drop / 1000000

                                Dim itemId As String = orderClass.GetNewOrderItemId()

                                Using thisConn As SqlConnection = New SqlConnection(myConn)
                                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails (Id, HeaderId, ProductId, PriceProductGroupId, SubType, Qty, Room, Mounting, ControlPosition, TilterPosition, Width, [Drop], Supply, ControlLength, ControlLengthValue, WandLength, WandLengthValue, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, @SubType, 1, @Room, @Mounting, @ControlPosition, @TilterPosition, @Width, @Drop, @Supply, @ControlLength, @ControlLengthValue, @WandLength, @WandLengthValue, @LinearMetre, @SquareMetre, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                                        myCmd.Parameters.AddWithValue("@Id", itemId)
                                        myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                        myCmd.Parameters.AddWithValue("@ProductId", productId)
                                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                        myCmd.Parameters.AddWithValue("@Room", room)
                                        myCmd.Parameters.AddWithValue("@Mounting", String.Format("Opening Size {0}", mounting))
                                        myCmd.Parameters.AddWithValue("@SubType", subType)
                                        myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                        myCmd.Parameters.AddWithValue("@TilterPosition", tilterPosition)
                                        myCmd.Parameters.AddWithValue("@Width", width)
                                        myCmd.Parameters.AddWithValue("@Drop", drop)

                                        myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)

                                        myCmd.Parameters.AddWithValue("@WandLength", wandLength)
                                        myCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)

                                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)

                                        myCmd.Parameters.AddWithValue("@Supply", supply)
                                        myCmd.Parameters.AddWithValue("@TotalItems", 1)
                                        myCmd.Parameters.AddWithValue("@Notes", notes)
                                        myCmd.Parameters.AddWithValue("@MarkUp", 0)

                                        thisConn.Open()
                                        myCmd.ExecuteNonQuery()
                                    End Using
                                End Using

                                orderClass.ResetPriceDetail(headerId, itemId)
                                orderClass.CalculatePrice(headerId, itemId)
                                orderClass.FinalCostItem(headerId, itemId)

                                dataLog = {"OrderDetails", itemId, Session("LoginId").ToString(), "Order Item Added"}
                                orderClass.Logs(dataLog)
                            End If
                        End If

                        If blindLower.Contains("basswood") OrElse blindLower.Contains("ultraslat") Then
                            designId = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='Venetian Blind'")

                            Dim blindName As String = Regex.Replace(blindType, "\bVenetian\b", "", RegexOptions.IgnoreCase).Trim()
                            blindName = Regex.Replace(blindName, "\s+", " ")

                            Dim subType As String = "Single"
                            Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 3).Text), 0, CInt(sheetDetail.Cells(row, 3).Text))
                            Dim room As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                            Dim mounting As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                            Dim colour As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                            Dim controlPosition As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                            Dim tilterPosition As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                            Dim width As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 9).Text), 0, CInt(sheetDetail.Cells(row, 9).Text))
                            Dim drop As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 10).Text), 0, CInt(sheetDetail.Cells(row, 10).Text))
                            Dim cordLength As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                            Dim supply As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                            Dim tassel As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                            Dim valanceType As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                            Dim valanceSizeText As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                            Dim returnPosition As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                            Dim returnLengthText As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                            Dim notes As String = (sheetDetail.Cells(row, 19).Text & "").Trim()

                            Dim valanceSize As String = String.Empty
                            Dim valanceSizeValue As Integer = 0

                            Dim returnLength As String = String.Empty
                            Dim returnLengthValue As Integer = 0

                            If String.IsNullOrEmpty(designId) Then
                                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                                Exit For
                            End If

                            Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindName & "' AND CompanyArray.VALUE='" & companyDetailId & "'")

                            If String.IsNullOrEmpty(blindId) Then
                                MessageError(True, "PLEASE CHECK YOUR ORDER TYPE !")
                                Exit For
                            End If

                            If qty <> "1" Then
                                MessageError(True, "QTY ORDER MUST BE 1 !")
                                Exit For
                            End If

                            Dim validMounting As String() = {"Face Fit", "Reveal Fit", "Opening Size Face Fit", "Opening Size Reveal Fit", "Make Size Face Fit", "Make Size Reveal Fit"}
                            If Not validMounting.Contains(mounting) Then
                                MessageError(True, "PLEASE CHECK YOUR MOUNTING DATA !")
                                Exit For
                            End If

                            If mounting = "Face Fit" OrElse mounting = "Reveal Fit" Then
                                mounting = String.Format("Opening Size {0}", mounting)
                            End If

                            If String.IsNullOrEmpty(colour) Then
                                MessageError(True, "COLOUR IS REQUIRED !")
                                Exit For
                            End If

                            Dim tubeId As String = "0" : Dim controlId As String = "0"
                            Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colour & "'")

                            If String.IsNullOrEmpty(colourId) Then
                                MessageError(True, "COLOUR TIDAK TERDAFTAR. SILAHKAN HUBUNGI IT SUPPORT REZA@BIGBLINDS.CO.ID !")
                                Exit For
                            End If

                            Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.value='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "' AND Active=1")

                            If String.IsNullOrEmpty(productId) Then
                                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                                Exit For
                            End If

                            Dim validCP As String() = {"Left", "Right", "No Control"}
                            If Not validCP.Contains(controlPosition) Then
                                MessageError(True, "PLEASE CHECK YOUR CONTROL POSITION DATA !")
                                Exit For
                            End If

                            Dim validTP As String() = {"Left", "Right", "Center"}
                            If Not validTP.Contains(tilterPosition) Then
                                MessageError(True, "PLEASE CHECK YOUR TILTER POSITION DATA !")
                                Exit For
                            End If

                            If width < 270 Then
                                MessageError(True, "PLEASE CHECK YOUR WIDTH ORDER & MINIMUM WIDTH IS 270MM !")
                                Exit For
                            End If

                            If width <= 300 Then
                                If controlPosition = "No Control" Then
                                    MessageError(True, "YOUR WIDTH ORDER UNDER 300MM. PLEASE CHANGE PULL CORD POSITION TO NO CONTROL !")
                                    Exit For
                                End If

                                If tilterPosition <> "Center" Then
                                    MessageError(True, "YOUR WIDTH ORDER UNDER 300MM. PLEASE CHANGE TILTER POSITION TO CENTER !")
                                    Exit For
                                End If
                            End If

                            If drop < 200 OrElse drop > 3200 Then
                                MessageError(True, "DROP MUST BE BETWEEN 200MM - 3200MM !")
                                Exit For
                            End If

                            Dim controlLength As String = String.Empty
                            Dim controlLengthValue As Integer = 0

                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)
                            If controlLengthValue < 550 Then controlLengthValue = 550

                            If Not String.IsNullOrEmpty(cordLength) AndAlso Not cordLength.ToLower().Contains("standard") AndAlso Not cordLength.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                cordLength = cordLength.Replace("mm", "")
                                If Not Integer.TryParse(cordLength, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    MessageError(True, "PLEASE CHECK YOUR CORD LENGTH !")
                                    Exit For
                                End If
                            End If

                            Dim validSupply As String() = {"No", "Yes"}
                            If Not validSupply.Contains(supply) Then
                                MessageError(True, "PLEASE CHECK YOUR HOLD DOWN CLIP !")
                                Exit For
                            End If

                            Dim validTassel As String() = {"Plastic", "Gold", "Antique Brass"}
                            If Not validTassel.Contains(tassel) Then
                                MessageError(True, "PLEASE CHECK YOUR METAL TASSEL OPTION !")
                                Exit For
                            End If

                            If blindName.Contains("Basswood") AndAlso valanceType <> "75mm Valance" Then
                                MessageError(True, "PLEASE CHECK YOUR VALANCE TYPE !")
                                Exit For
                            End If

                            If blindName.Contains("Ultraslat") AndAlso valanceType <> "76mm Valance" Then
                                MessageError(True, "PLEASE CHECK YOUR VALANCE TYPE !")
                                Exit For
                            End If

                            valanceSize = "Standard"
                            valanceSizeValue = width - 1
                            If mounting = "Make Size Reveal Fit" Then
                                valanceSizeValue = width + 9
                            End If

                            If mounting = "Make Size Face Fit" OrElse mounting = "Make Size Face Fit" Then
                                valanceSizeValue = width + 20
                            End If

                            If Not String.IsNullOrEmpty(valanceSizeText) AndAlso Not valanceSizeText.ToLower().Contains("standard") AndAlso Not valanceSizeText.ToLower().Contains("std") Then
                                valanceSize = "Custom"
                                valanceSizeText = valanceSizeText.Replace("mm", "")
                                If Not Integer.TryParse(valanceSizeText, valanceSizeValue) OrElse valanceSizeValue < 0 Then
                                    MessageError(True, "PLEASE CHECK YOUR VALANCE SIZE !")
                                    Exit For
                                End If
                            End If

                            Dim validRP As String() = {"None", "Left", "Right", "Both Sides"}
                            If Not validRP.Contains(returnPosition) Then
                                MessageError(True, "PLEASE CHECK YOUR VALANCE RETURN POSITION !")
                                Exit For
                            End If

                            If returnPosition = "None" AndAlso Not String.IsNullOrEmpty(returnLengthText) Then
                                MessageError(True, "VALANCE RETURN LENGTH IS NOT REQUIRED !")
                                Exit For
                            End If

                            If Not returnPosition = "None" Then
                                If mounting = "Opening Size Face Fit" OrElse mounting = "Make Size Face Fit" Then
                                    returnLength = "Standard"
                                    returnLengthValue = 70
                                    If blindType.Contains("Ultraslat") Then returnLengthValue = 77

                                    If Not String.IsNullOrEmpty(returnLengthText) AndAlso Not returnLengthText.ToLower().Contains("standard") AndAlso Not returnLengthText.ToLower().Contains("std") Then
                                        returnLength = "Custom"
                                        returnLengthText = returnLengthText.Replace("mm", "")

                                        If Not Integer.TryParse(returnLengthText, returnLengthValue) OrElse returnLengthValue < 0 Then
                                            MessageError(True, "PLEASE CHECK YOUR VALANCE RETURN LENGTH !")
                                            Exit For
                                        End If
                                    End If
                                End If

                                If mounting = "Opening Size Reveal Fit" OrElse mounting = "Make Size Reveal Fit" Then
                                    returnLength = "Custom"
                                    returnLengthText = returnLengthText.Replace("mm", "")

                                    If Not Integer.TryParse(returnLengthText, returnLengthValue) OrElse returnLengthValue < 0 Then
                                        MessageError(True, "VALANCE RETURN LENGTH IS REQUIRED !")
                                        Exit For
                                    End If
                                End If
                            End If

                            If msgError.InnerText = "" Then
                                If returnPosition = "None" Then returnPosition = String.Empty
                                Dim customName As String = blindName
                                If blindName = "Ultraslat 50mm" Then customName = "Econo 50mm"
                                If blindName = "Ultraslat 63mm" Then customName = "Econo 63mm"

                                Dim groupName As String = String.Format("Venetian Blind - {0}", customName)

                                Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId)

                                Dim linearMetre As Decimal = width / 10000
                                Dim squareMetre As Decimal = width * drop / 1000000

                                Dim itemId As String = orderClass.GetNewOrderItemId()

                                Using thisConn As SqlConnection = New SqlConnection(myConn)
                                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails (Id, HeaderId, ProductId, PriceProductGroupId, SubType, Qty, Room, Mounting, ControlPosition, TilterPosition, Width, [Drop], Supply, Tassel, ControlLength, ControlLengthValue, ValanceType, ValanceSize, ValanceSizeValue, ReturnPosition, ReturnLength, ReturnLengthValue, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, @SubType, 1, @Room, @Mounting, @ControlPosition, @TilterPosition, @Width, @Drop, @Supply, @Tassel, @ControlLength, @ControlLengthValue, @ValanceType, @ValanceSize, @ValanceSizeValue, @ReturnPosition, @ReturnLength, @ReturnLengthValue, @LinearMetre, @SquareMetre, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                                        myCmd.Parameters.AddWithValue("@Id", itemId)
                                        myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                        myCmd.Parameters.AddWithValue("@ProductId", productId)
                                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                        myCmd.Parameters.AddWithValue("@Room", room)
                                        myCmd.Parameters.AddWithValue("@Mounting", mounting)
                                        myCmd.Parameters.AddWithValue("@SubType", subType)
                                        myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                        myCmd.Parameters.AddWithValue("@TilterPosition", tilterPosition)
                                        myCmd.Parameters.AddWithValue("@Width", width)
                                        myCmd.Parameters.AddWithValue("@Drop", drop)

                                        myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)

                                        myCmd.Parameters.AddWithValue("@ValanceType", valanceType)
                                        myCmd.Parameters.AddWithValue("@ValanceSize", valanceSize)
                                        myCmd.Parameters.AddWithValue("@ValanceSizeValue", valanceSizeValue)

                                        myCmd.Parameters.AddWithValue("@ReturnPosition", returnPosition)
                                        myCmd.Parameters.AddWithValue("@ReturnLength", returnLength)
                                        myCmd.Parameters.AddWithValue("@ReturnLengthValue", returnLengthValue)

                                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)

                                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)

                                        myCmd.Parameters.AddWithValue("@Tassel", tassel)
                                        myCmd.Parameters.AddWithValue("@Supply", supply)
                                        myCmd.Parameters.AddWithValue("@TotalItems", 1)
                                        myCmd.Parameters.AddWithValue("@Notes", notes)
                                        myCmd.Parameters.AddWithValue("@MarkUp", 0)

                                        thisConn.Open()
                                        myCmd.ExecuteNonQuery()
                                    End Using
                                End Using

                                orderClass.ResetPriceDetail(headerId, itemId)
                                orderClass.CalculatePrice(headerId, itemId)
                                orderClass.FinalCostItem(headerId, itemId)

                                dataLog = {"OrderDetails", itemId, Session("LoginId").ToString(), "Order Item Added"}
                                orderClass.Logs(dataLog)
                            End If
                        End If
                    End If

                    If designType = "Cellular Shades" Then
                        Dim controlType As String = (sheetDetail.Cells(row, 3).Text & "").Trim()
                        Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 4).Text), 0, CInt(sheetDetail.Cells(row, 4).Text))
                        Dim room As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim width As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 7).Text), 0, CInt(sheetDetail.Cells(row, 7).Text))
                        Dim drop As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 8).Text), 0, CInt(sheetDetail.Cells(row, 8).Text))
                        Dim fabricType As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim fabricTypeB As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim fabricColourB As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim controlPosition As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim cordLength As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim supply As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 16).Text & "").Trim()

                        Dim widthB As Integer = 0
                        Dim dropB As Integer = 0
                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0

                        designId = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='Cellular Shades'")
                        If String.IsNullOrEmpty(designId) Then
                            MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                            Exit For
                        End If
                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindType & "' AND companyArray.value='" & companyDetailId & "' AND Active=1")
                        If String.IsNullOrEmpty(blindId) Then
                            MessageError(True, "YOUR ORDER TYPE NOT REGISTERED YET !")
                            Exit For
                        End If
                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & blindType & "'")
                        Dim controlId As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlType & "'")
                        If String.IsNullOrEmpty(controlId) Then
                            MessageError(True, "YOUR CONTROL TYPE IS NOT REGISTERED YET !")
                            Exit For
                        End If

                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.value='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='0' AND Active=1")
                        If String.IsNullOrEmpty(productId) Then
                            MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                            Exit For
                        End If

                        If qty <> 1 Then
                            MessageError(True, "QTY ORDER MUST BE 1 !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(room) Then
                            MessageError(True, "ROOM / LOCATION IS REQUIRED !")
                            Exit For
                        End If

                        Dim validMounting As String() = {"Face Fit", "Reveal Fit", "Opening Size Face Fit", "Opening Size Reveal Fit", "Make Size Face Fit", "Make Size Reveal Fit"}
                        If Not validMounting.Contains(mounting) Then
                            MessageError(True, "PLEASE CHECK YOUR MOUNTING DATA !")
                            Exit For
                        End If

                        If mounting = "Face Fit" OrElse mounting = "Reveal Fit" Then
                            mounting = String.Format("Opening Size {0}", mounting)
                        End If

                        If String.IsNullOrEmpty(fabricType) Then
                            MessageError(True, "FABRIC TYPE IS REQUIRED !")
                            Exit For
                        End If

                        Dim fabricId As String = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND tubeArray.VALUE='" & tubeId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricId) Then
                            MessageError(True, "PLEASE CHECK YOUR FABRIC TYPE !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricColour) Then
                            MessageError(True, "FABRIC COLOUR IS REQUIRED !")
                            Exit For
                        End If

                        Dim fabricColourId As String = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricColourId) Then
                            MessageError(True, "PLEASE CHECK YOUR FABRIC COLOUR !")
                            Exit For
                        End If

                        Dim fabricIdB As String = String.Empty
                        Dim fabricColourIdB As String = String.Empty

                        If blindType = "Day & Night" Then
                            If String.IsNullOrEmpty(fabricTypeB) Then
                                MessageError(True, "FABRIC TYPE IS REQUIRED !")
                                Exit For
                            End If

                            fabricIdB = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricTypeB & "' AND designArray.VALUE='" & designId & "' AND tubeArray.VALUE='" & tubeId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricIdB) Then
                                MessageError(True, "PLEASE CHECK YOUR SECOND FABRIC TYPE !")
                                Exit For
                            End If

                            fabricColourIdB = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricIdB & "' AND Colour='" & fabricColourB & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricColourIdB) Then
                                MessageError(True, "PLEASE CHECK YOUR SECOND FABRIC COLOUR !")
                                Exit For
                            End If

                            widthB = width
                            dropB = drop
                        End If

                        If controlType = "Corded" Then
                            If String.IsNullOrEmpty(controlPosition) Then
                                MessageError(True, "CORD POSITION IS REQUIRED !")
                                Exit Sub
                            End If

                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)

                            If Not String.IsNullOrEmpty(cordLength) AndAlso Not cordLength.ToLower().Contains("standard") AndAlso Not cordLength.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                cordLength = cordLength.Replace("mm", "")
                                If Not Integer.TryParse(cordLength, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    MessageError(True, "PLEASE CHECK YOUR CORD LENGTH !")
                                    Exit For
                                End If
                            End If
                        End If

                        Dim linearMetre As Decimal = width / 1000
                        Dim linearMetreB As Decimal = 0D

                        Dim squareMetre As Decimal = width * drop / 1000000
                        Dim squareMetreB As Decimal = 0

                        Dim totalItems As Integer = 1
                        Dim fabricGroup As String = orderClass.GetFabricGroup(fabricId)

                        Dim factory As String = orderClass.GetFabricFactory(fabricColourId)

                        Dim groupName As String = String.Format("{0} - {1} - {2} - {3}", blindType, controlType, fabricGroup, factory)
                        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId)
                        Dim priceProductGroupB As String = String.Empty

                        If blindType = "Day & Night" Then
                            linearMetreB = width / 1000
                            squareMetreB = widthB * dropB / 1000000
                            totalItems = 2

                            Dim factoryB As String = orderClass.GetFabricFactory(fabricColourIdB)

                            groupName = String.Format("{0} - {1} - {2}", blindType, controlType, factory)
                            Dim groupNameB As String = String.Format("{0} - {1} - {2}", blindType, controlType, factoryB)

                            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, designId)
                            priceProductGroupB = orderClass.GetPriceProductGroupId(groupNameB, designId)
                        End If

                        Dim itemId As String = orderClass.GetNewOrderItemId()

                        Using thisConn As SqlConnection = New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricIdB, FabricColourId, FabricColourIdB, PriceProductGroupId, PriceProductGroupIdB, Qty, Room, Mounting, Width, WidthB, [Drop], DropB, ControlPosition, ControlLength, ControlLengthValue, Supply, LinearMetre, LinearMetreB, SquareMetre, SquareMetreB, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricIdB, @FabricColourId, @FabricColourIdB, @PriceProductGroupId, @PriceProductGroupIdB, @Qty, @Room, @Mounting, @Width, @WidthB, @Drop, @DropB, @ControlPosition, @ControlLength, @ControlLengthValue, @Supply, @LinearMetre, @LinearMetreB, @SquareMetre, @SquareMetreB, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", itemId)
                                myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                myCmd.Parameters.AddWithValue("@ProductId", productId)
                                myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                                myCmd.Parameters.AddWithValue("@Qty", "1")
                                myCmd.Parameters.AddWithValue("@Room", room)
                                myCmd.Parameters.AddWithValue("@FabricId", fabricId)
                                myCmd.Parameters.AddWithValue("@FabricColourId", fabricColourId)
                                myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(fabricIdB), CType(DBNull.Value, Object), fabricIdB))
                                myCmd.Parameters.AddWithValue("@FabricColourIdB", If(String.IsNullOrEmpty(fabricColourIdB), CType(DBNull.Value, Object), fabricColourIdB))
                                myCmd.Parameters.AddWithValue("@Mounting", mounting)
                                myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                myCmd.Parameters.AddWithValue("@Width", width)
                                myCmd.Parameters.AddWithValue("@WidthB", widthB)
                                myCmd.Parameters.AddWithValue("@Drop", drop)
                                myCmd.Parameters.AddWithValue("@DropB", dropB)
                                myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                                myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                                myCmd.Parameters.AddWithValue("@LinearMetreB", linearMetreB)
                                myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                                myCmd.Parameters.AddWithValue("@SquareMetreB", squareMetreB)
                                myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                                myCmd.Parameters.AddWithValue("@Supply", supply)
                                myCmd.Parameters.AddWithValue("@Notes", notes)
                                myCmd.Parameters.AddWithValue("@MarkUp", 0)

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        orderClass.ResetPriceDetail(headerId, itemId)
                        orderClass.CalculatePrice(headerId, itemId)
                        orderClass.FinalCostItem(headerId, itemId)

                        dataLog = {"OrderDetails", itemId, Session("LoginId"), "Order Item Added"}
                        orderClass.Logs(dataLog)
                    End If

                    If designType = "Panel Glide" Then
                        Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 3).Text), 0, CInt(sheetDetail.Cells(row, 3).Text))
                        Dim room As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim sizeType As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim panelStyle As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                        Dim trackColour As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim wandColour As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim batten As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim battenb As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim fabricType As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 13).Text & "").Trim()

                        Dim widthText As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim width As Integer

                        Dim dropText As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim drop As Integer

                        Dim wandLengthText As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                        Dim layoutCode As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim trackType As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                        Dim panelQty As String = (sheetDetail.Cells(row, 19).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 20).Text & "").Trim()

                        designId = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='Panel Glide'")
                        If String.IsNullOrEmpty(designId) Then
                            MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID")
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindType & "' AND companyArray.value='" & companyDetailId & "' AND Active=1")

                        If String.IsNullOrEmpty(blindId) Then
                            MessageError(True, "YOUR ORDER TYPE NOT REGISTERED YET !")
                            Exit For
                        End If

                        If qty <> 1 Then
                            MessageError(True, "QTY ORDER MUST BE 1 !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(room) Then
                            MessageError(True, "ROOM / LOCATION IS REQUIRED !")
                            Exit For
                        End If

                        Dim validSizeType As String() = {"Opening Size", "Make Size"}
                        If Not validSizeType.Contains(sizeType) Then
                            MessageError(True, "PLEASE CHECK YOUR SIZE TYPE !")
                            Exit For
                        End If

                        Dim validMounting As String() = {"Face Fit", "Reveal Fit"}
                        If Not validMounting.Contains(mounting) Then
                            MessageError(True, "PLEASE CHECK YOUR MOUNTING DATA !")
                            Exit For
                        End If

                        Dim tubeName As String = panelStyle
                        If panelStyle = "Classic" Then tubeName = "Plain"

                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & tubeName & "'")
                        If String.IsNullOrEmpty(tubeId) Then
                            MessageError(True, "PLEASE CHECK YOUR PANEL STYLE !")
                            Exit For
                        End If

                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & trackColour & "'")
                        If String.IsNullOrEmpty(colourId) Then
                            MessageError(True, "PLEASE CHECK YOUR TRACK COLOUR !")
                            Exit For
                        End If

                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='" & tubeId & "' AND ColourType='" & colourId & "' AND Active=1")

                        If String.IsNullOrEmpty(wandColour) Then
                            MessageError(True, "WAND COLOUR IS REQUIRED !")
                            Exit For
                        End If

                        If tubeName = "Plantation" AndAlso String.IsNullOrEmpty(batten) Then
                            MessageError(True, "FRONT BATTEN COLOUR IS REQUIRED !")
                            Exit For
                        End If

                        If tubeName = "Plantation" OrElse tubeName = "Sewless" Then
                            If String.IsNullOrEmpty(battenb) Then
                                MessageError(True, "BACK BATTEN COLOUR IS REQUIRED !")
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(fabricType) Then
                            MessageError(True, "FABRIC TYPE IS REQUIRED !")
                            Exit For
                        End If

                        Dim fabricId As String = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND tubeArray.VALUE='" & tubeId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricType) Then
                            MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID ! [FABRIC TYPE]")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricColour) Then
                            MessageError(True, "FABRIC COLOUR IS REQUIRED !")
                            Exit For
                        End If

                        Dim fabricColourId As String = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricColourId) Then
                            MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID ! [FABRIC COLOUR]")
                            Exit For
                        End If

                        If Not Integer.TryParse(widthText, width) Then
                            MessageError(True, "PLEASE CHECK YOUR WIDTH ORDER !")
                            Exit For
                        End If

                        If Not Integer.TryParse(dropText, drop) Then
                            MessageError(True, "PLEASE CHECK YOUR DROP ORDER !")
                            Exit For
                        End If

                        Dim wandLength As String = "Standard"
                        Dim wandLengthValue As Integer = Math.Ceiling(drop * 2 / 3)
                        If wandLengthValue > 1000 Then wandLengthValue = 1000

                        If Not String.IsNullOrEmpty(wandLengthText) AndAlso Not wandLengthText.ToLower().Contains("standard") AndAlso Not wandLengthText.ToLower().Contains("std") Then
                            wandLength = "Custom"
                            wandLengthText = wandLengthText.Replace("mm", "")
                            If Not Integer.TryParse(wandLengthText, wandLengthValue) OrElse wandLengthValue < 0 OrElse wandLengthValue > 1000 Then
                                MessageError(True, "PLEASE CHECK YOUR CORD LENGTH & MAXIMUM IS 1000MM !")
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(layoutCode) Then
                            MessageError(True, "LAYOUT CODE IS REQUIRED !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(trackType) Then
                            MessageError(True, "NUMBER OF TRACKS IS REQUIRED !")
                            Exit For
                        End If

                        trackType = trackType.Replace("Track", "").Trim()

                        If String.IsNullOrEmpty(panelQty) Then
                            MessageError(True, "NUMBER OF PANELS IS REQUIRED !")
                            Exit For
                        End If

                        panelQty = panelQty.Replace("Panels", "").Trim()

                        Dim linearMetre As Decimal = width / 1000
                        Dim squareMetre As Decimal = width * drop / 1000000

                        If tubeName = "Plain" Then
                            batten = String.Empty : battenb = String.Empty
                        End If
                        If tubeName = "Sewless" Then batten = String.Empty

                        Dim groupFabric As String = orderClass.GetFabricGroup(fabricId)
                        Dim groupName As String = String.Format("Panel Glide - {0} - {1} - {2}", blindType, tubeName, groupFabric)

                        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId)

                        Dim itemId As String = orderClass.GetNewOrderItemId()

                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], WandColour, WandLength, WandLengthValue, LayoutCode, LayoutCodeCustom, TrackType, PanelQty, Batten, BattenB, LinearMetre, SquareMetre, TotalItems, Notes, Markup, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @WandColour, @WandLength, @WandLengthValue, @LayoutCode, @LayoutCodeCustom, @TrackType, @PanelQty, @Batten, @BattenB, @LinearMetre, @SquareMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", itemId)
                                myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                myCmd.Parameters.AddWithValue("@ProductId", productId)
                                myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                myCmd.Parameters.AddWithValue("@Qty", 1)
                                myCmd.Parameters.AddWithValue("@Room", room)
                                myCmd.Parameters.AddWithValue("@Mounting", String.Format("{0} {1}", sizeType, mounting))
                                myCmd.Parameters.AddWithValue("@Width", width)
                                myCmd.Parameters.AddWithValue("@Drop", drop)
                                myCmd.Parameters.AddWithValue("@WandColour", wandColour)
                                myCmd.Parameters.AddWithValue("@WandLength", wandLength)
                                myCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)
                                myCmd.Parameters.AddWithValue("@LayoutCode", layoutCode)
                                myCmd.Parameters.AddWithValue("@LayoutCodeCustom", String.Empty)
                                myCmd.Parameters.AddWithValue("@TrackType", trackType)
                                myCmd.Parameters.AddWithValue("@PanelQty", panelQty)
                                myCmd.Parameters.AddWithValue("@Batten", batten)
                                myCmd.Parameters.AddWithValue("@BattenB", battenb)
                                myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                                myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                                myCmd.Parameters.AddWithValue("@Notes", notes)
                                myCmd.Parameters.AddWithValue("@MarkUp", 0)

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        orderClass.ResetPriceDetail(headerId, itemId)
                        orderClass.CalculatePrice(headerId, itemId)
                        orderClass.FinalCostItem(headerId, itemId)

                        dataLog = {"OrderDetails", itemId, Session("LoginId"), "Order Item Added"}
                        orderClass.Logs(dataLog)
                    End If

                    If designType = "Vertical" Then
                        designId = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='Vertical'")

                        Dim tubeType As String = (sheetDetail.Cells(row, 3).Text & "").Trim()
                        Dim controlType As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim colourType As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 6).Text), 0, CInt(sheetDetail.Cells(row, 6).Text))
                        Dim qtyBlade As String = If(sheetDetail.Cells(row, 7).Text IsNot Nothing, sheetDetail.Cells(row, 7).Text, "")
                        Dim room As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim sizeType As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim width As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 11).Text), 0, CInt(sheetDetail.Cells(row, 11).Text))
                        Dim drop As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 12).Text), 0, CInt(sheetDetail.Cells(row, 12).Text))
                        Dim fabricInsert As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim fabricType As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim stackPosition As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                        Dim controlPosition As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim wandColour As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                        Dim controlLengthText As String = (sheetDetail.Cells(row, 19).Text & "").Trim()
                        Dim bottomJoining As String = (sheetDetail.Cells(row, 20).Text & "").Trim()
                        Dim bracketExtension As String = (sheetDetail.Cells(row, 21).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 22).Text & "").Trim()


                        Dim chainId As String = String.Empty
                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0
                        Dim wandLengthValue As Integer = 0

                        If String.IsNullOrEmpty(designId) Then
                            MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindType & "' AND CompanyArray.VALUE='" & companyDetailId & "'")

                        If String.IsNullOrEmpty(blindId) Then
                            MessageError(True, "PLEASE CHECK YOUR ORDER TYPE DATA !")
                            Exit For
                        End If

                        Dim tubeName As String = String.Empty
                        If tubeType = "Wide Blade (127mm)" Then tubeName = "127mm"
                        If tubeType = "Narrow Blade (89mm)" Then tubeName = "89mm"

                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & tubeName & "'")
                        If String.IsNullOrEmpty(tubeId) Then
                            MessageError(True, "PLEASE CHECK YOUR SLAT SIZE !")
                            Exit For
                        End If

                        Dim controlId As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlType & "'")
                        If String.IsNullOrEmpty(controlId) Then
                            MessageError(True, "PLEASE CHECK YOUR CONTROL TYPE !")
                            Exit For
                        End If

                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colourType & "'")
                        If String.IsNullOrEmpty(colourType) Then
                            MessageError(True, "PLEASE CHECK YOUR HEADRAIL COLOUR !")
                            Exit For
                        End If

                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.VALUE='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "' AND Active=1")
                        If String.IsNullOrEmpty(productId) Then
                            MessageError(True, "PLEASE CHECK YOUR PRODUCT DATA !")
                            Exit For
                        End If

                        If qty <> 1 Then
                            MessageError(True, "QTY ORDER MUST BE 1 !")
                            Exit For
                        End If

                        Dim validSizeType As String() = {"Opening Size", "Make Size"}
                        If Not validSizeType.Contains(sizeType) Then
                            MessageError(True, "PLEASE CHECK YOUR SIZE TYPE !")
                            Exit For
                        End If

                        Dim validMounting As String() = {"Face Fit", "Reveal Fit"}
                        If Not validMounting.Contains(mounting) Then
                            MessageError(True, "PLEASE CHECK YOUR MOUNTING DATA !")
                            Exit For
                        End If

                        If blindType = "Complete Set" OrElse blindType = "Slat Only" Then
                            If width < 300 OrElse width > 6000 Then
                                MessageError(True, "WIDTH MUST BE BETWEEN 300MM - 6000MM !")
                                Exit For
                            End If
                            If drop < 300 OrElse drop > 3050 Then
                                MessageError(True, "DROP MUST BE BETWEEN 300MM - 3050MM !")
                                Exit For
                            End If
                        End If

                        Dim validFabricInsert As String() = {"No", "Yes"}
                        If Not validFabricInsert.Contains(fabricInsert) Then
                            MessageError(True, "PLEASE CHECK YOUR FABRIC INSERT !")
                            Exit For
                        End If

                        Dim fabricId As String = String.Empty
                        Dim fabricColourId As String = String.Empty

                        If blindType = "Complete Set" Then
                            If String.IsNullOrEmpty(fabricType) Then
                                MessageError(True, "FABRIC TYPE IS REQUIRED !")
                                Exit For
                            End If

                            fabricId = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND tubeArray.VALUE='" & tubeId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricId) Then
                                MessageError(True, "PLEASE CHECK YOUR FABRIC TYPE !")
                                Exit For
                            End If

                            If String.IsNullOrEmpty(fabricColour) Then
                                MessageError(True, "FABRIC COLOUR IS REQUIRED !")
                                Exit For
                            End If

                            fabricColourId = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricColourId) Then
                                MessageError(True, "PLEASE CHECK YOUR FABRIC COLOUR !")
                                Exit For
                            End If
                        End If

                        Dim validStack As String() = {"Left", "Right", "Centre", "Split"}

                        If blindType = "Complete Set" OrElse blindType = "Track Only" Then
                            If Not validStack.Contains(stackPosition) Then
                                MessageError(True, "PLEASE CHECK YOUR STACK POSITION !")
                                Exit For
                            End If
                        End If

                        If controlType = "Chain" Then
                            If String.IsNullOrEmpty(controlPosition) Then
                                MessageError(True, "CONTROL POSITION IS REQUIRED !")
                                Exit For
                            End If
                        End If

                        If controlType = "Wand" AndAlso String.IsNullOrEmpty(wandColour) Then
                            MessageError(True, "WAND COLOUR IS REQUIRED !")
                            Exit For
                        End If

                        If blindType = "Complete Set" Then
                            controlLength = "Standard"
                            If controlType = "Wand" Then
                                wandLengthValue = Math.Ceiling(drop * 2 / 3)
                                If wandLengthValue > 1000 Then wandLengthValue = 1000
                            End If
                            If controlType = "Chain" Then
                                controlLengthValue = Math.Ceiling(drop * 2 / 3)
                                If controlLengthValue > 1000 Then controlLengthValue = 1000
                            End If

                            If Not String.IsNullOrEmpty(controlLengthText) AndAlso Not controlLengthText.ToLower().Contains("standard") AndAlso Not controlLengthText.ToLower().Contains("std") Then
                                wandLengthValue = "Custom"
                                controlLengthText = controlLengthText.Replace("mm", "")

                                If Not Integer.TryParse(controlLengthText, wandLengthValue) OrElse wandLengthValue < 0 Then
                                    MessageError(True, "PLEASE CHECK YOUR CORD LENGTH !")
                                    Exit For
                                End If
                            End If
                        End If

                        If String.IsNullOrEmpty(bottomJoining) Then
                            MessageError(True, "BOTTOM JOINING IS REQUIRED !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(bracketExtension) Then
                            MessageError(True, "BRACKET EXTENSION IS REQUIRED !")
                            Exit For
                        End If

                        If msgError.InnerText = "" Then
                            Dim totalItems As Integer = 1

                            Dim groupName As String = String.Format("Vertical - {0} - {1}", blindType, tubeName)
                            Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId)

                            Dim linearMetre As Decimal = 0D
                            Dim squareMetre As Decimal = 0D

                            If blindType = "Complete Set" Then
                                linearMetre = width / 1000
                                squareMetre = width * drop / 1000000
                            End If

                            If controlType = "Wand" Then
                                If stackPosition = "Left" Then controlPosition = "Right"
                                If stackPosition = "Right" Then controlPosition = "Left"
                                If stackPosition = "Centre" Then controlPosition = "Right and Left"
                                If stackPosition = "Split" Then controlPosition = "Middle"
                            End If

                            If bottomJoining = "Sewn" Then bottomJoining = "Sewn In"

                            Dim itemId As String = orderClass.GetNewOrderItemId()

                            Using thisConn As SqlConnection = New SqlConnection(myConn)
                                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, ChainId, PriceProductGroupId, Qty, QtyBlade, Room, Mounting, Width, [Drop], StackPosition, ControlPosition, ControlLength, ControlLengthValue, WandColour, WandLengthValue, FabricInsert, BottomJoining, BracketExtension, Sloping, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @ChainId, @PriceProductGroupId, @Qty, @QtyBlade, @Room, @Mounting, @Width, @Drop, @StackPosition, @ControlPosition, @ControlLength, @ControlLengthValue, @WandColour, @WandLengthValue, @FabricInsert, @BottomJoining, @BracketExtension, @Sloping, @LinearMetre, @SquareMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                                    myCmd.Parameters.AddWithValue("@Id", itemId)
                                    myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                    myCmd.Parameters.AddWithValue("@ProductId", productId)
                                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                    myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                    myCmd.Parameters.AddWithValue("@Qty", "1")
                                    myCmd.Parameters.AddWithValue("@QtyBlade", qtyBlade)
                                    myCmd.Parameters.AddWithValue("@Room", room)
                                    myCmd.Parameters.AddWithValue("@Mounting", sizeType & " " & mounting)
                                    myCmd.Parameters.AddWithValue("@Width", width)
                                    myCmd.Parameters.AddWithValue("@Drop", drop)
                                    myCmd.Parameters.AddWithValue("@FabricInsert", fabricInsert)
                                    myCmd.Parameters.AddWithValue("@StackPosition", stackPosition)
                                    myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                    myCmd.Parameters.AddWithValue("@ControlLength", controllength)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                    myCmd.Parameters.AddWithValue("@WandColour", wandColour)
                                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)
                                    myCmd.Parameters.AddWithValue("@BottomJoining", bottomJoining)
                                    myCmd.Parameters.AddWithValue("@BracketExtension", bracketextension)
                                    myCmd.Parameters.AddWithValue("@Sloping", String.Empty)
                                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                                    myCmd.Parameters.AddWithValue("@Notes", notes)
                                    myCmd.Parameters.AddWithValue("@MarkUp", "0")

                                    thisConn.Open()
                                    myCmd.ExecuteNonQuery()
                                End Using
                            End Using

                            orderClass.ResetPriceDetail(headerId, itemId)
                            orderClass.CalculatePrice(headerId, itemId)
                            orderClass.FinalCostItem(headerId, itemId)

                            dataLog = {"OrderDetails", itemId, Session("LoginId").ToString(), "Order Item Added"}
                            orderClass.Logs(dataLog)
                        End If


                    End If

                    If designType = "Roller" Then
                        Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 3).Text), 0, CInt(sheetDetail.Cells(row, 3).Text))
                        Dim room As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim sizeType As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim mechanism As String = (sheetDetail.Cells(row, 7).Text & "").Trim()

                        Dim width As Integer = 0
                        Dim widthText As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim widthValue As Integer
                        Dim widthData As Integer = If(Integer.TryParse(widthText, widthValue), widthValue, 0)

                        Dim widthB As Integer = 0
                        Dim widthTextB As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim widthValueB As Integer
                        Dim widthDataB As Integer = If(Integer.TryParse(widthTextB, widthValueB), widthValueB, 0)

                        Dim drop As Integer = 0
                        Dim dropB As Integer = 0
                        Dim dropC As Integer = 0
                        Dim dropD As Integer = 0
                        Dim dropE As Integer = 0
                        Dim dropF As Integer = 0

                        Dim dropText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim dropValue As Integer
                        Dim dropData As Integer = If(Integer.TryParse(dropText, dropValue), dropValue, 0)

                        Dim widthc As Integer = If(Integer.TryParse(sheetDetail.Cells(row, 10).Text.Trim(), Nothing), CInt(sheetDetail.Cells(row, 10).Text.Trim()), 0)
                        Dim widthd As Integer = 0
                        Dim widthe As Integer = 0
                        Dim widthf As Integer = 0

                        Dim bracketType As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim bracketColour As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim fabricType As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim fabricTypeDB As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                        Dim fabricColourDB As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim roll As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                        Dim rollDB As String = (sheetDetail.Cells(row, 19).Text & "").Trim()
                        Dim controlText As String = (sheetDetail.Cells(row, 20).Text & "").Trim()
                        Dim controlType As String = (sheetDetail.Cells(row, 21).Text & "").Trim()
                        Dim chainColour As String = (sheetDetail.Cells(row, 22).Text & "").Trim()
                        Dim chainLength As String = (sheetDetail.Cells(row, 23).Text & "").Trim()
                        Dim motorType As String = (sheetDetail.Cells(row, 23).Text & "").Trim()
                        Dim bottomType As String = (sheetDetail.Cells(row, 24).Text & "").Trim()
                        Dim bottomColour As String = (sheetDetail.Cells(row, 25).Text & "").Trim()
                        Dim bottomOption As String = (sheetDetail.Cells(row, 26).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 27).Text & "")

                        Dim fabricId As String = String.Empty : Dim fabricColourId As String = String.Empty
                        Dim fabricIdB As String = String.Empty : Dim fabricColourIdB As String = String.Empty
                        Dim fabricIdC As String = String.Empty : Dim fabricColourIdC As String = String.Empty
                        Dim fabricIdD As String = String.Empty : Dim fabricColourIdD As String = String.Empty
                        Dim fabricIdE As String = String.Empty : Dim fabricColourIdE As String = String.Empty
                        Dim fabricIdF As String = String.Empty : Dim fabricColourIdF As String = String.Empty

                        Dim blindName As String = blindType
                        If blindType = "Double: (2 Blinds)" Then blindName = "Dual Blinds"
                        If blindType = "Single: Linked (2 Blinds)" Then
                            If controlText = "SC" OrElse controlText = "CS" Then blindName = "Link 2 Blinds Dependent"
                            If controlText = "II" Then blindName = "Link 2 Blinds Independent"
                        End If

                        designId = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='Roller Blind'")

                        If String.IsNullOrEmpty(designId) Then
                            MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                            Exit For
                        End If

                        If qty <> 1 Then
                            MessageError(True, "QTY ORDER MUST BE 1 !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(room) Then
                            MessageError(True, "ROOM / LOCATION IS REQUIRED !")
                            Exit For
                        End If

                        Dim validSizeType As String() = {"Opening Size", "Make Size"}
                        If Not validSizeType.Contains(sizeType) Then
                            MessageError(True, "PLEASE CHECK YOUR SIZE TYPE !")
                            Exit For
                        End If

                        Dim validMounting As String() = {"Face Fit", "Reveal Fit"}
                        If Not validMounting.Contains(mounting) Then
                            MessageError(True, "PLEASE CHECK YOUR MOUNTING DATA !")
                            Exit For
                        End If

                        If mechanism <> "GR" Then
                            MessageError(True, "PLEASE CHECK YOUR MECHANISM DATA !")
                            Exit For
                        End If

                        If widthData = 0 Then
                            MessageError(True, "PLEASE CHECK YOUR WIDTH DATA !")
                            Exit For
                        End If

                        If blindType = "Single: Linked (2 Blinds)" Then
                            If widthDataB = 0 Then
                                MessageError(True, "PLEASE CHECK YOUR SECOND WIDTH DATA !")
                                Exit For
                            End If
                        End If

                        If dropData = 0 Then
                            MessageError(True, "PLEASE CHECK YOUR DROP DATA !")
                            Exit For
                        End If

                        Dim linearMetre As Decimal = widthData / 1000
                        Dim linearMetreB As Decimal = 0
                        Dim linearMetreC As Decimal = 0
                        Dim linearMetreD As Decimal = 0
                        Dim linearMetreE As Decimal = 0
                        Dim linearMetreF As Decimal = 0

                        Dim squareMetre As Decimal = widthData * dropData / 1000000
                        Dim squareMetreB As Decimal = 0
                        If blindType = "Single: Linked (2 Blinds)" Then
                            squareMetreB = widthDataB * dropData / 1000000
                        End If
                        Dim squareMetreC As Decimal = 0
                        Dim squareMetreD As Decimal = 0
                        Dim squareMetreE As Decimal = 0
                        Dim squareMetreF As Decimal = 0

                        Dim validBType As String() = {"Standard", "Extension", "Slim"}
                        If Not validBType.Contains(bracketType) Then
                            MessageError(True, "YOUR BRACKET TYPE DATA NOT REGISTERED !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(bracketColour) Then
                            MessageError(True, "BRACKET COLOUR IS REQUIRED !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricType) Then
                            MessageError(True, "FABRIC TYPE IS REQUIRED !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricColour) Then
                            MessageError(True, "FABRIC COLOUR IS REQUIRED !")
                            Exit For
                        End If

                        fabricId = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricId) Then
                            MessageError(True, "PLEASE CHECK YOUR FABRIC TYPE !")
                            Exit For
                        End If

                        fabricColourId = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricColourId) Then
                            MessageError(True, "PLEASE CHECK YOUR FABRIC COLOUR !")
                            Exit For
                        End If

                        If blindName = "Dual Blinds" Then
                            If String.IsNullOrEmpty(fabricTypeDB) Then
                                MessageError(True, "SECOND FABRIC TYPE IS REQUIRED !")
                                Exit For
                            End If

                            If String.IsNullOrEmpty(fabricColourDB) Then
                                MessageError(True, "SECOND FABRIC COLOUR IS REQUIRED !")
                                Exit For
                            End If

                            fabricIdB = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricTypeDB & "' AND designArray.VALUE='" & designId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricIdB) Then
                                MessageError(True, "PLEASE CHECK YOUR SECOND FABRIC TYPE !")
                                Exit For
                            End If

                            fabricColourIdB = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricIdB & "' AND Colour='" & fabricColourDB & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricColourIdB) Then
                                MessageError(True, "PLEASE CHECK YOUR SECOND FABRIC COLOUR !")
                                Exit For
                            End If
                        End If

                        Dim validRoll As String() = {"Front Roll", "Back Roll", "Standard", "Reverse"}
                        If Not validRoll.Contains(roll) Then
                            MessageError(True, "PLEASE CHECK YOUR ROLL DIRECTION DATA !")
                            Exit For
                        End If

                        If blindName = "Dual Blinds" Then
                            If Not validRoll.Contains(rollDB) Then
                                MessageError(True, "PLEASE CHECK YOUR SECOND ROLL DIRECTION DATA !")
                                Exit For
                            End If

                            If (roll = "Standard" OrElse roll = "Back Roll") AndAlso (rollDB = "Standard" OrElse rollDB = "Back Roll") Then
                                MessageError(True, "PLEASE CHECK YOUR ROLL DIRECTION DATA !")
                                Exit For
                            End If
                        End If

                        Dim validControl As String() = {"Left", "Right"}
                        If blindType = "Double: (2 Blinds)" Then
                            validControl = New String() {"L - L", "R - R"}
                        End If
                        If blindType = "Single: Linked (2 Blinds)" Then
                            validControl = New String() {"II", "CS", "SC"}
                        End If

                        If Not validControl.Contains(controlText) Then
                            MessageError(True, "PLEASE CHECK YOUR CONTROL POSITION DATA !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(controlType) Then
                            MessageError(True, "CONTROL TYPE IS REQUIRED !")
                            Exit For
                        End If

                        Dim chainName As String = String.Empty
                        Dim chainType As String = String.Empty
                        Dim chainStopper As String = String.Empty

                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0

                        If controlType = "Chain" Then
                            If String.IsNullOrEmpty(chainColour) Then
                                MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                                Exit For
                            End If

                            chainName = String.Format("Cont {0}", chainColour)
                            chainType = "Continuous"
                            chainStopper = "No Stopper"

                            If chainLength.ToLower.Contains("custom w joiner") OrElse chainLength.ToLower.Contains("custom w/ joiner") Then
                                chainName = String.Format("Non Cont {0}", chainColour)
                                chainType = "Non Continuous"
                                chainStopper = "With Stopper"

                                controlLength = "Custom"
                                controlLengthValue = 0
                            End If

                            If chainType = "Continuous" Then
                                controlLength = "Standard"

                                Dim stdControlLength As Integer = Math.Ceiling(dropData * 2 / 3)

                                controlLengthValue = stdControlLength
                                If stdControlLength > 500 Then controlLengthValue = 750
                                If stdControlLength > 750 Then controlLengthValue = 1000
                                If stdControlLength > 1000 Then controlLengthValue = 1200
                                If stdControlLength > 1200 Then controlLengthValue = 1500

                                If Not String.IsNullOrEmpty(chainLength) AndAlso Not chainLength.ToLower().Contains("standard") AndAlso Not chainLength.ToLower().Contains("std") Then
                                    controlLength = "Custom"
                                    chainLength = chainLength.Replace("mm", "")
                                    If Not Integer.TryParse(chainLength, controlLengthValue) OrElse controlLengthValue < 0 Then
                                        MessageError(True, "PLEASE CHECK YOUR CHAIN LENGTH !")
                                        Exit For
                                    End If
                                End If
                            End If
                        End If

                        If controlType = "Motorized" OrElse controlType = "Motorised" Then
                            controlType = motorType
                            chainName = "No Remote"
                        End If

                        Dim tubeType As String = String.Empty
                        If blindType = "Single Blind" OrElse blindType = "Double: (2 Blinds)" Then
                            tubeType = "Gear Reduction 49mm"
                            If controlType = "Chain" Then
                                If widthData <= 1810 Then tubeType = "Gear Reduction 38mm"
                                If widthData > 1810 OrElse widthDataB > 1810 Then tubeType = "Gear Reduction 45mm"
                                If squareMetre > 6 Then tubeType = "Gear Reduction 49mm"
                            End If
                        End If

                        If blindType = "Single: Linked (2 Blinds)" Then
                            tubeType = "Gear Reduction 49mm"
                            If controlType = "Chain" Then
                                If widthData <= 1810 AndAlso widthDataB <= 1810 Then tubeType = "Gear Reduction 38mm"
                                If widthData > 1810 Then tubeType = "Gear Reduction 45mm"
                                If squareMetre > 6 OrElse squareMetreB > 6 Then tubeType = "Gear Reduction 49mm"
                            End If
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindName & "' AND CompanyArray.VALUE='" & companyDetailId & "'")
                        If String.IsNullOrEmpty(blindId) Then
                            MessageError(True, "ORDER TYPE NOT REGISTERED !")
                            Exit For
                        End If

                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & tubeType & "'")
                        If String.IsNullOrEmpty(tubeId) Then
                            MessageError(True, "YOUR TUBE TYPE DATA IS NOT REGISTERED !")
                            Exit For
                        End If

                        Dim controlId As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlType & "'")
                        If String.IsNullOrEmpty(controlId) Then
                            MessageError(True, "YOUR CONTROL TYPE DATA IS NOT REGISTERED !")
                            Exit For
                        End If

                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & bracketColour & "'")
                        If String.IsNullOrEmpty(colourId) Then
                            MessageError(True, "YOUR BRACKET COLOUR IS NOT REGISTERED !")
                            Exit For
                        End If

                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.value='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "' AND Active=1")
                        If String.IsNullOrEmpty(colourId) Then
                            MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID [PRODUCTS ID]")
                            Exit For
                        End If

                        Dim chainId As String = orderClass.GetItemData("SELECT Id FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(ControlTypeId, ',') AS controlArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & chainName & "' AND designArray.VALUE='" & designId & "' AND controlArray.VALUE='" & controlId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")

                        If String.IsNullOrEmpty(chainId) Then
                            MessageError(True, "PLEASE CHECK YOUR CHAIN COLOUR / MOTOR REMOTE DATA !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(bottomType) Then
                            MessageError(True, "BOTTOM TYPE IS REQUIRED !")
                            Exit For
                        End If

                        If bottomType = "Silent" Then bottomType = "Flat Mohair"
                        If bottomType = "Fabric Wrap" Then bottomType = "Flat"

                        Dim bottomId As String = orderClass.GetItemData("SELECT Id FROM Bottoms CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name = '" & bottomType.Trim() & "' AND companyArray.VALUE='" & companyDetailId & "'")
                        If String.IsNullOrEmpty(bottomId) Then
                            MessageError(True, "BOTTOM TYPE NOT REGISTERED YET !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(bottomColour) Then
                            MessageError(True, "BOTTOM COLOUR IS REQUIRED !")
                            Exit For
                        End If

                        If bottomColour = "Cream" Then bottomColour = "Ivory"
                        If bottomColour = "Platinum" Then bottomColour = "Silver"

                        Dim bottomColourId As String = orderClass.GetItemData("SELECT Id FROM BottomColours WHERE BottomId='" & bottomId & "' AND Colour='" & bottomColour.Trim() & "'")
                        If String.IsNullOrWhiteSpace(bottomColourId) Then
                            MessageError(True, "BOTTOM COLOUR NOT REGISTERED YET !")
                            Exit For
                        End If

                        If bottomType = "Flat" OrElse bottomType = "Flat Mohair" Then
                            If String.IsNullOrEmpty(bottomOption) Then
                                MessageError(True, "FLAT BOTTOM IS REQUIRED !")
                                Exit For
                            End If
                        End If

                        If bottomOption = "Front Wrap" Then bottomOption = "Fabric on Front"
                        If bottomOption = "Back Wrap" Then bottomOption = "Fabric on Back"

                        If msgError.InnerText = "" Then
                            Dim controlPosition As String = String.Empty

                            Dim rollB As String = String.Empty : Dim controlPositionB As String = String.Empty
                            Dim rollC As String = String.Empty : Dim controlPositionC As String = String.Empty
                            Dim rollD As String = String.Empty : Dim controlPositionD As String = String.Empty
                            Dim rollE As String = String.Empty : Dim controlPositionE As String = String.Empty
                            Dim rollF As String = String.Empty : Dim controlPositionF As String = String.Empty

                            Dim bottomIdB As String = String.Empty : Dim bottomColourIdB As String = String.Empty
                            Dim bottomIdC As String = String.Empty : Dim bottomColourIdC As String = String.Empty
                            Dim bottomIdD As String = String.Empty : Dim bottomColourIdD As String = String.Empty
                            Dim bottomIdE As String = String.Empty : Dim bottomColourIdE As String = String.Empty
                            Dim bottomIdF As String = String.Empty : Dim bottomColourIdF As String = String.Empty

                            Dim bottomOptionB As String = String.Empty : Dim bottomOptionC As String = String.Empty : Dim bottomOptionD As String = String.Empty : Dim bottomOptionE As String = String.Empty : Dim bottomOptionF As String = String.Empty

                            Dim chainIdB As String = String.Empty : Dim chainStopperB As String = String.Empty
                            Dim chainIdC As String = String.Empty : Dim chainStopperC As String = String.Empty
                            Dim chainIdD As String = String.Empty : Dim chainStopperD As String = String.Empty
                            Dim chainIdE As String = String.Empty : Dim chainStopperE As String = String.Empty
                            Dim chainIdF As String = String.Empty : Dim chainStopperF As String = String.Empty

                            Dim controlLengthB As String = String.Empty : Dim controlLengthValueB As Integer = 0
                            Dim controlLengthC As String = String.Empty : Dim controlLengthValueC As Integer = 0
                            Dim controlLengthD As String = String.Empty : Dim controlLengthValueD As Integer = 0
                            Dim controlLengthE As String = String.Empty : Dim controlLengthValueE As Integer = 0
                            Dim controlLengthF As String = String.Empty : Dim controlLengthValueF As Integer = 0

                            Dim priceProductGroup As String = String.Empty
                            Dim priceProductGroupB As String = String.Empty
                            Dim priceProductGroupC As String = String.Empty
                            Dim priceProductGroupD As String = String.Empty
                            Dim priceProductGroupE As String = String.Empty
                            Dim priceProductGroupF As String = String.Empty

                            Dim bracketextension As String = String.Empty

                            Dim totalItems As Integer = 1

                            Dim groupFabric As String = String.Empty

                            If blindName = "Single Blind" Then
                                If bracketType = "Extension" Then bracketextension = "Yes"

                                If roll = "Back Roll" Then roll = "Standard"
                                If roll = "Front Roll" Then roll = "Reverse"

                                If bottomOption = "Front Wrap" Then bottomOption = "Fabric on Front"
                                If bottomOption = "Back Wrap" Then bottomOption = "Fabric on Back"

                                controlPosition = controlText

                                groupFabric = orderClass.GetFabricGroup(fabricId)
                                Dim groupName As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabric)
                                priceProductGroup = orderClass.GetPriceProductGroupId(groupName, designId)
                            End If

                            If blindName = "Dual Blinds" Then
                                groupFabric = orderClass.GetFabricGroup(fabricId)
                                Dim groupFabricDB As String = orderClass.GetFabricGroup(fabricIdB)

                                Dim groupName As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabric)
                                Dim groupNameDB As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabricDB)

                                widthDataB = widthData

                                If controlText = "L - L" Then
                                    controlPosition = "Left" : controlPositionB = "Left"
                                End If
                                If controlText = "R - R" Then
                                    controlPosition = "Right" : controlPositionB = "Right"
                                End If

                                If roll = "Back Roll" Then roll = "Standard"
                                If roll = "Front Roll" Then roll = "Reverse"

                                If rollDB = "Back Roll" Then rollB = "Standard"
                                If rollDB = "Front Roll" Then rollB = "Reverse"

                                totalItems = 2

                                linearMetreB = linearMetre
                                squareMetreB = squareMetre

                                priceProductGroup = orderClass.GetPriceProductGroupId(groupName, designId)
                                priceProductGroupB = orderClass.GetPriceProductGroupId(groupNameDB, designId)

                                If controlType = "Chain" Then
                                    chainIdB = chainId
                                    chainStopperB = chainStopper
                                    controlLengthB = controlLength
                                    controlLengthValueB = controlLengthValue
                                End If

                                bottomIdB = bottomId
                                bottomColourIdB = bottomColourId
                                bottomOptionB = bottomOption
                            End If

                            Dim itemId As String = orderClass.GetNewOrderItemId()

                            Using thisConn As SqlConnection = New SqlConnection(myConn)
                                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricIdB, FabricIdC, FabricIdD, FabricIdE, FabricIdF, FabricColourId, FabricColourIdB, FabricColourIdC, FabricColourIdD, FabricColourIdE, FabricColourIdF, ChainId, ChainIdB, ChainIdC, ChainIdD, ChainIdE, ChainIdF, BottomId, BottomIdB, BottomIdC, BottomIdD, BottomIdE, BottomIdF, BottomColourId, BottomColourIdB, BottomColourIdC, BottomColourIdD, BottomColourIdE, BottomColourIdF, PriceProductGroupId, PriceProductGroupIdB, PriceProductGroupIdC, PriceProductGroupIdD, PriceProductGroupIdE, PriceProductGroupIdF, Qty, Room, Mounting, Width, WidthB, WidthC, WidthD, WidthE, WidthF, [Drop], DropB, DropC, DropD, DropE, DropF, Roll, RollB, RollC, RollD, RollE, RollF, ControlPosition, ControlPositionB, ControlPositionC, ControlPositionD, ControlPositionE, ControlPositionF, ControlLength, ControlLengthB, ControlLengthC, ControlLengthD, ControlLengthE, ControlLengthF, ControlLengthValue, ControlLengthValueB, ControlLengthValueC, ControlLengthValueD, ControlLengthValueE, ControlLengthValueF, ChainStopper, ChainStopperB, ChainStopperC, ChainStopperD, ChainStopperE, ChainStopperF, FlatOption, FlatOptionB, FlatOptionC, FlatOptionD, FlatOptionE, FlatOptionF, BracketExtension, LinearMetre, LinearMetreB, LinearMetreC, LinearMetreD, LinearMetreE, LinearMetreF, SquareMetre, SquareMetreB, SquareMetreC, SquareMetreD, SquareMetreE, SquareMetreF, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricIdB, @FabricIdC, @FabricIdD, @FabricIdE, @FabricIdF, @FabricColourId, @FabricColourIdB, @FabricColourIdC, @FabricColourIdD, @FabricColourIdE, @FabricColourIdF, @ChainId, @ChainIdB, @ChainIdC, @ChainIdD, @ChainIdE, @ChainIdF, @BottomId, @BottomIdB, @BottomIdC, @BottomIdD, @BottomIdE, @BottomIdF, @BottomColourId, @BottomColourIdB, @BottomColourIdC, @BottomColourIdD, @BottomColourIdE, @BottomColourIdF, @PriceProductGroupId, @PriceProductGroupIdB, @PriceProductGroupIdC, @PriceProductGroupIdD, @PriceProductGroupIdE, @PriceProductGroupIdF, @Qty, @Room, @Mounting, @Width, @WidthB, @WidthC, @WidthD, @WidthE, @WidthF, @Drop, @DropB, @DropC, @DropD, @DropE, @DropF, @Roll, @RollB, @RollC, @RollD, @RollE, @RollF, @ControlPosition, @ControlPositionB, @ControlPositionC, @ControlPositionD, @ControlPositionE, @ControlPositionF, @ControlLength, @ControlLengthB, @ControlLengthC, @ControlLengthD, @ControlLengthE, @ControlLengthF, @ControlLengthValue, @ControlLengthValueB, @ControlLengthValueC, @ControlLengthValueD, @ControlLengthValueE, @ControlLengthValueF, @ChainStopper, @ChainStopperB, @ChainStopperC, @ChainStopperD, @ChainStopperE, @ChainStopperF, @FlatOption, @FlatOptionB, @FlatOptionC, @FlatOptionD, @FlatOptionE, @FlatOptionF, @BracketExtension, @LinearMetre, @LinearMetreB, @LinearMetreC, @LinearMetreD, @LinearMetreE, @LinearMetreF, @SquareMetre, @SquareMetreB, @SquareMetreC, @SquareMetreD, @SquareMetreE, @SquareMetreF, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                                    myCmd.Parameters.AddWithValue("@Id", itemId)
                                    myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                    myCmd.Parameters.AddWithValue("@ProductId", productId)
                                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                    myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(fabricIdB), CType(DBNull.Value, Object), fabricIdB))
                                    myCmd.Parameters.AddWithValue("@FabricIdC", If(String.IsNullOrEmpty(fabricIdC), CType(DBNull.Value, Object), fabricIdC))
                                    myCmd.Parameters.AddWithValue("@FabricIdD", If(String.IsNullOrEmpty(fabricIdD), CType(DBNull.Value, Object), fabricIdD))
                                    myCmd.Parameters.AddWithValue("@FabricIdE", If(String.IsNullOrEmpty(fabricIdE), CType(DBNull.Value, Object), fabricIdE))
                                    myCmd.Parameters.AddWithValue("@FabricIdF", If(String.IsNullOrEmpty(fabricIdF), CType(DBNull.Value, Object), fabricIdF))
                                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                    myCmd.Parameters.AddWithValue("@FabricColourIdB", If(String.IsNullOrEmpty(fabricColourIdB), CType(DBNull.Value, Object), fabricColourIdB))
                                    myCmd.Parameters.AddWithValue("@FabricColourIdC", If(String.IsNullOrEmpty(fabricColourIdC), CType(DBNull.Value, Object), fabricColourIdC))
                                    myCmd.Parameters.AddWithValue("@FabricColourIdD", If(String.IsNullOrEmpty(fabricColourIdD), CType(DBNull.Value, Object), fabricColourIdD))
                                    myCmd.Parameters.AddWithValue("@FabricColourIdE", If(String.IsNullOrEmpty(fabricColourIdE), CType(DBNull.Value, Object), fabricColourIdE))
                                    myCmd.Parameters.AddWithValue("@FabricColourIdF", If(String.IsNullOrEmpty(fabricColourIdF), CType(DBNull.Value, Object), fabricColourIdF))
                                    myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                    myCmd.Parameters.AddWithValue("@ChainIdB", If(String.IsNullOrEmpty(chainIdB), CType(DBNull.Value, Object), chainIdB))
                                    myCmd.Parameters.AddWithValue("@ChainIdC", If(String.IsNullOrEmpty(chainIdC), CType(DBNull.Value, Object), chainIdC))
                                    myCmd.Parameters.AddWithValue("@ChainIdD", If(String.IsNullOrEmpty(chainIdD), CType(DBNull.Value, Object), chainIdD))
                                    myCmd.Parameters.AddWithValue("@ChainIdE", If(String.IsNullOrEmpty(chainIdE), CType(DBNull.Value, Object), chainIdE))
                                    myCmd.Parameters.AddWithValue("@ChainIdF", If(String.IsNullOrEmpty(chainIdF), CType(DBNull.Value, Object), chainIdF))
                                    myCmd.Parameters.AddWithValue("@BottomId", If(String.IsNullOrEmpty(bottomId), CType(DBNull.Value, Object), bottomId))
                                    myCmd.Parameters.AddWithValue("@BottomIdB", If(String.IsNullOrEmpty(bottomIdB), CType(DBNull.Value, Object), bottomIdB))
                                    myCmd.Parameters.AddWithValue("@BottomIdC", If(String.IsNullOrEmpty(bottomIdC), CType(DBNull.Value, Object), bottomIdC))
                                    myCmd.Parameters.AddWithValue("@BottomIdD", If(String.IsNullOrEmpty(bottomIdD), CType(DBNull.Value, Object), bottomIdD))
                                    myCmd.Parameters.AddWithValue("@BottomIdE", If(String.IsNullOrEmpty(bottomIdE), CType(DBNull.Value, Object), bottomIdE))
                                    myCmd.Parameters.AddWithValue("@BottomIdF", If(String.IsNullOrEmpty(bottomIdF), CType(DBNull.Value, Object), bottomIdF))
                                    myCmd.Parameters.AddWithValue("@BottomColourId", If(String.IsNullOrEmpty(bottomColourId), CType(DBNull.Value, Object), bottomColourId))
                                    myCmd.Parameters.AddWithValue("@BottomColourIdB", If(String.IsNullOrEmpty(bottomColourIdB), CType(DBNull.Value, Object), bottomColourIdB))
                                    myCmd.Parameters.AddWithValue("@BottomColourIdC", If(String.IsNullOrEmpty(bottomColourIdC), CType(DBNull.Value, Object), bottomColourIdC))
                                    myCmd.Parameters.AddWithValue("@BottomColourIdD", If(String.IsNullOrEmpty(bottomColourIdD), CType(DBNull.Value, Object), bottomColourIdD))
                                    myCmd.Parameters.AddWithValue("@BottomColourIdE", If(String.IsNullOrEmpty(bottomColourIdE), CType(DBNull.Value, Object), bottomColourIdE))
                                    myCmd.Parameters.AddWithValue("@BottomColourIdF", If(String.IsNullOrEmpty(bottomColourIdF), CType(DBNull.Value, Object), bottomColourIdF))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdC", If(String.IsNullOrEmpty(priceProductGroupC), CType(DBNull.Value, Object), priceProductGroupC))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdD", If(String.IsNullOrEmpty(priceProductGroupD), CType(DBNull.Value, Object), priceProductGroupD))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdE", If(String.IsNullOrEmpty(priceProductGroupE), CType(DBNull.Value, Object), priceProductGroupE))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdF", If(String.IsNullOrEmpty(priceProductGroupF), CType(DBNull.Value, Object), priceProductGroupF))
                                    myCmd.Parameters.AddWithValue("@Qty", "1")
                                    myCmd.Parameters.AddWithValue("@Room", room)
                                    myCmd.Parameters.AddWithValue("@Mounting", sizeType & " " & mounting)
                                    myCmd.Parameters.AddWithValue("@Width", widthData)
                                    myCmd.Parameters.AddWithValue("@WidthB", widthDataB)
                                    myCmd.Parameters.AddWithValue("@WidthC", widthc)
                                    myCmd.Parameters.AddWithValue("@WidthD", widthd)
                                    myCmd.Parameters.AddWithValue("@WidthE", widthe)
                                    myCmd.Parameters.AddWithValue("@WidthF", widthf)
                                    myCmd.Parameters.AddWithValue("@Drop", dropData)
                                    myCmd.Parameters.AddWithValue("@DropB", dropData)
                                    myCmd.Parameters.AddWithValue("@DropC", dropData)
                                    myCmd.Parameters.AddWithValue("@DropD", dropData)
                                    myCmd.Parameters.AddWithValue("@DropE", dropData)
                                    myCmd.Parameters.AddWithValue("@DropF", dropData)
                                    myCmd.Parameters.AddWithValue("@Roll", roll)
                                    myCmd.Parameters.AddWithValue("@RollB", rollB)
                                    myCmd.Parameters.AddWithValue("@RollC", rollC)
                                    myCmd.Parameters.AddWithValue("@RollD", rollD)
                                    myCmd.Parameters.AddWithValue("@RollE", rollE)
                                    myCmd.Parameters.AddWithValue("@RollF", rollF)
                                    myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                    myCmd.Parameters.AddWithValue("@ControlPositionB", controlPositionB)
                                    myCmd.Parameters.AddWithValue("@ControlPositionC", controlPositionC)
                                    myCmd.Parameters.AddWithValue("@ControlPositionD", controlPositionD)
                                    myCmd.Parameters.AddWithValue("@ControlPositionE", controlPositionE)
                                    myCmd.Parameters.AddWithValue("@ControlPositionF", controlPositionF)
                                    myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                                    myCmd.Parameters.AddWithValue("@ControlLengthB", controlLengthB)
                                    myCmd.Parameters.AddWithValue("@ControlLengthC", controlLengthC)
                                    myCmd.Parameters.AddWithValue("@ControlLengthD", controlLengthD)
                                    myCmd.Parameters.AddWithValue("@ControlLengthE", controlLengthE)
                                    myCmd.Parameters.AddWithValue("@ControlLengthF", controlLengthF)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValueB", controlLengthValueB)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValueC", controlLengthValueC)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValueD", controlLengthValueD)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValueE", controlLengthValueE)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValueF", controlLengthValueF)
                                    myCmd.Parameters.AddWithValue("@ChainStopper", chainStopper)
                                    myCmd.Parameters.AddWithValue("@ChainStopperB", chainStopperB)
                                    myCmd.Parameters.AddWithValue("@ChainStopperC", chainStopperC)
                                    myCmd.Parameters.AddWithValue("@ChainStopperD", chainStopperD)
                                    myCmd.Parameters.AddWithValue("@ChainStopperE", chainStopperE)
                                    myCmd.Parameters.AddWithValue("@ChainStopperF", chainStopperF)
                                    myCmd.Parameters.AddWithValue("@FlatOption", bottomOption)
                                    myCmd.Parameters.AddWithValue("@FlatOptionB", bottomOptionB)
                                    myCmd.Parameters.AddWithValue("@FlatOptionC", bottomOptionC)
                                    myCmd.Parameters.AddWithValue("@FlatOptionD", bottomOptionD)
                                    myCmd.Parameters.AddWithValue("@FlatOptionE", bottomOptionE)
                                    myCmd.Parameters.AddWithValue("@FlatOptionF", bottomOptionF)
                                    myCmd.Parameters.AddWithValue("@BracketExtension", bracketextension)
                                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                                    myCmd.Parameters.AddWithValue("@LinearMetreB", linearMetreB)
                                    myCmd.Parameters.AddWithValue("@LinearMetreC", linearMetreC)
                                    myCmd.Parameters.AddWithValue("@LinearMetreD", linearMetreD)
                                    myCmd.Parameters.AddWithValue("@LinearMetreE", linearMetreE)
                                    myCmd.Parameters.AddWithValue("@LinearMetreF", linearMetreF)
                                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                                    myCmd.Parameters.AddWithValue("@SquareMetreB", squareMetreB)
                                    myCmd.Parameters.AddWithValue("@SquareMetreC", squareMetreC)
                                    myCmd.Parameters.AddWithValue("@SquareMetreD", squareMetreD)
                                    myCmd.Parameters.AddWithValue("@SquareMetreE", squareMetreE)
                                    myCmd.Parameters.AddWithValue("@SquareMetreF", squareMetreF)
                                    myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                                    myCmd.Parameters.AddWithValue("@Notes", notes)
                                    myCmd.Parameters.AddWithValue("@MarkUp", 0)

                                    thisConn.Open()
                                    myCmd.ExecuteNonQuery()
                                End Using
                            End Using

                            orderClass.ResetPriceDetail(headerId, itemId)
                            orderClass.CalculatePrice(headerId, itemId)
                            orderClass.FinalCostItem(headerId, itemId)

                            dataLog = {"OrderDetails", itemId, Session("LoginId"), "Order Item Added"}
                            orderClass.Logs(dataLog)
                        End If
                    End If

                    If designType = "Roman" Then
                        Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 3).Text), 0, CInt(sheetDetail.Cells(row, 3).Text))
                        Dim room As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim tubeType As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim batten As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                        Dim fabricType As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim widthText As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        'Dim width As Integer

                        Dim dropText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        'Dim drop As Integer

                        Dim controlPosition As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim controlType As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                    End If
                Next
            End Using

            If Not msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()

                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET Active=0 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", headerId)
                        myCmd.ExecuteNonQuery()
                    End Using

                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET Active=0 WHERE HeaderId=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", headerId)
                        myCmd.ExecuteNonQuery()
                    End Using

                    thisConn.Close()
                End Using
            End If

            If msgError.InnerText = "" Then
                url = String.Format("~/order/detail?orderid={0}", headerId)
                Response.Redirect(url, False)
            End If
        End Using
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
