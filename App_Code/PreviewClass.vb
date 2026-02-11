Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class PreviewClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetDataRow(thisString As String) As DataRow
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        thisAdapter.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

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

    Public Function GetDataTableSP(spName As String, params As List(Of SqlParameter)) As DataTable
        Dim dt As New DataTable()
        Using conn As New SqlConnection(myConn)
            Using cmd As New SqlCommand(spName, conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddRange(params.ToArray())

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim aluminiumData As DataTable = GetDataTableSP("sp_GetAluminiumBlindData", params)

                If aluminiumData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Aluminium"
                    pageEvent.PageTitle2 = "Blind"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(14, aluminiumData.Rows.Count - 1) As String

                    For i As Integer = 0 To aluminiumData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim cordLength As String = aluminiumData.Rows(i)("CL").ToString()
                        Dim cordLengthValue As String = aluminiumData.Rows(i)("CLValue").ToString()

                        Dim cordLengthText As String = cordLength
                        If cordLength = "Custom" Then
                            cordLengthText = String.Format("{0} : {1}mm", cordLength, cordLengthValue)
                        End If

                        Dim wandLength As String = aluminiumData.Rows(i)("WL").ToString()
                        Dim wandLengthValue As String = aluminiumData.Rows(i)("WLValue").ToString()

                        Dim wandLengthText As String = wandLength
                        If wandLength = "Custom" Then
                            wandLengthText = String.Format("{0} : {1}mm", wandLength, wandLengthValue)
                        End If

                        Dim totalBlinds As Integer = aluminiumData.Rows(i)("TotalItems")
                        Dim subType As String = "Single"
                        If totalBlinds > 1 Then subType = "2 on 1"

                        items(0, i) = "Item : " & number
                        items(1, i) = aluminiumData.Rows(i)("Room").ToString()
                        items(2, i) = aluminiumData.Rows(i)("Mounting").ToString()
                        items(3, i) = aluminiumData.Rows(i)("BlindAlias").ToString()
                        items(4, i) = aluminiumData.Rows(i)("ColourName").ToString()
                        items(5, i) = subType
                        items(6, i) = aluminiumData.Rows(i)("Width").ToString()
                        items(7, i) = aluminiumData.Rows(i)("Height").ToString()
                        items(8, i) = aluminiumData.Rows(i)("CtrlPosition").ToString()
                        items(9, i) = aluminiumData.Rows(i)("TiltPosition").ToString()
                        items(10, i) = cordLengthText
                        items(11, i) = wandLengthText
                        items(12, i) = aluminiumData.Rows(i)("Supply").ToString()
                        items(13, i) = aluminiumData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Aluminium Type", "Aluminium Colour", "Sub Type", "Width (mm)", "Drop (mm)", "Control Position", "Tilter Position", "Cord Length", "Wand Length", "Hold Down Clip", "Notes"}

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim cellularData As DataTable = GetDataTableSP("sp_GetCellularShadesData", params)

                If cellularData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Cellular"
                    pageEvent.PageTitle2 = "Shades"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(15, cellularData.Rows.Count - 1) As String

                    For i As Integer = 0 To cellularData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim cordLength As String = cellularData.Rows(i)("ControlLength").ToString()
                        Dim cordLengthValue As String = cellularData.Rows(i)("ControlLengthValue").ToString()

                        Dim cordLengthText As String = cordLength
                        If cordLength = "Custom" Then
                            cordLengthText = String.Format("{0} : {1}mm", cordLength, cordLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = cellularData.Rows(i)("Room").ToString()
                        items(2, i) = cellularData.Rows(i)("Mounting").ToString()
                        items(3, i) = cellularData.Rows(i)("BlindAlias").ToString()
                        items(4, i) = cellularData.Rows(i)("ControlName").ToString()
                        items(5, i) = cellularData.Rows(i)("Width").ToString()
                        items(6, i) = cellularData.Rows(i)("Drop").ToString()
                        items(7, i) = cellularData.Rows(i)("FabricName").ToString()
                        items(8, i) = cellularData.Rows(i)("FabricColour").ToString()
                        items(9, i) = cellularData.Rows(i)("FabricNameB").ToString()
                        items(10, i) = cellularData.Rows(i)("FabricColourB").ToString()
                        items(11, i) = cellularData.Rows(i)("ControlPosition").ToString()
                        items(12, i) = cordLengthText
                        items(13, i) = cellularData.Rows(i)("Supply").ToString()
                        items(14, i) = cellularData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim curtainData As DataTable = GetDataTableSP("sp_GetCurtainData", params)

                If curtainData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Curtain"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(19, curtainData.Rows.Count - 1) As String

                    For i As Integer = 0 To curtainData.Rows.Count - 1
                        Dim fabricId As String = curtainData.Rows(i)("FabricId").ToString()
                        Dim fabricName As String = GetFabricName(fabricId)

                        Dim fabricColourId As String = curtainData.Rows(i)("FabricColourId").ToString()
                        Dim fabricColourName As String = GetFabricColourName(fabricColourId)

                        Dim controlLength As String = curtainData.Rows(i)("CL").ToString()
                        If controlLength = "0" Then controlLength = String.Empty

                        Dim leftRetLengthValue As Integer = curtainData.Rows(i)("LeftRetLengthValue")
                        Dim rightRetLengthValue As Integer = curtainData.Rows(i)("RightRetLengthValue")

                        Dim returnLengthText As String = String.Format("L : {0} - R : {1}", leftRetLengthValue, rightRetLengthValue)

                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = curtainData.Rows(i)("Room").ToString()
                        items(2, i) = curtainData.Rows(i)("Mounting").ToString()
                        items(3, i) = curtainData.Rows(i)("BlindAlias").ToString()
                        items(4, i) = curtainData.Rows(i)("Heading").ToString()
                        items(5, i) = fabricName
                        items(6, i) = fabricColourName
                        items(7, i) = curtainData.Rows(i)("Width").ToString()
                        items(8, i) = curtainData.Rows(i)("Height").ToString()
                        items(9, i) = curtainData.Rows(i)("TrackType").ToString()
                        items(10, i) = curtainData.Rows(i)("TrackColour").ToString()
                        items(11, i) = curtainData.Rows(i)("TrackDraw").ToString()
                        items(12, i) = curtainData.Rows(i)("StackPosition").ToString()
                        items(13, i) = curtainData.Rows(i)("ControlColour").ToString()
                        items(14, i) = controlLength
                        items(15, i) = returnLengthText
                        items(16, i) = curtainData.Rows(i)("BottomHem").ToString()
                        items(17, i) = curtainData.Rows(i)("Supply").ToString()
                        items(18, i) = curtainData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim designData As DataTable = GetDataTableSP("sp_GetDesignShadesData", params)

                If designData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Design"
                    pageEvent.PageTitle2 = "Shades"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(14, designData.Rows.Count - 1) As String

                    For i As Integer = 0 To designData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlName As String = designData.Rows(i)("ControlName").ToString()
                        Dim controlColour As String = String.Empty
                        If controlName = "Chain" Then
                            controlColour = designData.Rows(i)("ChainName").ToString()
                        End If
                        If controlName = "Wand" Then
                            controlColour = designData.Rows(i)("WandColour").ToString()
                        End If

                        Dim controlLength As String = designData.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = designData.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = designData.Rows(i)("Room").ToString()
                        items(2, i) = designData.Rows(i)("Mounting").ToString()
                        items(3, i) = designData.Rows(i)("Width").ToString()
                        items(4, i) = designData.Rows(i)("Drop").ToString()
                        items(5, i) = designData.Rows(i)("FabricName").ToString()
                        items(6, i) = designData.Rows(i)("FabricColour").ToString()
                        items(7, i) = designData.Rows(i)("ColourName").ToString()
                        items(8, i) = designData.Rows(i)("StackPosition").ToString()
                        items(9, i) = designData.Rows(i)("ControlPosition").ToString()
                        items(10, i) = controlName
                        items(11, i) = controlColour
                        items(12, i) = controlLengthText
                        items(13, i) = designData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim lineaData As DataTable = GetDataTableSP("sp_GetLineaValanceData", params)

                If lineaData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Linea"
                    pageEvent.PageTitle2 = "Valance"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(14, lineaData.Rows.Count - 1) As String

                    For i As Integer = 0 To lineaData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim returnLength As String = lineaData.Rows(i)("ReturnLength").ToString()
                        Dim returnLengthValue As String = lineaData.Rows(i)("ReturnLengthValue").ToString()

                        Dim returnLengthText As String = returnLength
                        If returnLength = "Custom" Then
                            returnLengthText = String.Format("{0} : {1}mm", returnLength, returnLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = lineaData.Rows(i)("Room").ToString()
                        items(2, i) = lineaData.Rows(i)("Mounting").ToString()
                        items(3, i) = lineaData.Rows(i)("Width").ToString()
                        items(4, i) = lineaData.Rows(i)("TubeName").ToString()
                        items(5, i) = lineaData.Rows(i)("ColourName").ToString()
                        items(6, i) = lineaData.Rows(i)("FabricInsert").ToString()
                        items(7, i) = lineaData.Rows(i)("FabricName").ToString()
                        items(8, i) = lineaData.Rows(i)("FabricColour").ToString()
                        items(9, i) = lineaData.Rows(i)("BracketType").ToString()
                        items(10, i) = lineaData.Rows(i)("IsBlindIn").ToString()
                        items(11, i) = lineaData.Rows(i)("ReturnPosition").ToString()
                        items(12, i) = returnLengthText
                        items(13, i) = lineaData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim panelData As DataTable = GetDataTableSP("sp_GetPanelGlideData", params)

                If panelData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Panel"
                    pageEvent.PageTitle2 = "Glide"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(17, panelData.Rows.Count - 1) As String

                    For i As Integer = 0 To panelData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim layoutCode As String = panelData.Rows(i)("LayoutCode").ToString()
                        If panelData.Rows(i)("LayoutCode") = "Custom" Then
                            layoutCode = panelData.Rows(i)("LayoutCodeCustom").ToString()
                        End If

                        Dim wandLength As String = panelData.Rows(i)("WandLength").ToString()
                        Dim wandLengthValue As String = panelData.Rows(i)("WandLengthValue").ToString()

                        Dim wandLengthText As String = wandLength
                        If wandLength = "Custom" Then
                            wandLengthText = String.Format("{0} : {1}mm", wandLength, wandLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = panelData.Rows(i)("Room").ToString()
                        items(2, i) = panelData.Rows(i)("Mounting").ToString()
                        items(3, i) = panelData.Rows(i)("BlindName").ToString()
                        items(4, i) = panelData.Rows(i)("TubeName").ToString()
                        items(5, i) = panelData.Rows(i)("ColourName").ToString()
                        items(6, i) = panelData.Rows(i)("FabricName").ToString()
                        items(7, i) = panelData.Rows(i)("FabricColour").ToString()
                        items(8, i) = panelData.Rows(i)("Width").ToString()
                        items(9, i) = If(panelData.Rows(i)("Drop").ToString() <> "" AndAlso panelData.Rows(i)("Drop").ToString() <> "0", panelData.Rows(i)("Drop").ToString(), "")
                        items(10, i) = panelData.Rows(i)("WandColour").ToString()
                        items(11, i) = wandLengthText
                        items(12, i) = If(panelData.Rows(i)("PanelQty").ToString() <> "" AndAlso panelData.Rows(i)("PanelQty").ToString() <> "0", panelData.Rows(i)("PanelQty").ToString(), "")
                        items(13, i) = panelData.Rows(i)("TrackType").ToString()
                        items(14, i) = layoutCode
                        items(15, i) = panelData.Rows(i)("Batten").ToString()
                        items(16, i) = panelData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim pelmetData As DataTable = GetDataTableSP("sp_GetPelmetData", params)

                If pelmetData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Pelmet"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(15, pelmetData.Rows.Count - 1) As String

                    For i As Integer = 0 To pelmetData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = pelmetData.Rows(i)("Room").ToString()
                        items(2, i) = pelmetData.Rows(i)("Mounting").ToString()
                        items(3, i) = pelmetData.Rows(i)("TubeName").ToString()
                        items(4, i) = pelmetData.Rows(i)("FabricName").ToString()
                        items(5, i) = pelmetData.Rows(i)("FabricColour").ToString()
                        items(6, i) = pelmetData.Rows(i)("Batten").ToString()
                        items(7, i) = pelmetData.Rows(i)("LayoutCode").ToString()
                        items(8, i) = pelmetData.Rows(i)("Width").ToString()
                        items(9, i) = If(pelmetData.Rows(i)("WidthB").ToString() <> "" AndAlso pelmetData.Rows(i)("WidthB").ToString() <> "0", pelmetData.Rows(i)("WidthB").ToString(), "")
                        items(10, i) = If(pelmetData.Rows(i)("WidthC").ToString() <> "" AndAlso pelmetData.Rows(i)("WidthC").ToString() <> "0", pelmetData.Rows(i)("WidthC").ToString(), "")
                        items(11, i) = pelmetData.Rows(i)("ReturnPosition").ToString()
                        items(12, i) = pelmetData.Rows(i)("ReturnLengthValue").ToString()
                        items(13, i) = pelmetData.Rows(i)("ReturnLengthValueB").ToString()
                        items(14, i) = pelmetData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim privacyData As DataTable = GetDataTableSP("sp_GetPrivacyVenetianData", params)

                If privacyData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Privacy"
                    pageEvent.PageTitle2 = "Venetian"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(12, privacyData.Rows.Count - 1) As String

                    For i As Integer = 0 To privacyData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlLength As String = privacyData.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = privacyData.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = privacyData.Rows(i)("Room").ToString()
                        items(2, i) = privacyData.Rows(i)("Mounting").ToString()
                        items(3, i) = privacyData.Rows(i)("BlindName").ToString()
                        items(4, i) = privacyData.Rows(i)("ColourName").ToString()
                        items(5, i) = privacyData.Rows(i)("ControlPosition").ToString()
                        items(6, i) = privacyData.Rows(i)("TilterPosition").ToString()
                        items(7, i) = privacyData.Rows(i)("Width").ToString()
                        items(8, i) = privacyData.Rows(i)("Drop").ToString()
                        items(9, i) = controlLengthText
                        items(10, i) = privacyData.Rows(i)("Supply").ToString()
                        items(11, i) = privacyData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim rollerData As DataTable = GetDataTableSP("sp_GetRollerBlindData", params)

                If rollerData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Roller"
                    pageEvent.PageTitle2 = "Blind"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(31, rollerData.Rows.Count - 1) As String

                    For i As Integer = 0 To rollerData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim itemBlind As String = rollerData.Rows(i)("Item").ToString()

                        Dim blindName As String = rollerData.Rows(i)("BlindName").ToString()
                        Dim blindAlias As String = rollerData.Rows(i)("BlindAlias").ToString()

                        Dim tubeType As String = rollerData.Rows(i)("TubeType").ToString()

                        Dim tubeName As String = GetItemData("SELECT Name FROM ProductTubes WHERE Id='" & rollerData.Rows(i)("TubeType").ToString() & "'")
                        Dim tubeAlias As String = GetItemData("SELECT Alias FROM ProductTubes WHERE Id='" & rollerData.Rows(i)("TubeType").ToString() & "'")
                        Dim controlName As String = GetItemData("SELECT Name FROM ProductControls WHERE Id='" & rollerData.Rows(i)("ControlType").ToString() & "'")
                        Dim controlAlias As String = GetItemData("SELECT Name FROM ProductControls WHERE Id='" & rollerData.Rows(i)("ControlType").ToString() & "'")
                        Dim colourName As String = GetItemData("SELECT Name FROM ProductColours WHERE Id='" & rollerData.Rows(i)("ColourType").ToString() & "'")

                        If tubeType = "Standard" Then

                        End If

                        Dim fabricType As String = GetItemData("SELECT Name FROM Fabrics WHERE Id='" & rollerData.Rows(i)("Fabric").ToString() & "'")
                        Dim fabricColour As String = GetItemData("SELECT Colour FROM FabricColours WHERE Id='" & rollerData.Rows(i)("FabricColour").ToString() & "'")

                        Dim chainName As String = GetItemData("SELECT Name FROM Chains WHERE Id='" & rollerData.Rows(i)("Chain").ToString() & "'")

                        Dim chainColour As String = String.Empty
                        Dim remoteType As String = chainName

                        If controlName = "Chain" Then
                            chainColour = chainName : remoteType = ""
                        End If

                        Dim chainLength As String = String.Empty
                        Dim chainLengthValue As Integer = rollerData.Rows(i)("ChainLength")
                        If chainLengthValue > 0 Then
                            chainLength = rollerData.Rows(i)("ChainLength").ToString()
                        End If

                        Dim bottomType As String = GetItemData("SELECT Name FROM Bottoms WHERE Id='" & rollerData.Rows(i)("BottomType").ToString() & "'")
                        Dim bottomColour As String = GetItemData("SELECT Colour FROM BottomColours WHERE Id='" & rollerData.Rows(i)("BottomColour").ToString() & "'")
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
                        items(1, i) = rollerData.Rows(i)("Room").ToString()
                        items(2, i) = rollerData.Rows(i)("Mounting").ToString()
                        items(3, i) = rollerType
                        items(4, i) = tubeAlias
                        items(5, i) = controlName
                        items(6, i) = colourName
                        items(7, i) = fabricType
                        items(8, i) = fabricColour
                        items(9, i) = rollerData.Rows(i)("Roll").ToString()
                        items(10, i) = rollerData.Rows(i)("Width").ToString()
                        items(11, i) = rollerData.Rows(i)("Height").ToString()
                        items(12, i) = rollerData.Rows(i)("ControlPosition").ToString()
                        items(13, i) = remoteType
                        items(14, i) = rollerData.Rows(i)("DryContact").ToString()
                        items(15, i) = rollerData.Rows(i)("Charger").ToString()
                        items(16, i) = rollerData.Rows(i)("ExtensionCable").ToString()
                        items(17, i) = rollerData.Rows(i)("NeoBox").ToString()
                        items(18, i) = chainColour
                        items(19, i) = rollerData.Rows(i)("ChainStopper").ToString()
                        items(20, i) = chainLength
                        items(21, i) = bottomType
                        items(22, i) = bottomColour
                        items(23, i) = rollerData.Rows(i)("FlatOption").ToString()
                        items(24, i) = rollerData.Rows(i)("BracketExtension").ToString()
                        items(25, i) = rollerData.Rows(i)("SpringAssist").ToString()
                        items(26, i) = rollerData.Rows(i)("BracketSize").ToString()
                        items(27, i) = rollerData.Rows(i)("Adjusting").ToString()
                        items(28, i) = rollerData.Rows(i)("TopTrack").ToString()
                        items(29, i) = rollerData.Rows(i)("PrintingImage").ToString()
                        items(30, i) = rollerData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Roller Type", "Tube Type", "Control Type", "Bracket Colour", "Fabric Type", "Fabric Colour", "Roll Direction", "Width (mm)", "Drop (mm)", "Control Position", "Remote Type", "Dry Contact", "Battery Charger", "Extension Cable", "Neo Box", "Chain Type", "Chain Stopper", "Chain Length", "Bottom Type", "Bottom Colour", "Bottom Option", "Bracket Extension", "Spring Assist", "Bracket Size", "Adjusting Spanner", "Top Track", "Fabric Printing", "Notes"}

                        For row As Integer = 0 To headers.Length - 1
                            Dim cellHeader As New PdfPCell(New Phrase(headers(row), fontHeader))
                            cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT
                            cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellHeader.BackgroundColor = New BaseColor(200, 200, 200)
                            cellHeader.MinimumHeight = 20
                            table.AddCell(cellHeader)

                            For col As Integer = i To Math.Min(i + 5, items.GetLength(1) - 1)
                                Dim cellContent As New PdfPCell(New Phrase(items(row, col), fontContent))
                                cellContent.HorizontalAlignment = Element.ALIGN_CENTER
                                cellContent.VerticalAlignment = Element.ALIGN_MIDDLE
                                cellContent.MinimumHeight = 20
                                table.AddCell(cellContent)
                            Next

                            For col As Integer = items.GetLength(1) To i + 5
                                Dim emptyCell As New PdfPCell(New Phrase("", fontContent))
                                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER
                                emptyCell.VerticalAlignment = Element.ALIGN_MIDDLE
                                emptyCell.MinimumHeight = 20
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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim romanData As DataTable = GetDataTableSP("sp_GetRomanBlindData", params)

                If romanData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Roman"
                    pageEvent.PageTitle2 = "Blind"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(20, romanData.Rows.Count - 1) As String

                    For i As Integer = 0 To romanData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlName As String = romanData.Rows(i)("ControlName").ToString()

                        Dim chainName As String = GetItemData("SELECT Name FROM Chains WHERE Id='" & romanData.Rows(i)("ChainId").ToString() & "'")

                        Dim controlColour As String = String.Empty
                        Dim remoteType As String = String.Empty

                        If controlName = "Chain" Then controlColour = chainName
                        If controlName = "Reg Cord Lock" OrElse controlName = "Cord Lock" Then
                            controlColour = romanData.Rows(i)("ControlColour").ToString()
                        End If
                        If controlName.Contains("Alpha") OrElse controlName = "Mercure" OrElse controlName = "Altus" OrElse controlName = "Sonesse 30 WF" Then
                            remoteType = chainName
                        End If

                        Dim controlLength As String = romanData.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = romanData.Rows(i)("ControlLengthValue").ToString()
                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = romanData.Rows(i)("Room").ToString()
                        items(2, i) = romanData.Rows(i)("Mounting").ToString()
                        items(3, i) = romanData.Rows(i)("Width").ToString()
                        items(4, i) = romanData.Rows(i)("FabricName").ToString()
                        items(5, i) = romanData.Rows(i)("FabricColour").ToString()
                        items(6, i) = romanData.Rows(i)("Drop").ToString()
                        items(7, i) = romanData.Rows(i)("TubeName").ToString()
                        items(8, i) = romanData.Rows(i)("ControlName").ToString()
                        items(9, i) = romanData.Rows(i)("ControlPosition").ToString()
                        items(10, i) = controlColour
                        items(11, i) = controlLengthText
                        items(12, i) = remoteType
                        items(13, i) = romanData.Rows(i)("Charger").ToString()
                        items(14, i) = romanData.Rows(i)("DryContact").ToString()
                        items(15, i) = romanData.Rows(i)("ExtensionCable").ToString()
                        items(16, i) = romanData.Rows(i)("Supply").ToString()
                        items(17, i) = romanData.Rows(i)("ValanceOption").ToString()
                        items(18, i) = romanData.Rows(i)("Batten").ToString()
                        items(19, i) = romanData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Fabric Type", "Fabric Colour", "Width (mm)", "Drop (mm)", "Roman Style", "Control Type", "Control Position", "Control Colour", "Control Length", "Remote Type", "DryContact", "Motor Charger", "Extension Cable", "Neo Box", "Valance Option", "Batten Colour", "Notes"}

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim venetianData As DataTable = GetDataTableSP("sp_GetVenetianBlindData", params)

                If venetianData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Venetian"
                    pageEvent.PageTitle2 = "Blind"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(18, venetianData.Rows.Count - 1) As String

                    For i As Integer = 0 To venetianData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlLength As String = venetianData.Rows(i)("CL").ToString()
                        Dim controlLengthValue As String = venetianData.Rows(i)("CLValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        Dim valancesize As String = venetianData.Rows(i)("ValanceSize").ToString()
                        Dim valancesizeValue As String = venetianData.Rows(i)("ValanceSizeValue").ToString()

                        Dim valancesizeText As String = valancesize
                        If valancesize = "Custom" Then
                            valancesizeText = String.Format("{0} : {1}mm", valancesize, valancesizeValue)
                        End If

                        Dim returnLength As String = venetianData.Rows(i)("ReturnLength").ToString()
                        Dim returnLengthValue As String = venetianData.Rows(i)("ReturnLengthValue").ToString()

                        Dim returnLengthText As String = returnLength
                        If returnLength = "Custom" Then
                            returnLengthText = String.Format("{0} : {1}mm", returnLength, returnLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = venetianData.Rows(i)("Room").ToString()
                        items(2, i) = venetianData.Rows(i)("Mounting").ToString()
                        items(3, i) = venetianData.Rows(i)("BlindName").ToString()
                        items(4, i) = venetianData.Rows(i)("ColourName").ToString()
                        items(5, i) = venetianData.Rows(i)("SubType").ToString()
                        items(6, i) = venetianData.Rows(i)("Width").ToString()
                        items(7, i) = venetianData.Rows(i)("Height").ToString()
                        items(8, i) = venetianData.Rows(i)("Tassel").ToString()
                        items(9, i) = venetianData.Rows(i)("CtrlPosition").ToString()
                        items(10, i) = venetianData.Rows(i)("TiltPosition").ToString()
                        items(11, i) = controlLengthText
                        items(12, i) = venetianData.Rows(i)("ValanceType").ToString()
                        items(13, i) = valancesizeText
                        items(14, i) = venetianData.Rows(i)("ReturnPosition").ToString()
                        items(15, i) = returnLengthText
                        items(16, i) = venetianData.Rows(i)("Supply").ToString()
                        items(17, i) = venetianData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }

                Dim verticalData As DataTable = GetDataTableSP("sp_GetVerticalBlindData", params)

                If verticalData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Vertical"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(21, verticalData.Rows.Count - 1) As String

                    For i As Integer = 0 To verticalData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlName As String = verticalData.Rows(i)("ControlName").ToString()
                        Dim controlColour As String = String.Empty

                        If controlName = "Chain" Then controlColour = verticalData.Rows(i)("ChainName").ToString()
                        If controlName = "Wand" Then controlColour = verticalData.Rows(i)("WandColour").ToString()

                        Dim controlLength As String = verticalData.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = verticalData.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = verticalData.Rows(i)("Room").ToString()
                        items(2, i) = verticalData.Rows(i)("Mounting").ToString()
                        items(3, i) = verticalData.Rows(i)("BlindName").ToString()
                        items(4, i) = verticalData.Rows(i)("TubeName").ToString()
                        items(5, i) = If(verticalData.Rows(i)("QtyBlade").ToString() <> "" AndAlso verticalData.Rows(i)("QtyBlade").ToString() <> "0", verticalData.Rows(i)("QtyBlade").ToString(), "")
                        items(6, i) = verticalData.Rows(i)("FabricInsert").ToString()
                        items(7, i) = verticalData.Rows(i)("FabricName").ToString()
                        items(8, i) = verticalData.Rows(i)("FabricColour").ToString()
                        items(9, i) = verticalData.Rows(i)("Width").ToString()
                        items(10, i) = If(verticalData.Rows(i)("Drop").ToString() <> "" AndAlso verticalData.Rows(i)("Drop").ToString() <> "0", verticalData.Rows(i)("Drop").ToString(), "")
                        items(11, i) = verticalData.Rows(i)("StackPosition").ToString()
                        items(12, i) = verticalData.Rows(i)("ControlPosition").ToString()
                        items(13, i) = controlName
                        items(14, i) = verticalData.Rows(i)("ColourName").ToString()
                        items(15, i) = controlColour
                        items(16, i) = controlLengthText
                        items(17, i) = verticalData.Rows(i)("BottomJoining").ToString()
                        items(18, i) = verticalData.Rows(i)("BracketExtension").ToString()
                        items(19, i) = verticalData.Rows(i)("Sloping").ToString()
                        items(20, i) = verticalData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Vertical Type", "Slat Type", "Blade Qty", "Fabric Insert", "Fabric Type", "Fabric Colour", "Width (mm)", "Drop (mm)", "Stack Position", "Control Position", "Control Type", "Track Colour", "Control Colour", "Control Length", "Bottom Joining", "Extension Bracket", "Sloping", "Notes"}

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim saphoraData As DataTable = GetDataTableSP("sp_GetSaphoraDrapeData", params)

                If saphoraData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Saphora"
                    pageEvent.PageTitle2 = "Drape"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(17, saphoraData.Rows.Count - 1) As String

                    For i As Integer = 0 To saphoraData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlName As String = saphoraData.Rows(i)("ControlName").ToString()
                        Dim controlColour As String = String.Empty
                        If controlName = "Chain" Then controlColour = saphoraData.Rows(i)("ChainName").ToString()
                        If controlName = "Wand" Then controlColour = saphoraData.Rows(i)("WandColour").ToString()

                        Dim controlLength As String = saphoraData.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = saphoraData.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = saphoraData.Rows(i)("Room").ToString()
                        items(2, i) = saphoraData.Rows(i)("Mounting").ToString()
                        items(3, i) = saphoraData.Rows(i)("BlindName").ToString()
                        items(4, i) = saphoraData.Rows(i)("TubeName").ToString()
                        items(5, i) = saphoraData.Rows(i)("FabricName").ToString()
                        items(6, i) = saphoraData.Rows(i)("FabricColour").ToString()
                        items(7, i) = saphoraData.Rows(i)("Width").ToString()
                        items(8, i) = saphoraData.Rows(i)("Drop").ToString()
                        items(9, i) = saphoraData.Rows(i)("ColourName").ToString()
                        items(10, i) = saphoraData.Rows(i)("StackPosition").ToString()
                        items(11, i) = saphoraData.Rows(i)("ControlPosition").ToString()
                        items(12, i) = controlName
                        items(13, i) = controlColour
                        items(14, i) = controlLengthText
                        items(15, i) = saphoraData.Rows(i)("BracketExtension").ToString()
                        items(16, i) = saphoraData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Vertical Type", "Slat Type", "Fabric Type", "Fabric Colour", "Width (mm)", "Drop (mm)", "Track Colour", "Stack Position", "Control Position", "Control Type", "Control Colour", "Control Length", "Extension Bracket", "Notes"}

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim shutterData As DataTable = GetDataTableSP("sp_GetShutterExpressData", params)

                If shutterData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "EXPRESS"
                    pageEvent.PageTitle2 = "Skyline Shutter"

                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim items(38, shutterData.Rows.Count - 1) As String

                    For i As Integer = 0 To shutterData.Rows.Count - 1
                        Dim layoutCode As String = If(shutterData.Rows(i)("LayoutCode").ToString() = "Other", shutterData.Rows(i)("LayoutCodeCustom").ToString(), shutterData.Rows(i)("LayoutCode").ToString())

                        Dim gapList As New List(Of String)

                        Dim midrailList As New List(Of String)

                        Dim midrailHeight1 As Integer = 0
                        Dim midrailHeight2 As Integer = 0

                        If Not IsDBNull(shutterData.Rows(i)("MidrailHeight1")) Then
                            midrailHeight1 = shutterData.Rows(i)("MidrailHeight1")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("MidrailHeight2")) Then
                            midrailHeight2 = shutterData.Rows(i)("MidrailHeight2")
                        End If

                        If midrailHeight1 > 0 Then midrailList.Add(String.Format("1 : {0}", midrailHeight1))
                        If midrailHeight2 > 0 Then midrailList.Add(String.Format("2 : {0}", midrailHeight2))

                        Dim midrailHeight As String = String.Join(", ", midrailList)

                        Dim headerLengthValue As Integer = 0
                        Dim headerLengthText As String = String.Empty
                        If Not IsDBNull(shutterData.Rows(i)("CustomHeaderLength")) Then
                            headerLengthValue = shutterData.Rows(i)("CustomHeaderLength")
                        End If
                        If headerLengthValue > 0 Then
                            headerLengthText = headerLengthValue.ToString()
                        End If

                        Dim gap1 As Integer = 0
                        Dim gap2 As Integer = 0
                        Dim gap3 As Integer = 0
                        Dim gap4 As Integer = 0
                        Dim gap5 As Integer = 0

                        If Not IsDBNull(shutterData.Rows(i)("Gap1")) Then
                            gap1 = shutterData.Rows(i)("Gap1")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("Gap2")) Then
                            gap2 = shutterData.Rows(i)("Gap2")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("Gap3")) Then
                            gap3 = shutterData.Rows(i)("Gap3")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("Gap4")) Then
                            gap4 = shutterData.Rows(i)("Gap4")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("Gap5")) Then
                            gap5 = shutterData.Rows(i)("Gap5")
                        End If

                        If gap1 > 0 Then gapList.Add("Gap 1 : " & gap1)
                        If gap2 > 0 Then gapList.Add("Gap 2 : " & gap2)
                        If gap3 > 0 Then gapList.Add("Gap 3 : " & gap3)
                        If gap4 > 0 Then gapList.Add("Gap 4 : " & gap4)
                        If gap5 > 0 Then gapList.Add("Gap 5 : " & gap5)

                        Dim gapPosition As String = String.Join(", ", gapList)

                        Dim split1 As Integer = 0
                        Dim split2 As Integer = 0

                        If Not IsDBNull(shutterData.Rows(i)("SplitHeight1")) Then
                            split1 = shutterData.Rows(i)("SplitHeight1")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("SplitHeight2")) Then
                            split2 = shutterData.Rows(i)("SplitHeight2")
                        End If

                        Dim splitHeigth As String = String.Format("1st : {0}, 2nd : {1}", split1, split2)

                        Dim horizontalHeight As String = String.Empty
                        Dim horizontalTPostHeight As Integer = 0
                        If Not IsDBNull(shutterData.Rows(i)("HorizontalTPostHeight")) Then
                            horizontalTPostHeight = shutterData.Rows(i)("HorizontalTPostHeight")
                        End If

                        If horizontalTPostHeight > 0 Then
                            horizontalHeight = shutterData.Rows(i)("HorizontalTPostHeight").ToString()
                        End If

                        Dim bottomTrack As String = shutterData.Rows(i)("BottomTrackType").ToString()
                        If shutterData.Rows(i)("BottomTrackRecess").ToString() = "Yes" Then
                            bottomTrack = String.Format("{0} | Recess: Yes", shutterData.Rows(i)("BottomTrackType").ToString())
                        End If
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = shutterData.Rows(i)("Room").ToString()
                        items(2, i) = shutterData.Rows(i)("Width").ToString()
                        items(3, i) = shutterData.Rows(i)("Drop").ToString()
                        items(4, i) = shutterData.Rows(i)("Mounting").ToString()
                        items(5, i) = shutterData.Rows(i)("ColourType").ToString()
                        items(6, i) = shutterData.Rows(i)("LouvreSize").ToString()
                        items(7, i) = shutterData.Rows(i)("LouvrePosition").ToString()
                        items(8, i) = midrailHeight
                        items(9, i) = shutterData.Rows(i)("MidrailCritical").ToString()
                        items(10, i) = shutterData.Rows(i)("HingeColour").ToString()
                        items(11, i) = shutterData.Rows(i)("BlindName").ToString()
                        items(12, i) = shutterData.Rows(i)("SemiInsideMount").ToString()
                        items(13, i) = shutterData.Rows(i)("PanelQty").ToString()
                        items(14, i) = headerLengthText
                        items(15, i) = shutterData.Rows(i)("JoinedPanels").ToString()
                        items(16, i) = layoutCode
                        items(17, i) = shutterData.Rows(i)("FrameType").ToString()
                        items(18, i) = shutterData.Rows(i)("FrameLeft").ToString()
                        items(19, i) = shutterData.Rows(i)("FrameRight").ToString()
                        items(20, i) = shutterData.Rows(i)("FrameTop").ToString()
                        items(21, i) = shutterData.Rows(i)("FrameBottom").ToString()
                        items(22, i) = bottomTrack
                        items(23, i) = shutterData.Rows(i)("Buildout").ToString()
                        items(24, i) = shutterData.Rows(i)("BuildoutPosition").ToString()
                        items(25, i) = shutterData.Rows(i)("SameSizePanel").ToString()
                        items(26, i) = gapPosition
                        items(27, i) = horizontalHeight
                        items(28, i) = shutterData.Rows(i)("HorizontalTPost").ToString()
                        items(29, i) = shutterData.Rows(i)("TiltrodType").ToString()
                        items(30, i) = shutterData.Rows(i)("TiltrodSplit").ToString()
                        items(31, i) = splitHeigth
                        items(32, i) = shutterData.Rows(i)("ReverseHinged").ToString()
                        items(33, i) = shutterData.Rows(i)("PelmetFlat").ToString()
                        items(34, i) = shutterData.Rows(i)("ExtraFascia").ToString()
                        items(35, i) = shutterData.Rows(i)("HingesLoose").ToString()
                        items(36, i) = FormatNumber(shutterData.Rows(i)("SquareMetre"), 2)
                        items(37, i) = shutterData.Rows(i)("Notes").ToString()
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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim shutterData As DataTable = GetDataTableSP("sp_GetShutterOceanData", params)

                If shutterData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "OCEAN"
                    pageEvent.PageTitle2 = "Skyline Shutter"

                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim items(41, shutterData.Rows.Count - 1) As String

                    For i As Integer = 0 To shutterData.Rows.Count - 1
                        Dim layoutCode As String = If(shutterData.Rows(i)("LayoutCode").ToString() = "Other", shutterData.Rows(i)("LayoutCodeCustom").ToString(), shutterData.Rows(i)("LayoutCode").ToString())

                        Dim gapList As New List(Of String)

                        Dim midrailList As New List(Of String)

                        Dim midrailHeight1 As Integer = 0
                        Dim midrailHeight2 As Integer = 0

                        If Not IsDBNull(shutterData.Rows(i)("MidrailHeight1")) Then
                            midrailHeight1 = shutterData.Rows(i)("MidrailHeight1")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("MidrailHeight2")) Then
                            midrailHeight2 = shutterData.Rows(i)("MidrailHeight2")
                        End If

                        If midrailHeight1 > 0 Then midrailList.Add(String.Format("1 : {0}", midrailHeight1))
                        If midrailHeight2 > 0 Then midrailList.Add(String.Format("2 : {0}", midrailHeight2))

                        Dim midrailHeight As String = String.Join(", ", midrailList)

                        Dim headerLengthValue As Integer = 0
                        Dim headerLengthText As String = String.Empty
                        If Not IsDBNull(shutterData.Rows(i)("CustomHeaderLength")) Then
                            headerLengthValue = shutterData.Rows(i)("CustomHeaderLength")
                        End If
                        If headerLengthValue > 0 Then
                            headerLengthText = headerLengthValue.ToString()
                        End If

                        Dim gap1 As Integer = 0
                        Dim gap2 As Integer = 0
                        Dim gap3 As Integer = 0
                        Dim gap4 As Integer = 0
                        Dim gap5 As Integer = 0

                        If Not IsDBNull(shutterData.Rows(i)("Gap1")) Then
                            gap1 = shutterData.Rows(i)("Gap1")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("Gap2")) Then
                            gap2 = shutterData.Rows(i)("Gap2")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("Gap3")) Then
                            gap3 = shutterData.Rows(i)("Gap3")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("Gap4")) Then
                            gap4 = shutterData.Rows(i)("Gap4")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("Gap5")) Then
                            gap5 = shutterData.Rows(i)("Gap5")
                        End If

                        If gap1 > 0 Then gapList.Add("Gap 1 : " & gap1)
                        If gap2 > 0 Then gapList.Add("Gap 2 : " & gap2)
                        If gap3 > 0 Then gapList.Add("Gap 3 : " & gap3)
                        If gap4 > 0 Then gapList.Add("Gap 4 : " & gap4)
                        If gap5 > 0 Then gapList.Add("Gap 5 : " & gap5)

                        Dim gapPosition As String = String.Join(", ", gapList)

                        Dim split1 As Integer = 0
                        Dim split2 As Integer = 0

                        If Not IsDBNull(shutterData.Rows(i)("SplitHeight1")) Then
                            split1 = shutterData.Rows(i)("SplitHeight1")
                        End If
                        If Not IsDBNull(shutterData.Rows(i)("SplitHeight2")) Then
                            split2 = shutterData.Rows(i)("SplitHeight2")
                        End If

                        Dim splitHeigth As String = String.Format("1st : {0}, 2nd : {1}", split1, split2)

                        Dim horizontalHeight As String = String.Empty
                        Dim horizontalTPostHeight As Integer = 0
                        If Not IsDBNull(shutterData.Rows(i)("HorizontalTPostHeight")) Then
                            horizontalTPostHeight = shutterData.Rows(i)("HorizontalTPostHeight")
                        End If

                        If horizontalTPostHeight > 0 Then
                            horizontalHeight = shutterData.Rows(i)("HorizontalTPostHeight").ToString()
                        End If

                        Dim bottomTrack As String = shutterData.Rows(i)("BottomTrackType").ToString()
                        If shutterData.Rows(i)("BottomTrackRecess").ToString() = "Yes" Then
                            bottomTrack = String.Format("{0} | Recess: Yes", shutterData.Rows(i)("BottomTrackType").ToString())
                        End If
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = shutterData.Rows(i)("Room").ToString()
                        items(2, i) = shutterData.Rows(i)("Width").ToString()
                        items(3, i) = shutterData.Rows(i)("Drop").ToString()
                        items(4, i) = shutterData.Rows(i)("Mounting").ToString()
                        items(5, i) = shutterData.Rows(i)("ColourType").ToString()
                        items(6, i) = shutterData.Rows(i)("LouvreSize").ToString()
                        items(7, i) = shutterData.Rows(i)("LouvrePosition").ToString()
                        items(8, i) = midrailHeight
                        items(9, i) = shutterData.Rows(i)("MidrailCritical").ToString()
                        items(10, i) = shutterData.Rows(i)("HingeColour").ToString()
                        items(11, i) = shutterData.Rows(i)("BlindName").ToString()
                        items(12, i) = shutterData.Rows(i)("SemiInsideMount").ToString()
                        items(13, i) = shutterData.Rows(i)("PanelQty").ToString()
                        items(14, i) = headerLengthText
                        items(15, i) = shutterData.Rows(i)("JoinedPanels").ToString()
                        items(16, i) = layoutCode
                        items(17, i) = shutterData.Rows(i)("FrameType").ToString()
                        items(18, i) = shutterData.Rows(i)("FrameLeft").ToString()
                        items(19, i) = shutterData.Rows(i)("FrameRight").ToString()
                        items(20, i) = shutterData.Rows(i)("FrameTop").ToString()
                        items(21, i) = shutterData.Rows(i)("FrameBottom").ToString()
                        items(22, i) = bottomTrack
                        items(23, i) = shutterData.Rows(i)("Buildout").ToString()
                        items(24, i) = shutterData.Rows(i)("BuildoutPosition").ToString()
                        items(25, i) = shutterData.Rows(i)("SameSizePanel").ToString()
                        items(26, i) = gapPosition
                        items(27, i) = horizontalHeight
                        items(28, i) = shutterData.Rows(i)("HorizontalTPost").ToString()
                        items(29, i) = shutterData.Rows(i)("TiltrodType").ToString()
                        items(30, i) = shutterData.Rows(i)("TiltrodSplit").ToString()
                        items(31, i) = splitHeigth
                        items(32, i) = shutterData.Rows(i)("ReverseHinged").ToString()
                        items(33, i) = shutterData.Rows(i)("PelmetFlat").ToString()
                        items(34, i) = shutterData.Rows(i)("ExtraFascia").ToString()
                        items(35, i) = shutterData.Rows(i)("HingesLoose").ToString()
                        items(36, i) = shutterData.Rows(i)("DoorCutOut").ToString()
                        items(37, i) = shutterData.Rows(i)("SpecialShape").ToString()
                        items(38, i) = shutterData.Rows(i)("TemplateProvided").ToString()
                        items(39, i) = FormatNumber(shutterData.Rows(i)("SquareMetre"), 2)
                        items(40, i) = shutterData.Rows(i)("Notes").ToString()
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
                Dim evolveData As DataTable = GetDataTable("SELECT OrderDetails.*, ProductColours.Name AS ColourType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id LEFT JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE OrderDetails.HeaderId='" & headerId & "' AND Designs.Name='Evolve Shutter Ocean' AND OrderDetails.Active=1 ORDER BY OrderDetails.Id ASC")

                If evolveData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "OCEAN"
                    pageEvent.PageTitle2 = "Evolve Shutter"

                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim items(37, evolveData.Rows.Count - 1) As String

                    For i As Integer = 0 To evolveData.Rows.Count - 1
                        Dim layoutCode As String = If(evolveData.Rows(i)("LayoutCode").ToString() = "Other", evolveData.Rows(i)("LayoutCodeCustom").ToString(), evolveData.Rows(i)("LayoutCode").ToString())

                        Dim gapList As New List(Of String)

                        Dim midrailList As New List(Of String)

                        Dim midrailHeight1 As Integer = 0
                        Dim midrailHeight2 As Integer = 0

                        If Not IsDBNull(evolveData.Rows(i)("MidrailHeight1")) Then
                            midrailHeight1 = evolveData.Rows(i)("MidrailHeight1")
                        End If
                        If Not IsDBNull(evolveData.Rows(i)("MidrailHeight2")) Then
                            midrailHeight2 = evolveData.Rows(i)("MidrailHeight2")
                        End If

                        If midrailHeight1 > 0 Then midrailList.Add(String.Format("1 : {0}", midrailHeight1))
                        If midrailHeight2 > 0 Then midrailList.Add(String.Format("2 : {0}", midrailHeight2))

                        Dim midrailHeight As String = String.Join(", ", midrailList)

                        Dim headerLengthValue As Integer = 0
                        Dim headerLengthText As String = String.Empty
                        If Not IsDBNull(evolveData.Rows(i)("CustomHeaderLength")) Then
                            headerLengthValue = evolveData.Rows(i)("CustomHeaderLength")
                        End If
                        If headerLengthValue > 0 Then
                            headerLengthText = headerLengthValue.ToString()
                        End If

                        Dim gap1 As Integer = 0
                        Dim gap2 As Integer = 0
                        Dim gap3 As Integer = 0
                        Dim gap4 As Integer = 0
                        Dim gap5 As Integer = 0

                        If Not IsDBNull(evolveData.Rows(i)("Gap1")) Then
                            gap1 = evolveData.Rows(i)("Gap1")
                        End If
                        If Not IsDBNull(evolveData.Rows(i)("Gap2")) Then
                            gap2 = evolveData.Rows(i)("Gap2")
                        End If
                        If Not IsDBNull(evolveData.Rows(i)("Gap3")) Then
                            gap3 = evolveData.Rows(i)("Gap3")
                        End If
                        If Not IsDBNull(evolveData.Rows(i)("Gap4")) Then
                            gap4 = evolveData.Rows(i)("Gap4")
                        End If
                        If Not IsDBNull(evolveData.Rows(i)("Gap5")) Then
                            gap5 = evolveData.Rows(i)("Gap5")
                        End If

                        If gap1 > 0 Then gapList.Add("Gap 1 : " & gap1)
                        If gap2 > 0 Then gapList.Add("Gap 2 : " & gap2)
                        If gap3 > 0 Then gapList.Add("Gap 3 : " & gap3)
                        If gap4 > 0 Then gapList.Add("Gap 4 : " & gap4)
                        If gap5 > 0 Then gapList.Add("Gap 5 : " & gap5)

                        Dim gapPosition As String = String.Join(", ", gapList)

                        Dim split1 As Integer = 0
                        Dim split2 As Integer = 0

                        If Not IsDBNull(evolveData.Rows(i)("SplitHeight1")) Then
                            split1 = evolveData.Rows(i)("SplitHeight1")
                        End If
                        If Not IsDBNull(evolveData.Rows(i)("SplitHeight2")) Then
                            split2 = evolveData.Rows(i)("SplitHeight2")
                        End If

                        Dim splitHeigth As String = String.Format("1st : {0}, 2nd : {1}", split1, split2)

                        Dim horizontalHeight As String = String.Empty
                        Dim horizontalTPostHeight As Integer = 0
                        If Not IsDBNull(evolveData.Rows(i)("HorizontalTPostHeight")) Then
                            horizontalTPostHeight = evolveData.Rows(i)("HorizontalTPostHeight")
                        End If

                        If horizontalTPostHeight > 0 Then
                            horizontalHeight = evolveData.Rows(i)("HorizontalTPostHeight").ToString()
                        End If

                        Dim bottomTrack As String = evolveData.Rows(i)("BottomTrackType").ToString()
                        If evolveData.Rows(i)("BottomTrackRecess").ToString() = "Yes" Then
                            bottomTrack = String.Format("{0} | Recess: Yes", evolveData.Rows(i)("BottomTrackType").ToString())
                        End If
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = evolveData.Rows(i)("Room").ToString()
                        items(2, i) = evolveData.Rows(i)("Width").ToString()
                        items(3, i) = evolveData.Rows(i)("Drop").ToString()
                        items(4, i) = evolveData.Rows(i)("Mounting").ToString()
                        items(5, i) = evolveData.Rows(i)("ColourType").ToString()
                        items(6, i) = evolveData.Rows(i)("LouvreSize").ToString()
                        items(7, i) = evolveData.Rows(i)("LouvrePosition").ToString()
                        items(8, i) = midrailHeight
                        items(9, i) = evolveData.Rows(i)("MidrailCritical").ToString()
                        items(10, i) = evolveData.Rows(i)("HingeColour").ToString()
                        items(11, i) = evolveData.Rows(i)("BlindName").ToString()
                        items(12, i) = evolveData.Rows(i)("SemiInsideMount").ToString()
                        items(13, i) = evolveData.Rows(i)("PanelQty").ToString()
                        items(14, i) = headerLengthText
                        items(15, i) = evolveData.Rows(i)("JoinedPanels").ToString()
                        items(16, i) = layoutCode
                        items(17, i) = evolveData.Rows(i)("FrameType").ToString()
                        items(18, i) = evolveData.Rows(i)("FrameLeft").ToString()
                        items(19, i) = evolveData.Rows(i)("FrameRight").ToString()
                        items(20, i) = evolveData.Rows(i)("FrameTop").ToString()
                        items(21, i) = evolveData.Rows(i)("FrameBottom").ToString()
                        items(22, i) = bottomTrack
                        items(23, i) = evolveData.Rows(i)("Buildout").ToString()
                        items(24, i) = evolveData.Rows(i)("SameSizePanel").ToString()
                        items(25, i) = gapPosition
                        items(26, i) = horizontalHeight
                        items(27, i) = evolveData.Rows(i)("HorizontalTPost").ToString()
                        items(28, i) = evolveData.Rows(i)("TiltrodType").ToString()
                        items(29, i) = evolveData.Rows(i)("TiltrodSplit").ToString()
                        items(30, i) = splitHeigth
                        items(31, i) = evolveData.Rows(i)("ReverseHinged").ToString()
                        items(32, i) = evolveData.Rows(i)("PelmetFlat").ToString()
                        items(33, i) = evolveData.Rows(i)("ExtraFascia").ToString()
                        items(34, i) = evolveData.Rows(i)("HingesLoose").ToString()
                        items(35, i) = FormatNumber(evolveData.Rows(i)("SquareMetre"), 2)
                        items(36, i) = evolveData.Rows(i)("Notes").ToString()
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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim windowData As DataTable = GetDataTableSP("sp_GetWindowData", params)

                If windowData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Aluminium"
                    pageEvent.PageTitle2 = "Window"
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(20, windowData.Rows.Count - 1) As String

                    For i As Integer = 0 To windowData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim swivelQty As String = windowData.Rows(i)("SwivelQty").ToString()
                        If swivelQty = "0" Then swivelQty = String.Empty

                        Dim swivelQtyB As String = windowData.Rows(i)("SwivelQtyB").ToString()
                        If swivelQtyB = "0" Then swivelQtyB = String.Empty

                        Dim springQty As String = windowData.Rows(i)("SpringQty").ToString()
                        If springQty = "0" Then springQty = String.Empty

                        Dim topPlascticQty As String = windowData.Rows(i)("TopPlasticQty").ToString()
                        If topPlascticQty = "0" Then topPlascticQty = String.Empty

                        items(0, i) = "Item : " & number
                        items(1, i) = windowData.Rows(i)("Room").ToString()
                        items(2, i) = windowData.Rows(i)("Mounting").ToString()
                        items(3, i) = windowData.Rows(i)("Width").ToString()
                        items(4, i) = windowData.Rows(i)("Drop").ToString()
                        items(5, i) = windowData.Rows(i)("BlindName").ToString()
                        items(6, i) = windowData.Rows(i)("FrameColour").ToString()
                        items(7, i) = windowData.Rows(i)("MeshType").ToString()
                        items(8, i) = windowData.Rows(i)("Brace").ToString()
                        items(9, i) = windowData.Rows(i)("AngleType").ToString()
                        items(10, i) = windowData.Rows(i)("AngleLength").ToString()
                        items(11, i) = windowData.Rows(i)("AngleQty").ToString()
                        items(12, i) = windowData.Rows(i)("PortHole").ToString()
                        items(13, i) = windowData.Rows(i)("PlungerPin").ToString()
                        items(14, i) = windowData.Rows(i)("SwivelColour").ToString()
                        items(15, i) = swivelQty
                        items(16, i) = swivelQtyB
                        items(17, i) = springQty
                        items(18, i) = topPlascticQty
                        items(19, i) = windowData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim doorData As DataTable = GetDataTableSP("sp_GetDoorData", params)

                If doorData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Aluminium"
                    pageEvent.PageTitle2 = "Door"
                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100

                    Dim items(33, doorData.Rows.Count - 1) As String

                    For i As Integer = 0 To doorData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim width As String = String.Format("{0} - {1} - {2}", doorData.Rows(i)("Width").ToString(), doorData.Rows(i)("WidthB").ToString(), doorData.Rows(i)("WidthC").ToString())

                        Dim handleLength As String = String.Empty
                        Dim handleLengthValue As Integer = doorData.Rows(i)("HandleLength")
                        If handleLengthValue > 0 Then
                            handleLength = doorData.Rows(i)("HandleLength").ToString()
                        End If

                        Dim topTrackLength As String = String.Empty
                        Dim topTrackValue As Integer = doorData.Rows(i)("TopTrackLength")
                        If topTrackValue > 0 Then
                            topTrackLength = doorData.Rows(i)("TopTrackLength").ToString()
                        End If

                        Dim bottomTrackLength As String = String.Empty
                        Dim bottomTrackValue As Integer = doorData.Rows(i)("BottomTrackLength")
                        If bottomTrackValue > 0 Then bottomTrackLength = doorData.Rows(i)("BottomTrackLength").ToString()

                        Dim receiverLength As String = String.Empty
                        Dim receiverValue As Integer = doorData.Rows(i)("ReceiverLength")
                        If receiverValue > 0 Then receiverLength = doorData.Rows(i)("ReceiverLength").ToString()

                        items(0, i) = "Item : " & number
                        items(1, i) = doorData.Rows(i)("Room").ToString()
                        items(2, i) = doorData.Rows(i)("Mounting").ToString()
                        items(3, i) = width
                        items(4, i) = doorData.Rows(i)("Drop").ToString()
                        items(5, i) = doorData.Rows(i)("BlindName").ToString()
                        items(6, i) = doorData.Rows(i)("TubeName").ToString()
                        items(7, i) = doorData.Rows(i)("FrameColour").ToString()
                        items(8, i) = doorData.Rows(i)("MeshType").ToString()
                        items(9, i) = doorData.Rows(i)("LayoutCode").ToString()
                        items(10, i) = doorData.Rows(i)("MidrailPosition").ToString()
                        items(11, i) = doorData.Rows(i)("HandleType").ToString()
                        items(12, i) = handleLength
                        items(13, i) = doorData.Rows(i)("TripleLock").ToString()
                        items(14, i) = doorData.Rows(i)("BugSeal").ToString()
                        items(15, i) = doorData.Rows(i)("PetType").ToString()
                        items(16, i) = doorData.Rows(i)("PetPosition").ToString()
                        items(17, i) = doorData.Rows(i)("DoorCloser").ToString()
                        items(18, i) = doorData.Rows(i)("AngleType").ToString()
                        items(19, i) = doorData.Rows(i)("AngleLength").ToString()
                        items(20, i) = doorData.Rows(i)("Beading").ToString()
                        items(21, i) = doorData.Rows(i)("JambType").ToString()
                        items(22, i) = doorData.Rows(i)("JambPosition").ToString()
                        items(23, i) = doorData.Rows(i)("FlushBold").ToString()
                        items(24, i) = doorData.Rows(i)("InterlockType").ToString()
                        items(25, i) = doorData.Rows(i)("TopTrack").ToString()
                        items(26, i) = topTrackLength
                        items(27, i) = doorData.Rows(i)("BottomTrack").ToString()
                        items(28, i) = bottomTrackLength
                        items(29, i) = doorData.Rows(i)("Receiver").ToString()
                        items(30, i) = receiverLength
                        items(31, i) = doorData.Rows(i)("SlidingQty").ToString()
                        items(32, i) = doorData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 4
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim sampleData As DataTable = GetDataTableSP("sp_GetSampleData", params)

                If sampleData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Sample"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(5, sampleData.Rows.Count - 1) As String

                    For i As Integer = 0 To sampleData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        items(0, i) = "Item : " & number
                        items(1, i) = sampleData.Rows(i)("BlindName").ToString()
                        items(2, i) = sampleData.Rows(i)("FabricName").ToString()
                        items(3, i) = sampleData.Rows(i)("FabricColour").ToString()
                        items(4, i) = sampleData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

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
                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@HeaderId", headerId)
                }
                Dim outdoorData As DataTable = GetDataTableSP("sp_GetOutdoorData", params)

                If outdoorData.Rows.Count > 0 Then
                    pageEvent.PageTitle = "Outdoor"
                    pageEvent.PageTitle2 = ""
                    Dim table As New PdfPTable(7)
                    table.WidthPercentage = 100

                    Dim items(12, outdoorData.Rows.Count - 1) As String

                    For i As Integer = 0 To outdoorData.Rows.Count - 1
                        Dim number As Integer = i + 1

                        Dim controlLength As String = outdoorData.Rows(i)("ControlLength").ToString()
                        Dim controlLengthValue As String = outdoorData.Rows(i)("ControlLengthValue").ToString()

                        Dim controlLengthText As String = controlLength
                        If controlLength = "Custom" Then
                            controlLengthText = String.Format("{0} : {1}mm", controlLength, controlLengthValue)
                        End If

                        items(0, i) = "Item : " & number
                        items(1, i) = outdoorData.Rows(i)("Room").ToString()
                        items(2, i) = outdoorData.Rows(i)("Mounting").ToString()
                        items(3, i) = outdoorData.Rows(i)("Width").ToString()
                        items(4, i) = outdoorData.Rows(i)("Drop").ToString()
                        items(5, i) = outdoorData.Rows(i)("BlindName").ToString()
                        items(6, i) = outdoorData.Rows(i)("FabricName").ToString()
                        items(7, i) = outdoorData.Rows(i)("FabricColour").ToString()
                        items(8, i) = outdoorData.Rows(i)("ControlName").ToString()
                        items(9, i) = outdoorData.Rows(i)("ControlPosition").ToString()
                        items(10, i) = controlLengthText
                        items(11, i) = outdoorData.Rows(i)("Notes").ToString()
                    Next

                    For i As Integer = 0 To items.GetLength(1) - 1 Step 6
                        If i > 0 Then doc.NewPage()

                        Dim fontHeader As New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)
                        Dim fontContent As New Font(Font.FontFamily.TIMES_ROMAN, 8)

                        Dim headers As String() = {"", "Location", "Mounting", "Width (mm)", "Drop (mm)", "Type", "Fabric Type", "Fabric Colour", "Control Type", "Control Position", "Control Length", "Notes"}

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

        Dim logoPath As String = HttpContext.Current.Server.MapPath("~/Assets/images/logo/general.jpg")
        If pageCompany = "2" Then
            logoPath = HttpContext.Current.Server.MapPath("~/Assets/images/logo/jpmdirect.jpg")
        End If
        If pageCompany = "3" Then
            logoPath = HttpContext.Current.Server.MapPath("~/Assets/images/logo/accent.png")
        End If
        If pageCompany = "4" Then
            logoPath = HttpContext.Current.Server.MapPath("~/Assets/images/logo/sunlight.jpg")
        End If
        If pageCompany = "5" Then
            logoPath = HttpContext.Current.Server.MapPath("~/Assets/images/logo/big.JPG")
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
