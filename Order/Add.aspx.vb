Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports OfficeOpenXml

Partial Class Order_Add
    Inherits Page

    Dim orderClass As New OrderClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        If Session("RoleName") = "Customer" Then
            Dim status As Boolean = orderClass.GetCustomerOnStop(Session("CustomerId").ToString())
            If status = True Then
                Response.Redirect("~/order/", False)
                Exit Sub
            End If
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDataCustomer()
            BindDataUser(ddlCustomer.SelectedValue)

            BindComponentForm(ddlCustomer.SelectedValue, ddlMethod.SelectedValue)
        End If
    End Sub

    Protected Sub ddlCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataUser(ddlCustomer.SelectedValue)
        BindComponentForm(ddlCustomer.SelectedValue, ddlMethod.SelectedValue)
    End Sub

    Protected Sub ddlMethod_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindComponentForm(ddlCustomer.SelectedValue, ddlMethod.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim companyAlias As String = orderClass.GetCompanyAliasByCustomer(ddlCustomer.SelectedValue)
            Dim companyDetailName As String = orderClass.GetCompanyDetailNameByCustomer(ddlCustomer.SelectedValue)

            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                Exit Sub
            End If

            If ddlMethod.SelectedValue = "" Then
                MessageError(True, "ORDER METHOD IS REQUIRED !")
                Exit Sub
            End If

            If ddlMethod.SelectedValue = "Manual" Then
                If txtOrderNumber.Text = "" Then
                    MessageError(True, "ORDER NUMBER IS REQUIRED !")
                    Exit Sub
                End If

                If InStr(txtOrderNumber.Text, "\") > 0 OrElse InStr(txtOrderNumber.Text, "/") > 0 OrElse InStr(txtOrderNumber.Text, ",") > 0 OrElse InStr(txtOrderNumber.Text, "&") > 0 OrElse InStr(txtOrderNumber.Text, "#") > 0 OrElse InStr(txtOrderNumber.Text, "'") > 0 OrElse InStr(txtOrderNumber.Text, ".") > 0 Then
                    MessageError(True, "PLEASE DON'T USE [ / ], [ \ ], [ & ], [ # ], [ ' ], [ . ] AND [ , ]")
                    Exit Sub
                End If

                If Trim(txtOrderNumber.Text).Length > 20 Then
                    MessageError(True, "MAXIMUM 20 CHARACTERS FOR RETAILER ORDER NUMBER !")
                    Exit Sub
                End If

                If txtOrderName.Text = "" Then
                    MessageError(True, "ORDER NAME IS REQUIRED !")
                    Exit Sub
                End If

                If txtOrderNumber.Text = orderClass.IsOrderExist(ddlCustomer.SelectedValue, txtOrderNumber.Text.Trim()) Then
                    MessageError(True, "ORDER NUMBER ALREADY EXISTS !")
                    Exit Sub
                End If

                If ddlOrderType.SelectedValue = "Builder" AndAlso Not companyDetailName = "JPMD BP" Then
                    MessageError(True, "ORDER TYPE SHOULD BE REGULAR !")
                    Exit Sub
                End If

                If msgError.InnerText = "" Then
                    Dim thisId As String = orderClass.GetNewOrderHeaderId()

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
                                Using myCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, DownloadBOE, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, @OrderType, 'Unsubmitted', @CreatedBy, GETDATE(), 0, 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
                                    myCmd.Parameters.AddWithValue("@Id", thisId)
                                    myCmd.Parameters.AddWithValue("@OrderId", orderId)
                                    myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                                    myCmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumber.Text.Trim())
                                    myCmd.Parameters.AddWithValue("@OrderName", txtOrderName.Text.Trim())
                                    myCmd.Parameters.AddWithValue("@OrderNote", txtOrderNote.Text.Trim())
                                    myCmd.Parameters.AddWithValue("@OrderType", ddlOrderType.SelectedValue)
                                    myCmd.Parameters.AddWithValue("@CreatedBy", ddlCreatedBy.SelectedValue)

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

                    If ddlOrderType.SelectedValue = "Builder" Then
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As New SqlCommand("INSERT INTO OrderBuilders(Id) VALUES (@Id)", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", thisId)

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using
                    End If

                    Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Order/{0}/", orderId))
                    If Not Directory.Exists(directoryOrder) Then
                        Directory.CreateDirectory(directoryOrder)
                    End If

                    Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId").ToString(), "Order Created"}
                    orderClass.Logs(dataLog)

                    url = String.Format("~/order/detail?orderid={0}", thisId)
                    Response.Redirect(url, False)
                End If
            End If

            If ddlMethod.SelectedValue = "Upload" Then
                If Not fuFile.HasFiles Then
                    MessageError(True, "NO FILE SELECTED. PLEASE SELECT A FILE TO UPLOAD !")
                    Exit Sub
                End If

                Dim fileExtension As String = Path.GetExtension(fuFile.FileName).ToLower()
                If fileExtension = ".xls" Or fileExtension = ".xlsx" Then
                    Dim fileName As String = fuFile.FileName

                    Dim savePath As String = Server.MapPath(String.Format("~/file/cws/{0}", fileName))
                    fuFile.SaveAs(savePath)

                    If ddlCustomer.SelectedValue = "985" Then
                        ReadFileData(savePath, ddlCustomer.SelectedValue)
                    End If
                    If ddlCustomer.SelectedValue = "127" Then
                        ReadExcelData(savePath)
                    End If

                    ReadExcelData(savePath)
                End If
            End If

            If ddlMethod.SelectedValue = "API" Then
                MessageError(True, "UNDER MAINTENANCE !")
                Exit Sub
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

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order/", False)
    End Sub

    Protected Sub ReadFileData(filePath As String, customerId As String)
        Using package As New ExcelPackage(New FileInfo(filePath))
            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)

            Dim customerData As DataRow = orderClass.GetDataRow("SELECT Customers.CompanyDetailId AS CompanyDetailId, Companys.Alias AS CompanyAlias FROM Customers LEFT JOIN Companys ON Customers.CompanyId=Companys.Id WHERE Customers.Id='" & customerId & "'")
            Dim companyAlias As String = customerData("CompanyAlias")
            Dim companyDetailId As String = customerData("CompanyDetailId")

            Dim headerId As String = orderClass.GetNewOrderHeaderId

            Dim orderNumber As String = worksheet.Cells(2, 1).Text
            Dim orderName As String = worksheet.Cells(2, 2).Text
            Dim orderNote As String = worksheet.Cells(2, 3).Text

            If orderNumber = orderClass.IsOrderExist(ddlCustomer.SelectedValue, orderNumber.Trim()) Then
                MessageError(True, "ORDER NUMBER ALREADY EXISTS !")
                Exit Sub
            End If

            Dim success As Boolean = False
            Dim retry As Integer = 0
            Dim maxRetry As Integer = 100
            Dim orderId As String = ""

            Do While Not success
                retry += 1
                If retry > maxRetry Then Throw New Exception("FAILED TO GENERATE UNIQUE ORDER ID")

                Dim randomCode As String = orderClass.GenerateRandomCode()
                orderId = companyAlias & randomCode
                Try
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, DownloadBOE, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, 'Regular', 'Unsubmitted', @CreatedBy, GETDATE(), 0, 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
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

            Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Order/{0}/", orderId))
            If Not Directory.Exists(directoryOrder) Then
                Directory.CreateDirectory(directoryOrder)
            End If

            Dim fileName As String = Path.GetFileName(filePath)
            Dim newPath As String = Path.Combine(directoryOrder, fileName)
            File.Copy(filePath, newPath, True)

            Dim dataLog As Object() = {"OrderHeaders", headerId, Session("LoginId").ToString(), "Order Created | CSV"}
            orderClass.Logs(dataLog)

            Using orderItem As New ExcelPackage(New FileInfo(filePath))
                Dim sheetDetail As ExcelWorksheet = orderItem.Workbook.Worksheets(0)
                Dim startRow As Integer = 4
                Dim lastRow As Integer = sheetDetail.Dimension.End.Row

                For row As Integer = startRow To lastRow
                    Dim itemNumber As String = row - 3

                    Dim rowData As DataRow = Nothing
                    Dim rowActive As Boolean = Nothing

                    Dim designName As String = (sheetDetail.Cells(row, 1).Text & "").Trim()

                    Dim designId As String = String.Empty

                    If designName = "Roller Blind" Then
                        Dim blindName As String = (sheetDetail.Cells(row, 2).Text & "").Trim()
                        Dim controlName As String = (sheetDetail.Cells(row, 3).Text & "").Trim()
                        Dim tubeName As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim colourName As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim qty As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim room As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim fabricType As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim fabricTypeDB As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim fabricColourDB As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim rollDirection As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim rollDirectionDB As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim controlPosition As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim controlPositionDB As String = (sheetDetail.Cells(row, 16).Text & "").Trim()

                        Dim widthText As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim widthData As Integer = 0

                        Dim widthTextB As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                        Dim widthDataB As Integer = 0

                        Dim widthTextC As String = (sheetDetail.Cells(row, 19).Text & "").Trim()
                        Dim widthDataC As Integer = 0

                        Dim dropText As String = (sheetDetail.Cells(row, 20).Text & "").Trim()
                        Dim dropData As Integer = 0

                        Dim chainName As String = (sheetDetail.Cells(row, 21).Text & "").Trim()
                        Dim chainStopper As String = (sheetDetail.Cells(row, 22).Text & "").Trim()
                        Dim chainLengthText As String = (sheetDetail.Cells(row, 23).Text & "").Trim()
                        Dim charger As String = (sheetDetail.Cells(row, 24).Text & "").Trim()
                        Dim extensionCable As String = (sheetDetail.Cells(row, 25).Text & "").Trim()
                        Dim neobox As String = (sheetDetail.Cells(row, 26).Text & "").Trim()
                        Dim bottomType As String = (sheetDetail.Cells(row, 27).Text & "").Trim()
                        Dim bottomColour As String = (sheetDetail.Cells(row, 28).Text & "").Trim()
                        Dim bottomFlat As String = (sheetDetail.Cells(row, 29).Text & "").Trim()
                        Dim bracketExtension As String = (sheetDetail.Cells(row, 30).Text & "").Trim()
                        Dim bracketSize As String = (sheetDetail.Cells(row, 31).Text & "").Trim()
                        Dim springAssist As String = (sheetDetail.Cells(row, 32).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 33).Text & "").Trim()

                        If String.IsNullOrEmpty(blindName) Then
                            Dim thisAlert As String = String.Format("BLIND TYPE IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(controlName) Then
                            Dim thisAlert As String = String.Format("CONTROL TYPE IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(tubeName) Then
                            Dim thisAlert As String = String.Format("TUBE TYPE IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(colourName) Then
                            Dim thisAlert As String = String.Format("BRACKET COLOUR IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindName & "' AND CompanyArray.VALUE='" & companyDetailId & "'")
                        Dim controlId As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlName & "'")
                        Dim controlType As String = orderClass.GetItemData("SELECT Type FROM ProductControls WHERE Id='" & controlId & "'")
                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & controlName & "'")
                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & controlName & "'")

                        rowData = orderClass.GetDataRow("SELECT * FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.VALUE='" & companyDetailId & "' AND ControlType='" & controlId & "' AND TubeType='" & tubeId & "' AND ColourType='" & colourId & "'")
                        If rowData Is Nothing Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE PRODUCT IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim productId As String = rowData("Id").ToString()
                        rowActive = Convert.ToBoolean(rowData("Active"))

                        If rowActive = False Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE PRODUCT IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim fabricId As String = String.Empty : Dim fabricColourId As String = String.Empty
                        Dim fabricIdB As String = String.Empty : Dim fabricColourIdB As String = String.Empty
                        Dim fabricIdC As String = String.Empty : Dim fabricColourIdC As String = String.Empty
                        Dim fabricIdD As String = String.Empty : Dim fabricColourIdD As String = String.Empty
                        Dim fabricIdE As String = String.Empty : Dim fabricColourIdE As String = String.Empty
                        Dim fabricIdF As String = String.Empty : Dim fabricColourIdF As String = String.Empty

                        Dim fabricIdDB As String = String.Empty : Dim fabricColourIdDB As String = String.Empty

                        Dim chainId As String = String.Empty
                        Dim chainIdB As String = String.Empty
                        Dim chainIdC As String = String.Empty
                        Dim chainIdD As String = String.Empty
                        Dim chainIdE As String = String.Empty
                        Dim chainIdF As String = String.Empty

                        Dim chainLength As String = String.Empty

                        Dim controlLength As String = String.Empty : Dim controlLengthValue As Integer = 0
                        Dim controlLengthB As String = String.Empty : Dim controlLengthValueB As Integer = 0
                        Dim controlLengthC As String = String.Empty : Dim controlLengthValueC As Integer = 0
                        Dim controlLengthD As String = String.Empty : Dim controlLengthValueD As Integer = 0
                        Dim controlLengthE As String = String.Empty : Dim controlLengthValueE As Integer = 0
                        Dim controlLengthF As String = String.Empty : Dim controlLengthValueF As Integer = 0

                        Dim bottomId As String = String.Empty : Dim bottomColourId As String = String.Empty
                        Dim bottomIdB As String = String.Empty : Dim bottomColourIdB As String = String.Empty
                        Dim bottomIdC As String = String.Empty : Dim bottomColourIdC As String = String.Empty
                        Dim bottomIdD As String = String.Empty : Dim bottomColourIdD As String = String.Empty
                        Dim bottomIdE As String = String.Empty : Dim bottomColourIdE As String = String.Empty
                        Dim bottomIdF As String = String.Empty : Dim bottomColourIdF As String = String.Empty

                        Dim bottomIdDB As String = String.Empty : Dim bottomColourIdDB As String = String.Empty

                        Dim linearMetre As Decimal = 0D : Dim squareMetre As Decimal = 0D
                        Dim linearMetreB As Decimal = 0D : Dim squareMetreB As Decimal = 0D
                        Dim linearMetreC As Decimal = 0D : Dim squareMetreC As Decimal = 0D
                        Dim linearMetreD As Decimal = 0D : Dim squareMetreD As Decimal = 0D
                        Dim linearMetreE As Decimal = 0D : Dim squareMetreE As Decimal = 0D
                        Dim linearMetreF As Decimal = 0D : Dim squareMetreF As Decimal = 0D

                        If String.IsNullOrEmpty(blindId) Then
                            Dim thisAlert As String = String.Format("THE BLIND TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CHECK AGAIN. !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If qty <> 1 Then
                            Dim thisAlert As String = String.Format("THE ORDER QTY MUS BE 1 PER ITEM LINE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(room) Then
                            Dim thisAlert As String = String.Format("ROOM / LOCATION IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(mounting) Then
                            Dim thisAlert As String = String.Format("MOUNTING IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validMounting As String() = {"Opening Size Face Fit", "Opening Size Reveal Fit", "Make Size Face Fit", "Make Size Reveal Fit"}
                        If Not validMounting.Contains(mounting) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE MOUNTING FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricType) Then
                            Dim thisAlert As String = String.Format("FABRIC TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        rowData = orderClass.GetDataRow("SELECT * FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND companyArray.VALUE='" & companyDetailId & "'")
                        If rowData Is Nothing Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC TYPE IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        fabricId = rowData("Id").ToString()
                        rowActive = Convert.ToBoolean(rowData("Active"))

                        If rowActive = False Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC TYPE IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricColour) Then
                            Dim thisAlert As String = String.Format("FABRIC COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        rowData = orderClass.GetDataRow("SELECT * FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "'")
                        If rowData Is Nothing Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC COLOUR IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        fabricColourId = rowData("Id")
                        rowActive = Convert.ToBoolean(rowData("Active"))

                        If rowActive = False Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC COLOUR IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Dual Blinds" Then
                            If String.IsNullOrEmpty(fabricTypeDB) Then
                                Dim thisAlert As String = String.Format("FABRIC TYPE (DB) IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            rowData = orderClass.GetDataRow("SELECT * FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricTypeDB & "' AND designArray.VALUE='" & designId & "' AND companyArray.VALUE='" & companyDetailId & "'")
                            If rowData Is Nothing Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC TYPE (DB) IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            fabricIdDB = rowData("Id").ToString()
                            rowActive = Convert.ToBoolean(rowData("Active"))

                            If rowActive = False Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC TYPE (DB) IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If String.IsNullOrEmpty(fabricColourDB) Then
                                Dim thisAlert As String = String.Format("FABRIC COLOUR (DB) IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            rowData = orderClass.GetDataRow("SELECT * FROM FabricColours WHERE FabricId='" & fabricIdDB & "' AND Colour='" & fabricColourDB & "'")
                            If rowData Is Nothing Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC COLOUR (DB) IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            fabricColourIdDB = rowData("Id")
                            rowActive = Convert.ToBoolean(rowData("Active"))

                            If rowActive = False Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC COLOUR  (DB) IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(rollDirection) Then
                            Dim thisAlert As String = String.Format("ROLL DIRECTION IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validRoll As String() = {"Standard", "Reverse"}
                        If Not validRoll.Contains(rollDirection) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE ROLL DIRECTION FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Dual Blinds" Then
                            If String.IsNullOrEmpty(rollDirectionDB) Then
                                Dim thisAlert As String = String.Format("ROLL DIRECTION (DB) IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If Not validRoll.Contains(rollDirectionDB) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE ROLL DIRECTION (DB) FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(controlPosition) Then
                            Dim thisAlert As String = String.Format("CONTROL POSITION IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validControl As String() = {"Left", "Right"}
                        If Not validControl.Contains(controlPosition) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE CONTROL POSITION FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Dual Blinds" Then
                            If String.IsNullOrEmpty(controlPositionDB) Then
                                Dim thisAlert As String = String.Format("CONTROL POSITION (DB) IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If Not validControl.Contains(controlPositionDB) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE CONTROL POSITION (DB) FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(widthText) Then
                            Dim thisAlert As String = String.Format("WIDTH IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If Not Integer.TryParse(widthText, widthData) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If widthData < 200 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} & MINIMUM WIDTH IS 200MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If tubeName.Contains("Gear Reduction") Then
                            If tubeName = "Gear Reduction 38mm" AndAlso widthData > 1810 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} & MAXIMUM WIDTH IS 1810MM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If
                        If widthData > 2910 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} & MAXIMUM WIDTH IS 2910MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(dropText) Then
                            Dim thisAlert As String = String.Format("DROP IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If Not Integer.TryParse(dropText, dropData) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If dropData < 200 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} & MINIMUM DROP IS 200MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If dropData > 3200 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} & MAXIMUM DROP IS 3200MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        squareMetre = widthData * dropData / 1000000
                        If squareMetre >= 6 AndAlso (tubeName = "Gear Reduction 38mm" OrElse tubeName = "Gear Reduction 45mm") Then
                            Dim thisAlert As String = String.Format("BLIND AREA FOR ITEM {0} EXCEEDS 6 SQM.<br />PLEASE USE <b>GEAR REDUCTION 49MM</b> IF YOU WISH TO CONTINUE USING THE GEAR REDUCTION SYSTEM.<br />OUR ALTERNATIVE RECOMMENDATION:<br />ACMEDA SYSTEM: <b>ACMEDA 49MM</b><br />SUNBOSS SYSTEM: <b>SUNBOSS 43MM</b> OR <b>SUNBOSS 50MM</b>", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(chainName) Then
                            Dim thisAlert As String = String.Format("CHAIN COLOUR IS REQUIRED FOR ITEM {0}", itemNumber)
                            If controlType = "Motorised" Then
                                thisAlert = String.Format("REMOTE TYPE IS REQUIRED FOR ITEM {0}", itemNumber)
                            End If
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        rowData = orderClass.GetDataRow("SELECT * FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(ControlTypeId, ',') AS controlArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & chainName & "' AND designArray.VALUE='" & designId & "' AND controlArray.VALUE='" & controlId & "' AND companyArray.VALUE='" & companyDetailId & "'")
                        If rowData Is Nothing Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE CHAIN COLOUR IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        chainId = rowData("Id").ToString()
                        rowActive = Convert.ToBoolean(rowData("Active"))

                        If rowActive = False Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE CHAIN COLOUR IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If controlName = "Chain" AndAlso String.IsNullOrEmpty(chainStopper) Then
                            Dim thisAlert As String = String.Format("CHAIN STOPPER IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validStopper As String() = {"No Stopper", "With Stopper"}
                        If Not validStopper.Contains(chainStopper) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE CHAIN STOPPER FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If controlName = "Chain" Then
                            chainLength = orderClass.GetChainLength(chainId)
                            Dim stdControlLength As Integer = Math.Ceiling(dropData * 2 / 3)

                            controlLength = "Standard"
                            If chainLength = "Static" Then
                                controlLengthValue = 500
                                If stdControlLength > 500 Then controlLengthValue = 750
                                If stdControlLength > 750 Then controlLengthValue = 1000
                                If stdControlLength > 1000 Then controlLengthValue = 1200
                                If stdControlLength > 1200 Then controlLengthValue = 1500
                            End If
                            If chainLength = "Flexible" Then
                                controlLengthValue = Math.Ceiling(dropData * 2 / 3)
                                If tubeName.Contains("Gear Reduction") Then
                                    controlLengthValue = Math.Ceiling((dropData * 3 / 4) + 80)
                                End If
                            End If

                            If Not String.IsNullOrEmpty(chainLengthText) AndAlso Not chainLengthText = "Standard" Then
                                controlLength = "Custom"
                                chainLengthText = chainLengthText.Replace("mm", "")
                                If Not Integer.TryParse(chainLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE CHAIN LENGTH VALUE FOR ITEM {0}", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If String.IsNullOrEmpty(bottomType) Then
                            Dim thisAlert As String = String.Format("BOTTOM TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        rowData = orderClass.GetDataRow("SELECT * FROM Bottoms CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & bottomType & "' AND designArray.VALUE='" & designId & "' AND companyArray.VALUE='" & companyDetailId & "'")
                        If rowData Is Nothing Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE BOTTOM TYPE IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        bottomId = rowData("Id").ToString()
                        rowActive = Convert.ToBoolean(rowData("Active"))

                        If rowActive = False Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE BOTTOM TYPE IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(bottomColour) Then
                            Dim thisAlert As String = String.Format("BOTTOM COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        rowData = orderClass.GetDataRow("SELECT * FROM BottomColours WHERE FabricId='" & bottomId & "' AND Colour='" & bottomColour & "'")
                        If rowData Is Nothing Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE BOTTOM COLOUR IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        bottomColourId = rowData("Id")
                        rowActive = Convert.ToBoolean(rowData("Active"))

                        If rowActive = False Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE BOTTOM COLOUR IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                    End If

                    If designName = "Vertical" Then
                        Dim blindName As String = (sheetDetail.Cells(row, 2).Text & "").Trim()
                        Dim tubeName As String = (sheetDetail.Cells(row, 3).Text & "").Trim()
                        Dim controlName As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim colourName As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim qty As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim qtyBladeText As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                        Dim room As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim widthText As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim dropText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim fabricInsert As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim fabricType As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim stackPosition As String = (sheetDetail.Cells(row, 15).Text & "").Trim()

                        Dim controlPosition As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                        Dim controlColour As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim controlLengthText As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                        Dim bottomJoining As String = (sheetDetail.Cells(row, 19).Text & "").Trim()
                        Dim bracketExtension As String = (sheetDetail.Cells(row, 20).Text & "").Trim()
                        Dim sloping As String = (sheetDetail.Cells(row, 21).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 22).Text & "").Trim()

                        Dim productId As String = String.Empty
                        Dim qtyBlade As Integer
                        Dim width As Integer
                        Dim drop As Integer
                        Dim fabricId As String = String.Empty
                        Dim fabricColourId As String = String.Empty
                        Dim chainId As String = String.Empty
                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0
                        Dim wandLengthValue As Integer = 0

                        If String.IsNullOrEmpty(blindName) Then
                            Dim thisAlert As String = String.Format("BLIND TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindName & "' AND CompanyArray.VALUE='" & companyDetailId & "'")

                        If String.IsNullOrEmpty(blindId) Then
                            Dim thisAlert As String = String.Format("THE ORDER TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CHECK AGAIN. !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(tubeName) Then
                            Dim thisAlert As String = String.Format("TUBE TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & tubeName & "'")

                        If String.IsNullOrEmpty(tubeId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE TUBE TYPE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Slat Only" Then controlName = "N/A"

                        If String.IsNullOrEmpty(controlName) Then
                            Dim thisAlert As String = String.Format("CONTROL TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim controlId As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlName & "'")

                        If String.IsNullOrEmpty(controlId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL TYPE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Slat Only" Then colourName = "N/A"

                        If String.IsNullOrEmpty(colourName) Then
                            Dim thisAlert As String = String.Format("HEADRAIL COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colourName & "'")

                        If String.IsNullOrEmpty(colourName) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR HEADRAIL COLOUR FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        rowData = orderClass.GetDataRow("SELECT * FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.VALUE='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "'")
                        If rowData Is Nothing Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE PRODUCT IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        productId = rowData("Id")
                        rowActive = Convert.ToBoolean(rowData("Active"))

                        If rowActive = False Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE PRODUCT IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If qty <> 1 Then
                            Dim thisAlert As String = String.Format("THE ORDER QTY MUS BE 1 PER ITEM LINE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Slat Only" OrElse blindName = "Track Only" Then
                            If String.IsNullOrEmpty(qtyBlade) Then
                                Dim thisAlert As String = String.Format("QTY BLADE IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                            If Not Integer.TryParse(qtyBladeText, qtyBlade) Then
                                Dim thisAlert As String = String.Format("QTY BLADE MUST BE IN NUMERIC FORMAT FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(room) Then
                            Dim thisAlert As String = String.Format("ROOM / LOCATION IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
                            If String.IsNullOrEmpty(mounting) Then
                                Dim thisAlert As String = String.Format("MOUNTING IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            Dim validMounting As String() = {"Opening Size Face Fit", "Opening Size Reveal Fit", "Make Size Face Fit", "Make Size Reveal Fit"}
                            If Not validMounting.Contains(mounting) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE MOUNTING FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
                            If String.IsNullOrEmpty(widthText) Then
                                Dim thisAlert As String = String.Format("WIDTH IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If Not Integer.TryParse(widthText, width) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If width < 300 OrElse width > 6000 Then
                                MessageError(True, "WIDTH MUST BE BETWEEN 300MM - 6000MM !")
                                Exit For
                            End If
                        End If

                        If blindName = "Slat Only" AndAlso Not String.IsNullOrEmpty(widthText) Then
                            Dim thisAlert As String = String.Format("WIDTH IS NOT REQUIRED FOR THIS TYPE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" OrElse blindName = "Slat Only" Then
                            If String.IsNullOrEmpty(dropText) Then
                                Dim thisAlert As String = String.Format("DROP IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If Not Integer.TryParse(dropText, drop) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If drop < 300 OrElse drop > 3050 Then
                                MessageError(True, "DROP MUST BE BETWEEN 300MM - 3050MM !")
                                Exit For
                            End If
                        End If

                        If blindName = "Track Only" AndAlso Not String.IsNullOrEmpty(dropText) Then
                            Dim thisAlert As String = String.Format("DROP IS NOT REQUIRED FOR THIS TYPE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
                            If Not String.IsNullOrEmpty(fabricInsert) Then
                                Dim validFabricInsert As String() = {"No", "Yes"}
                                If Not validFabricInsert.Contains(fabricInsert) Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE FABRIC INSERT FOR ITEM {0} !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If blindName = "Slat Only" AndAlso Not String.IsNullOrEmpty(fabricInsert) Then
                            Dim thisAlert As String = String.Format("FABRIC INSERT IS NOT REQUIRED FOR THIS TYPE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" OrElse blindName = "Slat Only" OrElse (blindName = "Track Only" AndAlso fabricInsert = "Yes") Then
                            If String.IsNullOrEmpty(fabricType) Then
                                Dim thisAlert As String = String.Format("FABRIC TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If fabricType = "Essentials" Then fabricType = fabricType & " " & tubeName
                            If fabricType = "Essence Blockout" Then fabricType = fabricType & " " & tubeName

                            rowData = orderClass.GetDataRow("SELECT * FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND tubeArray.VALUE='" & tubeId & "' AND companyArray.VALUE='" & companyDetailId & "'")
                            If rowData Is Nothing Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC TYPE IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            fabricId = rowData("Id").ToString()
                            rowActive = Convert.ToBoolean(rowData("Active"))

                            If rowActive = False Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC TYPE IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If String.IsNullOrEmpty(fabricColour) Then
                                Dim thisAlert As String = String.Format("FABRIC COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            rowData = orderClass.GetDataRow("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "'")
                            If rowData Is Nothing Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC COLOUR IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            fabricColourId = rowData("Id")
                            rowActive = Convert.ToBoolean(rowData("Active"))

                            If rowActive = False Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE FABRIC COLOUR IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindName = "Track Only" AndAlso Not String.IsNullOrEmpty(fabricType) Then
                            Dim thisAlert As String = String.Format("A FABRIC INSERT IS REQUIRED FOR ITEM {0}, AS YOU HAVE SELECTED A FABRIC TYPE WITHOUT SPECIFYING A FABRIC INSERT !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
                            If String.IsNullOrEmpty(stackPosition) Then
                                Dim thisAlert As String = String.Format("STACK POSITION IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            Dim validStack As String() = {"Left", "Right", "Centre", "Split"}
                            If Not validStack.Contains(stackPosition) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK YOUR STACK POSITION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindName = "Slat Only" AndAlso Not String.IsNullOrEmpty(stackPosition) Then
                            Dim thisAlert As String = String.Format("STACK POSITION IS NOT REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If controlName = "Chain" Then
                            If String.IsNullOrEmpty(controlPosition) Then
                                Dim thisAlert As String = String.Format("CONTROL POSITION IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            Dim validControlPosition As String() = {"Left", "Right"}
                            If Not validControlPosition.Contains(controlPosition) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL POSITION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If
                        If controlName = "Wand" OrElse controlName = "N/A" Then
                            If Not String.IsNullOrEmpty(controlPosition) Then
                                Dim thisAlert As String = String.Format("CONTROL POSITION IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(controlColour) Then
                            If controlName = "Chain" Then controlColour = "Chromed Metal"
                            If controlName = "Wand" Then controlColour = colourName
                        End If

                        If controlName = "N/A" AndAlso Not String.IsNullOrEmpty(controlColour) Then
                            Dim thisAlert As String = String.Format("CONTROL COLOUR IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" Then
                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)
                            If controlName = "Wand" Then
                                If controlLengthValue > 1000 Then controlLengthValue = 1000
                            End If

                            If Not String.IsNullOrEmpty(controlLengthText) AndAlso Not controlLengthText.ToLower().Contains("standard") AndAlso Not controlLengthText.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                controlLengthText = controlLengthText.Replace("mm", "")

                                If Not Integer.TryParse(controlLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL LENGTH FOR ITEM {0} !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If

                                If controlName = "Wand" AndAlso controlLengthValue > 1000 Then
                                    Dim thisAlert As String = String.Format("MAXIMUM CONTROL / WAND LENGTH IS 1000MM FOR ITEM {0}", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If blindName = "Track Only" Then
                            controlLength = "Custom"
                            controlLengthText = controlLengthText.Replace("mm", "")

                            If Not Integer.TryParse(controlLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                MessageError(True, "PLEASE CHECK YOUR CONTROL LENGTH !")
                                Exit For
                            End If

                            If controlName = "Wand" AndAlso controlLengthValue > 1000 Then
                                Dim thisAlert As String = String.Format("MAXIMUM CONTROL / WAND LENGTH IS 1000MM FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindName = "Slat Only" Then
                            If Not String.IsNullOrEmpty(controlLengthText) Then
                                Dim thisAlert As String = String.Format("CONTROL LENGTH IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindName = "Complete Set" OrElse blindName = "Slat Only" Then
                            If String.IsNullOrEmpty(bottomJoining) Then
                                Dim thisAlert As String = String.Format("BOTTOM JOINING IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            Dim validBottomJoining As String() = {"Chainless", "Sewn", "Sewn In", "With Chain"}

                            If Not validBottomJoining.Contains(bottomJoining) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE BOTTOM JOINING FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindName = "Track Only" AndAlso Not String.IsNullOrEmpty(bottomJoining) Then
                            Dim thisAlert As String = String.Format("BOTTOM JOINING IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
                            If Not String.IsNullOrEmpty(bracketExtension) Then
                                Dim validBracketExt As String() = {"No", "Yes"}
                                If Not validBracketExt.Contains(bracketExtension) Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE BRACKET EXTENSION FOR ITEM {0} !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If blindName = "Slat Only" AndAlso Not String.IsNullOrEmpty(bracketExtension) Then
                            Dim thisAlert As String = String.Format("BRACKET EXTENSION IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" AndAlso Not String.IsNullOrEmpty(sloping) Then
                            Dim validSloping As String() = {"No", "Yes"}
                            If Not validSloping.Contains(sloping) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE SLOPING FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If msgError.InnerText = "" Then
                            Dim totalItems As Integer = 1

                            If blindName = "Track Only" Then drop = 0

                            If controlName = "Chain" Then
                                chainId = orderClass.GetItemData("SELECT Id FROM Chains WHERE Name='" & controlColour & "'")
                                controlColour = String.Empty
                            End If

                            If bracketExtension = "No" Then bracketExtension = String.Empty

                            wandLengthValue = 0
                            If controlName = "Wand" Then wandLengthValue = controlLengthValue

                            If controlName = "Wand" Then
                                If stackPosition = "Left" Then controlPosition = "Right"
                                If stackPosition = "Right" Then controlPosition = "Left"
                                If stackPosition = "Centre" Then controlPosition = "Right and Left"
                                If stackPosition = "Split" Then controlPosition = "Middle"
                            End If
                            If Not fabricInsert = "Yes" Then fabricInsert = String.Empty
                            If bottomJoining = "Sewn" Then bottomJoining = "Sewn In"

                            If blindName = "Slat Only" Then
                                If tubeName = "127mm" Then width = qtyBlade * 115 : If qtyBlade < 6 Then width = 472
                                If tubeName = "89mm" Then width = qtyBlade * 79 : If qtyBlade < 5 Then width = 591
                            End If

                            Dim linearMetre As Decimal = width / 1000
                            Dim squareMetre As Decimal = width * drop / 1000000

                            Dim groupName As String = String.Format("Vertical - {0} - {1}", blindName, tubeName)
                            Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)

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
                                    myCmd.Parameters.AddWithValue("@Mounting", mounting)
                                    myCmd.Parameters.AddWithValue("@Width", width)
                                    myCmd.Parameters.AddWithValue("@Drop", drop)
                                    myCmd.Parameters.AddWithValue("@FabricInsert", fabricInsert)
                                    myCmd.Parameters.AddWithValue("@StackPosition", stackPosition)
                                    myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                    myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                    myCmd.Parameters.AddWithValue("@WandColour", controlColour)
                                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)
                                    myCmd.Parameters.AddWithValue("@BottomJoining", bottomJoining)
                                    myCmd.Parameters.AddWithValue("@BracketExtension", bracketExtension)
                                    myCmd.Parameters.AddWithValue("@Sloping", sloping)
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

                    If designName = "Saphora Drape" Then
                        Dim blindName As String = (sheetDetail.Cells(row, 2).Text & "").Trim()
                        Dim tubeName As String = (sheetDetail.Cells(row, 3).Text & "").Trim()
                        Dim controlType As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim colourType As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim qty As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim room As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 9).Text & "").Trim()

                        Dim widthText As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim dropText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim fabricInsert As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim fabricType As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim stackPosition As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim controlPosition As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                        Dim controlColour As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim controlLengthText As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                        Dim bottomJoining As String = (sheetDetail.Cells(row, 19).Text & "").Trim()
                        Dim bracketExtension As String = (sheetDetail.Cells(row, 20).Text & "").Trim()
                        Dim sloping As String = (sheetDetail.Cells(row, 21).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 22).Text & "").Trim()

                        Dim productId As String = String.Empty
                        Dim width As Integer
                        Dim drop As Integer
                        Dim fabricId As String = String.Empty
                        Dim fabricColourId As String = String.Empty

                        Dim chainId As String = String.Empty
                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0
                        Dim wandLengthValue As Integer = 0

                        If String.IsNullOrEmpty(blindName) Then
                            Dim thisAlert As String = String.Format("BLIND TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindName & "' AND CompanyArray.VALUE='" & companyDetailId & "'")

                        If String.IsNullOrEmpty(blindId) Then
                            Dim thisAlert As String = String.Format("THE BLIND TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CHECK AGAIN !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(tubeName) Then
                            Dim thisAlert As String = String.Format("TUBE TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & tubeName & "'")

                        If String.IsNullOrEmpty(tubeId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE TUBE TYPE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Fabric Only" Then controlType = "N/A"

                        If String.IsNullOrEmpty(controlType) Then
                            Dim thisAlert As String = String.Format("CONTROL TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim controlId As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlType & "'")

                        If String.IsNullOrEmpty(controlId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL TYPE  FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Fabric Only" Then colourType = "N/A"

                        If String.IsNullOrEmpty(colourType) Then
                            Dim thisAlert As String = String.Format("HEADRAIL COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colourType & "'")

                        If String.IsNullOrEmpty(colourType) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR HEADRAIL COLOUR FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        rowData = orderClass.GetDataRow("SELECT * FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.VALUE='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "'")
                        If rowData Is Nothing Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE PRODUCT IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        productId = rowData("Id")
                        rowActive = Convert.ToBoolean(rowData("Active"))

                        If rowActive = False Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE PRODUCT IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If qty <> 1 Then
                            Dim thisAlert As String = String.Format("THE ORDER QTY MUS BE 1 PER ITEM LINE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" Then
                            Dim validMounting As String() = {"Opening Size Face Fit", "Make Size Face Fit", "Opening Size Reveal Fit", "Make Size Reveal Fit"}
                            If Not validMounting.Contains(mounting) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE MOUNTING FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If Not Integer.TryParse(widthText, width) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        If width < 300 OrElse width > 6000 Then
                            MessageError(True, "WIDTH MUST BE BETWEEN 300MM - 6000MM !")
                            Exit For
                        End If

                        If Not Integer.TryParse(dropText, drop) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        If drop < 300 OrElse drop > 3050 Then
                            MessageError(True, "DROP MUST BE BETWEEN 300MM - 3050MM !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricType) Then
                            Dim thisAlert As String = String.Format("FABRIC TYPE FOR ITEM {0} IS REQUIRED !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        fabricId = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricId) Then
                            Dim thisAlert As String = String.Format("THE FABRIC TYPE FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricColour) Then
                            Dim thisAlert As String = String.Format("FABRIC COLOUR FOR ITEM {0} IS REQUIRED !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        fabricColourId = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricColourId) Then
                            Dim thisAlert As String = String.Format("THE FABRIC COLOUR FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" Then
                            Dim validStack As String() = {"Left", "Right", "Centre", "Split"}
                            If Not validStack.Contains(stackPosition) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK YOUR STACK POSITION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindName = "Fabric Only" AndAlso Not String.IsNullOrEmpty(stackPosition) Then
                            Dim thisAlert As String = String.Format("STACK POSITION IS NOT REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If controlType = "Wand" OrElse controlType = "N/A" Then
                            If Not String.IsNullOrEmpty(controlPosition) Then
                                Dim thisAlert As String = String.Format("CONTROL POSITION IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(controlColour) Then
                            If controlType = "Chain" Then controlColour = "Chromed Metal"
                            If controlType = "Wand" Then controlColour = colourType
                        End If

                        If controlType = "N/A" AndAlso Not String.IsNullOrEmpty(controlColour) Then
                            Dim thisAlert As String = String.Format("CONTROL COLOUR IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Complete Set" Then
                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)
                            If controlType = "Wand" Then
                                If controlLengthValue > 1000 Then controlLengthValue = 1000
                            End If

                            If Not String.IsNullOrEmpty(controlLengthText) AndAlso Not controlLengthText.ToLower().Contains("standard") AndAlso Not controlLengthText.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                controlLengthText = controlLengthText.Replace("mm", "")

                                If Not Integer.TryParse(controlLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL LENGTH FOR ITEM {0} !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If

                                If controlType = "Wand" AndAlso controlLengthValue > 1000 Then
                                    Dim thisAlert As String = String.Format("MAXIMUM CONTROL / WAND LENGTH IS 1000MM FOR ITEM {0}", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If blindName = "Fabric Only" Then
                            If Not String.IsNullOrEmpty(controlLengthText) Then
                                Dim thisAlert As String = String.Format("CONTROL LENGTH IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindName = "Complete Set" Then
                            If String.IsNullOrEmpty(bracketExtension) Then
                                Dim thisAlert As String = String.Format("BRACKET EXTENSION IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                            Dim validExtension As String() = {"Standard Bracket", "Extension Bracket"}
                            If Not validExtension.Contains(bracketExtension) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE BRACKET EXTENSION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindName = "Fabric Only" AndAlso Not String.IsNullOrEmpty(bracketExtension) Then
                            Dim thisAlert As String = String.Format("BRACKET EXTENSION IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If msgError.InnerText = "" Then
                            Dim totalItems As Integer = 1

                            If bracketExtension = "Standard Bracket" Then bracketExtension = String.Empty
                            If bracketExtension = "Extension Bracket" Then bracketExtension = "Yes"

                            wandLengthValue = 0
                            If controlType = "Wand" Then wandLengthValue = controlLengthValue

                            If controlType = "Wand" Then
                                If stackPosition = "Left" Then controlPosition = "Right"
                                If stackPosition = "Right" Then controlPosition = "Left"
                                If stackPosition = "Centre" Then controlPosition = "Right and Left"
                                If stackPosition = "Split" Then controlPosition = "Middle"
                            End If

                            Dim linearMetre As Decimal = width / 1000
                            Dim squareMetre As Decimal = width * drop / 1000000

                            Dim groupName As String = String.Format("{0} - {1}", designName, blindName)
                            Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)

                            Dim itemId As String = orderClass.GetNewOrderItemId()

                            Using thisConn As SqlConnection = New SqlConnection(myConn)
                                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, ChainId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], StackPosition, ControlPosition, ControlLength, ControlLengthValue, WandColour, WandLengthValue, BracketExtension, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @ChainId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @StackPosition, @ControlPosition, @ControlLength, @ControlLengthValue, @WandColour, @WandLengthValue, @BracketExtension, @LinearMetre, @SquareMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                                    myCmd.Parameters.AddWithValue("@Id", itemId)
                                    myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                    myCmd.Parameters.AddWithValue("@ProductId", productId)
                                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                    myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                    myCmd.Parameters.AddWithValue("@Qty", "1")
                                    myCmd.Parameters.AddWithValue("@Room", room)
                                    myCmd.Parameters.AddWithValue("@Mounting", mounting)
                                    myCmd.Parameters.AddWithValue("@Width", width)
                                    myCmd.Parameters.AddWithValue("@Drop", drop)
                                    myCmd.Parameters.AddWithValue("@StackPosition", stackPosition)
                                    myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                    myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                    myCmd.Parameters.AddWithValue("@WandColour", controlColour)
                                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)
                                    myCmd.Parameters.AddWithValue("@BracketExtension", bracketExtension)
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

                    If designName = "Venetian Blind" Then
                        designId = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='" & designName & "'")
                        If String.IsNullOrEmpty(designId) Then
                            Dim thisAlert As String = String.Format("THE DESIGN TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindName As String = (sheetDetail.Cells(row, 2).Text & "").Trim()

                        If String.IsNullOrEmpty(blindName) Then
                            Dim thisAlert As String = String.Format("BLIND TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindName & "' AND CompanyArray.VALUE='" & companyDetailId & "'")

                        If String.IsNullOrEmpty(blindId) Then
                            Dim thisAlert As String = String.Format("THE BLIND TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CHECK AGAIN !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim subType As String = "Single"
                        Dim qty As String = (sheetDetail.Cells(row, 3).Text & "").Trim()
                        Dim room As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim colour As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim controlPosition As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                        Dim tilterPosition As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim widthText As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim dropText As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim cordLengthText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim wandLengthText As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim supply As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim tassel As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim valanceType As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim valanceSizeText As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                        Dim returnPosition As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim returnLengthText As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 19).Text & "").Trim()

                        Dim width As Integer
                        Dim drop As Integer

                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0

                        Dim valanceSize As String = String.Empty
                        Dim valanceSizeValue As Integer = 0

                        Dim returnLength As String = String.Empty
                        Dim returnLengthValue As Integer = 0

                        If qty <> 1 Then
                            Dim thisAlert As String = String.Format("THE ORDER QTY MUS BE 1 PER ITEM LINE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(room) Then
                            Dim thisAlert As String = String.Format("ROOM / LOCATION IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(mounting) Then
                            Dim thisAlert As String = String.Format("MOUNTING IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validMounting As String() = {"Make Size Face Fit", "Make Size Reveal Fit", "Opening Size Face Fit", "Opening Size Reveal Fit"}

                        If Not validMounting.Contains(mounting) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE MOUNTING FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(colour) Then
                            Dim thisAlert As String = String.Format("COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim tubeId As String = "9" : Dim controlId As String = "17"
                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colour & "'")
                        If String.IsNullOrEmpty(colourId) Then
                            Dim thisAlert As String = String.Format("THE COLOUR YOU REQUESTED IS CURRENTLY NOT AVAILABLE IN OUR STOCKS. PLEASE REVIEW YOUR ORDER OR CONTACT OUR CUSTOMER SERVICE TEAM FOR ASSISTANCE. ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.value='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "' AND Active=1")
                        If String.IsNullOrEmpty(productId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT YOU REQUESTED IS CURRENTLY NOT AVAILABLE IN OUR STOCKS. PLEASE REVIEW YOUR ORDER OR CONTACT OUR CUSTOMER SERVICE TEAM FOR ASSISTANCE. ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(controlPosition) Then
                            Dim thisAlert As String = String.Format("CONTROL POSITION IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validCP As String() = {"Left", "Right", "No Control"}
                        If Not validCP.Contains(controlPosition) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL POSITION VALUE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(tilterPosition) Then
                            Dim thisAlert As String = String.Format("TILTER POSITION IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validTP As String() = {"Left", "Right", "Center", "Centre"}
                        If Not validTP.Contains(tilterPosition) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR TILTER POSITION VALUE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(widthText) Then
                            Dim thisAlert As String = String.Format("WIDTH IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If Not Integer.TryParse(widthText, width) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If width < 250 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH ORDER FOR ITEM {0} & MINIMUM WIDTH IS 250MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If width > 2710 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH ORDER FOR ITEM {0} & MAXIMUM WIDTH IS 2710MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If width > 300 AndAlso width <= 400 AndAlso controlPosition = tilterPosition Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE CONTROL & TILTER POSITION FOR ITEM {0} & PLEASE USE OPPOSITE CONTROL AND TILTER POSITIONS !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If width < 300 Then
                            If controlPosition = "No Control" Then
                                Dim thisAlert As String = String.Format("YOUR WIDTH FOR ITEM {0} UNDER 250MM. PLEASE CHANGE PULL CORD POSITION TO NO CONTROL !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If tilterPosition <> "Center" OrElse tilterPosition <> "Centre" Then
                                Dim thisAlert As String = String.Format("YOUR WIDTH FOR ITEM {0} UNDER 250MM. PLEASE CHANGE TILTER POSITION TO CENTRE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(dropText) Then
                            Dim thisAlert As String = String.Format("DROP IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If Not Integer.TryParse(dropText, drop) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If drop < 200 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP ORDER FOR ITEM {0} & MINIMUM DROP IS 200MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If drop > 3200 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP ORDER FOR ITEM {0} & MAXIMUM DROP IS 3200MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        controlLength = "Standard"
                        controlLengthValue = Math.Ceiling(drop * 2 / 3)
                        If controlLengthValue < 450 Then controlLengthValue = 550

                        If Not String.IsNullOrEmpty(cordLengthText) AndAlso Not cordLengthText.ToLower().Contains("standard") AndAlso Not cordLengthText.ToLower().Contains("std") Then
                            controlLength = "Custom"
                            cordLengthText = cordLengthText.Replace("mm", "")
                            If Not Integer.TryParse(cordLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE CORD LENGTH FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If Not String.IsNullOrEmpty(supply) Then
                            Dim validSupply As String() = {"No", "Yes"}
                            If Not validSupply.Contains(supply) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE HOLD DOWN CLIP FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(tassel) Then
                            Dim thisAlert As String = String.Format("METAL TASSEL OPTION IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        Dim validTassel As String() = {"Plastic", "Gold", "Antique Brass"}
                        If Not validTassel.Contains(tassel) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE METAL TASSEL OPTION FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(valanceType) Then
                            Dim thisAlert As String = String.Format("VALANCE TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" Then
                            If valanceType <> "75mm Valance" Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE VALANCE TYPE FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" OrElse blindName = "Ultraslat 50mm" OrElse blindName = "Ultraslat 63mm" Then
                            If valanceType <> "76mm Valance" Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE VALANCE TYPE FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        valanceSize = "Standard"
                        If mounting = "Opening Size Reveal Fit" Then valanceSizeValue = width - 1
                        If mounting = "Make Size Reveal Fit" Then valanceSizeValue = width + 9
                        If mounting = "Opening Size Face Fit" Then valanceSizeValue = width + 20
                        If mounting = "Make Size Face Fit" Then valanceSizeValue = width + 20

                        If Not String.IsNullOrEmpty(valanceSizeText) AndAlso Not valanceSizeText.ToLower().Contains("standard") AndAlso Not valanceSizeText.ToLower().Contains("std") Then
                            valanceSize = "Custom"
                            valanceSizeText = valanceSizeText.Replace("mm", "")
                            If Not Integer.TryParse(valanceSizeText, valanceSizeValue) OrElse valanceSizeValue < 0 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE VALANCE SIZE VALUE FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(returnPosition) Then returnPosition = "None"
                        If Not String.IsNullOrEmpty(returnPosition) Then
                            Dim validRP As String() = {"None", "Left", "Right", "Both Sides"}
                            If Not validRP.Contains(returnPosition) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE VALANCE RETURN POSITION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If returnPosition <> "None" Then
                                returnLength = "Standard"
                                If mounting = "Opening Size Face Fit" OrElse mounting = "Make Size Face Fit" Then
                                    returnLengthValue = 70
                                    If blindName = "Econo 50" OrElse blindName = "Econo 63mm" OrElse blindName = "Ultraslat 50mm" OrElse blindName = "Ultraslat 63mm" Then
                                        returnLengthValue = 77
                                    End If
                                End If
                                If mounting = "Opening Size Reveal Fit" OrElse mounting = "Make Size Reveal Fit" Then
                                    returnLengthValue = 20
                                End If

                                If Not String.IsNullOrEmpty(returnLengthText) AndAlso Not returnLengthText.ToLower().Contains("standard") AndAlso Not returnLengthText.ToLower().Contains("std") Then
                                    returnLength = "Custom"
                                    returnLengthText = returnLengthText.Replace("mm", "")

                                    If Not Integer.TryParse(returnLengthText, returnLengthValue) OrElse returnLengthValue < 0 Then
                                        Dim thisAlert As String = String.Format("PLEASE CHECK YOUR VALANCE RETURN LENGTH !", itemNumber)
                                        MessageError(True, thisAlert)
                                        Exit For
                                    End If
                                End If
                            End If
                        End If

                        If msgError.InnerText = "" Then
                            Dim itemId As String = orderClass.GetNewOrderItemId()

                            If returnPosition = "None" Then returnPosition = String.Empty

                            Dim groupName As String = blindName
                            If blindName = "Ultraslat 50mm" Then groupName = "Econo 50mm"
                            If blindName = "Ultraslat 63mm" Then groupName = "Econo 63mm"

                            Dim productgroupName As String = String.Format("{0} - {1}", designName, groupName)

                            Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(productgroupName, designId, companyDetailId)

                            Dim linearMetre As Decimal = width / 10000
                            Dim squareMetre As Decimal = width * drop / 1000000

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

                    If designName = "Aluminium Blind" Then
                        designId = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='" & designName & "'")
                        If String.IsNullOrEmpty(designId) Then
                            Dim thisAlert As String = String.Format("THE DESIGN TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindName As String = (sheetDetail.Cells(row, 2).Text & "").Trim()

                        If String.IsNullOrEmpty(blindName) Then
                            Dim thisAlert As String = String.Format("BLIND TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindName & "' AND CompanyArray.VALUE='" & companyDetailId & "'")

                        If String.IsNullOrEmpty(blindId) Then
                            Dim thisAlert As String = String.Format("THE BLIND TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CHECK AGAIN !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim subType As String = "Single"
                        Dim qty As String = (sheetDetail.Cells(row, 3).Text & "").Trim()
                        Dim room As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim colour As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim controlPosition As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                        Dim tilterPosition As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim widthText As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim dropText As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim cordLengthText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim wandLengthText As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim supply As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 19).Text & "").Trim()


                        Dim productId As String = String.Empty
                        Dim width As Integer
                        Dim drop As Integer
                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0

                        Dim wandLength As String = String.Empty
                        Dim wandLengthValue As Integer = 0

                        If String.IsNullOrEmpty(qty) Then
                            Dim thisAlert As String = String.Format("QTY IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If qty <> "1" Then
                            Dim thisAlert As String = String.Format("THE ORDER QTY MUS BE 1 PER ITEM LINE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(room) Then
                            Dim thisAlert As String = String.Format("ROOM / LOCATION IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(mounting) Then
                            Dim thisAlert As String = String.Format("MOUNTING IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validMounting As String() = {"Make Size Face Fit", "Make Size Reveal Fit", "Opening Size Face Fit", "Opening Size Reveal Fit"}

                        If Not validMounting.Contains(mounting) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE MOUNTING FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(colour) Then
                            Dim thisAlert As String = String.Format("COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim tubeId As String = "9" : Dim controlId As String = "17"
                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colour & "'")
                        If String.IsNullOrEmpty(colourId) Then
                            Dim thisAlert As String = String.Format("THE COLOUR YOU REQUESTED IS CURRENTLY NOT AVAILABLE IN OUR STOCKS. PLEASE REVIEW YOUR ORDER OR CONTACT OUR CUSTOMER SERVICE TEAM FOR ASSISTANCE. ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        rowData = orderClass.GetDataRow("SELECT * FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.value='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "'")
                        If rowData Is Nothing Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE PRODUCT IS NOT REGISTERED IN OUR SYSTEM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        productId = rowData("Id").ToString()
                        rowActive = Convert.ToBoolean(rowData("Active"))

                        If rowActive = False Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK ITEM {0}. THE PRODUCT IS CURRENTLY NOT AVAILABLE IN OUR STOCK !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(controlPosition) Then
                            Dim thisAlert As String = String.Format("CONTROL POSITION IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validCP As String() = {"Left", "Right", "No Control"}
                        If Not validCP.Contains(controlPosition) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL POSITION VALUE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(tilterPosition) Then
                            Dim thisAlert As String = String.Format("TILTER POSITION IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validTP As String() = {"Left", "Right", "Center", "Centre"}
                        If Not validTP.Contains(tilterPosition) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR TILTER POSITION VALUE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(widthText) Then
                            Dim thisAlert As String = String.Format("WIDTH IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If Not Integer.TryParse(widthText, width) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If width < 200 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} & MINIMUM WIDTH IS 200MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If width > 3010 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} & MAXIMUM WIDTH IS 3010MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If width > 250 AndAlso width <= 299 AndAlso controlPosition = tilterPosition Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE CONTROL & TILTER POSITION FOR ITEM {0} & PLEASE USE OPPOSITE CONTROL AND TILTER POSITIONS !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If width < 250 Then
                            If controlPosition = "No Control" Then
                                Dim thisAlert As String = String.Format("YOUR WIDTH FOR ITEM {0} UNDER 250MM. PLEASE CHANGE PULL CORD POSITION TO NO CONTROL !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If tilterPosition <> "Center" OrElse tilterPosition <> "Centre" Then
                                Dim thisAlert As String = String.Format("YOUR WIDTH FOR ITEM {0} UNDER 250MM. PLEASE CHANGE TILTER POSITION TO CENTRE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(dropText) Then
                            Dim thisAlert As String = String.Format("DROP IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If Not Integer.TryParse(dropText, drop) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If drop < 250 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} & MINIMUM DROP IS 250MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If drop > 3200 Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} & MAXIMUM DROP IS 3200MM !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        controlLength = "Standard"
                        controlLengthValue = Math.Ceiling(drop * 2 / 3)
                        If controlLengthValue < 450 Then controlLengthValue = 450

                        If Not String.IsNullOrEmpty(cordLengthText) AndAlso Not cordLengthText = "Standard" Then
                            controlLength = "Custom"
                            cordLengthText = cordLengthText.Replace("mm", "")
                            If Not Integer.TryParse(cordLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE CORD LENGTH FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        wandLength = "Standard"
                        wandLengthValue = Math.Ceiling(drop * 2 / 3)
                        If wandLengthValue < 450 Then wandLengthValue = 450

                        If Not String.IsNullOrEmpty(wandLengthText) AndAlso Not wandLengthText = "Standard" Then
                            wandLength = "Custom"
                            wandLengthText = wandLengthText.Replace("mm", "")
                            If Not Integer.TryParse(wandLengthText, wandLengthValue) OrElse wandLengthValue < 0 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE WAND LENGTH FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If Not String.IsNullOrEmpty(supply) Then
                            Dim validSupply As String() = {"No", "Yes"}
                            If Not validSupply.Contains(supply) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE HOLD DOWN CLIP FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If


                        If msgError.InnerText = "" Then
                            Dim itemId As String = orderClass.GetNewOrderItemId()
                            Dim priceProductGroupId As String = orderClass.GetPriceProductGroupId(blindName, designId, companyDetailId)

                            Dim linearMetre As Decimal = width / 10000
                            Dim squareMetre As Decimal = width * drop / 1000000

                            Using thisConn As SqlConnection = New SqlConnection(myConn)
                                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails (Id, HeaderId, ProductId, PriceProductGroupId, SubType, Qty, Room, Mounting, ControlPosition, TilterPosition, Width, [Drop], Supply, ControlLength, ControlLengthValue, WandLength, WandLengthValue, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, @SubType, 1, @Room, @Mounting, @ControlPosition, @TilterPosition, @Width, @Drop, @Supply, @ControlLength, @ControlLengthValue, @WandLength, @WandLengthValue, @LinearMetre, @SquareMetre, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                                    myCmd.Parameters.AddWithValue("@Id", itemId)
                                    myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                    myCmd.Parameters.AddWithValue("@ProductId", productId)
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroupId), CType(DBNull.Value, Object), priceProductGroupId))
                                    myCmd.Parameters.AddWithValue("@Room", room)
                                    myCmd.Parameters.AddWithValue("@Mounting", mounting)
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

    Protected Sub ReadExcelData(filePath As String)
        Using package As New ExcelPackage(New FileInfo(filePath))
            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)

            Dim customerData As DataRow = orderClass.GetDataRow("SELECT Customers.CompanyDetailId AS CompanyDetailId, Companys.Alias AS CompanyAlias FROM Customers LEFT JOIN Companys ON Customers.CompanyId=Companys.Id WHERE Customers.Id='" & ddlCustomer.SelectedValue & "'")
            Dim companyAlias As String = customerData("CompanyAlias")
            Dim companyDetailId As String = customerData("CompanyDetailId")

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
            Dim maxRetry As Integer = 100
            Dim orderId As String = ""

            Do While Not success
                retry += 1
                If retry > maxRetry Then Throw New Exception("FAILED TO GENERATE UNIQUE ORDER ID")

                Dim randomCode As String = orderClass.GenerateRandomCode()
                orderId = companyAlias & randomCode
                Try
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, DownloadBOE, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, 'Regular', 'Unsubmitted', @CreatedBy, GETDATE(), 0, 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
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

            Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Order/{0}/", orderId))
            If Not Directory.Exists(directoryOrder) Then
                Directory.CreateDirectory(directoryOrder)
            End If

            Dim fileName As String = Path.GetFileName(filePath)
            Dim newPath As String = Path.Combine(directoryOrder, fileName)
            File.Copy(filePath, newPath, True)

            Dim dataLog As Object() = {"OrderHeaders", headerId, Session("LoginId").ToString(), "Order Created | CSV"}
            orderClass.Logs(dataLog)

            Using orderItem As New ExcelPackage(New FileInfo(filePath))
                Dim sheetDetail As ExcelWorksheet = orderItem.Workbook.Worksheets(0)
                Dim startRow As Integer = 3
                Dim lastRow As Integer = sheetDetail.Dimension.End.Row

                For row As Integer = startRow To lastRow
                    Dim designType As String = If(sheetDetail.Cells(row, 1).Text IsNot Nothing, sheetDetail.Cells(row, 1).Text, "")

                    If designType = "Vertical" Then
                        Dim itemNumber As String = row - 3

                        Dim designId As String = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='" & designType & "'")

                        Dim blindType As String = If(sheetDetail.Cells(row, 2).Text IsNot Nothing, sheetDetail.Cells(row, 2).Text, "")
                        Dim tubeType As String = (sheetDetail.Cells(row, 3).Text & "").Trim()
                        Dim controlType As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim colourType As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 6).Text), 0, CInt(sheetDetail.Cells(row, 6).Text))

                        Dim qtyBladeText As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                        Dim qtyBlade As Integer

                        Dim room As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim sizeType As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 10).Text & "").Trim()

                        Dim widthText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim width As Integer

                        Dim dropText As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim drop As Integer

                        Dim fabricInsert As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim fabricType As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim stackPosition As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                        Dim controlPosition As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim controlColour As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                        Dim controlLengthText As String = (sheetDetail.Cells(row, 19).Text & "").Trim()
                        Dim bottomJoining As String = (sheetDetail.Cells(row, 20).Text & "").Trim()
                        Dim bracketExtension As String = (sheetDetail.Cells(row, 21).Text & "").Trim()
                        Dim sloping As String = (sheetDetail.Cells(row, 22).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 23).Text & "").Trim()

                        Dim fabricId As String = String.Empty
                        Dim fabricColourId As String = String.Empty

                        Dim chainId As String = String.Empty
                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0
                        Dim wandLengthValue As Integer = 0

                        If String.IsNullOrEmpty(designId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT IN ITEM {0} IS NOT REGISTERED. PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindType & "' AND CompanyArray.VALUE='" & companyDetailId & "'")
                        If String.IsNullOrEmpty(blindId) Then
                            Dim thisAlert As String = String.Format("THE ORDER TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CHECK AGAIN. !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim tubeName As String = tubeType
                        If tubeType = "Wide Blade (127mm)" Then tubeName = "127mm"
                        If tubeType = "Narrow Blade (89mm)" Then tubeName = "89mm"
                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & tubeName & "'")
                        If String.IsNullOrEmpty(tubeId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE SLAT SIZE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Slat Only" Then controlType = "N/A"
                        Dim controlId As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlType & "'")
                        If String.IsNullOrEmpty(controlId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL TYPE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Slat Only" Then colourType = "N/A"
                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colourType & "'")
                        If String.IsNullOrEmpty(colourType) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR HEADRAIL COLOUR FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.VALUE='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "' AND Active=1")
                        If String.IsNullOrEmpty(productId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT YOU REQUESTED IS CURRENTLY NOT AVAILABLE IN OUR STOCKS. PLEASE REVIEW YOUR ORDER OR CONTACT OUR CUSTOMER SERVICE TEAM FOR ASSISTANCE. ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If qty <> 1 Then
                            Dim thisAlert As String = String.Format("THE ORDER QTY MUS BE 1 PER ITEM LINE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Slat Only" OrElse blindType = "Track Only" Then
                            If String.IsNullOrEmpty(qtyBlade) Then
                                Dim thisAlert As String = String.Format("QTY BLADE FOR ITEM {0} IS REQUIRED !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                            If Not Integer.TryParse(qtyBladeText, qtyBlade) Then
                                Dim thisAlert As String = String.Format("QTY BLADE FOR ITEM {0} MUST BE IN NUMERIC FORMAT !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(room) Then
                            Dim thisAlert As String = String.Format("ROOM / LOCATION IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" OrElse blindType = "Track Only" Then
                            If String.IsNullOrEmpty(sizeType) Then
                                Dim thisAlert As String = String.Format("SIZE TYPE IS REQUIRED FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            Dim validSizeType As String() = {"Opening Size", "Make Size"}
                            If Not validSizeType.Contains(sizeType) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE SIZE TYPE FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If String.IsNullOrEmpty(mounting) Then
                                Dim thisAlert As String = String.Format("MOUNTING IS REQUIRED FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Complete Set" OrElse blindType = "Track Only" Then
                            Dim validMounting As String() = {"Face Fit", "Reveal Fit"}
                            If Not validMounting.Contains(mounting) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE MOUNTING FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Complete Set" OrElse blindType = "Track Only" Then
                            If Not Integer.TryParse(widthText, width) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                            If width < 300 OrElse width > 6000 Then
                                MessageError(True, "WIDTH MUST BE BETWEEN 300MM - 6000MM !")
                                Exit For
                            End If
                        End If

                        If blindType = "Slat Only" AndAlso Not String.IsNullOrEmpty(widthText) Then
                            Dim thisAlert As String = String.Format("WIDTH IS NOT REQUIRED FOR THIS TYPE. PLEASE CHECK ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" OrElse blindType = "Slat Only" Then
                            If Not Integer.TryParse(dropText, drop) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                            If drop < 300 OrElse drop > 3050 Then
                                MessageError(True, "DROP MUST BE BETWEEN 300MM - 3050MM !")
                                Exit For
                            End If
                        End If

                        If blindType = "Track Only" AndAlso Not String.IsNullOrEmpty(dropText) Then
                            Dim thisAlert As String = String.Format("DROP IS NOT REQUIRED FOR THIS TYPE. PLEASE CHECK ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" OrElse blindType = "Track Only" Then
                            If Not String.IsNullOrEmpty(fabricInsert) Then
                                Dim validFabricInsert As String() = {"No", "Yes"}
                                If Not validFabricInsert.Contains(fabricInsert) Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE FABRIC INSERT FOR ITEM {0}", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If blindType = "Slat Only" AndAlso Not String.IsNullOrEmpty(fabricInsert) Then
                            Dim thisAlert As String = String.Format("FABRIC INSERT IS NOT REQUIRED FOR THIS TYPE. PLEASE CHECK ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" OrElse blindType = "Slat Only" OrElse (blindType = "Track Only" AndAlso fabricInsert = "Yes") Then
                            If String.IsNullOrEmpty(fabricType) Then
                                Dim thisAlert As String = String.Format("FABRIC TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If fabricType = "Essentials" Then fabricType = fabricType & " " & tubeName
                            If fabricType = "Essence Blockout" Then fabricType = fabricType & " " & tubeName

                            fabricId = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND tubeArray.VALUE='" & tubeId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricId) Then
                                Dim thisAlert As String = String.Format("THE FABRIC TYPE FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If String.IsNullOrEmpty(fabricColour) Then
                                Dim thisAlert As String = String.Format("FABRIC COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            fabricColourId = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricColourId) Then
                                Dim thisAlert As String = String.Format("THE FABRIC COLOUR FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Track Only" AndAlso Not String.IsNullOrEmpty(fabricType) Then
                            Dim thisAlert As String = String.Format("A FABRIC INSERT IS REQUIRED FOR ITEM {0}, AS YOU HAVE SELECTED A FABRIC TYPE WITHOUT SPECIFYING A FABRIC INSERT.", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" OrElse blindType = "Track Only" Then
                            Dim validStack As String() = {"Left", "Right", "Centre", "Split"}
                            If Not validStack.Contains(stackPosition) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK YOUR STACK POSITION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Slat Only" AndAlso Not String.IsNullOrEmpty(stackPosition) Then
                            Dim thisAlert As String = String.Format("STACK POSITION IS NOT REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If controlType = "Chain" Then
                            If String.IsNullOrEmpty(controlPosition) Then
                                Dim thisAlert As String = String.Format("CONTROL POSITION IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            Dim validControlPosition As String() = {"Left", "Right"}
                            If Not validControlPosition.Contains(controlPosition) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL POSITION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If
                        If controlType = "Wand" OrElse controlType = "N/A" Then
                            If Not String.IsNullOrEmpty(controlPosition) Then
                                Dim thisAlert As String = String.Format("CONTROL POSITION IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(controlColour) Then
                            If controlType = "Chain" Then controlColour = "Chromed Metal"
                            If controlType = "Wand" Then controlColour = colourType
                        End If

                        If controlType = "N/A" AndAlso Not String.IsNullOrEmpty(controlColour) Then
                            Dim thisAlert As String = String.Format("CONTROL COLOUR IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" Then
                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)
                            If controlType = "Wand" Then
                                If controlLengthValue > 1000 Then controlLengthValue = 1000
                            End If

                            If Not String.IsNullOrEmpty(controlLengthText) AndAlso Not controlLengthText.ToLower().Contains("standard") AndAlso Not controlLengthText.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                controlLengthText = controlLengthText.Replace("mm", "")

                                If Not Integer.TryParse(controlLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL LENGTH FOR ITEM {0} !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If

                                If controlType = "Wand" AndAlso controlLengthValue > 1000 Then
                                    Dim thisAlert As String = String.Format("MAXIMUM CONTROL / WAND LENGTH IS 1000MM FOR ITEM {0}", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If blindType = "Track Only" Then
                            controlLength = "Custom"
                            controlLengthText = controlLengthText.Replace("mm", "")

                            If Not Integer.TryParse(controlLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                MessageError(True, "PLEASE CHECK YOUR CONTROL LENGTH !")
                                Exit For
                            End If

                            If controlType = "Wand" AndAlso controlLengthValue > 1000 Then
                                Dim thisAlert As String = String.Format("MAXIMUM CONTROL / WAND LENGTH IS 1000MM FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Slat Only" Then
                            If Not String.IsNullOrEmpty(controlLengthText) Then
                                Dim thisAlert As String = String.Format("CONTROL LENGTH IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Complete Set" OrElse blindType = "Slat Only" Then
                            If String.IsNullOrEmpty(bottomJoining) Then
                                MessageError(True, "BOTTOM JOINING IS REQUIRED !")
                                Exit For
                            End If

                            Dim validBottomJoining As String() = {"Chainless", "Sewn", "Sewn In", "With Chain"}
                            If Not validBottomJoining.Contains(bottomJoining) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE BOTTOM JOINING FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Track Only" AndAlso Not String.IsNullOrEmpty(bottomJoining) Then
                            Dim thisAlert As String = String.Format("BOTTOM JOINING IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" OrElse blindType = "Track Only" Then
                            If String.IsNullOrEmpty(bracketExtension) Then
                                Dim thisAlert As String = String.Format("BRACKET EXTENSION IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                            Dim validExtension As String() = {"Standard Bracket", "Extension Bracket"}
                            If Not validExtension.Contains(bracketExtension) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE BRACKET EXTENSION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Slat Only" AndAlso Not String.IsNullOrEmpty(bracketExtension) Then
                            Dim thisAlert As String = String.Format("BRACKET EXTENSION IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" AndAlso Not String.IsNullOrEmpty(sloping) Then
                            Dim validSloping As String() = {"No", "Yes"}
                            If Not validSloping.Contains(sloping) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE SLOPING FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If msgError.InnerText = "" Then
                            Dim totalItems As Integer = 1

                            If blindType = "Track Only" Then drop = 0

                            If controlType = "Chain" Then
                                chainId = orderClass.GetItemData("SELECT Id FROM Chains WHERE Name='" & controlColour & "'")
                                controlColour = String.Empty
                            End If

                            If bracketExtension = "Standard Bracket" Then bracketExtension = String.Empty
                            If bracketExtension = "Extension Bracket" Then bracketExtension = "Yes"

                            wandLengthValue = 0
                            If controlType = "Wand" Then wandLengthValue = controlLengthValue

                            If controlType = "Wand" Then
                                If stackPosition = "Left" Then controlPosition = "Right"
                                If stackPosition = "Right" Then controlPosition = "Left"
                                If stackPosition = "Centre" Then controlPosition = "Right and Left"
                                If stackPosition = "Split" Then controlPosition = "Middle"
                            End If
                            If Not fabricInsert = "Yes" Then fabricInsert = String.Empty
                            If bottomJoining = "Sewn" Then bottomJoining = "Sewn In"

                            If blindType = "Slat Only" Then
                                If tubeName = "127mm" Then width = qtyBlade * 115 : If qtyBlade < 6 Then width = 472
                                If tubeName = "89mm" Then width = qtyBlade * 79 : If qtyBlade < 5 Then width = 591
                            End If

                            Dim linearMetre As Decimal = width / 1000
                            Dim squareMetre As Decimal = width * drop / 1000000

                            Dim groupName As String = String.Format("Vertical - {0} - {1}", blindType, tubeName)
                            Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)

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
                                    myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                    myCmd.Parameters.AddWithValue("@WandColour", controlColour)
                                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)
                                    myCmd.Parameters.AddWithValue("@BottomJoining", bottomJoining)
                                    myCmd.Parameters.AddWithValue("@BracketExtension", bracketExtension)
                                    myCmd.Parameters.AddWithValue("@Sloping", sloping)
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

                    If designType = "Saphora Drape" Then
                        Dim itemNumber As String = row - 3

                        Dim designId As String = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='" & designType & "'")

                        Dim blindType As String = If(sheetDetail.Cells(row, 2).Text IsNot Nothing, sheetDetail.Cells(row, 2).Text, "")
                        Dim tubeType As String = (sheetDetail.Cells(row, 3).Text & "").Trim()
                        Dim controlType As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim colourType As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 6).Text), 0, CInt(sheetDetail.Cells(row, 6).Text))
                        Dim qtyBlade As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 7).Text), 0, CInt(sheetDetail.Cells(row, 7).Text))
                        Dim room As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim sizeType As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 10).Text & "").Trim()

                        Dim widthText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim width As Integer

                        Dim dropText As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim drop As Integer

                        Dim fabricInsert As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim fabricType As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim stackPosition As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                        Dim controlPosition As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim controlColour As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                        Dim controlLengthText As String = (sheetDetail.Cells(row, 19).Text & "").Trim()
                        Dim bottomJoining As String = (sheetDetail.Cells(row, 20).Text & "").Trim()
                        Dim bracketExtension As String = (sheetDetail.Cells(row, 21).Text & "").Trim()
                        Dim sloping As String = (sheetDetail.Cells(row, 22).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 23).Text & "").Trim()

                        Dim fabricId As String = String.Empty
                        Dim fabricColourId As String = String.Empty

                        Dim chainId As String = String.Empty
                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0
                        Dim wandLengthValue As Integer = 0

                        If String.IsNullOrEmpty(designId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT IN ITEM {0} IS NOT REGISTERED. PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindType & "' AND CompanyArray.VALUE='" & companyDetailId & "'")
                        If String.IsNullOrEmpty(blindId) Then
                            Dim thisAlert As String = String.Format("THE ORDER TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CHECK AGAIN. !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & tubeType & "'")
                        If String.IsNullOrEmpty(tubeId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE SLAT SIZE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Fabric Only" Then controlType = "N/A"
                        Dim controlId As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlType & "'")
                        If String.IsNullOrEmpty(controlId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL TYPE  FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Fabric Only" Then colourType = "N/A"
                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colourType & "'")
                        If String.IsNullOrEmpty(colourType) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR HEADRAIL COLOUR FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.VALUE='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "' AND Active=1")
                        If String.IsNullOrEmpty(productId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT YOU REQUESTED IS CURRENTLY NOT AVAILABLE IN OUR STOCKS. PLEASE REVIEW YOUR ORDER OR CONTACT OUR CUSTOMER SERVICE TEAM FOR ASSISTANCE. ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If qty <> 1 Then
                            Dim thisAlert As String = String.Format("THE ORDER QTY MUS BE 1 PER ITEM LINE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" Then
                            Dim validSizeType As String() = {"Opening Size", "Make Size"}
                            If Not validSizeType.Contains(sizeType) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE SIZE TYPE FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            Dim validMounting As String() = {"Face Fit", "Reveal Fit"}
                            If Not validMounting.Contains(mounting) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE MOUNTING FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If Not Integer.TryParse(widthText, width) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        If width < 300 OrElse width > 6000 Then
                            MessageError(True, "WIDTH MUST BE BETWEEN 300MM - 6000MM !")
                            Exit For
                        End If

                        If Not Integer.TryParse(dropText, drop) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        If drop < 300 OrElse drop > 3050 Then
                            MessageError(True, "DROP MUST BE BETWEEN 300MM - 3050MM !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricType) Then
                            Dim thisAlert As String = String.Format("FABRIC TYPE FOR ITEM {0} IS REQUIRED !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        fabricId = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricId) Then
                            Dim thisAlert As String = String.Format("THE FABRIC TYPE FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricColour) Then
                            Dim thisAlert As String = String.Format("FABRIC COLOUR FOR ITEM {0} IS REQUIRED !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        fabricColourId = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricColourId) Then
                            Dim thisAlert As String = String.Format("THE FABRIC COLOUR FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" Then
                            Dim validStack As String() = {"Left", "Right", "Centre", "Split"}
                            If Not validStack.Contains(stackPosition) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK YOUR STACK POSITION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Fabric Only" AndAlso Not String.IsNullOrEmpty(stackPosition) Then
                            Dim thisAlert As String = String.Format("STACK POSITION IS NOT REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If controlType = "Wand" OrElse controlType = "N/A" Then
                            If Not String.IsNullOrEmpty(controlPosition) Then
                                Dim thisAlert As String = String.Format("CONTROL POSITION IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(controlColour) Then
                            If controlType = "Chain" Then controlColour = "Chromed Metal"
                            If controlType = "Wand" Then controlColour = colourType
                        End If

                        If controlType = "N/A" AndAlso Not String.IsNullOrEmpty(controlColour) Then
                            Dim thisAlert As String = String.Format("CONTROL COLOUR IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If blindType = "Complete Set" Then
                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)
                            If controlType = "Wand" Then
                                If controlLengthValue > 1000 Then controlLengthValue = 1000
                            End If

                            If Not String.IsNullOrEmpty(controlLengthText) AndAlso Not controlLengthText.ToLower().Contains("standard") AndAlso Not controlLengthText.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                controlLengthText = controlLengthText.Replace("mm", "")

                                If Not Integer.TryParse(controlLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL LENGTH FOR ITEM {0} !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If

                                If controlType = "Wand" AndAlso controlLengthValue > 1000 Then
                                    Dim thisAlert As String = String.Format("MAXIMUM CONTROL / WAND LENGTH IS 1000MM FOR ITEM {0}", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If blindType = "Fabric Only" Then
                            If Not String.IsNullOrEmpty(controlLengthText) Then
                                Dim thisAlert As String = String.Format("CONTROL LENGTH IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Complete Set" Then
                            If String.IsNullOrEmpty(bracketExtension) Then
                                Dim thisAlert As String = String.Format("BRACKET EXTENSION IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                            Dim validExtension As String() = {"Standard Bracket", "Extension Bracket"}
                            If Not validExtension.Contains(bracketExtension) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE BRACKET EXTENSION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If blindType = "Fabric Only" AndAlso Not String.IsNullOrEmpty(bracketExtension) Then
                            Dim thisAlert As String = String.Format("BRACKET EXTENSION IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If msgError.InnerText = "" Then
                            Dim totalItems As Integer = 1

                            If bracketExtension = "Standard Bracket" Then bracketExtension = String.Empty
                            If bracketExtension = "Extension Bracket" Then bracketExtension = "Yes"

                            wandLengthValue = 0
                            If controlType = "Wand" Then wandLengthValue = controlLengthValue

                            If controlType = "Wand" Then
                                If stackPosition = "Left" Then controlPosition = "Right"
                                If stackPosition = "Right" Then controlPosition = "Left"
                                If stackPosition = "Centre" Then controlPosition = "Right and Left"
                                If stackPosition = "Split" Then controlPosition = "Middle"
                            End If

                            Dim linearMetre As Decimal = width / 1000
                            Dim squareMetre As Decimal = width * drop / 1000000

                            Dim groupName As String = String.Format("Saphora Drape - {0}", blindType, blindType)
                            Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)

                            Dim itemId As String = orderClass.GetNewOrderItemId()

                            Using thisConn As SqlConnection = New SqlConnection(myConn)
                                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, ChainId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], StackPosition, ControlPosition, ControlLength, ControlLengthValue, WandColour, WandLengthValue, BracketExtension, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @ChainId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @StackPosition, @ControlPosition, @ControlLength, @ControlLengthValue, @WandColour, @WandLengthValue, @BracketExtension, @LinearMetre, @SquareMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                                    myCmd.Parameters.AddWithValue("@Id", itemId)
                                    myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                    myCmd.Parameters.AddWithValue("@ProductId", productId)
                                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(fabricId), CType(DBNull.Value, Object), fabricId))
                                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                                    myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                    myCmd.Parameters.AddWithValue("@Qty", "1")
                                    myCmd.Parameters.AddWithValue("@Room", room)
                                    myCmd.Parameters.AddWithValue("@Mounting", sizeType & " " & mounting)
                                    myCmd.Parameters.AddWithValue("@Width", width)
                                    myCmd.Parameters.AddWithValue("@Drop", drop)
                                    myCmd.Parameters.AddWithValue("@StackPosition", stackPosition)
                                    myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                    myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                    myCmd.Parameters.AddWithValue("@WandColour", controlColour)
                                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)
                                    myCmd.Parameters.AddWithValue("@BracketExtension", bracketExtension)
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

                    If designType = "Cellular Shades" Then
                        Dim itemNumber As String = row - 3

                        Dim blindType As String = If(sheetDetail.Cells(row, 2).Text IsNot Nothing, sheetDetail.Cells(row, 2).Text, "")
                        Dim controlType As String = (sheetDetail.Cells(row, 3).Text & "").Trim()
                        Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 4).Text), 0, CInt(sheetDetail.Cells(row, 4).Text))
                        Dim room As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 6).Text & "").Trim()

                        Dim widthText As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                        Dim width As Integer

                        Dim dropText As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim drop As Integer

                        Dim fabricType As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim fabricTypeB As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim fabricColourB As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim controlPosition As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim cordLengthText As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim supply As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 16).Text & "").Trim()

                        Dim widthB As Integer = 0
                        Dim dropB As Integer = 0
                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0

                        Dim designId As String = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='" & designType & "'")
                        If String.IsNullOrEmpty(designId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT IN ITEM {0} IS NOT REGISTERED. PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindType & "' AND companyArray.value='" & companyDetailId & "' AND Active=1")
                        If String.IsNullOrEmpty(blindId) Then
                            Dim thisAlert As String = String.Format("THE ORDER TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CHECK AGAIN. !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & blindType & "'")

                        Dim controlId As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlType & "'")
                        If String.IsNullOrEmpty(controlId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL TYPE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.value='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='56' AND Active=1")
                        If String.IsNullOrEmpty(productId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT YOU REQUESTED IS CURRENTLY NOT AVAILABLE IN OUR STOCKS. PLEASE REVIEW YOUR ORDER OR CONTACT OUR CUSTOMER SERVICE TEAM FOR ASSISTANCE. ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If qty <> 1 Then
                            Dim thisAlert As String = String.Format("THE ORDER QTY MUS BE 1 PER ITEM LINE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validMounting As String() = {"Face Fit", "Reveal Fit", "Opening Size Face Fit", "Opening Size Reveal Fit", "Make Size Face Fit", "Make Size Reveal Fit"}
                        If Not validMounting.Contains(mounting) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE MOUNTING FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(widthText) Then
                            Dim thisAlert As String = String.Format("WIDTH IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        If Not Integer.TryParse(widthText, width) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(dropText) Then
                            Dim thisAlert As String = String.Format("DROP IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        If Not Integer.TryParse(dropText, drop) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricType) Then
                            Dim thisAlert As String = String.Format("FABRIC TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim fabricId As String = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND tubeArray.VALUE='" & tubeId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricId) Then
                            Dim thisAlert As String = String.Format("THE FABRIC TYPE FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricColour) Then
                            Dim thisAlert As String = String.Format("FABRIC COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim fabricColourId As String = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricColourId) Then
                            Dim thisAlert As String = String.Format("THE FABRIC COLOUR FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim fabricIdB As String = String.Empty
                        Dim fabricColourIdB As String = String.Empty

                        If blindType = "Day & Night" Then
                            If String.IsNullOrEmpty(fabricTypeB) Then
                                Dim thisAlert As String = String.Format("SECOND FABRIC TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            fabricIdB = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricTypeB & "' AND designArray.VALUE='" & designId & "' AND tubeArray.VALUE='" & tubeId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricIdB) Then
                                Dim thisAlert As String = String.Format("THE SECOND FABRIC TYPE FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            fabricColourIdB = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricIdB & "' AND Colour='" & fabricColourB & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricColourIdB) Then
                                Dim thisAlert As String = String.Format("THE SECOND FABRIC COLOUR FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If controlType = "Corded" Then
                            If String.IsNullOrEmpty(controlPosition) Then
                                Dim thisAlert As String = String.Format("CORD POSITION IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            Dim validControlPosition As String() = {"Left", "Right"}
                            If blindType = "Day & Night" OrElse blindType = "TDBU" Then
                                validControlPosition = {"Both Sides"}
                            End If
                            If Not validControlPosition.Contains(controlPosition) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE CONTROL POSITION FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)

                            If Not String.IsNullOrEmpty(cordLengthText) AndAlso Not cordLengthText.ToLower().Contains("standard") AndAlso Not cordLengthText.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                cordLengthText = cordLengthText.Replace("mm", "")
                                If Not Integer.TryParse(cordLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    MessageError(True, "PLEASE CHECK YOUR CORD LENGTH !")
                                    Exit For
                                End If
                            End If
                        End If

                        If controlType = "Cordless" Then
                            If Not String.IsNullOrEmpty(controlPosition) Then
                                Dim thisAlert As String = String.Format("CORD POSITION IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If Not String.IsNullOrEmpty(cordLengthText) Then
                                Dim thisAlert As String = String.Format("CORD LENGTH IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If Not String.IsNullOrEmpty(supply) Then
                            Dim validSupply As String() = {"No", "Yes"}
                            If Not validSupply.Contains(supply) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE HOLD DOWN CLIP FOR ITEM {0}", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If msgError.InnerText = "" Then
                            Dim totalItems As Integer = 1

                            If mounting = "Face Fit" OrElse mounting = "Reveal Fit" Then
                                mounting = String.Format("Opening Size {0}", mounting)
                            End If

                            Dim linearMetre As Decimal = width / 1000
                            Dim linearMetreB As Decimal = 0D

                            Dim squareMetre As Decimal = width * drop / 1000000
                            Dim squareMetreB As Decimal = 0

                            If blindType = "Day & Night" OrElse blindType = "Top Down Bottom Up" Then
                                controlPosition = "Both Sides"
                            End If

                            If controlType = "Cordless" Then
                                controlPosition = String.Empty
                                controlLength = String.Empty : controlLengthValue = 0
                            End If

                            Dim fabricGroup As String = orderClass.GetFabricGroup(fabricId)

                            Dim factory As String = orderClass.GetFabricFactory(fabricColourId)

                            Dim groupName As String = String.Format("{0} - {1} - {2} - {3}", blindType, controlType, fabricGroup, factory)
                            Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                            Dim priceProductGroupB As String = String.Empty

                            If blindType = "Day & Night" Then
                                widthB = width : dropB = drop

                                linearMetreB = width / 1000
                                squareMetreB = widthB * dropB / 1000000
                                totalItems = 2

                                Dim factoryB As String = orderClass.GetFabricFactory(fabricColourIdB)

                                groupName = String.Format("{0} - {1} - {2}", blindType, controlType, factory)
                                Dim groupNameB As String = String.Format("{0} - {1} - {2}", blindType, controlType, factoryB)

                                priceProductGroup = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupB = orderClass.GetPriceProductGroupId(groupNameB, designId, companyDetailId)
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
                    End If

                    If designType = "Roman" Then
                        Dim itemNumber As String = row - 3

                        Dim blindType As String = If(sheetDetail.Cells(row, 2).Text IsNot Nothing, sheetDetail.Cells(row, 2).Text, "")
                        Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 3).Text), 0, CInt(sheetDetail.Cells(row, 3).Text))
                        Dim room As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim romanType As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim batten As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                        Dim battenColour As String = String.Empty
                        Dim fabricType As String = (sheetDetail.Cells(row, 8).Text & "").Trim()
                        Dim fabricColour As String = (sheetDetail.Cells(row, 9).Text & "").Trim()

                        Dim widthText As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim width As Integer

                        Dim dropText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim drop As Integer

                        Dim controlPosition As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim controlType As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim controlColour As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim remoteMotor As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim controlLengthText As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                        Dim valanceOption As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 18).Text & "").Trim()

                        Dim designId As String = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='Roman Blind'")
                        If String.IsNullOrEmpty(designId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT IN ITEM {0} IS NOT REGISTERED. PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='Roman Blind' AND companyArray.value='" & companyDetailId & "' AND Active=1")
                        If String.IsNullOrEmpty(blindId) Then
                            Dim thisAlert As String = String.Format("THE ORDER TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CHECK AGAIN. !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If qty <> 1 Then
                            Dim thisAlert As String = String.Format("THE ORDER QTY MUS BE 1 PER ITEM LINE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(room) Then
                            MessageError(True, "ROOM / LOCATION IS REQUIRED !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(mounting) Then
                            Dim thisAlert As String = String.Format("MOUNTING IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If mounting <> "Face Fit" Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE MOUNTING FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim tubeName As String = romanType
                        Dim tubeId As String = orderClass.GetItemData("SELECT Id FROM ProductTubes WHERE Name='" & tubeName & "'")
                        If String.IsNullOrEmpty(tubeId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THEN ROMAN TYPE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If tubeName = "Plantation" AndAlso String.IsNullOrEmpty(batten) Then
                            Dim thisAlert As String = String.Format("BATTEN COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If tubeName = "Classic" OrElse tubeName = "Sewless" Then
                            If Not String.IsNullOrEmpty(batten) Then
                                Dim thisAlert As String = String.Format("BATTEN COLOUR IS NOT REQUIRED FOR ITEM {0} AND THIS TYPE !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If String.IsNullOrEmpty(fabricType) Then
                            Dim thisAlert As String = String.Format("FABRIC TYPE FOR ITEM {0} IS REQUIRED !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        Dim fabricId As String = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricType & "' AND designArray.VALUE='" & designId & "' AND tubeArray.VALUE='" & tubeId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricId) Then
                            Dim thisAlert As String = String.Format("THE FABRIC TYPE FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(fabricColour) Then
                            Dim thisAlert As String = String.Format("FABRIC COLOUR FOR ITEM {0} IS REQUIRED !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim fabricColourId As String = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricId & "' AND Colour='" & fabricColour & "' AND Active=1")
                        If String.IsNullOrEmpty(fabricColourId) Then
                            Dim thisAlert As String = String.Format("THE FABRIC COLOUR FOR ITEM {0} IS CURRENTLY UNAVAILABLE !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(widthText) Then
                            Dim thisAlert As String = String.Format("WIDTH IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        If Not Integer.TryParse(widthText, width) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(dropText) Then
                            Dim thisAlert As String = String.Format("DROP IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        If Not Integer.TryParse(dropText, drop) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(controlPosition) Then
                            Dim thisAlert As String = String.Format("CONTROL POSITION IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(controlType) Then
                            Dim thisAlert As String = String.Format("MECHANISM TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim controlName As String = controlType
                        If controlType = "Cord" Then controlName = "Reg Cord Lock"
                        If controlType = "Cord Lock" Then controlName = "Reg Cord Lock"
                        Dim controlId As String = orderClass.GetItemData("SELECT Id FROM ProductControls WHERE Name='" & controlName & "'")
                        If String.IsNullOrEmpty(controlId) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE MECHANISM TYPE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='56' AND Active=1")
                        If String.IsNullOrEmpty(productId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT YOU REQUESTED IS CURRENTLY NOT AVAILABLE IN OUR STOCKS. PLEASE REVIEW YOUR ORDER OR CONTACT OUR CUSTOMER SERVICE TEAM FOR ASSISTANCE. ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If controlType = "Chain" AndAlso String.IsNullOrEmpty(controlColour) Then
                            MessageError(True, "MECHANISM / CHAIN COLOUR IS REQUIRED !")
                            Exit For
                        End If

                        Dim chainId As String = String.Empty
                        Dim chainName As String = String.Empty
                        If controlType = "Chain" OrElse controlType = "Motorised" Then
                            chainName = String.Format("Cont Plastic {0}", controlColour)
                            If controlColour = "Stainless Steel" Then chainName = String.Format("Cont {0}", controlColour)
                            If controlType = "Motorised" Then chainName = "No Remote"

                            chainId = orderClass.GetItemData("SELECT Id FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(ControlTypeId, ',') AS controlArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & chainName & "' AND designArray.VALUE='" & designId & "' AND controlArray.VALUE='" & controlId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                            If String.IsNullOrEmpty(chainId) Then
                                MessageError(True, "PLEASE CHECK YOUR CHAIN COLOUR / MOTOR REMOTE DATA !")
                                Exit For
                            End If
                        End If

                        If controlType = "Reg Cord Lock" Then controlColour = "White"

                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0
                        If controlType = "Cord" Then
                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)

                            If Not String.IsNullOrEmpty(controlLengthText) AndAlso Not controlLengthText.ToLower().Contains("standard") AndAlso Not controlLengthText.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                controlLengthText = controlLengthText.Replace("mm", "")
                                If Not Integer.TryParse(controlLengthText, controlLengthValue) OrElse controlLengthValue < 0 OrElse controlLengthValue > 1000 Then
                                    MessageError(True, "PLEASE CHECK YOUR CORD LENGTH !")
                                    Exit For
                                End If
                            End If
                        End If

                        If controlType = "Chain" Then
                            controlLength = "Standard"
                            controlLengthValue = 500
                            Dim thisFormula As Integer = Math.Ceiling(drop * 2 / 3)
                            If thisFormula > 500 Then controlLengthValue = 750
                            If thisFormula > 750 Then controlLengthValue = 1000
                            If thisFormula > 1000 Then controlLengthValue = 1200
                            If thisFormula > 1200 Then controlLengthValue = 1500

                            If Not String.IsNullOrEmpty(controlLengthText) AndAlso Not controlLengthText.ToLower().Contains("standard") AndAlso Not controlLengthText.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                controlLengthText = controlLengthText.Replace("mm", "")
                                If Not Integer.TryParse(controlLengthText, controlLengthValue) OrElse controlLengthValue < 0 OrElse controlLengthValue > 1000 Then
                                    MessageError(True, "PLEASE CHECK YOUR CHAIN LENGTH !")
                                    Exit For
                                End If
                            End If
                        End If

                        If controlType = "Chain" Then valanceOption = "Retrousse"
                        If String.IsNullOrEmpty(valanceOption) Then
                            MessageError(True, "VALANCE OPTION IS REQUIRED !")
                            Exit For
                        End If

                        Dim validValanceOption As String() = {"Facade", "Retrousse"}
                        If Not validValanceOption.Contains(valanceOption) Then
                            MessageError(True, "PLEASE CHECK YOUR SIZE TYPE !")
                            Exit For
                        End If

                        If controlType = "Chain" AndAlso Not valanceOption = "Retrousse" Then
                            MessageError(True, "VALANCE OPTION MUST BE RETROUSSE !")
                            Exit For
                        End If

                        If msgError.InnerText = "" Then
                            If batten = "Snow" Then battenColour = "White"
                            If batten = "Ivory" Then battenColour = "Alabaster"
                            If batten = "Pine" Then battenColour = "Natural"
                            If batten = "Tawny" Then battenColour = "Baltic"
                            If batten = "Mocha" Then battenColour = "Teak"
                            If batten = "Garnet" Then battenColour = "Cherry"
                            If batten = "Earth" Then battenColour = "Brown"
                            If batten = "Midnight" Then battenColour = "Black"
                        End If

                        Dim linearMetre As Decimal = width / 1000
                        Dim squareMetre As Decimal = width * drop / 1000000

                        Dim groupFabric As String = orderClass.GetFabricGroup(fabricId)
                        Dim groupName As String = String.Format("Roman Blind - {0} - {1}", tubeName, groupFabric)
                        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)

                        Dim itemId As String = orderClass.GetNewOrderItemId()

                        Using thisConn As SqlConnection = New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, ChainId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], ControlPosition, ControlColour, ControlLength, ControlLengthValue, ValanceOption, Batten, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @ChainId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @ControlPosition, @ControlColour, @ControlLength, @ControlLengthValue, @ValanceOption, @Batten, @LinearMetre, @SquareMetre, 1, @Notes, 0, 1)", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", itemId)
                                myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                myCmd.Parameters.AddWithValue("@ProductId", productId)
                                myCmd.Parameters.AddWithValue("@FabricId", fabricId)
                                myCmd.Parameters.AddWithValue("@FabricColourId", fabricColourId)
                                myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(chainId), CType(DBNull.Value, Object), chainId))
                                myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                myCmd.Parameters.AddWithValue("@Qty", "1")
                                myCmd.Parameters.AddWithValue("@Room", room)
                                myCmd.Parameters.AddWithValue("@Mounting", mounting)
                                myCmd.Parameters.AddWithValue("@Width", width)
                                myCmd.Parameters.AddWithValue("@Drop", drop)
                                myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                myCmd.Parameters.AddWithValue("@ControlColour", controlColour)
                                myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                                myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)
                                myCmd.Parameters.AddWithValue("@ValanceOption", valanceOption)
                                myCmd.Parameters.AddWithValue("@Batten", batten)
                                myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                                myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                                myCmd.Parameters.AddWithValue("@Notes", notes)

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

                    If designType = "Panel Glide" Then
                        Dim itemNumber As String = row - 3

                        Dim blindType As String = If(sheetDetail.Cells(row, 2).Text IsNot Nothing, sheetDetail.Cells(row, 2).Text, "")
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

                        Dim designId As String = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='Panel Glide'")
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

                        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)

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

                    If designType = "Venetian" Then
                        Dim itemNumber As String = row - 3
                        Dim blindType As String = If(sheetDetail.Cells(row, 2).Text IsNot Nothing, sheetDetail.Cells(row, 2).Text, "")

                        Dim designName As String = blindType
                        Dim blindName As String = String.Empty
                        If blindType = "Aluminium Venetian" OrElse blindType = "Aluminium" Then
                            designName = "Aluminium Blind" : blindName = "Aluminium 25mm x 0.21mm"
                        End If
                        If blindType = "Basswood Venetian 50mm" OrElse blindType = "Basswood 50mm" Then
                            designName = "Venetian Blind" : blindName = "Basswood 50mm"
                        End If
                        If blindType = "Basswood Venetian 63mm" OrElse blindType = "Basswood 63mm" Then
                            designName = "Venetian Blind" : blindName = "Basswood 63mm"
                        End If
                        If blindType = "Econo 50mm" Then
                            designName = "Venetian Blind" : blindName = "Econo 50mm"
                        End If
                        If blindType = "Econo 63mm" Then
                            designName = "Venetian Blind" : blindName = "Econo 63mm"
                        End If
                        If blindType = "Ultraslat Venetian 50mm" Then
                            designName = "Venetian Blind" : blindName = "Ultraslat 50mm"
                        End If
                        If blindType = "Basswood Venetian 63mm" Then
                            designName = "Venetian Blind" : blindName = "Ultraslat 63mm"
                        End If

                        Dim designId As String = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='" & designName & "'")
                        If String.IsNullOrEmpty(designId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT IN ITEM {0} IS NOT REGISTERED. PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim blindId As String = orderClass.GetItemData("SELECT Id FROM Blinds CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND Name='" & blindName & "' AND CompanyArray.VALUE='" & companyDetailId & "'")
                        If String.IsNullOrEmpty(blindId) Then
                            Dim thisAlert As String = String.Format("THE ORDER TYPE IN ITEM {0} IS NOT REGISTERED. PLEASE CHECK AGAIN. !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim subType As String = "Single"
                        Dim qty As Integer = If(String.IsNullOrWhiteSpace(sheetDetail.Cells(row, 3).Text), 0, CInt(sheetDetail.Cells(row, 3).Text))
                        Dim room As String = (sheetDetail.Cells(row, 4).Text & "").Trim()
                        Dim mounting As String = (sheetDetail.Cells(row, 5).Text & "").Trim()
                        Dim colour As String = (sheetDetail.Cells(row, 6).Text & "").Trim()
                        Dim controlPosition As String = (sheetDetail.Cells(row, 7).Text & "").Trim()
                        Dim tilterPosition As String = (sheetDetail.Cells(row, 8).Text & "").Trim()

                        Dim widthText As String = (sheetDetail.Cells(row, 9).Text & "").Trim()
                        Dim width As Integer

                        Dim dropText As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim drop As Integer

                        Dim cordLengthText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim wandLengthText As String = (sheetDetail.Cells(row, 12).Text & "").Trim()
                        Dim supply As String = (sheetDetail.Cells(row, 13).Text & "").Trim()
                        Dim tassel As String = (sheetDetail.Cells(row, 14).Text & "").Trim()
                        Dim valanceType As String = (sheetDetail.Cells(row, 15).Text & "").Trim()
                        Dim valanceSizeText As String = (sheetDetail.Cells(row, 16).Text & "").Trim()
                        Dim returnPosition As String = (sheetDetail.Cells(row, 17).Text & "").Trim()
                        Dim returnLengthText As String = (sheetDetail.Cells(row, 18).Text & "").Trim()
                        Dim notes As String = (sheetDetail.Cells(row, 19).Text & "").Trim()

                        Dim controlLength As String = String.Empty
                        Dim controlLengthValue As Integer = 0

                        Dim wandLength As String = String.Empty
                        Dim wandLengthValue As Integer = 0

                        Dim valanceSize As String = String.Empty
                        Dim valanceSizeValue As Integer = 0

                        Dim returnLength As String = String.Empty
                        Dim returnLengthValue As Integer = 0

                        If qty <> 1 Then
                            Dim thisAlert As String = String.Format("THE ORDER QTY MUS BE 1 PER ITEM LINE. PLEASE CHECK ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(room) Then
                            Dim thisAlert As String = String.Format("ROOM / LOCATION IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(mounting) Then
                            Dim thisAlert As String = String.Format("MOUNTING IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If
                        Dim validMounting As String() = {"Face Fit", "Reveal Fit", "Make Size Face Fit", "Make Size Reveal Fit", "Opening Size Face Fit", "Opening Size Reveal Fit"}
                        If Not validMounting.Contains(mounting) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE MOUNTING FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim finalMounting As String = mounting
                        If ddlCustomer.SelectedValue = "127" Then finalMounting = String.Format("Opening Size {0}", mounting)

                        If String.IsNullOrEmpty(colour) Then
                            Dim thisAlert As String = String.Format("COLOUR IS REQUIRED FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim tubeId As String = "9" : Dim controlId As String = "17"
                        Dim colourId As String = orderClass.GetItemData("SELECT Id FROM ProductColours WHERE Name='" & colour & "'")
                        If String.IsNullOrEmpty(colourId) Then
                            Dim thisAlert As String = String.Format("THE COLOUR YOU REQUESTED IS CURRENTLY NOT AVAILABLE IN OUR STOCKS. PLEASE REVIEW YOUR ORDER OR CONTACT OUR CUSTOMER SERVICE TEAM FOR ASSISTANCE. ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim productId As String = orderClass.GetItemData("SELECT Id FROM Products CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE DesignId='" & designId & "' AND BlindId='" & blindId & "' AND companyArray.value='" & companyDetailId & "' AND TubeType='" & tubeId & "' AND ControlType='" & controlId & "' AND ColourType='" & colourId & "' AND Active=1")
                        If String.IsNullOrEmpty(productId) Then
                            Dim thisAlert As String = String.Format("THE PRODUCT YOU REQUESTED IS CURRENTLY NOT AVAILABLE IN OUR STOCKS. PLEASE REVIEW YOUR ORDER OR CONTACT OUR CUSTOMER SERVICE TEAM FOR ASSISTANCE. ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(controlPosition) Then
                            Dim thisAlert As String = String.Format("CONTROL POSITION IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validCP As String() = {"Left", "Right", "No Control"}
                        If Not validCP.Contains(controlPosition) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR CONTROL POSITION VALUE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(tilterPosition) Then
                            Dim thisAlert As String = String.Format("TILTER POSITION IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        Dim validTP As String() = {"Left", "Right", "Center", "Centre"}
                        If Not validTP.Contains(tilterPosition) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR TILTER POSITION VALUE FOR ITEM {0} !", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If String.IsNullOrEmpty(widthText) Then
                            Dim thisAlert As String = String.Format("WIDTH IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If Not Integer.TryParse(widthText, width) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If designName = "Aluminium Blind" Then
                            If width < 200 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} & MINIMUM WIDTH IS 200MM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If width > 3010 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH FOR ITEM {0} & MAXIMUM WIDTH IS 3010MM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If width > 250 AndAlso width <= 299 AndAlso controlPosition = tilterPosition Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE CONTROL & TILTER POSITION FOR ITEM {0} & PLEASE USE OPPOSITE CONTROL AND TILTER POSITIONS !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If width < 250 Then
                                If controlPosition = "No Control" Then
                                    Dim thisAlert As String = String.Format("YOUR WIDTH FOR ITEM {0} UNDER 250MM. PLEASE CHANGE PULL CORD POSITION TO NO CONTROL !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If

                                If tilterPosition <> "Center" OrElse tilterPosition <> "Centre" Then
                                    Dim thisAlert As String = String.Format("YOUR WIDTH FOR ITEM {0} UNDER 250MM. PLEASE CHANGE TILTER POSITION TO CENTRE !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If designName = "Venetian Blind" Then
                            If width < 250 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH ORDER FOR ITEM {0} & MINIMUM WIDTH IS 250MM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If width > 2710 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE WIDTH ORDER FOR ITEM {0} & MAXIMUM WIDTH IS 2710MM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If width > 300 AndAlso width <= 400 AndAlso controlPosition = tilterPosition Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE CONTROL & TILTER POSITION FOR ITEM {0} & PLEASE USE OPPOSITE CONTROL AND TILTER POSITIONS !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If width < 300 Then
                                If controlPosition = "No Control" Then
                                    Dim thisAlert As String = String.Format("YOUR WIDTH FOR ITEM {0} UNDER 250MM. PLEASE CHANGE PULL CORD POSITION TO NO CONTROL !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If

                                If tilterPosition <> "Center" OrElse tilterPosition <> "Centre" Then
                                    Dim thisAlert As String = String.Format("YOUR WIDTH FOR ITEM {0} UNDER 250MM. PLEASE CHANGE TILTER POSITION TO CENTRE !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If String.IsNullOrEmpty(dropText) Then
                            Dim thisAlert As String = String.Format("DROP IS REQUIRED FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If Not Integer.TryParse(dropText, drop) Then
                            Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0}", itemNumber)
                            MessageError(True, thisAlert)
                            Exit For
                        End If

                        If designName = "Aluminium Blind" Then
                            If drop < 250 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} & MINIMUM DROP IS 250MM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If drop > 3200 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP FOR ITEM {0} & MAXIMUM DROP IS 3200MM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If designName = "Venetian Blind" Then
                            If drop < 200 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP ORDER FOR ITEM {0} & MINIMUM DROP IS 200MM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If drop > 3200 Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE DROP ORDER FOR ITEM {0} & MAXIMUM DROP IS 3200MM !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If designName = "Venetian Blind" Then
                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)
                            If controlLengthValue < 450 Then controlLengthValue = 550

                            If Not String.IsNullOrEmpty(cordLengthText) AndAlso Not cordLengthText.ToLower().Contains("standard") AndAlso Not cordLengthText.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                cordLengthText = cordLengthText.Replace("mm", "")
                                If Not Integer.TryParse(cordLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE CORD LENGTH FOR ITEM {0}", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If designName = "Aluminium Blind" Then
                            controlLength = "Standard"
                            controlLengthValue = Math.Ceiling(drop * 2 / 3)
                            If controlLengthValue < 450 Then controlLengthValue = 450

                            If Not String.IsNullOrEmpty(cordLengthText) AndAlso Not cordLengthText.ToLower().Contains("standard") AndAlso Not cordLengthText.ToLower().Contains("std") Then
                                controlLength = "Custom"
                                cordLengthText = cordLengthText.Replace("mm", "")
                                If Not Integer.TryParse(cordLengthText, controlLengthValue) OrElse controlLengthValue < 0 Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE CORD LENGTH FOR ITEM {0}", itemNumber)
                                    MessageError(True, thisAlert)
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
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE WAND LENGTH FOR ITEM {0}", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If
                        End If

                        If Not String.IsNullOrEmpty(supply) Then
                            Dim validSupply As String() = {"No", "Yes"}
                            If Not validSupply.Contains(supply) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE HOLD DOWN CLIP FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                        End If

                        If designName = "Venetian Blind" Then
                            If String.IsNullOrEmpty(tassel) Then
                                Dim thisAlert As String = String.Format("METAL TASSEL OPTION IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If
                            Dim validTassel As String() = {"Plastic", "Gold", "Antique Brass"}
                            If Not validTassel.Contains(tassel) Then
                                Dim thisAlert As String = String.Format("PLEASE CHECK THE METAL TASSEL OPTION FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If String.IsNullOrEmpty(valanceType) Then
                                Dim thisAlert As String = String.Format("VALANCE TYPE IS REQUIRED FOR ITEM {0} !", itemNumber)
                                MessageError(True, thisAlert)
                                Exit For
                            End If

                            If blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" Then
                                If valanceType <> "75mm Valance" Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE VALANCE TYPE FOR ITEM {0} !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If

                            If blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" OrElse blindName = "Ultraslat 50mm" OrElse blindName = "Ultraslat 63mm" Then
                                If valanceType <> "76mm Valance" Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE VALANCE TYPE FOR ITEM {0} !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If

                            valanceSize = "Standard"
                            If finalMounting = "Opening Size Reveal Fit" Then valanceSizeValue = width - 1
                            If finalMounting = "Make Size Reveal Fit" Then valanceSizeValue = width + 9
                            If finalMounting = "Opening Size Face Fit" Then valanceSizeValue = width + 20
                            If finalMounting = "Make Size Face Fit" Then valanceSizeValue = width + 20

                            If Not String.IsNullOrEmpty(valanceSizeText) AndAlso Not valanceSizeText.ToLower().Contains("standard") AndAlso Not valanceSizeText.ToLower().Contains("std") Then
                                valanceSize = "Custom"
                                valanceSizeText = valanceSizeText.Replace("mm", "")
                                If Not Integer.TryParse(valanceSizeText, valanceSizeValue) OrElse valanceSizeValue < 0 Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE VALANCE SIZE VALUE FOR ITEM {0} !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If
                            End If

                            If String.IsNullOrEmpty(returnPosition) Then returnPosition = "None"
                            If Not String.IsNullOrEmpty(returnPosition) Then
                                Dim validRP As String() = {"None", "Left", "Right", "Both Sides"}
                                If Not validRP.Contains(returnPosition) Then
                                    Dim thisAlert As String = String.Format("PLEASE CHECK THE VALANCE RETURN POSITION FOR ITEM {0} !", itemNumber)
                                    MessageError(True, thisAlert)
                                    Exit For
                                End If

                                If returnPosition <> "None" Then
                                    returnLength = "Standard"
                                    If finalMounting = "Opening Size Face Fit" OrElse finalMounting = "Make Size Face Fit" Then
                                        returnLengthValue = 70
                                        If blindName = "Econo 50" OrElse blindName = "Econo 63mm" OrElse blindName = "Ultraslat 50mm" OrElse blindName = "Ultraslat 63mm" Then
                                            returnLengthValue = 77
                                        End If
                                    End If
                                    If finalMounting = "Opening Size Reveal Fit" OrElse finalMounting = "Make Size Reveal Fit" Then
                                        returnLengthValue = 20
                                    End If

                                    If Not String.IsNullOrEmpty(returnLengthText) AndAlso Not returnLengthText.ToLower().Contains("standard") AndAlso Not returnLengthText.ToLower().Contains("std") Then
                                        returnLength = "Custom"
                                        returnLengthText = returnLengthText.Replace("mm", "")

                                        If Not Integer.TryParse(returnLengthText, returnLengthValue) OrElse returnLengthValue < 0 Then
                                            Dim thisAlert As String = String.Format("PLEASE CHECK YOUR VALANCE RETURN LENGTH !", itemNumber)
                                            MessageError(True, thisAlert)
                                            Exit For
                                        End If
                                    End If
                                End If
                            End If


                        End If

                        If msgError.InnerText = "" Then
                            Dim itemId As String = orderClass.GetNewOrderItemId()

                            If returnPosition = "None" Then returnPosition = String.Empty

                            Dim groupName As String = blindName
                            If blindName = "Ultraslat 50mm" Then groupName = "Econo 50mm"
                            If blindName = "Ultraslat 63mm" Then groupName = "Econo 63mm"

                            Dim productgroupName As String = String.Format("{0} - {1}", designName, groupName)
                            If designName = "Aluminium Blind" Then
                                productgroupName = blindName
                            End If

                            Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(productgroupName, designId, companyDetailId)

                            Dim linearMetre As Decimal = width / 10000
                            Dim squareMetre As Decimal = width * drop / 1000000

                            Using thisConn As SqlConnection = New SqlConnection(myConn)
                                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails (Id, HeaderId, ProductId, PriceProductGroupId, SubType, Qty, Room, Mounting, ControlPosition, TilterPosition, Width, [Drop], Supply, Tassel, ControlLength, ControlLengthValue, WandLength, WandLengthValue, ValanceType, ValanceSize, ValanceSizeValue, ReturnPosition, ReturnLength, ReturnLengthValue, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, @SubType, 1, @Room, @Mounting, @ControlPosition, @TilterPosition, @Width, @Drop, @Supply, @Tassel, @ControlLength, @ControlLengthValue, @WandLength, @WandLengthValue, @ValanceType, @ValanceSize, @ValanceSizeValue, @ReturnPosition, @ReturnLength, @ReturnLengthValue, @LinearMetre, @SquareMetre, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                                    myCmd.Parameters.AddWithValue("@Id", itemId)
                                    myCmd.Parameters.AddWithValue("@HeaderId", headerId)
                                    myCmd.Parameters.AddWithValue("@ProductId", productId)
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                                    myCmd.Parameters.AddWithValue("@Room", room)
                                    myCmd.Parameters.AddWithValue("@Mounting", finalMounting)
                                    myCmd.Parameters.AddWithValue("@SubType", subType)
                                    myCmd.Parameters.AddWithValue("@ControlPosition", controlPosition)
                                    myCmd.Parameters.AddWithValue("@TilterPosition", tilterPosition)
                                    myCmd.Parameters.AddWithValue("@Width", width)
                                    myCmd.Parameters.AddWithValue("@Drop", drop)

                                    myCmd.Parameters.AddWithValue("@ControlLength", controlLength)
                                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controlLengthValue)

                                    myCmd.Parameters.AddWithValue("@WandLength", wandLength)
                                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandLengthValue)

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

                    If designType = "Roller" Then
                        Dim itemNumber As String = row - 3
                        Dim blindType As String = If(sheetDetail.Cells(row, 2).Text IsNot Nothing, sheetDetail.Cells(row, 2).Text, "")

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

                        Dim widthC As Integer = 0
                        Dim widthTextC As String = (sheetDetail.Cells(row, 10).Text & "").Trim()
                        Dim widthValueC As Integer
                        Dim widthDataC As Integer = If(Integer.TryParse(widthTextC, widthValueC), widthValueC, 0)

                        Dim widthD As Integer = 0
                        Dim widthE As Integer = 0
                        Dim widthF As Integer = 0

                        Dim drop As Integer = 0
                        Dim dropText As String = (sheetDetail.Cells(row, 11).Text & "").Trim()
                        Dim dropValue As Integer
                        Dim dropData As Integer = If(Integer.TryParse(dropText, dropValue), dropValue, 0)

                        Dim dropB As Integer = 0
                        Dim dropC As Integer = 0
                        Dim dropD As Integer = 0
                        Dim dropE As Integer = 0
                        Dim dropF As Integer = 0

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

                        Dim fabricIdDB As String = String.Empty : Dim fabricColourIdDB As String = String.Empty

                        Dim blindName As String = blindType
                        If blindType = "Double: (2 Blinds)" Then blindName = "Dual Blinds"
                        If blindType = "Single: Linked (2 Blinds)" Then
                            If controlText = "SC" OrElse controlText = "CS" Then blindName = "Link 2 Blinds Dependent"
                            If controlText = "II" Then blindName = "Link 2 Blinds Independent"
                        End If
                        If blindType = "Single: Linked (3 Blinds)" Then
                            If controlText = "ISC" OrElse controlText = "CSI" Then blindName = "Link 3 Blinds Independent with Dependent"
                            If controlText = "CSS" OrElse controlText = "SSC" Then blindName = "Link 3 Blinds Dependent"
                        End If
                        If blindType = "Double: Linked (4 Blinds)" Then
                            If controlText = "II - II" Then blindName = "DB Link 2 Blinds Independent"
                            If controlText = "CS - CS" OrElse controlText = "CS - SC" OrElse controlText = "SC - SC" OrElse controlText = "SC - CS" Then blindName = "DB Link 2 Blinds Dependent"
                        End If
                        If blindType = "Double: Linked (6 Blinds)" Then
                            If controlText = "CSI - CSI" OrElse controlText = "ISC - ISC" Then blindName = "DB Link 3 Blinds Independent"
                            If controlText = "CSS - CSS" Or controlText = "SSC - SSC" Then blindName = "DB Link 3 Blinds Dependent"
                        End If

                        Dim designId As String = orderClass.GetItemData("SELECT Id FROM Designs WHERE Name='Roller Blind'")
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

                        If String.IsNullOrEmpty(mechanism) Then
                            MessageError(True, "MECHANISM TUBE IS REQUIRED !")
                            Exit For
                        End If

                        If widthData = 0 Then
                            MessageError(True, "PLEASE CHECK YOUR WIDTH DATA !")
                            Exit For
                        End If

                        If blindName = "Link 2 Blinds Dependent" OrElse blindName = "Link 2 Blinds Independent" OrElse blindName = "DB Link 2 Blinds Independent" OrElse blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" OrElse blindName = "DB Link 3 Blinds Dependent" Then
                            If widthDataB = 0 Then
                                MessageError(True, "PLEASE CHECK YOUR SECOND WIDTH DATA !")
                                Exit For
                            End If
                        End If

                        If blindName = "Link 3 Blinds Dependent" OrElse blindName = "Link 3 Blinds Independent with Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" OrElse blindName = "DB Link 3 Blinds Dependent" Then
                            If widthDataC = 0 Then
                                MessageError(True, "PLEASE CHECK YOUR THIRD WIDTH DATA !")
                                Exit For
                            End If
                        End If

                        If dropData = 0 Then
                            MessageError(True, "PLEASE CHECK YOUR DROP DATA !")
                            Exit For
                        End If

                        Dim linearMetre As Decimal = widthData / 1000
                        Dim linearMetreB As Decimal = widthDataB / 1000
                        Dim linearMetreC As Decimal = widthDataC / 1000
                        Dim linearMetreD As Decimal = 0
                        Dim linearMetreE As Decimal = 0
                        Dim linearMetreF As Decimal = 0

                        Dim squareMetre As Decimal = widthData * dropData / 1000000
                        Dim squareMetreB As Decimal = widthDataB * dropData / 1000000
                        Dim squareMetreC As Decimal = widthDataC * dropData / 1000000
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

                        If blindName = "Dual Blinds" OrElse blindName = "DB Link 2 Blinds Independent" OrElse blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" OrElse blindName = "DB Link 3 Blinds Dependent" Then
                            If String.IsNullOrEmpty(fabricTypeDB) Then
                                MessageError(True, "SECOND FABRIC TYPE IS REQUIRED !")
                                Exit For
                            End If

                            If String.IsNullOrEmpty(fabricColourDB) Then
                                MessageError(True, "SECOND FABRIC COLOUR IS REQUIRED !")
                                Exit For
                            End If

                            fabricIdDB = orderClass.GetItemData("SELECT Id FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE Name='" & fabricTypeDB & "' AND designArray.VALUE='" & designId & "' AND companyArray.VALUE='" & companyDetailId & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricIdDB) Then
                                MessageError(True, "PLEASE CHECK YOUR SECOND FABRIC TYPE !")
                                Exit For
                            End If

                            fabricColourIdDB = orderClass.GetItemData("SELECT Id FROM FabricColours WHERE FabricId='" & fabricIdDB & "' AND Colour='" & fabricColourDB & "' AND Active=1")
                            If String.IsNullOrEmpty(fabricColourIdDB) Then
                                MessageError(True, "PLEASE CHECK YOUR SECOND FABRIC COLOUR !")
                                Exit For
                            End If
                        End If

                        Dim validRoll As String() = {"Front Roll", "Back Roll", "Standard", "Reverse"}
                        If Not validRoll.Contains(roll) Then
                            MessageError(True, "PLEASE CHECK YOUR ROLL DIRECTION DATA !")
                            Exit For
                        End If

                        If blindName = "Dual Blinds" OrElse blindName = "DB Link 2 Blinds Independent" OrElse blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" OrElse blindName = "DB Link 3 Blinds Dependent" Then
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
                        If blindName = "Dual Blinds" Then
                            validControl = New String() {"L - L", "R - R", "L - R", "R - L"}
                        End If
                        If blindType = "Single: Linked (2 Blinds)" Then
                            validControl = New String() {"II", "CS", "SC"}
                        End If
                        If blindType = "Single: Linked (3 Blinds)" Then
                            validControl = New String() {"ISC", "CSI", "CSS", "SSC"}
                        End If
                        If blindType = "Double: Linked (4 Blinds)" Then
                            validControl = New String() {"II - II", "CS - CS", "CS - SC", "SC - SC", "SC - CS"}
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

                            chainColour = chainColour.Replace("Cream", "Ivory")
                            chainColour = chainColour.Replace("Platinum", "Grey")
                            chainColour = chainColour.Replace("Metal Nickel Plated", "Nickel Plated")

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
                            If motorType = "Altus 40" Then controlType = "Altus"
                            If motorType = "Sonesse 30" Then controlType = "Sonesse 30 WF"
                            chainName = "No Remote"
                        End If

                        Dim tubeType As String = mechanism
                        If blindName = "Single Blind" OrElse blindName = "Dual Blinds" Then
                            tubeType = "Gear Reduction 49mm"
                            If controlType = "Chain" Then
                                tubeType = "Gear Reduction 38mm"
                                If widthData > 1810 OrElse widthDataB > 1810 Then tubeType = "Gear Reduction 45mm"
                                If squareMetre >= 6 Then tubeType = "Gear Reduction 49mm"
                            End If
                        End If

                        If blindType = "Single: Linked (2 Blinds)" Then
                            tubeType = "Gear Reduction 49mm"
                            If controlType = "Chain" Then
                                tubeType = "Gear Reduction 38mm"
                                If widthData > 1810 OrElse widthDataB > 1810 Then tubeType = "Gear Reduction 45mm"
                                If squareMetre >= 6 OrElse squareMetreB >= 6 Then tubeType = "Gear Reduction 49mm"
                            End If
                        End If

                        If blindType = "Single: Linked (3 Blinds)" Then
                            tubeType = "Gear Reduction 49mm"
                            If controlType = "Chain" Then
                                tubeType = "Gear Reduction 38mm"
                                If widthData > 1810 OrElse widthDataB > 1810 OrElse widthDataC > 1810 Then tubeType = "Gear Reduction 45mm"
                                If squareMetre >= 6 OrElse squareMetreB >= 6 OrElse squareMetreC >= 6 Then tubeType = "Gear Reduction 49mm"
                            End If
                        End If

                        If blindType = "Double: Linked (4 Blinds)" Then
                            tubeType = "Gear Reduction 49mm"
                            If controlType = "Chain" Then
                                tubeType = "Gear Reduction 38mm"
                                If widthData > 1810 OrElse widthDataB > 1810 Then tubeType = "Gear Reduction 45mm"
                                If squareMetre >= 6 OrElse squareMetreB >= 6 Then tubeType = "Gear Reduction 49mm"
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
                            MessageError(True, "PLEASE CHECK YOUR CHAIN COLOUR / MOTORISED DATA !")
                            Exit For
                        End If

                        If String.IsNullOrEmpty(bottomType) Then
                            MessageError(True, "BOTTOM TYPE IS REQUIRED !")
                            Exit For
                        End If

                        If bottomType = "Silent" Then bottomType = "Flat Mohair"
                        If bottomType = "Fabric Wrap" Then bottomType = "Flat"

                        Dim bottomId As String = orderClass.GetItemData("SELECT Id FROM Bottoms CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE designArray.VALUE='" & designId & "' AND Name = '" & bottomType.Trim() & "' AND companyArray.VALUE='" & companyDetailId & "'")
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

                            Dim priceProductGroupId As String = String.Empty
                            Dim priceProductGroupIdB As String = String.Empty
                            Dim priceProductGroupIdC As String = String.Empty
                            Dim priceProductGroupIdD As String = String.Empty
                            Dim priceProductGroupE As String = String.Empty
                            Dim priceProductGroupF As String = String.Empty

                            Dim bracketextension As String = String.Empty

                            Dim totalItems As Integer = 1

                            Dim groupFabric As String = String.Empty

                            If blindName = "Single Blind" Then
                                width = widthData
                                drop = dropData

                                If bracketType = "Extension" Then bracketextension = "Yes"

                                If roll = "Back Roll" Then roll = "Standard"
                                If roll = "Front Roll" Then roll = "Reverse"

                                If bottomOption = "Front Wrap" Then bottomOption = "Fabric on Front"
                                If bottomOption = "Back Wrap" Then bottomOption = "Fabric on Back"

                                controlPosition = controlText

                                groupFabric = orderClass.GetFabricGroup(fabricId)
                                Dim groupName As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabric)
                                priceProductGroupId = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                            End If

                            If blindName = "Dual Blinds" Then
                                fabricIdB = fabricIdDB
                                fabricColourIdB = fabricColourIdDB

                                groupFabric = orderClass.GetFabricGroup(fabricId)
                                Dim groupFabricDB As String = orderClass.GetFabricGroup(fabricIdB)

                                Dim groupName As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabric)
                                Dim groupNameDB As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabricDB)

                                width = widthData
                                widthB = widthData
                                drop = dropData
                                dropB = dropData

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

                                priceProductGroupId = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdB = orderClass.GetPriceProductGroupId(groupNameDB, designId, companyDetailId)

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

                            If blindName = "Link 2 Blinds Dependent" Then
                                drop = dropData : dropB = dropData
                                If controlText = "CS" Then
                                    controlPosition = "Left"
                                    width = widthData
                                    widthB = widthDataB
                                End If
                                If controlText = "SC" Then
                                    controlPosition = "Right"
                                    width = widthDataB
                                    widthB = widthData
                                End If

                                If bracketType = "Extension" Then bracketextension = "Yes"

                                If roll = "Back Roll" Then roll = "Standard" : rollB = "Standard"
                                If roll = "Front Roll" Then roll = "Reverse" : rollB = "Reverse"

                                fabricIdB = fabricId
                                fabricColourIdB = fabricColourId

                                bottomIdB = bottomId
                                bottomColourIdB = bottomColourId
                                bottomOptionB = bottomOption

                                If bottomOption = "Front Wrap" Then
                                    bottomOption = "Fabric on Front"
                                    bottomOptionB = "Fabric on Front"
                                End If
                                If bottomOption = "Back Wrap" Then
                                    bottomOption = "Fabric on Back"
                                    bottomOptionB = "Fabric on Back"
                                End If

                                totalItems = 2

                                linearMetre = width / 1000
                                linearMetreB = widthB / 1000

                                squareMetre = width * drop / 1000000
                                squareMetreB = widthB * drop / 1000000

                                groupFabric = orderClass.GetFabricGroup(fabricId)
                                Dim groupName As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabric)

                                priceProductGroupId = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdB = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                            End If

                            If blindName = "Link 2 Blinds Independent" Then
                                controlPosition = "Left" : controlPositionB = "Right"
                                width = widthData : widthB = widthDataB
                                drop = dropData : dropB = dropData

                                If bracketType = "Extension" Then bracketextension = "Yes"

                                If roll = "Back Roll" Then roll = "Standard" : rollB = "Standard"
                                If roll = "Front Roll" Then roll = "Reverse" : rollB = "Reverse"

                                fabricIdB = fabricId
                                fabricColourIdB = fabricColourId

                                If controlType = "Chain" Then
                                    chainIdB = chainId
                                    chainStopperB = chainStopper
                                    controlLengthB = controlLength
                                    controlLengthValueB = controlLengthValue
                                End If

                                bottomIdB = bottomId
                                bottomColourIdB = bottomColourId
                                bottomOptionB = bottomOption

                                If bottomOption = "Front Wrap" Then
                                    bottomOption = "Fabric on Front"
                                    bottomOptionB = "Fabric on Front"
                                End If
                                If bottomOption = "Back Wrap" Then
                                    bottomOption = "Fabric on Back"
                                    bottomOptionB = "Fabric on Back"
                                End If

                                totalItems = 2

                                linearMetre = width / 1000
                                linearMetreB = widthB / 1000

                                squareMetre = width * drop / 1000000
                                squareMetreB = widthB * drop / 1000000

                                groupFabric = orderClass.GetFabricGroup(fabricId)
                                Dim groupName As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabric)

                                priceProductGroupId = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdB = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                            End If

                            If blindName = "Link 3 Blinds Dependent" Then
                                drop = dropData : dropB = dropData : dropC = dropData
                                If controlText = "CSS" Then
                                    controlPosition = "Left"
                                    width = widthData
                                    widthB = widthDataB
                                    widthC = widthDataC
                                End If
                                If controlText = "SSC" Then
                                    controlPosition = "Right"
                                    width = widthDataC
                                    widthB = widthDataB
                                    widthC = widthData
                                End If

                                If bracketType = "Extension" Then bracketextension = "Yes"

                                If roll = "Back Roll" Then roll = "Standard" : rollB = "Standard" : rollC = "Standard"
                                If roll = "Front Roll" Then roll = "Reverse" : rollB = "Reverse" : rollC = "Standard"

                                fabricIdB = fabricId : fabricIdC = fabricId
                                fabricColourIdB = fabricColourId : fabricColourIdC = fabricColourId

                                bottomIdB = bottomId
                                bottomColourIdB = bottomColourId
                                bottomOptionB = bottomOption

                                bottomIdC = bottomId
                                bottomColourIdC = bottomColourId
                                bottomOptionC = bottomOption

                                totalItems = 3

                                linearMetre = width / 1000
                                linearMetreB = widthB / 1000
                                linearMetreC = widthC / 1000

                                squareMetre = width * drop / 1000000
                                squareMetreB = widthB * drop / 1000000
                                squareMetreC = widthC * dropC / 1000000

                                groupFabric = orderClass.GetFabricGroup(fabricId)
                                Dim groupName As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabric)

                                priceProductGroupId = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdB = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdC = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                            End If

                            If blindName = "Link 3 Blinds Independent with Dependent" Then
                                controlPosition = "Left" : controlPositionB = "" : controlPositionC = "Right"
                                If controlText = "ISC" Then
                                    width = widthData : widthB = widthDataB : widthC = widthDataC
                                End If
                                If controlText = "CSI" Then
                                    width = widthDataC : widthB = widthDataB : widthC = widthData
                                End If

                                drop = dropData : dropB = dropData : dropC = dropData

                                If bracketType = "Extension" Then bracketextension = "Yes"

                                If roll = "Back Roll" Then roll = "Standard" : rollB = "Standard" : rollC = "Standard"
                                If roll = "Front Roll" Then roll = "Reverse" : rollB = "Reverse" : rollC = "Standard"

                                fabricIdB = fabricId
                                fabricIdC = fabricId

                                fabricColourIdB = fabricColourId
                                fabricColourIdC = fabricColourId

                                If controlType = "Chain" Then
                                    chainIdC = chainId
                                    chainStopperC = chainStopper
                                    controlLengthC = controlLength
                                    controlLengthValueC = controlLengthValue
                                End If

                                bottomIdB = bottomId
                                bottomColourIdB = bottomColourId
                                bottomOptionB = bottomOption

                                bottomIdC = bottomId
                                bottomColourIdC = bottomColourId
                                bottomOptionC = bottomOption

                                totalItems = 3

                                linearMetre = width / 1000
                                linearMetreB = widthB / 1000
                                linearMetreC = widthC / 1000

                                squareMetre = width * drop / 1000000
                                squareMetreB = widthB * drop / 1000000
                                squareMetreC = widthC * dropC / 1000000

                                groupFabric = orderClass.GetFabricGroup(fabricId)
                                Dim groupName As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabric)

                                priceProductGroupId = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdB = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdC = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                            End If

                            If blindName = "DB Link 2 Blinds Dependent" Then
                                If controlText = "CS - CS" Then
                                    controlPosition = "Left" : controlPositionC = "Left"
                                    width = widthData : widthC = widthData
                                    widthB = widthDataB : widthD = widthDataB
                                End If

                                If controlText = "CS - SC" Then
                                    controlPosition = "Left" : controlPositionC = "Right"
                                    width = widthData : widthC = widthDataB
                                    widthB = widthDataB : widthD = widthData
                                End If

                                If controlText = "SC - SC" Then
                                    controlPosition = "Right" : controlPositionC = "Right"
                                    width = widthDataB : widthC = widthDataB
                                    widthB = widthData : widthD = widthData
                                End If

                                If controlText = "SC - CS" Then
                                    controlPosition = "Right" : controlPositionC = "Left"
                                    width = widthDataB : widthC = widthData
                                    widthB = widthData : widthD = widthDataB
                                End If

                                drop = dropData : dropB = dropData : dropC = dropData : dropD = dropData

                                If bracketType = "Extension" Then bracketextension = "Yes"

                                If roll = "Back Roll" Then roll = "Standard" : rollB = "Standard"
                                If roll = "Front Roll" Then roll = "Reverse" : rollB = "Reverse"

                                If rollDB = "Back Roll" Then rollC = "Standard" : rollD = "Standard"
                                If rollDB = "Front Roll" Then rollC = "Reverse" : rollD = "Reverse"

                                fabricIdB = fabricId
                                fabricColourIdB = fabricColourId

                                fabricIdC = fabricIdDB : fabricIdD = fabricIdDB
                                fabricColourIdC = fabricColourIdDB : fabricColourIdD = fabricColourIdDB

                                bottomIdB = bottomId
                                bottomColourIdB = bottomColourId
                                bottomOptionB = bottomOption

                                bottomIdC = bottomId
                                bottomColourIdC = bottomColourId
                                bottomOptionC = bottomOption

                                bottomIdD = bottomId
                                bottomColourIdD = bottomColourId
                                bottomOptionD = bottomOption

                                If controlType = "Chain" Then
                                    chainIdC = chainId
                                    chainStopperC = chainStopper
                                    controlLengthC = controlLength
                                    controlLengthValueC = controlLengthValue
                                End If

                                totalItems = 4

                                linearMetre = width / 1000
                                linearMetreB = widthB / 1000
                                linearMetreC = widthC / 1000
                                linearMetreD = widthD / 1000

                                squareMetre = width * drop / 1000000
                                squareMetreB = widthB * drop / 1000000
                                squareMetreC = widthC * dropC / 1000000
                                squareMetreD = widthD * dropD / 1000000

                                groupFabric = orderClass.GetFabricGroup(fabricId)
                                Dim groupFabricDB As String = orderClass.GetFabricGroup(fabricIdDB)

                                Dim groupName As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabric)
                                Dim groupNameDB As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabricDB)

                                priceProductGroupId = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdB = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdC = orderClass.GetPriceProductGroupId(groupNameDB, designId, companyDetailId)
                                priceProductGroupIdD = orderClass.GetPriceProductGroupId(groupNameDB, designId, companyDetailId)
                            End If

                            If blindName = "DB Link 2 Blinds Independent" Then
                                controlPosition = "Left" : controlPositionB = "Right"
                                controlPositionC = "Left" : controlPositionD = "Right"

                                width = widthData : widthC = widthData
                                widthB = widthDataB : widthD = widthDataB
                                drop = dropData : dropB = dropData : dropC = dropData : dropD = dropData

                                If bracketType = "Extension" Then bracketextension = "Yes"

                                If roll = "Back Roll" Then roll = "Standard" : rollB = "Standard"
                                If roll = "Front Roll" Then roll = "Reverse" : rollB = "Reverse"

                                If rollDB = "Back Roll" Then rollC = "Standard" : rollD = "Standard"
                                If rollDB = "Front Roll" Then rollC = "Reverse" : rollD = "Reverse"

                                fabricIdB = fabricId
                                fabricColourIdB = fabricColourId

                                fabricIdC = fabricIdDB : fabricIdD = fabricIdDB
                                fabricColourIdC = fabricColourIdDB : fabricColourIdD = fabricColourIdDB

                                bottomIdB = bottomId
                                bottomColourIdB = bottomColourId
                                bottomOptionB = bottomOption

                                bottomIdC = bottomId
                                bottomColourIdC = bottomColourId
                                bottomOptionC = bottomOption

                                bottomIdD = bottomId
                                bottomColourIdD = bottomColourId
                                bottomOptionD = bottomOption

                                If controlType = "Chain" Then
                                    chainIdB = chainId
                                    chainStopperB = chainStopper
                                    controlLengthB = controlLength
                                    controlLengthValueB = controlLengthValue

                                    chainIdC = chainId
                                    chainStopperC = chainStopper
                                    controlLengthC = controlLength
                                    controlLengthValueC = controlLengthValue

                                    chainIdD = chainId
                                    chainStopperD = chainStopper
                                    controlLengthD = controlLength
                                    controlLengthValueD = controlLengthValue
                                End If

                                totalItems = 4

                                linearMetre = width / 1000
                                linearMetreB = widthB / 1000
                                linearMetreC = widthC / 1000
                                linearMetreD = widthD / 1000

                                squareMetre = width * drop / 1000000
                                squareMetreB = widthB * drop / 1000000
                                squareMetreC = widthC * dropC / 1000000
                                squareMetreD = widthD * dropD / 1000000

                                groupFabric = orderClass.GetFabricGroup(fabricId)
                                Dim groupFabricDB As String = orderClass.GetFabricGroup(fabricIdDB)

                                Dim groupName As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabric)
                                Dim groupNameDB As String = String.Format("Roller Blind - Gear Reduction - {0}", groupFabricDB)

                                priceProductGroupId = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdB = orderClass.GetPriceProductGroupId(groupName, designId, companyDetailId)
                                priceProductGroupIdC = orderClass.GetPriceProductGroupId(groupNameDB, designId, companyDetailId)
                                priceProductGroupIdD = orderClass.GetPriceProductGroupId(groupNameDB, designId, companyDetailId)
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
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroupId), CType(DBNull.Value, Object), priceProductGroupId))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupIdB), CType(DBNull.Value, Object), priceProductGroupIdB))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdC", If(String.IsNullOrEmpty(priceProductGroupIdC), CType(DBNull.Value, Object), priceProductGroupIdC))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdD", If(String.IsNullOrEmpty(priceProductGroupIdD), CType(DBNull.Value, Object), priceProductGroupIdD))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdE", If(String.IsNullOrEmpty(priceProductGroupE), CType(DBNull.Value, Object), priceProductGroupE))
                                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdF", If(String.IsNullOrEmpty(priceProductGroupF), CType(DBNull.Value, Object), priceProductGroupF))
                                    myCmd.Parameters.AddWithValue("@Qty", "1")
                                    myCmd.Parameters.AddWithValue("@Room", room)
                                    myCmd.Parameters.AddWithValue("@Mounting", sizeType & " " & mounting)
                                    myCmd.Parameters.AddWithValue("@Width", width)
                                    myCmd.Parameters.AddWithValue("@WidthB", widthB)
                                    myCmd.Parameters.AddWithValue("@WidthC", widthC)
                                    myCmd.Parameters.AddWithValue("@WidthD", widthD)
                                    myCmd.Parameters.AddWithValue("@WidthE", widthE)
                                    myCmd.Parameters.AddWithValue("@WidthF", widthF)
                                    myCmd.Parameters.AddWithValue("@Drop", drop)
                                    myCmd.Parameters.AddWithValue("@DropB", dropB)
                                    myCmd.Parameters.AddWithValue("@DropC", dropC)
                                    myCmd.Parameters.AddWithValue("@DropD", dropD)
                                    myCmd.Parameters.AddWithValue("@DropE", dropD)
                                    myCmd.Parameters.AddWithValue("@DropF", dropE)
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

    Protected Sub BindComponentForm(customerId As String, method As String)
        Try
            divCustomer.Visible = False
            divCreatedBy.Visible = False

            divMethod.Visible = False
            divManual.Visible = False
            divApi.Visible = False
            divUpload.Visible = False

            divOrderType.Visible = False

            Dim companyDetailName As String = String.Empty
            If Not String.IsNullOrEmpty(customerId) Then
                companyDetailName = orderClass.GetCompanyDetailNameByCustomer(ddlCustomer.SelectedValue)
            End If

            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then
                divCustomer.Visible = True
                If companyDetailName = "JPMD BP" Then divOrderType.Visible = True
            End If
            If Session("RoleName") = "Developer" Then
                divCreatedBy.Visible = True
            End If

            If Session("CustomerId") = "127" OrElse customerId = "127" OrElse Session("CustomerId") = "985" OrElse customerId = "985" Then divMethod.Visible = True

            If method = "Manual" Then divManual.Visible = True
            If method = "Upload" Then divUpload.Visible = True
            If method = "API" Then divApi.Visible = True
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

    Protected Sub BindDataCustomer()
        ddlCustomer.Items.Clear()
        Try
            Dim role As String = String.Empty
            If Session("RoleName") = "Customer" Then role = "AND Id='" & Session("CustomerId") & "'"
            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Customer Service" Then role = "AND Id='" & Session("CustomerId") & "'"

            Dim thisQuery As String = String.Format("SELECT * FROM Customers WHERE Active=1 {0} ORDER BY Name ASC", role)

            ddlCustomer.DataSource = orderClass.GetDataTable(thisQuery)
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindDataUser(customerId As String)
        ddlCreatedBy.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                Dim thisQuery As String = "SELECT * FROM CustomerLogins WHERE CustomerId='" & customerId & "' OR Id='" & Session("LoginId") & "' ORDER BY UserName ASC"
                If Session("RoleName") = "Customer" Then
                    thisQuery = "SELECT * FROM CustomerLogins WHERE CustomerId='" & customerId & "' OR Id='" & Session("LoginId") & "' ORDER BY UserName ASC"
                End If
                ddlCreatedBy.DataSource = orderClass.GetDataTable(thisQuery)
                ddlCreatedBy.DataTextField = "UserName"
                ddlCreatedBy.DataValueField = "Id"
                ddlCreatedBy.DataBind()

                If ddlCreatedBy.Items.Count > 0 Then
                    ddlCreatedBy.Items.Insert(0, New ListItem("", ""))
                End If

                ddlCreatedBy.SelectedValue = Session("LoginId")
            End If
        Catch ex As Exception
            ddlCreatedBy.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
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
