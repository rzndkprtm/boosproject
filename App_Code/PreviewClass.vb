Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class PreviewClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetDataRow(thisString As String) As DataRow
        Using thisConn As New SqlConnection(myConn)
            Using thisCmd As New SqlCommand(thisString, thisConn)
                Using thisAdapter As New SqlDataAdapter(thisCmd)
                    Using dt As New DataTable()
                        thisAdapter.Fill(dt)
                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        End Using
    End Function

    Public Function GetDataTable(thisString As String) As DataTable
        Using thisConn As New SqlConnection(myConn)
            Using thisCmd As New SqlCommand(thisString, thisConn)
                Using da As New SqlDataAdapter(thisCmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        End Using
    End Function

    Public Function GetListData(thisString As String) As DataSet
        Dim thisCmd As New SqlCommand(thisString)
        Using thisConn As New SqlConnection(myConn)
            Using thisAdapter As New SqlDataAdapter()
                thisCmd.Connection = thisConn
                thisAdapter.SelectCommand = thisCmd
                Using thisDataSet As New DataSet()
                    thisAdapter.Fill(thisDataSet)
                    Return thisDataSet
                End Using
            End Using
        End Using
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
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
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetItemData_Integer(thisString As String) As Integer
        Dim result As Double = 0
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Function GetFabricName(fabricId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(fabricId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("SELECT Name FROM Fabrics WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", fabricId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetFabricColourName(fabricColourId As String) As String
        Dim result As String = String.Empty
        Try
            If Not String.IsNullOrEmpty(fabricColourId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("SELECT Colour FROM FabricColours WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", fabricColourId)

                        thisConn.Open()
                        Dim obj = myCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            result = obj.ToString()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Sub BindContent(headerId As String, filePath As String)
        Using fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
            Dim headerData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, Customers.CompanyDetailId AS CompanyDetailId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
            Dim customerId As String = headerData("CustomerId").ToString()
            Dim customerName As String = headerData("CustomerName").ToString()
            Dim companyId As String = headerData("CompanyId").ToString()

            Dim orderId As String = headerData("OrderId").ToString()
            Dim orderDate As String = String.Empty
            If Not IsDBNull(headerData("CreatedDate")) Then
                orderDate = Convert.ToDateTime(headerData("CreatedDate")).ToString("dd MMM yyyy")
            End If

            Dim submitDate As String = String.Empty
            If Not IsDBNull(headerData("SubmittedDate")) Then
                submitDate = Convert.ToDateTime(headerData("SubmittedDate")).ToString("dd MMM yyyy")
            End If

            Dim orderNumber As String = headerData("OrderNumber").ToString()
            Dim orderName As String = headerData("OrderName").ToString()
            Dim orderNote As String = headerData("OrderNote").ToString()

            Dim totalItems As Integer = GetItemData_Integer("SELECT SUM(CASE WHEN Designs.Type='Blinds' THEN OrderDetails.TotalItems ELSE OrderDetails.Qty END) AS TotalItem FROM OrderDetails INNER JOIN Products ON OrderDetails.ProductId=Products.Id INNER JOIN Designs ON Products.DesignId=Designs.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1")
            Dim pageTotalItem As String = String.Format("{0} Item", totalItems)
            If totalItems > 1 Then pageTotalItem = String.Format("{0} Items", totalItems)

            Dim doc As New Document(PageSize.A4, 20, 20, 135, 50)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)

            Dim pageEvent As New PreviewEvents() With {
                .PageOrderId = orderId, .PageOrderDate = orderDate,
                .PageSubmitDate = submitDate, .PageCustomerName = customerName,
                .PageOrderNumber = orderNumber, .PageOrderName = orderName,
                .PageNote = orderNote, .PageTotalItem = pageTotalItem,
                .pageCompany = companyId
            }
            writer.PageEvent = pageEvent

            doc.Open()

            ' START ALUMINIUM BLIND
            Try
                Dim aluminiumData As DataSet = GetListData("SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.Width AS Width, OrderDetails.[Drop] AS Height, OrderDetails.ControlPosition AS CtrlPosition, OrderDetails.TilterPosition AS TiltPosition, OrderDetails.ControlLength AS CL, OrderDetails.ControlLengthValue AS CLValue, OrderDetails.WandLength AS WL, OrderDetails.WandLengthValue AS WLValue, OrderDetails.Supply, OrderDetails.Notes, OrderDetails.SubType, OrderDetails.TotalItems AS TotalItems, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, ProductColours.Name AS ColourName, 1 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND Designs.Name='Aluminium Blind' UNION ALL SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthB AS Width, OrderDetails.DropB AS Height, OrderDetails.ControlPositionB AS CtrlPosition, OrderDetails.TilterPositionB AS TiltPosition, OrderDetails.ControlLengthB AS CL, OrderDetails.ControlLengthValueB AS CLValue, OrderDetails.WandLengthB AS WL, OrderDetails.WandLengthValueB AS WLValue, OrderDetails.Supply, OrderDetails.Notes, OrderDetails.SubType, OrderDetails.TotalItems AS TotalItems, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, ProductColours.Name AS ColourName, 2 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.SubType LIKE '%2 on 1%' AND OrderDetails.Active=1 AND Designs.Name='Aluminium Blind' ORDER BY OrderDetails.Id, Item ASC")

                If Not aluminiumData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Aluminium"
                    pageEvent.PageTitle2 = "Blind"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = aluminiumData.Tables(0)
                    Dim items(14, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim cordLength As String = dt.Rows(i)("CL").ToString()
                        Dim cordLengthValue As String = dt.Rows(i)("CLValue").ToString()

                        Dim cordLengthText As String = cordLength
                        If cordLength = "Custom" Then
                            cordLengthText = String.Format("{0} : {1}mm", cordLength, cordLengthValue)
                        End If

                        Dim wandLength As String = dt.Rows(i)("WL").ToString()
                        Dim wandLengthValue As String = dt.Rows(i)("WLValue").ToString()

                        Dim wandLengthText As String = wandLength
                        If wandLength = "Custom" Then
                            wandLengthText = String.Format("{0} : {1}mm", wandLength, wandLengthValue)
                        End If

                        Dim totalBlinds As Integer = dt.Rows(i)("TotalItems")
                        Dim subType As String = "Single"
                        If totalBlinds > 1 Then subType = "2 on 1"

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("BlindAlias").ToString()
                        items(4, i) = dt.Rows(i)("ColourName").ToString()
                        items(5, i) = subType
                        items(6, i) = dt.Rows(i)("Width").ToString()
                        items(7, i) = dt.Rows(i)("Height").ToString()
                        items(8, i) = dt.Rows(i)("CtrlPosition").ToString()
                        items(9, i) = dt.Rows(i)("TiltPosition").ToString()
                        items(10, i) = cordLengthText
                        items(11, i) = wandLengthText
                        items(12, i) = dt.Rows(i)("Supply").ToString()
                        items(13, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Aluminium Type", "Aluminium Colour", "Sub Type", "Width (mm)", "Drop (mm)", "Control Position", "Tilter Position", "Cord Length", "Wand Length", "Supply", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 22
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END ALUMINIUM BLIND

            ' START CELLULAR SHADES
            Try
                Dim cellularData As DataSet = GetListData("SELECT OrderDetails.*, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, ProductControls.Name AS ControlName, Fab.Name AS FabricName, FabB.Name AS FabricNameB, FabColour.Colour AS FabricColour, FabColourB.Colour AS FabricColourB FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductControls ON Products.ControlType=ProductControls.Id LEFT JOIN Fabrics AS Fab ON OrderDetails.FabricId=Fab.Id LEFT JOIN Fabrics AS FabB ON OrderDetails.FabricIdB=FabB.Id LEFT JOIN FabricColours AS FabColour ON OrderDetails.FabricColourId=FabColour.Id LEFT JOIN FabricColours AS FabColourB ON OrderDetails.FabricColourIdB=FabColourB.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Cellular Shades' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not cellularData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Cellular"
                    pageEvent.PageTitle2 = "Shades"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = cellularData.Tables(0)
                    Dim items(15, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim cordLength As String = dt.Rows(i)("ControlLength").ToString()
                        Dim cordLengthValue As String = dt.Rows(i)("ControlLengthValue").ToString()

                        Dim cordLengthText As String = cordLength
                        If cordLength = "Custom" Then
                            cordLengthText = String.Format("{0} : {1}mm", cordLength, cordLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("BlindAlias").ToString()
                        items(4, i) = dt.Rows(i)("ControlName").ToString()
                        items(5, i) = dt.Rows(i)("Width").ToString()
                        items(6, i) = dt.Rows(i)("Drop").ToString()
                        items(7, i) = dt.Rows(i)("FabricName").ToString()
                        items(8, i) = dt.Rows(i)("FabricColour").ToString()
                        items(9, i) = dt.Rows(i)("FabricNameB").ToString()
                        items(10, i) = dt.Rows(i)("FabricColourB").ToString()
                        items(11, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(12, i) = cordLengthText
                        items(13, i) = dt.Rows(i)("Supply").ToString()
                        items(14, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Cellular Type", "Control Type", "Width (mm)", "Drop (mm)", "Fabric Type", "Fabric Colour", "Fabric Type (N)", "Fabric Colour (N)", "Control Position", "Control Length", "Hold Down Clip", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END CELLULAR SHADES

            ' START CURTAIN
            Try
                Dim curtainData As DataSet = GetListData("SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.Width AS Width, OrderDetails.[Drop] AS Height, OrderDetails.FabricId AS FabricId, OrderDetails.FabricColourId AS FabricColourId, OrderDetails.Heading AS Heading, OrderDetails.TrackType AS TrackType, OrderDetails.TrackColour AS TrackColour, OrderDetails.TrackDraw AS TrackDraw, OrderDetails.StackPosition AS StackPosition, OrderDetails.ControlColour AS ControlColour, OrderDetails.ControlLengthValue AS CL, OrderDetails.ReturnLengthValue AS RetLengthValue, OrderDetails.BottomHem, OrderDetails.Supply AS Supply, OrderDetails.Notes, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, 1 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND Designs.Name='Curtain' UNION ALL SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthB AS Width, OrderDetails.DropB AS Height, OrderDetails.FabricIdB AS FabricId, OrderDetails.FabricColourIdB AS FabricColourId, OrderDetails.HeadingB AS Heading, OrderDetails.TrackTypeB AS TrackType, OrderDetails.TrackColourB AS TrackColour, OrderDetails.TrackDrawB AS TrackDraw, OrderDetails.StackPositionB AS StackPosition, OrderDetails.ControlColourB AS ControlColour, OrderDetails.ControlLengthValueB AS CL, OrderDetails.ReturnLengthValueB AS RetLengthValue, OrderDetails.BottomHem, OrderDetails.Supply AS Supply, OrderDetails.Notes, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, 2 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND Designs.Name='Curtain' AND Blinds.Name='Double Curtain & Track' ORDER BY OrderDetails.Id, Item ASC")

                If Not curtainData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Curtain"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = curtainData.Tables(0)
                    Dim items(19, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim fabricId As String = dt.Rows(i)("FabricId").ToString()
                        Dim fabricName As String = GetFabricName(fabricId)

                        Dim fabricColourId As String = dt.Rows(i)("FabricColourId").ToString()
                        Dim fabricColourName As String = GetFabricColourName(fabricColourId)

                        Dim controlLength As String = dt.Rows(i)("CL").ToString()
                        If controlLength = "0" Then controlLength = String.Empty

                        Dim returnLength As String = dt.Rows(i)("RetLengthValue").ToString()
                        If returnLength = "0" Then returnLength = String.Empty

                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("BlindAlias").ToString()
                        items(4, i) = dt.Rows(i)("Heading").ToString()
                        items(5, i) = fabricName
                        items(6, i) = fabricColourName
                        items(7, i) = dt.Rows(i)("Width").ToString()
                        items(8, i) = dt.Rows(i)("Height").ToString()
                        items(9, i) = dt.Rows(i)("TrackType").ToString()
                        items(10, i) = dt.Rows(i)("TrackColour").ToString()
                        items(11, i) = dt.Rows(i)("TrackDraw").ToString()
                        items(12, i) = dt.Rows(i)("StackPosition").ToString()
                        items(13, i) = dt.Rows(i)("ControlColour").ToString()
                        items(14, i) = controlLength
                        items(15, i) = returnLength
                        items(16, i) = dt.Rows(i)("BottomHem").ToString()
                        items(17, i) = dt.Rows(i)("Supply").ToString()
                        items(18, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Fitting", "Curtain Type", "Heading", "Fabric Type", "Fabric Colour", "Width (mm)", "Drop (mm)", "Track Type", "Track Colour", "Track Draw", "Stack Position", "Control Colour", "Control Length", "Return Length (mm)", "Bottom HEM", "Tie Back Req", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END CURTAIN

            ' START DESIGN SHADES
            Try
                Dim designData As DataSet = GetListData("SELECT OrderDetails.*, Chains.Name AS ChainName, Fabrics.Name AS FabricName, FabricColours.Colour AS FabricColour, ProductControls.Name AS ControlName, ProductColours.Name AS ColourName FROM OrderDetails LEFT JOIN FabricColours ON OrderDetails.FabricColourId=FabricColours.Id LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN ProductControls ON Products.ControlType=ProductControls.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id LEFT JOIN Fabrics ON OrderDetails.FabricId=Fabrics.Id LEFT JOIN Chains ON OrderDetails.ChainId=Chains.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Design Shades' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not designData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Design"
                    pageEvent.PageTitle2 = "Shades"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = designData.Tables(0)
                    Dim items(14, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlName As String = dt.Rows(i)("ControlName").ToString()
                        Dim controlColour As String = String.Empty
                        If controlName = "Chain" Then
                            controlColour = dt.Rows(i)("ChainName").ToString()
                        End If
                        If controlName = "Wand" Then
                            controlColour = dt.Rows(i)("WandColour").ToString()
                        End If

                        Dim controlLength As String = dt.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = dt.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = dt.Rows(i)("FabricName").ToString()
                        items(6, i) = dt.Rows(i)("FabricColour").ToString()
                        items(7, i) = dt.Rows(i)("ColourName").ToString()
                        items(8, i) = dt.Rows(i)("StackPosition").ToString()
                        items(9, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(10, i) = controlName
                        items(11, i) = controlColour
                        items(12, i) = controlLengthText
                        items(13, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Fabric Type", "Fabric Colour", "Track Colour", "Stack Position", "Control Position", "Control Type", "Control Colour", "Control Length", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END DESIGN SHADES

            ' START LINEA VALANCE
            Try
                Dim lineaData As DataSet = GetListData("SELECT OrderDetails.*, ProductTubes.Name AS TubeName, ProductColours.Name AS ColourName, Fabrics.Name AS FabricName, FabricColours.Colour AS FabricColour FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN ProductTubes ON Products.TubeType=ProductTubes.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id LEFT JOIN Fabrics ON OrderDetails.FabricId=Fabrics.Id LEFT JOIN FabricColours ON OrderDetails.FabricColourId=FabricColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Linea Valance' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not lineaData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Linea"
                    pageEvent.PageTitle2 = "Valance"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = lineaData.Tables(0)
                    Dim items(14, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim returnLength As String = dt.Rows(i)("ReturnLength").ToString()
                        Dim returnLengthValue As String = dt.Rows(i)("ReturnLengthValue").ToString()

                        Dim returnLengthText As String = returnLength
                        If returnLength = "Custom" Then
                            returnLengthText = String.Format("{0} : {1}mm", returnLength, returnLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("TubeName").ToString()
                        items(5, i) = dt.Rows(i)("ColourName").ToString()
                        items(6, i) = dt.Rows(i)("FabricInsert").ToString()
                        items(7, i) = dt.Rows(i)("FabricName").ToString()
                        items(8, i) = dt.Rows(i)("FabricColour").ToString()
                        items(9, i) = dt.Rows(i)("BracketType").ToString()
                        items(10, i) = dt.Rows(i)("IsBlindIn").ToString()
                        items(11, i) = dt.Rows(i)("ReturnPosition").ToString()
                        items(12, i) = returnLengthText
                        items(13, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Valance Type", "Valance Colour", "Width (mm)", "Fabric Insert", "Fabric Type", "Fabric Colour", "Bracket Type", "Is Blind In", "Return Position", "Return Length", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END LINEA VALANCE

            ' START PANEL GLIDE
            Try
                Dim panelData As DataSet = GetListData("SELECT OrderDetails.*, Blinds.Name AS BlindName, ProductTubes.Name AS TubeName, ProductColours.Name AS ColourName, Fabrics.Name AS FabricName, FabricColours.Colour AS FabricColour FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductTubes ON Products.TubeType=ProductTubes.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id LEFT JOIN Fabrics ON OrderDetails.FabricId=Fabrics.Id LEFT JOIN FabricColours ON OrderDetails.FabricColourId=FabricColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Panel Glide' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not panelData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Panel"
                    pageEvent.PageTitle2 = "Glide"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = panelData.Tables(0)
                    Dim items(17, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim layoutCode As String = dt.Rows(i)("LayoutCode").ToString()
                        If dt.Rows(i)("LayoutCode") = "Custom" Then
                            layoutCode = dt.Rows(i)("LayoutCodeCustom").ToString()
                        End If

                        Dim wandLength As String = dt.Rows(i)("WandLength").ToString()
                        Dim wandLengthValue As String = dt.Rows(i)("WandLengthValue").ToString()

                        Dim wandLengthText As String = wandLength
                        If wandLength = "Custom" Then
                            wandLengthText = String.Format("{0} : {1}mm", wandLength, wandLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("BlindName").ToString()
                        items(4, i) = dt.Rows(i)("TubeName").ToString()
                        items(5, i) = dt.Rows(i)("ColourName").ToString()
                        items(6, i) = dt.Rows(i)("FabricName").ToString()
                        items(7, i) = dt.Rows(i)("FabricColour").ToString()
                        items(8, i) = dt.Rows(i)("Width").ToString()
                        items(9, i) = If(dt.Rows(i)("Drop").ToString() <> "" AndAlso dt.Rows(i)("Drop").ToString() <> "0", dt.Rows(i)("Drop").ToString(), "")
                        items(10, i) = dt.Rows(i)("WandColour").ToString()
                        items(11, i) = wandLengthText
                        items(12, i) = If(dt.Rows(i)("PanelQty").ToString() <> "" AndAlso dt.Rows(i)("PanelQty").ToString() <> "0", dt.Rows(i)("PanelQty").ToString(), "")
                        items(13, i) = dt.Rows(i)("TrackType").ToString()
                        items(14, i) = layoutCode
                        items(15, i) = dt.Rows(i)("Batten").ToString()
                        items(16, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Panel System", "Panel Style", "Track Colour", "Fabric Type", "Fabric Colour", "Width (mm)", "Drop (mm)", "Wand Colour", "Wand Length", "Panel Qty", "Track Type", "Layout Code", "Batten Colour", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END PANEL GLIDE

            ' START PELMET
            Try
                Dim pelmetData As DataSet = GetListData("SELECT OrderDetails.*, ProductTubes.Name AS TubeName, Fabrics.Name AS FabricName, FabricColours.Colour AS FabricColour FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN ProductTubes ON Products.TubeType=ProductTubes.Id LEFT JOIN Fabrics ON OrderDetails.FabricId=Fabrics.Id LEFT JOIN FabricColours ON OrderDetails.FabricColourId=FabricColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Pelmet' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not pelmetData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Pelmet"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = pelmetData.Tables(0)
                    Dim items(15, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("TubeName").ToString()
                        items(4, i) = dt.Rows(i)("FabricName").ToString()
                        items(5, i) = dt.Rows(i)("FabricColour").ToString()
                        items(6, i) = dt.Rows(i)("Batten").ToString()
                        items(7, i) = dt.Rows(i)("LayoutCode").ToString()
                        items(8, i) = dt.Rows(i)("Width").ToString()
                        items(9, i) = If(dt.Rows(i)("WidthB").ToString() <> "" AndAlso dt.Rows(i)("WidthB").ToString() <> "0", dt.Rows(i)("WidthB").ToString(), "")
                        items(10, i) = If(dt.Rows(i)("WidthC").ToString() <> "" AndAlso dt.Rows(i)("WidthC").ToString() <> "0", dt.Rows(i)("WidthC").ToString(), "")
                        items(11, i) = dt.Rows(i)("ReturnPosition").ToString()
                        items(12, i) = dt.Rows(i)("ReturnLengthValue").ToString()
                        items(13, i) = dt.Rows(i)("ReturnLengthValueB").ToString()
                        items(14, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Pelmet Type", "Fabric Type", "Fabric Colour", "Batten Colour", "Pelmet Layout", "Width (mm)", "2nd Width (mm)", "3rd Width (mm)", "Return Position", "Return Length (L)", "Return Length (R)", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END PELMET

            ' START PRIVACY VENETIAN
            Try
                Dim privacyData As DataSet = GetListData("SELECT OrderDetails.*, Blinds.Name AS BlindName, ProductColours.Name AS ColourName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Privacy Venetian' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not privacyData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Privacy"
                    pageEvent.PageTitle2 = "Venetian"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = privacyData.Tables(0)
                    Dim items(12, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlLength As String = dt.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = dt.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("BlindName").ToString()
                        items(4, i) = dt.Rows(i)("ColourName").ToString()
                        items(5, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(6, i) = dt.Rows(i)("TilterPosition").ToString()
                        items(7, i) = dt.Rows(i)("Width").ToString()
                        items(8, i) = dt.Rows(i)("Drop").ToString()
                        items(9, i) = controlLengthText
                        items(10, i) = dt.Rows(i)("Supply").ToString()
                        items(11, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Privacy Type", "Privacy Colour", "Control Position", "Tilter Position", "Width (mm)", "Drop (mm)", "Cord Length", "Hold Down Clip", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END PRIVACY VENETIAN

            ' START ROLLER BLIND
            Try
                Dim rollerData As DataSet = GetListData("SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.Width AS Width, OrderDetails.[Drop] AS Height, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricId AS Fabric, OrderDetails.FabricColourId AS FabricColour, OrderDetails.Roll, OrderDetails.ControlPosition, OrderDetails.Charger AS Charger, OrderDetails.ExtensionCable AS ExtensionCable, OrderDetails.Supply AS NeoBox, OrderDetails.Adjusting AS Adjusting, OrderDetails.ChainId AS Chain, OrderDetails.ChainStopper AS ChainStopper, OrderDetails.ControlLengthValue AS ChainLength, OrderDetails.BottomId AS BottomType, OrderDetails.BottomColourId AS BottomColour, OrderDetails.FlatOption AS FlatOption, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.BracketExtension AS BracketExtension, OrderDetails.Printing AS PrintingImage, OrderDetails.Notes, 1 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Roller Blind' AND (Blinds.Name='Single Blind' OR Blinds.Name='Full Cassette' OR Blinds.Name='Semi Cassette' OR Blinds.Name='Wire Guide' OR Blinds.Name='Dual Blinds' OR Blinds.Name='Link 2 Blinds Dependent' OR Blinds.Name='Link 2 Blinds Independent' OR Blinds.Name='Link 3 Blinds Dependent' OR Blinds.Name='Link 3 Blinds Independent with Dependent' OR Blinds.Name='DB Link 2 Blinds Dependent' OR Blinds.Name='DB Link 2 Blinds Independent' OR Blinds.Name='DB Link 3 Blinds Dependent' OR Blinds.Name='DB Link 3 Blinds Independent with Dependent') AND OrderDetails.Active=1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthB AS Width, OrderDetails.DropB AS Height, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricIdB AS Fabric, OrderDetails.FabricColourIdB AS FabricColour, OrderDetails.RollB, OrderDetails.ControlPositionB, OrderDetails.Charger AS Charger, OrderDetails.ExtensionCable AS ExtensionCable, OrderDetails.Supply AS NeoBox, OrderDetails.Adjusting AS Adjusting, OrderDetails.ChainIdB AS Chain, OrderDetails.ChainStopperB AS ChainStopper, OrderDetails.ControlLengthValueB AS ChainLength, OrderDetails.BottomIdB AS BottomType, OrderDetails.BottomColourIdB AS BottomColour, OrderDetails.FlatOptionB AS FlatOption, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.BracketExtension AS BracketExtension, OrderDetails.PrintingB AS PrintingImage, OrderDetails.Notes, 2 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Roller Blind' AND (Blinds.Name='Dual Blinds' OR Blinds.Name='Link 2 Blinds Dependent' OR Blinds.Name='Link 2 Blinds Independent' OR Blinds.Name='Link 3 Blinds Dependent' OR Blinds.Name='Link 3 Blinds Independent with Dependent' OR Blinds.Name='DB Link 2 Blinds Dependent' OR Blinds.Name='DB Link 2 Blinds Independent' OR Blinds.Name='DB Link 3 Blinds Dependent' OR Blinds.Name='DB Link 3 Blinds Independent with Dependent') AND OrderDetails.Active=1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthC AS Width, OrderDetails.DropC AS Height, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricIdC AS Fabric, OrderDetails.FabricColourIdC AS FabricColour, OrderDetails.RollC, OrderDetails.ControlPositionC, OrderDetails.Charger AS Charger, OrderDetails.ExtensionCable AS ExtensionCable, OrderDetails.Supply AS NeoBox, OrderDetails.Adjusting AS Adjusting, OrderDetails.ChainIdC AS Chain, OrderDetails.ChainStopperC AS ChainStopper, OrderDetails.ControlLengthValueC AS ChainLength, OrderDetails.BottomIdC AS BottomType, OrderDetails.BottomColourIdC AS BottomColour, OrderDetails.FlatOptionC AS FlatOption, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.BracketExtension AS BracketExtension, OrderDetails.PrintingC AS PrintingImage, OrderDetails.Notes, 3 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Roller Blind' AND (Blinds.Name='Link 3 Blinds Dependent' OR Blinds.Name='Link 3 Blinds Independent with Dependent' OR Blinds.Name='DB Link 2 Blinds Dependent' OR Blinds.Name='DB Link 2 Blinds Independent' OR Blinds.Name='DB Link 3 Blinds Dependent' OR Blinds.Name='DB Link 3 Blinds Independent with Dependent') AND OrderDetails.Active=1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthD AS Width, OrderDetails.DropD AS Height, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricIdC AS Fabric, OrderDetails.FabricColourIdD AS FabricColour, OrderDetails.RollD, OrderDetails.ControlPositionD, OrderDetails.Charger AS Charger, OrderDetails.ExtensionCable AS ExtensionCable, OrderDetails.Supply AS NeoBox, OrderDetails.Adjusting AS Adjusting, OrderDetails.ChainIdD AS Chain, OrderDetails.ChainStopperD AS ChainStopper, OrderDetails.ControlLengthValueD AS ChainLength, OrderDetails.BottomIdD AS BottomType, OrderDetails.BottomColourIdD AS BottomColour, OrderDetails.FlatOptionD AS FlatOption, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.BracketExtension AS BracketExtension, OrderDetails.PrintingD AS PrintingImage, OrderDetails.Notes, 4 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Roller Blind' AND (Blinds.Name='DB Link 2 Blinds Dependent' OR Blinds.Name='DB Link 2 Blinds Independent' OR Blinds.Name='DB Link 3 Blinds Dependent' OR Blinds.Name='DB Link 3 Blinds Independent with Dependent') AND OrderDetails.Active=1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthE AS Width, OrderDetails.DropE AS Height, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricIdE AS Fabric, OrderDetails.FabricColourIdE AS FabricColour, OrderDetails.RollE, OrderDetails.ControlPositionE, OrderDetails.Charger AS Charger, OrderDetails.ExtensionCable AS ExtensionCable, OrderDetails.Supply AS NeoBox, OrderDetails.Adjusting AS Adjusting, OrderDetails.ChainIdE AS Chain, OrderDetails.ChainStopperE AS ChainStopper, OrderDetails.ControlLengthValueE AS ChainLength, OrderDetails.BottomIdE AS BottomType, OrderDetails.BottomColourIdE AS BottomColour, OrderDetails.FlatOptionE AS FlatOption, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.BracketExtension AS BracketExtension, OrderDetails.PrintingE AS PrintingImage, OrderDetails.Notes, 5 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId =Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Roller Blind' AND (Blinds.Name='DB Link 3 Blinds Dependent' OR Blinds.Name='DB Link 3 Blinds Independent with Dependent') AND OrderDetails.Active=1 UNION ALL SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthF AS Width, OrderDetails.DropF AS Height, Blinds.Name AS BlindName, Blinds.Alias AS BlindAlias, Products.TubeType AS TubeType, Products.ControlType, Products.ColourType, OrderDetails.FabricIdF AS Fabric, OrderDetails.FabricColourIdF AS FabricColour, OrderDetails.RollF, OrderDetails.ControlPositionF, OrderDetails.Charger AS Charger, OrderDetails.ExtensionCable AS ExtensionCable, OrderDetails.Supply AS NeoBox, OrderDetails.Adjusting AS Adjusting, OrderDetails.ChainIdF AS Chain, OrderDetails.ChainStopperF AS ChainStopper, OrderDetails.ControlLengthValueF AS ChainLength, OrderDetails.BottomIdF AS BottomType, OrderDetails.BottomColourIdF AS BottomColour, OrderDetails.FlatOptionF AS FlatOption, OrderDetails.SpringAssist, OrderDetails.BracketSize, OrderDetails.BracketExtension AS BracketExtension, OrderDetails.PrintingF AS PrintingImage, OrderDetails.Notes, 6 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Roller Blind' AND (Blinds.Name='DB Link 3 Blinds Dependent' OR Blinds.Name='DB Link 3 Blinds Independent with Dependent') AND OrderDetails.Active=1 ORDER BY OrderDetails.Id, Item ASC")

                If Not rollerData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Roller"
                    pageEvent.PageTitle2 = "Blind"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = rollerData.Tables(0)
                    Dim items(29, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim itemBlind As String = dt.Rows(i)("Item").ToString()

                        Dim blindName As String = dt.Rows(i)("BlindName").ToString()
                        Dim blindAlias As String = dt.Rows(i)("BlindAlias").ToString()

                        Dim tubeType As String = dt.Rows(i)("TubeType").ToString()

                        Dim tubeName As String = GetItemData("SELECT Name FROM ProductTubes WHERE Id='" & dt.Rows(i)("TubeType").ToString() & "'")
                        Dim tubeAlias As String = GetItemData("SELECT Alias FROM ProductTubes WHERE Id='" & dt.Rows(i)("TubeType").ToString() & "'")
                        Dim controlName As String = GetItemData("SELECT Name FROM ProductControls WHERE Id='" & dt.Rows(i)("ControlType").ToString() & "'")
                        Dim controlAlias As String = GetItemData("SELECT Name FROM ProductControls WHERE Id='" & dt.Rows(i)("ControlType").ToString() & "'")
                        Dim colourName As String = GetItemData("SELECT Name FROM ProductColours WHERE Id='" & dt.Rows(i)("ColourType").ToString() & "'")

                        If tubeType = "Standard" Then

                        End If

                        Dim fabricType As String = GetItemData("SELECT Name FROM Fabrics WHERE Id='" & dt.Rows(i)("Fabric").ToString() & "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id='" & dt.Rows(i)("FabricColour").ToString() & "'")

                        Dim chainName As String = GetItemData("SELECT Name FROM Chains WHERE Id='" & dt.Rows(i)("Chain").ToString() & "'")

                        Dim chainColour As String = String.Empty
                        Dim remoteType As String = chainName

                        If controlName = "Chain" Then
                            chainColour = chainName : remoteType = ""
                        End If

                        Dim chainLength As String = String.Empty
                        Dim chainLengthValue As Integer = dt.Rows(i)("ChainLength")
                        If chainLengthValue > 0 Then
                            chainLength = dt.Rows(i)("ChainLength").ToString()
                        End If

                        Dim bottomType As String = GetItemData("SELECT Name FROM Bottoms WHERE Id='" & dt.Rows(i)("BottomType").ToString() & "'")
                        Dim bottomColour As String = GetItemData("SELECT Colour FROM BottomColours WHERE Id='" & dt.Rows(i)("BottomColour").ToString() & "'")
                        If blindName = "Full Cassette" Then
                            bottomType = "Cassette" : bottomColour = "White"
                        End If

                        Dim rollerType As String = blindAlias
                        If blindName = "Link 2 Blinds Dependent" OrElse blindName = "Link 2 Blinds Independent" Then
                            If itemBlind = "1" Then rollerType = String.Format("{0} (C)", blindAlias)
                            If itemBlind = "2" Then rollerType = String.Format("{0} (E)", blindAlias)
                        End If

                        If blindName = "Link 3 Blinds Dependent" Then
                            If itemBlind = "1" Then rollerType = String.Format("{0} (C)", blindAlias)
                            If itemBlind = "2" Then rollerType = String.Format("{0} (M)", blindAlias)
                            If itemBlind = "3" Then rollerType = String.Format("{0} (E)", blindAlias)
                        End If

                        If blindName = "Link 3 Blinds Independent with Dependent" Then
                            If itemBlind = "1" Then rollerType = String.Format("{0} (C) (IND)", blindAlias)
                            If itemBlind = "2" Then rollerType = String.Format("{0} (M)", blindAlias)
                            If itemBlind = "3" Then rollerType = String.Format("{0} (E)", blindAlias)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = rollerType
                        items(4, i) = tubeAlias
                        items(5, i) = controlName
                        items(6, i) = colourName
                        items(7, i) = fabricType
                        items(8, i) = fabricColour
                        items(9, i) = dt.Rows(i)("Roll").ToString()
                        items(10, i) = dt.Rows(i)("Width").ToString()
                        items(11, i) = dt.Rows(i)("Height").ToString()
                        items(12, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(13, i) = remoteType
                        items(14, i) = dt.Rows(i)("Charger").ToString()
                        items(15, i) = dt.Rows(i)("ExtensionCable").ToString()
                        items(16, i) = dt.Rows(i)("NeoBox").ToString()
                        items(17, i) = chainColour
                        items(18, i) = dt.Rows(i)("ChainStopper").ToString()
                        items(19, i) = chainLength
                        items(20, i) = bottomType
                        items(21, i) = bottomColour
                        items(22, i) = dt.Rows(i)("FlatOption").ToString()
                        items(23, i) = dt.Rows(i)("BracketExtension").ToString()
                        items(24, i) = dt.Rows(i)("SpringAssist").ToString()
                        items(25, i) = dt.Rows(i)("BracketSize").ToString()
                        items(26, i) = dt.Rows(i)("Adjusting").ToString()
                        items(27, i) = dt.Rows(i)("PrintingImage").ToString()
                        items(28, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Roller Type", "Tube Type", "Control Type", "Bracket Colour", "Fabric Type", "Fabric Colour", "Roll Direction", "Width (mm)", "Drop (mm)", "Control Position", "Remote Type", "Battery Charger", "Extension Cable", "Neo Box", "Chain Type", "Chain Stopper", "Chain Length", "Bottom Type", "Bottom Colour", "Bottom Option", "Bracket Extension", "Spring Assist", "Bracket Size", "Adjusting Spanner", "Fabric Printing", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 22
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            'End ROLLER BLIND

            ' START ROMAN BLIND
            Try
                Dim romanData As DataSet = GetListData("SELECT OrderDetails.*, ProductTubes.Name AS TubeName, ProductControls.Name AS ControlName, Fabrics.Name AS FabricName, FabricColours.Colour AS FabricColour, Chains.Name AS ChainName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN ProductTubes ON Products.TubeType = ProductTubes.Id LEFT JOIN ProductControls ON Products.ControlType=ProductControls.Id LEFT JOIN Fabrics ON OrderDetails.FabricId=Fabrics.Id LEFT JOIN FabricColours ON OrderDetails.FabricColourId=FabricColours.Id LEFT JOIN Chains ON OrderDetails.ChainId=Chains.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Roman Blind' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not romanData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Roman"
                    pageEvent.PageTitle2 = "Blind"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = romanData.Tables(0)
                    Dim items(19, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlName As String = dt.Rows(i)("ControlName").ToString()

                        Dim chainName As String = GetItemData("SELECT Name FROM Chains WHERE Id='" & dt.Rows(i)("ChainId").ToString() & "'")

                        Dim chainColour As String = String.Empty
                        Dim remoteType As String = String.Empty

                        If controlName = "Chain" OrElse controlName = "Reg Cord Lock" Then
                            chainColour = chainName
                        End If

                        If controlName.Contains("Alpha") OrElse controlName = "Mercure" OrElse controlName = "Altus" OrElse controlName = "Sonesse 30 WF" Then
                            remoteType = chainName
                        End If

                        Dim controlLength As String = dt.Rows(i)("ControlLength").ToString()

                        Dim controlLengthValue As String = dt.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("FabricName").ToString()
                        items(5, i) = dt.Rows(i)("FabricColour").ToString()
                        items(6, i) = dt.Rows(i)("Drop").ToString()
                        items(7, i) = dt.Rows(i)("TubeName").ToString()
                        items(8, i) = dt.Rows(i)("ControlName").ToString()
                        items(9, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(10, i) = chainColour
                        items(11, i) = controlLengthText
                        items(12, i) = remoteType
                        items(13, i) = dt.Rows(i)("Charger").ToString()
                        items(14, i) = dt.Rows(i)("ExtensionCable").ToString()
                        items(15, i) = dt.Rows(i)("Supply").ToString()
                        items(16, i) = dt.Rows(i)("ValanceOption").ToString()
                        items(17, i) = dt.Rows(i)("Batten").ToString()
                        items(18, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Fabric Type", "Fabric Colour", "Width (mm)", "Drop (mm)", "Roman Style", "Control Type", "Control Position", "Control Colour", "Control Length", "Remote Type", "Motor Charger", "Extension Cable", "Neo Box", "Valance Option", "Batten Colour", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END ROMAN BLIND

            ' START VENETIAN BLIND
            Try
                Dim venetianData As DataSet = GetListData("SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.Width AS Width, OrderDetails.[Drop] AS Height, OrderDetails.ControlPosition AS CtrlPosition, OrderDetails.TilterPosition AS TiltPosition, OrderDetails.ControlLength AS CL, OrderDetails.ControlLengthValue AS CLValue, OrderDetails.Supply, OrderDetails.Tassel, OrderDetails.ValanceType, OrderDetails.ValanceSize, OrderDetails.ValanceSizeValue, OrderDetails.ReturnPosition, OrderDetails.ReturnLength, OrderDetails.ReturnLengthValue, OrderDetails.Notes, OrderDetails.SubType, Blinds.Name AS BlindName, ProductColours.Name AS ColourName, 1 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.Active=1 AND Designs.Name='Venetian Blind' UNION ALL SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthB AS Width, OrderDetails.DropB AS Height, OrderDetails.ControlPositionB AS CtrlPosition, OrderDetails.TilterPositionB AS TiltPosition, OrderDetails.ControlLengthB AS CL, OrderDetails.ControlLengthValueB AS CLValue, OrderDetails.Supply, OrderDetails.Tassel, OrderDetails.ValanceType, OrderDetails.ValanceSize, OrderDetails.ValanceSizeValue, OrderDetails.ReturnPosition, OrderDetails.ReturnLength, OrderDetails.ReturnLengthValue, OrderDetails.Notes, OrderDetails.SubType, Blinds.Name AS BlindName, ProductColours.Name AS ColourName, 2 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND OrderDetails.SubType LIKE '%2 on 1%' AND OrderDetails.Active=1 AND Designs.Name='Venetian Blind' UNION ALL SELECT OrderDetails.Id, OrderDetails.Room, OrderDetails.Mounting, OrderDetails.WidthC AS Width, OrderDetails.DropC AS Height, OrderDetails.ControlPositionC AS CtrlPosition, OrderDetails.TilterPositionC AS TiltPosition, OrderDetails.ControlLengthC AS CL, OrderDetails.ControlLengthValueC AS CLValue, OrderDetails.Supply, OrderDetails.Tassel, OrderDetails.ValanceType, OrderDetails.ValanceSize, OrderDetails.ValanceSizeValue, OrderDetails.ReturnPosition, OrderDetails.ReturnLength, OrderDetails.ReturnLengthValue, OrderDetails.Notes, OrderDetails.SubType, Blinds.Name AS BlindName, ProductColours.Name AS ColourName, 3 AS Item FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType = ProductColours.Id WHERE OrderDetails.HeaderId='" + headerId + "' AND OrderDetails.SubType LIKE '%3 on 1%' AND OrderDetails.Active=1 AND Designs.Name='Venetian Blind' ORDER BY OrderDetails.Id, Item ASC")

                If Not venetianData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Venetian"
                    pageEvent.PageTitle2 = "Blind"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = venetianData.Tables(0)
                    Dim items(18, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlLength As String = dt.Rows(i)("CL").ToString()
                        Dim controlLengthValue As String = dt.Rows(i)("CLValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        Dim valancesize As String = dt.Rows(i)("ValanceSize").ToString()
                        Dim valancesizeValue As String = dt.Rows(i)("ValanceSizeValue").ToString()

                        Dim valancesizeText As String = valancesize
                        If valancesize = "Custom" Then
                            valancesizeText = String.Format("{0} : {1}mm", valancesize, valancesizeValue)
                        End If

                        Dim returnLength As String = dt.Rows(i)("ReturnLength").ToString()
                        Dim returnLengthValue As String = dt.Rows(i)("ReturnLengthValue").ToString()

                        Dim returnLengthText As String = returnLength
                        If returnLength = "Custom" Then
                            returnLengthText = String.Format("{0} : {1}mm", returnLength, returnLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("BlindName").ToString()
                        items(4, i) = dt.Rows(i)("ColourName").ToString()
                        items(5, i) = dt.Rows(i)("SubType").ToString()
                        items(6, i) = dt.Rows(i)("Width").ToString()
                        items(7, i) = dt.Rows(i)("Height").ToString()
                        items(8, i) = dt.Rows(i)("Tassel").ToString()
                        items(9, i) = dt.Rows(i)("CtrlPosition").ToString()
                        items(10, i) = dt.Rows(i)("TiltPosition").ToString()
                        items(11, i) = controlLengthText
                        items(12, i) = dt.Rows(i)("ValanceType").ToString()
                        items(13, i) = valancesizeText
                        items(14, i) = dt.Rows(i)("ReturnPosition").ToString()
                        items(15, i) = returnLengthText
                        items(16, i) = dt.Rows(i)("Supply").ToString()
                        items(17, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Venetian Type", "Venetian Colour", "Sub Type", "Width (mm)", "Drop (mm)", "Tassel Colour", "Control Position", "Tilter Position", "Control Length", "Valance Type", "Valance Size", "Return Position", "Return Length", "Supply", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 22
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 22
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 22
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END VENETIAN BLIND

            ' START VERTICAL
            Try
                Dim verticalData As DataSet = GetListData("SELECT OrderDetails.*, Blinds.Name AS BlindName, ProductTubes.Name AS TubeName, ProductControls.Name AS ControlName, ProductColours.Name AS ColourName, Fabrics.Name AS FabricName, FabricColours.Colour AS FabricColour, Chains.Name AS ChainName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductTubes ON Products.TubeType=ProductTubes.Id LEFT JOIN ProductControls ON Products.ControlType=ProductControls.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id LEFT JOIN Fabrics ON OrderDetails.FabricId=Fabrics.Id LEFT JOIN FabricColours ON OrderDetails.FabricColourId=FabricColours.Id LEFT JOIN Chains ON OrderDetails.ChainId=Chains.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Vertical' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not verticalData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Vertical"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = verticalData.Tables(0)
                    Dim items(21, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlName As String = dt.Rows(i)("ControlName").ToString()
                        Dim controlColour As String = String.Empty

                        If controlName = "Chain" Then controlColour = dt.Rows(i)("ChainName").ToString()
                        If controlName = "Wand" Then controlColour = dt.Rows(i)("WandColour").ToString()

                        Dim controlLength As String = dt.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = dt.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("BlindName").ToString()
                        items(4, i) = dt.Rows(i)("TubeName").ToString()
                        items(5, i) = If(dt.Rows(i)("QtyBlade").ToString() <> "" AndAlso dt.Rows(i)("QtyBlade").ToString() <> "0", dt.Rows(i)("QtyBlade").ToString(), "")
                        items(6, i) = dt.Rows(i)("FabricInsert").ToString()
                        items(7, i) = dt.Rows(i)("FabricName").ToString()
                        items(8, i) = dt.Rows(i)("FabricColour").ToString()
                        items(9, i) = dt.Rows(i)("Width").ToString()
                        items(10, i) = If(dt.Rows(i)("Drop").ToString() <> "" AndAlso dt.Rows(i)("Drop").ToString() <> "0", dt.Rows(i)("Drop").ToString(), "")
                        items(11, i) = dt.Rows(i)("StackPosition").ToString()
                        items(12, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(13, i) = controlName
                        items(14, i) = dt.Rows(i)("ColourName").ToString()
                        items(15, i) = controlColour
                        items(16, i) = controlLengthText
                        items(17, i) = dt.Rows(i)("BottomJoining").ToString()
                        items(18, i) = dt.Rows(i)("BracketExtension").ToString()
                        items(19, i) = dt.Rows(i)("Sloping").ToString()
                        items(20, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Vertical Type", "Blade Type", "Blade Qty", "Fabric Insert", "Fabric Type", "Fabric Colour", "Width (mm)", "Drop (mm)", "Stack Position", "Control Position", "Control Type", "Track Colour", "Control Colour", "Control Length", "Bottom Joining", "Extension Bracket", "Sloping", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END VERTICAL

            ' START SAPHORA DRAPE
            Try
                Dim saphoraData As DataSet = GetListData("SELECT OrderDetails.*, Blinds.Name AS BlindName, ProductTubes.Name AS TubeName, ProductControls.Name AS ControlName, ProductColours.Name AS ColourName, Fabrics.Name AS FabricName, FabricColours.Colour AS FabricColour, Chains.Name AS ChainName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductTubes ON Products.TubeType=ProductTubes.Id LEFT JOIN ProductControls ON Products.ControlType=ProductControls.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id LEFT JOIN Fabrics ON OrderDetails.FabricId=Fabrics.Id LEFT JOIN FabricColours ON OrderDetails.FabricColourId=FabricColours.Id LEFT JOIN Chains ON OrderDetails.ChainId=Chains.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Saphora Drape' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not saphoraData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Saphora"
                    pageEvent.PageTitle2 = "Drape"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = saphoraData.Tables(0)
                    Dim items(19, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlName As String = dt.Rows(i)("ControlName").ToString()
                        Dim controlColour As String = String.Empty
                        If controlName = "Chain" Then controlColour = dt.Rows(i)("ChainName").ToString()
                        If controlName = "Wand" Then controlColour = dt.Rows(i)("WandColour").ToString()

                        Dim controlLength As String = dt.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = dt.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("BlindName").ToString()
                        items(4, i) = dt.Rows(i)("TubeName").ToString()
                        items(5, i) = dt.Rows(i)("FabricName").ToString()
                        items(6, i) = dt.Rows(i)("FabricColour").ToString()
                        items(7, i) = dt.Rows(i)("Width").ToString()
                        items(8, i) = dt.Rows(i)("Drop").ToString()
                        items(9, i) = dt.Rows(i)("ColourName").ToString()
                        items(10, i) = dt.Rows(i)("StackPosition").ToString()
                        items(11, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(12, i) = controlName
                        items(13, i) = controlColour
                        items(14, i) = controlLengthText
                        items(15, i) = dt.Rows(i)("BottomJoining").ToString()
                        items(16, i) = dt.Rows(i)("BracketExtension").ToString()
                        items(17, i) = dt.Rows(i)("Sloping").ToString()
                        items(18, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Vertical Type", "Blade Type", "Fabric Type", "Fabric Colour", "Width (mm)", "Drop (mm)", "Track Colour", "Stack Position", "Control Position", "Control Type", "Control Colour", "Control Length", "Bottom Joining", "Extension Bracket", "Sloping", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END SAPHORA DRAPE

            ' START SKYLINE SHUTTER EXPRESS
            Try
                Dim shutterExpressData As DataSet = GetListData("SELECT OrderDetails.*, ProductColours.Name AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Skyline Shutter Express' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not shutterExpressData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "EXPRESS"
                    pageEvent.PageTitle2 = "Skyline Shutter"

                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = shutterExpressData.Tables(0)
                    Dim items(38, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim layoutCode As String = If(dt.Rows(i)("LayoutCode").ToString() = "Other", dt.Rows(i)("LayoutCodeCustom").ToString(), dt.Rows(i)("LayoutCode").ToString())

                        Dim gapList As New List(Of String)

                        Dim midrailList As New List(Of String)

                        Dim midrailHeight1 As Integer = 0
                        Dim midrailHeight2 As Integer = 0

                        If Not IsDBNull(dt.Rows(i)("MidrailHeight1")) Then
                            midrailHeight1 = dt.Rows(i)("MidrailHeight1")
                        End If
                        If Not IsDBNull(dt.Rows(i)("MidrailHeight2")) Then
                            midrailHeight2 = dt.Rows(i)("MidrailHeight2")
                        End If

                        If midrailHeight1 > 0 Then midrailList.Add(String.Format("1 : {0}", midrailHeight1))
                        If midrailHeight2 > 0 Then midrailList.Add(String.Format("2 : {0}", midrailHeight2))

                        Dim midrailHeight As String = String.Join(", ", midrailList)

                        Dim headerLengthValue As Integer = 0
                        Dim headerLengthText As String = String.Empty
                        If Not IsDBNull(dt.Rows(i)("CustomHeaderLength")) Then
                            headerLengthValue = dt.Rows(i)("CustomHeaderLength")
                        End If
                        If headerLengthValue > 0 Then
                            headerLengthText = headerLengthValue.ToString()
                        End If

                        Dim gap1 As Integer = 0
                        Dim gap2 As Integer = 0
                        Dim gap3 As Integer = 0
                        Dim gap4 As Integer = 0
                        Dim gap5 As Integer = 0

                        If Not IsDBNull(dt.Rows(i)("Gap1")) Then
                            gap1 = dt.Rows(i)("Gap1")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap2")) Then
                            gap2 = dt.Rows(i)("Gap2")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap3")) Then
                            gap3 = dt.Rows(i)("Gap3")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap4")) Then
                            gap4 = dt.Rows(i)("Gap4")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap5")) Then
                            gap5 = dt.Rows(i)("Gap5")
                        End If

                        If gap1 > 0 Then gapList.Add("Gap 1 : " & gap1)
                        If gap2 > 0 Then gapList.Add("Gap 2 : " & gap2)
                        If gap3 > 0 Then gapList.Add("Gap 3 : " & gap3)
                        If gap4 > 0 Then gapList.Add("Gap 4 : " & gap4)
                        If gap5 > 0 Then gapList.Add("Gap 5 : " & gap5)

                        Dim gapPosition As String = String.Join(", ", gapList)

                        Dim split1 As Integer = 0
                        Dim split2 As Integer = 0

                        If Not IsDBNull(dt.Rows(i)("SplitHeight1")) Then
                            split1 = dt.Rows(i)("SplitHeight1")
                        End If
                        If Not IsDBNull(dt.Rows(i)("SplitHeight2")) Then
                            split2 = dt.Rows(i)("SplitHeight2")
                        End If

                        Dim splitHeigth As String = String.Format("1st : {0}, 2nd : {1}", split1, split2)

                        Dim horizontalHeight As String = String.Empty
                        Dim horizontalTPostHeight As Integer = 0
                        If Not IsDBNull(dt.Rows(i)("HorizontalTPostHeight")) Then
                            horizontalTPostHeight = dt.Rows(i)("HorizontalTPostHeight")
                        End If

                        If horizontalTPostHeight > 0 Then
                            horizontalHeight = dt.Rows(i)("HorizontalTPostHeight").ToString()
                        End If

                        Dim bottomTrack As String = dt.Rows(i)("BottomTrackType").ToString()
                        If dt.Rows(i)("BottomTrackRecess").ToString() = "Yes" Then
                            bottomTrack = String.Format("{0} | Recess: Yes", dt.Rows(i)("BottomTrackType").ToString())
                        End If
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Width").ToString()
                        items(3, i) = dt.Rows(i)("Drop").ToString()
                        items(4, i) = dt.Rows(i)("Mounting").ToString()
                        items(5, i) = dt.Rows(i)("ColourType").ToString()
                        items(6, i) = dt.Rows(i)("LouvreSize").ToString()
                        items(7, i) = dt.Rows(i)("LouvrePosition").ToString()
                        items(8, i) = midrailHeight
                        items(9, i) = dt.Rows(i)("MidrailCritical").ToString()
                        items(10, i) = dt.Rows(i)("HingeColour").ToString()
                        items(11, i) = dt.Rows(i)("BlindName").ToString()
                        items(12, i) = dt.Rows(i)("SemiInsideMount").ToString()
                        items(13, i) = dt.Rows(i)("PanelQty").ToString()
                        items(14, i) = headerLengthText
                        items(15, i) = dt.Rows(i)("JoinedPanels").ToString()
                        items(16, i) = layoutCode
                        items(17, i) = dt.Rows(i)("FrameType").ToString()
                        items(18, i) = dt.Rows(i)("FrameLeft").ToString()
                        items(19, i) = dt.Rows(i)("FrameRight").ToString()
                        items(20, i) = dt.Rows(i)("FrameTop").ToString()
                        items(21, i) = dt.Rows(i)("FrameBottom").ToString()
                        items(22, i) = bottomTrack
                        items(23, i) = dt.Rows(i)("Buildout").ToString()
                        items(24, i) = dt.Rows(i)("BuildoutPosition").ToString()
                        items(25, i) = dt.Rows(i)("SameSizePanel").ToString()
                        items(26, i) = gapPosition
                        items(27, i) = horizontalHeight
                        items(28, i) = dt.Rows(i)("HorizontalTPost").ToString()
                        items(29, i) = dt.Rows(i)("TiltrodType").ToString()
                        items(30, i) = dt.Rows(i)("TiltrodSplit").ToString()
                        items(31, i) = splitHeigth
                        items(32, i) = dt.Rows(i)("ReverseHinged").ToString()
                        items(33, i) = dt.Rows(i)("PelmetFlat").ToString()
                        items(34, i) = dt.Rows(i)("ExtraFascia").ToString()
                        items(35, i) = dt.Rows(i)("HingesLoose").ToString()
                        items(36, i) = FormatNumber(dt.Rows(i)("SquareMetre"), 2)
                        items(37, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 4
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Width (mm)", "Height (mm)", "Mounting", "Colour", "Louvre Size", "Sliding Louvre Position", "Midrail Height (mm)", "Critical Midrail", "Hinge Colour", "Installation Method", "Semi Inside Mount", "Panel Qty", "Custom Header Length (mm)", "Co-joined Panels", "Layout Code", "Frame Type", "Left Frame", "Right Frame", "Top Frame", "Bottom Frame", "Bottom Track", "Buildout", "Buildout Position", "Same Size Panel", "Gap / T-Post (mm)", "Hor T-Post Height (mm)", "Hor T-Post Required", "Tiltrod Type", "Split Tiltrod Rotation", "Split Height (mm)", "Reverse Hinged", "Pelmet Flat Packed", "Extra Fascia", "Hinges Loose", "M2", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 16
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 3, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 16
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 3
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 16
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END SKYLINE SHUTTER EXPRESS

            ' START SKYLINE SHUTTER OCEAN
            Try
                Dim shutterOceanData As DataSet = GetListData("SELECT OrderDetails.*, ProductColours.Name AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Skyline Shutter Ocean' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not shutterOceanData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "OCEAN"
                    pageEvent.PageTitle2 = "Skyline Shutter"

                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = shutterOceanData.Tables(0)
                    Dim items(41, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim layoutCode As String = If(dt.Rows(i)("LayoutCode").ToString() = "Other", dt.Rows(i)("LayoutCodeCustom").ToString(), dt.Rows(i)("LayoutCode").ToString())

                        Dim gapList As New List(Of String)

                        Dim midrailList As New List(Of String)

                        Dim midrailHeight1 As Integer = 0
                        Dim midrailHeight2 As Integer = 0

                        If Not IsDBNull(dt.Rows(i)("MidrailHeight1")) Then
                            midrailHeight1 = dt.Rows(i)("MidrailHeight1")
                        End If
                        If Not IsDBNull(dt.Rows(i)("MidrailHeight2")) Then
                            midrailHeight2 = dt.Rows(i)("MidrailHeight2")
                        End If

                        If midrailHeight1 > 0 Then midrailList.Add(String.Format("1 : {0}", midrailHeight1))
                        If midrailHeight2 > 0 Then midrailList.Add(String.Format("2 : {0}", midrailHeight2))

                        Dim midrailHeight As String = String.Join(", ", midrailList)

                        Dim headerLengthValue As Integer = 0
                        Dim headerLengthText As String = String.Empty
                        If Not IsDBNull(dt.Rows(i)("CustomHeaderLength")) Then
                            headerLengthValue = dt.Rows(i)("CustomHeaderLength")
                        End If
                        If headerLengthValue > 0 Then
                            headerLengthText = headerLengthValue.ToString()
                        End If

                        Dim gap1 As Integer = 0
                        Dim gap2 As Integer = 0
                        Dim gap3 As Integer = 0
                        Dim gap4 As Integer = 0
                        Dim gap5 As Integer = 0

                        If Not IsDBNull(dt.Rows(i)("Gap1")) Then
                            gap1 = dt.Rows(i)("Gap1")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap2")) Then
                            gap2 = dt.Rows(i)("Gap2")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap3")) Then
                            gap3 = dt.Rows(i)("Gap3")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap4")) Then
                            gap4 = dt.Rows(i)("Gap4")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap5")) Then
                            gap5 = dt.Rows(i)("Gap5")
                        End If

                        If gap1 > 0 Then gapList.Add("Gap 1 : " & gap1)
                        If gap2 > 0 Then gapList.Add("Gap 2 : " & gap2)
                        If gap3 > 0 Then gapList.Add("Gap 3 : " & gap3)
                        If gap4 > 0 Then gapList.Add("Gap 4 : " & gap4)
                        If gap5 > 0 Then gapList.Add("Gap 5 : " & gap5)

                        Dim gapPosition As String = String.Join(", ", gapList)

                        Dim split1 As Integer = 0
                        Dim split2 As Integer = 0

                        If Not IsDBNull(dt.Rows(i)("SplitHeight1")) Then
                            split1 = dt.Rows(i)("SplitHeight1")
                        End If
                        If Not IsDBNull(dt.Rows(i)("SplitHeight2")) Then
                            split2 = dt.Rows(i)("SplitHeight2")
                        End If

                        Dim splitHeigth As String = String.Format("1st : {0}, 2nd : {1}", split1, split2)

                        Dim horizontalHeight As String = String.Empty
                        Dim horizontalTPostHeight As Integer = 0
                        If Not IsDBNull(dt.Rows(i)("HorizontalTPostHeight")) Then
                            horizontalTPostHeight = dt.Rows(i)("HorizontalTPostHeight")
                        End If

                        If horizontalTPostHeight > 0 Then
                            horizontalHeight = dt.Rows(i)("HorizontalTPostHeight").ToString()
                        End If

                        Dim bottomTrack As String = dt.Rows(i)("BottomTrackType").ToString()
                        If dt.Rows(i)("BottomTrackRecess").ToString() = "Yes" Then
                            bottomTrack = String.Format("{0} | Recess: Yes", dt.Rows(i)("BottomTrackType").ToString())
                        End If
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Width").ToString()
                        items(3, i) = dt.Rows(i)("Drop").ToString()
                        items(4, i) = dt.Rows(i)("Mounting").ToString()
                        items(5, i) = dt.Rows(i)("ColourType").ToString()
                        items(6, i) = dt.Rows(i)("LouvreSize").ToString()
                        items(7, i) = dt.Rows(i)("LouvrePosition").ToString()
                        items(8, i) = midrailHeight
                        items(9, i) = dt.Rows(i)("MidrailCritical").ToString()
                        items(10, i) = dt.Rows(i)("HingeColour").ToString()
                        items(11, i) = dt.Rows(i)("BlindName").ToString()
                        items(12, i) = dt.Rows(i)("SemiInsideMount").ToString()
                        items(13, i) = dt.Rows(i)("PanelQty").ToString()
                        items(14, i) = headerLengthText
                        items(15, i) = dt.Rows(i)("JoinedPanels").ToString()
                        items(16, i) = layoutCode
                        items(17, i) = dt.Rows(i)("FrameType").ToString()
                        items(18, i) = dt.Rows(i)("FrameLeft").ToString()
                        items(19, i) = dt.Rows(i)("FrameRight").ToString()
                        items(20, i) = dt.Rows(i)("FrameTop").ToString()
                        items(21, i) = dt.Rows(i)("FrameBottom").ToString()
                        items(22, i) = bottomTrack
                        items(23, i) = dt.Rows(i)("Buildout").ToString()
                        items(24, i) = dt.Rows(i)("BuildoutPosition").ToString()
                        items(25, i) = dt.Rows(i)("SameSizePanel").ToString()
                        items(26, i) = gapPosition
                        items(27, i) = horizontalHeight
                        items(28, i) = dt.Rows(i)("HorizontalTPost").ToString()
                        items(29, i) = dt.Rows(i)("TiltrodType").ToString()
                        items(30, i) = dt.Rows(i)("TiltrodSplit").ToString()
                        items(31, i) = splitHeigth
                        items(32, i) = dt.Rows(i)("ReverseHinged").ToString()
                        items(33, i) = dt.Rows(i)("PelmetFlat").ToString()
                        items(34, i) = dt.Rows(i)("ExtraFascia").ToString()
                        items(35, i) = dt.Rows(i)("HingesLoose").ToString()
                        items(36, i) = dt.Rows(i)("DoorCutOut").ToString()
                        items(37, i) = dt.Rows(i)("SpecialShape").ToString()
                        items(38, i) = dt.Rows(i)("TemplateProvided").ToString()
                        items(39, i) = FormatNumber(dt.Rows(i)("SquareMetre"), 2)
                        items(40, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 4
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Width (mm)", "Height (mm)", "Mounting", "Colour", "Louvre Size", "Sliding Louvre Position", "Midrail Height (mm)", "Critical Midrail", "Hinge Colour", "Installation Method", "Semi Inside Mount", "Panel Qty", "Custom Header Length (mm)", "Co-joined Panels", "Layout Code", "Frame Type", "Left Frame", "Right Frame", "Top Frame", "Bottom Frame", "Bottom Track", "Buildout", "Buildout Position", "Same Size Panel", "Gap / T-Post (mm)", "Hor T-Post Height (mm)", "Hor T-Post Required", "Tiltrod Type", "Split Tiltrod Rotation", "Split Height (mm)", "Reverse Hinged", "Pelmet Flat Packed", "Extra Fascia", "Hinges Loose", "French Door Cut-Out", "Special Shape", "Template Provided", "M2", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 16
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 3, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 16
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 3
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 16
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END SKYLINE SHUTTER OCEAN

            ' START EVOLVE SHUTTER OCEAN
            Try
                Dim evolveOceanData As DataSet = GetListData("SELECT OrderDetails.*, ProductColours.Name AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Evolve Shutter Ocean' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not evolveOceanData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "OCEAN"
                    pageEvent.PageTitle2 = "Evolve Shutter"

                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = evolveOceanData.Tables(0)
                    Dim items(37, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim layoutCode As String = If(dt.Rows(i)("LayoutCode").ToString() = "Other", dt.Rows(i)("LayoutCodeCustom").ToString(), dt.Rows(i)("LayoutCode").ToString())

                        Dim gapList As New List(Of String)

                        Dim midrailList As New List(Of String)

                        Dim midrailHeight1 As Integer = 0
                        Dim midrailHeight2 As Integer = 0

                        If Not IsDBNull(dt.Rows(i)("MidrailHeight1")) Then
                            midrailHeight1 = dt.Rows(i)("MidrailHeight1")
                        End If
                        If Not IsDBNull(dt.Rows(i)("MidrailHeight2")) Then
                            midrailHeight2 = dt.Rows(i)("MidrailHeight2")
                        End If

                        If midrailHeight1 > 0 Then midrailList.Add(String.Format("1 : {0}", midrailHeight1))
                        If midrailHeight2 > 0 Then midrailList.Add(String.Format("2 : {0}", midrailHeight2))

                        Dim midrailHeight As String = String.Join(", ", midrailList)

                        Dim headerLengthValue As Integer = 0
                        Dim headerLengthText As String = String.Empty
                        If Not IsDBNull(dt.Rows(i)("CustomHeaderLength")) Then
                            headerLengthValue = dt.Rows(i)("CustomHeaderLength")
                        End If
                        If headerLengthValue > 0 Then
                            headerLengthText = headerLengthValue.ToString()
                        End If

                        Dim gap1 As Integer = 0
                        Dim gap2 As Integer = 0
                        Dim gap3 As Integer = 0
                        Dim gap4 As Integer = 0
                        Dim gap5 As Integer = 0

                        If Not IsDBNull(dt.Rows(i)("Gap1")) Then
                            gap1 = dt.Rows(i)("Gap1")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap2")) Then
                            gap2 = dt.Rows(i)("Gap2")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap3")) Then
                            gap3 = dt.Rows(i)("Gap3")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap4")) Then
                            gap4 = dt.Rows(i)("Gap4")
                        End If
                        If Not IsDBNull(dt.Rows(i)("Gap5")) Then
                            gap5 = dt.Rows(i)("Gap5")
                        End If

                        If gap1 > 0 Then gapList.Add("Gap 1 : " & gap1)
                        If gap2 > 0 Then gapList.Add("Gap 2 : " & gap2)
                        If gap3 > 0 Then gapList.Add("Gap 3 : " & gap3)
                        If gap4 > 0 Then gapList.Add("Gap 4 : " & gap4)
                        If gap5 > 0 Then gapList.Add("Gap 5 : " & gap5)

                        Dim gapPosition As String = String.Join(", ", gapList)

                        Dim split1 As Integer = 0
                        Dim split2 As Integer = 0

                        If Not IsDBNull(dt.Rows(i)("SplitHeight1")) Then
                            split1 = dt.Rows(i)("SplitHeight1")
                        End If
                        If Not IsDBNull(dt.Rows(i)("SplitHeight2")) Then
                            split2 = dt.Rows(i)("SplitHeight2")
                        End If

                        Dim splitHeigth As String = String.Format("1st : {0}, 2nd : {1}", split1, split2)

                        Dim horizontalHeight As String = String.Empty
                        Dim horizontalTPostHeight As Integer = 0
                        If Not IsDBNull(dt.Rows(i)("HorizontalTPostHeight")) Then
                            horizontalTPostHeight = dt.Rows(i)("HorizontalTPostHeight")
                        End If

                        If horizontalTPostHeight > 0 Then
                            horizontalHeight = dt.Rows(i)("HorizontalTPostHeight").ToString()
                        End If

                        Dim bottomTrack As String = dt.Rows(i)("BottomTrackType").ToString()
                        If dt.Rows(i)("BottomTrackRecess").ToString() = "Yes" Then
                            bottomTrack = String.Format("{0} | Recess: Yes", dt.Rows(i)("BottomTrackType").ToString())
                        End If
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Width").ToString()
                        items(3, i) = dt.Rows(i)("Drop").ToString()
                        items(4, i) = dt.Rows(i)("Mounting").ToString()
                        items(5, i) = dt.Rows(i)("ColourType").ToString()
                        items(6, i) = dt.Rows(i)("LouvreSize").ToString()
                        items(7, i) = dt.Rows(i)("LouvrePosition").ToString()
                        items(8, i) = midrailHeight
                        items(9, i) = dt.Rows(i)("MidrailCritical").ToString()
                        items(10, i) = dt.Rows(i)("HingeColour").ToString()
                        items(11, i) = dt.Rows(i)("BlindName").ToString()
                        items(12, i) = dt.Rows(i)("SemiInsideMount").ToString()
                        items(13, i) = dt.Rows(i)("PanelQty").ToString()
                        items(14, i) = headerLengthText
                        items(15, i) = dt.Rows(i)("JoinedPanels").ToString()
                        items(16, i) = layoutCode
                        items(17, i) = dt.Rows(i)("FrameType").ToString()
                        items(18, i) = dt.Rows(i)("FrameLeft").ToString()
                        items(19, i) = dt.Rows(i)("FrameRight").ToString()
                        items(20, i) = dt.Rows(i)("FrameTop").ToString()
                        items(21, i) = dt.Rows(i)("FrameBottom").ToString()
                        items(22, i) = bottomTrack
                        items(23, i) = dt.Rows(i)("Buildout").ToString()
                        items(24, i) = dt.Rows(i)("SameSizePanel").ToString()
                        items(25, i) = gapPosition
                        items(26, i) = horizontalHeight
                        items(27, i) = dt.Rows(i)("HorizontalTPost").ToString()
                        items(28, i) = dt.Rows(i)("TiltrodType").ToString()
                        items(29, i) = dt.Rows(i)("TiltrodSplit").ToString()
                        items(30, i) = splitHeigth
                        items(31, i) = dt.Rows(i)("ReverseHinged").ToString()
                        items(32, i) = dt.Rows(i)("PelmetFlat").ToString()
                        items(33, i) = dt.Rows(i)("ExtraFascia").ToString()
                        items(34, i) = dt.Rows(i)("HingesLoose").ToString()
                        items(35, i) = FormatNumber(dt.Rows(i)("SquareMetre"), 2)
                        items(36, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 4
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Width (mm)", "Height (mm)", "Mounting", "Colour", "Louvre Size", "Sliding Louvre Position", "Midrail Height (mm)", "Critical Midrail", "Hinge Colour", "Installation Method", "Semi Inside Mount", "Panel Qty", "Custom Header Length (mm)", "Co-joined Panels", "Layout Code", "Frame Type", "Left Frame", "Right Frame", "Top Frame", "Bottom Frame", "Bottom Track", "Buildout", "Same Size Panel", "Gap / T-Post (mm)", "Hor T-Post Height (mm)", "Hor T-Post Required", "Tiltrod Type", "Split Tiltrod Rotation", "Split Height (mm)", "Reverse Hinged", "Pelmet Flat Packed", "Extra Fascia", "Hinges Loose", "M2", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 16
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 3, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 16
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 3
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 16
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END EVOLVE SHUTTER OCEAN

            ' START WINDOW
            Try
                Dim windowData As DataSet = GetListData("SELECT OrderDetails.*, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Window' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not windowData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Aluminium"
                    pageEvent.PageTitle2 = "Window"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = windowData.Tables(0)
                    Dim items(20, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim swivelQty As String = dt.Rows(i)("SwivelQty").ToString()
                        If swivelQty = "0" Then swivelQty = String.Empty

                        Dim swivelQtyB As String = dt.Rows(i)("SwivelQtyB").ToString()
                        If swivelQtyB = "0" Then swivelQtyB = String.Empty

                        Dim springQty As String = dt.Rows(i)("SpringQty").ToString()
                        If springQty = "0" Then springQty = String.Empty

                        Dim topPlascticQty As String = dt.Rows(i)("TopPlasticQty").ToString()
                        If topPlascticQty = "0" Then topPlascticQty = String.Empty

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = dt.Rows(i)("BlindName").ToString()
                        items(6, i) = dt.Rows(i)("FrameColour").ToString()
                        items(7, i) = dt.Rows(i)("MeshType").ToString()
                        items(8, i) = dt.Rows(i)("Brace").ToString()
                        items(9, i) = dt.Rows(i)("AngleType").ToString()
                        items(10, i) = dt.Rows(i)("AngleLength").ToString()
                        items(11, i) = dt.Rows(i)("AngleQty").ToString()
                        items(12, i) = dt.Rows(i)("PortHole").ToString()
                        items(13, i) = dt.Rows(i)("PlungerPin").ToString()
                        items(14, i) = dt.Rows(i)("SwivelColour").ToString()
                        items(15, i) = swivelQty
                        items(16, i) = swivelQtyB
                        items(17, i) = springQty
                        items(18, i) = topPlascticQty
                        items(19, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Window Type", "Frame Colour", "Mesh Type", "Brace / Joiner Height", "Angle Type", "Angle Length (mm)", "Angle Qty", "Screen Port Hole", "Plunger Pin", "Swivel Clip Colour", "Swivel Clip (1.6mm)", "Swivel Clip (11mm)", "Spring Clip Qty", "Top Clip Plastic", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END WINDOW

            ' START DOOR
            Try
                Dim doorData As DataSet = GetListData("SELECT OrderDetails.*, Blinds.Name AS BlindName, ProductTubes.Name AS TubeName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductTubes ON Products.TubeType=ProductTubes.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Door' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not doorData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Aluminium"
                    pageEvent.PageTitle2 = "Door"
                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = doorData.Tables(0)
                    Dim items(33, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim width As String = String.Format("{0} - {1} - {2}", dt.Rows(i)("Width").ToString(), dt.Rows(i)("WidthB").ToString(), dt.Rows(i)("WidthC").ToString())

                        Dim handleLength As String = String.Empty
                        Dim handleLengthValue As Integer = dt.Rows(i)("HandleLength")
                        If handleLengthValue > 0 Then
                            handleLength = dt.Rows(i)("HandleLength").ToString()
                        End If

                        Dim topTrackLength As String = String.Empty
                        Dim topTrackValue As Integer = dt.Rows(i)("TopTrackLength")
                        If topTrackValue > 0 Then
                            topTrackLength = dt.Rows(i)("TopTrackLength").ToString()
                        End If

                        Dim bottomTrackLength As String = String.Empty
                        Dim bottomTrackValue As Integer = dt.Rows(i)("BottomTrackLength")
                        If bottomTrackValue > 0 Then bottomTrackLength = dt.Rows(i)("BottomTrackLength").ToString()

                        Dim receiverLength As String = String.Empty
                        Dim receiverValue As Integer = dt.Rows(i)("ReceiverLength")
                        If receiverValue > 0 Then receiverLength = dt.Rows(i)("ReceiverLength").ToString()

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = width
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = dt.Rows(i)("BlindName").ToString()
                        items(6, i) = dt.Rows(i)("TubeName").ToString()
                        items(7, i) = dt.Rows(i)("FrameColour").ToString()
                        items(8, i) = dt.Rows(i)("MeshType").ToString()
                        items(9, i) = dt.Rows(i)("LayoutCode").ToString()
                        items(10, i) = dt.Rows(i)("MidrailPosition").ToString()
                        items(11, i) = dt.Rows(i)("HandleType").ToString()
                        items(12, i) = handleLength
                        items(13, i) = dt.Rows(i)("TripleLock").ToString()
                        items(14, i) = dt.Rows(i)("BugSeal").ToString()
                        items(15, i) = dt.Rows(i)("PetType").ToString()
                        items(16, i) = dt.Rows(i)("PetPosition").ToString()
                        items(17, i) = dt.Rows(i)("DoorCloser").ToString()
                        items(18, i) = dt.Rows(i)("AngleType").ToString()
                        items(19, i) = dt.Rows(i)("AngleLength").ToString()
                        items(20, i) = dt.Rows(i)("Beading").ToString()
                        items(21, i) = dt.Rows(i)("JambType").ToString()
                        items(22, i) = dt.Rows(i)("JambPosition").ToString()
                        items(23, i) = dt.Rows(i)("FlushBold").ToString()
                        items(24, i) = dt.Rows(i)("InterlockType").ToString()
                        items(25, i) = dt.Rows(i)("TopTrack").ToString()
                        items(26, i) = topTrackLength
                        items(27, i) = dt.Rows(i)("BottomTrack").ToString()
                        items(28, i) = bottomTrackLength
                        items(29, i) = dt.Rows(i)("Receiver").ToString()
                        items(30, i) = receiverLength
                        items(31, i) = dt.Rows(i)("SlidingQty").ToString()
                        items(32, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 4
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Door Type", "Mechanism", "Frame Colour", "Mesh Type", "Layout Code", "Midrail Position", "Handle Type", "Handle Length (mm)", "Triple Lock", "Bug Seal", "Pet Door", "Pet Door Position", "Door Closer", "Angle Type", "Angle Length (mm)", "Beading", "Jamb Adaptor", "Jamb Adaptor Position", "Flush Bold", "Interlock", "Top Track", "Top Track Length (mm)", "Bottom Track", "Bottom Track Length (mm)", "Receiver", "Receiver Length (mm)", "Sliding Roller", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 19
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 3, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 19
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 3
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 19
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END DOOR

            ' START SAMPLE
            Try
                Dim sampleData As DataSet = GetListData("SELECT OrderDetails.*, Blinds.Name AS BlindName, Fabrics.Name AS FabricName, FabricColours.Colour AS FabricColour FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN Fabrics ON OrderDetails.FabricId=Fabrics.Id LEFT JOIN FabricColours ON OrderDetails.FabricColourId=FabricColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Sample' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not sampleData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Sample"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = sampleData.Tables(0)
                    Dim items(5, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("BlindName").ToString()
                        items(2, i) = dt.Rows(i)("FabricName").ToString()
                        items(3, i) = dt.Rows(i)("FabricColour").ToString()
                        items(4, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Type", "Fabric Type", "Fabric Colour", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END SAMPLE

            ' START OUTDOOR
            Try
                Dim outdoorData As DataSet = GetListData("SELECT OrderDetails.*, Blinds.Name AS BlindName, Fabrics.Name AS FabricName, FabricColours.Colour AS FabricColour, ProductControls.Name AS ControlName FROM OrderDetails LEFT JOIN FabricColours ON OrderDetails.FabricColourId=FabricColours.Id LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductControls ON Products.ControlType=ProductControls.Id LEFT JOIN Fabrics ON OrderDetails.FabricId=Fabrics.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Outdoor' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If Not outdoorData.Tables(0).Rows.Count = 0 Then
                    pageEvent.PageTitle = "Outdoor"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim dt As DataTable = outdoorData.Tables(0)
                    Dim items(12, dt.Rows.Count - 1) As String

                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlLength As String = dt.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = dt.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = dt.Rows(i)("Room").ToString()
                        items(2, i) = dt.Rows(i)("Mounting").ToString()
                        items(3, i) = dt.Rows(i)("Width").ToString()
                        items(4, i) = dt.Rows(i)("Drop").ToString()
                        items(5, i) = dt.Rows(i)("BlindName").ToString()
                        items(6, i) = dt.Rows(i)("ControlName").ToString()
                        items(7, i) = dt.Rows(i)("FabricName").ToString()
                        items(8, i) = dt.Rows(i)("FabricColour").ToString()
                        items(9, i) = dt.Rows(i)("ControlPosition").ToString()
                        items(10, i) = controlLengthText
                        items(11, i) = dt.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then
                            doc.NewPage()
                        End If

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Type", "Control Type", "Fabric Type", "Fabric Colour", "Control Position", "Control Length", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 26
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 26
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 26
                                table.AddCell(emptyCell)
                            Next
                        Next
                        doc.Add(table)
                        table.DeleteBodyRows()
                        doc.NewPage()
                    Next
                End If
            Catch ex As Exception
            End Try
            ' END OUTDOOR

            pageTotalItem = String.Format("{0} Item", totalItems)
            If totalItems > 1 Then pageTotalItem = String.Format("{0} Items", totalItems)

            pageEvent.PageTotalItem = pageTotalItem

            doc.Close()
            writer.Close()
        End Using
    End Sub
End Class

Public Class PreviewEvents
    Inherits PdfPageEventHelper

    Public Property PageTitle As String
    Public Property PageTitle2 As String
    Public Property PageOrderId As String
    Public Property PageOrderDate As String
    Public Property PageSubmitDate As String
    Public Property PageCustomerName As String
    Public Property PageOrderNumber As String
    Public Property PageOrderName As String
    Public Property PageNote As String
    Public Property PageTotalItem As String
    Public Property PageTotalDoc As Integer
    Public Property pageCompany As String

    Private baseFont As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
    Private template As PdfTemplate

    Public Overrides Sub OnOpenDocument(writer As PdfWriter, document As Document)
        template = writer.DirectContent.CreateTemplate(50, 50)
    End Sub

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent

        If template Is Nothing Then
            template = cb.CreateTemplate(50, 50)
        End If

        Dim headerTable As New PdfPTable(3)
        headerTable.TotalWidth = document.PageSize.Width - 40
        headerTable.LockedWidth = True
        headerTable.SetWidths(New Single() {0.3F, 0.5F, 0.2F})

        Dim nestedTable As New PdfPTable(3)
        nestedTable.TotalWidth = headerTable.TotalWidth * 0.5F
        nestedTable.SetWidths(New Single() {0.25F, 0.05F, 0.7F})

        Dim innerTable As New PdfPTable(1)
        innerTable.WidthPercentage = 100

        Dim logoPath As String = HttpContext.Current.Server.MapPath("~/assets/images/logo/general.jpg")
        If pageCompany = "2" Then
            logoPath = HttpContext.Current.Server.MapPath("~/assets/images/logo/jpmdirect.jpg")
        End If
        If pageCompany = "3" Then
            logoPath = HttpContext.Current.Server.MapPath("~/assets/images/logo/accent.png")
        End If
        If pageCompany = "4" Then
            logoPath = HttpContext.Current.Server.MapPath("~/assets/images/logo/jpmdirect.jpg")
        End If
        If pageCompany = "5" Then
            logoPath = HttpContext.Current.Server.MapPath("~/assets/images/logo/big.JPG")
        End If

        Dim logoImage As Image = Image.GetInstance(logoPath)

        logoImage.ScaleToFit(120.0F, 40.0F)
        logoImage.Alignment = Element.ALIGN_LEFT
        Dim logoCell As New PdfPCell(logoImage)
        logoCell.Border = 0
        logoCell.HorizontalAlignment = Element.ALIGN_LEFT
        innerTable.AddCell(logoCell)

        If pageCompany = "2" Then
            Dim phraseTitle As New Phrase()
            phraseTitle.Add(New Chunk("JPM Direct Pty Ltd", New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk("Suite 265 97-99 Bathurst Street", New Font(Font.FontFamily.TIMES_ROMAN, 8)))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk("Sydney, NSW 2000", New Font(Font.FontFamily.TIMES_ROMAN, 8)))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk("Phone : 0417 705 109", New Font(Font.FontFamily.TIMES_ROMAN, 8)))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk("Email : order@jpmdirect.com.au", New Font(Font.FontFamily.TIMES_ROMAN, 8)))
            Dim textCell As New PdfPCell(phraseTitle)
            textCell.Border = 0
            textCell.HorizontalAlignment = Element.ALIGN_LEFT
            innerTable.AddCell(textCell)
        End If

        If pageCompany = "3" Then
            Dim phraseTitle As New Phrase()
            phraseTitle.Add(New Chunk("Accent At Home", New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk("Ruko De Mansion Blok D No 9, Kunciran", New Font(Font.FontFamily.TIMES_ROMAN, 8)))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk("Kota Tangerang, Banten 15143", New Font(Font.FontFamily.TIMES_ROMAN, 8)))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk("Phone : 0821 1426 8322", New Font(Font.FontFamily.TIMES_ROMAN, 8)))
            phraseTitle.Add(New Chunk(Environment.NewLine))
            phraseTitle.Add(New Chunk("Email : cs@accentblinds.id", New Font(Font.FontFamily.TIMES_ROMAN, 8)))
            Dim textCell As New PdfPCell(phraseTitle)
            textCell.Border = 0
            textCell.HorizontalAlignment = Element.ALIGN_LEFT
            innerTable.AddCell(textCell)
        End If

        Dim firstHeaderCell As New PdfPCell(innerTable)
        firstHeaderCell.Border = 0
        firstHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT
        firstHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(firstHeaderCell)

        Dim labels As String() = {"Customer Account", "Order #", "Order Number", "Order Name", "Created Date", "Submitted Date", "Total Item Order"}
        Dim values As String() = {PageCustomerName, PageOrderId, PageOrderNumber, PageOrderName, PageOrderDate, PageSubmitDate, PageTotalItem}

        For i As Integer = 0 To labels.Length - 1
            nestedTable.AddCell(New PdfPCell(New Phrase(labels(i), New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD))) With {
                .Border = 0,
                .HorizontalAlignment = Element.ALIGN_LEFT
            })
            nestedTable.AddCell(New PdfPCell(New Phrase(":", New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD))) With {
                .Border = 0,
                .HorizontalAlignment = Element.ALIGN_CENTER
            })
            nestedTable.AddCell(New PdfPCell(New Phrase(values(i), New Font(Font.FontFamily.TIMES_ROMAN, 8))) With {
                .Border = 0,
                .HorizontalAlignment = Element.ALIGN_LEFT
            })
        Next

        Dim secondHeaderCell As New PdfPCell(nestedTable) With {
            .Border = 0,
            .HorizontalAlignment = Element.ALIGN_LEFT
        }
        headerTable.AddCell(secondHeaderCell)

        Dim phraseThird As New Phrase()
        phraseThird.Add(New Chunk(PageTitle, New Font(Font.FontFamily.TIMES_ROMAN, 16, Font.BOLD)))
        phraseThird.Add(New Chunk(Environment.NewLine))
        phraseThird.Add(New Chunk(PageTitle2, New Font(Font.FontFamily.TIMES_ROMAN, 16, Font.BOLD)))
        phraseThird.Add(New Chunk(Environment.NewLine))
        phraseThird.Add(New Chunk(Environment.NewLine))
        phraseThird.Add(New Chunk(PageNote, New Font(Font.FontFamily.TIMES_ROMAN, 9)))

        Dim thirdHeaderCell As New PdfPCell(phraseThird)
        thirdHeaderCell.Border = 0
        thirdHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        thirdHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(thirdHeaderCell)

        headerTable.WriteSelectedRows(0, -1, 20, document.PageSize.Height - 20, cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True
        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(New Phrase("Print Date: " & Now.ToString("dd MMM yyyy HH:mm"), New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD))) With {
            .Border = 0,
            .HorizontalAlignment = Element.ALIGN_LEFT
        }
        footerTable.AddCell(leftFooterCell)

        Dim pageText As String = "Page " & writer.PageNumber.ToString() & " of "
        Dim pageFont As New Font(Font.FontFamily.TIMES_ROMAN, 8)
        Dim rightFooterCell As New PdfPCell(New Phrase(pageText, pageFont)) With {
            .Border = 0,
            .HorizontalAlignment = Element.ALIGN_RIGHT
        }
        footerTable.AddCell(rightFooterCell)

        Dim footerY As Single = document.PageSize.GetBottom(30)
        footerTable.WriteSelectedRows(0, -1, 36, footerY, cb)

        Dim baseFont As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
        Dim textWidth As Single = baseFont.GetWidthPoint(pageText, 8)
        Dim xPos As Single = document.PageSize.Width - textWidth - 1
        Dim yPos As Single = footerY - 10.0F

        cb.AddTemplate(template, xPos, yPos)
    End Sub

    Public Overrides Sub OnCloseDocument(writer As PdfWriter, document As Document)
        template.BeginText()
        template.SetFontAndSize(baseFont, 8)
        template.SetTextMatrix(0, 0)
        template.ShowText((writer.PageNumber - 1).ToString())
        template.EndText()

        PageTotalDoc = writer.PageNumber

        MyBase.OnCloseDocument(writer, document)
    End Sub
End Class
